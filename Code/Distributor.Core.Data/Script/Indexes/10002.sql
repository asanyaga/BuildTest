GO
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>]
ON [dbo].[tblDocument] ([DocumentDateIssued])
INCLUDE ([Id],[OrderParentId])
GO