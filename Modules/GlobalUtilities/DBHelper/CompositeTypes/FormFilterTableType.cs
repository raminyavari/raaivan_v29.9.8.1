using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class FormFilterTableType : ITableType
    {
        public FormFilterTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "FormFilterTableType"; } }

        [PgName("element_id")]
        public Guid? ElementID;

        [PgName("owner_id")]
        public Guid? OwnerID;

        [PgName("text")]
        public string Text;

        [PgName("text_items")]
        public string TextItems;

        [PgName("or")]
        public bool? Or;

        [PgName("exact")]
        public bool? Exact;

        [PgName("date_from")]
        public DateTime? DateFrom;

        [PgName("date_to")]
        public DateTime? DateTo;

        [PgName("float_from")]
        public double? FloatFrom;

        [PgName("float_to")]
        public double? FloatTo;

        [PgName("bit")]
        public bool? Bit;

        [PgName("guid")]
        public Guid? Guid;

        [PgName("guid_items")]
        public string GuidItems;

        [PgName("compulsory")]
        public bool? Compulsory;

        public FormFilterTableType(Guid? elementId, Guid? ownerId, string text, string textItems,
            bool? or, bool? exact, DateTime? dateFrom, DateTime? dateTo, double? floatFrom, double? floatTo,
            bool? bit, Guid? guid, string guidItems, bool? compulsory)
        {
            ElementID = elementId;
            OwnerID = ownerId;
            Text = text;
            TextItems = textItems;
            Or = or;
            Exact = exact;
            DateFrom = dateFrom;
            DateTo = dateTo;
            FloatFrom = floatFrom;
            FloatTo = floatTo;
            Bit = bit;
            Guid = guid;
            GuidItems = guidItems;
            Compulsory = compulsory;
        }

        public object[] to_array()
        {
            return new List<object>() {
                ElementID.HasValue ? ElementID : null,
                OwnerID.HasValue ? OwnerID : null,
                Text,
                TextItems,
                Or.HasValue ? Or : null,
                Exact.HasValue ? Exact : null,
                DateFrom.HasValue ? DateFrom : null,
                DateTo.HasValue ? DateTo : null,
                FloatFrom.HasValue ? FloatFrom : null,
                FloatTo.HasValue? FloatTo: null,
                Bit.HasValue ? Bit : null,
                Guid.HasValue ? Guid : null,
                GuidItems,
                Compulsory.HasValue ? Compulsory : null
            }.ToArray();
        }

        public FormFilterTableType[] get_array(List<FormFilterTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<FormFilterTableType> getCompositeType(List<FormFilter> lst)
        {
            if (lst == null) lst = new List<FormFilter>();

            return new DBCompositeType<FormFilterTableType>()
                .add(lst.Select(f => new FormFilterTableType(
                    elementId: f.ElementID,
                    ownerId: f.OwnerID,
                    text: f.Text,
                    textItems: ProviderUtil.list_to_string<string>(f.TextItems),
                    or: f.Or,
                    exact: f.Exact,
                    dateFrom: f.DateFrom,
                    dateTo: f.DateTo,
                    floatFrom: f.FloatFrom,
                    floatTo: f.FloatTo,
                    bit: f.Bit,
                    guid: f.Guid,
                    guidItems: ProviderUtil.list_to_string<Guid>(f.GuidItems),
                    compulsory: f.Compulsory)).ToList());
        }
    }
}
