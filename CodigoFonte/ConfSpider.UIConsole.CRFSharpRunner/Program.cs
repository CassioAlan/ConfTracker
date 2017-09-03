using ConfSpider.INL.ProcessComunicator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.CRFSharpRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configurar

            //mais threads dá problema de concorrência em um arquivo de log do CRFSharp
            int _threadCount = 1; 

            var _crfSharpExe = @"U:\development\CRFSharp\CRFSharp-master\CRFSharpConsole\bin\Release\CRFSharpConsole.exe";
            var _model = @"D:\Dropbox\Mestrado\ConfSpider\development\CRF_webpages\all_data\model_window5.model";
            var _dataPath = @"D:\webpages_2017"; 

            #endregion

            try
            {
                var _datas = Directory.GetFiles(_dataPath, "*.data", SearchOption.TopDirectoryOnly).ToList();
                _datas = _datas.Where(_w => !_w.Contains("result")).ToList();

                ConcurrentQueue<string> _dataQ = new ConcurrentQueue<string>(_datas);
                List<Thread> _downloaders = new List<Thread>();
                for (int _i = 0; _i < _threadCount; _i++)
                {
                    var _t = new System.Threading.Thread(() =>
                    {
                        string _data;
                        while (_dataQ.TryDequeue(out _data))
                        {
                            Console.WriteLine("[{0}] Arquivo: {1}", Thread.CurrentThread.ManagedThreadId, _data);

                            var _result = _data.Replace("site.data", "site_result.data");
                            var _args = $"-decode -modelfile {_model} -inputfile {_data} -outputfile {_result} -maxword 5000";

                            ProcessComunicatorSimple.ExecCommandSimple(_crfSharpExe, _args);
                        }
                    });

                    _downloaders.Add(_t);
                    _t.Start();
                }

                foreach (var _t in _downloaders)
                {
                    _t.Join();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[{0}] Erro: {1}", Thread.CurrentThread.ManagedThreadId, ex.Message);
            }
        }
    }
}
