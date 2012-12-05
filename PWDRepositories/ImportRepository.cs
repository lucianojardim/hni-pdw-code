using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Data.Objects;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

namespace PWDRepositories
{
	public class ImportRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public ImportRepository()
		{
		}

		private void DeleteAllObjects( string dbTable )
		{
			database.ExecuteStoreCommand( string.Format( "DELETE FROM {0}", dbTable ) );
		}

		public void ImportFileData( Stream fStream, int fileLength )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );

			DeleteAllObjects( "[SeriesOptionAttributes]" );
			DeleteAllObjects( "[SeriesTextAttributes]" );
			DeleteAllObjects( "[SeriesIntAttributes]" );
			DeleteAllObjects( "[AttributeOptions]" );
			DeleteAllObjects( "[Attributes]" );
			DeleteAllObjects( "[Serieses]" );
			DeleteAllObjects( "[Categories]" );

			while( csvReader.ReadNextRecord() )
			{
				Series sData = new Series();
				sData.CreatedDate = DateTime.Now;

				foreach( var header in csvReader.GetFieldHeaders() )
				{
					string val = csvReader[header];
					switch( header.ToLower() )
					{
						case "combo":
							// fields to skip
							break;
						case "category":
							{
								var cat = database.Categories.FirstOrDefault( c => c.Name == val );
								if( cat == null )
								{
									cat = new Category();
									cat.Name = val;
									database.Categories.AddObject( cat );
								}
								sData.Category = cat;
							}
							break;
						case "series name":
							sData.Name = val;
							break;
						case "value statement headline":
						case "marketing statement subheadline":
						case "marketing description":
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = (val ?? "").Any() ? val : (header + " goes here");
								attForSeries.Series = sData;
								database.SeriesTextAttributes.AddObject( attForSeries );
							}
							break;
						case "image - featured":
						case "image":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var img = database.ImageFiles.FirstOrDefault( i => i.Name == indVal );
									if( img != null )
									{
										SeriesImageFile sif = new SeriesImageFile();
										sif.IsFeatured = (header.ToLower() == "image - featured");
										sif.ImageFile = img;
										sif.Series = sData;
										database.SeriesImageFiles.AddObject( sif );
									}
								}
							}
							break;
						case "winning assortment":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == "ranking" );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = "Ranking";
									database.Attributes.AddObject( attData );
								}

								var attForSeries = new SeriesIntAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val.ToLower() == "yes" ? 2 : 1;
								attForSeries.Series = sData;
								database.SeriesIntAttributes.AddObject( attForSeries );
							}
							break;
						default:
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
									if( optVal == null )
									{
										if( indVal.Length > 100 )
										{
											throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
										}
										optVal = new AttributeOption();
										optVal.Name = indVal;
										database.AttributeOptions.AddObject( optVal );
										attData.AttributeOptions.Add( optVal );
									}

									var attForSeries = new SeriesOptionAttribute();
									attForSeries.Attribute = attData;
									attForSeries.AttributeOption = optVal;
									attForSeries.Series = sData;
									database.SeriesOptionAttributes.AddObject( attForSeries );
								}
							}
							break;
					}
				}
				database.Serieses.AddObject( sData );
				database.SaveChanges();
			}
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
				switch( cropLocation )
				{
					case "Left":
						startWidth = 0;
						break;
					case "Right":
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
				switch( cropLocation )
				{
					case "Top":
						startHeight = 0;
						break;
					case "Bottom":
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

		public class ImageSizeAndRatio
		{
			public string Description { get; set; }
			public string Suffix { get; set; }
			public int RatioHeight { get; set; }
			public int RatioWidth { get; set; }
			public int MaxHeight { get; set; }
			public int MaxWidth { get; set; }
		}

		static public List<ImageSizeAndRatio> ImageSizes = new List<ImageSizeAndRatio>()
		{
			new ImageSizeAndRatio() { Description = "Small 1:1", Suffix = "s1to1", RatioWidth = 1, RatioHeight = 1, MaxHeight = 200, MaxWidth = 200 },
			new ImageSizeAndRatio() { Description = "Small 3:4", Suffix = "s3to4", RatioWidth = 3, RatioHeight = 4, MaxHeight = 200, MaxWidth = 200 },
			new ImageSizeAndRatio() { Description = "Small 4:3", Suffix = "s4to3", RatioWidth = 4, RatioHeight = 3, MaxHeight = 200, MaxWidth = 200 },
			new ImageSizeAndRatio() { Description = "Small 16:9", Suffix = "s16to9", RatioWidth = 16, RatioHeight = 9, MaxHeight = 200, MaxWidth = 200 },
		};

		static public List<string> CropLocations = new List<string>()
		{
			"Center", "Top", "Left", "Right", "Bottom"
		};

		public void ImportImageFileData( string imageName, Stream fStream, int fileLength, string origFileName,
			string caption, bool hasPeople, string imageType, Dictionary<string, string> cropLocations )
		{
			DirectoryInfo dInfo = new DirectoryInfo( ConfigurationManager.AppSettings["ImageFileLocation"] );
			foreach( var f in dInfo.EnumerateFiles( "*" + imageName + "*.*" ).ToList() )
			{
				f.Delete();
			}

			Image fullSizeImg = Image.FromStream( fStream );
			fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], imageName + Path.GetExtension( origFileName ) ) );

			foreach( var iSize in ImageSizes )
			{
				ResizeImage( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"], imageName + "_" + iSize.Suffix + ".png" ),
					fullSizeImg, iSize.RatioWidth, iSize.RatioHeight, iSize.MaxWidth, iSize.MaxHeight, cropLocations[iSize.Suffix] );
			}

			var imgData = database.ImageFiles.FirstOrDefault( i => i.Name == imageName );
			if( imgData != null )
			{
				imgData.Caption = caption;
				imgData.HasPeople = hasPeople;
				imgData.ImageType = imageType;
			}
			else
			{
				imgData = new ImageFile();
				imgData.Name = imageName;
				imgData.OriginalExtension = Path.GetExtension( origFileName );
				imgData.Caption = caption;
				imgData.HasPeople = hasPeople;
				imgData.ImageType = imageType;
				database.ImageFiles.AddObject( imgData );
			}
			database.SaveChanges();
		}

		private bool ThumbnailCallback()
		{
			return false;
		}
	}
}
