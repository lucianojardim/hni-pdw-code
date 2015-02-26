alter table [Attributes] add [DetailItem] [bit] not null constraint [defDI] default(1)
alter table [Attributes] drop constraint [defDI]
go