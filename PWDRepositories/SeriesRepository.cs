using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Series;

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

		public IEnumerable<SeriesListData> GetSeriesData( string category )
		{
			var theList = database.Serieses.Where( s => s.Category.Name == category || category == null )
				.ToList();

			return theList.Select( s => 
				new SeriesListData() 
				{ 
					SeriesID = s.SeriesID, 
					Name = s.Name, 
					Category = s.Category.Name, 
					ImageFileName = s.FeaturedImageForSize( "m4to3" ), 
					DateCreated = s.CreatedDate,
 					Ranking = s.Ranking,
					Style = s.AttributeSet( "Style" )
				} 
			);
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
				sInfo.FeaturedImageFileName = theData.FeaturedImageForSize( "l16to9" );
				sInfo.Images = theData.ImageListForSize( "m16to9", 3 );
				sInfo.Options = new Dictionary<string, IEnumerable<string>>();
				sInfo.Details = new Dictionary<string, IEnumerable<string>>();

				foreach( var attr in theData.SeriesOptionAttributes.Select( soa => soa.Attribute ).Distinct() )
				{
					if( attr.DetailItem )
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
						ImageFileName = cs.FeaturedImageForSize( "m4to3" )
					} );

				sInfo.Typicals = theData.SeriesTypicals
					.OrderByDescending( st => st.IsPrimary )
					.Select( t => new SeriesInformation.TypicalInfoForSeries()
					{
						TypicalID = t.TypicalID,
						Name = t.Typical.Name,
						ImageFileName = t.Typical.FeaturedImageForSize( "m4to3" )
					} );
			}

			return sInfo;
		}

		public IEnumerable<string> GetFullSeriesImageList( int id )
		{
			var theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "m4to3" );
			}

			return new List<string>();
		}
	}
}
