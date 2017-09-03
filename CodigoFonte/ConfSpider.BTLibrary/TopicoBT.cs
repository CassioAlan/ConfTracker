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
    public class TopicoBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public TopicoBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<int> SaveTopico(TopicoTO topicoTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (topicoTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Topico não pode ser nulo.", ResponseKind.Error));
                    return _response;
                }

                if (topicoTO.IdTopico == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(topicoTO.IdTopico)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }
                
                if (topicoTO.Descricao == null || topicoTO.Descricao == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(topicoTO.Descricao)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (topicoTO.IdTopicoPai == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(topicoTO.IdTopicoPai)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _topico = m_EntityContext.topicoes.FirstOrDefault(c => c.idTopico == topicoTO.IdTopico);
                if (_topico == null)
                {
                    _topico = new topico
                    {
                        idTopico = topicoTO.IdTopico
                    };

                    m_EntityContext.topicoes.Add(_topico);
                }

                _topico.descricao = topicoTO.Descricao;
                _topico.idTopicoPai = topicoTO.IdTopicoPai;

                m_EntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<List<TopicoTO>> RetrieveConferencias(TopicoTO topicoTO)
        {
            var _response = new MessageResponse<List<TopicoTO>>();

            try
            {
                _response.Result = new List<TopicoTO>();

                var _query = (from p in m_EntityContext.topicoes
                              where topicoTO.Descricao == null || p.descricao.Contains(topicoTO.Descricao)
                              where topicoTO.IdTopicoPai == 0 || p.idTopicoPai == topicoTO.IdTopicoPai
                              orderby p.idTopicoPai, p.idTopico
                              select p);

                foreach (var _t in _query)
                {
                    var _conf = new TopicoTO
                    {
                        IdTopico = _t.idTopico,
                        Descricao = _t.descricao,
                        IdTopicoPai = _t.idTopicoPai
                    };
                    
                    _response.Result.Add(_conf);
                }
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<bool> DeleteTopico(TopicoTO topicoTO)
        {
            var _response = new MessageResponse<bool>();

            try
            {
                #region Validations
                if (topicoTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Topico não pode ser nulo.", ResponseKind.Error));
                    return _response;
                }

                if (topicoTO.IdTopico == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(topicoTO.IdTopico)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }
                #endregion

                var _topicoToDelete = m_EntityContext.topicoes.FirstOrDefault(_w => _w.idTopico == topicoTO.IdTopico);

                if (_topicoToDelete == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Topico não encontrado.", ResponseKind.Error));
                    return _response;
                }

                if (_topicoToDelete.topico1.Count() > 0)
                {
                    _response.Exceptions.Add(new MessageResponseException("Não é possível deletar a tópico, pois há tópicos filhos.", ResponseKind.Error));
                    return _response;
                }

                m_EntityContext.topicoes.Remove(_topicoToDelete);

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
