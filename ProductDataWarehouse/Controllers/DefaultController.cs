using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ProductDataWarehouse.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
			return Redirect( ConfigurationManager.AppSettings["HomePageRedirect"] );
		}

    }
}
