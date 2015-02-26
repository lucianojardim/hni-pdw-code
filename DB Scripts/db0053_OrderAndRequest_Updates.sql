alter table [SpecRequests] add [LaminatePreference] [nvarchar](10) null
go

alter table [CollateralOrderShipments] add [ShipmentDate] [datetime] not null constraint [defshipmentdate] default(getutcdate())
alter table [CollateralOrderShipments] drop constraint [defshipmentdate]
go

alter table [SpecRequestFiles] add [IsSpecTeam] [bit] not null constraint [defIsSpecTeam] default(0)
alter table [SpecRequestFiles] drop constraint [defIsSpecTeam]
go

alter table [SpecRequests] add [DealerPOCText] [nvarchar](max) null
go

alter table [CollateralOrders] add
	[ShippingParty] [int] not null constraint [defShippingParty] default(0),
	[SPPaoliMemberID] [int] null constraint [fkCOrder_SPPaoliMember] foreign key references [Users](UserID),
	[SPPaoliRepGroupID] [int] null constraint [fkCOrder_SPSalesRep] foreign key references [Companies](CompanyID),
	[SPPaoliRepGroupMemberID] [int] null constraint [fkCOrder_SPSalesRepMember] foreign key references [Users](UserID),
	[SPDealerID] [int] null constraint [fkCOrder_SPDealer] foreign key references [Companies](CompanyID),
	[SPDealerMemberID] [int] null constraint [fkCOrder_SPDealerMember] foreign key references [Users](UserID),
	[SPEndUserFirstName] [nvarchar](50) null,
	[SPEndUserLastName] [nvarchar](50) null,
	[SPEndUserPhoneNumber] [nvarchar](20) null,
	[SPEndUserEMailAddress] [nvarchar](50) null
alter table [CollateralOrders] drop constraint [defShippingParty]
go

select [OrderID], [RequestingParty], [PaoliMemberID], [PaoliRepGroupID], [DealerID], [EndUserFirstName] from [CollateralOrders]
go
/*
update [CollateralOrders] set [ShippingParty] = 2, [SPDealerID] = [DealerID] where OrderID in (10000, 10001, 10010, 10011)
update [CollateralOrders] set [ShippingParty] = 1, [SPPaoliRepGroupID] = [PaoliRepGroupID], [SPPaoliRepGroupMemberID] = [PaoliRepGroupMemberID] where OrderID in (10002, 10007)
*/

create function dbo.GetPartyName( @partyType int, @paoliMemberID int, @salesRepID int, @salesRepMemberID int,
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
		select @displayName = c.Name + ' - ' + u.FirstName + ' ' + u.LastName
			from Users u inner join Companies c on u.CompanyID = c.CompanyID
			where u.UserID = @userID
	end
	else if( @companyID is not null )
	begin
		select @displayName = Name 
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