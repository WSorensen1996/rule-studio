using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Stable rule identity, trigger and live/draft pointers.</para>
/// <para>Display Name: Rule</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_rule")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_Rule : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_rule";
    public const int EntityTypeCode = 10605;

    public rs_Rule() : base(EntityLogicalName) { }
    public rs_Rule(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_ruleid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_ruleid", value);
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
    /// <para>Display Name: Description</para>
    /// </summary>
    [AttributeLogicalName("rs_description")]
    [DisplayName("Description")]
    [MaxLength(4000)]
    public string? rs_Description
    {
        get => GetAttributeValue<string?>("rs_description");
        set => SetAttributeValue("rs_description", value);
    }

    /// <summary>
    /// <para>Display Name: Draft version</para>
    /// </summary>
    [AttributeLogicalName("rs_draftversionid")]
    [DisplayName("Draft version")]
    public EntityReference? rs_DraftVersionId
    {
        get => GetAttributeValue<EntityReference?>("rs_draftversionid");
        set => SetAttributeValue("rs_draftversionid", value);
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
    /// <para>Display Name: Last deployed hash</para>
    /// </summary>
    [AttributeLogicalName("rs_lastdeployedhash")]
    [DisplayName("Last deployed hash")]
    [MaxLength(128)]
    public string? rs_LastDeployedHash
    {
        get => GetAttributeValue<string?>("rs_lastdeployedhash");
        set => SetAttributeValue("rs_lastdeployedhash", value);
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
    /// <para>Display Name: Published version</para>
    /// </summary>
    [AttributeLogicalName("rs_publishedversionid")]
    [DisplayName("Published version")]
    public EntityReference? rs_PublishedVersionId
    {
        get => GetAttributeValue<EntityReference?>("rs_publishedversionid");
        set => SetAttributeValue("rs_publishedversionid", value);
    }

    /// <summary>
    /// <para>Display Name: Rule</para>
    /// </summary>
    [AttributeLogicalName("rs_ruleid")]
    [DisplayName("Rule")]
    public Guid? rs_RuleId
    {
        get => GetAttributeValue<Guid?>("rs_ruleid");
        set => SetId("rs_ruleid", value);
    }

    /// <summary>
    /// <para>Display Name: Rule key</para>
    /// </summary>
    [AttributeLogicalName("rs_rulekey")]
    [DisplayName("Rule key")]
    [MaxLength(100)]
    public string? rs_RuleKey
    {
        get => GetAttributeValue<string?>("rs_rulekey");
        set => SetAttributeValue("rs_rulekey", value);
    }

    /// <summary>
    /// <para>Status of the Rule</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_rule_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_rule_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Rule</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_rule_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_rule_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_rule_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_rule_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_rule_createdby", null);
        set => SetRelatedEntity("lk_rs_rule_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_rule_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_rule_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_rule_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_rule_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_rule_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_rule_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_rule_modifiedby", null);
        set => SetRelatedEntity("lk_rs_rule_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_rule_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_rule_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_rule_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_rule_modifiedonbehalfby", null, value);
    }

    [RelationshipSchemaName("rs_rs_rule_rs_executionlog_rs_ruleid")]
    [RelationshipMetadata("OneToMany", "rs_ruleid", "rs_executionlog", "rs_ruleid", "Referenced")]
    public IEnumerable<rs_ExecutionLog> rs_rs_rule_rs_executionlog_rs_ruleid
    {
        get => GetRelatedEntities<rs_ExecutionLog>("rs_rs_rule_rs_executionlog_rs_ruleid", null);
        set => SetRelatedEntities("rs_rs_rule_rs_executionlog_rs_ruleid", null, value);
    }

    [RelationshipSchemaName("rs_rs_rule_rs_ruleversion_rs_ruleid")]
    [RelationshipMetadata("OneToMany", "rs_ruleid", "rs_ruleversion", "rs_ruleid", "Referenced")]
    public IEnumerable<rs_RuleVersion> rs_rs_rule_rs_ruleversion_rs_ruleid
    {
        get => GetRelatedEntities<rs_RuleVersion>("rs_rs_rule_rs_ruleversion_rs_ruleid", null);
        set => SetRelatedEntities("rs_rs_rule_rs_ruleversion_rs_ruleid", null, value);
    }

    [AttributeLogicalName("rs_draftversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_rule_rs_draftversionid")]
    [RelationshipMetadata("ManyToOne", "rs_draftversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_rule_rs_draftversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_rule_rs_draftversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_rule_rs_draftversionid", null, value);
    }

    [AttributeLogicalName("rs_publishedversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_rule_rs_publishedversionid")]
    [RelationshipMetadata("ManyToOne", "rs_publishedversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_rule_rs_publishedversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_rule_rs_publishedversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_rule_rs_publishedversionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_Rule entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_Rule, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_Rule with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_Rule to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_Rule</returns>
    public static rs_Rule Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_Rule, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }

    /// <summary>
    /// Retrieves the rs_Rule using the Rule key alternate key.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="rs_RuleKey">rs_RuleKey key value</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_Rule</returns>
    public static rs_Rule Retrieve_rs_RuleKey_Key(IOrganizationService service, string rs_RuleKey, params Expression<Func<rs_Rule, object?>>[] columns)
    {
        var keyedEntityReference = new EntityReference(EntityLogicalName, new KeyAttributeCollection
        {
            ["rs_rulekey"] = rs_RuleKey,
        });

        return service.Retrieve(keyedEntityReference, columns);
    }
}
