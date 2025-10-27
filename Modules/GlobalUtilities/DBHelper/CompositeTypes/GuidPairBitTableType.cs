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
    public class GuidPairBitTableType : ITableType
    {
        public GuidPairBitTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "GuidPairBitTableType"; } }

        [PgName("first_value")]
        public Guid? FirstValue;

        [PgName("second_value")]
        public Guid? SecondValue;

        [PgName("third_value")]
        public bool? ThirdValue;

        public GuidPairBitTableType(Guid? firstValue, Guid? secondValue, bool? thirdValue)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
            ThirdValue = thirdValue;
        }

        public object[] to_array()
        {
            return new List<object>() { FirstValue, SecondValue, ThirdValue }.ToArray();
        }

        public GuidPairBitTableType[] get_array(List<GuidPairBitTableType> list)
        {
            return list.ToArray();
        }
    }
}
