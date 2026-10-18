using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>rs_ValidateRelease: one draft per validation code, checked in XrmMockup.</summary>
public class ValidationTests : RuleStudioTestBase
{
    private static readonly int[] OneBound = [1];
    private static readonly int[] Reversed = [10, 1];

    public ValidationTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void ValidDraftPasses()
    {
        var (rule, version) = Draft("TEST.OK");
        Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Contains, Fixed("Co"));
        Rows.SetField(version, "description", "ok");
        Validate(rule)["outcome"]!.ToString().Should().Be("passed");
    }

    [Fact]
    public void UnknownTable() =>
        Codes(Validate(Draft("TEST.TABLE", "nosuchtable").Rule)).Should().Contain(ValidationCodes.TableNotFound);

    [Fact]
    public void MissingField()
    {
        var (rule, version) = Draft("TEST.FIELD");
        Rows.Condition(version, RuleRows.Path("account", "nosuchcolumn"), rs_condition_rs_operator.Equals, Fixed(1));
        Codes(Validate(rule)).Should().Contain(ValidationCodes.FieldNotFound);
    }

    [Fact]
    public void WrongOperatorForTheType()
    {
        var (rule, version) = Draft("TEST.TYPE");
        Rows.Condition(version, RuleRows.Path("account", "numberofemployees"), rs_condition_rs_operator.Contains, Fixed("x"));
        Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Between, Fixed(OneBound));
        Codes(Validate(rule)).Should().Contain(ValidationCodes.TypeMismatch);
    }

    [Fact]
    public void ChangedOnlyOnUpdate()
    {
        var (rule, version) = Draft("TEST.CHANGED", @event: rs_event.Create);
        Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Changed);
        Codes(Validate(rule)).Should().Contain(ValidationCodes.OperatorNotAllowed);
    }

    [Fact]
    public void ChildPathNeedsAnAggregate()
    {
        var (rule, version) = Draft("TEST.CHILD");
        Rows.Condition(version, RuleRows.Path("account", "jobtitle", ("child", "parentcustomerid", "contact")), rs_condition_rs_operator.Equals, Fixed("CEO"));
        Codes(Validate(rule)).Should().Contain(ValidationCodes.ChildPathWithoutAggregate);
    }

    [Fact]
    public void HopLimit()
    {
        var (rule, version) = Draft("TEST.HOPS", "contact");
        var hops = Enumerable.Range(0, 6)
            .Select(i => i % 2 == 0 ? ("parent", "parentcustomerid", "account") : ("parent", "primarycontactid", "contact"))
            .ToArray();
        Rows.Condition(version, RuleRows.Path("contact", "name", hops), rs_condition_rs_operator.Isnotempty);
        Codes(Validate(rule)).Should().Contain(ValidationCodes.HopLimitExceeded);
    }

    [Fact]
    public void BlockSaveOnlyBeforeSave()
    {
        var (rule, version) = Draft("TEST.BLOCK", stage: rs_stage.Aftersave);
        Rows.Action(version, rs_action_rs_actiontype.Blocksave, a => a.rs_Message = "No");
        Codes(Validate(rule)).Should().Contain(ValidationCodes.BlockSaveAfterSave);
    }

    [Fact]
    public void ExpressionsAreParsed()
    {
        var (rule, version) = Draft("TEST.EXPR");
        Rows.Action(version, rs_action_rs_actiontype.Setfield, a => (a.rs_TargetField, a.rs_ValueSourceType, a.rs_Value) = ("revenue", rs_valuesourcetype.Expression, "[revenue] *"));
        Rows.Action(version, rs_action_rs_actiontype.Setfield, a => (a.rs_TargetField, a.rs_ValueSourceType, a.rs_Value) = ("revenue", rs_valuesourcetype.Expression, "Nope([revenue])"));
        var codes = Codes(Validate(rule)).ToList();
        codes.Should().Contain(ValidationCodes.ExpressionInvalid);
        codes.Should().Contain(ValidationCodes.FunctionNotRegistered);
    }

    [Fact]
    public void FunctionInputsAndSideEffects()
    {
        var stamp = Rows.Stamp();
        var discount = Rows.Discount();
        var (rule, version) = Draft("TEST.INPUTS");
        var call = Rows.Action(version, rs_action_rs_actiontype.Callfunction, a => (a.rs_ValueSourceType, a.rs_CustomFunctionId) = (rs_valuesourcetype.Function, new EntityReference(rs_CustomFunction.EntityLogicalName, stamp)));
        Rows.Input(call, ownerIsAction: true, "NotAParameter", rs_valuesourcetype.Fixed, "1");
        Rows.Action(version, rs_action_rs_actiontype.Callfunction, a => (a.rs_ValueSourceType, a.rs_CustomFunctionId) = (rs_valuesourcetype.Function, new EntityReference(rs_CustomFunction.EntityLogicalName, discount)));
        AdminDao.Update(new rs_CustomFunction(discount) { rs_Enabled = false });

        var codes = Codes(Validate(rule)).ToList();
        codes.Should().Contain(ValidationCodes.SideEffectFunction);
        codes.Should().Contain(ValidationCodes.RequiredInputMissing);
        codes.Should().Contain(ValidationCodes.InputMappingInvalid);
        codes.Should().Contain(ValidationCodes.FunctionNotRegistered);
    }

    [Fact]
    public void LookupForAnotherTable()
    {
        var lookup = AdminDao.Create(new rs_LookupDefinition
        {
            rs_Name = "TEST – contact lookup",
            rs_LookupTable = "account",
            rs_ReturnColumn = "creditlimit",
            rs_MatchCriteria = "{\"version\":1,\"recordTable\":\"contact\",\"criteria\":[]}",
        });
        var (rule, version) = Draft("TEST.LOOKUP");
        Rows.Action(version, rs_action_rs_actiontype.Setfield, a => (a.rs_TargetField, a.rs_ValueSourceType, a.rs_LookupDefinitionId) = ("creditlimit", rs_valuesourcetype.Lookup, new EntityReference(rs_LookupDefinition.EntityLogicalName, lookup)));
        Codes(Validate(rule)).Should().Contain(ValidationCodes.LookupInvalid);
    }

    [Fact]
    public void ChainWarningsAcrossRules()
    {
        var (first, a) = Draft("TEST.R8A");
        Rows.SetField(a, "description", "a");
        var (second, b) = Draft("TEST.R8B");
        Rows.SetField(b, "description", "b");
        var (after, c) = Draft("TEST.AFTER", stage: rs_stage.Aftersave);
        Rows.SetField(c, "description", "after");

        var result = Validate(first, second, after);
        result["outcome"]!.ToString().Should().Be("passedWithWarnings");
        var codes = Codes(result).ToList();
        codes.Should().Contain(ValidationCodes.FieldSetConflict);
        codes.Should().Contain(ValidationCodes.RetriggerChain);
    }

    [Fact]
    public void ConflictWithStopProcessingSaysWhenTheLaterRuleApplies()
    {
        var (first, a) = Draft("TEST.STOP_A");
        AdminDao.Update(new rs_RuleVersion(a) { rs_Priority = 10, rs_StopProcessing = true });
        Rows.SetField(a, "description", "a");
        var (second, b) = Draft("TEST.STOP_B");
        AdminDao.Update(new rs_RuleVersion(b) { rs_Priority = 20 });
        Rows.SetField(b, "description", "b");

        Validate(first, second)["issues"]!.Select(i => i["message"]!.ToString())
            .Should().Contain(m => m.Contains("only overrides", StringComparison.Ordinal) && m.Contains("stops processing", StringComparison.Ordinal));
    }

    [Fact]
    public void RuleWithoutDraft() =>
        Codes(Validate(Rows.NewRule("TEST.NODRAFT"))).Should().Contain(ValidationCodes.NoDraft);

    [Fact]
    public void PublishWithoutEngineWarns()
    {
        var (rule, version) = Draft("TEST.NOENGINE");
        Rows.SetField(version, "description", "x");
        var response = Admin.Execute(Api("rs_PublishRelease", ("RuleIds", Json(new[] { rule })), ("Name", "TEST – release")));
        response["ValidationResult"].ToString().Should().Contain(ValidationCodes.EngineNotRegistered);
    }

    [Fact]
    public void BetweenWithReversedBoundsIsRejected()
    {
        var (rule, version) = Draft("TEST.BETWEEN_REVERSED");
        Rows.Condition(version, RuleRows.Path("account", "numberofemployees"), rs_condition_rs_operator.Between, Fixed(Reversed));
        Validate(rule)["issues"]!.Select(i => i["message"]!.ToString()).Should().Contain(m => m.Contains("the first value is larger than the second", StringComparison.Ordinal));
    }

    [Fact]
    public void BetweenWithOneOpenBoundIsAccepted()
    {
        var (rule, version) = Draft("TEST.BETWEEN_OPEN");
        Rows.Condition(version, RuleRows.Path("account", "numberofemployees"), rs_condition_rs_operator.Between, Fixed(new int?[] { 10, null }));
        Rows.SetField(version, "description", "x");
        Validate(rule)["outcome"]!.ToString().Should().Be("passed");
    }

    [Theory]
    [InlineData(rs_condition_rs_operator.Contains)]
    [InlineData(rs_condition_rs_operator.Startswith)]
    public void TextSearchForNothingIsRejected(rs_condition_rs_operator op)
    {
        var (rule, version) = Draft("TEST.EMPTY_SEARCH_" + (int)op);
        Rows.Condition(version, RuleRows.Path("account", "name"), op, Fixed(string.Empty));
        Validate(rule)["issues"]!.Select(i => i["message"]!.ToString()).Should().Contain(m => m.Contains("with nothing to look for", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("yes", false)]
    [InlineData("ja", false)]
    [InlineData("true", true)]
    [InlineData(1, true)]
    [InlineData(true, true)]
    public void YesNoColumnAcceptsOnlyYesNoValues(object value, bool passes)
    {
        ArgumentNullException.ThrowIfNull(value);
        var (rule, version) = Draft("TEST.YESNO_" + value.ToString()!.ToUpperInvariant());
        Rows.Condition(version, RuleRows.Path("account", "creditonhold"), rs_condition_rs_operator.Equals, Fixed(value));
        Rows.SetField(version, "description", "x");
        Validate(rule)["outcome"]!.ToString().Should().Be(passes ? "passed" : "failed");
    }

    [Fact]
    public void ExpressionBuiltInsIgnoreCaseAtPublish()
    {
        var (rule, version) = Draft("TEST.ROUND_CASE");
        Rows.Action(version, rs_action_rs_actiontype.Setfield, a => (a.rs_TargetField, a.rs_ValueSourceType, a.rs_Value) = ("numberofemployees", rs_valuesourcetype.Expression, "round(2.5, 0)"));
        Validate(rule)["outcome"]!.ToString().Should().Be("passed");
    }

    private static Action<rs_Condition> Fixed(object value) => c =>
    {
        c.rs_ValueSourceType = rs_valuesourcetype.Fixed;
        c.rs_Value = Json(value);
    };

    private (Guid Rule, Guid Version) Draft(string key, string table = "account", rs_event @event = rs_event.Update, rs_stage stage = rs_stage.Beforesave)
    {
        var rule = Rows.NewRule(key);
        return (rule, Rows.Draft(rule, table, @event, stage));
    }
}
