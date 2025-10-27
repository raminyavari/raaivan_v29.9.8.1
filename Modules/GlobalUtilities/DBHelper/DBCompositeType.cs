using Npgsql;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public interface IDBCompositeType
    {
        NpgsqlParameter toNpgSqlParameter();

        SqlParameter toMSSQLParameter(string name);
    };

    public class DBCompositeType<T> : IDBCompositeType
    {
        public List<T> Values { get; }

        public DBCompositeType()
        {
            Values = new List<T>();
        }

        public int count
        {
            get { return Values == null ? 0 : Values.Count; }
        }

        public DBCompositeType<T> add(T value)
        {
            Values.Add(value);
            return this;
        }

        public DBCompositeType<T> add(List<T> list)
        {
            Values.AddRange(list);
            return this;
        }

        public NpgsqlParameter toNpgSqlParameter()
        {
            NpgsqlParameter p = new NpgsqlParameter();
            p.Value = Values.ToArray();
            return p;
        }

        public SqlParameter toMSSQLParameter(string name)
        {
            DataTable tbl = new DataTable();

            if (!typeof(ITableType).IsAssignableFrom(typeof(T))) return null;

            ITableType emptyInstance = ((ITableType)Activator.CreateInstance(typeof(T)));

            emptyInstance.to_array().ToList().ForEach(c => tbl.Columns.Add());

            Values.ForEach(v => tbl.Rows.Add(((ITableType)v).to_array()));

            SqlParameter tblParam = new SqlParameter(name, SqlDbType.Structured);
            tblParam.TypeName = "[dbo].[" + emptyInstance.MSSQLName + "]";
            tblParam.Value = tbl;

            return tblParam;
        }

        private static object fromJson<TP>(List<string> items)
        {
            try
            {
                if (items == null || !typeof(ITableType).IsAssignableFrom(typeof(TP))) return null;
                else
                {
                    return new DBCompositeType<TP>().add(items.Where(i => !string.IsNullOrEmpty(i))
                        .Select(i => PublicMethods.fromJSON_typed<TP>(i))
                        .Where(i => i != null).ToList());
                }
            }
            catch
            {
                return null;
            }
        }

        public static object fromJson(string typeName, List<string> items)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            switch (typeName.ToLower())
            {
                case "biginttabletype":
                    return fromJson<BigIntTableType>(items);
                case "cnextensiontabletype":
                    return fromJson<CNExtensionTableType>(items);
                case "docfileinfotabletype":
                    return fromJson<DocFileInfoTableType>(items);
                case "emailqueueitemtabletype":
                    return fromJson<EmailQueueItemTableType>(items);
                case "exchangeauthortabletype":
                    return fromJson<ExchangeAuthorTableType>(items);
                case "exchangemembertabletype":
                    return fromJson<ExchangeMemberTableType>(items);
                case "exchangenodetabletype":
                    return fromJson<ExchangeNodeTableType>(items);
                case "exchangepermissiontabletype":
                    return fromJson<ExchangePermissionTableType>(items);
                case "exchangerelationtabletype":
                    return fromJson<ExchangeRelationTableType>(items);
                case "exchangeusertabletype":
                    return fromJson<ExchangeUserTableType>(items);
                case "formelementtabletype":
                    return fromJson<FormElementTableType>(items);
                case "formfiltertabletype":
                    return fromJson<FormFilterTableType>(items);
                case "forminstancetabletype":
                    return fromJson<FormInstanceTableType>(items);
                case "guidfloattabletype":
                    return fromJson<GuidFloatTableType>(items);
                case "guidpairbittabletype":
                    return fromJson<GuidPairBitTableType>(items);
                case "guidpairtabletype":
                    return fromJson<GuidPairTableType>(items);
                case "guidstringpairtabletype":
                    return fromJson<GuidStringPairTableType>(items);
                case "guidstringtabletype":
                    return fromJson<GuidStringTableType>(items);
                case "guidtabletype":
                    return fromJson<GuidTableType>(items);
                case "messagetabletype":
                    return fromJson<MessageTableType>(items);
                case "privacyaudiencetabletype":
                    return fromJson<PrivacyAudienceTableType>(items);
                case "stringpairtabletype":
                    return fromJson<StringPairTableType>(items);
                case "stringtabletype":
                    return fromJson<StringTableType>(items);
                case "taggeditemtabletype":
                    return fromJson<TaggedItemTableType>(items);
                default:
                    return null;
            }
        }
    }
}
