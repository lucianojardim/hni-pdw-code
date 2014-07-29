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

    }
}
