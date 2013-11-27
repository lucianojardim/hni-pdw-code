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
			public const int PaoliRepGroup = 2;
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
						{ EndUser, "End Users" },
						{ AandDUser, "A&D Users" },
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
							PaoliWebRole.DealerDesigner, PaoliWebRole.DealerAccounting } },
						{ EndUser, new List<int>() { PaoliWebRole.EndUser } },
						{ AandDUser, new List<int>() { PaoliWebRole.AandDUser } }
					};
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
			public const int DealerAccounting = 11;
			public const int EndUser = 12;
			public const int AandDUser = 13;

			public const string SuperAdminRole = "Super Admin";
			public const string PaoliMemberAdminRole = "Paoli Member - Admin";
			public const string PaoliMemberMarketingRole = "Paoli Member - Marketing";
			public const string PaoliMemberSpecTeamRole = "Paoli Member - Spec Team";
			public const string PaoliMemberCustomerServiceRole = "Paoli Member - Customer Service";
			public const string PaoliMemberSalesRole = "Paoli Member - Sales";
			public const string PaoliSalesRepRole = "Paoli Sales Rep";
			public const string DealerPrincipalRole = "Dealer Principal";
			public const string DealerSalesRepRole = "Dealer Sales Rep";
			public const string DealerDesignerRole = "Dealer Designer";
			public const string DealerAccountingRole = "Dealer Accounting";
			public const string EndUserRole = "End User";
			public const string AandDUserRole = "A&D User";

			public static Dictionary<int, string> RoleList
			{
				get
				{
					return new Dictionary<int, string>() {
						{ SuperAdmin,					SuperAdminRole },				
						{ PaoliMemberAdmin,				PaoliMemberAdminRole },			
						{ PaoliMemberMarketing,			PaoliMemberMarketingRole },		
						{ PaoliMemberSpecTeam,			PaoliMemberSpecTeamRole },		
						{ PaoliMemberCustomerService,	PaoliMemberCustomerServiceRole },
						{ PaoliMemberSales,				PaoliMemberSalesRole },			
						{ PaoliSalesRep,				PaoliSalesRepRole },			
						{ DealerPrincipal,				DealerPrincipalRole },			
						{ DealerSalesRep,				DealerSalesRepRole },			
						{ DealerDesigner,				DealerDesignerRole },			
						{ DealerAccounting,				DealerAccountingRole },			
						{ EndUser,						EndUserRole },			
						{ AandDUser,					AandDUserRole },			
                    };
				}
			}

			public static string RoleHomePage( int role )
			{
				UrlHelper u = new UrlHelper( HttpContext.Current.Request.RequestContext );

				switch( role )
				{
					case SuperAdmin:
						return u.Action( "Index", "Import" );
					case PaoliMemberAdmin:
						return u.Action( "Index", "Import" );
					case PaoliMemberMarketing:
						return u.Action( "Images", "Import" );
				}

				return u.Action( "MyAccount", "User" );
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

        public PaoliWebUser(string email, string authType, int userId, string fullName, int role)
        {
			_identity = new PaoliWebIdentity( email, authType );
            UserId = userId;
			FullName = fullName;
			UserRole = PaoliWebRole.RoleList[role];
			AccountType = role;
        }

		private PaoliWebIdentity _identity { get; set; }
		public int UserId { get; private set; }
		public string FullName { get; private set; }
		private string UserRole { get; set; }
		private int AccountType { get; set; }

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

		public bool CanSeeMainUsers { get { return CanManageUsers || CanManageCompanies; } }
		public bool CanSeeMainProducts { get { return CanManageImport || CanManageImages || CanManageCollateral || CanManageTypicals || CanManageSearchLog; } }

		public bool CanSeeMainCMSLink { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }

		public bool CanManageImport { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }
		public bool CanManageImages { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin, PaoliWebRole.PaoliMemberMarketing ); } }
		public bool CanManageCollateral { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }
		public bool CanManageTypicals { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin,
					PaoliWebRole.PaoliMemberMarketing, PaoliWebRole.PaoliMemberSpecTeam, PaoliWebRole.PaoliMemberCustomerService, PaoliWebRole.PaoliSalesRep ); } }
		public bool CanManageSearchLog { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin, PaoliWebRole.PaoliMemberMarketing ); } }

		public bool CanManageUsers { get { return OneOfRoles( PaoliWebRole.SuperAdmin, PaoliWebRole.PaoliMemberAdmin ); } }
		public bool CanManageCompanies { get { return OneOfRoles( PaoliWebRole.SuperAdmin ); } }

	}
}
