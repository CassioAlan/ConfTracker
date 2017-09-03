using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.INL.UrlSearchEngine
{
    public class UrlSearchEngineEventArgs : EventArgs
    {
        private string m_message;
        public string Message
        {
            get { return m_message; }
            private set { m_message = value; }
        }

        public UrlSearchEngineEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
