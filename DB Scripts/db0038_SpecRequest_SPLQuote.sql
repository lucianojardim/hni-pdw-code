alter table [SpecRequests] alter column [SPLQuote] [nvarchar](20) null
go

update [SpecRequests] set [SPLQuote] = [SPL Quote] 
	from [SpecRequests] inner join [ImportSpecRequests] on [SpecRequests].[Name] = [ImportSpecRequests].[Typical Name]
where ISNUMERIC([SPL Quote]) = 0 and [Spl quote] is not null
go

/*
select * from [SpecRequests] inner join [ImportSpecRequests] on [SpecRequests].[Name] = [ImportSpecRequests].[Project Number]
where ISNUMERIC([SPL Quote]) = 0 and [Spl quote] is not null
*/