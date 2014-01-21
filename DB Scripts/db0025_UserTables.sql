create table [Users] (
	[UserID] [int] identity(1,1) not null,
	[FirstName] [nvarchar](50) not null,
	[LastName] [nvarchar](50) not null, 
	[CompanyName] [nvarchar](50) not null, 
	[Address1] [nvarchar](100) null, 
	[Address2] [nvarchar](100) null, 
	[City] [nvarchar](50) null, 
	[State] [nvarchar](5) null, 
	[Zip] [nvarchar](12) null, 
	[Email] [nvarchar](50) not null, 
	[Password] [nvarchar](200) not null,
	[BusinessPhone] [nvarchar](20) null, 
	[CellPhone] [nvarchar](20) null, 
	[Title] [nvarchar](50) null, 
	[UserType] [nvarchar](50) not null, 
	[Role] [nvarchar](50) not null
	CONSTRAINT [pkUsers] PRIMARY KEY CLUSTERED ([UserID])
) on [PRIMARY]
go

