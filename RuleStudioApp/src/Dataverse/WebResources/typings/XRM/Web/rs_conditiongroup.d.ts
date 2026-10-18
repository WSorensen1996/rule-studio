declare namespace XDT {
  interface rs_ConditionGroup_Fixed {
    rs_conditiongroupid: string;
  }
  interface rs_ConditionGroup_Result extends rs_ConditionGroup_Fixed {
    createdby_guid: string | null;
    createdon: Date | null;
    createdonbehalfby_guid: string | null;
    importsequencenumber: number | null;
    modifiedby_guid: string | null;
    modifiedon: Date | null;
    modifiedonbehalfby_guid: string | null;
    organizationid_guid: string | null;
    overriddencreatedon: Date | null;
    rs_AggregateScopeConditionId?: rs_Condition_Result | null;
    rs_ParentGroupId?: rs_ConditionGroup_Result | null;
    rs_RuleVersionId?: rs_RuleVersion_Result | null;
    rs_aggregatescopeconditionid_guid: string | null;
    rs_logicaloperator: rs_conditiongroup_rs_logicaloperator | null;
    rs_name: string | null;
    rs_order: number | null;
    rs_parentgroupid_guid: string | null;
    rs_rs_conditiongroup_rs_condition_rs_conditiongroupid?: rs_Condition_Result[] | null;
    rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid?: rs_ConditionGroup_Result[] | null;
    rs_ruleversionid_guid: string | null;
    statecode: rs_conditiongroup_statecode | null;
    statuscode: rs_conditiongroup_statuscode | null;
    timezoneruleversionnumber: number | null;
    utcconversiontimezonecode: number | null;
    versionnumber: number | null;
  }
  interface rs_ConditionGroup_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    rs_aggregatescopeconditionid_formatted?: string;
    rs_logicaloperator_formatted?: string;
    rs_parentgroupid_formatted?: string;
    rs_ruleversionid_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface rs_ConditionGroup_Select {
    createdby_guid: WebAttribute<rs_ConditionGroup_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<rs_ConditionGroup_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<rs_ConditionGroup_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<rs_ConditionGroup_Select, { importsequencenumber: number | null }, object>;
    modifiedby_guid: WebAttribute<rs_ConditionGroup_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<rs_ConditionGroup_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<rs_ConditionGroup_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    organizationid_guid: WebAttribute<rs_ConditionGroup_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<rs_ConditionGroup_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    rs_aggregatescopeconditionid_guid: WebAttribute<rs_ConditionGroup_Select, { rs_aggregatescopeconditionid_guid: string | null }, { rs_aggregatescopeconditionid_formatted?: string }>;
    rs_conditiongroupid: WebAttribute<rs_ConditionGroup_Select, { rs_conditiongroupid: string | null }, object>;
    rs_logicaloperator: WebAttribute<rs_ConditionGroup_Select, { rs_logicaloperator: rs_conditiongroup_rs_logicaloperator | null }, { rs_logicaloperator_formatted?: string }>;
    rs_name: WebAttribute<rs_ConditionGroup_Select, { rs_name: string | null }, object>;
    rs_order: WebAttribute<rs_ConditionGroup_Select, { rs_order: number | null }, object>;
    rs_parentgroupid_guid: WebAttribute<rs_ConditionGroup_Select, { rs_parentgroupid_guid: string | null }, { rs_parentgroupid_formatted?: string }>;
    rs_ruleversionid_guid: WebAttribute<rs_ConditionGroup_Select, { rs_ruleversionid_guid: string | null }, { rs_ruleversionid_formatted?: string }>;
    statecode: WebAttribute<rs_ConditionGroup_Select, { statecode: rs_conditiongroup_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<rs_ConditionGroup_Select, { statuscode: rs_conditiongroup_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<rs_ConditionGroup_Select, { timezoneruleversionnumber: number | null }, object>;
    utcconversiontimezonecode: WebAttribute<rs_ConditionGroup_Select, { utcconversiontimezonecode: number | null }, object>;
    versionnumber: WebAttribute<rs_ConditionGroup_Select, { versionnumber: number | null }, object>;
  }
  interface rs_ConditionGroup_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    rs_aggregatescopeconditionid_guid: XQW.Guid;
    rs_conditiongroupid: XQW.Guid;
    rs_logicaloperator: rs_conditiongroup_rs_logicaloperator;
    rs_name: string;
    rs_order: number;
    rs_parentgroupid_guid: XQW.Guid;
    rs_ruleversionid_guid: XQW.Guid;
    statecode: rs_conditiongroup_statecode;
    statuscode: rs_conditiongroup_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface rs_ConditionGroup_Expand {
    createdby: WebExpand<rs_ConditionGroup_Expand, SystemUser_Select, SystemUser_Filter, { createdby: SystemUser_Result }>;
    createdonbehalfby: WebExpand<rs_ConditionGroup_Expand, SystemUser_Select, SystemUser_Filter, { createdonbehalfby: SystemUser_Result }>;
    modifiedby: WebExpand<rs_ConditionGroup_Expand, SystemUser_Select, SystemUser_Filter, { modifiedby: SystemUser_Result }>;
    modifiedonbehalfby: WebExpand<rs_ConditionGroup_Expand, SystemUser_Select, SystemUser_Filter, { modifiedonbehalfby: SystemUser_Result }>;
    rs_AggregateScopeConditionId: WebExpand<rs_ConditionGroup_Expand, rs_Condition_Select, rs_Condition_Filter, { rs_AggregateScopeConditionId: rs_Condition_Result }>;
    rs_ParentGroupId: WebExpand<rs_ConditionGroup_Expand, rs_ConditionGroup_Select, rs_ConditionGroup_Filter, { rs_ParentGroupId: rs_ConditionGroup_Result }>;
    rs_RuleVersionId: WebExpand<rs_ConditionGroup_Expand, rs_RuleVersion_Select, rs_RuleVersion_Filter, { rs_RuleVersionId: rs_RuleVersion_Result }>;
    rs_rs_conditiongroup_rs_condition_rs_conditiongroupid: WebExpand<rs_ConditionGroup_Expand, rs_Condition_Select, rs_Condition_Filter, { rs_rs_conditiongroup_rs_condition_rs_conditiongroupid: rs_Condition_Result[] }>;
    rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid: WebExpand<rs_ConditionGroup_Expand, rs_ConditionGroup_Select, rs_ConditionGroup_Filter, { rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid: rs_ConditionGroup_Result[] }>;
  }
  interface rs_ConditionGroup_Create {
    importsequencenumber?: number | null;
    overriddencreatedon?: Date | null;
    rs_AggregateScopeConditionId_bind$rs_conditions?: string | null;
    rs_ParentGroupId_bind$rs_conditiongroups?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_conditiongroupid?: string | null;
    rs_logicaloperator?: rs_conditiongroup_rs_logicaloperator | null;
    rs_name?: string | null;
    rs_order?: number | null;
    statuscode?: rs_conditiongroup_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ConditionGroup_Update {
    rs_AggregateScopeConditionId_bind$rs_conditions?: string | null;
    rs_ParentGroupId_bind$rs_conditiongroups?: string | null;
    rs_RuleVersionId_bind$rs_ruleversions?: string | null;
    rs_logicaloperator?: rs_conditiongroup_rs_logicaloperator | null;
    rs_name?: string | null;
    rs_order?: number | null;
    statecode?: rs_conditiongroup_statecode | null;
    statuscode?: rs_conditiongroup_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
  }
  interface rs_ConditionGroup_RelatedOne {
    createdby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    createdonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    modifiedonbehalfby: WebMappingRetrieve<SystemUser_Select, SystemUser_Expand, SystemUser_Filter, SystemUser_Fixed, SystemUser_Result, SystemUser_FormattedResult>;
    rs_AggregateScopeConditionId: WebMappingRetrieve<rs_Condition_Select, rs_Condition_Expand, rs_Condition_Filter, rs_Condition_Fixed, rs_Condition_Result, rs_Condition_FormattedResult>;
    rs_ParentGroupId: WebMappingRetrieve<rs_ConditionGroup_Select, rs_ConditionGroup_Expand, rs_ConditionGroup_Filter, rs_ConditionGroup_Fixed, rs_ConditionGroup_Result, rs_ConditionGroup_FormattedResult>;
    rs_RuleVersionId: WebMappingRetrieve<rs_RuleVersion_Select, rs_RuleVersion_Expand, rs_RuleVersion_Filter, rs_RuleVersion_Fixed, rs_RuleVersion_Result, rs_RuleVersion_FormattedResult>;
  }
  interface rs_ConditionGroup_RelatedMany {
    rs_rs_conditiongroup_rs_condition_rs_conditiongroupid: WebMappingRetrieve<rs_Condition_Select, rs_Condition_Expand, rs_Condition_Filter, rs_Condition_Fixed, rs_Condition_Result, rs_Condition_FormattedResult>;
    rs_rs_conditiongroup_rs_conditiongroup_rs_parentgroupid: WebMappingRetrieve<rs_ConditionGroup_Select, rs_ConditionGroup_Expand, rs_ConditionGroup_Filter, rs_ConditionGroup_Fixed, rs_ConditionGroup_Result, rs_ConditionGroup_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  rs_conditiongroups: WebMappingRetrieve<XDT.rs_ConditionGroup_Select, XDT.rs_ConditionGroup_Expand, XDT.rs_ConditionGroup_Filter, XDT.rs_ConditionGroup_Fixed, XDT.rs_ConditionGroup_Result, XDT.rs_ConditionGroup_FormattedResult>;
}

interface WebEntitiesRelated {
  rs_conditiongroups: WebMappingRelated<XDT.rs_ConditionGroup_RelatedOne, XDT.rs_ConditionGroup_RelatedMany>;
}

interface WebEntitiesCUDA {
  rs_conditiongroups: WebMappingCUDA<XDT.rs_ConditionGroup_Create, XDT.rs_ConditionGroup_Update, XDT.rs_ConditionGroup_Select>;
}
