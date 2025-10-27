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
    public class CNExtensionTableType : ITableType
    {
        public CNExtensionTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "CNExtensionTableType"; } }

        [PgName("owner_id")]
        public Guid? OwnerID;

        [PgName("extension")]
        public string Extension;

        [PgName("title")]
        public string Title;

        [PgName("sequence_number")]
        public int? SequenceNumber;

        [PgName("disabled")]
        public bool? Disabled;

        public CNExtensionTableType(Guid? ownerId, string extension, string title, int? sequenceNumber, bool? disabled)
        {
            OwnerID = ownerId;
            Extension = extension;
            Title = title;
            SequenceNumber = sequenceNumber;
            Disabled = disabled;
        }

        public object[] to_array()
        {
            return new List<object>() {
                OwnerID.HasValue ? OwnerID : null,
                Extension,
                Title,
                SequenceNumber.HasValue ? SequenceNumber : null,
                Disabled.HasValue ? Disabled : null
            }.ToArray();
        }

        public CNExtensionTableType[] get_array(List<CNExtensionTableType> list)
        {
            return list.ToArray();
        }
    }
}
