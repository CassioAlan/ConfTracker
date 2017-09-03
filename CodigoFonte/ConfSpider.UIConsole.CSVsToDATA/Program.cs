using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.CSVsToDATA
{
    public class Program
    {
        private static int m_threadCount = 10;

        public static void Main(string[] args)
        {
            #region Configurar

            string _csvPath = @"D:\webpages_2017\";
            var _crossValidation = false;

            #endregion

            var _csvXX = "1"; // quanto não há crossvalidation
            if (_crossValidation)
            {
                _csvXX = "11"; //quando há crossvalidation
            }

            var _pastaExec = string.Empty;
            //var _csvs = Directory.GetFiles(_csvPath, "*.csv" + _csvXX, SearchOption.AllDirectories).ToList();
            //_csvs = ConfSpider.Utils.Util.FilterFiles(_csvs);

            //setar com o número de features + label existentes, para criar a linha com o número da conferência
            var _countFeaturesAndLabel = 10;

            #region Sem cross validation

            if (!_crossValidation)
            {
                var _folders = Directory.GetDirectories(_csvPath, "*", SearchOption.TopDirectoryOnly);

                ConcurrentQueue<string> _foldersQueue = new ConcurrentQueue<string>(_folders);
                List<Thread> _threads = new List<Thread>();

                try
                {
                    for (int _j = 0; _j < m_threadCount; _j++)
                    {
                        var _t = new System.Threading.Thread(() =>
                        {
                            string _folder;
                            while (_foldersQueue.TryDequeue(out _folder))
                            {
                                Console.WriteLine("[{0}] Pasta: {1}", Thread.CurrentThread.ManagedThreadId, _folder);
                                var _csvs = Directory.GetFiles(_folder, "*.csv" + _csvXX, SearchOption.AllDirectories).ToList();
                                _csvs = ConfSpider.Utils.Util.FilterFiles(_csvs);

                                var _allCSVs = string.Empty;

                                foreach (var _csv in _csvs)
                                {
                                    // Conference identificator
                                    var _confID = "conf" + Path.GetDirectoryName(_csv).Split(Path.DirectorySeparatorChar)[2];
                                    for (int i = 0; i < _countFeaturesAndLabel; i++)
                                    {
                                        _confID += "\t0";
                                    }

                                    if (_allCSVs == string.Empty)
                                    {
                                        _allCSVs = _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
                                    }
                                    else
                                    {
                                        _allCSVs += "\n" + _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
                                    }
                                }

                                File.WriteAllText(_folder + "site.data", _allCSVs);
                            }
                        });

                        _threads.Add(_t);
                        _t.Start();
                    }

                    foreach (var _t in _threads)
                    {
                        _t.Join();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[{0}] Erro: {1}", Thread.CurrentThread.ManagedThreadId, ex.Message);
                }

                //_pastaExec = "all_data\\";
                //if (!Directory.Exists(_csvPath + _pastaExec))
                //{
                //    Directory.CreateDirectory(_csvPath + _pastaExec);
                //}

                //var _allCSVs = string.Empty;

                //foreach (var _csv in _csvs)
                //{
                //    // Conference identificator
                //    var _confID = "conf" + Path.GetDirectoryName(_csv).Split(Path.DirectorySeparatorChar).Last();
                //    for (int i = 0; i < _countFeaturesAndLabel; i++)
                //    {
                //        _confID += "\t0";
                //    }

                //    if (_allCSVs == string.Empty)
                //    {
                //        _allCSVs = _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
                //    }
                //    else
                //    {
                //        _allCSVs += "\n" + _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
                //    }
                //}

                //File.WriteAllText(_csvPath + _pastaExec + "allSites.data", _allCSVs); 
            }

            #endregion

            #region Cross Validation

            //if (_crossValidation)
            //{
            //    _pastaExec = "fold_data\\";
            //    if (!Directory.Exists(_csvPath + _pastaExec))
            //    {
            //        Directory.CreateDirectory(_csvPath + _pastaExec);
            //    }

            //    var _folds = 10;

            //    var _countCsv = _csvs.Count();
            //    var _countInFold = _countCsv / _folds;

            //    for (int _fold = 0; _fold < _folds; _fold++)
            //    {
            //        var _testItems = _csvs.Skip(_fold * _countInFold).Take(_countInFold).ToList();

            //        var _trainItems = _csvs.Take(_fold * _countInFold).ToList();
            //        _trainItems.AddRange(_csvs.Skip((_fold + 1) * _countInFold).Take((_folds - _fold + 1) * _countInFold).ToList());

            //        var _foldFolder = _csvPath + _pastaExec + "fold_" + _fold.ToString().PadLeft(3, '0') + "\\";
            //        if (!Directory.Exists(_foldFolder))
            //        {
            //            Directory.CreateDirectory(_foldFolder);
            //        }

            //        var _testText = string.Empty;
            //        foreach (var _csv in _testItems)
            //        {
            //            // Conference identificator
            //            var _confID = "conf" + Path.GetDirectoryName(_csv).Split(Path.DirectorySeparatorChar).Last();
            //            for (int i = 0; i < _countFeaturesAndLabel; i++)
            //            {
            //                _confID += "\t0";
            //            }

            //            if (_testText == string.Empty)
            //            {
            //                _testText = _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
            //            }
            //            else
            //            {
            //                _testText += "\n" + _confID + "\n" + File.ReadAllText(_csv, Encoding.UTF8);
            //            }
            //        }
            //        File.WriteAllText(_foldFolder + "test.data", _testText);

            //        var _trainText = string.Empty;
            //        foreach (var _csv in _trainItems)
            //        {
            //            if (_trainText == string.Empty)
            //            {
            //                _trainText = File.ReadAllText(_csv, Encoding.UTF8);
            //            }
            //            else
            //            {
            //                _trainText += "\n" + File.ReadAllText(_csv, Encoding.UTF8);
            //            }
            //        }
            //        File.WriteAllText(_foldFolder + "train.data", _trainText);
            //    } 
            //}

            #endregion
        }
    }
}