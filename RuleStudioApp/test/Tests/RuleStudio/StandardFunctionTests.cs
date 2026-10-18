using DataverseLogic.RuleStudio.Functions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedContext.Dao;

namespace Tests.RuleStudio;

/// <summary>The standard custom functions' logic: e-mail, phone and VAT checks, and the webhook call with a fake sender.</summary>
public class StandardFunctionTests
{
    [Theory]
    [InlineData("anna@example.com")]
    [InlineData("anna.berg+sales@mail.example.co.uk")]
    [InlineData("o'neil@example.dk")]
    public void ValidEmailAddresses(string email) =>
        ValidationFunctions.ValidateEmail(email).Should().Be((true, true, "Valid e-mail address."));

    [Theory]
    [InlineData("anna")]
    [InlineData("anna@")]
    [InlineData("@example.com")]
    [InlineData("anna@@example.com")]
    [InlineData("anna@example")]
    [InlineData("anna..berg@example.com")]
    [InlineData("anna berg@example.com")]
    [InlineData("anna@-example.com")]
    [InlineData("anna@example.c")]
    public void InvalidEmailAddresses(string email)
    {
        var (success, valid, message) = ValidationFunctions.ValidateEmail(email);
        success.Should().BeTrue();
        valid.Should().BeFalse();
        message.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void EmptyValuesAreValid(string? value)
    {
        ValidationFunctions.ValidateEmail(value).Value.Should().BeTrue();
        ValidationFunctions.ValidatePhone(value, "DK").Value.Should().BeTrue();
        ValidationFunctions.ValidateVatNumber(value, null).Value.Should().BeTrue();
    }

    [Theory]
    [InlineData("+45 12 34 56 78", null, "+4512345678")]
    [InlineData("0045 12345678", "DK", "+4512345678")]
    [InlineData("12 34 56 78", "DK", "+4512345678")]
    [InlineData("020 7946 0958", "GB", "+442079460958")]
    [InlineData("(212) 555-0123", "US", "+12125550123")]
    [InlineData("+46 70 123 45 67", "SE", "+46701234567")]
    [InlineData("+81 3 1234 5678", null, "+81312345678")]
    public void ValidPhoneNumbersAreGivenInInternationalForm(string phone, string? country, string international) =>
        ValidationFunctions.ValidatePhone(phone, country).Should().Be((true, true, "Valid: " + international));

    [Theory]
    [InlineData("1234567", "DK")]
    [InlineData("+45 1234 5678 9", "DK")]
    [InlineData("+46 70 123 45 67", "DK")]
    [InlineData("12345678", null)]
    [InlineData("12345678", "ZZ")]
    [InlineData("+45 12 34 ab", null)]
    [InlineData("+0123456789", null)]
    public void InvalidPhoneNumbers(string phone, string? country) =>
        ValidationFunctions.ValidatePhone(phone, country).Value.Should().BeFalse();

    // Real, published numbers: each country's check digits are verified.
    [Theory]
    [InlineData("ATU13585627")]
    [InlineData("BE0403170701")]
    [InlineData("CHE-116.281.710 MWST")]
    [InlineData("DE136695976")]
    [InlineData("DK 13 58 56 28")]
    [InlineData("FI01120389")]
    [InlineData("FR40303265045")]
    [InlineData("IT00743110157")]
    [InlineData("NL004495445B01")]
    [InlineData("NO923609016MVA")]
    [InlineData("PL5260250274")]
    [InlineData("SE556036079301")]
    public void ValidVatNumbersWithCheckDigits(string vat) =>
        ValidationFunctions.ValidateVatNumber(vat, null).Message.Should().EndWith("(format and check digits).");

    [Theory]
    [InlineData("ATU13585628")]
    [InlineData("DE136695977")]
    [InlineData("DK13585629")]
    [InlineData("FI01120388")]
    [InlineData("NO923609017")]
    [InlineData("SE556036079401")]
    [InlineData("DK1358562")]
    [InlineData("XX123456789")]
    public void InvalidVatNumbers(string vat) =>
        ValidationFunctions.ValidateVatNumber(vat, null).Value.Should().BeFalse();

    [Fact]
    public void VatCountryCanBeGivenSeparatelyAndMustMatchThePrefix()
    {
        ValidationFunctions.ValidateVatNumber("13585628", "DK").Value.Should().BeTrue();
        ValidationFunctions.ValidateVatNumber("094259216", "GR").Value.Should().BeTrue();
        ValidationFunctions.ValidateVatNumber("DK13585628", "SE").Message.Should().Be("The number starts with DK, but the country is SE.");
        ValidationFunctions.ValidateVatNumber("13585628", null).Value.Should().BeFalse();
    }

    [Fact]
    public void FormatOnlyCountriesSaySoInTheMessage() =>
        ValidationFunctions.ValidateVatNumber("ESB12345678", null).Should().Be((true, true, "Valid ES VAT number format (this country's check digits are not verified)."));

    [Fact]
    public void WebhookPostsJsonAndReportsTheStatus()
    {
        var sender = Substitute.For<IWebhookSender>();
        sender.Post(Arg.Any<Uri>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>()).Returns((202, "queued"));
        var webhook = new WebhookFunction(Substitute.For<IUserDataverseAccessObjectService>(), sender);

        webhook.CallWebhook("https://hooks.example.com/rule", "{\"id\":1}", null).Should().Be((true, 202, "HTTP 202: queued"));
        sender.Received(1).Post(new Uri("https://hooks.example.com/rule"), "{\"id\":1}", "application/json", null);
    }

    [Theory]
    [InlineData(404, false)]
    [InlineData(500, false)]
    [InlineData(204, true)]
    public void WebhookSucceedsOnlyOn2xx(int status, bool success)
    {
        var sender = Substitute.For<IWebhookSender>();
        sender.Post(Arg.Any<Uri>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>()).Returns((status, string.Empty));
        var result = new WebhookFunction(Substitute.For<IUserDataverseAccessObjectService>(), sender).CallWebhook("https://hooks.example.com", "text", null);
        result.Should().Be((success, status, $"HTTP {status}"));
    }

    [Theory]
    [InlineData("http://hooks.example.com")]
    [InlineData("ftp://hooks.example.com")]
    [InlineData("")]
    public void WebhookNeedsAnHttpsUrl(string address)
    {
        var sender = Substitute.For<IWebhookSender>();
        new WebhookFunction(Substitute.For<IUserDataverseAccessObjectService>(), sender).CallWebhook(address, "x", null).Success.Should().BeFalse();
        sender.DidNotReceiveWithAnyArgs().Post(default!, default!, default!, default);
    }

    [Fact]
    public void WebhookReadsTheUrlAndAuthorizationFromEnvironmentVariables()
    {
        var dao = Substitute.For<IUserDataverseAccessObjectService>();
        dao.RetrieveList<Entity>(Arg.Any<QueryExpression>(), Arg.Any<string>()).Returns(call =>
        {
            var name = (string)call.Arg<QueryExpression>().Criteria.Conditions[0].Values[0];
            var row = new Entity("environmentvariabledefinition") { ["defaultvalue"] = "default" };
            row["v.value"] = new AliasedValue("environmentvariablevalue", "value", name == "rs_HookUrl" ? "https://hooks.example.com/env" : "Bearer abc");
            return [row];
        });
        var sender = Substitute.For<IWebhookSender>();
        sender.Post(Arg.Any<Uri>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>()).Returns((200, "ok"));

        new WebhookFunction(dao, sender).CallWebhook("rs_HookUrl", "plain", "rs_HookToken").Success.Should().BeTrue();
        sender.Received(1).Post(new Uri("https://hooks.example.com/env"), "plain", "text/plain", "Bearer abc");
    }
}
