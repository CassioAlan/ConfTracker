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
    public class EdicaoBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public EdicaoBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<int> SaveEdicao(EdicaoTO edicaoTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (edicaoTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("Edição não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (edicaoTO.Conferencia == null || edicaoTO.Conferencia.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(edicaoTO.Conferencia.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (edicaoTO.Ano == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(edicaoTO.Ano)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (edicaoTO.Qualis == null || edicaoTO.Qualis == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(edicaoTO.Qualis)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (edicaoTO.Query == null || edicaoTO.Query == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(edicaoTO.Query)} é obrigatória.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _edicao = m_EntityContext.edicaos.FirstOrDefault(c => c.idConferencia == edicaoTO.Conferencia.IdConferencia &&
                                                                          c.ano == edicaoTO.Ano);
                if (_edicao == null)
                {
                    _edicao = new edicao
                    {
                        idConferencia = edicaoTO.Conferencia.IdConferencia,
                        ano = edicaoTO.Ano
                    };

                    m_EntityContext.edicaos.Add(_edicao);
                }

                _edicao.qualis = edicaoTO.Qualis;
                _edicao.query = edicaoTO.Query;

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
