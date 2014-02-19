using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PDWModels.Collateral;
using PWDRepositories;

namespace ProductDataWarehouse.Controllers
{
    public class CollateralController : BaseController
    {
		[PaoliAuthorize( "CanManageCollateral" )]
        public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageCollateral" )]
		public JsonResult FullCollateralList( CollateralTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			CollateralRepository cRepository = new CollateralRepository();

			var results = cRepository.GetFullCollateralList(
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

		[PaoliAuthorize( "CanManageCollateral" )]
		public ActionResult Add()
		{
			return View( new CollateralInformation() );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[HttpPost]
		public ActionResult Add( CollateralInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.AddCollateral( cInfo, 
						CollateralImage != null ? CollateralImage.InputStream : null, 
						CollateralImage != null ? CollateralImage.FileName : null );

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

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		public ActionResult AddGroup()
		{
			var detail = new CollateralGroupInformation();
			detail.GroupItems.Add( new CollateralGroupInformation.GroupInfoDetail() );

			return View( detail );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[HttpPost]
		public ActionResult AddGroup( CollateralGroupInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.AddCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

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

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		public ActionResult Edit( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			var cInfo = cRepository.GetCollateral( id );

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[HttpPost]
		public ActionResult Edit( CollateralInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.UpdateCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

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

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		public ActionResult EditGroup( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			var cInfo = cRepository.GetCollateralGroup( id );

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[HttpPost]
		public ActionResult EditGroup( CollateralGroupInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.UpdateCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

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

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		public JsonResult Delete( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			bool bSuccess = cRepository.DeleteCollateral( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		public ActionResult AddShipment()
		{
			return View( new List<CollateralShipmentDetail>() { new CollateralShipmentDetail() } );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[HttpPost]
		public ActionResult AddShipment( List<CollateralShipmentDetail> theList )
		{
			try
			{
				CollateralRepository cRepository = new CollateralRepository();

				cRepository.AddCollateralShipment( theList );

				return RedirectToAction( "Manage" );
			}
			catch( Exception )
			{
				ModelState.AddModelError( "", "Unable to add a shipment at this time." );
			}
			return View( theList );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public ActionResult AddOrder()
		{
			return View( ( new CollateralRepository() ).BlankOrderInformation() );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[HttpPost]
		public ActionResult AddOrder( NewOrderInformation orderInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.AddCollateralOrder( orderInfo );

					return RedirectToAction( "Orders" );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to add an order at this time." );
				}
			}

			return View( orderInfo );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public ActionResult EditOrder( int id )
		{
			return View( ( new CollateralRepository() ).GetOrderInformation( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[HttpPost]
		public ActionResult EditOrder( NewOrderInformation orderInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.UpdateCollateralOrder( orderInfo );

					return RedirectToAction( "Orders" );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to update order at this time." );
				}
			}

			return View( orderInfo );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public ActionResult Orders()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public JsonResult FullCollateralOrderList( CollateralOrderTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			CollateralRepository cRepository = new CollateralRepository();

			var results = cRepository.GetFullCollateralOrderList(
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

		[PaoliAuthorize( "CanManageOrders" )]
		public ActionResult ShipOrder( int id )
		{
			return View( (new CollateralRepository()).GetPendingOrder( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[HttpPost]
		public ActionResult ShipOrder( int id, PendingOrderInformation.ShipmentSummary summary )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.AddOrderShipment( id, summary );

					return RedirectToAction( "ShipOrder", new { id = id } );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to add order shipment at this time." );
				}
			}

			return View( ( new CollateralRepository() ).GetPendingOrder( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public ActionResult ViewShipment( int id )
		{

			return View( ( new CollateralRepository() ).GetOrderShipment( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		public JsonResult CancelOrder( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			return Json( cRepository.CancelOrder( id ), JsonRequestBehavior.AllowGet );
		}

		public static IEnumerable<SelectListItem> GetCollateralTypeDDList()
		{
			return ( new CollateralRepository() ).GetCollateralTypeList().Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );
		}

		public static IEnumerable<SelectListItem> GetCollateralTypeFilterList()
		{
			var theList = GetCollateralTypeDDList().ToList();
			theList.Add( new SelectListItem() { Text = "Bundle", Value = "-1" } );
			theList.Sort( delegate( SelectListItem x, SelectListItem y )
				{
					return x.Text.CompareTo( y.Text );
				} );
			theList.Insert( 0, new SelectListItem() { Text = "All", Value = "0", Selected = true } );
			return theList;
		}

		public static IEnumerable<SelectListItem> GetProductDDList( int? itemId = null )
		{
			return ( new CollateralRepository() ).GetCollateralList( false ).Select( c => new SelectListItem() { Value = c.Key.ToString(), Text = c.Value, Selected = (c.Key == (itemId ?? 0)) } );
		}
    }
}
