alter table [Users] add [IsTempPassword] [bit] not null constraint [defITP] default(0)
alter table [Users] drop constraint [defITP]
go