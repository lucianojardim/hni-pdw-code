using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Companies;

namespace ProductDataWarehouse.Controllers
{
    public class CompanyController : Controller
	{
		[PaoliAuthorize( "CanManageCompanies" )]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
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
		public ActionResult Add()
		{
			return View( new CompanyInformation() );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[HttpPost]
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
		public ActionResult Edit( int id )
		{
			CompanyRepository cRepository = new CompanyRepository();

			var uInfo = cRepository.GetCompany( id );

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[HttpPost]
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

		public static IEnumerable<SelectListItem> GetCompanyDDList( int? companyType = null )
		{
			var theList = ( new CompanyRepository() ).GetFullCompanyList( companyType ).Select( c => new SelectListItem() { Value = c.CompanyID.ToString(), Text = c.Name } ).ToList();
			theList.Insert( 0, new SelectListItem() );
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

		public static IEnumerable<SelectListItem> GetTerritoryDDList()
		{
			return new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true } }
				.Union( (new CompanyRepository()).GetTerritoryList().Select( t => new SelectListItem() { Value = t.TerritoryID.ToString(), Text = t.Name }) );
		}
	}
}
