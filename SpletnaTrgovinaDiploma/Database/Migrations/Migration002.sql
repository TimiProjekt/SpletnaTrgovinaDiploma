 CREATE TABLE [dbo].[ItemDescriptions]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [ItemId] [int] NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [Description] [nvarchar](max) NOT NULL
);

ALTER TABLE [dbo].[ItemDescriptions]  WITH CHECK ADD  CONSTRAINT [FK_ItemDescriptions_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE