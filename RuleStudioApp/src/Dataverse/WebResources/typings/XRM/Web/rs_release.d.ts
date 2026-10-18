declare namespace XDT {
  interface rs_Release_Fixed {
    rs_releaseid: string;
  }
  interface rs_Release_Result extends rs_Release_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_PublishedBy?: SystemUser_Result | null;
    rs_RolledBackReleaseId?: rs_Release_Result | null;
    rs_description: string | null;
    rs_name: string | null;
    rs_publishedby_guid: string | null;
    rs_publishedon: Date | null;
    rs_rolledbackreleaseid_guid: string | null;
    rs_rs_release_rs_release_rs_rolledbackreleaseid?: rs_Release_Result[] | null;
    rs_rs_release_rs_ruleversion_rs_releaseid?: rs_RuleVersion_Result[] | null;
    rs_status: rs_release_rs_status | null;
    rs_validationresult: string | null;
    statecode: rs_release_statecode | null;
    statuscode: rs_release_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_Release_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_publishedby_formatted?: string;
    rs_publishedon_formatted?: string;
    rs_rolledbackreleaseid_formatted?: string;
    rs_status_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_Release_Select {
    createdby_guid: WebAttribute<rs_Release_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_Release_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_Release_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_Release_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_Release_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_Release_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_Release_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_Release_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_Release_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_description: WebAttribute<rs_Release_Select, { rs_description: string | null }, object>;
    rs_name: WebAttribute<rs_Release_Select, { rs_name: string | null }, object>;
    rs_publishedby_guid: WebAttribute<rs_Release_Select, { rs_publishedby_guid: string | null }, { rs_publishedby_formatted?: string }>;
    rs_publishedon: WebAttribute<rs_Release_Select, { rs_publishedon: Date | null }, { rs_publishedon_formatted?: string }>;
    rs_releaseid: WebAttribute<rs_Release_Select, { rs_releaseid: string | null }, object>;
    rs_rolledbackreleaseid_guid: WebAttribute<rs_Release_Select, { rs_rolledbackreleaseid_guid: string | null }, { rs_rolledbackreleaseid_formatted?: string }>;
    rs_status: WebAttribute<rs_Release_Select, { rs_status: rs_release_rs_status | null }, { rs_status_formatted?: string }>;
    rs_validationresult: WebAttribute<rs_Release_Select, { rs_validationresult: string | null }, object>;
    statecode: WebAttribute<rs_Release_Select, { statecode: rs_release_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_Release_Select, { statuscode: rs_release_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_Release_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_Release_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_Release_Select, { versionnumber: number | null }, object>;
  }
  interface rs_Release_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_description: string;
    rs_name: string;
    rs_publishedby_guid: XQW.Guid;
    rs_publishedon: Date;
    rs_releaseid: XQW.Guid;
    rs_rolledbackreleaseid_guid: XQW.Guid;
    rs_status: rs_release_rs_status;
    rs_validationresult: string;
    statecode: rs_release_statecode;
    statuscode: rs_release_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_Release_Expand {
    createdby: WebExpand<rs_Release_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_Release_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_Release_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_Release_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_PublishedBy: WebExpand<rs_Release_Expand, SystemUser_Select, SystemUser_Filter, { rs_PublishedBy: SystemUser_Result }>;
    rs_RolledBackReleaseId: WebExpand<rs_Release_Expand, rs_Release_Select, rs_Release_Filter, { rs_RolledBackReleaseId: rs_Release_Result }>;
    rs_rs_release_rs_release_rs_rolledbackreleaseid: WebExpand<rs_Release_Expand, rs_Release_Select, rs_Release_Filter, { rs_rs_release_rs_release_rs_rolledbackreleaseid: rs_Release_Result[] }>;
    rs_rs_release_rs_ruleversion_rs_releaseid: WebExpand<rs_Release_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_rs_release_rs_ruleversion_rs_releaseid: rs_RuleVersion_Result[] }>;
  }
  interface rs_Release_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_PublishedBy_bind$systemusers?: string | null;
    rs_RolledBackReleaseId_bind$rs_releases?: string | null;
    rs_description?: string | null;
    rs_name?: string | null;
    rs_publishedon?: Date | null;
    rs_releaseid?: string | null;
    rs_status?: rs_release_rs_status | null;
    rs_validationresult?: string | null;
    statuscode?: rs_release_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Release_Update {
    rs_PublishedBy_bind$systemusers?: string | null;
    rs_RolledBackReleaseId_bind$rs_releases?: string | null;
    rs_description?: string | null;
    rs_name?: string | null;
    rs_publishedon?: Date | null;
    rs_status?: rs_release_rs_status | null;
    rs_validationresult?: string | null;
    statecode?: rs_release_statecode | null;
    statuscode?: rs_release_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Release_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_PublishedBy: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_RolledBackReleaseId: WebMappingRetrieve<rs_Release_Select, rs_Release_Expand, rs_Release_Filter, rs_Release_Fixed, rs_Release_Result, rs_Release_FormattedResult>;
  }
  interface rs_Release_RelatedMany {
    rs_rs_release_rs_release_rs_rolledbackreleaseid: WebMappingRetrieve<rs_Release_Select, rs_Release_Expand, rs_Release_Filter, rs_Release_Fixed, rs_Release_Result, rs_Release_FormattedResult>;
    rs_rs_release_rs_ruleversion_rs_releaseid: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_releases: WebMappingRetrieve<XDT.rs_Release_Select, XDT.rs_Release_Expand, XDT.rs_Release_Filter, XDT.rs_Release_Fixed, XDT.rs_Release_Result, XDT.rs_Release_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_releases: WebMappingRelated<XDT.rs_Release_RelatedOne, XDT.rs_Release_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_releases: WebMappingCUDA<XDT.rs_Release_Create, XDT.rs_Release_Update, XDT.rs_Release_Select>;
}
