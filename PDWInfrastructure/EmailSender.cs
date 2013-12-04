using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace PDWInfrastructure
{
	public class EmailSender
	{
		protected static MailAddress FromAddress = new MailAddress( "support@paoli.com", "Paoli Support" );

		protected static bool SubmitEmail(
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
			msg.From = FromAddress;

			return SubmitEmail( msg );
		}

		protected static bool SubmitEmail( MailMessage message )
		{
			if( message != null )
			{
				try
				{
					SmtpClient smtp = new SmtpClient();

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

		protected static bool ReadEmailTemplate( string emailType, out StringBuilder template )
		{
			string emailTemplateFileName = emailType + ".htm";

			try
			{
				template = new StringBuilder( GetTemplate( emailTemplateFileName ) );

				PerformMachineNameAdditions( template );
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to read email template: {0}", ex.Message );
				throw ex;
			}

			return true;
		}

		protected static string GetTemplate( string templateFileName )
		{
			// get this from the web.config
			string templateDirectory = ConfigurationManager.AppSettings["EmailTemplates"];
			string templateFilePath = Path.Combine( templateDirectory, templateFileName );

			if( !System.IO.File.Exists( templateFilePath ) )
			{
				throw new Exception();
			}

			using( StreamReader reader = new StreamReader( new FileStream( templateFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) ) )
			{
				return reader.ReadToEnd();
			}
		}

		protected static string GetSubject( StringBuilder template )
		{
			string pattern = Regex.Escape( "<title>" ) + "(.*?)" + Regex.Escape( "</title>" );
			var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase );

			if( matching.Success )
			{
				return matching.Groups[1].ToString();
			}

			return "Paoli Email";
		}

		protected static void PerformMachineNameAdditions( StringBuilder template )
		{
			var extraInfo = "";
			if( Environment.MachineName.ToLower().Contains( "matt3400" ) )
				extraInfo = "<p>(from Matt's Development machine)</p>";
			else if( Environment.MachineName.ToLower().Contains( "paoli-test01" ) )
				extraInfo = "<p>(from WDD Test Server)</p>";
			
			template.Replace( "[{MachineName}]", extraInfo );
		}
	}
}
