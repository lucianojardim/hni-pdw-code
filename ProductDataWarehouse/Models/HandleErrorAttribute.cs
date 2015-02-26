using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.EmailSenders;
using PDWInfrastructure;

namespace ProductDataWarehouse.Models
{
	public class HandlePaoliErrorAttribute : HandleErrorAttribute
	{
		public override void OnException( ExceptionContext filterContext )
		{
			if( filterContext.ExceptionHandled )
			{
				return;
			}

			var currentUser = "Unknown";
			if( HttpContext.Current.User.Identity.IsAuthenticated )
			{
				currentUser = string.Format( "{0} [{1}]", PaoliWebUser.CurrentUser.FullName, PaoliWebUser.CurrentUser.EmailAddress );
			}

			( new ErrorEmailSender() ).SubmitErrorEmail( filterContext.Exception, currentUser, 
				filterContext.RequestContext.HttpContext.Request.Url.ToString(), filterContext.RequestContext.HttpContext.Request.Form );

			if( filterContext.HttpContext.Request.IsAjaxRequest() )
			{
				string jsoncallback = ( filterContext.RouteData.Values["callback"] as string ) ?? filterContext.HttpContext.Request["callback"];
				if( !string.IsNullOrEmpty( jsoncallback ) )
				{
					var response = filterContext.HttpContext.Response;
					response.ContentType = "application/x-javascript";
					response.Write( string.Format( "{0}({{ success: false }});", jsoncallback ) );
				}
				else
				{
					filterContext.Result = new JsonResult
					{
						Data = new
						{
							success = false
						},
						JsonRequestBehavior = JsonRequestBehavior.AllowGet
					};
				}
				filterContext.ExceptionHandled = true;
			}
			else
			{
				base.OnException( filterContext );
			}
		}
	}
}