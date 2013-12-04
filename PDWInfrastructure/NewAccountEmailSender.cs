using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace PDWInfrastructure
{
	public class NewAccountEmailSender : EmailSender
	{
		public static bool SubmitEmail(
			string emailAddress,
			string newPassword )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "NewAccount", out template ) )
				{
					// perform substitutions
					template.Replace( "[{Password}]", newPassword );

					var message = new MailMessage();

					if( ( emailAddress ?? "" ).Any() )
						message.To.Add( emailAddress );
					else
						return false;

					message.Subject = GetSubject( template );
					message.Body = template.ToString();
					message.IsBodyHtml = true;
					message.From = FromAddress;

					return SubmitEmail( message );
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
