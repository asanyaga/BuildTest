
if not exists(select * from information_schema.columns where table_name = 'tblLineItems' and column_name = 'ReturnsNoteReason')
	begin
		alter table tblLineItems add ReturnsNoteReason nvarchar(250)
		print 'Asta Lavista 1'
	end
	
if not exists(select * from information_schema.columns where table_name = 'tblLineItems' and column_name = 'Other')
	begin
		alter table tblLineItems add Other nvarchar(250)
		print 'Asta Lavista 2'
	end