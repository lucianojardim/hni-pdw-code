using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Series;
using PDWModels.Images;
using PDWModels;

namespace PWDRepositories
{
	public class SeriesRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public SeriesRepository()
		{
		}

		public IEnumerable<SeriesComboItem> GetSeriesNameList( string category )
		{
			return database.Serieses
				.Where( s => s.Category.Name == category || category == null )
				.Select( s => new SeriesComboItem() { SeriesID = s.SeriesID, Name = s.Name } ).Distinct();
		}

		public IEnumerable<SeriesComboItem> GetSeriesNameListWithTypicals()
		{
			return database.Serieses.Where( se => se.SeriesTypicals.Any() ).Select( s => new SeriesComboItem() { SeriesID = s.SeriesID, Name = s.Name } ).Distinct();
		}

		private SeriesSearchResult ToSeriesSearchResult( Series s )
		{
			return
				new SeriesSearchResult()
				{
					SeriesID = s.SeriesID,
					Name = s.Name,
					Category = s.Category.Name,
					ImageData = s.FeaturedImageForSize( "m4to3" ),
					Headline = s.AttributeText( "Marketing Statement Subheadline" )
				};
		}

		private SeriesListData ToSeriesListData( Series s )
		{
			return
				new SeriesListData()
				{
					SeriesID = s.SeriesID,
					Name = s.Name,
					Category = s.Category.Name,
					ImageData = s.FeaturedImageForSize( "m4to3" ),
					DateCreated = s.CreatedDate,
					Ranking = s.Ranking,
					Price = s.StartingPrice,
					Style = s.AttributeSet( "Style" ),
					Applications = s.AttributeSet( "Casegood Application" ).Union( s.AttributeSet( "Table Application" ) ).Union( s.AttributeSet( "Seating Application" ) )
				};
		}

		private SeriesDocListData ToSeriesDocListData( Series s, IEnumerable<string> docList )
		{
			var docData = new SeriesDocListData()
			{
				SeriesID = s.SeriesID,
				Name = s.Name,
				Category = s.Category.Name,
				FeaturedImageFile = s.FeaturedImageForSize( "s16to9" ),
				Documents = new Dictionary<string,string>()
			};

			foreach( var textAttribute in s.SeriesTextAttributes.Where( sta => docList.Contains( sta.Attribute.Name ) ) )
			{
				docData.Documents.Add( textAttribute.Attribute.Name, textAttribute.Value );
			}

			return docData;
		}

		public IEnumerable<SeriesSearchResult> Search( string searchText )
		{
			var termList = SearchText.GetSearchList( searchText );

			List<Series> results = null;

			foreach( var term in termList )
			{
				var theList = database.Serieses
					.Where( s => s.DBKeywords.Contains( term ) )
					.Distinct()
					.ToList();

				if( results == null )
				{
					results = theList;
				}
				else
				{
					results = results.Intersect( theList ).ToList();
				}
			}

			return results
				.Distinct()
				.Select( s => ToSeriesSearchResult( s ) );
		}

		public IEnumerable<SeriesListData> GetSeriesData( string category )
		{
			var theList = database.Serieses.Where( s => s.Category.Name == category || category == null )
				.ToList();

			return theList.Select( s => ToSeriesListData( s ) );
		}

		public IEnumerable<SeriesDocListData> GetSeriesTextData( IEnumerable<string> attList )
		{
			return database.Serieses
				.OrderBy( s => s.Name )
				.ToList()
				.Select( s => ToSeriesDocListData( s, attList ) );
		}

		public SeriesInformation GetSeriesInfo( int? id = null, string seriesName = null )
		{
			SeriesInformation sInfo = new SeriesInformation();

			Series theData = null;
			if( id.HasValue )
				theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			else if( (seriesName ?? "").Any() )
				theData = database.Serieses.FirstOrDefault( s => s.Name == seriesName );

			if( theData != null )
			{
				sInfo.SeriesID = theData.SeriesID;
				sInfo.Name = theData.Name;
				sInfo.Category = theData.Category.Name;
				sInfo.FeaturedImageFile = theData.FeaturedImageForSize( "l16to9" );
				sInfo.Images = theData.ImageListForSize( "m16to9", 3 ).Select( i => (ImageComboItem)i );
				sInfo.Options = new Dictionary<string, IEnumerable<string>>();
				sInfo.Details = new Dictionary<string, IEnumerable<string>>();

				foreach( var attr in theData.SeriesOptionAttributes.Select( soa => soa.Attribute ).Distinct() )
				{
					if( string.Compare( attr.Name, "Pull Options - Style", true ) == 0 )
					{
						sInfo.PullImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.FeaturedPull == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();						
					}
					else if( string.Compare( attr.Name, "Edge Options - Profile", true ) == 0 )
					{
						sInfo.EdgeImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.FeaturedEdge == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();						
					}
					else if( string.Compare( attr.Name, "Finish", true ) == 0 )
					{
						sInfo.FinishImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.FeaturedFinish == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();
					}
					else if( attr.DetailItem )
					{
						sInfo.Details.Add( attr.Name, new List<string>(
						   theData.SeriesOptionAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.AttributeOption.Name )
						   ) );
					}
					else
					{
						sInfo.Options.Add( attr.Name, new List<string>(
						   theData.SeriesOptionAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.AttributeOption.Name )
						   ) );
					}
				}

				foreach( var attr in theData.SeriesIntAttributes.Select( soa => soa.Attribute ).Distinct() )
				{
					if( attr.DetailItem )
					{
						sInfo.Details.Add( attr.Name, new List<string>(
						   theData.SeriesIntAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value.ToString() )
						   ) );
					}
					else
					{
						sInfo.Options.Add( attr.Name, new List<string>(
						   theData.SeriesIntAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value.ToString() )
						   ) );
					}
				}

				foreach( var attr in theData.SeriesTextAttributes.Select( soa => soa.Attribute ).Distinct() )
				{
					if( attr.DetailItem )
					{
						sInfo.Details.Add( attr.Name, new List<string>(
						   theData.SeriesTextAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value )
						   ) );
					}
					else
					{
						sInfo.Options.Add( attr.Name, new List<string>(
						   theData.SeriesTextAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value )
						   ) );
					}
				}

				sInfo.RelatedSeries = theData.ChildSerieses
					.Select( cs => new SeriesInformation.RelatedSeriesInfo()
					{
						SeriesID = cs.SeriesID,
						Name = cs.Name,
						ImageFileData = cs.FeaturedImageForSize( "m4to3" )
					} );

				sInfo.Typicals = theData.SeriesTypicals
					.OrderByDescending( st => st.IsPrimary )
					.Select( t => new SeriesInformation.TypicalInfoForSeries()
					{
						TypicalID = t.TypicalID,
						Name = t.Typical.Name,
						ImageFileData = t.Typical.FeaturedImageForSize( "m4to3" )
					} );
			}

			return sInfo;
		}

		public IEnumerable<ImageComboItem> GetFullSeriesImageList( int id )
		{
			var theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "m4to3" ).Select( i => (ImageComboItem)i );
			}

			return new List<ImageComboItem>();
		}
	}
}
