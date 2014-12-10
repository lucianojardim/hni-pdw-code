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

		[PaoliAuthorize( "CanViewMyTerritory" )]
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
		public ActionResult Add( CompanyInformation uInfo )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CompanyRepository() )
				{
					cRepository.AddCompany( uInfo );

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
		public ActionResult Edit( CompanyInformation uInfo )
		{
			if( ModelState.IsValid )
			{
				using( var cRepository = new CompanyRepository() )
				{
					cRepository.UpdateCompany( uInfo );

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
					theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
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
					theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
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
					theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
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

		public delegate IEnumerable<SelectListItem> GetCompanyListFunction( int? companyType = null, bool includeBlank = true, bool includeTerritory = false );

		public static IEnumerable<SelectListItem> GetThisCompanyAsDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var uRepository = new UserRepository() )
			{
				var cInfo = uRepository.GetCurrentCompanyInfo( includeTerritory );

				return new List<SelectListItem>() { new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } };
			}
		}

		public static IEnumerable<SelectListItem> GetDealerForSalesRepDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetMyDealerList().ToList();

				if( includeBlank )
				{
					theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
				}

				return theList.Select( cInfo => new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } );
			}
		}

		public static IEnumerable<SelectListItem> GetSalesRepForDealerDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var cInfo = cRepository.GetMySalesRepInfo( includeTerritory );

				return new List<SelectListItem>() { new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } };
			}
		}

		public static IEnumerable<SelectListItem> GetCompanyDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				var theList = cRepository.GetFullCompanyList( companyType, includeTerritory ).Select( c => new SelectListItem() { Value = c.ID.ToString(), Text = c.Text } ).ToList();
				if( includeBlank )
				{
					theList.Insert( 0, new SelectListItem() );
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

		public static IEnumerable<SelectListItem> GetTerritoryDDList( bool addCompany = false )
		{
			using( var cRepository = new CompanyRepository() )
			{
				return new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true } }
					.Union( cRepository.GetTerritoryList().Select( t => new SelectListItem() { Value = t.TerritoryID.ToString(), Text = t.Name + ( addCompany ? ( " - " + t.SalesRepCompany ) : "" ) } ) );
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
	}
}
