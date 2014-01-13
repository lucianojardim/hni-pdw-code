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

		public static IEnumerable<SelectListItem> GetCollateralTypeDDList()
		{
			return ( new CollateralRepository() ).GetCollateralTypeList().Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );
		}

		public static IEnumerable<SelectListItem> GetCollateralTypeFilterList()
		{
			return new List<SelectListItem>() { new SelectListItem() { Text = "All", Value = "0", Selected = true } }.Union( GetCollateralTypeDDList() );
		}

		public static IEnumerable<SelectListItem> GetCollateralStatusDDList()
		{
			return CollateralStatus.DisplayStrings.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );
		}

		public static IEnumerable<SelectListItem> GetProductDDList( int? itemId = null )
		{
			return ( new CollateralRepository() ).GetCollateralList( false ).Select( c => new SelectListItem() { Value = c.Key.ToString(), Text = c.Value, Selected = (c.Key == (itemId ?? 0)) } );
		}
    }
}
