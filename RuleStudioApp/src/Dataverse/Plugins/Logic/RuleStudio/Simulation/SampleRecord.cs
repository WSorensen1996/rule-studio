using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Simulation;

/// <summary>
/// The record a test run evaluates: a real record, sample values, or sample values over a real
/// record. Sample values on an Update are the columns being saved; the real record is the pre-image.
/// </summary>
internal sealed class SampleRecord
{
    private SampleRecord(Entity target, Entity? preImage, ISet<string>? changed)
    {
        Target = target;
        PreImage = preImage;
        Changed = changed;
    }

    internal Entity Target { get; }

    internal Entity? PreImage { get; }

    // Null when no sample values were given: then no rule is skipped for unchanged filtered columns.
    internal ISet<string>? Changed { get; }

    internal static SampleRecord Build(IDataverseAccessObject dao, SimulatedChain chain, Guid? recordId, string? sampleValues)
    {
        var metadata = new TableMetadata(dao);
        var table = chain.Table;
        if (metadata.Table(table) == null)
            throw new InvalidPluginExecutionException($"Table {table} does not exist.");

        var record = recordId is { } id && id != Guid.Empty ? Load(dao, metadata, table, id) : null;
        var target = new Entity(table, record?.Id ?? Guid.Empty);
        foreach (var attribute in record?.Attributes ?? [])
        {
            target[attribute.Key] = attribute.Value;
        }

        var samples = Samples(sampleValues);
        foreach (var sample in samples)
        {
            var column = metadata.Column(table, sample.Name)
                ?? throw new InvalidPluginExecutionException($"Sample value {sample.Name}: {table} has no column {sample.Name}.");
            target[column.LogicalName] = Coerce.ToColumn(Values.Normalize(sample.Value), column);
        }

        var update = string.Equals(chain.Event, "update", StringComparison.Ordinal);
        ISet<string>? changed = update && samples.Count > 0
            ? new HashSet<string>(samples.Select(s => s.Name.ToLowerInvariant()), StringComparer.OrdinalIgnoreCase)
            : null;
        var preImage = string.Equals(chain.Event, "create", StringComparison.Ordinal) ? null : record;
        return new SampleRecord(target, preImage, changed);
    }

    private static Entity Load(IDataverseAccessObject dao, TableMetadata metadata, string table, Guid id)
    {
        var query = new QueryExpression(table) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition(metadata.Table(table)?.PrimaryIdAttribute ?? table + "id", ConditionOperator.Equal, id);
        return dao.RetrieveList<Entity>(query).FirstOrDefault()
            ?? throw new InvalidPluginExecutionException($"The {table} record {id} does not exist or you cannot read it.");
    }

    private static List<JProperty> Samples(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        try
        {
            return JToken.Parse(text!) is JObject values
                ? values.Properties().ToList()
                : throw new InvalidPluginExecutionException("SampleValues must be a JSON object of column → value.");
        }
        catch (JsonReaderException)
        {
            throw new InvalidPluginExecutionException("SampleValues must be a JSON object of column → value.");
        }
    }
}
