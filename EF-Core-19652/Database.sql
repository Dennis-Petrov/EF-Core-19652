USE [EF-Core-19652]
GO
/****** Object:  Table [dbo].[ComboStatuses]    Script Date: 22.01.2020 15:49:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ComboStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ComboStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentEvents]    Script Date: 22.01.2020 15:49:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[StatusCode] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_DocumentEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Documents]    Script Date: 22.01.2020 15:49:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[Id] [uniqueidentifier] NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ComboStatuses]  WITH CHECK ADD  CONSTRAINT [FK_ComboStatuses_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([Id])
GO
ALTER TABLE [dbo].[ComboStatuses] CHECK CONSTRAINT [FK_ComboStatuses_DocumentId]
GO
ALTER TABLE [dbo].[ComboStatuses]  WITH CHECK ADD  CONSTRAINT [FK_ComboStatuses_EventId] FOREIGN KEY([EventId])
REFERENCES [dbo].[DocumentEvents] ([Id])
GO
ALTER TABLE [dbo].[ComboStatuses] CHECK CONSTRAINT [FK_ComboStatuses_EventId]
GO
ALTER TABLE [dbo].[DocumentEvents]  WITH CHECK ADD  CONSTRAINT [FK_DocumentEvents_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([Id])
GO
ALTER TABLE [dbo].[DocumentEvents] CHECK CONSTRAINT [FK_DocumentEvents_DocumentId]
GO
