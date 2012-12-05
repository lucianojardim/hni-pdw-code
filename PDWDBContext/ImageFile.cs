using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class ImageFile
	{
		public string ThumbnailImageName( string suffix )
		{
			return Name + "_" + suffix + ".png";
		}
	}
}
