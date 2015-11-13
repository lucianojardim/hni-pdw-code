alter table [SpecRequests] add [Status] [nvarchar](10) null,
	[OrderPlacedDate] [date] null,
	[OrderCost] [nvarchar](50) null,
	[AckNumber] [nvarchar](50) null,
	[LostReason] [nvarchar](250) null
go