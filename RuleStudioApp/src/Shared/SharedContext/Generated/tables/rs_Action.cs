using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Ordered THEN/ELSE action of a rule version.</para>
/// <para>Display Name: Action</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_action")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_Action : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_action";
    public const int EntityTypeCode = 10609;

    public rs_Action() : base(EntityLogicalName) { }
    public rs_Action(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_actionid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_actionid", value);
        }
    }

    /// <summary>
    /// <para>Unique identifier of the user who created the record.</para>
    /// <para>Display Name: Created By</para>
    /// </summary>
    [AttributeLogicalName("createdby")]
    [DisplayName("Created By")]
    public EntityReference? CreatedBy
    {
        get => GetAttributeValue<EntityReference?>("createdby");
        set => SetAttributeValue("createdby", value);
    }

    /// <summary>
    /// <para>Date and time when the record was created.</para>
    /// <para>Display Name: Created On</para>
    /// </summary>
    [AttributeLogicalName("createdon")]
    [DisplayName("Created On")]
    public DateTime? CreatedOn
    {
        get => GetAttributeValue<DateTime?>("createdon");
        set => SetAttributeValue("createdon", value);
    }

    /// <summary>
    /// <para>Unique identifier of the delegate user who created the record.</para>
    /// <para>Display Name: Created By (Delegate)</para>
    /// </summary>
    [AttributeLogicalName("createdonbehalfby")]
    [DisplayName("Created By (Delegate)")]
    public EntityReference? CreatedOnBehalfBy
    {
        get => GetAttributeValue<EntityReference?>("createdonbehalfby");
        set => SetAttributeValue("createdonbehalfby", value);
    }

    /// <summary>
    /// <para>Sequence number of the import that created this record.</para>
    /// <para>Display Name: Import Sequence Number</para>
    /// </summary>
    [AttributeLogicalName("importsequencenumber")]
    [DisplayName("Import Sequence Number")]
    [Range(-2147483648, 2147483647)]
    public int? ImportSequenceNumber
    {
        get => GetAttributeValue<int?>("importsequencenumber");
        set => SetAttributeValue("importsequencenumber", value);
    }

    /// <summary>
    /// <para>Unique identifier of the user who modified the record.</para>
    /// <para>Display Name: Modified By</para>
    /// </summary>
    [AttributeLogicalName("modifiedby")]
    [DisplayName("Modified By")]
    public EntityReference? ModifiedBy
    {
        get => GetAttributeValue<EntityReference?>("modifiedby");
        set => SetAttributeValue("modifiedby", value);
    }

    /// <summary>
    /// <para>Date and time when the record was modified.</para>
    /// <para>Display Name: Modified On</para>
    /// </summary>
    [AttributeLogicalName("modifiedon")]
    [DisplayName("Modified On")]
    public DateTime? ModifiedOn
    {
        get => GetAttributeValue<DateTime?>("modifiedon");
        set => SetAttributeValue("modifiedon", value);
    }

    /// <summary>
    /// <para>Unique identifier of the delegate user who modified the record.</para>
    /// <para>Display Name: Modified By (Delegate)</para>
    /// </summary>
    [AttributeLogicalName("modifiedonbehalfby")]
    [DisplayName("Modified By (Delegate)")]
    public EntityReference? ModifiedOnBehalfBy
    {
        get => GetAttributeValue<EntityReference?>("modifiedonbehalfby");
        set => SetAttributeValue("modifiedonbehalfby", value);
    }

    /// <summary>
    /// <para>Unique identifier for the organization</para>
    /// <para>Display Name: Organization Id</para>
    /// </summary>
    [AttributeLogicalName("organizationid")]
    [DisplayName("Organization Id")]
    public EntityReference? OrganizationId
    {
        get => GetAttributeValue<EntityReference?>("organizationid");
        set => SetAttributeValue("organizationid", value);
    }

    /// <summary>
    /// <para>Date and time that the record was migrated.</para>
    /// <para>Display Name: Record Created On</para>
    /// </summary>
    [AttributeLogicalName("overriddencreatedon")]
    [DisplayName("Record Created On")]
    public DateTime? OverriddenCreatedOn
    {
        get => GetAttributeValue<DateTime?>("overriddencreatedon");
        set => SetAttributeValue("overriddencreatedon", value);
    }

    /// <summary>
    /// <para>Display Name: Action</para>
    /// </summary>
    [AttributeLogicalName("rs_actionid")]
    [DisplayName("Action")]
    public Guid? rs_ActionId
    {
        get => GetAttributeValue<Guid?>("rs_actionid");
        set => SetId("rs_actionid", value);
    }

    /// <summary>
    /// <para>Display Name: Action type</para>
    /// </summary>
    [AttributeLogicalName("rs_actiontype")]
    [DisplayName("Action type")]
    public rs_action_rs_actiontype? rs_ActionType
    {
        get => this.GetOptionSetValue<rs_action_rs_actiontype>("rs_actiontype");
        set => this.SetOptionSetValue("rs_actiontype", value);
    }

    /// <summary>
    /// <para>Display Name: Branch</para>
    /// </summary>
    [AttributeLogicalName("rs_branch")]
    [DisplayName("Branch")]
    public rs_action_rs_branch? rs_Branch
    {
        get => this.GetOptionSetValue<rs_action_rs_branch>("rs_branch");
        set => this.SetOptionSetValue("rs_branch", value);
    }

    /// <summary>
    /// <para>Display Name: Custom function</para>
    /// </summary>
    [AttributeLogicalName("rs_customfunctionid")]
    [DisplayName("Custom function")]
    public EntityReference? rs_CustomFunctionId
    {
        get => GetAttributeValue<EntityReference?>("rs_customfunctionid");
        set => SetAttributeValue("rs_customfunctionid", value);
    }

    /// <summary>
    /// <para>Display Name: Lookup definition</para>
    /// </summary>
    [AttributeLogicalName("rs_lookupdefinitionid")]
    [DisplayName("Lookup definition")]
    public EntityReference? rs_LookupDefinitionId
    {
        get => GetAttributeValue<EntityReference?>("rs_lookupdefinitionid");
        set => SetAttributeValue("rs_lookupdefinitionid", value);
    }

    /// <summary>
    /// <para>Display Name: Message</para>
    /// </summary>
    [AttributeLogicalName("rs_message")]
    [DisplayName("Message")]
    [MaxLength(2000)]
    public string? rs_Message
    {
        get => GetAttributeValue<string?>("rs_message");
        set => SetAttributeValue("rs_message", value);
    }

    /// <summary>
    /// <para>Display Name: Message level</para>
    /// </summary>
    [AttributeLogicalName("rs_messagelevel")]
    [DisplayName("Message level")]
    public rs_messagelevel? rs_MessageLevel
    {
        get => this.GetOptionSetValue<rs_messagelevel>("rs_messagelevel");
        set => this.SetOptionSetValue("rs_messagelevel", value);
    }

    /// <summary>
    /// <para>Primary name.</para>
    /// <para>Display Name: Name</para>
    /// </summary>
    [AttributeLogicalName("rs_name")]
    [DisplayName("Name")]
    [MaxLength(850)]
    public string? rs_Name
    {
        get => GetAttributeValue<string?>("rs_name");
        set => SetAttributeValue("rs_name", value);
    }

    /// <summary>
    /// <para>Display Name: Order</para>
    /// </summary>
    [AttributeLogicalName("rs_order")]
    [DisplayName("Order")]
    [Range(0, 2147483647)]
    public int? rs_Order
    {
        get => GetAttributeValue<int?>("rs_order");
        set => SetAttributeValue("rs_order", value);
    }

    /// <summary>
    /// <para>Display Name: Rule version</para>
    /// </summary>
    [AttributeLogicalName("rs_ruleversionid")]
    [DisplayName("Rule version")]
    public EntityReference? rs_RuleVersionId
    {
        get => GetAttributeValue<EntityReference?>("rs_ruleversionid");
        set => SetAttributeValue("rs_ruleversionid", value);
    }

    /// <summary>
    /// <para>Display Name: Target field</para>
    /// </summary>
    [AttributeLogicalName("rs_targetfield")]
    [DisplayName("Target field")]
    [MaxLength(100)]
    public string? rs_TargetField
    {
        get => GetAttributeValue<string?>("rs_targetfield");
        set => SetAttributeValue("rs_targetfield", value);
    }

    /// <summary>
    /// <para>Display Name: Value</para>
    /// </summary>
    [AttributeLogicalName("rs_value")]
    [DisplayName("Value")]
    [MaxLength(4000)]
    public string? rs_Value
    {
        get => GetAttributeValue<string?>("rs_value");
        set => SetAttributeValue("rs_value", value);
    }

    /// <summary>
    /// <para>Display Name: Value path</para>
    /// </summary>
    [AttributeLogicalName("rs_valuepath")]
    [DisplayName("Value path")]
    [MaxLength(4000)]
    public string? rs_ValuePath
    {
        get => GetAttributeValue<string?>("rs_valuepath");
        set => SetAttributeValue("rs_valuepath", value);
    }

    /// <summary>
    /// <para>Display Name: Value source</para>
    /// </summary>
    [AttributeLogicalName("rs_valuesourcetype")]
    [DisplayName("Value source")]
    public rs_valuesourcetype? rs_ValueSourceType
    {
        get => this.GetOptionSetValue<rs_valuesourcetype>("rs_valuesourcetype");
        set => this.SetOptionSetValue("rs_valuesourcetype", value);
    }

    /// <summary>
    /// <para>Status of the Action</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_action_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_action_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Action</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_action_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_action_statuscode>("statuscode");
        set => this.SetOptionSetValue("statuscode", value);
    }

    /// <summary>
    /// <para>For internal use only.</para>
    /// <para>Display Name: Time Zone Rule Version Number</para>
    /// </summary>
    [AttributeLogicalName("timezoneruleversionnumber")]
    [DisplayName("Time Zone Rule Version Number")]
    [Range(-1, 2147483647)]
    public int? TimeZoneRuleVersionNumber
    {
        get => GetAttributeValue<int?>("timezoneruleversionnumber");
        set => SetAttributeValue("timezoneruleversionnumber", value);
    }

    /// <summary>
    /// <para>Time zone code that was in use when the record was created.</para>
    /// <para>Display Name: UTC Conversion Time Zone Code</para>
    /// </summary>
    [AttributeLogicalName("utcconversiontimezonecode")]
    [DisplayName("UTC Conversion Time Zone Code")]
    [Range(-1, 2147483647)]
    public int? UTCConversionTimeZoneCode
    {
        get => GetAttributeValue<int?>("utcconversiontimezonecode");
        set => SetAttributeValue("utcconversiontimezonecode", value);
    }

    /// <summary>
    /// <para>Version Number</para>
    /// <para>Display Name: Version Number</para>
    /// </summary>
    [AttributeLogicalName("versionnumber")]
    [DisplayName("Version Number")]
    public long? VersionNumber
    {
        get => GetAttributeValue<long?>("versionnumber");
        set => SetAttributeValue("versionnumber", value);
    }

    [AttributeLogicalName("createdby")]
    [RelationshipSchemaName("lk_rs_action_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_action_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_action_createdby", null);
        set => SetRelatedEntity("lk_rs_action_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_action_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_action_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_action_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_action_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_action_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_action_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_action_modifiedby", null);
        set => SetRelatedEntity("lk_rs_action_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_action_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_action_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_action_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_action_modifiedonbehalfby", null, value);
    }

    [RelationshipSchemaName("rs_rs_action_rs_actioninputmapping_rs_actionid")]
    [RelationshipMetadata("OneToMany", "rs_actionid", "rs_actioninputmapping", "rs_actionid", "Referenced")]
    public IEnumerable<rs_ActionInputMapping> rs_rs_action_rs_actioninputmapping_rs_actionid
    {
        get => GetRelatedEntities<rs_ActionInputMapping>("rs_rs_action_rs_actioninputmapping_rs_actionid", null);
        set => SetRelatedEntities("rs_rs_action_rs_actioninputmapping_rs_actionid", null, value);
    }

    [AttributeLogicalName("rs_customfunctionid")]
    [RelationshipSchemaName("rs_rs_customfunction_rs_action_rs_customfunctionid")]
    [RelationshipMetadata("ManyToOne", "rs_customfunctionid", "rs_customfunction", "rs_customfunctionid", "Referencing")]
    public rs_CustomFunction rs_rs_customfunction_rs_action_rs_customfunctionid
    {
        get => GetRelatedEntity<rs_CustomFunction>("rs_rs_customfunction_rs_action_rs_customfunctionid", null);
        set => SetRelatedEntity("rs_rs_customfunction_rs_action_rs_customfunctionid", null, value);
    }

    [AttributeLogicalName("rs_lookupdefinitionid")]
    [RelationshipSchemaName("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid")]
    [RelationshipMetadata("ManyToOne", "rs_lookupdefinitionid", "rs_lookupdefinition", "rs_lookupdefinitionid", "Referencing")]
    public rs_LookupDefinition rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid
    {
        get => GetRelatedEntity<rs_LookupDefinition>("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid", null);
        set => SetRelatedEntity("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid", null, value);
    }

    [AttributeLogicalName("rs_ruleversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_action_rs_ruleversionid")]
    [RelationshipMetadata("ManyToOne", "rs_ruleversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_action_rs_ruleversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_action_rs_ruleversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_action_rs_ruleversionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_Action entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_Action, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_Action with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_Action to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_Action</returns>
    public static rs_Action Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_Action, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
