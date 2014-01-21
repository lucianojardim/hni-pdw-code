﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;
using System.Net.Mail;
using System.Net;

namespace PDWInfrastructure
{
	public class EmailSender
	{
		protected delegate bool SubmitEmailFunction(
			IList<string> toList,
			IList<string> ccList,
			IList<string> bccList,
			string messageSubject,
			string messageBody );

		protected SubmitEmailFunction SubmitEmail = null;
		private ExchangeService service = null;
		private bool UseExchange { get; set; }

		protected EmailSender()
		{
			var config = ExchangeMailConfiguration.Config;

			if( config.Settings.UseExchange )
			{
				service = new ExchangeService();
				service.Credentials = new NetworkCredential( config.Settings.Username, config.Settings.Password, config.Settings.Domain );
				service.Url = new Uri( config.Settings.ServerAddress );
				SubmitEmail = SubmitExchangeEmail;
			}
			else
			{
				SubmitEmail = SubmitSMTPEmail;
			}
		}

		private bool SubmitExchangeEmail(
			IList<string> toList,
			IList<string> ccList,
			IList<string> bccList,
			string messageSubject,
			string messageBody )
		{
			EmailMessage message = new EmailMessage( service );
			message.Subject = messageSubject;
			message.Body = messageBody;
			
			if( ( toList != null ) && toList.Any() )
				message.ToRecipients.AddRange( toList );
			else
				return false;

			if( ccList != null )
				message.CcRecipients.AddRange( ccList );
			if( bccList != null )
				message.BccRecipients.AddRange( bccList );

			message.From = new EmailAddress( "Paoli Helpdesk", "helpdesk@paoli.com" );

			try
			{
				message.SendAndSaveCopy();

				return true;
			}
			catch( Exception e )
			{
				System.Diagnostics.Debug.WriteLine( format: "Error sending email: {0}", args: e.Message );
			}

			return false;
		}

		protected bool SubmitSMTPEmail(
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
			msg.From = new MailAddress( "helpdesk@paoli.com", "Paoli Helpdesk" );

			try
			{
				SmtpClient smtp = new SmtpClient();

				smtp.Send( msg );

				return true;
			}
			catch( Exception e )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", e.Message );
			}

			return false;
		}

		protected bool ReadEmailTemplate( string emailType, out StringBuilder template )
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

		protected string GetTemplate( string templateFileName )
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

		protected string GetSubject( StringBuilder template )
		{
			string pattern = Regex.Escape( "<title>" ) + "(.*?)" + Regex.Escape( "</title>" );
			var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase );

			if( matching.Success )
			{
				return matching.Groups[1].ToString();
			}

			return "Paoli Email";
		}

		protected void PerformMachineNameAdditions( StringBuilder template )
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
