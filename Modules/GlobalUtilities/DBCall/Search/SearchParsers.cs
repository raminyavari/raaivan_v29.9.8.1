using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Search
{
    public static class SearchParsers
    {
        public static List<SearchDoc> search_docs(DBResultSet results, Guid applicationId, SearchDocType itemType)
        {
            List<SearchDoc> retList = new List<SearchDoc>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                SearchDoc sd = new SearchDoc(applicationId)
                {
                    ID = table.GetGuid(i, "ID", defaultValue: Guid.Empty).Value,
                    Deleted = table.GetBool(i, "Deleted", defaultValue: false).Value,
                    Title = table.GetString(i, "Title")
                };

                switch (itemType)
                {
                    case SearchDocType.Node:
                        sd.TypeID = table.GetGuid(i, "TypeID");
                        sd.Type = table.GetString(i, "Type");
                        sd.AdditionalID = table.GetString(i, "AdditionalID");
                        sd.Description = table.GetString(i, "Description");
                        sd.Tags = table.GetString(i, "Tags");
                        sd.Content = table.GetString(i, "Content");
                        sd.FileContent = table.GetString(i, "FileContent");
                        sd.SearchDocType = SearchDocType.Node;
                        break;
                    case SearchDocType.NodeType:
                        sd.Description = table.GetString(i, "Description");
                        sd.SearchDocType = SearchDocType.NodeType;
                        break;
                    case SearchDocType.Question:
                        sd.Description = table.GetString(i, "Description");
                        sd.Content = table.GetString(i, "Content");
                        sd.SearchDocType = SearchDocType.Question;
                        break;
                    case SearchDocType.File:
                        sd.Type = table.GetString(i, "Type");
                        sd.FileContent = table.GetString(i, "FileContent");
                        sd.SearchDocType = SearchDocType.File;
                        break;
                    case SearchDocType.User:
                        sd.AdditionalID = table.GetString(i, "AdditionalID");
                        sd.SearchDocType = SearchDocType.User;
                        break;
                }

                if (!string.IsNullOrEmpty(sd.Description)) sd.Description =
                        PublicMethods.markup2plaintext(applicationId,
                    Expressions.replace(sd.Description, Expressions.Patterns.HTMLTag, " "));
                if (!string.IsNullOrEmpty(sd.Content)) sd.Content =
                        PublicMethods.markup2plaintext(applicationId,
                    Expressions.replace(sd.Content, Expressions.Patterns.HTMLTag, " "));

                retList.Add(sd);
            }

            return retList;
        }
    }
}
