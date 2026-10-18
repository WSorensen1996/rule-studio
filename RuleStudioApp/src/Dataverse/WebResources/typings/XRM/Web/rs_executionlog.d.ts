declare namespace XDT {
  interface rs_ExecutionLog_Fixed {
    rs_executionlogid: string;
  }
  interface rs_ExecutionLog_Result extends rs_ExecutionLog_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_RuleId?: rs_Rule_Result | null;
    rs_RuleVersionId?: rs_RuleVersion_Result | null;
    rs_adminflag: boolean | null;
    rs_attempts: number | null;
    rs_correlationid: string | null;
    rs_durationms: number | null;
    rs_errormessage: string | null;
    rs_event: rs_event | null;
    rs_istest: boolean | null;
    rs_loglevel: rs_loglevel | null;
    rs_name: string | null;
    rs_outcome: rs_executionlog_rs_outcome | null;
    rs_recordid: string | null;
    rs_ruleid_guid: string | null;
    rs_ruleversionid_guid: string | null;
    rs_stage: rs_stage | null;
    rs_targettable: string | null;
    rs_trace: string | null;
    statecode: rs_executionlog_statecode | null;
    statuscode: rs_executionlog_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_ExecutionLog_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_event_formatted?: string;
    rs_loglevel_formatted?: string;
    rs_outcome_formatted?: string;
    rs_ruleid_formatted?: string;
    rs_ruleversionid_formatted?: string;
    rs_stage_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_ExecutionLog_Select {
    createdby_guid: WebAttribute<rs_ExecutionLog_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_ExecutionLog_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_ExecutionLog_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_ExecutionLog_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_ExecutionLog_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_ExecutionLog_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_ExecutionLog_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_ExecutionLog_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_ExecutionLog_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_adminflag: WebAttribute<rs_ExecutionLog_Select, { rs_adminflag: boolean | null }, object>;
    rs_attempts: WebAttribute<rs_ExecutionLog_Select, { rs_attempts: number | null }, object>;
    rs_correlationid: WebAttribute<rs_ExecutionLog_Select, { rs_correlationid: string | null }, object>;
    rs_durationms: WebAttribute<rs_ExecutionLog_Select, { rs_durationms: number | null }, object>;
    rs_errormessage: WebAttribute<rs_ExecutionLog_Select, { rs_errormessage: string | null }, object>;
    rs_event: WebAttribute<rs_ExecutionLog_Select, { rs_event: rs_event | null }, { rs_event_formatted?: string }>;
    rs_executionlogid: WebAttribute<rs_ExecutionLog_Select, { rs_executionlogid: string | null }, object>;
    rs_istest: WebAttribute<rs_ExecutionLog_Select, { rs_istest: boolean | null }, object>;
    rs_loglevel: WebAttribute<rs_ExecutionLog_Select, { rs_loglevel: rs_loglevel | null }, { rs_loglevel_formatted?: string }>;
    rs_name: WebAttribute<rs_ExecutionLog_Select, { rs_name: string | null }, object>;
    rs_outcome: WebAttribute<rs_ExecutionLog_Select, { rs_outcome: rs_executionlog_rs_outcome | null }, { rs_outcome_formatted?: string }>;
    rs_recordid: WebAttribute<rs_ExecutionLog_Select, { rs_recordid: string | null }, object>;
    rs_ruleid_guid: WebAttribute<rs_ExecutionLog_Select, { rs_ruleid_guid: string | null }, { rs_ruleid_formatted?: string }>;
    rs_ruleversionid_guid: WebAttribute<rs_ExecutionLog_Select, { rs_ruleversionid_guid: string | null }, { rs_ruleversionid_formatted?: string }>;
    rs_stage: WebAttribute<rs_ExecutionLog_Select, { rs_stage: rs_stage | null }, { rs_stage_formatted?: string }>;
    rs_targettable: WebAttribute<rs_ExecutionLog_Select, { rs_targettable: string | null }, object>;
    rs_trace: WebAttribute<rs_ExecutionLog_Select, { rs_trace: string | null }, object>;
    statecode: WebAttribute<rs_ExecutionLog_Select, { statecode: rs_executionlog_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_ExecutionLog_Select, { statuscode: rs_executionlog_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_ExecutionLog_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_ExecutionLog_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_ExecutionLog_Select, { versionnumber: number | null }, object>;
  }
  interface rs_ExecutionLog_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_adminflag: boolean;
    rs_attempts: number;
    rs_correlationid: string;
    rs_durationms: number;
    rs_errormessage: string;
    rs_event: rs_event;
    rs_executionlogid: XQW.Guid;
    rs_istest: boolean;
    rs_loglevel: rs_loglevel;
    rs_name: string;
    rs_outcome: rs_executionlog_rs_outcome;
    rs_recordid: string;
    rs_ruleid_guid: XQW.Guid;
    rs_ruleversionid_guid: XQW.Guid;
    rs_stage: rs_stage;
    rs_targettable: string;
    rs_trace: string;
    statecode: rs_executionlog_statecode;
    statuscode: rs_executionlog_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_ExecutionLog_Expand {
    createdby: WebExpand<rs_ExecutionLog_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_ExecutionLog_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_ExecutionLog_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_ExecutionLog_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_RuleId: WebExpand<rs_ExecutionLog_Expand, rs_Rule_Select, rs_Rule_Filter, { rs_RuleId: rs_Rule_Result }>;
    rs_RuleVersionId: WebExpand<rs_ExecutionLog_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_RuleVersionId: rs_RuleVersion_Result }>;
  }
  interface rs_ExecutionLog_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_RuleId_bind$rs_rules?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_adminflag?: boolean | null;
    rs_attempts?: number | null;
    rs_correlationid?: string | null;
    rs_durationms?: number | null;
    rs_errormessage?: string | null;
    rs_event?: rs_event | null;
    rs_executionlogid?: string | null;
    rs_istest?: boolean | null;
    rs_loglevel?: rs_loglevel | null;
    rs_name?: string | null;
    rs_outcome?: rs_executionlog_rs_outcome | null;
    rs_recordid?: string | null;
    rs_stage?: rs_stage | null;
    rs_targettable?: string | null;
    rs_trace?: string | null;
    statuscode?: rs_executionlog_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ExecutionLog_Update {
    rs_RuleId_bind$rs_rules?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_adminflag?: boolean | null;
    rs_attempts?: number | null;
    rs_correlationid?: string | null;
    rs_durationms?: number | null;
    rs_errormessage?: string | null;
    rs_event?: rs_event | null;
    rs_istest?: boolean | null;
    rs_loglevel?: rs_loglevel | null;
    rs_name?: string | null;
    rs_outcome?: rs_executionlog_rs_outcome | null;
    rs_recordid?: string | null;
    rs_stage?: rs_stage | null;
    rs_targettable?: string | null;
    rs_trace?: string | null;
    statecode?: rs_executionlog_statecode | null;
    statuscode?: rs_executionlog_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ExecutionLog_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_RuleId: WebMappingRetrieve<rs_Rule_Select, rs_Rule_Expand, rs_Rule_Filter, rs_Rule_Fixed, rs_Rule_Result, rs_Rule_FormattedResult>;
    rs_RuleVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
  interface rs_ExecutionLog_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  rs_executionlogs: WebMappingRetrieve<XDT.rs_ExecutionLog_Select, XDT.rs_ExecutionLog_Expand, XDT.rs_ExecutionLog_Filter, XDT.rs_ExecutionLog_Fixed, XDT.rs_ExecutionLog_Result, XDT.rs_ExecutionLog_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_executionlogs: WebMappingRelated<XDT.rs_ExecutionLog_RelatedOne, XDT.rs_ExecutionLog_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_executionlogs: WebMappingCUDA<XDT.rs_ExecutionLog_Create, XDT.rs_ExecutionLog_Update, XDT.rs_ExecutionLog_Select>;
}
