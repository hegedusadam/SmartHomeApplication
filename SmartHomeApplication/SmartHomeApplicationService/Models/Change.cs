//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartHomeApplicationService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Change
    {
        public int Id { get; set; }
        public System.DateTime date { get; set; }
        public bool state { get; set; }
        public int lampid { get; set; }
    
        public virtual Lamp Lamp { get; set; }
    }
}
