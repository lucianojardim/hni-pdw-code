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
    public partial class GSAContract
    {
        public GSAContract()
        {
            this.SpecRequests = new HashSet<SpecRequest>();
        }
    
        public int ContractID { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<SpecRequest> SpecRequests { get; set; }
    }
    
}
