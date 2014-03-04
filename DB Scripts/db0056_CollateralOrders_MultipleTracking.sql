alter table CollateralOrderShipments add 
	[TrackingNumber1] [nvarchar](100) null,
	[TrackingNumber2] [nvarchar](100) null,
	[TrackingNumber3] [nvarchar](100) null,
	[TrackingNumber4] [nvarchar](100) null
go
update CollateralOrderShipments set [TrackingNumber1] = [TrackingNumber]
go
alter table CollateralOrderShipments
	drop column [TrackingNumber]
go
