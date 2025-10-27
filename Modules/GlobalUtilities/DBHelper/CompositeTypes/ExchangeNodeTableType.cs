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
    public class ExchangeNodeTableType : ITableType
    {
        public ExchangeNodeTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangeNodeTableType"; } }

        [PgName("node_id")]
        public Guid? NodeID;

        [PgName("node_additional_id")]
        public string NodeAdditionalID;

        [PgName("name")]
        public string Name;

        [PgName("parent_additional_id")]
        public string ParentAdditionalID;

        [PgName("abstract")]
        public string Abstract;

        [PgName("tags")]
        public string Tags;

        public ExchangeNodeTableType(Guid? nodeId, string nodeAdditionalId, 
            string name, string parentAdditionalId, string abstractDesc, string tags)
        {
            NodeID = nodeId;
            NodeAdditionalID = nodeAdditionalId;
            Name = name;
            ParentAdditionalID = parentAdditionalId;
            Abstract = abstractDesc;
            Tags = tags;
        }

        public object[] to_array()
        {
            return new List<object>() {
                NodeID,
                NodeAdditionalID,
                Name,
                ParentAdditionalID,
                Abstract,
                Tags
            }.ToArray();
        }

        public ExchangeNodeTableType[] get_array(List<ExchangeNodeTableType> list)
        {
            return list.ToArray();
        }
    }
}
