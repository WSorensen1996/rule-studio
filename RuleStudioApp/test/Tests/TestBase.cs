#pragma warning disable SA1210 // Using order varies with project name after template replacement
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;
#pragma warning restore SA1210

namespace Tests;

public class TestBase : IClassFixture<XrmMockupFixture>
{
    private readonly XrmMockup365 xrm;
    private readonly IDataverseAccessObject adminDao;
    private readonly DataProducer producer;
    private readonly IDataverseAccessObject userDao;
    private readonly Guid userIdOfUserDao;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObject AdminDao => adminDao;

    protected DataProducer Producer => producer;

    /// <summary>
    /// This is for testing stuff that depends on the user context
    /// </summary>
    protected IDataverseAccessObject UserDao => userDao;

    protected Guid UserIdOfUserDao => userIdOfUserDao;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = XrmMockup365.GetInstance(fixture.Settings);

        // Setting up a user DAO for testing stuff that depends on the user context, you probably want to change the role to your basic role or some other role more restricted than the System Administrator role
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<TestBase>();
        userIdOfUserDao = Guid.NewGuid();
        CreateUser(userIdOfUserDao, Xrm.RootBusinessUnit, SecurityRoles.SystemAdministrator);
        var userService = Xrm.CreateOrganizationService(userIdOfUserDao);
        userDao = new DataverseAccessObject(userService, logger);

        adminDao = new DataverseAccessObject(xrm.GetAdminService(), Substitute.For<ILogger>());
        producer = new DataProducer(AdminDao);
    }

    protected SystemUser CreateUser(Guid userId, EntityReference businessUnit, params Guid[] securityRoles)
    {
        return Xrm.CreateUser(Xrm.GetAdminService(), new SystemUser { Id = userId, BusinessUnitId = businessUnit }, securityRoles).ToEntity<SystemUser>();
    }
}
