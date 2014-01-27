alter table [CollateralItems] drop column [Status]
go

create table [CollateralOrders] (
	[OrderID] [int] not null identity(1,1),
	[RequestingParty] [int] not null,
	[MemberFirstName] [nvarchar](50) null,
	[MemberLastName] [nvarchar](50) null,
	[PaoliRepGroupID] [int] null constraint [fkCOrder_SalesRep] foreign key references [Companies](CompanyID),
	[PaoliRepGroupMemberID] [int] null constraint [fkCOrder_SalesRepMember] foreign key references [Users](UserID),
	[DealerID] [int] null constraint [fkCOrder_Dealer] foreign key references [Companies](CompanyID),
	[DealerMemberID] [int] null constraint [fkCOrder_DealerMember] foreign key references [Users](UserID),
	[EndUserFirstName] [nvarchar](50) null,
	[EndUserLastName] [nvarchar](50) null,
	[EndUserPhoneNumber] [nvarchar](20) null,
	[EndUserEMailAddress] [nvarchar](50) null,
	[ShippingType] [int] not null,
	[ShippingAddressType] [int] not null,
	[ShippingFedexAccount] [nvarchar](50) null,
	[ShippingFirstName] [nvarchar](50) null,
	[ShippingLastName] [nvarchar](50) null,
	[ShippingCompanyName] [nvarchar](50) null,
	[ShippingAddress1] [nvarchar](100) null,
	[ShippingAddress2] [nvarchar](100) null,
	[ShippingCity] [nvarchar](50) null,
	[ShippingState] [nvarchar](5) null,
	[ShippingZip] [nvarchar](15) null,
	[ShippingPhoneNumber] [nvarchar](20) null,
	[ShippingEmailAddress] [nvarchar](50) null,
	[OrderDate] [datetime] not null,
	CONSTRAINT [pkCollateralOrders] PRIMARY KEY CLUSTERED ([OrderID])
) on [primary]
go


create table [CollateralOrderDetails] (
	[DetailID] [int] not null identity(1,1),
	[OrderID] [int] not null constraint [fkCODetail_Order] foreign key references [CollateralOrders]([OrderID]),
	[CollateralID] [int] not null constraint [fkCODetail_Collateral] foreign key references [CollateralItems]([CollateralID]),
	[Quantity] [int] not null,
	CONSTRAINT [pkCollateralOrderDetails] PRIMARY KEY CLUSTERED ([DetailID])	
) on [primary]
go