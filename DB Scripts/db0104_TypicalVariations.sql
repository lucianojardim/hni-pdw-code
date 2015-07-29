declare @tAttId int
insert into TAttributes ([Name], [DetailItem]) values ('Variation', 0)
select @tAttId = @@IDENTITY

insert into TypicalTextAttributes ([Value], [AttributeID], [TypicalID]) (select 'Base', @tAttId, TypicalID from Typicals)
go
