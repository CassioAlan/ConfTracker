using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class UrlTO
    {
        public EdicaoTO Edicao { get; set; } = new EdicaoTO();
        //Edicao.Conferencia.IdConferencia, Edicao.Ano
        public int Ordem { get; set; }
        public string Url { get; set; }
        public EnumTipoExtracao TipoExtracao { get; set; }

    }
}
