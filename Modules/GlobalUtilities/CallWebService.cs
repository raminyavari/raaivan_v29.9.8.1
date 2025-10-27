using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace RaaiVan.Modules.GlobalUtilities
{
    public interface ICallWebService
    {
        string Url { get; set; }
        string MethodName { get; set; }
        string ResultString { get; set; }
        XDocument ResultXML { get; set; }
        Dictionary<string, string> PParams { get; set; }
        string Invoke();
        string Invoke(bool encode);
        void AddParameter(string Key, string val);
    }

    public class CallWebService : ICallWebService
    {
        private string url;
        private string methodName;
        private string resultString;
        public Dictionary<string, string> PParams { get; set; }
        private XDocument resultXML;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public string ResultString
        {
            get { return resultString; }
            set { resultString = value; }
        }

        public XDocument ResultXML
        {
            get { return resultXML; }
            set { resultXML = value; }
        }


        public CallWebService() { }

        public CallWebService(string url, string methodName)
        {
            Url = url;
            MethodName = methodName;
        }

        public void AddParameter(string Key, string val)
        {
            if (PParams == null) PParams = new Dictionary<string, string>();
            PParams.Add(Key, val);
        }

        /// <summary>
        /// Invokes service
        /// </summary>
        /// <param name="encode">Added parameters will encode? (default: true)</param>
        public string Invoke(bool encode)
        {
            return CallWebMethod(Url, MethodName, null, null, PParams, new Dictionary<string, string>(), encode);
        }

        /// <summary>
        /// Invokes service
        /// </summary>
        public string Invoke()
        {
            return CallWebMethod(Url, MethodName, null, null, PParams, new Dictionary<string, string>(), true);
        }


        public static string CallWebMethod(string url, string methodName, string username, string password,
            IDictionary<string, string> parameters, IDictionary<string, string> headers,  bool encode, string actionUri = null)
        {
            string defaultActionUri = "http://tempuri.org/";

            if (string.IsNullOrEmpty(actionUri)) actionUri = defaultActionUri;
            if (!actionUri.EndsWith("/")) actionUri += "/";

            try
            {
                // security checking
                // checkUser(username);
                //if (Isvalid) ...
                string soapStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                       "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                       "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                       (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) ? string.Empty :
                        "<soap:Header>" +
                        "<AuthHeader xmlns=\"" + defaultActionUri + "\">" +
                          "<Username>" + username + "</Username>" +
                          "<Password>" + password + "</Password>" +
                        "</AuthHeader>" +
                       "</soap:Header>"
                       ) +
                       "<soap:Body>" +
                        "<{0} xmlns=\"" + defaultActionUri + "\">" +
                          "{1}" +
                        "</{0}>" +
                      "</soap:Body>" +
                    "</soap:Envelope>";

                string postValues = "";
                foreach (var param in parameters)
                {
                    postValues += string.Format("<{0}>{1}</{0}>", (encode ? HttpUtility.UrlEncode(param.Key) : param.Key),
                        (encode ? HttpUtility.UrlEncode(param.Value) : param.Value));
                }

                soapStr = string.Format(soapStr, methodName, postValues);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
                req.Headers.Add("SOAPAction", "\"" + actionUri + methodName + "\"");
                req.ContentType = "text/xml; charset=\"utf-8\"";
                req.ContentLength = soapStr.Length;
                //req.Accept = "text/xml";
                req.Method = "POST";
                req.Headers["Accept-Encoding"] = "gzip";
                //req.Headers["Accept-Language"] = "en-us";
                req.Credentials = CredentialCache.DefaultNetworkCredentials;
                req.AutomaticDecompression = DecompressionMethods.GZip;
                req.Timeout = 60 * 1000;

                if (headers != null)
                    foreach (string k in headers.Keys) req.Headers[k] = headers[k];

                //req.Proxy = WebRequest.GetSystemWebProxy();

                using (Stream stm = req.GetRequestStream())
                {
                    using (StreamWriter stmw = new StreamWriter(stm))
                        stmw.Write(soapStr);
                }

                string ResultString = string.Empty;
                using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    ResultString = responseReader.ReadToEnd();
                }

                return ResultString;
            }
            catch (WebException ex)
            {
                return PublicMethods.web_exception(ex);
            }
        }

        public static string get(string url, string methodName, string username,
            string password, bool encode)
        {
            try
            {
                // security checking
                // checkUser(username);
                //if (Isvalid) ...
                string soapStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                       "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                       "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                       (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) ? string.Empty :
                        "<soap:Header>" +
                        "<AuthHeader xmlns=\"http://tempuri.org/\">" +
                          "<Username>" + username + "</Username>" +
                          "<Password>" + password + "</Password>" +
                        "</AuthHeader>" +
                       "</soap:Header>"
                       ) +
                       "<soap:Body>" +
                        "<{0} xmlns=\"http://tempuri.org/\">" +
                          "{1}" +
                        "</{0}>" +
                      "</soap:Body>" +
                    "</soap:Envelope>";

                string postValues = "";

                soapStr = string.Format(soapStr, methodName, postValues);

                return soapStr;
            }
            catch (WebException ex)
            {
                return PublicMethods.web_exception(ex);
            }
        }

        public static string CallWebMethod(string url, string methodName,
            IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return CallWebMethod(url, methodName, null, null, parameters, headers, true);
        }

        public static string CallWebMethod(string url, string methodName, 
            string username, string password, IDictionary<string, string> parameters)
        {
            return CallWebMethod(url, methodName, username, password, parameters, new Dictionary<string, string>(), true);
        }

        public static string CallWebMethod(string url, string methodName, 
            IDictionary<string, string> parameters, bool encode = true, string actionUrl = null)
        {
            return CallWebMethod(url, methodName, null, null, parameters, 
                new Dictionary<string, string>(), encode, actionUrl);
        }
    }
}
