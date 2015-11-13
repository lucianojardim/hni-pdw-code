using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.SpecRequests
{
	public class SpecRequestInformation
	{
		public class TempFileDetail
		{
			public string TempFile { get; set; }
			public string OriginalFile { get; set; }
		}

		public class FileInformation
		{
			public int FileID { get; set; }
			public int VersionNumber { get; set; }
			public string FileName { get; set; }
			public DateTime UploadDate { get; set; }
			public string Extension { get; set; }
		}

		public class FileListing
		{
			public FileListing()
			{
				FileList = new List<FileInformation>();
			}

			public string SpecName { get; set; }
			public List<FileInformation> FileList { get; set; }
		}


		public int RequestID { get; set; }
		public string Name { get; set; }

		[DisplayName( "Select A Sales Rep Territory" )]
		public int? PaoliSalesRepGroupID { get; set; }
		[DisplayName( "Sales Rep Point of Contact" )]
		public int? PaoliSalesRepMemberID { get; set; }
		[DisplayName( "What dealership is this request for?" )]
		public int? DealerID { get; set; }
		[DisplayName( "Dealership Point of Contact" )]
		public int? DealerMemberID { get; set; }
		public string DealerPointOfContact { get; set; }
		public string DealerPointOfContactEmail { get; set; }
		public string DealerPointOfContactPhone { get; set; }
		public int? DealerPointOfContactAcctType { get; set; }
		public string DealerPointOfContactAcctTypeName { get; set; }
		[DisplayName( "Paoli Spec Team Member" )]
		public int? PaoliSpecTeamMember { get; set; }

		[DisplayName( "Who is the end customer?" )]
		public int? EndCustomerID { get; set; }
		public string RealEndCustomer { get; set; }
		[DisplayName( "What project is this for?" )]
		public int? ProjectID { get; set; }
		public string RealProjectName { get; set; }
		[DisplayName( "What is the size and scope of the project?" )]
		public string ProjectScope { get; set; }
		public string ProjectedListPrice { get; set; }
		public bool IsPhasedProject { get; set; }
		public int? NumberOfPhases { get; set; }
		public bool IsStandardsProgram { get; set; }
		public DateTime? FirstOrderDate { get; set; }
		public string Competitors { get; set; }
		public string CompetitorSeries { get; set; }
		[DisplayName( "Is in2?" )]
		public bool AvailableForIn2 { get; set; }
		public bool RealIsGSA { get; set; }
		public string RealContractName { get; set; }

		public string Status { get; set; }
		public DateTime? OrderPlacedDate { get; set; }
		public string OrderCost { get; set; }
		public string AckNumber { get; set; }
		public string LostReason { get; set; }

		[DisplayName( "Floorplan Specifications" )]
		public bool NeedFloorplanSpecs { get; set; }
		/* obselete
		[DisplayName( "3D Drawing" )]
		public bool Need3DDrawing { get; set; }
		 */
		[DisplayName( "Value Engineering" )]
		public bool NeedValueEng { get; set; }
		[DisplayName( "Photo Rendering" )]
		public bool NeedPhotoRendering { get; set; }
		[DisplayName( "2D Drawing" )]
		public bool Need2DDrawing { get; set; }
		[DisplayName( "Spec Check Audit" )]
		public bool NeedAuditSpecs { get; set; }
		[DisplayName( "Related Drawings or Notes" )]
		public List<HttpPostedFileBase> addlFiles { get; set; }
		public List<TempFileDetail> tempFiles { get; set; }

		[DisplayName( "Notes and Directions for Spec Team" )]
		public string Notes { get; set; }
		[DisplayName( "DWG" )]
		public bool NeedDWGFiles { get; set; }
		[DisplayName( "SP4" )]
		public bool NeedSP4Files { get; set; }
		[DisplayName( "SIF" )]
		public bool NeedSIFFiles { get; set; }
		[DisplayName( "XLS" )]
		public bool NeedXLSFiles { get; set; }
		[DisplayName( "PDF" )]
		public bool NeedPDFFiles { get; set; }

		[DisplayName( "Casegoods" )]
		public List<string> Casegoods { get; set; }
		[DisplayName( "Conferencing" )]
		public List<string> Conferencing { get; set; }
		[DisplayName( "Finishes" )]
		public List<string> Finishes { get; set; }
		public string SeriesList { get; set; }
		[DisplayName( "Laminate Preference" )]
		public string LaminatePreference { get; set; }
		[DisplayName( "Veneer Preference" )]
		public string VeneerPreference { get; set; }
		public string OtherFinishDetails { get; set; }
		[DisplayName( "Include Grommets" )]
		public bool Grommets { get; set; }
		[DisplayName( "Grommet Specifications" )]
		public string GrommetDetails { get; set; }
		[DisplayName( "Position of Grommets" )]
		public string GrommetPosition { get; set; }
		[DisplayName( "Drawer Option" )]
		public string DrawerOption { get; set; }
		[DisplayName( "Seating" )]
		public List<string> Seating { get; set; }
		[DisplayName( "Fabric Grade" )]
		public string FabricGrade { get; set; }
		[DisplayName( "What fabric does the user want?" )]
		public string Fabric { get; set; }

		[DisplayName( "Special Requests" )]
		public string SpecialRequests { get; set; }

		// edit only
		[DisplayName( "Saved Location" )]
		[StringLength( 500 )]
		public string SavedLocation { get; set; }
		[DisplayName( "List Price" )]
		public int? ListPrice { get; set; }
		[DisplayName( "Received?" )]
		public bool Received { get; set; }
		[DisplayName( "Completed?" )]
		public bool IsCompleted { get; set; }
		public bool IsCanceled { get; set; }
		public bool IsOnHold { get; set; }
		[DisplayName( "SPL Quote" )]
		public string SPLQuote { get; set; }
		[DisplayName( "Recommended for Web?" )]
		public bool IsGoodForWeb { get; set; }
		[DisplayName( "Send Email Update To Dealers and Sales Reps?" )]
		public bool SendCompleteEmail { get; set; }
		public bool SendInProgressEmail { get; set; }
		[DisplayName( "Footprint" )]
		[StringLength( 100 )]
		public string Footprint { get; set; }
		[DisplayName( "Final Client Files" )]
		public List<HttpPostedFileBase> specTeamFiles { get; set; }
		public string deletespecTeamFileList { get; set; }
		[DisplayName( "Spec Team Notes to Recipients" )]
		public string SpecTeamNotes { get; set; }
		[DisplayName( "Previous Spec Team Notes" )]
		public string PreviousSpecTeamNotes { get; set; }
		[DisplayName( "Internal Notes" )]
		public string InternalNotes { get; set; }

		/* obselete
		[DisplayName( "Featured Series" )]
		[StringLength( 50 )]
		public string FeaturedSeries { get; set; }
		[DisplayName( "Additional Series" )]
		public string SeriesList { get; set; }
		[DisplayName( "Material" )]
		[StringLength( 100 )]
		public string Material { get; set; }
		[DisplayName( "Finish" )]
		[StringLength( 100 )]
		public string Finish { get; set; }
		*/

		public FileListing addlFileList { get; set; }
		public FileListing specTeamFileList { get; set; }

		[DisplayName( "This quote request was submitted" )]
		public DateTime? CreatedDate { get; set; }
		[DisplayName( "by" )]
		public string CreatedByUser { get; set; }
		public string CreatedByUserCompany { get; set; }
		public string CreatedByUserPhone { get; set; }
		public string CreatedByUserEmail { get; set; }

		[DisplayName( "This quote request was re-opened" )]
		public DateTime? ReOpenedDate { get; set; }
		[DisplayName( "by" )]
		public string ReOpenedByUser { get; set; }
		public string ReOpenedByUserCompany { get; set; }
		public string ReOpenedByUserPhone { get; set; }
		public string ReOpenedByUserEmail { get; set; }

		[DisplayName( "This quote request was canceled" )]
		public DateTime? CanceledDate { get; set; }
		[DisplayName( "by" )]
		public string CanceledByUser { get; set; }

		public string DealerName { get; set; }
		public string DealerSalesRepName { get; set; }
		public string DealerSalesRepContact { get; set; }
		public string SalesRepGroupName { get; set; }
		public string SalesRepMemberName { get; set; }
		public string SalesRepMemberContact { get; set; }
		public string SpecTeamMemberName { get; set; }
	}
}
