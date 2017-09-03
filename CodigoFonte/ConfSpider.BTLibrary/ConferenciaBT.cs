using ConfSpider.POLibrary;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.BTLibrary
{
    public class ConferenciaBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public ConferenciaBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<ConferenciaTO> RetrieveConferencia(ConferenciaTO conferenciaTO)
        {
            var _response = new MessageResponse<ConferenciaTO>();

            try
            {
                #region Validations
                if (conferenciaTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Conferência não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (conferenciaTO.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(conferenciaTO.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                } 
                #endregion

                var _query = m_EntityContext.conferencias.FirstOrDefault(w => w.idConferencia == conferenciaTO.IdConferencia);

                if (_query == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Conferência não encontrada", ResponseKind.Error));
                }
                else
                {
                    _response.Result = new ConferenciaTO
                    {
                        IdConferencia = _query.idConferencia,
                        Nome = _query.nome,
                        Sigla = _query.sigla
                    };
                }
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<List<ConferenciaTO>> RetrieveConferencias(ConferenciaTO conferenciaTO)
        {
            var _response = new MessageResponse<List<ConferenciaTO>>();

            try
            {
                _response.Result = new List<ConferenciaTO>();

                var _conferencias = (from p in m_EntityContext.conferencias.Include("edicaos")
                                     where conferenciaTO.Nome == null || p.nome.Contains(conferenciaTO.Nome)
                                     where conferenciaTO.Sigla == null || p.sigla == conferenciaTO.Sigla
                                     where conferenciaTO.Enabled == false || p.enabled
                                     orderby p.idConferencia
                                     select p).ToList();

                foreach (var _c in _conferencias)
                {
                    var _conf = new ConferenciaTO
                    {
                        IdConferencia = _c.idConferencia,
                        Nome = _c.nome,
                        Sigla = _c.sigla
                    };

                    var _edicoes = _c.edicaos.OrderBy(o => o.ano).ToList();

                    foreach (var _e in _edicoes)
                    {
                        var _ed = new EdicaoTO
                        {
                            Ano = _e.ano,
                            Qualis = _e.qualis,
                            Query = _e.query
                        };
                        _conf.Edicoes.Add(_ed);

                        var _urls = _e.urls.ToList();

                        foreach (var _url in _urls)
                        {
                            _ed.Urls.Add(new UrlTO
                            {
                                Ordem = _url.ordem,
                                TipoExtracao = (EnumTipoExtracao)_url.idTipoExtracao,
                                Url = _url.url1
                            });
                        }
                    }

                    _response.Result.Add(_conf);
                }
                
                //_response.Result = _query.Select(c => new ConferenciaTO
                //{
                //    IdConferencia = c.idConferencia,
                //    Nome = c.nome,
                //    Sigla = c.sigla,
                //    Edicoes = c.edicaos.Select(e => new EdicaoTO
                //    {
                //        Ano = e.ano,
                //        Qualis = e.qualis,
                //        Query = e.query
                //    }).ToList()
                //}).ToList();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<List<ConferenciaTO>> RetrieveConferenciasWithDatesAndURL(int ano, EnumTipoExtracao tipoExtracaoData, EnumTipoExtracao tipoExtracaoUrl)
        {
            var _response = new MessageResponse<List<ConferenciaTO>>();

            try
            {
                _response.Result = new List<ConferenciaTO>();

                var _tipoExtracaoData = Convert.ToInt32(tipoExtracaoData);
                var _tipoExtracaoUrl = Convert.ToInt32(tipoExtracaoUrl);

                var _queryConf = (from p in m_EntityContext.conferencias
                              where p.enabled
                              orderby p.idConferencia
                              select p).ToList();

                foreach (var _c in _queryConf)
                {
                    var _conf = new ConferenciaTO
                    {
                        IdConferencia = _c.idConferencia,
                        Nome = _c.nome,
                        Sigla = _c.sigla
                    };

                    var _queryEd = (from e in m_EntityContext.edicaos
                                    where e.idConferencia == _c.idConferencia
                                    where e.ano == ano
                                    select e).ToList();
                    
                    foreach (var _e in _queryEd)
                    {
                        var _ed = new EdicaoTO
                        {
                            Ano = _e.ano,
                            Qualis = _e.qualis,
                            Query = _e.query
                        };

                        _conf.Edicoes.Add(_ed);

                        var _queryDt = (from _d in m_EntityContext.data
                                        where _d.idConferencia == _c.idConferencia
                                        where _d.edicao.ano == ano
                                        where _d.idTipoExtracao == _tipoExtracaoData
                                        select _d).ToList();

                        foreach (var _dt in _queryDt)
                        {
                            var _data = new DataTO
                            {
                                Data = _dt.data,
                                IdTipoData = _dt.tipodata.idTipoData
                            };

                            _ed.Datas.Add(_data);
                        }

                        var _urls = (from _d in m_EntityContext.urls
                                     where _d.idConferencia == _c.idConferencia
                                     where _d.edicao.ano == ano
                                     where _d.idTipoExtracao == _tipoExtracaoUrl
                                     select _d).ToList();

                        foreach (var _url in _urls)
                        {
                            var _u = new UrlTO
                            {
                                Url = _url.url1,
                                Ordem = _url.ordem,
                                TipoExtracao = (EnumTipoExtracao)_url.idTipoExtracao
                            };

                            _ed.Urls.Add(_u);
                        }
                    }

                    _response.Result.Add(_conf);
                }
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        //public MessageResponse<List<ConferenciaTO>> RetrieveURLsConferencias(int ano, EnumTipoExtracao tipoExtracao)
        //{
        //    var _response = new MessageResponse<List<ConferenciaTO>>();

        //    try
        //    {
        //        _response.Result = new List<ConferenciaTO>();

        //        var _tipoExtracao = Convert.ToInt32(tipoExtracao);

        //        var _queryConf = (from p in m_EntityContext.conferencias
        //                          where p.enabled
        //                          orderby p.idConferencia
        //                          select p).ToList();

        //        foreach (var _c in _queryConf)
        //        {
        //            var _conf = new ConferenciaTO
        //            {
        //                IdConferencia = _c.idConferencia,
        //                Nome = _c.nome,
        //                Sigla = _c.sigla
        //            };

        //            var _eds = (from e in m_EntityContext.edicaos
        //                            where e.idConferencia == _c.idConferencia
        //                            where e.ano == ano
        //                            select e).ToList();

        //            foreach (var _e in _eds)
        //            {
        //                var _ed = new EdicaoTO
        //                {
        //                    Ano = _e.ano,
        //                    Qualis = _e.qualis,
        //                    Query = _e.query
        //                };

        //                _conf.Edicoes.Add(_ed);

        //                var _urls = (from _d in m_EntityContext.urls
        //                                where _d.idConferencia == _c.idConferencia
        //                                where _d.edicao.ano == ano
        //                                where _d.idTipoExtracao == _tipoExtracao
        //                                select _d).ToList();

        //                foreach (var _url in _urls)
        //                {
        //                    var _u = new UrlTO
        //                    {
        //                        Url = _url.url1,
        //                        Ordem = _url.ordem,
        //                        TipoExtracao = (EnumTipoExtracao)_url.idTipoExtracao
        //                    };

        //                    _ed.Urls.Add(_u);
        //                }
        //            }

        //            _response.Result.Add(_conf);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
        //    }

        //    return _response;
        //}

        public MessageResponse<int> SaveConferencia(ConferenciaTO conferenciaTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (conferenciaTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Conferência não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (conferenciaTO.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(conferenciaTO.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (conferenciaTO.Nome == null || conferenciaTO.Nome == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(conferenciaTO.Nome)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (conferenciaTO.Sigla == null || conferenciaTO.Sigla == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(conferenciaTO.Sigla)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _conferencia = m_EntityContext.conferencias.FirstOrDefault(c => c.idConferencia == conferenciaTO.IdConferencia);
                if (_conferencia == null)
                {
                    _conferencia = new conferencia
                    {
                        idConferencia = conferenciaTO.IdConferencia
                    };

                    m_EntityContext.conferencias.Add(_conferencia);
                }

                _conferencia.nome = conferenciaTO.Nome;
                _conferencia.sigla = conferenciaTO.Sigla;
                _conferencia.enabled = conferenciaTO.Enabled;

                m_EntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }
        
        public MessageResponse<bool> DeleteConferencia(ConferenciaTO conferenciaTO)
        {
            var _response = new MessageResponse<bool>();

            try
            {
                #region Validations
                if (conferenciaTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Conferência não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (conferenciaTO.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(conferenciaTO.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }
                #endregion

                var _confToDelete = m_EntityContext.conferencias.FirstOrDefault(_w => _w.idConferencia == conferenciaTO.IdConferencia);

                if (_confToDelete == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Conferência não encontrada.", ResponseKind.Error));
                    return _response;
                }

                if(_confToDelete.edicaos.Count() > 0)
                {
                    _response.Exceptions.Add(new MessageResponseException("Não é possível deletar a conferência, pois há edições vinculadas a ela.", ResponseKind.Error));
                    return _response;
                }

                m_EntityContext.conferencias.Remove(_confToDelete);

                m_EntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        #endregion
    }
}
