using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Draft or immutable published version of a rule with its compiled snapshot.</para>
/// <para>Display Name: Rule Version</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_ruleversion")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_RuleVersion : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_ruleversion";
    public const int EntityTypeCode = 10606;

    public rs_RuleVersion() : base(EntityLogicalName) { }
    public rs_RuleVersion(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_ruleversionid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_ruleversionid", value);
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
    /// <para>Display Name: Base hash</para>
    /// </summary>
    [AttributeLogicalName("rs_basehash")]
    [DisplayName("Base hash")]
    [MaxLength(128)]
    public string? rs_BaseHash
    {
        get => GetAttributeValue<string?>("rs_basehash");
        set => SetAttributeValue("rs_basehash", value);
    }

    /// <summary>
    /// <para>Display Name: Change note</para>
    /// </summary>
    [AttributeLogicalName("rs_changenote")]
    [DisplayName("Change note")]
    [MaxLength(2000)]
    public string? rs_ChangeNote
    {
        get => GetAttributeValue<string?>("rs_changenote");
        set => SetAttributeValue("rs_changenote", value);
    }

    /// <summary>
    /// <para>Display Name: Compiled snapshot</para>
    /// </summary>
    [AttributeLogicalName("rs_compiledsnapshot")]
    [DisplayName("Compiled snapshot")]
    [MaxLength(1048576)]
    public string? rs_CompiledSnapshot
    {
        get => GetAttributeValue<string?>("rs_compiledsnapshot");
        set => SetAttributeValue("rs_compiledsnapshot", value);
    }

    /// <summary>
    /// <para>Display Name: Cost level</para>
    /// </summary>
    [AttributeLogicalName("rs_costlevel")]
    [DisplayName("Cost level")]
    public rs_ruleversion_rs_costlevel? rs_CostLevel
    {
        get => this.GetOptionSetValue<rs_ruleversion_rs_costlevel>("rs_costlevel");
        set => this.SetOptionSetValue("rs_costlevel", value);
    }

    /// <summary>
    /// <para>Display Name: Derived from version</para>
    /// </summary>
    [AttributeLogicalName("rs_derivedfromversionid")]
    [DisplayName("Derived from version")]
    public EntityReference? rs_DerivedFromVersionId
    {
        get => GetAttributeValue<EntityReference?>("rs_derivedfromversionid");
        set => SetAttributeValue("rs_derivedfromversionid", value);
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
    /// <para>Display Name: On failure</para>
    /// </summary>
    [AttributeLogicalName("rs_failurepolicy")]
    [DisplayName("On failure")]
    public rs_ruleversion_rs_failurepolicy? rs_FailurePolicy
    {
        get => this.GetOptionSetValue<rs_ruleversion_rs_failurepolicy>("rs_failurepolicy");
        set => this.SetOptionSetValue("rs_failurepolicy", value);
    }

    /// <summary>
    /// <para>Display Name: Triggering columns</para>
    /// </summary>
    [AttributeLogicalName("rs_filteredcolumns")]
    [DisplayName("Triggering columns")]
    [MaxLength(1000)]
    public string? rs_FilteredColumns
    {
        get => GetAttributeValue<string?>("rs_filteredcolumns");
        set => SetAttributeValue("rs_filteredcolumns", value);
    }

    /// <summary>
    /// <para>Display Name: Execution log level</para>
    /// </summary>
    [AttributeLogicalName("rs_loglevel")]
    [DisplayName("Execution log level")]
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
    /// <para>Display Name: Priority</para>
    /// </summary>
    [AttributeLogicalName("rs_priority")]
    [DisplayName("Priority")]
    [Range(0, 2147483647)]
    public int? rs_Priority
    {
        get => GetAttributeValue<int?>("rs_priority");
        set => SetAttributeValue("rs_priority", value);
    }

    /// <summary>
    /// <para>Display Name: Published by</para>
    /// </summary>
    [AttributeLogicalName("rs_publishedby")]
    [DisplayName("Published by")]
    public EntityReference? rs_PublishedBy
    {
        get => GetAttributeValue<EntityReference?>("rs_publishedby");
        set => SetAttributeValue("rs_publishedby", value);
    }

    /// <summary>
    /// <para>Display Name: Published on</para>
    /// </summary>
    [AttributeLogicalName("rs_publishedon")]
    [DisplayName("Published on")]
    public DateTime? rs_PublishedOn
    {
        get => GetAttributeValue<DateTime?>("rs_publishedon");
        set => SetAttributeValue("rs_publishedon", value);
    }

    /// <summary>
    /// <para>Display Name: Release</para>
    /// </summary>
    [AttributeLogicalName("rs_releaseid")]
    [DisplayName("Release")]
    public EntityReference? rs_ReleaseId
    {
        get => GetAttributeValue<EntityReference?>("rs_releaseid");
        set => SetAttributeValue("rs_releaseid", value);
    }

    /// <summary>
    /// <para>Display Name: Retry count</para>
    /// </summary>
    [AttributeLogicalName("rs_retrycount")]
    [DisplayName("Retry count")]
    [Range(0, 2147483647)]
    public int? rs_RetryCount
    {
        get => GetAttributeValue<int?>("rs_retrycount");
        set => SetAttributeValue("rs_retrycount", value);
    }

    /// <summary>
    /// <para>Display Name: Retry delay (ms)</para>
    /// </summary>
    [AttributeLogicalName("rs_retrydelayms")]
    [DisplayName("Retry delay (ms)")]
    [Range(0, 2147483647)]
    public int? rs_RetryDelayMs
    {
        get => GetAttributeValue<int?>("rs_retrydelayms");
        set => SetAttributeValue("rs_retrydelayms", value);
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
    /// <para>Display Name: Rule Version</para>
    /// </summary>
    [AttributeLogicalName("rs_ruleversionid")]
    [DisplayName("Rule Version")]
    public Guid? rs_RuleVersionId
    {
        get => GetAttributeValue<Guid?>("rs_ruleversionid");
        set => SetId("rs_ruleversionid", value);
    }

    /// <summary>
    /// <para>Display Name: Run as</para>
    /// </summary>
    [AttributeLogicalName("rs_runas")]
    [DisplayName("Run as")]
    public rs_ruleversion_rs_runas? rs_RunAs
    {
        get => this.GetOptionSetValue<rs_ruleversion_rs_runas>("rs_runas");
        set => this.SetOptionSetValue("rs_runas", value);
    }

    /// <summary>
    /// <para>Display Name: Snapshot hash</para>
    /// </summary>
    [AttributeLogicalName("rs_snapshothash")]
    [DisplayName("Snapshot hash")]
    [MaxLength(128)]
    public string? rs_SnapshotHash
    {
        get => GetAttributeValue<string?>("rs_snapshothash");
        set => SetAttributeValue("rs_snapshothash", value);
    }

    /// <summary>
    /// <para>Display Name: Source</para>
    /// </summary>
    [AttributeLogicalName("rs_source")]
    [DisplayName("Source")]
    public rs_ruleversion_rs_source? rs_Source
    {
        get => this.GetOptionSetValue<rs_ruleversion_rs_source>("rs_source");
        set => this.SetOptionSetValue("rs_source", value);
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
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("rs_status")]
    [DisplayName("Status")]
    public rs_ruleversion_rs_status? rs_Status
    {
        get => this.GetOptionSetValue<rs_ruleversion_rs_status>("rs_status");
        set => this.SetOptionSetValue("rs_status", value);
    }

    /// <summary>
    /// <para>Display Name: Stop processing after match</para>
    /// </summary>
    [AttributeLogicalName("rs_stopprocessing")]
    [DisplayName("Stop processing after match")]
    public bool? rs_StopProcessing
    {
        get => GetAttributeValue<bool?>("rs_stopprocessing");
        set => SetAttributeValue("rs_stopprocessing", value);
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
    /// <para>Display Name: Version number</para>
    /// </summary>
    [AttributeLogicalName("rs_versionnumber")]
    [DisplayName("Version number")]
    [Range(0, 2147483647)]
    public int? rs_VersionNumber
    {
        get => GetAttributeValue<int?>("rs_versionnumber");
        set => SetAttributeValue("rs_versionnumber", value);
    }

    /// <summary>
    /// <para>Status of the Rule Version</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_ruleversion_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_ruleversion_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Rule Version</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_ruleversion_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_ruleversion_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_ruleversion_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_ruleversion_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_ruleversion_createdby", null);
        set => SetRelatedEntity("lk_rs_ruleversion_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_ruleversion_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_ruleversion_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_ruleversion_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_ruleversion_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_ruleversion_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_ruleversion_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_ruleversion_modifiedby", null);
        set => SetRelatedEntity("lk_rs_ruleversion_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_ruleversion_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_ruleversion_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_ruleversion_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_ruleversion_modifiedonbehalfby", null, value);
    }

    [AttributeLogicalName("rs_releaseid")]
    [RelationshipSchemaName("rs_rs_release_rs_ruleversion_rs_releaseid")]
    [RelationshipMetadata("ManyToOne", "rs_releaseid", "rs_release", "rs_releaseid", "Referencing")]
    public rs_Release rs_rs_release_rs_ruleversion_rs_releaseid
    {
        get => GetRelatedEntity<rs_Release>("rs_rs_release_rs_ruleversion_rs_releaseid", null);
        set => SetRelatedEntity("rs_rs_release_rs_ruleversion_rs_releaseid", null, value);
    }

    [AttributeLogicalName("rs_ruleid")]
    [RelationshipSchemaName("rs_rs_rule_rs_ruleversion_rs_ruleid")]
    [RelationshipMetadata("ManyToOne", "rs_ruleid", "rs_rule", "rs_ruleid", "Referencing")]
    public rs_Rule rs_rs_rule_rs_ruleversion_rs_ruleid
    {
        get => GetRelatedEntity<rs_Rule>("rs_rs_rule_rs_ruleversion_rs_ruleid", null);
        set => SetRelatedEntity("rs_rs_rule_rs_ruleversion_rs_ruleid", null, value);
    }

    [RelationshipSchemaName("rs_rs_ruleversion_rs_action_rs_ruleversionid")]
    [RelationshipMetadata("OneToMany", "rs_ruleversionid", "rs_action", "rs_ruleversionid", "Referenced")]
    public IEnumerable<rs_Action> rs_rs_ruleversion_rs_action_rs_ruleversionid
    {
        get => GetRelatedEntities<rs_Action>("rs_rs_ruleversion_rs_action_rs_ruleversionid", null);
        set => SetRelatedEntities("rs_rs_ruleversion_rs_action_rs_ruleversionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid")]
    [RelationshipMetadata("OneToMany", "rs_ruleversionid", "rs_conditiongroup", "rs_ruleversionid", "Referenced")]
    public IEnumerable<rs_ConditionGroup> rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid
    {
        get => GetRelatedEntities<rs_ConditionGroup>("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid", null);
        set => SetRelatedEntities("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid")]
    [RelationshipMetadata("OneToMany", "rs_ruleversionid", "rs_executionlog", "rs_ruleversionid", "Referenced")]
    public IEnumerable<rs_ExecutionLog> rs_rs_ruleversion_rs_executionlog_rs_ruleversionid
    {
        get => GetRelatedEntities<rs_ExecutionLog>("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid", null);
        set => SetRelatedEntities("rs_rs_ruleversion_rs_executionlog_rs_ruleversionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_ruleversion_rs_rule_rs_draftversionid")]
    [RelationshipMetadata("OneToMany", "rs_ruleversionid", "rs_rule", "rs_draftversionid", "Referenced")]
    public IEnumerable<rs_Rule> rs_rs_ruleversion_rs_rule_rs_draftversionid
    {
        get => GetRelatedEntities<rs_Rule>("rs_rs_ruleversion_rs_rule_rs_draftversionid", null);
        set => SetRelatedEntities("rs_rs_ruleversion_rs_rule_rs_draftversionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_ruleversion_rs_rule_rs_publishedversionid")]
    [RelationshipMetadata("OneToMany", "rs_ruleversionid", "rs_rule", "rs_publishedversionid", "Referenced")]
    public IEnumerable<rs_Rule> rs_rs_ruleversion_rs_rule_rs_publishedversionid
    {
        get => GetRelatedEntities<rs_Rule>("rs_rs_ruleversion_rs_rule_rs_publishedversionid", null);
        set => SetRelatedEntities("rs_rs_ruleversion_rs_rule_rs_publishedversionid", null, value);
    }

    [AttributeLogicalName("rs_derivedfromversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid")]
    [RelationshipMetadata("ManyToOne", "rs_derivedfromversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid", null, value);
    }

    [AttributeLogicalName("rs_publishedby")]
    [RelationshipSchemaName("rs_systemuser_rs_ruleversion_rs_publishedby")]
    [RelationshipMetadata("ManyToOne", "rs_publishedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser rs_systemuser_rs_ruleversion_rs_publishedby
    {
        get => GetRelatedEntity<SystemUser>("rs_systemuser_rs_ruleversion_rs_publishedby", null);
        set => SetRelatedEntity("rs_systemuser_rs_ruleversion_rs_publishedby", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_RuleVersion entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_RuleVersion, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_RuleVersion with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_RuleVersion to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_RuleVersion</returns>
    public static rs_RuleVersion Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_RuleVersion, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
