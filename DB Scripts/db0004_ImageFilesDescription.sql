/*
alter table [ImageFiles] add [Description] [nvarchar](1000) not null constraint [defDesc] default('')
alter table [ImageFiles] drop constraint [defDesc]
go
alter table [ImageFiles] drop column [Description]
go
*/