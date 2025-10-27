using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class ExchangeAuthorTableType : ITableType
    {
        public ExchangeAuthorTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangeAuthorTableType"; } }

        [PgName("node_type_additional_id")]
        public string NodeTypeAdditionalID;

        [PgName("node_additional_id")]
        public string NodeAdditionalID;

        [PgName("username")]
        public string UserName;

        [PgName("Percentage")]
        public int? Percentage;

        public ExchangeAuthorTableType(string nodeTypeAdditionalId, string nodeAdditionalId, string username, int? percentage)
        {
            NodeTypeAdditionalID = nodeTypeAdditionalId;
            NodeAdditionalID = nodeAdditionalId;
            UserName = username;
            Percentage = percentage;
        }

        public object[] to_array()
        {
            return new List<object>() {
                NodeTypeAdditionalID,
                NodeAdditionalID,
                UserName,
                Percentage
            }.ToArray();
        }

        public ExchangeAuthorTableType[] get_array(List<ExchangeAuthorTableType> list)
        {
            return list.ToArray();
        }
    }
}
