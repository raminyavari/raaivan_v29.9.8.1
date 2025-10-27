using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.Documents
{
    public class DocumentsController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[DCT_" + name + "]"; //'[dbo].' is database owner and 'DCT_' is module qualifier
        }

        public static bool create_tree(Guid applicationId, Tree info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CreateTree"),
                applicationId, info.TreeID, info.IsPrivate, info.OwnerID, info.Name, info.Description,
                info.CreatorUserID, DateTime.Now, info.Privacy, info.IsTemplate);
        }

        public static bool change_tree(Guid applicationId, Tree info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ChangeTree"),
                applicationId, info.TreeID, info.Name, info.Description, info.LastModifierUserID, DateTime.Now, info.IsTemplate);
        }

        public static bool remove_trees(Guid applicationId, List<Guid> treeIds, Guid? ownerId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteTree"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeIds), ',', ownerId, currentUserId, DateTime.Now);
        }

        public static bool remove_tree(Guid applicationId, Guid treeId, Guid? ownerId, Guid currentUserId)
        {
            return remove_trees(applicationId, new List<Guid>() { treeId }, ownerId, currentUserId);
        }

        public static bool recycle_tree(Guid applicationId, Guid treeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleTree"),
                applicationId, treeId, currentUserId, DateTime.Now);
        }

        public static List<Tree> get_trees(Guid applicationId, List<Guid> treeIds)
        {
            return DCTParsers.trees(DBConnector.read(applicationId, GetFullyQualifiedName("GetTreesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeIds), ','));
        }

        public static Tree get_tree(Guid applicationId, Guid treeIdOrTreeNodeId)
        {
            return get_trees(applicationId, new List<Guid>() { treeIdOrTreeNodeId }).FirstOrDefault();
        }

        public static List<Tree> get_trees(Guid applicationId, Guid? ownerId = null, bool? archive = null)
        {
            return DCTParsers.trees(DBConnector.read(applicationId, GetFullyQualifiedName("GetTrees"), applicationId, ownerId, archive));
        }

        public static bool add_tree_node(Guid applicationId, TreeNode info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddTreeNode"),
                applicationId, info.TreeNodeID, info.TreeID, info.ParentNodeID, info.Name, 
                info.Description, info.CreatorUserID, DateTime.Now, info.Privacy);
        }

        public static bool change_tree_node(Guid applicationId, TreeNode info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ChangeTreeNode"),
                 applicationId, info.TreeNodeID, info.Name, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static List<Guid> copy_trees_or_tree_nodes(Guid applicationId, 
            Guid treeIdOrTreeNodeId, List<Guid> copiedIds, Guid currentUserId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("CopyTreesOrTreeNodes"),
                applicationId, treeIdOrTreeNodeId, ProviderUtil.list_to_string<Guid>(copiedIds), ',', currentUserId, DateTime.Now);
        }

        public static List<Guid> move_trees_or_tree_nodes(Guid applicationId, Guid treeIdOrTreeNodeId,
            List<Guid> movedIds, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.get_guid_list(applicationId, ref errorMessage, GetFullyQualifiedName("MoveTreesOrTreeNodes"),
                applicationId, treeIdOrTreeNodeId, ProviderUtil.list_to_string<Guid>(movedIds), ',', currentUserId, DateTime.Now);
        }

        public static bool move_tree_node(Guid applicationId, List<Guid> treeNodeIds, Guid? parentTreeNodeId, 
            Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("MoveTreeNode"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeNodeIds), ',', parentTreeNodeId, currentUserId, DateTime.Now);
        }

        public static bool remove_tree_node(Guid applicationId, 
            List<Guid> treeNodeIds, Guid? treeOwnerId, bool? removeHierarchy, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteTreeNode"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeNodeIds), ',', 
                treeOwnerId, removeHierarchy, currentUserId, DateTime.Now);
        }

        public static List<TreeNode> get_tree_nodes(Guid applicationId, Guid treeId)
        {
            return DCTParsers.tree_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetTreeNodes"), applicationId, treeId));
        }

        public static List<TreeNode> get_tree_nodes(Guid applicationId, List<Guid> treeNodeIds)
        {
            return DCTParsers.tree_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetTreeNodesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeNodeIds), ','));
        }

        public static TreeNode get_tree_node(Guid applicationId, Guid treeNodeId)
        {
            return get_tree_nodes(applicationId, new List<Guid>() { treeNodeId }).FirstOrDefault();
        }

        public static List<TreeNode> get_root_nodes(Guid applicationId, Guid treeId)
        {
            return DCTParsers.tree_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetRootNodes"), applicationId, treeId));
        }

        public static List<TreeNode> get_child_nodes(Guid applicationId, Guid? parentNodeId, 
            Guid? treeId = null, string searchText = null)
        {
            if ((!parentNodeId.HasValue || parentNodeId == Guid.Empty) && (!treeId.HasValue || treeId == Guid.Empty))
                return new List<TreeNode>();

            return DCTParsers.tree_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetChildNodes"),
                applicationId, parentNodeId, treeId, ProviderUtil.get_search_text(searchText)));
        }

        public static TreeNode get_parent_node(Guid applicationId, Guid treeNodeId)
        {
            return DCTParsers.tree_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetParentNode"),
                applicationId, treeNodeId)).FirstOrDefault();
        }

        public static bool add_files(Guid applicationId, 
            Guid ownerId, FileOwnerTypes ownerType, List<DocFileInfo> attachments, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddFiles"), applicationId, ownerId, 
                ownerType.ToString(), DocFileInfoTableType.getCompositeType(attachments), currentUserId, DateTime.Now);
        }

        public static bool add_file(Guid applicationId, Guid ownerId, 
            FileOwnerTypes ownerType, DocFileInfo attachment, Guid currentUserId)
        {
            return add_files(applicationId, ownerId, ownerType, new List<DocFileInfo>() { attachment }, currentUserId);
        }

        public static List<DocFileInfo> get_owner_files(Guid applicationId, 
            List<Guid> ownerIds, FileOwnerTypes ownerType = FileOwnerTypes.None)
        {
            if (ownerIds.Count == 0) return new List<DocFileInfo>();

            string strOwnerType = null;
            if (ownerType != FileOwnerTypes.None) strOwnerType = ownerType.ToString();

            return DCTParsers.files(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerFiles"),
                applicationId, ProviderUtil.list_to_string<Guid>(ownerIds), ',', strOwnerType), applicationId);
        }

        public static List<DocFileInfo> get_owner_files(Guid applicationId, 
            Guid ownerId, FileOwnerTypes ownerType = FileOwnerTypes.None)
        {
            return get_owner_files(applicationId, new List<Guid>() { ownerId }, ownerType);
        }

        public static List<DocFileInfo> get_files(Guid applicationId, List<Guid> fileIds)
        {
            return DCTParsers.files(DBConnector.read(applicationId, GetFullyQualifiedName("GetFilesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(fileIds), ','), applicationId);
        }

        public static DocFileInfo get_file(Guid applicationId, Guid fileId)
        {
            return get_files(applicationId, new List<Guid>() { fileId }).FirstOrDefault();
        }

        public static List<DocFileInfo> get_file_owner_nodes(Guid applicationId, List<Guid> fileIds)
        {
            return DCTParsers.file_owner_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetFileOwnerNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(fileIds), ','), applicationId);
        }

        public static DocFileInfo get_file_owner_node(Guid applicationId, Guid fileId)
        {
            return get_file_owner_nodes(applicationId, new List<Guid> { fileId }).FirstOrDefault();
        }

        public static bool rename_file(Guid applicationId, Guid fileId, string name)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RenameFile"), applicationId, fileId, name);
        }

        private static bool _remove_files(Guid applicationId, Guid? ownerId, List<Guid> fileIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteFiles"),
                applicationId, ownerId, ProviderUtil.list_to_string<Guid>(fileIds), ',');
        }

        public static bool remove_files(Guid applicationId, Guid ownerId, ref List<Guid> fileIds)
        {
            return _remove_files(applicationId, ownerId, fileIds);
        }

        public static bool remove_files(Guid applicationId, List<Guid> fileIds)
        {
            return _remove_files(applicationId, ownerId: null, fileIds);
        }

        public static bool remove_file(Guid applicationId, Guid ownerId, Guid fileId)
        {
            return _remove_files(applicationId, ownerId, new List<Guid>() { fileId });
        }

        public static bool remove_file(Guid applicationId, Guid fileId)
        {
            return remove_files(applicationId, new List<Guid>() { fileId });
        }

        public static bool copy_file(Guid applicationId, Guid ownerId, Guid fileId, 
            FileOwnerTypes ownerType, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CopyFile"),
                applicationId, ownerId, fileId, ownerType.ToString(), currentUserId, DateTime.Now);
        }

        public static List<Hierarchy> get_tree_node_hierarchy(Guid applicationId, Guid treeNodeId)
        {
            return DBConnector.get_hierarchy(applicationId, GetFullyQualifiedName("GetTreeNodeHierarchy"), applicationId, treeNodeId);
        }

        //get not extracted file from DB
        public static List<DocFileInfo> get_not_extracted_files(Guid applicationId, 
            string allowedExtractions, char delimiter, int? count)
        {
            return DCTParsers.files(DBConnector.read(applicationId, GetFullyQualifiedName("GetNotExtractedFiles"),
                applicationId, allowedExtractions, delimiter, count), applicationId);
        }

        //save extracted file content in DB
        public static bool save_file_content(Guid applicationId, Guid FileID, string Content, 
            bool NotExtractable, bool fileNotFound, double duration, string errorText)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveFileContent"),
                applicationId, FileID, Content, NotExtractable, fileNotFound, duration, DateTime.Now, errorText);
        }

        public static bool set_tree_nodes_order(Guid applicationId, List<Guid> treeNodeIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetTreeNodesOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeNodeIds), ',');
        }

        public static bool is_private_tree(Guid applicationId, Guid treeIdOrTreeNodeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsPrivateTree"), applicationId, treeIdOrTreeNodeId);
        }

        public static bool add_owner_tree(Guid applicationId, Guid ownerId, Guid treeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddOwnerTree"),
                applicationId, ownerId, treeId, currentUserId, DateTime.Now);
        }

        public static bool remove_owner_tree(Guid applicationId, Guid ownerId, Guid treeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteOwnerTree"),
                applicationId, ownerId, treeId, currentUserId, DateTime.Now);
        }

        public static Guid? get_tree_owner_id(Guid applicationId, Guid treeIdOrTreeNodeId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetTreeOwnerID"), applicationId, treeIdOrTreeNodeId);
        }

        public static List<Tree> get_owner_trees(Guid applicationId, Guid ownerId)
        {
            return DCTParsers.trees(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerTrees"), applicationId, ownerId));
        }

        public static List<Tree> clone_trees(Guid applicationId, 
            List<Guid> treeIds, Guid? ownerId, bool? allowMultiple, Guid currentUserId)
        {
            return DCTParsers.trees(DBConnector.read(applicationId, GetFullyQualifiedName("CloneTrees"),
                applicationId, ProviderUtil.list_to_string<Guid>(treeIds), ',', ownerId, allowMultiple, currentUserId, DateTime.Now));
        }

        public static bool add_tree_node_contents(Guid applicationId,
            Guid treeNodeId, List<Guid> nodeIds, Guid? removeFrom, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddTreeNodeContents"),
                applicationId, treeNodeId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', removeFrom, currentUserId, DateTime.Now);
        }

        public static bool remove_tree_node_contents(Guid applicationId,
            Guid treeNodeId, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveTreeNodeContents"),
                applicationId, treeNodeId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }
    }
}
