declare @NodeTypeID uniqueidentifier = N'5E2073DC-5039-4180-BDFD-F79D5032BCF4'
declare @NodeIDs Table(Value uniqueidentifier primary key clustered)
declare @TitleIDs Table(Value uniqueidentifier primary key clustered)
declare @ParagraphIDs Table(Value uniqueidentifier primary key clustered)
declare @ChangeIDs Table(Value uniqueidentifier primary key clustered)
declare @AttIDs Table(Value uniqueidentifier primary key clustered)
declare @ATFIDs Table(Value uniqueidentifier primary key clustered)

insert into @NodeIDs
select NodeID
from [ENDB].[dbo].[CN_Nodes]
where NodeTypeID = @nodeTypeId

insert into @TitleIDs
select TitleID
from [ENDB].[dbo].[WK_Titles]
where OwnerID in (select * from @NodeIDs)

insert into @ParagraphIDs
select ParagraphID
from [ENDB].[dbo].[WK_Paragraphs]
where TitleID in (select * from @TitleIDs)

insert into @ChangeIDs
select ChangeID
from [ENDB].[dbo].[WK_Changes]
where ParagraphID in (select * from @ParagraphIDs)

insert into @AttIDs
select ID
from [ENDB].[dbo].[Attachments]
where ObjectID in (select * from @ParagraphIDs) or
	ObjectID in (select * from @ChangeIDs)
	
insert into @ATFIDs
select ID
from [ENDB].[dbo].[AttachmentFiles]
where AttachmentID in (select * from @AttIDs)


insert into [EKM_App].[dbo].[CN_NodeTypes]
select * from [ENDB].[dbo].[CN_NodeTypes] as s
where s.NodeTypeID = @NodeTypeID

insert into [EKM_App].[dbo].[CN_Nodes]
select * from [ENDB].[dbo].[CN_Nodes] as s
where s.NodeID in (select * from @NodeIDs)

insert into [EKM_App].[dbo].[WK_Titles]
select * from [ENDB].[dbo].[WK_Titles] as s
where s.TitleID in (select * from @TitleIDs)

insert into [EKM_App].[dbo].[WK_Paragraphs]
select * from [ENDB].[dbo].[WK_Paragraphs] as s
where s.ParagraphID in (select * from @ParagraphIDs)

insert into [EKM_App].[dbo].[WK_Changes]
select * from [ENDB].[dbo].[WK_Changes] as s
where s.ChangeID in (select * from @ChangeIDs)

insert into [EKM_App].[dbo].[Attachments]
select * from [ENDB].[dbo].[Attachments] as s
where s.ID in (select * from @AttIDs)

insert into [EKM_App].[dbo].[AttachmentFiles]
select * from [ENDB].[dbo].[AttachmentFiles] as s
where s.ID in (select * from @ATFIDs)
	