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
    public class BigIntTableType : ITableType
    {
        public BigIntTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "BigIntTableType"; } }

        [PgName("value")]
        public long? Value;

        public BigIntTableType(long? value)
        {
            Value = value;
        }

        public object[] to_array()
        {
            return new List<object>() { Value.HasValue ? Value : null }.ToArray();
        }

        public BigIntTableType[] get_array(List<BigIntTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<BigIntTableType> getCompositeType(List<long> lst)
        {
            if (lst == null) lst = new List<long>();
            return new DBCompositeType<BigIntTableType>().add(lst.Distinct().Select(i => new BigIntTableType(i)).ToList());
        }
    }
}
