using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Documents
{
    public static class DCTParsers
    {
        public static List<Tree> trees(DBResultSet results)
        {
            List<Tree> retList = new List<Tree>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Tree tree = new Tree();

                tree.TreeID = table.GetGuid(i, "TreeID");
                tree.Name = table.GetString(i, "Name");
                tree.Description = table.GetString(i, "Description");
                tree.IsTemplate = table.GetBool(i, "IsTemplate");

                retList.Add(tree);
            }

            return retList;
        }

        public static List<TreeNode> tree_nodes(DBResultSet results)
        {
            List<TreeNode> retList = new List<TreeNode>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                TreeNode treeNode = new TreeNode();

                treeNode.TreeNodeID = table.GetGuid(i, "TreeNodeID");
                treeNode.TreeID = table.GetGuid(i, "TreeID");
                treeNode.ParentNodeID = table.GetGuid(i, "ParentNodeID");
                treeNode.Name = table.GetString(i, "Name");
                treeNode.HasChild = table.GetBool(i, "HasChild");

                retList.Add(treeNode);
            }

            return retList;
        }

        public static List<DocFileInfo> files(DBResultSet results, Guid? applicationId)
        {
            List<DocFileInfo> retList = new List<DocFileInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DocFileInfo file = new DocFileInfo(applicationId);

                file.OwnerID = table.GetGuid(i, "OwnerID");
                file.FileID = table.GetGuid(i, "FileID");
                file.FileName = table.GetString(i, "FileName");
                file.Extension = table.GetString(i, "Extension");
                file.Size = table.GetLong(i, "Size");
                file.OwnerType = table.GetEnum<FileOwnerTypes>(i, "OwnerType", FileOwnerTypes.None);

                retList.Add(file);
            }

            return retList;
        }

        public static List<DocFileInfo> file_owner_nodes(DBResultSet results, Guid? applicationId)
        {
            List<DocFileInfo> retList = new List<DocFileInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DocFileInfo file = new DocFileInfo(applicationId);

                file.FileID = table.GetGuid(i, "FileID");
                file.OwnerNodeID = table.GetGuid(i, "NodeID");
                file.OwnerNodeName = table.GetString(i, "Name");
                file.OwnerNodeType = table.GetString(i, "NodeType");

                retList.Add(file);
            }

            return retList;
        }
    }
}
