 CREATE TABLE [dbo].[ItemDescription]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [ItemId] [int] NOT NULL,
    [Name] [nvarchar](1024) NOT NULL,
    [Description] [nvarchar](1024) NOT NULL
);

ALTER TABLE [dbo].[ItemDescription]  WITH CHECK ADD  CONSTRAINT [FK_ItemDescription_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE