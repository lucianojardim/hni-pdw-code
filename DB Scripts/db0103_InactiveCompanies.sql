alter table [Companies] add [IsDisabled] [bit] not null constraint [defIsDisabled] default(0)
alter table [Companies] drop constraint [defIsDisabled]
go
