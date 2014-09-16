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
			var currentUser = "Unknown";
			if( HttpContext.Current.User.Identity.IsAuthenticated )
			{
				currentUser = string.Format( "{0} [{1}]", PaoliWebUser.CurrentUser.FullName, PaoliWebUser.CurrentUser.EmailAddress );
			}

			( new ErrorEmailSender() ).SubmitErrorEmail( filterContext.Exception, currentUser, 
				filterContext.RequestContext.HttpContext.Request.Url.ToString(), filterContext.RequestContext.HttpContext.Request.Form );

			base.OnException( filterContext );
		}
	}
}