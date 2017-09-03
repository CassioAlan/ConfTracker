using ConfSpider.BTLibrary;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.BLL.DateExtraction
{
    public class MergeExtractions
    {
        private List<ConferenciaTO> m_mainExtraction;
        private List<ConferenciaTO> m_secondaryExtraction;
        private int m_year;

        public event EventHandler<DateExtractorEventArgs> ReceivedMessage;
        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new DateExtractorEventArgs(message));
        }

        public MergeExtractions(int year, EnumTipoExtracao tipoMainExtraction, EnumTipoExtracao tipoSecondaryExtraction)
        {
            var _conferenciaBT = new ConferenciaBT();

            OnReceivedMessage("Consultando conferências.");

            var _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(year, tipoMainExtraction, EnumTipoExtracao.Manual); //tanto faz aqui o último parâmetro

            if (!_resp.HasErrors())
            {
                m_mainExtraction = _resp.Result;
            }

            _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(year, tipoSecondaryExtraction, EnumTipoExtracao.Manual); //tanto faz aqui o último parâmetro

            if (!_resp.HasErrors())
            {
                m_secondaryExtraction = _resp.Result;
            }

            m_year = year;
        }

        public MessageResponse<bool> ExecuteMerge()
        {
            var _response = new MessageResponse<bool>();

            var _dataBT = new DataBT();

            var _listConfIDs = m_mainExtraction.Select(_s => _s.IdConferencia).ToList();

            foreach (var _id in _listConfIDs)
            {
                OnReceivedMessage($"Fazendo merge da conferência {_id}.");

                var _mainConf = m_mainExtraction.FirstOrDefault(_w => _w.IdConferencia == _id);
                var _secConf = m_secondaryExtraction.FirstOrDefault(_w => _w.IdConferencia == _id);

                if (_mainConf != null && _mainConf.Edicoes.FirstOrDefault() != null &&
                    _secConf != null && _secConf.Edicoes.FirstOrDefault() != null)
                {
                    var _mainDates = _mainConf.Edicoes.FirstOrDefault().Datas;
                    var _secDates = _secConf.Edicoes.FirstOrDefault().Datas;

                    #region AbstractSubmission
                    var _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.AbstractSubmission);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion

                    #region PaperSubmission
                    _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.PaperSubmission);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion

                    #region AcceptanceNotification
                    _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.AcceptanceNotification);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion

                    #region CameraReadyDue
                    _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.CameraReadyDue);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion

                    #region ConferenceStart
                    _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.ConferenceStart);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion

                    #region ConferenceEnd
                    _responseData = SaveDate(_dataBT, _id, _mainDates, _secDates, EnumTipoDataPai.ConferenceEnd);

                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    #endregion
                }
            }

            return _response;
        }

        private MessageResponse<int> SaveDate(DataBT _dataBT, int _id, List<DataTO> _mainDates, List<DataTO> _secDates, EnumTipoDataPai tipoDataPai)
        {
            var _responseData = new MessageResponse<int>();

            var _date = _mainDates.Where(_w => _w.IdTipoData == (int)tipoDataPai).FirstOrDefault();
            if (_date == null)
            {
                _date = _secDates.Where(_w => _w.IdTipoData == (int)tipoDataPai).FirstOrDefault();
            }

            if (_date != null)
            {
                _responseData = _dataBT.SaveData(new DataTO
                {
                    Edicao = new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO
                        {
                            IdConferencia = _id
                        },
                        Ano = m_year,
                    },
                    TipoExtracao = EnumTipoExtracao.Texto_Posicional_URLsGoldstandard,
                    IdTipoData = (int)tipoDataPai,
                    Data = _date.Data
                });
            }

            return _responseData;
        }
    }
}
