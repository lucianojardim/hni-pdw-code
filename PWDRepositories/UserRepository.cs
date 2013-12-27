﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Users;

namespace PWDRepositories
{
    public class UserRepository
    {
		private PaoliPDWEntities database = new PaoliPDWEntities();

        public UserRepository()
		{
		}

		private UserSummary ToUserSummary( User u )
		{
			return new UserSummary()
			{
				UserID = u.UserID,
				FirstName = u.FirstName,
				LastName = u.LastName,
				CompanyName = u.Company.Name,
				EmailAddress = u.Email,
				Enabled = u.Enabled,
				SentWelcomeEmail = u.RecWelcomeEmail
			};
		}

        public PaoliWebUser CreatePaoliWebUser(string email, string authType)
        {
			User user = database.Users.FirstOrDefault( u => u.Email == email );
			if( user != null )
			{
				return new PaoliWebUser( user.Email, authType, user.UserID, user.FullName, user.AccountType );
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
					database.AddToUsers( user );

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

			bool bValid = ( string.Compare( enc.Decrypt( user.Password ), password, false ) == 0 );

			homePage = PaoliWebUser.PaoliWebRole.RoleHomePage( user.AccountType );

			return bValid;
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

		public bool AddUser( UserInformation uInfo )
		{
			if( database.Users.Any( u => u.Email == uInfo.EmailAddress ) )
			{
				throw new Exception( "Unable to add user - Email address already exists." );
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
			newUser.Title = uInfo.Title;
			newUser.AccountType = uInfo.AccountType;
			newUser.Enabled = uInfo.Enabled && uInfo.SendWelcomeEmail;
			newUser.RecWelcomeEmail = uInfo.SendWelcomeEmail;

			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			newUser.Password = enc.Encrypt( password );

			database.Users.AddObject( newUser );

			if( database.SaveChanges() > 0 )
			{
				if( uInfo.SendWelcomeEmail )
				{
					NewAccountEmailSender.SubmitEmail( newUser.Email, password );
				}

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
				CompanyName = eUser.Company.Name,
				Address1 = eUser.Address1,
				Address2 = eUser.Address2,
				City = eUser.City,
				State = eUser.State,
				Zip = eUser.Zip,
				BusinessPhone = eUser.BusinessPhone,
				CellPhone = eUser.CellPhone,
				Title = eUser.Title,
				AccountType = eUser.AccountType,
				Enabled = eUser.Enabled,
				SendWelcomeEmail = eUser.RecWelcomeEmail,
				LockAccountType = eUser.SpecRequests.Any() || eUser.SpecRequests1.Any()
			};
		}

		public bool UpdateUser( MyAccountInfo uInfo )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == uInfo.UserID );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			if( database.Users.Any( u => u.Email == uInfo.EmailAddress && u.UserID != uInfo.UserID ) )
			{
				throw new Exception( "Unable to update user - Email address already exists." );
			}

			bool bChangeEmail = false, bWelcomeEmail = false;
			if( eUser.Email == PaoliWebUser.CurrentUser.EmailAddress )
			{
				bChangeEmail = (eUser.Email != uInfo.EmailAddress);
			}

			eUser.Email = uInfo.EmailAddress;
			eUser.FirstName = uInfo.FirstName;
			eUser.LastName = uInfo.LastName;
			eUser.Address1 = uInfo.Address1;
			eUser.Address2 = uInfo.Address2;
			eUser.City = uInfo.City;
			eUser.State = uInfo.State;
			eUser.Zip = uInfo.Zip;
			eUser.BusinessPhone = uInfo.BusinessPhone;
			eUser.CellPhone = uInfo.CellPhone;
			eUser.Title = uInfo.Title;

			string password = GenerateNewPassword();
			if( uInfo is UserInformation )
			{
				eUser.AccountType = ( uInfo as UserInformation ).AccountType;
				eUser.Enabled = ( uInfo as UserInformation ).Enabled;
				eUser.CompanyID = ( uInfo as UserInformation ).CompanyID;
				if( !eUser.RecWelcomeEmail && ( uInfo as UserInformation ).SendWelcomeEmail )
				{
					bWelcomeEmail = true;
					PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
					eUser.Password = enc.Encrypt( password );
				}
			}

			if( database.SaveChanges() > 0 )
			{
				if( bChangeEmail )
				{
					System.Web.Security.FormsAuthentication.SetAuthCookie( uInfo.EmailAddress, false );
				}
				if( bWelcomeEmail )
				{
					NewAccountEmailSender.SubmitEmail( eUser.Email, password );
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
			eUser.RecWelcomeEmail = true;
			eUser.Enabled = true;

			if( database.SaveChanges() > 0 )
			{
				NewAccountEmailSender.SubmitEmail( eUser.Email, password );

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

			if( database.SaveChanges() > 0 )
			{
				ResetPasswordEmailSender.SubmitEmail( eUser.Email, password );

				return true;
			}

			return false;
		}

		public IEnumerable<UserSummary> GetFullUserList( UserTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var userList = database.Users.AsQueryable();

			totalRecords = userList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				userList = userList.Where( i =>
					i.FirstName.Contains( param.sSearch ) ||
					i.LastName.Contains( param.sSearch ) ||
					i.Email.Contains( param.sSearch ) ||
					i.Company.Name.Contains( param.sSearch ) );
			}
			if( param.accountType != 0 )
			{
				userList = userList.Where( u => u.AccountType == param.accountType );
			}
			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
			{
				userList = userList.Where( i => i.AccountType != PaoliWebUser.PaoliWebRole.SuperAdmin );
			}

			displayedRecords = userList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<User> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
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
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToUserSummary( v ) );
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

				return database.SaveChanges() > 0;
			}

			throw new ApplicationException( "Current password does not match our records." );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( int accountType )
		{
			return database.Users
				.Where( u => u.AccountType == accountType || accountType == 0 )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( int companyId, int accountType )
		{
			return database.Users
				.Where( u => u.CompanyID == companyId )
				.Where( u => u.AccountType == accountType || accountType == 0 )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public UserSubscriptionSummary GetUserSubscriptionSummary( int userId )
		{
			var usSummary = new UserSubscriptionSummary();

			usSummary.UserID = userId;
			usSummary.subInfo = database.Subscriptions
				.Select( s1 => new UserSubscriptionSummary.SubscriptionInfo()
				{
					SubscriptionID = s1.SubscriptionID,
					Name = s1.Name,
					Checked = s1.Users.Any( u => u.UserID == userId )
				} )
				.ToList();

			return usSummary;
		}

		public void UpdateUserSubscriptions( UserSubscriptionSummary uSummary )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == uSummary.UserID );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			eUser.Subscriptions.Clear();

			foreach( var sub in uSummary.subInfo )
			{
				if( sub.Checked )
				{
					eUser.Subscriptions.Add( database.Subscriptions.First( s => s.SubscriptionID == sub.SubscriptionID ) );
				}
			}

			database.SaveChanges();
		}
	}
}
