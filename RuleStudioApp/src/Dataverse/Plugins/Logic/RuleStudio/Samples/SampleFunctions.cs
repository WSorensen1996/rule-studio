using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Samples;

/// <summary>
/// Two sample custom functions, used by the Guide and the tests. They follow the
/// result contract the engine reads: Success, Value and Message.
/// </summary>
public class SampleFunctions(IUserDataverseAccessObjectService dao)
{
    private const int MaxContacts = 500;

    private readonly IUserDataverseAccessObjectService dao = dao;

    // Pure and safe to retry: 10 % of revenue from 100 employees, else 5 %. Negative input fails.
    public static (bool Success, decimal Value, string Message) CalculateDiscount(decimal? revenue, int? employees)
    {
        if (revenue < 0 || employees < 0)
            return (false, 0m, "Revenue and employees must not be negative.");

        var rate = (employees ?? 0) >= 100 ? 0.10m : 0.05m;
        return (true, decimal.Round((revenue ?? 0m) * rate, 2), $"{rate:P0} discount");
    }

    // Side effects, not safe to retry: writes text into description of the account's active contacts.
    public (bool Success, int Value, string Message) StampContacts(EntityReference? account, string? text)
    {
        if (account == null || account.Id == Guid.Empty)
            return (false, 0, "AccountId is required.");

        var query = new QueryExpression("contact") { ColumnSet = new ColumnSet(false), TopCount = MaxContacts };
        query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, account.Id);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var contacts = dao.RetrieveList<Entity>(query);
        foreach (var contact in contacts)
        {
            dao.Update(new Entity("contact", contact.Id) { ["description"] = text });
        }

        return (true, contacts.Count, $"{contacts.Count} contact(s) updated");
    }
}
