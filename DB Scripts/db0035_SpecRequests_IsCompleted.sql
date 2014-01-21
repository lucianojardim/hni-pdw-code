alter table [SpecRequests] add [IsCompleted] [bit] not null constraint [defIC] default(0)
alter table [SpecRequests] drop constraint [defIC]
go