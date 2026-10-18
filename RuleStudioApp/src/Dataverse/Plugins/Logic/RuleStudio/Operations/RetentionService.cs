using System.Globalization;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Operations;

/// <summary>
/// rs_ConfigureLogRetention (docs/custom-apis.md). Replaces Rule Studio's recurring
/// bulk-delete jobs for rs_executionlog: one deletes rows older than Days, except failed and
/// blocked runs, which another deletes after ErrorDays. Runs as the calling user, who needs the
/// Bulk Delete privilege (System Administrator).
/// </summary>
public class RetentionService(IUserDataverseAccessObjectService dao)
{
    public const int DefaultDays = 30;
    public const int DefaultErrorDays = 90;

    // Every job's name starts with this, so the next call can find and cancel it.
    internal const string JobPrefix = "Rule Studio log retention";

    private const int MaxDays = 3650;

    private readonly IUserDataverseAccessObjectService dao = dao;

    // Returns a one-line summary of the scheduled jobs. Absent or 0 means the default.
    public string Configure(int? days, int? errorDays)
    {
        var normal = Check(days is null or 0 ? DefaultDays : days.Value, "Days");
        var errors = Check(errorDays is null or 0 ? DefaultErrorDays : errorDays.Value, "ErrorDays");
        var cancelled = CancelExisting();
        Submit($"{JobPrefix}: runs older than {normal} days", normal, errorsOnly: false);
        Submit($"{JobPrefix}: errors older than {errors} days", errors, errorsOnly: true);
        return string.Format(CultureInfo.InvariantCulture, "Daily jobs scheduled: logs kept {0} days, failed and blocked runs {1} days. {2} earlier job(s) cancelled.", normal, errors, cancelled);
    }

    // The query of one job: rs_executionlog rows older than days, either the errors or everything else.
    internal static QueryExpression Query(int days, bool errorsOnly)
    {
        var query = new QueryExpression(rs_ExecutionLog.EntityLogicalName) { ColumnSet = new ColumnSet("rs_executionlogid") };
        query.Criteria.AddCondition("createdon", ConditionOperator.OlderThanXDays, days);
        object[] errorOutcomes = [(int)rs_executionlog_rs_outcome.Failed, (int)rs_executionlog_rs_outcome.Blocked];
        if (errorsOnly)
        {
            query.Criteria.AddCondition("rs_outcome", ConditionOperator.In, errorOutcomes);
        }
        else
        {
            var notError = query.Criteria.AddFilter(LogicalOperator.Or);
            notError.AddCondition("rs_outcome", ConditionOperator.Null);
            notError.AddCondition("rs_outcome", ConditionOperator.NotIn, errorOutcomes);
        }

        return query;
    }

    private static int Check(int value, string name) =>
        value is >= 1 and <= MaxDays ? value : throw new InvalidPluginExecutionException($"{name} must be between 1 and {MaxDays}.");

    // Cancels the waiting recurring jobs of earlier calls; returns how many.
    private int CancelExisting()
    {
        var query = new QueryExpression("asyncoperation") { ColumnSet = new ColumnSet("asyncoperationid"), TopCount = 50 };
        query.Criteria.AddCondition("name", ConditionOperator.BeginsWith, JobPrefix);
        query.Criteria.AddCondition("statecode", ConditionOperator.NotEqual, 3);
        var jobs = dao.RetrieveList<Entity>(query);
        foreach (var job in jobs)
        {
            dao.Update(new Entity("asyncoperation", job.Id) { ["statecode"] = new OptionSetValue(3), ["statuscode"] = new OptionSetValue(32) });
        }

        return jobs.Count;
    }

    private void Submit(string name, int days, bool errorsOnly) =>
        dao.Execute(new BulkDeleteRequest
        {
            JobName = name,
            QuerySet = [Query(days, errorsOnly)],
            StartDateTime = DateTime.UtcNow,
            RecurrencePattern = "FREQ=DAILY;INTERVAL=1;",
            SendEmailNotification = false,
            ToRecipients = [],
            CCRecipients = [],
        });
}
