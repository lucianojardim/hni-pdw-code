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
using PDWModels;
using System.Data.Entity;

namespace PWDRepositories
{
	public class ImageRepository : BaseRepository
	{
		public ImageRepository()
		{
		}

		public class PublicImageType
		{
			public string Abbreviation { get; set; }
			public string Description { get; set; }
		}

		public static IEnumerable<PublicImageType> ImageTypes { get { return ImageFile.ImageTypes.Select( i => new PublicImageType() { Abbreviation = i.Abbreviation, Description = i.Description } ); } }

		private ImageDetail ToImageDetail( ImageFile img )
		{
			return new ImageDetail()
			{
				ImageID = img.ImageID,
				Name = img.Name,
				Caption = img.Caption,
				Keywords = img.Keyword,
				HasReferences = img.SeriesImageFiles.Any() || img.TypicalImageFiles.Any() || img.Showrooms.Any() || img.eCollateralSections.Any()
			};
		}

		private PubImageDetail ToPubImageDetail( ImageFile img, int pubId )
		{
			var pubImage = img.PublicationImages.FirstOrDefault( pi => pi.PublicationID == pubId );
			return new PubImageDetail()
			{
				ImageID = img.ImageID,
				Name = img.Name,
				PageNumber = (pubImage != null) ? pubImage.PageNumber : null
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
				ImageContent = img.ImageContent,
				ImageType = img.ImageType,
				HasPeople = img.HasPeople,
				FeaturedEdge = img.FeaturedEdge,
				FeaturedPull = img.FeaturedPull,
				FeaturedFinish = img.FeaturedFinish,
				FinishType = img.FinishType,
				FinishCode = img.FinishCode,
				LaminatePattern = img.LaminatePattern,
				LaminateIsTFL = img.LaminateIsTFL,
				LaminateIsHPL = img.LaminateIsHPL,
				VeneerGrade = img.VeneerGrade,
				VeneerSpecies = img.VeneerSpecies,
				SeatingGrade = img.SeatingGrade,
				FeaturedTableBase = img.TableBase,
				FeaturedTableShape = img.TableShape,
				ControlMechanism = img.ControlMechanism,
				ControlDescription = img.ControlDescription,
				GoToGuidePageNum = img.GoToGuidePage ?? 0
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
								.Where( t => t.Typical.IsPublished && t.Typical.TypicalImageFiles.Any( tif => tif.ImageID == s.ImageID ) )
								.Select( st1 => st1.Typical.Name )
								.ToList()
					} ).ToList();

			// add orphaned typicals
			foreach( var typicalImage in img.TypicalImageFiles.Where( tif => tif.Typical.IsPublished ) )
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

		private ImageItemDetails ToImageItemDetails( AttributeOption ao, ImageFile img )
		{
			return new ImageItemDetails()
			{
				FileName = img.ThumbnailImageData( "l16to9" ).FileName,
				Caption = img.Caption,
				Name = img.Name,
				SecondaryName = ao.Name,
				SeriesList = ao.SeriesOptionAttributes.Select( s => new ImageItemDetails.ImageSeries()
					{
						SeriesID = s.SeriesID,
						Name = s.Series.Name,
					} ).ToList(),
				HiResFileName = img.OriginalImage
			};
		}

		public IEnumerable<ImageComboItem> GetRecentImageList( int ct )
		{
			var imageTypeList = ImageFile.ImageTypes.Where( i => i.CommonImage )
					.Select( iType => iType.Abbreviation )
					.ToList();
			var imgList = database.ImageFiles
				.Where( i => !i.HasPeople )
				.Where( imgFile => imageTypeList.Any( it => it == imgFile.ImageType ) )
				.AsQueryable();

			return imgList
				.OrderByDescending( i => i.CreatedDate )
				.Take( ct )
				.ToList()
				.Select( i => (ImageComboItem)(i.ThumbnailImageData( "s16to9" ))  );
		}

		public ImageThumbnailGallery GetGoToGuideImageList()
		{
			ImageThumbnailGallery gallery = new ImageThumbnailGallery();

			var imgList = database.ImageFiles
				.Where( imgFile => imgFile.ImageContent == (int)ImageInformation.ImageContents.GoToGuide )
				.AsQueryable();
			gallery.TotalImageCount = imgList.Count();

			// filter by...

			gallery.FilteredImageCount = imgList.Count();
			var orderedList = imgList
				.OrderBy( i => i.GoToGuidePage );

			gallery.GalleryOfImages = orderedList
				.ToList()
				.Select( img => new ImageSummary()
				{
					Name = img.Name,
					FileName = img.ThumbnailImageData( "mBase" ).FileName,
					ImageID = img.ImageID,
					CanLightbox = ImageFile.ImageCanLightbox( img.ImageType ),
					HiResFileName = img.OriginalImage,
					TypicalName = img.ThumbnailImageData( "mBase" ).TypicalName
				} );

			return gallery;
		}

		public ImageThumbnailGallery GetImageThumbnailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords,
			int? pubId, string pubPageNum, string contentTypes, bool includePeople, int pageNum, int pageSize )
		{
			ImageThumbnailGallery gallery = new ImageThumbnailGallery();

			var imageTypeList = ImageFile.ImageTypes.Where( i => i.CommonImage )
					.Select( iType => iType.Abbreviation )
					.ToList();
			var imgList = database.ImageFiles
				.Include( i => i.TypicalImageFiles.Select( tif => tif.Typical ) )
				.Where( i => !i.HasPeople || includePeople )
				.Where( imgFile => imageTypeList.Any( it => it == imgFile.ImageType ) )
				.AsQueryable();
			gallery.TotalImageCount = imgList.Count();

			IEnumerable<int> contentTypeList = new List<int>() { (int)ImageInformation.ImageContents.Image };
			if( (contentTypes ?? "").Any() )
			{
				contentTypeList = contentTypeList.Union( (contentTypes ?? "").ToLower().Split( ',' ).Select( c => int.Parse( c ) ) );
			}
			imgList = imgList.Where( img => contentTypeList.Contains( img.ImageContent ) );

			// require image types
			{
				var imgTypeList = (imageTypes ?? "").ToLower().Split( ',' );
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
			if( pubId.HasValue )
			{
				imgList = imgList.Where( img => img.PublicationImages.Any( pi => pi.PublicationID == pubId.Value ) );
				if( (pubPageNum ?? "").Any() )
				{
					int nPubPageNum = 0;
					if( int.TryParse( pubPageNum, out nPubPageNum ) )
					{
						imgList = imgList.Where( img => img.PublicationImages.Any( pi => (pi.PublicationID == pubId.Value) && ((pi.PageNumber == nPubPageNum) || (pi.PageNumber == null)) ) );
					}
				}
			}
			if( (keywords ?? "").Any() )
			{
				var termList = SearchText.GetSearchList( keywords );

				Dictionary<int, int> termImageList = new Dictionary<int, int>();
				foreach( var term in termList )
				{
					var theList = database.ImageFiles
						.Where( s => s.DBKeywords.Contains( term.searchText ) )
						.Select( i => i.ImageID )
						.ToList();

					theList.ForEach( delegate( int imgId )
						{
							termImageList[imgId] = (termImageList.Keys.Contains( imgId ) ? termImageList[imgId] : 0) | (int)Math.Pow(2, term.idxWord);
						} );
				}

				int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
				var imageIdList = termImageList.Keys.Where( i => termImageList[i] == rightAnswer );

				imgList = imgList.Where( i => imageIdList.Contains( i.ImageID ) );
			}

			gallery.FilteredImageCount = imgList.Count();
			var orderedList = imgList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.Popularity ).ThenByDescending( i => i.CreatedDate );
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
				.Select( img => new ImageSummary() 
				{ 
					Name = img.Name, 
					FileName = img.ThumbnailImageData( "m16to9" ).FileName, 
					ImageID = img.ImageID,
					CanLightbox = ImageFile.ImageCanLightbox( img.ImageType ),
					HiResFileName = img.OriginalImage,
					TypicalName = img.ThumbnailImageData( "m16to9" ).TypicalName
				} );

			return gallery;
		}

		public ImageListGallery GetImageDetailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords,
			int? pubId, string pubPageNum, string contentTypes, int pageNum, int pageSize )
		{
			ImageListGallery gallery = new ImageListGallery();

			var imageTypeList = ImageFile.ImageTypes.Where( i => i.CommonImage )
					.Select( iType => iType.Abbreviation );
			var imgList = database.ImageFiles
				.Include( i => i.TypicalImageFiles.Select( tif => tif.Typical ) )
				.Where( imgFile => imageTypeList.Any( it => it == imgFile.ImageType ) )
				.AsQueryable();
			gallery.TotalImageCount = imgList.Count();

			IEnumerable<int> contentTypeList = new List<int>() { (int)ImageInformation.ImageContents.Image };
			if( (contentTypes ?? "").Any() )
			{
				contentTypeList = contentTypeList.Union( (contentTypes ?? "").ToLower().Split( ',' ).Select( c => int.Parse( c ) ) );
			}
			imgList = imgList.Where( img => contentTypeList.Contains( img.ImageContent ) );

			// require image types
			{
				var imgTypeList = (imageTypes ?? "").ToLower().Split( ',' );
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
				var termList = SearchText.GetSearchList( keywords );

				Dictionary<int, int> termImageList = new Dictionary<int, int>();
				foreach( var term in termList )
				{
					var theList = database.ImageFiles
						.Where( s => s.DBKeywords.Contains( term.searchText ) )
						.Select( i => i.ImageID )
						.ToList();

					theList.ForEach( delegate( int imgId )
						{
							termImageList[imgId] = (termImageList.Keys.Contains( imgId ) ? termImageList[imgId] : 0) | (int)Math.Pow( 2, term.idxWord );
						} );
				}

				int rightAnswer = ((int)Math.Pow( 2, (termList.Select( t => t.idxWord ).Max() + 1) ) - 1);
				var imageIdList = termImageList.Keys.Where( i => termImageList[i] == rightAnswer );

				imgList = imgList.Where( i => imageIdList.Contains( i.ImageID ) );
			}

			gallery.FilteredImageCount = imgList.Count();
			var orderedList = imgList
				.OrderBy( i => i.Name );
			switch( (sortBy ?? "").ToLower() )
			{
				case "mostpopular":
					orderedList = orderedList
						.OrderByDescending( i => i.Popularity ).ThenByDescending( i => i.CreatedDate );
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
				.Select( img => ToImageListSummary( img ) );

			return gallery;
		}

		private ImageListSummary ToImageListSummary( ImageFile img, bool largeImage = false )
		{
			return new ImageListSummary()
				{
					Caption = img.Caption,
					FileName = img.ThumbnailImageData( largeImage ? "l16to9" : "m16to9" ).FileName,
					Name = img.Name,
					ImageID = img.ImageID,
					CanLightbox = ImageFile.ImageCanLightbox( img.ImageType ),
					TypicalName = img.ThumbnailImageData( "m16to9" ).TypicalName
				};
		}

		public IEnumerable<ImageListSummary> GetECollateralImageList( string imgFilter )
		{
			var imageTypeList = ImageFile.ImageTypes.Where( i => i.CommonImage )
					.Select( iType => iType.Abbreviation );

			return database.ImageFiles
				.Where( imgFile => imageTypeList.Any( it => it == imgFile.ImageType ) )
				.Where( i => i.Name.Contains( imgFilter ) )
				.Distinct()
				.ToList()
				.Select( img => ToImageListSummary( img ) );
		}

		public IEnumerable<ImageListSummary> Search( string searchText, bool largeImage = false )
		{
			var termList = SearchText.GetSearchList( searchText );

			var imageTypeList = ImageFile.ImageTypes.Where( i => i.CommonImage )
					.Select( iType => iType.Abbreviation );

			Dictionary<int, int> termImageList = new Dictionary<int,int>();
			foreach( var term in termList )
			{
				var theList = database.ImageFiles
					.Where( s => s.DBKeywords.Contains( term.searchText ) )
					.Select( i => i.ImageID )
					.ToList();

				theList.ForEach( delegate( int imgId )
				{
					termImageList[imgId] = (termImageList.Keys.Contains( imgId ) ? termImageList[imgId] : 0) | (int)Math.Pow(2, term.idxWord);
				} );
			}

			int rightAnswer = ((int)Math.Pow(2, (termList.Select( t => t.idxWord ).Max() + 1)) - 1);
			var imageIdList = termImageList.Keys.Where( i => termImageList[i] == rightAnswer );

			return database.ImageFiles
				.Where( i => !i.HasPeople )
				.Where( imgFile => imageTypeList.Any( it => it == imgFile.ImageType ) )
				.Where( i => imageIdList.Contains( i.ImageID ) )
				.Distinct()
				.OrderByDescending( i => i.CreatedDate )
				.ToList()
				.Select( img => ToImageListSummary( img, largeImage ) )
				.ToList();
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

		public ImageItemDetails GetImageFinishInfo( int imageId )
		{
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( img != null )
			{
				var aOption = database.AttributeOptions
					.Where( ao => ao.Attribute.Name == "Finish" && ao.Name == img.FeaturedFinish )
					.FirstOrDefault();

				if( aOption != null )
				{
					if( ( img.FinishCode ?? "" ).Any() )
					{
						aOption.Name += (" (" + img.FinishCode + ")");
					}

					return ToImageItemDetails( aOption, img );
				}
			}

			return null;
		}

		public IEnumerable<ImageContentType> GetImageContentTypeList()
		{
			return new List<ImageContentType>()
			{
				new ImageContentType( (int)ImageInformation.ImageContents.Edge, "Edge" ),
				new ImageContentType( (int)ImageInformation.ImageContents.Pull, "Pull" ),
				new ImageContentType( (int)ImageInformation.ImageContents.Finish, "Finish" ),
				new ImageContentType( (int)ImageInformation.ImageContents.TableShape, "Table Shape" ),
				new ImageContentType( (int)ImageInformation.ImageContents.TableBase, "Table Base" ),
				new ImageContentType( (int)ImageInformation.ImageContents.ControlMech, "Control Mechanism" )
			};
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

		public IEnumerable<ImageDetail> GetFullImageList( ImageTableParams param,
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
			if( param.imageContent != (int)ImageInformation.ImageContents.All )
			{
				imgList = imgList.Where( i => i.ImageContent == param.imageContent );
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

			return filteredAndSorted
				.Include( i => i.SeriesImageFiles )
				.Include( i => i.TypicalImageFiles )
				.Include( i => i.Showrooms )
				.Include( i => i.eCollateralSections )
				.ToList().Select( v => ToImageDetail( v ) );
		}

		public IEnumerable<PubImageDetail> GetPubImageList( PubImageTableParams param, bool bInPublication,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var imgList = database.ImageFiles
				.Where( i => i.PublicationImages.Any( pi => pi.PublicationID == param.pubId ) == bInPublication )
				.AsQueryable();

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

			return filteredAndSorted.ToList().Select( v => ToPubImageDetail( v, param.pubId ) );
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

		public ImageUsage GetImageUsage( int id )
		{
			var img = database.ImageFiles
				.Include( i => i.SeriesImageFiles.Select( s => s.Series ) )
				.Include( i => i.TypicalImageFiles.Select( s => s.Typical ) )
				.Include( i => i.Showrooms )
				.Include( i => i.eCollateralSections.Select( s => s.eCollateralItem ) )
				.FirstOrDefault( i => i.ImageID == id );

			if( img != null )
			{
				return new ImageUsage()
				{
					Serieses = img.SeriesImageFiles.Select( s => s.Series.Name ),
					Typicals = img.TypicalImageFiles.Select( t => t.Typical.Name ),
					Showrooms = img.Showrooms.Select( s => s.Name ),
					ECollateral = img.eCollateralSections.Select( e => e.eCollateralItem.FileName )
				};
			}

			return new ImageUsage();
		}

		private void ResizeImage( string fileName, Image fullSizeImg, int widthRatio, int heightRatio, int maxWidth, int maxHeight, string cropLocation )
		{
			int height = fullSizeImg.Height, width = fullSizeImg.Width;
			int startHeight = 0, startWidth = 0, scaledHeight = 0, scaledWidth = 0;
			if( (heightRatio > 0) && (widthRatio > 0) )
			{
				float heightFactor = (float)height / heightRatio, widthFactor = (float)width / widthRatio;

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
				var thumbNailImg = smImage.GetThumbnailImage( scaledWidth, scaledHeight, dummyCallBack, IntPtr.Zero );

				thumbNailImg.Save( fileName, ImageFormat.Jpeg );
			}
			else
			{
				smImage.Save( fileName, ImageFormat.Jpeg );
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
			imgData.OriginalExtension = Path.GetExtension( origFileName );

			database.SaveChanges();

			UploadImage( imgData.Name, fStream, fileLength, origFileName );
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
			imgData.FeaturedEdge = imgInfo.FeaturedEdge;
			imgData.FeaturedPull = imgInfo.FeaturedPull;
			imgData.ImageContent = imgInfo.ImageContent;
			imgData.FeaturedFinish = imgInfo.FeaturedFinish;
			imgData.FinishType = imgInfo.FinishType;
			imgData.FinishCode = imgInfo.FinishCode;
			imgData.LaminatePattern = imgInfo.LaminatePattern;
			imgData.LaminateIsTFL = imgInfo.LaminateIsTFL;
			imgData.LaminateIsHPL = imgInfo.LaminateIsHPL;
			imgData.VeneerGrade = imgInfo.VeneerGrade;
			imgData.VeneerSpecies = imgInfo.VeneerSpecies;
			imgData.SeatingGrade = imgInfo.SeatingGrade;
			imgData.DBKeywords = SearchText.GetKeywordList( new List<string>() { imgData.Name, imgData.Caption, imgData.Keyword } );
			imgData.TableBase = imgInfo.FeaturedTableBase;
			imgData.TableShape = imgInfo.FeaturedTableShape;
			imgData.ControlMechanism = imgInfo.ControlMechanism;
			imgData.ControlDescription = imgInfo.ControlDescription;
			imgData.GoToGuidePage = imgInfo.GoToGuidePageNum;

			database.ImageFiles.Add( imgData );

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
			imgData.FeaturedEdge = imgInfo.FeaturedEdge;
			imgData.FeaturedPull = imgInfo.FeaturedPull;
			imgData.ImageContent = imgInfo.ImageContent;
			imgData.FeaturedFinish = imgInfo.FeaturedFinish;
			imgData.FinishType = imgInfo.FinishType;
			imgData.FinishCode = imgInfo.FinishCode;
			imgData.LaminatePattern = imgInfo.LaminatePattern;
			imgData.LaminateIsTFL = imgInfo.LaminateIsTFL;
			imgData.LaminateIsHPL = imgInfo.LaminateIsHPL;
			imgData.VeneerGrade = imgInfo.VeneerGrade;
			imgData.VeneerSpecies = imgInfo.VeneerSpecies;
			imgData.SeatingGrade = imgInfo.SeatingGrade;
			imgData.DBKeywords = SearchText.GetKeywordList( new List<string>() { imgData.Name, imgData.Caption, imgData.Keyword } );
			imgData.TableBase = imgInfo.FeaturedTableBase;
			imgData.TableShape = imgInfo.FeaturedTableShape;
			imgData.ControlMechanism = imgInfo.ControlMechanism;
			imgData.ControlDescription = imgInfo.ControlDescription;
			imgData.GoToGuidePage = imgInfo.GoToGuidePageNum;

			database.SaveChanges();
		}

		public void DeleteImageFile( int imageID )
		{
			ImageFile imgData = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageID );
			if( imgData != null )
			{
				if( !imgData.SeriesImageFiles.Any() && !imgData.TypicalImageFiles.Any() )
				{
					database.ImageFiles.Remove( imgData );

					database.SaveChanges();
				}
			}
		}

		private void UploadImage( string imageName, Stream fStream, int fileLength, string origFileName )
		{
			DirectoryInfo dInfo = new DirectoryInfo( ConfigurationManager.AppSettings["ImageFileLocation"] );
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];

			var ImageSizes = (ImageCropConfiguration)ConfigurationManager.GetSection( "imageCropSettings" );
			foreach( PDWInfrastructure.ImageCropConfiguration.CropSettingElement iSize in ImageSizes.ImageSizes )
			{
				foreach( var f in dInfo.EnumerateFiles( prependImageName + " " + imageName + "_" + iSize.Suffix + ".jpg" ).ToList() )
				{
					f.Delete();
				}
			}

			Image fullSizeImg = Image.FromStream( fStream );
			fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], prependImageName + " " + imageName + Path.GetExtension( origFileName ) ) );

			foreach( PDWInfrastructure.ImageCropConfiguration.CropSettingElement iSize in ImageSizes.ImageSizes )
			{
				ResizeImage( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], prependImageName + " " + imageName + "_" + iSize.Suffix + ".jpg" ),
					fullSizeImg, iSize.RatioWidth, iSize.RatioHeight, iSize.MaxWidth, iSize.MaxHeight, iSize.CropType );
			}
		}

		private bool ThumbnailCallback()
		{
			return false;
		}

		public void RebuildImageGallery()
		{
			string prependImageName = ConfigurationManager.AppSettings["PrependImageName"];
			foreach( var imgData in database.ImageFiles )
			{
				try
				{
					FileStream fStream = new FileStream(
						Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], prependImageName + " " + imgData.Name + Path.GetExtension( imgData.OriginalExtension ) ),
						FileMode.Open );
					if( fStream != null )
					{
						MemoryStream stream = new MemoryStream();
						fStream.CopyTo( stream );

						fStream.Close();

						UploadImage( imgData.Name, stream, 0, imgData.OriginalExtension );
					}
				}
				catch( FileNotFoundException )
				{
				}
			}
		}

		public IEnumerable<string> GetTypeAheadList( string query, int maxNumber )
		{
			return database.ImageFiles
				.Where( i => i.ImageContent == (int)ImageInformation.ImageContents.Image )
				.Where( i => i.Name.Contains( query ) )
				.OrderBy( i => i.Name )
				.Take( maxNumber )
				.Select( i => i.Name )
				.ToList();
		}
	}
}
