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

		public TypicalDetailListGallery GetTypicalDetailData( string category, int? seriesId, string fpSize, string keywords,
			int? minPrice, int? maxPrice, bool? in2Only,
			string sortBy, int pageNum, int pageSize )
		{
			TypicalDetailListGallery gallery = new TypicalDetailListGallery();

			var theList = database.Typicals.AsQueryable();
			gallery.TotalListCount = theList.Count();

			if( (keywords ?? "").Any() )
			{
				var termList = SearchText.GetSearchList( keywords );

				Dictionary<int, int> termTypicalList = new Dictionary<int, int>();
				foreach( var term in termList )
				{
					var searchList = database.Typicals
						.Where( s => s.DBKeywords.Contains( term.searchText ) )
						.Select( i => i.TypicalID )
						.ToList();

					searchList.ForEach( delegate( int typId )
					{
						termTypicalList[typId] = (termTypicalList.Keys.Contains( typId ) ? termTypicalList[typId] : 0) | (int)Math.Pow( 2, term.idxWord );
					} );
				}

				int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
				var typicalIdList = termTypicalList.Keys.Where( i => termTypicalList[i] == rightAnswer );

				theList = theList.Where( i => typicalIdList.Contains( i.TypicalID ) );
			}
			if( (category ?? "").Any() )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( st => st.Series.Category.Name == category ) );
			}
			if( seriesId.HasValue )
			{
				theList = theList.Where( s => s.SeriesTypicals.Any( s1 => s1.SeriesID == seriesId.Value ) );
			}
			if( (fpSize ?? "").Any() )
			{
				var userDimList = fpSize.Split( 'x' );
				if( userDimList.Length == 2 )
				{
					int nLength = 0, nWidth = 0;
					if( int.TryParse( userDimList[0], out nLength ) && int.TryParse( userDimList[1], out nWidth ) )
					{
						if( nLength > 0 && nWidth > 0 )
						{
							List<int> fpList = new List<int>();
							var fpAttribute = database.TAttributes.FirstOrDefault( a => a.Name == "Footprint" );
							if( fpAttribute != null )
							{
								foreach( var tOption in fpAttribute.TypicalOptionAttributes )
								{
									var dimList = tOption.TAttributeOption.Name.Split( 'x' );
									if( dimList.Count() == 2 )
									{
										int nDim1 = 0, nDim2 = 0;
										if( int.TryParse( dimList[0], out nDim1 ) && int.TryParse( dimList[1], out nDim2 ) )
										{
											if( ((nLength >= nDim1) && (nWidth >= nDim2)) ||
												((nLength >= nDim2) && (nWidth >= nDim1)) )
											{
												fpList.Add( tOption.OptionID );
											}
										}
									}
								}
							}
							theList = theList.Where( s => fpList.Contains( s.TypicalOptionAttributes.FirstOrDefault( a => a.TAttribute.Name == "Footprint" ).OptionID ) );
						}
					}
				}
			}
			if( minPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value >= minPrice.Value );
			}
			if( maxPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value <= maxPrice.Value );
			}
			if( in2Only ?? false )
			{
				theList = theList.Where( t => t.AvailableForIn2 ?? false );
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
					ImageFileData = t.FeaturedImageForSize( "m16to9" ),
					Price = t.IntAttribute( "Pricing" ),
					Footprints = t.AttributeSet( "Footprint" ),
					SeriesList = t.SeriesTypicals.Select( st => st.Series.Name )
				} );

			return gallery;
		}

		public IEnumerable<TypicalListData> Search( string searchText )
		{
			var termList = SearchText.GetSearchList( searchText );

			int theSeriesID = 0;

			Dictionary<int, int> termTypicalList = new Dictionary<int, int>();
			foreach( var term in termList )
			{
				var searchList = database.Typicals
					.Where( s => s.DBKeywords.Contains( term.searchText ) )
					.Select( i => i.TypicalID )
					.ToList();

				searchList.ForEach( delegate( int typId )
				{
					termTypicalList[typId] = (termTypicalList.Keys.Contains( typId ) ? termTypicalList[typId] : 0) | (int)Math.Pow( 2, term.idxWord );
				} );
			}

			int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
			var typicalIdList = termTypicalList.Keys.Where( i => termTypicalList[i] == rightAnswer );

			var typicalList = database.Typicals
				.Where( t => typicalIdList.Contains( t.TypicalID ) )
				.Distinct()
				.ToList();

			foreach( var term in termList )
			{
				var theSeries = typicalList.SelectMany( t => t.SeriesTypicals ).FirstOrDefault( st => st.Series.Name.IndexOf( term.searchText.TrimStart( ' ' ), 0, StringComparison.OrdinalIgnoreCase ) >= 0 );
				if( (theSeries != null) && (theSeriesID == 0) )
				{
					theSeriesID = theSeries.SeriesID;
					break;
				}
				theSeries = typicalList.SelectMany( t => t.SeriesTypicals ).FirstOrDefault( st => st.Series.Name.IndexOf( term.searchText.Trim(), 0, StringComparison.OrdinalIgnoreCase ) >= 0 );
				if( (theSeries != null) && (theSeriesID == 0) )
				{
					theSeriesID = theSeries.SeriesID;
					break;
				}
			}

			var returnList = typicalList
				.Select( t => ToTypicalListData( t ) )
				.ToList();

			returnList.ForEach( t => t.SeriesID = theSeriesID );

			return returnList;
		}

		public TypicalListGallery GetTypicalData( string category, int? seriesId, string fpSize, string keywords,
			int? minPrice, int? maxPrice, bool? in2Only,
			string sortBy, int pageNum, int pageSize )
		{
			TypicalListGallery gallery = new TypicalListGallery();

			var theList = database.Typicals.AsQueryable();
			gallery.TotalListCount = theList.Count();

			if( (keywords ?? "").Any() )
			{
				var termList = SearchText.GetSearchList( keywords );

				Dictionary<int, int> termTypicalList = new Dictionary<int, int>();
				foreach( var term in termList )
				{
					var searchList = database.Typicals
						.Where( s => s.DBKeywords.Contains( term.searchText ) )
						.Select( i => i.TypicalID )
						.ToList();

					searchList.ForEach( delegate( int typId )
					{
						termTypicalList[typId] = (termTypicalList.Keys.Contains( typId ) ? termTypicalList[typId] : 0) | (int)Math.Pow( 2, term.idxWord );
					} );
				}

				int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
				var typicalIdList = termTypicalList.Keys.Where( i => termTypicalList[i] == rightAnswer );

				theList = theList.Where( i => typicalIdList.Contains( i.TypicalID ) );
			}
			if( (category ?? "").Any() )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( st => st.Series.Category.Name == category ) );
			}
			if( seriesId.HasValue )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( s1 => s1.SeriesID == seriesId.Value ) );
			}
			if( (fpSize ?? "").Any() )
			{
				var userDimList = fpSize.Split( 'x' );
				if( userDimList.Length == 2 )
				{
					int nLength = 0, nWidth = 0;
					if( int.TryParse( userDimList[0], out nLength ) && int.TryParse( userDimList[1], out nWidth ) )
					{
						if( nLength > 0 && nWidth > 0 )
						{
							List<int> fpList = new List<int>();
							var fpAttribute = database.TAttributes.FirstOrDefault( a => a.Name == "Footprint" );
							if( fpAttribute != null )
							{
								foreach( var tOption in fpAttribute.TypicalOptionAttributes )
								{
									var dimList = tOption.TAttributeOption.Name.Split( 'x' );
									if( dimList.Count() == 2 )
									{
										int nDim1 = 0, nDim2 = 0;
										if( int.TryParse( dimList[0], out nDim1 ) && int.TryParse( dimList[1], out nDim2 ) )
										{
											if( ((nLength >= nDim1) && (nWidth >= nDim2)) ||
												((nLength >= nDim2) && (nWidth >= nDim1)) )
											{
												fpList.Add( tOption.OptionID );
											}
										}
									}
								}
							}
							theList = theList.Where( s => fpList.Contains( s.TypicalOptionAttributes.FirstOrDefault( a => a.TAttribute.Name == "Footprint" ).OptionID ) );
						}
					}
				}
			}
			if( minPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value >= minPrice.Value );
			}
			if( maxPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value <= maxPrice.Value );
			}
			if( in2Only ?? false )
			{
				theList = theList.Where( t => t.AvailableForIn2 ?? false );
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
					ImageFileData = t.FeaturedImageForSize( "m16to9" )
				};
		}

		public TypicalListGallery GetTypicalCoverData( string category, int? seriesId, string fpSize, string keywords,
			int? minPrice, int? maxPrice, bool? in2Only,
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

			if( (keywords ?? "").Any() )
			{
				var termList = SearchText.GetSearchList( keywords );

				Dictionary<int, int> termTypicalList = new Dictionary<int, int>();
				foreach( var term in termList )
				{
					var searchList = database.Typicals
						.Where( s => s.DBKeywords.Contains( term.searchText ) )
						.Select( i => i.TypicalID )
						.ToList();

					searchList.ForEach( delegate( int typId )
					{
						termTypicalList[typId] = (termTypicalList.Keys.Contains( typId ) ? termTypicalList[typId] : 0) | (int)Math.Pow( 2, term.idxWord );
					} );
				}

				int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
				var typicalIdList = termTypicalList.Keys.Where( i => termTypicalList[i] == rightAnswer );

				theList = theList.Where( i => typicalIdList.Contains( i.TypicalID ) );
			}
			if( (category ?? "").Any() )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( st => st.Series.Category.Name == category ) );
			}
			if( seriesId.HasValue )
			{
				theList = theList.Where( t => t.SeriesTypicals.Any( s1 => s1.SeriesID == seriesId.Value ) );
			}
			if( (fpSize ?? "").Any() )
			{
				var userDimList = fpSize.Split( 'x' );
				if( userDimList.Length == 2 )
				{
					int nLength = 0, nWidth = 0;
					if( int.TryParse( userDimList[0], out nLength ) && int.TryParse( userDimList[1], out nWidth ) )
					{
						if( nLength > 0 && nWidth > 0 )
						{
							List<int> fpList = new List<int>();
							var fpAttribute = database.TAttributes.FirstOrDefault( a => a.Name == "Footprint" );
							if( fpAttribute != null )
							{
								foreach( var tOption in fpAttribute.TypicalOptionAttributes )
								{
									var dimList = tOption.TAttributeOption.Name.Split( 'x' );
									if( dimList.Count() == 2 )
									{
										int nDim1 = 0, nDim2 = 0;
										if( int.TryParse( dimList[0], out nDim1 ) && int.TryParse( dimList[1], out nDim2 ) )
										{
											if( ((nLength >= nDim1) && (nWidth >= nDim2)) ||
												((nLength >= nDim2) && (nWidth >= nDim1)) )
											{
												fpList.Add( tOption.OptionID );
											}
										}
									}
								}
							}
							theList = theList.Where( s => fpList.Contains( s.TypicalOptionAttributes.FirstOrDefault( a => a.TAttribute.Name == "Footprint" ).OptionID ) );
						}
					}
				}
			}
			if( minPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value >= minPrice.Value );
			}
			if( maxPrice.HasValue )
			{
				theList = theList.Where( s => s.TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == "Pricing" ).Value <= maxPrice.Value );
			}
			if( in2Only ?? false )
			{
				theList = theList.Where( t => t.AvailableForIn2 ?? false );
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
				var serTypical = theData.SeriesTypicals.FirstOrDefault( st => st.IsPrimary );
				if( serTypical != null )
				{
					tInfo.Category = serTypical.Series.Category.Name;
					tInfo.Series = serTypical.Series.Name;
				}
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
				return theData.ImageListForSize( "m16to9" ).Select( i => (ImageComboItem)i );
			}

			return new List<ImageComboItem>();
		}
	}
}
