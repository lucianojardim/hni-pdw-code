using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Dealers;
using System.Web;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace PWDRepositories
{
	public class DealerRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public DealerRepository()
		{
		}

		private DealerSummary ToDealerSummary( Dealer dealer )
		{
			return new DealerSummary()
			{
				DealerID = dealer.DealerID,
				Name = dealer.Name,
				URL = dealer.URL
			};
		}

		private DealerInformation ToDealerInformation( Dealer dealer )
		{
			return new DealerInformation()
			{
				DealerID = dealer.DealerID,
				Name = dealer.Name,
				URL = dealer.URL,
				MainContent = dealer.MainContent,
				FeaturedVideo = dealer.FeaturedVideoID,
				ProductHeadline = dealer.ProductsHeadline,
				PageHeadline = dealer.PagesHeadline,
				VideoHeadline = dealer.VideosHeadline,
				VideoList = dealer.VideoLinks.Select( v => v.VideoID ).ToList(),
				PageList = dealer.PageLinks.Select( p => p.PageID ).ToList(),
				ProductList = dealer.DealerFeaturedProducts.Select( p => p.SeriesName ).ToList()
			};
		}

		private DealerDetail ToDealerDetail( Dealer dealer )
		{
			return new DealerDetail()
			{
				DealerID = dealer.DealerID,
				Name = dealer.Name,
				URL = dealer.URL,
				MainContent = dealer.MainContent,
				FeaturedVideoLink = dealer.FeaturedVideo != null ? dealer.FeaturedVideo.YouTubeID : "",
				ProductHeadline = dealer.ProductsHeadline,
				PageHeadline = dealer.PagesHeadline,
				VideoHeadline = dealer.VideosHeadline,
				VideoLinks = dealer.VideoLinks.ToList().Select( v => new DealerDetail.VideoDetail()
				{
					Display = v.Display,
					VideoLink = v.YouTubeID
				} ),
				ProductList = dealer.DealerFeaturedProducts.Select( s => new DealerDetail.SeriesDetail()
				{
					SeriesName = s.SeriesName,
					FeaturedImageName = database.Serieses.First( f => f.Name == s.SeriesName ).FeaturedImageForSize( "m16to9" ).FileName
				} ),
				PageLinks = dealer.PageLinks.Select( p => new DealerDetail.PageDetail()
				{
					Display = p.Display,
					URL = p.URLLocation
				} ),
			};
		}

		public IEnumerable<DealerSummary> GetFullDealerList()
		{
			return database.Dealers
				.ToList()
				.OrderBy( d => d.Name )
				.Select( v => ToDealerSummary( v ) );
		}

		public IEnumerable<DealerSummary> GetFullDealerList( DealerTableParams param,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var dealerList = database.Dealers.AsQueryable();

			totalRecords = dealerList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				dealerList = dealerList.Where( i => 
					i.Name.Contains( param.sSearch ) ||
					i.URL.Contains( param.sSearch ) );
			}
			displayedRecords = dealerList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<Dealer> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = dealerList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = dealerList.OrderByDescending( v => v.Name );
					}
					break;
				case "url":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = dealerList.OrderBy( v => v.URL );
					}
					else
					{
						filteredAndSorted = dealerList.OrderByDescending( v => v.URL );
					}
					break;
			}

			if( (displayedRecords > param.iDisplayLength) && (param.iDisplayLength > 0) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToDealerSummary( v ) );
		}

		public DealerInformation GetDealer( int id )
		{
			var dealer = database.Dealers.FirstOrDefault( d => d.DealerID == id );
			if( dealer == null )
			{
				throw new Exception( "Dealer cannot be found." );
			}

			return ToDealerInformation( dealer );
		}

		public DealerDetail GetDealer( string url )
		{
			var dealer = database.Dealers.FirstOrDefault( d => d.URL == url );
			if( dealer == null )
			{
				throw new Exception( "Dealer cannot be found." );
			}

			return ToDealerDetail( dealer );
		}

		public bool AddDealer( DealerInformation dInfo )
		{
			if( database.Dealers.Any( d => d.URL == dInfo.URL ) )
			{
				throw new Exception( "Dealer with this URL already exists." );
			}
			var fVideo = database.VideoLinks.FirstOrDefault( v => v.VideoID == dInfo.FeaturedVideo );
			if( fVideo == null )
			{
				throw new Exception( "Featured Video cannot be found." );
			}

			Dealer newDealer = new Dealer();
			newDealer.Name = dInfo.Name;
			newDealer.URL = dInfo.URL;
			newDealer.MainContent = HttpUtility.HtmlDecode( dInfo.MainContent );
			newDealer.FeaturedVideo = fVideo;
			newDealer.ProductsHeadline = dInfo.ProductHeadline;
			newDealer.PagesHeadline = dInfo.PageHeadline;
			newDealer.VideosHeadline = dInfo.VideoHeadline;

			foreach( var v in dInfo.VideoList )
			{
				var aVideo = database.VideoLinks.FirstOrDefault( a => a.VideoID == v );
				if( aVideo != null )
				{
					newDealer.VideoLinks.Add( aVideo );
				}
			}

			foreach( var v in dInfo.PageList )
			{
				var aPLink = database.PageLinks.FirstOrDefault( a => a.PageID == v );
				if( aPLink != null )
				{
					newDealer.PageLinks.Add( aPLink );
				}
			}

			foreach( var p in dInfo.ProductList )
			{
				var aSeries = database.Serieses.FirstOrDefault( a => a.Name == p );
				if( aSeries != null )
				{
					newDealer.DealerFeaturedProducts.Add( new DealerFeaturedProduct() { SeriesName = aSeries.Name }  );
				}
			}

			database.Dealers.AddObject( newDealer );

			return database.SaveChanges() > 0;
		}

		public bool UpdateDealer( DealerInformation dInfo )
		{
			var dealer = database.Dealers.FirstOrDefault( d => d.DealerID == dInfo.DealerID );
			if( dealer == null )
			{
				throw new Exception( "Dealer cannot be found." );
			}
			if( database.Dealers.Any( d => d.URL == dInfo.URL && d.DealerID != dInfo.DealerID ) )
			{
				throw new Exception( "Dealer with this URL already exists." );
			}
			var fVideo = database.VideoLinks.FirstOrDefault( v => v.VideoID == dInfo.FeaturedVideo );
			if( fVideo == null )
			{
				throw new Exception( "Featured Video cannot be found." );
			}

			dealer.Name = dInfo.Name;
			dealer.URL = dInfo.URL;
			dealer.MainContent = HttpUtility.HtmlDecode( dInfo.MainContent );
			dealer.FeaturedVideo = fVideo;
			dealer.ProductsHeadline = dInfo.ProductHeadline;
			dealer.PagesHeadline = dInfo.PageHeadline;
			dealer.VideosHeadline = dInfo.VideoHeadline;

			dealer.VideoLinks.Clear();
			foreach( var v in dInfo.VideoList )
			{
				var aVideo = database.VideoLinks.FirstOrDefault( a => a.VideoID == v );
				if( aVideo != null )
				{
					dealer.VideoLinks.Add( aVideo );
				}
			}

			dealer.PageLinks.Clear();
			foreach( var p in dInfo.PageList )
			{
				var aPLink = database.PageLinks.FirstOrDefault( a => a.PageID == p );
				if( aPLink != null )
				{
					dealer.PageLinks.Add( aPLink );
				}
			}

			dealer.DealerFeaturedProducts.Clear();
			foreach( var p in dInfo.ProductList )
			{
				var aSeries = database.Serieses.FirstOrDefault( a => a.Name == p );
				if( aSeries != null )
				{
					dealer.DealerFeaturedProducts.Add( new DealerFeaturedProduct() { SeriesName = aSeries.Name } );
				}
			}

			return database.SaveChanges() > 0;
		}

		public bool DeleteDealer( int id )
		{
			var dealer = database.Dealers.FirstOrDefault( d => d.DealerID == id );
			if( dealer != null )
			{
				dealer.VideoLinks.Clear();
				dealer.PageLinks.Clear();
				dealer.DealerFeaturedProducts.Clear();

				database.Dealers.DeleteObject( dealer );

				return database.SaveChanges() > 0;
			}

			return false;
		}

		private void DeleteAllObjects( string dbTable )
		{
			database.ExecuteStoreCommand( string.Format( "DELETE FROM {0}", dbTable ) );
		}

		public bool ImportDealers( Stream fStream )
		{
			DeleteAllObjects( "[DealerVideoLinks]" );
			DeleteAllObjects( "[DealerPageLinks]" );
			DeleteAllObjects( "[DealerFeaturedProducts]" );
			DeleteAllObjects( "[Dealers]" );
			
			var csvReader = new CsvReader( new StreamReader( fStream ), true );
			while( csvReader.ReadNextRecord() )
			{
				if( !csvReader["Dealer Name"].Any() )
				{
					continue;
				}

				var dealer = new Dealer();

				foreach( var header in csvReader.GetFieldHeaders() )
				{
					string val = csvReader[header].Trim();
					switch( header.ToLower() )
					{
						case "dealer name":
							dealer.Name = val;
							break;
						case "url":
							dealer.URL = val ?? "";
							if( !dealer.URL.Any() )
							{
								throw new Exception( string.Format( "URL is required for {0}.", dealer.Name ) );
							}
							if( database.Dealers.Any( d => d.URL == dealer.URL && d.DealerID != dealer.DealerID ) )
							{
								throw new Exception( string.Format( "URL for {0} already exists.", dealer.Name ) );
							}

							break;
						case "main content":
							dealer.MainContent = val ?? "";
							break;
						case "featured video":
							{
								var fVideo = database.VideoLinks.FirstOrDefault( v => v.Display == val );
								if( fVideo != null )
								{
									dealer.FeaturedVideo = fVideo;
								}
								else
								{
									throw new Exception( string.Format( "Featured video cannot be found for {0}", dealer.Name ) );
								}
							}
							break;
						case "product list":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var p = database.Serieses.FirstOrDefault( i => i.Name == indVal );
									if( p != null )
									{
										if( !dealer.DealerFeaturedProducts.Any( dfp => dfp.SeriesName == p.Name ) )
										{
											dealer.DealerFeaturedProducts.Add( new DealerFeaturedProduct() { SeriesName = p.Name } );
										}
									}
								}
							}
							else
							{
								dealer.DealerFeaturedProducts.Clear();
							}
							break;
						case "video list":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var v = database.VideoLinks.FirstOrDefault( i => i.Display == indVal );
									if( v != null )
									{
										if( !dealer.VideoLinks.Any( dvl => dvl.VideoID == v.VideoID ) )
										{
											dealer.VideoLinks.Add( v );
										}
									}
								}
							}
							else
							{
								dealer.VideoLinks.Clear();
							}
							break;
						case "page list":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var p = database.PageLinks.FirstOrDefault( i => i.Display == indVal );
									if( p != null )
									{
										if( !dealer.PageLinks.Any( dpl => dpl.PageID == p.PageID ) )
										{
											dealer.PageLinks.Add( p );
										}
									}
								}
							}
							else
							{
								dealer.PageLinks.Clear();
							}
							break;
						case "product headline":
							dealer.ProductsHeadline = val;
							break;
						case "page headline":
							dealer.PagesHeadline = val;
							break;
						case "video headline":
							dealer.VideosHeadline = val;
							break;
					}
				}

				database.Dealers.AddObject( dealer );
			}

			return database.SaveChanges() > 0;
		}

		#region Dealer Video Functions
		private DealerVideoSummary ToDealerVideoSummary( VideoLink video )
		{
			return new DealerVideoSummary()
			{
				VideoID = video.VideoID,
				Name = video.Display
			};
		}

		private DealerVideoInformation ToDealerVideoInformation( VideoLink video )
		{
			return new DealerVideoInformation()
			{
				VideoID = video.VideoID,
				Name = video.Display,
				YouTubeID = video.YouTubeID
			};
		}

		public IEnumerable<DealerVideoSummary> GetFullVideoList()
		{
			return database.VideoLinks.ToList().Select( v => ToDealerVideoSummary( v ) );
		}

		public IEnumerable<DealerVideoSummary> GetFullVideoList( VideoTableParams param,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var videoList = database.VideoLinks.AsQueryable();

			totalRecords = videoList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				videoList = videoList.Where( i =>
					i.Display.Contains( param.sSearch ) );
			}
			displayedRecords = videoList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<VideoLink> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "display":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = videoList.OrderBy( v => v.Display );
					}
					else
					{
						filteredAndSorted = videoList.OrderByDescending( v => v.Display );
					}
					break;
			}

			if( (displayedRecords > param.iDisplayLength) && (param.iDisplayLength > 0) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToDealerVideoSummary( v ) );
		}

		public DealerVideoInformation GetDealerVideo( int id )
		{
			var video = database.VideoLinks.FirstOrDefault( d => d.VideoID == id );
			if( video == null )
			{
				throw new Exception( "Video cannot be found." );
			}

			return ToDealerVideoInformation( video );
		}

		public bool AddDealerVideo( DealerVideoInformation vInfo )
		{
			VideoLink newVideo = new VideoLink();
			newVideo.Display = vInfo.Name;
			newVideo.YouTubeID = vInfo.YouTubeID;

			database.VideoLinks.AddObject( newVideo );

			return database.SaveChanges() > 0;
		}

		public bool UpdateDealerVideo( DealerVideoInformation vInfo )
		{
			var video = database.VideoLinks.FirstOrDefault( d => d.VideoID == vInfo.VideoID );
			if( video == null )
			{
				throw new Exception( "Video cannot be found." );
			}

			video.Display = vInfo.Name;
			video.YouTubeID = vInfo.YouTubeID;

			return database.SaveChanges() > 0;
		}

		public bool DeleteDealerVideo( int id )
		{
			var video = database.VideoLinks.FirstOrDefault( d => d.VideoID == id );
			if( video != null )
			{
				video.Dealers.Clear();

				database.VideoLinks.DeleteObject( video );

				return database.SaveChanges() > 0;
			}

			return false;
		}
		#endregion

		#region Dealer Page Functions
		private DealerPageSummary ToDealerPageSummary( PageLink pageLink )
		{
			return new DealerPageSummary()
			{
				PageID = pageLink.PageID,
				Name = pageLink.Display,
				URLLocation = pageLink.URLLocation
			};
		}

		private DealerPageInformation ToDealerPageInformation( PageLink pageLink )
		{
			return new DealerPageInformation()
			{
				PageID = pageLink.PageID,
				Name = pageLink.Display,
				URLLocation = pageLink.URLLocation
			};
		}

		public IEnumerable<DealerPageSummary> GetFullPageList()
		{
			return database.PageLinks.ToList().Select( v => ToDealerPageSummary( v ) );
		}

		public IEnumerable<DealerPageSummary> GetFullPageList( DealerPageTableParams param,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var pageList = database.PageLinks.AsQueryable();

			totalRecords = pageList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				pageList = pageList.Where( i =>
					i.Display.Contains( param.sSearch ) );
			}
			displayedRecords = pageList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<PageLink> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "display":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = pageList.OrderBy( v => v.Display );
					}
					else
					{
						filteredAndSorted = pageList.OrderByDescending( v => v.Display );
					}
					break;
				case "url":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = pageList.OrderBy( v => v.URLLocation );
					}
					else
					{
						filteredAndSorted = pageList.OrderByDescending( v => v.URLLocation );
					}
					break;
			}

			if( (displayedRecords > param.iDisplayLength) && (param.iDisplayLength > 0) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToDealerPageSummary( v ) );
		}

		public DealerPageInformation GetDealerPage( int id )
		{
			var pageLink = database.PageLinks.FirstOrDefault( d => d.PageID == id );
			if( pageLink == null )
			{
				throw new Exception( "Page cannot be found." );
			}

			return ToDealerPageInformation( pageLink );
		}

		public bool AddDealerPage( DealerPageInformation vInfo )
		{
			PageLink newPage = new PageLink();
			newPage.Display = vInfo.Name;
			newPage.URLLocation = vInfo.URLLocation;

			database.PageLinks.AddObject( newPage );

			return database.SaveChanges() > 0;
		}

		public bool UpdateDealerPage( DealerPageInformation vInfo )
		{
			var pageLink = database.PageLinks.FirstOrDefault( d => d.PageID == vInfo.PageID );
			if( pageLink == null )
			{
				throw new Exception( "Page cannot be found." );
			}

			pageLink.Display = vInfo.Name;
			pageLink.URLLocation = vInfo.URLLocation;

			return database.SaveChanges() > 0;
		}

		public bool DeleteDealerPage( int id )
		{
			var pageLink = database.PageLinks.FirstOrDefault( d => d.PageID == id );
			if( pageLink != null )
			{
				pageLink.Dealers.Clear();

				database.PageLinks.DeleteObject( pageLink );

				return database.SaveChanges() > 0;
			}

			return false;
		}
		#endregion
	}
}
