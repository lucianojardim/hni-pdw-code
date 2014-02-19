alter table [Companies] add [BusinessUnitName] [nvarchar](200) null
go

alter table [CollateralOrders] drop column RequestingPartyName
alter table [CollateralOrders] drop column ShippingPartyName
go

alter function dbo.GetPartyName( @partyType int, @paoliMemberID int, @salesRepID int, @salesRepMemberID int,
	@dealerID int, @dealerMemberID int, @endUserFirst nvarchar(100), @endUserLast nvarchar(100) )
returns nvarchar(300)
begin
	declare @companyID int, @userID int
	select @companyID = null, @userID = null
	
	if( @partyType = 0 )
	begin
		set @userID = @paoliMemberID
	end
	else if( @partyType = 1 )
	begin
		set @companyID = @salesRepID
		set @userID = @salesRepMemberID
	end
	else if( @partyType = 2 )
	begin
		set @companyID = @dealerID
		set @userID = @dealerMemberID
	end
	else if( @partyType = 3 )
	begin
		return @endUserFirst + ' ' + @endUserLast
	end
	
	declare @displayName nvarchar(300)
	if( @userID is not null )
	begin
		select @displayName = c.Name + case when c.BusinessUnitName is not null then ' - ' + c.BusinessUnitName else '' end + ' - ' + u.FirstName + ' ' + u.LastName
			from Users u inner join Companies c on u.CompanyID = c.CompanyID
			where u.UserID = @userID
	end
	else if( @companyID is not null )
	begin
		select @displayName = Name + case when BusinessUnitName is not null then ' - ' + BusinessUnitName else '' end
			from Companies 
			where CompanyID = @companyID
	end
	return @displayName
end
go

alter table [CollateralOrders] add [RequestingPartyName] as [dbo].GetPartyName( RequestingParty, PaoliMemberID, PaoliRepGroupID, PaoliRepGroupMemberID,
	DealerID, DealerMemberID, EndUserFirstName, EndUserLastName )
alter table [CollateralOrders] add [ShippingPartyName] as [dbo].GetPartyName( ShippingParty, SPPaoliMemberID, SPPaoliRepGroupID, SPPaoliRepGroupMemberID,
	SPDealerID, SPDealerMemberID, SPEndUserFirstName, SPEndUserLastName )
go