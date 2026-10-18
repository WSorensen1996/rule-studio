using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using Tests.RuleStudio;

namespace LiveTests;

/// <summary>
/// What XrmMockup cannot show, checked in a real environment: the deployed engine on real saves,
/// after-save updates, and FetchXML aggregates over real child records. Opt-in: see README.md.
/// </summary>
public sealed class LiveEngineTests : IClassFixture<LiveEnvironment>
{
    private readonly LiveEnvironment live;

    public LiveEngineTests(LiveEnvironment live)
    {
        this.live = live;
    }

    [LiveFact]
    public void BeforeSaveRuleSetsAFieldOnCreate()
    {
        var (rule, version) = live.Draft("BEFORE_CREATE", rs_event.Create, rs_stage.Beforesave);
        live.Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Startswith, Fixed(LiveEnvironment.Prefix + "before"));
        live.Rows.SetField(version, "description", "set before save");
        live.Publish(rule);

        LiveEnvironment.Eventually(() =>
        {
            var id = live.Create(new Entity("account") { ["name"] = LiveEnvironment.Prefix + "before create" });
            return live.Read("account", id, "description").GetAttributeValue<string>("description") == "set before save";
        }).Should().BeTrue();
    }

    [LiveFact]
    public void AfterSaveRuleUpdatesTheSavedRecordOnce()
    {
        var (rule, version) = live.Draft("AFTER_UPDATE", rs_event.Update, rs_stage.Aftersave, v => v.rs_FilteredColumns = "name");
        live.Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Startswith, Fixed(LiveEnvironment.Prefix + "after"));
        live.Rows.SetField(version, "description", "set after save");
        live.Publish(rule);
        var id = live.Create(new Entity("account") { ["name"] = LiveEnvironment.Prefix + "after x" });

        LiveEnvironment.Eventually(() =>
        {
            live.Service.Update(new Entity("account", id) { ["name"] = LiveEnvironment.Prefix + "after " + Guid.NewGuid().ToString("N")[..6] });
            return live.Read("account", id, "description").GetAttributeValue<string>("description") == "set after save";
        }).Should().BeTrue("the filter on name keeps the rule's own update from triggering it again");
    }

    [LiveFact]
    public void CountAndSumRunAsRealAggregates()
    {
        var (rule, version) = live.Draft("AGGREGATE", rs_event.Update, rs_stage.Beforesave, v => v.rs_FilteredColumns = "accountnumber");
        var contacts = RuleRows.Path("account", "numberofchildren", ("child", "parentcustomerid", "contact"));
        live.Rows.Condition(version, contacts, rs_condition_rs_operator.Greaterthan, Fixed(1), rs_condition_rs_aggregate.Count);
        live.Rows.Condition(version, contacts, rs_condition_rs_operator.Equals, Fixed(5), rs_condition_rs_aggregate.Sum);
        live.Rows.SetField(version, "description", "two contacts, five children");
        live.Publish(rule);
        var account = live.Create(new Entity("account") { ["name"] = LiveEnvironment.Prefix + "parent" });
        foreach (var children in new[] { 2, 3 })
        {
            // Email and phone too, so typical contact rules in the environment accept the record.
            live.Create(new Entity("contact")
            {
                ["lastname"] = LiveEnvironment.Prefix + "child",
                ["emailaddress1"] = "live.test@example.invalid",
                ["telephone1"] = "+4500000000",
                ["numberofchildren"] = children,
                ["parentcustomerid"] = new EntityReference("account", account),
            });
        }

        LiveEnvironment.Eventually(() =>
        {
            live.Service.Update(new Entity("account", account) { ["accountnumber"] = Guid.NewGuid().ToString("N")[..8] });
            return live.Read("account", account, "description").GetAttributeValue<string>("description") == "two contacts, five children";
        }).Should().BeTrue();
    }

    [LiveFact]
    public void TextThatLooksLikeANumberIsComparedAsText()
    {
        var (rule, version) = live.Draft("TEXT_AS_TEXT", rs_event.Create, rs_stage.Beforesave);
        live.Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Startswith, Fixed(LiveEnvironment.Prefix + "number"));
        live.Rows.Condition(version, RuleRows.Path("account", "accountnumber"), rs_condition_rs_operator.Equals, Fixed("7"));
        live.Rows.SetField(version, "description", "matched 7");
        live.Publish(rule);

        // Wait until the rule runs at all, then check that "007" does not match "7".
        LiveEnvironment.Eventually(() =>
        {
            var id = live.Create(new Entity("account") { ["name"] = LiveEnvironment.Prefix + "number 7", ["accountnumber"] = "7" });
            return live.Read("account", id, "description").GetAttributeValue<string>("description") == "matched 7";
        }).Should().BeTrue();
        var padded = live.Create(new Entity("account") { ["name"] = LiveEnvironment.Prefix + "number 007", ["accountnumber"] = "007" });
        live.Read("account", padded, "description").GetAttributeValue<string>("description").Should().BeNull();
    }

    private static Action<rs_Condition> Fixed(object value) => c =>
    {
        c.rs_ValueSourceType = rs_valuesourcetype.Fixed;
        c.rs_Value = JsonConvert.SerializeObject(value);
    };
}
