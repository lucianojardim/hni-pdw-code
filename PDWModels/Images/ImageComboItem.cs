﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;

namespace PDWModels.Images
{
	public class ImageComboItem
	{
		public ImageComboItem()
		{
		}

		private ImageComboItem( ImageFile.ImageData img )
		{
			ID = img.ID;
			Name = img.FileName;
			CanLightbox = img.CanLightbox;
			TypicalName = img.TypicalName;
		}

		public int ID { get; set; }
		public string Name { get; set; }
		public bool CanLightbox { get; set; }
		public string TypicalName { get; set; }

		public static implicit operator ImageComboItem( ImageFile.ImageData img )
		{
			if( img == null )
				return null;

			return new ImageComboItem( img );
		}
	}
}
