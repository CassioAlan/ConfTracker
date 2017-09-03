using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.LabelVerification
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configurar

            string _csvPath = @"D:\Dropbox\ConfSpider\CRF_webpages\";

            #endregion
            
            // lista de possíveis labels
            List<string> _positions = new List<string> { "S_", "B_", "M_", "E_" };
            List<string> _types = new List<string> { "VLU_", "LBL_" };
            List<string> _dateTypes = new List<string> { "ABS", "PPR", "ACC", "CAM", "EVE" };

            List<string> _possibleLabels = new List<string> { "0" };

            foreach (var _pos in _positions)
            {
                foreach (var _type in _types)
                {
                    foreach (var _dateT in _dateTypes)
                    {
                        _possibleLabels.Add(_pos + _type + _dateT);
                    }
                }
            }

            var _csvs = Directory.GetFiles(_csvPath, "*.csv", SearchOption.AllDirectories);
            foreach (var _csv in _csvs)
            {
                Console.WriteLine(_csv);

                var _allLines = File.ReadAllLines(_csv, Encoding.UTF8).ToList();
                var _labels = _allLines.Select(_s => _s.Split('\t')[1]).ToList();

                for (int i = 0; i < _labels.Count; i++)
                {
                    var _label = _labels[i];

                    #region Verifica se os labels estão no conjunto esperado
                    if (!_possibleLabels.Contains(_label))
                    {
                        Console.WriteLine("{0} : {1} - fora dos possíveis valores", i+1, _label);
                        Console.ReadLine();
                    }
                    #endregion

                    #region S_
                    // S_ não pode ocorrer após B_ nem M_, nem antes de E_

                    if (_label.StartsWith("S_"))
                    {
                        if (i > 0 && (_labels[i - 1].StartsWith("B_") || _labels[i - 1].StartsWith("M_")))
                        {
                            Console.WriteLine("{0} : {1} - S_ fora do lugar?", i+1, _label);
                            Console.ReadLine();
                        }
                        if (i < _labels.Count - 1 && _labels[i + 1].StartsWith("E_"))
                        {
                            Console.WriteLine("{0} : {1} - S_ fora do lugar?", i+1, _label);
                            Console.ReadLine();
                        }
                    }

                    #endregion

                    #region B_
                    // Depois de B_ deve ocorrer ou M_ ou E_

                    if (_label.StartsWith("B_"))
                    {
                        if (i < _labels.Count - 1 && (!_labels[i + 1].StartsWith("M_") && !_labels[i + 1].StartsWith("E_")))
                        {
                            Console.WriteLine("{0} : {1} - B_ fora do lugar?", i+1, _label);
                            Console.ReadLine();
                        }
                    }

                    #endregion

                    #region M_
                    // Antes de M_ deve ocorrer B_ e depois de M_ deve ocorrer E_, ou M_ em qualquer um dos lados
                    if (_label.StartsWith("M_"))
                    {
                        if (i > 0 && (!_labels[i - 1].StartsWith("B_") && !_labels[i - 1].StartsWith("M_")))
                        {
                            Console.WriteLine("{0} : {1} - M_ fora do lugar? (anterior nao é B_ nem M_)", i+1, _label);
                            Console.ReadLine();
                        }
                        if (i < _labels.Count - 1 && (!_labels[i + 1].StartsWith("E_") && !_labels[i + 1].StartsWith("M_")))
                        {
                            Console.WriteLine("{0} : {1} - M_ fora do lugar? (posterior não é E_ nem M_)", i+1, _label);
                            Console.ReadLine();
                        }
                    }
                    #endregion

                    #region E_
                    // E_ não pode ocorrer depois de 0, as demais verificações já estão contempladas nos testes anteriores
                    if (_label.StartsWith("E_"))
                    {
                        if (i > 0 && _labels[i - 1].StartsWith("0"))
                        {
                            Console.WriteLine("{0} : {1} - E_ fora do lugar? (anterior é 0)", i+1, _label);
                            Console.ReadLine();
                        }
                    }

                    #endregion
                }
            }

            Console.WriteLine("Verificação finalizada. Tecle Enter para sair.");
            Console.ReadLine();
        }
    }
}