using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Data.Objects;
using System.Configuration;
using PDWModels.Images;
using System.Diagnostics;
using PDWModels;
using System.Text.RegularExpressions;
using PDWModels.LeadTimes;

namespace PWDRepositories
{
	public class ImportRepository : BaseRepository
	{
		public ImportRepository()
		{
		}

		private void DeleteAllObjects( string dbTable )
		{
			database.Database.ExecuteSqlCommand( string.Format( "DELETE FROM {0}", dbTable ) );
		}

		public void ImportFileData( Stream fStream, int fileLength )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );

			DeleteAllObjects( "[SeriesOptionAttributes]" );
			DeleteAllObjects( "[SeriesTextAttributes]" );
			DeleteAllObjects( "[SeriesIntAttributes]" );
			DeleteAllObjects( "[AttributeOptions]" );
			DeleteAllObjects( "[Attributes]" );
			DeleteAllObjects( "[SeriesImageFiles]" );
			DeleteAllObjects( "[RelatedSeries]" );
			DeleteAllObjects( "[SeriesTypicals]" );
			DeleteAllObjects( "[ImageFileSerieses]" );
			DeleteAllObjects( "[Serieses]" );
			DeleteAllObjects( "[Categories]" );

			Dictionary<int, string> mapRelatedSeries = new Dictionary<int, string>();

			while( csvReader.ReadNextRecord() )
			{
				if( !csvReader["Series Name"].Any() )
				{
					continue;
				}

				Series sData = new Series();
				sData.CreatedDate = DateTime.Now;
				sData.IsActive = true;

				string relatedSeries = "";
				List<string> arrKeywordList = new List<string>();

				foreach( var header in csvReader.GetFieldHeaders() )
				{
					string val = csvReader[header];
					switch( header.ToLower() )
					{
						case "combo":
						case "image-1":
						case "image-2":
						case "image-3":
							// fields to skip
							break;
						case "category":
							{
								var cat = database.Categories.FirstOrDefault( c => c.Name == val );
								if( cat == null )
								{
									cat = new Category();
									cat.Name = val;
									database.Categories.Add( cat );
								}
								sData.Category = cat;
							}
							break;
						case "series name":
							sData.Name = val;
							break;
						case "active":
						case "status":
							switch( val.ToLower() )
							{
								case "active":
								case "yes":
								case "true":
								case "1":
									sData.IsActive = true;
									break;
								default:
									sData.IsActive = false;
									break;
							}
							break;
						case "value statement headline":
						case "marketing statement subheadline":
						case "marketing description":
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute( false );
									attData.Name = header;
									database.Attributes.Add( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val ?? "";
								attForSeries.Series = sData;
								database.SeriesTextAttributes.Add( attForSeries );
							}
							break;
						case "image - featured":
						case "image":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								int displayOrder = 1;
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var img = database.ImageFiles.FirstOrDefault( i => i.Name == indVal );
									if( img != null )
									{
										SeriesImageFile sif = new SeriesImageFile();
										sif.IsFeatured = (header.ToLower() == "image - featured");
										sif.ImageFile = img;
										sif.Series = sData;
										sif.DisplayOrder = sif.IsFeatured ? 0 : displayOrder;
										database.SeriesImageFiles.Add( sif );

										displayOrder++;
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
									attData = new PDWDBContext.Attribute( false );
									attData.Name = "Ranking";
									database.Attributes.Add( attData );
								}

								var attForSeries = new SeriesIntAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val.ToLower() == "yes" ? 2 : 1;
								attForSeries.Series = sData;
								database.SeriesIntAttributes.Add( attForSeries );
							}
							break;
						case "related series":
							if( (val ?? "").Any() )
							{
								relatedSeries = val;
							}
							break;
						case "price list":
						case "product technical spec sheet":
						case "spec guide":
						case "gsa spec sheet":
						case "installation guide":
						case "eds sheet":
						case "leed sheet":
						case "control guide":
						case "brochure file name":
						case "keywords":
						case "in2 price list":
						case "interactive price list":
						case "government price list":
							if( ( val ?? "" ).Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute( false );
									attData.Name = header;
									database.Attributes.Add( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val;
								attForSeries.Series = sData;
								database.SeriesTextAttributes.Add( attForSeries );
							}
							break;
						case "pricing range":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute( false );
									attData.Name = header;
									database.Attributes.Add( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val;
								attForSeries.Series = sData;
								database.SeriesTextAttributes.Add( attForSeries );

								var startingPrice = string.Join( "", val.Where( c => Char.IsDigit( c ) ) );
								int price = 0;
								if( startingPrice.Any() && int.TryParse( startingPrice, out price ) )
								{
									var priceData = database.Attributes.FirstOrDefault( a => a.Name == "Starting Price" );
									if( priceData == null )
									{
										priceData = new PDWDBContext.Attribute( false );
										priceData.Name = "Starting Price";
										database.Attributes.Add( priceData );
									}

									var priceForSeries = new SeriesIntAttribute();
									priceForSeries.Attribute = priceData;
									priceForSeries.Value = price;
									priceForSeries.Series = sData;
									database.SeriesIntAttributes.Add( priceForSeries );
								}
							}
							break;
						case "claires 5 favorites":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute( true );
									attData.Name = header;
									database.Attributes.Add( attData );
								}
								var values = val.Split( '\r', '\n' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									if( indVal.Length > 0 )
									{
										var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
										if( optVal == null )
										{
											if( indVal.Length > 500 )
											{
												throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
											}
											optVal = new AttributeOption();
											optVal.Name = indVal;
											database.AttributeOptions.Add( optVal );
											attData.AttributeOptions.Add( optVal );
										}

										var attForSeries = new SeriesOptionAttribute();
										attForSeries.Attribute = attData;
										attForSeries.AttributeOption = optVal;
										attForSeries.Series = sData;
										database.SeriesOptionAttributes.Add( attForSeries );
									}
								}
							}
							break;
						default:
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute( true );
									attData.Name = header;
									database.Attributes.Add( attData );
								}
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ).Where( s => s.Any() ) )
								{
									var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
									if( optVal == null )
									{
										if( indVal.Length > 500 )
										{
											throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
										}
										optVal = new AttributeOption();
										optVal.Name = indVal;
										database.AttributeOptions.Add( optVal );
										attData.AttributeOptions.Add( optVal );
									}

									var attForSeries = new SeriesOptionAttribute();
									attForSeries.Attribute = attData;
									attForSeries.AttributeOption = optVal;
									attForSeries.Series = sData;
									database.SeriesOptionAttributes.Add( attForSeries );
								}
							}
							break;
					}

					switch( header.ToLower() )
					{
						case "series name":
						case "category":
						case "style":
						case "value statement headline":
						case "marketing statement subheadline":
						case "claire 5 favorites":
						case "iaq":
						case "finish":
						case "drawer options - configuration":
						case "edge options - profile":
						case "pull options - style":
						case "seating option - back":
						case "shape":
						case "casegood application":
						case "table application":
						case "seating application":
						case "keywords":
							arrKeywordList.Add( val.ToLower() );
							break;
					}
				}

				sData.DBKeywords = SearchText.GetKeywordList( arrKeywordList );

				database.Serieses.Add( sData );
				database.SaveChanges();
				database.Entry( sData ).Reload();

				if( relatedSeries.Any() )
				{
					mapRelatedSeries.Add( sData.SeriesID, relatedSeries );
				}
			}

			foreach( int seriesId in mapRelatedSeries.Keys )
			{
				var parentSeries = database.Serieses.FirstOrDefault( s => s.SeriesID == seriesId );
				if( parentSeries != null )
				{
					var otherSeries = mapRelatedSeries[seriesId].Split( ',' );
					foreach( var seriesName in otherSeries.Select( o => o.Trim() ) )
					{
						var rSeries = database.Serieses.FirstOrDefault( s => s.Name == seriesName );
						if( rSeries != null )
						{
							rSeries.ParentSerieses.Add( parentSeries );
						}
						else
						{
							Debug.WriteLine( format: "Unable to find series: {0}", args: seriesName );
						}
					}
				}
			}

			foreach( var tData in database.Typicals )
			{
				var rSeries = database.Serieses.FirstOrDefault( s => s.Name == tData.FeaturedSeries );
				tData.FeaturedSeries = rSeries.Name;
				if( rSeries != null )
				{
					SeriesTypical stData = new SeriesTypical();
					stData.IsPrimary = true;
					stData.Series = rSeries;
					stData.Typical = tData;
					database.SeriesTypicals.Add( stData );

					tData.FeaturedSeries = rSeries.Name;
				}

				if( tData.SeriesList != null )
				{
					foreach( var indVal in tData.SeriesList.Split( ',' ).Select( s => s.Trim() ) )
					{
						var oSeries = database.Serieses.FirstOrDefault( s => s.Name == indVal );
						if( oSeries != null )
						{
							SeriesTypical stData = new SeriesTypical();
							stData.IsPrimary = false;
							stData.Series = oSeries;
							stData.Typical = tData;
							database.SeriesTypicals.Add( stData );
						}
					}
				}
				tData.SeriesList = string.Join( ", ", tData.SeriesTypicals.Where( st => !st.IsPrimary ).Select( st => st.Series.Name ) );

				tData.DBKeywords = SearchText.GetKeywordList( new List<string>() { tData.Name, tData.FeaturedSeries, tData.Notes, tData.SeriesList } );
			}

			foreach( var imgData in database.ImageFiles )
			{
				var rSeries = database.Serieses.FirstOrDefault( s => s.Name == imgData.FeaturedSeries );
				imgData.FeaturedSeries = rSeries.Name;
				if( rSeries != null )
				{
					ImageFileSeries stData = new ImageFileSeries();
					stData.IsFeatured = true;
					stData.Series = rSeries;
					stData.ImageFile = imgData;
					database.ImageFileSerieses.Add( stData );

					imgData.FeaturedSeries = rSeries.Name;
				}

				if( imgData.SeriesList != null )
				{
					foreach( var indVal in imgData.SeriesList.Split( ',' ).Select( s => s.Trim() ) )
					{
						var oSeries = database.Serieses.FirstOrDefault( s => s.Name == indVal );
						if( oSeries != null )
						{
							ImageFileSeries stData = new ImageFileSeries();
							stData.IsFeatured = false;
							stData.Series = oSeries;
							stData.ImageFile = imgData;
							database.ImageFileSerieses.Add( stData );
						}
					}
				}
				imgData.SeriesList = string.Join( ", ", imgData.ImageFileSerieses.Where( st => !st.IsFeatured ).Select( st => st.Series.Name ) );

				imgData.DBKeywords = SearchText.GetKeywordList( new List<string>() { imgData.Name, imgData.Caption, imgData.Keyword, imgData.FeaturedSeries, imgData.SeriesList } );
			}

			database.SaveChanges();
		}

		public bool LogSearchResults( string searchText, int seriesCount, int imageCount, int typicalCount, int pageCount )
		{
			SearchResultsLog srl = new SearchResultsLog();

			srl.SearchDateTime = DateTime.UtcNow;
			srl.SearchTerm = searchText;
			srl.SeriesCount = seriesCount;
			srl.ImageCount = imageCount;
			srl.TypicalCount = typicalCount;
			srl.PageCount = pageCount;
			database.SearchResultsLogs.Add( srl );

			return (database.SaveChanges() > 0);
		}

		public IEnumerable<SearchResults> GetSearchLogList()
		{
			return database.SearchResultsLogs
				.OrderByDescending( s => s.LogID )
				.ToList()
				.Select( srl => new SearchResults()
				{
					SearchDateTime = srl.SearchDateTime,
					SearchText = srl.SearchTerm,
					SeriesCount = srl.SeriesCount,
					ImageCount = srl.ImageCount,
					TypicalCount = srl.TypicalCount,
					PageCount = srl.PageCount,
				} );
		}

		public void RebuildDBKeywords()
		{/*
			List<string> arrSeriesAttributes = new List<string>() { 
				"Style",
				"Claire 5 Favorites",
				"IAQ",
				"Finish",
				"Drawer Options - Configuration",
				"Edge Options - Profile",
				"Pull Options - Style",
				"Seating Option - Back",
				"Shape",
				"Casegood Application",
				"Table Application",
				"Seating Application",
				"Value Statement Headline",
				"Marketing Statement Subheadline",
				"Keywords"
			};

			foreach( var s in database.Serieses )
			{
				List<string> keywords = new List<string>()
					{ 
						s.Name,
						s.Category.Name 
					};

				keywords.AddRange( s.SeriesOptionAttributes.Where( a => arrSeriesAttributes.Contains( a.Attribute.Name ) ).Select( o => o.AttributeOption.Name ) );
				keywords.AddRange( s.SeriesTextAttributes.Where( a => arrSeriesAttributes.Contains( a.Attribute.Name ) ).Select( o => o.Value ) );

				s.DBKeywords = SearchText.GetKeywordList( keywords );
			}

			List<string> arrTypicalAttributes = new List<string>() { 
				"Other Series Shown",
				"Keywords"
			};
*/
			foreach( var t in database.Typicals )
			{
				List<string> keywords = new List<string>()
					{ 
						t.Name,
						t.FeaturedSeries,
						t.SeriesList,
						t.Notes
					};

				t.DBKeywords = SearchText.GetKeywordList( keywords );
			}
			/*
			foreach( var i in database.ImageFiles )
			{
				List<string> keywords = new List<string>()
					{ 
						i.Name,
						i.Caption,
						i.Keyword,
					};

				i.DBKeywords = SearchText.GetKeywordList( keywords );
			}
			*/
			database.SaveChanges();
		}

		public HomePageContentInformation GetHomePageContent()
		{
			var content = database.HomePageContents.FirstOrDefault();
			if( content != null )
			{
				return new HomePageContentInformation() { ContentArea = content.ContentData };
			}

			return new HomePageContentInformation();
		}

		public bool UpsertHomePageContent( HomePageContentInformation content )
		{
			var dbContent = database.HomePageContents.FirstOrDefault();
			if( dbContent != null )
			{
				dbContent.ContentData = content.ContentArea;
			}
			else
			{
				database.HomePageContents.Add( new HomePageContent() { ContentData = content.ContentArea } );
			}

			return database.SaveChanges() > 0;
		}

		public LeadTimeSummary GetLeadTimeSummary()
		{
			var detail = database.LeadTimeDetails.FirstOrDefault();
			if( detail != null )
			{
				return new LeadTimeSummary()
				{
					PromoHeadline = detail.Headline,
					PromoText = detail.PromoText,
					UserImage = detail.UserID.HasValue ? detail.User.ImageFileName : null,
					UserName = detail.UserID.HasValue ? detail.User.FullName : null,
					UserEmail = detail.UserID.HasValue ? detail.User.Email : null,
					UserPhone = detail.UserID.HasValue ? detail.User.BusinessPhone : null,
					LeftSide = detail.LeftSide,
					RightSide = detail.RightSide
				};
			}

			return null;
		}

		public LeadTimeInformation GetLeadTimeInformation()
		{
			var detail = database.LeadTimeDetails.FirstOrDefault();
			if( detail != null )
			{
				return new LeadTimeInformation()
				{
					PromoHeadline = detail.Headline,
					PromoText = detail.PromoText,
					LeftSide = detail.LeftSide,
					RightSide = detail.RightSide,
					UserID = detail.UserID
				};
			}

			return new LeadTimeInformation();
		}

		public bool UpdateLeadTimes( LeadTimeInformation info )
		{
			var detail = database.LeadTimeDetails.FirstOrDefault();
			if( detail == null )
			{
				detail = new LeadTimeDetail();
				database.LeadTimeDetails.Add( detail );
			}

			detail.PromoText = info.PromoText;
			detail.Headline = info.PromoHeadline;
			detail.LeftSide = info.LeftSide;
			detail.RightSide = info.RightSide;
			detail.UserID = info.UserID;

			return database.SaveChanges() > 0;
		}
	}
}
