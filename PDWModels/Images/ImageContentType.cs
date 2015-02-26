using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageContentType
	{
		public ImageContentType()
		{
		}

		public ImageContentType( int i, string d )
		{
			TypeId = i;
			DisplayText = d;
		}

		public int TypeId { get; set; }
		public string DisplayText { get; set; }
	}
}
