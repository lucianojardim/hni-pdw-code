using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Dealers;

namespace ProductDataWarehouse.Controllers
{
	public class DealerController : BaseController
    {
		public ActionResult Index( string id )
		{
			DealerRepository dRepository = new DealerRepository();

			try
			{
				var dInfo = dRepository.GetDealer( id );

				ViewBag.FullSiteURL = FullSiteURL();

				return View( dInfo );
			}
			catch
			{
			}

			return Redirect( FullSiteURL() + "/404-page/" );
		}
    }
}
