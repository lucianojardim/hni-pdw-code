alter table [ImageFiles] add [ImageApplication] [int] null
go

alter table [ImageFiles] add 
	[FeaturedSeries] [nvarchar](50) null,
	[SeriesList] [ntext] null
go

create table [ImageFileSerieses] (
	[AssociationID] [int] not null identity(1,1),
	[IsFeatured] [bit] not null,
	[ImageID] [int] NOT NULL constraint [FK_ImageFileSerieses_Image] foreign key references [ImageFiles] ([ImageID]),
	[SeriesID] [int] not null constraint [FK_ImageFileSerieses_Series] foreign key references [Serieses] ([SeriesID]),
	CONSTRAINT [pkImageFileSerieses] PRIMARY KEY CLUSTERED ([AssociationID])
) on [primary]
go
