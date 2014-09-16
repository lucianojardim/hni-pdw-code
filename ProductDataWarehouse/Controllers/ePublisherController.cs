﻿using System;
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
	public class ePublisherController : BaseController
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
		[ValidateAntiForgeryToken]
		public ActionResult Add( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
					ECollateralRepository eRepository = new ECollateralRepository();

					int itemId = 0;
					eRepository.AddSettings( settings, PaoliWebUser.CurrentUser.UserId, out itemId );

					return RedirectToAction( "SetLayout", new { id = itemId } );
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
		[ValidateAntiForgeryToken]
		public ActionResult EditSettings( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
					ECollateralRepository eRepository = new ECollateralRepository();

					eRepository.EditSettings( settings, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = settings.ItemID } );
			}

			return View( settings );
		}

		public static IEnumerable<ECollateralRepository.Layouts.LayoutDetails> GetLayouts( int layoutType )
		{
			return new ECollateralRepository().GetLayoutSelectionDetails( layoutType );
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
		[ValidateAntiForgeryToken]
		public ActionResult SetLayout( ECollateralLayout layoutInfo )
		{
			if( ModelState.IsValid )
			{
					ECollateralRepository eRepository = new ECollateralRepository();

					eRepository.SetLayout( layoutInfo, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = layoutInfo.ItemID } );
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
			var details = ( new ECollateralRepository() ).GetItemDetails( id );

			if( details == null )
			{
				return RedirectToAction( "SetLayout", new { id = id } );
			}

			return View( details );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditLayout( ECollateralDetails dInfo )
		{
			if( ModelState.IsValid )
			{
					ECollateralRepository eRepository = new ECollateralRepository();

					bool bNeedVerify;
					eRepository.SetItemSections( dInfo, PaoliWebUser.CurrentUser.UserId, out bNeedVerify );

					return RedirectToAction( bNeedVerify ? "VerifyLayout" : "ViewLayout", new { id = dInfo.ItemID } );
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

		[PaoliAuthorize( "CanReviewECollateral" )]
		public JsonResult UpdateStatus( int itemId, int updateStatus )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			bool bSuccess = eRepository.UpdateStatus( itemId, updateStatus );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult ValidateURL( int itemId, string url )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			bool bSuccess = eRepository.ValidateURL( itemId, url );

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

		[PaoliAuthorize( "CanManageAllECollateral" )]
		public JsonResult GetAllPagesList( int skipItems, string filterText )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			var theList = eRepository.GetAllItemsList( skipItems, filterText );

			return Json( theList,
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManageAllECollateral" )]
		public JsonResult Delete( int id )
		{
			ECollateralRepository eRepository = new ECollateralRepository();

			bool bSuccess = eRepository.DeletePage( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}
	}
}
