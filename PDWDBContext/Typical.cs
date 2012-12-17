using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class Typical
	{
		public IEnumerable<string> ImageListForSize( string suffix, int maxImageCount = 0 )
		{
			var imgList = TypicalImageFiles.Where( s => !s.IsFeatured );

			if( (maxImageCount > 0) )
			{
				var randNum = new Random();
				imgList = imgList.OrderBy( i => randNum.Next() ).Take( maxImageCount );
			}

			return imgList.Select( i => i.ImageFile.ThumbnailImageName( suffix ) );
		}

		public string FeaturedImageForSize( string suffix )
		{
			var fImg = TypicalImageFiles.FirstOrDefault( i => i.IsFeatured );
			if( fImg != null )
			{
				return fImg.ImageFile.ThumbnailImageName( suffix );
			}

			return null;
		}

		public int IntAttribute( string attName )
		{
			var att = TypicalIntAttributes.FirstOrDefault( a => a.TAttribute.Name == attName );
			if( att != null )
			{
				return att.Value;
			}

			return 0;
		}

		public string TextAttribute( string attName )
		{
			var att = TypicalTextAttributes.FirstOrDefault( a => a.TAttribute.Name == attName );
			if( att != null )
			{
				return att.Value;
			}

			return "";
		}

		public IEnumerable<string> AttributeSet( string attName )
		{
			return TypicalOptionAttributes.Where( s => s.TAttribute.Name == attName ).Select( a => a.TAttributeOption.Name );
		}
	}
}
