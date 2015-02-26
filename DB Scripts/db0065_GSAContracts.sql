create table [GSAContracts] (
	[ContractID] [int] not null identity(1,1),
	[Name] [nvarchar](100) not null,
	CONSTRAINT [pkGSAContracts] PRIMARY KEY CLUSTERED ([ContractID])
) on [primary]
go

insert into [GSAContracts] ([Name]) values
	('Federal - GS-28F-0006T'),
	('Alabama: 4008429 T390 Office Furniture'),
	('California: CMAS 4-13-71-0104B'),
	('Connecticut: 07PSX0285'),
	('Florida: 425-001-12-1  (View Complete Contract Info)'),
	('Kentucky: MA-758 1100000660 1'),
	('Mississippi: 5-420-24820-12'),
	('New Jersey: 81636'),
	('New Mexico: Price Agreement Number 30-000-00-00015'),
	('North Carolina: 425-B Wood Desks, Credenzas, Conference Tables, etc.'),
	('Pennsylvania: 4400009465'),
	('South Carolina: 5000011597'),
	('Texas: TXMAS-7-711080 (View Complete Contract Info)'),
	('Wisconsin: 15-42594-900'),
	('Baltimore Metro Contract 2008-01'),
	('BuyBoard Cooperative Contract: Buyboard'),
	('IUCPG (Ohio State University) 05-Office Furniture'),
	('Lockheed Martin: NPG020'),
	('Miami-Dade County: 1072-1/16-1'),
	('Navy BPA: N00189-11-A-0126'),
	('NCPA: NCPA07-14')
go

alter table [SpecRequests] add [ContractID] [int] null CONSTRAINT fkSpecRequest_GSAContract FOREIGN KEY REFERENCES [GSAContracts]([ContractID])
go