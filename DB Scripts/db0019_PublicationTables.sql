create table [Publications] (
	[PublicationID] [int] identity(1,1) not null,
	[Name] [nvarchar](100) not null,
	[PubDate] [datetime] not null,
	[FilterVisible] [bit] not null,
	CONSTRAINT [pkPublications] PRIMARY KEY CLUSTERED ([PublicationID])	
) on [Primary]
go

create table [PublicationImages] (
	[PublicationID] [int] not null CONSTRAINT fkPI_Publication FOREIGN KEY REFERENCES [Publications]([PublicationID]),
	[ImageID] [int] not null CONSTRAINT fkPI_Image FOREIGN KEY REFERENCES [ImageFiles]([ImageID]),
	[PageNumber] [int] null,
	CONSTRAINT [pkPublicationImages] PRIMARY KEY CLUSTERED ([PublicationID], [ImageID])	
) on [primary]
go
