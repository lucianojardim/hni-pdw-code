create table [UserLogins] (
	[LoginRecID] [int] identity(1,1) not null,
	[UserID] [int] not null CONSTRAINT fkLogin_User FOREIGN KEY REFERENCES [Users]([UserID]),
	[LoginDate] [datetime] not null,
	CONSTRAINT [pkUserLogins] PRIMARY KEY CLUSTERED ([LoginRecID])	
) on [PRIMARY]
go
