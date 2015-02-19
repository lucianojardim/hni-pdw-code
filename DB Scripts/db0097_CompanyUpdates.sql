create function [CanDeleteCompany] (@CompanyID int)
returns int
as
begin
	if exists( select * from SpecRequests sr where sr.PrimaryCompanyID = @CompanyID or sr.PaoliSalesRepGroupID = @CompanyID ) or
		exists( select * from CollateralOrders co where @CompanyID in (co.DealerID, co.SPDealerID, co.PaoliRepGroupID, co.SPPaoliRepGroupID ) ) or 
		exists( select * from Users u where u.CompanyID = @CompanyID ) or
		exists( select * from Projects p where p.DealerID = @CompanyID or p.EndCustomerID = @CompanyID ) or
		exists( select * from eCollateralItems eci where eci.DealershipID = @CompanyID )
	begin
		return 0
	end

	return 1
end
go

alter table [Companies] add [CanDelete] as dbo.CanDeleteCompany(CompanyID)
go

--drop table [TerritoryTripData]
create table [TerritoryTripData] (
	[CompanyID] [int] not null CONSTRAINT [fkTerritoryTripData_Company] FOREIGN KEY REFERENCES [Companies] ([CompanyID]),
	[NewProductPoints] [int] not null,
	[FocusDealerPoints] [int] not null,
	[CurrentTerritoryVol] [int] not null,
	[TerritoryPoints] [int] not null,
	[NumDealerTripsAwarded] [int] not null,
	[DealerTripPoints] [int] not null,
	[EducationPoints] [int] not null,
	[TotalTripPoints] [int] not null,
	[ImportDate] [datetime] not null,
	constraint [pkTerritoryTripData] primary key clustered ([CompanyID])	
) on [primary]
go
