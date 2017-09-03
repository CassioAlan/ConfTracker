using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ConfSpider.TOLibrary;
using ConfSpider.BTLibrary;
using ConfSpider.BLL.UrlSearch;

namespace ConfSpider.UIConsole.ImporterToMySQL
{
    class Program
    {
        private const string mFilesFolder = @"U:\development\Files\";

        static void Main(string[] args)
        {
            var _year = 2017;

            AddNewConferencesFromExcel(_year);

            //UpdateQualis(_year);

            //ImportConferencesFromExcel(_year);

            //ImportTopicsFromTxt();

            //ImportTipoDataFromTxt();

            //GenerateEdition(_year);

            //ImportGoldStandardFromExcel();

            //ImportGoldUrlsFromExcel();
        }

        private static void AddNewConferencesFromExcel(int year)
        {
            try
            {
                #region Excel variables
                var _xlApp = new Microsoft.Office.Interop.Excel.Application();
                var _xlWorkBook = _xlApp.Workbooks.Open(mFilesFolder + "Qualis_conferencia_2017.xlsx", 2, false);
                Worksheet _sheet = _xlWorkBook.Worksheets[1];
                #endregion

                var _conferenciaBT = new ConferenciaBT();
                var _edicaoBT = new EdicaoBT();

                var _ret = _conferenciaBT.RetrieveConferencias(new ConferenciaTO());
                if (_ret.HasErrors())
                {
                    throw new Exception(_ret.Exceptions.First().Description);
                }
                var _oldConfs = _ret.Result;

                var _countId = _oldConfs.Max(s => s.IdConferencia);

                for (int _i = 3; _i <= 1181; _i++)
                {
                    Console.WriteLine("Importando {0}/{1}", _i, 1181);

                    Range _range = _sheet.Rows[string.Format("{0}:{0}", _i)];

                    var _values = _range.Value as object[,];

                    var _sigla = _values[1, 1].ToString();
                    _sigla = _sigla.Length <= 20 ? _sigla : _sigla.Substring(0, 20);
                    var _titulo = _values[1, 2].ToString();
                    _titulo = _titulo.Length <= 150 ? _titulo : _titulo.Substring(0, 150);
                    var _qualis = _values[1, 3].ToString();

                    if (_oldConfs.Any(_w => _w.Sigla.Trim().ToLower() == _sigla.Trim().ToLower()))
                    {
                        continue;
                    }

                    _countId++;
                    Console.WriteLine("Salvando Conf {0} - Sigla {1}", _countId, _sigla);

                    var _resC = _conferenciaBT.SaveConferencia(new ConferenciaTO
                    {
                        IdConferencia = _countId,
                        Sigla = _sigla,
                        Nome = _titulo,
                        Enabled = false
                    });

                    if (_resC.HasErrors())
                    {
                        throw new Exception(_resC.Exceptions.First().Description);
                    }

                    var _res = _edicaoBT.SaveEdicao(new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO { IdConferencia = _countId },
                        Ano = year,
                        Query = $"{year} {_sigla} {_titulo}",
                        Qualis = _qualis
                    });
                    if (_res.HasErrors())
                    {
                        throw new Exception(_res.Exceptions.First().Description);
                    }
                }

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void UpdateQualis(int year)
        {
            try
            {
                #region Excel variables
                var _xlApp = new Microsoft.Office.Interop.Excel.Application();
                var _xlWorkBook = _xlApp.Workbooks.Open(mFilesFolder + "Qualis_conferencia_2017.xlsx", 2, false);
                Worksheet _sheet = _xlWorkBook.Worksheets[1];
                #endregion

                var _conferenciaBT = new ConferenciaBT();
                var _edicaoBT = new EdicaoBT();

                var _qualisList = new Dictionary<string, string>();

                for (int _i = 3; _i <= 1181; _i++)
                {
                    Range _range = _sheet.Rows[string.Format("{0}:{0}", _i)];
                    var _values = _range.Value as object[,];
                    var _sigla = _values[1, 1].ToString();
                    var _qualis = _values[1, 3].ToString();
                    _qualisList.Add(_sigla, _qualis);
                }

                var _res = _conferenciaBT.RetrieveConferencias(new ConferenciaTO());
                if (_res.HasErrors())
                {
                    throw new Exception(_res.Exceptions.First().Description);
                }

                var _conferencias = _res.Result;

                foreach (var _conf in _conferencias)
                {
                    var _q = _qualisList.Where(_w => _w.Key == _conf.Sigla).FirstOrDefault();

                    if (_q.Key != null)
                    {
                        var _query = _conf.Edicoes.Where(_w => _w.Ano == year).Select(_s => _s.Query).FirstOrDefault();

                        var _res2 = _edicaoBT.SaveEdicao(new EdicaoTO
                        {
                            Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                            Ano = year,
                            Query = _query,
                            Qualis = _q.Value
                        });

                        if (_res2.HasErrors())
                        {
                            throw new Exception(_res2.Exceptions.First().Description);
                        }
                    }
                }

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void ImportTipoDataFromTxt()
        {
            try
            {
                var _filePath = @"U:\development\Files\datas_agrupadas.txt";
                var _tiposData = System.IO.File.ReadAllLines(_filePath);

                var _tipoDataBT = new TipoDataBT();

                var _idTipoData = 0;
                var _idPai = 1;
                foreach (var _tipoData in _tiposData)
                {
                    if (string.IsNullOrWhiteSpace(_tipoData))
                    {
                        _idPai = _idTipoData + 1;
                        continue;
                    }
                    var _ret = _tipoDataBT.SaveTipoData(new TipoDataTO
                    {
                        IdTipoData = ++_idTipoData,
                        Descricao = _tipoData,
                        IdTipoDataPai = _idPai
                    });

                    if (_ret.HasErrors())
                    {
                        Console.WriteLine(_ret.Exceptions.First().Description);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void ImportTopicsFromTxt()
        {
            try
            {
                var _filePath = @"U:\development\Files\topicos_agrupados.txt";
                var _topics = System.IO.File.ReadAllLines(_filePath);

                var _topicoBT = new TopicoBT();

                var _idTopico = 0;
                var _idPai = 1;
                foreach (var _topic in _topics)
                {
                    if (string.IsNullOrWhiteSpace(_topic))
                    {
                        _idPai = _idTopico + 1;
                        continue;
                    }
                    var _ret = _topicoBT.SaveTopico(new TopicoTO
                    {
                        IdTopico = ++_idTopico,
                        Descricao = _topic,
                        IdTopicoPai = _idPai
                    });

                    if (_ret.HasErrors())
                    {
                        Console.WriteLine(_ret.Exceptions.First().Description);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void ImportConferencesFromExcel(int year)
        {
            try
            {
                #region Excel variables
                var _xlApp = new Microsoft.Office.Interop.Excel.Application();
                var _xlWorkBook = _xlApp.Workbooks.Open(mFilesFolder + "qualis_conferencias_2012.xlsx", 2, false);
                Worksheet _sheet = _xlWorkBook.Worksheets[1];
                #endregion

                var _countId = 0;

                var _conferenciaBT = new ConferenciaBT();
                var _edicaoBT = new EdicaoBT();

                for (int _i = 5; _i <= 1707; _i++)
                {
                    Console.WriteLine("Importando {0}/{1}", _i, 1707);

                    Range _range = _sheet.Rows[string.Format("{0}:{0}", _i)];

                    var _values = _range.Value as object[,];

                    var _sigla = _values[1, 1].ToString();
                    _sigla = _sigla.Length <= 20 ? _sigla : _sigla.Substring(0, 20);
                    var _titulo = _values[1, 2].ToString();
                    _titulo = _titulo.Length <= 150 ? _titulo : _titulo.Substring(0, 150);
                    var _qualis = _values[1, 4].ToString();

                    // método específico para montar a query implementando na classe BingSearch
                    //var _search = $"{DateTime.Now.Year} {_sigla} {_titulo}";
                    //_search = _search.Length <= 250 ? _search : _search.Substring(0, 250);

                    _countId++;

                    //_conferenciaBT.SaveConferencia(new ConferenciaTO
                    //{
                    //    IdConferencia = _countId,
                    //    Sigla = _sigla,
                    //    Nome = _titulo,
                    //    Enabled = true
                    //});

                    var _res = _edicaoBT.SaveEdicao(new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO { IdConferencia = _countId },
                        Ano = year,
                        Query = $"{year} {_sigla} {_titulo}",
                        Qualis = _qualis
                    });

                    if (_res.HasErrors())
                    {
                        throw new Exception(_res.Exceptions.First().Description);
                    }
                }

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void GenerateEdition(int year)
        {
            try
            {
                var _conferenciaBT = new ConferenciaBT();
                var _edicaoBT = new EdicaoBT();

                var _retConferencias = _conferenciaBT.RetrieveConferencias(new ConferenciaTO());

                if (_retConferencias.HasErrors())
                {
                    throw new Exception(_retConferencias.Exceptions.First().Description);
                }

                var _listConferencias = _retConferencias.Result;

                foreach (var _conf in _listConferencias)
                {
                    var _lastEd = _conf.Edicoes.LastOrDefault();

                    var _retAddEd = _edicaoBT.SaveEdicao(new EdicaoTO
                    {
                        Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                        Ano = year,
                        Query = $"{year} {year + 1} {_conf.Sigla} {_conf.Nome}",
                        Qualis = _lastEd != null ? _lastEd.Qualis : ""
                    });

                    if (_retAddEd.HasErrors())
                    {
                        throw new Exception(_retAddEd.Exceptions.First().Description);
                    }
                }

                var _urlSearcher = new UrlSearcher();

                //var _retGenQ = _urlSearcher.GenerateQueries();

                //if (_retGenQ.HasErrors())
                //{
                //    throw new Exception(_retGenQ.Exceptions.First().Description);
                //}

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void ImportGoldStandardFromExcel()
        {
            try
            {
                #region Excel variables
                var _xlApp = new Microsoft.Office.Interop.Excel.Application();
                var _xlWorkBook = _xlApp.Workbooks.Open(mFilesFolder + "Gold 20170313 - datas nos htmls selecionados.xlsx", 2, false);
                Worksheet _sheet = _xlWorkBook.Worksheets[1];
                #endregion

                var _dataBT = new DataBT();

                for (int _i = 3; _i <= 82; _i++)
                {
                    Console.WriteLine("Importando {0}/{1}", _i, 101);

                    Range _range = _sheet.Rows[string.Format("{0}:{0}", _i)];

                    var _values = _range.Value as object[,];

                    var _idConferencia = Convert.ToInt32(_values[1, 1]);
                    var _ano = 2016;
                    var _tipoExtracao = EnumTipoExtracao.Manual;

                    if (_values[1, 2] != null)
                    {
                        var _tipoData = 1;
                        var _date = Convert.ToDateTime(_values[1, 2].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                    if (_values[1, 3] != null)
                    {
                        var _tipoData = 5;
                        var _date = Convert.ToDateTime(_values[1, 3].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                    if (_values[1, 4] != null)
                    {
                        var _tipoData = 17;
                        var _date = Convert.ToDateTime(_values[1, 4].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                    if (_values[1, 5] != null)
                    {
                        var _tipoData = 26;
                        var _date = Convert.ToDateTime(_values[1, 5].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                    if (_values[1, 6] != null)
                    {
                        var _tipoData = 33;
                        var _date = Convert.ToDateTime(_values[1, 6].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                    if (_values[1, 7] != null)
                    {
                        var _tipoData = 37;
                        var _date = Convert.ToDateTime(_values[1, 7].ToString());

                        _dataBT.SaveData(new DataTO
                        {
                            TipoExtracao = _tipoExtracao,
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO
                                {
                                    IdConferencia = _idConferencia,
                                },
                                Ano = _ano
                            },
                            IdTipoData = _tipoData,
                            Data = _date
                        });
                    }
                }

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void ImportGoldUrlsFromExcel()
        {
            try
            {
                #region Excel variables
                var _xlApp = new Microsoft.Office.Interop.Excel.Application();
                var _xlWorkBook = _xlApp.Workbooks.Open(mFilesFolder + "gold_04_copy.xlsx", 2, false);
                Worksheet _sheet = _xlWorkBook.Worksheets[1];
                #endregion

                var _urlBT = new UrlBT();

                for (int _i = 2; _i <= 101; _i++)
                {
                    Console.WriteLine("Importando {0}/{1}", _i, 101);

                    Range _range = _sheet.Rows[string.Format("{0}:{0}", _i)];

                    var _values = _range.Value as object[,];

                    var _idConferencia = Convert.ToInt32(_values[1, 1]);

                    if (_values[1, 4] != null)
                    {
                        var _url = _values[1, 4].ToString();

                        var _ret = _urlBT.SaveUrl(new UrlTO
                        {
                            Edicao = new EdicaoTO
                            {
                                Conferencia = new ConferenciaTO { IdConferencia = _idConferencia },
                                Ano = 2016
                            },
                            Ordem = 101,
                            Url = _url,
                            TipoExtracao = EnumTipoExtracao.Manual
                        });
                    }
                }

                Console.WriteLine("Sucesso!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

    }
}
