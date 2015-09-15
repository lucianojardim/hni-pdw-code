alter table [Users] add [AllowNewsMgmt] [bit] not null constraint [defANM] default(0)
alter table [Users] drop constraint [defANM]
go