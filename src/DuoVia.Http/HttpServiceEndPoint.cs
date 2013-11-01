using System;

namespace DuoVia.Http
{
    public class HttpServiceEndPoint
    {
        private string url;
        private Uri uri;

        /// <summary>
        /// The server endpoint that the client will communicate with.
        /// Only override the path parameters if your host has also modified the default values.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="appPath"></param>
        /// <param name="metadataPath"></param>
        public HttpServiceEndPoint(string url,
            string appPath = "/dv/app", string metadataPath = "/dv/metadata")
        {
            if (null == url) throw new ArgumentNullException("url");
            this.Url = url;
            this.AppPath = appPath ?? "/dv/app";
            this.MetadataPath = metadataPath ?? "/dv/metatada";
        }

        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                uri = new Uri(url);
            }
        }

        internal Uri Uri 
        {
            get
            {
                return uri;
            }
        }

        public string AppPath { get; set; }
        public string MetadataPath { get; set; }
    }
}
