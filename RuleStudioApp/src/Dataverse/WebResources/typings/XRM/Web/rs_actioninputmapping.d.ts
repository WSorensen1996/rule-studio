declare namespace XDT {
  interface rs_ActionInputMapping_Fixed {
    rs_actioninputmappingid: string;
  }
  interface rs_ActionInputMapping_Result extends rs_ActionInputMapping_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_ActionId?: rs_Action_Result | null;
    rs_ConditionId?: rs_Condition_Result | null;
    rs_actionid_guid: string | null;
    rs_conditionid_guid: string | null;
    rs_name: string | null;
    rs_parametername: string | null;
    rs_value: string | null;
    rs_valuepath: string | null;
    rs_valuesourcetype: rs_valuesourcetype | null;
    statecode: rs_actioninputmapping_statecode | null;
    statuscode: rs_actioninputmapping_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_ActionInputMapping_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_actionid_formatted?: string;
    rs_conditionid_formatted?: string;
    rs_valuesourcetype_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_ActionInputMapping_Select {
    createdby_guid: WebAttribute<rs_ActionInputMapping_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_ActionInputMapping_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_ActionInputMapping_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_ActionInputMapping_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_ActionInputMapping_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_ActionInputMapping_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_ActionInputMapping_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_ActionInputMapping_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_ActionInputMapping_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_actionid_guid: WebAttribute<rs_ActionInputMapping_Select, { rs_actionid_guid: string | null }, { rs_actionid_formatted?: string }>;
    rs_actioninputmappingid: WebAttribute<rs_ActionInputMapping_Select, { rs_actioninputmappingid: string | null }, object>;
    rs_conditionid_guid: WebAttribute<rs_ActionInputMapping_Select, { rs_conditionid_guid: string | null }, { rs_conditionid_formatted?: string }>;
    rs_name: WebAttribute<rs_ActionInputMapping_Select, { rs_name: string | null }, object>;
    rs_parametername: WebAttribute<rs_ActionInputMapping_Select, { rs_parametername: string | null }, object>;
    rs_value: WebAttribute<rs_ActionInputMapping_Select, { rs_value: string | null }, object>;
    rs_valuepath: WebAttribute<rs_ActionInputMapping_Select, { rs_valuepath: string | null }, object>;
    rs_valuesourcetype: WebAttribute<rs_ActionInputMapping_Select, { rs_valuesourcetype: rs_valuesourcetype | null }, { rs_valuesourcetype_formatted?: string }>;
    statecode: WebAttribute<rs_ActionInputMapping_Select, { statecode: rs_actioninputmapping_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_ActionInputMapping_Select, { statuscode: rs_actioninputmapping_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_ActionInputMapping_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_ActionInputMapping_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_ActionInputMapping_Select, { versionnumber: number | null }, object>;
  }
  interface rs_ActionInputMapping_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_actionid_guid: XQW.Guid;
    rs_actioninputmappingid: XQW.Guid;
    rs_conditionid_guid: XQW.Guid;
    rs_name: string;
    rs_parametername: string;
    rs_value: string;
    rs_valuepath: string;
    rs_valuesourcetype: rs_valuesourcetype;
    statecode: rs_actioninputmapping_statecode;
    statuscode: rs_actioninputmapping_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_ActionInputMapping_Expand {
    createdby: WebExpand<rs_ActionInputMapping_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_ActionInputMapping_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_ActionInputMapping_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_ActionInputMapping_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_ActionId: WebExpand<rs_ActionInputMapping_Expand, rs_Action_Select, rs_Action_Filter, { rs_ActionId: rs_Action_Result }>;
    rs_ConditionId: WebExpand<rs_ActionInputMapping_Expand, rs_Condition_Select, rs_Condition_Filter, { rs_ConditionId: rs_Condition_Result }>;
  }
  interface rs_ActionInputMapping_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_ActionId_bind$rs_actions?: string | null;
    rs_ConditionId_bind$rs_conditions?: string | null;
    rs_actioninputmappingid?: string | null;
    rs_name?: string | null;
    rs_parametername?: string | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statuscode?: rs_actioninputmapping_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ActionInputMapping_Update {
    rs_ActionId_bind$rs_actions?: string | null;
    rs_ConditionId_bind$rs_conditions?: string | null;
    rs_name?: string | null;
    rs_parametername?: string | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statecode?: rs_actioninputmapping_statecode | null;
    statuscode?: rs_actioninputmapping_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ActionInputMapping_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_ActionId: WebMappingRetrieve<rs_Action_Select, rs_Action_Expand, rs_Action_Filter, rs_Action_Fixed, rs_Action_Result, rs_Action_FormattedResult>;
    rs_ConditionId: WebMappingRetrieve<rs_Condition_Select, rs_Condition_Expand, rs_Condition_Filter, rs_Condition_Fixed, rs_Condition_Result, rs_Condition_FormattedResult>;
  }
  interface rs_ActionInputMapping_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  rs_actioninputmappings: WebMappingRetrieve<XDT.rs_ActionInputMapping_Select, XDT.rs_ActionInputMapping_Expand, XDT.rs_ActionInputMapping_Filter, XDT.rs_ActionInputMapping_Fixed, XDT.rs_ActionInputMapping_Result, XDT.rs_ActionInputMapping_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_actioninputmappings: WebMappingRelated<XDT.rs_ActionInputMapping_RelatedOne, XDT.rs_ActionInputMapping_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_actioninputmappings: WebMappingCUDA<XDT.rs_ActionInputMapping_Create, XDT.rs_ActionInputMapping_Update, XDT.rs_ActionInputMapping_Select>;
}
