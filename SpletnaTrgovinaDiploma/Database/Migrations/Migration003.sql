ALTER TABLE [dbo].[Items] 
ADD [ProductCode] [nvarchar](max) NULL;

ALTER TABLE [dbo].[Items] 
ADD [Availability] [int] NULL DEFAULT 0;
