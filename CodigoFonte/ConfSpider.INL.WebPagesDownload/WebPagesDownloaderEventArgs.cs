using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.INL.WebPagesDownload
{
    public class WebPagesDownloaderEventArgs : EventArgs
    {
        private string m_message;
        public string Message
        {
            get { return m_message; }
            private set { m_message = value; }
        }

        public WebPagesDownloaderEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
