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
    public partial class CollateralOrderDetail
    {
        public CollateralOrderDetail()
        {
            this.CollateralOrderShipmentDetails = new HashSet<CollateralOrderShipmentDetail>();
        }
    
        public int DetailID { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public string ItemName { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> GroupQuantity { get; set; }
        public int CollateralTypeNKID { get; set; }
        public string CollateralTypeName { get; set; }
        public int CollateralNKID { get; set; }
        public Nullable<int> GroupNKID { get; set; }
    
        public virtual CollateralOrder CollateralOrder { get; set; }
        public virtual ICollection<CollateralOrderShipmentDetail> CollateralOrderShipmentDetails { get; set; }
    }
    
}
