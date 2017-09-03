using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.Evaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            var _ano = 2016;

            //================================================AJUSTAR AQUI O TIPO DE EXTRAÇÃO A AVALIAR==============================
            var _evaluator = new BLL.Evaluation.Evaluator(_ano, EnumTipoExtracao.CRF_Posicional, EnumTipoExtracao.Google);

            var _text = string.Empty;
            _text += UrlEvaluation(_ano, _evaluator);
            _text += "=================================================================\n";
            _text += DateEvaluation(_ano, _evaluator);
            _text += "=================================================================\n";
            //_text += UrlDateEvaluation(_ano, _evaluator);
            //_text += "=================================================================\n";

            Console.Write(_text);

            _text += String.Concat("Data/hora avaliação: ", DateTime.Now.ToString(), "\n",
                                   "Observações gerais: [escrever aqui]\n");

            var _path = @"U:\development\Files\Evaluations\";

            //var _fileNane = String.Concat(DateTime.Now.Year, "_", DateTime.Now.Month.ToString().PadLeft(2, '0'), "_", DateTime.Now.Day.ToString().PadLeft(2, '0'), "_", DateTime.Now. Hour. ToString().PadLeft(2, '0'), "_", DateTime.Now.Minute.ToString().PadLeft(2, '0'), "_", DateTime.Now.Second.ToString().PadLeft(2, '0'));
            var _fileNane = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }

            System.IO.File.WriteAllText(_path + _fileNane + ".txt", _text);

            #region CSV

            var _csv = DateEvaluationCSV(_ano, _evaluator);
            //_csv += UrlDateEvaluationCSV(_ano, _evaluator);

            System.IO.File.WriteAllText(_path + _fileNane + ".csv", _csv);

            #endregion

            Console.WriteLine("Tecle ENTER para abrir a pasta de Avaliações e sair.");
            Console.ReadLine();
            System.Diagnostics.Process.Start(@"U:\development\Files\Evaluations\");
        }

        private static string UrlEvaluation(int ano, BLL.Evaluation.Evaluator evaluator)
        {
            string _text = string.Empty;
            
            var _response = evaluator.EvaluateUrl();

            if (!_response.HasErrors())
            {
                var _eval = _response.Result;

                _text = "AVALIAÇÃO DE URLs - " + evaluator.UrlExtractionType.ToString().ToUpper() + "\n\n";

                _text += String.Concat(
                                          "Total Gold                   : ", _eval.TotalGold, "\n",
                                          "Total recuperado             : ", _eval.TotalRetrieved, "\n",
                                          "Acertos na primeira posição  : ", _eval.CountFirst, "\n",
                                          "Acertos na segunda posição   : ", _eval.CountSecond, "\n",
                                          "Acertos na terceira posição  : ", _eval.CountThird, "\n",
                                          "Acertos em posição 4+        : ", _eval.CountOther, "\n",
                                          "Nenhuma das posições         : ", _eval.CountWrong, "\n\n"
                                      );
            }

            return _text;
        }

        private static string DateEvaluation(int _ano, BLL.Evaluation.Evaluator evaluator)
        {
            string _text = string.Empty;
            
            var _response = evaluator.EvaluateDate();

            if (!_response.HasErrors())
            {
                var _evaluations = _response.Result;

                _text = "AVALIAÇÃO DE DATAS - " + evaluator.DateExtractionType.ToString().ToUpper() + "\n\n";

                foreach (var _eval in _evaluations)
                {

                    _text += String.Concat(_eval.TipoData.ToString().ToUpper(), "\n",
                                          "Gold             - Econtrados: ", _eval.CountGold, "\n",
                                          "Extrator         - Econtrados: ", _eval.CountRetr, "\n",
                                          "Extrator         - Corretos  : ", _eval.CountCorrect, "\n",
                                          "Extrator         - Precision : ", _eval.Precision(), "\n",
                                          "Extrator         - Recall    : ", _eval.Recall(), "\n",
                                          "Extrator         - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
            }

            return _text;
        }

        private static string DateEvaluationCSV(int _ano, BLL.Evaluation.Evaluator evaluator)
        {
            string _text = string.Empty;

            var _response = evaluator.EvaluateDate();

            if (!_response.HasErrors())
            {
                var _evaluations = _response.Result;
                
                foreach (var _eval in _evaluations)
                {
                    _text += _eval.CountGold + "\t";
                                          //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                                          //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                                          //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                                          //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                                          //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                                          //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";

                foreach (var _eval in _evaluations)
                {
                    _text += _eval.CountRetr + "\t";
                    //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                    //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                    //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                    //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                    //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                    //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";

                foreach (var _eval in _evaluations)
                {
                    _text += _eval.CountCorrect + "\t";
                    //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                    //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                    //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                    //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                    //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                    //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";

                foreach (var _eval in _evaluations)
                {
                    _text += _eval.Precision() + "\t";
                    //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                    //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                    //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                    //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                    //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                    //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";

                foreach (var _eval in _evaluations)
                {
                    _text += _eval.Recall() + "\t";
                    //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                    //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                    //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                    //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                    //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                    //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";

                foreach (var _eval in _evaluations)
                {
                    _text += _eval.Fmeasure() + "\t";
                    //"Gold             - Econtrados: ", _eval.CountGold, "\n",
                    //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
                    //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
                    //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
                    //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
                    //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
                }
                _text += "\n";
            }

            return _text;
        }

        //private static string UrlDateEvaluation(int _ano, BLL.Evaluation.Evaluator evaluator)
        //{
        //    string _text = string.Empty;

        //    var _response = evaluator.EvaluateUrlDate();

        //    if (!_response.HasErrors())
        //    {
        //        var _evaluations = _response.Result;

        //        _text = "AVALIAÇÃO DE DATAS COM URLs CORRETAS - " + evaluator.DateExtractionType.ToString().ToUpper() + "\n\n";

        //        foreach (var _eval in _evaluations)
        //        {

        //            _text += String.Concat(_eval.TipoData.ToString().ToUpper(), "\n",
        //                                  "Gold             - Econtrados: ", _eval.CountGold, "\n",
        //                                  "AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //                                  "AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //                                  "AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //                                  "AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //                                  "AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //    }

        //    return _text;
        //}

        //private static string UrlDateEvaluationCSV(int _ano, BLL.Evaluation.Evaluator evaluator)
        //{
        //    string _text = string.Empty;

        //    var _response = evaluator.EvaluateUrlDate();

        //    if (!_response.HasErrors())
        //    {
        //        var _evaluations = _response.Result;

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.CountGold + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.CountRetr + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.CountCorrect + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.Precision() + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.Recall() + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";

        //        foreach (var _eval in _evaluations)
        //        {
        //            _text += _eval.Fmeasure() + "\t";
        //            //"Gold             - Econtrados: ", _eval.CountGold, "\n",
        //            //"AnaliseTexto     - Econtrados: ", _eval.CountRetr, "\n",
        //            //"AnaliseTexto     - Corretos  : ", _eval.CountCorrect, "\n",
        //            //"AnaliseTexto     - Precision : ", _eval.Precision(), "\n",
        //            //"AnaliseTexto     - Recall    : ", _eval.Recall(), "\n",
        //            //"AnaliseTexto     - Fmeasure  : ", _eval.Fmeasure(), "\n\n");
        //        }
        //        _text += "\n";
        //    }

        //    return _text;
        //}
    }
}
