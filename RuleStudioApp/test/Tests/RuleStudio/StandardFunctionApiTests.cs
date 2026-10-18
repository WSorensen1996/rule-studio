using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>The standard custom functions as Custom APIs in XrmMockup: the result contract and the records they change.</summary>
public class StandardFunctionApiTests : RuleStudioTestBase
{
    public StandardFunctionApiTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void ValidationApisReturnTheResultContract()
    {
        var response = Admin.Execute(Api("rs_ValidateVatNumber", ("VatNumber", "DK13585628")));
        response["Success"].Should().Be(true);
        response["Value"].Should().Be(true);
        response["Message"].Should().Be("Valid DK VAT number (format and check digits).");

        Admin.Execute(Api("rs_ValidateEmail", ("Email", "not-an-email")))["Value"].Should().Be(false);
        Admin.Execute(Api("rs_ValidatePhone", ("Phone", "12345678"), ("CountryCode", "DK")))["Message"].Should().Be("Valid: +4512345678");
    }

    [Fact]
    public void CreateTaskCreatesAnOpenTaskRegardingTheRecord()
    {
        var account = NewAccount("Task target");
        var response = Admin.Execute(Api(
            "rs_CreateTask",
            ("RegardingTable", "account"),
            ("RegardingId", account.ToString()),
            ("Subject", "Call the customer"),
            ("Description", "Created by a rule"),
            ("DueInDays", 3)));

        response["Success"].Should().Be(true);
        var reference = (EntityReference)response["Value"];
        var task = AdminDao.Retrieve<global::RuleStudio.SharedContext.Task>(reference.Id, t => t.Subject, t => t.Description, t => t.RegardingObjectId, t => t.ScheduledEnd);
        task.Subject.Should().Be("Call the customer");
        task.Description.Should().Be("Created by a rule");
        task.RegardingObjectId!.Id.Should().Be(account);
        task.ScheduledEnd!.Value.Date.Should().Be(DateTime.UtcNow.Date.AddDays(3));
    }

    [Theory]
    [InlineData(null, null, "Subject is required.")]
    [InlineData("Call", "account", "Give both RegardingTable and RegardingId, or neither.")]
    public void CreateTaskReportsBadInputAsAFailure(string? subject, string? table, string message)
    {
        var response = Admin.Execute(Api("rs_CreateTask", ("Subject", subject), ("RegardingTable", table)));
        response["Success"].Should().Be(false);
        response["Message"].Should().Be(message);
    }

    [Fact]
    public void AssignToMakesTheUserTheOwner()
    {
        var account = NewAccount("Assign target");
        var owner = new EntityReference(SystemUser.EntityLogicalName, UserIdOfUserDao);

        var response = Admin.Execute(Api("rs_AssignTo", ("RecordTable", "account"), ("RecordId", account.ToString()), ("OwnerId", owner)));

        response["Success"].Should().Be(true);
        AdminDao.Retrieve<Account>(account, a => a.OwnerId).OwnerId!.Id.Should().Be(UserIdOfUserDao);
    }

    [Fact]
    public void AssignToRefusesAnOwnerThatIsNotAUserOrTeam()
    {
        var account = NewAccount("Assign target");
        var response = Admin.Execute(Api("rs_AssignTo", ("RecordTable", "account"), ("RecordId", account.ToString()), ("OwnerId", new EntityReference("account", account))));
        response["Success"].Should().Be(false);
        response["Message"].Should().Be("The owner must be a user or a team, not a account record.");
    }
}
