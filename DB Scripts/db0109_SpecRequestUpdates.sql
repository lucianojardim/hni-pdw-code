alter table [SpecRequests] add 
	[ProjectScope] [nvarchar](50) null,
	[ProjectListPrice] [nvarchar](50) null,
	[IsPhasedProject] [bit] null,
	[NumberOfPhases] [int] null,
	[IsStandardsProgram] [bit] null,
	[ProjectFirstOrderDate] [date] null,
	[Competitors] [nvarchar](max) null,
	[CompetitorSeries] [nvarchar](200) null
go
