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
    public partial class PublicationImage
    {
        public int PublicationID { get; set; }
        public int ImageID { get; set; }
        public Nullable<int> PageNumber { get; set; }
    
        public virtual ImageFile ImageFile { get; set; }
        public virtual Publication Publication { get; set; }
    }
    
}
