create table [RelatedSeries] (
	[ParentSeriesID] [int] not null CONSTRAINT fkRelatedSeries_Parent FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	[ChildSeriesID] [int] not null CONSTRAINT fkRelatedSeries_Child FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkRelatedSeries] PRIMARY KEY CLUSTERED ([ParentSeriesID], [ChildSeriesID])

) on [primary]
go