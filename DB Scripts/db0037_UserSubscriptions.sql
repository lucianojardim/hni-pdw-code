create table [UserSubscriptions] (
	[UserID] [int] not null CONSTRAINT [fkUSub_User] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[ProductIntroductions] [bit] not null,
	[BehindTheScenes] [bit] not null,
	[MeetOurMembers] [bit] not null,
	[ProgramChanges] [bit] not null,
	[PricelistUpdates] [bit] not null,
	[QuoteRequests] [bit] not null,
	[SMSAlerts] [bit] not null,
	[SMSPhoneNumber] [nvarchar](20) null,
	CONSTRAINT [pkUserSubscriptions] PRIMARY KEY CLUSTERED ([UserID])
) on [primary]
go
