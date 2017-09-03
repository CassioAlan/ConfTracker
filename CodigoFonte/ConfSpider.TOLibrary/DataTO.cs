using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class DataTO
    {
        public EdicaoTO Edicao { get; set; } = new EdicaoTO();
        public int IdTipoData { get; set; }
        public DateTime Data { get; set; }
        public EnumTipoExtracao TipoExtracao { get; set; }
    }
}
