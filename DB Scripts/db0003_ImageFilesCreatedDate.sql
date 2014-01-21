alter table [ImageFiles] add [CreatedDate] [datetime] not null constraint [defCD] default(getDate())
alter table [ImageFiles] drop constraint [defCD]
go