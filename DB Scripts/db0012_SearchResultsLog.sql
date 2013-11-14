create table [SearchResultsLog] (
	[LogID] [int] not null identity(1,1),
	[SearchDateTime] [datetime] not null,
	[SeriesCount] [int] not null,
	[ImageCount] [int] not null,
	[TypicalCount] [int] not null,
	[PageCount] [int] not null,
	[SearchTerm] [ntext] not null,
	CONSTRAINT [pkSearchResultsLog] PRIMARY KEY CLUSTERED ([LogID])		
) on [primary]
go