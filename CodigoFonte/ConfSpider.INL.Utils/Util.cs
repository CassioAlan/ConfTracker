using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfSpider.Utils
{
    public static class Util
    {
        public static List<string> FilterFiles(List<string> files)
        {
            List<string> _ret = files;

            // arquivos com nomes que possui maior probabilidade de ter as datas
            var _dateURLsPattern = @"index|home|conference|deadline|due|call.?for|cfp|submission|information|date|panel|datas|confer.?ncia|limite|chamada"; //var _dateURLsPattern = @"conference|deadline|call.for|cfp|submission|date|panel|datas|information";
            var _r = new Regex(_dateURLsPattern, RegexOptions.IgnoreCase);
            _ret = _ret.Where(_w => _r.IsMatch(_w) && !_w.Contains('@')).ToList();

            // Tamanho máximo de 500KB
            _ret = _ret.Where(_w => new FileInfo(_w).Length < 50000).ToList(); 

            return _ret;
        }
    }
}
