//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace PDWDBContext
{
    public partial class ImageFile
    {
        public ImageFile()
        {
            this.SeriesImageFiles = new HashSet<SeriesImageFile>();
            this.TypicalImageFiles = new HashSet<TypicalImageFile>();
            this.PublicationImages = new HashSet<PublicationImage>();
            this.Showrooms = new HashSet<Company>();
            this.eCollateralSections = new HashSet<eCollateralSection>();
            this.ImageFileSerieses = new HashSet<ImageFileSeries>();
        }
    
        public int ImageID { get; set; }
        public string Name { get; set; }
        public string OriginalExtension { get; set; }
        public string Caption { get; set; }
        public bool HasPeople { get; set; }
        public string ImageType { get; set; }
        public string Keyword { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string MIMEType { get; set; }
        public string FeaturedEdge { get; set; }
        public string FeaturedPull { get; set; }
        public int ImageContent { get; set; }
        public Nullable<int> FinishType { get; set; }
        public string FeaturedFinish { get; set; }
        public string DBKeywords { get; set; }
        public string TableShape { get; set; }
        public string TableBase { get; set; }
        public string ControlMechanism { get; set; }
        public string ControlDescription { get; set; }
        public Nullable<int> GoToGuidePage { get; set; }
        public Nullable<int> Popularity { get; set; }
        public Nullable<int> LaminatePattern { get; set; }
        public Nullable<int> VeneerGrade { get; set; }
        public Nullable<int> VeneerSpecies { get; set; }
        public bool LaminateIsHPL { get; set; }
        public string FinishCode { get; set; }
        public Nullable<int> SeatingGrade { get; set; }
        public bool LaminateIsTFL { get; set; }
        public string FeaturedSeries { get; set; }
        public string SeriesList { get; set; }
        public string ImageApplicationList { get; set; }
    
        public virtual ICollection<SeriesImageFile> SeriesImageFiles { get; set; }
        public virtual ICollection<TypicalImageFile> TypicalImageFiles { get; set; }
        public virtual ICollection<PublicationImage> PublicationImages { get; set; }
        public virtual ICollection<Company> Showrooms { get; set; }
        public virtual ICollection<eCollateralSection> eCollateralSections { get; set; }
        public virtual ICollection<ImageFileSeries> ImageFileSerieses { get; set; }
    }
    
}
