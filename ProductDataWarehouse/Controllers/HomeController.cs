using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;

namespace ProductDataWarehouse.Controllers
{
	public class HomeController : BaseController
    {
        //
        // GET: /Home/

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public ActionResult Index()
        {
			ViewBag.RSSURL = FullSiteURL() + "/";

			return View();
        }
    }
}
