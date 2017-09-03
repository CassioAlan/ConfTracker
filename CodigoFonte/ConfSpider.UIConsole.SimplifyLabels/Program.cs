using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.SimplifyLabels
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Configurar

            string _csvPath = @"U:\development\CRF_webpages\";
            var _crossValidation = true;

            #endregion

            var _pastaExec = string.Empty;
            var _dataFileNames = new List<string>();

            if (_crossValidation)
            {
                _pastaExec = "fold_data\\";
                _dataFileNames.Add("train.data");
                _dataFileNames.Add("test.data");
            }
            //else
            //{
            //    _pastaExec = "all_data\\";
            //    _dataFileNames.Add("allSites.data");
            //}
            // Se não vai criar novo modelo (utilizando crossvalidation) não pode rodar esse script

            foreach (var _dataFileName in _dataFileNames)
            {
                var _csvs = Directory.GetFiles(_csvPath + _pastaExec, _dataFileName, SearchOption.AllDirectories).ToList();
                foreach (var _csv in _csvs)
                {
                    var _file = File.ReadAllText(_csv, Encoding.UTF8);

                    _file = _file.Replace("S_", "");
                    _file = _file.Replace("B_", "");
                    _file = _file.Replace("M_", "");
                    _file = _file.Replace("E_", "");

                    File.WriteAllText(_csv, _file, Encoding.UTF8);
                }
            }
        }
    }
}
