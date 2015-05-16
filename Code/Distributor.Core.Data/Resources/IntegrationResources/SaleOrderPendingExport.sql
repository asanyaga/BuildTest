select top 20 * from tbldocument doc where  doc.DocumentTypeId=1 and doc.OrderOrderTypeId='1'
and DocumentReference  not in(select ex.DocumentReference  from tblExportImportAudit ex)
union all
select top 20 * from tbldocument doc where  doc.DocumentTypeId=5 
and DocumentReference  not in(select ex.DocumentReference  from tblExportImportAudit ex)
union all
select top 20 * from tbldocument doc where  doc.DocumentTypeId=8
and DocumentReference  not in(select ex.DocumentReference  from tblExportImportAudit ex)


