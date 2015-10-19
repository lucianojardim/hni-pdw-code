using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Users;
using System.IO;
using System.Drawing;
using System.Configuration;
using PDWModels;
using PDWInfrastructure.EmailSenders;
using System.Data.Entity;

namespace PWDRepositories
{
    public class UserRepository : BaseRepository
    {
        public UserRepository()
		{
		}

		public class AddressTypes
		{
			public const int Business = 1;
			public const int Home = 2;
		}

		private UserSummary ToUserSummary( User u )
		{
			return new UserSummary()
			{
				UserID = u.UserID,
				FirstName = u.FirstName,
				LastName = u.LastName,
				CompanyName = u.Company.FullName,
				EmailAddress = u.Email,
				PhoneNumber = u.BusinessPhone,
				Enabled = u.Enabled,
				SentWelcomeEmail = u.RecWelcomeEmail,
				LastLogin = u.LastLogin,
				Title = u.Title,
				Role = PaoliWebUser.PaoliWebRole.RoleList[u.AccountType]
			};
		}

        public PaoliWebUser CreatePaoliWebUser(string email, string authType)
        {
			User user = database.Users.FirstOrDefault( u => u.Email == email );
			if( user != null )
			{
				return new PaoliWebUser( user.Email, authType, user.UserID,
					user.FirstName, user.LastName,
					user.AccountType, user.IsTempPassword, user.PreviousLogin,
					user.Company.Name, user.BusinessPhone,
					( ( user.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ) && user.Company.SignedUpForTrip && ( user.Company.CompanyTripData != null ) ) ||
					( ( user.Company.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup ) ),
					( user.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Paoli && user.AllowNewsMgmt ) );
			}

			return null;
		}

		public bool ValidateUserAccount( string userName, string password, out string homePage )
		{
			homePage = "";

			var user = database.Users
				.FirstOrDefault( u => u.Email == userName && u.Enabled );

			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			if( user == null )
			{
				if( !database.Users.Any() )
				{
					user = new User();
					user.Email = "matt.james@wddsoftware.com";
					user.FirstName = "Matt";
					user.LastName = "James";
					user.Password = enc.Encrypt( "Password!" );
					user.AccountType = PaoliWebUser.PaoliWebRole.SuperAdmin;
					user.CompanyID = database.Companies.First( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Paoli ).CompanyID;
					database.Users.Add( user );

					if( database.SaveChanges() == 0 )
					{
						return false;
					}
					else if( userName != user.Email )
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			bool bValid = ( string.Compare( enc.Decrypt( user.Password ), password, false ) == 0 ) && ( !user.Company.IsDisabled );

			homePage = PaoliWebUser.PaoliWebRole.RoleHomePage( user.AccountType );

			if( bValid )
			{
				user.UserLogins.Add( new UserLogin() { LoginDate = DateTime.UtcNow } );

				return database.SaveChanges() > 0;
			}

			return false;
		}

		private string GenerateNewPassword()
		{
			string characterList = "bcdfghjkmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ23456789!@#$%^&*()";

			string retVal = string.Empty;
			do
			{
				retVal = string.Empty;
				Random rdm = new Random();
				while( retVal.Length < 10 )
				{
					retVal += characterList[rdm.Next( characterList.Length - 1 )];
				}
			} while( !ChangePWDetail.IsGoodPassword( retVal ) );

			return retVal;
		}

		public bool AddUser( UserInformation uInfo, Stream imgStream, string fileName, int currentUserId )
		{
			if( database.Users.Any( u => u.Email == uInfo.EmailAddress ) )
			{
				throw new ApplicationException( "Email address already exists." );
			}

			User newUser = new User();

			newUser.Email = uInfo.EmailAddress;
			newUser.FirstName = uInfo.FirstName;
			newUser.LastName = uInfo.LastName;
			newUser.CompanyID = uInfo.CompanyID;
			newUser.Address1 = uInfo.Address1;
			newUser.Address2 = uInfo.Address2;
			newUser.City = uInfo.City;
			newUser.State = uInfo.State;
			newUser.Zip = uInfo.Zip;
			newUser.BusinessPhone = uInfo.BusinessPhone;
			newUser.CellPhone = uInfo.CellPhone;
			newUser.HomeAddress1 = uInfo.HomeAddress1;
			newUser.HomeAddress2 = uInfo.HomeAddress2;
			newUser.HomeCity = uInfo.HomeCity;
			newUser.HomeState = uInfo.HomeState;
			newUser.HomeZip = uInfo.HomeZip;
			newUser.HomePhone = uInfo.HomePhone;
			newUser.PersonalCellPhone = uInfo.PersonalCellPhone;
			newUser.FAX = uInfo.FaxNumber;
			newUser.Extension = uInfo.Extension;
			newUser.Title = uInfo.Title;
			newUser.AccountType = uInfo.AccountType;
			newUser.Enabled = uInfo.Enabled && uInfo.SendWelcomeEmail;
			newUser.RecWelcomeEmail = uInfo.SendWelcomeEmail;
			newUser.IsActive = uInfo.IsActive;
			newUser.AuthorCredit = uInfo.AuthorCredit;
			newUser.DefaultShippingAddress = uInfo.DefaultShippingAddress;
			newUser.ViewPerfData = uInfo.ViewPerfData;
			newUser.TierGroup = uInfo.TierGroup;
			newUser.AllowNewsMgmt = uInfo.AllowNewsMgmt;

			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			newUser.Password = enc.Encrypt( password );
			newUser.IsTempPassword = true;

			if( imgStream != null )
			{
				newUser.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( imgStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					newUser.ImageFileName ) );
			}

			newUser.UserNotification = new UserNotification()
			{
				NewCollateralOrder = true,
				NewCollateralOrderTerritory = true,
				NewCollateralOrderShipment = true,
				NewCollateralOrderShipmentTerritory = true,
				NewSpecRequest = true,
				NewSpecRequestTerritory = true,
				CompleteSpecRequest = true,
				CompleteSpecRequestTerritory = true,
				UpdateSpecRequest = true,
				UpdateSpecRequestTerritory = true,
				ReOpenSpecRequest = true,
				ReOpenSpecRequestTerritory = true,
				CompleteSpecRequestMyDealers = true,
				NewCollateralOrderMyDealers = true,
				NewCollateralOrderShipmentMyDealers = true,
				NewSpecRequestMyDealers = true,
				ReOpenSpecRequestMyDealers = true,
				UpdateSpecRequestMyDealers = true
			};

			newUser.CreatedByUser = currentUserId;
			newUser.CreatedDateTime = DateTime.UtcNow;

			database.Users.Add( newUser );

			if( database.SaveChanges() > 0 )
			{
				if( uInfo.SendWelcomeEmail )
				{
					( new NewAccountEmailSender() ).SubmitNewAccountEmail( newUser.Email, password );
				}

				var reloadedUser = database.Users
					.Include( u => u.Company )
					.FirstOrDefault( u => u.UserID == newUser.UserID );

				if( PaoliWebUser.PaoliWebRole.IsDealerAccountType( newUser.AccountType ) )
				{
					( new NewDealerUserEmailSender() ).SubmitNewUserEmail( new NewDealerUserEmailSender.EmailUserSummary()
						{
							UserID = reloadedUser.UserID,
							EmailAddress = reloadedUser.Email,
							FirstName = reloadedUser.FirstName,
							LastName = reloadedUser.LastName,
							CompanyName = reloadedUser.Company.Name,
							Address1 = reloadedUser.Address1,
							Address2 = reloadedUser.Address2,
							City = reloadedUser.City,
							State = reloadedUser.State,
							Zip = reloadedUser.Zip,
							BusinessPhone = reloadedUser.BusinessPhone,
							CellPhone = reloadedUser.CellPhone,
							Title = reloadedUser.Title,
							AccountType = PaoliWebUser.PaoliWebRole.RoleList[reloadedUser.AccountType],
							Enabled = reloadedUser.Enabled.ToString(),
							SendWelcomeEmail = reloadedUser.RecWelcomeEmail.ToString(),
							IsActive = reloadedUser.IsActive.ToString()
						} );

					MailChimpSubscribe.AddDealerEmailAddress( reloadedUser.Email, reloadedUser.FirstName, reloadedUser.LastName, reloadedUser.Company.CompanyID, 
						reloadedUser.Company.Name, reloadedUser.Company.SubCompanyIDs,
						reloadedUser.Company.PaoliMember.FirstName, reloadedUser.Company.PaoliMember.LastName, reloadedUser.UserID, 
						reloadedUser.Company.Address1, reloadedUser.Company.Address2, reloadedUser.Company.City, reloadedUser.Company.State, reloadedUser.Company.Zip,
						reloadedUser.Company.Phone, reloadedUser.Company.TerritoryID );
				}
				else if( newUser.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
				{
					MailChimpSubscribe.AddSalesRepEmailAddress( reloadedUser.Email, reloadedUser.FirstName, reloadedUser.LastName, reloadedUser.Company.Name,
						reloadedUser.CellPhone, reloadedUser.Company.Address1, reloadedUser.Company.Address2, reloadedUser.Company.City, reloadedUser.Company.State,
						reloadedUser.BusinessPhone, reloadedUser.FAX, reloadedUser.Company.TerritoryID );
				}

				// MailChimpSubscribe.AddEmailAddress( newUser.Email, newUser.AccountType );

				return true;
			}

			return false;
		}

		public UserInformation GetUser( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return new UserInformation()
			{
				UserID = eUser.UserID,
				EmailAddress = eUser.Email,
				FirstName = eUser.FirstName,
				LastName = eUser.LastName,
				CompanyID = eUser.CompanyID,
				CompanyName = eUser.Company.FullName,
				Address1 = eUser.Address1,
				Address2 = eUser.Address2,
				City = eUser.City,
				State = eUser.State,
				Zip = eUser.Zip,
				BusinessPhone = eUser.BusinessPhone,
				CellPhone = eUser.CellPhone,
				HomeAddress1 = eUser.HomeAddress1,
				HomeAddress2 = eUser.HomeAddress2,
				HomeCity = eUser.HomeCity,
				HomeState = eUser.HomeState,
				HomeZip = eUser.HomeZip,
				HomePhone = eUser.HomePhone,
				PersonalCellPhone = eUser.PersonalCellPhone,
				FaxNumber = eUser.FAX,
				Extension = eUser.Extension,
				Title = eUser.Title,
				AccountType = eUser.AccountType,
				Enabled = eUser.Enabled,
				IsActive = eUser.IsActive,
				SendWelcomeEmail = eUser.RecWelcomeEmail,
				LockAccountType = eUser.SpecRequests.Any() || eUser.SpecRequests1.Any() || eUser.SpecRequests2.Any() ||
					eUser.CollateralOrders.Any() || eUser.CollateralOrders1.Any() || eUser.CollateralOrders1_1.Any() || eUser.CollateralOrders_1.Any() ||
					eUser.CollateralOrders_2.Any() || eUser.CollateralOrders5.Any() || eUser.CollateralOrders6.Any() || eUser.CollateralOrders7.Any() ||
					eUser.eCollateralItems.Any() || eUser.eCollateralItems1.Any() || eUser.eCollateralItems2.Any() ||
					eUser.CompaniesAsPaoliMember.Any() || eUser.CompaniesAsPaoliSalesRep.Any(),
				UserImageFileName = eUser.ImageFileName,
				AuthorCredit = eUser.AuthorCredit,
				DefaultShippingAddress = eUser.DefaultShippingAddress,
				AllowNewsMgmt = eUser.AllowNewsMgmt,
				ViewPerfData = eUser.ViewPerfData,
				TierGroup = eUser.TierGroup,
				CompanyTierGroup = eUser.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? eUser.Company.TierGroup : null,
				CreatedByUser = eUser.CreatedByUser.HasValue ? eUser.User1.FullName : "Unknown",
				CreatedDateTime = eUser.CreatedDateTime
			};
		}

		public bool UpdateUser( MyAccountInfo uInfo, Stream imgStream = null, string fileName = null )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == uInfo.UserID );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			if( database.Users.Any( u => u.Email == uInfo.EmailAddress && u.UserID != uInfo.UserID ) )
			{
				throw new ApplicationException( "Unable to update user - Email address already exists." );
			}

			bool bChangeEmail = false, bWelcomeEmail = false;

			eUser.FirstName = uInfo.FirstName;
			eUser.LastName = uInfo.LastName;
			eUser.Address1 = uInfo.Address1;
			eUser.Address2 = uInfo.Address2;
			eUser.City = uInfo.City;
			eUser.State = uInfo.State;
			eUser.Zip = uInfo.Zip;
			eUser.BusinessPhone = uInfo.BusinessPhone;
			eUser.CellPhone = uInfo.CellPhone;
			eUser.HomeAddress1 = uInfo.HomeAddress1;
			eUser.HomeAddress2 = uInfo.HomeAddress2;
			eUser.HomeCity = uInfo.HomeCity;
			eUser.HomeState = uInfo.HomeState;
			eUser.HomeZip = uInfo.HomeZip;
			eUser.HomePhone = uInfo.HomePhone;
			eUser.PersonalCellPhone = uInfo.PersonalCellPhone;
			eUser.FAX = uInfo.FaxNumber;
			eUser.Extension = uInfo.Extension;
			eUser.Title = uInfo.Title;
			eUser.DefaultShippingAddress = uInfo.DefaultShippingAddress;

			string password = GenerateNewPassword();
			if( uInfo is UserInformation )
			{
				if( eUser.Email == PaoliWebUser.CurrentUser.EmailAddress )
				{
					bChangeEmail = ( eUser.Email != uInfo.EmailAddress );
				}

				eUser.Email = uInfo.EmailAddress;	// only update email address if updating the user - no updating the profile
				eUser.AccountType = ( uInfo as UserInformation ).AccountType;
				eUser.Enabled = ( uInfo as UserInformation ).Enabled;
				eUser.IsActive = ( uInfo as UserInformation ).IsActive;
				eUser.CompanyID = ( uInfo as UserInformation ).CompanyID;
				eUser.AuthorCredit = ( uInfo as UserInformation ).AuthorCredit;
				eUser.ViewPerfData = ( uInfo as UserInformation ).ViewPerfData;
				eUser.AllowNewsMgmt = ( uInfo as UserInformation ).AllowNewsMgmt;
				eUser.TierGroup = ( uInfo as UserInformation ).TierGroup;
				if( !eUser.RecWelcomeEmail && ( uInfo as UserInformation ).SendWelcomeEmail )
				{
					bWelcomeEmail = true;
					PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
					eUser.Password = enc.Encrypt( password );
					eUser.IsTempPassword = true;
					eUser.RecWelcomeEmail = true;
				}
			}

			if( imgStream != null )
			{
				eUser.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( imgStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					eUser.ImageFileName ) );
			}

			if( database.SaveChanges() > 0 )
			{
				if( bChangeEmail )
				{
					System.Web.Security.FormsAuthentication.SetAuthCookie( uInfo.EmailAddress, false );
				}
				if( bWelcomeEmail )
				{
					( new NewAccountEmailSender() ).SubmitNewAccountEmail( eUser.Email, password );
				}

				return true;
			}

			return false;
		}

		public bool SendWelcomeEmail( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			eUser.Password = enc.Encrypt( password );
			eUser.IsTempPassword = true;
			eUser.RecWelcomeEmail = true;
			eUser.Enabled = true;

			if( database.SaveChanges() > 0 )
			{
				( new NewAccountEmailSender() ).SubmitNewAccountEmail( eUser.Email, password );

				return true;
			}

			return false;
		}

		public bool ResetUserPassword( string email )
		{
			var eUser = database.Users.FirstOrDefault( u => u.Email == email && u.Enabled );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return ResetUserPassword( eUser );
		}

		public bool ResetUserPassword( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return ResetUserPassword( eUser );
		}

		private bool ResetUserPassword( User eUser )
		{
			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			eUser.Password = enc.Encrypt( password );
			eUser.IsTempPassword = true;

			if( database.SaveChanges() > 0 )
			{
				( new ResetPasswordEmailSender() ).SubmitResetPasswordEmail( eUser.Email, password );

				return true;
			}

			return false;
		}

		public IEnumerable<UserSummary> GetFullUserList( UserTableParams param, bool showInactive, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var userList = database.Users
				.Where( u => u.Enabled || showInactive )
				.AsQueryable();

			totalRecords = userList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				userList = userList.Where( i =>
					i.FirstName.Contains( param.sSearch ) ||
					i.LastName.Contains( param.sSearch ) ||
					i.Email.Contains( param.sSearch ) ||
					i.Company.Name.Contains( param.sSearch ) ||
					i.Company.BusinessUnitName.Contains( param.sSearch ) );
			}
			if( param.accountType != 0 )
			{
				userList = userList.Where( u => u.AccountType == param.accountType );
			}
			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
			{
				userList = userList.Where( i => i.AccountType != PaoliWebUser.PaoliWebRole.SuperAdmin );
			}
			if( param.territoryId.HasValue )
			{
				userList = userList.Where( c => c.Company.TerritoryID == param.territoryId.Value );
			}
			if( param.companyId.HasValue )
			{
				userList = userList.Where( u => u.CompanyID == param.companyId.Value );
				totalRecords = userList.Count();
			}
			if( param.disabledUsers ?? false )
			{
				userList = userList.Where( u => !u.Enabled && u.IsActive );
			}
			if( param.hideInactiveUsers ?? false )
			{
				userList = userList.Where( u => u.IsActive );
			}

			displayedRecords = userList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<User> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "userid":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.UserID );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.UserID );
					}
					break;
				case "firstname":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.FirstName );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.FirstName );
					}
					break;
				case "lastname":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.LastName );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.LastName );
					}
					break;
				case "emailaddress":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.Email );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.Email );
					}
					break;
				case "companyname":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.Company.Name );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.Company.Name );
					}
					break;
				case "lastlogin":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.UserLogins.Max( ul => ul.LoginDate ) );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.UserLogins.Max( ul => ul.LoginDate ) );
					}
					break;
				case "title":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.Title );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.Title );
					}
					break;
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted
				.Include( u => u.Company )
				.Include( u => u.UserLogins )
				.ToList().Select( v => ToUserSummary( v ) );
		}

		public bool ChangePassword( ChangePWDetail pwDetail )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			if( string.Compare( enc.Decrypt( eUser.Password ), pwDetail.OldPassword, false ) == 0 )
			{
				eUser.Password = enc.Encrypt( pwDetail.NewPassword );
				eUser.IsTempPassword = false;

				return database.SaveChanges() > 0;
			}

			throw new ApplicationException( "Current password does not match our records." );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( List<int> accountTypes, bool enabledOnly )
		{
			return database.Users
				.Include( u => u.Company )
				.Include( u => u.UserLogins )
				.Where( u => accountTypes.Contains( u.AccountType ) || !accountTypes.Any() || accountTypes.Contains( 0 ) )
				.Where( u => u.IsActive || !enabledOnly )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public IEnumerable<UserSummary> GetSalesRepListForTerritory( int territoryId, bool enabledOnly )
		{
			return database.Users
				.Include( u => u.Company )
				.Include( u => u.UserLogins )
				.Where( u => u.Company.TerritoryID == territoryId )
				.Where( u => u.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
				.Where( u => u.IsActive || !enabledOnly )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( int companyId, int accountType, bool enabledOnly )
		{
			return GetUserListForAccountType( companyId, new List<int>() { accountType }, enabledOnly );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( int companyId, List<int> accountTypes, bool enabledOnly )
		{
			return database.Users
				.Include( u => u.Company )
				.Include( u => u.UserLogins )
				.Where( u => u.CompanyID == companyId )
				.Where( u => accountTypes.Contains( u.AccountType ) || !accountTypes.Any() || accountTypes.Contains( 0 ) )
				.Where( u => u.IsActive || !enabledOnly )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public UserSubscriptionSummary GetUserSubscriptionSummary( int userId )
		{
			var eUser = database.UserNotifications.FirstOrDefault( u => u.UserID == userId );
			if( eUser == null )
			{
				return new UserSubscriptionSummary()
				{
					UserID = userId
				};
			}

			return new UserSubscriptionSummary()
			{
				UserID = userId,
				NewCollateralOrder = eUser.NewCollateralOrder,
				NewCollateralOrderTerritory = eUser.NewCollateralOrderTerritory,
				NewCollateralOrderMyDealers = eUser.NewCollateralOrderMyDealers,
				NewCollateralOrderShipment = eUser.NewCollateralOrderShipment,
				NewCollateralOrderShipmentTerritory = eUser.NewCollateralOrderShipmentTerritory,
				NewCollateralOrderShipmentMyDealers = eUser.NewCollateralOrderShipmentMyDealers,
				NewSpecRequest = eUser.NewSpecRequest,
				NewSpecRequestTerritory = eUser.NewSpecRequestTerritory,
				NewSpecRequestMyDealers = eUser.NewSpecRequestMyDealers,
				CompleteSpecRequest = eUser.CompleteSpecRequest,
				CompleteSpecRequestTerritory = eUser.CompleteSpecRequestTerritory,
				CompleteSpecRequestMyDealers = eUser.CompleteSpecRequestMyDealers,
				UpdateSpecRequest = eUser.UpdateSpecRequest,
				UpdateSpecRequestTerritory = eUser.UpdateSpecRequestTerritory,
				UpdateSpecRequestMyDealers = eUser.UpdateSpecRequestMyDealers,
				ReOpenSpecRequest = eUser.ReOpenSpecRequest,
				ReOpenSpecRequestTerritory = eUser.ReOpenSpecRequestTerritory,
				ReOpenSpecRequestMyDealers = eUser.ReOpenSpecRequestMyDealers,
			};
		}

		public void UpdateUserSubscriptions( UserSubscriptionSummary uSummary )
		{
			var eUser = database.UserNotifications.FirstOrDefault( u => u.UserID == uSummary.UserID );
			if( eUser == null )
			{
				eUser = new UserNotification();
				eUser.UserID = uSummary.UserID;
				database.UserNotifications.Add( eUser );
			}

			eUser.NewCollateralOrder = uSummary.NewCollateralOrder;
			eUser.NewCollateralOrderTerritory = uSummary.NewCollateralOrderTerritory;
			eUser.NewCollateralOrderMyDealers = uSummary.NewCollateralOrderMyDealers;
			eUser.NewCollateralOrderShipment = uSummary.NewCollateralOrderShipment;
			eUser.NewCollateralOrderShipmentTerritory = uSummary.NewCollateralOrderShipmentTerritory;
			eUser.NewCollateralOrderShipmentMyDealers = uSummary.NewCollateralOrderShipmentMyDealers;
			eUser.NewSpecRequest = uSummary.NewSpecRequest;
			eUser.NewSpecRequestTerritory = uSummary.NewSpecRequestTerritory;
			eUser.NewSpecRequestMyDealers = uSummary.NewSpecRequestMyDealers;
			eUser.CompleteSpecRequest = uSummary.CompleteSpecRequest;
			eUser.CompleteSpecRequestTerritory = uSummary.CompleteSpecRequestTerritory;
			eUser.CompleteSpecRequestMyDealers = uSummary.CompleteSpecRequestMyDealers;
			eUser.UpdateSpecRequest = uSummary.UpdateSpecRequest;
			eUser.UpdateSpecRequestTerritory = uSummary.UpdateSpecRequestTerritory;
			eUser.UpdateSpecRequestMyDealers = uSummary.UpdateSpecRequestMyDealers;
			eUser.ReOpenSpecRequest = uSummary.ReOpenSpecRequest;
			eUser.ReOpenSpecRequestTerritory = uSummary.ReOpenSpecRequestTerritory;
			eUser.ReOpenSpecRequestMyDealers = uSummary.ReOpenSpecRequestMyDealers;

			database.SaveChanges();
		}

		public IEnumerable<UserContactInfo> GetHeaderContacts( int userId )
		{
			var theList = new List<UserContactInfo>();

			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser != null )
			{
				if( eUser.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
				{
					if( eUser.Company.PaoliMemberID.HasValue )
					{
						theList.Add( new UserContactInfo()
						{
							FullName = eUser.Company.PaoliMember.FullName,
							EmailAddress = eUser.Company.PaoliMember.Email,
							PhoneNumber = eUser.Company.PaoliMember.BusinessPhone,
							ImageFileName = eUser.Company.PaoliMember.ImageFileName
						} );
					}
				}
				else if( eUser.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer )
				{
					if( eUser.Company.PaoliMemberID.HasValue )
					{
						theList.Add( new UserContactInfo()
						{
							FullName = eUser.Company.PaoliMember.FullName,
							EmailAddress = eUser.Company.PaoliMember.Email,
							PhoneNumber = eUser.Company.PaoliMember.BusinessPhone,
							ImageFileName = eUser.Company.PaoliMember.ImageFileName
						} );
					}
					if( eUser.Company.PaoliSalesRepMemberID.HasValue )
					{
						theList.Add( new UserContactInfo()
						{
							FullName = eUser.Company.PaoliSalesRepMember.FullName,
							EmailAddress = eUser.Company.PaoliSalesRepMember.Email,
							PhoneNumber = eUser.Company.PaoliSalesRepMember.BusinessPhone,
							ImageFileName = eUser.Company.PaoliSalesRepMember.ImageFileName
						} );
					}
				}
			}

			theList.Add( new UserContactInfo() { FullName = "My.Paoli Feedback", EmailAddress = "mypaoli@paoli.com", PhoneNumber = "800-457-7415" } );

			return theList;
		}

		public IEnumerable<UserContactInfo> GetPaoliRepContacts( int userId )
		{
			var theList = new List<UserContactInfo>();

			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser != null )
			{
				if( eUser.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer )
				{
					if( eUser.Company.Territory != null )
					{
						theList = eUser.Company
							.Territory
							.Companies
							.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
							.SelectMany( c => c.Users )
							.ToList()
							.Select( u => new UserContactInfo() { FullName = u.FullName, EmailAddress = u.Email, PhoneNumber = u.BusinessPhone, CompanyName = u.Company.Name } )
							.ToList();
					}
				}
				else if( eUser.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
				{
					theList = new List<UserContactInfo>() { 
						new UserContactInfo() { FullName = "Steve Smith", EmailAddress = "steve-smith@paoli.com", PhoneNumber = "812-865-7014", CompanyName = null }
					};
				}
			}

			return theList;
			
		}

		public UserContactInfo GetPaoliMemberContact( int userId )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser != null )
			{
				if( eUser.Company.PaoliMember != null )
				{
					return new UserContactInfo()
					{
						FullName = eUser.Company.PaoliMember.FullName,
						EmailAddress = eUser.Company.PaoliMember.Email,
						PhoneNumber = eUser.Company.PaoliMember.BusinessPhone,
						CompanyName = eUser.Company.PaoliMember.Company.Name
					};
				}
			}

			return null;

		}

		public PDWModels.Companies.ShippingAddress GetUserAddress( int userId )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser == null )
			{
				return new PDWModels.Companies.ShippingAddress();
			}

			if( eUser.DefaultShippingAddress == AddressTypes.Home )
			{
				return new PDWModels.Companies.ShippingAddress()
				{
					CompanyID = eUser.CompanyID,
					ContactAttn = eUser.FullName,
					Name = eUser.Company.Name,
					Address1 = eUser.HomeAddress1,
					Address2 = eUser.HomeAddress2,
					City = eUser.HomeCity,
					State = eUser.HomeState,
					Zip = eUser.HomeZip,
					Phone = eUser.HomePhone,
					ContactEmail = eUser.Email
				};
			}

			return new PDWModels.Companies.ShippingAddress()
			{
				CompanyID = eUser.CompanyID,
				ContactAttn = eUser.FullName,
				Name = eUser.Company.Name,
				Address1 = eUser.Address1,
				Address2 = eUser.Address2,
				City = eUser.City,
				State = eUser.State,
				Zip = eUser.Zip,
				Phone = eUser.BusinessPhone,
				ContactEmail = eUser.Email
			};
		}

		public IDToTextItemExtra GetCurrentCompanyInfo( bool includeTerritory = false )
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user != null )
			{
				return new IDToTextItemExtra() { ID = user.Company.CompanyID, Text = includeTerritory ? user.Company.FullNameWithTerritory : user.Company.FullName, Extra = (user.Company.TerritoryID ?? 0).ToString() };
			}

			throw new Exception( "Unable to find current user" );
		}

		public IDToTextItem GetCurrentTerritoryInfo( bool includeCompany )
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user != null )
			{
				var tInfo = database.Territories
					.Include( t => t.Companies )
					.First( t => t.TerritoryID == user.Company.TerritoryID );

				return new IDToTextItem() { ID = tInfo.TerritoryID, Text = includeCompany ? (tInfo.TerritoryID.ToString() + " - " + tInfo.SalesRepCompanyName) : tInfo.Name };
			}

			throw new Exception( "Unable to find current user" );
		}

		public IEnumerable<UserActivityExport> GetExportList()
		{
			return database.UserLogins
				.OrderByDescending( a => a.LoginDate )
				.Select( a => new UserActivityExport()
				{
					EmailAddress = a.User.Email,
					FirstName = a.User.FirstName,
					LastName = a.User.LastName,
					Activity = "Login",
					UTCDateTime = a.LoginDate
				} );
		}
		public bool RequestUserEmailChange( int reqUserId, int userId, string newAddress )
		{
			var dbUser = database.Users.FirstOrDefault( c => c.UserID == userId );
			var dbReqUser = database.Users.FirstOrDefault( u => u.UserID == reqUserId );

			if( dbUser != null && dbReqUser != null )
			{
				( new RequestEmailChangeEmailSender() ).SubmitRequestEmail( dbReqUser.FullName, dbReqUser.Company.Name, dbReqUser.Email,
					userId, dbUser.FullName, dbUser.Company.Name, newAddress );
			}

			return false;
		}

		public bool RequestUserDeactiviation( int reqUserId, int userId, string reason )
		{
			var dbUser = database.Users.FirstOrDefault( c => c.UserID == userId );
			var dbReqUser = database.Users.FirstOrDefault( u => u.UserID == reqUserId );

			if( dbUser != null && dbReqUser != null )
			{
				( new RequestDeactivationEmailSender() ).SubmitRequestEmail( dbReqUser.FullName, dbReqUser.Company.Name, dbReqUser.Email, 
					userId, dbUser.FullName, dbUser.Company.Name, reason );
			}

			return false;
		}

		public DealerContactInfo GetDealerContactInfo( int userId )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return new DealerContactInfo()
			{
				UserID = eUser.UserID,
				UserCompanyID = eUser.CompanyID,
				
				EmailAddress = eUser.Email,
				FirstName = eUser.FirstName,
				LastName = eUser.LastName,
				Title = eUser.Title,
				AccountType = eUser.AccountType,

				BusinessAddress1 = eUser.Address1,
				BusinessAddress2 = eUser.Address2,
				BusinessCity = eUser.City,
				BusinessState = eUser.State,
				BusinessZip = eUser.Zip,
				BusinessPhone = eUser.BusinessPhone,
				CellPhone = eUser.CellPhone,
				FaxNumber = eUser.FAX,
				Extension = eUser.Extension,

				HomeAddress1 = eUser.HomeAddress1,
				HomeAddress2 = eUser.HomeAddress2,
				HomeCity = eUser.HomeCity,
				HomeState = eUser.HomeState,
				HomeZip = eUser.HomeZip,
				HomePhone = eUser.HomePhone,
				PersonalCellPhone = eUser.PersonalCellPhone
			};
		}

		public void UpdateDealerContact( int reqUserId, DealerContactInfo uInfo )
		{
			var dbReqUser = database.Users.FirstOrDefault( u => u.UserID == reqUserId );
			var companyName = "";
			var accountType = 0;
			string password = "";
			if( uInfo.UserID > 0 )
			{
				var eUser = database.Users.FirstOrDefault( u => u.UserID == uInfo.UserID );
				if( eUser == null )
				{
					throw new Exception( "Unable to find user." );
				}

				companyName = eUser.Company.Name;

				eUser.FirstName = uInfo.FirstName;
				eUser.LastName = uInfo.LastName;
				eUser.Title = uInfo.Title;

				eUser.Address1 = uInfo.BusinessAddress1;
				eUser.Address2 = uInfo.BusinessAddress2;
				eUser.City = uInfo.BusinessCity;
				eUser.State = uInfo.BusinessState;
				eUser.Zip = uInfo.BusinessZip;
				eUser.BusinessPhone = uInfo.BusinessPhone;
				eUser.CellPhone = uInfo.CellPhone;
				eUser.FAX = uInfo.FaxNumber;
				eUser.Extension = uInfo.Extension;

				eUser.HomeAddress1 = uInfo.HomeAddress1;
				eUser.HomeAddress2 = uInfo.HomeAddress2;
				eUser.HomeCity = uInfo.HomeCity;
				eUser.HomeState = uInfo.HomeState;
				eUser.HomeZip = uInfo.HomeZip;
				eUser.HomePhone = uInfo.HomePhone;
				eUser.PersonalCellPhone = uInfo.PersonalCellPhone;
				accountType = eUser.AccountType;

				database.SaveChanges();
			}
			else
			{
				if( database.Users.Any( u => u.Email == uInfo.EmailAddress ) )
				{
					throw new ApplicationException( "Unable to add user - Email address already exists." );
				}

				var dbCompany = database.Companies.FirstOrDefault( c => c.CompanyID == uInfo.UserCompanyID );
				companyName = dbCompany.Name;
				accountType = uInfo.AccountType;

				User newUser = new User();

				newUser.Email = uInfo.EmailAddress;
				newUser.FirstName = uInfo.FirstName;
				newUser.LastName = uInfo.LastName;
				newUser.CompanyID = uInfo.UserCompanyID;
				newUser.Address1 = uInfo.BusinessAddress1;
				newUser.Address2 = uInfo.BusinessAddress2;
				newUser.City = uInfo.BusinessCity;
				newUser.State = uInfo.BusinessState;
				newUser.Zip = uInfo.BusinessZip;
				newUser.BusinessPhone = uInfo.BusinessPhone;
				newUser.CellPhone = uInfo.CellPhone;
				newUser.HomeAddress1 = uInfo.HomeAddress1;
				newUser.HomeAddress2 = uInfo.HomeAddress2;
				newUser.HomeCity = uInfo.HomeCity;
				newUser.HomeState = uInfo.HomeState;
				newUser.HomeZip = uInfo.HomeZip;
				newUser.HomePhone = uInfo.HomePhone;
				newUser.PersonalCellPhone = uInfo.PersonalCellPhone;
				newUser.FAX = uInfo.FaxNumber;
				newUser.Extension = uInfo.Extension;
				newUser.Title = uInfo.Title;
				newUser.AccountType = uInfo.AccountType;
				newUser.Enabled = true;
				newUser.RecWelcomeEmail = true;
				newUser.IsActive = true;
				newUser.AuthorCredit = null;
				newUser.DefaultShippingAddress = AddressTypes.Business;
				newUser.ViewPerfData = false;
				newUser.TierGroup = PaoliWebUser.PaoliTierGroup.General;

				password = GenerateNewPassword();
				PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
				newUser.Password = enc.Encrypt( password );
				newUser.IsTempPassword = true;

				newUser.UserNotification = new UserNotification()
				{
					NewCollateralOrder = true,
					NewCollateralOrderTerritory = true,
					NewCollateralOrderShipment = true,
					NewCollateralOrderShipmentTerritory = true,
					NewSpecRequest = true,
					NewSpecRequestTerritory = true,
					CompleteSpecRequest = true,
					CompleteSpecRequestTerritory = true,
					UpdateSpecRequest = true,
					UpdateSpecRequestTerritory = true,
					ReOpenSpecRequest = true,
					ReOpenSpecRequestTerritory = true,
					CompleteSpecRequestMyDealers = true,
					NewCollateralOrderMyDealers = true,
					NewCollateralOrderShipmentMyDealers = true,
					NewSpecRequestMyDealers = true,
					ReOpenSpecRequestMyDealers = true,
					UpdateSpecRequestMyDealers = true
				};

				newUser.CreatedByUser = reqUserId;
				newUser.CreatedDateTime = DateTime.UtcNow;

				database.Users.Add( newUser );

				database.SaveChanges();

				database.Entry<User>( newUser ).Reload();

				( new NewAccountEmailSender() ).SubmitNewAccountEmail( uInfo.EmailAddress, password );

				MailChimpSubscribe.AddDealerEmailAddress( uInfo.EmailAddress, uInfo.FirstName, uInfo.LastName, dbCompany.CompanyID, dbCompany.Name, dbCompany.SubCompanyIDs,
					dbCompany.PaoliMember.FirstName, dbCompany.PaoliMember.LastName, newUser.UserID, dbCompany.Address1, dbCompany.Address2, dbCompany.City, dbCompany.State, dbCompany.Zip,
					dbCompany.Phone, dbCompany.TerritoryID );
			}

			new ChangeDealerUserInfoEmailSender( uInfo.UserID > 0 ? "ChangeDealerUserInfo" : "AddDealerUserInfo" ).SubmitRequestEmail( 
				dbReqUser.FullName, dbReqUser.Company.Name, uInfo.UserID, companyName, 
				uInfo.EmailAddress, uInfo.FirstName, uInfo.LastName, uInfo.Title,
				uInfo.BusinessAddress1, uInfo.BusinessAddress2, uInfo.BusinessCity, uInfo.BusinessState, uInfo.BusinessZip, uInfo.BusinessPhone,
				uInfo.CellPhone, uInfo.Extension, uInfo.FaxNumber, 
				uInfo.HomeAddress1, uInfo.HomeAddress2, uInfo.HomeCity, uInfo.HomeState, uInfo.HomeZip, uInfo.HomePhone, uInfo.PersonalCellPhone,
				PaoliWebUser.PaoliWebRole.RoleList[accountType] );
		}
	}
}
