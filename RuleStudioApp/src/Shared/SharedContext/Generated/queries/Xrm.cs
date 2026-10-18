using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
public class Xrm : OrganizationServiceContext
{
    public Xrm(IOrganizationService service)
        : base(service)
    {
    }

    public IQueryable<Account> AccountSet
    {
        get { return CreateQuery<Account>(); }
    }

    public IQueryable<ActivityParty> ActivityPartySet
    {
        get { return CreateQuery<ActivityParty>(); }
    }

    public IQueryable<Annotation> AnnotationSet
    {
        get { return CreateQuery<Annotation>(); }
    }

    public IQueryable<appnotification> appnotificationSet
    {
        get { return CreateQuery<appnotification>(); }
    }

    public IQueryable<Contact> ContactSet
    {
        get { return CreateQuery<Contact>(); }
    }

    public IQueryable<DuplicateRule> DuplicateRuleSet
    {
        get { return CreateQuery<DuplicateRule>(); }
    }

    public IQueryable<EnvironmentVariableDefinition> EnvironmentVariableDefinitionSet
    {
        get { return CreateQuery<EnvironmentVariableDefinition>(); }
    }

    public IQueryable<EnvironmentVariableValue> EnvironmentVariableValueSet
    {
        get { return CreateQuery<EnvironmentVariableValue>(); }
    }

    public IQueryable<Queue> QueueSet
    {
        get { return CreateQuery<Queue>(); }
    }

    public IQueryable<rs_Action> rs_ActionSet
    {
        get { return CreateQuery<rs_Action>(); }
    }

    public IQueryable<rs_ActionInputMapping> rs_ActionInputMappingSet
    {
        get { return CreateQuery<rs_ActionInputMapping>(); }
    }

    public IQueryable<rs_Condition> rs_ConditionSet
    {
        get { return CreateQuery<rs_Condition>(); }
    }

    public IQueryable<rs_ConditionGroup> rs_ConditionGroupSet
    {
        get { return CreateQuery<rs_ConditionGroup>(); }
    }

    public IQueryable<rs_CustomFunction> rs_CustomFunctionSet
    {
        get { return CreateQuery<rs_CustomFunction>(); }
    }

    public IQueryable<rs_ExecutionLog> rs_ExecutionLogSet
    {
        get { return CreateQuery<rs_ExecutionLog>(); }
    }

    public IQueryable<rs_LookupDefinition> rs_LookupDefinitionSet
    {
        get { return CreateQuery<rs_LookupDefinition>(); }
    }

    public IQueryable<rs_Release> rs_ReleaseSet
    {
        get { return CreateQuery<rs_Release>(); }
    }

    public IQueryable<rs_Rule> rs_RuleSet
    {
        get { return CreateQuery<rs_Rule>(); }
    }

    public IQueryable<rs_RuleVersion> rs_RuleVersionSet
    {
        get { return CreateQuery<rs_RuleVersion>(); }
    }

    public IQueryable<SavedQuery> SavedQuerySet
    {
        get { return CreateQuery<SavedQuery>(); }
    }

    public IQueryable<SystemUser> SystemUserSet
    {
        get { return CreateQuery<SystemUser>(); }
    }

    public IQueryable<Task> TaskSet
    {
        get { return CreateQuery<Task>(); }
    }

    public IQueryable<Template> TemplateSet
    {
        get { return CreateQuery<Template>(); }
    }
}
