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
    public class ExchangeMemberTableType : ITableType
    {
        public ExchangeMemberTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangeMemberTableType"; } }

        [PgName("node_type_additional_id")]
        public string NodeTypeAdditionalID;

        [PgName("node_additional_id")]
        public string NodeAdditionalID;

        [PgName("node_id")]
        public Guid? NodeID;

        [PgName("username")]
        public string UserName;

        [PgName("is_admin")]
        public bool? IsAdmin;

        public ExchangeMemberTableType(string nodeTypeAdditionalId, string nodeAdditionalId, Guid? nodeId, string username, bool? isAdmin)
        {
            NodeTypeAdditionalID = nodeTypeAdditionalId;
            NodeAdditionalID = nodeAdditionalId;
            NodeID = nodeId;
            UserName = username;
            IsAdmin = isAdmin;
        }

        public object[] to_array()
        {
            return new List<object>() {
                NodeTypeAdditionalID,
                NodeAdditionalID,
                NodeID,
                UserName,
                IsAdmin
            }.ToArray();
        }

        public ExchangeMemberTableType[] get_array(List<ExchangeMemberTableType> list)
        {
            return list.ToArray();
        }
    }
}
