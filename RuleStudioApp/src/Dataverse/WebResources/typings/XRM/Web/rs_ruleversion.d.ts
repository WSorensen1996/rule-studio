declare namespace XDT {
  interface rs_RuleVersion_Fixed {
    rs_ruleversionid: string;
  }
  interface rs_RuleVersion_Result extends rs_RuleVersion_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_DerivedFromVersionId?: rs_RuleVersion_Result | null;
    rs_PublishedBy?: SystemUser_Result | null;
    rs_ReleaseId?: rs_Release_Result | null;
    rs_RuleId?: rs_Rule_Result | null;
    rs_basehash: string | null;
    rs_changenote: string | null;
    rs_compiledsnapshot: string | null;
    rs_costlevel: rs_ruleversion_rs_costlevel | null;
    rs_derivedfromversionid_guid: string | null;
    rs_event: rs_event | null;
    rs_failurepolicy: rs_ruleversion_rs_failurepolicy | null;
    rs_filteredcolumns: string | null;
    rs_loglevel: rs_loglevel | null;
    rs_name: string | null;
    rs_priority: number | null;
    rs_publishedby_guid: string | null;
    rs_publishedon: Date | null;
    rs_releaseid_guid: string | null;
    rs_retrycount: number | null;
    rs_retrydelayms: number | null;
    rs_rs_ruleversion_rs_action_rs_ruleversionid?: rs_Action_Result[] | null;
    rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid?: rs_ConditionGroup_Result[] | null;
    rs_rs_ruleversion_rs_executionlog_rs_ruleversionid?: rs_ExecutionLog_Result[] | null;
    rs_rs_ruleversion_rs_rule_rs_draftversionid?: rs_Rule_Result[] | null;
    rs_rs_ruleversion_rs_rule_rs_publishedversionid?: rs_Rule_Result[] | null;
    rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid?: rs_RuleVersion_Result[] | null;
    rs_ruleid_guid: string | null;
    rs_runas: rs_ruleversion_rs_runas | null;
    rs_snapshothash: string | null;
    rs_source: rs_ruleversion_rs_source | null;
    rs_stage: rs_stage | null;
    rs_status: rs_ruleversion_rs_status | null;
    rs_stopprocessing: boolean | null;
    rs_targettable: string | null;
    rs_versionnumber: number | null;
    statecode: rs_ruleversion_statecode | null;
    statuscode: rs_ruleversion_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_RuleVersion_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_costlevel_formatted?: string;
    rs_derivedfromversionid_formatted?: string;
    rs_event_formatted?: string;
    rs_failurepolicy_formatted?: string;
    rs_loglevel_formatted?: string;
    rs_publishedby_formatted?: string;
    rs_publishedon_formatted?: string;
    rs_releaseid_formatted?: string;
    rs_ruleid_formatted?: string;
    rs_runas_formatted?: string;
    rs_source_formatted?: string;
    rs_stage_formatted?: string;
    rs_status_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_RuleVersion_Select {
    createdby_guid: WebAttribute<rs_RuleVersion_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_RuleVersion_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_RuleVersion_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_RuleVersion_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_RuleVersion_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_RuleVersion_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_RuleVersion_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_RuleVersion_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_RuleVersion_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_basehash: WebAttribute<rs_RuleVersion_Select, { rs_basehash: string | null }, object>;
    rs_changenote: WebAttribute<rs_RuleVersion_Select, { rs_changenote: string | null }, object>;
    rs_compiledsnapshot: WebAttribute<rs_RuleVersion_Select, { rs_compiledsnapshot: string | null }, object>;
    rs_costlevel: WebAttribute<rs_RuleVersion_Select, { rs_costlevel: rs_ruleversion_rs_costlevel | null }, { rs_costlevel_formatted?: string }>;
    rs_derivedfromversionid_guid: WebAttribute<rs_RuleVersion_Select, { rs_derivedfromversionid_guid: string | null }, { rs_derivedfromversionid_formatted?: string }>;
    rs_event: WebAttribute<rs_RuleVersion_Select, { rs_event: rs_event | null }, { rs_event_formatted?: string }>;
    rs_failurepolicy: WebAttribute<rs_RuleVersion_Select, { rs_failurepolicy: rs_ruleversion_rs_failurepolicy | null }, { rs_failurepolicy_formatted?: string }>;
    rs_filteredcolumns: WebAttribute<rs_RuleVersion_Select, { rs_filteredcolumns: string | null }, object>;
    rs_loglevel: WebAttribute<rs_RuleVersion_Select, { rs_loglevel: rs_loglevel | null }, { rs_loglevel_formatted?: string }>;
    rs_name: WebAttribute<rs_RuleVersion_Select, { rs_name: string | null }, object>;
    rs_priority: WebAttribute<rs_RuleVersion_Select, { rs_priority: number | null }, object>;
    rs_publishedby_guid: WebAttribute<rs_RuleVersion_Select, { rs_publishedby_guid: string | null }, { rs_publishedby_formatted?: string }>;
    rs_publishedon: WebAttribute<rs_RuleVersion_Select, { rs_publishedon: Date | null }, { rs_publishedon_formatted?: string }>;
    rs_releaseid_guid: WebAttribute<rs_RuleVersion_Select, { rs_releaseid_guid: string | null }, { rs_releaseid_formatted?: string }>;
    rs_retrycount: WebAttribute<rs_RuleVersion_Select, { rs_retrycount: number | null }, object>;
    rs_retrydelayms: WebAttribute<rs_RuleVersion_Select, { rs_retrydelayms: number | null }, object>;
    rs_ruleid_guid: WebAttribute<rs_RuleVersion_Select, { rs_ruleid_guid: string | null }, { rs_ruleid_formatted?: string }>;
    rs_ruleversionid: WebAttribute<rs_RuleVersion_Select, { rs_ruleversionid: string | null }, object>;
    rs_runas: WebAttribute<rs_RuleVersion_Select, { rs_runas: rs_ruleversion_rs_runas | null }, { rs_runas_formatted?: string }>;
    rs_snapshothash: WebAttribute<rs_RuleVersion_Select, { rs_snapshothash: string | null }, object>;
    rs_source: WebAttribute<rs_RuleVersion_Select, { rs_source: rs_ruleversion_rs_source | null }, { rs_source_formatted?: string }>;
    rs_stage: WebAttribute<rs_RuleVersion_Select, { rs_stage: rs_stage | null }, { rs_stage_formatted?: string }>;
    rs_status: WebAttribute<rs_RuleVersion_Select, { rs_status: rs_ruleversion_rs_status | null }, { rs_status_formatted?: string }>;
    rs_stopprocessing: WebAttribute<rs_RuleVersion_Select, { rs_stopprocessing: boolean | null }, object>;
    rs_targettable: WebAttribute<rs_RuleVersion_Select, { rs_targettable: string | null }, object>;
    rs_versionnumber: WebAttribute<rs_RuleVersion_Select, { rs_versionnumber: number | null }, object>;
    statecode: WebAttribute<rs_RuleVersion_Select, { statecode: rs_ruleversion_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_RuleVersion_Select, { statuscode: rs_ruleversion_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_RuleVersion_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_RuleVersion_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_RuleVersion_Select, { versionnumber: number | null }, object>;
  }
  interface rs_RuleVersion_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_basehash: string;
    rs_changenote: string;
    rs_compiledsnapshot: string;
    rs_costlevel: rs_ruleversion_rs_costlevel;
    rs_derivedfromversionid_guid: XQW.Guid;
    rs_event: rs_event;
    rs_failurepolicy: rs_ruleversion_rs_failurepolicy;
    rs_filteredcolumns: string;
    rs_loglevel: rs_loglevel;
    rs_name: string;
    rs_priority: number;
    rs_publishedby_guid: XQW.Guid;
    rs_publishedon: Date;
    rs_releaseid_guid: XQW.Guid;
    rs_retrycount: number;
    rs_retrydelayms: number;
    rs_ruleid_guid: XQW.Guid;
    rs_ruleversionid: XQW.Guid;
    rs_runas: rs_ruleversion_rs_runas;
    rs_snapshothash: string;
    rs_source: rs_ruleversion_rs_source;
    rs_stage: rs_stage;
    rs_status: rs_ruleversion_rs_status;
    rs_stopprocessing: boolean;
    rs_targettable: string;
    rs_versionnumber: number;
    statecode: rs_ruleversion_statecode;
    statuscode: rs_ruleversion_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_RuleVersion_Expand {
    createdby: WebExpand<rs_RuleVersion_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_RuleVersion_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_RuleVersion_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_RuleVersion_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_DerivedFromVersionId: WebExpand<rs_RuleVersion_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_DerivedFromVersionId: rs_RuleVersion_Result }>;
    rs_PublishedBy: WebExpand<rs_RuleVersion_Expand, SystemUser_Select, SystemUser_Filter, { rs_PublishedBy: SystemUser_Result }>;
    rs_ReleaseId: WebExpand<rs_RuleVersion_Expand, rs_Release_Select, rs_Release_Filter, { rs_ReleaseId: rs_Release_Result }>;
    rs_RuleId: WebExpand<rs_RuleVersion_Expand, rs_Rule_Select, rs_Rule_Filter, { rs_RuleId: rs_Rule_Result }>;
    rs_rs_ruleversion_rs_action_rs_ruleversionid: WebExpand<rs_RuleVersion_Expand, rs_Action_Select, rs_Action_Filter, { rs_rs_ruleversion_rs_action_rs_ruleversionid: rs_Action_Result[] }>;
    rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid: WebExpand<rs_RuleVersion_Expand, rs_ConditionGroup_Select, rs_ConditionGroup_Filter, { rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid: rs_ConditionGroup_Result[] }>;
    rs_rs_ruleversion_rs_executionlog_rs_ruleversionid: WebExpand<rs_RuleVersion_Expand, rs_ExecutionLog_Select, rs_ExecutionLog_Filter, { rs_rs_ruleversion_rs_executionlog_rs_ruleversionid: rs_ExecutionLog_Result[] }>;
    rs_rs_ruleversion_rs_rule_rs_draftversionid: WebExpand<rs_RuleVersion_Expand, rs_Rule_Select, rs_Rule_Filter, { rs_rs_ruleversion_rs_rule_rs_draftversionid: rs_Rule_Result[] }>;
    rs_rs_ruleversion_rs_rule_rs_publishedversionid: WebExpand<rs_RuleVersion_Expand, rs_Rule_Select, rs_Rule_Filter, { rs_rs_ruleversion_rs_rule_rs_publishedversionid: rs_Rule_Result[] }>;
    rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid: WebExpand<rs_RuleVersion_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid: rs_RuleVersion_Result[] }>;
  }
  interface rs_RuleVersion_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_DerivedFromVersionId_bind$rs_ruleversions?: string | null;
    rs_PublishedBy_bind$systemusers?: string | null;
    rs_ReleaseId_bind$rs_releases?: string | null;
    rs_RuleId_bind$rs_rules?: string | null;
    rs_basehash?: string | null;
    rs_changenote?: string | null;
    rs_compiledsnapshot?: string | null;
    rs_costlevel?: rs_ruleversion_rs_costlevel | null;
    rs_event?: rs_event | null;
    rs_failurepolicy?: rs_ruleversion_rs_failurepolicy | null;
    rs_filteredcolumns?: string | null;
    rs_loglevel?: rs_loglevel | null;
    rs_name?: string | null;
    rs_priority?: number | null;
    rs_publishedon?: Date | null;
    rs_retrycount?: number | null;
    rs_retrydelayms?: number | null;
    rs_ruleversionid?: string | null;
    rs_runas?: rs_ruleversion_rs_runas | null;
    rs_snapshothash?: string | null;
    rs_source?: rs_ruleversion_rs_source | null;
    rs_stage?: rs_stage | null;
    rs_status?: rs_ruleversion_rs_status | null;
    rs_stopprocessing?: boolean | null;
    rs_targettable?: string | null;
    rs_versionnumber?: number | null;
    statuscode?: rs_ruleversion_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_RuleVersion_Update {
    rs_DerivedFromVersionId_bind$rs_ruleversions?: string | null;
    rs_PublishedBy_bind$systemusers?: string | null;
    rs_ReleaseId_bind$rs_releases?: string | null;
    rs_RuleId_bind$rs_rules?: string | null;
    rs_basehash?: string | null;
    rs_changenote?: string | null;
    rs_compiledsnapshot?: string | null;
    rs_costlevel?: rs_ruleversion_rs_costlevel | null;
    rs_event?: rs_event | null;
    rs_failurepolicy?: rs_ruleversion_rs_failurepolicy | null;
    rs_filteredcolumns?: string | null;
    rs_loglevel?: rs_loglevel | null;
    rs_name?: string | null;
    rs_priority?: number | null;
    rs_publishedon?: Date | null;
    rs_retrycount?: number | null;
    rs_retrydelayms?: number | null;
    rs_runas?: rs_ruleversion_rs_runas | null;
    rs_snapshothash?: string | null;
    rs_source?: rs_ruleversion_rs_source | null;
    rs_stage?: rs_stage | null;
    rs_status?: rs_ruleversion_rs_status | null;
    rs_stopprocessing?: boolean | null;
    rs_targettable?: string | null;
    rs_versionnumber?: number | null;
    statecode?: rs_ruleversion_statecode | null;
    statuscode?: rs_ruleversion_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_RuleVersion_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_DerivedFromVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
    rs_PublishedBy: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_ReleaseId: WebMappingRetrieve<rs_Release_Select, rs_Release_Expand, rs_Release_Filter, rs_Release_Fixed, rs_Release_Result, rs_Release_FormattedResult>;
    rs_RuleId: WebMappingRetrieve<rs_Rule_Select, rs_Rule_Expand, rs_Rule_Filter, rs_Rule_Fixed, rs_Rule_Result, rs_Rule_FormattedResult>;
  }
  interface rs_RuleVersion_RelatedMany {
    rs_rs_ruleversion_rs_action_rs_ruleversionid: WebMappingRetrieve<rs_Action_Select, rs_Action_Expand, rs_Action_Filter, rs_Action_Fixed, rs_Action_Result, rs_Action_FormattedResult>;
    rs_rs_ruleversion_rs_conditiongroup_rs_ruleversionid: WebMappingRetrieve<rs_ConditionGroup_Select, rs_ConditionGroup_Expand, rs_ConditionGroup_Filter, rs_ConditionGroup_Fixed, rs_ConditionGroup_Result, rs_ConditionGroup_FormattedResult>;
    rs_rs_ruleversion_rs_executionlog_rs_ruleversionid: WebMappingRetrieve<rs_ExecutionLog_Select, rs_ExecutionLog_Expand, rs_ExecutionLog_Filter, rs_ExecutionLog_Fixed, rs_ExecutionLog_Result, rs_ExecutionLog_FormattedResult>;
    rs_rs_ruleversion_rs_rule_rs_draftversionid: WebMappingRetrieve<rs_Rule_Select, rs_Rule_Expand, rs_Rule_Filter, rs_Rule_Fixed, rs_Rule_Result, rs_Rule_FormattedResult>;
    rs_rs_ruleversion_rs_rule_rs_publishedversionid: WebMappingRetrieve<rs_Rule_Select, rs_Rule_Expand, rs_Rule_Filter, rs_Rule_Fixed, rs_Rule_Result, rs_Rule_FormattedResult>;
    rs_rs_ruleversion_rs_ruleversion_rs_derivedfromversionid: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_ruleversions: WebMappingRetrieve<XDT.rs_RuleVersion_Select, XDT.rs_RuleVersion_Expand, XDT.rs_RuleVersion_Filter, XDT.rs_RuleVersion_Fixed, XDT.rs_RuleVersion_Result, XDT.rs_RuleVersion_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_ruleversions: WebMappingRelated<XDT.rs_RuleVersion_RelatedOne, XDT.rs_RuleVersion_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_ruleversions: WebMappingCUDA<XDT.rs_RuleVersion_Create, XDT.rs_RuleVersion_Update, XDT.rs_RuleVersion_Select>;
}
