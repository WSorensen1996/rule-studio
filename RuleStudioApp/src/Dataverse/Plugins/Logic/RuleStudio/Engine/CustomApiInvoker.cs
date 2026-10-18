using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Calls custom functions as Dataverse Custom APIs through the given access object, so they run
/// with that service's identity. Response properties Success, Value and Message map onto the
/// uniform result contract; a function without a Success property succeeded when it did not throw.
/// </summary>
public sealed class CustomApiInvoker(IDataverseAccessObject dao) : IFunctionInvoker
{
    private readonly IDataverseAccessObject dao = dao;
    private readonly Dictionary<string, CompiledFunction?> found = new(StringComparer.OrdinalIgnoreCase);

    public FunctionResult Invoke(CompiledFunction customFunction, IDictionary<string, object?> inputs)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(customFunction, nameof(customFunction));
        ArgumentNullExceptionHelper.ThrowIfNull(inputs, nameof(inputs));
        var request = new OrganizationRequest(customFunction.ApiName);
        foreach (var input in inputs)
        {
            var parameter = customFunction.InputSchema?.Parameters.FirstOrDefault(p => string.Equals(p.Name, input.Key, StringComparison.Ordinal));
            request[input.Key] = Coerce.ToParameter(input.Value, parameter);
        }

        var response = dao.Execute(request);
        var results = response?.Results ?? new ParameterCollection();
        var success = Result(results, "Success");
        return new FunctionResult
        {
            Success = success is not bool flag || flag,
            Value = Values.Normalize(Result(results, "Value")),
            Message = Result(results, "Message") as string,
        };
    }

    public CompiledFunction? Find(string name)
    {
        if (found.TryGetValue(name, out var cached))
            return cached;

        var query = new QueryExpression(rs_CustomFunction.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var names = query.Criteria.AddFilter(LogicalOperator.Or);
        names.AddCondition("rs_customapiname", ConditionOperator.Equal, name);
        names.AddCondition("rs_name", ConditionOperator.Equal, name);
        var row = dao.RetrieveList<rs_CustomFunction>(query).Find(f => f.rs_Enabled != false);
        var function = row == null ? null : new CompiledFunction
        {
            Id = row.Id,
            Name = row.rs_Name ?? string.Empty,
            ApiName = row.rs_CustomApiName ?? string.Empty,
            HasSideEffects = row.rs_HasSideEffects ?? false,
            SafeToRetry = row.rs_SafeToRetry ?? false,
            TimeoutMs = row.rs_TimeoutMs,
            InputSchema = Schema(row.rs_InputSchema),
        };
        found[name] = function;
        return function;
    }

    private static object? Result(ParameterCollection results, string name)
    {
        var key = results.Keys.FirstOrDefault(k => string.Equals(k, name, StringComparison.OrdinalIgnoreCase));
        return key == null ? null : results[key];
    }

    private static InputSchema? Schema(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<InputSchema>(text!);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
