﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Users;
using PDWInfrastructure.Attributes;

namespace ProductDataWarehouse.Controllers
{
	public class UserController : BaseController
    {
		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public JsonResult FullUserList( UserTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var uRepository = new UserRepository() )
			{
				var results = uRepository.GetFullUserList(
					param, true, out totalCount, out filteredCount );

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

		[PaoliAuthorize( "CanViewMyTerritoryUserList" )]
		[TempPasswordCheck]
		public JsonResult MyTerritoryUserList( UserTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var uRepository = new UserRepository() )
			{
				var results = uRepository.GetFullUserList(
					param, false, out totalCount, out filteredCount );

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

		[PaoliAuthorize( "CanViewMyCompanyUserList" )]
		[TempPasswordCheck]
		public JsonResult MyCompanyUserList( UserTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var uRepository = new UserRepository() )
			{
				var results = uRepository.GetFullUserList(
					param, false, out totalCount, out filteredCount );

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

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View( new UserInformation() { Enabled = true, SendWelcomeEmail = true, IsActive = true } );
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Add( UserInformation uInfo, HttpPostedFileBase UserImage )
		{
			if( ModelState.IsValid )
			{
				using( var uRepository = new UserRepository() )
				{
					try
					{
						uRepository.AddUser( uInfo, UserImage != null ? UserImage.InputStream : null, UserImage != null ? UserImage.FileName : null, PaoliWebUser.CurrentUser.UserId );

						return RedirectToAction( "Manage" );
					}
					catch( ApplicationException e )
					{
						ModelState.AddModelError( "", e.Message );
					}
				}
			}

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			using( var uRepository = new UserRepository() )
			{
				var uInfo = uRepository.GetUser( id );

				return View( uInfo );
			}
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit( UserInformation uInfo, HttpPostedFileBase UserImage )
		{
			if( ModelState.IsValid )
			{
				using( var uRepository = new UserRepository() )
				{
					try
					{
						uRepository.UpdateUser( uInfo, UserImage != null ? UserImage.InputStream : null, UserImage != null ? UserImage.FileName : null );

						return RedirectToAction( "Manage" );
					}
					catch( ApplicationException e )
					{
						ModelState.AddModelError( "", e.Message );
					}
				}

			}

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public JsonResult SendWelcomeEmail( int id )
		{
			using( var uRepository = new UserRepository() )
			{
				bool bSuccess = uRepository.SendWelcomeEmail( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[TempPasswordCheck]
		public JsonResult ResetPassword( int id )
		{
			using( var uRepository = new UserRepository() )
			{
				bool bSuccess = uRepository.ResetUserPassword( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult MyAccount()
		{
			using( var uRepository = new UserRepository() )
			{
				var uInfo = uRepository.GetUser( PaoliWebUser.CurrentUser.UserId );

				return View( viewName: "NewMyAccount", model: uInfo );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult MyAccount( MyAccountInfo uInfo, HttpPostedFileBase UserImage )
		{
			if( ModelState.IsValid )
			{
				using( var uRepository = new UserRepository() )
				{
					uRepository.UpdateUser( uInfo, UserImage != null ? UserImage.InputStream : null, UserImage != null ? UserImage.FileName : null );

					ViewBag.AccountUpdateSuccess = true;

					// yes, do this - it forces the account to be retrieved from the database again, but keeps the viewbag entry
					return MyAccount();
				}
			}

			return View( viewName: "NewMyAccount", model: uInfo );
		}

		[PaoliAuthorize( "CanManageTerritories" )]
		public ActionResult Territories()
		{
			using( var cRepository = new CompanyRepository() )
			{
				return View();
			}
		}

		[PaoliAuthorize( "CanManageTerritories" )]
		public ActionResult MyTerritoryInfo( int id )
		{
			using( var cRepository = new CompanyRepository() )
			{
				return View( "MyTerritory", cRepository.GetTerritoryInfo( id ) );
			}
		}

		[PaoliAuthorize( "CanViewMyTerritory" )]
		public ActionResult MyTerritory()
		{
			using( var cRepository = new CompanyRepository() )
			{
				return View( cRepository.GetMyTerritoryInfo( PaoliWebUser.CurrentUser.UserId ) );
			}
		}

		[PaoliAuthorize( "CanViewMyCompanyInfo" )]
		public ActionResult MyCompanyInfo( int id )
		{
			using( var cRepository = new CompanyRepository() )
			{
				return View( "MyCompany", cRepository.GetMyCompanyInfo( companyId: id ) );
			}
		}

		[PaoliAuthorize( "CanViewMyCompany" )]
		public ActionResult MyCompany()
		{
			using( var cRepository = new CompanyRepository() )
			{
				return View( "MyCompany", cRepository.GetMyCompanyInfo( userId: PaoliWebUser.CurrentUser.UserId ) );
			}
		}

		[PaoliAuthorize( "CanViewMyTerritory" )]
		public ActionResult DealerTripInfo( int id )
		{
			using( var cRepo = new CompanyRepository() )
			{
				return View( "MyTripInfo", cRepo.GetCompanyTripInfo( id ) );
			}
		}

		[PaoliAuthorize( "CanViewMyTerritory" )]
		public ActionResult TerritoryTripInfo()
		{
			using( var cRepo = new CompanyRepository() )
			{
				return View( cRepo.GetTerritoryTripInfo( PaoliWebUser.CurrentUser.UserId ) );
			}
		}

		[PaoliAuthorize( "IsTripIncentive" )]
		public ActionResult MyTripInfo()
		{
			using( var cRepo = new CompanyRepository() )
			{
				return View( cRepo.GetMyCompanyTripInfo( PaoliWebUser.CurrentUser.UserId ) );
			}
		}

		[PaoliAuthorize( "CanChangeMyCompanyUserImage" )]
		[HttpPost]
		public ActionResult MyCompanyImage( int TheCompanyID, HttpPostedFileBase CompanyImage )
		{
			using( var cRepository = new CompanyRepository() )
			{
				if( CompanyImage != null )
				{
					if( CompanyImage.ContentLength <= 500 * 1024 )
					{
						cRepository.UpdateImage( TheCompanyID, CompanyImage != null ? CompanyImage.InputStream : null, CompanyImage != null ? CompanyImage.FileName : null );
					}
					else
					{
						ViewBag.ImageTooBig = true;
					}
				}

				if( PaoliWebUser.CurrentUser.CanViewMyTerritory )
				{
					return MyCompanyInfo( TheCompanyID );
				}

				return MyCompany();
			}
		}

		[PaoliAuthorize( "CanUpdateDealerOnMyTerritory" )]
		public JsonResult GetDealerContactInfo( int userId )
		{
			using( var aRepository = new UserRepository() )
			{
				return Json( aRepository.GetDealerContactInfo( userId ),
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public JsonResult DealershipContact( DealerContactInfo uInfo )
		{
			using( var aRepository = new UserRepository() )
			{
				try
				{
					aRepository.UpdateDealerContact( PaoliWebUser.CurrentUser.UserId, uInfo );

					return Json( new
					{
						success = true
					},
										JsonRequestBehavior.AllowGet );
				}
				catch( ApplicationException e )
				{
					return Json( new { success = false, message = e.Message } );
				}
			}
		}

		[PaoliAuthorize( "CanUpdateDealerOnMyTerritory" )]
		public JsonResult RequestEmailChange( int userId, string emailAddress )
		{
			using( var aRepository = new UserRepository() )
			{
				bool bSuccess = aRepository.RequestUserEmailChange( PaoliWebUser.CurrentUser.UserId, userId, emailAddress );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public JsonResult RequestDeactiviation( int userId, string reason )
		{
			using( var aRepository = new UserRepository() )
			{
				bool bSuccess = aRepository.RequestUserDeactiviation( PaoliWebUser.CurrentUser.UserId, userId, reason );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public ActionResult ChangePW()
		{
			return View( viewName: "NewChangePW" );
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePW( ChangePWDetail pwDetail )
		{
			if( ModelState.IsValid )
			{
				try
				{
					using( var uRepository = new UserRepository() )
					{
						uRepository.ChangePassword( pwDetail );

						return RedirectToAction( "Index", "Home" );
					}
				}
				catch( ApplicationException ex )
				{
					ModelState.AddModelError( "OldPassword", ex.Message );
				}
			}

			return View( viewName: "NewChangePW" );
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		public ActionResult Notifications()
		{
			using( var uRepository = new UserRepository() )
			{
				return View( uRepository.GetUserSubscriptionSummary( PaoliWebUser.CurrentUser.UserId ) );
			}
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Notifications( UserSubscriptionSummary uSummary )
		{
			if( ModelState.IsValid )
			{
				try
				{
					using( var uRepository = new UserRepository() )
					{
						uRepository.UpdateUserSubscriptions( uSummary );

						ViewBag.AccountUpdateSuccess = true;

						// yes, do this - it forces the account to be retrieved from the database again, but keeps the viewbag entry
						return Notifications();
					}
				}
				catch( ApplicationException e )
				{
					ModelState.AddModelError( "", e.Message );
				}
			}

			return View( uSummary );
		}

		public static IEnumerable<SelectListItem> GetStateDDList()
		{
			return new List<SelectListItem>() {
				new SelectListItem()  { Text = "", Value = ""   },
				new SelectListItem()  { Text = "Alabama", Value = "AL"   },
				new SelectListItem()  { Text = "Alaska", Value = "AK"   },
				new SelectListItem()  { Text = "Arizona", Value = "AZ"   },
				new SelectListItem()  { Text = "Arkansas", Value = "AR"   },
				new SelectListItem()  { Text = "California", Value = "CA"   },
				new SelectListItem()  { Text = "Colorado", Value = "CO"   },
				new SelectListItem()  { Text = "Connecticut", Value = "CT"   },
				new SelectListItem()  { Text = "Delaware", Value = "DE"   },
				new SelectListItem()  { Text = "District of Columbia", Value = "DC"   },
				new SelectListItem()  { Text = "Florida", Value = "FL"   },
				new SelectListItem()  { Text = "Georgia", Value = "GA"   },
				new SelectListItem()  { Text = "Hawaii", Value = "HI"   },
				new SelectListItem()  { Text = "Idaho", Value = "ID"   },
				new SelectListItem()  { Text = "Illinois", Value = "IL"   },
				new SelectListItem()  { Text = "Indiana", Value = "IN"   },
				new SelectListItem()  { Text = "Iowa", Value = "IA"   },
				new SelectListItem()  { Text = "Kansas", Value = "KS"   },
				new SelectListItem()  { Text = "Kentucky", Value = "KY"   },
				new SelectListItem()  { Text = "Louisiana", Value = "LA"   },
				new SelectListItem()  { Text = "Maine", Value = "ME"   },
				new SelectListItem()  { Text = "Maryland", Value = "MD"   },
				new SelectListItem()  { Text = "Massachusetts", Value = "MA"   },
				new SelectListItem()  { Text = "Michigan", Value = "MI"   },
				new SelectListItem()  { Text = "Minnesota", Value = "MN"   },
				new SelectListItem()  { Text = "Mississippi", Value = "MS"   },
				new SelectListItem()  { Text = "Missouri", Value = "MO"   },
				new SelectListItem()  { Text = "Montana", Value = "MT"   },
				new SelectListItem()  { Text = "Nebraska", Value = "NE"   },
				new SelectListItem()  { Text = "Nevada", Value = "NV"   },
				new SelectListItem()  { Text = "New Hampshire", Value = "NH"   },
				new SelectListItem()  { Text = "New Jersey", Value = "NJ"   },
				new SelectListItem()  { Text = "New Mexico", Value = "NM"   },
				new SelectListItem()  { Text = "New York", Value = "NY"   },
				new SelectListItem()  { Text = "North Carolina", Value = "NC"   },
				new SelectListItem()  { Text = "North Dakota", Value = "ND"   },
				new SelectListItem()  { Text = "Ohio", Value = "OH"   },
				new SelectListItem()  { Text = "Oklahoma", Value = "OK"   },
				new SelectListItem()  { Text = "Oregon", Value = "OR"   },
				new SelectListItem()  { Text = "Pennsylvania", Value = "PA"   },
				new SelectListItem()  { Text = "Rhode Island", Value = "RI"   },
				new SelectListItem()  { Text = "South Carolina", Value = "SC",   },
				new SelectListItem()  { Text = "South Dakota", Value = "SD",   },
				new SelectListItem()  { Text = "Tennessee", Value = "TN"   },
				new SelectListItem()  { Text = "Texas", Value = "TX"   },
				new SelectListItem()  { Text = "Utah", Value = "UT"   },
				new SelectListItem()  { Text = "Vermont", Value = "VT"   },
				new SelectListItem()  { Text = "Virginia", Value = "VA"   },
				new SelectListItem()  { Text = "Washington", Value = "WA"   },
				new SelectListItem()  { Text = "West Virginia", Value = "WV"   },
				new SelectListItem()  { Text = "Wisconsin", Value = "WI"   },
				new SelectListItem()  { Text = "Wyoming", Value = "WY"			   }
			};
		}

		public static IEnumerable<SelectListItem> GetTierGroupDDList( bool includeBlank = false )
		{
			var list = PaoliWebUser.PaoliTierGroup.TierGroupList
				.Select( tg => new SelectListItem() { Text = tg, Value = tg } )
				.ToList();

			if( includeBlank )
			{
				list.Insert( 0, new SelectListItem() );
			}

			return list;
		}

		public static IEnumerable<SelectListItem> GetUserRoleDDList()
		{
			var theList = PaoliWebUser.PaoliWebRole.RoleList.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );

			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
				theList = theList.Where( i => i.Value != PaoliWebUser.PaoliWebRole.SuperAdmin.ToString() );

			return theList;
		}

		public static IEnumerable<SelectListItem> GetAddressTypeDDList()
		{
			return new List<SelectListItem>() {
				new SelectListItem() { Text = "Business Address", Value = UserRepository.AddressTypes.Business.ToString() },
				new SelectListItem() { Text = "Home Address", Value = UserRepository.AddressTypes.Home.ToString() }
			};
		}

		public static IEnumerable<SelectListItem> GetUserRoleFilterList()
		{
			return new List<SelectListItem>() { new SelectListItem() { Text = "All", Value = "0", Selected = true } }.Union( GetUserRoleDDList() );
		}

		public static IEnumerable<SelectListItem> GetDealerUserRoleDDList()
		{
			var theList = PaoliWebUser.PaoliWebRole.RoleList
				.Where( r => PaoliWebUser.PaoliWebRole.IsDealerAccountType( r.Key ) )
				.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );

			return theList;
		}

		private static List<SelectListItem> GetJustUserDDList( List<int> accountTypes, bool enabledOnly )
		{
			using( var uRepository = new UserRepository() )
			{
				return uRepository.GetUserListForAccountType( accountTypes, enabledOnly )
					.Select( u => new SelectListItem() { Value = u.UserID.ToString(), Text = u.FullName } ).ToList();
			}
		}

		public static IEnumerable<SelectListItem> GetPaoliMemberDDList( bool includeBlank )
		{
			var theList = GetJustUserDDList( new List<int>() { PaoliWebUser.PaoliWebRole.PaoliMemberAdmin, 
				PaoliWebUser.PaoliWebRole.PaoliMemberMarketing, PaoliWebUser.PaoliWebRole.PaoliMemberSpecTeam, 
				PaoliWebUser.PaoliWebRole.PaoliMemberCustomerService, PaoliWebUser.PaoliWebRole.PaoliMemberSales }, true );

			if( includeBlank )
			{
				theList.Insert( 0, new SelectListItem() );
			}

			return theList;
		}

		public static IEnumerable<SelectListItem> GetPaoliMemberDDListForCompany( bool includeBlank )
		{
			var theList = GetJustUserDDList( new List<int>() { PaoliWebUser.PaoliWebRole.PaoliMemberCustomerService, PaoliWebUser.PaoliWebRole.PaoliMemberSales }, true );

			if( includeBlank )
			{
				theList.Insert( 0, new SelectListItem() );
			}

			return theList;
		}

		public static IEnumerable<SelectListItem> GetUserDDList( int accountType )
		{
			var theList = GetJustUserDDList( new List<int>() { accountType }, false );
			if( accountType == PaoliWebUser.PaoliWebRole.PaoliMemberSpecTeam )
			{
				theList.Insert( 0, new SelectListItem() { Text = "Not Assigned", Value = "0" } );
			}
			else
			{
				theList.Insert( 0, new SelectListItem() );
			}
			return theList;
		}

		public JsonResult GetPaoliSalesRepListForTerritory( int territoryId, bool enabledOnly )
		{
			using( var uRepository = new UserRepository() )
			{
				var theList = uRepository.GetSalesRepListForTerritory( territoryId, enabledOnly ).ToList();

				theList.Insert( 0, null );

				return Json( new
				{
					theList = theList
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetPaoliSalesRepListForCompany( int companyId, bool enabledOnly )
		{
			using( var uRepository = new UserRepository() )
			{
				var theList = uRepository.GetUserListForAccountType( companyId, PaoliWebUser.PaoliWebRole.PaoliSalesRep, enabledOnly );

				return Json( new
				{
					theList = theList
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		public JsonResult GetDealerSalesRepListForCompany( int companyId, bool enabledOnly )
		{
			using( var uRepository = new UserRepository() )
			{
				var theList = uRepository.GetUserListForAccountType( companyId, 
					new List<int>() { PaoliWebUser.PaoliWebRole.DealerAdmin, 
						PaoliWebUser.PaoliWebRole.DealerDesigner, 
						PaoliWebUser.PaoliWebRole.DealerPrincipal, 
						PaoliWebUser.PaoliWebRole.DealerSalesRep 
					}, enabledOnly );

				return Json( new
				{
					theList = theList
				},
					JsonRequestBehavior.AllowGet );
			}
		}
	}
}
