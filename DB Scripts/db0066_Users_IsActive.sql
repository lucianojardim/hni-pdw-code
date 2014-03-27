alter table [users] add [IsActive] [bit] not null constraint [defUIA] default(1)
go
alter table [Users] drop constraint [defUIA]
go