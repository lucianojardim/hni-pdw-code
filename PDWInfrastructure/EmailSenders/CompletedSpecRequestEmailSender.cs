using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDWInfrastructure.EmailSenders
{
	public class CompletedSpecRequestEmailSender : EmailSender
	{
		public class EmailCompleteSpecRequestSummary
		{
			public int requestId { get; set; }
			public string requestName { get; set; }
			public string specTeamMember { get; set; }
			public string firstName { get; set; }
			public string projectName { get; set; }
			public string specTeamNotes { get; set; }
			public List<FileDetail> fullFileList { get; set; }
			public List<FileDetail> oldFileList { get; set; }
		}

		public bool SubmitCompletedRequestEmail(
			string emailAddress,
			EmailCompleteSpecRequestSummary summary, 
			EmailFromDetails fromDetails )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "CompletedSpecRequest", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestID}]", summary.requestId.ToString() );
					template.Replace( "[{RequestTNum}]", summary.requestName );
					template.Replace( "[{SpecTeamMember}]", summary.specTeamMember );
					template.Replace( "[{FirstName}]", summary.firstName );
					template.Replace( "[{ProjectName}]", summary.projectName );

					{
						string pattern = Regex.Escape( "[{SpecTeamNotesTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/SpecTeamNotesTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullSpecTeamNotes = string.Empty;
						if( matching.Success && ( summary.specTeamNotes ?? "" ).Any() )
						{
							fullSpecTeamNotes = matching.Groups[1].ToString().Replace( "[{SpecTeamNotes}]", summary.specTeamNotes.Replace( "\n", "<br/>" ) );
						}
						template.Replace( "[{SpecTeamNotesArea}]", fullSpecTeamNotes );
					}

					{
						string pattern = Regex.Escape( "[{FileListTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/FileListTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullFileTextString = string.Empty;
						if( matching.Success && summary.fullFileList.Any() )
						{
							fullFileTextString = matching.Groups[1].ToString()
								.Replace( "[{FullFinalFileList}]", string.Join( "", summary.fullFileList
									.Select( f => string.Format( "<li><a href=\"http://my.paoli.com/Documents/Typicals/{1}/{0}\">{0}</a></li>",
										f.fileName, f.filePath ) ) ) );
						}
						template.Replace( "[{FinalFileListArea}]", fullFileTextString );
					}

					{
						string pattern = Regex.Escape( "[{OldFileListTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/OldFileListTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullFileTextString = string.Empty;
						if( matching.Success && summary.oldFileList.Any() )
						{
							fullFileTextString = matching.Groups[1].ToString()
								.Replace( "[{OldFinalFileList}]", string.Join( "", summary.oldFileList
									.Select( f => string.Format( "<li><a href=\"http://my.paoli.com/Documents/Typicals/{1}/{0}\">{0}</a></li>",
										f.fileName, f.filePath ) ) ) );
						}
						template.Replace( "[{OldFileListArea}]", fullFileTextString );
					}

					PerformFromAreaChanges( template, fromDetails );

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
