using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Per-rule execution outcome, duration and trace.</para>
/// <para>Display Name: Execution Log</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_executionlog")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_ExecutionLog : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_executionlog";
    public const int EntityTypeCode = 10614;

    public rs_ExecutionLog() : base(EntityLogicalName) { }
    public rs_ExecutionLog(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_executionlogid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_executionlogid", value);
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
    /// <para>Display Name: Admin flag</para>
    /// </summary>
    [AttributeLogicalName("rs_adminflag")]
    [DisplayName("Admin flag")]
    public bool? rs_AdminFlag
    {
        get => GetAttributeValue<bool?>("rs_adminflag");
        set => SetAttributeValue("rs_adminflag", value);
    }

    /// <summary>
    /// <para>Display Name: Attempts</para>
    /// </summary>
    [AttributeLogicalName("rs_attempts")]
    [DisplayName("Attempts")]
    [Range(0, 2147483647)]
    public int? rs_Attempts
    {
        get => GetAttributeValue<int?>("rs_attempts");
        set => SetAttributeValue("rs_attempts", value);
    }

    /// <summary>
    /// <para>Display Name: Correlation ID</para>
    /// </summary>
    [AttributeLogicalName("rs_correlationid")]
    [DisplayName("Correlation ID")]
    [MaxLength(100)]
    public string? rs_CorrelationId
    {
        get => GetAttributeValue<string?>("rs_correlationid");
        set => SetAttributeValue("rs_correlationid", value);
    }

    /// <summary>
    /// <para>Display Name: Duration (ms)</para>
    /// </summary>
    [AttributeLogicalName("rs_durationms")]
    [DisplayName("Duration (ms)")]
    [Range(0, 2147483647)]
    public int? rs_DurationMs
    {
        get => GetAttributeValue<int?>("rs_durationms");
        set => SetAttributeValue("rs_durationms", value);
    }

    /// <summary>
    /// <para>Display Name: Error message</para>
    /// </summary>
    [AttributeLogicalName("rs_errormessage")]
    [DisplayName("Error message")]
    [MaxLength(4000)]
    public string? rs_ErrorMessage
    {
        get => GetAttributeValue<string?>("rs_errormessage");
        set => SetAttributeValue("rs_errormessage", value);
    }

    /// <summary>
    /// <para>Display Name: Event</para>
    /// </summary>
    [AttributeLogicalName("rs_event")]
    [DisplayName("Event")]
    public rs_event? rs_Event
    {
        get => this.GetOptionSetValue<rs_event>("rs_event");
        set => this.SetOptionSetValue("rs_event", value);
    }

    /// <summary>
    /// <para>Display Name: Execution Log</para>
    /// </summary>
    [AttributeLogicalName("rs_executionlogid")]
    [DisplayName("Execution Log")]
    public Guid? rs_ExecutionLogId
    {
        get => GetAttributeValue<Guid?>("rs_executionlogid");
        set => SetId("rs_executionlogid", value);
    }

    /// <summary>
    /// <para>Display Name: Is test</para>
    /// </summary>
    [AttributeLogicalName("rs_istest")]
    [DisplayName("Is test")]
    public bool? rs_IsTest
    {
        get => GetAttributeValue<bool?>("rs_istest");
        set => SetAttributeValue("rs_istest", value);
    }

    /// <summary>
    /// <para>Display Name: Log level</para>
    /// </summary>
    [AttributeLogicalName("rs_loglevel")]
    [DisplayName("Log level")]
    public rs_loglevel? rs_LogLevel
    {
        get => this.GetOptionSetValue<rs_loglevel>("rs_loglevel");
        set => this.SetOptionSetValue("rs_loglevel", value);
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
    /// <para>Display Name: Outcome</para>
    /// </summary>
    [AttributeLogicalName("rs_outcome")]
    [DisplayName("Outcome")]
    public rs_executionlog_rs_outcome? rs_Outcome
    {
        get => this.GetOptionSetValue<rs_executionlog_rs_outcome>("rs_outcome");
        set => this.SetOptionSetValue("rs_outcome", value);
    }

    /// <summary>
    /// <para>Display Name: Record ID</para>
    /// </summary>
    [AttributeLogicalName("rs_recordid")]
    [DisplayName("Record ID")]
    [MaxLength(100)]
    public string? rs_RecordId
    {
        get => GetAttributeValue<string?>("rs_recordid");
        set => SetAttributeValue("rs_recordid", value);
    }

    /// <summary>
    /// <para>Display Name: Rule</para>
    /// </summary>
    [AttributeLogicalName("rs_ruleid")]
    [DisplayName("Rule")]
    public EntityReference? rs_RuleId
    {
        get => GetAttributeValue<EntityReference?>("rs_ruleid");
        set => SetAttributeValue("rs_ruleid", value);
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
    /// <para>Display Name: Stage</para>
    /// </summary>
    [AttributeLogicalName("rs_stage")]
    [DisplayName("Stage")]
    public rs_stage? rs_Stage
    {
        get => this.GetOptionSetValue<rs_stage>("rs_stage");
        set => this.SetOptionSetValue("rs_stage", value);
    }

    /// <summary>
    /// <para>Display Name: Target table</para>
    /// </summary>
    [AttributeLogicalName("rs_targettable")]
    [DisplayName("Target table")]
    [MaxLength(100)]
    public string? rs_TargetTable
    {
        get => GetAttributeValue<string?>("rs_targettable");
        set => SetAttributeValue("rs_targettable", value);
    }

    /// <summary>
    /// <para>Display Name: Trace</para>
    /// </summary>
    [AttributeLogicalName("rs_trace")]
    [DisplayName("Trace")]
    [MaxLength(1048576)]
    public string? rs_Trace
    {
        get => GetAttributeValue<string?>("rs_trace");
        set => SetAttributeValue("rs_trace", value);
    }

    /// <summary>
    /// <para>Status of the Execution Log</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_executionlog_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_executionlog_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Execution Log</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_executionlog_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_executionlog_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_executionlog_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_executionlog_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_executionlog_createdby", null);
        set => SetRelatedEntity("lk_rs_executionlog_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_executionlog_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_executionlog_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_executionlog_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_executionlog_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_executionlog_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_executionlog_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_executionlog_modifiedby", null);
        set => SetRelatedEntity("lk_rs_executionlog_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_executionlog_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_executionlog_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_executionlog_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_executionlog_modifiedonbehalfby", null, value);
    }

    [AttributeLogicalName("rs_ruleid")]
    [RelationshipSchemaName("rs_rs_rule_rs_executionlog_rs_ruleid")]
    [RelationshipMetadata("ManyToOne", "rs_ruleid", "rs_rule", "rs_ruleid", "Referencing")]
    public rs_Rule rs_rs_rule_rs_executionlog_rs_ruleid
    {
        get => GetRelatedEntity<rs_Rule>("rs_rs_rule_rs_executionlog_rs_ruleid", null);
        set => SetRelatedEntity("rs_rs_rule_rs_executionlog_rs_ruleid", null, value);
    }

    [AttributeLogicalName("rs_ruleversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid")]
    [RelationshipMetadata("ManyToOne", "rs_ruleversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_executionlog_rs_ruleversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_ExecutionLog entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_ExecutionLog, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_ExecutionLog with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_ExecutionLog to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_ExecutionLog</returns>
    public static rs_ExecutionLog Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_ExecutionLog, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
