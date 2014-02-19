alter table [Typicals] add [IsPublished] [bit] not null constraint [defIsPub] default(1)
alter table [Typicals] drop constraint [defIsPub]
go