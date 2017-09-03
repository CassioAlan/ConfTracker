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
    public class UrlEvaluator
    {
        private List<ConferenciaTO> m_goldStandard;
        private List<ConferenciaTO> m_retrieved;
        
        public UrlEvaluator(int year, EnumTipoExtracao extractionToEvaluate)
        {
            var _conferenciaBT = new ConferenciaBT();

            var _resp = _conferenciaBT.RetrieveURLsConferencias(year, EnumTipoExtracao.Manual);

            if (!_resp.HasErrors())
            {
                m_goldStandard = _resp.Result;
            }

            _resp = _conferenciaBT.RetrieveURLsConferencias(year, extractionToEvaluate);

            if (!_resp.HasErrors())
            {
                m_retrieved = _resp.Result;
            }
        }
        
        
    }
}
