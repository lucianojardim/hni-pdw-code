using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Companies;
using PDWInfrastructure.Attributes;

namespace ProductDataWarehouse.Controllers
{
	public class CompanyController : BaseController
	{
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		public ActionResult ImportTripData()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		public ActionResult ImportTerritoryTripData()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		public ActionResult ImportTripData( HttpPostedFileBase dataFile )
		{
			using( var cRepo = new CompanyRepository() )
			{
				var errors = new List<string>();

				ViewBag.CloseFancyBox = cRepo.ImportTripData( dataFile.InputStream, errors );

				foreach( var e in errors )
				{
					ModelState.AddModelError( "", e );
				}
			}

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		public ActionResult ImportTerritoryTripData( HttpPostedFileBase dataFile )
		{
			using( var cRepo = new CompanyRepository() )
			{
				var errors = new List<string>();

				ViewBag.CloseFancyBox = cRepo.ImportTripTerritoryData( dataFile.InputStream, errors );

				foreach( var e in errors )
				{
					ModelState.AddModelError( "", e );
				}
			}

			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult FullCompanyList( CompanyTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var cRepository = new CompanyRepository() )
			{
				var results = cRepository.GetFullCompanyList(
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

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public JsonResult DealershipContact( MyCompanyInfo cInfo )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var companyId = cRepository.UpdateCompany( PaoliWebUser.CurrentUser.UserId, cInfo );

				return Json( new
				{
					success = true,
					companyId = companyId
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanUpdateDealerOnMyTerritory" )]
		[TempPasswordCheck]
		public JsonResult UpdateSalesRepForDealer( int companyId, int psrContact )
		{
			using( var cRepository = new CompanyRepository() )
			{
				bool bSuccess = cRepository.UpdateSalesRepForDealer( companyId, psrContact );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanViewMyTerritory" )]
		[TempPasswordCheck]
		public JsonResult TripDataForTerritoryCompanyList( CompanyTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var cRepository = new CompanyRepository() )
			{
				var results = cRepository.GetCompanyTripList(
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

		[PaoliAuthorize( "CanViewMyTerritoryDealerList" )]
		[TempPasswordCheck]
		public JsonResult DealerForTerritoryCompanyList( CompanyTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var cRepository = new CompanyRepository() )
			{
				var results = cRepository.GetFullCompanyList(
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

		[PaoliAuthorize( "CanManageTerritories" )]
		[TempPasswordCheck]
		public JsonResult TerritoryCompanyList( CompanyTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var cRepository = new CompanyRepository() )
			{
				var results = cRepository.GetFullCompanyList(
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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View( new CompanyInformation() );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Add( CompanyInformation uInfo, HttpPostedFileBase CompanyImage )
		{
			if( ModelState.IsValid )
			{
				if( CompanyImage != null )
				{
					if( CompanyImage.ContentLength > 500 * 1024 )
					{
						ModelState.AddModelError( "CompanyImage", "Image must be less than 500Kb." );

						return View( uInfo );
					}
				}

				using( var cRepository = new CompanyRepository() )
				{
					cRepository.AddCompany( uInfo, CompanyImage != null ? CompanyImage.InputStream : null, CompanyImage != null ? CompanyImage.FileName : null );

					return RedirectToAction( "Manage" );
				}
			}

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var uInfo = cRepository.GetCompany( id );

				return View( uInfo );
			}
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit( CompanyInformation uInfo, HttpPostedFileBase CompanyImage )
		{
			if( ModelState.IsValid )
			{
				if( CompanyImage != null )
				{
					if( CompanyImage.ContentLength > 500 * 1024 )
					{
						ModelState.AddModelError( "CompanyImage", "Image must be less than 500Kb." );

						return View( uInfo );
					}
				}

				using( var cRepository = new CompanyRepository() )
				{
					cRepository.UpdateCompany( uInfo, CompanyImage != null ? CompanyImage.InputStream : null, CompanyImage != null ? CompanyImage.FileName : null );

					return RedirectToAction( "Manage" );
				}
			}

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult Delete( int id )
		{
			using( var cRepository = new CompanyRepository() )
			{
				bool bSuccess = cRepository.DeleteCompany( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanUpdateDealerOnMyTerritory" )]
		public JsonResult RequestDeactiviation( int companyId, string reason )
		{
			using( var cRepository = new CompanyRepository() )
			{
				bool bSuccess = cRepository.RequestCompanyDeactiviation( PaoliWebUser.CurrentUser.UserId, companyId, reason );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetTypeListForCompany( int companyId )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var uInfo = cRepository.GetCompany( companyId );

				var theList = PaoliWebUser.PaoliWebRole.RoleList
					.Where( r => PaoliWebUser.PaoliCompanyType.CompanyTypeAllowedUsers[uInfo.CompanyType].Contains( r.Key ) )
					.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );

				if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
					theList = theList.Where( i => i.Value != PaoliWebUser.PaoliWebRole.SuperAdmin.ToString() );

				return Json( new
				{
					companyType = uInfo.CompanyType,
					isTrip = uInfo.SignedUpForTrip,
					theList = theList
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetShippingAddress( int? companyID, int? userId )
		{
			if( userId.HasValue )
			{
				using( var uRepository = new UserRepository() )
				{
					return Json( uRepository.GetUserAddress( userId.Value ), JsonRequestBehavior.AllowGet );
				}
			}
			else if( companyID.HasValue )
			{
				using( var cRepository = new CompanyRepository() )
				{
					return Json( cRepository.GetCompanyAddress( companyID.Value ), JsonRequestBehavior.AllowGet );
				}
			}

			throw new Exception( "Unable to find shipping address" );
		}

		public JsonResult GetDealerList( bool includeBlank = true )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetFullCompanyList( PaoliWebUser.PaoliCompanyType.Dealer )
					.ToList();
				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItemExtra() { ID = 0, Text = "" } );
				}
				return Json( theList, JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetPaoliRepGroupList( bool includeBlank = true )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetFullCompanyList( PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
					.ToList();
				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItemExtra() { ID = 0, Text = "" } );
				}
				return Json( theList, JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetDealerListForSalesRep( int salesRepCompanyId, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetDealerList( salesRepCompanyId, includeTerritory ).ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItemExtra() { ID = 0, Text = "" } );
				}

				return Json( theList, JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetDealerListForTerritory( int territoryId, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetDealerListForTerritory( territoryId, includeTerritory ).ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItemExtra() { ID = 0, Text = "" } );
				}

				return Json( theList, JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetDealerListForUser( bool includeBlank = true )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetDealerListForUser( PaoliWebUser.CurrentUser.UserId ).ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
				}

				return Json( theList, JsonRequestBehavior.AllowGet );
			}
		}

		public class SelectListItemPlus : SelectListItem
		{
			public string Extra { get; set; }
		}

		public delegate IEnumerable<SelectListItemPlus> GetCompanyListFunction( int? companyType = null, bool includeBlank = true, bool includeTerritory = false );

		public static IEnumerable<SelectListItemPlus> GetThisCompanyAsDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var uRepository = new UserRepository() )
			{
				var cInfo = uRepository.GetCurrentCompanyInfo( includeTerritory );

				return new List<SelectListItemPlus>() { new SelectListItemPlus() { Value = cInfo.ID.ToString(), Text = cInfo.Text, Extra = cInfo.Extra } };
			}
		}

		public static IEnumerable<SelectListItemPlus> GetDealerForSalesRepDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetMyDealerList().ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItemExtra() { ID = 0, Text = "", Extra = "" } );
				}

				return theList.Select( cInfo => new SelectListItemPlus() { Value = cInfo.ID.ToString(), Text = cInfo.Text, Extra = cInfo.Extra } );
			}
		}

		public static IEnumerable<SelectListItemPlus> GetSalesRepForDealerDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var cInfo = cRepository.GetMySalesRepInfo( includeTerritory );

				return new List<SelectListItemPlus>() { new SelectListItemPlus() { Value = cInfo.ID.ToString(), Text = cInfo.Text, Extra = cInfo.Extra } };
			}
		}

		public static IEnumerable<SelectListItemPlus> GetCompanyDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetFullCompanyList( companyType, includeTerritory )
					.Select( c => new SelectListItemPlus() { Value = c.ID.ToString(), Text = c.Text, Extra = c.Extra } )
					.ToList();
				if( includeBlank )
				{
					theList.Insert( 0, new SelectListItemPlus() );
				}
				return theList;
			}
		}

		public static IEnumerable<SelectListItem> GetCompanyTypeDDList()
		{
			return PaoliWebUser.PaoliCompanyType.CompanyTypeList.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );
		}

		public static IEnumerable<SelectListItem> GetCompanyTypeFilterList()
		{
			return new List<SelectListItem>() { new SelectListItem() { Text = "All", Value = "0", Selected = true } }.Union( GetCompanyTypeDDList() );
		}

		public delegate IEnumerable<SelectListItem> GetTerritoryListFunction( bool addCompany = false );

		public static IEnumerable<SelectListItem> GetTerritoryDDList( bool addCompany = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				return new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true } }
					.Union( cRepository.GetTerritoryList().Select( t => new SelectListItem() { Value = t.TerritoryID.ToString(), Text = t.Name + ( addCompany ? ( " - " + t.SalesRepCompany ) : "" ) } ) );
			}
		}

		public static IEnumerable<SelectListItem> GetThisTerritoryAsDDItem( bool addCompany = false )
		{
			using( var uRepository = new UserRepository() )
			{
				var cInfo = uRepository.GetCurrentTerritoryInfo( addCompany );

				return new List<SelectListItem>() { new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } };
			}
		}

		public static IEnumerable<SelectListItem> GetTripGroupDDList()
		{
			return new List<SelectListItem>() {
				new SelectListItem() { Text = "Ohana", Value = "Ohana" },
				new SelectListItem() { Text = "Shaka", Value = "Shaka" },
				new SelectListItem() { Text = "Aloha", Value = "Aloha" },
				new SelectListItem() { Text = "Mahalo", Value = "Mahalo" }
			};
		}

		public static int GetMyTerritoryID()
		{
			using( var cRepo = new CompanyRepository() )
			{
				var info = cRepo.GetMyTerritoryInfo( PaoliWebUser.CurrentUser.UserId );

				return info.TerritoryID;
			}
		}
	}
}
