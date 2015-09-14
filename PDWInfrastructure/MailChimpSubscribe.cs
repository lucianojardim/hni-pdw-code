using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailChimp;
using MailChimp.Helper;
using System.Configuration;

namespace PDWInfrastructure
{
	public static class MailChimpSubscribe
	{
		public static void AddEmailAddress( string emailAddress, string lists )
		{
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

				foreach( var listId in lists.Split( ',' ).Select( s => s.Trim() ) )
				{
					EmailParameter results = mc.Subscribe( listId, email, doubleOptIn: false );
				}
			}
			catch
			{
			}
		}

		public static void AddEmailAddress( string emailAddress, int accountType )
		{
			var ListID = "";

			switch( accountType )
			{
				case PaoliWebUser.PaoliWebRole.SuperAdmin:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliMemberAdmin:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliMemberMarketing:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliMemberSpecTeam:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliMemberCustomerService:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliMemberSales:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.PaoliSalesRep:
					ListID = "MailChimpSalesRepList";
					break;
				case PaoliWebUser.PaoliWebRole.DealerPrincipal:
					ListID = "MailChimpDealerList";
					break;
				case PaoliWebUser.PaoliWebRole.DealerSalesRep:
					ListID = "MailChimpDealerList";
					break;
				case PaoliWebUser.PaoliWebRole.DealerDesigner:
					ListID = "MailChimpDealerList";
					break;
				case PaoliWebUser.PaoliWebRole.DealerAdmin:
					ListID = "MailChimpDealerList";
					break;
				case PaoliWebUser.PaoliWebRole.EndUser:
					ListID = "";
					break;
				case PaoliWebUser.PaoliWebRole.AandDUser:
					ListID = "";
					break;
			}

			if( !string.IsNullOrEmpty( ListID ) )
			{
				AddEmailAddress( emailAddress, ConfigurationManager.AppSettings[ListID] );
			}
		}
	}
}
