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

		public FileResult GetHiResImage( int imageId )
		{
			ImageRepository iRepo = new ImageRepository();

			var theData = iRepo.GetHiResImageInfo( imageId );

			return File( ConfigurationManager.AppSettings["ImageFileLocation"] + "/" + theData.FileName, theData.MIMEType, theData.FileName );
		}
	}
}
