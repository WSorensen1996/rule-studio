using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Registry of developer-built Custom APIs usable in rules.</para>
/// <para>Display Name: Custom Function</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_customfunction")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_CustomFunction : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_customfunction";
    public const int EntityTypeCode = 10611;

    public rs_CustomFunction() : base(EntityLogicalName) { }
    public rs_CustomFunction(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_customfunctionid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_customfunctionid", value);
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
    /// <para>Display Name: Custom API name</para>
    /// </summary>
    [AttributeLogicalName("rs_customapiname")]
    [DisplayName("Custom API name")]
    [MaxLength(128)]
    public string? rs_CustomApiName
    {
        get => GetAttributeValue<string?>("rs_customapiname");
        set => SetAttributeValue("rs_customapiname", value);
    }

    /// <summary>
    /// <para>Display Name: Custom Function</para>
    /// </summary>
    [AttributeLogicalName("rs_customfunctionid")]
    [DisplayName("Custom Function")]
    public Guid? rs_CustomFunctionId
    {
        get => GetAttributeValue<Guid?>("rs_customfunctionid");
        set => SetId("rs_customfunctionid", value);
    }

    /// <summary>
    /// <para>Display Name: Description</para>
    /// </summary>
    [AttributeLogicalName("rs_description")]
    [DisplayName("Description")]
    [MaxLength(2000)]
    public string? rs_Description
    {
        get => GetAttributeValue<string?>("rs_description");
        set => SetAttributeValue("rs_description", value);
    }

    /// <summary>
    /// <para>Display Name: Enabled</para>
    /// </summary>
    [AttributeLogicalName("rs_enabled")]
    [DisplayName("Enabled")]
    public bool? rs_Enabled
    {
        get => GetAttributeValue<bool?>("rs_enabled");
        set => SetAttributeValue("rs_enabled", value);
    }

    /// <summary>
    /// <para>Display Name: Has side effects</para>
    /// </summary>
    [AttributeLogicalName("rs_hassideeffects")]
    [DisplayName("Has side effects")]
    public bool? rs_HasSideEffects
    {
        get => GetAttributeValue<bool?>("rs_hassideeffects");
        set => SetAttributeValue("rs_hassideeffects", value);
    }

    /// <summary>
    /// <para>Display Name: Input schema</para>
    /// </summary>
    [AttributeLogicalName("rs_inputschema")]
    [DisplayName("Input schema")]
    [MaxLength(10000)]
    public string? rs_InputSchema
    {
        get => GetAttributeValue<string?>("rs_inputschema");
        set => SetAttributeValue("rs_inputschema", value);
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
    /// <para>Display Name: Output type</para>
    /// </summary>
    [AttributeLogicalName("rs_outputtype")]
    [DisplayName("Output type")]
    public rs_customfunction_rs_outputtype? rs_OutputType
    {
        get => this.GetOptionSetValue<rs_customfunction_rs_outputtype>("rs_outputtype");
        set => this.SetOptionSetValue("rs_outputtype", value);
    }

    /// <summary>
    /// <para>Display Name: Safe to retry</para>
    /// </summary>
    [AttributeLogicalName("rs_safetoretry")]
    [DisplayName("Safe to retry")]
    public bool? rs_SafeToRetry
    {
        get => GetAttributeValue<bool?>("rs_safetoretry");
        set => SetAttributeValue("rs_safetoretry", value);
    }

    /// <summary>
    /// <para>Display Name: Timeout (ms)</para>
    /// </summary>
    [AttributeLogicalName("rs_timeoutms")]
    [DisplayName("Timeout (ms)")]
    [Range(0, 2147483647)]
    public int? rs_TimeoutMs
    {
        get => GetAttributeValue<int?>("rs_timeoutms");
        set => SetAttributeValue("rs_timeoutms", value);
    }

    /// <summary>
    /// <para>Status of the Custom Function</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_customfunction_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_customfunction_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Custom Function</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_customfunction_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_customfunction_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_customfunction_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_customfunction_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_customfunction_createdby", null);
        set => SetRelatedEntity("lk_rs_customfunction_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_customfunction_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_customfunction_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_customfunction_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_customfunction_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_customfunction_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_customfunction_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_customfunction_modifiedby", null);
        set => SetRelatedEntity("lk_rs_customfunction_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_customfunction_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_customfunction_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_customfunction_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_customfunction_modifiedonbehalfby", null, value);
    }

    [RelationshipSchemaName("rs_rs_customfunction_rs_action_rs_customfunctionid")]
    [RelationshipMetadata("OneToMany", "rs_customfunctionid", "rs_action", "rs_customfunctionid", "Referenced")]
    public IEnumerable<rs_Action> rs_rs_customfunction_rs_action_rs_customfunctionid
    {
        get => GetRelatedEntities<rs_Action>("rs_rs_customfunction_rs_action_rs_customfunctionid", null);
        set => SetRelatedEntities("rs_rs_customfunction_rs_action_rs_customfunctionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_customfunction_rs_condition_rs_customfunctionid")]
    [RelationshipMetadata("OneToMany", "rs_customfunctionid", "rs_condition", "rs_customfunctionid", "Referenced")]
    public IEnumerable<rs_Condition> rs_rs_customfunction_rs_condition_rs_customfunctionid
    {
        get => GetRelatedEntities<rs_Condition>("rs_rs_customfunction_rs_condition_rs_customfunctionid", null);
        set => SetRelatedEntities("rs_rs_customfunction_rs_condition_rs_customfunctionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_CustomFunction entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_CustomFunction, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_CustomFunction with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_CustomFunction to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_CustomFunction</returns>
    public static rs_CustomFunction Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_CustomFunction, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
