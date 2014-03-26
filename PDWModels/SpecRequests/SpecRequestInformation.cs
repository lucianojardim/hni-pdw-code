﻿using System;
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
		public class FileInformation
		{
			public int FileID { get; set; }
			public int VersionNumber { get; set; }
			public string FileName { get; set; }
			public string UploadDate { get; set; }
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
		[DisplayName( "Paoli Spec Team Member" )]
		public int? PaoliSpecTeamMember { get; set; }

		[DisplayName( "Who is the end customer?" )]
		[StringLength( 100 )]
		public string EndCustomer { get; set; }
		[DisplayName( "What will we call this project?" )]
		[StringLength( 50 )]
		[Required]
		public string ProjectName { get; set; }
		[DisplayName( "What is the size and scope of the project?" )]
		[StringLength( 100 )]
		public string ProjectSize { get; set; }
		[DisplayName( "Is GSA?" )]
		public bool IsGSA { get; set; }
		[DisplayName( "Which contract does this relate to?" )]
		public int? ContractID { get; set; }
		public string ContractName { get; set; }
		[DisplayName( "Is in2?" )]
		public bool AvailableForIn2 { get; set; }

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
		[DisplayName( "Laminate Preference" )]
		public string LaminatePreference { get; set; }
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
		[DisplayName( "SPL Quote" )]
		public string SPLQuote { get; set; }
		[DisplayName( "Recommended for Web?" )]
		public bool IsGoodForWeb { get; set; }
		[DisplayName( "Footprint" )]
		[StringLength( 100 )]
		public string Footprint { get; set; }
		[DisplayName( "Final Client Files" )]
		public List<HttpPostedFileBase> specTeamFiles { get; set; }
		public string deletespecTeamFileList { get; set; }
		[DisplayName( "Spec Team Notes" )]
		public string SpecTeamNotes { get; set; }

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
