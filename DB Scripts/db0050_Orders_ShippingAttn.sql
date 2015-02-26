alter table [CollateralOrders] add [ShippingAttn] [nvarchar](200) null
go
update [CollateralOrders] set [ShippingAttn] = [ShippingFirstName] + ' ' + [ShippingLastName]
go
alter table [CollateralOrders] drop column [ShippingFirstName]
alter table [CollateralOrders] drop column [ShippingLastName]
go
