using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public IEnumerable<string> ImageListForSize( string suffix )
		{
			return SeriesImageFiles.Where( s => !s.IsFeatured ).Select( i => i.ImageFile.ThumbnailImageName( suffix ) );
		}

		public string FeaturedImageForSize( string suffix )
		{
			var fImg = SeriesImageFiles.FirstOrDefault( i => i.IsFeatured );
			if( fImg != null )
			{
				return fImg.ImageFile.ThumbnailImageName( suffix );
			}

			return null;
		}

		public string FeaturedImageFileName
		{
			get
			{
				var fImg = SeriesImageFiles.FirstOrDefault( i => i.IsFeatured );
				if( fImg != null )
				{
					return fImg.ImageFile.Name + fImg.ImageFile.OriginalExtension;
				}

				return null;
			}
		}

		public IEnumerable<string> AttributeSet( string attName )
		{
			return SeriesOptionAttributes.Where( s => s.Attribute.Name == attName ).Select( a => a.AttributeOption.Name );
		}
	}
}
