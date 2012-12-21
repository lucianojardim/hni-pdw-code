alter table [ImageFiles] add [MIMEType] [nvarchar](100) not null constraint [defMT] default('')
alter table [ImageFiles] drop constraint [defMT]
go
update [ImageFiles] set [MIMEType] = 'image/jpeg' where OriginalExtension = '.jpg'
go
