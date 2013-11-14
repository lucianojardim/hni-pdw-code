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
		public static class PaoliWebRole
		{
			public const string PaoliStaffMarketing = "Paoli Staff Marketing";
			public const string PaoliStaffSpecTeam = "Paoli Staff Spec Team";
			public const string PaoliStaffSupport = "Paoli Staff Support";
			public const string SuperAdmin = "Super Admin";
			public const string PaoliStaffSalesRep = "Paoli Staff Sales Rep";
			public const string DealerSalesRep = "Dealer Sales Rep";
            public const string DealerPrinciple = "Dealer Principle";

            public static List<string> RoleList
            {
                get
                {
                    return new List<string>() {
                        PaoliStaffMarketing,
                        PaoliStaffSpecTeam,
                        PaoliStaffSupport,
                        SuperAdmin,
                        PaoliStaffSalesRep,
                        DealerSalesRep,
                        DealerPrinciple           
                    };
                }
            }

			public static string RoleHomePage( string role )
			{
				UrlHelper u = new UrlHelper( HttpContext.Current.Request.RequestContext );

				switch( role )
				{
					case SuperAdmin:
						return u.Action( "Index", "Import" );
				}

				return u.Action( "MyAccount", "User" );
			}
		}

		public static class PaoliUserType
		{
			public const string DealerPrinciple = "Dealer Principle";
			public const string DealerSalesRep = "Dealer Sales Rep";
			public const string Designer = "Designer";
			public const string DealerAccounting = "Dealer Accounting";
			public const string PaoliSalesRep = "Paoli Sales Rep";
			public const string PaoliMember = "Paoli Member";

			public static List<string> UserTypeList
			{
				get
				{
					return new List<string>() {
                        Designer,
                        DealerAccounting,
                        PaoliSalesRep,
                        PaoliMember,
                        DealerSalesRep,
                        DealerPrinciple           
                    };
				}
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

        public PaoliWebUser(string email, string authType, int userId, string fullName, string role)
        {
			_identity = new PaoliWebIdentity( email, authType );
            UserId = userId;
			FullName = fullName;
			UserRole = role;
        }

		private PaoliWebIdentity _identity { get; set; }
		public int UserId { get; private set; }
		public string FullName { get; private set; }
		private string UserRole { get; set; }

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

		public bool OneOfRoles( string roleList )
		{
			var roleSet = roleList.Split( ',' ).Select( s => s.Trim() );

			return roleSet.Any( r => IsInRole( r ) );
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
				return PaoliWebRole.RoleHomePage( UserRole );
			}
		}
	}
}
