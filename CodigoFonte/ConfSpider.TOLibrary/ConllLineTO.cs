using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.TOLibrary
{
    public class ConllLineTO
    {
        public string Token { get; set; }
        public string FAbstract { get; set; }
        public string FPaper { get; set; }
        public string FAcception { get; set; }
        public string FCameraReady { get; set; }
        public string FEvent { get; set; }
        public string Label { get; set; }

        public ConllLineTO(string line)
        {
            var _p = line.Split('\t');

            Token = _p[0];
            FAbstract = _p[1];
            FPaper = _p[2];
            FAcception = _p[3];
            FCameraReady = _p[4];
            FEvent = _p[5];
            Label = _p[6];
        }
    }
}
