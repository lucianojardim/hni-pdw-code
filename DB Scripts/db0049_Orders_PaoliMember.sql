alter table [CollateralOrders] add 	
	[PaoliMemberID] [int] null constraint [fkCOrder_PaoliMember] foreign key references [Users](UserID)
go
alter table [CollateralOrders] drop column [MemberFirstName]
alter table [CollateralOrders] drop column [MemberLastName]
go
DBCC CHECKIDENT ('CollateralOrders', RESEED, 10000)
go
alter table [CollateralOrders] add [Status] [int] not null constraint [defmj1] default(1)
alter table [CollateralOrders] drop constraint [defmj1]
go