using ConfSpider.BTLibrary;
using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.ResultToDB
{
    class Program
    {

        static void Main(string[] args)
        {
            string _dataPath = string.Empty;
            string[] _resultFiles = { };

            #region Configurar

            var _runningYear = 2017;
            var _crossValidation = false;

            #endregion

            if (_crossValidation)
            {
                _dataPath = @"U:\development\CRF_webpages\fold_data\";
                _resultFiles = Directory.GetFiles(_dataPath, "result1.data", SearchOption.AllDirectories);
            }
            else
            {
                //_dataPath = @"U:\development\CRF_webpages\all_data\";
                //_resultFiles = Directory.GetFiles(_dataPath, "result_window5.data", SearchOption.TopDirectoryOnly);

                _dataPath = @"D:\webpages_2017\";
                _resultFiles = Directory.GetFiles(_dataPath, "*site_result.data", SearchOption.TopDirectoryOnly);
            }
            
            foreach (var _resultFullPath in _resultFiles)
            {
                var _resultContent = File.ReadAllText(_resultFullPath, Encoding.UTF8);
                //var _conferencesResults = _resultContent.Split(new string[] { Environment.NewLine + Environment.NewLine, "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var _conferencesResults = new List<string>() { _resultContent };

                var _dataBT = new DataBT();

                foreach (var _cr in _conferencesResults)
                {
                    var _idConferencia = Path.GetFileName(_resultFullPath).Substring(0, 4);
                    Console.WriteLine("Processando Conferência {0}.", _idConferencia);

                    #region Extração
                    var _dates = new List<List<DateTime>>
                        {
                            new List<DateTime>(), // VLU_ABS 0
                            new List<DateTime>(), // VLU_PPR 1
                            new List<DateTime>(), // VLU_ACC 2
                            new List<DateTime>(), // VLU_CAM 3
                            new List<DateTime>(), // VLU_EVE 4
                            new List<DateTime>()  // VLU_EVE 5
                        };

                    var _lines = Regex.Split(_cr, Environment.NewLine);

                    // começa na terceira linha e vai até a penúltima, pq sempre analisa as linhas anterior e próxima
                    // e a primeira se trata apenas do ID da conferência
                    int _i = 1;
                    while (_i < _lines.Length - 1)
                    {
                        // sempre de um em um, pois se a linha corrente nao for compativel com a anterior, nao faz nada
                        var _plusControl = 1;

                        var _currLineValues = _lines[_i].Split(new string[] { "\t" }, StringSplitOptions.None);
                        var _prevLineValues = _lines[_i - 1].Split(new string[] { "\t" }, StringSplitOptions.None);
                        var _nextLineValues = _lines[_i + 1].Split(new string[] { "\t" }, StringSplitOptions.None);

                        #region Encontrando datas
                        // se a tag for de VLU e for igual a anterior
                        if (_currLineValues.Last().StartsWith("VLU_") &&
                            _currLineValues.Last() == _prevLineValues.Last())
                        {
                            // se a linha corrente for compativel com a anterior, ja posso pular mais uma linha
                            _plusControl++;

                            // tenta transformar os VLU em um DateTime
                            DateTime _date = new DateTime();
                            DateTime _date2 = new DateTime();

                            var _dateStringValues = new List<string>();

                            AddDateStringValue(_prevLineValues.First(), _dateStringValues);

                            AddDateStringValue(_currLineValues.First(), _dateStringValues);

                            // verifica se há um terceiro VLU igual
                            if (_currLineValues.Last() == _nextLineValues.Last())
                            {
                                AddDateStringValue(_nextLineValues.First(), _dateStringValues);

                                _plusControl++;
                            }

                            // caso seja data da conferência, pode ter um intervalo de datas
                            if (_currLineValues.Last() == "VLU_EVE")
                            {
                                for (int _j = 2; _j <= 6; _j++)
                                {
                                    if (_i + _j < _lines.Length)
                                    {
                                        var _otherLineValues = _lines[_i + _j].Split(new string[] { "\t" }, StringSplitOptions.None);
                                        if (_currLineValues.Last() == _otherLineValues.Last())
                                        {
                                            AddDateStringValue(_otherLineValues.First(), _dateStringValues);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            // achou apenas 2 valores = assume o ano que está sendo pesquisado
                            if (_dateStringValues.Count() == 2)
                            {
                                _dateStringValues.Add(_runningYear.ToString());
                            }

                            // achou 3 valores
                            if (_dateStringValues.Count() == 3)
                            {
                                // DD/MM/YYYY
                                if (!DateTime.TryParse(String.Join("/", _dateStringValues), out _date))
                                {
                                    // MM/DD/YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[0], _dateStringValues[2] }), out _date))
                                    {
                                        // YYYY/MM/DD
                                        if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[2], _dateStringValues[1], _dateStringValues[0] }), out _date))
                                        {
                                            // o DateTime fica apenas instanciado
                                        }
                                    }
                                }
                            }

                            // se for rótulo de datas do evento e possui apenas 3 valores, 
                            // assume que falta o ano: coloca-se o atual
                            if (_currLineValues.Last() == "VLU_EVE" && _dateStringValues.Count == 3)
                            {
                                _dateStringValues.Add(_runningYear.ToString());
                            }

                            // achou 4 valores e é data de evento
                            if (_dateStringValues.Count == 4 && _currLineValues.Last() == "VLU_EVE")
                            {
                                // caso o item 0 seja mês por extenso
                                if (Regex.IsMatch(_dateStringValues[0], "[a-z]"))
                                {
                                    // MM DD DD YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[0], _dateStringValues[3] }), out _date)
                                      || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[2], _dateStringValues[0], _dateStringValues[3] }), out _date2))
                                    {
                                    }
                                }

                                // caso o item 2 seja mês por extenso
                                if (_date == DateTime.MinValue
                                 && Regex.IsMatch(_dateStringValues[2], "[a-z]"))
                                {
                                    // DD DD MM YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[0], _dateStringValues[2], _dateStringValues[3] }), out _date)
                                     || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[2], _dateStringValues[3] }), out _date2))
                                    {
                                        // o DateTime fica apenas instanciado
                                    }
                                }

                                // caso não tenha mês por extenso
                                if (_date == DateTime.MinValue)
                                {
                                    // MM DD DD YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[0], _dateStringValues[3] }), out _date)
                                      || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[2], _dateStringValues[0], _dateStringValues[3] }), out _date2))
                                    {
                                        // DD DD MM YYYY
                                        if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[0], _dateStringValues[2], _dateStringValues[3] }), out _date)
                                         || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[2], _dateStringValues[3] }), out _date2))
                                        {
                                            // o DateTime fica apenas instanciado
                                        }
                                    }
                                }
                            }

                            // achou 5 valores e é data de evento
                            if (_dateStringValues.Count == 5 && _currLineValues.Last() == "VLU_EVE")
                            {
                                // caso o item 0 ou 2 seja mês por extenso
                                if (Regex.IsMatch(_dateStringValues[0], "[a-z]") || Regex.IsMatch(_dateStringValues[2], "[a-z]"))
                                {
                                    // MM DD MM DD YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[0], _dateStringValues[4] }), out _date)
                                      || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[3], _dateStringValues[2], _dateStringValues[4] }), out _date2))
                                    {
                                    }
                                }

                                // caso o item 1 ou 3 seja mês por extenso
                                if (_date == DateTime.MinValue
                                 && (Regex.IsMatch(_dateStringValues[1], "[a-z]") || Regex.IsMatch(_dateStringValues[3], "[a-z]")))
                                {
                                    // DD MM DD MM YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[0], _dateStringValues[1], _dateStringValues[4] }), out _date)
                                     || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[2], _dateStringValues[3], _dateStringValues[4] }), out _date2))
                                    {
                                        // o DateTime fica apenas instanciado
                                    }
                                }

                                // caso não tenha mês por extenso
                                if (_date == DateTime.MinValue)
                                {
                                    // MM DD MM DD YYYY
                                    if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[1], _dateStringValues[0], _dateStringValues[4] }), out _date)
                                      || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[3], _dateStringValues[2], _dateStringValues[4] }), out _date2))
                                    {
                                        // DD MM DD MM YYYY
                                        if (!DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[0], _dateStringValues[1], _dateStringValues[4] }), out _date)
                                         || !DateTime.TryParse(String.Join("/", new string[] { _dateStringValues[2], _dateStringValues[3], _dateStringValues[4] }), out _date2))
                                        {
                                            // o DateTime fica apenas instanciado
                                        }
                                    }
                                }
                            }

                            // Já Filtra datas com diferença de anos maior que 1 em relação ao ano de execução
                            if (_date != DateTime.MinValue && Math.Abs(_date.Year - _runningYear) <= 1)
                            {
                                switch (_currLineValues.Last())
                                {
                                    //case "VLU_ABS":
                                    //    if (_dates[0] == DateTime.MinValue)
                                    //    {
                                    //        _dates[0] = _date;
                                    //    }
                                    //    break;
                                    //case "VLU_PPR":
                                    //    if (_dates[1] == DateTime.MinValue)
                                    //    {
                                    //        _dates[1] = _date;
                                    //    }
                                    //    break;
                                    //case "VLU_ACC":
                                    //    if (_dates[2] == DateTime.MinValue)
                                    //    {
                                    //        _dates[2] = _date;
                                    //    }
                                    //    break;
                                    //case "VLU_CAM":
                                    //    if (_dates[3] == DateTime.MinValue)
                                    //    {
                                    //        _dates[3] = _date;
                                    //    }
                                    //    break;
                                    //case "VLU_EVE":
                                    //    if (_dates[4] == DateTime.MinValue && _date2 != DateTime.MinValue)
                                    //    {
                                    //        _dates[4] = _date;
                                    //        _dates[5] = _date2;
                                    //    }

                                    case "VLU_ABS":
                                        _dates[0].Add(_date);
                                        break;
                                    case "VLU_PPR":
                                        _dates[1].Add(_date);
                                        break;
                                    case "VLU_ACC":
                                        _dates[2].Add(_date);
                                        break;
                                    case "VLU_CAM":
                                        _dates[3].Add(_date);
                                        break;
                                    case "VLU_EVE":
                                        if (_date2 != DateTime.MinValue
                                            && _date2 >= _date
                                            && Math.Abs(_date2.Year - _runningYear) <= 1)
                                        {
                                            _dates[4].Add(_date);
                                            _dates[5].Add(_date2);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                // caso não consiga extrair data, força que o próximo imediato seja avaliado
                                _plusControl = 1;
                            }
                        }

                        #endregion

                        _i += _plusControl;
                    }

                    int _countTotal = _dates.Count(_w => _w.Count > 0);
                    decimal _tolerance = Math.Round(((decimal)_countTotal) / 2, MidpointRounding.ToEven); 
                    #endregion

                    #region Verificação da ordem das datas

                    int i = 0;
                    while (i < _dates.Count)
                    {
                        int _countError = 0;

                        if (_dates[i].Count > 0)
                        {
                            for (int j = 0; j < _dates.Count; j++)
                            {
                                if (_dates[j].Count > 0)
                                {
                                    if (i < j)
                                    {
                                        if (_dates[i][0] > _dates[j][0] || Math.Abs(_dates[i][0].Year - _dates[j][0].Year) > 1)
                                        {
                                            _countError++;
                                        }
                                    }
                                    if (i > j)
                                    {
                                        if (_dates[i][0] < _dates[j][0] || Math.Abs(_dates[i][0].Year - _dates[j][0].Year) > 1)
                                        {
                                            _countError++;
                                        }
                                    }
                                }
                            }

                            if (_tolerance < _countError)
                            {
                                _dates[i].RemoveAt(0);
                                // quando remover uma data de evento, remove a outra também (inicial e final)
                                if (i == 4)
                                {
                                    _dates[5].RemoveAt(0);
                                }
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }

                    #endregion

                    #region Salvar na base

                    //var _idConferencia = _lines.First().Split(new string[] { "\t" }, StringSplitOptions.None).First().Substring(4);
                    
                    int[] _tiposData = new int[] { 1, 5, 17, 26, 33, 37 };

                    for (int j = 0; j < _dates.Count; j++)
                    {
                        if (_dates[j].Count > 0)
                        {
                            var _conf = new ConferenciaTO
                            {
                                IdConferencia = Convert.ToInt32(_idConferencia)
                            };
                            var _ed = new EdicaoTO
                            {
                                Ano = _runningYear,
                                Conferencia = _conf
                            };
                            var _data = new DataTO
                            {
                                Edicao = _ed,
                                Data = _dates[j][0],
                                TipoExtracao = EnumTipoExtracao.CRF,
                                IdTipoData = _tiposData[j]
                            };

                            var _response = _dataBT.SaveData(_data);
                        }
                    }

                    #endregion
                }
            }
        }

        private static void AddDateStringValue(string value, List<string> dateStringValues)
        {
            if (DateUtilTO.IsDay(value)
             || DateUtilTO.IsMonth(value)
             || DateUtilTO.IsYear(value))
            {
                if (DateUtilTO.IsDay(value))
                {
                    dateStringValues.Add(Regex.Replace(value, @"st|nd|rd|th", ""));
                }
                else
                {
                    dateStringValues.Add(value);
                }
            }
        }
    }
}
