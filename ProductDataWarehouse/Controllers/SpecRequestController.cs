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

		[PaoliAuthorize( "CanViewSpecRequests" )]
		public ActionResult ViewAll()
		{
			return View();
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		public JsonResult UserRequestList( UserSpecRequestTableParams paramDetails )
		{
			int totalCount = 0, filteredCount = 0;

			SpecRequestRepository sRepository = new SpecRequestRepository();

			var results = sRepository.GetUserRequestList(
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

			return View( sInfo );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public ActionResult AddTypical( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			var newTypical = sRepository.GetNewTypical( id );

			return View( newTypical );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[HttpPost]
		public ActionResult AddTypical( TypicalMgmtInfo tInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.AddTypical( tInfo );

					ViewBag.CloseFancyBox = true;

					return View( tInfo );
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

			return View( tInfo );
		}

		public static string GetJustSeriesNameList()
		{
			var list = (new SeriesRepository()).GetJustSeriesNameList();

			return "'" + string.Join( "','", list ) + "'";
		}

		public static IEnumerable<SelectListItem> GetFootprintDDList()
		{
			var footprints = ( new AttributeRepository() ).GetTypicalOptionList( "Footprint" ).Select( s => s.Name ).ToList();

			footprints.Add( "" );

			footprints.Sort( delegate( string x, string y )
			{
				var xSizes = x.Split( 'x' );
				var ySizes = y.Split( 'x' );
				if( xSizes.Count() == ySizes.Count() && xSizes.Count() == 2 )
				{
					int x1 = 0, x2 = 0, y1 = 0, y2 = 0;

					if( int.TryParse( xSizes[0], out x1 ) &&
						int.TryParse( xSizes[1], out x2 ) &&
						int.TryParse( ySizes[0], out y1 ) &&
						int.TryParse( ySizes[1], out y1 ) )
					{
						if( x1 == y1 )
						{
							return x2 < y2 ? -1 : ( ( x2 > y2 ) ? 1 : 0 );
						}
						return x1 < y1 ? -1 : ( ( x1 > y1 ) ? 1 : 0 );
					}
				}

				return string.Compare( x, y );
			} );

			return footprints.Select( a => new SelectListItem() { Text = a, Value = a } );
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		public ActionResult View( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			return View( sRepository.GetSpecRequest( id ) );
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

			return View( sInfo );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public ActionResult EditTypical( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			var typical = sRepository.GetTypical( id );

			return View( typical );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[HttpPost]
		public ActionResult EditTypical( TypicalMgmtInfo tInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.UpdateTypical( tInfo );

					ViewBag.CloseFancyBox = true;

					return View( tInfo );
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

			return View( tInfo );
		}

    }
}
