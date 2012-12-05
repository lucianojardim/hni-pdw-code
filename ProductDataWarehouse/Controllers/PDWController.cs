﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;

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

		public JsonpResult GetSeriesInfo( int id )
		{
			SeriesRepository sRepo = new SeriesRepository();

			var theData = sRepo.GetSeriesInfo( id );

			return new JsonpResult()
			{
				Data = theData,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet
			};
		}
    }
}
