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
    public class DataBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public DataBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<int> SaveData(DataTO dataTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (dataTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("TipoData não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (dataTO.Edicao == null || dataTO.Edicao.Conferencia == null || dataTO.Edicao.Conferencia.IdConferencia == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(dataTO.Edicao.Conferencia.IdConferencia)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (dataTO.Edicao.Ano == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(dataTO.Edicao.Ano)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (dataTO.IdTipoData == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(dataTO.IdTipoData)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (dataTO.Data == null || dataTO.Data == DateTime.MinValue)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(dataTO.Data)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (dataTO.TipoExtracao == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(dataTO.TipoExtracao)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _data = m_EntityContext.data.FirstOrDefault(c => c.idConferencia == dataTO.Edicao.Conferencia.IdConferencia
                                                                  && c.ano == dataTO.Edicao.Ano
                                                                  && c.idTipoData == dataTO.IdTipoData
                                                                  && c.idTipoExtracao == (int)dataTO.TipoExtracao);
                if (_data == null)
                {
                    _data = new datum
                    {
                        idConferencia = dataTO.Edicao.Conferencia.IdConferencia,
                        ano = dataTO.Edicao.Ano,
                        idTipoData = dataTO.IdTipoData,
                        idTipoExtracao = (int)dataTO.TipoExtracao
                    };

                    m_EntityContext.data.Add(_data);
                }

                _data.data = dataTO.Data;

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
