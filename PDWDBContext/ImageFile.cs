using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWDBContext
{
	public partial class ImageFile
	{
		public string ThumbnailImageName( string suffix )
		{
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];
			return prependImageName + " " + Name + "_" + suffix + ".png";
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
