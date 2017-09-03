using ConfSpider.BTLibrary;
using ConfSpider.INL.UrlSearchEngine;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConfSpider.BLL.UrlSearch
{
    public class UrlSearcher
    {
        private int m_retrieve_url_count;

        public UrlSearcher()
        {
            m_retrieve_url_count = Convert.ToInt32(ConfigurationManager.AppSettings["retrieve_url_count"]);
        }

        public MessageResponse<object> SearchURLs()
        {
            var _response = new MessageResponse<object>();

            try
            {
                var _conferenciaBT = new ConferenciaBT();
                var _urlBT = new UrlBT();

                //var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO
                //{
                //    Enabled = true
                //});
                var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO());

                if (_resp.HasErrors())
                {
                    return new MessageResponse<object> { Exceptions = _response.Exceptions };
                }

                var _conferencias = _resp.Result;

                #region BingSearch
                //var _bingSearch = new BingSearch();
                //_bingSearch.ReceivedMessage += _bingSearch_ReceivedMessage;
                //var _retURLs = _bingSearch.SearchURLs(_conferencias, m_retrieve_url_count);
                //if (_retURLs.HasErrors())
                //{
                //    return _retURLs;
                //} 

                //foreach (var _conf in _conferencias)
                //{
                //    foreach (var _ed in _conf.Edicoes)
                //    {
                //        for (int _c = 0; _c < _ed.Urls.Count; _c++)
                //        {
                //            var _ret = _urlBT.SaveUrl(new UrlTO
                //            {
                //                Edicao = new EdicaoTO
                //                {
                //                    Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                //                    Ano = _ed.Ano
                //                },
                //                Ordem = _c + 1,
                //                Url = _ed.Urls[_c].Url
                //            });

                //            if (_ret.HasErrors())
                //            {
                //                Console.WriteLine(_ret.Exceptions.First().Description);
                //                _response.Exceptions.AddRange(_ret.Exceptions);
                //            }
                //        }
                //    }
                //}
                #endregion

                #region GoogleSearch

                var _googleSearch = new GoogleSearch();

                foreach (var _conf in _conferencias)
                {
                    var _edicao = _conf.Edicoes.LastOrDefault();

                    OnReceivedMessage($"Buscando urls para a conferência {_conf.IdConferencia}");

                    if (_edicao != null)
                    {
                        //não posso usar, pois está zoando a pesquisa... resultado diferente do retornado no ambiente web da google
                        //var _excludingTerms = "";// " -wikicfp -.pdf -.doc -" + (DateTime.Now.Year - 1).ToString();

                        var _query = _edicao.Query.Replace("\"", "");

                        var _results = _googleSearch.CustomSearch(_query, 10);

                        var _posExcludingTerms = new List<String>() { "wikicfp", "facebook", "wikipedia", "twitter", "linkedin", "github", ".pdf", ".doc", (DateTime.Now.Year - 1).ToString() };

                        _results = _results.Where(_w => !_posExcludingTerms.Any(_w.Contains)).Take(m_retrieve_url_count).ToList();

                        for (int _c = 0; _c < _results.Count; _c++)
                        {
                            var _ret = _urlBT.SaveUrl(new UrlTO
                            {
                                Edicao = new EdicaoTO
                                {
                                    Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                                    Ano = _edicao.Ano
                                },
                                Ordem = _c + 1,
                                Url = _results[_c],
                                TipoExtracao = EnumTipoExtracao.Google
                            });

                            if (_ret.HasErrors())
                            {
                                Console.WriteLine(_ret.Exceptions.First().Description);
                                _response.Exceptions.AddRange(_ret.Exceptions);
                            }
                        }
                    }
                }
                #endregion
                
                return _response;
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
                return _response;
            }
        }

        public MessageResponse<object> GenerateQueries()
        {
            var _response = new MessageResponse<object>();

            try
            {
                var _conferenciaBT = new ConferenciaBT();
                var _edicaoBT = new EdicaoBT();

                //var _resp = _conferenciaBT.RetrieveConferencias(new ConferenciaTO
                //{
                //    Enabled = true
                //});
                var _resp = _conferenciaBT.RetrieveConferencias(new ConferenciaTO());

                if (_resp.HasErrors())
                {
                    _response.Exceptions.AddRange(_resp.Exceptions);
                    return _response;
                }

                var _conferencias = _resp.Result;

                var _countConf = _conferencias.Count;
                foreach (var _conf in _conferencias)
                {
                    OnReceivedMessage($"Gerando query para conferência {_conf.IdConferencia} de {_countConf}");
                    var _edicaoTO = _conf.Edicoes.LastOrDefault();

                    if (_edicaoTO != null)
                    {
                        //_edicaoTO.Query = $"{DateTime.Now.Year} {DateTime.Now.Year + 1} {_conf.Sigla} {_conf.Nome} important date deadline submition call for paper";
                        _edicaoTO.Query = $"{DateTime.Now.Year} {_conf.Sigla} {_conf.Nome}";
                    }

                    var _edicaoToSave = new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                        Ano = _edicaoTO.Ano,
                        Query = _edicaoTO.Query,
                        Qualis = _edicaoTO.Qualis
                    };

                    var _respEd = _edicaoBT.SaveEdicao(_edicaoToSave);
                    if (_respEd.HasErrors())
                    {
                        _response.Exceptions.AddRange(_respEd.Exceptions);
                    }
                }

                return _response;
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
                return _response;
            }
        }
        
        public event EventHandler<UrlSearchEventArgs> ReceivedMessage;

        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new UrlSearchEventArgs(message));
        }

        private void _bingSearch_ReceivedMessage(object sender, UrlSearchEngineEventArgs e)
        {
            this.OnReceivedMessage(e.Message);
        }

    }
}
