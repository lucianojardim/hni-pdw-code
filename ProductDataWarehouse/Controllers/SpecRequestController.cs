using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.SpecRequests;

namespace ProductDataWarehouse.Controllers
{
    public class SpecRequestController : Controller
    {
        //
        // GET: /SpecRequest/

		[PaoliAuthorize( "CanManageTypicals" )]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public JsonResult FullRequestList( SpecRequestTableParams paramDetails )
		{
			int totalCount = 0, filteredCount = 0;

			SpecRequestRepository sRepository = new SpecRequestRepository();

			var results = sRepository.GetFullRequestList(
				paramDetails, out totalCount, out filteredCount );

			return Json( new
			{
				sEcho = paramDetails.sEcho,
				iTotalRecords = totalCount,
				iTotalDisplayRecords = filteredCount,
				aaData = results
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public ActionResult Add()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[HttpPost]
		public ActionResult Add( SpecRequestInformation sInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.AddSpecRequest( sInfo );

					ViewBag.CloseFancyBox = true;

					return View( sInfo );
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

			return View( sInfo );
		}

		public static string GetJustSeriesNameList()
		{
			var list = (new SeriesRepository()).GetJustSeriesNameList();

			return "'" + string.Join( "','", list ) + "'";
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public ActionResult Edit( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			return View( sRepository.GetSpecRequest( id ) );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[HttpPost]
		public ActionResult Edit( SpecRequestInformation sInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.UpdateSpecRequest( sInfo );

					ViewBag.CloseFancyBox = true;

					return View( sInfo );
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

			return View( sInfo );
		}

    }
}
