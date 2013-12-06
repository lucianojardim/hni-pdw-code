alter table [Users] add [RecWelcomeEmail] [bit] not null constraint [defRWE] default(1)
alter table [Users] drop constraint [defRWE]
go