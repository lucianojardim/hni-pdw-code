using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Images;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;

namespace PWDRepositories
{
	public class ImageRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public ImageRepository()
		{
		}

		public class PDWImageType
		{
			public string Abbreviation { get; set; }
			public string Description { get; set; }
		}

		public static List<PDWImageType> ImageTypes = new List<PDWImageType>()
			{
				new PDWImageType() { Abbreviation= "Env", Description = "Environmental" },
				new PDWImageType() { Abbreviation= "WS", Description = "White Sweep" },
				new PDWImageType() { Abbreviation= "Det", Description = "Detail" },
				new PDWImageType() { Abbreviation= "ILD", Description = "Isometric Line Drawing" },
				new PDWImageType() { Abbreviation= "OFp", Description = "Overhead Footprint" },
				new PDWImageType() { Abbreviation= "RR", Description = "Rough Render" },
				new PDWImageType() { Abbreviation= "LDr", Description = "Line Drawing" },
			};

		private ImageDetail ToImageDetail( ImageFile img )
		{
			return new ImageDetail()
			{
				ImageID = img.ImageID,
				Name = img.Name,
				Caption = img.Caption,
				Keywords = img.Keyword,
				HasReferences = img.SeriesImageFiles.Any()
			};
		}

		private ImageInformation ToImageInformation( ImageFile img )
		{
			return new ImageInformation()
			{
				ImageID = img.ImageID,
				ImageName = img.Name,
				Caption = img.Caption,
				Keywords = img.Keyword,
				ImageType = img.ImageType,
				HasPeople = img.HasPeople,
			};
		}

		private ImageItemDetails ToImageItemDetails( ImageFile img )
		{
			var seriesList = img.SeriesImageFiles
					.Select( s => new ImageItemDetails.ImageSeries()
					{
						SeriesID = s.SeriesID,
						Name = s.Series.Name,
						TypicalList = s.Series
							.SeriesTypicals
								.Where( t => t.Typical.TypicalImageFiles.Any( tif => tif.ImageID == s.ImageID ) )
								.Select( st1 => st1.Typical.Name )
					} ).ToList();

			// add orphaned typicals
			foreach( var typicalImage in img.TypicalImageFiles )
			{
				foreach( var typicalSeries in typicalImage.Typical.SeriesTypicals.Select( st => st.Series ) )
				{
					var imageSeries = seriesList.FirstOrDefault( s => typicalSeries.SeriesID == s.SeriesID );
					if( imageSeries == null )
					{
						// series is not already listed
						seriesList.Add( new ImageItemDetails.ImageSeries()
						{
							SeriesID = typicalSeries.SeriesID,
							Name = typicalSeries.Name,
							TypicalList = new List<string>() { typicalImage.Typical.Name }
						} );
					}
					else
					{
						// make sure the typical is listed
						if( !imageSeries.TypicalList.Contains( typicalImage.Typical.Name ) )
						{
							imageSeries.TypicalList = imageSeries.TypicalList.Union( new List<string>() { typicalImage.Typical.Name } );
						}
					}
				}
			}

			return new ImageItemDetails()
			{
				FileName = img.ThumbnailImageData( "l16to9" ).FileName,
				Caption = img.Caption,
				Name = img.Name,
				SeriesList = seriesList,
				HiResFileName = img.OriginalImage
			};
		}

		public ImageThumbnailGallery GetImageThumbnailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords, int pageNum, int pageSize )
		{
			ImageThumbnailGallery gallery = new ImageThumbnailGallery();

			var imgList = database.ImageFiles.AsQueryable();
			gallery.TotalImageCount = imgList.Count();

			if( (imageTypes ?? "").Any() )
			{
				var imgTypeList = imageTypes.ToLower().Split( ',' );
				imgList = imgList.Where( img => imgTypeList.Contains( img.ImageType ) );
			}
			if( seriesId.HasValue )
			{
				imgList = imgList.Where( img => img.SeriesImageFiles.Any( s => s.SeriesID == seriesId.Value ) );
			}
			if( (categories ?? "").Any() )
			{
				var catList = categories.Split( ',' );
				imgList = imgList.Where( img => catList.Intersect( img.SeriesImageFiles.Select( s => s.Series.Category.Name ) ).Any() );
			}
			if( (keywords ?? "").Any() )
			{
				var wordList = keywords.Split( ',' );
				imgList = imgList.Where( img => wordList.Any( w => img.Keyword.Contains( w.Trim() ) ) );
			}

			gallery.FilteredImageCount = imgList.Count();
			var orderedList = imgList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.SeriesImageFiles.Max( sif => sif.Series.SeriesIntAttributes.FirstOrDefault( a => a.Attribute.Name == "Ranking" ).Value ) );
					break;
				case "mostrecent":
					orderedList = orderedList
						.OrderByDescending( i => i.CreatedDate );
					break;
			}
			gallery.GalleryOfImages = orderedList
				.Skip( (pageNum - 1) * pageSize )
				.Take( pageSize )
				.ToList()
				.Select( img => new ImageSummary() { Name = img.Name, FileName = img.ThumbnailImageData( "m4to3" ).FileName, ImageID = img.ImageID } );

			return gallery;
		}

		public ImageListGallery GetImageDetailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords, int pageNum, int pageSize )
		{
			ImageListGallery gallery = new ImageListGallery();

			var imgList = database.ImageFiles.AsQueryable();
			gallery.TotalImageCount = imgList.Count();

			if( (imageTypes ?? "").Any() )
			{
				var imgTypeList = imageTypes.ToLower().Split( ',' );
				imgList = imgList.Where( img => imgTypeList.Contains( img.ImageType ) );
			}
			if( seriesId.HasValue )
			{
				imgList = imgList.Where( img => img.SeriesImageFiles.Any( s => s.SeriesID == seriesId.Value ) );
			}
			if( (categories ?? "").Any() )
			{
				var catList = categories.Split( ',' );
				imgList = imgList.Where( img => catList.Intersect( img.SeriesImageFiles.Select( s => s.Series.Category.Name ) ).Any() );
			}
			if( (keywords ?? "").Any() )
			{
				var wordList = keywords.Split( ',' );
				imgList = imgList.Where( img => wordList.Any( w => img.Keyword.Contains( w.Trim() ) ) );
			}

			gallery.FilteredImageCount = imgList.Count();
			var orderedList = imgList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.SeriesImageFiles.Max( sif => sif.Series.SeriesIntAttributes.FirstOrDefault( a => a.Attribute.Name == "Ranking" ).Value ) );
					break;
				case "mostrecent":
					orderedList = orderedList
						.OrderByDescending( i => i.CreatedDate );
					break;
			}
			gallery.GalleryOfImages = orderedList
				.Skip( (pageNum - 1) * pageSize )
				.Take( pageSize )
				.ToList()
				.Select( img => new ImageListSummary() { Caption = img.Caption, FileName = img.ThumbnailImageData( "m4to3" ).FileName, Name = img.Name, ImageID = img.ImageID } );

			return gallery;
		}

		public ImageItemDetails GetImageDetailInfo( int imageId )
		{
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( img != null )
			{
				return ToImageItemDetails( img );
			}

			return null;
		}

		public HiResImageInfo GetHiResImageInfo( int imageId )
		{
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( img != null )
			{
				return new HiResImageInfo() { FileName = img.OriginalImage, MIMEType = img.MIMEType };
			}

			return null;
		}

		public IEnumerable<ImageDetail> GetFullImageList( DataTableParams param,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var imgList = database.ImageFiles.AsQueryable();

			totalRecords = imgList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				imgList = imgList.Where( i => i.Keyword.Contains( param.sSearch ) ||
					i.Name.Contains( param.sSearch ) ||
					i.Caption.Contains( param.sSearch ) );
			}
			displayedRecords = imgList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<ImageFile> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = imgList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = imgList.OrderByDescending( v => v.Name );
					}
					break;
			}

			if( (displayedRecords > param.iDisplayLength) && (param.iDisplayLength > 0) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToImageDetail( v ) );
		}

		public ImageInformation GetImageInformation( int id )
		{
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == id );

			if( img != null )
			{
				return ToImageInformation( img );
			}

			return null;
		}

		private void ResizeImage( string fileName, Image fullSizeImg, int widthRatio, int heightRatio, int maxWidth, int maxHeight, string cropLocation )
		{
			int height = fullSizeImg.Height, width = fullSizeImg.Width;
			float heightFactor = (float)height / heightRatio, widthFactor = (float)width / widthRatio;
			int startHeight = 0, startWidth = 0, scaledHeight = 0, scaledWidth = 0;

			if( heightFactor < widthFactor )
			{
				scaledHeight = height;
				scaledWidth = (height * widthRatio) / heightRatio;
				startHeight = 0;
				switch( cropLocation.ToLower() )
				{
					case "left":
						startWidth = 0;
						break;
					case "right":
						startWidth = width - scaledWidth;
						break;
					default:
						startWidth = (width - scaledWidth) / 2;
						break;
				}
			}
			else if( heightFactor > widthFactor )
			{
				scaledHeight = (width * heightRatio) / widthRatio;
				scaledWidth = width;
				switch( cropLocation.ToLower() )
				{
					case "top":
						startHeight = 0;
						break;
					case "bottom":
						startHeight = height - scaledHeight;
						break;
					default:
						startHeight = (height - scaledHeight) / 2;
						break;
				}
				startWidth = 0;
			}
			else
			{
				scaledHeight = height;
				scaledWidth = width;
				startHeight = 0;
				startWidth = 0;
			}

			Image smImage = new Bitmap( scaledHeight, scaledWidth );

			using( Graphics g = Graphics.FromImage( smImage ) )
			{
				g.DrawImage( fullSizeImg, new Rectangle( 0, 0, smImage.Width, smImage.Height ),
								 new Rectangle( startWidth, startHeight, scaledWidth, scaledHeight ),
								 GraphicsUnit.Pixel );
			}

			if( (scaledHeight > maxHeight) && (maxHeight > 0) )
			{
				scaledWidth = (scaledWidth * maxHeight) / scaledHeight;
				scaledHeight = maxHeight;
			}
			if( (scaledWidth > maxWidth) && (maxWidth > 0) )
			{
				scaledHeight = (scaledHeight * maxWidth) / scaledWidth;
				scaledWidth = maxWidth;
			}

			if( (scaledHeight != fullSizeImg.Height) || (scaledWidth != fullSizeImg.Width) )
			{
				Image.GetThumbnailImageAbort dummyCallBack = new Image.GetThumbnailImageAbort( ThumbnailCallback );
				Image thumbNailImg = smImage.GetThumbnailImage( scaledWidth, scaledHeight, dummyCallBack, IntPtr.Zero );

				thumbNailImg.Save( fileName, ImageFormat.Png );
			}
			else
			{
				smImage.Save( fileName, ImageFormat.Png );
			}
		}

		public void UploadImageFile( int id, Stream fStream, int fileLength, string origFileName, string mimeType )
		{
			ImageFile imgData = database.ImageFiles.FirstOrDefault( i => i.ImageID == id );
			if( imgData == null )
			{
				throw new Exception( "Cannot find Image record." );
			}

			imgData.MIMEType = mimeType;

			if( database.SaveChanges() > 0 )
			{
				UploadImage( imgData.Name, fStream, fileLength, origFileName );
			}
		}

		public void ImportImageFileData( ImageInformation imgInfo, Stream fStream, int fileLength, string origFileName, string mimeType )
		{
			if( database.ImageFiles.Any( i => i.Name == imgInfo.ImageName ) )
			{
				throw new Exception( "Image with this name already exists." );
			}

			ImageFile imgData = new ImageFile();
			imgData.Name = imgInfo.ImageName;
			imgData.OriginalExtension = Path.GetExtension( origFileName );
			imgData.Caption = imgInfo.Caption;
			imgData.HasPeople = imgInfo.HasPeople;
			imgData.ImageType = imgInfo.ImageType;
			imgData.Keyword = imgInfo.Keywords;
			imgData.CreatedDate = DateTime.Now;
			imgData.MIMEType = mimeType;

			database.ImageFiles.AddObject( imgData );

			database.SaveChanges();

			UploadImage( imgInfo.ImageName, fStream, fileLength, origFileName );
		}

		public void UpdateImageFile( ImageInformation imgInfo )
		{
			ImageFile imgData = database.ImageFiles.FirstOrDefault( i => i.ImageID == imgInfo.ImageID );
			if( imgData == null )
			{
				throw new Exception( "Cannot find Image record." );
			}

			imgData.Caption = imgInfo.Caption;
			imgData.HasPeople = imgInfo.HasPeople;
			imgData.ImageType = imgInfo.ImageType;
			imgData.Keyword = imgInfo.Keywords;

			database.SaveChanges();
		}

		public void DeleteImageFile( int imageID )
		{
			ImageFile imgData = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageID );
			if( imgData != null )
			{
				if( !imgData.SeriesImageFiles.Any() )
				{
					database.ImageFiles.DeleteObject( imgData );

					database.SaveChanges();
				}
			}
		}

		private void UploadImage( string imageName, Stream fStream, int fileLength, string origFileName )
		{
			DirectoryInfo dInfo = new DirectoryInfo( ConfigurationManager.AppSettings["ImageFileLocation"] );
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];

			foreach( var f in dInfo.EnumerateFiles( "*" + prependImageName + " " + imageName + "*.*" ).ToList() )
			{
				f.Delete();
			}

			Image fullSizeImg = Image.FromStream( fStream );
			fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], prependImageName + " " + imageName + Path.GetExtension( origFileName ) ) );

			var ImageSizes = (ImageCropConfiguration)ConfigurationManager.GetSection( "imageCropSettings" );
			foreach( PDWInfrastructure.ImageCropConfiguration.CropSettingElement iSize in ImageSizes.ImageSizes )
			{
				ResizeImage( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], prependImageName + " " + imageName + "_" + iSize.Suffix + ".png" ),
					fullSizeImg, iSize.RatioWidth, iSize.RatioHeight, iSize.MaxWidth, iSize.MaxHeight, iSize.CropType );
			}
		}

		private bool ThumbnailCallback()
		{
			return false;
		}
	}
}
