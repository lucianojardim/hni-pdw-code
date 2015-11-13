alter table [ImageFiles] add [ImageApplicationList] [nvarchar](max) null
go
update [ImageFiles] set [ImageApplicationList] = 'Private Office' where [ImageApplication] = 1
update [ImageFiles] set [ImageApplicationList] = 'Teaming' where [ImageApplication] = 2
update [ImageFiles] set [ImageApplicationList] = 'Collaborative' where [ImageApplication] = 3
go
alter table [ImageFiles] drop column [ImageApplication]
go
