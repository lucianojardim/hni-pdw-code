using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWDBContext
{
	public partial class ImageFile
	{
		public class ImageData
		{
			public int ID { get; set; }
			public string FileName { get; set; }
			public bool CanLightbox { get; set; }
		}

		public class PDWImageType
		{
			public string Abbreviation { get; set; }
			public string Description { get; set; }
			public bool CanLightbox { get; set; }
		}

		// any changes to the CanLightbox values here means they will need to be updated in imageGallery.js of main site
		public static List<PDWImageType> ImageTypes = new List<PDWImageType>()
			{
				new PDWImageType() { Abbreviation= "Env", Description = "Environmental", CanLightbox = true },
				new PDWImageType() { Abbreviation= "WS", Description = "White Sweep", CanLightbox = true },
				new PDWImageType() { Abbreviation= "Det", Description = "Detail", CanLightbox = true },
				new PDWImageType() { Abbreviation= "ILD", Description = "Isometric Line Drawing", CanLightbox = false },
				new PDWImageType() { Abbreviation= "OFp", Description = "Overhead Footprint", CanLightbox = false },
				new PDWImageType() { Abbreviation= "RR", Description = "Rough Render", CanLightbox = false },
				new PDWImageType() { Abbreviation= "LDr", Description = "Line Drawing", CanLightbox = false },
			};

		public static bool ImageCanLightbox( string imageType )
		{
			var iType = ImageTypes.FirstOrDefault( i => i.Abbreviation == imageType );
			if( iType != null )
				return iType.CanLightbox;

			return false;
		}

		public ImageData ThumbnailImageData( string suffix )
		{
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];
			return new ImageData() { ID = ImageID, FileName = prependImageName + " " + Name + "_" + suffix + ".jpg", CanLightbox = ImageCanLightbox( ImageType ) };
		}

		public string OriginalImage
		{
			get
			{
				string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];
				return prependImageName + " " + Name + OriginalExtension;
			}
		}
	}
}
