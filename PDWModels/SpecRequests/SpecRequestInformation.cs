﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel;

namespace PDWModels.SpecRequests
{
	public class SpecRequestInformation
	{
		public class FileInformation
		{
			public int VersionNumber { get; set; }
			public string FileName { get; set; }
			public string UploadDate { get; set; }
		}

		public class FileListing
		{
			public FileListing()
			{
				FileList = new List<FileInformation>();
			}

			public string SpecName { get; set; }
			public string Extension { get; set; }
			public List<FileInformation> FileList { get; set; }
		}


		public int RequestID { get; set; }
		public string Name { get; set; }

		[DisplayName( "Project Name" )]
		public string ProjectName { get; set; }
		[DisplayName( "Paoli Sales Rep Group" )]
		public int? PaoliSalesRepGroupID { get; set; }
		[DisplayName( "Dealer" )]
		public int? CompanyID { get; set; }
		[DisplayName( "Dealer Sales Rep" )]
		public int? DealerSalesRep { get; set; }
		[DisplayName( "Is GSA?" )]
		public bool IsGSA { get; set; }
		[DisplayName( "Saved Location" )]
		public string SavedLocation { get; set; }
		[DisplayName( "List Price" )]
		public int? ListPrice { get; set; }
		[DisplayName( "Series List" )]
		public string SeriesList { get; set; }
		[DisplayName( "Received?" )]
		public bool Received { get; set; }
		[DisplayName( "SPL Quote" )]
		public int? SPLQuote { get; set; }
		[DisplayName( "Paoli Spec Team Member" )]
		public int? PaoliSpecTeamMember { get; set; }
		[DisplayName( "Recommended for Web" )]
		public bool IsGoodForWeb { get; set; }
		[DisplayName( "Avaiable for In2" )]
		public bool AvailableForIn2 { get; set; }
		[DisplayName( "Footprint" )]
		public string Footprint { get; set; }
		[DisplayName( "Featured Series" )]
		public string FeaturedSeries { get; set; }
		[DisplayName( "Material" )]
		public string Material { get; set; }
		[DisplayName( "Finish" )]
		public string Finish { get; set; }
		[DisplayName( "Notes" )]
		public string Notes { get; set; }

		[DisplayName( "XLS File" )]
		public HttpPostedFileBase xlsFile { get; set; }
		[DisplayName( "SIF File" )]
		public HttpPostedFileBase sifFile { get; set; }
		[DisplayName( "SP4 File" )]
		public HttpPostedFileBase sp4File { get; set; }
		[DisplayName( "PDF File" )]
		public HttpPostedFileBase pdfFile { get; set; }
		[DisplayName( "DWG File" )]
		public HttpPostedFileBase dwgFile { get; set; }
		[DisplayName( "Rendering Image" )]
		public HttpPostedFileBase imgFile { get; set; }

		public FileListing xlsFileList { get; set; }
		public FileListing sifFileList { get; set; }
		public FileListing sp4FileList { get; set; }
		public FileListing pdfFileList { get; set; }
		public FileListing dwgFileList { get; set; }
		public FileListing imgFileList { get; set; }
		
	}
}
