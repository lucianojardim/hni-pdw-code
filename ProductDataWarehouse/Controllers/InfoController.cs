using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;
using PDWInfrastructure.Attributes;
using ProductDataWarehouse.Models;

namespace ProductDataWarehouse.Controllers
{
	public class InfoController : BaseController
    {
		[PaoliAuthorize( "CanBeLoggedIn" )]
		[ECollateralVerify]
		[TempPasswordCheck]
		public ActionResult Index( string id )
        {
			ECollateralRepository eRepository = new ECollateralRepository();

			try
			{
				return View( eRepository.GetItemDetails( id ) );
			}
			catch
			{
			}

			return RedirectToAction( "Index", "Home" );
		}

    }
}
