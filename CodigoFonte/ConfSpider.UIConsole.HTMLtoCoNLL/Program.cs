using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.HTMLtoCoNLL
{
    public class Program
    {
        private static string[] delimiters = new string[] {
            "{ ", " }", "( ", " )", " [", "]", ">", "<","-", "_", "= ", "+",
            "|", "\\", ":", ";", " ", ",", ".", "/", "?", "~", "!",
            "@", "#", "$", "%", "^", "&", "*", " ", "\r", "\n", "\t" };

        public static void Main(string[] args)
        {
            #region Configurar

            string _htmlPath = @"D:\webpages_2017";
            int _threadCount = 10;

            #endregion
            Console.WriteLine("[{0}] Iniciando processo principal.", Thread.CurrentThread.ManagedThreadId);

            var _folders = Directory.GetDirectories(_htmlPath, "*", SearchOption.TopDirectoryOnly);
            ConcurrentQueue<string> _foldersQueue = new ConcurrentQueue<string>(_folders);
            List<Thread> _threads = new List<Thread>();

            try
            {
                for (int _i = 0; _i < _threadCount; _i++)
                {
                    var _t = new System.Threading.Thread(() =>
                    {
                        string _folder;
                        while (_foldersQueue.TryDequeue(out _folder))
                        {
                            Console.WriteLine("[{0}] Executando pasta {1}.", Thread.CurrentThread.ManagedThreadId, _folder);
                            var _htmls = Directory.GetFiles(_folder, @"*.htm?", SearchOption.AllDirectories).ToList();
                            _htmls = ConfSpider.Utils.Util.FilterFiles(_htmls);

                            foreach (var _html in _htmls)
                            {
                                Console.WriteLine("[{0}] Arquivo: {1}", Thread.CurrentThread.ManagedThreadId, _html);
                                if (new FileInfo(_html).Length < 2000000)
                                {
                                    try
                                    {
                                        var _alltextFilepath = _html + "_all.txt";
                                        if (_alltextFilepath.Length < 260)
                                        {
                                            var _document = new HtmlDocument();
                                            _document.Load(_html);

                                            var _htmlText = _document.DocumentNode.InnerText.ToLower();

                                            File.WriteAllText(_alltextFilepath, _htmlText, Encoding.UTF8);

                                            _htmlText = Regex.Replace(_htmlText, @"(<(.|\n)*?>)", "\n"); // |(\s+)
                                            _htmlText = Regex.Replace(_htmlText, @"(\s{2,})", "\n");

                                            var _textFilepath = _html + ".txt";
                                            File.WriteAllText(_textFilepath, _htmlText, Encoding.UTF8);

                                            var _lines = new List<string>();

                                            // Tokens
                                            _lines.AddRange(_htmlText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList());

                                            // setting a default Feature
                                            //_lines = _lines.Select(_line => setDefaultFeature(_line)).ToList();

                                            // setting a default Label
                                            _lines = _lines.Select(_line => setDefaultLabel(_line)).ToList();

                                            var _CoNLLfilepath = _html + "_CoNLL.csv";
                                            File.WriteAllLines(_CoNLLfilepath, _lines, Encoding.UTF8);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("[{0}] Erro: {1} - {2}", Thread.CurrentThread.ManagedThreadId, _html, ex.Message);
                                    }
                                }
                            }

                            Console.WriteLine("[{0}] Concluiu pasta {1}.", Thread.CurrentThread.ManagedThreadId, _folder);
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


            //var _htmls = Directory.GetFiles(_htmlPath, @"*.htm?", SearchOption.AllDirectories).ToList();

            //foreach (var _html in _htmls)
            //{
            //    Console.WriteLine("File: {0}", _html);
            //    if (new FileInfo(_html).Length < 2000000)
            //    {
            //        try
            //        {
            //            var _alltextFilepath = _html + "_all.txt";
            //            if (_alltextFilepath.Length < 260)
            //            {
            //                var _document = new HtmlDocument();
            //                _document.Load(_html);

            //                var _htmlText = _document.DocumentNode.InnerText.ToLower();

            //                File.WriteAllText(_alltextFilepath, _htmlText, Encoding.UTF8);

            //                _htmlText = Regex.Replace(_htmlText, @"(<(.|\n)*?>)", "\n"); // |(\s+)
            //                _htmlText = Regex.Replace(_htmlText, @"(\s{2,})", "\n");

            //                var _textFilepath = _html + ".txt";
            //                File.WriteAllText(_textFilepath, _htmlText, Encoding.UTF8);

            //                var _lines = new List<string>();

            //                // Tokens
            //                _lines.AddRange(_htmlText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList());

            //                // setting a default Feature
            //                //_lines = _lines.Select(_line => setDefaultFeature(_line)).ToList();

            //                // setting a default Label
            //                _lines = _lines.Select(_line => setDefaultLabel(_line)).ToList();

            //                var _CoNLLfilepath = _html + "_CoNLL.csv";
            //                File.WriteAllLines(_CoNLLfilepath, _lines, Encoding.UTF8);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine("Erro: {0} - {1}", _html, ex.Message);
            //        }
            //    }
            //}

        }

        static string setDefaultFeature(string s)
        {
            // new Collumn
            s += addCollumn();

            // add default Label
            s += "_";

            return s;
        }

        static string setDefaultLabel(string s)
        {
            // new Collumn
            s += addCollumn();

            // add default Label
            s += "0";

            return s;
        }

        static string addCollumn()
        {
            return "\t";
        }
    }
}
