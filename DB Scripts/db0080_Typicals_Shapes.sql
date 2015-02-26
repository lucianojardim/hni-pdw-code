declare @attId int

insert into [TAttributes] (Name, DetailItem) values ('Shape', 1)
select @attId = SCOPE_IDENTITY()

insert into [TAttributeOptions] (Name, AttributeID) values ('L Unit', @attId)
insert into [TAttributeOptions] (Name, AttributeID) values ('U Unit', @attId)
insert into [TAttributeOptions] (Name, AttributeID) values ('Freestanding', @attId)
go
