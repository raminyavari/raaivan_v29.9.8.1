using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.Wiki
{
    public static class WikiController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[WK_" + name + "]"; //'[dbo].' is database owner and 'WK_' is module qualifier
        }

        public static bool add_title(Guid applicationId, WikiTitle info, bool? accept)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddTitle"),
                applicationId, info.TitleID, info.OwnerID, info.Title, info.SequenceNumber, info.CreatorUserID, 
                DateTime.Now, info.OwnerType, accept);
        }

        public static bool modify_title(Guid applicationId, WikiTitle info, bool? accept)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyTitle"),
                applicationId, info.TitleID, info.Title, info.LastModifierUserID, DateTime.Now, accept);
        }

        public static bool remove_title(Guid applicationId, Guid titleId, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteTitle"),
                applicationId, titleId, lastModifierUserId, DateTime.Now);
        }

        public static bool recycle_title(Guid applicationId, Guid titleId, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleTitle"),
                applicationId, titleId, lastModifierUserId, DateTime.Now);
        }

        public static bool set_titles_order(Guid applicationId, List<Guid> titleIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetTitlesOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(titleIds), ',');
        }

        public static List<WikiTitle> get_titles(Guid applicationId, 
            Guid ownerId, bool? isAdmin, Guid? currentUserId, bool deleted = false)
        {
            return WikiParsers.titles(DBConnector.read(applicationId, GetFullyQualifiedName("GetTitles"),
                 applicationId, ownerId, isAdmin, currentUserId, deleted));
        }

        public static List<WikiTitle> get_titles(Guid applicationId, List<Guid> titleIds, Guid currentUserId)
        {
            return WikiParsers.titles(DBConnector.read(applicationId, GetFullyQualifiedName("GetTitlesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(titleIds), ',', currentUserId));
        }

        public static WikiTitle get_title(Guid applicationId, Guid titleId, Guid currentUserId)
        {
            return get_titles(applicationId, new List<Guid>() { titleId }, currentUserId).FirstOrDefault();
        }

        public static bool has_title(Guid applicationId, Guid ownerId, Guid? currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasTitle"), applicationId, ownerId, currentUserId);
        }

        public static bool add_paragraph(Guid applicationId, Paragraph info, bool? sendToAdmins, bool? hasAdmin, 
            List<Guid> adminUserIds, ref List<Dashboard> dashboards)
        {
            if (string.IsNullOrEmpty(info.Title)) info.Title = null;
            if (string.IsNullOrEmpty(info.BodyText)) info.BodyText = string.Empty;

            return DBConnector.get_dashboards(applicationId, ref dashboards, GetFullyQualifiedName("AddParagraph"),
                applicationId, info.ParagraphID, info.TitleID, info.Title, info.BodyText, info.SequenceNumber,
                info.CreatorUserID, DateTime.Now, info.IsRichText, sendToAdmins, hasAdmin, 
                GuidTableType.getCompositeType(adminUserIds)) > 0;
        }

        public static bool modify_paragraph(Guid applicationId, Paragraph info, Guid? changeId2Accept, 
            bool? hasAdmin, List<Guid> adminUserIds, ref List<Dashboard> dashboards, 
            bool? citationNeeded = null, bool? apply = null, bool? accept = null)
        {
            if (string.IsNullOrEmpty(info.Title)) info.Title = null;
            if (string.IsNullOrEmpty(info.BodyText)) info.BodyText = string.Empty;

            return DBConnector.get_dashboards(applicationId, ref dashboards, GetFullyQualifiedName("ModifyParagraph"),
                applicationId, info.ParagraphID, changeId2Accept, info.Title, info.BodyText, info.LastModifierUserID, 
                DateTime.Now, citationNeeded, apply, accept, hasAdmin, GuidTableType.getCompositeType(adminUserIds)) > 0;
        }

        public static bool remove_paragraph(Guid applicationId, Guid paragraphId, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteParagraph"),
                applicationId, paragraphId, lastModifierUserId, DateTime.Now);
        }

        public static bool recycle_paragraph(Guid applicationId, Guid paragraphId, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleParagraph"),
                applicationId, paragraphId, lastModifierUserId, DateTime.Now);
        }

        public static bool set_paragraphs_order(Guid applicationId, List<Guid> paragraphIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetParagraphsOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(paragraphIds), ',');
        }

        public static List<Paragraph> get_paragraphs(Guid applicationId, List<Guid> paragraphIds, Guid currentUserId)
        {
            return WikiParsers.paragraphs(DBConnector.read(applicationId, GetFullyQualifiedName("GetParagraphsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(paragraphIds), ',', currentUserId));
        }

        public static Paragraph get_paragraph(Guid applicationId, Guid paragraphId, Guid currentUserId)
        {
            return get_paragraphs(applicationId, new List<Guid>() { paragraphId }, currentUserId).FirstOrDefault();
        }

        public static List<Paragraph> get_title_paragraphs(Guid applicationId, List<Guid> titleIds, 
            bool? isAdmin, Guid? currentUserId, bool removed)
        {
            return WikiParsers.paragraphs(DBConnector.read(applicationId, GetFullyQualifiedName("GetParagraphs"),
                applicationId, ProviderUtil.list_to_string<Guid>(titleIds), ',', isAdmin, currentUserId, removed));
        }

        public static List<Paragraph> get_title_paragraphs(Guid applicationId, 
            Guid titleId, bool? isAdmin, Guid? currentUserId, bool removed)
        {
            return get_title_paragraphs(applicationId, new List<Guid>() { titleId }, isAdmin, currentUserId, removed);
        }

        public static bool has_paragraph(Guid applicationId, Guid titleOrOwnerId, Guid? currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasParagraph"),
                applicationId, titleOrOwnerId, currentUserId);
        }

        public static List<Paragraph> get_dashboard_paragraphs(Guid applicationId, Guid userId)
        {
            return WikiParsers.paragraphs(DBConnector.read(applicationId, GetFullyQualifiedName("GetDashboardParagraphs"),
                applicationId, userId));
        }

        public static List<User> get_paragraph_related_users(Guid applicationId, Guid paragraphId)
        {
            List<Guid> userIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetParagraphRelatedUserIDs"),
                applicationId, paragraphId);
            return UsersController.get_users(applicationId, userIds);
        }

        public static bool reject_change(Guid applicationId, Guid changeId, Guid evaluatorUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RejectChange"),
                applicationId, changeId, evaluatorUserId, DateTime.Now);
        }

        public static bool accept_change(Guid applicationId, Guid changeId, Guid evaluatorUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AcceptChange"),
                applicationId, changeId, evaluatorUserId, DateTime.Now);
        }

        public static bool remove_change(Guid applicationId, Guid changeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteChange"), applicationId, changeId);
        }

        public static List<Change> get_changes(Guid applicationId, List<Guid> changeIds)
        {
            return WikiParsers.changes(DBConnector.read(applicationId, GetFullyQualifiedName("GetChangesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(changeIds), ','));
        }

        public static Change get_change(Guid applicationId, Guid changeId)
        {
            return get_changes(applicationId, new List<Guid>() { changeId }).FirstOrDefault();
        }

        public static List<Change> get_changes(Guid applicationId, List<Guid> paragraphIds, 
            Guid? creatorUserId,  WikiStatuses? status, bool? applied)
        {
            string strStatus = null;
            if (status.HasValue) strStatus = status.ToString();

            return WikiParsers.changes(DBConnector.read(applicationId, GetFullyQualifiedName("GetParagraphChanges"),
                applicationId, ProviderUtil.list_to_string<Guid>(paragraphIds), ',', creatorUserId, strStatus, applied));
        }

        public static List<Change> get_changes(Guid applicationId, Guid paragraphId, Guid? creatorUserId,
            WikiStatuses? status, bool? applied)
        {
            return get_changes(applicationId, new List<Guid>() { paragraphId }, creatorUserId, status, applied);
        }

        public static Change get_last_pending_change(Guid applicationId, Guid paragraphId, Guid userId)
        {
            return WikiParsers.changes(DBConnector.read(applicationId, GetFullyQualifiedName("GetLastPendingChange"),
                applicationId, paragraphId, userId)).FirstOrDefault();
        }

        public static List<Guid> get_changed_wiki_owner_ids(Guid applicationId, List<Guid> ownerIds)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetChangedWikiOwnerIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(ownerIds), ',');
        }

        private static void _get_wiki_owner(Guid applicationId, Guid id, ref Guid? ownerId, ref WikiOwnerType ownerType)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetWikiOwner"), applicationId, id);
            WikiParsers.wiki_owner(results, ref ownerId, ref ownerType);
        }

        public static Guid get_wiki_owner(Guid applicationId, Guid id)
        {
            Guid? ownerId = null;
            WikiOwnerType ownerType = WikiOwnerType.NotSet;
            _get_wiki_owner(applicationId, id, ref ownerId, ref ownerType);

            if (!ownerId.HasValue) return Guid.Empty;
            else return ownerId.Value;
        }

        public static string get_wiki_content(Guid applicationId, Guid ownerId)
        {
            return DBConnector.get_string(applicationId, GetFullyQualifiedName("GetWikiContent"), applicationId, ownerId);
        }

        public static int get_titles_count(Guid applicationId, 
            Guid ownerId, bool? isAdmin, Guid? currentUserId, bool? removed)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetTitlesCount"),
                applicationId, ownerId, isAdmin, currentUserId, removed);
        }

        public static Dictionary<Guid, int> get_paragraphs_count(Guid applicationId, List<Guid> titleIds,
            bool? isAdmin, Guid? currentUserId, bool? removed)
        {
            return DBConnector.get_items_count(applicationId, GetFullyQualifiedName("GetParagraphsCount"),
                applicationId, ProviderUtil.list_to_string<Guid>(titleIds), ',', isAdmin, currentUserId, removed);
        }

        public static int get_paragraphs_count(Guid applicationId, 
            Guid titleId, bool? isAdmin, Guid? currentUserId, bool? removed)
        {
            Dictionary<Guid, int> dic = get_paragraphs_count(applicationId,
                new List<Guid>() { titleId }, isAdmin, currentUserId, removed);
            return dic.ContainsKey(titleId) ? dic[titleId] : 0;
        }

        public static Dictionary<Guid, int> get_changes_count(Guid applicationId, List<Guid> paragraphIds, bool? applied)
        {
            return DBConnector.get_items_count(applicationId, GetFullyQualifiedName("GetChangesCount"),
                applicationId, ProviderUtil.list_to_string<Guid>(paragraphIds), ',', applied);
        }

        public static int get_changes_count(Guid applicationId, Guid paragraphId, bool? applied)
        {
            Dictionary<Guid, int> dic = get_changes_count(applicationId, new List<Guid>() { paragraphId }, applied);
            return dic.ContainsKey(paragraphId) ? dic[paragraphId] : 0;
        }

        public static DateTime? last_modification_date(Guid applicationId, Guid ownerId)
        {
            return DBConnector.get_date(applicationId, GetFullyQualifiedName("LastModificationDate"), applicationId, ownerId);
        }

        public static List<KeyValuePair<Guid, int>> wiki_authors(Guid applicationId, Guid ownerId)
        {
            return DBConnector.get_items_count_list(applicationId, GetFullyQualifiedName("WikiAuthors"), applicationId, ownerId);
        }
    }
}
