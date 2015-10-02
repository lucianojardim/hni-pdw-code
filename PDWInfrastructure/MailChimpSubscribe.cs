using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailChimp;
using MailChimp.Helper;
using System.Configuration;
using MailChimp.Lists;

namespace PDWInfrastructure
{
	public static class MailChimpSubscribe
	{
		/*
		 * Copy Of Dealers Invited To My.Paoli.Com (Dev-Test)
		 * Dealer List (Test)
		 * Sales Rep List (Test)
		 */
		private static void AddEmailAddress( string emailAddress, string listConfig, Dictionary<string, string> map )
		{
			var lists = ConfigurationManager.AppSettings[listConfig].ToString();

			if( string.IsNullOrEmpty( lists ) )
			{
				return;
			}

			try
			{
				MailChimpManager mc = new MailChimpManager( ConfigurationManager.AppSettings["MailChimpApiKey"] );

				//  Create the email parameter
				EmailParameter email = new EmailParameter()
				{
					Email = emailAddress
				};
				
				var details = new MergeVar();
				foreach( var item in map )
				{
					details.Add( item.Key, item.Value );
				}

				foreach( var listId in lists.Split( ',' ).Select( s => s.Trim() ) )
				{
					EmailParameter results = mc.Subscribe( listId, email, doubleOptIn: false, mergeVars: details );
				}
			}
			catch
			{
			}
		}

		private static void AddMergeItem( this Dictionary<string, string> map, string key, string val )
		{
			if( !string.IsNullOrEmpty( val ) )
			{
				map.Add( key, val );
			}
		}

		/*
Dealer list #1:
First Name - FNAME
Last Name - LNAME
Dealership ID (Base Number) - DLR_ID
Dealership Name - DLR_NAME
My.Paoli Company ID - MP_C_ID
My.Paoli Account Active - MP_ACTIVE - Options - "Yes", "No"
CSR First Name - CSRFIRST
CSR Last Name - CSRLAST
My.Paoli User ID - MP_U_ID

Dealer list #2:
First Name - FNAME
Last Name - LNAME
Portal User ID - MMERGE3
Company Name - COMPANY
Company Address - ADDRESS1
Company Address 2 - ADDRESS2
Company City - CITY
Company State - STATE
Company Zip Code - ZIPCODE
Company Phone - PHONE
Territory ID - TERRITORY
		 */
		public static void AddDealerEmailAddress( string emailAddress, string firstName, string lastName, int dealershipId, string dealershipName,
			string dealershipBaseNumber, string csrFirstName, string csrLastName, int userId,
			string address1, string address2, string city, string state, string zipCode, string phone, int? territoryId )
		{
			var map = new Dictionary<string, string>();

			map.AddMergeItem( "FNAME", firstName );
			map.AddMergeItem( "LNAME", lastName );
			map.AddMergeItem( "DLR_ID", dealershipBaseNumber );
			map.AddMergeItem( "DLR_NAME", dealershipName );
			map.AddMergeItem( "MP_C_ID", dealershipId.ToString() );
			map.AddMergeItem( "MP_ACTIVE", "Yes" );
			map.AddMergeItem( "CSRFIRST", csrFirstName );
			map.AddMergeItem( "CSRLAST", csrLastName );
			map.AddMergeItem( "MP_U_ID", userId.ToString() );
			map.AddMergeItem( "MMERGE3", userId.ToString() );
			map.AddMergeItem( "COMPANY", dealershipName );
			map.AddMergeItem( "ADDRESS1", address1 );
			map.AddMergeItem( "ADDRESS2", address2 );
			map.AddMergeItem( "CITY", city );
			map.AddMergeItem( "STATE", state );
			map.AddMergeItem( "ZIPCODE", zipCode );
			map.AddMergeItem( "PHONE", phone );
			map.AddMergeItem( "TERRITORY", territoryId.HasValue ? territoryId.ToString() : "" );

			AddEmailAddress( emailAddress, "MailChimpDealerList", map );
		}

		/*
Sales Rep list:
First Name - FNAME
Last Name - LNAME
Company - COMPANY
Office Phone - PHONE
Mobile - MOBILE
Fax - FAX
Territory ID - TERRITORY
Company Address - ADDRESS1
Company Address 2 - ADDRESS2
Company City - CITY
Company State - STATE
		 */
		public static void AddSalesRepEmailAddress( string emailAddress, string firstName, string lastName, string companyName,
			string mobilePhone, string address1, string address2, string city, string state, string phone, string fax, int? territoryId )
		{
			var map = new Dictionary<string, string>();

			map.AddMergeItem( "FNAME", firstName );
			map.AddMergeItem( "LNAME", lastName );
			map.AddMergeItem( "COMPANY", companyName );
			map.AddMergeItem( "PHONE", phone );
			map.AddMergeItem( "MOBILE", mobilePhone );
			map.AddMergeItem( "FAX", fax );
			map.AddMergeItem( "TERRITORY", territoryId.HasValue ? territoryId.ToString() : "" );
			map.AddMergeItem( "ADDRESS1", address1 );
			map.AddMergeItem( "ADDRESS2", address2 );
			map.AddMergeItem( "CITY", city );
			map.AddMergeItem( "STATE", state );

			AddEmailAddress( emailAddress, "MailChimpSalesRepList", map );
		}
	}
}
