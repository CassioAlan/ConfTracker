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
    public class TipoDataBT
    {
        #region Members

        private ConfSpiderEntities m_EntityContext = null;

        #endregion

        #region Constructors

        public TipoDataBT()
        {
            m_EntityContext = new ConfSpiderEntities();
        }

        #endregion

        #region Methods

        public MessageResponse<int> SaveTipoData(TipoDataTO tipoDataTO)
        {
            var _response = new MessageResponse<int>();

            try
            {
                #region Validations

                if (tipoDataTO == null)
                {
                    _response.Exceptions.Add(new MessageResponseException("TipoData não pode ser nula.", ResponseKind.Error));
                    return _response;
                }

                if (tipoDataTO.IdTipoData == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(tipoDataTO.IdTipoData)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (tipoDataTO.Descricao == null || tipoDataTO.Descricao == string.Empty)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(tipoDataTO.Descricao)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                if (tipoDataTO.IdTipoDataPai == 0)
                {
                    _response.Exceptions.Add(new MessageResponseException($"{nameof(tipoDataTO.IdTipoDataPai)} é obrigatório.", ResponseKind.Error));
                    return _response;
                }

                #endregion

                var _tipoData = m_EntityContext.tipodatas.FirstOrDefault(c => c.idTipoData == tipoDataTO.IdTipoData);
                if (_tipoData == null)
                {
                    _tipoData = new tipodata
                    {
                        idTipoData = tipoDataTO.IdTipoData,
                    };

                    m_EntityContext.tipodatas.Add(_tipoData);
                }

                _tipoData.descricao = tipoDataTO.Descricao;
                _tipoData.idTipoDataPai = tipoDataTO.IdTipoDataPai;

                m_EntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
            }

            return _response;
        }

        public MessageResponse<List<TipoDataTO>> RetrieveTiposData(TipoDataTO tipoDataTO)
        {
            var _response = new MessageResponse<List<TipoDataTO>>();

            try
            {
                _response.Result = new List<TipoDataTO>();

                var _query = (from p in m_EntityContext.tipodatas
                              where tipoDataTO.Descricao == null || p.descricao.Contains(tipoDataTO.Descricao)
                              where tipoDataTO.IdTipoDataPai == 0 || p.idTipoDataPai == tipoDataTO.IdTipoDataPai
                              orderby p.idTipoDataPai, p.idTipoData
                              select p);

                foreach (var _t in _query)
                {
                    var _conf = new TipoDataTO
                    {
                        IdTipoData = _t.idTipoData,
                        Descricao = _t.descricao,
                        IdTipoDataPai = _t.idTipoDataPai
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

        #endregion
    }
}
