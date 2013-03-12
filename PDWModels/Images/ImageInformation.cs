using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDWModels.Images
{
	public class ImageInformation
	{
		public enum ImageContents
		{
			All = 0,
			Image = 1,
			Edge = 2,
			Pull = 3,
			Finish = 4
		}

		public int ImageID { get; set; }
		[Required]
		[DisplayName("Image Name")]
		public string ImageName { get; set; }
		[Required]
		[DisplayName( "Caption" )]
		public string Caption { get; set; }
		[Required]
		[DisplayName( "Keywords" )]
		public string Keywords { get; set; }
		[DisplayName( "People in Picture?" )]
		public bool HasPeople { get; set; }
		[Required]
		[DisplayName( "Image Type" )]
		public string ImageType { get; set; }
		[DisplayName( "Featured Pull" )]
		public string FeaturedPull { get; set; }
		[DisplayName( "Featured Edge" )]
		public string FeaturedEdge { get; set; }
		[DisplayName( "Image Content" )]
		public int ImageContent { get; set; }
	}
}
