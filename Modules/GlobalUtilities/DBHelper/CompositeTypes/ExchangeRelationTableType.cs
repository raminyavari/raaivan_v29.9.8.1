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
    public class ExchangeRelationTableType : ITableType
    {
        public ExchangeRelationTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangeRelationTableType"; } }

        [PgName("source_type_additional_id")]
        public string SourceTypeAdditionalID;

        [PgName("source_additional_id")]
        public string SourceAdditionalID;

        [PgName("source_id")]
        public Guid? SourceID;

        [PgName("destination_type_additional_id")]
        public string DestinationTypeAdditionalID;

        [PgName("destination_additional_id")]
        public string DestinationAdditionalID;

        [PgName("destination_id")]
        public Guid? DestinationID;

        [PgName("bidirectional")]
        public bool? Bidirectional;

        public ExchangeRelationTableType(string sourceTypeAdditionalId, string sourceAdditionalId, Guid? sourceId,
            string destinationTypeAdditionalId, string destinationAdditionalId, Guid? destinationId, bool? bidirectional)
        {
            SourceTypeAdditionalID = sourceTypeAdditionalId;
            SourceAdditionalID = sourceAdditionalId;
            SourceID = sourceId;
            DestinationTypeAdditionalID = destinationTypeAdditionalId;
            DestinationAdditionalID = destinationAdditionalId;
            DestinationID = destinationId;
            Bidirectional = bidirectional;
        }

        public object[] to_array()
        {
            return new List<object>() {
                SourceTypeAdditionalID,
                SourceAdditionalID,
                SourceID,
                DestinationTypeAdditionalID,
                DestinationAdditionalID,
                DestinationID,
                Bidirectional
            }.ToArray();
        }

        public ExchangeRelationTableType[] get_array(List<ExchangeRelationTableType> list)
        {
            return list.ToArray();
        }
    }
}
