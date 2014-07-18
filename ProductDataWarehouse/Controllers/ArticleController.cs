using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.Attributes;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Articles;

namespace ProductDataWarehouse.Controllers
{
    public class ArticleController : BaseController
    {
		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public JsonResult FullArticleList( ArticleTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			ArticleRepository aRepository = new ArticleRepository();

			var results = aRepository.GetFullArticleList(
				param, out totalCount, out filteredCount );

			return Json( new
			{
				sEcho = param.sEcho,
				iTotalRecords = totalCount,
				iTotalDisplayRecords = filteredCount,
				aaData = results
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult Add( ArticleInformation aInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ArticleRepository aRepository = new ArticleRepository();

					aRepository.AddArticle( aInfo );

					return RedirectToAction( "Manage" );
				}
				catch( Exception ex )
				{
					ModelState.AddModelError( "", ex.Message );
					if( ex.InnerException != null )
					{
						ModelState.AddModelError( "", ex.InnerException.Message );
					}
				}

			}

			return View( aInfo );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			return View( ( new ArticleRepository() ).GetArticle( id ) );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult Edit( ArticleInformation aInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ArticleRepository aRepository = new ArticleRepository();

					aRepository.EditArticle( aInfo );

					return RedirectToAction( "Manage" );
				}
				catch( Exception ex )
				{
					ModelState.AddModelError( "", ex.Message );
					if( ex.InnerException != null )
					{
						ModelState.AddModelError( "", ex.InnerException.Message );
					}
				}

			}

			return View( aInfo );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateInput( false )]
		public JsonResult SavePreview( ArticleInformation aInfo )
		{
			Session["PreviewArticle"] = aInfo;

			return Json( new
			{
				success = true
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public JsonResult Delete( int id )
		{
			ArticleRepository aRepository = new ArticleRepository();

			bool bSuccess = aRepository.DeleteArticle( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult View( int id )
		{
			ArticleRepository aRepository = new ArticleRepository();

			return View( aRepository.GetArticleViewer( id ) );
		}

		[PaoliAuthorize( "IsPaoliUser" )]
		[TempPasswordCheck]
		public ActionResult ViewAllInternal()
		{
			ArticleRepository aRepository = new ArticleRepository();

			ViewBag.PageTitle = "Here's your Paoli news";
			ViewBag.HeaderArea = "InternalNews";

			return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.Internal ) );
		}

		[PaoliAuthorize( "CanSeeTheScoop" )]
		[TempPasswordCheck]
		public ActionResult ViewAllScoop()
		{
			ArticleRepository aRepository = new ArticleRepository();

			ViewBag.PageTitle = "Here's the Scoop";
			ViewBag.HeaderArea = "TheScoop";

			return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.Scoop ) );
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult ViewAllNewsAndUpdates()
		{
			ArticleRepository aRepository = new ArticleRepository();

			ViewBag.PageTitle = "Here's your News and Updates";
			ViewBag.HeaderArea = "NewsUpdates";

			return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.NewsAndUpdates ) );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Preview()
		{
			if( Session["PreviewArticle"] == null )
			{
				return RedirectToAction( "Manage" );
			}

			return View( "View", (new ArticleRepository()).GetArticlePreview( (ArticleInformation)Session["PreviewArticle"] ) );
		}

		public static IEnumerable<SelectListItem> GetArticleTypeList( bool includeBlank = false )
		{
			var theList = new List<SelectListItem>();

			if( includeBlank )
			{
				theList.Add( new SelectListItem() { Text = "All", Value = "0", Selected = true } );
			}

			theList.AddRange(
				ArticleInformation.ArticleTypes.ArticleTypeList
					.Select( a => new SelectListItem() { Text = a.Value, Value = a.Key.ToString() } )
			);

			return theList;
		}

		public static IEnumerable<SelectListItem> GetArticleRankList()
		{
			var theList = ArticleInformation.ArticleRanks.ArticleRankList.Select( a => new SelectListItem() { Text = a.Value, Value = a.Key.ToString() } ).ToList();
			return theList;
		}
	}
}
