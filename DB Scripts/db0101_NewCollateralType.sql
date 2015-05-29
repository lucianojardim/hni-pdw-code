insert into CollateralTypes (Name) values ('Upholstery Sample')
go

alter table [CollateralTypes] add [DisplayOrder] [int] null
go

update CollateralTypes set [DisplayOrder] = [CollateralTypeID]
update CollateralTypes set [DisplayOrder] = 1000 where Name = 'Other'
go
