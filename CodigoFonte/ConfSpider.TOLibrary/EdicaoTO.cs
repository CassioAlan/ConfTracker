using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class EdicaoTO
    {
        public ConferenciaTO Conferencia { get; set; } = new ConferenciaTO();
        public int Ano { get; set; }
        public string Query { get; set; }
        public string Qualis { get; set; }

        public List<UrlTO> Urls { get; set; } = new List<UrlTO>();

        public List<DataTO> Datas { get; set; } = new List<DataTO>();
    }
}
