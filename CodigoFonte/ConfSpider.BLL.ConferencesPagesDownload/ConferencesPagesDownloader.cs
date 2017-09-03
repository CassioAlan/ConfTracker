using ConfSpider.BTLibrary;
using ConfSpider.INL.WebPagesDownload;
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

namespace ConfSpider.BLL.ConferencesPagesDownload
{
    public class ConferencesPagesDownloader
    {
        private string m_saving_webpages_path;
        
        public ConferencesPagesDownloader()
        {
            m_saving_webpages_path = ConfigurationManager.AppSettings["saving_webpages_path"];
        }

        public event EventHandler<ConferencesPagesDownloaderEventArgs> ReceivedMessage;
        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new ConferencesPagesDownloaderEventArgs(message));
        }

        public MessageResponse<object> DownloadAll(int anoEdicao)
        {
            var _response = new MessageResponse<object>();

            try
            {
                var _urlBT = new UrlBT();

                //var _respUrl = _urlBT.RetrieveURLsConferenciasHabilitadas(new UrlTO { TipoExtracao = EnumTipoExtracao.Google, Edicao = new EdicaoTO { Ano = anoEdicao } });

                //pegando apenas a primeira URL
                var _respUrl = _urlBT.RetrieveURLsConferenciasHabilitadas(new UrlTO { Ordem = 1, TipoExtracao = EnumTipoExtracao.Google, Edicao = new EdicaoTO { Ano = anoEdicao } });

                //var _respUrl = _urlBT.RetrieveURLsConferenciasHabilitadas(new UrlTO { TipoExtracao = EnumTipoExtracao.Manual, Edicao = new EdicaoTO { Ano = anoEdicao } });


                if (_respUrl.HasErrors())
                {
                    _response.Exceptions.AddRange(_respUrl.Exceptions);
                    return _response;
                }

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

                var _downloader = new WGetDownloader();
                _downloader.ReceivedMessage += _downloader_ReceivedMessage;
                var _respDownloader = _downloader.DownloadAll(_respUrl.Result, m_saving_webpages_path);
                if (_respDownloader.HasErrors())
                {
                    return _respDownloader;
                }
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        private void _downloader_ReceivedMessage(object sender, WebPagesDownloaderEventArgs e)
        {
            OnReceivedMessage(e.Message);
        }
    }
}
