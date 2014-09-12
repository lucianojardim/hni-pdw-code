using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PDWInfrastructure.Attributes
{
	public class PaoliAuthorizeAttribute : AuthorizeAttribute
	{
		private string PropertyName { get; set; }
		public PaoliAuthorizeAttribute( string propertyName )
		{
			PropertyName = propertyName;
		}

		protected override bool AuthorizeCore( System.Web.HttpContextBase httpContext )
		{
			if( !httpContext.User.Identity.IsAuthenticated )
				return false;

			var currentUser = PaoliWebUser.CurrentUser;
			
			var checkProperty = currentUser.GetType().GetProperty( PropertyName );
			if( checkProperty == null )
				return false;

			if( checkProperty.PropertyType == typeof(bool) )
			{
				return (bool)(checkProperty.GetValue( currentUser, new object[0] ));
			}

			return false;
		}

		protected override void HandleUnauthorizedRequest( AuthorizationContext filterContext )
		{
			if( filterContext.HttpContext.Request.IsAjaxRequest() )
            {
				var urlHelper = new UrlHelper( filterContext.RequestContext );
				filterContext.HttpContext.Response.StatusCode = 403;
				filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized",
                        LogOnUrl = "/Login" //urlHelper.Action("LogOn", "Account")
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
				base.HandleUnauthorizedRequest( filterContext );
            }
		}
	}
}
