USE [ProjectCollaborationDb]
GO

/****** Object:  Table [dbo].[Tasks]    Script Date: 20-03-2025 08:55:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[AssignedTo] [int] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tasks] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD FOREIGN KEY([AssignedTo])
REFERENCES [dbo].[Users] ([Id])
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD CHECK  (([Status]='Done' OR [Status]='In Progress' OR [Status]='To Do'))
GO


