using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class TopicoTO
    {
        public int IdTopico { get; set; }
        public string Descricao { get; set; }
        public int IdTopicoPai { get; set; }
    }
}
