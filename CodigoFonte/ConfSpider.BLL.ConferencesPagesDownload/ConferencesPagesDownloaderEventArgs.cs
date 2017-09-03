using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.BLL.ConferencesPagesDownload
{
    public class ConferencesPagesDownloaderEventArgs : EventArgs
    {
        private string m_message;
        public string Message
        {
            get { return m_message; }
            private set { m_message = value; }
        }

        public ConferencesPagesDownloaderEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
