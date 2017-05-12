using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class NewDealerUserEmailSender : EmailSender
	{
		public class EmailUserSummary
		{
			public int UserID { get; set; }
			public string EmailAddress { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string CompanyName { get; set; }
			public string Address1 { get; set; }
			public string Address2 { get; set; }
			public string City { get; set; }
			public string State { get; set; }
			public string Zip { get; set; }
			public string BusinessPhone { get; set; }
			public string CellPhone { get; set; }
			public string Title { get; set; }
			public string AccountType { get; set; }
			public string Enabled { get; set; }
			public string SendWelcomeEmail { get; set; }
			public string IsActive { get; set; }
		}

		public bool SubmitNewUserEmail(
			EmailUserSummary uInfo )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "NewDealerUserAdded", out template ) )
				{
					template.Replace( "[{DealerCompany}]", uInfo.CompanyName );
					template.Replace( "[{ID}]", uInfo.UserID.ToString() );
					template.Replace( "[{EmailAddress}]", uInfo.EmailAddress );
					template.Replace( "[{FirstName}]", uInfo.FirstName );
					template.Replace( "[{LastName}]", uInfo.LastName );
					template.Replace( "[{Address1}]", uInfo.Address1 );
					template.Replace( "[{Address2}]", uInfo.Address2 );
					template.Replace( "[{City}]", uInfo.City );
					template.Replace( "[{State}]", uInfo.State );
					template.Replace( "[{ZipCode}]", uInfo.Zip );
					template.Replace( "[{BusinessPhone}]", uInfo.BusinessPhone );
					template.Replace( "[{MobilePhone}]", uInfo.CellPhone );
					template.Replace( "[{Title}]", uInfo.Title );
					template.Replace( "[{AccountType}]", uInfo.AccountType );
					template.Replace( "[{Enabled}]", uInfo.Enabled );
					template.Replace( "[{IsActive}]", uInfo.IsActive );
					template.Replace( "[{SendWelcomeEmail}]", uInfo.SendWelcomeEmail );

					return SubmitEmail( new List<string>() { "mypaoli@paoli.com" }, null, null, GetSubject( template ), template );
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", ex.Message );
			}

			return false;
		}
	}
}
