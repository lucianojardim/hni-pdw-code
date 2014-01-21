alter table [ImageFiles] add [ImageContent] [int] not null constraint [defImageType] default(1)
go
alter table [ImageFiles] drop constraint [defImageType]
go