//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class topico
    {
        public topico()
        {
            this.edicaotopicoes = new HashSet<edicaotopico>();
            this.topico1 = new HashSet<topico>();
        }
    
        public int idTopico { get; set; }
        public string descricao { get; set; }
        public int idTopicoPai { get; set; }
    
        public virtual ICollection<edicaotopico> edicaotopicoes { get; set; }
        public virtual ICollection<topico> topico1 { get; set; }
        public virtual topico topico2 { get; set; }
    }
}