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

		public class SeriesListData
		{
			public int SeriesID { get; set; }
			public string Name { get; set; }
			public string Category { get; set; }
			public string ImageFileName { get; set; }
			public DateTime DateCreated { get; set; }
			public int Ranking { get; set; }
			public IEnumerable<string> Style { get; set; }
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
					ImageFileName = s.FeaturedImageForSize( "s1to1" ), 
					DateCreated = s.CreatedDate,
 					Ranking = s.Ranking,
					Style = s.AttributeSet( "Style" )
				} 
			);
		}

		public SeriesInformation GetSeriesInfo( int id )
		{
			SeriesInformation sInfo = new SeriesInformation();

			var theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			if( theData != null )
			{
				sInfo.SeriesID = theData.SeriesID;
				sInfo.Name = theData.Name;
				sInfo.Category = theData.Category.Name;
				sInfo.FeaturedImageFileName = theData.FeaturedImageFileName;
				sInfo.Images = theData.ImageListForSize( "s1to1", 3 );
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
						ImageFileName = cs.FeaturedImageForSize( "s1to1" )
					} );
			}

			return sInfo;
		}

		public IEnumerable<string> GetFullSeriesImageList( int id )
		{
			var theData = database.Serieses.FirstOrDefault( s => s.SeriesID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "s1to1" );
			}

			return new List<string>();
		}
	}
}
