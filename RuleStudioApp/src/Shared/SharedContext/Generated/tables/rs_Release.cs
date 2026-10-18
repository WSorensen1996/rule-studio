using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace RuleStudio.SharedContext;

/// <summary>
/// <para>Atomic publication of one or more rule versions; unit of rollback.</para>
/// <para>Display Name: Release</para>
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[EntityLogicalName("rs_release")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DataContract]
#pragma warning disable CS8981 // Allows: Only lowercase characters
public partial class rs_Release : ExtendedEntity
#pragma warning restore CS8981
{
    public const string EntityLogicalName = "rs_release";
    public const int EntityTypeCode = 10613;

    public rs_Release() : base(EntityLogicalName) { }
    public rs_Release(Guid id) : base(EntityLogicalName, id) { }

    private string DebuggerDisplay => GetDebuggerDisplay("rs_name");

    [AttributeLogicalName("rs_releaseid")]
    public override Guid Id {
        get {
            return base.Id;
        }
        set {
            SetId("rs_releaseid", value);
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
    [MaxLength(2000)]
    public string? rs_Description
    {
        get => GetAttributeValue<string?>("rs_description");
        set => SetAttributeValue("rs_description", value);
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
    public Guid? rs_ReleaseId
    {
        get => GetAttributeValue<Guid?>("rs_releaseid");
        set => SetId("rs_releaseid", value);
    }

    /// <summary>
    /// <para>Display Name: Rolled back release</para>
    /// </summary>
    [AttributeLogicalName("rs_rolledbackreleaseid")]
    [DisplayName("Rolled back release")]
    public EntityReference? rs_RolledBackReleaseId
    {
        get => GetAttributeValue<EntityReference?>("rs_rolledbackreleaseid");
        set => SetAttributeValue("rs_rolledbackreleaseid", value);
    }

    /// <summary>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("rs_status")]
    [DisplayName("Status")]
    public rs_release_rs_status? rs_Status
    {
        get => this.GetOptionSetValue<rs_release_rs_status>("rs_status");
        set => this.SetOptionSetValue("rs_status", value);
    }

    /// <summary>
    /// <para>Display Name: Validation result</para>
    /// </summary>
    [AttributeLogicalName("rs_validationresult")]
    [DisplayName("Validation result")]
    [MaxLength(100000)]
    public string? rs_ValidationResult
    {
        get => GetAttributeValue<string?>("rs_validationresult");
        set => SetAttributeValue("rs_validationresult", value);
    }

    /// <summary>
    /// <para>Status of the Release</para>
    /// <para>Display Name: Status</para>
    /// </summary>
    [AttributeLogicalName("statecode")]
    [DisplayName("Status")]
    public rs_release_statecode? statecode
    {
        get => this.GetOptionSetValue<rs_release_statecode>("statecode");
        set => this.SetOptionSetValue("statecode", value);
    }

    /// <summary>
    /// <para>Reason for the status of the Release</para>
    /// <para>Display Name: Status Reason</para>
    /// </summary>
    [AttributeLogicalName("statuscode")]
    [DisplayName("Status Reason")]
    public rs_release_statuscode? statuscode
    {
        get => this.GetOptionSetValue<rs_release_statuscode>("statuscode");
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
    [RelationshipSchemaName("lk_rs_release_createdby")]
    [RelationshipMetadata("ManyToOne", "createdby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_release_createdby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_release_createdby", null);
        set => SetRelatedEntity("lk_rs_release_createdby", null, value);
    }

    [AttributeLogicalName("createdonbehalfby")]
    [RelationshipSchemaName("lk_rs_release_createdonbehalfby")]
    [RelationshipMetadata("ManyToOne", "createdonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_release_createdonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_release_createdonbehalfby", null);
        set => SetRelatedEntity("lk_rs_release_createdonbehalfby", null, value);
    }

    [AttributeLogicalName("modifiedby")]
    [RelationshipSchemaName("lk_rs_release_modifiedby")]
    [RelationshipMetadata("ManyToOne", "modifiedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_release_modifiedby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_release_modifiedby", null);
        set => SetRelatedEntity("lk_rs_release_modifiedby", null, value);
    }

    [AttributeLogicalName("modifiedonbehalfby")]
    [RelationshipSchemaName("lk_rs_release_modifiedonbehalfby")]
    [RelationshipMetadata("ManyToOne", "modifiedonbehalfby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser lk_rs_release_modifiedonbehalfby
    {
        get => GetRelatedEntity<SystemUser>("lk_rs_release_modifiedonbehalfby", null);
        set => SetRelatedEntity("lk_rs_release_modifiedonbehalfby", null, value);
    }

    [AttributeLogicalName("rs_rolledbackreleaseid")]
    [RelationshipSchemaName("rs_rs_release_rs_release_rs_rolledbackreleaseid")]
    [RelationshipMetadata("ManyToOne", "rs_rolledbackreleaseid", "rs_release", "rs_releaseid", "Referencing")]
    public rs_Release rs_rs_release_rs_release_rs_rolledbackreleaseid
    {
        get => GetRelatedEntity<rs_Release>("rs_rs_release_rs_release_rs_rolledbackreleaseid", null);
        set => SetRelatedEntity("rs_rs_release_rs_release_rs_rolledbackreleaseid", null, value);
    }

    [RelationshipSchemaName("rs_rs_release_rs_ruleversion_rs_releaseid")]
    [RelationshipMetadata("OneToMany", "rs_releaseid", "rs_ruleversion", "rs_releaseid", "Referenced")]
    public IEnumerable<rs_RuleVersion> rs_rs_release_rs_ruleversion_rs_releaseid
    {
        get => GetRelatedEntities<rs_RuleVersion>("rs_rs_release_rs_ruleversion_rs_releaseid", null);
        set => SetRelatedEntities("rs_rs_release_rs_ruleversion_rs_releaseid", null, value);
    }

    [AttributeLogicalName("rs_publishedby")]
    [RelationshipSchemaName("rs_systemuser_rs_release_rs_publishedby")]
    [RelationshipMetadata("ManyToOne", "rs_publishedby", "systemuser", "systemuserid", "Referencing")]
    public SystemUser rs_systemuser_rs_release_rs_publishedby
    {
        get => GetRelatedEntity<SystemUser>("rs_systemuser_rs_release_rs_publishedby", null);
        set => SetRelatedEntity("rs_systemuser_rs_release_rs_publishedby", null, value);
    }

    /// <summary>
    /// Gets the logical column name for a property on the rs_Release entity, using the AttributeLogicalNameAttribute if present.
    /// </summary>
    /// <param name="column">Expression to pick the column</param>
    /// <returns>Name of column</returns>
    /// <exception cref="ArgumentNullException">If no expression is provided</exception>
    /// <exception cref="ArgumentException">If the expression is not x => x.column</exception>
    public static string GetColumnName(Expression<Func<rs_Release, object?>> column)
    {
        return TableAttributeHelpers.GetColumnName(column);
    }

    /// <summary>
    /// Retrieves the rs_Release with the specified columns.
    /// </summary>
    /// <param name="service">Organization service</param>
    /// <param name="id">Id of rs_Release to retrieve</param>
    /// <param name="columns">Expressions that specify columns to retrieve</param>
    /// <returns>The retrieved rs_Release</returns>
    public static rs_Release Retrieve(IOrganizationService service, Guid id, params Expression<Func<rs_Release, object?>>[] columns)
    {
        return service.Retrieve(id, columns);
    }
}
