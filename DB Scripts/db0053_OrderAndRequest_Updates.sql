alter table [SpecRequests] add [LaminatePreference] [nvarchar](10) null
go

alter table [CollateralOrderShipments] add [ShipmentDate] [datetime] not null constraint [defshipmentdate] default(getutcdate())
alter table [CollateralOrderShipments] drop constraint [defshipmentdate]
go