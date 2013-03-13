using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Typicals;
using PDWModels.Images;
using PDWModels;

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
					ImageFileData = t.FeaturedImageForSize( "m4to3" ),
					Price = t.IntAttribute( "Pricing" ),
					Footprints = t.AttributeSet( "Footprint" ),
					SeriesList = t.SeriesTypicals.Select( st => st.Series.Name )
				} );

			return gallery;
		}

		public IEnumerable<TypicalListData> Search( string searchText )
		{
			var termList = SearchText.GetSearchList( searchText );

			List<Typical> results = null;
			int theSeriesID = 0;

			foreach( var term in termList )
			{
				var theList = database.Typicals
					.Where( t => t.DBKeywords.Contains( term ) )
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

				var theSeries = results.SelectMany( t => t.SeriesTypicals ).FirstOrDefault( st => st.Series.Name.IndexOf( term, 0, StringComparison.OrdinalIgnoreCase ) >= 0 );
				if( (theSeries != null) && (theSeriesID == 0 ) )
				{
					theSeriesID = theSeries.SeriesID;
				}
			}

			var returnList = results
				.Distinct()
				.Select( t => ToTypicalListData( t ) )
				.ToList();

			returnList.ForEach( t => t.SeriesID = theSeriesID );

			return returnList;
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
				.Select( t => ToTypicalListData( t ) );

			return gallery;
		}

		private TypicalListData ToTypicalListData( Typical t )
		{
			return new TypicalListData()
				{
					TypicalID = t.TypicalID,
					Name = t.Name,
					SeriesID = t.SeriesTypicals.Where( st => st.IsPrimary ).Select( st => st.SeriesID ).FirstOrDefault(),
					ImageFileData = t.FeaturedImageForSize( "m4to3" )
				};
		}

		public TypicalListGallery GetTypicalCoverData( string category, int? seriesId, string footprints,
			int? minPrice, int? maxPrice,
			string sortBy, int? typicalId, string itemList )
		{
			if( !typicalId.HasValue )
			{
				return new TypicalListGallery()
					{
						TotalListCount = 0,
						FilteredListCount = 0,
						Typicals = new List<TypicalListData>()
					};
			}

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

			var idList = orderedList.Select( t => t.TypicalID ).ToList();

			if( !idList.Contains( typicalId.Value ) || (gallery.FilteredListCount == 0) )
			{
				gallery.Typicals = new List<TypicalListData>();
				return gallery;
			}

			var getMeList = itemList.Split( ',' ).Select( int.Parse );
			var typIndex = idList.IndexOf( typicalId.Value );
			getMeList = getMeList.Select( i => ((i + typIndex) % gallery.FilteredListCount + gallery.FilteredListCount) % gallery.FilteredListCount );

			var goodIdList = getMeList.Select( i => idList[i] ).ToList();

			if( (getMeList.Count() > 1) && (idList.Count() < getMeList.Count()) )
			{
				// need multiple items, but original list had less items than requested
				// just use original list
				goodIdList = idList;
			}

			gallery.Typicals = goodIdList
				.Select( i => database.Typicals.FirstOrDefault( typ => typ.TypicalID == i ) )
				.Select( t => ToTypicalListData( t ) );

			return gallery;
		}

		public TypicalInformation GetTypicalInfo( int? id = null, string typicalName = null )
		{
			TypicalInformation tInfo = new TypicalInformation();

			Typical theData = null;
			if( id.HasValue )
				theData = database.Typicals.FirstOrDefault( s => s.TypicalID == id );
			else if( (typicalName ?? "").Any() )
				theData = database.Typicals.FirstOrDefault( s => s.Name == typicalName );

			if( theData != null )
			{
				tInfo.TypicalID = theData.TypicalID;
				tInfo.Name = theData.Name;
				tInfo.Category = theData.SeriesTypicals.FirstOrDefault( st => st.IsPrimary ).Series.Category.Name;
				tInfo.Series = theData.SeriesTypicals.FirstOrDefault( st => st.IsPrimary ).Series.Name;
				tInfo.FeaturedImageFileData = theData.FeaturedImageForSize( "l16to9" );
				tInfo.Images = theData.ImageListForSize( "m16to9", 3 ).Select( i => (ImageComboItem)i );
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

		public IEnumerable<ImageComboItem> GetFullTypicalImageList( int id )
		{
			var theData = database.Typicals.FirstOrDefault( s => s.TypicalID == id );
			if( theData != null )
			{
				return theData.ImageListForSize( "m4to3" ).Select( i => (ImageComboItem)i );
			}

			return new List<ImageComboItem>();
		}
	}
}
