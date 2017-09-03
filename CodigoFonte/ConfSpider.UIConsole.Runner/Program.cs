using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.Runner
{
    class Program
    {
        static void Main(string[] args)
        {

            ConfSpider.UIConsole.HTMLtoCoNLL.Program.Main(args);

            ConfSpider.UIConsole.CoNLLFeatures.Program.Main(args);

            ConfSpider.UIConsole.TokenFilter.Program.Main(args);

            ConfSpider.UIConsole.CSVsToDATA.Program.Main(args);

            ConfSpider.UIConsole.SimplifyLabels.Program.Main(args);


        }
    }
}
