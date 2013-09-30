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
    public class DealerController : Controller
    {
		public ActionResult Index( string id )
		{
			DealerRepository dRepository = new DealerRepository();

			var dInfo = dRepository.GetDealer( id );

			return View( dInfo );
		}
    }
}
