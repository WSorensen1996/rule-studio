using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Typed mapping of a value to a custom function input parameter.</para>
/// <para>Display Name: Action Input Mapping</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_actioninputmapping")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_ActionInputMapping : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_actioninputmapping";
    public const int EntityTypeCode = 10610;

    public rs_ActionInputMapping() : base(EntityLogicalName) { }
    public rs_ActionInputMapping(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_actioninputmappingid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_actioninputmappingid", value);
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
    public EntityReference? rs_ActionId
    {
        get => GetAttributeValue<EntityReference?>("rs_actionid");
        set => SetAttributeValue("rs_actionid", value);
    }

    /// <summary>
    /// <para>Display Name: Action Input Mapping</para>
    /// </summary>
    [AttributeLogicalName("rs_actioninputmappingid")]
    [DisplayName("Action Input Mapping")]
    public Guid? rs_ActionInputMappingId
    {
        get => GetAttributeValue<Guid?>("rs_actioninputmappingid");
        set => SetId("rs_actioninputmappingid", value);
    }

    /// <summary>
    /// <para>Display Name: Condition</para>
    /// </summary>
    [AttributeLogicalName("rs_conditionid")]
    [DisplayName("Condition")]
    public EntityReference? rs_ConditionId
    {
        get => GetAttributeValue<EntityReference?>("rs_conditionid");
        set => SetAttributeValue("rs_conditionid", value);
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
    /// <para>Display Name: Parameter name</para>
    /// </summary>
    [AttributeLogicalName("rs_parametername")]
    [DisplayName("Parameter name")]
    [MaxLength(100)]
    public string? rs_ParameterName
    {
        get => GetAttributeValue<string?>("rs_parametername");
        set => SetAttributeValue("rs_parametername", value);
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
    /// <para>Status of the Action Input Mapping</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_actioninputmapping_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_actioninputmapping_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Action Input Mapping</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_actioninputmapping_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_actioninputmapping_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_actioninputmapping_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_actioninputmapping_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_actioninputmapping_createdby", null);
        set => SetRelatedEntity("lk_rs_actioninputmapping_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_actioninputmapping_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_actioninputmapping_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_actioninputmapping_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_actioninputmapping_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_actioninputmapping_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_actioninputmapping_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_actioninputmapping_modifiedby", null);
        set => SetRelatedEntity("lk_rs_actioninputmapping_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_actioninputmapping_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_actioninputmapping_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_actioninputmapping_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_actioninputmapping_modifiedonbehalfby", null, value);
    }

    [AttributeLogicalName("rs_actionid")]
    [RelationshipSchemaName("rs_rs_action_rs_actioninputmapping_rs_actionid")]
    [RelationshipMetadata("ManyToOne", "rs_actionid", "rs_action", "rs_actionid", "Referencing")]
    public rs_Action rs_rs_action_rs_actioninputmapping_rs_actionid
    {
        get => GetRelatedEntity<rs_Action>("rs_rs_action_rs_actioninputmapping_rs_actionid", null);
        set => SetRelatedEntity("rs_rs_action_rs_actioninputmapping_rs_actionid", null, value);
    }

    [AttributeLogicalName("rs_conditionid")]
    [RelationshipSchemaName("rs_rs_condition_rs_actioninputmapping_rs_conditionid")]
    [RelationshipMetadata("ManyToOne", "rs_conditionid", "rs_condition", "rs_conditionid", "Referencing")]
    public rs_Condition rs_rs_condition_rs_actioninputmapping_rs_conditionid
    {
        get => GetRelatedEntity<rs_Condition>("rs_rs_condition_rs_actioninputmapping_rs_conditionid", null);
        set => SetRelatedEntity("rs_rs_condition_rs_actioninputmapping_rs_conditionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_ActionInputMapping entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_ActionInputMapping, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_ActionInputMapping with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_ActionInputMapping to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_ActionInputMapping</returns>
    public static rs_ActionInputMapping Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_ActionInputMapping, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
