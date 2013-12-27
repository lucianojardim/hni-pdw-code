--select * from importcompanies where [company name] in (select name from Companies)
/*
select * from ImportCompanies
	where [Company Type] not in ('Dealer', 'Paoli Sales Rep Group', 'End User')
*/
INSERT INTO [PaoliPDW].[dbo].[Companies]
	([Name],[TripIncentive],[CompanyType])
	(select rtrim([Company Name]), 0, 
	case [Company Type] when 'Dealer' then 3 when 'Paoli Sales Rep Group' then 2 when 'End User' then 4 when 'Paoli' then 1 end from ImportCompanies
	where [company name] not in (select name from Companies)
)