using Microsoft.Xrm.Sdk;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>
/// Every condition operator on every column type: a match, a non-match and an empty value
/// (docs/json-formats.md, condition operators). One theory per column type.
/// </summary>
public class ConditionMatrixTests
{
    private const string Matched = "matched";
    private const string NotMatched = "notMatched";

    private static readonly Guid AccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly DateTime Morning = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    public static TheoryData<object?, string, object?, string> Text => new()
    {
        { "Contoso", "equals", "contoso", Matched },
        { "007", "equals", "7", NotMatched },
        { "1e3", "equals", "1000", NotMatched },
        { "a", "notEquals", "b", Matched },
        { null, "notEquals", "x", Matched },
        { "b", "greaterThan", "a", Matched },
        { "10", "greaterThan", "9", NotMatched },
        { "abc", "lessThan", "ABD", Matched },
        { "b", "greaterOrEqual", "B", Matched },
        { "b", "lessOrEqual", "a", NotMatched },
        { "Contoso Ltd", "contains", "TOSO", Matched },
        { null, "contains", "x", NotMatched },
        { "Contoso", "startsWith", "con", Matched },
        { "Contoso", "startsWith", "toso", NotMatched },
        { "ABC", "inList", new object[] { "abc", "x" }, Matched },
        { "q", "inList", new object[] { "abc", "x" }, NotMatched },
        { "m", "between", new object[] { "a", "z" }, Matched },
        { "m", "between", new object[] { "n", "z" }, NotMatched },
        { string.Empty, "isEmpty", null, Matched },
        { null, "isEmpty", null, Matched },
        { "x", "isEmpty", null, NotMatched },
        { "x", "isNotEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> WholeNumber => new()
    {
        { 100, "equals", 100, Matched },
        { 100, "equals", "100", Matched },
        { 100, "equals", 99, NotMatched },
        { 100, "notEquals", 99, Matched },
        { null, "notEquals", 5, Matched },
        { 100, "greaterThan", 99, Matched },
        { 99, "greaterThan", 100, NotMatched },
        { 100, "greaterOrEqual", 100, Matched },
        { 5, "lessThan", 6, Matched },
        { null, "lessThan", 6, NotMatched },
        { 7, "lessOrEqual", 6, NotMatched },
        { 50, "inList", new object[] { 10, 50 }, Matched },
        { 51, "inList", new object[] { 10, 50 }, NotMatched },
        { 50, "between", new object[] { 10, 50 }, Matched },
        { 50, "between", new object[] { 10, 49 }, NotMatched },
        { 50, "between", new object?[] { 10, null }, Matched },
        { 50, "between", new object?[] { null, 49 }, NotMatched },
        { null, "between", new object[] { 10, 50 }, NotMatched },
        { null, "isEmpty", null, Matched },
        { 0, "isEmpty", null, NotMatched },
        { 0, "isNotEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> Money => new()
    {
        { new Money(10.50m), "equals", 10.5m, Matched },
        { new Money(10.50m), "equals", "10.5", Matched },
        { new Money(10.50m), "greaterOrEqual", 10.51m, NotMatched },
        { new Money(10.50m), "between", new object[] { 10, 11 }, Matched },
        { null, "isEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> Boolean => new()
    {
        { true, "equals", true, Matched },
        { true, "equals", 1, Matched },
        { false, "equals", 0, Matched },
        { false, "equals", true, NotMatched },
        { true, "notEquals", false, Matched },
        { true, "inList", new object[] { true }, Matched },
        { null, "isEmpty", null, Matched },
        { false, "isNotEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> Choice => new()
    {
        { new OptionSetValue(1), "equals", 1, Matched },
        { new OptionSetValue(1), "equals", 2, NotMatched },
        { new OptionSetValue(1), "notEquals", 2, Matched },
        { new OptionSetValue(2), "inList", new object[] { 1, 2 }, Matched },
        { new OptionSetValue(3), "inList", new object[] { 1, 2 }, NotMatched },
        { null, "isEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> MultiSelect => new()
    {
        { Options(1, 2), "equals", new object[] { 2, 1 }, Matched },
        { Options(1, 2), "equals", new object[] { 1 }, NotMatched },
        { Options(1), "equals", 1, Matched },
        { Options(1, 2), "notEquals", new object[] { 1 }, Matched },
        { Options(1, 2), "contains", 1, Matched },
        { Options(1, 2), "contains", new object[] { 2, 1 }, Matched },
        { Options(1, 2), "contains", new object[] { 1, 3 }, NotMatched },
        { Options(126250010), "contains", 12625001, NotMatched },
        { Options(1, 2), "inList", new object[] { 3, 2 }, Matched },
        { Options(1, 2), "inList", new object[] { 3 }, NotMatched },
        { Options(), "isEmpty", null, Matched },
        { null, "isEmpty", null, Matched },
        { Options(1), "isNotEmpty", null, Matched },
        { Options(1), "startsWith", "1", NotMatched },
    };

    public static TheoryData<object?, string, object?, string> Date => new()
    {
        { Morning, "equals", "2026-01-01", Matched },
        { Morning, "equals", "2026-01-01T10:00:00Z", Matched },
        { Morning, "equals", "2026-01-01T09:00:00Z", NotMatched },
        { Morning, "notEquals", "2026-01-02", Matched },
        { Morning, "greaterThan", "2025-12-31", Matched },
        { Morning, "greaterThan", "2026-01-01", NotMatched },
        { Morning, "greaterThan", "2026-01-01T09:00:00Z", Matched },
        { Morning, "lessOrEqual", "2026-01-01", Matched },
        { Morning, "lessThan", "2026-01-01", NotMatched },
        { Morning, "between", new object[] { "2026-01-01", "2026-01-01" }, Matched },
        { Morning, "between", new object[] { "2026-01-02", "2026-12-31" }, NotMatched },
        { new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), "equals", "2026-01-01", Matched },
        { null, "isEmpty", null, Matched },
    };

    public static TheoryData<object?, string, object?, string> Lookup => new()
    {
        { new EntityReference("account", AccountId), "equals", AccountId.ToString(), Matched },
        { new EntityReference("account", AccountId), "equals", new RecordReference { Table = "account", Id = AccountId }, Matched },
        { new EntityReference("account", AccountId), "equals", OtherId.ToString(), NotMatched },
        { new EntityReference("account", AccountId), "notEquals", OtherId.ToString(), Matched },
        { new EntityReference("account", AccountId), "inList", new object[] { OtherId.ToString(), AccountId.ToString() }, Matched },
        { null, "isEmpty", null, Matched },
        { new EntityReference("account", AccountId), "isNotEmpty", null, Matched },
    };

    [Theory]
    [MemberData(nameof(Text))]
    public void TextOperators(object? actual, string op, object? expected, string outcome) =>
        Outcome("name", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(WholeNumber))]
    public void WholeNumberOperators(object? actual, string op, object? expected, string outcome) =>
        Outcome("numberofemployees", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(Money))]
    public void MoneyOperators(object? actual, string op, object? expected, string outcome) =>
        Outcome("revenue", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(Boolean))]
    public void BooleanOperators(object? actual, string op, object? expected, string outcome) =>
        Outcome("creditonhold", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(Choice))]
    public void ChoiceOperators(object? actual, string op, object? expected, string outcome) =>
        Outcome("industrycode", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(MultiSelect))]
    public void MultiSelectChoiceIsASet(object? actual, string op, object? expected, string outcome) =>
        Outcome("tags", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(Date))]
    public void DateWithoutTimeComparesCalendarDays(object? actual, string op, object? expected, string outcome) =>
        Outcome("opendate", actual, op, expected).Should().Be(outcome);

    [Theory]
    [MemberData(nameof(Lookup))]
    public void LookupComparesIds(object? actual, string op, object? expected, string outcome) =>
        Outcome("parentaccountid", actual, op, expected).Should().Be(outcome);

    [Fact]
    public void TextIsComparedAsANumberOnlyAgainstANumber() =>
        Outcome("accountnumber", "007", "equals", 7).Should().Be(Matched);

    [Fact]
    public void UnknownOperatorFails() =>
        Outcome("name", "abc", "endsWith", "c").Should().Be("failed (OPERATOR_NOT_SUPPORTED)");

    private static OptionSetValueCollection Options(params int[] values) => new(values.Select(v => new OptionSetValue(v)).ToList());
}
