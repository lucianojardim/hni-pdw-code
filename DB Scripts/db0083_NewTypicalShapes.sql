declare @attId int
select @attId = AttributeID from [TAttributes] where [Name] = 'Shape'

insert into [TAttributeOptions] (Name, AttributeID) values ('Work Wall', @attId)
insert into [TAttributeOptions] (Name, AttributeID) values ('Teaming', @attId)
insert into [TAttributeOptions] (Name, AttributeID) values ('Meeting', @attId)
go