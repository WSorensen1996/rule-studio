declare namespace XDT {
  interface rs_CustomFunction_Fixed {
    rs_customfunctionid: string;
  }
  interface rs_CustomFunction_Result extends rs_CustomFunction_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_customapiname: string | null;
    rs_description: string | null;
    rs_enabled: boolean | null;
    rs_hassideeffects: boolean | null;
    rs_inputschema: string | null;
    rs_name: string | null;
    rs_outputtype: rs_customfunction_rs_outputtype | null;
    rs_rs_customfunction_rs_action_rs_customfunctionid?: rs_Action_Result[] | null;
    rs_rs_customfunction_rs_condition_rs_customfunctionid?: rs_Condition_Result[] | null;
    rs_safetoretry: boolean | null;
    rs_timeoutms: number | null;
    statecode: rs_customfunction_statecode | null;
    statuscode: rs_customfunction_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_CustomFunction_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_outputtype_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_CustomFunction_Select {
    createdby_guid: WebAttribute<rs_CustomFunction_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_CustomFunction_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_CustomFunction_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_CustomFunction_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_CustomFunction_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_CustomFunction_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_CustomFunction_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_CustomFunction_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_CustomFunction_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_customapiname: WebAttribute<rs_CustomFunction_Select, { rs_customapiname: string | null }, object>;
    rs_customfunctionid: WebAttribute<rs_CustomFunction_Select, { rs_customfunctionid: string | null }, object>;
    rs_description: WebAttribute<rs_CustomFunction_Select, { rs_description: string | null }, object>;
    rs_enabled: WebAttribute<rs_CustomFunction_Select, { rs_enabled: boolean | null }, object>;
    rs_hassideeffects: WebAttribute<rs_CustomFunction_Select, { rs_hassideeffects: boolean | null }, object>;
    rs_inputschema: WebAttribute<rs_CustomFunction_Select, { rs_inputschema: string | null }, object>;
    rs_name: WebAttribute<rs_CustomFunction_Select, { rs_name: string | null }, object>;
    rs_outputtype: WebAttribute<rs_CustomFunction_Select, { rs_outputtype: rs_customfunction_rs_outputtype | null }, { rs_outputtype_formatted?: string }>;
    rs_safetoretry: WebAttribute<rs_CustomFunction_Select, { rs_safetoretry: boolean | null }, object>;
    rs_timeoutms: WebAttribute<rs_CustomFunction_Select, { rs_timeoutms: number | null }, object>;
    statecode: WebAttribute<rs_CustomFunction_Select, { statecode: rs_customfunction_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_CustomFunction_Select, { statuscode: rs_customfunction_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_CustomFunction_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_CustomFunction_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_CustomFunction_Select, { versionnumber: number | null }, object>;
  }
  interface rs_CustomFunction_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_customapiname: string;
    rs_customfunctionid: XQW.Guid;
    rs_description: string;
    rs_enabled: boolean;
    rs_hassideeffects: boolean;
    rs_inputschema: string;
    rs_name: string;
    rs_outputtype: rs_customfunction_rs_outputtype;
    rs_safetoretry: boolean;
    rs_timeoutms: number;
    statecode: rs_customfunction_statecode;
    statuscode: rs_customfunction_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_CustomFunction_Expand {
    createdby: WebExpand<rs_CustomFunction_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_CustomFunction_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_CustomFunction_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_CustomFunction_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_rs_customfunction_rs_action_rs_customfunctionid: WebExpand<rs_CustomFunction_Expand, rs_Action_Select, rs_Action_Filter, { rs_rs_customfunction_rs_action_rs_customfunctionid: rs_Action_Result[] }>;
    rs_rs_customfunction_rs_condition_rs_customfunctionid: WebExpand<rs_CustomFunction_Expand, rs_Condition_Select, rs_Condition_Filter, { rs_rs_customfunction_rs_condition_rs_customfunctionid: rs_Condition_Result[] }>;
  }
  interface rs_CustomFunction_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_customapiname?: string | null;
    rs_customfunctionid?: string | null;
    rs_description?: string | null;
    rs_enabled?: boolean | null;
    rs_hassideeffects?: boolean | null;
    rs_inputschema?: string | null;
    rs_name?: string | null;
    rs_outputtype?: rs_customfunction_rs_outputtype | null;
    rs_safetoretry?: boolean | null;
    rs_timeoutms?: number | null;
    statuscode?: rs_customfunction_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_CustomFunction_Update {
    rs_customapiname?: string | null;
    rs_description?: string | null;
    rs_enabled?: boolean | null;
    rs_hassideeffects?: boolean | null;
    rs_inputschema?: string | null;
    rs_name?: string | null;
    rs_outputtype?: rs_customfunction_rs_outputtype | null;
    rs_safetoretry?: boolean | null;
    rs_timeoutms?: number | null;
    statecode?: rs_customfunction_statecode | null;
    statuscode?: rs_customfunction_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_CustomFunction_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
  }
  interface rs_CustomFunction_RelatedMany {
    rs_rs_customfunction_rs_action_rs_customfunctionid: WebMappingRetrieve<rs_Action_Select, rs_Action_Expand, rs_Action_Filter, rs_Action_Fixed, rs_Action_Result, rs_Action_FormattedResult>;
    rs_rs_customfunction_rs_condition_rs_customfunctionid: WebMappingRetrieve<rs_Condition_Select, rs_Condition_Expand, rs_Condition_Filter, rs_Condition_Fixed, rs_Condition_Result, rs_Condition_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_customfunctions: WebMappingRetrieve<XDT.rs_CustomFunction_Select, XDT.rs_CustomFunction_Expand, XDT.rs_CustomFunction_Filter, XDT.rs_CustomFunction_Fixed, XDT.rs_CustomFunction_Result, XDT.rs_CustomFunction_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_customfunctions: WebMappingRelated<XDT.rs_CustomFunction_RelatedOne, XDT.rs_CustomFunction_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_customfunctions: WebMappingCUDA<XDT.rs_CustomFunction_Create, XDT.rs_CustomFunction_Update, XDT.rs_CustomFunction_Select>;
}
