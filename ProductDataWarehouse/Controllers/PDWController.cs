using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;
using System.Configuration;

namespace ProductDataWarehouse.Controllers
{
    public class PDWController : Controller
    {
		public class JsonpResult : JsonResult
		{
			public override void ExecuteResult( ControllerContext context )
			{
				if( context == null )
				{
					throw new ArgumentNullException( "context" );
				}
				var request = context.HttpContext.Request;
				var response = context.HttpContext.Response;
				string jsoncallback = (context.RouteData.Values["callback"] as string) ?? request["callback"];
				if( !string.IsNullOrEmpty( jsoncallback ) )
				{
					if( string.IsNullOrEmpty( base.ContentType ) )
					{
						base.ContentType = "application/x-javascript";
					}
					response.Write( string.Format( "{0}(", jsoncallback ) );
				}
				base.ExecuteResult( context );
				if( !string.IsNullOrEmpty( jsoncallback ) )
				{
					response.Write( ")" );
				}
			}
		}

		public JsonpResult GetSeriesList( string category )
        {
			SeriesRepository sRepo = new SeriesRepository();

			var theList = sRepo.GetSeriesData( category );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
        }

		public JsonpResult GetTypicalList( string category, int? seriesId, string footprints,
			int? minPrice, int? maxPrice,
			string sortBy, int? pageNum = 1, int? pageSize = 28 )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theList = tRepo.GetTypicalData( category, seriesId, footprints,
				minPrice, maxPrice,
				sortBy, pageNum.Value, pageSize.Value );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalDetailList( string category, int? seriesId, string footprints,
			int? minPrice, int? maxPrice,
			string sortBy, int? pageNum = 1, int? pageSize = 28 )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theList = tRepo.GetTypicalDetailData( category, seriesId, footprints,
				minPrice, maxPrice,
				sortBy, pageNum.Value, pageSize.Value );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalCoverList( string category, int? seriesId, string footprints,
			int? minPrice, int? maxPrice,
			string sortBy, int? typicalId, string itemList )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theList = tRepo.GetTypicalCoverData( category, seriesId, footprints,
				minPrice, maxPrice,
				sortBy, typicalId, itemList );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetSeriesNameList()
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theList = sRepo.GetSeriesNameList();

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetSeriesNameListWithTypicals()
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theList = sRepo.GetSeriesNameListWithTypicals();

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetOptionList( string attr )
		{
			AttributeRepository aRepo = new AttributeRepository();

			var theList = aRepo.GetOptionList( attr );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalOptionList( string attr )
		{
			AttributeRepository aRepo = new AttributeRepository();

			var theList = aRepo.GetTypicalOptionList( attr );

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalPriceRange()
		{
			AttributeRepository aRepo = new AttributeRepository();

			var theList = aRepo.GetTypicalPriceRange();

			return new JsonpResult()
			{
				Data = theList,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetSeriesInfo( int id )
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theData = sRepo.GetSeriesInfo( id: id );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetSeriesInfoByName( string seriesName )
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theData = sRepo.GetSeriesInfo( seriesName: seriesName );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalInfo( int id )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theData = tRepo.GetTypicalInfo( id: id );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetTypicalInfoByName( string typicalName )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theData = tRepo.GetTypicalInfo( typicalName: typicalName );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetFullSeriesImageList( int id )
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theData = sRepo.GetFullSeriesImageList( id );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetImageFormatList()
		{
			return new JsonpResult()
			{
				Data = ImageRepository.ImageTypes,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult GetImageThumbnailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords, int? pageNum = 1, int? pageSize = 28 )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.GetImageThumbnailList( categories, imageTypes, seriesId, sortBy, keywords, pageNum.Value, pageSize.Value );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};

		}

		public JsonpResult GetImageDetailList( string categories, string imageTypes, int? seriesId, string sortBy, string keywords, int? pageNum = 1, int? pageSize = 28 )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.GetImageDetailList( categories, imageTypes, seriesId, sortBy, keywords, pageNum.Value, pageSize.Value );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};

		}

		public JsonpResult GetImageDetailInfo( int imageId )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.GetImageDetailInfo( imageId );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}
		
		public JsonpResult SearchSeries( string searchText )
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theData = sRepo.Search( searchText );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult SearchImages( string searchText )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.Search( searchText );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult SearchTypicals( string searchText )
		{
			TypicalRepository tRepo = new TypicalRepository();

			var theData = tRepo.Search( searchText );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public JsonpResult LogSearchResults( string searchText, int seriesCount, int imageCount, int typicalCount, int pageCount )
		{
			ImportRepository iRepo = new ImportRepository();

			return new JsonpResult()
			{
				Data = iRepo.LogSearchResults( searchText, seriesCount, imageCount, typicalCount, pageCount ),
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public FileResult GetHiResImage( int imageId )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.GetHiResImageInfo( imageId );

			return File( ConfigurationManager.AppSettings["ImageFileLocation"] + "/" + theData.FileName, theData.MIMEType, theData.FileName );
		}

		public JsonpResult GetSeriesPriceList()
		{
			SeriesRepository sRepo = new SeriesRepository();

			return new JsonpResult()
			{
				Data = sRepo.GetSeriesTextData( new List<string>() { "Price List" } ),
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}

		public ActionResult APIDef()
        {
//			if( (Request.ServerVariables["SERVER_NAME"].ToLower().Contains( "matt3400" )) ||
//				(Request.ServerVariables["SERVER_NAME"].ToLower().Contains( "localhost" )) )
			{
				return View();
			}

			throw new HttpException( 404, "HTTP/1.1 404 Not Found" );
//			return HttpNotFound();	// this is a better answer, but keeps returning an empty page
        }

		public class APIDefinition
		{
			public string url { get; set; }
			public string parameters { get; set; }
			public string notes { get; set; }
		}

		public static IEnumerable<APIDefinition> APIDefinitions = new List<APIDefinition>()
		{
			new APIDefinition() { url = "/PDW/GetSeriesList", parameters = "{\"category\":\"\"}", notes = "category is optional" },
			new APIDefinition() { url = "/PDW/GetTypicalList", parameters = "{\"category\":\"\",\"seriesId\":0,\"footprints\":\"\",\"minPrice\":0,\"maxPrice\":0,\"sortBy\":\"\",\"pageNum\":0,\"pageSize\":0,}", notes = "all fields are optional" },
			new APIDefinition() { url = "/PDW/GetTypicalDetailList", parameters = "{\"category\":\"\",\"seriesId\":0,\"footprints\":\"\",\"minPrice\":0,\"maxPrice\":0,\"sortBy\":\"\",\"pageNum\":0,\"pageSize\":0,}", notes = "all fields are optional" },
			new APIDefinition() { url = "/PDW/GetTypicalCoverList", parameters = "{\"category\":\"\",\"seriesId\":0,\"footprints\":\"\",\"minPrice\":0,\"maxPrice\":0,\"sortBy\":\"\",\"typicalId\":0,\"itemList\":\"\",}", notes = "all fields are optional" },
			new APIDefinition() { url = "/PDW/GetSeriesNameList", parameters = "" },
			new APIDefinition() { url = "/PDW/GetSeriesNameListWithTypicals", parameters = "" },
			new APIDefinition() { url = "/PDW/GetOptionList", parameters = "{\"attr\":\"\"" },
			new APIDefinition() { url = "/PDW/GetTypicalOptionList", parameters = "{\"attr\":\"\"" },
			new APIDefinition() { url = "/PDW/GetTypicalPriceRange", parameters = "" },
			new APIDefinition() { url = "/PDW/GetSeriesInfo", parameters = "{\"id\":0}" },
			new APIDefinition() { url = "/PDW/GetSeriesInfoByName", parameters = "{\"seriesName\":\"\"}" },
			new APIDefinition() { url = "/PDW/GetTypicalInfo", parameters = "{\"id\":0}" },
			new APIDefinition() { url = "/PDW/GetTypicalInfoByName", parameters = "{\"typicalName\":\"\"}" },
			new APIDefinition() { url = "/PDW/GetFullSeriesImageList", parameters = "{\"id\":0}" },
			new APIDefinition() { url = "/PDW/GetImageFormatList", parameters = "" },
			new APIDefinition() { url = "/PDW/GetImageThumbnailList", parameters = "{\"categories\":\"\",\"seriesId\":0,\"imageTypes\":\"\",\"keywords\":\"\",\"sortBy\":\"\",\"pageNum\":0,\"pageSize\":0,}", notes = "all fields are optional" },
			new APIDefinition() { url = "/PDW/GetImageDetailList", parameters = "{\"categories\":\"\",\"seriesId\":0,\"imageTypes\":\"\",\"keywords\":\"\",\"sortBy\":\"\",\"pageNum\":0,\"pageSize\":0,}", notes = "all fields are optional" },
			new APIDefinition() { url = "/PDW/GetImageDetailInfo", parameters = "{\"imageId\":0}" },
			new APIDefinition() { url = "/PDW/SearchSeries", parameters = "{\"searchText\":\"\"}" },
			new APIDefinition() { url = "/PDW/SearchImages", parameters = "{\"searchText\":\"\"}" },
			new APIDefinition() { url = "/PDW/SearchTypicals", parameters = "{\"searchText\":\"\"}" },
//			new APIDefinition() { url = "/PDW/LogSearchResults", parameters = "{\"searchText\":\"\",\"seriesCount\":0,\"imageCount\":0,\"typicalCount\":0,\"pageCount\":0}" },
			new APIDefinition() { url = "/PDW/GetSeriesPriceList", parameters = "" },
		};
	}
}
