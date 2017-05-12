using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProductDataWarehouse.Models;

namespace ProductDataWarehouse.Controllers
{
	[HandlePaoliError]
    public abstract class BaseController : Controller
    {
		public string FullSiteURL()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

            if (url.Contains("qa"))
                return "http://qa-desktop.paoli.com";
			else if( url.Contains( "localhost" ) )
				return "http://localhost:8787";
			else if( url.Contains( "192" ) )
				return string.Format( "http://{0}", url );

			return "http://desktop.paoli.com";
		}

		public string FullSiteURLLocal()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

			if( url.Contains( "qa" ) )
                return "http://qa-desktop.paoli.com";
			else if( url.Contains( "localhost" ) )
				return "http://localhost:8787";
			else if( url.Contains( "192" ) )
				return string.Format( "http://{0}", url );

			return "http://desktop.paoli.com";
		}

		public string MyPaoliURLLocal()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

            if (url.Contains("qa"))
                return "http://qa-my.paoli.com";
            else if (url.Contains("localhost"))
				return "http://localhost:13801";
			else if( url.Contains( "192" ) )
				return string.Format( "http://{0}:8780", url );

			return "http://my.paoli.com";
		}

		public string MyPaoliURL()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

            if (url.Contains("qa"))
                return "http://qa-my.paoli.com";
            else if (url.Contains("localhost"))
				return "http://localhost:13801";
			else if( url.Contains( "192" ) )
				return string.Format( "http://{0}:8780", url );

			return "http://my.paoli.com";
		}

	}
}
