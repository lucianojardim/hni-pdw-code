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

		public IEnumerable<string> GetTypicalOptionList( string attr, string query )
		{
			var attrData = database.TAttributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.TAttributeOptions
					.Where( ao => ao.Name.Contains( query ) )
					.OrderBy( ao => ao.Name )
					.Select( ao => ao.Name );
			}

			return new List<string>();
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
						Serieses = ao.SeriesOptionAttributes.Where( s => s.Series.IsActive ).Select( soa => soa.Series.Name )
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
			case "shape":
				aList.ForEach( aoi =>
					{
						var img = database.ImageFiles.FirstOrDefault( i => i.TableShape == aoi.Name );
						if( img != null )
						{
							aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
						}
					} );
				break;
			case "base options":
				aList.ForEach( aoi =>
					{
						var img = database.ImageFiles.FirstOrDefault( i => i.TableBase == aoi.Name );
						if( img != null )
						{
							aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
						}
					} );
				break;
			case "control mechanism":
				aList.ForEach( aoi =>
				{
					var img = database.ImageFiles.FirstOrDefault( i => i.ControlMechanism == aoi.Name );
					if( img != null )
					{
						aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
					}
				} );
				break;
			}

			return aList;
		}

		public IEnumerable<FinishInformation> GetFinishDetailList()
		{
			var aInfo = database.Attributes
				.FirstOrDefault( a => a.Name == "Finish" );

			if( aInfo == null )
			{
				return new List<FinishInformation>();
			}

			var aList = aInfo.AttributeOptions
				.Select( ao => new FinishInformation()
				{
					Name = ao.Name,
					Serieses = ao.SeriesOptionAttributes.Where( s => s.Series.IsActive ).Select( soa => soa.Series.Name ).OrderBy( s => s )
				} )
				.OrderBy( a => a.Name )
				.ToList();

			aList.ForEach( aoi =>
			{
				var img = database.ImageFiles.FirstOrDefault( i => i.FeaturedFinish == aoi.Name );
				if( img != null )
				{
					aoi.Caption = img.Caption;
					aoi.FinishType = img.FinishType ?? 0;
					aoi.FinishSubType = img.FinishSubType ?? 0;
					aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
				}
			} );

			return aList;
		}

		public CMInformation GetImageCMInfo( int imageId )
		{
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( img != null )
			{
				var aOption = database.AttributeOptions
					.Where( ao => ao.Attribute.Name == "Control Mechanism" && ao.Name == img.ControlMechanism )
					.FirstOrDefault();

				if( aOption != null )
				{
					return new CMInformation()
					{
						Name = img.ControlMechanism,
						Serieses = aOption.SeriesOptionAttributes.Where( s => s.Series.IsActive ).Select( soa => soa.Series.Name ).OrderBy( s => s ),
						Title = img.Name,
						FeaturedImage = img.ThumbnailImageData( "l16to9" ),
						Description = img.ControlDescription
					};
				}
			}

			return null;
		}

		public IEnumerable<CMInformation> GetCMDetailList()
		{
			var aInfo = database.Attributes
				.FirstOrDefault( a => a.Name == "Control Mechanism" );

			if( aInfo == null )
			{
				return new List<CMInformation>();
			}

			var aList = aInfo.AttributeOptions
				.Select( ao => new CMInformation()
				{
					Name = ao.Name,
					Serieses = ao.SeriesOptionAttributes.Where( s => s.Series.IsActive ).Select( soa => soa.Series.Name ).OrderBy( s => s )
				} )
				.OrderBy( a => a.Name )
				.ToList();

			aList.ForEach( aoi =>
			{
				var img = database.ImageFiles.FirstOrDefault( i => i.ControlMechanism == aoi.Name );
				if( img != null )
				{
					aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
					aoi.Description = img.ControlDescription;
				}
			} );

			return aList;
		}
	}
}
