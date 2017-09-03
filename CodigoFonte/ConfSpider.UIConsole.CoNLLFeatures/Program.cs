using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfSpider.TOLibrary;
using ConfSpider.BTLibrary;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Concurrent;

namespace ConfSpider.UIConsole.CoNLLFeatures
{
    public class Program
    {
        #region Configurar

        private static string m_csvPath = @"D:\webpages_2017\";
        //private static string m_csvPath = @"U:\development\CRF_webpages\_anotadas";
        private static int m_threadCount = 10;

        #endregion

        public static void Main(string[] args)
        {
            // TOKEN | Feature_W_ABS | Feature_W_PPR | Feature_W_ACC | Feature_W_CAM | Feature_W_EVE | DIA | MES | ANO | LABEL
            Console.WriteLine("[{0}] Iniciando processo principal.", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("[{0}] Buscando arquivos CSV.", Thread.CurrentThread.ManagedThreadId);
            var _csvs = Directory.GetFiles(m_csvPath, "*.csv", SearchOption.AllDirectories).ToList();
            Console.WriteLine("[{0}] {1} arquivos encontrados.", Thread.CurrentThread.ManagedThreadId, _csvs.Count);
            Console.Beep(1000, 1500);
            
            //Console.WriteLine("[{0}] Filtrando arquivos.", Thread.CurrentThread.ManagedThreadId);
            //_csvs = ConfSpider.Utils.Util.FilterFiles(_csvs);
            //Console.WriteLine("[{0}] {1} arquivos mantidos.", Thread.CurrentThread.ManagedThreadId, _csvs.Count);

            Feature_W(_csvs);
        }

        static void Feature_W(List<string> csvs)
        {
            Console.WriteLine("[{0}] Consultando base de dados: TiposData, Conferencias.", Thread.CurrentThread.ManagedThreadId);

            // Busca tokens de interesse na base
            var _tiposData = new TipoDataBT().RetrieveTiposData(new TipoDataTO());
            if (_tiposData.HasErrors())
            {
                Console.WriteLine(_tiposData.Exceptions);
                return;
            }

            // Busca siglas de conferência na base
            var _conferencias = new ConferenciaBT().RetrieveConferencias(new ConferenciaTO { Enabled = true });
            if (_conferencias.HasErrors())
            {
                Console.WriteLine(_conferencias.Exceptions);
                return;
            }
            var _siglas = _conferencias.Result.Select(_s => _s.Sigla.ToLower()).ToList();


            // TIPO DATA PAI - valores fixos na base de dados, por isso posso testá-los no código
            // 1 - abstract submission
            // 5 - paper submission
            // 17 - acceptance notification
            // 26 - camera-ready due
            // 33 - conference
            // 37 - conference end

            string[] _split = { " ?", " " };

            //ABS
            var _abss = _tiposData.Result.Where(_w => _w.IdTipoDataPai == 1).ToList();
            var _absTokens = new List<string>();
            foreach (var _i in _abss)
            {
                _absTokens.AddRange(_i.Descricao.Split(_split, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            _absTokens = _absTokens.Distinct().Where(_w => _w.Length > 2).ToList();
            _absTokens = _absTokens.Where(_w => !_w.Contains("paper") && !_w.Contains("submission")).ToList();

            //PPR
            var _pprs = _tiposData.Result.Where(_w => _w.IdTipoDataPai == 5).ToList();
            var _pprTokens = new List<string>();
            foreach (var _i in _pprs)
            {
                _pprTokens.AddRange(_i.Descricao.Split(_split, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            _pprTokens = _pprTokens.Distinct().Where(_w => _w.Length > 2 && _w != "for").ToList();

            //ACC
            var _accs = _tiposData.Result.Where(_w => _w.IdTipoDataPai == 17).ToList();
            var _accTokens = new List<string>();
            foreach (var _i in _accs)
            {
                _accTokens.AddRange(_i.Descricao.Split(_split, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            _accTokens = _accTokens.Distinct().Where(_w => _w.Length > 2).ToList();
            _accTokens = _accTokens.Where(_w => !_w.Contains("paper") && !_w.Contains("submission")).ToList();

            //CAM
            var _cams = _tiposData.Result.Where(_w => _w.IdTipoDataPai == 26).ToList();
            var _camTokens = new List<string>();
            foreach (var _i in _cams)
            {
                _camTokens.AddRange(_i.Descricao.Split(_split, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            _camTokens = _camTokens.Distinct().Where(_w => _w.Length > 2 && _w != "the").ToList();
            _camTokens = _camTokens.Where(_w => !_w.Contains("paper") && !_w.Contains("submission")).ToList();

            //EVE
            var _eves = _tiposData.Result.Where(_w => _w.IdTipoDataPai == 33 || _w.IdTipoDataPai == 37).ToList();
            var _eveTokens = new List<string>();
            foreach (var _i in _eves)
            {
                _eveTokens.AddRange(_i.Descricao.Split(_split, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            _eveTokens = _eveTokens.Distinct().Where(_w => _w.Length > 2).ToList();
            _eveTokens = _eveTokens.Where(_w => !_w.Contains("paper") && !_w.Contains("submission")).ToList();


            Console.WriteLine("[{0}] Criando threads.", Thread.CurrentThread.ManagedThreadId);

            ConcurrentQueue<string> _csvQueue = new ConcurrentQueue<string>(csvs);
            List<Thread> _threads = new List<Thread>();

            try
            {
                for (int _j = 0; _j < m_threadCount; _j++)
                {
                    var _t = new System.Threading.Thread(() =>
                    {
                        string _csv;
                        while (_csvQueue.TryDequeue(out _csv))
                        {
                            Console.WriteLine("[{0}] Arquivo: {1}", Thread.CurrentThread.ManagedThreadId, _csv);
                            var _csvLines = File.ReadAllLines(_csv);
                            
                            for (int _index = 0; _index < _csvLines.Length; _index++)
                            {
                                var _line = _csvLines[_index];
                                var _cols = _line.Split('\t').ToList();

                                int[] _found = { 0, // 0 ABS
                                     0, // 1 PPR
                                     0, // 2 ACC
                                     0, // 3 CAM
                                     0, // 4 EVE
                                     0, // 5 DIA
                                     0, // 6 MES
                                     0, // 7 ANO
                                     0, // 8 SIGLA DE CONFERENCIA
                                  // 0, // 9 Data com LBL de interesse antes [-1], Data com LBL depois [1]
                                   };

                                if (_cols.Count > 0)
                                {
                                    var _token = _cols.First();

                                    #region LBL de interesse
                                    //ABS
                                    foreach (var _i in _absTokens)
                                    {
                                        if (Regex.IsMatch(_token, _i))
                                        {
                                            _found[0] = 1;
                                            break;
                                        }
                                    }

                                    //PPR
                                    foreach (var _i in _pprTokens)
                                    {
                                        if (Regex.IsMatch(_token, _i))
                                        {
                                            _found[1] = 1;
                                            break;
                                        }
                                    }

                                    //ACC
                                    foreach (var _i in _accTokens)
                                    {
                                        if (Regex.IsMatch(_token, _i))
                                        {
                                            _found[2] = 1;
                                            break;
                                        }
                                    }

                                    //CAM
                                    foreach (var _i in _camTokens)
                                    {
                                        if (Regex.IsMatch(_token, _i))
                                        {
                                            _found[3] = 1;
                                            break;
                                        }
                                    }

                                    //EVE
                                    foreach (var _i in _eveTokens)
                                    {
                                        if (Regex.IsMatch(_token, _i))
                                        {
                                            _found[4] = 1;
                                            break;
                                        }
                                    }
                                    #endregion

                                    #region Dia, Mês, Ano

                                    // DIA
                                    if (DateUtilTO.IsDay(_token))
                                    {
                                        _found[5] = 1;
                                    }
                                    // MES
                                    if (DateUtilTO.IsMonth(_token))
                                    {
                                        _found[6] = 1;
                                    }
                                    // ANO
                                    if (DateUtilTO.IsYear(_token))
                                    {
                                        _found[7] = 1;
                                    }

                                    #endregion

                                    #region Sigla de Conferência

                                    foreach (var _i in _siglas)
                                    {
                                        if (Regex.IsMatch(_token, "^" + _i + "$"))
                                        {
                                            _found[8] = 1;
                                            break;
                                        }
                                    }

                                    #endregion
                                }

                                for (int _i = 0; _i < _found.Length; _i++)
                                {
                                    _cols.Insert(_i + 1, _found[_i].ToString());
                                }

                                _csvLines[_index] = string.Join("\t", _cols);
                            }
                            
                            var _csvX = "1";
                            File.WriteAllLines(_csv + _csvX, _csvLines);
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

            Console.WriteLine("[{0}] Concluído.", Thread.CurrentThread.ManagedThreadId);
            Console.ReadLine();

            
            ////Para cada linha de cada CSV, marca as colunas de feature nas quais o token é de interesse: ABS, PPR, ACC, CAM, EVE
            ////As datas são marcadas diretamente neste foreach
            //foreach (var _csv in csvs)
            //{
            //    var _csvLines = File.ReadAllLines(_csv);

            //    for (int _index = 0; _index < _csvLines.Length; _index++)
            //    {
            //        var _line = _csvLines[_index];
            //        var _cols = _line.Split('\t').ToList();

            //        int[] _found = { 0, // 0 ABS
            //                         0, // 1 PPR
            //                         0, // 2 ACC
            //                         0, // 3 CAM
            //                         0, // 4 EVE
            //                         0, // 5 DIA
            //                         0, // 6 MES
            //                         0, // 7 ANO
            //                         0, // 8 SIGLA DE CONFERENCIA
            //                      // 0, // 9 Data com LBL de interesse antes [-1], Data com LBL depois [1]
            //                       };

            //        if (_cols.Count > 0)
            //        {
            //            var _token = _cols.First();

            //            #region LBL de interesse
            //            //ABS
            //            foreach (var _i in _absTokens)
            //            {
            //                if (Regex.IsMatch(_token, _i))
            //                {
            //                    _found[0] = 1;
            //                    break;
            //                }
            //            }

            //            //PPR
            //            foreach (var _i in _pprTokens)
            //            {
            //                if (Regex.IsMatch(_token, _i))
            //                {
            //                    _found[1] = 1;
            //                    break;
            //                }
            //            }

            //            //ACC
            //            foreach (var _i in _accTokens)
            //            {
            //                if (Regex.IsMatch(_token, _i))
            //                {
            //                    _found[2] = 1;
            //                    break;
            //                }
            //            }

            //            //CAM
            //            foreach (var _i in _camTokens)
            //            {
            //                if (Regex.IsMatch(_token, _i))
            //                {
            //                    _found[3] = 1;
            //                    break;
            //                }
            //            }

            //            //EVE
            //            foreach (var _i in _eveTokens)
            //            {
            //                if (Regex.IsMatch(_token, _i))
            //                {
            //                    _found[4] = 1;
            //                    break;
            //                }
            //            } 
            //            #endregion

            //            #region Dia, Mês, Ano

            //            // DIA
            //            if (DateUtilTO.IsDay(_token))
            //            {
            //                _found[5] = 1;
            //            }
            //            // MES
            //            if (DateUtilTO.IsMonth(_token))
            //            {
            //                _found[6] = 1;
            //            }
            //            // ANO
            //            if (DateUtilTO.IsYear(_token))
            //            {
            //                _found[7] = 1;
            //            } 

            //            #endregion

            //            #region Sigla de Conferência

            //            foreach (var _i in _siglas)
            //            {
            //                if (Regex.IsMatch(_token, "^"+_i+"$"))
            //                {
            //                    _found[8] = 1;
            //                    break;
            //                }
            //            }

            //            #endregion
            //        }

            //        for (int _i = 0; _i < _found.Length; _i++)
            //        {
            //            _cols.Insert(_i + 1, _found[_i].ToString());
            //        }

            //        _csvLines[_index] = string.Join("\t", _cols);
            //    }


            //    #region LBL próximo a VLU
            //    // removida pois prejudicou os resultados
                
            //    /*
                
            //    // Funcionamento:
            //    //  Para cada linha do CSV
            //    //      Para cada coluna referente a data (combinação for e if)
            //    //          Para cada uma das x(_windowSize) linhas antes e depois
            //    //              Verifica cada coluna(feature) de interesse nas linhas antes e depois
            //    //                  Marca coluna 10 se há proximidade entre uma data qualquer e um feature de interesse

            //    // Features de interesse
            //    // 1-5: tipos de data
            //    // 9: sigla de conferência
            //    List<int> _columnInterestingFeatures = new List<int> { 1, 2, 3, 4, 5, 9 };

            //    //Rodando separadamente (depois do restante), pois a criação desta feature depende das features anteriores
            //    for (int _index = 0; _index < _csvLines.Length; _index++)
            //    {
            //        var _line = _csvLines[_index];
            //        var _cols = _line.Split('\t').ToList();
            //        for (int _dates = 6; _dates <= 8; _dates++) //_dates = posicoes na linha atual referente a dia, mes e ano
            //        {
            //            if (_cols[_dates] == "1")
            //            {
            //                var _windowSize = 5; //quantos tokens olhar antes e depois

            //                for (int _nearLineIndex = _windowSize; _nearLineIndex > 0; _nearLineIndex--) // _prevLineIndex = linha anterior sendo analisada
            //                {
            //                    //há labels de interesse ANTES da data
            //                    if (_index >= _nearLineIndex) //testa se não vai dar out of range exception na linha a baixo
            //                    {
            //                        var _prevLine = _csvLines[_index - _nearLineIndex];
            //                        var _prevCols = _prevLine.Split('\t').ToList();

            //                        foreach (var _fCol in _columnInterestingFeatures)
            //                        {
            //                            if (_prevCols[_fCol] == "1")
            //                            {
            //                                // nao é indice 9 pois aqui é a linha toda, nao so as features
            //                                _cols[10] = "-1"; //marca a coluna 9 sinalizando que há labels de interesse ANTES da data 
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    if (_cols[10] != "0")
            //                    {
            //                        break;
            //                    }

            //                    //há labels de interesse DEPOIS da data
            //                    if (_csvLines.Length > _index + _nearLineIndex)
            //                    {
            //                        var _nextLine = _csvLines[_index + _nearLineIndex];
            //                        var _nextCols = _nextLine.Split('\t').ToList();

            //                        foreach (var _fCol in _columnInterestingFeatures)
            //                        {
            //                            if (_nextCols[_fCol] == "1")
            //                            {
            //                                // nao é indice 9 pois aqui é a linha toda, nao so as features
            //                                _cols[10] = "1"; //marca a coluna 9 sinalizando que há labels de interesse ANTES da data 
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    if (_cols[10] != "0")
            //                    {
            //                        break;
            //                    }

            //                    //há labels de interesse DEPOIS da data
            //                    //if (_index <= _windowSize + _nearLineIndex) //testa se não vai dar out of range exception na linha a baixo
            //                    //{
            //                    //    var _nextLine = _csvLines[_index + _nearLineIndex];
            //                    //    var _nextCols = _nextLine.Split('\t').ToList();

            //                    //    for (int _lblCol = 0; _lblCol <= 4; _lblCol++) //colunas referente a labels de interesse
            //                    //    {
            //                    //        if (_nextCols[_lblCol] == "1")
            //                    //        {
            //                    //            // nao é indice 9 pois aqui é a linha toda, nao so as features
            //                    //            _cols[10] = "1"; //marca a coluna 9 sinalizando que há labels de interesse DEPOIS da data 
            //                    //        }
            //                    //    }
            //                    //}
            //                }
            //            }
            //            if (_cols[10] != "0")
            //            {
            //                break;
            //            }
            //        }
            //        if (_cols[10] != "0")
            //        {
            //            _csvLines[_index] = string.Join("\t", _cols);
            //        }
            //    }
            //    */
            //    #endregion

            //    var _csvX = "1";
            //    File.WriteAllLines(_csv+_csvX, _csvLines);
            //}
        }
    }
}
