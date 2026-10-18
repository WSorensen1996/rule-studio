using Microsoft.Xrm.Sdk;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>Set field converts the value to each column type, and refuses values the column cannot hold.</summary>
public class SetFieldWriteTests
{
    private static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Theory]
    [InlineData(2.5, 3)]
    [InlineData(3.5, 4)]
    [InlineData(-2.5, -3)]
    [InlineData(2.4, 2)]
    public void WholeNumbersRoundHalfAwayFromZero(double value, int expected) =>
        Written("wholenumber", (decimal)value).Should().Be(expected);

    [Fact]
    public void NumbersOutsideTheColumnRangeFailReadably()
    {
        Written("numberofemployees", 2_000_000).Should().Be("failed (RULE_FAILED): 2000000 is outside the range of numberofemployees (0 to 1000000).");
        Written("numberofemployees", -1).Should().BeOfType<string>().Which.Should().Contain("outside the range");
        Written("decimalnumber", 150).Should().BeOfType<string>().Which.Should().Contain("outside the range of decimalnumber");
        Written("revenue", -5).Should().BeOfType<string>().Which.Should().Contain("outside the range of revenue");
        Written("wholenumber", 1_000_000_000_000m).Should().BeOfType<string>().Which.Should().Contain("outside the range of wholenumber");
    }

    [Fact]
    public void TextThatIsNotANumberFailsReadably() =>
        Written("numberofemployees", "many").Should().Be("failed (RULE_FAILED): many is not a number, so it cannot be written to numberofemployees.");

    [Fact]
    public void TextLongerThanTheColumnFails()
    {
        Written("name", new string('a', 101)).Should().Be("failed (RULE_FAILED): The text is 101 characters; name allows 100.");
        Written("name", new string('a', 100)).Should().Be(new string('a', 100));
    }

    [Fact]
    public void EachColumnTypeGetsItsSdkType()
    {
        Written("revenue", 10.5).Should().BeEquivalentTo(new Money(10.5m));
        Written("decimalnumber", "12.25").Should().Be(12.25m);
        Written("floatnumber", 1.5).Should().Be(1.5d);
        Written("bignumber", 1_099_511_627_776).Should().Be(1_099_511_627_776L);
        Written("creditonhold", "true").Should().Be(true);
        Written("creditonhold", 0).Should().Be(false);
        Written("industrycode", 3).Should().BeEquivalentTo(new OptionSetValue(3));
        Written("description", 42).Should().Be("42");
        Written("opendate", "2026-01-01").Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void MultiSelectWritesAnOptionCollection()
    {
        Written("tags", new object[] { 1, 2 }).Should().BeOfType<OptionSetValueCollection>().Which.Select(o => o.Value).Should().Equal(1, 2);
        Written("tags", 3).Should().BeOfType<OptionSetValueCollection>().Which.Select(o => o.Value).Should().Equal(3);
    }

    [Fact]
    public void EmptyValueClearsTheColumn()
    {
        Written("revenue", string.Empty).Should().BeNull();
        Written("parentaccountid", null).Should().BeNull();
    }

    [Fact]
    public void SingleTableLookupAcceptsABareId() =>
        Written("parentaccountid", Id.ToString()).Should().BeEquivalentTo(new EntityReference("account", Id));

    [Fact]
    public void PolymorphicLookupKeepsTheTableOfAReference() =>
        Written("customerid", new RecordReference { Table = "contact", Id = Id }).Should().BeEquivalentTo(new EntityReference("contact", Id));

    [Fact]
    public void PolymorphicLookupRefusesABareId() =>
        Written("customerid", Id.ToString()).Should().Be("failed (RULE_FAILED): customerid can point to account or contact; give the table, not only the id.");

    [Fact]
    public void LookupRefusesATableItCannotPointTo() =>
        Written("ownerid", new RecordReference { Table = "account", Id = Id }).Should().Be("failed (RULE_FAILED): ownerid cannot point to account records; it allows systemuser or team.");
}
