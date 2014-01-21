alter table [Users] add [Enabled] bit not null constraint [defEnabled] default(1)
alter table [Users] drop constraint [defEnabled]
go