/****** Object:  StoredProcedure [dbo].[AddFolder]    Script Date: 03/14/2012 11:38:59 ******/
USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'KeyLessGuidTableType')
--DROP TYPE dbo.KeyLessGuidTableType

CREATE TYPE [dbo].[KeyLessGuidTableType] AS TABLE
(Value uniqueidentifier NOT NULL, SequenceNumber int IDENTITY(1,1));
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidTableType')
--DROP TYPE dbo.GuidTableType

CREATE TYPE [dbo].[GuidTableType] AS TABLE
(Value uniqueidentifier NOT NULL PRIMARY KEY CLUSTERED);
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidPairTableType')
--DROP TYPE dbo.GuidPairTableType

CREATE TYPE [dbo].[GuidPairTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue uniqueidentifier NOT NULL,
PRIMARY KEY CLUSTERED(FirstValue ASC, SecondValue ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidPairBitTableType')
--DROP TYPE dbo.GuidPairBitTableType

CREATE TYPE [dbo].[GuidPairBitTableType] AS TABLE(
	FirstValue uniqueidentifier NULL,
	SecondValue uniqueidentifier NULL,
	BitValue bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidTripleTableType')
--DROP TYPE dbo.GuidTripleTableType

CREATE TYPE [dbo].[GuidTripleTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue uniqueidentifier NOT NULL,
	ThirdValue uniqueidentifier NOT NULL
PRIMARY KEY CLUSTERED(FirstValue ASC, SecondValue ASC, ThirdValue ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidStringTableType')
--DROP TYPE dbo.GuidStringTableType

CREATE TYPE [dbo].[GuidStringTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue nvarchar(max) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidStringPairTableType')
--DROP TYPE dbo.GuidStringPairTableType

CREATE TYPE [dbo].[GuidStringPairTableType] AS TABLE(
	GuidValue uniqueidentifier NULL,
	FirstValue nvarchar(max) NULL,
	SecondValue nvarchar(max) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'BigIntTableType')
--DROP TYPE dbo.BigIntTableType

CREATE TYPE [dbo].[BigIntTableType]
AS 
TABLE(
	Value bigint NOT NULL
PRIMARY KEY CLUSTERED(Value ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'StringTableType')
--DROP TYPE dbo.StringTableType

CREATE TYPE [dbo].[StringTableType] AS TABLE(
	Value nvarchar(max) NOT NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'StringPairTableType')
--DROP TYPE dbo.StringPairTableType

CREATE TYPE [dbo].[StringPairTableType] AS TABLE(
	FirstValue nvarchar(max) NOT NULL,
	SecondValue nvarchar(max) NOT NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FloatStringTableType')
--DROP TYPE dbo.FloatStringTableType

CREATE TYPE [dbo].[FloatStringTableType] AS TABLE(
	FirstValue float NOT NULL,
	SecondValue nvarchar(max) NOT NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidIntTableType')
--DROP TYPE dbo.GuidIntTableType

CREATE TYPE [dbo].[GuidIntTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue int NOT NULL,
PRIMARY KEY CLUSTERED(FirstValue ASC, SecondValue ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidFloatTableType')
--DROP TYPE dbo.GuidFloatTableType

CREATE TYPE [dbo].[GuidFloatTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue float NOT NULL,
PRIMARY KEY CLUSTERED(FirstValue ASC, SecondValue ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'GuidBitTableType')
--DROP TYPE dbo.GuidBitTableType

CREATE TYPE [dbo].[GuidBitTableType] AS TABLE(
	FirstValue uniqueidentifier NOT NULL,
	SecondValue bit NOT NULL,
PRIMARY KEY CLUSTERED(FirstValue ASC, SecondValue ASC)
)
GO


--IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'NodesHierarchyTableType')
--DROP TYPE dbo.NodesHierarchyTableType

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'NodesHierarchyTableType')
DROP TYPE dbo.NodesHierarchyTableType

CREATE TYPE [dbo].[NodesHierarchyTableType] AS TABLE(
	NodeID uniqueidentifier NOT NULL,
	ParentNodeID uniqueidentifier NULL,
	[Level] int NULL,
	Name nvarchar(2000)
PRIMARY KEY CLUSTERED(NodeID ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'DocFileInfoTableType')
--DROP TYPE dbo.DocFileInfoTableType

CREATE TYPE [dbo].[DocFileInfoTableType] AS TABLE(
	FileID uniqueidentifier NOT NULL,
	[FileName] nvarchar(255) NOT NULL,
	Extension varchar(20) NULL,
	MIME varchar(100) NULL,
	Size bigint NULL,
	OwnerID uniqueidentifier NULL,
	OwnerType varchar(50) NULL
PRIMARY KEY CLUSTERED(FileID ASC)
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeNodeTableType')
--DROP TYPE dbo.ExchangeNodeTableType

CREATE TYPE [dbo].[ExchangeNodeTableType] AS TABLE(
	NodeID uniqueidentifier NULL,
	NodeAdditionalID varchar(50) NULL,
	Name nvarchar(500) NULL,
	ParentAdditionalID varchar(50) NULL,
	Abstract nvarchar(max) NULL,
	Tags nvarchar(2000) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeUserTableType')
--DROP TYPE dbo.ExchangeUserTableType

CREATE TYPE [dbo].[ExchangeUserTableType] AS TABLE(
	UserID	uniqueidentifier NULL,
	UserName nvarchar(50) NULL,
	NewUserName nvarchar(50) NULL,
	FirstName nvarchar(500) NULL,
	LastName nvarchar(500) NULL,
	EmploymentType varchar(50) NULL,
	DepartmentID varchar(50) NULL,
	IsManager bit NULL,
	Email varchar(100) NULL,
	PhoneNumber varchar(50) NULL,
	ResetPassword bit NULL,
	[Password] nvarchar(255) NULL,
	PasswordSalt nvarchar(255) NULL,
	EncryptedPassword nvarchar(255) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeMemberTableType')
--DROP TYPE dbo.ExchangeMemberTableType

CREATE TYPE [dbo].[ExchangeMemberTableType] AS TABLE(
	NodeTypeAdditionalID nvarchar(50),
	NodeAdditionalID nvarchar(50),
	NodeID uniqueidentifier NULL,
	UserName nvarchar(50) NULL,
	IsAdmin bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeRelationTableType')
--DROP TYPE dbo.ExchangeRelationTableType

CREATE TYPE [dbo].[ExchangeRelationTableType] AS TABLE(
	SourceTypeAdditionalID nvarchar(50),
	SourceAdditionalID nvarchar(50),
	SourceID uniqueidentifier NULL,
	DestinationTypeAdditionalID nvarchar(50),
	DestinationAdditionalID nvarchar(50),
	DestinationID uniqueidentifier NULL,
	Bidirectional bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeAuthorTableType')
--DROP TYPE dbo.ExchangeAuthorTableType

CREATE TYPE [dbo].[ExchangeAuthorTableType] AS TABLE(
	NodeTypeAdditionalID nvarchar(50) NULL,
	NodeAdditionalID nvarchar(50) NULL,
	UserName nvarchar(50) NULL,
	Percentage int NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangePermissionTableType')
--DROP TYPE dbo.ExchangePermissionTableType

CREATE TYPE [dbo].[ExchangePermissionTableType] AS TABLE(
	NodeTypeAdditionalID nvarchar(50) NULL,
	NodeAdditionalID nvarchar(50) NULL,
	GroupTypeAdditionalID nvarchar(50) NULL,
	GroupAdditionalID nvarchar(50) NULL,
	UserName nvarchar(50) NULL,
	PermissionType nvarchar(50) NULL,
	Allow bit NULL,
	DropAll bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FormInstanceTableType')
-- DROP TYPE dbo.FormInstanceTableType

CREATE TYPE [dbo].[FormInstanceTableType] AS TABLE(
	InstanceID	uniqueidentifier NOT NULL PRIMARY KEY CLUSTERED,
	FormID uniqueidentifier NULL,
	OwnerID uniqueidentifier NULL,
	DirectorID uniqueidentifier NULL,
	[Admin] bit NULL,
	IsTemporary bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FormElementTableType')
-- DROP TYPE dbo.FormElementTableType

CREATE TYPE [dbo].[FormElementTableType] AS TABLE(
	ElementID	uniqueidentifier NOT NULL PRIMARY KEY CLUSTERED,
	TemplateElementID uniqueidentifier NULL,
	InstanceID uniqueidentifier NOT NULL,
	RefElementID uniqueidentifier NULL,
	Title nvarchar(2000) NULL,
	Name varchar(100) NULL,
	SequenceNubmer int NOT NULL,
	Necessary bit NULL,
	UniqueValue bit NULL,
	[Type] varchar(20) NOT NULL,
	Help nvarchar(2000) NULL,
	Info nvarchar(max) NULL,
	[Weight] float NULL,
	TextValue nvarchar(max) NULL,
	FloatValue float NULL,
	BitValue bit NULL,
	DateValue datetime NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'DashboardTableType')
--DROP TYPE dbo.DashboardTableType

CREATE TYPE [dbo].[DashboardTableType] AS TABLE(
	UserID			uniqueidentifier NOT NULL,
	NodeID			uniqueidentifier NOT NULL,
	RefItemID		uniqueidentifier NULL,
	[Type]			varchar(20) NOT NULL,
	SubType			varchar(20) NULL,
	Info			nvarchar(max) NULL,
	Removable		bit	NULL,
	SenderUserID	uniqueidentifier NULL,
	SendDate		datetime NULL,
	ExpirationDate	datetime NULL,
	Seen			bit NULL,
	ViewDate		datetime NULL,
	Done			bit NULL,
	ActionDate		datetime NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FormFilterTableType')
--DROP TYPE dbo.FormFilterTableType

CREATE TYPE [dbo].[FormFilterTableType] AS TABLE(
	ElementID		uniqueidentifier NOT NULL,
	OwnerID			uniqueidentifier NULL,
	[Text]			nvarchar(max) NULL,
	[TextItems]		nvarchar(max) NULL,
	[Or]			bit NULL,
	Exact			bit NULL,
	DateFrom		datetime NULL,
	DateTo			datetime NULL,
	FloatFrom		float NULL,
	FloatTo			float NULL,
	[Bit]			bit,
	[Guid]			uniqueidentifier NULL,
	GuidItems		varchar(max) NULL,
	Compulsory		bit NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'SentMessageTableType')
--DROP TYPE dbo.SentMessageTableType

CREATE TYPE [dbo].[SentMessageTableType] AS TABLE(
    RefItemID		UNIQUEIDENTIFIER NOT NULL,
    ReceiverUserID	UNIQUEIDENTIFIER NOT NULL,
    ReceiverAddress	VARCHAR(100) NOT NULL,
    SenderAddress	VARCHAR(100) NOT NULL,
	MessageText		NVARCHAR(MAX) NOT NULL,
	UserStatus		VARCHAR(20) NOT NULL,
    SubjectType		VARCHAR(20) NOT NULL,
    [Action]		VARCHAR(20) NOT NULL,
    Media			VARCHAR(20) NOT NULL,
    [Language]		VARCHAR(20) NOT NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'MessageTableType')
--DROP TYPE dbo.MessageTableType

CREATE TYPE [dbo].[MessageTableType] AS TABLE(
    MessageID		UNIQUEIDENTIFIER NOT NULL,
    SenderUserID	UNIQUEIDENTIFIER NOT NULL,
    Title			NVARCHAR(512) NULL,
    MessageText		NVARCHAR(MAX) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'EmailQueueItemTableType')
--DROP TYPE dbo.EmailQueueItemTableType

CREATE TYPE [dbo].[EmailQueueItemTableType] AS TABLE(
    ID				BIGINT NULL,
    SenderUserID	UNIQUEIDENTIFIER NULL,
    [Action]		VARCHAR(50) NOT NULL,
    Email			NVARCHAR(200) NOT NULL,
    Title			NVARCHAR(2000) NULL,
    EmailBody		NVARCHAR(MAX) NOT NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TaggedItemTableType')
--DROP TYPE dbo.TaggedItemTableType

CREATE TYPE [dbo].[TaggedItemTableType] AS TABLE(
    ContextID		UNIQUEIDENTIFIER NOT NULL,
    TaggedID		UNIQUEIDENTIFIER NULL,
    ContextType		VARCHAR(50) NULL,
    TaggedType		VARCHAR(50) NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'PrivacyAudienceTableType')
--DROP TYPE dbo.PrivacyAudienceTableType

CREATE TYPE [dbo].[PrivacyAudienceTableType] AS TABLE(
    ObjectID		uniqueidentifier NULL,
    RoleID			uniqueidentifier NULL,
    PermissionType	varchar(50) NULL,
    Allow			bit NULL,
    ExpirationDate	datetime NULL
)
GO


IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'CNExtensionTableType')
--DROP TYPE dbo.CNExtensionTableType

CREATE TYPE [dbo].[CNExtensionTableType] AS TABLE(
    OwnerID			uniqueidentifier NULL,
    Extension		varchar(50) NOT NULL,
    Title			nvarchar(100) NULL,
    SequenceNumber	int NULL,
    [Disabled]		bit NULL
)
GO