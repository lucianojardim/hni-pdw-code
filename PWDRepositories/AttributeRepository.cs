using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Attributes;

namespace PWDRepositories
{
	public class AttributeRepository : BaseRepository
	{
		public AttributeRepository()
		{
		}

		public IEnumerable<string> GetOptionList( string attr )
		{
			var attrData = database.Attributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.AttributeOptions.Select( ao => ao.Name ).ToList();
			}

			return new List<string>();
		}

		public IEnumerable<AttributeComboItem> GetTypicalOptionList( string attr )
		{
			var attrData = database.TAttributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.TAttributeOptions.Select( ao => new AttributeComboItem() { ID = ao.OptionID, Name = ao.Name } ).ToList();
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
					.Select( ao => ao.Name )
					.ToList();
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
						Serieses = ao.SeriesOptionAttributes.Select( soa => soa.Series.Name ).ToList()
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
					Serieses = ao.SeriesOptionAttributes.Select( soa => soa.Series.Name ).OrderBy( s => s ).ToList()
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
					aoi.LaminatePattern = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Laminate ) ? ( img.LaminatePattern ?? 0 ) : 0;
					aoi.LaminateIsHPL = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Laminate ) ? ( img.LaminateIsHPL ) : false;
					aoi.LaminateIsTFL = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Laminate ) ? ( img.LaminateIsTFL ) : false;
					aoi.VeneerGrade = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Veneer ) ? ( img.VeneerGrade ?? 0 ) : 0;
					aoi.VeneerSpecies = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Veneer ) ? ( img.VeneerSpecies ?? 0 ) : 0;
					aoi.SeatingGrade = ( img.FinishType == (int)PDWModels.Images.ImageInformation.FinishTypes.Seating ) ? ( img.SeatingGrade ?? 0 ) : 0;
					aoi.FeaturedImage = img.ThumbnailImageData( "s16to9" );
					aoi.Popularity = ( aoi.VeneerGrade == (int)PDWModels.Images.ImageInformation.VeneerGrades.Standard ? 4 : 0 ) +
						( aoi.SeatingGrade == (int)PDWModels.Images.ImageInformation.SeatingGrades.Standard ? 2 : 0 ) +
						1; // those with images go higher
				}
			} );

			return aList.OrderByDescending( a => a.Popularity );
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
						Serieses = aOption.SeriesOptionAttributes.Select( soa => soa.Series.Name ).OrderBy( s => s ).ToList(),
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
					Serieses = ao.SeriesOptionAttributes.Select( soa => soa.Series.Name ).OrderBy( s => s )
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
