﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PDWModels.Collateral;
using PWDRepositories;
using PDWInfrastructure.Attributes;

namespace ProductDataWarehouse.Controllers
{
    public class CollateralController : BaseController
    {
		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
        public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
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
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View( new CollateralInformation() { IsActive = true } );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		[TempPasswordCheck]
		public ActionResult AddGroup()
		{
			var detail = new CollateralGroupInformation() { IsActive = true };
			detail.GroupItems.Add( new CollateralGroupInformation.GroupInfoDetail() );

			return View( detail );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			var cInfo = cRepository.GetCollateral( id );

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		[TempPasswordCheck]
		public ActionResult EditGroup( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			var cInfo = cRepository.GetCollateralGroup( id );

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		/*
		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
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
		*/
		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		public ActionResult AddShipment()
		{
			return View( new List<CollateralShipmentDetail>() { new CollateralShipmentDetail() } );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult AddOrder()
		{
			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewAddOrder", model: ( new CollateralRepository() ).BlankOrderInformation() );
			}

			return View( ( new CollateralRepository() ).BlankOrderInformation() );
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddOrder( NewOrderInformation orderInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CollateralRepository cRepository = new CollateralRepository();

					cRepository.AddCollateralOrder( orderInfo );

					if( PaoliWebUser.CurrentUser.CanManageOrders )
					{
						return RedirectToAction( "Orders" );
					}

					return RedirectToAction( "ViewOrders" );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to add an order at this time." );
				}
			}

			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewAddOrder", model: orderInfo );
			}

			return View( orderInfo );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult EditOrder( int id )
		{
			return View( ( new CollateralRepository() ).GetOrderInformation( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewOrder( int id )
		{
			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewViewOrder", model: ( new CollateralRepository() ).GetPendingOrder( id, true ) );
			}
			return View( ( new CollateralRepository() ).GetPendingOrder( id, true ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult Orders()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
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

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewOrders()
		{
			if( PaoliWebUser.CurrentUser.IsNewLayout )
			{
				return View( viewName: "NewOrderList" );
			}

			return View();
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public JsonResult UserHomePageList( int itemCount )
		{
			CollateralRepository cRepository = new CollateralRepository();

			var results = cRepository.GetHomePageOrderList( itemCount );

			return Json( results, JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public JsonResult FullCollateralOrderListForUser( CollateralOrderTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			CollateralRepository cRepository = new CollateralRepository();

			var results = cRepository.GetFullCollateralOrderListForUser(
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
		[TempPasswordCheck]
		public ActionResult ShipOrder( int id )
		{
			return View( (new CollateralRepository()).GetPendingOrder( id, false ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
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

			return View( ( new CollateralRepository() ).GetPendingOrder( id, false ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewShipment( int id )
		{

			return View( ( new CollateralRepository() ).GetOrderShipment( id ) );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public JsonResult CancelOrder( int id )
		{
			CollateralRepository cRepository = new CollateralRepository();

			return Json( cRepository.CancelOrder( id ), JsonRequestBehavior.AllowGet );
		}

		public static IEnumerable<SelectListItem> GetPartyListForUser()
		{
			var rpList = NewOrderInformation.RequestingParties.ToList();

			if( PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
			{
				rpList.RemoveAll( i => i.Key == NewOrderInformation.RPPaoliMember );
			}
			if( PaoliWebUser.CurrentUser.IsDealerUser )
			{
				rpList.RemoveAll( i => i.Key == NewOrderInformation.RPPaoliMember );
				rpList.RemoveAll( i => i.Key == NewOrderInformation.RPPaoliRepresentative );
			}

			return rpList.Select( rp => new SelectListItem() { Text = rp.Value, Value = rp.Key.ToString() } );
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
