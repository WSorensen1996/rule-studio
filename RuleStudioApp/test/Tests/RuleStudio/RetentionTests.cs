using DataverseLogic.RuleStudio.Operations;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace Tests.RuleStudio;

/// <summary>
/// rs_ConfigureLogRetention: the two recurring bulk-delete jobs, their queries, defaults and limits,
/// and the cancelling of earlier jobs (docs/custom-apis.md). The queries are also run against
/// XrmMockup to show which log rows each job deletes.
/// </summary>
public class RetentionTests : RuleStudioTestBase
{
    public RetentionTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, null, 30, 90)]
    [InlineData(0, 0, 30, 90)]
    [InlineData(7, 365, 7, 365)]
    [InlineData(1, 3650, 1, 3650)]
    public void DefaultsAndBoundsAreAccepted(int? days, int? errorDays, int expectedDays, int expectedErrorDays)
    {
        var (service, requests) = Service();

        var summary = service.Configure(days, errorDays);

        requests.Should().HaveCount(2);
        requests.Select(r => r.JobName).Should().Equal(
            $"Rule Studio log retention: runs older than {expectedDays} days",
            $"Rule Studio log retention: errors older than {expectedErrorDays} days");
        requests.Should().OnlyContain(r => r.RecurrencePattern == "FREQ=DAILY;INTERVAL=1;" && !r.SendEmailNotification);
        summary.Should().Be($"Daily jobs scheduled: logs kept {expectedDays} days, failed and blocked runs {expectedErrorDays} days. 0 earlier job(s) cancelled.");
    }

    [Theory]
    [InlineData(-1, 90, "Days")]
    [InlineData(3651, 90, "Days")]
    [InlineData(30, -5, "ErrorDays")]
    [InlineData(30, 4000, "ErrorDays")]
    public void OutOfRangeValuesAreRejectedBeforeAnythingChanges(int days, int errorDays, string name)
    {
        var (service, requests) = Service();

        var call = () => service.Configure(days, errorDays);

        call.Should().Throw<InvalidPluginExecutionException>().WithMessage($"{name} must be between 1 and 3650.");
        requests.Should().BeEmpty();
    }

    [Fact]
    public void EarlierJobsAreCancelledFirst()
    {
        var (service, _, dao) = ServiceWithDao([new Entity("asyncoperation", Guid.NewGuid()), new Entity("asyncoperation", Guid.NewGuid())]);

        service.Configure(null, null).Should().EndWith("2 earlier job(s) cancelled.");

        dao.Received(2).Update(Arg.Is<Entity>(e => e.LogicalName == "asyncoperation" && ((OptionSetValue)e["statecode"]).Value == 3), Arg.Any<string>());
    }

    [Fact]
    public void JobsDeleteOldRowsAndKeepErrorsLonger()
    {
        var (service, requests) = Service();
        service.Configure(30, 90);
        var normal = requests[0].QuerySet[0];
        var errors = requests[1].QuerySet[0];

        var old = Log("old matched", rs_executionlog_rs_outcome.Matched, 40);
        var oldNoOutcome = Log("old without outcome", null, 40);
        var fresh = Log("new matched", rs_executionlog_rs_outcome.Matched, 5);
        var oldFailed = Log("old failed", rs_executionlog_rs_outcome.Failed, 40);
        var ancientBlocked = Log("ancient blocked", rs_executionlog_rs_outcome.Blocked, 100);

        Matching(normal).Should().Contain([old, oldNoOutcome]).And.NotContain([fresh, oldFailed, ancientBlocked]);
        Matching(errors).Should().Contain(ancientBlocked).And.NotContain([oldFailed, old, fresh]);
    }

    private static (RetentionService Service, List<BulkDeleteRequest> Requests) Service()
    {
        var (service, requests, _) = ServiceWithDao([]);
        return (service, requests);
    }

    private static (RetentionService Service, List<BulkDeleteRequest> Requests, IUserDataverseAccessObjectService Dao) ServiceWithDao(List<Entity> existingJobs)
    {
        var requests = new List<BulkDeleteRequest>();
        var dao = Substitute.For<IUserDataverseAccessObjectService>();
        dao.RetrieveList<Entity>(Arg.Any<QueryExpression>(), Arg.Any<string>()).Returns(existingJobs);
        var capture = Arg.Do<OrganizationRequest>(r =>
        {
            if (r is BulkDeleteRequest bulk)
            {
                requests.Add(bulk);
            }
        });
        dao.Execute(capture, Arg.Any<string>()).Returns(new BulkDeleteResponse());
        return (new RetentionService(dao), requests, dao);
    }

    private Guid Log(string name, rs_executionlog_rs_outcome? outcome, int daysAgo) =>
        AdminDao.Create(new rs_ExecutionLog { rs_Name = "TEST – " + name, rs_Outcome = outcome, OverriddenCreatedOn = DateTime.UtcNow.AddDays(-daysAgo) });

    // XrmMockup cannot evaluate OlderThanXDays with a whole number (Dataverse's format), so that one
    // condition becomes the equivalent "created before now − days"; the outcome filters stay as sent.
    private List<Guid> Matching(QueryExpression query)
    {
        var older = query.Criteria.Conditions.Single(c => c.Operator == ConditionOperator.OlderThanXDays);
        query.Criteria.Conditions.Remove(older);
        query.Criteria.AddCondition(older.AttributeName, ConditionOperator.LessThan, DateTime.UtcNow.AddDays(-(int)older.Values[0]));
        query.Criteria.AddCondition("rs_name", ConditionOperator.BeginsWith, "TEST – ");
        return Admin.RetrieveMultiple(query).Entities.Select(e => e.Id).ToList();
    }
}
