﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConfSpider.POLibrary
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ConfSpiderEntities : DbContext
    {
        public ConfSpiderEntities()
            : base("name=ConfSpiderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<conferencia> conferencias { get; set; }
        public DbSet<datum> data { get; set; }
        public DbSet<edicao> edicaos { get; set; }
        public DbSet<edicaotopico> edicaotopicoes { get; set; }
        public DbSet<tipodata> tipodatas { get; set; }
        public DbSet<url> urls { get; set; }
        public DbSet<topico> topicoes { get; set; }
        public DbSet<tipoextracao> tipoextracaos { get; set; }
    }
}
