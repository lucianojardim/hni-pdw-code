alter table [CollateralOrderDetails] add [GroupID] [int] null constraint [fkCOD_Group] foreign key references [CollateralItems](CollateralID)
go

insert into [CollateralOrderDetails] ([CollateralID], [OrderID], [Quantity], [GroupID])
	(select cgi.[CollateralID], cod.[OrderID], cod.[Quantity] * cgi.Quantity, cod.CollateralID from CollateralOrderDetails cod
		inner join CollateralGroupItems cgi on cod.CollateralID = cgi.[GroupID])

delete from CollateralOrderDetails where CollateralID in (select CollateralID from CollateralItems where IsGroup = 1)
go

alter table [CollateralOrders] add [CreatedByUserID] [int] null
go
update [CollateralOrders] set [CreatedByUserID] = (select top 1 UserID from Users where Email like '%vitaminj%')
go
alter table [CollateralOrders] alter column [CreatedByUserID] [int] not null 
alter table [CollateralOrders] add constraint [fkCO_CreatedUser] foreign key ([CreatedByUserID]) references [Users](UserID)
go

create table [CollateralOrderShipments] (
	[ShipmentID] [int] not null identity(1, 1),
	[OrderID] [int] not null constraint [fkShipment_COrder] foreign key references [CollateralOrders]([OrderID]),
	[Vendor] nvarchar(100) null,
	[TrackingNumber] nvarchar(100) null,
	[GLCode] nvarchar(100) null,
	[ShippingType] nvarchar(100) null,
	[ShippingFedexAccount] nvarchar(50) null,
	[ShippingAttn] nvarchar(200) null,
	[ShippingCompanyName] nvarchar(50) null,
	[ShippingAddress1] nvarchar(100) null,
	[ShippingAddress2] nvarchar(100) null,
	[ShippingCity] nvarchar(50) null,
	[ShippingState] nvarchar(5) null,
	[ShippingZip] nvarchar(15) null,
	[ShippingPhoneNumber] nvarchar(20) null,
	[ShippingEmailAddress] nvarchar(100) null,
	CONSTRAINT [pkCollateralOrderShipments] PRIMARY KEY CLUSTERED ([ShipmentID])	
) on [primary]
go

create table [CollateralOrderShipmentDetails] (
	[DetailID] [int] not null identity(1, 1),
	[ShipmentID] [int] not null constraint [fkShipmentDetail_Shipment] foreign key references [CollateralOrderShipments]([ShipmentID]),
	[OrderDetailID] [int] not null constraint [fkShipmentDetail_CODetail] foreign key references [CollateralOrderDetails]([DetailID]),
	[Quantity] [int] not null,
	CONSTRAINT [pkCollateralOrderShipmentDetails] PRIMARY KEY CLUSTERED ([DetailID])	
) on [primary]
go

alter table [CollateralOrders] add [CanceledByUserID] [int] null constraint [fkCO_CanceledUser] foreign key references [Users](UserID)
alter table [CollateralOrders] add [CanceledOnDateTime] [datetime] null
go
