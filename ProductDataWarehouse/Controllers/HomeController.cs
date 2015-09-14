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
			return View( viewName: "NewIndex" );
        }

		public static IEnumerable<UserContactInfo> GetHomeContacts()
		{
			using( var uRepo = new UserRepository() )
			{
				return uRepo.GetHeaderContacts( PaoliWebUser.CurrentUser.UserId );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public ActionResult ArticleListing( int? articleType )
		{
			return PartialView( articleType );
		}

		public static List<ArticleDisplayInfo> GetHomePageContent( int? articleType, out int outputType )
		{
			if( !articleType.HasValue )
			{
				articleType = ArticleInformation.ArticleTypes.NewsAndUpdates;
				if( PaoliWebUser.CurrentUser.CanSeeTheScoop )
					articleType = ArticleInformation.ArticleTypes.Scoop;
			}

			outputType = articleType.Value;

			using( var aRepo = new ArticleRepository() )
			{
				var contentArea = aRepo.GetMainArticleList( articleType.Value );
				if( contentArea != null )
				{
					return contentArea.ToList();
				}
			}

			return new List<ArticleDisplayInfo>();
		}

		public static string GetHomePageContentHTML()
 		{
			using( var iRepo = new ImportRepository() )
			{
				var contentArea = iRepo.GetHomePageContent();
				if( contentArea != null )
				{
					return contentArea.ContentArea;
				}
			}

			return "";
 		}

		[PaoliAuthorize( "CanViewLeadTimes" )]
		[TempPasswordCheck]
		public ActionResult LeadTimes()
		{
			using( var iRepo = new ImportRepository() )
			{
				return View( iRepo.GetLeadTimeSummary() );
			}
		}

		[PaoliAuthorize( "CanManageLeadTimes" )]
		[TempPasswordCheck]
		public ActionResult EditLeadTimes()
		{
			using( var iRepo = new ImportRepository() )
			{
				return View( iRepo.GetLeadTimeInformation() );
			}
		}

		[PaoliAuthorize( "CanManageLeadTimes" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		[HttpPost]
		public ActionResult EditLeadTimes( LeadTimeInformation info )
		{
			if( ModelState.IsValid )
			{
				using( var iRepo = new ImportRepository() )
				{
					iRepo.UpdateLeadTimes( info );
				}

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
