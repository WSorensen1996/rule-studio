using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Services and state of one chain evaluation, shared by its rules.</summary>
internal sealed class EngineContext
{
    internal EngineContext(IDataverseAccessObject dao, IFunctionInvoker invoker, EvaluationRequest request)
    {
        Dao = dao;
        Invoker = invoker;
        Request = request;
        Metadata = new TableMetadata(dao);

        // A copy, so the caller's entity only changes through EvaluationResult.Changes.
        Record = new Entity(request.Target.LogicalName, request.Target.Id);
        foreach (var attribute in request.Target.Attributes)
        {
            Record[attribute.Key] = attribute.Value;
        }

        Records = new RecordCache(this);
        Paths = new PathReader(this);
        Expressions = new ExpressionEvaluator(this);
        Lookups = new LookupResolver(this);
        Functions = new FunctionRunner(this);
        Sources = new ValueResolver(this);
        Conditions = new ConditionEvaluator(this);
        Actions = new ActionRunner(this);
    }

    internal IDataverseAccessObject Dao { get; }

    internal IFunctionInvoker Invoker { get; }

    internal EvaluationRequest Request { get; }

    internal TableMetadata Metadata { get; }

    // The triggering record with the changes of the rules that completed so far.
    internal Entity Record { get; }

    internal RecordCache Records { get; }

    internal PathReader Paths { get; }

    internal ExpressionEvaluator Expressions { get; }

    internal LookupResolver Lookups { get; }

    internal FunctionRunner Functions { get; }

    internal ValueResolver Sources { get; }

    internal ConditionEvaluator Conditions { get; }

    internal ActionRunner Actions { get; }

    internal bool BeforeSave => string.Equals(Request.Stage, "beforeSave", StringComparison.Ordinal);

    // The primary key column of a table; tables without metadata fall back to the <table>id convention.
    internal string PrimaryId(string table) => Metadata.Table(table)?.PrimaryIdAttribute ?? table + "id";
}
