using ConfSpider.TOLibrary;
using HtmlAgilityPack;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfSpider.INL.HtmlManipulate
{
    public class HtmlManipulator
    {
        #region Members
        //private string m_filePath;
        private HtmlDocument m_document;
        private string m_htmlText;
        private Regex m_href_regex;
   
        private string[] m_excludingWords;
        private string m_dateURLsPattern;
        
        #endregion

        public HtmlManipulator(string html, bool isFile)
        {
            // se isFile == true, precisa passar o local de um arquivo
            // se for false, precisa passar o conteúdo html
            if (isFile)
            {
                //m_filePath = filePath;
                m_document = new HtmlDocument();
                m_document.Load(html);

                m_htmlText = m_document.DocumentNode.InnerText.ToLower();
                m_htmlText = Regex.Replace(m_htmlText, @"<!--(.|\n)*?-->", " ");
            }
            else
            {
                m_htmlText = html;
                m_document = new HtmlDocument();
                m_document.LoadHtml(html);
            }

            m_href_regex = new Regex("<a[^>]* href=\"(?<href>([^ \"]*))\"");
            m_excludingWords = ConfigurationManager.AppSettings["excludingWords"].Split(',');
            m_dateURLsPattern = ConfigurationManager.AppSettings["dateURLsPattern"];
        }
        
        public enum EnumSideOfDate
        {
            Both,
            Left,
            Right
        }

        public MessageResponse<List<string>> GetHREFs()
        {
            var _response = new MessageResponse<List<string>>();

            try
            {
                var _h = m_href_regex.Matches(m_document.DocumentNode.InnerHtml);
                _response.Result = _h.Cast<Match>().Select(_s => _s.Groups["href"].Value).ToList();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex.Message));
            }

            //try
            //{
            //    var _nodes = m_document.DocumentNode.SelectNodes("//a");
            //    if (_nodes != null && _nodes.Count > 0)
            //    {
            //        var _hrefs = _nodes.Select(p => p.GetAttributeValue("href", "not found")).ToList();
            //        _response.Result = _hrefs;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _response.Exceptions.Add(new MessageResponseException(ex.Message));
            //}

            return _response;
        }

        //public MessageResponse<List<string>> GetURLs()
        //{
        //    var _response = new MessageResponse<List<string>>();

        //    foreach (HtmlNode _link in m_document.DocumentNode.SelectNodes("//a[@href]"))
        //    {
        //        HtmlAttribute _att = _link.Attributes["href"];
        //        _response.Result.Add(_att.Value);
        //    }

        //    return _response;
        //}

        public MessageResponse<List<string>> GetDateURLs(List<string> _respGetURLs)
        {
            var _response = new MessageResponse<List<string>>();
            _response.Result = new List<string>();

            try
            {
                var _r = new Regex(m_dateURLsPattern, RegexOptions.IgnoreCase);

                foreach (var _link in _respGetURLs)
                {
                    if (!m_excludingWords.Any(_w => _link.Contains(_w)) &&
                        _r.IsMatch(_link))
                    {
                        _response.Result.Add(_link);
                    }
                }
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }
            
            return _response;
        }

        public MessageResponse<int> HasKeywords(List<string> keywords)
        {
            var _response = new MessageResponse<int>();

            _response.Result = keywords.Count(_w => m_htmlText.Contains(_w));

            return _response;
        }

        public MessageResponse<int> HasDates()
        {
            var _response = new MessageResponse<int>();

            var _regex = new Regex(DateUtilTO.AllDatesPattern, RegexOptions.IgnoreCase);
            
            var _matches = _regex.Matches(m_htmlText);

            _response.Result = _matches.Count;

            return _response;
        }

        public MessageResponse<Tuple<DateTime, EnumSideOfDate>> FindNearDate(List<string> keywords, EnumSideOfDate sideToLook)
        {
            var _response = new MessageResponse<Tuple<DateTime, EnumSideOfDate>>();

            var _foundKeywords = keywords.Where(_w => m_htmlText.Contains(_w)).ToList();

            //var _limit = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);

            //foreach (var _foundK in _foundKeywords)
            //{
            //    var _pieces = m_htmlText.Split(new string[1] { _foundK }, StringSplitOptions.RemoveEmptyEntries);
            
            var _limit = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);

            foreach (var _key in keywords)
            {
                var _pieces = Regex.Split(m_htmlText, _key, RegexOptions.IgnoreCase);
                //m_htmlText.Split(new string[1] { _foundK }, StringSplitOptions.RemoveEmptyEntries);
                //
                if (_pieces.Length >= 2) //achou a chave no texto
                {
                    for (int i = 0; i < _pieces.Length; i++)
                    {
                        #region Regra antiga, acho que furada...
                        ////pedaço par (iniciando em 0), tenho que encontrar a data mais próxima do final do pedaço
                        //// "um texto qualquer 01/01/16 submission deadline outro texto qualquer"
                        //// "um texto qualquer submission deadline 01/01/16 outro texto qualquer"
                        //if (i % 2 == 0 && sideToLook != EnumSideOfDate.Right) //se quer apenas datas à direita, não posso analisar os pedaços par
                        //{
                        //    var _text = _pieces[i].Substring(_pieces[i].Length > 50 ? _pieces[i].Length - 50 : 0);

                        //    //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                        //    var _dates = FindDatesOnText(_text);
                        //    if (_dates.Count > 0)
                        //    {
                        //        var _last = _dates.Last();
                        //        if (_last > _limit)
                        //        {
                        //            _response.Result = new Tuple<DateTime, EnumSideOfDate>(_last, EnumSideOfDate.Left);
                        //            return _response;
                        //        }
                        //    }

                        //}
                        ////pedaço ímpar, tenho que encontrar a data mais próxima do início
                        //if (i % 2 == 1 && sideToLook != EnumSideOfDate.Left) //se quer apenas datas à esquerda, não posso analisar os pedaços ímpar
                        //{
                        //    var _text = _pieces[i].Substring(0, _pieces[i].Length > 50 ? 50 : _pieces[i].Length);

                        //    //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                        //    var _dates = FindDatesOnText(_text);
                        //    if (_dates.Count > 0)
                        //    {
                        //        var _first = _dates.First();
                        //        if (_first > _limit)
                        //        {
                        //            _response.Result = new Tuple<DateTime, EnumSideOfDate>(_first, EnumSideOfDate.Right);
                        //            return _response;
                        //        }
                        //    }
                        //} 
                        #endregion
                        
                        if (sideToLook != EnumSideOfDate.Right) //se for left ou both
                        {
                            //pego a parte final do texto
                            var _text = _pieces[i].Substring(_pieces[i].Length > 50 ? _pieces[i].Length - 50 : 0);

                            //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                            var _dates = DateUtilTO.FindDatesOnText(_text);
                            if (_dates.Count > 0)
                            {
                                var _last = _dates.Last();
                                if (_last > _limit)
                                {
                                    _response.Result = new Tuple<DateTime, EnumSideOfDate>(_last, EnumSideOfDate.Left);
                                    return _response;
                                }
                            }
                        }

                        if (sideToLook != EnumSideOfDate.Left) //se for right ou both
                        {
                            //pego a parte inicial do texto
                            var _text = _pieces[i].Substring(0, _pieces[i].Length > 50 ? 50 : _pieces[i].Length);

                            //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                            var _dates = DateUtilTO.FindDatesOnText(_text);
                            if (_dates.Count > 0)
                            {
                                var _first = _dates.First();
                                if (_first > _limit)
                                {
                                    _response.Result = new Tuple<DateTime, EnumSideOfDate>(_first, EnumSideOfDate.Right);
                                    return _response;
                                }
                            }
                        }
                    }
                }
            }

            return _response;
        }

        public MessageResponse<Tuple<DateTime, DateTime>> FindNearIntervalDate(List<string> keywords, EnumSideOfDate sideToLook)
        {
            var _response = new MessageResponse<Tuple<DateTime, DateTime>>();

            var _foundKeywords = keywords.Where(_w => m_htmlText.Contains(_w)).ToList();

            var _limit = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);

            foreach (var _foundK in _foundKeywords)
            {
                var _pieces = m_htmlText.Split(new string[1] { _foundK }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < _pieces.Length; i++)
                {
                    //pedaço par (iniciando em 0), tenho que encontrar a data mais próxima do final do pedaço
                    // "um texto qualquer 01/01/16 submission deadline outro texto qualquer"
                    // "um texto qualquer submission deadline 01/01/16 outro texto qualquer"
                    if (i % 2 == 0 && sideToLook != EnumSideOfDate.Right) //se quer apenas datas à direita, não posso analisar os pedaços par
                    {
                        var _text = _pieces[i].Substring(_pieces[i].Length > 100 ? _pieces[i].Length - 100 : 0);

                        //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                        var _dates = DateUtilTO.FindIntervalDatesOnText(_text);
                        if (_dates.Count > 0)
                        {
                            var _last = _dates.Last();
                            if (_last.Item1 > _limit)
                            {
                                _response.Result = new Tuple<DateTime, DateTime>(_last.Item1, _last.Item2);
                                return _response;
                            }
                        }

                    }
                    //pedaço ímpar, tenho que encontrar a data mais próxima do início
                    if (i % 2 == 1 && sideToLook != EnumSideOfDate.Left) //se quer apenas datas à esquerda, não posso analisar os pedaços ímpar
                    {
                        var _text = _pieces[i].Substring(0, _pieces[i].Length > 100 ? 100 : _pieces[i].Length);

                        //não uso LastOrDefault para ter certeza que manterá nulo caso não encontre data
                        var _dates = DateUtilTO.FindIntervalDatesOnText(_text);
                        if (_dates.Count > 0)
                        {
                            var _first = _dates.Last();
                            if (_first.Item1 > _limit)
                            {
                                _response.Result = new Tuple<DateTime, DateTime>(_first.Item1, _first.Item2);
                                return _response;
                            }
                        }
                    }
                }
            }

            return _response;
        }

        
    }
}
