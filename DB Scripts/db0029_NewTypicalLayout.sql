alter table [SpecRequests] alter column [Material] [nvarchar](100) null
alter table [SpecRequests] alter column [Finish] [nvarchar](100) null
alter table [SpecRequests] alter column [Footprint] [nvarchar](100) null
go
alter table [Typicals] add
	[Notes] [ntext] null,
	[SpecRequestID] [int] null CONSTRAINT [fkTypical_Spec] FOREIGN KEY REFERENCES [SpecRequests] ([RequestID]),
	[FeaturedSeries] [nvarchar](50) null,
	[SeriesList] [ntext] null,
	[AvailableForIn2] [bit] null
go
update [Typicals] set [FeaturedSeries] = (Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 1
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main])
go
update [Typicals] set [SeriesList] = (Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 0
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main])
go
