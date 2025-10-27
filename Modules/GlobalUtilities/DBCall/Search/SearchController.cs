using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Search
{
    public static class SearchController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[SRCH_" + name + "]"; //'[dbo].' is database owner and 'SRCH_' is module qualifier
        }

        public static List<SearchDoc> get_index_queue_items(Guid applicationId, int count, SearchDocType type)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetIndexQueueItems"),
                applicationId, count, type.ToString());

            return SearchParsers.search_docs(results, applicationId, type);
        }

        public static bool set_index_last_update_date(Guid applicationId, SearchDocType itemType, List<Guid> IDs)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetIndexLastUpdateDate"),
                applicationId, itemType.ToString(), ProviderUtil.list_to_string<Guid>(IDs), ',', DateTime.Now);
        }
    }
}
