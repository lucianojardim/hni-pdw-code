using System;
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

			using( var cRepository = new CollateralRepository() )
			{
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
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.AddCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

					return RedirectToAction( "Manage" );
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
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.AddCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

					return RedirectToAction( "Manage" );
				}
			}

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				var cInfo = cRepository.GetCollateral( id );

				return View( cInfo );
			}
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit( CollateralInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.UpdateCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

					return RedirectToAction( "Manage" );
				}
			}

			return View( cInfo );
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		public ActionResult EditGroup( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				var cInfo = cRepository.GetCollateralGroup( id );

				return View( cInfo );
			}
		}

		[PaoliAuthorize( "CanManageCollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditGroup( CollateralGroupInformation cInfo, HttpPostedFileBase CollateralImage )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.UpdateCollateral( cInfo,
						CollateralImage != null ? CollateralImage.InputStream : null,
						CollateralImage != null ? CollateralImage.FileName : null );

					return RedirectToAction( "Manage" );
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
			using( var cRepository = new CollateralRepository() )
			{
				cRepository.AddCollateralShipment( theList );

				return RedirectToAction( "Manage" );
			}
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult AddOrder()
		{
			using( var cRepository = new CollateralRepository() )
			{
				return View( viewName: "NewAddOrder", model: cRepository.BlankOrderInformation() );
			}
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddOrder( NewOrderInformation orderInfo )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.AddCollateralOrder( orderInfo );

					if( PaoliWebUser.CurrentUser.CanManageOrders )
					{
						return RedirectToAction( "Orders" );
					}

					return RedirectToAction( "ViewOrders" );
				}
			}

			return View( viewName: "NewAddOrder", model: orderInfo );
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult EditOrder( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				return View( cRepository.GetOrderInformation( id ) );
			}
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditOrder( NewOrderInformation orderInfo )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CollateralRepository() )
				{
					cRepository.UpdateCollateralOrder( orderInfo );

					return RedirectToAction( "Orders" );
				}
			}

			return View( orderInfo );
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewOrder( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				return View( viewName: "NewViewOrder", model: cRepository.GetPendingOrder( id, true ) );
			}
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

			using( var cRepository = new CollateralRepository() )
			{
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
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewOrders()
		{
			return View( viewName: "NewOrderList" );
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public JsonResult UserHomePageList( int itemCount )
		{
			using( var cRepository = new CollateralRepository() )
			{
				var results = cRepository.GetHomePageOrderList( itemCount );

				return Json( results, JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanAddOrders" )]
		[TempPasswordCheck]
		public JsonResult FullCollateralOrderListForUser( CollateralOrderTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var cRepository = new CollateralRepository() )
			{
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
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult ShipOrder( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				return View( cRepository.GetPendingOrder( id, false ) );
			}
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ShipOrder( int id, PendingOrderInformation.ShipmentSummary summary )
		{
			using( var cRepository = new CollateralRepository() )
			{
				if( ModelState.IsValid )
				{
					cRepository.AddOrderShipment( id, summary );

					return RedirectToAction( "ShipOrder", new { id = id } );
				}

				return View( cRepository.GetPendingOrder( id, false ) );
			}
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public ActionResult ViewShipment( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				return View( cRepository.GetOrderShipment( id ) );
			}
		}

		[PaoliAuthorize( "CanManageOrders" )]
		[TempPasswordCheck]
		public JsonResult CancelOrder( int id )
		{
			using( var cRepository = new CollateralRepository() )
			{
				return Json( cRepository.CancelOrder( id ), JsonRequestBehavior.AllowGet );
			}
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
			using( var cRepository = new CollateralRepository() )
			{
				return cRepository.GetCollateralTypeList().Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );
			}
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
			using( var cRepository = new CollateralRepository() )
			{
				return cRepository.GetCollateralList( false ).Select( c => new SelectListItem() { Value = c.Key.ToString(), Text = c.Value, Selected = ( c.Key == ( itemId ?? 0 ) ) } );
			}
		}
    }
}
