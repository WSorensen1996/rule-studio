using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Field path → operator → value predicate.</para>
/// <para>Display Name: Condition</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_condition")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_Condition : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_condition";
    public const int EntityTypeCode = 10608;

    public rs_Condition() : base(EntityLogicalName) { }
    public rs_Condition(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_conditionid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_conditionid", value);
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
    /// <para>Display Name: Child operator</para>
    /// </summary>
    [AttributeLogicalName("rs_aggregate")]
    [DisplayName("Child operator")]
    public rs_condition_rs_aggregate? rs_Aggregate
    {
        get => this.GetOptionSetValue<rs_condition_rs_aggregate>("rs_aggregate");
        set => this.SetOptionSetValue("rs_aggregate", value);
    }

    /// <summary>
    /// <para>Display Name: Condition group</para>
    /// </summary>
    [AttributeLogicalName("rs_conditiongroupid")]
    [DisplayName("Condition group")]
    public EntityReference? rs_ConditionGroupId
    {
        get => GetAttributeValue<EntityReference?>("rs_conditiongroupid");
        set => SetAttributeValue("rs_conditiongroupid", value);
    }

    /// <summary>
    /// <para>Display Name: Condition</para>
    /// </summary>
    [AttributeLogicalName("rs_conditionid")]
    [DisplayName("Condition")]
    public Guid? rs_ConditionId
    {
        get => GetAttributeValue<Guid?>("rs_conditionid");
        set => SetId("rs_conditionid", value);
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
    /// <para>Display Name: Field path</para>
    /// </summary>
    [AttributeLogicalName("rs_fieldpath")]
    [DisplayName("Field path")]
    [MaxLength(4000)]
    public string? rs_FieldPath
    {
        get => GetAttributeValue<string?>("rs_fieldpath");
        set => SetAttributeValue("rs_fieldpath", value);
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
    /// <para>Display Name: Comparison</para>
    /// </summary>
    [AttributeLogicalName("rs_operator")]
    [DisplayName("Comparison")]
    public rs_condition_rs_operator? rs_Operator
    {
        get => this.GetOptionSetValue<rs_condition_rs_operator>("rs_operator");
        set => this.SetOptionSetValue("rs_operator", value);
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
    /// <para>Status of the Condition</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_condition_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_condition_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Condition</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_condition_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_condition_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_condition_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_condition_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_condition_createdby", null);
        set => SetRelatedEntity("lk_rs_condition_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_condition_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_condition_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_condition_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_condition_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_condition_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_condition_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_condition_modifiedby", null);
        set => SetRelatedEntity("lk_rs_condition_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_condition_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_condition_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_condition_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_condition_modifiedonbehalfby", null, value);
    }

    [RelationshipSchemaName("rs_rs_condition_rs_actioninputmapping_rs_conditionid")]
    [RelationshipMetadata("OneToMany", "rs_conditionid", "rs_actioninputmapping", "rs_conditionid", "Referenced")]
    public IEnumerable<rs_ActionInputMapping> rs_rs_condition_rs_actioninputmapping_rs_conditionid
    {
        get => GetRelatedEntities<rs_ActionInputMapping>("rs_rs_condition_rs_actioninputmapping_rs_conditionid", null);
        set => SetRelatedEntities("rs_rs_condition_rs_actioninputmapping_rs_conditionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid")]
    [RelationshipMetadata("OneToMany", "rs_conditionid", "rs_conditiongroup", "rs_aggregatescopeconditionid", "Referenced")]
    public IEnumerable<rs_ConditionGroup> rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid
    {
        get => GetRelatedEntities<rs_ConditionGroup>("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid", null);
        set => SetRelatedEntities("rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid", null, value);
    }

    [AttributeLogicalName("rs_conditiongroupid")]
    [RelationshipSchemaName("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid")]
    [RelationshipMetadata("ManyToOne", "rs_conditiongroupid", "rs_conditiongroup", "rs_conditiongroupid", "Referencing")]
    public rs_ConditionGroup rs_rs_conditiongroup_rs_condition_rs_conditiongroupid
    {
        get => GetRelatedEntity<rs_ConditionGroup>("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid", null);
        set => SetRelatedEntity("rs_rs_conditiongroup_rs_condition_rs_conditiongroupid", null, value);
    }

    [AttributeLogicalName("rs_customfunctionid")]
    [RelationshipSchemaName("rs_rs_customfunction_rs_condition_rs_customfunctionid")]
    [RelationshipMetadata("ManyToOne", "rs_customfunctionid", "rs_customfunction", "rs_customfunctionid", "Referencing")]
    public rs_CustomFunction rs_rs_customfunction_rs_condition_rs_customfunctionid
    {
        get => GetRelatedEntity<rs_CustomFunction>("rs_rs_customfunction_rs_condition_rs_customfunctionid", null);
        set => SetRelatedEntity("rs_rs_customfunction_rs_condition_rs_customfunctionid", null, value);
    }

    [AttributeLogicalName("rs_lookupdefinitionid")]
    [RelationshipSchemaName("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid")]
    [RelationshipMetadata("ManyToOne", "rs_lookupdefinitionid", "rs_lookupdefinition", "rs_lookupdefinitionid", "Referencing")]
    public rs_LookupDefinition rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid
    {
        get => GetRelatedEntity<rs_LookupDefinition>("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid", null);
        set => SetRelatedEntity("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_Condition entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_Condition, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_Condition with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_Condition to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_Condition</returns>
    public static rs_Condition Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_Condition, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
