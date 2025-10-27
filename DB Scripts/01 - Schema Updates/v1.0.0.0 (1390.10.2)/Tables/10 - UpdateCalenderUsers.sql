USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[Calender_Users]    Script Date: 11/03/2011 13:56:33 ******/
CREATE TABLE [dbo].[Calender_Users](
	[CalenderID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](max) NULL,
	[Done] [bit] NULL,
 CONSTRAINT [PK_Calender_Users] PRIMARY KEY CLUSTERED 
(
	[CalenderID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Calender_Users]  WITH CHECK ADD  CONSTRAINT [FK_Calender_Users_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[Calender_Users] CHECK CONSTRAINT [FK_Calender_Users_aspnet_Users]
GO

ALTER TABLE [dbo].[Calender_Users]  WITH CHECK ADD  CONSTRAINT [FK_Calender_Users_Calendars] FOREIGN KEY([CalenderID])
REFERENCES [dbo].[Calendars] ([ID])
GO

ALTER TABLE [dbo].[Calender_Users] CHECK CONSTRAINT [FK_Calender_Users_Calendars]
GO

insert into [dbo].[Calender_Users](CalenderID, UserID, Status, Done)
select [CalendarUsers].CalendarID, [CalendarUsers].UserID,
	[CalendarUsers].Status, 0
	from [CalendarUsers]
	where [CalendarUsers].CalendarID > '00000000-0000-0000-0000-000000000000' and [CalendarUsers].UserID != null
go

drop table [dbo].[CalendarUsers]
go