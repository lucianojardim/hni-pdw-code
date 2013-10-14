using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Dealers;

namespace ProductDataWarehouse.Controllers
{
    public class DealerController : Controller
    {
		public ActionResult Index( string id )
		{
			DealerRepository dRepository = new DealerRepository();

			var dInfo = dRepository.GetDealer( id );

			ViewBag.FullSiteURL = FullSiteURL();

			return View( dInfo );
		}

		public string FullSiteURL()
		{
			var url = Request.Url.DnsSafeHost.ToLower();

			if( url.Contains( "localhost" ) )
				return "http://localhost:8787";
			else if( url.Contains( "matt3400" ) )
				return "http://matt3400.wdd.local:8787";
			else if( url.Contains( "jamesburnes" ) || url.Contains( "getvitaminj" ) )
				return "http://library.paoli.getvitaminj.com";
			else if( url.Contains( "paoli-test01" ) )
				return "http://paoli-test01";

			return "http://www.paoli.com";
		}
    }
}
