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
    public partial class Typical
    {
        public Typical()
        {
            this.TypicalImageFiles = new HashSet<TypicalImageFile>();
            this.TypicalIntAttributes = new HashSet<TypicalIntAttribute>();
            this.TypicalOptionAttributes = new HashSet<TypicalOptionAttribute>();
            this.TypicalTextAttributes = new HashSet<TypicalTextAttribute>();
            this.SeriesTypicals = new HashSet<SeriesTypical>();
        }
    
        public int TypicalID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string DBKeywords { get; set; }
        public Nullable<int> SpecRequestID { get; set; }
        public string FeaturedSeries { get; set; }
        public string SeriesList { get; set; }
        public Nullable<bool> AvailableForIn2 { get; set; }
        public string Notes { get; set; }
        public bool IsPublished { get; set; }
    
        public virtual ICollection<TypicalImageFile> TypicalImageFiles { get; set; }
        public virtual ICollection<TypicalIntAttribute> TypicalIntAttributes { get; set; }
        public virtual ICollection<TypicalOptionAttribute> TypicalOptionAttributes { get; set; }
        public virtual ICollection<TypicalTextAttribute> TypicalTextAttributes { get; set; }
        public virtual ICollection<SeriesTypical> SeriesTypicals { get; set; }
        public virtual SpecRequest SpecRequest { get; set; }
    }
    
}
