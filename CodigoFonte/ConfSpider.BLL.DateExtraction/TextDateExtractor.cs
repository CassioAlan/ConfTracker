using ConfSpider.BTLibrary;
using ConfSpider.INL.HtmlManipulate;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfSpider.BLL.DateExtraction
{
    public class TextDateExtractor
    {
        private string m_saving_webpages_path;
        private EnumTipoExtracao m_tipoExtracao;
        public TextDateExtractor(EnumTipoExtracao tipoExtracao)
        {
            m_saving_webpages_path = ConfigurationManager.AppSettings["saving_webpages_path"];
            m_tipoExtracao = tipoExtracao;
        }
        
        public event EventHandler<DateExtractorEventArgs> ReceivedMessage;
        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new DateExtractorEventArgs(message));
        }


        /// <summary>
        /// Este método realiza a extração básica, encontrando palavras cheves e datas dentro do HTML e associando por proximidade de caracteres
        /// </summary>
        /// <returns></returns>
        public MessageResponse<bool> TextExtraction()
        {
            var _response = new MessageResponse<bool>();

            #region Conferências
            var _conferenciaBT = new ConferenciaBT();

            var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO
            {
                Enabled = true
            });
            //var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO());

            if (_resp.HasErrors())
            {
                return new MessageResponse<bool> { Exceptions = _response.Exceptions };
            }

            var _conferencias = _resp.Result;
            #endregion

            #region Tipos de data
            var _tipoDataBT = new TipoDataBT();

            var _resp2 = _tipoDataBT.RetrieveTiposData(new TOLibrary.TipoDataTO());

            if (_resp2.HasErrors())
            {
                return new MessageResponse<bool> { Exceptions = _response.Exceptions };
            }

            var _tiposData = _resp2.Result;
            var _tiposDataPai = _tiposData.GroupBy(_d => _d.IdTipoDataPai).Select(_s => _s.First().IdTipoDataPai).ToList();

            #endregion

            var _dataBT = new DataBT();

            foreach (var _conferencia in _conferencias)
            {
                OnReceivedMessage($"Extraindo datas - Conferência {_conferencia.IdConferencia}.");

                var _subfolders = System.IO.Directory.GetDirectories(m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0'));

                var _conferenceDates = new Dictionary<int, DateTime>();

                foreach (var _sf in _subfolders)
                {
                    var _htmlFiles = System.IO.Directory.GetFiles(_sf, "*.html", System.IO.SearchOption.AllDirectories);

                    //var _regEx = new Regex("index|important.?date|cfp|call");

                    //var _temp = _htmlFiles.ToList().Where(_w => _regEx.IsMatch(_w.ToLower())).ToList();

                    var _temp = _htmlFiles.ToList();

                    var _selectedHtmlFiles = _temp.Where(_w => _w.ToLower().Contains("index")).ToList();
                    _temp.RemoveAll(_w => _w.ToLower().Contains("index"));
                    _selectedHtmlFiles.AddRange(_temp.Where(_w => _w.ToLower().Contains("important")).ToList());
                    _temp.RemoveAll(_w => _w.ToLower().Contains("important"));
                    _selectedHtmlFiles.AddRange(_temp);
                    //_temp = new List<string>();

                    //TODO: verificar se não é melhor olhar o conteúdo de cada html para ver se contém as palavras-chaves

                    foreach (var _htmlFile in _selectedHtmlFiles)
                    {
                        var _htmlText = System.IO.File.ReadAllText(_htmlFile);

                        // Arquivo sendo analisado precisa conter a sigla da conferência e o ano
                        if (_htmlText.Contains(_conferencia.Edicoes.Last().Ano.ToString()) && _htmlText.ToLower().Contains(_conferencia.Sigla.ToLower()))
                        {
                            ExtractDatesFromText(_tiposData, _tiposDataPai, _conferencia, _conferenceDates, _htmlFile, true);
                        }
                        //var _respDate = _htmlManipulator.HasDates();
                        //var _respKeyw = _htmlManipulator.HasKeywords(_td);
                    }
                }

                foreach (var _data in _conferenceDates)
                {
                    var _responseData = _dataBT.SaveData(new DataTO
                    {
                        Edicao = new EdicaoTO
                        {
                            Conferencia = new ConferenciaTO
                            {
                                IdConferencia = _conferencia.IdConferencia
                            },
                            Ano = _conferencia.Edicoes.Last().Ano,
                        },
                        TipoExtracao = m_tipoExtracao,
                        IdTipoData = _data.Key,
                        Data = _data.Value
                    });
                }
            }

            return _response;
        }

        public static void ExtractDatesFromText(List<TipoDataTO> tiposData, List<int> tiposDataPai, ConferenciaTO conferencia, Dictionary<int, DateTime> conferenceDates, string html, bool isFile)
        {
            var _htmlManipulator = new HtmlManipulator(html, isFile);

            var _sideToLook = HtmlManipulator.EnumSideOfDate.Both;

            // TIPO DATA PAI - valores fixos na base de dados, por isso posso testá-los no código
            // 1 - abstract submission
            // 5 - paper submission
            // 17 - acceptance notification
            // 26 - camera-ready due
            // 33 - conference
            // 37 - conference end
            foreach (var _tipoDataPai in tiposDataPai)
            {
                if (!conferenceDates.ContainsKey(_tipoDataPai))
                {
                    var _td = tiposData.Where(_w => _w.IdTipoDataPai == _tipoDataPai).Select(_s => _s.Descricao).ToList();

                    var _deadlines = new List<int>() { 1, 5, 17, 26 };
                    if (_deadlines.Contains(_tipoDataPai)) //Busca data
                    {
                        var _respDate = _htmlManipulator.FindNearDate(_td, _sideToLook);

                        if (_respDate.Result == null && _sideToLook != HtmlManipulator.EnumSideOfDate.Both)
                        {
                            _sideToLook = HtmlManipulator.EnumSideOfDate.Both;
                            _respDate = _htmlManipulator.FindNearDate(_td, _sideToLook);
                        }

                        if (_respDate.Result != null)
                        {
                            _sideToLook = _respDate.Result.Item2;
                            conferenceDates.Add(_tipoDataPai, _respDate.Result.Item1);
                        }
                    }
                    else //Busca intervalo
                    {
                        if (_tipoDataPai == 33) //o tipo 37 não precisa analisar, pois pego junto no tipo 33
                        {
                            _td.Add(conferencia.Sigla.ToLower());

                            var _respDate = _htmlManipulator.FindNearIntervalDate(_td, _sideToLook);

                            if (_respDate.Result == null && _sideToLook != HtmlManipulator.EnumSideOfDate.Both)
                            {
                                _respDate = _htmlManipulator.FindNearIntervalDate(_td, _sideToLook);
                            }

                            if (_respDate.Result != null)
                            {
                                conferenceDates.Add(_tipoDataPai, _respDate.Result.Item1);
                                conferenceDates.Add(37, _respDate.Result.Item2);
                            }
                        }
                    }
                }
            }
        }
    }
}
