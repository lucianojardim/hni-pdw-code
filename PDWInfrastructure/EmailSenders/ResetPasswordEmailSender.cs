using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace PDWInfrastructure.EmailSenders
{
	public class ResetPasswordEmailSender : EmailSender
	{
		public bool SubmitResetPasswordEmail(
			string emailAddress,
			string newPassword )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "ResetPassword", out template ) )
				{
					// perform substitutions
					template.Replace( "[{Password}]", newPassword );

					return SubmitEmail( new List<string>() { emailAddress }, null, null, GetSubject( template ), template.ToString() );
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
