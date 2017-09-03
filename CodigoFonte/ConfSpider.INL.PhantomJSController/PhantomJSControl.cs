using ConfSpider.TOLibrary;
using NReco.PhantomJS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.INL.PhantomJSController
{
    public class PhantomJSControl
    {
        string m_phantomjsFilesPath;
        public PhantomJSControl()
        {
            //m_phantomjsFilesPath = ConfigurationManager.AppSettings["phantomjs_files_path"];
            m_phantomjsFilesPath = "";
        }

        public void FindInformationPosition(string htmlFilePath, string abstractPattern, string paperPattern, string notificationPattern, string finalPattern, string conferecePattern)
        {
            var _phantomJS = new PhantomJS();

            _phantomJS.ExecutionTimeout = TimeSpan.FromSeconds(45);

            _phantomJS.OutputReceived += (sender, e) =>
            {
                Console.WriteLine("PhantomJS output: {0}", e.Data);
            };
            _phantomJS.ErrorReceived += (sender, e) =>
            {
                Console.WriteLine("PhantomJS error: {0}", e.Data);
            };

            try
            {
                _phantomJS.Run(m_phantomjsFilesPath + "main.js", new[] { htmlFilePath, abstractPattern, paperPattern, notificationPattern, finalPattern, conferecePattern });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao rodar script PhantomJS: {0}", ex.Message);
            }
        }
    }
}
