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
    public partial class CollateralType
    {
        public CollateralType()
        {
            this.CollateralItems = new HashSet<CollateralItem>();
        }
    
        public int CollateralTypeID { get; set; }
        public string Name { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    
        public virtual ICollection<CollateralItem> CollateralItems { get; set; }
    }
    
}
