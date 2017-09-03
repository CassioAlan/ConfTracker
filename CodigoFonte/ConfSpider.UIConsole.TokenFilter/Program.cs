using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.TokenFilter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Configurar

            string _csvPath = @"U:\development\CRF_webpages\_anotadas";
            var _csvX = "1";

            #endregion

            var _csvs = Directory.GetFiles(_csvPath, "*.csv"+_csvX, SearchOption.AllDirectories);
            foreach (var _csv in _csvs)
            {
                var _allLines = File.ReadAllLines(_csv, Encoding.UTF8).ToList();

                var _lines = new List<ConllLineTO>();

                foreach (var _line in _allLines)
                {
                    _lines.Add(new ConllLineTO(_line));
                }

                var _label0List = _allLines.Where(_w => _w.Split('\t').Last() == "0").Skip(50).ToList();

                foreach (var _label0 in _label0List)
                {
                    var _line = new ConllLineTO(_label0);

                    if (_line.FAbstract == "0" && _line.FPaper == "0" && _line.FAcception == "0" && _line.FCameraReady == "0" && _line.FEvent == "0")
                    {
                        _allLines.Remove(_label0);
                    }
                }

                File.WriteAllLines(_csv + _csvX, _allLines);
            }
        }
    }
}
