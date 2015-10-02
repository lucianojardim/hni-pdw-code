using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;
using PDWModels.Articles;

namespace ProductDataWarehouse.Controllers
{
    public class MemberNewsController : Controller
    {
		public ActionResult View( int id )
		{
			using( var aRepository = new ArticleRepository() )
			{
				ViewBag.HeaderArea = "MemberNews";

				return View( viewName: "../Article/View", model: aRepository.GetArticleViewer( id ) );
			}
		}

		public ActionResult Index( string search )
		{
			using( var aRepository = new ArticleRepository() )
			{
				ViewBag.PageTitle = "Here's your Member news";
				ViewBag.HeaderArea = "MemberNews";
				ViewBag.SearchText = search;

				return View( viewName: "../Article/ViewAll", model: aRepository.GetArticleViewList( ArticleInformation.ArticleTypes.Internal, search ) );
			}
		}
	}
}
