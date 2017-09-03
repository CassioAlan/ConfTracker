using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class ConferenciaTO
    {
        public int IdConferencia { get; set; }
        public string Sigla { get; set; }
        public string Nome { get; set; }
        public bool Enabled { get; set; }
        public List<EdicaoTO> Edicoes { get; set; } = new List<EdicaoTO>();
    }
}
