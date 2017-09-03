using ConfSpider.INL.ProcessComunicator;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using Masters.LogHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfSpider.INL.WebPagesDownload
{
    public class WGetDownloader
    {
        private string m_wget_exe_path;

        //const string EXE_PATH_TEST = @"U:\development\ConfSpider\ConfSpider.Aux.CountingTime\bin\Debug\ConfSpider.Aux.CountingTime.exe";

        public WGetDownloader()
        {
            m_wget_exe_path = ConfigurationManager.AppSettings["wget_exe_path"]; 
        }

        public event EventHandler<WebPagesDownloaderEventArgs> ReceivedMessage;
        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new WebPagesDownloaderEventArgs(message));
        }

        public MessageResponse<object> DownloadAll(List<UrlTO> urls, string savingPath)
        {
            var _response = new MessageResponse<object>();

            try
            {
                #region Test
                //ConcurrentQueue<string> _urls = new ConcurrentQueue<string>(_respUrl.Result.Select(_s => _s.Url).Take(35));
                //List<Thread> _downloaders = new List<Thread>();
                //for (int _i = 0; _i < 10; _i++)
                //{
                //    var _t = new System.Threading.Thread(() =>
                //    {
                //        string url;
                //        while (_urls.TryDequeue(out url))
                //        {
                //            ProcessComunicatorSimple.ExecCommandSimple(EXE_PATH_TEST, url);
                //            Console.WriteLine(url);
                //        }
                //    });

                //    _downloaders.Add(_t);
                //    _t.Start();
                //} 

                //foreach (var _t in _downloaders)
                //{
                //    _t.Join();
                //}
                #endregion

                ConcurrentQueue<UrlTO> _urls = new ConcurrentQueue<UrlTO>(urls);
                List<Thread> _downloaders = new List<Thread>();
                for (int _i = 0; _i < 10; _i++)
                {
                    var _t = new System.Threading.Thread(() =>
                    {
                        UrlTO _url;
                        while (_urls.TryDequeue(out _url))
                        {
                            Logger.WriteDebug($"Iniciando - Conferência {_url.Edicao.Conferencia.IdConferencia} - Url {_url.Url}.");
                            OnReceivedMessage($"Iniciando - Conferência {_url.Edicao.Conferencia.IdConferencia} - Url {_url.Url}.");
                            //var _args = $"-p {_url.Url} -P {SAVE_PATH} -olog";
                            //var _args = $"-p -k -e robots=off -U Mozilla {_url.Url} -P {SAVE_PATH} -olog";
                            var _args = $"-nc -E -r -l 2 -k -p -R mp3,mp4,pdf,ppt,pptx,doc,docx,zip,7z,rar {_url.Url} -P {savingPath}/{_url.Edicao.Conferencia.IdConferencia.ToString().PadLeft(4, '0')}/{_url.Ordem} --no-check-certificate -U Mozilla -e robots=off";
                            // -r               -> download recursively all files linked to a page you want to download (pesa muito)
                            // -l 3             -> baixa 3 níveis de documentos linkados pela página (pesa muito)
                            // -p               -> download all the files that are necessary to properly display a given HTML page
                            
                            // a princípio não está sendo necessário ignorar robots
                            // -e robots=off    -> ignora o robots \o/
                            // -U Mozilla       -> muda o user-agent (when web server does not allow download managers)
                            
                            // -k               -> converte os links para exibição offline

                            //TESTAR COM    -p --convert-links     no lugar de -k
                            //TESTAR COM    -p --convert-links -r     no lugar de -k

                            ProcessComunicatorSimple.ExecCommandSimple(m_wget_exe_path, _args);
                            Logger.WriteDebug($"Concluído - Conferência {_url.Edicao.Conferencia.IdConferencia} - Url {_url.Url}.");
                            OnReceivedMessage($"Concluído - Conferência {_url.Edicao.Conferencia.IdConferencia} - Url {_url.Url}.");
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
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }
    }
}
