alter table [SpecRequests] add 	
	[CreatedByUserId] [int] null constraint [fkSRequest_CreatedBy] foreign key references [Users](UserID)
go
