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
    public class GuidPairTableType : ITableType
    {
        public GuidPairTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "GuidPairTableType"; } }

        [PgName("first_value")]
        public Guid? FirstValue;

        [PgName("second_value")]
        public Guid? SecondValue;

        public GuidPairTableType(Guid? firstValue, Guid? secondValue)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
        }

        public object[] to_array()
        {
            return new List<object>() { FirstValue, SecondValue }.ToArray();
        }

        public GuidPairTableType[] get_array(List<GuidPairTableType> list)
        {
            return list.ToArray();
        }
    }
}
