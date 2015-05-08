alter table [Users] add
	[CreatedByUser] [int] null CONSTRAINT [fkUsers_CreatedByUser] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[CreatedDateTime] [datetime] null
go
