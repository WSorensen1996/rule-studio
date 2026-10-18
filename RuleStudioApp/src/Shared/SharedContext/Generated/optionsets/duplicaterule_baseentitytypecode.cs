using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum duplicaterule_baseentitytypecode
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Account", 1033)]
    Account = 1,

    [EnumMember]
    [OptionSetMetadata("Contact", 1033)]
    Contact = 2,

    [EnumMember]
    [OptionSetMetadata("Note", 1033)]
    Note = 5,

    [EnumMember]
    [OptionSetMetadata("Business Unit Map", 1033)]
    BusinessUnitMap = 6,

    [EnumMember]
    [OptionSetMetadata("Owner", 1033)]
    Owner = 7,

    [EnumMember]
    [OptionSetMetadata("User", 1033)]
    User = 8,

    [EnumMember]
    [OptionSetMetadata("Team", 1033)]
    Team = 9,

    [EnumMember]
    [OptionSetMetadata("Business Unit", 1033)]
    BusinessUnit = 10,

    [EnumMember]
    [OptionSetMetadata("System User Principal", 1033)]
    SystemUserPrincipal = 14,

    [EnumMember]
    [OptionSetMetadata("Subscription", 1033)]
    Subscription = 29,

    [EnumMember]
    [OptionSetMetadata("Filter Template", 1033)]
    FilterTemplate = 30,

    [EnumMember]
    [OptionSetMetadata("Privilege Object Type Code", 1033)]
    PrivilegeObjectTypeCode = 31,

    [EnumMember]
    [OptionSetMetadata("Subscription Synchronization Information", 1033)]
    SubscriptionSynchronizationInformation = 33,

    [EnumMember]
    [OptionSetMetadata("Tracking information for deleted entities", 1033)]
    Trackinginformationfordeletedentities = 35,

    [EnumMember]
    [OptionSetMetadata("Client update", 1033)]
    Clientupdate = 36,

    [EnumMember]
    [OptionSetMetadata("Subscription Manually Tracked Object", 1033)]
    SubscriptionManuallyTrackedObject = 37,

    [EnumMember]
    [OptionSetMetadata("SystemUser BusinessUnit Entity Map", 1033)]
    SystemUserBusinessUnitEntityMap = 42,

    [EnumMember]
    [OptionSetMetadata("Field Sharing", 1033)]
    FieldSharing = 44,

    [EnumMember]
    [OptionSetMetadata("Subscription Statistic Offline", 1033)]
    SubscriptionStatisticOffline = 45,

    [EnumMember]
    [OptionSetMetadata("Subscription Statistic Outlook", 1033)]
    SubscriptionStatisticOutlook = 46,

    [EnumMember]
    [OptionSetMetadata("Subscription Sync Entry Offline", 1033)]
    SubscriptionSyncEntryOffline = 47,

    [EnumMember]
    [OptionSetMetadata("Subscription Sync Entry Outlook", 1033)]
    SubscriptionSyncEntryOutlook = 48,

    [EnumMember]
    [OptionSetMetadata("Position", 1033)]
    Position = 50,

    [EnumMember]
    [OptionSetMetadata("System User Manager Map", 1033)]
    SystemUserManagerMap = 51,

    [EnumMember]
    [OptionSetMetadata("User Search Facet", 1033)]
    UserSearchFacet = 52,

    [EnumMember]
    [OptionSetMetadata("Global Search Configuration", 1033)]
    GlobalSearchConfiguration = 54,

    [EnumMember]
    [OptionSetMetadata("FileAttachment", 1033)]
    FileAttachment = 55,

    [EnumMember]
    [OptionSetMetadata("SystemUserAuthorizationChangeTracker", 1033)]
    SystemUserAuthorizationChangeTracker = 60,

    [EnumMember]
    [OptionSetMetadata("Record Filter", 1033)]
    RecordFilter = 72,

    [EnumMember]
    [OptionSetMetadata("EntityRecordFilter", 1033)]
    EntityRecordFilter = 73,

    [EnumMember]
    [OptionSetMetadata("Secured Masking Rule", 1033)]
    SecuredMaskingRule = 74,

    [EnumMember]
    [OptionSetMetadata("Privilege Checker Run", 1033)]
    PrivilegeCheckerRun = 75,

    [EnumMember]
    [OptionSetMetadata("Privilege Checker Log", 1033)]
    PrivilegeCheckerLog = 76,

    [EnumMember]
    [OptionSetMetadata("Virtual Entity Data Provider", 1033)]
    VirtualEntityDataProvider = 78,

    [EnumMember]
    [OptionSetMetadata("Virtual Entity Data Source", 1033)]
    VirtualEntityDataSource = 85,

    [EnumMember]
    [OptionSetMetadata("Team template", 1033)]
    Teamtemplate = 92,

    [EnumMember]
    [OptionSetMetadata("Social Profile", 1033)]
    SocialProfile = 99,

    [EnumMember]
    [OptionSetMetadata("Service Plan", 1033)]
    ServicePlan = 101,

    [EnumMember]
    [OptionSetMetadata("Privileges Removal Setting", 1033)]
    PrivilegesRemovalSetting = 103,

    [EnumMember]
    [OptionSetMetadata("Indexed Article", 1033)]
    IndexedArticle = 126,

    [EnumMember]
    [OptionSetMetadata("Article", 1033)]
    Article = 127,

    [EnumMember]
    [OptionSetMetadata("Subject", 1033)]
    Subject = 129,

    [EnumMember]
    [OptionSetMetadata("Announcement", 1033)]
    Announcement = 132,

    [EnumMember]
    [OptionSetMetadata("Activity Party", 1033)]
    ActivityParty = 135,

    [EnumMember]
    [OptionSetMetadata("User Settings", 1033)]
    UserSettings = 150,

    [EnumMember]
    [OptionSetMetadata("Canvas App", 1033)]
    CanvasApp = 300,

    [EnumMember]
    [OptionSetMetadata("Callback Registration", 1033)]
    CallbackRegistration = 301,

    [EnumMember]
    [OptionSetMetadata("Connector", 1033)]
    Connector = 372,

    [EnumMember]
    [OptionSetMetadata("Connection Instance", 1033)]
    ConnectionInstance = 373,

    [EnumMember]
    [OptionSetMetadata("Environment Variable Definition", 1033)]
    EnvironmentVariableDefinition = 380,

    [EnumMember]
    [OptionSetMetadata("Environment Variable Value", 1033)]
    EnvironmentVariableValue = 381,

    [EnumMember]
    [OptionSetMetadata("AI Template", 1033)]
    AITemplate = 400,

    [EnumMember]
    [OptionSetMetadata("AI Model", 1033)]
    AIModel = 401,

    [EnumMember]
    [OptionSetMetadata("AI Configuration", 1033)]
    AIConfiguration = 402,

    [EnumMember]
    [OptionSetMetadata("Dataflow", 1033)]
    Dataflow = 418,

    [EnumMember]
    [OptionSetMetadata("Entity Analytics Config", 1033)]
    EntityAnalyticsConfig = 430,

    [EnumMember]
    [OptionSetMetadata("Image Attribute Configuration", 1033)]
    ImageAttributeConfiguration = 431,

    [EnumMember]
    [OptionSetMetadata("Entity Image Configuration", 1033)]
    EntityImageConfiguration = 432,

    [EnumMember]
    [OptionSetMetadata("New Process", 1033)]
    NewProcess = 950,

    [EnumMember]
    [OptionSetMetadata("Translation Process", 1033)]
    TranslationProcess = 951,

    [EnumMember]
    [OptionSetMetadata("Expired Process", 1033)]
    ExpiredProcess = 955,

    [EnumMember]
    [OptionSetMetadata("Attachment", 1033)]
    Attachment = 1001,

    [EnumMember]
    [OptionSetMetadata("Attachment", 1033)]
    Attachment_1 = 1002,

    [EnumMember]
    [OptionSetMetadata("Internal Address", 1033)]
    InternalAddress = 1003,

    [EnumMember]
    [OptionSetMetadata("Image Descriptor", 1033)]
    ImageDescriptor = 1007,

    [EnumMember]
    [OptionSetMetadata("Article Template", 1033)]
    ArticleTemplate = 1016,

    [EnumMember]
    [OptionSetMetadata("Organization", 1033)]
    Organization = 1019,

    [EnumMember]
    [OptionSetMetadata("Organization UI", 1033)]
    OrganizationUI = 1021,

    [EnumMember]
    [OptionSetMetadata("Privilege", 1033)]
    Privilege = 1023,

    [EnumMember]
    [OptionSetMetadata("System Form", 1033)]
    SystemForm = 1030,

    [EnumMember]
    [OptionSetMetadata("User Dashboard", 1033)]
    UserDashboard = 1031,

    [EnumMember]
    [OptionSetMetadata("Security Role", 1033)]
    SecurityRole = 1036,

    [EnumMember]
    [OptionSetMetadata("Role Template", 1033)]
    RoleTemplate = 1037,

    [EnumMember]
    [OptionSetMetadata("View", 1033)]
    View = 1039,

    [EnumMember]
    [OptionSetMetadata("String Map", 1033)]
    StringMap = 1043,

    [EnumMember]
    [OptionSetMetadata("Address", 1033)]
    Address = 1071,

    [EnumMember]
    [OptionSetMetadata("Subscription Clients", 1033)]
    SubscriptionClients = 1072,

    [EnumMember]
    [OptionSetMetadata("Status Map", 1033)]
    StatusMap = 1075,

    [EnumMember]
    [OptionSetMetadata("Article Comment", 1033)]
    ArticleComment = 1082,

    [EnumMember]
    [OptionSetMetadata("User Fiscal Calendar", 1033)]
    UserFiscalCalendar = 1086,

    [EnumMember]
    [OptionSetMetadata("Authorization Server", 1033)]
    AuthorizationServer = 1094,

    [EnumMember]
    [OptionSetMetadata("Partner Application", 1033)]
    PartnerApplication = 1095,

    [EnumMember]
    [OptionSetMetadata("System Chart", 1033)]
    SystemChart = 1111,

    [EnumMember]
    [OptionSetMetadata("User Chart", 1033)]
    UserChart = 1112,

    [EnumMember]
    [OptionSetMetadata("Ribbon Tab To Command Mapping", 1033)]
    RibbonTabToCommandMapping = 1113,

    [EnumMember]
    [OptionSetMetadata("Ribbon Context Group", 1033)]
    RibbonContextGroup = 1115,

    [EnumMember]
    [OptionSetMetadata("Ribbon Command", 1033)]
    RibbonCommand = 1116,

    [EnumMember]
    [OptionSetMetadata("Ribbon Rule", 1033)]
    RibbonRule = 1117,

    [EnumMember]
    [OptionSetMetadata("Application Ribbons", 1033)]
    ApplicationRibbons = 1120,

    [EnumMember]
    [OptionSetMetadata("Ribbon Difference", 1033)]
    RibbonDifference = 1130,

    [EnumMember]
    [OptionSetMetadata("Replication Backlog", 1033)]
    ReplicationBacklog = 1140,

    [EnumMember]
    [OptionSetMetadata("Document Suggestions", 1033)]
    DocumentSuggestions = 1189,

    [EnumMember]
    [OptionSetMetadata("SuggestionCardTemplate", 1033)]
    SuggestionCardTemplate = 1190,

    [EnumMember]
    [OptionSetMetadata("Field Security Profile", 1033)]
    FieldSecurityProfile = 1200,

    [EnumMember]
    [OptionSetMetadata("Field Permission", 1033)]
    FieldPermission = 1201,

    [EnumMember]
    [OptionSetMetadata("Team Profiles", 1033)]
    TeamProfiles = 1203,

    [EnumMember]
    [OptionSetMetadata("Application", 1033)]
    Application = 1204,

    [EnumMember]
    [OptionSetMetadata("Channel Property Group", 1033)]
    ChannelPropertyGroup = 1234,

    [EnumMember]
    [OptionSetMetadata("Channel Property", 1033)]
    ChannelProperty = 1236,

    [EnumMember]
    [OptionSetMetadata("SocialInsightsConfiguration", 1033)]
    SocialInsightsConfiguration = 1300,

    [EnumMember]
    [OptionSetMetadata("Saved Organization Insights Configuration", 1033)]
    SavedOrganizationInsightsConfiguration = 1309,

    [EnumMember]
    [OptionSetMetadata("Sync Attribute Mapping Profile", 1033)]
    SyncAttributeMappingProfile = 1400,

    [EnumMember]
    [OptionSetMetadata("Sync Attribute Mapping", 1033)]
    SyncAttributeMapping = 1401,

    [EnumMember]
    [OptionSetMetadata("Team Sync-Attribute Mapping Profiles", 1033)]
    TeamSyncAttributeMappingProfiles = 1403,

    [EnumMember]
    [OptionSetMetadata("Principal Sync Attribute Map", 1033)]
    PrincipalSyncAttributeMap = 1404,

    [EnumMember]
    [OptionSetMetadata("Annual Fiscal Calendar", 1033)]
    AnnualFiscalCalendar = 2000,

    [EnumMember]
    [OptionSetMetadata("Semiannual Fiscal Calendar", 1033)]
    SemiannualFiscalCalendar = 2001,

    [EnumMember]
    [OptionSetMetadata("Quarterly Fiscal Calendar", 1033)]
    QuarterlyFiscalCalendar = 2002,

    [EnumMember]
    [OptionSetMetadata("Monthly Fiscal Calendar", 1033)]
    MonthlyFiscalCalendar = 2003,

    [EnumMember]
    [OptionSetMetadata("Fixed Monthly Fiscal Calendar", 1033)]
    FixedMonthlyFiscalCalendar = 2004,

    [EnumMember]
    [OptionSetMetadata("Email Template", 1033)]
    EmailTemplate = 2010,

    [EnumMember]
    [OptionSetMetadata("Unresolved Address", 1033)]
    UnresolvedAddress = 2012,

    [EnumMember]
    [OptionSetMetadata("Territory", 1033)]
    Territory = 2013,

    [EnumMember]
    [OptionSetMetadata("Theme", 1033)]
    Theme = 2015,

    [EnumMember]
    [OptionSetMetadata("User Mapping", 1033)]
    UserMapping = 2016,

    [EnumMember]
    [OptionSetMetadata("Queue", 1033)]
    Queue = 2020,

    [EnumMember]
    [OptionSetMetadata("QueueItemCount", 1033)]
    QueueItemCount = 2023,

    [EnumMember]
    [OptionSetMetadata("QueueMemberCount", 1033)]
    QueueMemberCount = 2024,

    [EnumMember]
    [OptionSetMetadata("License", 1033)]
    License = 2027,

    [EnumMember]
    [OptionSetMetadata("Queue Item", 1033)]
    QueueItem = 2029,

    [EnumMember]
    [OptionSetMetadata("User Entity UI Settings", 1033)]
    UserEntityUISettings = 2500,

    [EnumMember]
    [OptionSetMetadata("User Entity Instance Data", 1033)]
    UserEntityInstanceData = 2501,

    [EnumMember]
    [OptionSetMetadata("Integration Status", 1033)]
    IntegrationStatus = 3000,

    [EnumMember]
    [OptionSetMetadata("Channel Access Profile", 1033)]
    ChannelAccessProfile = 3005,

    [EnumMember]
    [OptionSetMetadata("External Party", 1033)]
    ExternalParty = 3008,

    [EnumMember]
    [OptionSetMetadata("Connection Role", 1033)]
    ConnectionRole = 3231,

    [EnumMember]
    [OptionSetMetadata("Connection Role Object Type Code", 1033)]
    ConnectionRoleObjectTypeCode = 3233,

    [EnumMember]
    [OptionSetMetadata("Connection", 1033)]
    Connection = 3234,

    [EnumMember]
    [OptionSetMetadata("Calendar", 1033)]
    Calendar = 4003,

    [EnumMember]
    [OptionSetMetadata("Calendar Rule", 1033)]
    CalendarRule = 4004,

    [EnumMember]
    [OptionSetMetadata("Inter Process Lock", 1033)]
    InterProcessLock = 4011,

    [EnumMember]
    [OptionSetMetadata("Email Hash", 1033)]
    EmailHash = 4023,

    [EnumMember]
    [OptionSetMetadata("Display String Map", 1033)]
    DisplayStringMap = 4101,

    [EnumMember]
    [OptionSetMetadata("Display String", 1033)]
    DisplayString = 4102,

    [EnumMember]
    [OptionSetMetadata("Notification", 1033)]
    Notification = 4110,

    [EnumMember]
    [OptionSetMetadata("Exchange Sync Id Mapping", 1033)]
    ExchangeSyncIdMapping = 4120,

    [EnumMember]
    [OptionSetMetadata("Activity", 1033)]
    Activity = 4200,

    [EnumMember]
    [OptionSetMetadata("Appointment", 1033)]
    Appointment = 4201,

    [EnumMember]
    [OptionSetMetadata("Email", 1033)]
    Email = 4202,

    [EnumMember]
    [OptionSetMetadata("Fax", 1033)]
    Fax = 4204,

    [EnumMember]
    [OptionSetMetadata("Letter", 1033)]
    Letter = 4207,

    [EnumMember]
    [OptionSetMetadata("Phone Call", 1033)]
    PhoneCall = 4210,

    [EnumMember]
    [OptionSetMetadata("Task", 1033)]
    Task = 4212,

    [EnumMember]
    [OptionSetMetadata("Social Activity", 1033)]
    SocialActivity = 4216,

    [EnumMember]
    [OptionSetMetadata("UntrackedEmail", 1033)]
    UntrackedEmail = 4220,

    [EnumMember]
    [OptionSetMetadata("Saved View", 1033)]
    SavedView = 4230,

    [EnumMember]
    [OptionSetMetadata("Metadata Difference", 1033)]
    MetadataDifference = 4231,

    [EnumMember]
    [OptionSetMetadata("Business Data Localized Label", 1033)]
    BusinessDataLocalizedLabel = 4232,

    [EnumMember]
    [OptionSetMetadata("Recurrence Rule", 1033)]
    RecurrenceRule = 4250,

    [EnumMember]
    [OptionSetMetadata("Recurring Appointment", 1033)]
    RecurringAppointment = 4251,

    [EnumMember]
    [OptionSetMetadata("Email Search", 1033)]
    EmailSearch = 4299,

    [EnumMember]
    [OptionSetMetadata("Data Import", 1033)]
    DataImport = 4410,

    [EnumMember]
    [OptionSetMetadata("Data Map", 1033)]
    DataMap = 4411,

    [EnumMember]
    [OptionSetMetadata("Import Source File", 1033)]
    ImportSourceFile = 4412,

    [EnumMember]
    [OptionSetMetadata("Import Data", 1033)]
    ImportData = 4413,

    [EnumMember]
    [OptionSetMetadata("Duplicate Detection Rule", 1033)]
    DuplicateDetectionRule = 4414,

    [EnumMember]
    [OptionSetMetadata("Duplicate Record", 1033)]
    DuplicateRecord = 4415,

    [EnumMember]
    [OptionSetMetadata("Duplicate Rule Condition", 1033)]
    DuplicateRuleCondition = 4416,

    [EnumMember]
    [OptionSetMetadata("Column Mapping", 1033)]
    ColumnMapping = 4417,

    [EnumMember]
    [OptionSetMetadata("List Value Mapping", 1033)]
    ListValueMapping = 4418,

    [EnumMember]
    [OptionSetMetadata("Lookup Mapping", 1033)]
    LookupMapping = 4419,

    [EnumMember]
    [OptionSetMetadata("Owner Mapping", 1033)]
    OwnerMapping = 4420,

    [EnumMember]
    [OptionSetMetadata("Import Log", 1033)]
    ImportLog = 4423,

    [EnumMember]
    [OptionSetMetadata("Bulk Delete Operation", 1033)]
    BulkDeleteOperation = 4424,

    [EnumMember]
    [OptionSetMetadata("Bulk Delete Failure", 1033)]
    BulkDeleteFailure = 4425,

    [EnumMember]
    [OptionSetMetadata("Transformation Mapping", 1033)]
    TransformationMapping = 4426,

    [EnumMember]
    [OptionSetMetadata("Transformation Parameter Mapping", 1033)]
    TransformationParameterMapping = 4427,

    [EnumMember]
    [OptionSetMetadata("Import Entity Mapping", 1033)]
    ImportEntityMapping = 4428,

    [EnumMember]
    [OptionSetMetadata("Data Performance Dashboard", 1033)]
    DataPerformanceDashboard = 4450,

    [EnumMember]
    [OptionSetMetadata("Office Document", 1033)]
    OfficeDocument = 4490,

    [EnumMember]
    [OptionSetMetadata("Relationship Role", 1033)]
    RelationshipRole = 4500,

    [EnumMember]
    [OptionSetMetadata("Relationship Role Map", 1033)]
    RelationshipRoleMap = 4501,

    [EnumMember]
    [OptionSetMetadata("Customer Relationship", 1033)]
    CustomerRelationship = 4502,

    [EnumMember]
    [OptionSetMetadata("Auditing", 1033)]
    Auditing = 4567,

    [EnumMember]
    [OptionSetMetadata("Ribbon Client Metadata.", 1033)]
    RibbonClientMetadata = 4579,

    [EnumMember]
    [OptionSetMetadata("Entity Map", 1033)]
    EntityMap = 4600,

    [EnumMember]
    [OptionSetMetadata("Attribute Map", 1033)]
    AttributeMap = 4601,

    [EnumMember]
    [OptionSetMetadata("Plug-in Type", 1033)]
    PluginType = 4602,

    [EnumMember]
    [OptionSetMetadata("Plug-in Type Statistic", 1033)]
    PluginTypeStatistic = 4603,

    [EnumMember]
    [OptionSetMetadata("Plug-in Assembly", 1033)]
    PluginAssembly = 4605,

    [EnumMember]
    [OptionSetMetadata("Sdk Message", 1033)]
    SdkMessage = 4606,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Filter", 1033)]
    SdkMessageFilter = 4607,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Processing Step", 1033)]
    SdkMessageProcessingStep = 4608,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Request", 1033)]
    SdkMessageRequest = 4609,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Response", 1033)]
    SdkMessageResponse = 4610,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Response Field", 1033)]
    SdkMessageResponseField = 4611,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Pair", 1033)]
    SdkMessagePair = 4613,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Request Field", 1033)]
    SdkMessageRequestField = 4614,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Processing Step Image", 1033)]
    SdkMessageProcessingStepImage = 4615,

    [EnumMember]
    [OptionSetMetadata("Sdk Message Processing Step Secure Configuration", 1033)]
    SdkMessageProcessingStepSecureConfiguration = 4616,

    [EnumMember]
    [OptionSetMetadata("Service Endpoint", 1033)]
    ServiceEndpoint = 4618,

    [EnumMember]
    [OptionSetMetadata("Plug-in Trace Log", 1033)]
    PluginTraceLog = 4619,

    [EnumMember]
    [OptionSetMetadata("System Job", 1033)]
    SystemJob = 4700,

    [EnumMember]
    [OptionSetMetadata("Workflow Wait Subscription", 1033)]
    WorkflowWaitSubscription = 4702,

    [EnumMember]
    [OptionSetMetadata("Process", 1033)]
    Process = 4703,

    [EnumMember]
    [OptionSetMetadata("Process Dependency", 1033)]
    ProcessDependency = 4704,

    [EnumMember]
    [OptionSetMetadata("ISV Config", 1033)]
    ISVConfig = 4705,

    [EnumMember]
    [OptionSetMetadata("Process Log", 1033)]
    ProcessLog = 4706,

    [EnumMember]
    [OptionSetMetadata("Application File", 1033)]
    ApplicationFile = 4707,

    [EnumMember]
    [OptionSetMetadata("Organization Statistic", 1033)]
    OrganizationStatistic = 4708,

    [EnumMember]
    [OptionSetMetadata("Site Map", 1033)]
    SiteMap = 4709,

    [EnumMember]
    [OptionSetMetadata("Process Session", 1033)]
    ProcessSession = 4710,

    [EnumMember]
    [OptionSetMetadata("Expander Event", 1033)]
    ExpanderEvent = 4711,

    [EnumMember]
    [OptionSetMetadata("Process Trigger", 1033)]
    ProcessTrigger = 4712,

    [EnumMember]
    [OptionSetMetadata("Flow Session", 1033)]
    FlowSession = 4720,

    [EnumMember]
    [OptionSetMetadata("Process Stage", 1033)]
    ProcessStage = 4724,

    [EnumMember]
    [OptionSetMetadata("Business Process Flow Instance", 1033)]
    BusinessProcessFlowInstance = 4725,

    [EnumMember]
    [OptionSetMetadata("Web Wizard", 1033)]
    WebWizard = 4800,

    [EnumMember]
    [OptionSetMetadata("Wizard Page", 1033)]
    WizardPage = 4802,

    [EnumMember]
    [OptionSetMetadata("Web Wizard Access Privilege", 1033)]
    WebWizardAccessPrivilege = 4803,

    [EnumMember]
    [OptionSetMetadata("Time Zone Definition", 1033)]
    TimeZoneDefinition = 4810,

    [EnumMember]
    [OptionSetMetadata("Time Zone Rule", 1033)]
    TimeZoneRule = 4811,

    [EnumMember]
    [OptionSetMetadata("Time Zone Localized Name", 1033)]
    TimeZoneLocalizedName = 4812,

    [EnumMember]
    [OptionSetMetadata("Recently Used", 1033)]
    RecentlyUsed = 5000,

    [EnumMember]
    [OptionSetMetadata("NL2SQ Registration Information", 1033)]
    NL2SQRegistrationInformation = 5004,

    [EnumMember]
    [OptionSetMetadata("Event Expander Breadcrumb", 1033)]
    EventExpanderBreadcrumb = 5006,

    [EnumMember]
    [OptionSetMetadata("System Application Metadata", 1033)]
    SystemApplicationMetadata = 7000,

    [EnumMember]
    [OptionSetMetadata("User Application Metadata", 1033)]
    UserApplicationMetadata = 7001,

    [EnumMember]
    [OptionSetMetadata("Solution", 1033)]
    Solution = 7100,

    [EnumMember]
    [OptionSetMetadata("Publisher", 1033)]
    Publisher = 7101,

    [EnumMember]
    [OptionSetMetadata("Publisher Address", 1033)]
    PublisherAddress = 7102,

    [EnumMember]
    [OptionSetMetadata("Solution Component", 1033)]
    SolutionComponent = 7103,

    [EnumMember]
    [OptionSetMetadata("Solution Component Definition", 1033)]
    SolutionComponentDefinition = 7104,

    [EnumMember]
    [OptionSetMetadata("Dependency", 1033)]
    Dependency = 7105,

    [EnumMember]
    [OptionSetMetadata("Dependency Node", 1033)]
    DependencyNode = 7106,

    [EnumMember]
    [OptionSetMetadata("Invalid Dependency", 1033)]
    InvalidDependency = 7107,

    [EnumMember]
    [OptionSetMetadata("Dependency Feature", 1033)]
    DependencyFeature = 7108,

    [EnumMember]
    [OptionSetMetadata("RuntimeDependency", 1033)]
    RuntimeDependency = 7200,

    [EnumMember]
    [OptionSetMetadata("ElasticFileAttachment", 1033)]
    ElasticFileAttachment = 7755,

    [EnumMember]
    [OptionSetMetadata("Post", 1033)]
    Post = 8000,

    [EnumMember]
    [OptionSetMetadata("Post Role", 1033)]
    PostRole = 8001,

    [EnumMember]
    [OptionSetMetadata("Post Regarding", 1033)]
    PostRegarding = 8002,

    [EnumMember]
    [OptionSetMetadata("Follow", 1033)]
    Follow = 8003,

    [EnumMember]
    [OptionSetMetadata("Comment", 1033)]
    Comment = 8005,

    [EnumMember]
    [OptionSetMetadata("Like", 1033)]
    Like = 8006,

    [EnumMember]
    [OptionSetMetadata("ACIViewMapper", 1033)]
    ACIViewMapper = 8040,

    [EnumMember]
    [OptionSetMetadata("Trace", 1033)]
    Trace = 8050,

    [EnumMember]
    [OptionSetMetadata("Trace Association", 1033)]
    TraceAssociation = 8051,

    [EnumMember]
    [OptionSetMetadata("Trace Regarding", 1033)]
    TraceRegarding = 8052,

    [EnumMember]
    [OptionSetMetadata("Routing Rule Set", 1033)]
    RoutingRuleSet = 8181,

    [EnumMember]
    [OptionSetMetadata("Rule Item", 1033)]
    RuleItem = 8199,

    [EnumMember]
    [OptionSetMetadata("AppModule Metadata", 1033)]
    AppModuleMetadata = 8700,

    [EnumMember]
    [OptionSetMetadata("AppModule Metadata Dependency", 1033)]
    AppModuleMetadataDependency = 8701,

    [EnumMember]
    [OptionSetMetadata("AppModule Metadata Async Operation", 1033)]
    AppModuleMetadataAsyncOperation = 8702,

    [EnumMember]
    [OptionSetMetadata("Hierarchy Rule", 1033)]
    HierarchyRule = 8840,

    [EnumMember]
    [OptionSetMetadata("Model-driven App", 1033)]
    ModeldrivenApp = 9006,

    [EnumMember]
    [OptionSetMetadata("App Module Component", 1033)]
    AppModuleComponent = 9007,

    [EnumMember]
    [OptionSetMetadata("App Module Roles", 1033)]
    AppModuleRoles = 9009,

    [EnumMember]
    [OptionSetMetadata("App Config Master", 1033)]
    AppConfigMaster = 9011,

    [EnumMember]
    [OptionSetMetadata("App Configuration", 1033)]
    AppConfiguration = 9012,

    [EnumMember]
    [OptionSetMetadata("App Configuration Instance", 1033)]
    AppConfigurationInstance = 9013,

    [EnumMember]
    [OptionSetMetadata("Report", 1033)]
    Report = 9100,

    [EnumMember]
    [OptionSetMetadata("Report Related Entity", 1033)]
    ReportRelatedEntity = 9101,

    [EnumMember]
    [OptionSetMetadata("Report Related Category", 1033)]
    ReportRelatedCategory = 9102,

    [EnumMember]
    [OptionSetMetadata("Report Visibility", 1033)]
    ReportVisibility = 9103,

    [EnumMember]
    [OptionSetMetadata("Report Link", 1033)]
    ReportLink = 9104,

    [EnumMember]
    [OptionSetMetadata("Currency", 1033)]
    Currency = 9105,

    [EnumMember]
    [OptionSetMetadata("Mail Merge Template", 1033)]
    MailMergeTemplate = 9106,

    [EnumMember]
    [OptionSetMetadata("Import Job", 1033)]
    ImportJob = 9107,

    [EnumMember]
    [OptionSetMetadata("LocalConfigStore", 1033)]
    LocalConfigStore = 9201,

    [EnumMember]
    [OptionSetMetadata("Record Creation and Update Rule", 1033)]
    RecordCreationandUpdateRule = 9300,

    [EnumMember]
    [OptionSetMetadata("Record Creation and Update Rule Item", 1033)]
    RecordCreationandUpdateRuleItem = 9301,

    [EnumMember]
    [OptionSetMetadata("Web Resource", 1033)]
    WebResource = 9333,

    [EnumMember]
    [OptionSetMetadata("Channel Access Profile Rule", 1033)]
    ChannelAccessProfileRule = 9400,

    [EnumMember]
    [OptionSetMetadata("Channel Access Profile Rule Item", 1033)]
    ChannelAccessProfileRuleItem = 9401,

    [EnumMember]
    [OptionSetMetadata("SharePoint Site", 1033)]
    SharePointSite = 9502,

    [EnumMember]
    [OptionSetMetadata("Sharepoint Document", 1033)]
    SharepointDocument = 9507,

    [EnumMember]
    [OptionSetMetadata("Document Location", 1033)]
    DocumentLocation = 9508,

    [EnumMember]
    [OptionSetMetadata("SharePoint Data", 1033)]
    SharePointData = 9509,

    [EnumMember]
    [OptionSetMetadata("Rollup Properties", 1033)]
    RollupProperties = 9510,

    [EnumMember]
    [OptionSetMetadata("Rollup Job", 1033)]
    RollupJob = 9511,

    [EnumMember]
    [OptionSetMetadata("Goal", 1033)]
    Goal = 9600,

    [EnumMember]
    [OptionSetMetadata("Rollup Query", 1033)]
    RollupQuery = 9602,

    [EnumMember]
    [OptionSetMetadata("Goal Metric", 1033)]
    GoalMetric = 9603,

    [EnumMember]
    [OptionSetMetadata("Rollup Field", 1033)]
    RollupField = 9604,

    [EnumMember]
    [OptionSetMetadata("Email Server Profile", 1033)]
    EmailServerProfile = 9605,

    [EnumMember]
    [OptionSetMetadata("Mailbox", 1033)]
    Mailbox = 9606,

    [EnumMember]
    [OptionSetMetadata("Mailbox Statistics", 1033)]
    MailboxStatistics = 9607,

    [EnumMember]
    [OptionSetMetadata("Mailbox Auto Tracking Folder", 1033)]
    MailboxAutoTrackingFolder = 9608,

    [EnumMember]
    [OptionSetMetadata("Mailbox Tracking Category", 1033)]
    MailboxTrackingCategory = 9609,

    [EnumMember]
    [OptionSetMetadata("Process Configuration", 1033)]
    ProcessConfiguration = 9650,

    [EnumMember]
    [OptionSetMetadata("Organization Insights Notification", 1033)]
    OrganizationInsightsNotification = 9690,

    [EnumMember]
    [OptionSetMetadata("Organization Insights Metric", 1033)]
    OrganizationInsightsMetric = 9699,

    [EnumMember]
    [OptionSetMetadata("SLA", 1033)]
    SLA = 9750,

    [EnumMember]
    [OptionSetMetadata("SLA Item", 1033)]
    SLAItem = 9751,

    [EnumMember]
    [OptionSetMetadata("SLA KPI Instance", 1033)]
    SLAKPIInstance = 9752,

    [EnumMember]
    [OptionSetMetadata("Custom Control", 1033)]
    CustomControl = 9753,

    [EnumMember]
    [OptionSetMetadata("Custom Control Resource", 1033)]
    CustomControlResource = 9754,

    [EnumMember]
    [OptionSetMetadata("Custom Control Default Config", 1033)]
    CustomControlDefaultConfig = 9755,

    [EnumMember]
    [OptionSetMetadata("Entity", 1033)]
    Entity = 9800,

    [EnumMember]
    [OptionSetMetadata("Attribute", 1033)]
    Attribute = 9808,

    [EnumMember]
    [OptionSetMetadata("OptionSet", 1033)]
    OptionSet = 9809,

    [EnumMember]
    [OptionSetMetadata("Entity Key", 1033)]
    EntityKey = 9810,

    [EnumMember]
    [OptionSetMetadata("Entity Relationship", 1033)]
    EntityRelationship = 9811,

    [EnumMember]
    [OptionSetMetadata("Managed Property", 1033)]
    ManagedProperty = 9812,

    [EnumMember]
    [OptionSetMetadata("Relationship Entity", 1033)]
    RelationshipEntity = 9813,

    [EnumMember]
    [OptionSetMetadata("Relationship Attribute", 1033)]
    RelationshipAttribute = 9814,

    [EnumMember]
    [OptionSetMetadata("Entity Index", 1033)]
    EntityIndex = 9815,

    [EnumMember]
    [OptionSetMetadata("Index Attribute", 1033)]
    IndexAttribute = 9816,

    [EnumMember]
    [OptionSetMetadata("Option Set Value", 1033)]
    OptionSetValue = 9817,

    [EnumMember]
    [OptionSetMetadata("Secured Masking Column", 1033)]
    SecuredMaskingColumn = 9820,

    [EnumMember]
    [OptionSetMetadata("Mobile Offline Profile", 1033)]
    MobileOfflineProfile = 9866,

    [EnumMember]
    [OptionSetMetadata("Mobile Offline Profile Item", 1033)]
    MobileOfflineProfileItem = 9867,

    [EnumMember]
    [OptionSetMetadata("Mobile Offline Profile Item Association", 1033)]
    MobileOfflineProfileItemAssociation = 9868,

    [EnumMember]
    [OptionSetMetadata("Sync Error", 1033)]
    SyncError = 9869,

    [EnumMember]
    [OptionSetMetadata("Offline Command Definition", 1033)]
    OfflineCommandDefinition = 9870,

    [EnumMember]
    [OptionSetMetadata("Language Provisioning State", 1033)]
    LanguageProvisioningState = 9875,

    [EnumMember]
    [OptionSetMetadata("Ribbon Metadata To Process", 1033)]
    RibbonMetadataToProcess = 9880,

    [EnumMember]
    [OptionSetMetadata("SolutionHistoryData", 1033)]
    SolutionHistoryData = 9890,

    [EnumMember]
    [OptionSetMetadata("Navigation Setting", 1033)]
    NavigationSetting = 9900,

    [EnumMember]
    [OptionSetMetadata("MultiEntitySearch", 1033)]
    MultiEntitySearch = 9910,

    [EnumMember]
    [OptionSetMetadata("Multi Select Option Value", 1033)]
    MultiSelectOptionValue = 9912,

    [EnumMember]
    [OptionSetMetadata("Hierarchy Security Configuration", 1033)]
    HierarchySecurityConfiguration = 9919,

    [EnumMember]
    [OptionSetMetadata("Knowledge Base Record", 1033)]
    KnowledgeBaseRecord = 9930,

    [EnumMember]
    [OptionSetMetadata("Time Stamp Date Mapping", 1033)]
    TimeStampDateMapping = 9932,

    [EnumMember]
    [OptionSetMetadata("Azure Service Connection", 1033)]
    AzureServiceConnection = 9936,

    [EnumMember]
    [OptionSetMetadata("Document Template", 1033)]
    DocumentTemplate = 9940,

    [EnumMember]
    [OptionSetMetadata("Personal Document Template", 1033)]
    PersonalDocumentTemplate = 9941,

    [EnumMember]
    [OptionSetMetadata("Text Analytics Entity Mapping", 1033)]
    TextAnalyticsEntityMapping = 9945,

    [EnumMember]
    [OptionSetMetadata("Knowledge Search Model", 1033)]
    KnowledgeSearchModel = 9947,

    [EnumMember]
    [OptionSetMetadata("Advanced Similarity Rule", 1033)]
    AdvancedSimilarityRule = 9949,

    [EnumMember]
    [OptionSetMetadata("Office Graph Document", 1033)]
    OfficeGraphDocument = 9950,

    [EnumMember]
    [OptionSetMetadata("Similarity Rule", 1033)]
    SimilarityRule = 9951,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article", 1033)]
    KnowledgeArticle = 9953,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article Views", 1033)]
    KnowledgeArticleViews = 9955,

    [EnumMember]
    [OptionSetMetadata("Language", 1033)]
    Language = 9957,

    [EnumMember]
    [OptionSetMetadata("Feedback", 1033)]
    Feedback = 9958,

    [EnumMember]
    [OptionSetMetadata("Category", 1033)]
    Category = 9959,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article Category", 1033)]
    KnowledgeArticleCategory = 9960,

    [EnumMember]
    [OptionSetMetadata("DelveActionHub", 1033)]
    DelveActionHub = 9961,

    [EnumMember]
    [OptionSetMetadata("Action Card", 1033)]
    ActionCard = 9962,

    [EnumMember]
    [OptionSetMetadata("ActionCardUserState", 1033)]
    ActionCardUserState = 9968,

    [EnumMember]
    [OptionSetMetadata("Action Card User Settings", 1033)]
    ActionCardUserSettings = 9973,

    [EnumMember]
    [OptionSetMetadata("Action Card Type", 1033)]
    ActionCardType = 9983,

    [EnumMember]
    [OptionSetMetadata("Interaction for Email", 1033)]
    InteractionforEmail = 9986,

    [EnumMember]
    [OptionSetMetadata("External Party Item", 1033)]
    ExternalPartyItem = 9987,

    [EnumMember]
    [OptionSetMetadata("HolidayWrapper", 1033)]
    HolidayWrapper = 9996,

    [EnumMember]
    [OptionSetMetadata("Email Signature", 1033)]
    EmailSignature = 9997,

    [EnumMember]
    [OptionSetMetadata("Solution Component Attribute Configuration", 1033)]
    SolutionComponentAttributeConfiguration = 10000,

    [EnumMember]
    [OptionSetMetadata("Solution Component Batch Configuration", 1033)]
    SolutionComponentBatchConfiguration = 10001,

    [EnumMember]
    [OptionSetMetadata("Solution Component Configuration", 1033)]
    SolutionComponentConfiguration = 10002,

    [EnumMember]
    [OptionSetMetadata("Solution Component Relationship Configuration", 1033)]
    SolutionComponentRelationshipConfiguration = 10003,

    [EnumMember]
    [OptionSetMetadata("Solution History", 1033)]
    SolutionHistory = 10004,

    [EnumMember]
    [OptionSetMetadata("Solution History Data Source", 1033)]
    SolutionHistoryDataSource = 10005,

    [EnumMember]
    [OptionSetMetadata("Component Layer", 1033)]
    ComponentLayer = 10006,

    [EnumMember]
    [OptionSetMetadata("Component Layer Data Source", 1033)]
    ComponentLayerDataSource = 10007,

    [EnumMember]
    [OptionSetMetadata("StageSolutionUpload", 1033)]
    StageSolutionUpload = 10008,

    [EnumMember]
    [OptionSetMetadata("ExportSolutionUpload", 1033)]
    ExportSolutionUpload = 10009,

    [EnumMember]
    [OptionSetMetadata("FeatureControlSetting", 1033)]
    FeatureControlSetting = 10010,

    [EnumMember]
    [OptionSetMetadata("Package", 1033)]
    Package = 10011,

    [EnumMember]
    [OptionSetMetadata("Package History", 1033)]
    PackageHistory = 10012,

    [EnumMember]
    [OptionSetMetadata("Solution Component Summary", 1033)]
    SolutionComponentSummary = 10014,

    [EnumMember]
    [OptionSetMetadata("Solution Component Count Summary", 1033)]
    SolutionComponentCountSummary = 10015,

    [EnumMember]
    [OptionSetMetadata("Solution Component Data Source", 1033)]
    SolutionComponentDataSource = 10016,

    [EnumMember]
    [OptionSetMetadata("Solution Component Count Data Source", 1033)]
    SolutionComponentCountDataSource = 10017,

    [EnumMember]
    [OptionSetMetadata("Microsoft Entra ID", 1033)]
    MicrosoftEntraID = 10018,

    [EnumMember]
    [OptionSetMetadata("Staged attribute lookup value", 1033)]
    Stagedattributelookupvalue = 10019,

    [EnumMember]
    [OptionSetMetadata("Staged attribute picklist value", 1033)]
    Stagedattributepicklistvalue = 10020,

    [EnumMember]
    [OptionSetMetadata("Staged Entity", 1033)]
    StagedEntity = 10021,

    [EnumMember]
    [OptionSetMetadata("Staged Entity Attribute", 1033)]
    StagedEntityAttribute = 10022,

    [EnumMember]
    [OptionSetMetadata("Staged entity relationship", 1033)]
    Stagedentityrelationship = 10023,

    [EnumMember]
    [OptionSetMetadata("Staged entity relationship relationships", 1033)]
    Stagedentityrelationshiprelationships = 10024,

    [EnumMember]
    [OptionSetMetadata("Staged entity relationship role", 1033)]
    Stagedentityrelationshiprole = 10025,

    [EnumMember]
    [OptionSetMetadata("Staged Metadata Async Operation", 1033)]
    StagedMetadataAsyncOperation = 10026,

    [EnumMember]
    [OptionSetMetadata("Staged optionset", 1033)]
    Stagedoptionset = 10027,

    [EnumMember]
    [OptionSetMetadata("Staged relationship", 1033)]
    Stagedrelationship_2 = 10028,

    [EnumMember]
    [OptionSetMetadata("Staged relationship", 1033)]
    Stagedrelationship_1 = 10029,

    [EnumMember]
    [OptionSetMetadata("Staged relationship", 1033)]
    Stagedrelationship = 10030,

    [EnumMember]
    [OptionSetMetadata("Attribute Cluster Config", 1033)]
    AttributeClusterConfig = 10031,

    [EnumMember]
    [OptionSetMetadata("Entity Cluster Configuration", 1033)]
    EntityClusterConfiguration = 10032,

    [EnumMember]
    [OptionSetMetadata("Key Vault Reference", 1033)]
    KeyVaultReference = 10033,

    [EnumMember]
    [OptionSetMetadata("Managed Identity", 1033)]
    ManagedIdentity = 10034,

    [EnumMember]
    [OptionSetMetadata("Catalog", 1033)]
    Catalog = 10035,

    [EnumMember]
    [OptionSetMetadata("Catalog Assignment", 1033)]
    CatalogAssignment = 10036,

    [EnumMember]
    [OptionSetMetadata("Internal Catalog Assignment", 1033)]
    InternalCatalogAssignment = 10037,

    [EnumMember]
    [OptionSetMetadata("Custom API", 1033)]
    CustomAPI = 10038,

    [EnumMember]
    [OptionSetMetadata("Custom API Request Parameter", 1033)]
    CustomAPIRequestParameter = 10039,

    [EnumMember]
    [OptionSetMetadata("Custom API Response Property", 1033)]
    CustomAPIResponseProperty = 10040,

    [EnumMember]
    [OptionSetMetadata("Plugin Package", 1033)]
    PluginPackage = 10041,

    [EnumMember]
    [OptionSetMetadata("Sensitivity Label", 1033)]
    SensitivityLabel = 10042,

    [EnumMember]
    [OptionSetMetadata("NonRelational Data Source", 1033)]
    NonRelationalDataSource = 10043,

    [EnumMember]
    [OptionSetMetadata("ProvisionLanguageForUser", 1033)]
    ProvisionLanguageForUser = 10044,

    [EnumMember]
    [OptionSetMetadata("Purview Label Info", 1033)]
    PurviewLabelInfo = 10045,

    [EnumMember]
    [OptionSetMetadata("Purview Label Sync Cache", 1033)]
    PurviewLabelSyncCache = 10046,

    [EnumMember]
    [OptionSetMetadata("Sensitivity Label Attribute Mapping", 1033)]
    SensitivityLabelAttributeMapping = 10047,

    [EnumMember]
    [OptionSetMetadata("Entity Storage Profile", 1033)]
    EntityStorageProfile = 10048,

    [EnumMember]
    [OptionSetMetadata("Policy Criterion", 1033)]
    PolicyCriterion = 10049,

    [EnumMember]
    [OptionSetMetadata("App Notification Signal", 1033)]
    AppNotificationSignal = 10050,

    [EnumMember]
    [OptionSetMetadata("Shared Object", 1033)]
    SharedObject = 10051,

    [EnumMember]
    [OptionSetMetadata("Shared Workspace", 1033)]
    SharedWorkspace = 10052,

    [EnumMember]
    [OptionSetMetadata("Shared Workspace Access Token", 1033)]
    SharedWorkspaceAccessToken = 10053,

    [EnumMember]
    [OptionSetMetadata("Shared Workspace Pool", 1033)]
    SharedWorkspacePool = 10054,

    [EnumMember]
    [OptionSetMetadata("Data Lake Folder", 1033)]
    DataLakeFolder = 10055,

    [EnumMember]
    [OptionSetMetadata("Data Lake Folder Permission", 1033)]
    DataLakeFolderPermission = 10056,

    [EnumMember]
    [OptionSetMetadata("Data Lake Workspace", 1033)]
    DataLakeWorkspace = 10057,

    [EnumMember]
    [OptionSetMetadata("Data Lake Workspace Permission", 1033)]
    DataLakeWorkspacePermission = 10058,

    [EnumMember]
    [OptionSetMetadata("Data Processing configuration", 1033)]
    DataProcessingconfiguration = 10059,

    [EnumMember]
    [OptionSetMetadata("Exported Excel", 1033)]
    ExportedExcel = 10060,

    [EnumMember]
    [OptionSetMetadata("RetainedData Excel", 1033)]
    RetainedDataExcel = 10061,

    [EnumMember]
    [OptionSetMetadata("Synapse Database", 1033)]
    SynapseDatabase = 10062,

    [EnumMember]
    [OptionSetMetadata("AthenaLtrMigrationState", 1033)]
    AthenaLtrMigrationState = 10063,

    [EnumMember]
    [OptionSetMetadata("AthenaReconciliationInfo", 1033)]
    AthenaReconciliationInfo = 10064,

    [EnumMember]
    [OptionSetMetadata("Synapse Link External Table State", 1033)]
    SynapseLinkExternalTableState = 10065,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Monitoring Daily Summary", 1033)]
    SynapseLinkMonitoringDailySummary = 10066,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Monitoring Link Status", 1033)]
    SynapseLinkMonitoringLinkStatus = 10067,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Profile", 1033)]
    SynapseLinkProfile = 10068,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Profile Entity", 1033)]
    SynapseLinkProfileEntity = 10069,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Profile Entity State", 1033)]
    SynapseLinkProfileEntityState = 10070,

    [EnumMember]
    [OptionSetMetadata("Synapse Link Schedule", 1033)]
    SynapseLinkSchedule = 10071,

    [EnumMember]
    [OptionSetMetadata("Component Changeset Payload", 1033)]
    ComponentChangesetPayload = 10072,

    [EnumMember]
    [OptionSetMetadata("Component Changeset Version", 1033)]
    ComponentChangesetVersion = 10073,

    [EnumMember]
    [OptionSetMetadata("Component Version", 1033)]
    ComponentVersion = 10074,

    [EnumMember]
    [OptionSetMetadata("Component Version Data Source", 1033)]
    ComponentVersionDataSource = 10075,

    [EnumMember]
    [OptionSetMetadata("Component Version (Internal)", 1033)]
    ComponentVersionInternal = 10076,

    [EnumMember]
    [OptionSetMetadata("DataflowRefreshHistory", 1033)]
    DataflowRefreshHistory = 10077,

    [EnumMember]
    [OptionSetMetadata("EntityRefreshHistory", 1033)]
    EntityRefreshHistory = 10078,

    [EnumMember]
    [OptionSetMetadata("Shared Link Setting", 1033)]
    SharedLinkSetting = 10079,

    [EnumMember]
    [OptionSetMetadata("Any Privilege Entity", 1033)]
    AnyPrivilegeEntity = 10080,

    [EnumMember]
    [OptionSetMetadata("DelegatedAuthorization", 1033)]
    DelegatedAuthorization = 10081,

    [EnumMember]
    [OptionSetMetadata("CascadeGrantRevokeAccessRecordsTracker", 1033)]
    CascadeGrantRevokeAccessRecordsTracker = 10083,

    [EnumMember]
    [OptionSetMetadata("CascadeGrantRevokeAccessVersionTracker", 1033)]
    CascadeGrantRevokeAccessVersionTracker = 10084,

    [EnumMember]
    [OptionSetMetadata("RevokeInheritedAccessRecordsTracker", 1033)]
    RevokeInheritedAccessRecordsTracker = 10085,

    [EnumMember]
    [OptionSetMetadata("TdsMetadata", 1033)]
    TdsMetadata = 10086,

    [EnumMember]
    [OptionSetMetadata("Model-Driven App Element", 1033)]
    ModelDrivenAppElement = 10087,

    [EnumMember]
    [OptionSetMetadata("Model-Driven App Component Node's Edge", 1033)]
    ModelDrivenAppComponentNodesEdge = 10088,

    [EnumMember]
    [OptionSetMetadata("Model-Driven App Component Node", 1033)]
    ModelDrivenAppComponentNode = 10089,

    [EnumMember]
    [OptionSetMetadata("Model-Driven App Setting", 1033)]
    ModelDrivenAppSetting = 10090,

    [EnumMember]
    [OptionSetMetadata("Model-Driven App User Setting", 1033)]
    ModelDrivenAppUserSetting = 10091,

    [EnumMember]
    [OptionSetMetadata("Organization Setting", 1033)]
    OrganizationSetting = 10092,

    [EnumMember]
    [OptionSetMetadata("Setting Definition", 1033)]
    SettingDefinition = 10093,

    [EnumMember]
    [OptionSetMetadata("CanvasApp Extended Metadata", 1033)]
    CanvasAppExtendedMetadata = 10094,

    [EnumMember]
    [OptionSetMetadata("Service Plan Mapping", 1033)]
    ServicePlanMapping = 10095,

    [EnumMember]
    [OptionSetMetadata("Service Plan Custom Control", 1033)]
    ServicePlanCustomControl = 10096,

    [EnumMember]
    [OptionSetMetadata("ApplicationUser", 1033)]
    ApplicationUser = 10098,

    [EnumMember]
    [OptionSetMetadata("Git Branch", 1033)]
    GitBranch = 10101,

    [EnumMember]
    [OptionSetMetadata("Git Configuration Retrieval Data Source", 1033)]
    GitConfigurationRetrievalDataSource = 10102,

    [EnumMember]
    [OptionSetMetadata("GitHubAppConfig", 1033)]
    GitHubAppConfig = 10103,

    [EnumMember]
    [OptionSetMetadata("Git Organization", 1033)]
    GitOrganization = 10104,

    [EnumMember]
    [OptionSetMetadata("Git Project", 1033)]
    GitProject = 10105,

    [EnumMember]
    [OptionSetMetadata("Git Repository", 1033)]
    GitRepository = 10106,

    [EnumMember]
    [OptionSetMetadata("Git Solution", 1033)]
    GitSolution = 10107,

    [EnumMember]
    [OptionSetMetadata("Source Control Branch Configuration", 1033)]
    SourceControlBranchConfiguration = 10108,

    [EnumMember]
    [OptionSetMetadata("Source Control Component", 1033)]
    SourceControlComponent = 10109,

    [EnumMember]
    [OptionSetMetadata("Source Control Component Payload", 1033)]
    SourceControlComponentPayload = 10110,

    [EnumMember]
    [OptionSetMetadata("Source Control Configuration", 1033)]
    SourceControlConfiguration = 10111,

    [EnumMember]
    [OptionSetMetadata("Source Control Operation Tracking", 1033)]
    SourceControlOperationTracking = 10112,

    [EnumMember]
    [OptionSetMetadata("Staged Source Control Component", 1033)]
    StagedSourceControlComponent = 10113,

    [EnumMember]
    [OptionSetMetadata("OData v4 Data Source", 1033)]
    ODatav4DataSource = 10114,

    [EnumMember]
    [OptionSetMetadata("Workflow Binary", 1033)]
    WorkflowBinary = 10115,

    [EnumMember]
    [OptionSetMetadata("Flow Group", 1033)]
    FlowGroup = 10116,

    [EnumMember]
    [OptionSetMetadata("Business Process", 1033)]
    BusinessProcess = 10117,

    [EnumMember]
    [OptionSetMetadata("ComputerUseAgent", 1033)]
    ComputerUseAgent = 10118,

    [EnumMember]
    [OptionSetMetadata("Credential", 1033)]
    Credential = 10119,

    [EnumMember]
    [OptionSetMetadata("Desktop Flow Module", 1033)]
    DesktopFlowModule = 10120,

    [EnumMember]
    [OptionSetMetadata("Flow Capacity Assignment", 1033)]
    FlowCapacityAssignment = 10121,

    [EnumMember]
    [OptionSetMetadata("Flow Credential Application", 1033)]
    FlowCredentialApplication = 10122,

    [EnumMember]
    [OptionSetMetadata("Flow Event", 1033)]
    FlowEvent = 10123,

    [EnumMember]
    [OptionSetMetadata("Flow Machine", 1033)]
    FlowMachine = 10124,

    [EnumMember]
    [OptionSetMetadata("Flow Machine Group", 1033)]
    FlowMachineGroup = 10125,

    [EnumMember]
    [OptionSetMetadata("Flow Machine Image", 1033)]
    FlowMachineImage = 10126,

    [EnumMember]
    [OptionSetMetadata("Flow Machine Image Version", 1033)]
    FlowMachineImageVersion = 10127,

    [EnumMember]
    [OptionSetMetadata("Flow Machine Network", 1033)]
    FlowMachineNetwork = 10128,

    [EnumMember]
    [OptionSetMetadata("Flow Session Binary", 1033)]
    FlowSessionBinary = 10129,

    [EnumMember]
    [OptionSetMetadata("Flow Test Session", 1033)]
    FlowTestSession = 10130,

    [EnumMember]
    [OptionSetMetadata("Flow Trigger", 1033)]
    FlowTrigger = 10131,

    [EnumMember]
    [OptionSetMetadata("Flow Trigger Instance", 1033)]
    FlowTriggerInstance = 10132,

    [EnumMember]
    [OptionSetMetadata("ProcessStageParameter", 1033)]
    ProcessStageParameter = 10133,

    [EnumMember]
    [OptionSetMetadata("Saving Rule", 1033)]
    SavingRule = 10134,

    [EnumMember]
    [OptionSetMetadata("Tag", 1033)]
    Tag = 10135,

    [EnumMember]
    [OptionSetMetadata("Tagged Flow Session", 1033)]
    TaggedFlowSession = 10136,

    [EnumMember]
    [OptionSetMetadata("Tagged Process", 1033)]
    TaggedProcess = 10137,

    [EnumMember]
    [OptionSetMetadata("Workflow Metadata", 1033)]
    WorkflowMetadata = 10138,

    [EnumMember]
    [OptionSetMetadata("Work Queue", 1033)]
    WorkQueue = 10139,

    [EnumMember]
    [OptionSetMetadata("Work Queue Item", 1033)]
    WorkQueueItem = 10140,

    [EnumMember]
    [OptionSetMetadata("Desktop Flow Binary", 1033)]
    DesktopFlowBinary = 10141,

    [EnumMember]
    [OptionSetMetadata("Flow Aggregation", 1033)]
    FlowAggregation = 10142,

    [EnumMember]
    [OptionSetMetadata("Flow Log", 1033)]
    FlowLog = 10143,

    [EnumMember]
    [OptionSetMetadata("Flow Run", 1033)]
    FlowRun = 10144,

    [EnumMember]
    [OptionSetMetadata("Approval Process", 1033)]
    ApprovalProcess = 10145,

    [EnumMember]
    [OptionSetMetadata("Approval Stage Approval", 1033)]
    ApprovalStageApproval = 10146,

    [EnumMember]
    [OptionSetMetadata("Approval Stage Condition", 1033)]
    ApprovalStageCondition = 10147,

    [EnumMember]
    [OptionSetMetadata("Approval Stage Intelligent", 1033)]
    ApprovalStageIntelligent = 10148,

    [EnumMember]
    [OptionSetMetadata("Approval Stage Order", 1033)]
    ApprovalStageOrder = 10149,

    [EnumMember]
    [OptionSetMetadata("Action Approval Model", 1033)]
    ActionApprovalModel = 10150,

    [EnumMember]
    [OptionSetMetadata("Approval", 1033)]
    Approval = 10151,

    [EnumMember]
    [OptionSetMetadata("Approval Request", 1033)]
    ApprovalRequest = 10152,

    [EnumMember]
    [OptionSetMetadata("Approval Response", 1033)]
    ApprovalResponse = 10153,

    [EnumMember]
    [OptionSetMetadata("Approval Step", 1033)]
    ApprovalStep = 10154,

    [EnumMember]
    [OptionSetMetadata("Await All Action Approval Model", 1033)]
    AwaitAllActionApprovalModel = 10155,

    [EnumMember]
    [OptionSetMetadata("Await All Approval Model", 1033)]
    AwaitAllApprovalModel = 10156,

    [EnumMember]
    [OptionSetMetadata("Basic Approval Model Data", 1033)]
    BasicApprovalModelData = 10157,

    [EnumMember]
    [OptionSetMetadata("Flow Approval", 1033)]
    FlowApproval = 10158,

    [EnumMember]
    [OptionSetMetadata("Connection Reference", 1033)]
    ConnectionReference = 10167,

    [EnumMember]
    [OptionSetMetadata("Knowledge Source Consumer", 1033)]
    KnowledgeSourceConsumer = 10168,

    [EnumMember]
    [OptionSetMetadata("Knowledge Source Profile", 1033)]
    KnowledgeSourceProfile = 10169,

    [EnumMember]
    [OptionSetMetadata("UnstructuredFileSearchEntity", 1033)]
    UnstructuredFileSearchEntity = 10170,

    [EnumMember]
    [OptionSetMetadata("UnstructuredFileSearchRecord", 1033)]
    UnstructuredFileSearchRecord = 10171,

    [EnumMember]
    [OptionSetMetadata("UnstructuredFileSearchRecordStatus", 1033)]
    UnstructuredFileSearchRecordStatus = 10172,

    [EnumMember]
    [OptionSetMetadata("DVFileSearch", 1033)]
    DVFileSearch = 10173,

    [EnumMember]
    [OptionSetMetadata("DVFileSearchAttribute", 1033)]
    DVFileSearchAttribute = 10174,

    [EnumMember]
    [OptionSetMetadata("DVFileSearchEntity", 1033)]
    DVFileSearchEntity = 10175,

    [EnumMember]
    [OptionSetMetadata("DVTableSearch", 1033)]
    DVTableSearch = 10176,

    [EnumMember]
    [OptionSetMetadata("DVTableSearchAttribute", 1033)]
    DVTableSearchAttribute = 10177,

    [EnumMember]
    [OptionSetMetadata("DVTableSearchEntity", 1033)]
    DVTableSearchEntity = 10178,

    [EnumMember]
    [OptionSetMetadata("AICopilot", 1033)]
    AICopilot = 10179,

    [EnumMember]
    [OptionSetMetadata("AIPluginAuth", 1033)]
    AIPluginAuth = 10180,

    [EnumMember]
    [OptionSetMetadata("AI Plugin Conversation Starter", 1033)]
    AIPluginConversationStarter = 10181,

    [EnumMember]
    [OptionSetMetadata("AI Plugin Conversation Starter Mapping", 1033)]
    AIPluginConversationStarterMapping = 10182,

    [EnumMember]
    [OptionSetMetadata("AI Plugin Governance", 1033)]
    AIPluginGovernance = 10183,

    [EnumMember]
    [OptionSetMetadata("AI Plugin Governance Extended", 1033)]
    AIPluginGovernanceExtended = 10184,

    [EnumMember]
    [OptionSetMetadata("AIPluginOperationResponseTemplate", 1033)]
    AIPluginOperationResponseTemplate = 10185,

    [EnumMember]
    [OptionSetMetadata("AIPluginTitle", 1033)]
    AIPluginTitle = 10186,

    [EnumMember]
    [OptionSetMetadata("SideloadedAIPlugin", 1033)]
    SideloadedAIPlugin = 10187,

    [EnumMember]
    [OptionSetMetadata("AIPlugin", 1033)]
    AIPlugin = 10188,

    [EnumMember]
    [OptionSetMetadata("AIPluginExternalSchema", 1033)]
    AIPluginExternalSchema = 10189,

    [EnumMember]
    [OptionSetMetadata("AIPluginExternalSchemaProperty", 1033)]
    AIPluginExternalSchemaProperty = 10190,

    [EnumMember]
    [OptionSetMetadata("AIPluginInstance", 1033)]
    AIPluginInstance = 10191,

    [EnumMember]
    [OptionSetMetadata("AIPluginOperation", 1033)]
    AIPluginOperation = 10192,

    [EnumMember]
    [OptionSetMetadata("AIPluginOperationParameter", 1033)]
    AIPluginOperationParameter = 10193,

    [EnumMember]
    [OptionSetMetadata("AIPluginUserSetting", 1033)]
    AIPluginUserSetting = 10194,

    [EnumMember]
    [OptionSetMetadata("AI Configuration Search", 1033)]
    AIConfigurationSearch = 10196,

    [EnumMember]
    [OptionSetMetadata("Data Processing Event", 1033)]
    DataProcessingEvent = 10197,

    [EnumMember]
    [OptionSetMetadata("AI Document Template", 1033)]
    AIDocumentTemplate = 10198,

    [EnumMember]
    [OptionSetMetadata("AI Event", 1033)]
    AIEvent = 10199,

    [EnumMember]
    [OptionSetMetadata("AI Model Catalog", 1033)]
    AIModelCatalog = 10200,

    [EnumMember]
    [OptionSetMetadata("AI Builder Feedback Loop", 1033)]
    AIBuilderFeedbackLoop = 10202,

    [EnumMember]
    [OptionSetMetadata("AI Form Processing Document", 1033)]
    AIFormProcessingDocument = 10203,

    [EnumMember]
    [OptionSetMetadata("AI Object Detection Image", 1033)]
    AIObjectDetectionImage = 10204,

    [EnumMember]
    [OptionSetMetadata("AI Object Detection Label", 1033)]
    AIObjectDetectionLabel = 10205,

    [EnumMember]
    [OptionSetMetadata("AI Object Detection Bounding Box", 1033)]
    AIObjectDetectionBoundingBox = 10206,

    [EnumMember]
    [OptionSetMetadata("AI Object Detection Image Mapping", 1033)]
    AIObjectDetectionImageMapping = 10207,

    [EnumMember]
    [OptionSetMetadata("AI Builder Dataset", 1033)]
    AIBuilderDataset = 10209,

    [EnumMember]
    [OptionSetMetadata("AI Builder Dataset File", 1033)]
    AIBuilderDatasetFile = 10210,

    [EnumMember]
    [OptionSetMetadata("AI Builder Dataset Record", 1033)]
    AIBuilderDatasetRecord = 10211,

    [EnumMember]
    [OptionSetMetadata("AI Builder Datasets Container", 1033)]
    AIBuilderDatasetsContainer = 10212,

    [EnumMember]
    [OptionSetMetadata("AI Builder File", 1033)]
    AIBuilderFile = 10213,

    [EnumMember]
    [OptionSetMetadata("AI Builder File Attached Data", 1033)]
    AIBuilderFileAttachedData = 10214,

    [EnumMember]
    [OptionSetMetadata("AI Evaluation Configuration", 1033)]
    AIEvaluationConfiguration = 10215,

    [EnumMember]
    [OptionSetMetadata("AI Evaluation Metric", 1033)]
    AIEvaluationMetric = 10216,

    [EnumMember]
    [OptionSetMetadata("AI Evaluation Run", 1033)]
    AIEvaluationRun = 10217,

    [EnumMember]
    [OptionSetMetadata("AI Optimization", 1033)]
    AIOptimization = 10218,

    [EnumMember]
    [OptionSetMetadata("AI Optimization Private Data", 1033)]
    AIOptimizationPrivateData = 10219,

    [EnumMember]
    [OptionSetMetadata("AI Test Case", 1033)]
    AITestCase = 10220,

    [EnumMember]
    [OptionSetMetadata("AI Test Case Document", 1033)]
    AITestCaseDocument = 10221,

    [EnumMember]
    [OptionSetMetadata("AI Test Case Input", 1033)]
    AITestCaseInput = 10222,

    [EnumMember]
    [OptionSetMetadata("AI Test Run", 1033)]
    AITestRun = 10223,

    [EnumMember]
    [OptionSetMetadata("AI Test Run Batch", 1033)]
    AITestRunBatch = 10224,

    [EnumMember]
    [OptionSetMetadata("Help Page", 1033)]
    HelpPage = 10225,

    [EnumMember]
    [OptionSetMetadata("Tour", 1033)]
    Tour = 10226,

    [EnumMember]
    [OptionSetMetadata("BotContent", 1033)]
    BotContent = 10227,

    [EnumMember]
    [OptionSetMetadata("ConversationTranscript", 1033)]
    ConversationTranscript = 10228,

    [EnumMember]
    [OptionSetMetadata("Agent", 1033)]
    Agent = 10229,

    [EnumMember]
    [OptionSetMetadata("Agent component", 1033)]
    Agentcomponent = 10230,

    [EnumMember]
    [OptionSetMetadata("Agent component collection", 1033)]
    Agentcomponentcollection = 10231,

    [EnumMember]
    [OptionSetMetadata("Comment", 1033)]
    Comment_1 = 10242,

    [EnumMember]
    [OptionSetMetadata("Governance Configuration", 1033)]
    GovernanceConfiguration = 10243,

    [EnumMember]
    [OptionSetMetadata("Fabric AISkill", 1033)]
    FabricAISkill = 10244,

    [EnumMember]
    [OptionSetMetadata("App Insights Metadata", 1033)]
    AppInsightsMetadata = 10245,

    [EnumMember]
    [OptionSetMetadata("Dataflow Connection Reference", 1033)]
    DataflowConnectionReference = 10246,

    [EnumMember]
    [OptionSetMetadata("Schedule", 1033)]
    Schedule = 10247,

    [EnumMember]
    [OptionSetMetadata("Dataflow Template", 1033)]
    DataflowTemplate = 10248,

    [EnumMember]
    [OptionSetMetadata("Dataflow DatalakeFolder", 1033)]
    DataflowDatalakeFolder = 10249,

    [EnumMember]
    [OptionSetMetadata("Data Movement Service Request", 1033)]
    DataMovementServiceRequest = 10250,

    [EnumMember]
    [OptionSetMetadata("Data Movement Service Request Status", 1033)]
    DataMovementServiceRequestStatus = 10251,

    [EnumMember]
    [OptionSetMetadata("DMS Sync Request", 1033)]
    DMSSyncRequest = 10252,

    [EnumMember]
    [OptionSetMetadata("DMS Sync Status", 1033)]
    DMSSyncStatus = 10253,

    [EnumMember]
    [OptionSetMetadata("Knowledge Asset Configuration", 1033)]
    KnowledgeAssetConfiguration = 10254,

    [EnumMember]
    [OptionSetMetadata("Module Run Detail", 1033)]
    ModuleRunDetail = 10255,

    [EnumMember]
    [OptionSetMetadata("QnA", 1033)]
    QnA = 10256,

    [EnumMember]
    [OptionSetMetadata("Salesforce Structured Object", 1033)]
    SalesforceStructuredObject = 10257,

    [EnumMember]
    [OptionSetMetadata("Salesforce Structured QnA Config", 1033)]
    SalesforceStructuredQnAConfig = 10258,

    [EnumMember]
    [OptionSetMetadata("Workflow Action Status", 1033)]
    WorkflowActionStatus = 10259,

    [EnumMember]
    [OptionSetMetadata("Allowed MCP Client", 1033)]
    AllowedMCPClient = 10260,

    [EnumMember]
    [OptionSetMetadata("FederatedKnowledgeCitation", 1033)]
    FederatedKnowledgeCitation = 10261,

    [EnumMember]
    [OptionSetMetadata("FederatedKnowledgeConfiguration", 1033)]
    FederatedKnowledgeConfiguration = 10262,

    [EnumMember]
    [OptionSetMetadata("FederatedKnowledgeEntityConfiguration", 1033)]
    FederatedKnowledgeEntityConfiguration = 10263,

    [EnumMember]
    [OptionSetMetadata("FederatedKnowledgeMetadataRefresh", 1033)]
    FederatedKnowledgeMetadataRefresh = 10264,

    [EnumMember]
    [OptionSetMetadata("IntelligentMemory", 1033)]
    IntelligentMemory = 10265,

    [EnumMember]
    [OptionSetMetadata("Knowledge FAQ", 1033)]
    KnowledgeFAQ = 10266,

    [EnumMember]
    [OptionSetMetadata("Eval Result", 1033)]
    EvalResult = 10267,

    [EnumMember]
    [OptionSetMetadata("Form Mapping", 1033)]
    FormMapping = 10268,

    [EnumMember]
    [OptionSetMetadata("Copilot Interactions", 1033)]
    CopilotInteractions = 10269,

    [EnumMember]
    [OptionSetMetadata("PDF Setting", 1033)]
    PDFSetting = 10270,

    [EnumMember]
    [OptionSetMetadata("Activity File Attachment", 1033)]
    ActivityFileAttachment = 10271,

    [EnumMember]
    [OptionSetMetadata("Teams chat", 1033)]
    Teamschat = 10272,

    [EnumMember]
    [OptionSetMetadata("Service Configuration", 1033)]
    ServiceConfiguration = 10273,

    [EnumMember]
    [OptionSetMetadata("SLA KPI", 1033)]
    SLAKPI = 10274,

    [EnumMember]
    [OptionSetMetadata("Integrated search provider", 1033)]
    Integratedsearchprovider = 10275,

    [EnumMember]
    [OptionSetMetadata("Knowledge Management Setting", 1033)]
    KnowledgeManagementSetting = 10276,

    [EnumMember]
    [OptionSetMetadata("Knowledge Federated Article", 1033)]
    KnowledgeFederatedArticle = 10277,

    [EnumMember]
    [OptionSetMetadata("Knowledge Federated Article Incident", 1033)]
    KnowledgeFederatedArticleIncident = 10278,

    [EnumMember]
    [OptionSetMetadata("Search provider", 1033)]
    Searchprovider = 10279,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article Image", 1033)]
    KnowledgeArticleImage = 10280,

    [EnumMember]
    [OptionSetMetadata("Knowledge Configuration", 1033)]
    KnowledgeConfiguration = 10281,

    [EnumMember]
    [OptionSetMetadata("Knowledge Interaction Insight", 1033)]
    KnowledgeInteractionInsight = 10282,

    [EnumMember]
    [OptionSetMetadata("Knowledge Search Insight", 1033)]
    KnowledgeSearchInsight = 10283,

    [EnumMember]
    [OptionSetMetadata("Favorite knowledge article", 1033)]
    Favoriteknowledgearticle = 10284,

    [EnumMember]
    [OptionSetMetadata("Knowledge article language setting", 1033)]
    Knowledgearticlelanguagesetting = 10285,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article Attachment", 1033)]
    KnowledgeArticleAttachment = 10286,

    [EnumMember]
    [OptionSetMetadata("Knowledge personalization", 1033)]
    Knowledgepersonalization = 10287,

    [EnumMember]
    [OptionSetMetadata("Knowledge Article Template", 1033)]
    KnowledgeArticleTemplate = 10288,

    [EnumMember]
    [OptionSetMetadata("Knowledge search personal filter config", 1033)]
    Knowledgesearchpersonalfilterconfig = 10289,

    [EnumMember]
    [OptionSetMetadata("Knowledge search filter", 1033)]
    Knowledgesearchfilter = 10290,

    [EnumMember]
    [OptionSetMetadata("SupportUserTable", 1033)]
    SupportUserTable = 10292,

    [EnumMember]
    [OptionSetMetadata("FxExpression", 1033)]
    FxExpression = 10293,

    [EnumMember]
    [OptionSetMetadata("Function", 1033)]
    Function = 10294,

    [EnumMember]
    [OptionSetMetadata("Plug-in", 1033)]
    Plugin = 10295,

    [EnumMember]
    [OptionSetMetadata("PowerfxRule", 1033)]
    PowerfxRule = 10296,

    [EnumMember]
    [OptionSetMetadata("Planner Business Scenario", 1033)]
    PlannerBusinessScenario = 10297,

    [EnumMember]
    [OptionSetMetadata("Planner Sync Action", 1033)]
    PlannerSyncAction = 10298,

    [EnumMember]
    [OptionSetMetadata("Agent Rule", 1033)]
    AgentRule = 10299,

    [EnumMember]
    [OptionSetMetadata("MCPPrompt", 1033)]
    MCPPrompt = 10300,

    [EnumMember]
    [OptionSetMetadata("MCPResource", 1033)]
    MCPResource = 10301,

    [EnumMember]
    [OptionSetMetadata("MCPResourceContent", 1033)]
    MCPResourceContent = 10302,

    [EnumMember]
    [OptionSetMetadata("MCPServer", 1033)]
    MCPServer = 10303,

    [EnumMember]
    [OptionSetMetadata("MCPTool", 1033)]
    MCPTool = 10304,

    [EnumMember]
    [OptionSetMetadata("ToolingGateway", 1033)]
    ToolingGateway = 10305,

    [EnumMember]
    [OptionSetMetadata("ToolingGatewayMCPServer", 1033)]
    ToolingGatewayMCPServer = 10306,

    [EnumMember]
    [OptionSetMetadata("Email Address Configuration", 1033)]
    EmailAddressConfiguration = 10307,

    [EnumMember]
    [OptionSetMetadata("Ms Graph Resource To Subscription", 1033)]
    MsGraphResourceToSubscription = 10308,

    [EnumMember]
    [OptionSetMetadata("Virtual Entity  Metadata", 1033)]
    VirtualEntityMetadata = 10309,

    [EnumMember]
    [OptionSetMetadata("Background Operation", 1033)]
    BackgroundOperation = 10310,

    [EnumMember]
    [OptionSetMetadata("Report Parameter", 1033)]
    ReportParameter = 10311,

    [EnumMember]
    [OptionSetMetadata("MobileOfflineProfileExtension", 1033)]
    MobileOfflineProfileExtension = 10312,

    [EnumMember]
    [OptionSetMetadata("Mobile Offline Profile Suggestion", 1033)]
    MobileOfflineProfileSuggestion = 10313,

    [EnumMember]
    [OptionSetMetadata("Mobile Offline Profile Suggestion Impacted Table", 1033)]
    MobileOfflineProfileSuggestionImpactedTable = 10314,

    [EnumMember]
    [OptionSetMetadata("MobileOfflineProfileItemFilter", 1033)]
    MobileOfflineProfileItemFilter = 10315,

    [EnumMember]
    [OptionSetMetadata("TeamMobileOfflineProfileMembership", 1033)]
    TeamMobileOfflineProfileMembership = 10316,

    [EnumMember]
    [OptionSetMetadata("UserMobileOfflineProfileMembership", 1033)]
    UserMobileOfflineProfileMembership = 10317,

    [EnumMember]
    [OptionSetMetadata("OrganizationDataSyncSubscription", 1033)]
    OrganizationDataSyncSubscription = 10318,

    [EnumMember]
    [OptionSetMetadata("OrganizationDataSyncSubscriptionEntity", 1033)]
    OrganizationDataSyncSubscriptionEntity = 10319,

    [EnumMember]
    [OptionSetMetadata("OrganizationDataSyncSubscriptionFnoTable", 1033)]
    OrganizationDataSyncSubscriptionFnoTable = 10320,

    [EnumMember]
    [OptionSetMetadata("OrganizationDataSyncFnoState", 1033)]
    OrganizationDataSyncFnoState = 10321,

    [EnumMember]
    [OptionSetMetadata("OrganizationDataSyncState", 1033)]
    OrganizationDataSyncState = 10322,

    [EnumMember]
    [OptionSetMetadata("ArchiveCleanupInfo", 1033)]
    ArchiveCleanupInfo = 10323,

    [EnumMember]
    [OptionSetMetadata("ArchiveCleanupOperation", 1033)]
    ArchiveCleanupOperation = 10324,

    [EnumMember]
    [OptionSetMetadata("BulkArchiveConfig", 1033)]
    BulkArchiveConfig = 10325,

    [EnumMember]
    [OptionSetMetadata("BulkArchiveFailureDetail", 1033)]
    BulkArchiveFailureDetail = 10326,

    [EnumMember]
    [OptionSetMetadata("BulkArchiveOperation", 1033)]
    BulkArchiveOperation = 10327,

    [EnumMember]
    [OptionSetMetadata("BulkArchiveOperationDetail", 1033)]
    BulkArchiveOperationDetail = 10328,

    [EnumMember]
    [OptionSetMetadata("EnableArchivalRequest", 1033)]
    EnableArchivalRequest = 10329,

    [EnumMember]
    [OptionSetMetadata("MetadataForArchival", 1033)]
    MetadataForArchival = 10330,

    [EnumMember]
    [OptionSetMetadata("ReconciliationEntityInfo", 1033)]
    ReconciliationEntityInfo = 10331,

    [EnumMember]
    [OptionSetMetadata("ReconciliationEntityStepInfo", 1033)]
    ReconciliationEntityStepInfo = 10332,

    [EnumMember]
    [OptionSetMetadata("ReconciliationInfo", 1033)]
    ReconciliationInfo = 10333,

    [EnumMember]
    [OptionSetMetadata("RetentionCleanupInfo", 1033)]
    RetentionCleanupInfo = 10334,

    [EnumMember]
    [OptionSetMetadata("RetentionCleanupOperation", 1033)]
    RetentionCleanupOperation = 10335,

    [EnumMember]
    [OptionSetMetadata("Data Life Cycle Config", 1033)]
    DataLifeCycleConfig = 10336,

    [EnumMember]
    [OptionSetMetadata("Retention Config Table Mapping", 1033)]
    RetentionConfigTableMapping = 10337,

    [EnumMember]
    [OptionSetMetadata("RetentionFailureDetail", 1033)]
    RetentionFailureDetail = 10338,

    [EnumMember]
    [OptionSetMetadata("RetentionOperation", 1033)]
    RetentionOperation = 10339,

    [EnumMember]
    [OptionSetMetadata("RetentionOperationDetail", 1033)]
    RetentionOperationDetail = 10340,

    [EnumMember]
    [OptionSetMetadata("RetentionSuccessDetail", 1033)]
    RetentionSuccessDetail = 10341,

    [EnumMember]
    [OptionSetMetadata("CertificateCredential", 1033)]
    CertificateCredential = 10342,

    [EnumMember]
    [OptionSetMetadata("Notification", 1033)]
    Notification_1 = 10343,

    [EnumMember]
    [OptionSetMetadata("User Rating", 1033)]
    UserRating = 10344,

    [EnumMember]
    [OptionSetMetadata("Geolocation Record", 1033)]
    GeolocationRecord = 10345,

    [EnumMember]
    [OptionSetMetadata("Mobile App", 1033)]
    MobileApp = 10346,

    [EnumMember]
    [OptionSetMetadata("Native Extension", 1033)]
    NativeExtension = 10347,

    [EnumMember]
    [OptionSetMetadata("Power Apps Wrap Build", 1033)]
    PowerAppsWrapBuild = 10348,

    [EnumMember]
    [OptionSetMetadata("Insights Store Data Source", 1033)]
    InsightsStoreDataSource = 10349,

    [EnumMember]
    [OptionSetMetadata("Insights Store Virtual Entity", 1033)]
    InsightsStoreVirtualEntity = 10350,

    [EnumMember]
    [OptionSetMetadata("RoleEditorLayout", 1033)]
    RoleEditorLayout = 10351,

    [EnumMember]
    [OptionSetMetadata("Deleted Record Reference", 1033)]
    DeletedRecordReference = 10352,

    [EnumMember]
    [OptionSetMetadata("Restore Deleted Records Configuration", 1033)]
    RestoreDeletedRecordsConfiguration = 10353,

    [EnumMember]
    [OptionSetMetadata("App Action", 1033)]
    AppAction = 10354,

    [EnumMember]
    [OptionSetMetadata("App Action Migration", 1033)]
    AppActionMigration = 10355,

    [EnumMember]
    [OptionSetMetadata("App Action Rule", 1033)]
    AppActionRule = 10356,

    [EnumMember]
    [OptionSetMetadata("Card", 1033)]
    Card = 10359,

    [EnumMember]
    [OptionSetMetadata("Card State Item", 1033)]
    CardStateItem = 10360,

    [EnumMember]
    [OptionSetMetadata("Entity link chat configuration", 1033)]
    Entitylinkchatconfiguration = 10363,

    [EnumMember]
    [OptionSetMetadata("Agent Feed Item", 1033)]
    AgentFeedItem = 10364,

    [EnumMember]
    [OptionSetMetadata("Agent Hub Goal", 1033)]
    AgentHubGoal = 10365,

    [EnumMember]
    [OptionSetMetadata("Agent Hub Insight", 1033)]
    AgentHubInsight = 10366,

    [EnumMember]
    [OptionSetMetadata("Agent Hub Metric", 1033)]
    AgentHubMetric = 10367,

    [EnumMember]
    [OptionSetMetadata("Agentic Scenario", 1033)]
    AgenticScenario = 10368,

    [EnumMember]
    [OptionSetMetadata("Agent Memory", 1033)]
    AgentMemory = 10369,

    [EnumMember]
    [OptionSetMetadata("Agent Prompt", 1033)]
    AgentPrompt = 10370,

    [EnumMember]
    [OptionSetMetadata("Agent Task", 1033)]
    AgentTask = 10371,

    [EnumMember]
    [OptionSetMetadata("SharePoint Managed Identity", 1033)]
    SharePointManagedIdentity = 10372,

    [EnumMember]
    [OptionSetMetadata("ControlConfiguration", 1033)]
    ControlConfiguration = 10373,

    [EnumMember]
    [OptionSetMetadata("AI Insight Card", 1033)]
    AIInsightCard = 10374,

    [EnumMember]
    [OptionSetMetadata("AI Skill Config", 1033)]
    AISkillConfig = 10375,

    [EnumMember]
    [OptionSetMetadata("Suggested Action", 1033)]
    SuggestedAction = 10376,

    [EnumMember]
    [OptionSetMetadata("Suggested Action Criteria", 1033)]
    SuggestedActionCriteria = 10377,

    [EnumMember]
    [OptionSetMetadata("Data Workspace", 1033)]
    DataWorkspace = 10378,

    [EnumMember]
    [OptionSetMetadata("Plan", 1033)]
    Plan = 10379,

    [EnumMember]
    [OptionSetMetadata("Plan Artifact", 1033)]
    PlanArtifact = 10380,

    [EnumMember]
    [OptionSetMetadata("Plan Attachment", 1033)]
    PlanAttachment = 10381,

    [EnumMember]
    [OptionSetMetadata("UX Agent Component", 1033)]
    UXAgentComponent = 10382,

    [EnumMember]
    [OptionSetMetadata("UX Agent Component Revision", 1033)]
    UXAgentComponentRevision = 10383,

    [EnumMember]
    [OptionSetMetadata("UX Agent Project", 1033)]
    UXAgentProject = 10384,

    [EnumMember]
    [OptionSetMetadata("UX Agent Project File", 1033)]
    UXAgentProjectFile = 10385,

    [EnumMember]
    [OptionSetMetadata("Agent Conversation Message", 1033)]
    AgentConversationMessage = 10386,

    [EnumMember]
    [OptionSetMetadata("Agent Conversation Message File", 1033)]
    AgentConversationMessageFile = 10387,

    [EnumMember]
    [OptionSetMetadata("Rich Text Attachment", 1033)]
    RichTextAttachment = 10388,

    [EnumMember]
    [OptionSetMetadata("Structured Template", 1033)]
    StructuredTemplate = 10389,

    [EnumMember]
    [OptionSetMetadata("RTE Structured Template Config", 1033)]
    RTEStructuredTemplateConfig = 10390,

    [EnumMember]
    [OptionSetMetadata("RTE Template Mapping", 1033)]
    RTETemplateMapping = 10391,

    [EnumMember]
    [OptionSetMetadata("Custom Control Extended Setting", 1033)]
    CustomControlExtendedSetting = 10392,

    [EnumMember]
    [OptionSetMetadata("Timeline Pin", 1033)]
    TimelinePin = 10393,

    [EnumMember]
    [OptionSetMetadata("Virtual Connector Data Source", 1033)]
    VirtualConnectorDataSource = 10394,

    [EnumMember]
    [OptionSetMetadata("Virtual Table Column Candidate", 1033)]
    VirtualTableColumnCandidate = 10395,

    [EnumMember]
    [OptionSetMetadata("PM Analysis History", 1033)]
    PMAnalysisHistory = 10397,

    [EnumMember]
    [OptionSetMetadata("PM Business Rule Automation Config", 1033)]
    PMBusinessRuleAutomationConfig = 10398,

    [EnumMember]
    [OptionSetMetadata("PM Calendar", 1033)]
    PMCalendar = 10399,

    [EnumMember]
    [OptionSetMetadata("PM Calendar Version", 1033)]
    PMCalendarVersion = 10400,

    [EnumMember]
    [OptionSetMetadata("PM Inferred Task", 1033)]
    PMInferredTask = 10401,

    [EnumMember]
    [OptionSetMetadata("PM Process Extended Metadata Version", 1033)]
    PMProcessExtendedMetadataVersion = 10402,

    [EnumMember]
    [OptionSetMetadata("PM Process Template", 1033)]
    PMProcessTemplate = 10403,

    [EnumMember]
    [OptionSetMetadata("PM Process User Settings", 1033)]
    PMProcessUserSettings = 10404,

    [EnumMember]
    [OptionSetMetadata("PM Process Version", 1033)]
    PMProcessVersion = 10405,

    [EnumMember]
    [OptionSetMetadata("PM Recording", 1033)]
    PMRecording = 10406,

    [EnumMember]
    [OptionSetMetadata("PM Simulation", 1033)]
    PMSimulation = 10407,

    [EnumMember]
    [OptionSetMetadata("PM Tab", 1033)]
    PMTab = 10408,

    [EnumMember]
    [OptionSetMetadata("PM Template", 1033)]
    PMTemplate = 10409,

    [EnumMember]
    [OptionSetMetadata("PM View", 1033)]
    PMView = 10410,

    [EnumMember]
    [OptionSetMetadata("Analysis Component", 1033)]
    AnalysisComponent = 10411,

    [EnumMember]
    [OptionSetMetadata("Analysis Job", 1033)]
    AnalysisJob = 10412,

    [EnumMember]
    [OptionSetMetadata("Analysis Override", 1033)]
    AnalysisOverride = 10413,

    [EnumMember]
    [OptionSetMetadata("Analysis Result", 1033)]
    AnalysisResult = 10414,

    [EnumMember]
    [OptionSetMetadata("Analysis Result Detail", 1033)]
    AnalysisResultDetail = 10415,

    [EnumMember]
    [OptionSetMetadata("Solution Health Rule", 1033)]
    SolutionHealthRule = 10416,

    [EnumMember]
    [OptionSetMetadata("Solution Health Rule Argument", 1033)]
    SolutionHealthRuleArgument = 10417,

    [EnumMember]
    [OptionSetMetadata("Solution Health Rule Set", 1033)]
    SolutionHealthRuleSet = 10418,

    [EnumMember]
    [OptionSetMetadata("File Upload", 1033)]
    FileUpload = 10419,

    [EnumMember]
    [OptionSetMetadata("AppEntitySearchView", 1033)]
    AppEntitySearchView = 10420,

    [EnumMember]
    [OptionSetMetadata("MainFewShot", 1033)]
    MainFewShot = 10421,

    [EnumMember]
    [OptionSetMetadata("MakerFewShot", 1033)]
    MakerFewShot = 10422,

    [EnumMember]
    [OptionSetMetadata("SearchAttributeSettings", 1033)]
    SearchAttributeSettings = 10423,

    [EnumMember]
    [OptionSetMetadata("SearchCustomAnalyzer", 1033)]
    SearchCustomAnalyzer = 10424,

    [EnumMember]
    [OptionSetMetadata("SearchRelationshipSettings", 1033)]
    SearchRelationshipSettings = 10425,

    [EnumMember]
    [OptionSetMetadata("SearchResultsCache", 1033)]
    SearchResultsCache = 10426,

    [EnumMember]
    [OptionSetMetadata("Search Telemetry", 1033)]
    SearchTelemetry = 10427,

    [EnumMember]
    [OptionSetMetadata("Skill Changeset History", 1033)]
    SkillChangesetHistory = 10428,

    [EnumMember]
    [OptionSetMetadata("Skill Changeset Reviewer", 1033)]
    SkillChangesetReviewer = 10429,

    [EnumMember]
    [OptionSetMetadata("Skill Eval Result", 1033)]
    SkillEvalResult = 10430,

    [EnumMember]
    [OptionSetMetadata("Business Skill Metadata", 1033)]
    BusinessSkillMetadata = 10431,

    [EnumMember]
    [OptionSetMetadata("Business Skill Role Mapping", 1033)]
    BusinessSkillRoleMapping = 10432,

    [EnumMember]
    [OptionSetMetadata("TextDataRecordsIndexingStatus", 1033)]
    TextDataRecordsIndexingStatus = 10433,

    [EnumMember]
    [OptionSetMetadata("ViewAsExampleQuestion", 1033)]
    ViewAsExampleQuestion = 10434,

    [EnumMember]
    [OptionSetMetadata("CopilotExampleQuestion", 1033)]
    CopilotExampleQuestion = 10435,

    [EnumMember]
    [OptionSetMetadata("CopilotGlossaryTerm", 1033)]
    CopilotGlossaryTerm = 10436,

    [EnumMember]
    [OptionSetMetadata("CopilotSynonyms", 1033)]
    CopilotSynonyms = 10437,

    [EnumMember]
    [OptionSetMetadata("Business Skill", 1033)]
    BusinessSkill = 10438,

    [EnumMember]
    [OptionSetMetadata("Business Skill Resource", 1033)]
    BusinessSkillResource = 10439,

    [EnumMember]
    [OptionSetMetadata("Site Component", 1033)]
    SiteComponent = 10441,

    [EnumMember]
    [OptionSetMetadata("Site", 1033)]
    Site = 10442,

    [EnumMember]
    [OptionSetMetadata("Site Language", 1033)]
    SiteLanguage = 10443,

    [EnumMember]
    [OptionSetMetadata("Power Pages Site Published", 1033)]
    PowerPagesSitePublished = 10444,

    [EnumMember]
    [OptionSetMetadata("Site Source File", 1033)]
    SiteSourceFile = 10445,

    [EnumMember]
    [OptionSetMetadata("External Identity", 1033)]
    ExternalIdentity = 10448,

    [EnumMember]
    [OptionSetMetadata("Invitation", 1033)]
    Invitation = 10449,

    [EnumMember]
    [OptionSetMetadata("Invite Redemption", 1033)]
    InviteRedemption = 10450,

    [EnumMember]
    [OptionSetMetadata("Portal Comment", 1033)]
    PortalComment = 10451,

    [EnumMember]
    [OptionSetMetadata("Setting", 1033)]
    Setting = 10452,

    [EnumMember]
    [OptionSetMetadata("Multistep Form Session", 1033)]
    MultistepFormSession = 10453,

    [EnumMember]
    [OptionSetMetadata("Ad Placement", 1033)]
    AdPlacement = 10457,

    [EnumMember]
    [OptionSetMetadata("Column Permission", 1033)]
    ColumnPermission = 10458,

    [EnumMember]
    [OptionSetMetadata("Column Permission Profile", 1033)]
    ColumnPermissionProfile = 10459,

    [EnumMember]
    [OptionSetMetadata("Content Snippet", 1033)]
    ContentSnippet = 10460,

    [EnumMember]
    [OptionSetMetadata("Basic Form", 1033)]
    BasicForm = 10461,

    [EnumMember]
    [OptionSetMetadata("Basic Form Metadata", 1033)]
    BasicFormMetadata = 10462,

    [EnumMember]
    [OptionSetMetadata("List", 1033)]
    List = 10463,

    [EnumMember]
    [OptionSetMetadata("Table Permission", 1033)]
    TablePermission = 10464,

    [EnumMember]
    [OptionSetMetadata("Page Template", 1033)]
    PageTemplate = 10465,

    [EnumMember]
    [OptionSetMetadata("Poll Placement", 1033)]
    PollPlacement = 10466,

    [EnumMember]
    [OptionSetMetadata("Power Pages Core Entity DS", 1033)]
    PowerPagesCoreEntityDS = 10467,

    [EnumMember]
    [OptionSetMetadata("Publishing State", 1033)]
    PublishingState = 10468,

    [EnumMember]
    [OptionSetMetadata("Publishing State Transition Rule", 1033)]
    PublishingStateTransitionRule = 10469,

    [EnumMember]
    [OptionSetMetadata("Redirect", 1033)]
    Redirect = 10470,

    [EnumMember]
    [OptionSetMetadata("Shortcut", 1033)]
    Shortcut = 10471,

    [EnumMember]
    [OptionSetMetadata("Site Marker", 1033)]
    SiteMarker = 10472,

    [EnumMember]
    [OptionSetMetadata("Site Setting", 1033)]
    SiteSetting = 10473,

    [EnumMember]
    [OptionSetMetadata("Web File", 1033)]
    WebFile = 10474,

    [EnumMember]
    [OptionSetMetadata("Multistep Form", 1033)]
    MultistepForm = 10475,

    [EnumMember]
    [OptionSetMetadata("Multistep Form Metadata", 1033)]
    MultistepFormMetadata = 10476,

    [EnumMember]
    [OptionSetMetadata("Form Step", 1033)]
    FormStep = 10477,

    [EnumMember]
    [OptionSetMetadata("Web Link", 1033)]
    WebLink = 10478,

    [EnumMember]
    [OptionSetMetadata("Web Link Set", 1033)]
    WebLinkSet = 10479,

    [EnumMember]
    [OptionSetMetadata("Web Page", 1033)]
    WebPage = 10480,

    [EnumMember]
    [OptionSetMetadata("Web Page Access Control Rule", 1033)]
    WebPageAccessControlRule = 10481,

    [EnumMember]
    [OptionSetMetadata("Web Role", 1033)]
    WebRole = 10482,

    [EnumMember]
    [OptionSetMetadata("Website", 1033)]
    Website = 10483,

    [EnumMember]
    [OptionSetMetadata("Website Access", 1033)]
    WebsiteAccess = 10484,

    [EnumMember]
    [OptionSetMetadata("Website Language", 1033)]
    WebsiteLanguage = 10485,

    [EnumMember]
    [OptionSetMetadata("Web Template", 1033)]
    WebTemplate = 10486,

    [EnumMember]
    [OptionSetMetadata("Power Pages Scan Report", 1033)]
    PowerPagesScanReport = 10493,

    [EnumMember]
    [OptionSetMetadata("PowerPagesDDOSAlert", 1033)]
    PowerPagesDDOSAlert = 10494,

    [EnumMember]
    [OptionSetMetadata("Power Pages Log", 1033)]
    PowerPagesLog = 10495,

    [EnumMember]
    [OptionSetMetadata("PowerPagesManagedIdentity", 1033)]
    PowerPagesManagedIdentity = 10496,

    [EnumMember]
    [OptionSetMetadata("Power Pages Site AI Feedback", 1033)]
    PowerPagesSiteAIFeedback = 10497,

    [EnumMember]
    [OptionSetMetadata("PowerPagesUserMapping", 1033)]
    PowerPagesUserMapping = 10498,

    [EnumMember]
    [OptionSetMetadata("AgentActivity", 1033)]
    AgentActivity = 10504,

    [EnumMember]
    [OptionSetMetadata("SecurityIncident", 1033)]
    SecurityIncident = 10505,

    [EnumMember]
    [OptionSetMetadata("Catalog Submission Files", 1033)]
    CatalogSubmissionFiles = 10506,

    [EnumMember]
    [OptionSetMetadata("Package Submission Store", 1033)]
    PackageSubmissionStore = 10507,

    [EnumMember]
    [OptionSetMetadata("indexedtrait", 1033)]
    indexedtrait = 10508,

    [EnumMember]
    [OptionSetMetadata("processor registration", 1033)]
    processorregistration = 10509,

    [EnumMember]
    [OptionSetMetadata("signal", 1033)]
    signal = 10510,

    [EnumMember]
    [OptionSetMetadata("signal registration", 1033)]
    signalregistration = 10511,

    [EnumMember]
    [OptionSetMetadata("trait", 1033)]
    trait = 10512,

    [EnumMember]
    [OptionSetMetadata("trait registration", 1033)]
    traitregistration = 10513,

    [EnumMember]
    [OptionSetMetadata("MOS3 Management", 1033)]
    MOS3Management = 10514,

    [EnumMember]
    [OptionSetMetadata("Rule", 1033)]
    Rule = 10605,

    [EnumMember]
    [OptionSetMetadata("Rule Version", 1033)]
    RuleVersion = 10606,

    [EnumMember]
    [OptionSetMetadata("Condition Group", 1033)]
    ConditionGroup = 10607,

    [EnumMember]
    [OptionSetMetadata("Condition", 1033)]
    Condition = 10608,

    [EnumMember]
    [OptionSetMetadata("Action", 1033)]
    Action = 10609,

    [EnumMember]
    [OptionSetMetadata("Action Input Mapping", 1033)]
    ActionInputMapping = 10610,

    [EnumMember]
    [OptionSetMetadata("Custom Function", 1033)]
    CustomFunction = 10611,

    [EnumMember]
    [OptionSetMetadata("Lookup Definition", 1033)]
    LookupDefinition = 10612,

    [EnumMember]
    [OptionSetMetadata("Release", 1033)]
    Release = 10613,

    [EnumMember]
    [OptionSetMetadata("Execution Log", 1033)]
    ExecutionLog = 10614,

    [EnumMember]
    [OptionSetMetadata("Account Stakeholder", 1033)]
    AccountStakeholder = 10712,

    [EnumMember]
    [OptionSetMetadata("Lead", 1033)]
    Lead = 10713,

    [EnumMember]
    [OptionSetMetadata("Opportunity", 1033)]
    Opportunity = 10714,

    [EnumMember]
    [OptionSetMetadata("Opportunity Stakeholder", 1033)]
    OpportunityStakeholder = 10715,

    [EnumMember]
    [OptionSetMetadata("Agreement", 1033)]
    Agreement = 10716,

    [EnumMember]
    [OptionSetMetadata("Quote", 1033)]
    Quote = 10717,

    [EnumMember]
    [OptionSetMetadata("Quote Line", 1033)]
    QuoteLine = 10718,

    [EnumMember]
    [OptionSetMetadata("Product", 1033)]
    Product = 10719,

    [EnumMember]
    [OptionSetMetadata("Price List", 1033)]
    PriceList = 10720,

    [EnumMember]
    [OptionSetMetadata("Price List Item", 1033)]
    PriceListItem = 10721,

    [EnumMember]
    [OptionSetMetadata("Skill Event", 1033)]
    SkillEvent = 10722,
}
