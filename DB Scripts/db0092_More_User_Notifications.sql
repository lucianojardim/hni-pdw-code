alter table [UserNotifications] add
	[NewCollateralOrderMyDealers] [bit] not null constraint [defUserNot1] default(1),
	[NewCollateralOrderShipmentMyDealers] [bit] not null constraint [defUserNot2] default(1),
	[NewSpecRequestMyDealers] [bit] not null constraint [defUserNot3] default(1),
	[CompleteSpecRequestMyDealers] [bit] not null constraint [defUserNot4] default(1),
	[UpdateSpecRequestMyDealers] [bit] not null constraint [defUserNot5] default(1),
	[ReOpenSpecRequestMyDealers] [bit] not null constraint [defUserNot6] default(1)
go
alter table [UserNotifications] drop constraint [defUserNot1]
alter table [UserNotifications] drop constraint [defUserNot2]
alter table [UserNotifications] drop constraint [defUserNot3]
alter table [UserNotifications] drop constraint [defUserNot4]
alter table [UserNotifications] drop constraint [defUserNot5]
alter table [UserNotifications] drop constraint [defUserNot6]
go
