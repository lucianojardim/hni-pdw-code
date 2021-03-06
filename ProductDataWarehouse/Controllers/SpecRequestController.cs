﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.SpecRequests;
using PDWInfrastructure.Attributes;
using System.Configuration;

namespace ProductDataWarehouse.Controllers
{
	public class SpecRequestController : BaseController
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
			return View( viewName: "NewViewAll" );
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
		public JsonResult UserHomePageList( int itemCount )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				var results = sRepository.GetHomePageRequestList( itemCount );

				return Json( results, JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
		public JsonResult UserRequestList( UserSpecRequestTableParams paramDetails )
		{
			int totalCount = 0, filteredCount = 0;

			using( var sRepository = new SpecRequestRepository() )
			{
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
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public JsonResult FullRequestList( SpecRequestTableParams paramDetails )
		{
			int totalCount = 0, filteredCount = 0;

			using( var sRepository = new SpecRequestRepository() )
			{
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
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public JsonResult UploadNewFiles()
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				var tempFileList = sRepository.SaveTempFiles( Request.Files, ConfigurationManager.AppSettings["SpecRequestDocumentLocation"] );

				return Json( new { success = true, fileList = tempFileList }, JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanAddSpecRequests" )]
		[TempPasswordCheck]
		public ActionResult AddRequest()
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				ViewBag.BlankInformation = sRepository.NewSpecRequest( true );

				return View( viewName: "NewAddRequest" );
			}
		}

		[PaoliAuthorize( "CanAddSpecRequests" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult AddRequest( SpecRequestInformation sInfo )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				if( ModelState.IsValid )
				{
					sRepository.AddSpecRequest( sInfo );

					if( PaoliWebUser.CurrentUser.CanManageTypicals )
					{
						return RedirectToAction( "Manage" );
					}
					return RedirectToAction( "ViewAll" );
				}
				ViewBag.BlankInformation = sRepository.NewSpecRequest( true );
			}

			return View( viewName: "NewAddRequest", model: sInfo );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult AddTypical( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				var newTypical = sRepository.GetNewTypical( id );

				return View( newTypical );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddTypical( TypicalMgmtInfo tInfo, string typeOfSubmit )
		{
			if( ModelState.IsValid )
			{
				using( var sRepository = new SpecRequestRepository() )
				{
					sRepository.AddTypical( tInfo, typeOfSubmit == "Publish" );

					return RedirectToAction( "Manage" );
				}
			}

			return View( tInfo );
		}

		public static string GetJustSeriesNameList()
		{
			using( var sRepository = new SeriesRepository() )
			{
				var list = sRepository.GetJustSeriesNameList();

				return "'" + string.Join( "','", list ) + "'";
			}
		}

		public static IEnumerable<SelectListItem> GSAContractList( bool includeBlank )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				var theList = sRepository.GetGSAContractList()
					.Select( g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Text } )
					.ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new SelectListItem() );
				}

				return theList;
			}
		}

		public static IEnumerable<SelectListItem> GetShapeDDList()
		{
			using( var aRepository = new AttributeRepository() )
			{
				var shapes = aRepository.GetTypicalOptionList( "Shape" ).Select( s => s.Name ).ToList();

				shapes.Add( "" );

				shapes.Sort();

				return shapes.Select( a => new SelectListItem() { Text = a, Value = a } );
			}
		}

		public static IEnumerable<SelectListItem> GetFootprintDDList()
		{
			using( var aRepository = new AttributeRepository() )
			{
				var footprints = aRepository.GetTypicalOptionList( "Footprint" ).Select( s => s.Name ).ToList();

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
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		[TempPasswordCheck]
		public ActionResult ViewRequest( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				return View( viewName: "NewViewRequest", model: sRepository.GetSpecRequest( id ) );
			}
		}

		[PaoliAuthorize( "CanReOpenSpecRequests" )]
		[TempPasswordCheck]
		public ActionResult ReOpenRequest( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				return View( sRepository.GetReOpenRequest( id ) );
			}
		}

		[PaoliAuthorize( "CanReOpenSpecRequests" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult ReOpenRequest( ReOpenRequestInformation sInfo )
		{
			if( ModelState.IsValid )
			{
				using( var sRepository = new SpecRequestRepository() )
				{
					sRepository.ReOpenSpecRequest( sInfo );

					if( PaoliWebUser.CurrentUser.CanManageTypicals )
					{
						return RedirectToAction( "Manage" );
					}
					return RedirectToAction( "ViewAll" );
				}
			}

			return View( sInfo );
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult EditRequest( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				ViewBag.BlankInformation = sRepository.NewSpecRequest( false );

				return View( sRepository.GetSpecRequest( id ) );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateInput( false )]
		public ActionResult EditRequest( SpecRequestInformation sInfo )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				if( ModelState.IsValid )
				{
					sRepository.UpdateSpecRequest( sInfo );

					return RedirectToAction( "Manage" );
				}
				ViewBag.BlankInformation = sRepository.NewSpecRequest( false );
			}

			return View( sInfo );
		}

		[PaoliAuthorize( "CanViewSpecRequests" )]
		public JsonResult CancelRequest( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				bool bSuccess = sRepository.CancelRequest( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public JsonResult OpenRequest( int id, bool doEmail )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				bool bSuccess = sRepository.ReOpenRequest( id, doEmail );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		public ActionResult EditTypical( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				var typical = sRepository.GetTypical( id );

				return View( typical );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditTypical( TypicalMgmtInfo tInfo, string typeOfSubmit )
		{
			if( ModelState.IsValid )
			{
				using( var sRepository = new SpecRequestRepository() )
				{
					sRepository.UpdateTypical( tInfo, typeOfSubmit != "Save Changes" );

					return RedirectToAction( "Manage" );
				}
			}

			return View( tInfo );
		}

		public static IDictionary<string, string> GetFeatureImageList()
		{
			using( var sRepository = new SeriesRepository() )
			{
				return sRepository.GetFeatureImageList();
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public JsonResult GetProjectForCustomer( int customer, int? dealer, int territory )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				return Json( new { theList = sRepository.GetProjectForCustomer( customer, dealer, territory ) },
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanInLineAddProjects" )]
		public JsonResult CreateNewProject( NewProjectInformation pInfo )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				return Json( new { projectId = sRepository.Add( pInfo ) },
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageTypicals" )]
		public JsonResult GetProjectInfo( int id )
		{
			using( var sRepo = new SpecRequestRepository() )
			{
				return Json( sRepo.GetProject( id ), JsonRequestBehavior.AllowGet );
			}
		}

		public static IEnumerable<SelectListItem> ProjectScopeDDList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem(),
				new SelectListItem() { Text = "1-5 offices" },
				new SelectListItem() { Text = "6-10 offices" },
				new SelectListItem() { Text = "11-19 offices" },
				new SelectListItem() { Text = "20 or more﻿" }
			};
		}

		public static IEnumerable<SelectListItem> ProjectListPriceDDList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem(),
				new SelectListItem() { Text = "< $9,999" },
				new SelectListItem() { Text = "$10,000 - $19,999" },
				new SelectListItem() { Text = "$20,000 - $29,999" },
				new SelectListItem() { Text = "$30,000 - $49,999" },
				new SelectListItem() { Text = "$50,000 - $99,999" },
				new SelectListItem() { Text = "$100,000 - $249,999" },
				new SelectListItem() { Text = "> $250,000﻿" },
			};
		}

		public static IEnumerable<SelectListItem> SpecStatusDDList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem(),
				new SelectListItem() { Text = "Pending" },
				new SelectListItem() { Text = "Won" },
				new SelectListItem() { Text = "Lost" },
			};
		}
    }
}
