/**
 * Runtime values and exact numeric types for generated Dataverse option sets.
 *
 * Import the value whose members you need, for example:
 * `import { account_accountcategorycode } from "./OptionSets";`
 */

export const account_accountcategorycode = {
  PreferredCustomer: 1,
  Standard: 2,
} as const;

export type account_accountcategorycode =
  (typeof account_accountcategorycode)[keyof typeof account_accountcategorycode];

export const account_accountclassificationcode = {
  DefaultValue: 1,
} as const;

export type account_accountclassificationcode =
  (typeof account_accountclassificationcode)[keyof typeof account_accountclassificationcode];

export const account_accountratingcode = {
  DefaultValue: 1,
} as const;

export type account_accountratingcode =
  (typeof account_accountratingcode)[keyof typeof account_accountratingcode];

export const account_address1_addresstypecode = {
  BillTo: 1,
  ShipTo: 2,
  Primary: 3,
  Other: 4,
} as const;

export type account_address1_addresstypecode =
  (typeof account_address1_addresstypecode)[keyof typeof account_address1_addresstypecode];

export const account_address1_freighttermscode = {
  FOB: 1,
  NoCharge: 2,
} as const;

export type account_address1_freighttermscode =
  (typeof account_address1_freighttermscode)[keyof typeof account_address1_freighttermscode];

export const account_address1_shippingmethodcode = {
  Airborne: 1,
  DHL: 2,
  FedEx: 3,
  UPS: 4,
  PostalMail: 5,
  FullLoad: 6,
  WillCall: 7,
} as const;

export type account_address1_shippingmethodcode =
  (typeof account_address1_shippingmethodcode)[keyof typeof account_address1_shippingmethodcode];

export const account_address2_addresstypecode = {
  DefaultValue: 1,
} as const;

export type account_address2_addresstypecode =
  (typeof account_address2_addresstypecode)[keyof typeof account_address2_addresstypecode];

export const account_address2_freighttermscode = {
  DefaultValue: 1,
} as const;

export type account_address2_freighttermscode =
  (typeof account_address2_freighttermscode)[keyof typeof account_address2_freighttermscode];

export const account_address2_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type account_address2_shippingmethodcode =
  (typeof account_address2_shippingmethodcode)[keyof typeof account_address2_shippingmethodcode];

export const account_businesstypecode = {
  DefaultValue: 1,
} as const;

export type account_businesstypecode =
  (typeof account_businesstypecode)[keyof typeof account_businesstypecode];

export const account_customersizecode = {
  DefaultValue: 1,
} as const;

export type account_customersizecode =
  (typeof account_customersizecode)[keyof typeof account_customersizecode];

export const account_customertypecode = {
  Competitor: 1,
  Consultant: 2,
  Customer: 3,
  Investor: 4,
  Partner: 5,
  Influencer: 6,
  Press: 7,
  Prospect: 8,
  Reseller: 9,
  Supplier: 10,
  Vendor: 11,
  Other: 12,
} as const;

export type account_customertypecode =
  (typeof account_customertypecode)[keyof typeof account_customertypecode];

export const account_industrycode = {
  Accounting: 1,
  AgricultureandNonpetrolNaturalResourceExtraction: 2,
  BroadcastingPrintingandPublishing: 3,
  Brokers: 4,
  BuildingSupplyRetail: 5,
  BusinessServices: 6,
  Consulting: 7,
  ConsumerServices: 8,
  DesignDirectionandCreativeManagement: 9,
  DistributorsDispatchersandProcessors: 10,
  DoctorsOfficesandClinics: 11,
  DurableManufacturing: 12,
  EatingandDrinkingPlaces: 13,
  EntertainmentRetail: 14,
  EquipmentRentalandLeasing: 15,
  Financial: 16,
  FoodandTobaccoProcessing: 17,
  InboundCapitalIntensiveProcessing: 18,
  InboundRepairandServices: 19,
  Insurance: 20,
  LegalServices: 21,
  NonDurableMerchandiseRetail: 22,
  OutboundConsumerService: 23,
  PetrochemicalExtractionandDistribution: 24,
  ServiceRetail: 25,
  SIGAffiliations: 26,
  SocialServices: 27,
  SpecialOutboundTradeContractors: 28,
  SpecialtyRealty: 29,
  Transportation: 30,
  UtilityCreationandDistribution: 31,
  VehicleRetail: 32,
  Wholesale: 33,
} as const;

export type account_industrycode =
  (typeof account_industrycode)[keyof typeof account_industrycode];

export const account_ownershipcode = {
  Public: 1,
  Private: 2,
  Subsidiary: 3,
  Other: 4,
} as const;

export type account_ownershipcode =
  (typeof account_ownershipcode)[keyof typeof account_ownershipcode];

export const account_paymenttermscode = {
  Net30: 1,
  _210Net30: 2,
  Net45: 3,
  Net60: 4,
} as const;

export type account_paymenttermscode =
  (typeof account_paymenttermscode)[keyof typeof account_paymenttermscode];

export const account_preferredappointmentdaycode = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6,
} as const;

export type account_preferredappointmentdaycode =
  (typeof account_preferredappointmentdaycode)[keyof typeof account_preferredappointmentdaycode];

export const account_preferredappointmenttimecode = {
  Morning: 1,
  Afternoon: 2,
  Evening: 3,
} as const;

export type account_preferredappointmenttimecode =
  (typeof account_preferredappointmenttimecode)[keyof typeof account_preferredappointmenttimecode];

export const account_preferredcontactmethodcode = {
  Any: 1,
  Email: 2,
  Phone: 3,
  Fax: 4,
  Mail: 5,
} as const;

export type account_preferredcontactmethodcode =
  (typeof account_preferredcontactmethodcode)[keyof typeof account_preferredcontactmethodcode];

export const account_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type account_shippingmethodcode =
  (typeof account_shippingmethodcode)[keyof typeof account_shippingmethodcode];

export const account_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type account_statecode =
  (typeof account_statecode)[keyof typeof account_statecode];

export const account_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type account_statuscode =
  (typeof account_statuscode)[keyof typeof account_statuscode];

export const account_territorycode = {
  DefaultValue: 1,
} as const;

export type account_territorycode =
  (typeof account_territorycode)[keyof typeof account_territorycode];

export const contact_accountrolecode = {
  DecisionMaker: 1,
  Employee: 2,
  Influencer: 3,
} as const;

export type contact_accountrolecode =
  (typeof contact_accountrolecode)[keyof typeof contact_accountrolecode];

export const contact_address1_addresstypecode = {
  BillTo: 1,
  ShipTo: 2,
  Primary: 3,
  Other: 4,
} as const;

export type contact_address1_addresstypecode =
  (typeof contact_address1_addresstypecode)[keyof typeof contact_address1_addresstypecode];

export const contact_address1_freighttermscode = {
  FOB: 1,
  NoCharge: 2,
} as const;

export type contact_address1_freighttermscode =
  (typeof contact_address1_freighttermscode)[keyof typeof contact_address1_freighttermscode];

export const contact_address1_shippingmethodcode = {
  Airborne: 1,
  DHL: 2,
  FedEx: 3,
  UPS: 4,
  PostalMail: 5,
  FullLoad: 6,
  WillCall: 7,
} as const;

export type contact_address1_shippingmethodcode =
  (typeof contact_address1_shippingmethodcode)[keyof typeof contact_address1_shippingmethodcode];

export const contact_address2_addresstypecode = {
  DefaultValue: 1,
} as const;

export type contact_address2_addresstypecode =
  (typeof contact_address2_addresstypecode)[keyof typeof contact_address2_addresstypecode];

export const contact_address2_freighttermscode = {
  DefaultValue: 1,
} as const;

export type contact_address2_freighttermscode =
  (typeof contact_address2_freighttermscode)[keyof typeof contact_address2_freighttermscode];

export const contact_address2_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type contact_address2_shippingmethodcode =
  (typeof contact_address2_shippingmethodcode)[keyof typeof contact_address2_shippingmethodcode];

export const contact_address3_addresstypecode = {
  DefaultValue: 1,
} as const;

export type contact_address3_addresstypecode =
  (typeof contact_address3_addresstypecode)[keyof typeof contact_address3_addresstypecode];

export const contact_address3_freighttermscode = {
  DefaultValue: 1,
} as const;

export type contact_address3_freighttermscode =
  (typeof contact_address3_freighttermscode)[keyof typeof contact_address3_freighttermscode];

export const contact_address3_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type contact_address3_shippingmethodcode =
  (typeof contact_address3_shippingmethodcode)[keyof typeof contact_address3_shippingmethodcode];

export const contact_customersizecode = {
  DefaultValue: 1,
} as const;

export type contact_customersizecode =
  (typeof contact_customersizecode)[keyof typeof contact_customersizecode];

export const contact_customertypecode = {
  DefaultValue: 1,
} as const;

export type contact_customertypecode =
  (typeof contact_customertypecode)[keyof typeof contact_customertypecode];

export const contact_educationcode = {
  DefaultValue: 1,
} as const;

export type contact_educationcode =
  (typeof contact_educationcode)[keyof typeof contact_educationcode];

export const contact_familystatuscode = {
  Single: 1,
  Married: 2,
  Divorced: 3,
  Widowed: 4,
} as const;

export type contact_familystatuscode =
  (typeof contact_familystatuscode)[keyof typeof contact_familystatuscode];

export const contact_gendercode = {
  Male: 1,
  Female: 2,
} as const;

export type contact_gendercode =
  (typeof contact_gendercode)[keyof typeof contact_gendercode];

export const contact_haschildrencode = {
  DefaultValue: 1,
} as const;

export type contact_haschildrencode =
  (typeof contact_haschildrencode)[keyof typeof contact_haschildrencode];

export const contact_leadsourcecode = {
  DefaultValue: 1,
} as const;

export type contact_leadsourcecode =
  (typeof contact_leadsourcecode)[keyof typeof contact_leadsourcecode];

export const contact_paymenttermscode = {
  Net30: 1,
  _210Net30: 2,
  Net45: 3,
  Net60: 4,
} as const;

export type contact_paymenttermscode =
  (typeof contact_paymenttermscode)[keyof typeof contact_paymenttermscode];

export const contact_preferredappointmentdaycode = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6,
} as const;

export type contact_preferredappointmentdaycode =
  (typeof contact_preferredappointmentdaycode)[keyof typeof contact_preferredappointmentdaycode];

export const contact_preferredappointmenttimecode = {
  Morning: 1,
  Afternoon: 2,
  Evening: 3,
} as const;

export type contact_preferredappointmenttimecode =
  (typeof contact_preferredappointmenttimecode)[keyof typeof contact_preferredappointmenttimecode];

export const contact_preferredcontactmethodcode = {
  Any: 1,
  Email: 2,
  Phone: 3,
  Fax: 4,
  Mail: 5,
} as const;

export type contact_preferredcontactmethodcode =
  (typeof contact_preferredcontactmethodcode)[keyof typeof contact_preferredcontactmethodcode];

export const contact_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type contact_shippingmethodcode =
  (typeof contact_shippingmethodcode)[keyof typeof contact_shippingmethodcode];

export const contact_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type contact_statecode =
  (typeof contact_statecode)[keyof typeof contact_statecode];

export const contact_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type contact_statuscode =
  (typeof contact_statuscode)[keyof typeof contact_statuscode];

export const contact_territorycode = {
  DefaultValue: 1,
} as const;

export type contact_territorycode =
  (typeof contact_territorycode)[keyof typeof contact_territorycode];

export const powerpagelanguages = {
  Arabic: 1025,
  BulgarianBulgaria: 1026,
  CatalanCatalan: 1027,
  ChineseTraditional: 1028,
  CzechCzechRepublic: 1029,
  DanishDenmark: 1030,
  GermanGermany: 1031,
  GreekGreece: 1032,
  English: 1033,
  FinnishFinland: 1035,
  FrenchFrance: 1036,
  Hebrew: 1037,
  HungarianHungary: 1038,
  ItalianItaly: 1040,
  JapaneseJapan: 1041,
  KoreanKorea: 1042,
  DutchNetherlands: 1043,
  NorwegianBokmålNorway: 1044,
  PolishPoland: 1045,
  PortugueseBrazil: 1046,
  RomanianRomania: 1048,
  RussianRussia: 1049,
  CroatianCroatia: 1050,
  SlovakSlovakia: 1051,
  SwedishSweden: 1053,
  ThaiThailand: 1054,
  TurkishTürkiye: 1055,
  IndonesianIndonesia: 1057,
  UkrainianUkraine: 1058,
  SlovenianSlovenia: 1060,
  EstonianEstonia: 1061,
  LatvianLatvia: 1062,
  LithuanianLithuania: 1063,
  VietnameseVietnam: 1066,
  BasqueBasque: 1069,
  HindiIndia: 1081,
  MalayMalaysia: 1086,
  KazakhKazakhstan: 1087,
  GalicianSpain: 1110,
  ChineseChina: 2052,
  PortuguesePortugal: 2070,
  SerbianLatinSerbia: 2074,
  ChineseHongKongSAR: 3076,
  SpanishTraditionalSortSpain: 3082,
  SerbianCyrillicSerbia: 3098,
} as const;

export type powerpagelanguages =
  (typeof powerpagelanguages)[keyof typeof powerpagelanguages];

export const rs_action_rs_actiontype = {
  Setfield: 126250000,
  Blocksave: 126250001,
  Callfunction: 126250002,
  Logmessage: 126250003,
} as const;

export type rs_action_rs_actiontype =
  (typeof rs_action_rs_actiontype)[keyof typeof rs_action_rs_actiontype];

export const rs_action_rs_branch = {
  Then: 126250000,
  Else: 126250001,
} as const;

export type rs_action_rs_branch =
  (typeof rs_action_rs_branch)[keyof typeof rs_action_rs_branch];

export const rs_action_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_action_statecode =
  (typeof rs_action_statecode)[keyof typeof rs_action_statecode];

export const rs_action_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_action_statuscode =
  (typeof rs_action_statuscode)[keyof typeof rs_action_statuscode];

export const rs_actioninputmapping_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_actioninputmapping_statecode =
  (typeof rs_actioninputmapping_statecode)[keyof typeof rs_actioninputmapping_statecode];

export const rs_actioninputmapping_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_actioninputmapping_statuscode =
  (typeof rs_actioninputmapping_statuscode)[keyof typeof rs_actioninputmapping_statuscode];

export const rs_condition_rs_aggregate = {
  None: 126250000,
  Any: 126250001,
  All: 126250002,
  Noneof: 126250003,
  Count: 126250004,
  Sum: 126250005,
  Min: 126250006,
  Max: 126250007,
  Average: 126250008,
} as const;

export type rs_condition_rs_aggregate =
  (typeof rs_condition_rs_aggregate)[keyof typeof rs_condition_rs_aggregate];

export const rs_condition_rs_operator = {
  Equals: 126250000,
  Notequals: 126250001,
  Greaterthan: 126250002,
  Greaterorequal: 126250003,
  Lessthan: 126250004,
  Lessorequal: 126250005,
  Contains: 126250006,
  Startswith: 126250007,
  Isempty: 126250008,
  Isnotempty: 126250009,
  Changed: 126250010,
  Inlist: 126250011,
  Between: 126250012,
} as const;

export type rs_condition_rs_operator =
  (typeof rs_condition_rs_operator)[keyof typeof rs_condition_rs_operator];

export const rs_condition_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_condition_statecode =
  (typeof rs_condition_statecode)[keyof typeof rs_condition_statecode];

export const rs_condition_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_condition_statuscode =
  (typeof rs_condition_statuscode)[keyof typeof rs_condition_statuscode];

export const rs_conditiongroup_rs_logicaloperator = {
  AND: 126250000,
  OR: 126250001,
} as const;

export type rs_conditiongroup_rs_logicaloperator =
  (typeof rs_conditiongroup_rs_logicaloperator)[keyof typeof rs_conditiongroup_rs_logicaloperator];

export const rs_conditiongroup_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_conditiongroup_statecode =
  (typeof rs_conditiongroup_statecode)[keyof typeof rs_conditiongroup_statecode];

export const rs_conditiongroup_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_conditiongroup_statuscode =
  (typeof rs_conditiongroup_statuscode)[keyof typeof rs_conditiongroup_statuscode];

export const rs_customfunction_rs_outputtype = {
  Text: 126250000,
  Number: 126250001,
  Decimal: 126250002,
  Money: 126250003,
  Boolean: 126250004,
  Date: 126250005,
  Lookup: 126250006,
} as const;

export type rs_customfunction_rs_outputtype =
  (typeof rs_customfunction_rs_outputtype)[keyof typeof rs_customfunction_rs_outputtype];

export const rs_customfunction_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_customfunction_statecode =
  (typeof rs_customfunction_statecode)[keyof typeof rs_customfunction_statecode];

export const rs_customfunction_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_customfunction_statuscode =
  (typeof rs_customfunction_statuscode)[keyof typeof rs_customfunction_statuscode];

export const rs_event = {
  Create: 126250000,
  Update: 126250001,
  Delete: 126250002,
} as const;

export type rs_event =
  (typeof rs_event)[keyof typeof rs_event];

export const rs_executionlog_rs_outcome = {
  Matched: 126250000,
  Notmatched: 126250001,
  Skipped: 126250002,
  Failed: 126250003,
  Blocked: 126250004,
} as const;

export type rs_executionlog_rs_outcome =
  (typeof rs_executionlog_rs_outcome)[keyof typeof rs_executionlog_rs_outcome];

export const rs_executionlog_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_executionlog_statecode =
  (typeof rs_executionlog_statecode)[keyof typeof rs_executionlog_statecode];

export const rs_executionlog_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_executionlog_statuscode =
  (typeof rs_executionlog_statuscode)[keyof typeof rs_executionlog_statuscode];

export const rs_loglevel = {
  Off: 126250000,
  Errorsonly: 126250001,
  Summary: 126250002,
  Fulltrace: 126250003,
} as const;

export type rs_loglevel =
  (typeof rs_loglevel)[keyof typeof rs_loglevel];

export const rs_lookupdefinition_rs_multimatchpolicy = {
  Mostspecific: 126250000,
  Firstbysort: 126250001,
  Error: 126250002,
} as const;

export type rs_lookupdefinition_rs_multimatchpolicy =
  (typeof rs_lookupdefinition_rs_multimatchpolicy)[keyof typeof rs_lookupdefinition_rs_multimatchpolicy];

export const rs_lookupdefinition_rs_nomatchpolicy = {
  Block: 126250000,
  Usedefault: 126250001,
  Skip: 126250002,
} as const;

export type rs_lookupdefinition_rs_nomatchpolicy =
  (typeof rs_lookupdefinition_rs_nomatchpolicy)[keyof typeof rs_lookupdefinition_rs_nomatchpolicy];

export const rs_lookupdefinition_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_lookupdefinition_statecode =
  (typeof rs_lookupdefinition_statecode)[keyof typeof rs_lookupdefinition_statecode];

export const rs_lookupdefinition_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_lookupdefinition_statuscode =
  (typeof rs_lookupdefinition_statuscode)[keyof typeof rs_lookupdefinition_statuscode];

export const rs_messagelevel = {
  Info: 126250000,
  Warning: 126250001,
  Error: 126250002,
} as const;

export type rs_messagelevel =
  (typeof rs_messagelevel)[keyof typeof rs_messagelevel];

export const rs_release_rs_status = {
  Live: 126250000,
  Rolledback: 126250001,
  Failed: 126250002,
} as const;

export type rs_release_rs_status =
  (typeof rs_release_rs_status)[keyof typeof rs_release_rs_status];

export const rs_release_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_release_statecode =
  (typeof rs_release_statecode)[keyof typeof rs_release_statecode];

export const rs_release_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_release_statuscode =
  (typeof rs_release_statuscode)[keyof typeof rs_release_statuscode];

export const rs_rule_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_rule_statecode =
  (typeof rs_rule_statecode)[keyof typeof rs_rule_statecode];

export const rs_rule_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_rule_statuscode =
  (typeof rs_rule_statuscode)[keyof typeof rs_rule_statuscode];

export const rs_ruleversion_rs_costlevel = {
  Low: 126250000,
  Medium: 126250001,
  High: 126250002,
} as const;

export type rs_ruleversion_rs_costlevel =
  (typeof rs_ruleversion_rs_costlevel)[keyof typeof rs_ruleversion_rs_costlevel];

export const rs_ruleversion_rs_failurepolicy = {
  Block: 126250000,
  Logandcontinue: 126250001,
} as const;

export type rs_ruleversion_rs_failurepolicy =
  (typeof rs_ruleversion_rs_failurepolicy)[keyof typeof rs_ruleversion_rs_failurepolicy];

export const rs_ruleversion_rs_runas = {
  Callinguser: 126250000,
  System: 126250001,
} as const;

export type rs_ruleversion_rs_runas =
  (typeof rs_ruleversion_rs_runas)[keyof typeof rs_ruleversion_rs_runas];

export const rs_ruleversion_rs_source = {
  Authored: 126250000,
  Imported: 126250001,
} as const;

export type rs_ruleversion_rs_source =
  (typeof rs_ruleversion_rs_source)[keyof typeof rs_ruleversion_rs_source];

export const rs_ruleversion_rs_status = {
  Draft: 126250000,
  Published: 126250001,
  Superseded: 126250002,
  Discarded: 126250003,
} as const;

export type rs_ruleversion_rs_status =
  (typeof rs_ruleversion_rs_status)[keyof typeof rs_ruleversion_rs_status];

export const rs_ruleversion_statecode = {
  Active: 0,
  Inactive: 1,
} as const;

export type rs_ruleversion_statecode =
  (typeof rs_ruleversion_statecode)[keyof typeof rs_ruleversion_statecode];

export const rs_ruleversion_statuscode = {
  Active: 1,
  Inactive: 2,
} as const;

export type rs_ruleversion_statuscode =
  (typeof rs_ruleversion_statuscode)[keyof typeof rs_ruleversion_statuscode];

export const rs_stage = {
  Beforesave: 126250000,
  Aftersave: 126250001,
} as const;

export type rs_stage =
  (typeof rs_stage)[keyof typeof rs_stage];

export const rs_valuesourcetype = {
  Fixed: 126250000,
  Fieldpath: 126250001,
  Expression: 126250002,
  Lookup: 126250003,
  Function: 126250004,
} as const;

export type rs_valuesourcetype =
  (typeof rs_valuesourcetype)[keyof typeof rs_valuesourcetype];

export const systemuser_accessmode = {
  ReadWrite: 0,
  Administrative: 1,
  Read: 2,
  SupportUser: 3,
  Noninteractive: 4,
  DelegatedAdmin: 5,
} as const;

export type systemuser_accessmode =
  (typeof systemuser_accessmode)[keyof typeof systemuser_accessmode];

export const systemuser_address1_addresstypecode = {
  DefaultValue: 1,
} as const;

export type systemuser_address1_addresstypecode =
  (typeof systemuser_address1_addresstypecode)[keyof typeof systemuser_address1_addresstypecode];

export const systemuser_address1_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type systemuser_address1_shippingmethodcode =
  (typeof systemuser_address1_shippingmethodcode)[keyof typeof systemuser_address1_shippingmethodcode];

export const systemuser_address2_addresstypecode = {
  DefaultValue: 1,
} as const;

export type systemuser_address2_addresstypecode =
  (typeof systemuser_address2_addresstypecode)[keyof typeof systemuser_address2_addresstypecode];

export const systemuser_address2_shippingmethodcode = {
  DefaultValue: 1,
} as const;

export type systemuser_address2_shippingmethodcode =
  (typeof systemuser_address2_shippingmethodcode)[keyof typeof systemuser_address2_shippingmethodcode];

export const systemuser_azurestate = {
  Exists: 0,
  Softdeleted: 1,
  Notfoundorharddeleted: 2,
} as const;

export type systemuser_azurestate =
  (typeof systemuser_azurestate)[keyof typeof systemuser_azurestate];

export const systemuser_caltype = {
  Professional: 0,
  Administrative: 1,
  Basic: 2,
  DeviceProfessional: 3,
  DeviceBasic: 4,
  Essential: 5,
  DeviceEssential: 6,
  Enterprise: 7,
  DeviceEnterprise: 8,
  Sales: 9,
  Service: 10,
  FieldService: 11,
  ProjectService: 12,
} as const;

export type systemuser_caltype =
  (typeof systemuser_caltype)[keyof typeof systemuser_caltype];

export const systemuser_deletestate = {
  Notdeleted: 0,
  Softdeleted: 1,
} as const;

export type systemuser_deletestate =
  (typeof systemuser_deletestate)[keyof typeof systemuser_deletestate];

export const systemuser_emailrouteraccessapproval = {
  Empty: 0,
  Approved: 1,
  PendingApproval: 2,
  Rejected: 3,
} as const;

export type systemuser_emailrouteraccessapproval =
  (typeof systemuser_emailrouteraccessapproval)[keyof typeof systemuser_emailrouteraccessapproval];

export const systemuser_incomingemaildeliverymethod = {
  None: 0,
  MicrosoftDynamics365forOutlook: 1,
  ServerSideSynchronizationorEmailRouter: 2,
  ForwardMailbox: 3,
} as const;

export type systemuser_incomingemaildeliverymethod =
  (typeof systemuser_incomingemaildeliverymethod)[keyof typeof systemuser_incomingemaildeliverymethod];

export const systemuser_invitestatuscode = {
  InvitationNotSent: 0,
  Invited: 1,
  InvitationNearExpired: 2,
  InvitationExpired: 3,
  InvitationAccepted: 4,
  InvitationRejected: 5,
  InvitationRevoked: 6,
} as const;

export type systemuser_invitestatuscode =
  (typeof systemuser_invitestatuscode)[keyof typeof systemuser_invitestatuscode];

export const systemuser_outgoingemaildeliverymethod = {
  None: 0,
  MicrosoftDynamics365forOutlook: 1,
  ServerSideSynchronizationorEmailRouter: 2,
} as const;

export type systemuser_outgoingemaildeliverymethod =
  (typeof systemuser_outgoingemaildeliverymethod)[keyof typeof systemuser_outgoingemaildeliverymethod];

export const systemuser_preferredaddresscode = {
  MailingAddress: 1,
  OtherAddress: 2,
} as const;

export type systemuser_preferredaddresscode =
  (typeof systemuser_preferredaddresscode)[keyof typeof systemuser_preferredaddresscode];

export const systemuser_preferredemailcode = {
  DefaultValue: 1,
} as const;

export type systemuser_preferredemailcode =
  (typeof systemuser_preferredemailcode)[keyof typeof systemuser_preferredemailcode];

export const systemuser_preferredphonecode = {
  MainPhone: 1,
  OtherPhone: 2,
  HomePhone: 3,
  MobilePhone: 4,
} as const;

export type systemuser_preferredphonecode =
  (typeof systemuser_preferredphonecode)[keyof typeof systemuser_preferredphonecode];

export const systemuser_systemmanagedusertype = {
  EntraUser: 0,
  C2User: 1,
  ImpersonableStubUser: 2,
  AgenticUser: 3,
  NonLicensed: 4,
} as const;

export type systemuser_systemmanagedusertype =
  (typeof systemuser_systemmanagedusertype)[keyof typeof systemuser_systemmanagedusertype];
