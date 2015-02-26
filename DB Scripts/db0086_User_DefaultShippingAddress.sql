alter table [Users] add [DefaultShippingAddress] [int] not null constraint [defShippingAddress] default(1)
alter table [Users] drop constraint [defShippingAddress]
go

alter table [Users] add 
	[ViewPerfData] [bit] not null constraint [defViewPerfData] default(0),
	[TierGroup] [nvarchar](50) null
alter table [Users] drop constraint [defViewPerfData]

alter table [Companies] add
	[SignedUpForTrip] [bit] not null constraint [defSignedUpForTrip] default(0),
	[TripGroup] [nvarchar](50) null,
	[TierGroup] [nvarchar](50) null
alter table [Companies] drop constraint [defSignedUpForTrip]
go