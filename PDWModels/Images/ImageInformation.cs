using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDWModels.Images
{
	public class ImageInformation
	{
		public enum ImageContents
		{
			All = 0,
			Image = 1,
			Edge = 2,
			Pull = 3,
			Finish = 4,
			TableShape = 5,
			TableBase = 6,
			ControlMech = 7,
			GoToGuide = 8
		}

		public enum FinishTypes
		{
			Veneer = 1,
			Laminate = 2,
			Seating = 3
		}

		public enum VeneerSpecieses
		{
			Cherry = 1,
			Maple = 2,
			Walnut = 3,
			RiftedOak = 4,
			EuropeanWalnut = 5,
			GrayOak = 6
		}

		public enum VeneerGrades
		{
			Standard = 1,
			Premium = 2
		}

		public enum LaminatePatterns
		{
			Solid = 4, 
			WoodGrain = 5
		}

		public enum SeatingGrades
		{
			Standard = 1,
			Premium = 2
		}

		public enum ImageApplications
		{
			PrivateOffice = 1,
			Teaming = 2,
			Collaborative = 3,
			Training = 4,
			Cafe = 5
		}

		public int ImageID { get; set; }
		[Required]
		[DisplayName("Image Name")]
		public string ImageName { get; set; }
		[Required]
		[DisplayName( "Caption" )]
		public string Caption { get; set; }
		[Required]
		[DisplayName( "Keywords" )]
		public string Keywords { get; set; }
		[DisplayName( "People in Picture?" )]
		public bool HasPeople { get; set; }
		[Required]
		[DisplayName( "Image Type" )]
		public string ImageType { get; set; }
		[DisplayName( "Image Content" )]
		public int ImageContent { get; set; }

		[DisplayName( "Featured Pull" )]
		public string FeaturedPull { get; set; }

		[DisplayName( "Featured Edge" )]
		public string FeaturedEdge { get; set; }
		
		[DisplayName( "Featured Finish" )]
		public string FeaturedFinish { get; set; }
		[DisplayName( "Finish Type" )]
		public int? FinishType { get; set; }
		[DisplayName( "Finish Code" )]
		public string FinishCode { get; set; }
		[DisplayName( "Laminate Pattern" )]
		public int? LaminatePattern { get; set; }
		[DisplayName( "Is TFL Available?" )]
		public bool LaminateIsTFL { get; set; }
		[DisplayName( "Is HPL Available?" )]
		public bool LaminateIsHPL { get; set; }
		[DisplayName( "Veneer Grade" )]
		public int? VeneerGrade { get; set; }
		[DisplayName( "Veneer Species" )]
		public int? VeneerSpecies { get; set; }
		[DisplayName( "Seating Grade" )]
		public int? SeatingGrade { get; set; }
		[DisplayName( "Image Application" )]
		public string ImageApplication { get; set; }

		[DisplayName( "Featured Table Shape" )]
		public string FeaturedTableShape { get; set; }

		[DisplayName( "Featured Table Base" )]
		public string FeaturedTableBase { get; set; }

		[DisplayName( "Control Mechanism" )]
		public string ControlMechanism { get; set; }
		[DisplayName( "Control Description" )]
		public string ControlDescription { get; set; }

		[DisplayName( "Go To Guide Page Number" )]
		public int GoToGuidePageNum { get; set; }

		[DisplayName( "Additional Series" )]
		public string SeriesList { get; set; }
		[DisplayName( "Featured Series" )]
		[StringLength( 50 )]
		public string FeaturedSeries { get; set; }
	}
}
