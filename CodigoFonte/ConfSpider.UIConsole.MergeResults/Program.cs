using ConfSpider.BTLibrary;
using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.MergeResults
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configurar

            var _mainTipoExtracao = EnumTipoExtracao.CRF;
            var _secTipoExtracao = EnumTipoExtracao.Posicional_URLsGoldstandard;
            var _resultTipoExtracao = EnumTipoExtracao.CRF_Posicional;
            var _year = 2016; 

            #endregion
            
            var _conferenciaBT = new ConferenciaBT();
            List<ConferenciaTO> _mainDates = new List<ConferenciaTO>();
            List<ConferenciaTO> _secDates = new List<ConferenciaTO>();
            List<ConferenciaTO> _resultDates = new List<ConferenciaTO>();

            var _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(_year, _mainTipoExtracao, EnumTipoExtracao.Manual);
            if (_resp.HasErrors())
            {
                Console.WriteLine(_resp.Exceptions.First().Description);
                Console.ReadLine();
                return;
            }
            _mainDates = _resp.Result;

            
            _resp = _conferenciaBT.RetrieveConferenciasWithDatesAndURL(_year, _secTipoExtracao, EnumTipoExtracao.Manual);
            if (_resp.HasErrors())
            {
                Console.WriteLine(_resp.Exceptions.First().Description);
                Console.ReadLine();
                return;
            }
            _secDates = _resp.Result;

            #region Verificação da ordem das datas de SEC (_secDates)

            foreach (var _conf in _secDates)
            {
                var _dates = _conf.Edicoes.FirstOrDefault(_w => _w.Ano == _year).Datas;

                int _countTotal = _dates.Count();
                decimal _tolerance = Math.Round(((decimal)_countTotal) / 2, MidpointRounding.ToEven);

                for (int i = 0; i < _dates.Count; i++)
                {
                    int _countError = 0;

                    if (_dates[i].Data != DateTime.MinValue)
                    {
                        for (int j = 0; j < _dates.Count; j++)
                        {
                            if (_dates[j].Data != DateTime.MinValue)
                            {
                                if (i < j)
                                {
                                    if (_dates[i].Data > _dates[j].Data || Math.Abs(_dates[i].Data.Year - _dates[j].Data.Year) > 1)
                                    {
                                        _countError++;
                                    }
                                }
                                if (i > j)
                                {
                                    if (_dates[i].Data < _dates[j].Data || Math.Abs(_dates[i].Data.Year - _dates[j].Data.Year) > 1)
                                    {
                                        _countError++;
                                    }
                                }
                            }
                        }

                        if (_tolerance < _countError)
                        {
                            _dates[i].Data = DateTime.MinValue;
                        }
                    }
                }
            }

            #endregion
            
            var _listConfIds = _mainDates.Select(_s => _s.IdConferencia).Union(_secDates.Select(_s => _s.IdConferencia)).Distinct().ToList();
            int[] _tiposData = new int[] { 1, 5, 17, 26, 33, 37 };
            var _dataBT = new DataBT();

            foreach (var _confId in _listConfIds)
            {
                var _dates = new List<DateTime>()
                        //{
                        //    new List<DateTime>(), // VLU_ABS 0
                        //    new List<DateTime>(), // VLU_PPR 1
                        //    new List<DateTime>(), // VLU_ACC 2
                        //    new List<DateTime>(), // VLU_CAM 3
                        //    new List<DateTime>(), // VLU_EVE 4
                        //    new List<DateTime>()  // VLU_EVE 5
                        //}
                        ;

                #region Seleciona datas da conferencia

                foreach (var _tipoData in _tiposData)
                {
                    DateTime _mainData = new DateTime();
                    var _conf = _mainDates.Where(_w => _w.IdConferencia == _confId).Select(_s => _s).FirstOrDefault();
                    if (_conf != null)
                    {
                        var _ed = _conf.Edicoes.Where(_w => _w.Ano == _year).FirstOrDefault();
                        if (_ed != null)
                        {
                            _mainData = _ed.Datas.Where(_w => _w.IdTipoData == _tipoData).Select(_s => _s.Data).FirstOrDefault();
                        }
                    }

                    DateTime _secData = new DateTime();
                    _conf = _secDates.Where(_w => _w.IdConferencia == _confId).Select(_s => _s).FirstOrDefault();
                    if (_conf != null)
                    {
                        var _ed = _conf.Edicoes.Where(_w => _w.Ano == _year).FirstOrDefault();
                        if (_ed != null)
                        {
                            _secData = _ed.Datas.Where(_w => _w.IdTipoData == _tipoData).Select(_s => _s.Data).FirstOrDefault();
                        }
                    }

                    DateTime _resDate = new DateTime();
                    if (_mainData == _secData)
                    {
                        _resDate = _mainData;
                    }
                    else if (_mainData != DateTime.MinValue)
                    {
                        _resDate = _mainData;
                    }
                    else
                    {
                        _resDate = _secData;
                    }

                    _dates.Add(_resDate);
                }

                #endregion
                
                #region Salva na base
                for (int j = 0; j < _dates.Count; j++)
                {
                    if (_dates[j] != DateTime.MinValue)
                    {
                        var _conf = new ConferenciaTO
                        {
                            IdConferencia = Convert.ToInt32(_confId)
                        };
                        var _ed = new EdicaoTO
                        {
                            Ano = _year,
                            Conferencia = _conf
                        };
                        var _data = new DataTO
                        {
                            Edicao = _ed,
                            Data = _dates[j],
                            TipoExtracao = _resultTipoExtracao,
                            IdTipoData = _tiposData[j]
                        };

                        var _response = _dataBT.SaveData(_data);
                        if (_response.HasErrors())
                        {
                            Console.WriteLine("ERRO: " + _response.Exceptions.First().Description);
                        }
                    }
                }
                #endregion
            }

            Console.WriteLine("Processamento finalizado. Tecle ENTER para sair.");
            Console.ReadLine();
        }
    }
}
