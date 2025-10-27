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
    public class GuidTableType : ITableType
    {
        public GuidTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "GuidTableType"; } }

        [PgName("value")]
        public Guid? Value;

        public GuidTableType(Guid? value)
        {
            Value = value;
        }

        public object[] to_array()
        {
            return new List<object>() { Value.HasValue ? Value : null }.ToArray();
        }

        public GuidTableType[] get_array(List<GuidTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<GuidTableType> getCompositeType(List<Guid> lst)
        {
            if (lst == null) lst = new List<Guid>();
            return new DBCompositeType<GuidTableType>().add(lst.Distinct().Select(i => new GuidTableType(i)).ToList());
        }
    }
}
