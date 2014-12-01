using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace PDWInfrastructure
{
    public class PaoliWebUser : IPrincipal
    {
		public static class PaoliCompanyType
		{
			public const int Paoli = 1;
			public const int PaoliRepGroup = 2;	// this needs to change in Territory partial class as well - can't access this at that level
			public const int Dealer = 3;
			public const int EndUser = 4;
			public const int AandDUser = 5;

			public static Dictionary<int, string> CompanyTypeList
			{
				get
				{
					return new Dictionary<int, string>() {
						{ Paoli, "Paoli" },
						{ PaoliRepGroup, "Paoli Rep Group" },
						{ Dealer, "Dealer" },
						{ EndUser, "End User" },
						{ AandDUser, "A&D" },
					};
				}
			}

			public static Dictionary<int, List<int>> CompanyTypeAllowedUsers
			{
				get
				{
					return new Dictionary<int, List<int>>()
					{
						{ Paoli, new List<int>() { PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
							PaoliWebRole.PaoliMemberMarketing, PaoliWebRole.PaoliMemberSpecTeam,
							PaoliWebRole.PaoliMemberCustomerService, PaoliWebRole.PaoliMemberSales } },
						{ PaoliRepGroup, new List<int>() { PaoliWebRole.PaoliSalesRep } },
						{ Dealer, new List<int>() { PaoliWebRole.DealerPrincipal, PaoliWebRole.DealerSalesRep,
							PaoliWebRole.DealerDesigner, PaoliWebRole.DealerAdmin } },
						{ EndUser, new List<int>() { PaoliWebRole.EndUser } },
						{ AandDUser, new List<int>() { PaoliWebRole.AandDUser } }
					};
				}
			}

			public static List<int> HasTerritory
			{
				get
				{
					return new List<int>() { PaoliRepGroup, Dealer };
				}
			}
		}

		public static class PaoliWebRole
		{
			public const int SuperAdmin = 1;
			public const int PaoliMemberAdmin = 2;
			public const int PaoliMemberMarketing = 3;
			public const int PaoliMemberSpecTeam = 4;
			public const int PaoliMemberCustomerService = 5;
			public const int PaoliMemberSales = 6;
			public const int PaoliSalesRep = 7;
			public const int DealerPrincipal = 8;
			public const int DealerSalesRep = 9;
			public const int DealerDesigner = 10;
			public const int DealerAdmin = 11;
			public const int EndUser = 12;
			public const int AandDUser = 13;

			public static Dictionary<int, string> RoleList
			{
				get
				{
					return new Dictionary<int, string>() {
						{ SuperAdmin,					"Super Admin" },				
						{ PaoliMemberAdmin,				"Paoli Member - Admin" },			
						{ PaoliMemberMarketing,			"Paoli Member - Marketing" },		
						{ PaoliMemberSpecTeam,			"Paoli Member - Spec Team" },		
						{ PaoliMemberCustomerService,	"Paoli Member - Customer Service" },
						{ PaoliMemberSales,				"Paoli Member - Sales" },			
						{ PaoliSalesRep,				"Paoli Sales Rep" },			
						{ DealerAdmin,					"Dealer Admin" },			
						{ DealerPrincipal,				"Dealer Principal" },			
						{ DealerSalesRep,				"Dealer Sales Rep" },			
						{ DealerDesigner,				"Dealer Designer" },			
						{ EndUser,						"End User" },			
						{ AandDUser,					"A&D User" },			
                    };
				}
			}

			public static string RoleHomePage( int role )
			{
				UrlHelper u = new UrlHelper( HttpContext.Current.Request.RequestContext );

				return u.Action( "Index", "Home" );
			}

			public static bool IsDealerAccountType( int role )
			{
				return new List<int>() { PaoliWebRole.DealerAdmin, PaoliWebRole.DealerDesigner, 
					PaoliWebRole.DealerPrincipal, PaoliWebRole.DealerSalesRep }.Contains( role );
			}
		}

		private class PaoliWebIdentity : IIdentity
		{
			public PaoliWebIdentity( string email, string authType )
			{
				Name = email;
				AuthenticationType = authType;
			}

			#region IIdentity Members

			public string Name { get; private set; }
			public string AuthenticationType { get; private set; }
			public bool IsAuthenticated { get { return true; } }

			#endregion
		}

		public PaoliWebUser( string email, string authType, int userId, string firstName, string lastName, int role, 
			bool hasTempPassword, DateTime? previousLogin, string companyName, string phoneNumber )
        {
			_identity = new PaoliWebIdentity( email, authType );
            UserId = userId;
			UserRole = PaoliWebRole.RoleList[role];
			AccountType = role;
			FirstName = firstName;
			LastName = lastName;
			_hasTempPassword = hasTempPassword;
			_previousLogin = previousLogin;
			CompanyName = companyName;
			PhoneNumber = phoneNumber;
        }

		private PaoliWebIdentity _identity { get; set; }
		public int UserId { get; private set; }
		public string FullName { get { return FirstName + ' ' + LastName; } }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		private string UserRole { get; set; }
		private int AccountType { get; set; }
		private bool _hasTempPassword { get; set; }
		public string CompanyName { get; private set; }
		public string PhoneNumber { get; private set; }
		public string PreviousLogin
		{
			get
			{
				if( !_previousLogin.HasValue )
				{
					return "";
				}
				var dts = DateTime.UtcNow - _previousLogin.Value;
				if( dts.Days > 0 )
				{
					return string.Format( "{0} days", dts.Days );
				}
				if( dts.Hours > 0 )
				{
					return string.Format( "{0} hours", dts.Hours );
				}
				if( dts.Minutes > 0 )
				{
					return string.Format( "{0} minutes", dts.Minutes );
				}
				return string.Format( "moments" );
			}
		}
		private DateTime? _previousLogin { get; set; }

		#region IPrincipal Members

		public IIdentity Identity
		{
			get { return _identity; }
		}

		public bool IsInRole( string role )
		{
			return UserRole == role;
		}

		#endregion

		public bool IsInRole( int role )
		{
			return AccountType == role;
		}

		private bool OneOfRoles( params int[] roleList )
		{
			return roleList.Any( r => IsInRole( r ) );
		}

		public static PaoliWebUser CurrentUser
		{
			get
			{
				return HttpContext.Current.User as PaoliWebUser;
			}
		}

		public string EmailAddress
		{
			get
			{
				return _identity.Name;
			}
		}

		public string HomePage
		{
			get
			{
				return PaoliWebRole.RoleHomePage( AccountType );
			}
		}

		public bool HasTempPassword
		{
			get
			{
				return _hasTempPassword;
			}
		}

		public bool IsNewLayout { get { return true; } }
		public bool HasAdminLink { get { return CanManageUsers || CanManageCompanies || CanManageImages || CanManageImport || CanManageSearchLog; } }

		public bool IsSuperAdmin { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }
		public bool IsDealerUser { get { return OneOfRoles( PaoliWebRole.DealerAdmin, PaoliWebRole.DealerDesigner, 
			PaoliWebRole.DealerPrincipal, PaoliWebRole.DealerSalesRep ); } }
		public bool IsPaoliUser { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberMarketing, PaoliWebRole.PaoliMemberSpecTeam, PaoliWebRole.PaoliMemberCustomerService, 
					PaoliWebRole.PaoliMemberSales ); } }

		public bool CanBeLoggedIn { get { return true; } }
		public bool CanSeeMainUsers { get { return CanManageUsers || CanManageCompanies; } }
		public bool CanSeeMainProducts { get { return CanManageImport || CanManageImages || CanManagePrintMaterial || CanManageTypicals || CanManageSearchLog || 
			CanViewSpecRequests || CanManageCollateral; } }

		public bool CanSeeMainCMSLink { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberMarketing ); } }

		public bool CanManageImport { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }
		public bool CanManageNewHomePage { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }
		public bool CanManageImages { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin, PaoliWebRole.PaoliMemberMarketing ); } }
		public bool CanManagePrintMaterial { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }
		public bool CanManageTypicals { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberMarketing, PaoliWebRole.PaoliMemberSpecTeam, PaoliWebRole.PaoliMemberCustomerService, 
					PaoliWebRole.PaoliMemberSales ); } }
		public bool CanViewSpecRequests { get { return CanManageTypicals || IsDealerUser || OneOfRoles( PaoliWebRole.PaoliSalesRep ); } }
		public bool CanAddSpecRequests { get { return CanManageTypicals || IsDealerUser || OneOfRoles( PaoliWebRole.PaoliSalesRep ); } }
		public bool CanReOpenSpecRequests { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ); } }

		public bool CanManageSearchLog { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin, PaoliWebRole.PaoliMemberMarketing ); } }

		public bool CanManageUsers { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }
		public bool CanManageCompanies { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }

		public bool CanSeeYouniversity { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ) || IsPaoliUser; } }
		public bool CanSeeTheScoop { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ) || IsPaoliUser; } }
		public bool CanSeeNewsUpdates { get { return IsPaoliUser || IsDealerUser; } }
		public bool HasContacts { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ) || IsDealerUser; } }

		public bool CanManageCollateral { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberMarketing ); } }
		public bool CanManageOrders { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberCustomerService, PaoliWebRole.PaoliMemberSales,
					PaoliWebRole.PaoliMemberMarketing, PaoliWebRole.PaoliMemberSpecTeam ); } }
		public bool CanAddOrders { get { return CanManageOrders || OneOfRoles( PaoliWebRole.PaoliSalesRep ) || IsDealerUser; } }

		public bool CanManageArticles { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }

		public bool CanManageECollateral { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ) || IsPaoliUser; } }
		public bool CanReviewECollateral { get { return IsPaoliUser; } }
		public bool CanAddECTemplate { get { return IsPaoliUser; } }
		public bool CanManageAllECollateral { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberMarketing ); } }

		public bool CanViewReports { get { return IsPaoliUser; } }

		public bool CanViewLeadTimes { get { return CanBeLoggedIn; } }
		public bool CanManageLeadTimes { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }

		public bool CanViewMyTerritory { get { return OneOfRoles( PaoliWebRole.PaoliSalesRep ); } }
		public bool CanViewMyCompany { get { return IsDealerUser; } }
		public bool CanViewMyCompanyUserList { get { return CanViewMyCompany || CanViewMyTerritory; } }

		public string ProductsHomePage
		{
			get
			{
				UrlHelper u = new UrlHelper( HttpContext.Current.Request.RequestContext );
				switch( AccountType )
				{
					case PaoliWebRole.SuperAdmin:
						return u.Action( "Index", "Import" );
					case PaoliWebRole.PaoliMemberAdmin:
						return u.Action( "Index", "Import" );
					case PaoliWebRole.PaoliMemberMarketing:
						return u.Action( "Images", "Import" );
					case PaoliWebRole.PaoliMemberSpecTeam:
						return u.Action( "Orders", "Collateral" );
					case PaoliWebRole.PaoliMemberCustomerService:
						return u.Action( "Orders", "Collateral" );
					case PaoliWebRole.PaoliMemberSales:
						return u.Action( "Orders", "Collateral" );
					case PaoliWebRole.PaoliSalesRep:
						return u.Action( "ViewOrders", "Collateral" );
					case PaoliWebRole.DealerPrincipal:
						return u.Action( "ViewOrders", "Collateral" );
					case PaoliWebRole.DealerSalesRep:
						return u.Action( "ViewOrders", "Collateral" );
					case PaoliWebRole.DealerDesigner:
						return u.Action( "ViewOrders", "Collateral" );
					case PaoliWebRole.DealerAdmin:
						return u.Action( "ViewOrders", "Collateral" );
					case PaoliWebRole.EndUser:
					case PaoliWebRole.AandDUser:
						break;
				}

				return "";
			}
		}

	}
}
