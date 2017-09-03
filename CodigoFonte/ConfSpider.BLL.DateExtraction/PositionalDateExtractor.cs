using ConfSpider.BTLibrary;
using ConfSpider.INL.PhantomJSController;
using ConfSpider.INL.HtmlManipulate;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using Masters.LogHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.BLL.DateExtraction
{
    public class PositionalDateExtractor
    {
        private const int MINACCEPTABLERELEVANCE = 50;
        private const int MAXACCEPTABLEDISTANCE = 350;

        #region Members
        private System.Diagnostics.Stopwatch m_watch;
        private StringBuilder m_log_watch;

        private string m_saving_webpages_path;
        private EnumTipoExtracao m_tipoExtracaoData;
        private EnumTipoExtracao m_tipoExtracaoUrl;

        private List<ConferenciaTO> m_conferencias;
        private List<TipoDataTO> m_tiposData;
        private List<int> m_tiposDataPai;

        private string m_abstractPattern;
        private string m_paperPattern;
        private string m_notificationPattern;
        private string m_finalPattern;
        private string m_conferencePattern; 
        #endregion

        public PositionalDateExtractor(EnumTipoExtracao tipoExtracaoData, EnumTipoExtracao tipoExtracaoUrl, bool measureTime)
        {
            m_saving_webpages_path = ConfigurationManager.AppSettings["saving_webpages_path"];
            m_tipoExtracaoData = tipoExtracaoData;
            m_tipoExtracaoUrl = tipoExtracaoUrl;

            #region Stopwatch
            m_watch = System.Diagnostics.Stopwatch.StartNew();
            m_log_watch = new StringBuilder();
            m_log_watch.AppendLine($"\n{m_watch.Elapsed}: Instanciando PositionalDateExtractor com log de tempo");
            #endregion

            #region Conferências
            var _conferenciaBT = new ConferenciaBT();

            var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO
            {
                Enabled = true
            });
            //var _resp = _conferenciaBT.RetrieveConferencias(new TOLibrary.ConferenciaTO());

            if (_resp.HasErrors())
            {
                throw new Exception(_resp.Exceptions.FirstOrDefault().Description);
            }

            m_conferencias = _resp.Result;
            #endregion

            #region Tipos de data
            var _tipoDataBT = new TipoDataBT();

            var _resp2 = _tipoDataBT.RetrieveTiposData(new TOLibrary.TipoDataTO());

            if (_resp2.HasErrors())
            {
                throw new Exception(_resp.Exceptions.FirstOrDefault().Description);
            }

            m_tiposData = _resp2.Result;
            m_tiposDataPai = m_tiposData.GroupBy(_d => _d.IdTipoDataPai).Select(_s => _s.First().IdTipoDataPai).ToList();
            // TIPO DATA PAI - valores fixos na base de dados, por isso posso testá-los no código
            // 1 - abstract submission
            m_abstractPattern = string.Join("|", m_tiposData.Where(_w => _w.IdTipoDataPai == 1).Select(_s => _s.Descricao).ToList<string>());
            // 5 - paper submission
            m_paperPattern = string.Join("|", m_tiposData.Where(_w => _w.IdTipoDataPai == 5).Select(_s => _s.Descricao).ToList<string>());
            // 17 - acceptance notification
            m_notificationPattern = string.Join("|", m_tiposData.Where(_w => _w.IdTipoDataPai == 17).Select(_s => _s.Descricao).ToList<string>());
            // 26 - camera-ready due
            m_finalPattern = string.Join("|", m_tiposData.Where(_w => _w.IdTipoDataPai == 26).Select(_s => _s.Descricao).ToList<string>());
            // 33 - conference
            m_conferencePattern = string.Join("|", m_tiposData.Where(_w => _w.IdTipoDataPai == 33).Select(_s => _s.Descricao).ToList<string>());

            #endregion
        }

        ~PositionalDateExtractor()
        {
            if (m_watch != null && m_watch.IsRunning)
            {
                m_watch.Stop();
            }
        }

        public event EventHandler<DateExtractorEventArgs> ReceivedMessage;

        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new DateExtractorEventArgs(message));
        }

        public MessageResponse<bool> ExtractInformationPosition()
        {
            var _response = new MessageResponse<bool>();

            var _phantomJsControl = new PhantomJSControl();

            foreach (var _conferencia in m_conferencias)
            {
                OnReceivedMessage($"Extraindo posições de datas e rótulos - Conferência {_conferencia.IdConferencia}.");

                var _subfolders = System.IO.Directory.GetDirectories(m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0'));

                var _conferenceDates = new Dictionary<int, DateTime>();

                foreach (var _sf in _subfolders)
                {
                    var _htmlFiles = System.IO.Directory.GetFiles(_sf, "*.html", System.IO.SearchOption.AllDirectories).ToList();
                    //var _htmlFiles = System.IO.Directory.GetFiles(m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0'), "*.html", System.IO.SearchOption.AllDirectories).ToList();

                    foreach (var _htmlFile in _htmlFiles)
                    {
                        OnReceivedMessage($"{DateTime.Now.ToLongTimeString()} - Arquivo: {_htmlFile}.");
                        _phantomJsControl.FindInformationPosition(_htmlFile, m_abstractPattern, m_paperPattern, m_notificationPattern, m_finalPattern, m_conferencePattern + "|" + _conferencia.Sigla);
                        
                    }
                }
            }

            return _response;
        }

        public MessageResponse<bool> ExtractDates()
        {
            var _response = new MessageResponse<bool>();

            var _dataBT = new DataBT();

            foreach (var _conferencia in m_conferencias)
            {
                OnReceivedMessage($"Extraindo datas - Conferência {_conferencia.IdConferencia}.");
                m_log_watch.AppendLine($"{m_watch.Elapsed}: CONFERÊNCIA {_conferencia.IdConferencia}");

                var _subfolders = System.IO.Directory.GetDirectories(m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0'));

                var _conferenceDates = new Dictionary<int, DateTime>();

                List<LabelDateRelationTO> _listMaxAbstractRelevance = new List<LabelDateRelationTO>();
                List<LabelDateRelationTO> _listMaxPaperRelevance = new List<LabelDateRelationTO>();
                List<LabelDateRelationTO> _listMaxNotificationRelevance = new List<LabelDateRelationTO>();
                List<LabelDateRelationTO> _listMaxFinalRelevance = new List<LabelDateRelationTO>();
                List<LabelDateRelationTO> _listMaxConferenceRelevance = new List<LabelDateRelationTO>();

                //Para cada URL retornada...
                if (m_tipoExtracaoUrl == EnumTipoExtracao.Manual)
                {
                    _subfolders = _subfolders.Where(_w => _w.EndsWith("101")).ToArray();
                }
                foreach (var _sf in _subfolders)
                {
                    m_log_watch.AppendLine($"{m_watch.Elapsed}: Entrando em FilesToAnalize");
                    var _respFilesToAnalize = FilesToAnalize(_sf, _conferencia);
                    m_log_watch.AppendLine($"{m_watch.Elapsed}: Finalizou FilesToAnalize");

                    if (_respFilesToAnalize.HasErrors())
                    {
                        _response.Exceptions = _respFilesToAnalize.Exceptions;
                        return _response;
                    }
                    var _selectedHtmlFiles = _respFilesToAnalize.Result;
                    //var _selectedHtmlFiles = System.IO.Directory.GetFiles(m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0'), "*.html");

                    m_log_watch.AppendLine($"{m_watch.Elapsed}: Iniciando analise dos htmls selecionados");
                    foreach (var _htmlFile in _selectedHtmlFiles)
                    {
                        List<DateNodeTO> _datesPosition = new List<DateNodeTO>();

                        var _htmlContent = System.IO.File.ReadAllText(_htmlFile);

                        List<LabelDateRelationTO> _labelDateRelations = new List<LabelDateRelationTO>();

                        // Arquivo sendo analisado precisa conter a sigla da conferência e o ano
                        if (_htmlContent.Contains(_conferencia.Edicoes.Last().Ano.ToString()) && _htmlContent.ToLower().Contains(_conferencia.Sigla.ToLower()))
                        {
                            if (System.IO.File.Exists(_htmlFile + "_datesposition.txt"))
                            {
                                #region Dates
                                var _txtDates = System.IO.File.ReadAllText(_htmlFile + "_datesposition.txt");

                                var _registersDates = _txtDates.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var _register in _registersDates)
                                {
                                    //limitar anos entre o atual e o próximo
                                    var _dateNode = new DateNodeTO(_register);
                                    if (_dateNode.Date1.Year == DateTime.Now.Year || _dateNode.Date1.Year == DateTime.Now.Year + 1 || _dateNode.Date1.Year == DateTime.Now.Year - 1)
                                    {
                                        _datesPosition.Add(_dateNode);
                                    }
                                }
                                #endregion

                                List<LabelNodeTO> _labelsPosition = new List<LabelNodeTO>();

                                #region Abstract
                                if (System.IO.File.Exists(_htmlFile + "_abstractposition.txt"))
                                {
                                    var _txtAbstract = System.IO.File.ReadAllText(_htmlFile + "_abstractposition.txt");

                                    var _registersAbstract = _txtAbstract.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var _register in _registersAbstract)
                                    {
                                        //gambi para desconsiderar a keyword "proposals" (foi removida da base)
                                        if (!_register.ToLower().Contains("proposal"))
                                        {
                                            var _labelNode = new LabelNodeTO(_register, EnumTipoDataPai.AbstractSubmission);

                                            if (_labelNode.Text.Length > 200)
                                            {
                                                ExtractRelationsFromSingleNode(_conferencia, _labelDateRelations, _labelNode);
                                                continue;
                                            }

                                            _labelsPosition.Add(_labelNode);
                                        }
                                    }
                                }
                                #endregion

                                #region Paper
                                if (System.IO.File.Exists(_htmlFile + "_paperposition.txt"))
                                {
                                    var _txtPaper = System.IO.File.ReadAllText(_htmlFile + "_paperposition.txt");

                                    var _registersPaper = _txtPaper.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var _register in _registersPaper)
                                    {
                                        var _labelNode = new LabelNodeTO(_register, EnumTipoDataPai.PaperSubmission);

                                        if (_labelNode.Text.Length > 200)
                                        {
                                            ExtractRelationsFromSingleNode(_conferencia, _labelDateRelations, _labelNode);
                                            continue;
                                        }

                                        _labelsPosition.Add(_labelNode);
                                    }
                                }
                                #endregion

                                #region Notification
                                if (System.IO.File.Exists(_htmlFile + "_notificationposition.txt"))
                                {
                                    var _txtNotification = System.IO.File.ReadAllText(_htmlFile + "_notificationposition.txt");

                                    var _registersNotification = _txtNotification.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var _register in _registersNotification)
                                    {
                                        var _labelNode = new LabelNodeTO(_register, EnumTipoDataPai.AcceptanceNotification);

                                        if (_labelNode.Text.Length > 200)
                                        {
                                            ExtractRelationsFromSingleNode(_conferencia, _labelDateRelations, _labelNode);
                                            continue;
                                        }

                                        _labelsPosition.Add(_labelNode);
                                    }
                                }
                                #endregion

                                #region Final
                                if (System.IO.File.Exists(_htmlFile + "_finalposition.txt"))
                                {
                                    var _txtFinal = System.IO.File.ReadAllText(_htmlFile + "_finalposition.txt");

                                    var _registersFinal = _txtFinal.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var _register in _registersFinal)
                                    {
                                        var _labelNode = new LabelNodeTO(_register, EnumTipoDataPai.CameraReadyDue);

                                        if (_labelNode.Text.Length > 200)
                                        {
                                            ExtractRelationsFromSingleNode(_conferencia, _labelDateRelations, _labelNode);
                                            continue;
                                        }

                                        _labelsPosition.Add(_labelNode);
                                    }
                                }
                                #endregion

                                #region Conference
                                if (System.IO.File.Exists(_htmlFile + "_conferenceposition.txt"))
                                {
                                    var _txtConference = System.IO.File.ReadAllText(_htmlFile + "_conferenceposition.txt");

                                    var _registersConference = _txtConference.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var _register in _registersConference)
                                    {
                                        _labelsPosition.Add(new LabelNodeTO(_register, EnumTipoDataPai.ConferenceStart));
                                    }
                                }
                                #endregion

                                #region PARES LABEL X DATA
                                foreach (var _label in _labelsPosition)
                                {
                                    foreach (var _date in _datesPosition)
                                    {
                                        if (_date.DateType != 0)
                                        {
                                            var _relation = new LabelDateRelationTO(_label, _date);
                                            if (_relation.Position != EnumPositionBlockWeight.B9)
                                            {
                                                //impondo uma distância máxima a ser aceita:
                                                if (_relation.MinDistance <= MAXACCEPTABLEDISTANCE)
                                                {
                                                    _labelDateRelations.Add(_relation);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                //Considerar inclusive os nodos de datas que contenham mais de uma data,
                                // pois pode ocorrer de uma estar riscada e a outra ser a correta
                                //o costume é a riscada estar antes, então pegar a segunda
                                if (_labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AbstractSubmission).Any())
                                {
                                    var _maxRelevance1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AbstractSubmission).Max(_m => _m.Relevance);
                                    var _maxAtual = _listMaxAbstractRelevance.Count() == 0 ? 0 : _listMaxAbstractRelevance.Max(_m => _m.Relevance);

                                    if (_maxRelevance1 >= MINACCEPTABLERELEVANCE)
                                    {
                                        var _auxNode1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AbstractSubmission
                                                                                          && _w.Relevance == _maxRelevance1).ToList();

                                        if (_maxRelevance1 > _maxAtual)
                                        {
                                            _listMaxAbstractRelevance = _auxNode1;
                                            Logger.WriteInfo($"\nPares SELECIONADOS para AbstractSubmission:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                        if (_maxRelevance1 == _maxAtual)
                                        {
                                            _listMaxAbstractRelevance.AddRange(_auxNode1);
                                            Logger.WriteInfo($"\nPares ADICIONADOS para AbstractSubmission:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                    }
                                }

                                if (_labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.PaperSubmission).Any())
                                {
                                    var _maxRelevance1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.PaperSubmission).Max(_m => _m.Relevance);
                                    var _maxAtual = _listMaxPaperRelevance.Count() == 0 ? 0 : _listMaxPaperRelevance.Max(_m => _m.Relevance);

                                    if (_maxRelevance1 >= MINACCEPTABLERELEVANCE)
                                    {
                                        var _auxNode1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.PaperSubmission
                                                                                          && _w.Relevance == _maxRelevance1).ToList();

                                        if (_maxRelevance1 > _maxAtual)
                                        {
                                            _listMaxPaperRelevance = _auxNode1;
                                            Logger.WriteInfo($"\nPares SELECIONADOS para PaperSubmission:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                        if (_maxRelevance1 == _maxAtual)
                                        {
                                            _listMaxPaperRelevance.AddRange(_auxNode1);
                                            Logger.WriteInfo($"\nPares ADICIONADOS para PaperSubmission:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                    }
                                }

                                if (_labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AcceptanceNotification).Any())
                                {
                                    var _maxRelevance1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AcceptanceNotification).Max(_m => _m.Relevance);
                                    var _maxAtual = _listMaxNotificationRelevance.Count() == 0 ? 0 : _listMaxNotificationRelevance.Max(_m => _m.Relevance);

                                    if (_maxRelevance1 >= MINACCEPTABLERELEVANCE)
                                    {
                                        var _auxNode1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.AcceptanceNotification
                                                                                          && _w.Relevance == _maxRelevance1).ToList();

                                        if (_maxRelevance1 > _maxAtual)
                                        {
                                            _listMaxNotificationRelevance = _auxNode1;
                                            Logger.WriteInfo($"\nPares SELECIONADOS para AcceptanceNotification:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                        if (_maxRelevance1 == _maxAtual)
                                        {
                                            _listMaxNotificationRelevance.AddRange(_auxNode1);
                                            Logger.WriteInfo($"\nPares ADICIONADOS para AcceptanceNotification:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                    }
                                }

                                if (_labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.CameraReadyDue).Any())
                                {
                                    var _maxRelevance1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.CameraReadyDue).Max(_m => _m.Relevance);
                                    var _maxAtual = _listMaxFinalRelevance.Count() == 0 ? 0 : _listMaxFinalRelevance.Max(_m => _m.Relevance);

                                    if (_maxRelevance1 >= MINACCEPTABLERELEVANCE)
                                    {
                                        var _auxNode1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.CameraReadyDue
                                                                                          && _w.Relevance == _maxRelevance1).ToList();

                                        if (_maxRelevance1 > _maxAtual)
                                        {
                                            _listMaxFinalRelevance = _auxNode1;
                                            Logger.WriteInfo($"\nPares SELECIONADOS para CameraReady:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                        if (_maxRelevance1 == _maxAtual)
                                        {
                                            _listMaxFinalRelevance.AddRange(_auxNode1);
                                            Logger.WriteInfo($"\nPares ADICIONADOS para CameraReady:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                    }
                                }

                                if (_labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.ConferenceStart &&
                                                                    _w.DateNode.DateType == EnumDateType.IntervalDate).Any())
                                {
                                    var _maxRelevance1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.ConferenceStart &&
                                                                        _w.DateNode.DateType == EnumDateType.IntervalDate).Max(_m => _m.Relevance);
                                    var _maxAtual = _listMaxConferenceRelevance.Count() == 0 ? 0 : _listMaxConferenceRelevance.Max(_m => _m.Relevance);

                                    if (_maxRelevance1 >= MINACCEPTABLERELEVANCE)
                                    {
                                        var _auxNode1 = _labelDateRelations.Where(_w => _w.LabelNode.TipoDataPai == EnumTipoDataPai.ConferenceStart
                                                                                          && _w.Relevance == _maxRelevance1).ToList();

                                        if (_maxRelevance1 > _maxAtual)
                                        {
                                            _listMaxConferenceRelevance = _auxNode1;
                                            Logger.WriteInfo($"\nPares SELECIONADOS para ConferenceStart e ConferenceEnd:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                        if (_maxRelevance1 == _maxAtual)
                                        {
                                            _listMaxConferenceRelevance.AddRange(_auxNode1);
                                            Logger.WriteInfo($"\nPares ADICIONADOS para ConferenceStart e ConferenceEnd:\n\t-Arquivo: {_htmlFile}\n\t-Relevancia: {_maxRelevance1}");
                                            //foreach (var _node in _auxNode1)
                                            //{
                                            //    Logger.WriteInfo($"\n\t\t\t-Label texto: {_node.LabelNode.Text}\n\t\t\t-Data texto: {_node.DateNode.Text}\n\t\t\t-Distancia: {_node.MinDistance}\n\t\t\t-Posicao: {_node.Position}\n\t\t\t-Relevancia: {_node.Relevance}");
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                    m_log_watch.AppendLine($"{m_watch.Elapsed}: Finalizado analise dos htmls selecionados");

                }

                //TODO: DESCOMENTAR ESTA PARTE!!!
                /**/
                #region Save Conference Dates
                //Considerar inclusive os nodos de datas que contenham mais de uma data,
                // pois pode ocorrer de uma estar riscada e a outra ser a correta
                //o costume é a riscada estar antes, então pegar a segunda
                // -> FAIL, MUDANDO: PEGAR A MAIOR DELAS, POIS NUNCA SE ADIANTA UM DEADLINE, SEMPRE POSTERGA
                if (_listMaxAbstractRelevance.Count() > 0)
                {
                    //var _selected = _listMaxAbstractRelevance.Where(_w => _w.LabelNode.Text.ToLower().Contains("research")).FirstOrDefault();

                    //if (_selected == null)
                    //{
                    //    _selected = _listMaxAbstractRelevance.First();
                    //}

                    var _selected = _listMaxAbstractRelevance.First();
                    Logger.WriteInfo($"\nNodo selecionado para ABSTRACT:\n\t-Label texto: {_selected.LabelNode.Text}\n\t-Data texto: {_selected.DateNode.Text}\n\t-Distancia: {_selected.MinDistance}\n\t-Posicao: {_selected.Position}\n\t-Relevancia: {_selected.Relevance}");

                    //var _date = _selected.DateNode.DateType == EnumDateType.SingleDate ? _selected.DateNode.Date1 : _selected.DateNode.Date2;

                    DateTime _date;
                    if (_selected.DateNode.DateType == EnumDateType.SingleDate)
                    {
                        _date = _selected.DateNode.Date1;
                    }
                    else
                    {
                        _date = new List<DateTime> { _selected.DateNode.Date1, _selected.DateNode.Date2 }.Max();
                    }

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
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.AbstractSubmission,
                        Data = _date
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                }

                if (_listMaxPaperRelevance.Count() > 0)
                {
                    //var _selected = _listMaxPaperRelevance.Where(_w => _w.LabelNode.Text.ToLower().Contains("research")).FirstOrDefault();

                    //if (_selected == null)
                    //{
                    //    _selected = _listMaxPaperRelevance.First();
                    //}

                    var _selected = _listMaxPaperRelevance.First();
                    Logger.WriteInfo($"\nNodo selecionado para PAPER:\n\t-Label texto: {_selected.LabelNode.Text}\n\t-Data texto: {_selected.DateNode.Text}\n\t-Distancia: {_selected.MinDistance}\n\t-Posicao: {_selected.Position}\n\t-Relevancia: {_selected.Relevance}");

                    //var _date = _selected.DateNode.DateType == EnumDateType.SingleDate ? _selected.DateNode.Date1 : _selected.DateNode.Date2;

                    DateTime _date;
                    if (_selected.DateNode.DateType == EnumDateType.SingleDate)
                    {
                        _date = _selected.DateNode.Date1;
                    }
                    else
                    {
                        _date = new List<DateTime> { _selected.DateNode.Date1, _selected.DateNode.Date2 }.Max();
                    }

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
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.PaperSubmission,
                        Data = _date
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                }

                if (_listMaxNotificationRelevance.Count() > 0)
                {
                    //var _selected = _listMaxNotificationRelevance.Where(_w => _w.LabelNode.Text.ToLower().Contains("research")).FirstOrDefault();

                    //if (_selected == null)
                    //{
                    //    _selected = _listMaxNotificationRelevance.First();
                    //}

                    var _selected = _listMaxNotificationRelevance.First();
                    Logger.WriteInfo($"\nNodo selecionado para NOTIFICATION:\n\t-Label texto: {_selected.LabelNode.Text}\n\t-Data texto: {_selected.DateNode.Text}\n\t-Distancia: {_selected.MinDistance}\n\t-Posicao: {_selected.Position}\n\t-Relevancia: {_selected.Relevance}");

                    //var _date = _selected.DateNode.DateType == EnumDateType.SingleDate ? _selected.DateNode.Date1 : _selected.DateNode.Date2;

                    DateTime _date;
                    if (_selected.DateNode.DateType == EnumDateType.SingleDate)
                    {
                        _date = _selected.DateNode.Date1;
                    }
                    else
                    {
                        _date = new List<DateTime> { _selected.DateNode.Date1, _selected.DateNode.Date2 }.Max();
                    }

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
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.AcceptanceNotification,
                        Data = _date
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                }

                if (_listMaxFinalRelevance.Count() > 0)
                {
                    //var _selected = _listMaxFinalRelevance.Where(_w => _w.LabelNode.Text.ToLower().Contains("research")).FirstOrDefault();

                    //if (_selected == null)
                    //{
                    //    _selected = _listMaxFinalRelevance.First();
                    //}

                    var _selected = _listMaxFinalRelevance.First();
                    Logger.WriteInfo($"\nNodo selecionado para CAMERA-READY:\n\t-Label texto: {_selected.LabelNode.Text}\n\t-Data texto: {_selected.DateNode.Text}\n\t-Distancia: {_selected.MinDistance}\n\t-Posicao: {_selected.Position}\n\t-Relevancia: {_selected.Relevance}");

                    //var _date = _selected.DateNode.DateType == EnumDateType.SingleDate ? _selected.DateNode.Date1 : _selected.DateNode.Date2;

                    DateTime _date;
                    if (_selected.DateNode.DateType == EnumDateType.SingleDate)
                    {
                        _date = _selected.DateNode.Date1;
                    }
                    else
                    {
                        _date = new List<DateTime> { _selected.DateNode.Date1, _selected.DateNode.Date2 }.Max();
                    }

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
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.CameraReadyDue,
                        Data = _date
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                }

                if (_listMaxConferenceRelevance.Count() > 0)
                {
                    var _selected = _listMaxConferenceRelevance.First();
                    Logger.WriteInfo($"\nNodo selecionado para CONFERENCE START/END:\n\t-Label texto: {_selected.LabelNode.Text}\n\t-Data texto: {_selected.DateNode.Text}\n\t-Distancia: {_selected.MinDistance}\n\t-Posicao: {_selected.Position}\n\t-Relevancia: {_selected.Relevance}");

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
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.ConferenceStart,
                        Data = _selected.DateNode.Date1
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                    var _responseData2 = _dataBT.SaveData(new DataTO
                    {
                        Edicao = new EdicaoTO
                        {
                            Conferencia = new ConferenciaTO
                            {
                                IdConferencia = _conferencia.IdConferencia
                            },
                            Ano = _conferencia.Edicoes.Last().Ano,
                        },
                        TipoExtracao = m_tipoExtracaoData,
                        IdTipoData = (int)EnumTipoDataPai.ConferenceEnd,
                        Data = _selected.DateNode.Date2
                    });
                    if (_responseData.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao salvar data: {_responseData.Exceptions.FirstOrDefault().Description}");
                    }
                }
                #endregion
                /**/

                Logger.WriteDebug(m_log_watch.ToString());
                m_log_watch.Clear();
                m_log_watch.AppendLine("\n");
                m_watch.Restart();
            }
            
            return _response;
        }

        private MessageResponse<List<string>> FilesToAnalize(string _sf, ConferenciaTO _conferencia)
        {
            var _response = new MessageResponse<List<string>>();

            var _ordem = Convert.ToInt32(_sf.Split('\\').Last());
            var _urlTO = _conferencia.Edicoes.Last().Urls.Where(_w => _w.Ordem == _ordem).First();

            string _aux = string.Empty;

            if (_urlTO.Url.StartsWith("https"))
            {
                _aux = _urlTO.Url.Replace("https://", "");
            }
            else if (_urlTO.Url.StartsWith("http"))
            {
                _aux = _urlTO.Url.Replace("http://", "");
            }

            var _localStartFileAPROXIMADO = m_saving_webpages_path + _conferencia.IdConferencia.ToString().PadLeft(4, '0') + "\\" + _ordem + "\\" + _aux.Replace('/', '\\');

            if (_localStartFileAPROXIMADO.EndsWith("\\"))
            {
                _localStartFileAPROXIMADO += "index.html";
            }

            var _allHtmlFiles = System.IO.Directory.GetFiles(_sf, "*.html", System.IO.SearchOption.AllDirectories).ToList();
            if (!_allHtmlFiles.Any())
            {
                return _response;
            }
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Número total de htmls da conferência: {_allHtmlFiles.Count}");
            
            var _minDistance = _allHtmlFiles.Min(_s => LevenshteinDistance(_s, _localStartFileAPROXIMADO));

            var _tempHtmlFiles = _allHtmlFiles.Where(_w => LevenshteinDistance(_w, _localStartFileAPROXIMADO) == _minDistance).ToList();
            //_allHtmlFiles.RemoveAll(_w => LevenshteinDistance(_w, _localStartFileAPROXIMADO) == _minDistance);


            #region Nova loucura: percorrendo os hrefs

            var _listSelectedHtmlFiles = new List<Tuple<int, string>>();
            foreach (var _html in _tempHtmlFiles)
            {
                _listSelectedHtmlFiles.Add(new Tuple<int, string>(1, _html));
            }

            #region LVL 1
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Iniciando seleção de arquivos do lvl 1");
            OrderFiles(_listSelectedHtmlFiles, _allHtmlFiles, 1);
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Finalizando seleção de arquivos do lvl 1");
            #endregion

            #region LVL 2
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Iniciando seleção de arquivos do lvl 2");
            OrderFiles(_listSelectedHtmlFiles, _allHtmlFiles, 2);
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Finalizando seleção de arquivos do lvl 2");
            #endregion

            #region LVL 3
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Iniciando seleção de arquivos do lvl 3");
            OrderFiles(_listSelectedHtmlFiles, _allHtmlFiles, 3);
            m_log_watch.AppendLine($"{m_watch.Elapsed}: Finalizando seleção de arquivos do lvl 3");
            #endregion
            #endregion

            _response.Result = _listSelectedHtmlFiles.Select(_s => _s.Item2).ToList();

            return _response;
        }
        
        private void OrderFiles(List<Tuple<int, string>> _listSelectedHtmlFiles, List<string> _allHtmlFiles, int level)
        {
            try
            {
                var _listSelectedHtmlFilesLVLx = _listSelectedHtmlFiles.Where(_w => _w.Item1 == level).Select(_s => _s.Item2).ToList();
                m_log_watch.AppendLine($"{m_watch.Elapsed}: Número de arquivos selecionados no lvl {level}: {_listSelectedHtmlFilesLVLx.Count}");

                foreach (var _html in _listSelectedHtmlFilesLVLx)
                {
                    var _htmlManip = new HtmlManipulator(_html, true);
                    var _respGetHrefs = _htmlManip.GetHREFs();
                    if (_respGetHrefs.HasErrors())
                    {
                        OnReceivedMessage($"Erro ao extrair hrefs: {_respGetHrefs.Exceptions.FirstOrDefault().Description}");
                        //_response.Exceptions = _respGetHrefs.Exceptions;
                        //return _response;
                    }

                    var _listHrefs = new List<string>();
                    if (_respGetHrefs.Result != null)
                    {
                        _listHrefs = _respGetHrefs.Result.Where(_w => !_w.Contains("http")).Distinct().ToList();

                        var _respGetDateUrls = _htmlManip.GetDateURLs(_listHrefs);
                        if (!_respGetDateUrls.HasErrors())
                        {
                            _listHrefs = _respGetDateUrls.Result;
                        }
                    }

                    //tentar abrir um arquivo no diretério a cima...
                    var _path = System.IO.Path.GetDirectoryName(_html);

                    m_log_watch.AppendLine($"{m_watch.Elapsed}: Número de hrefs selecionados em um html no lvl {level}: {_listHrefs.Count}");
                    foreach (var _href in _listHrefs)
                    {
                        var _file = _href;

                        if (_file.EndsWith("\\"))
                        {
                            _file += "index.html";
                        }

                        m_log_watch.AppendLine($"{m_watch.Elapsed}: calculando MinDistance com LevenshteinDistance [inicio]");
                        var _minDistance = _allHtmlFiles.Min(_s => LevenshteinDistance(_s, _path + _file));
                        m_log_watch.AppendLine($"{m_watch.Elapsed}: calculando MinDistance com LevenshteinDistance [fim]");

                        m_log_watch.AppendLine($"{m_watch.Elapsed}: selecionando arquivos com LevenshteinDistance [inicio]");
                        var _tempHtmlFiles = _allHtmlFiles.Where(_w => LevenshteinDistance(_w, _path + _file) == _minDistance).ToList();
                        m_log_watch.AppendLine($"{m_watch.Elapsed}: selecionando arquivos com LevenshteinDistance [fim]");

                        foreach (var _temp in _tempHtmlFiles)
                        {
                            if (!_listSelectedHtmlFiles.Any(_w => _w.Item2 == _temp))
                            {
                                _listSelectedHtmlFiles.Add(new Tuple<int, string>(level + 1, _temp));
                            }
                            //_allHtmlFiles.RemoveAll(_w => LevenshteinDistance(_w, _file) == _minDistance);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnReceivedMessage($"Erro em OrderFiles: {ex.Message}");
            }
        }

        private void ExtractRelationsFromSingleNode(ConferenciaTO _conferencia, List<LabelDateRelationTO> _labelDateRelations, LabelNodeTO _labelNode)
        {
            var _confDates = new Dictionary<int, DateTime>();
            TextDateExtractor.ExtractDatesFromText(m_tiposData, m_tiposDataPai, _conferencia, _confDates, _labelNode.Text, false);

            foreach (var _date in _confDates)
            {
                //TODO: DEBUGAR ESSA BAITA MERDA AQUI
                var _fakeLabelNode = new LabelNodeTO("FAKE LABEL#|||#0,0#|||#0,0#|||#0,0#|||#0,0#|||#0,0", (EnumTipoDataPai)_date.Key);
                //TODO: DEBUGAR ESSA BAITA MERDA AQUI
                var _fakeDateNode = new DateNodeTO(_date.Value.ToShortDateString() + "#|||#0,0#|||#0,0#|||#0,0#|||#0,0#|||#0,0");
                var _relation = new LabelDateRelationTO(_fakeLabelNode, _fakeDateNode);
                _labelDateRelations.Add(_relation);
            }
        }

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
