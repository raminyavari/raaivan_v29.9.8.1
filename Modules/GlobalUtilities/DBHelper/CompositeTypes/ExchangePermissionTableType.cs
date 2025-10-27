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
    public class ExchangePermissionTableType : ITableType
    {
        public ExchangePermissionTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangePermissionTableType"; } }

        [PgName("node_type_additional_id")]
        public string NodeTypeAdditionalID;

        [PgName("node_additional_id")]
        public string NodeAdditionalID;

        [PgName("group_type_additional_id")]
        public string GroupTypeAdditionalID;

        [PgName("group_additional_id")]
        public string GroupAdditionalID;

        [PgName("username")]
        public string UserName;

        [PgName("permission_type")]
        public string PermissionType;

        [PgName("allow")]
        public bool? Allow;

        [PgName("drop_all")]
        public bool? DropAll;

        public ExchangePermissionTableType(string nodeTypeAdditionalId, string nodeAdditionalId, string groupTypeAdditionalId, 
            string groupAdditionalId, string username, string permissionType, bool? allow, bool? dropAll)
        {
            NodeTypeAdditionalID = nodeTypeAdditionalId;
            NodeAdditionalID = nodeAdditionalId;
            GroupTypeAdditionalID = groupTypeAdditionalId;
            GroupAdditionalID = groupAdditionalId;
            UserName = username;
            PermissionType = permissionType;
            Allow = allow;
            DropAll = dropAll;
        }

        public object[] to_array()
        {
            return new List<object>() {
                NodeTypeAdditionalID,
                NodeAdditionalID,
                GroupTypeAdditionalID,
                GroupAdditionalID,
                UserName,
                PermissionType,
                Allow,
                DropAll
            }.ToArray();
        }

        public ExchangePermissionTableType[] get_array(List<ExchangePermissionTableType> list)
        {
            return list.ToArray();
        }
    }
}
