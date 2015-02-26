/*
select * from ImportSpecRequests where [Dealer] not in (select Name from [Companies] where [CompanyType] = 3)
select * from ImportSpecRequests where [Paoli Sales Rep Group] not in (select Name from [Companies] where [CompanyType] = 2)
select distinct([Dealer]) from ImportSpecRequests where [Dealer] not in (select Name from [Companies] where [CompanyType] = 3)
select distinct([Paoli Sales Rep Group]) from ImportSpecRequests where [Paoli Sales Rep Group] not in (select Name from [Companies] where [CompanyType] = 2)

select * from ImportSpecRequests where [Spec Team Member] not in (select FirstName + ' ' + LastName from [Users] where [AccountType] = 4)

select * from ImportSpecRequests where ISNUMERIC([SPL Quote]) = 0 and [Spl quote] is not null
*/
INSERT INTO [PaoliPDW].[dbo].[SpecRequests]
           ([Name],[ProjectName],
           [PaoliSalesRepGroupID],
           [PrimaryCompanyID],[DealerSalesRepID],
           [PaoliSpecTeamMemberID],[RequestDate],
           [IsGSA],[SavedLocation],[ListPrice],
           [FeaturedSeries],
           [SeriesList],
           [Received],[SPLQuote],[LastModifiedDate],
           [Material],[Finish],[IsGoodForWeb],[AvailableForIn2],[Footprint],[Notes])
(select [Project Number], [Project Name], 
	(select [CompanyID] from [Companies] salesRepGroup where salesRepGroup.[Name] = [Paoli Sales Rep Group]),
	(select [CompanyID] from [Companies] dealer where dealer.[Name] = [Dealer]),null,
	(select [UserID] from [Users] specTeam where specTeam.[FirstName] + ' ' + specTeam.[LastName] = [Spec Team Member]),[Created],
	case [GSA] when 'GSA' then 1 else 0 end, [Saved Location], [Overall List Price], 
	case when [series] is null then null when CHARINDEX(',', [Series]) > 0 then SUBSTRING([Series], 1, charindex(',', [Series]) - 1) else [Series] end,
	case when [series] is null then null 
		when CHARINDEX(',', [series]) = LEN([series]) then null
		when charindex(',', [Series]) > 0 then ltrim(SUBSTRING([Series], charindex(',', [Series]) + 1, LEN([series]) - charindex(',', [Series])))
		else null end,
		[Received], case when isnumeric([SPL Quote]) = 1 then [SPL Quote] else null end, GETUTCDATE(),
		null, null, 0, 0, null, [Comments]
	from ImportSpecRequests)

update SpecRequests set FeaturedSeries = REPLACE(FeaturedSeries, 'R-Series', 'R Series'), SeriesList = REPLACE(cast(SeriesList as nvarchar(500)), 'R-Series', 'R Series')

update SpecRequests set SeriesList = REPLACE(cast(SeriesList as nvarchar(500)), 'conferencing', 'Conferencing Program')
update SpecRequests set FeaturedSeries = 'Conferencing Program' where FeaturedSeries = 'conferencing'