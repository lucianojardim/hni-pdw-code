using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Attributes;

namespace PWDRepositories
{
	public class AttributeRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public AttributeRepository()
		{
		}

		public IEnumerable<string> GetOptionList( string attr )
		{
			var attrData = database.Attributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.AttributeOptions.Select( ao => ao.Name );
			}

			return new List<string>();
		}

		public IEnumerable<AttributeComboItem> GetTypicalOptionList( string attr )
		{
			var attrData = database.TAttributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.TAttributeOptions.Select( ao => new AttributeComboItem() { ID = ao.OptionID, Name = ao.Name } );
			}

			return new List<AttributeComboItem>();
		}

		public IEnumerable<int> GetTypicalPriceRange()
		{
			var prices = new List<int>();

			var attr = database.TAttributes.FirstOrDefault( a => a.Name == "Pricing" );
			prices.Add( attr.TypicalIntAttributes.Min( a => a.Value ) );
			prices.Add( attr.TypicalIntAttributes.Max( a => a.Value ) );

			return prices;
		}

		public IEnumerable<AttributeOptionInformation> GetSeriesListForAttribute( string attr )
		{
			var aInfo = database.Attributes
				.FirstOrDefault( a => a.Name == attr );

			if( aInfo == null )
			{
				return new List<AttributeOptionInformation>();
			}

			var aList = aInfo.AttributeOptions
				.Select( ao => new AttributeOptionInformation()
					{
						Name = ao.Name,
						Serieses = ao.SeriesOptionAttributes.Select( soa => soa.Series.Name )
					} )
				.OrderBy( a => a.Name )
				.ToList();

			switch( attr.ToLower() )
			{
			case "pull options - style":
				aList.ForEach( aoi => 
					{ 
						var img = database.ImageFiles.FirstOrDefault( i => i.FeaturedPull == aoi.Name ); 
						if( img != null ) 
						{
							aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
						} 
					} );
				break;
			case "edge options - profile":
				aList.ForEach( aoi =>
					{
						var img = database.ImageFiles.FirstOrDefault( i => i.FeaturedEdge == aoi.Name );
						if( img != null )
						{
							aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
						}
					} );
				break;
			}

			return aList;
		}
	}
}
