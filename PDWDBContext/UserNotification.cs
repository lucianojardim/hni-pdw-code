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
    public partial class UserNotification
    {
        public int UserID { get; set; }
        public bool NewCollateralOrder { get; set; }
        public bool NewCollateralOrderTerritory { get; set; }
        public bool NewCollateralOrderShipment { get; set; }
        public bool NewCollateralOrderShipmentTerritory { get; set; }
        public bool NewSpecRequest { get; set; }
        public bool NewSpecRequestTerritory { get; set; }
        public bool CompleteSpecRequest { get; set; }
        public bool CompleteSpecRequestTerritory { get; set; }
        public bool UpdateSpecRequest { get; set; }
        public bool UpdateSpecRequestTerritory { get; set; }
        public bool ReOpenSpecRequest { get; set; }
        public bool ReOpenSpecRequestTerritory { get; set; }
    
        public virtual User User { get; set; }
    }
    
}
