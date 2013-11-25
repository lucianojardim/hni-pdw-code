using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.Users;

namespace ProductDataWarehouse.Controllers
{
    public class UserController : Controller
    {
		[PaoliAuthorize( "CanManageUsers" )]
		public ActionResult Manage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageUsers" )]
		public JsonResult FullUserList( UserTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			UserRepository uRepository = new UserRepository();

			var results = uRepository.GetFullUserList(
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

		[PaoliAuthorize( "CanManageUsers" )]
		public ActionResult Add()
		{
			return View( new UserInformation() );
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[HttpPost]
		public ActionResult Add( UserInformation uInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					UserRepository uRepository = new UserRepository();

					uRepository.AddUser( uInfo );

					ViewBag.CloseFancyBox = true;

					return View( uInfo );
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

		[PaoliAuthorize( "CanManageUsers" )]
		public ActionResult Edit( int id )
		{
			UserRepository uRepository = new UserRepository();

			var uInfo = uRepository.GetUser( id );

			return View( uInfo );
		}

		[PaoliAuthorize( "CanManageUsers" )]
		[HttpPost]
		public ActionResult Edit( UserInformation uInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					UserRepository uRepository = new UserRepository();

					uRepository.UpdateUser( uInfo );

					ViewBag.CloseFancyBox = true;

					return View( uInfo );
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

		[PaoliAuthorize( "CanManageUsers" )]
		public JsonResult ResetPassword( int id )
		{
			UserRepository uRepository = new UserRepository();

			bool bSuccess = uRepository.ResetUserPassword( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		[Authorize]
		public ActionResult MyAccount()
		{
			UserRepository uRepository = new UserRepository();

			var uInfo = uRepository.GetUser( PaoliWebUser.CurrentUser.UserId );

			return View( uInfo );
		}

		[Authorize]
		[HttpPost]
		public ActionResult MyAccount( MyAccountInfo uInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					UserRepository uRepository = new UserRepository();

					uRepository.UpdateUser( uInfo );

					return Redirect( PaoliWebUser.CurrentUser.HomePage );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to update your account at this time." );
				}
			}

			return View( uInfo );
		}

		[Authorize]
		public ActionResult ChangePW()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult ChangePW( ChangePWDetail pwDetail )
		{
			if( ModelState.IsValid )
			{
				try
				{
					UserRepository uRepository = new UserRepository();

					uRepository.ChangePassword( pwDetail );

					return Redirect( PaoliWebUser.CurrentUser.HomePage );
				}
				catch( ApplicationException ex )
				{
					ModelState.AddModelError( "", ex.Message );
				}
				catch( Exception )
				{
					ModelState.AddModelError( "", "Unable to change your password at this time." );
				}
			}

			return View();
		}

		public static IEnumerable<SelectListItem> GetStateDDList()
		{
			return new List<SelectListItem>() {
				new SelectListItem()  { Text = "Alabama", Value = "AL"   },
				new SelectListItem()  { Text = "Alaska", Value = "AK"   },
				new SelectListItem()  { Text = "Arizona", Value = "AZ"   },
				new SelectListItem()  { Text = "Arkansas", Value = "AR"   },
				new SelectListItem()  { Text = "California", Value = "CA"   },
				new SelectListItem()  { Text = "Colorado", Value = "CO"   },
				new SelectListItem()  { Text = "Connecticut", Value = "CT"   },
				new SelectListItem()  { Text = "Delaware", Value = "DE"   },
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

		public static IEnumerable<SelectListItem> GetUserRoleDDList()
		{
			var theList = PaoliWebUser.PaoliWebRole.RoleList.Select( u => new SelectListItem() { Value = u.Key.ToString(), Text = u.Value } );

			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
				theList = theList.Where( i => i.Value != PaoliWebUser.PaoliWebRole.SuperAdmin.ToString() );

			return theList;
		}

		public static IEnumerable<SelectListItem> GetUserRoleFilterList()
		{
			return new List<SelectListItem>() { new SelectListItem() { Text = "All", Value = "0", Selected = true } }.Union( GetUserRoleDDList() );
		}
	}
}
