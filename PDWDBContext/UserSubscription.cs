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
    public partial class UserSubscription
    {
        public int UserID { get; set; }
        public bool ProductIntroductions { get; set; }
        public bool BehindTheScenes { get; set; }
        public bool MeetOurMembers { get; set; }
        public bool ProgramChanges { get; set; }
        public bool PricelistUpdates { get; set; }
        public bool QuoteRequests { get; set; }
        public bool SMSAlerts { get; set; }
        public string SMSPhoneNumber { get; set; }
    
        public virtual User User { get; set; }
    }
    
}
