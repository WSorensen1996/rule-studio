declare namespace XDT {
  interface rs_Action_Fixed {
    rs_actionid: string;
  }
  interface rs_Action_Result extends rs_Action_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_CustomFunctionId?: rs_CustomFunction_Result | null;
    rs_LookupDefinitionId?: rs_LookupDefinition_Result | null;
    rs_RuleVersionId?: rs_RuleVersion_Result | null;
    rs_actiontype: rs_action_rs_actiontype | null;
    rs_branch: rs_action_rs_branch | null;
    rs_customfunctionid_guid: string | null;
    rs_lookupdefinitionid_guid: string | null;
    rs_message: string | null;
    rs_messagelevel: rs_messagelevel | null;
    rs_name: string | null;
    rs_order: number | null;
    rs_rs_action_rs_actioninputmapping_rs_actionid?: rs_ActionInputMapping_Result[] | null;
    rs_ruleversionid_guid: string | null;
    rs_targetfield: string | null;
    rs_value: string | null;
    rs_valuepath: string | null;
    rs_valuesourcetype: rs_valuesourcetype | null;
    statecode: rs_action_statecode | null;
    statuscode: rs_action_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_Action_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_actiontype_formatted?: string;
    rs_branch_formatted?: string;
    rs_customfunctionid_formatted?: string;
    rs_lookupdefinitionid_formatted?: string;
    rs_messagelevel_formatted?: string;
    rs_ruleversionid_formatted?: string;
    rs_valuesourcetype_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_Action_Select {
    createdby_guid: WebAttribute<rs_Action_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_Action_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_Action_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_Action_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_Action_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_Action_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_Action_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_Action_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_Action_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_actionid: WebAttribute<rs_Action_Select, { rs_actionid: string | null }, object>;
    rs_actiontype: WebAttribute<rs_Action_Select, { rs_actiontype: rs_action_rs_actiontype | null }, { rs_actiontype_formatted?: string }>;
    rs_branch: WebAttribute<rs_Action_Select, { rs_branch: rs_action_rs_branch | null }, { rs_branch_formatted?: string }>;
    rs_customfunctionid_guid: WebAttribute<rs_Action_Select, { rs_customfunctionid_guid: string | null }, { rs_customfunctionid_formatted?: string }>;
    rs_lookupdefinitionid_guid: WebAttribute<rs_Action_Select, { rs_lookupdefinitionid_guid: string | null }, { rs_lookupdefinitionid_formatted?: string }>;
    rs_message: WebAttribute<rs_Action_Select, { rs_message: string | null }, object>;
    rs_messagelevel: WebAttribute<rs_Action_Select, { rs_messagelevel: rs_messagelevel | null }, { rs_messagelevel_formatted?: string }>;
    rs_name: WebAttribute<rs_Action_Select, { rs_name: string | null }, object>;
    rs_order: WebAttribute<rs_Action_Select, { rs_order: number | null }, object>;
    rs_ruleversionid_guid: WebAttribute<rs_Action_Select, { rs_ruleversionid_guid: string | null }, { rs_ruleversionid_formatted?: string }>;
    rs_targetfield: WebAttribute<rs_Action_Select, { rs_targetfield: string | null }, object>;
    rs_value: WebAttribute<rs_Action_Select, { rs_value: string | null }, object>;
    rs_valuepath: WebAttribute<rs_Action_Select, { rs_valuepath: string | null }, object>;
    rs_valuesourcetype: WebAttribute<rs_Action_Select, { rs_valuesourcetype: rs_valuesourcetype | null }, { rs_valuesourcetype_formatted?: string }>;
    statecode: WebAttribute<rs_Action_Select, { statecode: rs_action_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_Action_Select, { statuscode: rs_action_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_Action_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_Action_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_Action_Select, { versionnumber: number | null }, object>;
  }
  interface rs_Action_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_actionid: XQW.Guid;
    rs_actiontype: rs_action_rs_actiontype;
    rs_branch: rs_action_rs_branch;
    rs_customfunctionid_guid: XQW.Guid;
    rs_lookupdefinitionid_guid: XQW.Guid;
    rs_message: string;
    rs_messagelevel: rs_messagelevel;
    rs_name: string;
    rs_order: number;
    rs_ruleversionid_guid: XQW.Guid;
    rs_targetfield: string;
    rs_value: string;
    rs_valuepath: string;
    rs_valuesourcetype: rs_valuesourcetype;
    statecode: rs_action_statecode;
    statuscode: rs_action_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_Action_Expand {
    createdby: WebExpand<rs_Action_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_Action_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_Action_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_Action_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_CustomFunctionId: WebExpand<rs_Action_Expand, rs_CustomFunction_Select, rs_CustomFunction_Filter, { rs_CustomFunctionId: rs_CustomFunction_Result }>;
    rs_LookupDefinitionId: WebExpand<rs_Action_Expand, rs_LookupDefinition_Select, rs_LookupDefinition_Filter, { rs_LookupDefinitionId: rs_LookupDefinition_Result }>;
    rs_RuleVersionId: WebExpand<rs_Action_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_RuleVersionId: rs_RuleVersion_Result }>;
    rs_rs_action_rs_actioninputmapping_rs_actionid: WebExpand<rs_Action_Expand, rs_ActionInputMapping_Select, rs_ActionInputMapping_Filter, { rs_rs_action_rs_actioninputmapping_rs_actionid: rs_ActionInputMapping_Result[] }>;
  }
  interface rs_Action_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_CustomFunctionId_bind$rs_customfunctions?: string | null;
    rs_LookupDefinitionId_bind$rs_lookupdefinitions?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_actionid?: string | null;
    rs_actiontype?: rs_action_rs_actiontype | null;
    rs_branch?: rs_action_rs_branch | null;
    rs_message?: string | null;
    rs_messagelevel?: rs_messagelevel | null;
    rs_name?: string | null;
    rs_order?: number | null;
    rs_targetfield?: string | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statuscode?: rs_action_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Action_Update {
    rs_CustomFunctionId_bind$rs_customfunctions?: string | null;
    rs_LookupDefinitionId_bind$rs_lookupdefinitions?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_actiontype?: rs_action_rs_actiontype | null;
    rs_branch?: rs_action_rs_branch | null;
    rs_message?: string | null;
    rs_messagelevel?: rs_messagelevel | null;
    rs_name?: string | null;
    rs_order?: number | null;
    rs_targetfield?: string | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statecode?: rs_action_statecode | null;
    statuscode?: rs_action_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Action_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_CustomFunctionId: WebMappingRetrieve<rs_CustomFunction_Select, rs_CustomFunction_Expand, rs_CustomFunction_Filter, rs_CustomFunction_Fixed, rs_CustomFunction_Result, rs_CustomFunction_FormattedResult>;
    rs_LookupDefinitionId: WebMappingRetrieve<rs_LookupDefinition_Select, rs_LookupDefinition_Expand, rs_LookupDefinition_Filter, rs_LookupDefinition_Fixed, rs_LookupDefinition_Result, rs_LookupDefinition_FormattedResult>;
    rs_RuleVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
  interface rs_Action_RelatedMany {
    rs_rs_action_rs_actioninputmapping_rs_actionid: WebMappingRetrieve<rs_ActionInputMapping_Select, rs_ActionInputMapping_Expand, rs_ActionInputMapping_Filter, rs_ActionInputMapping_Fixed, rs_ActionInputMapping_Result, rs_ActionInputMapping_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_actions: WebMappingRetrieve<XDT.rs_Action_Select, XDT.rs_Action_Expand, XDT.rs_Action_Filter, XDT.rs_Action_Fixed, XDT.rs_Action_Result, XDT.rs_Action_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_actions: WebMappingRelated<XDT.rs_Action_RelatedOne, XDT.rs_Action_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_actions: WebMappingCUDA<XDT.rs_Action_Create, XDT.rs_Action_Update, XDT.rs_Action_Select>;
}
