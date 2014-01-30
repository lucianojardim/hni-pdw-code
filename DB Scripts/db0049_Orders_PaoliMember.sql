alter table [CollateralOrders] add 	
	[PaoliMemberID] [int] null constraint [fkCOrder_PaoliMember] foreign key references [Users](UserID)
go
alter table [CollateralOrders] drop column [MemberFirstName]
alter table [CollateralOrders] drop column [MemberLastName]
go