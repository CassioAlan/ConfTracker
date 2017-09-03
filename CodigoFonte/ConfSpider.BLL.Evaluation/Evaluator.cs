using ConfSpider.BTLibrary;
using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.BLL.Evaluation
{
    public class Evaluator
    {
        private List<ConferenciaTO> m_goldStandard;
        private List<ConferenciaTO> m_retrieved;

        private List<int> m_confsCorrectURL;

        public EnumTipoExtracao DateExtractionType { get; set; }
        public EnumTipoExtracao UrlExtractionType { get; set; }

        public Evaluator(int year, EnumTipoExtracao extractionToEvaluateDate, EnumTipoExtracao extractionToEvaluateUrl)
        {
            this.DateExtractionType = extractionToEvaluateDate;
            this.UrlExtractionType = extractionToEvaluateUrl;

            var _conferenciaBT = new ConferenciaBT();

            var _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(year, EnumTipoExtracao.Manual, EnumTipoExtracao.Manual);

            if (!_resp.HasErrors())
            {
                m_goldStandard = _resp.Result;
            }

            _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(year, extractionToEvaluateDate, extractionToEvaluateUrl);

            if (!_resp.HasErrors())
            {
                m_retrieved = _resp.Result;
            }

            m_confsCorrectURL = new List<int>();
        }

        #region URL
        public MessageResponse<UrlEvaluationResults> EvaluateUrl()
        {
            var _response = new MessageResponse<UrlEvaluationResults>();

            var _listConfIDs = m_goldStandard.Select(_s => _s.IdConferencia).ToList();

            var _avaliation = new UrlEvaluationResults();

            _avaliation.TotalGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Urls)).Count(_w => _w.TipoExtracao == EnumTipoExtracao.Manual);
            _avaliation.TotalRetrieved = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Urls)).Count(_w => _w.TipoExtracao == this.UrlExtractionType);

            foreach (var _id in _listConfIDs)
            {
                var _goldConf = m_goldStandard.FirstOrDefault(_w => _w.IdConferencia == _id);
                var _retrConf = m_retrieved.FirstOrDefault(_w => _w.IdConferencia == _id);

                if (_retrConf != null && _retrConf.Edicoes.FirstOrDefault() != null && _goldConf.Edicoes.FirstOrDefault() != null)
                {
                    var _goldUrl = _goldConf.Edicoes.FirstOrDefault().Urls;
                    var _retrUrls = _retrConf.Edicoes.FirstOrDefault().Urls;

                    var _poss = _retrUrls.Where(_w => _goldUrl.Count > 0
                                               && (_w.Url.Substring(_w.Url.LastIndexOf(@"//") + 2).StartsWith(_goldUrl.First().Url.Substring(_goldUrl.First().Url.LastIndexOf(@"//")+2))
                                                   || _goldUrl.First().Url.Substring(_goldUrl.First().Url.LastIndexOf(@"//") + 2).StartsWith(_w.Url.Substring(_w.Url.LastIndexOf(@"//")+2)))
                                                  ).Select(_s => _s.Ordem).ToList();

                    foreach (var _pos in _poss)
                    {
                        switch (_pos)
                        {
                            case 1:
                            case 101:
                                _avaliation.CountFirst++;
                                //m_confsCorrectURL.Add(_id);
                                break;
                            case 2:
                                _avaliation.CountSecond++;
                                //m_confsCorrectURL.Add(_id);
                                break;
                            case 3:
                                _avaliation.CountThird++;
                                //m_confsCorrectURL.Add(_id);
                                break;
                            default:
                                _avaliation.CountOther++;
                                break;
                        }
                    }
                    
                    if (_poss.Any(_w => _w > 0))
                    {
                        m_confsCorrectURL.Add(_id);
                    }
                    else
                    {
                        _avaliation.CountWrong++;
                    }
                }
            }

            _response.Result = _avaliation;

            var _temp = _listConfIDs.RemoveAll(_w => m_confsCorrectURL.Contains(_w));

            return _response;
        }

        public class UrlEvaluationResults
        {
            public int TotalGold { get; set; } = 0;
            public int TotalRetrieved { get; set; } = 0;
            public int CountFirst { get; set; } = 0;
            public int CountSecond { get; set; } = 0;
            public int CountThird { get; set; } = 0;
            public int CountOther { get; set; } = 0;
            public int CountWrong { get; set; } = 0;
        }
        #endregion

        #region DATE
        public MessageResponse<List<DateEvaluationResults>> EvaluateDate()
        {
            var _response = new MessageResponse<List<DateEvaluationResults>>();

            var _listConfIDs = m_goldStandard.Select(_s => _s.IdConferencia).ToList();
            List<DateEvaluationResults> _avaliations = GetAvaliations();

            foreach (var _id in _listConfIDs)
            {
                var _goldConf = m_goldStandard.FirstOrDefault(_w => _w.IdConferencia == _id);
                var _retrConf = m_retrieved.FirstOrDefault(_w => _w.IdConferencia == _id);

                if (_retrConf != null && _retrConf.Edicoes.FirstOrDefault() != null && _goldConf.Edicoes.FirstOrDefault() != null)
                {
                    var _goldDates = _goldConf.Edicoes.FirstOrDefault().Datas;
                    var _retrDates = _retrConf.Edicoes.FirstOrDefault().Datas;

                    if (Hit(EnumTipoDataPai.AbstractSubmission, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.AbstractSubmission).CountCorrect++;
                    }

                    if (Hit(EnumTipoDataPai.PaperSubmission, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.PaperSubmission).CountCorrect++;
                    }

                    if (Hit(EnumTipoDataPai.AcceptanceNotification, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.AcceptanceNotification).CountCorrect++;
                    }

                    if (Hit(EnumTipoDataPai.CameraReadyDue, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.CameraReadyDue).CountCorrect++;
                    }

                    if (Hit(EnumTipoDataPai.ConferenceStart, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.ConferenceStart).CountCorrect++;
                    }

                    if (Hit(EnumTipoDataPai.ConferenceEnd, _goldDates, _retrDates))
                    {
                        _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.ConferenceEnd).CountCorrect++;
                    }
                }
            }

            _response.Result = _avaliations;

            return _response;
        }

        public MessageResponse<List<Tuple<int, double>>>TTestAux()
        {
            var _response = new MessageResponse<List<Tuple<int, double>>>();
            var _fmeasures = new List<Tuple<int, double>>();

            var _listConfIDs = m_goldStandard.Select(_s => _s.IdConferencia).ToList();

            foreach (var _id in _listConfIDs)
            {
                var _goldConf = m_goldStandard.FirstOrDefault(_w => _w.IdConferencia == _id);
                var _retrConf = m_retrieved.FirstOrDefault(_w => _w.IdConferencia == _id);

                if (_retrConf != null && _retrConf.Edicoes.FirstOrDefault() != null && _goldConf.Edicoes.FirstOrDefault() != null)
                {
                    var _goldDates = _goldConf.Edicoes.FirstOrDefault().Datas;
                    var _retrDates = _retrConf.Edicoes.FirstOrDefault().Datas;

                    var _countHit = 0;
                    var _countRetrieved = _retrDates.Count;
                    var _countGold = _goldDates.Count;

                    if (Hit(EnumTipoDataPai.AbstractSubmission, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    if (Hit(EnumTipoDataPai.PaperSubmission, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    if (Hit(EnumTipoDataPai.AcceptanceNotification, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    if (Hit(EnumTipoDataPai.CameraReadyDue, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    if (Hit(EnumTipoDataPai.ConferenceStart, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    if (Hit(EnumTipoDataPai.ConferenceEnd, _goldDates, _retrDates))
                    {
                        _countHit++;
                    }

                    var _precision = (double)_countHit / _countRetrieved;
                    var _recall = (double)_countHit / _countGold;
                    var _fmeasure = 2.0 * _precision * _recall / (_precision + _recall);
                    _fmeasure = double.IsNaN(_fmeasure) ? 0 : _fmeasure;

                    _fmeasures.Add(new Tuple<int, double>(_id, _fmeasure));
                }
            }

            _response.Result = _fmeasures;

            return _response;
        }

        private List<DateEvaluationResults> GetAvaliations()
        {
            return new List<DateEvaluationResults>()
            {
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.AbstractSubmission,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AbstractSubmission),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AbstractSubmission),
                },
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.PaperSubmission,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.PaperSubmission),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.PaperSubmission)
                },
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.AcceptanceNotification,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AcceptanceNotification),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AcceptanceNotification),
                },
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.CameraReadyDue,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.CameraReadyDue),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.CameraReadyDue)
                },
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.ConferenceStart,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceStart),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceStart)
                },
                new DateEvaluationResults
                {
                    TipoData = EnumTipoDataPai.ConferenceEnd,
                    CountGold = m_goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceEnd),
                    CountRetr = m_retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceEnd)
                }
            };
        }
        
        private bool Hit(EnumTipoDataPai tipoData, List<DataTO> gold, List<DataTO> retr)
        {
            var _goldDate = gold.FirstOrDefault(_w => _w.IdTipoData == (int)tipoData);
            var _avalDate = retr.FirstOrDefault(_w => _w.IdTipoData == (int)tipoData);
            if (_goldDate != null && _avalDate != null)
            {
                if (_goldDate.Data == _avalDate.Data)
                {
                    return true;
                }
            }
            return false;
        }

        public class DateEvaluationResults
        {
            public EnumTipoDataPai TipoData { get; set; }

            public int CountGold { get; set; } = 0;
            public int CountRetr { get; set; } = 0;
            public int CountCorrect { get; set; } = 0;

            public double Precision()
            {
                return ((double)CountCorrect) / CountRetr;
            }
            public double Recall()
            {
                return ((double)CountCorrect) / CountGold;
            }
            public double Fmeasure()
            {
                return 2.0 * Precision() * Recall() / (Precision() + Recall());
            }
        }
        #endregion

        #region URL + DATE
        /// <summary>
        /// Calcula acerto de datas apenas nos casos em que a URL está correta
        /// 
        /// OBSOLETO! POIS NÃO ERA ESSA FORMA DE AVALIAÇÃO QUE A VIVIANE ESPERAVA
        /// 
        /// </summary>
        /// <returns></returns>
        //public MessageResponse<List<DateEvaluationResults>> EvaluateUrlDate()
        //{
        //    var _response = new MessageResponse<List<DateEvaluationResults>>();

        //    var _listConfIDs = m_goldStandard.Where(_w => m_confsCorrectURL.Contains(_w.IdConferencia)).Select(_s => _s.IdConferencia).ToList();

        //    var _goldStandard = m_goldStandard.Where(_w => m_confsCorrectURL.Contains(_w.IdConferencia)).ToList();
        //    var _retrieved = m_retrieved.Where(_w => m_confsCorrectURL.Contains(_w.IdConferencia)).ToList();

        //    var _avaliations = new List<DateEvaluationResults>()
        //    {
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.AbstractSubmission,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AbstractSubmission),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AbstractSubmission),
        //        },
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.PaperSubmission,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.PaperSubmission),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.PaperSubmission)
        //        },
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.AcceptanceNotification,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AcceptanceNotification),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.AcceptanceNotification),
        //        },
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.CameraReadyDue,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.CameraReadyDue),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.CameraReadyDue)
        //        },
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.ConferenceStart,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceStart),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceStart)
        //        },
        //        new DateEvaluationResults
        //        {
        //            TipoData = EnumTipoDataPai.ConferenceEnd,
        //            CountGold = _goldStandard.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceEnd),
        //            CountRetr = _retrieved.SelectMany(_s => _s.Edicoes.SelectMany(_s2 => _s2.Datas)).Count(_w => _w.IdTipoData == (int)EnumTipoDataPai.ConferenceEnd)
        //        }
        //    };

        //    foreach (var _id in _listConfIDs)
        //    {
        //        var _goldConf = _goldStandard.FirstOrDefault(_w => _w.IdConferencia == _id);
        //        var _retrConf = _retrieved.FirstOrDefault(_w => _w.IdConferencia == _id);

        //        if (_retrConf != null && _retrConf.Edicoes.FirstOrDefault() != null && _goldConf.Edicoes.FirstOrDefault() != null)
        //        {
        //            var _goldDates = _goldConf.Edicoes.FirstOrDefault().Datas;
        //            var _retrDates = _retrConf.Edicoes.FirstOrDefault().Datas;

        //            if (Hit(EnumTipoDataPai.AbstractSubmission, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.AbstractSubmission).CountCorrect++;
        //            }

        //            if (Hit(EnumTipoDataPai.PaperSubmission, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.PaperSubmission).CountCorrect++;
        //            }

        //            if (Hit(EnumTipoDataPai.AcceptanceNotification, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.AcceptanceNotification).CountCorrect++;
        //            }

        //            if (Hit(EnumTipoDataPai.CameraReadyDue, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.CameraReadyDue).CountCorrect++;
        //            }

        //            if (Hit(EnumTipoDataPai.ConferenceStart, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.ConferenceStart).CountCorrect++;
        //            }

        //            if (Hit(EnumTipoDataPai.ConferenceEnd, _goldDates, _retrDates))
        //            {
        //                _avaliations.First(_w => _w.TipoData == EnumTipoDataPai.ConferenceEnd).CountCorrect++;
        //            }
        //        }
        //    }

        //    _response.Result = _avaliations;

        //    return _response;
        //}

        #endregion

        private int LevenshteinDistance(string s, string t)
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
