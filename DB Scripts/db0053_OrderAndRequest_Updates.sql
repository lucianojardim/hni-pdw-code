alter table [SpecRequests] add [LaminatePreference] [nvarchar](10) null
go

alter table [CollateralOrderShipments] add [ShipmentDate] [datetime] not null constraint [defshipmentdate] default(getutcdate())
alter table [CollateralOrderShipments] drop constraint [defshipmentdate]
go

alter table [SpecRequestFiles] add [IsSpecTeam] [bit] not null constraint [defIsSpecTeam] default(0)
alter table [SpecRequestFiles] drop constraint [defIsSpecTeam]
go
