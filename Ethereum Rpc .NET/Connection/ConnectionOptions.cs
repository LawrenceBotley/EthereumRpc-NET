using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EthereumRpc
{
    public class ConnectionOptions
    {
        public string Url { get; set; }
        public string Port { get; set; }
        public int TimeOut { get; set; }
        public NetworkCredential NetworkCredential { get; set; }
        public WebProxy Proxy { get; set; }
        public string FullUrl
        {
            get 
            {
                return string.Format("{0}:{1}", Url, Port);
            }
        }
        public bool IsUrlValid
        {
            get
            {
                return FullUrl.IsUri();
            }
        }

        public ConnectionOptions()
        {
            TimeOut = 5000;
        }
    }
}
