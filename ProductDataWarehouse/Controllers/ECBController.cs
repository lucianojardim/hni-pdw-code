using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.Attributes;
using PWDRepositories;
using PDWModels.eCollateral;
using PDWInfrastructure;

namespace ProductDataWarehouse.Controllers
{
	public class ECBController : BaseController
    {
		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
        public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View( new ECollateralSettings() { IsTemplate = PaoliWebUser.CurrentUser.CanAddECTemplate } );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		public ActionResult Add( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ECollateralRepository eRepository = new ECollateralRepository();

					int itemId = 0;
					eRepository.AddSettings( settings, PaoliWebUser.CurrentUser.UserId, out itemId );

					return RedirectToAction( "SetLayout", new { id = itemId } );
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

			return View( settings );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult EditSettings( int id )
		{
			var settings = ( new ECollateralRepository() ).GetItemSettings( id );
			
			settings.IsTemplate &= PaoliWebUser.CurrentUser.CanAddECTemplate;
			
			return View( settings );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		public ActionResult EditSettings( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ECollateralRepository eRepository = new ECollateralRepository();

					eRepository.EditSettings( settings, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = settings.ItemID } );
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

			return View( settings );
		}

		public static IEnumerable<SelectListItem> GetLayouts( int layoutType )
		{
			var retList = new List<SelectListItem>();
			var layoutList = ECollateralRepository.LayoutTypes.LayoutList[layoutType];
			foreach( var layout in layoutList )
			{
				retList.Add( new SelectListItem() { Text = ECollateralRepository.Layouts.LayoutTitles[layout], Value = layout.ToString() } );
			}

			return retList;
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult SetLayout( int id )
		{
			return View( new ECollateralLayout() { ItemID = id } );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		public ActionResult SetLayout( ECollateralLayout layoutInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ECollateralRepository eRepository = new ECollateralRepository();

					eRepository.SetLayout( layoutInfo, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = layoutInfo.ItemID } );
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

			return View( layoutInfo );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetImageList( string imgFilter )
		{
			return Json( (new ImageRepository()).Search( imgFilter, true ),
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult EditLayout( int id )
		{
			return View( (new ECollateralRepository()).GetItemDetails( id ) );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		public ActionResult EditLayout( ECollateralDetails dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ECollateralRepository eRepository = new ECollateralRepository();

					eRepository.SetItemSections( dInfo, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "VerifyLayout", new { id = dInfo.ItemID } );
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

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult VerifyLayout( int id )
		{
			return View( ( new ECollateralRepository() ).GetItemInformation( id ) );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult ConfirmLayout( int id )
		{
			var eRepository = new ECollateralRepository();

			eRepository.ConfirmLayout( id, PaoliWebUser.CurrentUser.UserId );

			return RedirectToAction( "ViewLayout", new { id = id } );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult ViewLayout( int id )
		{
			var eRepository = new ECollateralRepository();

			return View( eRepository.GetItemInformation( id ) );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult CopyLayout( int id )
		{
			var eRepository = new ECollateralRepository();

			var newId = eRepository.CopyLayout( id, PaoliWebUser.CurrentUser.UserId );

			return RedirectToAction( "EditSettings", new { id = newId } );
		}
		
		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult ValidateURL( string url )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			bool bSuccess = eRepository.ValidateURL( 0, url );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetMyPagesList( int skipItems, string filterText )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			var theList = eRepository.GetMyPagesList( skipItems, filterText, PaoliWebUser.CurrentUser.UserId );

			return Json( theList,
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetTemplateList( int skipItems, string filterText )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			var theList = eRepository.GetTemplateList( skipItems, filterText, PaoliWebUser.CurrentUser.CanAddECTemplate );

			return Json( theList,
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanReviewECollateral" )]
		public JsonResult GetReviewPagesList( int skipItems, string filterText )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			var theList = eRepository.GetReviewItemsList( skipItems, filterText );

			return Json( theList,
				JsonRequestBehavior.AllowGet );
		}
		
    }
}
