using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.SpecRequests;
using PDWInfrastructure.Attributes;

namespace ProductDataWarehouse.Controllers
{
    public class SpecRequestController : Controller
    {
        //
        // GET: /SpecRequest/

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
		public ActionResult ViewAll()
		{
			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewViewAll" );
			}

			return View();
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
		public JsonResult UserHomePageList( int itemCount )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			var results = sRepository.GetHomePageRequestList( itemCount );

			return Json( results, JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
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
		[TempPasswordCheck]
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

		[PaoliAuthorize( "CanAddSpecRequests" )]
		[TempPasswordCheck]
		public ActionResult AddRequest()
		{
			ViewBag.BlankInformation = new SpecRequestRepository().NewSpecRequest();

			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewAddRequest" );
			}

			return View();
		}

		[PaoliAuthorize( "CanAddSpecRequests" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult AddRequest( SpecRequestInformation sInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.AddSpecRequest( sInfo );

					if( PaoliWebUser.CurrentUser.CanManageTypicals )
					{
						return RedirectToAction( "Manage" );
					}
					return RedirectToAction( "ViewAll" );
				}
				catch( Exception ex )
				{
					ModelState.AddModelError( "", ex.Message );
					if( ex.InnerException != null )
					{
						ModelState.AddModelError( "", ex.InnerException.Message );
					}

					( new PDWInfrastructure.EmailSenders.ErrorEmailSender() ).SubmitErrorEmail( ex );
				}
			}
			ViewBag.BlankInformation = new SpecRequestRepository().NewSpecRequest();

			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewAddRequest", model: sInfo );
			}

			return View( sInfo );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult AddTypical( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			var newTypical = sRepository.GetNewTypical( id );

			return View( newTypical );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddTypical( TypicalMgmtInfo tInfo, string typeOfSubmit )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.AddTypical( tInfo, typeOfSubmit == "Publish" );

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

			return View( tInfo );
		}

		public static string GetJustSeriesNameList()
		{
			var list = (new SeriesRepository()).GetJustSeriesNameList();

			return "'" + string.Join( "','", list ) + "'";
		}

		public static IEnumerable<SelectListItem> GSAContractList( bool includeBlank )
		{
			var theList = ( new SpecRequestRepository() ).GetGSAContractList()
				.Select( g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Text } )
				.ToList();
				
			if( includeBlank )
			{
				theList.Insert( 0, new SelectListItem() );
			}

			return theList;
		}

		public static IEnumerable<SelectListItem> GetShapeDDList()
		{
			var shapes = ( new AttributeRepository() ).GetTypicalOptionList( "Shape" ).Select( s => s.Name ).ToList();

			shapes.Add( "" );

			shapes.Sort();

			return shapes.Select( a => new SelectListItem() { Text = a, Value = a } );
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
		[TempPasswordCheck]
		public ActionResult ViewRequest( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewViewRequest", model: sRepository.GetSpecRequest( id ) );
			}

			return View( sRepository.GetSpecRequest( id ) );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult EditRequest( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			ViewBag.BlankInformation = sRepository.NewSpecRequest();

			return View( sRepository.GetSpecRequest( id ) );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult EditRequest( SpecRequestInformation sInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.UpdateSpecRequest( sInfo );

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
			ViewBag.BlankInformation = new SpecRequestRepository().NewSpecRequest();

			return View( sInfo );
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		public JsonResult CancelRequest( int id )
		{
			bool bSuccess = ( new SpecRequestRepository() ).CancelRequest( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public JsonResult OpenRequest( int id )
		{
			bool bSuccess = ( new SpecRequestRepository() ).ReOpenRequest( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult EditTypical( int id )
		{
			SpecRequestRepository sRepository = new SpecRequestRepository();

			var typical = sRepository.GetTypical( id );

			return View( typical );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditTypical( TypicalMgmtInfo tInfo, string typeOfSubmit )
		{
			if( ModelState.IsValid )
			{
				try
				{
					SpecRequestRepository sRepository = new SpecRequestRepository();

					sRepository.UpdateTypical( tInfo, typeOfSubmit != "Save Changes" );

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

			return View( tInfo );
		}

		public static IDictionary<string, string> GetFeatureImageList()
		{
			return ( new SeriesRepository() ).GetFeatureImageList();
		}
    }
}
