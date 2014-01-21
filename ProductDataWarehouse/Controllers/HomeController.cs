using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;

namespace ProductDataWarehouse.Controllers
{
	public class HomeController : BaseController
    {
        //
        // GET: /Home/

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public ActionResult Index()
        {
			ViewBag.RSSURL = FullSiteURLLocal() + "/";
			ViewBag.SiteURL = FullSiteURL() + "/";
			ViewBag.PaoliRepContacts = ( new UserRepository() ).GetPaoliRepContacts( PaoliWebUser.CurrentUser.UserId );

			return View();
        }
    }
}
