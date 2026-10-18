using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Related records read through parent hops, cached per record and column for the whole chain.</summary>
internal sealed class RecordCache(EngineContext context)
{
    private readonly EngineContext context = context;
    private readonly Dictionary<string, Entity?> records = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> read = new(StringComparer.OrdinalIgnoreCase);

    // A column of a related record, normalized; null when the record does not exist.
    internal object? Read(string table, Guid id, string column)
    {
        var key = $"{table}|{id}";
        if (read.Add($"{key}|{column}"))
        {
            var query = new QueryExpression(table) { ColumnSet = new ColumnSet(column), TopCount = 1, NoLock = true };
            query.Criteria.AddCondition(context.PrimaryId(table), ConditionOperator.Equal, id);
            var row = Query(context, query).FirstOrDefault();
            if (!records.TryGetValue(key, out var cached) || cached == null)
            {
                records[key] = row;
            }
            else if (row != null && row.Attributes.TryGetValue(column, out var fetched))
            {
                cached[column] = fetched;
            }
        }

        return records.TryGetValue(key, out var record) && record != null && record.Attributes.TryGetValue(column, out var value)
            ? Values.Normalize(value)
            : null;
    }

    // Runs a query or FetchXML with the evaluation's identity.
    internal static IList<Entity> Query(EngineContext context, QueryBase query)
    {
        var response = context.Dao.Execute(new RetrieveMultipleRequest { Query = query }) as RetrieveMultipleResponse;
        return response?.EntityCollection?.Entities ?? (IList<Entity>)[];
    }
}
