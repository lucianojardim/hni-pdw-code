create table [eCollateralItems] (
	[ItemID] [int] identity(1,1) not null,
	[CreatedByUserID] [int] not null CONSTRAINT fkeCollateral_CreatedUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[CreatedByDateTime] [datetime] not null,
	[LastModifiedByUserID] [int] not null CONSTRAINT fkeCollateral_ModifiedUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[LastModifiedByDateTime] [datetime] not null,
	[IsTemplate] [bit] not null,
	[FileName] [nvarchar](200) not null,
	[URLText] [nvarchar](50) not null,
	[DealershipID] [int] null CONSTRAINT fkeCollateral_Dealership FOREIGN KEY REFERENCES [Companies]([CompanyID]),
	[DealershipPOCID] [int] null CONSTRAINT fkeCollateral_DealerUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[DealershipPOCName] [nvarchar](200) null,
	[DealershipPOCEmail] [nvarchar](200) null,
	[DealershipPOCPhone] [nvarchar](20) null,
	[DealershipPOCAcctType] [int] null,
	[CustomerName] [nvarchar](200) null,
	[ProjectName] [nvarchar](200) null,
	[LayoutID] [int] null,
	[Status] [int] not null,
	CONSTRAINT [pkeCollateralItems] PRIMARY KEY CLUSTERED ([ItemID])	
) on [primary]
go

create table [eCollateralSections] (
	[SectionID] [int] identity(1,1) not null,
	[ItemID] [int] not null CONSTRAINT fkeCSection_Item FOREIGN KEY REFERENCES [eCollateralItems]([ItemID]),
	[ImageID] [int] null CONSTRAINT fkeCSection_Image FOREIGN KEY REFERENCES [ImageFiles]([ImageID]),
	[Content] [ntext] null,
	[Sequence] [int] not null,	
	CONSTRAINT [pkeCollateralSections] PRIMARY KEY CLUSTERED ([SectionID])	
) on [primary]
go

alter table [eCollateralItems] add [ContentType] [int] null
go
update [eCollateralItems] set [ContentType] = 1
go