using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PDWInfrastructure.Attributes
{
	public class TempPasswordCheckAttribute : ActionFilterAttribute 
	{
		public override void OnActionExecuting( ActionExecutingContext filterContext )
		{
			if( PaoliWebUser.CurrentUser.Identity.IsAuthenticated )
			{
				if( PaoliWebUser.CurrentUser.HasTempPassword )
				{
					filterContext.Result = new RedirectResult( new UrlHelper( filterContext.HttpContext.Request.RequestContext ).Action( "ChangePW", "User" ) );
					return;
				}
			}

			base.OnActionExecuting( filterContext );
		}
	}
}
