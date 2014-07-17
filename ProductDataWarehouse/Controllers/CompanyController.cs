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
    public class CompanyController : Controller
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

			CompanyRepository cRepository = new CompanyRepository();

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
				try
				{
					CompanyRepository cRepository = new CompanyRepository();

					cRepository.AddCompany( uInfo );

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

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			CompanyRepository cRepository = new CompanyRepository();

			var uInfo = cRepository.GetCompany( id );

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit( CompanyInformation uInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					CompanyRepository cRepository = new CompanyRepository();

					cRepository.UpdateCompany( uInfo );

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

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult Delete( int id )
		{
			CompanyRepository cRepository = new CompanyRepository();

			bool bSuccess = cRepository.DeleteCompany( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		public JsonResult GetTypeListForCompany( int companyId )
		{
			CompanyRepository cRepository = new CompanyRepository();

			var uInfo = cRepository.GetCompany( companyId );

			var theList = PaoliWebUser.PaoliWebRole.RoleList
				.Where( r => PaoliWebUser.PaoliCompanyType.CompanyTypeAllowedUsers[uInfo.CompanyType].Contains( r.Key ) )
				.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );

			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
				theList = theList.Where( i => i.Value != PaoliWebUser.PaoliWebRole.SuperAdmin.ToString() );

			return Json( new
			{
				theList = theList
			},
				JsonRequestBehavior.AllowGet );
			
		}

		public JsonResult GetShippingAddress( int? companyID, int? userId )
		{
			if( userId.HasValue )
				return Json( ( new UserRepository() ).GetUserAddress( userId.Value ), JsonRequestBehavior.AllowGet );
			else if( companyID.HasValue )
				return Json( ( new CompanyRepository() ).GetCompanyAddress( companyID.Value ), JsonRequestBehavior.AllowGet );

			throw new Exception( "Unable to find shipping address" );
		}

		public JsonResult GetDealerList( bool includeBlank = true )
		{
			var theList = ( new CompanyRepository() ).GetFullCompanyList( PaoliWebUser.PaoliCompanyType.Dealer )
				.ToList();
			if( includeBlank )
			{
				theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
			}
			return Json( theList, JsonRequestBehavior.AllowGet );
		}

		public JsonResult GetPaoliRepGroupList( bool includeBlank = true )
		{
			var theList = ( new CompanyRepository() ).GetFullCompanyList( PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
				.ToList();
			if( includeBlank )
			{
				theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
			}
			return Json( theList, JsonRequestBehavior.AllowGet );
		}

		public JsonResult GetDealerListForSalesRep( int salesRepCompanyId, bool includeBlank = true, bool includeTerritory = false )
		{
			var theList = ( new CompanyRepository() ).GetDealerList( salesRepCompanyId, includeTerritory ).ToList();

			if( includeBlank )
			{
				theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
			}

			return Json( theList, JsonRequestBehavior.AllowGet );
		}

		public JsonResult GetDealerListForUser( bool includeBlank = true )
		{
			var theList = ( new CompanyRepository() ).GetDealerListForUser( PaoliWebUser.CurrentUser.UserId ).ToList();

			if( includeBlank )
			{
				theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
			}

			return Json( theList, JsonRequestBehavior.AllowGet );
		}

		public delegate IEnumerable<SelectListItem> GetCompanyListFunction( int? companyType = null, bool includeBlank = true, bool includeTerritory = false );

		public static IEnumerable<SelectListItem> GetThisCompanyAsDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			var cInfo = ( new UserRepository() ).GetCurrentCompanyInfo( includeTerritory );

			return new List<SelectListItem>() { new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } };
		}

		public static IEnumerable<SelectListItem> GetDealerForSalesRepDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			var theList = ( new CompanyRepository() ).GetMyDealerList().ToList();

			if( includeBlank )
			{
				theList.Insert( 0, new PDWModels.IDToTextItem() { ID = 0, Text = "" } );
			}

			return theList.Select( cInfo => new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } );
		}

		public static IEnumerable<SelectListItem> GetSalesRepForDealerDDItem( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			var cInfo = ( new CompanyRepository() ).GetMySalesRepInfo( includeTerritory );

			return new List<SelectListItem>() { new SelectListItem() { Value = cInfo.ID.ToString(), Text = cInfo.Text } };
		}

		public static IEnumerable<SelectListItem> GetCompanyDDList( int? companyType = null, bool includeBlank = true, bool includeTerritory = false )
		{
			var theList = ( new CompanyRepository() ).GetFullCompanyList( companyType, includeTerritory ).Select( c => new SelectListItem() { Value = c.ID.ToString(), Text = c.Text } ).ToList();
			if( includeBlank )
			{
				theList.Insert( 0, new SelectListItem() );
			}
			return theList;
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
			return new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true } }
				.Union( ( new CompanyRepository() ).GetTerritoryList().Select( t => new SelectListItem() { Value = t.TerritoryID.ToString(), Text = t.Name + ( addCompany ? (" - " + t.SalesRepCompany) : "" ) } ) );
		}
	}
}
