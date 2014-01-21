declare @repGroupID int, @dealerID int, @dsrID int, @specTeamID int
select @repGroupID = CompanyID from Companies where Name = 'Paoli - Marketing Team' and CompanyType = 2
select @dealerID = CompanyID from Companies where Name = 'Paoli - Marketing Team' and CompanyType = 3

INSERT INTO [SpecRequests] ([Name],[ProjectName],[PaoliSalesRepGroupID],[PrimaryCompanyID],[DealerSalesRepID],[PaoliSpecTeamMemberID]
      ,[RequestDate],[IsGSA],[SavedLocation],
      [ListPrice],
      [FeaturedSeries],
      [SeriesList],
      [Received],[SPLQuote],[LastModifiedDate],
      [Material],
      [Finish],
      [IsGoodForWeb],[AvailableForIn2],
      [Footprint],
      [Notes])
(select Name, null, @repGroupID, @dealerID, @dsrID, @specTeamID,
	CreatedDate, 0, null, 
	(select Value from TypicalIntAttributes tia inner join TAttributes ta on tia.AttributeID = ta.AttributeID and ta.Name = 'pricing' where tia.TypicalID = Typicals.TypicalID),
	(Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 1
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main]),
	(Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 0
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main]),
	0, 0, CreatedDate,
	(Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select topt.Name + ',' AS [text()]
					From TypicalOptionAttributes toa
						inner join TAttributes ta on toa.AttributeID = ta.AttributeID and ta.Name = 'Material'
						inner join TAttributeOptions topt on toa.OptionID = topt.OptionID
					Where toa.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main]),
	(Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select topt.Name + ',' AS [text()]
					From TypicalOptionAttributes toa
						inner join TAttributes ta on toa.AttributeID = ta.AttributeID and ta.Name = 'Finish'
						inner join TAttributeOptions topt on toa.OptionID = topt.OptionID
					Where toa.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main]),
	0, 0, 
	(Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select topt.Name + ',' AS [text()]
					From TypicalOptionAttributes toa
						inner join TAttributes ta on toa.AttributeID = ta.AttributeID and ta.Name = 'Footprint'
						inner join TAttributeOptions topt on toa.OptionID = topt.OptionID
					Where toa.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main]),
	null
from Typicals where typicals.SpecRequestID is null)
go

Update Typicals set SpecRequestID = sr.RequestID
	from Typicals t
		inner join SpecRequests sr on t.Name = sr.Name
where SpecRequestID is null
go