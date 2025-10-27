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
    public class GuidStringPairTableType : ITableType
    {
        public GuidStringPairTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "GuidStringPairTableType"; } }

        [PgName("first_value")]
        public Guid? FirstValue;

        [PgName("second_value")]
        public string SecondValue;

        [PgName("third_value")]
        public string ThirdValue;

        public GuidStringPairTableType(Guid? firstValue, string secondValue, string thirdValue)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
            ThirdValue = thirdValue;
        }

        public object[] to_array()
        {
            return new List<object>() { FirstValue, SecondValue, ThirdValue }.ToArray();
        }

        public GuidStringPairTableType[] get_array(List<GuidStringPairTableType> list)
        {
            return list.ToArray();
        }
    }
}
