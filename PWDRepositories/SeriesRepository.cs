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

		public IEnumerable<string> GetJustSeriesNameList()
		{
			return database.Serieses
				.Select( s => s.Name ).Distinct();
		}

		public IEnumerable<SeriesComboItem> GetSeriesNameListWithTypicals()
		{
			return database.Serieses
				.Where( se => se.SeriesTypicals.Any() )
				.Select( s => new SeriesComboItem() { SeriesID = s.SeriesID, Name = s.Name } ).Distinct();
		}

		private SeriesSearchResult ToSeriesSearchResult( Series s )
		{
			return
				new SeriesSearchResult()
				{
					SeriesID = s.SeriesID,
					Name = s.Name,
					Category = s.Category.Name,
					ImageData = s.FeaturedImageForSize( "m16to9" ),
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
					ImageData = s.FeaturedImageForSize( "m16to9" ),
					DateCreated = s.CreatedDate,
					Ranking = s.Ranking,
					Price = s.StartingPrice,
					Style = s.AttributeSet( "Style" ),
					Applications = s.AttributeSet( "Casegood Application" ).Union( s.AttributeSet( "Table Application" ) ).Union( s.AttributeSet( "Seating Application" ) ),
					IsInTwo = s.AttributeSet( "inTWO" ).Select( i => i.ToLower() ).Contains( "yes" )
				};
		}

		private SeriesDocListData ToSeriesDocListData( Series s, IEnumerable<string> docList, IEnumerable<string> detailList )
		{
			var docData = new SeriesDocListData()
			{
				SeriesID = s.SeriesID,
				Name = s.Name,
				Category = s.Category.Name,
				FeaturedImageFile = s.FeaturedImageForSize( "s16to9" ),
				Documents = new Dictionary<string,string>(),
				Details = new Dictionary<string,string>()
			};

			foreach( var textAttribute in s.SeriesTextAttributes.Where( sta => docList.Contains( sta.Attribute.Name ) ) )
			{
				docData.Documents.Add( textAttribute.Attribute.Name, textAttribute.Value );
			}
			foreach( var detailItem in detailList )
			{
				string val = string.Join( ", ", s.SeriesOptionAttributes.Where( soa => soa.Attribute.Name == detailItem ).Select( soa => soa.AttributeOption.Name ) );
				if( val.Any() )
				{
					docData.Details.Add( detailItem, val );
				}
			}

			return docData;
		}

		public IEnumerable<SeriesSearchResult> Search( string searchText )
		{
			var termList = SearchText.GetSearchList( searchText );

			Dictionary<int, int> termSeriesList = new Dictionary<int, int>();
			foreach( var term in termList )
			{
				var theList = database.Serieses
					.Where( s => s.DBKeywords.Contains( term.searchText ) )
					.Select( i => i.SeriesID )
					.ToList();

				theList.ForEach( delegate( int seriesId )
				{
					termSeriesList[seriesId] = (termSeriesList.Keys.Contains( seriesId ) ? termSeriesList[seriesId] : 0) | (int)Math.Pow( 2, term.idxWord );
				} );
			}

			int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
			var seriesIdList = termSeriesList.Keys.Where( i => termSeriesList[i] == rightAnswer );

			return database.Serieses
				.Where( s => seriesIdList.Contains( s.SeriesID ) )
				.Distinct()
				.ToList()
				.Select( s => ToSeriesSearchResult( s ) );
		}

		public IEnumerable<SeriesListData> GetSeriesData( string category )
		{
			var theList = database.Serieses
				.Where( s => s.Category.Name == category || category == null )
				.Where( s => s.IsActive )
				.ToList();

			return theList.Select( s => ToSeriesListData( s ) );
		}

		public IEnumerable<SeriesDocListData> GetSeriesTextData( IEnumerable<string> attList, IEnumerable<string> detailList )
		{
			return database.Serieses
				.OrderBy( s => s.Name )
				.ToList()
				.Select( s => ToSeriesDocListData( s, attList, detailList ) );
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
							.OrderBy( soa => soa.ValueID )
							.Take( 5 )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.FeaturedFinish == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();
					}
					else if( string.Compare( attr.Name, "Control Mechanism", true ) == 0 )
					{
						sInfo.ControlMechanisms = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.ControlMechanism == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();
					}
					else if( string.Compare( attr.Name, "Base Options", true ) == 0 )
					{
						sInfo.TableBaseImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.TableBase == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();
					}
					else if( string.Compare( attr.Name, "Shape", true ) == 0 )
					{
						sInfo.TableShapeImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.TableShape == soa.AttributeOption.Name ) } )
							.Select( soa => new ImageForObject()
							{
								Name = soa.Name,
								FeaturedImage = soa.Img != null ? soa.Img.ThumbnailImageData( "s16to9" ) : null
							} )
							.OrderBy( i => i.Name )
							.ToList();
					}
					else if( string.Compare( attr.Name, "Desk Layouts", true ) == 0 )
					{
						sInfo.DeskLayoutImages = theData.SeriesOptionAttributes
							.Where( soa => soa.AttributeID == attr.AttributeID )
							.Select( soa => new { Name = soa.AttributeOption.Name, Img = database.ImageFiles.FirstOrDefault( i => i.Name == soa.AttributeOption.Name ) } )
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
					.Where( s => s.IsActive )
					.Select( cs => new SeriesInformation.RelatedSeriesInfo()
					{
						SeriesID = cs.SeriesID,
						Name = cs.Name,
						ImageFileData = cs.FeaturedImageForSize( "m16to9" )
					} );

				sInfo.Typicals = theData.SeriesTypicals
					.Where( t => t.Typical.IsPublished )
					.OrderByDescending( st => st.IsPrimary )
					.Select( t => new SeriesInformation.TypicalInfoForSeries()
					{
						TypicalID = t.TypicalID,
						Name = t.Typical.Name,
						ImageFileData = t.Typical.FeaturedImageForSize( "m16to9" )
					} );
			}

			return sInfo;
		}

		public IEnumerable<ImageComboItem> GetFullSeriesImageList( int id )
		{
			var theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "m16to9" ).Select( i => (ImageComboItem)i );
			}

			return new List<ImageComboItem>();
		}

		public IDictionary<string, string> GetFeatureImageList()
		{
			return database.Serieses
				.Where( s => s.SeriesImageFiles.Any( sif => sif.IsFeatured ) )
				.ToDictionary( s => s.Name, s => s.FeaturedImageForSize( "s16to9" ).FileName );
		}
	}
}
