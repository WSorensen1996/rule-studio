declare namespace XDT {
  interface rs_Rule_Fixed {
    rs_ruleid: string;
  }
  interface rs_Rule_Result extends rs_Rule_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_DraftVersionId?: rs_RuleVersion_Result | null;
    rs_PublishedVersionId?: rs_RuleVersion_Result | null;
    rs_description: string | null;
    rs_draftversionid_guid: string | null;
    rs_enabled: boolean | null;
    rs_lastdeployedhash: string | null;
    rs_name: string | null;
    rs_publishedversionid_guid: string | null;
    rs_rs_rule_rs_executionlog_rs_ruleid?: rs_ExecutionLog_Result[] | null;
    rs_rs_rule_rs_ruleversion_rs_ruleid?: rs_RuleVersion_Result[] | null;
    rs_rulekey: string | null;
    statecode: rs_rule_statecode | null;
    statuscode: rs_rule_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_Rule_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_draftversionid_formatted?: string;
    rs_publishedversionid_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_Rule_Select {
    createdby_guid: WebAttribute<rs_Rule_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_Rule_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_Rule_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_Rule_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_Rule_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_Rule_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_Rule_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_Rule_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_Rule_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_description: WebAttribute<rs_Rule_Select, { rs_description: string | null }, object>;
    rs_draftversionid_guid: WebAttribute<rs_Rule_Select, { rs_draftversionid_guid: string | null }, { rs_draftversionid_formatted?: string }>;
    rs_enabled: WebAttribute<rs_Rule_Select, { rs_enabled: boolean | null }, object>;
    rs_lastdeployedhash: WebAttribute<rs_Rule_Select, { rs_lastdeployedhash: string | null }, object>;
    rs_name: WebAttribute<rs_Rule_Select, { rs_name: string | null }, object>;
    rs_publishedversionid_guid: WebAttribute<rs_Rule_Select, { rs_publishedversionid_guid: string | null }, { rs_publishedversionid_formatted?: string }>;
    rs_ruleid: WebAttribute<rs_Rule_Select, { rs_ruleid: string | null }, object>;
    rs_rulekey: WebAttribute<rs_Rule_Select, { rs_rulekey: string | null }, object>;
    statecode: WebAttribute<rs_Rule_Select, { statecode: rs_rule_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_Rule_Select, { statuscode: rs_rule_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_Rule_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_Rule_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_Rule_Select, { versionnumber: number | null }, object>;
  }
  interface rs_Rule_Filter {
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
    rs_draftversionid_guid: XQW.Guid;
    rs_enabled: boolean;
    rs_lastdeployedhash: string;
    rs_name: string;
    rs_publishedversionid_guid: XQW.Guid;
    rs_ruleid: XQW.Guid;
    rs_rulekey: string;
    statecode: rs_rule_statecode;
    statuscode: rs_rule_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_Rule_Expand {
    createdby: WebExpand<rs_Rule_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_Rule_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_Rule_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_Rule_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_DraftVersionId: WebExpand<rs_Rule_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_DraftVersionId: rs_RuleVersion_Result }>;
    rs_PublishedVersionId: WebExpand<rs_Rule_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_PublishedVersionId: rs_RuleVersion_Result }>;
    rs_rs_rule_rs_executionlog_rs_ruleid: WebExpand<rs_Rule_Expand, rs_ExecutionLog_Select, rs_ExecutionLog_Filter, { rs_rs_rule_rs_executionlog_rs_ruleid: rs_ExecutionLog_Result[] }>;
    rs_rs_rule_rs_ruleversion_rs_ruleid: WebExpand<rs_Rule_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_rs_rule_rs_ruleversion_rs_ruleid: rs_RuleVersion_Result[] }>;
  }
  interface rs_Rule_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_DraftVersionId_bind$rs_ruleversions?: string | null;
    rs_PublishedVersionId_bind$rs_ruleversions?: string | null;
    rs_description?: string | null;
    rs_enabled?: boolean | null;
    rs_lastdeployedhash?: string | null;
    rs_name?: string | null;
    rs_ruleid?: string | null;
    rs_rulekey?: string | null;
    statuscode?: rs_rule_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Rule_Update {
    rs_DraftVersionId_bind$rs_ruleversions?: string | null;
    rs_PublishedVersionId_bind$rs_ruleversions?: string | null;
    rs_description?: string | null;
    rs_enabled?: boolean | null;
    rs_lastdeployedhash?: string | null;
    rs_name?: string | null;
    rs_rulekey?: string | null;
    statecode?: rs_rule_statecode | null;
    statuscode?: rs_rule_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Rule_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_DraftVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
    rs_PublishedVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
  interface rs_Rule_RelatedMany {
    rs_rs_rule_rs_executionlog_rs_ruleid: WebMappingRetrieve<rs_ExecutionLog_Select, rs_ExecutionLog_Expand, rs_ExecutionLog_Filter, rs_ExecutionLog_Fixed, rs_ExecutionLog_Result, rs_ExecutionLog_FormattedResult>;
    rs_rs_rule_rs_ruleversion_rs_ruleid: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_rules: WebMappingRetrieve<XDT.rs_Rule_Select, XDT.rs_Rule_Expand, XDT.rs_Rule_Filter, XDT.rs_Rule_Fixed, XDT.rs_Rule_Result, XDT.rs_Rule_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_rules: WebMappingRelated<XDT.rs_Rule_RelatedOne, XDT.rs_Rule_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_rules: WebMappingCUDA<XDT.rs_Rule_Create, XDT.rs_Rule_Update, XDT.rs_Rule_Select>;
}
