using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWInfrastructure.Attributes;

namespace ProductDataWarehouse.Controllers
{
	public class HomeController : BaseController
    {
        //
        // GET: /Home/

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult Index()
        {
			ViewBag.RSSURL = FullSiteURLLocal() + "/";
			ViewBag.SiteURL = FullSiteURL() + "/";
			ViewBag.PaoliRepContacts = ( new UserRepository() ).GetPaoliRepContacts( PaoliWebUser.CurrentUser.UserId );
			ViewBag.PaoliMemberContact = ( new UserRepository() ).GetPaoliMemberContact( PaoliWebUser.CurrentUser.UserId );

			return View();
        }
    }
}
