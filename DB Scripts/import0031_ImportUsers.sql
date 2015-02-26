declare @companyId int
select @companyId = min(CompanyID) from Companies where CompanyType = 1

declare @password nvarchar(200)
select @password = [Password] from Users where Email = 'matt.james@wddsoftware.com'

insert into [Users] ([FirstName], [LastName], [Email], [AccountType], [CompanyID], [Enabled], [Password])
	(select rtrim([F1]), rtrim([F2]), [F3], 
			case [F4] when 'Paoli - Marketing' then 3 when 'Paoli - Spec Team' then 4 when 'Admin' then 1 when 'Paoli - Sales Team' then 6 end, 
			@companyId, 1, @password
		from ImportUsers 
		where [F3] not in (select [Email] from [Users]))

select @companyId = CompanyID from Companies where Name = 'Vitamin J'

update Users set CompanyID = @companyId where Email like '%vitaminj%'

select * from Users order by UserID desc

/*
delete from users where userid >= 6
select CompanyID from Companies where CompanyType = 1
select * from importusers
*/