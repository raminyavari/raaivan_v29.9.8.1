using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.Wiki
{
    public static class WikiParsers
    {
        public static List<WikiTitle> titles(DBResultSet results)
        {
            List<WikiTitle> retList = new List<WikiTitle>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new WikiTitle()
                {
                    TitleID = table.GetGuid(i, "TitleID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    Title = table.GetString(i, "Title"),
                    SequenceNumber = table.GetInt(i, "SequenceNumber"),
                    CreatorUserID = table.GetGuid(i, "CreatorUserID"),
                    CreationDate = table.GetDate(i, "CreationDate"),
                    LastModificationDate = table.GetDate(i, "LastModificationDate"),
                    Status = table.GetString(i, "Status")
                });
            }

            return retList;
        }

        public static List<Paragraph> paragraphs(DBResultSet results)
        {
            List<Paragraph> retList = new List<Paragraph>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Paragraph()
                {
                    ParagraphID = table.GetGuid(i, "ParagraphID"),
                    TitleID = table.GetGuid(i, "TitleID"),
                    Title = table.GetString(i, "Title"),
                    BodyText = table.GetString(i, "BodyText"),
                    SequenceNumber = table.GetInt(i, "SequenceNumber"),
                    IsRichText = table.GetBool(i, "IsRichText"),
                    CreatorUserID = table.GetGuid(i, "CreatorUserID"),
                    CreationDate = table.GetDate(i, "CreationDate"),
                    LastModificationDate = table.GetDate(i, "LastModificationDate"),
                    Status = table.GetString(i, "Status")
                });
            }

            return retList;
        }

        public static List<Change> changes(DBResultSet results)
        {
            List<Change> retList = new List<Change>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Change()
                {
                    ChangeID = table.GetGuid(i, "ChangeID"),
                    ParagraphID = table.GetGuid(i, "ParagraphID"),
                    Title = table.GetString(i, "Title"),
                    BodyText = table.GetString(i, "BodyText"),
                    Status = table.GetString(i, "Status"),
                    Applied = table.GetBool(i, "Applied"),
                    SendDate = table.GetDate(i, "SendDate"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    }
                });
            }

            return retList;
        }

        public static void wiki_owner(DBResultSet results, ref Guid? ownerId, ref WikiOwnerType ownerType)
        {
            RVDataTable table = results.get_table();

            ownerId = table.GetGuid(0, "OwnerID");
            ownerType = table.GetEnum<WikiOwnerType>(0, "OwnerType", WikiOwnerType.NotSet);
        }
    }
}
