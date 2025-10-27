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
    public class GuidFloatTableType : ITableType
    {
        public GuidFloatTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "GuidFloatTableType"; } }

        [PgName("first_value")]
        public Guid? FirstValue;

        [PgName("second_value")]
        public double? SecondValue;

        public GuidFloatTableType(Guid? firstValue, double? secondValue)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
        }

        public object[] to_array()
        {
            return new List<object>() { FirstValue, SecondValue }.ToArray();
        }

        public GuidFloatTableType[] get_array(List<GuidFloatTableType> list)
        {
            return list.ToArray();
        }
    }
}
