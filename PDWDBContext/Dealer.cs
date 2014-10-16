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
    public partial class Dealer
    {
        public Dealer()
        {
            this.PageLinks = new HashSet<PageLink>();
            this.VideoLinks = new HashSet<VideoLink>();
            this.DealerFeaturedProducts = new HashSet<DealerFeaturedProduct>();
        }
    
        public int DealerID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string MainContent { get; set; }
        public int FeaturedVideoID { get; set; }
        public string ProductsHeadline { get; set; }
        public string PagesHeadline { get; set; }
        public string VideosHeadline { get; set; }
    
        public virtual ICollection<PageLink> PageLinks { get; set; }
        public virtual ICollection<VideoLink> VideoLinks { get; set; }
        public virtual VideoLink FeaturedVideo { get; set; }
        public virtual ICollection<DealerFeaturedProduct> DealerFeaturedProducts { get; set; }
    }
    
}
