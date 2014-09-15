using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDWInfrastructure.EmailSenders
{
	public class RequestChangeSpecRequestEmailSender : EmailSender
	{
		public class RequestChangeSpecRequestSummary
		{
			public int requestId { get; set; }
			public string requestName { get; set; }
			public string firstName { get; set; }
			public string reOpenedByName { get; set; }
			public string projectName { get; set; }
			public string dealerPOCName { get; set; }
			public string dealership { get; set; }
			public List<FileDetail> newFiles { get; set; }
			public string newNotes { get; set; }
			public List<string> recipients { get; set; }
			public string specTeamMember { get; set; }

			public RequestChangeSpecRequestSummary()
			{
				recipients = new List<string>();
				newFiles = new List<FileDetail>();
			}
		}

		public RequestChangeSpecRequestEmailSender( string template )
		{
			TemplateName = template;
		}

		private string TemplateName { get; set; }

		public bool SubmitNewRequestEmail(
			string emailAddress,
			RequestChangeSpecRequestSummary summary )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( TemplateName, out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestID}]", summary.requestId.ToString() );
					template.Replace( "[{RequestTNum}]", summary.requestName );
					template.Replace( "[{FirstName}]", summary.firstName );
					template.Replace( "[{ReOpenedByName}]", summary.reOpenedByName );
					template.Replace( "[{ProjectName}]", summary.projectName );
					template.Replace( "[{DealerPOCName}]", ( summary.dealerPOCName ?? "" ).Any() ? string.Format( " for {0}", summary.dealerPOCName ) : "" );
					template.Replace( "[{Dealership}]", ( summary.dealership ?? "" ).Any() ? string.Format( " at {0}", summary.dealership ) : "" );
					template.Replace( "[{SpecTeamMember}]", summary.specTeamMember );
					template.Replace( "[{Recipients}]", string.Join( ", ", summary.recipients ) );

					{
						string pattern = Regex.Escape( "[{NotesTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/NotesTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullSpecTeamNotes = string.Empty;
						if( matching.Success && ( summary.newNotes ?? "" ).Any() )
						{
							fullSpecTeamNotes = matching.Groups[1].ToString().Replace( "[{Notes}]", summary.newNotes.Replace( "\n", "<br/>" ) );
						}
						template.Replace( "[{NotesArea}]", fullSpecTeamNotes );
					}

					{
						string pattern = Regex.Escape( "[{FileListTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/FileListTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullFileTextString = string.Empty;
						if( matching.Success && summary.newFiles.Any() )
						{
							fullFileTextString = matching.Groups[1].ToString()
								.Replace( "[{FullFinalFileList}]", string.Join( "", summary.newFiles
									.Select( f => string.Format( "<li><a href=\"http://my.paoli.com/Documents/Typicals/{1}/{0}\">{0}</a></li>",
										f.fileName, f.filePath ) ) ) );
						}
						template.Replace( "[{FileListArea}]", fullFileTextString );
					}


					return SubmitEmail( new List<string>() { emailAddress }, null, null, GetSubject( template ), template );
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
