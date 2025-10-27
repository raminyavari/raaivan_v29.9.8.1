using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Services;

namespace RaaiVanIntermediate
{
    public static class Knowledge
    {
        private static RaaiVanKnowledge.KnowledgeSoapClient _ServiceHandler;
        private static RaaiVanKnowledge.KnowledgeSoapClient ServiceHandler
        {
            get
            {
                if (_ServiceHandler == null)
                    _ServiceHandler = new RaaiVanKnowledge.KnowledgeSoapClient("KnowledgeSoap", "http://127.0.0.1/Services/Knowledge.asmx");
                
                return _ServiceHandler;
            }
        }

        public static DataTable GetRelatedKnowledges(string nodeId, string nodeTypeId)
        {
            return ServiceHandler.GetRelatedKnowledges(nodeId, nodeTypeId);
        }

        public static DataTable SearchKnowledges(string viewerusername, string creatorusername, string searchText, DataTable relatedNodes, 
            DateTime? lowerCreationDateLimit, DateTime? upperCreationDateLimit, 
            DateTime? lowerPublicationDateLimit, DateTime? upperPublicationDateLimit)
        {
            return ServiceHandler.SearchKnowledges(viewerusername, creatorusername, searchText, relatedNodes, 
                lowerCreationDateLimit, upperCreationDateLimit, lowerPublicationDateLimit, upperPublicationDateLimit);
        }

        public static int GetKnowledgesCount(string viewerusername, string creatorusername, string searchText, DataTable relatedNodes,
            DateTime? lowerCreationDateLimit, DateTime? upperCreationDateLimit,
            DateTime? lowerPublicationDateLimit, DateTime? upperPublicationDateLimit)
        {
            return ServiceHandler.GetKnowledgesCount(viewerusername, creatorusername, searchText, relatedNodes, 
                lowerCreationDateLimit, upperCreationDateLimit, lowerPublicationDateLimit, upperPublicationDateLimit);
        }
    }
    /*
    public static class Extensions
    {
        private static T Cast<T>(dynamic o) { return (T)o; }

        public static dynamic DynamicCast(this Type T, dynamic o)
        {
            return typeof(Extensions).GetMethod("Cast", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .MakeGenericMethod(T).Invoke(null, new object[] { o });
        }

        public static int GetTest()
        {
            //System.Reflection.MethodInfo mi = 

            WebRequest request = WebRequest.Create(new Uri("http://127.0.0.1:5148/Services/Knowledge.asmx/GetTest"));
            request.Method = "Post";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Credentials = CredentialCache.DefaultCredentials;

            Stream requestStream = request.GetRequestStream();
            //StreamWriter rStreamWriter = new StreamWriter(requestStream);
            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("message=<GetTest><serviceRequestTimeStamp>" + 
            //    System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "</serviceRequestTimeStamp></GetTest>");
            //rStreamWriter.Write(stringBuilder);
            //rStreamWriter.Dispose();

            //WebProxy proxyObject = new WebProxy(new Uri("http://127.0.0.1:5148/Services/Knowledge.asmx"), true);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Dispose();
            dataStream.Dispose();
            response.Close();

            return int.Parse(responseFromServer);
        }
    }
    */
}
