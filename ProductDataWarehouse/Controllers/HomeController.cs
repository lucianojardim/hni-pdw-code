using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWInfrastructure.Attributes;
using PDWModels.Users;
using PDWModels.Articles;
using PDWModels.LeadTimes;

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
			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewIndex" );
			}

			ViewBag.RSSURL = FullSiteURLLocal() + "/";
			ViewBag.SiteURL = FullSiteURL() + "/";
			ViewBag.PaoliRepContacts = ( new UserRepository() ).GetPaoliRepContacts( PaoliWebUser.CurrentUser.UserId );
			ViewBag.PaoliMemberContact = ( new UserRepository() ).GetPaoliMemberContact( PaoliWebUser.CurrentUser.UserId );

			return View();
        }

		public static IEnumerable<UserContactInfo> GetHomeContacts()
		{
			return ( new UserRepository() ).GetHeaderContacts( PaoliWebUser.CurrentUser.UserId );
		}

		public static List<ArticleDisplayInfo> GetHomePageContent()
		{
			var articleType = ArticleInformation.ArticleTypes.NewsAndUpdates;
			if( PaoliWebUser.CurrentUser.IsPaoliUser )
				articleType = ArticleInformation.ArticleTypes.Internal;
			else if( PaoliWebUser.CurrentUser.CanSeeTheScoop )
				articleType = ArticleInformation.ArticleTypes.Scoop;

			var contentArea = ( new ArticleRepository() ).GetMainArticleList( articleType );
			if( contentArea != null )
			{
				return contentArea.ToList();
			}

			return new List<ArticleDisplayInfo>();
		}

		public static string GetHomePageContentHTML()
 		{
			var contentArea = ( new ImportRepository() ).GetHomePageContent();
 			if( contentArea != null )
 			{
				return contentArea.ContentArea;
 			}
 
			return "";
 		}

		[PaoliAuthorize( "CanViewLeadTimes" )]
		[TempPasswordCheck]
		public ActionResult LeadTimes()
		{
			var iRepo = new ImportRepository();

			return View( iRepo.GetLeadTimeSummary() );
		}

		[PaoliAuthorize( "CanManageLeadTimes" )]
		[TempPasswordCheck]
		public ActionResult EditLeadTimes()
		{
			var iRepo = new ImportRepository();

			return View( iRepo.GetLeadTimeInformation() );
		}

		[PaoliAuthorize( "CanManageLeadTimes" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		[HttpPost]
		public ActionResult EditLeadTimes( LeadTimeInformation info )
		{
			if( ModelState.IsValid )
			{
				var iRepo = new ImportRepository();

				iRepo.UpdateLeadTimes( info );

				return RedirectToAction( "LeadTimes" );
			}

			return View( info );
		}

		public ActionResult Error()
		{
			return View();
		}
	}
}
