using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Nested AND/OR group of conditions.</para>
/// <para>Display Name: Condition Group</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_conditiongroup")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_ConditionGroup : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_conditiongroup";
    public const int EntityTypeCode = 10607;

    public rs_ConditionGroup() : base(EntityLogicalName) { }
    public rs_ConditionGroup(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_conditiongroupid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_conditiongroupid", value);
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
    /// <para>Display Name: Aggregate scope condition</para>
    /// </summary>
    [AttributeLogicalName("rs_aggregatescopeconditionid")]
    [DisplayName("Aggregate scope condition")]
    public EntityReference? rs_AggregateScopeConditionId
    {
        get => GetAttributeValue<EntityReference?>("rs_aggregatescopeconditionid");
        set => SetAttributeValue("rs_aggregatescopeconditionid", value);
    }

    /// <summary>
    /// <para>Display Name: Condition Group</para>
    /// </summary>
    [AttributeLogicalName("rs_conditiongroupid")]
    [DisplayName("Condition Group")]
    public Guid? rs_ConditionGroupId
    {
        get => GetAttributeValue<Guid?>("rs_conditiongroupid");
        set => SetId("rs_conditiongroupid", value);
    }

    /// <summary>
    /// <para>Display Name: Logical operator</para>
    /// </summary>
    [AttributeLogicalName("rs_logicaloperator")]
    [DisplayName("Logical operator")]
    public rs_conditiongroup_rs_logicaloperator? rs_LogicalOperator
    {
        get => this.GetOptionSetValue<rs_conditiongroup_rs_logicaloperator>("rs_logicaloperator");
        set => this.SetOptionSetValue("rs_logicaloperator", value);
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
    /// <para>Display Name: Parent group</para>
    /// </summary>
    [AttributeLogicalName("rs_parentgroupid")]
    [DisplayName("Parent group")]
    public EntityReference? rs_ParentGroupId
    {
        get => GetAttributeValue<EntityReference?>("rs_parentgroupid");
        set => SetAttributeValue("rs_parentgroupid", value);
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
    /// <para>Status of the Condition Group</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_conditiongroup_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_conditiongroup_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Condition Group</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_conditiongroup_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_conditiongroup_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_conditiongroup_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_conditiongroup_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_conditiongroup_createdby", null);
        set => SetRelatedEntity("lk_rs_conditiongroup_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_conditiongroup_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_conditiongroup_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_conditiongroup_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_conditiongroup_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_conditiongroup_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_conditiongroup_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_conditiongroup_modifiedby", null);
        set => SetRelatedEntity("lk_rs_conditiongroup_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_conditiongroup_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_conditiongroup_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_conditiongroup_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_conditiongroup_modifiedonbehalfby", null, value);
    }

    [AttributeLogicalName("rs_aggregatescopeconditionid")]
    [RelationshipSchemaName("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid")]
    [RelationshipMetadata("ManyToOne", "rs_aggregatescopeconditionid", "rs_condition", "rs_conditionid", "Referencing")]
    public rs_Condition rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid
    {
        get => GetRelatedEntity<rs_Condition>("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid", null);
        set => SetRelatedEntity("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid")]
    [RelationshipMetadata("OneToMany", "rs_conditiongroupid", "rs_condition", "rs_conditiongroupid", "Referenced")]
    public IEnumerable<rs_Condition> rs_rs_conditiongroup_rs_condition_rs_conditiongroupid
    {
        get => GetRelatedEntities<rs_Condition>("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid", null);
        set => SetRelatedEntities("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid", null, value);
    }

    [AttributeLogicalName("rs_parentgroupid")]
    [RelationshipSchemaName("rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid")]
    [RelationshipMetadata("ManyToOne", "rs_parentgroupid", "rs_conditiongroup", "rs_conditiongroupid", "Referencing")]
    public rs_ConditionGroup rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid
    {
        get => GetRelatedEntity<rs_ConditionGroup>("rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid", null);
        set => SetRelatedEntity("rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid", null, value);
    }

    [AttributeLogicalName("rs_ruleversionid")]
    [RelationshipSchemaName("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid")]
    [RelationshipMetadata("ManyToOne", "rs_ruleversionid", "rs_ruleversion", "rs_ruleversionid", "Referencing")]
    public rs_RuleVersion rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid
    {
        get => GetRelatedEntity<rs_RuleVersion>("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid", null);
        set => SetRelatedEntity("rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_ConditionGroup entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_ConditionGroup, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_ConditionGroup with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_ConditionGroup to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_ConditionGroup</returns>
    public static rs_ConditionGroup Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_ConditionGroup, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
