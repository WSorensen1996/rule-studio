declare namespace XDT {
  interface rs_Condition_Fixed {
    rs_conditionid: string;
  }
  interface rs_Condition_Result extends rs_Condition_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_ConditionGroupId?: rs_ConditionGroup_Result | null;
    rs_CustomFunctionId?: rs_CustomFunction_Result | null;
    rs_LookupDefinitionId?: rs_LookupDefinition_Result | null;
    rs_aggregate: rs_condition_rs_aggregate | null;
    rs_conditiongroupid_guid: string | null;
    rs_customfunctionid_guid: string | null;
    rs_fieldpath: string | null;
    rs_lookupdefinitionid_guid: string | null;
    rs_name: string | null;
    rs_operator: rs_condition_rs_operator | null;
    rs_order: number | null;
    rs_rs_condition_rs_actioninputmapping_rs_conditionid?: rs_ActionInputMapping_Result[] | null;
    rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid?: rs_ConditionGroup_Result[] | null;
    rs_value: string | null;
    rs_valuepath: string | null;
    rs_valuesourcetype: rs_valuesourcetype | null;
    statecode: rs_condition_statecode | null;
    statuscode: rs_condition_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_Condition_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_aggregate_formatted?: string;
    rs_conditiongroupid_formatted?: string;
    rs_customfunctionid_formatted?: string;
    rs_lookupdefinitionid_formatted?: string;
    rs_operator_formatted?: string;
    rs_valuesourcetype_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_Condition_Select {
    createdby_guid: WebAttribute<rs_Condition_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_Condition_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_Condition_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_Condition_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_Condition_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_Condition_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_Condition_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_Condition_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_Condition_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_aggregate: WebAttribute<rs_Condition_Select, { rs_aggregate: rs_condition_rs_aggregate | null }, { rs_aggregate_formatted?: string }>;
    rs_conditiongroupid_guid: WebAttribute<rs_Condition_Select, { rs_conditiongroupid_guid: string | null }, { rs_conditiongroupid_formatted?: string }>;
    rs_conditionid: WebAttribute<rs_Condition_Select, { rs_conditionid: string | null }, object>;
    rs_customfunctionid_guid: WebAttribute<rs_Condition_Select, { rs_customfunctionid_guid: string | null }, { rs_customfunctionid_formatted?: string }>;
    rs_fieldpath: WebAttribute<rs_Condition_Select, { rs_fieldpath: string | null }, object>;
    rs_lookupdefinitionid_guid: WebAttribute<rs_Condition_Select, { rs_lookupdefinitionid_guid: string | null }, { rs_lookupdefinitionid_formatted?: string }>;
    rs_name: WebAttribute<rs_Condition_Select, { rs_name: string | null }, object>;
    rs_operator: WebAttribute<rs_Condition_Select, { rs_operator: rs_condition_rs_operator | null }, { rs_operator_formatted?: string }>;
    rs_order: WebAttribute<rs_Condition_Select, { rs_order: number | null }, object>;
    rs_value: WebAttribute<rs_Condition_Select, { rs_value: string | null }, object>;
    rs_valuepath: WebAttribute<rs_Condition_Select, { rs_valuepath: string | null }, object>;
    rs_valuesourcetype: WebAttribute<rs_Condition_Select, { rs_valuesourcetype: rs_valuesourcetype | null }, { rs_valuesourcetype_formatted?: string }>;
    statecode: WebAttribute<rs_Condition_Select, { statecode: rs_condition_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_Condition_Select, { statuscode: rs_condition_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_Condition_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_Condition_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_Condition_Select, { versionnumber: number | null }, object>;
  }
  interface rs_Condition_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_aggregate: rs_condition_rs_aggregate;
    rs_conditiongroupid_guid: XQW.Guid;
    rs_conditionid: XQW.Guid;
    rs_customfunctionid_guid: XQW.Guid;
    rs_fieldpath: string;
    rs_lookupdefinitionid_guid: XQW.Guid;
    rs_name: string;
    rs_operator: rs_condition_rs_operator;
    rs_order: number;
    rs_value: string;
    rs_valuepath: string;
    rs_valuesourcetype: rs_valuesourcetype;
    statecode: rs_condition_statecode;
    statuscode: rs_condition_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_Condition_Expand {
    createdby: WebExpand<rs_Condition_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_Condition_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_Condition_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_Condition_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_ConditionGroupId: WebExpand<rs_Condition_Expand, rs_ConditionGroup_Select, rs_ConditionGroup_Filter, { rs_ConditionGroupId: rs_ConditionGroup_Result }>;
    rs_CustomFunctionId: WebExpand<rs_Condition_Expand, rs_CustomFunction_Select, rs_CustomFunction_Filter, { rs_CustomFunctionId: rs_CustomFunction_Result }>;
    rs_LookupDefinitionId: WebExpand<rs_Condition_Expand, rs_LookupDefinition_Select, rs_LookupDefinition_Filter, { rs_LookupDefinitionId: rs_LookupDefinition_Result }>;
    rs_rs_condition_rs_actioninputmapping_rs_conditionid: WebExpand<rs_Condition_Expand, rs_ActionInputMapping_Select, rs_ActionInputMapping_Filter, { rs_rs_condition_rs_actioninputmapping_rs_conditionid: rs_ActionInputMapping_Result[] }>;
    rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid: WebExpand<rs_Condition_Expand, rs_ConditionGroup_Select, rs_ConditionGroup_Filter, { rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid: rs_ConditionGroup_Result[] }>;
  }
  interface rs_Condition_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_ConditionGroupId_bind$rs_conditiongroups?: string | null;
    rs_CustomFunctionId_bind$rs_customfunctions?: string | null;
    rs_LookupDefinitionId_bind$rs_lookupdefinitions?: string | null;
    rs_aggregate?: rs_condition_rs_aggregate | null;
    rs_conditionid?: string | null;
    rs_fieldpath?: string | null;
    rs_name?: string | null;
    rs_operator?: rs_condition_rs_operator | null;
    rs_order?: number | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statuscode?: rs_condition_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Condition_Update {
    rs_ConditionGroupId_bind$rs_conditiongroups?: string | null;
    rs_CustomFunctionId_bind$rs_customfunctions?: string | null;
    rs_LookupDefinitionId_bind$rs_lookupdefinitions?: string | null;
    rs_aggregate?: rs_condition_rs_aggregate | null;
    rs_fieldpath?: string | null;
    rs_name?: string | null;
    rs_operator?: rs_condition_rs_operator | null;
    rs_order?: number | null;
    rs_value?: string | null;
    rs_valuepath?: string | null;
    rs_valuesourcetype?: rs_valuesourcetype | null;
    statecode?: rs_condition_statecode | null;
    statuscode?: rs_condition_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_Condition_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_ConditionGroupId: WebMappingRetrieve<rs_ConditionGroup_Select, rs_ConditionGroup_Expand, rs_ConditionGroup_Filter, rs_ConditionGroup_Fixed, rs_ConditionGroup_Result, rs_ConditionGroup_FormattedResult>;
    rs_CustomFunctionId: WebMappingRetrieve<rs_CustomFunction_Select, rs_CustomFunction_Expand, rs_CustomFunction_Filter, rs_CustomFunction_Fixed, rs_CustomFunction_Result, rs_CustomFunction_FormattedResult>;
    rs_LookupDefinitionId: WebMappingRetrieve<rs_LookupDefinition_Select, rs_LookupDefinition_Expand, rs_LookupDefinition_Filter, rs_LookupDefinition_Fixed, rs_LookupDefinition_Result, rs_LookupDefinition_FormattedResult>;
  }
  interface rs_Condition_RelatedMany {
    rs_rs_condition_rs_actioninputmapping_rs_conditionid: WebMappingRetrieve<rs_ActionInputMapping_Select, rs_ActionInputMapping_Expand, rs_ActionInputMapping_Filter, rs_ActionInputMapping_Fixed, rs_ActionInputMapping_Result, rs_ActionInputMapping_FormattedResult>;
    rs_rs_condition_rs_conditiongroup_rs_aggregatescopeconditionid: WebMappingRetrieve<rs_ConditionGroup_Select, rs_ConditionGroup_Expand, rs_ConditionGroup_Filter, rs_ConditionGroup_Fixed, rs_ConditionGroup_Result, rs_ConditionGroup_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_conditions: WebMappingRetrieve<XDT.rs_Condition_Select, XDT.rs_Condition_Expand, XDT.rs_Condition_Filter, XDT.rs_Condition_Fixed, XDT.rs_Condition_Result, XDT.rs_Condition_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_conditions: WebMappingRelated<XDT.rs_Condition_RelatedOne, XDT.rs_Condition_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_conditions: WebMappingCUDA<XDT.rs_Condition_Create, XDT.rs_Condition_Update, XDT.rs_Condition_Select>;
}
