using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;

namespace ProductDataWarehouse.Models
{
	public class ECollateralVerifyAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore( System.Web.HttpContextBase httpContext )
		{
			if( !httpContext.User.Identity.IsAuthenticated )
				return false;

			var currentUser = PaoliWebUser.CurrentUser;

			var id = ( httpContext.Request.RequestContext.RouteData.Values["id"] as string ) ?? ( httpContext.Request["id"] as string );

			return ( new ECollateralRepository() ).VerifyURLPermission( id, currentUser );
		}
	}
}