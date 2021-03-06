﻿using System;
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

			using( var aRepository = new ArticleRepository() )
			{
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
				using( var aRepository = new ArticleRepository() )
				{
					aRepository.AddArticle( aInfo );

					return RedirectToAction( "Manage" );
				}
			}

			return View( aInfo );
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			using( var aRepository = new ArticleRepository() )
			{
				return View( aRepository.GetArticle( id ) );
			}
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
				using( var aRepository = new ArticleRepository() )
				{
					aRepository.EditArticle( aInfo );

					return RedirectToAction( "Manage" );
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
			using( var aRepository = new ArticleRepository() )
			{
				bool bSuccess = aRepository.DeleteArticle( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult View( int id )
		{
			using( var aRepository = new ArticleRepository() )
			{
				var details = aRepository.GetArticleViewer( id );
				switch( details.ArticleType )
				{
					case ArticleInformation.ArticleTypes.Scoop:
						ViewBag.HeaderArea = "TheScoop";
						break;
					case ArticleInformation.ArticleTypes.NewsAndUpdates:
						ViewBag.HeaderArea = "NewsUpdates";
						break;
					case ArticleInformation.ArticleTypes.Internal:
						ViewBag.HeaderArea = "InternalNews";
						break;
				}

				return View( details );
			}
		}

		[PaoliAuthorize( "IsPaoliUser" )]
		[TempPasswordCheck]
		public ActionResult ViewAllInternal( string search )
		{
			using( var aRepository = new ArticleRepository() )
			{
				ViewBag.PageTitle = "Here's your Member news";
				ViewBag.HeaderArea = "InternalNews";
				ViewBag.SearchText = search;

				return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.Internal, search ) );
			}
		}

		[PaoliAuthorize( "CanSeeTheScoop" )]
		[TempPasswordCheck]
		public ActionResult ViewAllScoop( string search )
		{
			using( var aRepository = new ArticleRepository() )
			{
				ViewBag.PageTitle = "Here's the Scoop";
				ViewBag.HeaderArea = "TheScoop";
				ViewBag.SearchText = search;

				return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.Scoop, search ) );
			}
		}

		[PaoliAuthorize( "CanSeeNewsUpdates" )]
		[TempPasswordCheck]
		public ActionResult ViewAllNewsAndUpdates( string search )
		{
			using( var aRepository = new ArticleRepository() )
			{
				ViewBag.PageTitle = "Here's your News and Updates";
				ViewBag.HeaderArea = "NewsUpdates";
				ViewBag.SearchText = search;

				return View( viewName: "ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.NewsAndUpdates, search ) );
			}
		}

		[PaoliAuthorize( "CanManageArticles" )]
		[TempPasswordCheck]
		public ActionResult Preview()
		{
			if( Session["PreviewArticle"] == null )
			{
				return RedirectToAction( "Manage" );
			}

			using( var aRepository = new ArticleRepository() )
			{
				return View( "View", aRepository.GetArticlePreview( (ArticleInformation)Session["PreviewArticle"] ) );
			}
		}

		public static IEnumerable<SelectListItem> GetArticleTypeList( bool includeBlank = false )
		{
			var theList = new List<SelectListItem>();

			if( !PaoliWebUser.CurrentUser.IsSuperAdmin )
			{
				theList.AddRange(
					ArticleInformation.ArticleTypes.ArticleTypeList
						.Where( a => a.Key == ArticleInformation.ArticleTypes.Internal )
						.Select( a => new SelectListItem() { Text = a.Value, Value = a.Key.ToString() } )
				);
			}
			else
			{
				if( includeBlank )
				{
					theList.Add( new SelectListItem() { Text = "All", Value = "0", Selected = true } );
				}

				theList.AddRange(
					ArticleInformation.ArticleTypes.ArticleTypeList
						.Select( a => new SelectListItem() { Text = a.Value, Value = a.Key.ToString() } )
				);
			}
			return theList;
		}

		public static IEnumerable<SelectListItem> GetArticleRankList()
		{
			var theList = ArticleInformation.ArticleRanks.ArticleRankList.Select( a => new SelectListItem() { Text = a.Value, Value = a.Key.ToString() } ).ToList();
			return theList;
		}
	}
}
