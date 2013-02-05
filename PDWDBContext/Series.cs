using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWDBContext
{
	public partial class Series
	{
		public int Ranking
		{
			get
			{
				var rank = SeriesIntAttributes.FirstOrDefault( a => a.Attribute.Name == "Ranking" );
				if( rank != null )
				{
					return rank.Value;
				}

				return 0;
			}
		}

		public IEnumerable<ImageFile.ImageData> ImageListForSize( string suffix, int maxImageCount = 0 )
		{
			var imgList = SeriesImageFiles.Where( s => !s.IsFeatured );

			if( (maxImageCount > 0) )
			{
				imgList = imgList.OrderBy( i => i.DisplayOrder ).Take( maxImageCount );
			}

			return imgList.Select( i => i.ImageFile.ThumbnailImageData( suffix ) );
		}

		public ImageFile.ImageData FeaturedImageForSize( string suffix )
		{
			var fImg = SeriesImageFiles.FirstOrDefault( i => i.IsFeatured );
			if( fImg != null )
			{
				return fImg.ImageFile.ThumbnailImageData( suffix );
			}

			return null;
		}

		public IEnumerable<string> AttributeSet( string attName )
		{
			return SeriesOptionAttributes.Where( s => s.Attribute.Name == attName ).Select( a => a.AttributeOption.Name );
		}
	}
}
