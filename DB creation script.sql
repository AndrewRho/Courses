use master
go

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'CoursesBot')
	CREATE DATABASE [CoursesBot] 

go

USE [CoursesBot]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.disciplines', N'U') IS NULL
CREATE TABLE [dbo].[disciplines](
	[id] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	CONSTRAINT [PK_disciplines] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF OBJECT_ID(N'dbo.schedules', N'U') IS NULL
CREATE TABLE [dbo].[schedules](
	[id] [uniqueidentifier] NOT NULL,
	[workPlanId] [uniqueidentifier] NOT NULL,
	[timeSlotId] [int] NOT NULL,
	[lectures] [int] NOT NULL,
	[practices] [int] NOT NULL,
	[date] [datetime2](7) NOT NULL,
	[progress] [nvarchar](32) NOT NULL,
	 CONSTRAINT [PK_schedules] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


IF OBJECT_ID(N'dbo.timeSlots', N'U') IS NULL
CREATE TABLE [dbo].[timeSlots](
	[id] [int] NOT NULL,
	[timeFrom] [nvarchar](max) NOT NULL,
	[timeTo] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_timeSlots] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF OBJECT_ID(N'dbo.topics', N'U') IS NULL
CREATE TABLE [dbo].[topics](
	[id] [uniqueidentifier] NOT NULL,
	[disciplineId] [uniqueidentifier] NOT NULL,
	[number] [int] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_topics] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF OBJECT_ID(N'dbo.users', N'U') IS NULL
CREATE TABLE [dbo].[users](
	[id] [bigint] NOT NULL,
	[userName] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF OBJECT_ID(N'dbo.workPlans', N'U') IS NULL
CREATE TABLE [dbo].[workPlans](
	[id] [uniqueidentifier] NOT NULL,
	[userId] [bigint] NOT NULL,
	[topicId] [uniqueidentifier] NOT NULL,
	[lectures] [int] NOT NULL,
	[practices] [int] NOT NULL,
	 CONSTRAINT [PK_workPlans] PRIMARY KEY CLUSTERED (
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


-- foreign keys


ALTER TABLE [dbo].[schedules]  WITH CHECK ADD  CONSTRAINT [FK_schedules_timeSlot] FOREIGN KEY([timeSlotId]) REFERENCES [dbo].[timeSlots] ([id])
GO
ALTER TABLE [dbo].[schedules] CHECK CONSTRAINT [FK_schedules_timeSlot] 
GO
ALTER TABLE [dbo].[schedules]  WITH CHECK ADD  CONSTRAINT [FK_schedules_workPlans] FOREIGN KEY([workPlanId])REFERENCES [dbo].[workPlans] ([id])
GO
ALTER TABLE [dbo].[schedules] CHECK CONSTRAINT [FK_schedules_workPlans]
GO
ALTER TABLE [dbo].[topics]  WITH CHECK ADD  CONSTRAINT [FK_topics_disciplines] FOREIGN KEY([disciplineId]) REFERENCES [dbo].[disciplines] ([id])
GO
ALTER TABLE [dbo].[topics] CHECK CONSTRAINT [FK_topics_disciplines] 
GO
ALTER TABLE [dbo].[workPlans]  WITH CHECK ADD  CONSTRAINT [FK_workPlans_topics] FOREIGN KEY([topicId]) REFERENCES [dbo].[topics] ([id])
GO
ALTER TABLE [dbo].[workPlans] CHECK CONSTRAINT [FK_workPlans_topics] 
GO
ALTER TABLE [dbo].[workPlans]  WITH CHECK ADD  CONSTRAINT [FK_workPlans_users] FOREIGN KEY([userId]) REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[workPlans] CHECK CONSTRAINT [FK_workPlans_users]
GO
