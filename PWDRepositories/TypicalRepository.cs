using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Typicals;

namespace PWDRepositories
{
	public class TypicalRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public TypicalRepository()
		{
		}

		public TypicalDetailListGallery GetTypicalDetailData( string category, int? seriesId, string footprints,
			int? minPrice, int? maxPrice,
			string sortBy, int pageNum, int pageSize )
		{
			TypicalDetailListGallery gallery = new TypicalDetailListGallery();

			var theList = database.Typicals.AsQueryable();
			gallery.TotalListCount = theList.Count();

			if( (category ?? "").Any() )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( st => st.Series.Category.Name == category ) );
			}
			if( seriesId.HasValue )
			{
				theList = theList.Where( s => s.SeriesTypicals.Any( s1 => s1.SeriesID == seriesId.Value ) );
			}
			if( (footprints ?? "").Any() )
			{
				var fpList = footprints.Split( ',' ).Select( int.Parse );
				theList = theList.Where( s => fpList.Contains( s.TypicalOptionAttributes.FirstOrDefault( a => a.TAttribute.Name == "Footprint" ).OptionID ) );
			}
			if( minPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value >= minPrice.Value );
			}
			if( maxPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value <= maxPrice.Value );
			}

			gallery.FilteredListCount = theList.Count();

			var orderedList = theList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.SeriesTypicals.Max( sif => sif.Series.SeriesIntAttributes.FirstOrDefault( a => a.Attribute.Name == "Ranking" ).Value ) );
					break;
				case "mostrecent":
					orderedList = orderedList
						.OrderByDescending( i => i.CreatedDate );
					break;
			}
			gallery.Typicals = orderedList
				.Skip( (pageNum - 1) * pageSize )
				.Take( pageSize )
				.ToList()
				.Select( t => new TypicalDetailListData()
				{
					TypicalID = t.TypicalID,
					Name = t.Name,
					ImageFileName = t.FeaturedImageForSize( "m4to3" ),
					Price = t.IntAttribute( "Pricing" ),
					Footprints = t.AttributeSet( "Footprint" )
				} );

			return gallery;
		}

		public TypicalListGallery GetTypicalData( string category, int? seriesId, string footprints, 
			int? minPrice, int? maxPrice, 
			string sortBy, int pageNum, int pageSize )
		{
			TypicalListGallery gallery = new TypicalListGallery();

			var theList = database.Typicals.AsQueryable();
			gallery.TotalListCount = theList.Count();
			
			if( (category ?? "").Any() )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( st => st.Series.Category.Name == category ) );
			}
			if( seriesId.HasValue )
			{
				theList = theList.Where( s => s.SeriesTypicals.Any( s1 => s1.SeriesID == seriesId.Value ) );
			}
			if( (footprints ?? "").Any() )
			{
				var fpList = footprints.Split( ',' ).Select( int.Parse );
				theList = theList.Where( s => fpList.Contains( s.TypicalOptionAttributes.FirstOrDefault( a => a.TAttribute.Name == "Footprint" ).OptionID ) );
			}
			if( minPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value >= minPrice.Value );
			}
			if( maxPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value <= maxPrice.Value );
			}	

			gallery.FilteredListCount = theList.Count();

			var orderedList = theList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.SeriesTypicals.Max( sif => sif.Series.SeriesIntAttributes.FirstOrDefault( a => a.Attribute.Name == "Ranking" ).Value ) );
					break;
				case "mostrecent":
					orderedList = orderedList
						.OrderByDescending( i => i.CreatedDate );
					break;
			}
			gallery.Typicals = orderedList
				.Skip( (pageNum - 1) * pageSize )
				.Take( pageSize )
				.ToList()
				.Select( t => new TypicalListData()
				{
					TypicalID = t.TypicalID,
					Name = t.Name,
					ImageFileName = t.FeaturedImageForSize( "m4to3" )
				} );

			return gallery;
		}

		public TypicalInformation GetTypicalInfo( int id )
		{
			TypicalInformation tInfo = new TypicalInformation();

			var theData = database.Typicals.FirstOrDefault( s => s.TypicalID == id );
			if( theData != null )
			{
				tInfo.TypicalID = theData.TypicalID;
				tInfo.Name = theData.Name;
				tInfo.Category = theData.SeriesTypicals.FirstOrDefault( st => st.IsPrimary ).Series.Category.Name;
				tInfo.Series = theData.SeriesTypicals.FirstOrDefault( st => st.IsPrimary ).Series.Name;
				tInfo.FeaturedImageFileName = theData.FeaturedImageForSize( "l16to9" );
				tInfo.Images = theData.ImageListForSize( "m16to9", 3 );
				tInfo.Options = new Dictionary<string, IEnumerable<string>>();
				tInfo.Details = new Dictionary<string, IEnumerable<string>>();

				foreach( var attr in theData.TypicalOptionAttributes.Select( soa => soa.TAttribute ).Distinct() )
				{
					if( attr.DetailItem )
					{
						tInfo.Details.Add( attr.Name, new List<string>(
						   theData.TypicalOptionAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.TAttributeOption.Name )
						   ) );
					}
					else
					{
						tInfo.Options.Add( attr.Name, new List<string>(
						   theData.TypicalOptionAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.TAttributeOption.Name )
						   ) );
					}
				}

				foreach( var attr in theData.TypicalIntAttributes.Select( soa => soa.TAttribute ).Distinct() )
				{
					if( attr.DetailItem )
					{
						tInfo.Details.Add( attr.Name, new List<string>(
						   theData.TypicalIntAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value.ToString() )
						   ) );
					}
					else
					{
						tInfo.Options.Add( attr.Name, new List<string>(
						   theData.TypicalIntAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value.ToString() )
						   ) );
					}
				}

				foreach( var attr in theData.TypicalTextAttributes.Select( soa => soa.TAttribute ).Distinct() )
				{
					if( attr.DetailItem )
					{
						tInfo.Details.Add( attr.Name, new List<string>(
						   theData.TypicalTextAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value )
						   ) );
					}
					else
					{
						tInfo.Options.Add( attr.Name, new List<string>(
						   theData.TypicalTextAttributes
							   .Where( soa => soa.AttributeID == attr.AttributeID )
							   .Select( so => so.Value )
						   ) );
					}
				}
			}

			return tInfo;
		}

		public IEnumerable<string> GetFullTypicalImageList( int id )
		{
			var theData = database.Typicals.FirstOrDefault( s => s.TypicalID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "m4to3" );
			}

			return new List<string>();
		}
	}
}
