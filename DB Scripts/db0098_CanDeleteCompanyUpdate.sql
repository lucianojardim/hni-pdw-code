create procedure spCanDeleteCompany( @companyId int ) as
begin
	select dbo.CanDeleteCompany(@companyId)
end
go