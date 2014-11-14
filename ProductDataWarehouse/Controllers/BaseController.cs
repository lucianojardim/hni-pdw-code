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

			if( url.Contains( "localhost" ) )
				return "http://localhost:8787";
			else if( url.Contains( "matt3400" ) )
				return "http://matt3400.wdd.local:8787";
			else if( url.Contains( "jamesburnes" ) || url.Contains( "getvitaminj" ) )
				return "http://desktop.paoli.getvitaminj.com";
			else if( url.Contains( "paoli-test01" ) )
				return "http://paoli-test01";

			return "http://desktop.paoli.com";
		}

		public string FullSiteURLLocal()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

			if( url.Contains( "localhost" ) )
				return "http://localhost:8787";
			else if( url.Contains( "matt3400" ) )
				return "http://matt3400.wdd.local:8787";
			else if( url.Contains( "jamesburnes" ) || url.Contains( "getvitaminj" ) )
				return "http://paoli-test01";
			else if( url.Contains( "paoli-test01" ) )
				return "http://paoli-test01";

			return "http://desktop.paoli.com";
		}

		public string MyPaoliURLLocal()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

			if( url.Contains( "localhost" ) )
				return "http://localhost:13801";
			else if( url.Contains( "matt3400" ) )
				return "http://matt3400.wdd.local:8780";
			else if( url.Contains( "jamesburnes" ) || url.Contains( "getvitaminj" ) )
				return "http://paoli-test01:8780";
			else if( url.Contains( "paoli-test01" ) )
				return "http://paoli-test01:8780";

			return "http://my.paoli.com";
		}

		public string MyPaoliURL()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

			if( url.Contains( "localhost" ) )
				return "http://localhost:13801";
			else if( url.Contains( "matt3400" ) )
				return "http://matt3400.wdd.local:8780";
			else if( url.Contains( "jamesburnes" ) || url.Contains( "getvitaminj" ) )
				return "http://my.paoli.getvitaminj.com";
			else if( url.Contains( "paoli-test01" ) )
				return "http://paoli-test01:8780";

			return "http://my.paoli.com";
		}

	}
}
