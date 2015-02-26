alter table CollateralOrderDetails drop constraint fkCOD_Group
alter table CollateralOrderDetails drop constraint fkCODetail_Collateral
go
alter table CollateralOrderDetails add 
	[ItemName] [nvarchar](100) null,
	[GroupName] [nvarchar](100) null,
	[GroupQuantity] [int] null,
	[CollateralTypeNKID] [int] not null constraint [defCOD1] default(0),
	[CollateralTypeName] [nvarchar](100) null,
	[CollateralNKID] [int] not null constraint [defCOD2] default(0)
alter table CollateralOrderDetails drop constraint [defCOD1]
alter table CollateralOrderDetails drop constraint [defCOD2]
go
update CollateralOrderDetails set [CollateralNKID] = c.CollateralID, [ItemName] = c.Name, [CollateralTypeNKID] = ct.CollateralTypeID, 
	[CollateralTypeName] = ct.Name, [GroupName] = g.Name, [GroupQuantity] = cgi.Quantity
from CollateralOrderDetails cod
	inner join CollateralItems c on cod.CollateralID = c.CollateralID
	inner join CollateralTypes ct on c.CollateralTypeID = ct.CollateralTypeID
	left join CollateralItems g on cod.GroupID = g.CollateralID
	left join CollateralGroupItems cgi on cgi.CollateralID = c.CollateralID and cgi.GroupID = cod.GroupID
go
alter table CollateralOrderDetails drop column CollateralID
go
alter table CollateralOrderDetails add [GroupNKID] [int] null
go
update CollateralOrderDetails set [GroupNKID] = [GroupID]
go
alter table CollateralOrderDetails drop column [GroupID]
go

alter table CollateralItems add [IsActive] [bit] not null constraint [defCIActive] default(1)
alter table CollateralItems drop constraint [defCIActive]
go
--select * from CollateralOrderDetails