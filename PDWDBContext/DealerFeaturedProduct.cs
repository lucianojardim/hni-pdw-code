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
    public partial class DealerFeaturedProduct
    {
        public int DealerID { get; set; }
        public string SeriesName { get; set; }
    
        public virtual Dealer Dealer { get; set; }
    }
    
}
