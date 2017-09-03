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
    public class UrlBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public UrlBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<List<UrlTO>> RetrieveURLs(UrlTO urlTO)
        {
            var _response = new MessageResponse<List<UrlTO>>();

            try
            {
                _response.Result = new List<UrlTO>();

                var _idConferencia = 0;
                if (urlTO.Edicao != null && urlTO.Edicao.Conferencia != null)
                {
                    _idConferencia = urlTO.Edicao.Conferencia.IdConferencia;
                }

                var _ano = 0;
                if (urlTO.Edicao != null)
                {
                    _ano = urlTO.Edicao.Ano;
                }

                var _query = (from _u in m_EntityContext.urls
                              where _idConferencia == 0 || _u.idConferencia == _idConferencia
                              where _ano == 0 || _u.ano == _ano
                              where urlTO.Ordem == 0 || _u.ordem == urlTO.Ordem
                              orderby _u.idConferencia, _u.ano
                              select _u);

                _response.Result = _query.Select(_s => new UrlTO
                {
                    Edicao = new EdicaoTO { Conferencia = new ConferenciaTO { IdConferencia = _s.idConferencia},
                                            Ano = _s.ano },
                    Ordem = _s.ordem,
                    Url = _s.url1
                }).ToList();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<List<UrlTO>> RetrieveURLsConferenciasHabilitadas(UrlTO urlTO)
        {
            var _response = new MessageResponse<List<UrlTO>>();

            try
            {
                _response.Result = new List<UrlTO>();

                var _idConferencia = 0;
                if (urlTO.Edicao != null && urlTO.Edicao.Conferencia != null)
                {
                    _idConferencia = urlTO.Edicao.Conferencia.IdConferencia;
                }

                var _ano = 0;
                if (urlTO.Edicao != null)
                {
                    _ano = urlTO.Edicao.Ano;
                }

                var _idTipoExtracaoUrl = (int)urlTO.TipoExtracao;

                var _query = (from _conf in m_EntityContext.conferencias
                              from _ed in m_EntityContext.edicaos
                              from _url in m_EntityContext.urls

                              where _conf.idConferencia == _ed.idConferencia
                              where _ed.idConferencia == _url.idConferencia
                              where _ed.ano == _url.ano

                              //TODO: descomentar
                              //where _conf.enabled == true

                              where _idConferencia == 0 || _conf.idConferencia == _idConferencia
                              where _ano == 0 || _ed.ano == _ano
                              where urlTO.Ordem == 0 || _url.ordem == urlTO.Ordem

                              where _url.idTipoExtracao == _idTipoExtracaoUrl

                              select _url
                              );

                _response.Result = _query.Select(_s => new UrlTO
                {
                    Edicao = new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO { IdConferencia = _s.idConferencia },
                        Ano = _s.ano
                    },
                    Ordem = _s.ordem,
                    Url = _s.url1,
                    TipoExtracao = (EnumTipoExtracao)_s.idTipoExtracao
                }).ToList();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<int> SaveUrl(UrlTO urlTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (urlTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Url não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (urlTO.Edicao == null || urlTO.Edicao.Conferencia == null || urlTO.Edicao.Conferencia.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(urlTO.Edicao.Conferencia.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (urlTO.Edicao.Ano == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(urlTO.Edicao.Ano)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (urlTO.Ordem == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(urlTO.Ordem)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (urlTO.Url == null || urlTO.Url == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(urlTO.Url)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (urlTO.TipoExtracao == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(urlTO.TipoExtracao)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _url = m_EntityContext.urls.FirstOrDefault(c => c.idConferencia == urlTO.Edicao.Conferencia.IdConferencia &&
                                                                    c.ano == urlTO.Edicao.Ano &&
                                                                    c.ordem == urlTO.Ordem &&
                                                                    c.idTipoExtracao == (int)urlTO.TipoExtracao);
                if (_url == null)
                {
                    _url = new url
                    {
                        idConferencia = urlTO.Edicao.Conferencia.IdConferencia,
                        ano = urlTO.Edicao.Ano,
                        ordem = urlTO.Ordem,
                        idTipoExtracao = (int)urlTO.TipoExtracao
                    };

                    m_EntityContext.urls.Add(_url);
                }
                
                _url.url1 = urlTO.Url;

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
