alter table [Companies] alter column [Name] [nvarchar](100) not null
go

INSERT INTO [Companies]
	([Name],[Address1],[Address2],[City],[State],[Zip],[Phone],[FAX],[MasterID],[SubCompanyIDs],
		[TripIncentive],[CompanyType],[TerritoryID],
		[PublicAddress1],[PublicAddress2],[PublicCity],[PublicState],[PublicZip],[PublicPhone],[PublicFAX],
		[ContactEmail],[WebSite],[PublicContactEmail],[PublicWebSite],[PublicDisplayName],[BusinessUnitName],
		[PaoliMemberID],[PaoliSalesRepMemberID],[SignedUpForTrip],[TripGroup],[TierGroup],[ImageFileName])
    (SELECT Distinct EndCustomer, null, null, null, null, null, null, null, null, null,
		0, 4, null,
		null, null, null, null, null, null, null,
		null, null, null, null, null, null,
		null, null, 0, null, null, null from projects where EndCustomer is not null)
go

alter table [Projects] add [EndCustomerID] [int] null CONSTRAINT [fkProject_EndCustomer] FOREIGN KEY REFERENCES [Companies] ([CompanyID])
go

update [Projects] set [EndCustomerID] = [c].[CompanyID]
	from [Projects] inner join [Companies] [c] on [projects].EndCustomer = [c].[Name] and [c].CompanyType = 4
go

alter table [Projects] drop column [EndCustomer]
go