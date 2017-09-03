using ConfSpider.BLL.Evaluation;
using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.TTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var _ano = 2016;

            var _path = @"U:\development\Files\Evaluations\";
            var _fileNane = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_TTest";
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }
            
            var _evaluator = new Evaluator(_ano, EnumTipoExtracao.Posicional_URLsGoldstandard, EnumTipoExtracao.Manual);
            var _positionalEvaluation = _evaluator.TTestAux();

            var _evaluator2 = new Evaluator(_ano, EnumTipoExtracao.CRF, EnumTipoExtracao.Manual);
            var _crfEvaluation = _evaluator2.TTestAux();

            var _evaluator3 = new Evaluator(_ano, EnumTipoExtracao.CRF_Posicional, EnumTipoExtracao.Manual);
            var _crfPosEvaluation = _evaluator3.TTestAux();

            var _confIDs = _positionalEvaluation.Result.Select(_s => _s.Item1);

            List<string> _lines = new List<string>();
            foreach (var _confId in _confIDs)
            {
                string _line = _confId + "\t";
                var _aux = _positionalEvaluation.Result.Where(_w => _w.Item1 == _confId).Select(_s => _s.Item2).FirstOrDefault();
                _line += Math.Round(_aux, 4) + "\t";
                _aux = _crfEvaluation.Result.Where(_w => _w.Item1 == _confId).Select(_s => _s.Item2).FirstOrDefault();
                _line += Math.Round(_aux, 4) + "\t";
                _aux = _crfPosEvaluation.Result.Where(_w => _w.Item1 == _confId).Select(_s => _s.Item2).FirstOrDefault();
                _line += Math.Round(_aux, 4);

                _lines.Add(_line);
            }
            System.IO.File.WriteAllLines(_path + _fileNane + ".csv", _lines);
        }

    }
}
