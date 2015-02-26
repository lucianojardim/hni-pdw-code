alter table [SpecRequests] add [IsOnHold] [bit] not null constraint [defIsOnHold] default(0)
alter table [SpecRequests] drop constraint [defIsOnHold]
go