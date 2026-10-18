using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Table and column metadata, fetched once per table per call. Uses RetrieveMetadataChanges, which
/// returns nothing for a missing table: a failing request would abort the plug-in transaction.
/// </summary>
internal sealed class TableMetadata(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;
    private readonly Dictionary<string, EntityMetadata?> tables = new(StringComparer.OrdinalIgnoreCase);

    // The table, or null when it does not exist.
    internal EntityMetadata? Table(string? logicalName)
    {
        if (string.IsNullOrWhiteSpace(logicalName))
            return null;

        var name = logicalName!.Trim().ToLowerInvariant();
        if (tables.TryGetValue(name, out var cached))
            return cached;

        var query = new EntityQueryExpression
        {
            Criteria = new MetadataFilterExpression(Microsoft.Xrm.Sdk.Query.LogicalOperator.And),
            Properties = new MetadataPropertiesExpression("LogicalName", "PrimaryIdAttribute", "PrimaryNameAttribute", "Attributes"),
            AttributeQuery = new AttributeQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "AttributeType", "AttributeOf", "Targets", "MaxLength", "MinValue", "MaxValue"),
            },
        };
        query.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, name));
        var response = dao.Execute(new RetrieveMetadataChangesRequest { Query = query }) as RetrieveMetadataChangesResponse;
        var metadata = response?.EntityMetadata?.FirstOrDefault();
        tables[name] = metadata;
        return metadata;
    }

    // The column, or null when the table or column does not exist.
    internal AttributeMetadata? Column(string? table, string? column)
    {
        if (string.IsNullOrWhiteSpace(column))
            return null;

        var attributes = Table(table)?.Attributes;
        return attributes == null ? null : Array.Find(attributes, a =>
            string.Equals(a.LogicalName, column!.Trim(), StringComparison.OrdinalIgnoreCase) && a.AttributeOf == null);
    }
}
