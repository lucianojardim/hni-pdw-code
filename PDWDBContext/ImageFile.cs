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
		}

		public ImageData ThumbnailImageData( string suffix )
		{
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];
			return new ImageData() { ID = ImageID, FileName = prependImageName + " " + Name + "_" + suffix + ".png" };
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
