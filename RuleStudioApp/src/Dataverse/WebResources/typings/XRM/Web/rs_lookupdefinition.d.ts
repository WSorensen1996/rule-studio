declare namespace XDT {
  interface rs_LookupDefinition_Fixed {
    rs_lookupdefinitionid: string;
  }
  interface rs_LookupDefinition_Result extends rs_LookupDefinition_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_defaultvalue: string | null;
    rs_description: string | null;
    rs_lookuptable: string | null;
    rs_matchcriteria: string | null;
    rs_multimatchpolicy: rs_lookupdefinition_rs_multimatchpolicy | null;
    rs_name: string | null;
    rs_nomatchpolicy: rs_lookupdefinition_rs_nomatchpolicy | null;
    rs_returncolumn: string | null;
    rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid?: rs_Action_Result[] | null;
    rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid?: rs_Condition_Result[] | null;
    rs_sortcolumn: string | null;
    statecode: rs_lookupdefinition_statecode | null;
    statuscode: rs_lookupdefinition_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_LookupDefinition_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_multimatchpolicy_formatted?: string;
    rs_nomatchpolicy_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_LookupDefinition_Select {
    createdby_guid: WebAttribute<rs_LookupDefinition_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_LookupDefinition_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_LookupDefinition_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_LookupDefinition_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_LookupDefinition_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_LookupDefinition_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_LookupDefinition_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_LookupDefinition_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_LookupDefinition_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_defaultvalue: WebAttribute<rs_LookupDefinition_Select, { rs_defaultvalue: string | null }, object>;
    rs_description: WebAttribute<rs_LookupDefinition_Select, { rs_description: string | null }, object>;
    rs_lookupdefinitionid: WebAttribute<rs_LookupDefinition_Select, { rs_lookupdefinitionid: string | null }, object>;
    rs_lookuptable: WebAttribute<rs_LookupDefinition_Select, { rs_lookuptable: string | null }, object>;
    rs_matchcriteria: WebAttribute<rs_LookupDefinition_Select, { rs_matchcriteria: string | null }, object>;
    rs_multimatchpolicy: WebAttribute<rs_LookupDefinition_Select, { rs_multimatchpolicy: rs_lookupdefinition_rs_multimatchpolicy | null }, { rs_multimatchpolicy_formatted?: string }>;
    rs_name: WebAttribute<rs_LookupDefinition_Select, { rs_name: string | null }, object>;
    rs_nomatchpolicy: WebAttribute<rs_LookupDefinition_Select, { rs_nomatchpolicy: rs_lookupdefinition_rs_nomatchpolicy | null }, { rs_nomatchpolicy_formatted?: string }>;
    rs_returncolumn: WebAttribute<rs_LookupDefinition_Select, { rs_returncolumn: string | null }, object>;
    rs_sortcolumn: WebAttribute<rs_LookupDefinition_Select, { rs_sortcolumn: string | null }, object>;
    statecode: WebAttribute<rs_LookupDefinition_Select, { statecode: rs_lookupdefinition_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_LookupDefinition_Select, { statuscode: rs_lookupdefinition_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_LookupDefinition_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_LookupDefinition_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_LookupDefinition_Select, { versionnumber: number | null }, object>;
  }
  interface rs_LookupDefinition_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_defaultvalue: string;
    rs_description: string;
    rs_lookupdefinitionid: XQW.Guid;
    rs_lookuptable: string;
    rs_matchcriteria: string;
    rs_multimatchpolicy: rs_lookupdefinition_rs_multimatchpolicy;
    rs_name: string;
    rs_nomatchpolicy: rs_lookupdefinition_rs_nomatchpolicy;
    rs_returncolumn: string;
    rs_sortcolumn: string;
    statecode: rs_lookupdefinition_statecode;
    statuscode: rs_lookupdefinition_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_LookupDefinition_Expand {
    createdby: WebExpand<rs_LookupDefinition_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_LookupDefinition_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_LookupDefinition_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_LookupDefinition_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid: WebExpand<rs_LookupDefinition_Expand, rs_Action_Select, rs_Action_Filter, { rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid: rs_Action_Result[] }>;
    rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid: WebExpand<rs_LookupDefinition_Expand, rs_Condition_Select, rs_Condition_Filter, { rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid: rs_Condition_Result[] }>;
  }
  interface rs_LookupDefinition_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_defaultvalue?: string | null;
    rs_description?: string | null;
    rs_lookupdefinitionid?: string | null;
    rs_lookuptable?: string | null;
    rs_matchcriteria?: string | null;
    rs_multimatchpolicy?: rs_lookupdefinition_rs_multimatchpolicy | null;
    rs_name?: string | null;
    rs_nomatchpolicy?: rs_lookupdefinition_rs_nomatchpolicy | null;
    rs_returncolumn?: string | null;
    rs_sortcolumn?: string | null;
    statuscode?: rs_lookupdefinition_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_LookupDefinition_Update {
    rs_defaultvalue?: string | null;
    rs_description?: string | null;
    rs_lookuptable?: string | null;
    rs_matchcriteria?: string | null;
    rs_multimatchpolicy?: rs_lookupdefinition_rs_multimatchpolicy | null;
    rs_name?: string | null;
    rs_nomatchpolicy?: rs_lookupdefinition_rs_nomatchpolicy | null;
    rs_returncolumn?: string | null;
    rs_sortcolumn?: string | null;
    statecode?: rs_lookupdefinition_statecode | null;
    statuscode?: rs_lookupdefinition_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_LookupDefinition_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
  }
  interface rs_LookupDefinition_RelatedMany {
    rs_rs_lookupdefinition_rs_action_rs_lookupdefinitionid: WebMappingRetrieve<rs_Action_Select, rs_Action_Expand, rs_Action_Filter, rs_Action_Fixed, rs_Action_Result, rs_Action_FormattedResult>;
    rs_rs_lookupdefinition_rs_condition_rs_lookupdefinitionid: WebMappingRetrieve<rs_Condition_Select, rs_Condition_Expand, rs_Condition_Filter, rs_Condition_Fixed, rs_Condition_Result, rs_Condition_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_lookupdefinitions: WebMappingRetrieve<XDT.rs_LookupDefinition_Select, XDT.rs_LookupDefinition_Expand, XDT.rs_LookupDefinition_Filter, XDT.rs_LookupDefinition_Fixed, XDT.rs_LookupDefinition_Result, XDT.rs_LookupDefinition_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_lookupdefinitions: WebMappingRelated<XDT.rs_LookupDefinition_RelatedOne, XDT.rs_LookupDefinition_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_lookupdefinitions: WebMappingCUDA<XDT.rs_LookupDefinition_Create, XDT.rs_LookupDefinition_Update, XDT.rs_LookupDefinition_Select>;
}
