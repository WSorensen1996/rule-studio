using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Reusable value source reading from any Dataverse table.</para>
/// <para>Display Name: Lookup Definition</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_lookupdefinition")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_LookupDefinition : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_lookupdefinition";
    public const int EntityTypeCode = 10612;

    public rs_LookupDefinition() : base(EntityLogicalName) { }
    public rs_LookupDefinition(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_lookupdefinitionid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_lookupdefinitionid", value);
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
    /// <para>Display Name: Default value</para>
    /// </summary>
    [AttributeLogicalName("rs_defaultvalue")]
    [DisplayName("Default value")]
    [MaxLength(1000)]
    public string? rs_DefaultValue
    {
        get => GetAttributeValue<string?>("rs_defaultvalue");
        set => SetAttributeValue("rs_defaultvalue", value);
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
    /// <para>Display Name: Lookup Definition</para>
    /// </summary>
    [AttributeLogicalName("rs_lookupdefinitionid")]
    [DisplayName("Lookup Definition")]
    public Guid? rs_LookupDefinitionId
    {
        get => GetAttributeValue<Guid?>("rs_lookupdefinitionid");
        set => SetId("rs_lookupdefinitionid", value);
    }

    /// <summary>
    /// <para>Display Name: Lookup table</para>
    /// </summary>
    [AttributeLogicalName("rs_lookuptable")]
    [DisplayName("Lookup table")]
    [MaxLength(100)]
    public string? rs_LookupTable
    {
        get => GetAttributeValue<string?>("rs_lookuptable");
        set => SetAttributeValue("rs_lookuptable", value);
    }

    /// <summary>
    /// <para>Display Name: Match criteria</para>
    /// </summary>
    [AttributeLogicalName("rs_matchcriteria")]
    [DisplayName("Match criteria")]
    [MaxLength(10000)]
    public string? rs_MatchCriteria
    {
        get => GetAttributeValue<string?>("rs_matchcriteria");
        set => SetAttributeValue("rs_matchcriteria", value);
    }

    /// <summary>
    /// <para>Display Name: If multiple rows match</para>
    /// </summary>
    [AttributeLogicalName("rs_multimatchpolicy")]
    [DisplayName("If multiple rows match")]
    public rs_lookupdefinition_rs_multimatchpolicy? rs_MultiMatchPolicy
    {
        get => this.GetOptionSetValue<rs_lookupdefinition_rs_multimatchpolicy>("rs_multimatchpolicy");
        set => this.SetOptionSetValue("rs_multimatchpolicy", value);
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
    /// <para>Display Name: If no rows match</para>
    /// </summary>
    [AttributeLogicalName("rs_nomatchpolicy")]
    [DisplayName("If no rows match")]
    public rs_lookupdefinition_rs_nomatchpolicy? rs_NoMatchPolicy
    {
        get => this.GetOptionSetValue<rs_lookupdefinition_rs_nomatchpolicy>("rs_nomatchpolicy");
        set => this.SetOptionSetValue("rs_nomatchpolicy", value);
    }

    /// <summary>
    /// <para>Display Name: Return column</para>
    /// </summary>
    [AttributeLogicalName("rs_returncolumn")]
    [DisplayName("Return column")]
    [MaxLength(100)]
    public string? rs_ReturnColumn
    {
        get => GetAttributeValue<string?>("rs_returncolumn");
        set => SetAttributeValue("rs_returncolumn", value);
    }

    /// <summary>
    /// <para>Display Name: Sort column</para>
    /// </summary>
    [AttributeLogicalName("rs_sortcolumn")]
    [DisplayName("Sort column")]
    [MaxLength(100)]
    public string? rs_SortColumn
    {
        get => GetAttributeValue<string?>("rs_sortcolumn");
        set => SetAttributeValue("rs_sortcolumn", value);
    }

    /// <summary>
    /// <para>Status of the Lookup Definition</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_lookupdefinition_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_lookupdefinition_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Lookup Definition</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_lookupdefinition_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_lookupdefinition_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_lookupdefinition_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_lookupdefinition_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_lookupdefinition_createdby", null);
        set => SetRelatedEntity("lk_rs_lookupdefinition_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_lookupdefinition_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_lookupdefinition_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_lookupdefinition_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_lookupdefinition_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_lookupdefinition_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_lookupdefinition_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_lookupdefinition_modifiedby", null);
        set => SetRelatedEntity("lk_rs_lookupdefinition_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_lookupdefinition_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_lookupdefinition_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_lookupdefinition_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_lookupdefinition_modifiedonbehalfby", null, value);
    }

    [RelationshipSchemaName("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid")]
    [RelationshipMetadata("OneToMany", "rs_lookupdefinitionid", "rs_action", "rs_lookupdefinitionid", "Referenced")]
    public IEnumerable<rs_Action> rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid
    {
        get => GetRelatedEntities<rs_Action>("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid", null);
        set => SetRelatedEntities("rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid", null, value);
    }

    [RelationshipSchemaName("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid")]
    [RelationshipMetadata("OneToMany", "rs_lookupdefinitionid", "rs_condition", "rs_lookupdefinitionid", "Referenced")]
    public IEnumerable<rs_Condition> rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid
    {
        get => GetRelatedEntities<rs_Condition>("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid", null);
        set => SetRelatedEntities("rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_LookupDefinition entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_LookupDefinition, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_LookupDefinition with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_LookupDefinition to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_LookupDefinition</returns>
    public static rs_LookupDefinition Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_LookupDefinition, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
