using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace PDWInfrastructure
{
	public class EmailSender
	{
		public static bool SubmitEmail(
			IList<string> toList,
			IList<string> ccList,
			IList<string> bccList,
			string messageSubject,
			string messageBody )
		{
			MailMessage msg = new MailMessage();

			if( ( toList != null ) && toList.Any() )
				msg.To.Add( string.Join( ",", toList ) );
			else
				return false;

			if( ccList != null )
				msg.CC.Add( string.Join( ",", ccList ) );
			if( bccList != null )
				msg.Bcc.Add( string.Join( ",", bccList ) );

			msg.Subject = messageSubject;
			msg.Body = messageBody;
			msg.From = new MailAddress( "support@paoli.com", "Paoli Support" );

			return SubmitEmail( msg );
		}

		public static bool SubmitEmail( MailMessage message )
		{
			if( message != null )
			{
				try
				{
					SmtpClient smtp = new SmtpClient();

					if( Environment.MachineName.ToLower().Contains( "matt3400" ) )
						message.Body += " (from Matt's Development machine)";
					else if( Environment.MachineName.ToLower().Contains( "paoli-test01" ) )
						message.Body += " (from WDD Test Server)";

					smtp.Send( message );
					return true;
				}
				catch( Exception e )
				{
					System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", e.Message );
				}
			}
			return false;
		}
	}
}
