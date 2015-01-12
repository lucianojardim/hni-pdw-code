create table [Projects] (
	[ProjectID] [int] not null identity(1,1),
	[ProjectName] [nvarchar](50) NULL,
	[DealerID] [int] NULL CONSTRAINT [fkProject_Company] FOREIGN KEY REFERENCES [Companies] ([CompanyID]),
	[EndCustomer] [nvarchar](100) NULL,

	[IsGSA] [bit] NULL,
	[ContractID] [int] NULL CONSTRAINT [fkProject_GSAContract] FOREIGN KEY REFERENCES [GSAContracts] ([ContractID]),

	[HasADFirm] [bit] not null,
	[ADFirm] [nvarchar](50) null,
	
	[AnticipatedOrderDate] [datetime] null,
	[AnticipatedShipDate] [datetime] null,

	[VeneerCasegoods] [nvarchar](max) NULL,
	[NetVeneerCasegoods] [float] null,
	[LaminateCasegoods] [nvarchar](max) NULL,
	[NetLaminateCasegoods] [float] null,
	[Conferencing] [nvarchar](max) NULL,
	[NetConferencing] [float] null,
	[Seating] [nvarchar](max) NULL,
	[NetSeating] [float] null,
	
	[SPADone] [bit] not null,
	[Probability] [int] null,
	
	[PrimaryCompetitor] [nvarchar](50) null,
	[Comments] [ntext] null,
	
	[PipelineStatus] [int] not null,
	[ProjectSuccess] [int] not null,
	
	constraint [pkProjects] primary key clustered ([ProjectID])
) on [primary]
go

insert into Projects ([ProjectName], [DealerID], [EndCustomer], ProjectSuccess, HasADFirm, SPADone, PipelineStatus) 
	(select ProjectName, PrimaryCompanyID, EndCustomer, 0, 0, 0, 1 from SpecRequests where PrimaryCompanyID is not null and EndCustomer is not null and ProjectName is not null)
go

alter table [SpecRequests] add 
	[ProjectID] [int] NULL CONSTRAINT [fkSpecRequest_Project] FOREIGN KEY REFERENCES [Projects] ([ProjectID]),
	[OldProjectName] [nvarchar](50) NULL,
	[OldEndCustomer] [nvarchar](100) NULL,
	[OldIsGSA] [bit] NULL,
	[OldContractID] [int] NULL
go
update [SpecRequests] set
	[OldProjectName] = [ProjectName],
	[OldEndCustomer] = [EndCustomer],
	[OldIsGSA] = [IsGSA],
	[OldContractID] = [ContractID]
go
alter table [SpecRequests] drop column [ProjectName]
alter table [SpecRequests] drop column [EndCustomer]
alter table [SpecRequests] drop column [IsGSA]
alter table [SpecRequests] drop constraint [fkSpecRequest_GSAContract]
alter table [SpecRequests] drop column [ContractID]
go

update [SpecRequests] set [projectid] = [p].[projectid]
	from [Projects] [p]
where [OldProjectName] = p.ProjectName and
	[oldendcustomer] = p.EndCustomer and
	PrimaryCompanyID = p.DealerID
go


insert into Projects ([ProjectName], [DealerID], [EndCustomer], ProjectSuccess, HasADFirm, SPADone, PipelineStatus) 
	(select ProjectName, DealershipID, CustomerName, 0, 0, 0, 1 from eCollateralItems where DealershipID is not null and CustomerName is not null and ProjectName is not null)
go

alter table [eCollateralItems] add 
	[ProjectID] [int] NULL CONSTRAINT [fkECollateral_Project] FOREIGN KEY REFERENCES [Projects] ([ProjectID]),
	[OldProjectName] [nvarchar](50) NULL,
	[OldEndCustomer] [nvarchar](100) NULL
go
update [eCollateralItems] set
	[OldProjectName] = [ProjectName],
	[OldEndCustomer] = [CustomerName]
go
alter table [eCollateralItems] drop column [ProjectName]
alter table [eCollateralItems] drop column [CustomerName]
go

update [eCollateralItems] set [projectid] = [p].[projectid]
	from [Projects] [p]
where [OldProjectName] = p.ProjectName and
	[oldendcustomer] = p.EndCustomer and
	[DealershipID] = p.DealerID
go