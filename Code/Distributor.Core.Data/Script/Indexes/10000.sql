CREATE NONCLUSTERED INDEX [DocumentLineitem_index] ON [dbo].[tblLineItems]
(
	[DocumentID] ASC
)
INCLUDE ( 	[Quantity],
	[Value],
	[Vat]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	GO
	CREATE NONCLUSTERED INDEX [Document_index]
    ON [dbo].[tblDocument] ([DocumentTypeId],[DocumentParentId]);
	GO

