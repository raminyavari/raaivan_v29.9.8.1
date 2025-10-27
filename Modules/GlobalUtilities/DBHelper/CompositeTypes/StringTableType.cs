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
    public class StringTableType : ITableType
    {
        public StringTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "StringTableType"; } }

        [PgName("value")]
        public string Value;

        public StringTableType(string value)
        {
            Value = value;
        }

        public object[] to_array()
        {
            return new List<object>() { Value }.ToArray();
        }

        public StringTableType[] get_array(List<StringTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<StringTableType> getCompositeType(List<string> lst)
        {
            if (lst == null) lst = new List<string>();
            return new DBCompositeType<StringTableType>().add(lst.Distinct().Select(i => new StringTableType(i)).ToList());
        }
    }
}
