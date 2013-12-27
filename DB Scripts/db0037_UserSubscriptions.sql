create table [Subscriptions] (
	[SubscriptionID] [int] identity(1,1) not null,
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkSubscriptions] PRIMARY KEY CLUSTERED ([SubscriptionID])
) on [primary]
go

create table [UserSubscriptions] (
	[SubscriptionID] [int] not null CONSTRAINT [fkUSub_Subscription] FOREIGN KEY REFERENCES [Subscriptions] ([SubscriptionID]),
	[UserID] [int] not null CONSTRAINT [fkUSub_User] FOREIGN KEY REFERENCES [Users] ([UserID]),
	CONSTRAINT [pkUserSubscriptions] PRIMARY KEY CLUSTERED ([SubscriptionID], [UserID])
) on [primary]
go
/*
insert into [Subscriptions] ([Name]) values ('Test 1')
*/