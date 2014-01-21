alter table [ImageFiles] add [Keyword] [nvarchar](1000) not null constraint [defkeyword] default('')
alter table [ImageFiles] drop constraint [defkeyword]
go