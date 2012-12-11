using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageListGallery
	{
		public int TotalImageCount { get; set; }
		public int FilteredImageCount { get; set; }
		public IEnumerable<ImageListSummary> GalleryOfImages { get; set; }
	}
}
