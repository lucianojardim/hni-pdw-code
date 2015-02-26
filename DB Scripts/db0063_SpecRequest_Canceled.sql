alter table [SpecRequests] add
	[CompletedByUserID] [int] null CONSTRAINT fkSpecRequest_CompletedUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[CompletedDateTime] [datetime] null,
	[IsCanceled] [bit] not null constraint [defIsC] default(0),
	[CanceledByUserID] [int] null CONSTRAINT fkSpecRequest_CanceledUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[CanceledDateTime] [datetime] null
go

alter table [SpecRequests] drop constraint [defIsC]
go