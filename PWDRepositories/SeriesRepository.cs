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

		public IEnumerable<SeriesComboItem> GetSeriesNameList()
		{
			return database.Serieses.Select( s => new SeriesComboItem() { SeriesID = s.SeriesID, Name = s.Name } ).Distinct();
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

		public IEnumerable<SeriesSearchResult> Search( string searchText )
		{
			var termList = SearchText.GetSearchList( searchText );

			List<Series> results = null;

			foreach( var term in termList )
			{
				var theList = database.Serieses.Where( 
						s => s.Name.Contains( term ) ||
						s.Category.Name.Contains( term ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Style" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesTextAttributes.Any( sta => sta.Attribute.Name == "Value Statement Headline" && sta.Value.Contains( term ) ) ||
						s.SeriesTextAttributes.Any( sta => sta.Attribute.Name == "Marketing Statement Subheadline" && sta.Value.Contains( term ) ) ||
//						s.SeriesTextAttributes.Any( sta => sta.Attribute.Name == "Marketing Description" && sta.Value.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Claire 5 Favorites" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "IAQ" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Finish" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Drawer Options - Configuration" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Edge Options - Profile" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Pull Options - Style" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Seating Option - Back" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Shape" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Casegood Application" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Table Application" && soa.AttributeOption.Name.Contains( term ) ) ||
						s.SeriesOptionAttributes.Any( soa => soa.Attribute.Name == "Seating Application" && soa.AttributeOption.Name.Contains( term ) ) )
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
							.Join( database.ImageFiles,
								soa => soa.AttributeOption.Name,
								img => img.FeaturedPull,
								( s, i ) => new ImageSummary()
								{
									Name = s.AttributeOption.Name,
									FileName = i.ThumbnailImageData( "s16to9" ).FileName,
									ImageID = i.ImageID,
									CanLightbox = ImageFile.ImageCanLightbox( i.ImageType )
								} )
							.ToList();						
					}
					else if( string.Compare( attr.Name, "Edge Options - Profile", true ) == 0 )
					{
						sInfo.EdgeImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Join( database.ImageFiles,
								soa => soa.AttributeOption.Name,
								img => img.FeaturedEdge,
								( s, i ) => new ImageSummary()
								{
									Name = s.AttributeOption.Name,
									FileName = i.ThumbnailImageData( "s16to9" ).FileName,
									ImageID = i.ImageID,
									CanLightbox = ImageFile.ImageCanLightbox( i.ImageType )
								} )
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
