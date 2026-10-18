using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>XrmMockup base for Rule Studio: builds rules and calls the Custom APIs.</summary>
public abstract class RuleStudioTestBase : TestBase
{
    protected RuleStudioTestBase(XrmMockupFixture fixture)
        : base(fixture)
    {
        Rows = new RuleRows(AdminDao);
    }

    protected RuleRows Rows { get; }

    protected IOrganizationService Admin => Xrm.GetAdminService();

    protected static string Json(object value) => JsonConvert.SerializeObject(value);

    protected static OrganizationRequest Api(string name, params (string Key, object? Value)[] parameters)
    {
        var request = new OrganizationRequest(name);
        foreach (var (key, value) in parameters.Where(p => p.Value != null))
        {
            request[key] = value;
        }

        return request;
    }

    protected JObject Validate(params Guid[] ruleIds) =>
        JObject.Parse((string)Admin.Execute(Api("rs_ValidateRelease", ("RuleIds", Json(ruleIds))))["ValidationResult"]);

    protected Guid Publish(params Guid[] ruleIds) => Publish(Admin, ruleIds);

    protected static Guid Publish(IOrganizationService service, params Guid[] ruleIds)
    {
        ArgumentNullException.ThrowIfNull(service);
        var response = service.Execute(Api("rs_PublishRelease", ("RuleIds", Json(ruleIds)), ("Name", "TEST – release")));
        var id = (Guid)response["ReleaseId"];
        id.Should().NotBeEmpty(response["ValidationResult"]?.ToString());
        return id;
    }

    protected JArray Simulate(Guid ruleId, Guid? recordId = null, object? sample = null, bool mock = true) =>
        JArray.Parse((string)Admin.Execute(Api(
            "rs_SimulateRule",
            ("RuleIds", Json(new[] { ruleId })),
            ("RecordId", recordId),
            ("SampleValues", sample == null ? null : Json(sample)),
            ("CallFunctions", mock ? null : true)))["Trace"]);

    protected static IEnumerable<string> Codes(JObject validation)
    {
        ArgumentNullException.ThrowIfNull(validation);
        return validation["issues"]!.Select(i => i["code"]!.ToString());
    }

    protected Guid NewAccount(string name, Action<Account>? settings = null)
    {
        var account = new Account { Name = "TEST – " + name };
        settings?.Invoke(account);
        return AdminDao.Create(account);
    }

    protected Guid NewContact(string lastName, Guid? accountId, Action<Contact>? settings = null)
    {
        var contact = new Contact { LastName = "TEST – " + lastName, ParentCustomerId = accountId is { } a ? new EntityReference(Account.EntityLogicalName, a) : null };
        settings?.Invoke(contact);
        return AdminDao.Create(contact);
    }
}
