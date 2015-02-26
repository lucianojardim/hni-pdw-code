drop table [UserSubscriptions]
go

create table [UserNotifications] (
	[UserID] [int] not null CONSTRAINT [FK_User_Notifications] FOREIGN KEY REFERENCES [Users] ([UserID]),
	
	[NewCollateralOrder] [bit] not null,
	[NewCollateralOrderTerritory] [bit] not null,
	[NewCollateralOrderShipment] [bit] not null,
	[NewCollateralOrderShipmentTerritory] [bit] not null,
	
	[NewSpecRequest] [bit] not null,
	[NewSpecRequestTerritory] [bit] not null,
	
	[CompleteSpecRequest] [bit] not null,
	[CompleteSpecRequestTerritory] [bit] not null,
	
	[UpdateSpecRequest] [bit] not null,
	[UpdateSpecRequestTerritory] [bit] not null,
	
	[ReOpenSpecRequest] [bit] not null,
	[ReOpenSpecRequestTerritory] [bit] not null,
	
	constraint [pkUserNotifications] primary key clustered ([UserID])
) on [primary]
go

insert into [UserNotifications] ([UserID], [NewCollateralOrder], [NewCollateralOrderTerritory], [NewCollateralOrderShipment], [NewCollateralOrderShipmentTerritory],
		[NewSpecRequest], [NewSpecRequestTerritory], [CompleteSpecRequest], [CompleteSpecRequestTerritory], [UpdateSpecRequest], [UpdateSpecRequestTerritory],
		[ReOpenSpecRequest], [ReOpenSpecRequestTerritory]) 
	(select [UserID], 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 from [Users])
go
