using ConfSpider.BLL.UrlSearch;
using ConfSpider.BLL.ConferencesPagesDownload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.WebPagesDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            UrlSearcher _urlSearcher = new UrlSearcher();
            _urlSearcher.ReceivedMessage += _urlSearch_ReceivedMessage;

            #region Search Query Generator
            //var _retUrlGQ = _urlSearcher.GenerateQueries();
            //if (_retUrlGQ.HasErrors())
            //{
            //    foreach (var _ex in _retUrlGQ.Exceptions)
            //    {
            //        Console.WriteLine(_ex.Description);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Geração de queries concluída\nTecle ENTER para sair.");
            //}
            //Console.ReadLine();
            #endregion

            #region Search Runner
            var _retUrlSearcher = _urlSearcher.SearchURLs();
            if (_retUrlSearcher.HasErrors())
            {
                foreach (var _ex in _retUrlSearcher.Exceptions)
                {
                    Console.WriteLine(_ex.Description);
                }
            }
            Console.WriteLine("Pesquisa de URLs concluída\nTecle ENTER para sair.");
            Console.ReadLine();
            #endregion

            #region Downloader Runner
            ConferencesPagesDownloader _wDownloader = new ConferencesPagesDownloader();
            _wDownloader.ReceivedMessage += _wDownloader_ReceivedMessage;
            var _retWDownloader = _wDownloader.DownloadAll(2017);

            if (_retWDownloader.HasErrors())
            {
                Console.WriteLine(_retWDownloader.Exceptions.First().Description);
            }
            Console.WriteLine("Download de páginas concluído\nTecle ENTER para sair.");
            Console.ReadLine();
            #endregion
        }

        private static void _urlSearch_ReceivedMessage(object sender, UrlSearchEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void _wDownloader_ReceivedMessage(object sender, ConferencesPagesDownloaderEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
