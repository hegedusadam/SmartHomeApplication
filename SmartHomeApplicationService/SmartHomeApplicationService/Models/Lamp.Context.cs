﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SmartHomeApplicationDatabaseLamps : DbContext
    {
        public SmartHomeApplicationDatabaseLamps()
            : base("name=SmartHomeApplicationDatabaseLamps")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Lamp> Lamps { get; set; }

		public System.Data.Entity.DbSet<SmartHomeApplicationService.Models.Change> Changes { get; set; }
	}
}