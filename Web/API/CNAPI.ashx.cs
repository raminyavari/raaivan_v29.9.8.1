using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data;
using System.Drawing;
using QRCoder;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Knowledge;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Messaging;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.DataExchange;
using System.Drawing.Imaging;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for ManageNodes
    /// </summary>
    public class CNAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public ParamsContainer paramsContainer = null;

        //to be removed
        /*
        protected static bool FilesMoved = false;

        public class Ramin {
            public string Code;
            public string FileName;
            public string Extension;
            public string Name;
            public Guid? GuidName;
        }

        protected List<Ramin> _parse(ref System.Data.IDataReader reader) {
            List<Ramin> lst = new List<Ajax.ManageNodes.Ramin>();

            while (reader.Read())
            {
                try
                {
                    Ramin node = new Ajax.ManageNodes.Ramin();

                    if (!string.IsNullOrEmpty(reader["Code"].ToString())) node.Code = ((double)reader["Code"]).ToString();
                    if (!string.IsNullOrEmpty(reader["FileName"].ToString())) node.FileName = (string)reader["FileName"];
                    if (!string.IsNullOrEmpty(reader["Extension"].ToString())) node.Extension = (string)reader["Extension"];
                    if (!string.IsNullOrEmpty(reader["Name"].ToString())) node.Name = (string)reader["Name"];
                    if (!string.IsNullOrEmpty(reader["FileNameGuid"].ToString())) node.GuidName = (Guid)reader["FileNameGuid"];

                    lst.Add(node);
                }
                catch (Exception ex){ string strEx = ex.ToString(); }
            }
            
            if (!reader.IsClosed) reader.Close();

            return lst;
        }

        protected void _do_files()
        {
            try
            {
                List<Ramin> lst = new List<Ramin>();

                System.Data.SqlClient.SqlConnection con =
                    new System.Data.SqlClient.SqlConnection(ProviderUtil.ConnectionString);
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = "SELECT * FROM [dbo].[AAFiles] WHERE [Size] IS NULL";

                con.Open();
                try
                {
                    System.Data.IDataReader reader = (System.Data.IDataReader)cmd.ExecuteReader();
                    lst = _parse(ref reader);
                }
                catch (Exception ex) { }
                finally { con.Close(); }

                string sourceFolder = PublicMethods.map_path("~/files");

                List<System.IO.FileInfo> files = (new System.IO.DirectoryInfo(sourceFolder)).GetFiles().ToList();

                foreach (Ramin r in lst)
                {
                    if (string.IsNullOrEmpty(r.Extension) || !r.GuidName.HasValue || string.IsNullOrEmpty(r.Code)) continue;

                    System.IO.FileInfo f = files.Where(
                        u => System.IO.Path.GetFileNameWithoutExtension(u.FullName).Trim().ToLower() == r.Name.ToLower() && 
                        u.Extension.ToLower() == ("." + r.Extension).ToLower()).FirstOrDefault();

                    if (f == null) continue;

                    cmd.CommandText = "Update [dbo].[AAFiles] SET [Size] = N'" + f.Length + "' WHERE Code = N'" + r.Code + "'";

                    con.Open();
                    try
                    {
                        System.Data.IDataReader reader = (System.Data.IDataReader)cmd.ExecuteReader();
                        lst = _parse(ref reader);
                    }
                    catch (Exception ex) { }
                    finally { con.Close(); }

                    string destFolder = PublicMethods.map_path("~/App_Data/new_files/" +
                        DocumentUtilities.get_sub_folder(r.GuidName.Value));

                    if (!System.IO.Directory.Exists(destFolder)) System.IO.Directory.CreateDirectory(destFolder);

                    f.MoveTo(destFolder + "\\" + r.GuidName.ToString() + "." + r.Extension);
                }
            }
            catch { }
        }
        */
        //end of to be removed

        //to be removed
        /*
        private class FL
        {
            public Guid ApplicationID;
            public string OwnerType;
            public Guid GuidName;
            public string Extension;

            public void copy() {
                FileOwnerTypes oType = FileOwnerTypes.None;
                if (!Enum.TryParse<FileOwnerTypes>(OwnerType, out oType) || oType == FileOwnerTypes.None) return;
                FolderNames folderName = DocumentUtilities.get_folder_name(oType);
                string subFolder = !DocumentUtilities.has_sub_folder(folderName) ? string.Empty :
                    "\\" + DocumentUtilities.get_sub_folder(GuidName.ToString());
                string folderPath = DocumentUtilities.map_path(ApplicationID, folderName) + subFolder;
                string fileAddress = folderPath + "\\" + GuidName.ToString() + "." + Extension;
                if (!System.IO.File.Exists(fileAddress)) return;

                string newFolder = folderPath.ToLower().Replace("app_data", "new_app_data");
                if (!System.IO.Directory.Exists(newFolder)) System.IO.Directory.CreateDirectory(newFolder);

                string newFileAddr = newFolder + "\\" + GuidName.ToString() + "." + Extension;

                if (!System.IO.File.Exists(newFileAddr)) System.IO.File.Copy(fileAddress, newFileAddr);
            }
        }

        protected void find_files() {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ProviderUtil.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "select ApplicationID, OwnerType, FileNameGuid, Extension from dbo.DCT_Files where Deleted = 0";
            con.Open();
            System.Data.IDataReader reader = (System.Data.IDataReader)cmd.ExecuteReader();

            List<FL> fls = new List<FL>();

            while (reader.Read())
            {
                fls.Add(new FL() {
                    ApplicationID = (Guid)reader["ApplicationID"],
                    OwnerType = (string)reader["OwnerType"],
                    GuidName = (Guid)reader["FileNameGuid"],
                    Extension = (string)reader["Extension"]
                });
            }
                
            reader.Close();
            con.Close();

            foreach (FL f in fls) f.copy();
        }

        protected void find_node_icons()
        {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ProviderUtil.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "select x.NodeID from (select NodeID from dbo.CN_Nodes where Deleted = 0 union all select NodeTypeID from dbo.CN_NodeTypes where Deleted = 0) as x";
            con.Open();
            System.Data.IDataReader reader = (System.Data.IDataReader)cmd.ExecuteReader();

            List<Guid> ids = new List<Guid>();

            while (reader.Read()) ids.Add((Guid)reader["NodeID"]);

            reader.Close();
            con.Close();

            foreach (Guid id in ids)
            {
                string iconUrl = DocumentUtilities.get_icon_url(Guid.Parse("08C72552-4F2C-473F-B3B0-C2DACF8CD6A9"), id, "jpg");
                string hqIconUrl = DocumentUtilities.get_icon_url(Guid.Parse("08C72552-4F2C-473F-B3B0-C2DACF8CD6A9"), id, "jpg", true);

                if (string.IsNullOrEmpty(iconUrl) || string.IsNullOrEmpty(hqIconUrl)) continue;
                else {
                    iconUrl = PublicMethods.map_path(iconUrl.Replace("../..", "~"));
                    hqIconUrl = PublicMethods.map_path(hqIconUrl.Replace("../..", "~"));
                }

                string iconFolder = iconUrl.Substring(0, iconUrl.LastIndexOf('\\'));
                string hqIconFolder = hqIconUrl.Substring(0, hqIconUrl.LastIndexOf('\\'));

                string newIconFolder = iconFolder.ToLower().Replace("global_documents", "new_global_documents");
                string newHQIconFolder = hqIconFolder.ToLower().Replace("global_documents", "new_global_documents");

                if (!System.IO.Directory.Exists(newIconFolder)) System.IO.Directory.CreateDirectory(newIconFolder);
                if (!System.IO.Directory.Exists(newHQIconFolder)) System.IO.Directory.CreateDirectory(newHQIconFolder);

                string newIconFileAddr = newIconFolder + "\\" + iconUrl.Substring(iconUrl.LastIndexOf('\\') + 1);
                string newHQIconFileAddr = newHQIconFolder + "\\" + hqIconUrl.Substring(hqIconUrl.LastIndexOf('\\') + 1);

                if (!System.IO.File.Exists(newIconFileAddr)) System.IO.File.Copy(iconUrl, newIconFileAddr);
                if (!System.IO.File.Exists(newHQIconFileAddr)) System.IO.File.Copy(hqIconUrl, newHQIconFileAddr);
            }
        }
        */
        //end of to be removed

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;
            
            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "AddNodeType":
                    {
                        string typeName = PublicMethods.parse_string(context.Request.Params["TypeName"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        add_node_type(typeName,
                            PublicMethods.parse_guid(context.Request.Params["ParentID"]),
                            PublicMethods.parse_bool(context.Request.Params["IsCategory"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "RenameNodeType":
                    {
                        string typeName = PublicMethods.parse_string(context.Request.Params["TypeName"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        rename_node_type(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            typeName, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "SetNodeTypeAdditionalID":
                    set_node_type_additional_id(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["AdditionalID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetAdditionalIDPattern":
                    set_additional_id_pattern(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["AdditionalIDPattern"], false), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GenerateAdditionalID":
                    generate_additional_id(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["AdditionalIDPattern"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "MoveNodeType":
                    {
                        List<Guid> nodeTypeIds = ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|');
                        Guid? ntId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]);
                        if (nodeTypeIds.Count == 0 && ntId.HasValue) nodeTypeIds.Add(ntId.Value);

                        move_node_type(nodeTypeIds,
                            PublicMethods.parse_guid(context.Request.Params["ParentID"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "RemoveNodeType":
                    {
                        List<Guid> nodeTypeIds = ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|');
                        Guid? ntId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]);
                        if (nodeTypeIds.Count == 0 && ntId.HasValue) nodeTypeIds.Add(ntId.Value);

                        remove_node_types(nodeTypeIds,
                            PublicMethods.parse_bool(context.Request.Params["RemoveHierarchy"]), ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "RecoverNodeType":
                    recover_node_type(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetNodeTypes":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        List<ExtensionType> extensions = ListMaker.get_string_items(context.Request.Params["Extensions"], ',')
                            .Select(ex => PublicMethods.parse_enum<ExtensionType>(ex, defaultValue: ExtensionType.NotSet))
                            .Where(ex => ex != ExtensionType.NotSet).Distinct().ToList();

                        get_node_types(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                            PublicMethods.parse_bool(context.Request.Params["GrabSubNodeTypes"]),
                            PublicMethods.parse_bool(context.Request.Params["Icon"], false), 
                            searchText,
                            PublicMethods.parse_bool(context.Request.Params["IsKnowledge"]),
                            PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                            PublicMethods.parse_bool(context.Request.Params["Archive"]),
                            extensions,
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                            PublicMethods.parse_bool(context.Request.Params["Tree"]),
                            PublicMethods.parse_bool(context.Request.Params["CheckAccess"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetChildNodeTypes":
                    get_child_node_types(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Archive"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddRelationType":
                    add_relation_type(PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ModifyRelationType":
                    modify_relation_type(PublicMethods.parse_guid(context.Request.Params["RelationTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveRelationType":
                    remove_relation_type(PublicMethods.parse_guid(context.Request.Params["RelationTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetRelationTypes":
                    get_relation_types(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddNode":
                    {
                        string title = PublicMethods.parse_string(context.Request.Params["Title"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        add_node(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), title,
                            PublicMethods.parse_string(context.Request.Params["Description"]),
                            PublicMethods.parse_guid(context.Request.Params["ParentNodeID"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "ModifyNode":
                    {
                        string title = PublicMethods.parse_string(context.Request.Params["Title"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        modify_node(PublicMethods.parse_guid(context.Request.Params["NodeID"]), title,
                            PublicMethods.parse_string(context.Request.Params["Description"]),
                            ListMaker.get_string_items(context.Request.Params["Tags"], '|').Select(u => Base64.decode(u)).ToList(),
                            ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "ChangeNodeType":
                    change_node_type(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetDocumentTreeNodeID":
                    set_document_tree_node_id(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["DocumentTreeNodeID"]),
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ModifyNodeName":
                    {
                        string title = PublicMethods.parse_string(context.Request.Params["Title"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        modify_node_name(PublicMethods.parse_guid(context.Request.Params["NodeID"]), title,
                            PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "ModifyNodeDescription":
                    modify_node_description(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ModifyNodePublicDescription":
                    modify_node_public_description(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetNodeExpirationDate":
                    set_node_expiration_date(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_date(context.Request.Params["ExpirationDate"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetPreviousVersion":
                    set_previous_version(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["PreviousVersionID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ModifyNodeTags":
                    modify_node_tags(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        ListMaker.get_string_items(context.Request.Params["Tags"], '|').Select(u => Base64.decode(u)).ToList(),
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetNodeDescription":
                    get_node_description(PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetNodesSearchability":
                    set_nodes_searchability(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Searchable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveNode":
                    {
                        List<Guid> nodeIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        if (nodeIds.Count == 0 && nodeId.HasValue) nodeIds.Add(nodeId.Value);

                        remove_nodes(nodeIds,
                            PublicMethods.parse_bool(context.Request.Params["RemoveHierarchy"]), ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "RecycleNode":
                    {
                        List<Guid> nodeIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        if (nodeIds.Count == 0 && nodeId.HasValue) nodeIds.Add(nodeId.Value);

                        recycle_nodes(nodeIds, ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "GetNodeIcon":
                    get_node_icon(PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetDirectParent":
                case "MoveNode":
                    {
                        List<Guid> nIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        if (nIds.Count == 0 && nodeId.HasValue) nIds.Add(nodeId.Value);

                        set_direct_parent(nIds,
                            PublicMethods.parse_guid(context.Request.Params["ParentNodeID"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "SetNodeTypesOrder":
                    set_node_types_order(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetNodesOrder":
                    set_nodes_order(ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddRelation":
                    List<Guid> destinationNodeIds = ListMaker.get_guid_items(context.Request.Params["DestinationNodeIDs"], '|');
                    Guid? destId = PublicMethods.parse_guid(context.Request.Params["DestinationNodeID"]);
                    if (destId.HasValue) destinationNodeIds.Add(destId.Value);

                    add_relation(PublicMethods.parse_guid(context.Request.Params["SourceNodeID"]), destinationNodeIds,
                        PublicMethods.parse_guid(context.Request.Params["RelationTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SaveRelations":
                    save_relations(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        ListMaker.get_guid_items(context.Request.Params["RelatedNodeIDs"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveRelation":
                    remove_relation(PublicMethods.parse_guid(context.Request.Params["SourceNodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["DestinationNodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["RelationTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "Connect":
                case "Disconnect":
                    {
                        connect_disconnect_nodes(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                            command == "Connect", ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "GetNode":
                    get_node(PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "QRCode":
                case "qrcode":
                    {
                        Guid? nId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);

                        Node _nd = !nId.HasValue ? null : CNController.get_node(paramsContainer.Tenant.Id, nId.Value);

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(_nd.toJson(simple: true), QRCodeGenerator.ECCLevel.M);
                        QRCode qrCode = new QRCode(qrCodeData);

                        HttpContext.Current.Response.ContentType = "image/png";
                        qrCode.GetGraphic(20).Save(HttpContext.Current.Response.OutputStream, ImageFormat.Png);
                        return;
                    }
                case "GetNodes":
                    {
                        List<Guid> nodeIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');

                        int? fromNDaysAgo = PublicMethods.parse_int(context.Request.Params["CreatedFromNDaysAgo"]);
                        int? toNDaysAgo = PublicMethods.parse_int(context.Request.Params["CreatedToNDaysAgo"]);

                        DateTime now = DateTime.Now;

                        DateTime? lowerCreationDateLimit = fromNDaysAgo.HasValue ?
                            new DateTime(now.Year, now.Month, now.Day).AddDays(-1 * fromNDaysAgo.Value) :
                            PublicMethods.parse_date(context.Request.Params["CreationDateFrom"]);

                        DateTime? upperCreationDateLimit = toNDaysAgo.HasValue ?
                            new DateTime(now.Year, now.Month, now.Day).AddDays((-1 * toNDaysAgo.Value) + 1) :
                            PublicMethods.parse_date(context.Request.Params["CreationDateTo"], 1);

                        if (nodeIds.Count > 0)
                            get_nodes(nodeIds, ref responseText);
                        else
                        {
                            string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                                PublicMethods.parse_string(context.Request.Params["text"]));

                            get_nodes(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                                PublicMethods.parse_bool(context.Request.Params["UseNodeTypeHierarchy"]),
                                ListMaker.get_guid_items(context.Request.Params["RelatedToIDs"], '|'),
                                searchText,
                                PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                                PublicMethods.parse_bool(context.Request.Params["IsKnowledge"]),
                                PublicMethods.parse_bool(context.Request.Params["IsMine"]),
                                ListMaker.get_guid_items(context.Request.Params["CreatorUserIDs"], '|'),
                                PublicMethods.parse_bool(context.Request.Params["Archive"]),
                                lowerCreationDateLimit,
                                upperCreationDateLimit,
                                PublicMethods.parse_int(context.Request.Params["Count"]),
                                PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                                PublicMethods.parse_bool(context.Request.Params["Searchable"]),
                                PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                                PublicMethods.parse_bool(context.Request.Params["IsFavorite"]),
                                PublicMethods.parse_bool(context.Request.Params["IsGroup"]),
                                PublicMethods.parse_bool(context.Request.Params["IsExpertiseDomain"]),
                                FGUtilities.get_filters_from_json(PublicMethods.parse_string(context.Request.Params["FormFilters"])),
                                PublicMethods.parse_bool(context.Request.Params["MatchAllFilters"]),
                                PublicMethods.parse_bool(context.Request.Params["FetchCounts"]),
                                PublicMethods.parse_guid(context.Request.Params["GroupByElementID"]),
                                ref responseText);
                        }

                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "GetMostPopularNodes":
                    get_most_popular_nodes(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["ParentNodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetParentNodes":
                    get_parent_nodes(PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetChildNodes":
                    get_child_nodes(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["NodeTypeAdditionalID"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_string(context.Request.Params["OrderBy"], false),
                        PublicMethods.parse_bool(context.Request.Params["OrderByDesc"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"], decode: true, 
                            defaultValue: PublicMethods.parse_string(context.Request.Params["text"])),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetTreeDepth":
                    get_tree_depth(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "Like":
                case "Unlike":
                    like_unlike(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        command == "Unlike", ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFans":
                    get_fans(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddMember":
                case "RemoveMember":
                    if (command == "AddMember")
                    {
                        add_member(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }
                    else
                    {
                        remove_member(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }

                    _return_response(ref context, ref responseText);
                    return;
                case "AcceptMember":
                    accept_member(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetMembers":
                    {
                        List<Guid> nodeIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        bool isBatch = nodeIds.Count > 0;
                        if (nodeIds.Count == 0 && nodeId.HasValue) nodeIds.Add(nodeId.Value);

                        get_members(nodeIds,
                            PublicMethods.parse_bool(context.Request.Params["Admin"]), isBatch,
                            PublicMethods.parse_string(context.Request.Params["SearchText"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetPendingMembers":
                    get_pending_members(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetChildHierarchyMembers":
                    get_child_hierarchy_members(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetChildHierarchyExperts":
                    get_child_hierarchy_experts(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "MakeAdmin":
                case "UnAdmin":
                    if (command == "MakeAdmin")
                    {
                        make_admin(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }
                    else
                    {
                        unadmin(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }

                    _return_response(ref context, ref responseText);
                    return;
                case "AddExpert":
                case "RemoveExpert":
                    if (command == "AddExpert")
                    {
                        add_expert(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }
                    else
                    {
                        remove_expert(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    }

                    _return_response(ref context, ref responseText);
                    return;
                case "GetExperts":
                    {
                        List<Guid> nodeIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        bool isBatch = nodeIds.Count > 0;
                        if (nodeIds.Count == 0 && nodeId.HasValue) nodeIds.Add(nodeId.Value);

                        get_experts(nodeIds, isBatch,
                            PublicMethods.parse_string(context.Request.Params["SearchText"]),
                            PublicMethods.parse_bool(context.Request.Params["Hierarchy"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "RelationExists":
                    relation_exists(PublicMethods.parse_guid(context.Request.Params["SourceNodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["DestinationNodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetExpertiseDomainsCount":
                    get_expertise_domains_count(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetExpertiseDomains":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        get_expertise_domains(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], ','),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["AdditionalID"], false), searchText,
                            PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                            PublicMethods.parse_date(context.Request.Params["LowerDateLimit"]),
                            PublicMethods.parse_date(context.Request.Params["UpperDateLimit"], 1),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetMembershipDomainsCount":
                    get_membership_domains_count(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetMembershipDomains":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        get_membership_domains(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], ','),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["AdditionalID"], false), searchText,
                            PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                            PublicMethods.parse_date(context.Request.Params["LowerDateLimit"]),
                            PublicMethods.parse_date(context.Request.Params["UpperDateLimit"], 1),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetMemberNodes":
                    {
                        List<Guid> nodeTypeIds = ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|');
                        Guid? ntId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]);
                        if (nodeTypeIds.Count == 0 && ntId.HasValue) nodeTypeIds.Add(ntId.Value);

                        get_member_nodes(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            nodeTypeIds, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetFavoriteNodesCount":
                    get_favorite_nodes_count(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_bool(context.Request.Params["IsDocument"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFavoriteNodes":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        get_favorite_nodes(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["AdditionalID"], false),
                            searchText,
                            PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                            PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                            PublicMethods.parse_date(context.Request.Params["LowerDateLimit"]),
                            PublicMethods.parse_date(context.Request.Params["UpperDateLimit"], 1),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "IsFan":
                    is_fan(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetRelatedNodes":
                    get_related_nodes(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["RelatedNodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_bool(context.Request.Params["In"]),
                        PublicMethods.parse_bool(context.Request.Params["Out"]),
                        PublicMethods.parse_bool(context.Request.Params["InTags"]),
                        PublicMethods.parse_bool(context.Request.Params["OutTags"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetRelatedNodesAbstract":
                    get_related_nodes_abstract(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_bool(context.Request.Params["In"]),
                        PublicMethods.parse_bool(context.Request.Params["Out"]),
                        PublicMethods.parse_bool(context.Request.Params["InTags"]),
                        PublicMethods.parse_bool(context.Request.Params["OutTags"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddComplex":
                    add_complex(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ModifyComplex":
                    modify_complex(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveComplex":
                    remove_complex(PublicMethods.parse_guid(context.Request.Params["ListID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddNodeToComplex":
                    add_node_to_complex(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveComplexNode":
                    remove_complex_node(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddComplexAdmin":
                    add_complex_admin(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveComplexAdmin":
                    remove_complex_admin(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetComplexAdmins":
                    get_complex_admins(PublicMethods.parse_guid(context.Request.Params["ListID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetLists":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        get_lists(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), searchText,
                            PublicMethods.parse_guid(context.Request.Params["MinID"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetListNodes":
                    get_list_nodes(PublicMethods.parse_guid(context.Request.Params["ListID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SearchTags":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        search_tags(searchText, PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetMapItems":
                    {
                        Guid? userId = PublicMethods.parse_guid(context.Request.Params["UserID"]);
                        Guid? itemId = userId.HasValue ? userId : PublicMethods.parse_guid(context.Request.Params["NodeID"]);

                        get_map_items(itemId, userId.HasValue,
                            PublicMethods.parse_bool(context.Request.Params["Members"], true),
                            PublicMethods.parse_bool(context.Request.Params["Admins"], true),
                            PublicMethods.parse_bool(context.Request.Params["Experts"], true),
                            PublicMethods.parse_bool(context.Request.Params["Fans"], true),
                            PublicMethods.parse_bool(context.Request.Params["Creators"], true),
                            PublicMethods.parse_bool(context.Request.Params["Friends"], true),
                            PublicMethods.parse_bool(context.Request.Params["Hierarchy"], true),
                            PublicMethods.parse_bool(context.Request.Params["RelatedNodes"], true),
                            PublicMethods.parse_guid(context.Request.Params["RelatedNodesTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["MembersNodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["CreatorsNodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["ExpertsNodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["FansNodeTypeID"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetNodesCount":
                    {
                        get_nodes_count(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                            PublicMethods.parse_date(context.Request.Params["LowerCreationDateLimit"]),
                            PublicMethods.parse_date(context.Request.Params["UpperCreationDateLimit"], 1),
                            PublicMethods.parse_bool(context.Request.Params["Root"]),
                            PublicMethods.parse_bool(context.Request.Params["Archive"]), ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "GetMostPopulatedNodeTypes":
                    get_most_populated_node_types(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SuggestNodeRelations":
                    suggest_node_relations(PublicMethods.parse_guid(context.Request.Params["RelatedNodeTypeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SuggestNodeTypesForRelations":
                    suggest_node_types_for_relations(PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SuggestSimilarNodes":
                    suggest_similar_nodes(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SuggestKnowledgableUsers":
                    suggest_knowledgable_uesrs(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetNodeInfo":
                    {
                        bool? childsCount = PublicMethods.parse_bool(context.Request.Params["ChildsCount"]);
                        if (!childsCount.HasValue || !childsCount.Value)
                            childsCount = PublicMethods.parse_bool(context.Request.Params["HasChild"]);

                        List<Guid> nIds = ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|');
                        Guid? nodeId = PublicMethods.parse_guid(context.Request.Params["NodeID"]);
                        bool isArray = nIds.Count > 0;
                        if (nIds.Count == 0 && nodeId.HasValue) nIds.Add(nodeId.Value);

                        get_node_info(nIds, isArray,
                            PublicMethods.parse_bool(context.Request.Params["Keywords"]),
                            PublicMethods.parse_bool(context.Request.Params["Description"]),
                            PublicMethods.parse_bool(context.Request.Params["Attachments"]),
                            PublicMethods.parse_bool(context.Request.Params["Creator"]),
                            PublicMethods.parse_bool(context.Request.Params["ContributorsCount"]),
                            PublicMethods.parse_bool(context.Request.Params["LikesCount"]),
                            PublicMethods.parse_bool(context.Request.Params["VisitsCount"]),
                            PublicMethods.parse_bool(context.Request.Params["ExpertsCount"]),
                            PublicMethods.parse_bool(context.Request.Params["MembersCount"]), childsCount,
                            PublicMethods.parse_bool(context.Request.Params["RelatedNodesCount"]),
                            PublicMethods.parse_bool(context.Request.Params["LikeStatus"]),
                            PublicMethods.parse_bool(context.Request.Params["CoverPhotoURL"]),
                            PublicMethods.parse_bool(context.Request.Params["Extensions"]),
                            PublicMethods.parse_bool(context.Request.Params["Service"]),
                            PublicMethods.parse_bool(context.Request.Params["UserStatus"]),
                            ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "EnableExtension":
                case "DisableExtension":
                case "SetExtensionTitle":
                case "MoveExtension":
                case "GetExtensions":
                    {
                        Guid? ownerId = PublicMethods.parse_guid(context.Request.Params["OwnerID"]);

                        string title = PublicMethods.parse_string(context.Request.Params["Title"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        ExtensionType extension = ExtensionType.NotSet;
                        if (!Enum.TryParse<ExtensionType>(context.Request.Params["Extension"], out extension))
                            extension = ExtensionType.NotSet;

                        if (command == "EnableExtension") enable_extension(ownerId, extension, ref responseText);
                        else if (command == "DisableExtension") disable_extension(ownerId, extension, ref responseText);
                        else if (command == "SetExtensionTitle") set_extension_title(ownerId, extension, title, ref responseText);
                        else if (command == "MoveExtension")
                        {
                            move_extension(ownerId, extension,
                                PublicMethods.parse_bool(context.Request.Params["MoveDown"], false), ref responseText);
                        }
                        else
                        {
                            get_extensions(ownerId,
                                PublicMethods.parse_bool(context.Request.Params["Initialize"], false), ref responseText);
                        }

                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "HasExtension":
                    {
                        ExtensionType extension = ExtensionType.NotSet;
                        if (!Enum.TryParse<ExtensionType>(context.Request.Params["Extension"], out extension))
                            extension = ExtensionType.NotSet;

                        has_extension(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                            extension, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetIntellectualPropertiesCount":
                    get_intellectual_properties_count(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_bool(context.Request.Params["IsDocument"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetIntellectualProperties":
                    {
                        string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"], true,
                            PublicMethods.parse_string(context.Request.Params["text"]));

                        get_intellectual_properties(ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], ','),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["AdditionalID"], false), searchText,
                            PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                            PublicMethods.parse_bool(context.Request.Params["HasChild"]),
                            PublicMethods.parse_date(context.Request.Params["LowerDateLimit"]),
                            PublicMethods.parse_date(context.Request.Params["UpperDateLimit"], 1),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "GetIntellectualPropertiesOfFriends":
                    get_intellectual_properties_of_friends(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_bool(context.Request.Params["CreatorInfo"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "Explore":
                    explore(PublicMethods.parse_guid(context.Request.Params["BaseID"]),
                        PublicMethods.parse_guid(context.Request.Params["RelatedID"]),
                        ListMaker.get_guid_items(context.Request.Params["BaseTypeIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["RelatedTypeIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["SecondLevelNodeID"]),
                        PublicMethods.parse_bool(context.Request.Params["RegistrationArea"]),
                        PublicMethods.parse_bool(context.Request.Params["Tags"]),
                        PublicMethods.parse_bool(context.Request.Params["Relations"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_string(context.Request.Params["OrderBy"], false),
                        PublicMethods.parse_bool(context.Request.Params["OrderByDesc"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ExcelHeaders":
                    {
                        get_excel_headers(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            PublicMethods.fromJSON(Base64.decode(context.Request.Params["Dic"])),
                            ref responseText);

                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "ImportNodesFromExcel":
                    {
                        List<DocFileInfo> files = 
                            DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["Uploaded"]);

                        if (files == null || files.Count != 1)
                            responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        else
                        {
                            import_nodes_from_excel(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), files[0],
                                PublicMethods.parse_int(context.Request.Params["SheetNo"]), ref responseText);
                        }

                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "XML2Node":
                    {
                        List<DocFileInfo> files = 
                            DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["Uploaded"]);

                        if (files == null || files.Count != 1)
                            responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        else
                        {
                            xml2node(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), files[0],
                                PublicMethods.parse_string(context.Request.Params["Map"]),
                                PublicMethods.parse_bool(context.Request.Params["AttachUploadedFile"]),
                                ref responseText);
                        }

                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "GetService":
                    get_service(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetServices":
                    get_services(PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                        PublicMethods.parse_bool(context.Request.Params["IsKnowledge"]),
                        PublicMethods.parse_bool(context.Request.Params["All"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetServiceRegistrationInfo":
                    get_service_registeration_info(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetServiceTitle":
                    {
                        string title = PublicMethods.parse_string(context.Request.Params["Title"], true,
                            PublicMethods.parse_string(context.Request.Params["Name"]));

                        set_service_title(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            title, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "SetServiceDescription":
                    set_service_description(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetServiceSuccessMessage":
                    set_service_success_message(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["Message"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetServiceSuccessMessage":
                    get_service_success_message(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetServiceAdminType":
                    ServiceAdminType adminType = ServiceAdminType.NotSet;
                    if (!Enum.TryParse<ServiceAdminType>(context.Request.Params["AdminType"], out adminType))
                        adminType = ServiceAdminType.NotSet;

                    set_service_admin_type(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), adminType,
                        PublicMethods.parse_guid(context.Request.Params["AdminNodeID"]),
                        ListMaker.get_guid_items(context.Request.Params["Limits"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetMaxAcceptableAdminLevel":
                    set_max_acceptable_admin_level(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_int(context.Request.Params["MaxAcceptableAdminLevel"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetContributionLimits":
                    set_contribution_limits(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        ListMaker.get_guid_items(context.Request.Params["Limits"], '|'), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetContributionLimits":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        get_contribution_limits(itemId, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "EnableContribution":
                    enable_contribution(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Enable"], false), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "NoContentService":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        no_content_service(itemId,
                            PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "IsKnowledge":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        is_knowledge(itemId,
                            PublicMethods.parse_bool(context.Request.Params["IsKnowledge"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "IsDocument":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        is_document(itemId,
                            PublicMethods.parse_bool(context.Request.Params["IsDocument"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "EnablePreviousVersionSelect":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        enable_previous_version_select(itemId,
                            PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "IsTree":
                    is_tree(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["IsTree"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "HasUniqueMembership":
                    has_unique_membership(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "HasUniqueAdminMember":
                    has_unique_admin_member(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AbstractAndKeywordsDisabled":
                    abstract_and_keywords_disabled(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "FileUploadDisabled":
                    file_upload_disabled(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RelatedNodesSelectDisabled":
                    related_nodes_select_disabled(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditableForAdmin":
                    editable_for_admin(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Editable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditableForCreator":
                    editable_for_creator(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Editable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditableForOwners":
                    editable_for_owners(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Editable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditableForExperts":
                    editable_for_experts(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Editable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditableForMembers":
                    editable_for_members(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Editable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditSuggestion":
                    edit_suggestion(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Enable"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddFreeUser":
                    add_free_user(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveFreeUser":
                    remove_free_user(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetServiceAdmins":
                    get_service_admins(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddServiceAdmin":
                    add_service_admin(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveServiceAdmin":
                    remove_service_admin(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "IsServiceAdmin":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeTypeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]));

                        is_service_admin(itemId, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "CheckNodeCreationAccess":
                    check_node_creation_access(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RegisterNewNode":
                    string[] arrContributors = string.IsNullOrEmpty(context.Request.Params["Contributors"]) ? string.Empty.Split('|') :
                        context.Request.Params["Contributors"].Split('|');
                    List<NodeCreator> contributors = new List<NodeCreator>();
                    foreach (string str in arrContributors)
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        string[] item = str.Split(':');
                        NodeCreator nc = new NodeCreator();
                        nc.User.UserID = Guid.Parse(item[0]);
                        nc.CollaborationShare = double.Parse(item[1]);
                        contributors.Add(nc);
                    }

                    if (contributors.Count == 0 && paramsContainer.CurrentUserID.HasValue)
                    {
                        NodeCreator _nc = new NodeCreator();
                        _nc.User.UserID = paramsContainer.CurrentUserID.Value;
                        _nc.CollaborationShare = 100;
                        contributors.Add(_nc);
                    }

                    register_new_node(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["ParentNodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["DocumentTreeNodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["PreviousVersionID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        ListMaker.get_string_items(context.Request.Params["Tags"], '|').Select(u => Base64.decode(u)).ToList(),
                        contributors,
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_guid(context.Request.Params["AdminAreaID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormInstanceID"]),
                        PublicMethods.parse_date(context.Request.Params["CreationDate"]),
                        DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["Logo"]).FirstOrDefault(),
                        PublicMethods.parse_bool(context.Request.Params["GetExtendInfo"]),
                        PublicMethods.parse_bool(context.Request.Params["GetWorkFlowInfo"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetAdminAreaLimits":
                    {
                        Guid? itemId = PublicMethods.parse_guid(context.Request.Params["NodeID"],
                            PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]));

                        get_admin_area_limits(itemId, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "SetAdminArea":
                    set_admin_area(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_guid(context.Request.Params["AreaID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetContributors":
                    string[] _arrContributors = string.IsNullOrEmpty(context.Request.Params["Contributors"]) ? string.Empty.Split('|') :
                        context.Request.Params["Contributors"].Split('|');
                    List<NodeCreator> _contributors = new List<NodeCreator>();
                    foreach (string str in _arrContributors)
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        string[] item = str.Split(':');
                        NodeCreator nc = new NodeCreator();
                        nc.User.UserID = Guid.Parse(item[0]);
                        nc.CollaborationShare = double.Parse(item[1]);
                        _contributors.Add(nc);
                    }

                    if (_contributors.Count == 0 && paramsContainer.CurrentUserID.HasValue)
                    {
                        NodeCreator _nc = new NodeCreator();
                        _nc.User.UserID = paramsContainer.CurrentUserID.Value;
                        _nc.CollaborationShare = 100;
                        _contributors.Add(_nc);
                    }

                    set_contributors(PublicMethods.parse_guid(context.Request.Params["NodeID"]), _contributors,
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetTemplateTags":
                    get_template_tags(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetTemplates":
                    get_templates(PublicMethods.parse_guid(context.Request.Params["TagID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetTemplateJSON":
                    get_template_json(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetTemplateStatus":
                    get_template_status(PublicMethods.parse_string(context.Request.Params["Template"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "ActivateTemplate":
                    activate_template(PublicMethods.parse_string(context.Request.Params["Template"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref HttpContext httpContext, ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void _save_error_log(Modules.Log.Action action, List<Guid> subjectIds,
            Guid? secondSubjectId = null, string info = null)
        {
            try
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = action,
                    SubjectIDs = subjectIds,
                    SecondSubjectID = secondSubjectId,
                    Info = info,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            catch { }
        }

        protected void _save_error_log(Modules.Log.Action action, Guid? subjectId,
            Guid? secondSubjectId = null, string info = null)
        {
            if (!subjectId.HasValue) return;
            _save_error_log(action, new List<Guid>() { subjectId.Value }, secondSubjectId, info);
        }

        public void check_node_workflow_permissions(Node node, bool? isKnowledge,
            bool isSystemAdmin, bool isServiceAdmin, bool isAreaAdmin, bool isCreator,
            ref bool hasKnowledgePermission, ref bool hasWorkFlowPermission,
            ref bool hasWFEditPermission, ref bool hideContributors)
        {
            hasKnowledgePermission = hasWorkFlowPermission = hideContributors = false;

            bool isDirector = isSystemAdmin || isServiceAdmin || isAreaAdmin || isCreator;

            if (!isKnowledge.HasValue)
                isKnowledge = CNController.is_knowledge(paramsContainer.Tenant.Id, node.NodeID.Value);

            hasKnowledgePermission = isKnowledge.HasValue && isKnowledge.Value && isDirector &&
                (node.Status == Status.SentToAdmin || node.Status == Status.SentToEvaluators);

            if (isKnowledge.Value)
            {
                if (!hasKnowledgePermission && paramsContainer.CurrentUserID.HasValue)
                {
                    bool beforeWorkflow = (node.Status == Status.NotSet || node.Status == Status.Personal ||
                        node.Status == Status.Rejected || node.Status == Status.SentBackForRevision) && isDirector;

                    bool afterWorkflow = (node.Status == Status.Accepted || node.Status == Status.Rejected) && isDirector;

                    bool inWorkFlow = !beforeWorkflow && !afterWorkflow && isDirector &&
                        NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        null, node.NodeID.Value, DashboardType.Knowledge, null, null, false);

                    hasKnowledgePermission = beforeWorkflow || afterWorkflow || inWorkFlow ||
                        NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, node.NodeID.Value, DashboardType.Knowledge, null, null, false) ||
                        (node.Status == Status.SentToEvaluators && NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, node.NodeID.Value, DashboardType.Knowledge, DashboardSubType.Evaluator, null, null));
                }
            }
        }

        protected void add_node_type(string typeName, Guid? parentId, bool? isCategory, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //Initial Checks
            if (!RaaiVanSettings.SAASBasedMultiTenancy) isCategory = null;

            if (isCategory.HasValue && isCategory.Value) parentId = null;

            bool isService = (!isCategory.HasValue || !isCategory.Value) && RaaiVanSettings.SAASBasedMultiTenancy;
            //end of Initial Checks

            if (!string.IsNullOrEmpty(typeName) && typeName.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(typeName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddNodeType_PermissionFailed, Guid.Empty);
                return;
            }

            NodeType nodeType = new NodeType()
            {
                NodeTypeID = Guid.NewGuid(),
                Name = typeName,
                ParentID = parentId,
                AdditionalIDPattern = CNUtilities.DefaultAdditionalIDPattern,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now,
                Archive = false,
                IsService = isService
            };

            bool result = CNController.add_node_type(paramsContainer.Tenant.Id, nodeType);

            if (result && isService) CNController.initialize_extensions(paramsContainer.Tenant.Id, 
                nodeType.NodeTypeID.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + 
                    ",\"NodeTypeID\":\"" + nodeType.NodeTypeID.ToString() + "\"" + 
                    ",\"NodeType\":" + nodeType.toJson() + 
                "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeType.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNodeType,
                    SubjectID = nodeType.NodeTypeID,
                    Info = "{\"Name\":\"" + Base64.encode(typeName) + "\",\"ParentID\":\"" + parentId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void rename_node_type(Guid? nodeTypeId, string typeName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(typeName) && typeName.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(typeName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.RenameNodeType_PermissionFailed, nodeTypeId);
                return;
            }

            NodeType nodeType = new NodeType()
            {
                NodeTypeID = nodeTypeId,
                Name = typeName,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.rename_node_type(paramsContainer.Tenant.Id, nodeType);
            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"NodeType\":" + nodeType.toJson() + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RenameNodeType,
                    SubjectID = nodeType.NodeTypeID,
                    Info = "{\"Name\":\"" + Base64.encode(typeName) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_node_type_additional_id(Guid? nodeTypeId, string additionalId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(additionalId) && additionalId.Length > 45)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(additionalId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetNodeTypeAdditionalID_PermissionFailed, nodeTypeId);
                return;
            }

            string errorMessage = string.Empty;

            bool result = CNController.set_node_type_additional_id(paramsContainer.Tenant.Id, nodeTypeId.Value,
                additionalId, paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                    Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetNodeTypeAdditionalID,
                    SubjectID = nodeTypeId,
                    Info = "{\"AdditionalID\":\"" + Base64.encode(additionalId) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_additional_id_pattern(Guid? nodeTypeId, string additionalIdPattern, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(additionalIdPattern) && additionalIdPattern.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetNodeTypeAdditionalIDPattern_PermissionFailed, nodeTypeId);
                return;
            }

            if (!CNUtilities.validate_additional_id_pattern(additionalIdPattern))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PatternIsNotValid + "\"}";
                return;
            }

            NodeType nodeType = new NodeType()
            {
                NodeTypeID = nodeTypeId,
                AdditionalIDPattern = string.IsNullOrEmpty(additionalIdPattern) ? null : additionalIdPattern,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.set_additional_id_pattern(paramsContainer.Tenant.Id, nodeType);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                    ",\"NodeType\":" + nodeType.toJson() + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetNodeTypeAdditionalIDPattern,
                    SubjectID = nodeType.NodeTypeID,
                    Info = "{\"AdditionalIDPattern\":\"" + Base64.encode(additionalIdPattern) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void generate_additional_id(Guid? nodeTypeId, string pattern, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            else if (!CNUtilities.validate_additional_id_pattern(pattern))
            {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidAdditionalIDPattern + "\"}";
                return;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();

            string additionalId = CNUtilities.generate_new_additional_id(paramsContainer.Tenant.Id, nodeTypeId.Value, pattern, ref dic);

            responseText = "{\"AdditionalID\":\"" + additionalId + "\"" +
                ",\"Dic\":" + PublicMethods.toJSON(dic) + "}";
        }

        public static string update_additional_id(Guid applicationId, Guid nodeId, Guid currentUserId,
            string mainId = null, bool dontSave = false)
        {
            Node nodeObject = CNController.get_node(applicationId, nodeId);

            if (nodeObject == null || !nodeObject.NodeID.HasValue || string.IsNullOrEmpty(nodeObject.AdditionalID_Main))
                return string.Empty; //no update available

            string additionalId_main = !string.IsNullOrEmpty(mainId) ? mainId : nodeObject.AdditionalID_Main;
            string additionalId = string.Empty;


            //Update based on previous version
            string fVersionPattern = "~[[FVersionID]]";
            string pVersionPattern = "~[[PVersionID]]";
            string versionCounterPattern = "~[[VersionCounter]]";

            bool hasFVPattern = additionalId_main.IndexOf(fVersionPattern) >= 0,
                hasPVPattern = additionalId_main.IndexOf(pVersionPattern) >= 0,
                hasVCPattern = additionalId_main.IndexOf(versionCounterPattern) >= 0;
            
            if (nodeObject.Searchable.HasValue && nodeObject.Searchable.Value && (hasFVPattern || hasPVPattern || hasVCPattern))
            {
                List<Node> previousVersions = CNController.get_previous_versions(applicationId, nodeId);

                int versionCounter = previousVersions.Count + 1;

                if (previousVersions.Count > 0)
                {
                    string firstVersionId = string.Empty, previousVersionId = string.Empty;

                    if (hasFVPattern) {
                        firstVersionId = !hasVCPattern ? previousVersions.Last().AdditionalID :
                            update_additional_id(applicationId, previousVersions.Last().NodeID.Value, currentUserId,
                                mainId: previousVersions.Last().AdditionalID_Main.Replace(versionCounterPattern, ""), dontSave: true);
                    }

                    if (hasPVPattern) {
                        if (hasFVPattern && previousVersions.Count == 1) previousVersionId = firstVersionId;
                        else {
                            previousVersionId = !hasVCPattern ? previousVersions.First().AdditionalID :
                                update_additional_id(applicationId, previousVersions.First().NodeID.Value, currentUserId,
                                    mainId: previousVersions.First().AdditionalID_Main.Replace(versionCounterPattern, ""), dontSave: true);
                        }
                    }

                    int fIndex = additionalId_main.IndexOf(fVersionPattern);
                    int pIndex = additionalId_main.IndexOf(pVersionPattern);

                    if (!string.IsNullOrEmpty(firstVersionId) && fIndex >= 0)
                        additionalId_main = additionalId_main.Substring(fIndex).Replace(fVersionPattern, firstVersionId);

                    if (!string.IsNullOrEmpty(previousVersionId) && pIndex >= 0)
                        additionalId_main = additionalId_main.Substring(pIndex).Replace(pVersionPattern, previousVersionId);
                }

                additionalId_main = additionalId_main.Replace(versionCounterPattern, versionCounter.ToString());
            }
            //end of Update based on previous version


            //Update based on admin area
            string areaPattern = "~[[AreaID]]";

            if (nodeObject.AdminAreaID.HasValue && additionalId_main.IndexOf(areaPattern) >= 0)
            {
                Node adminArea = CNController.get_node(applicationId, nodeObject.AdminAreaID.Value);

                if (adminArea != null && !string.IsNullOrEmpty(adminArea.AdditionalID))
                    additionalId_main = additionalId_main.Replace(areaPattern, adminArea.AdditionalID);
            }
            //end of Update based on admin area


            //Update based on form elements
            if (additionalId_main.IndexOf("~[[FormField:") >= 0)
            {
                FormType formInstance = FGController.get_owner_form_instances(applicationId, nodeId).FirstOrDefault();

                List<FormElement> elements = formInstance == null || !formInstance.InstanceID.HasValue ? new List<FormElement>() :
                    FGController.get_form_instance_elements(applicationId, formInstance.InstanceID.Value)
                        .Where(e => e.RefElementID.HasValue && e.ElementID.HasValue && e.ElementID != e.RefElementID).ToList();

                //-------- fill selected nodes
                Dictionary<Guid, List<SelectedGuidItem>> sIds = FGController.get_selected_guids(applicationId,
                        elements.Where(u => u.Type == FormElementTypes.Node).Select(u => u.ElementID.Value).ToList());

                elements.Where(x => x.ElementID.HasValue && sIds.ContainsKey(x.ElementID.Value))
                    .ToList().ForEach(e => e.GuidItems = sIds[e.ElementID.Value]);
                //-------- end of fill selected nodes

                List<FormElement> nodeElems = elements.Where(u => u.Type == FormElementTypes.Node && u.GuidItems.Count == 1).ToList();
                List<Node> nodes = CNController.get_nodes(applicationId, nodeElems.Select(e => e.GuidItems.First().ID.Value).ToList(), full: null);

                elements.ForEach(elem =>
                {
                    bool isNode = nodeElems.Any(e => e == elem || e.ElementID == elem.ElementID);
                    Node nd = !isNode ? null : nodes.Where(u => u.NodeID == elem.GuidItems.First().ID.Value).FirstOrDefault();

                    string replacement = string.Empty;

                    switch (elem.Type)
                    {
                        case FormElementTypes.Node:
                            if (nd != null) replacement = nd.AdditionalID;
                            break;
                        case FormElementTypes.Numeric:
                            if (elem.FloatValue.HasValue) replacement = Math.Abs(Math.Round(elem.FloatValue.Value)).ToString();
                            break;
                        case FormElementTypes.Text:
                            if (!string.IsNullOrEmpty(elem.TextValue)) replacement = elem.TextValue.Trim();
                            break;
                        case FormElementTypes.Select:
                            if (elem.FloatValue.HasValue) replacement = Math.Abs(Math.Round(elem.FloatValue.Value)).ToString();
                            else if (!string.IsNullOrEmpty(elem.TextValue)) replacement = elem.TextValue.Trim();
                            break;
                    }

                    if (string.IsNullOrEmpty(replacement)) return;

                    string lower = "~[[FormField:" + elem.RefElementID.Value.ToString().ToLower() + "]]";
                    string upper = "~[[FormField:" + elem.RefElementID.Value.ToString().ToUpper() + "]]";

                    additionalId_main = additionalId_main.Replace(lower, replacement).Replace(upper, replacement);
                });
            }
            //end of Update based on form elements


            //Save the result
            additionalId = Expressions.replace(additionalId_main, Expressions.Patterns.AutoTag, ""); //remove unfilled parts

            if (dontSave) return additionalId;
            else if (additionalId != nodeObject.AdditionalID && CNController.set_additional_id(applicationId, nodeId,
                nodeObject.AdditionalID_Main, additionalId, currentUserId)) return additionalId;
            //end of Save the result

            return string.Empty;
        }

        protected void move_node_type(List<Guid> nodeTypeIds, Guid? parentId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.MoveNodeType_PermissionFailed, nodeTypeIds);
                return;
            }

            if (parentId.HasValue && RaaiVanSettings.SAASBasedMultiTenancy &&
                CNController.get_node_types(paramsContainer.Tenant.Id, nodeTypeIds).Any(nt => nt.IsCategory))
            {
                responseText = "{\"ErrorText\":\"" + Messages.CannotMoveCategoriesToSubLevels + "\"}";
                return;
            }

            bool result = CNController.move_node_type(paramsContainer.Tenant.Id,
                nodeTypeIds, parentId, paramsContainer.CurrentUserID.Value);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +/*,\"NodeType\":" + _get_node_type_json(nodeType, false) + */"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.MoveNodeType,
                    SubjectID = parentId,
                    Info = "{\"NodeTypeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeTypeIds.Select(u => "\"" + u.ToString() + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_node_types(List<Guid> nodeTypeIds, bool? removeHierarchy, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveNodeType_PermissionFailed, nodeTypeIds);
                return;
            }

            bool result = CNController.remove_node_types(paramsContainer.Tenant.Id,
                nodeTypeIds, removeHierarchy, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveNodeType,
                    Info = "{\"RemoveHierarchy\":" + (removeHierarchy.HasValue && removeHierarchy.Value).ToString().ToLower() + "}",
                    SubjectIDs = nodeTypeIds,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void recover_node_type(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.RecoverNodeType_PermissionFailed, nodeTypeId);
                return;
            }

            NodeType nodeType = new NodeType()
            {
                NodeTypeID = nodeTypeId,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.recover_node_type(paramsContainer.Tenant.Id, nodeType);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RecoverNodeType,
                    SubjectID = nodeType.NodeTypeID,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_node_types(List<Guid> nodeTypeIds, bool? grabSubNodeTypes, bool? icon, 
            string searchText, bool? isKnowledge, bool? isDocument, bool? archive, List<ExtensionType> extensions,
            int? count, long? lowerBoundary, bool? hasChild, bool? tree, bool? checkAccess, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;
            
            if (archive.HasValue && archive.Value &&
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            long totalCount = 0;

            List<NodeType> nodeTypes = nodeTypeIds.Count > 0 ?
                CNController.get_node_types(paramsContainer.Tenant.Id, nodeTypeIds, grabSubNodeTypes.HasValue && grabSubNodeTypes.Value) :
                CNController.get_node_types(paramsContainer.Tenant.Id, searchText, isKnowledge,
                    isDocument, archive, extensions, count, lowerBoundary, ref totalCount).OrderBy(u => u.Archive).ToList();

            nodeTypeIds = nodeTypes.Select(u => u.NodeTypeID.Value).ToList();


            //Check HaveChild
            List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                CNController.have_child_node_types(paramsContainer.Tenant.Id, nodeTypeIds);
            
            nodeTypes.ForEach(u => { u.HasChild = haveChild.Any(x => x == u.NodeTypeID); });
            //end of Check HaveChild


            //Check Access
            if (checkAccess.HasValue && checkAccess.Value)
            {
                bool isSystemAdmin = paramsContainer.CurrentUserID.HasValue &&
                    PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

                List<Guid> serviceAdmin = !paramsContainer.CurrentUserID.HasValue || isSystemAdmin ? new List<Guid>() :
                    CNController.is_service_admin(paramsContainer.Tenant.Id, nodeTypeIds, paramsContainer.CurrentUserID.Value);

                Dictionary<Guid, List<PermissionType>> accessDic = isSystemAdmin ? new Dictionary<Guid, List<PermissionType>>() :
                    PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        nodeTypeIds.Where(id => !serviceAdmin.Any(x => x == id)).ToList(),
                        PrivacyObjectType.NodeType, new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract });

                nodeTypes
                    .Where(nt => !isSystemAdmin && !serviceAdmin.Any(x => x == nt.NodeTypeID) && !accessDic.Keys.Any(x => x == nt.NodeTypeID))
                    .ToList().ForEach(nt => nt.Hidden = true);
            }
            //end of Check Access


            responseText = "{\"DefaultAdditionalIDPattern\":\"" + CNUtilities.DefaultAdditionalIDPattern + "\"" +
                ",\"TotalCount\":" + totalCount.ToString() +
                ",\"NodeTypes\":[" + string.Join(",", nodeTypes.Select(
                    u => u.toJson(paramsContainer.Tenant.Id, icon.HasValue && icon.Value)).ToList()) + "]" +
                (!tree.HasValue || !tree.Value ? string.Empty : 
                    ",\"Tree\":[" + string.Join(",", NodeType.toTree(nodeTypes)
                        .Select(t => t.toJson(paramsContainer.Tenant.Id, icon.HasValue && icon.Value))) + "]") +
                "}";
        }

        protected void get_child_node_types(Guid? parentId, bool? archive, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            archive = archive.HasValue && archive.Value && paramsContainer.CurrentUserID.HasValue &&
                AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID);

            List<NodeType> nodeTypes = archive.HasValue && archive.Value ?
                CNController.get_node_types(paramsContainer.Tenant.Id, null, new List<ExtensionType>(), archive) :
                CNController.get_child_node_types(paramsContainer.Tenant.Id, parentId, archive);

            List<Guid> nodeTypeIds = CNController.have_child_node_types(paramsContainer.Tenant.Id, 
                nodeTypeIds: nodeTypes.Select(u => u.NodeTypeID.Value).ToList());

            nodeTypes.ForEach(u => { u.HasChild = nodeTypeIds.Exists(x => x == u.NodeTypeID.Value); });

            responseText = "{\"DefaultAdditionalIDPattern\":\"" + CNUtilities.DefaultAdditionalIDPattern + "\"" +
                ",\"NodeTypes\":[" + string.Join(",", nodeTypes.Select(u => u.toJson())) + "]}";
        }

        protected void add_relation_type(string typeName, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddRelationType_PermissionFailed, Guid.Empty);
                return;
            }

            RelationType relationType = new RelationType()
            {
                RelationTypeID = Guid.NewGuid(),
                Name = typeName,
                Description = description,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now
            };

            bool result = CNController.add_relation_type(paramsContainer.Tenant.Id, relationType);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + 
                ",\"RelationType\":" + relationType.toJson() + 
                "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = relationType.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddRelationType,
                    SubjectID = relationType.RelationTypeID,
                    Info = "{\"Name\":\"" + Base64.encode(typeName) + "\",\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_relation_type(Guid? relationTypeId, string typeName, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.ModifyRelationType_PermissionFailed, relationTypeId);
                return;
            }

            RelationType relationType = new RelationType()
            {
                RelationTypeID = relationTypeId,
                Name = typeName,
                Description = description,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.modify_relation_type(paramsContainer.Tenant.Id, relationType);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"RelationType\":" + relationType.toJson() + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = relationType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyRelationType,
                    SubjectID = relationType.RelationTypeID,
                    Info = "{\"Name\":\"" + Base64.encode(typeName) + "\",\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_relation_type(Guid? relationTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveRelationType_PermissionFailed, relationTypeId);
                return;
            }

            RelationType relationType = new RelationType()
            {
                RelationTypeID = relationTypeId,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.remove_relation_type(paramsContainer.Tenant.Id, relationType);
            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = relationType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveRelationType,
                    SubjectID = relationType.RelationTypeID,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_relation_types(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<RelationType> relationTypes = CNController.get_relation_types(paramsContainer.Tenant.Id);

            responseText = "{\"RelationTypes\":[" + string.Join(",", relationTypes.Select(t => t.toJson())) + "]}";
        }

        protected void add_node(Guid? nodeTypeId, string name, string description,
            Guid? parentNodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name)) name = name.Trim();

            if (string.IsNullOrEmpty(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TitleIsNotDetermined + "\"}";
                return;
            }
            else if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (nodeTypeId == Guid.Empty) nodeTypeId = null;

            NodeType nt = nodeTypeId.HasValue ?
                CNController.get_node_type(paramsContainer.Tenant.Id, nodeTypeId.Value) : null;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nt == null ? null : nt.NodeTypeID);

            bool hasAccess= accessChecker.checkNodeEditAccess(AdminLevel.Service, roleName: AccessRoleName.ManageOntology, 
                privacyObjectType: PrivacyObjectType.NodeType, permission: PermissionType.Create);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nt != null && nt.NodeTypeID.HasValue)
                    _save_error_log(Modules.Log.Action.AddNode_PermissionFailed, nodeTypeId);
                return;
            }
            //end of check access

            if (nt != null && !string.IsNullOrEmpty(nt.AdditionalIDPattern) &&
                !Expressions.is_match(nt.AdditionalIDPattern, Expressions.Patterns.AdditionalID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AdditionalIDPatternIsNotValid.ToString() + "\"}";
                return;
            }

            Node newNode = new Node()
            {
                NodeID = Guid.NewGuid(),
                AdditionalID_Main = nt == null ? null : 
                    CNUtilities.generate_new_additional_id(paramsContainer.Tenant.Id, nt.NodeTypeID.Value, nt.AdditionalIDPattern),
                Name = name,
                Description = description,
                CreationDate = DateTime.Now
            };

            if (!string.IsNullOrEmpty(newNode.AdditionalID_Main))
                newNode.AdditionalID = Expressions.replace(newNode.AdditionalID_Main, Expressions.Patterns.AutoTag, "");

            newNode.Creator.UserID = paramsContainer.CurrentUserID.Value;

            if (nodeTypeId.HasValue && nodeTypeId.Value != Guid.Empty) newNode.NodeTypeID = nodeTypeId;
            if (parentNodeId.HasValue) newNode.ParentNodeID = parentNodeId;

            bool result = CNController.add_node(paramsContainer.Tenant.Id, newNode, addMember: false);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                    ",\"NodeID\":\"" + newNode.NodeID.ToString() + "\"" +
                    ",\"Node\":" + newNode.toJson() + "}";

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                List<InlineTag> inlineTags = Expressions.get_tagged_items(description);

                foreach (InlineTag tg in inlineTags)
                {
                    TaggedType tgTp = TaggedType.None;
                    if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                    tagged.Add(new TaggedItem(newNode.NodeID.Value, tg.ID.Value, TagContextType.Node, tgTp));
                }

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, false, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = newNode.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNode,
                    SubjectID = newNode.NodeID,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\",\"Description\":\"" + Base64.encode(description) +
                        "\",\"ParentNodeID\":\"" + (newNode.ParentNodeID.HasValue ? newNode.ParentNodeID.Value.ToString() : string.Empty) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_node(Guid? nodeId, string name, string description, List<string> tags, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name) || tags.Any(u => !PublicMethods.is_secure_title(u)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, roleName: AccessRoleName.ManageOntology,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.ModifyNode_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            Node node = new Node()
            {
                NodeID = nodeId,
                Name = name,
                Description = description,
                Tags = tags,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.modify_node(paramsContainer.Tenant.Id, node);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = node.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyNode,
                    SubjectID = node.NodeID,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"" +
                        ",\"Description\":\"" + Base64.encode(description) + "\"" + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void change_node_type(Guid? nodeId, Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeId.HasValue || !nodeTypeId.HasValue ||
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.ChangeNodeType_PermissionFailed, nodeId);
                return;
            }

            bool result = CNController.change_node_type(paramsContainer.Tenant.Id,
                nodeId.Value, nodeTypeId.Value, paramsContainer.CurrentUserID.Value);

            List<Hierarchy> typeHierarchy = !result ? new List<Hierarchy>() :
                CNController.get_node_type_hierarchy(paramsContainer.Tenant.Id,
                nodeTypeId.Value).OrderByDescending(u => u.Level).ToList();

            string strHierarchy = "[";
            bool isFirst = true;
            foreach (Hierarchy hr in typeHierarchy)
            {
                strHierarchy += (isFirst ? string.Empty : ",") + "\"" + Base64.encode(hr.Name) + "\"";
                isFirst = false;
            }
            strHierarchy += "]";

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\",\"NodeType\":" + strHierarchy + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ChangeNodeType,
                    SubjectID = nodeId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void set_document_tree_node_id(Guid? nodeId, Guid? documentTreeNodeId,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Node nd = !nodeId.HasValue ? null :
                CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);

            //check access
            bool creatorLevel = nd != null && nd.Creator.UserID.HasValue && nd.isPersonal(nd.Creator.UserID.Value);

            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId, node: nd);

            bool hasAccess = accessChecker.checkNodeEditAccess(creatorLevel ? AdminLevel.Creator : AdminLevel.Node,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetDocumentTreeNodeID_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool result = CNController.set_document_tree_node_id(paramsContainer.Tenant.Id,
                nodeId.Value, documentTreeNodeId, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetDocumentTreeNodeID,
                    SubjectID = nodeId,
                    Info = "{\"DocumentTreeNodeID\":" +
                        (documentTreeNodeId.HasValue ? "\"" + documentTreeNodeId.ToString() + "\"" : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void modify_node_name(Guid? nodeId, string name, bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, checkWorkFlowEditPermission: checkWorkFlowEditPermission,
                roleName: AccessRoleName.ManageOntology, privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue)
                    _save_error_log(Modules.Log.Action.ModifyNodeName_PermissionFailed, nodeId, null, null);
                return;
            }
            //end of check access

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            Node node = new Node()
            {
                NodeID = nodeId,
                Name = name,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now,
            };

            bool result = CNController.modify_node_name(paramsContainer.Tenant.Id, node);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = node.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyNodeName,
                    SubjectID = node.NodeID,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_node_description(Guid? nodeId, string description,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, 
                checkWorkFlowEditPermission: checkWorkFlowEditPermission,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.ModifyNodeDescription_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            Node node = new Node()
            {
                NodeID = nodeId,
                Description = description,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now,
            };

            bool result = CNController.modify_node_description(paramsContainer.Tenant.Id, node);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                List<InlineTag> inlineTags = Expressions.get_tagged_items(description);

                foreach (InlineTag tg in inlineTags)
                {
                    TaggedType tgTp = TaggedType.None;
                    if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                    tagged.Add(new TaggedItem(nodeId.Value, tg.ID.Value, TagContextType.Node, tgTp));
                }

                if (inlineTags.Count == 0)
                    tagged.Add(new TaggedItem() { ContextID = nodeId });

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, true, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = node.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyNodeDescription,
                    SubjectID = node.NodeID,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_node_public_description(Guid? nodeId, string description,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, 
                checkWorkFlowEditPermission: checkWorkFlowEditPermission,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.ModifyNodePublicDescription_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool result = nodeId.HasValue &&
                CNController.modify_node_public_description(paramsContainer.Tenant.Id, nodeId.Value, description);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyNodePublicDescription,
                    SubjectID = nodeId,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_node_expiration_date(Guid? nodeId, DateTime? expirationDate, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.isKnowledge() && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetNodeExpirationDate_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool result = nodeId.HasValue &&
                CNController.set_node_expiration_date(paramsContainer.Tenant.Id, nodeId.Value, expirationDate);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"ExpirationDate\":\"" + (!expirationDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(expirationDate.Value)) + "\"" +
                ",\"Expired\":" + (expirationDate.HasValue && expirationDate.Value < DateTime.Now).ToString().ToLower() +
                "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetNodeExpirationDate,
                    SubjectID = nodeId,
                    Info = !expirationDate.HasValue ? string.Empty :
                        "{\"ExpirationDate\":\"" + expirationDate.Value.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_previous_version(Guid? nodeId, Guid? previousVersionId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Node nd = !nodeId.HasValue ? null : CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);

            //check access
            bool creatorLevel = nd != null && nd.Creator.UserID.HasValue && nd.isPersonal(nd.Creator.UserID.Value);

            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId, node: nd);

            bool hasAccess = accessChecker.checkNodeEditAccess(creatorLevel ? AdminLevel.Creator : AdminLevel.Node,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetPreviousVersion_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            if (previousVersionId.HasValue) {
                Node newVersion = CNController.get_new_versions(paramsContainer.Tenant.Id, previousVersionId.Value)
                    .Where(v => v.NodeID != nodeId && v.Status != Status.Rejected).FirstOrDefault();

                if (newVersion != null) {
                    responseText = "{\"ErrorText\":\"" + Messages.TheSelectedItemAlreadyHasANewVersion.ToString() + "\"" +
                        ",\"NewVersion\":" + newVersion.toJson() + "}";
                    return;
                }
            }

            if (previousVersionId == Guid.Empty) previousVersionId = null;

            bool result = nodeId.HasValue && previousVersionId != nodeId &&
                CNController.set_previous_version(paramsContainer.Tenant.Id,
                nodeId.Value, previousVersionId, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Update Node AdditionalID
            if (result) update_additional_id(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value);
            //end of Update Node AdditionalID

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPreviousVersion,
                    SubjectID = nodeId,
                    Info = "{\"PreviousVersionID\":" +
                        (previousVersionId.HasValue ? "\"" + previousVersionId.ToString() + "\"" : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_node_tags(Guid? nodeId, List<string> tags,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (tags != null && tags.Sum(u => string.IsNullOrEmpty(u) ? 0 : u.Length) > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (tags.Any(u => !PublicMethods.is_secure_title(u)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator,
                checkWorkFlowEditPermission: checkWorkFlowEditPermission,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.ModifyNodeTags_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            Node node = new Node()
            {
                NodeID = nodeId,
                Tags = tags,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = CNController.modify_node_tags(paramsContainer.Tenant.Id, node);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = node.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyNodeTags,
                    SubjectID = node.NodeID,
                    Info = "{\"Tags\":\"" + Base64.encode(ProviderUtil.get_tags_text(node.Tags)) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_node_description(Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<PermissionType> lstPT = new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract };

            if (!nodeId.HasValue ||
                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, nodeId.Value,
                    PrivacyObjectType.Node, lstPT).Count == 0)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            string desc = CNController.get_node_description(paramsContainer.Tenant.Id, nodeId.Value);

            responseText = "{\"NodeID\":\"" + nodeId.ToString() + "\",\"Description\":\"" + Base64.encode(desc) + "\"}";
        }

        public void set_nodes_searchability(Guid? nodeId, bool? searchable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node,
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetNodeSearchability_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool result = nodeId.HasValue && CNController.set_node_searchability(paramsContainer.Tenant.Id,
                nodeId.Value, searchable.HasValue && searchable.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetNodeSearchability,
                    SubjectID = nodeId,
                    Info = "{\"Searchable\":" + (searchable.HasValue && searchable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void remove_nodes(List<Guid> nodeIds, bool? removeHierarchy, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasRight = AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID);

            if (nodeIds.Count == 0 || (!hasRight && nodeIds.Count > 1))
            {
                responseText = "{\"ErrorText\":\"" + Messages.YouHaveNotPermission + "\"}";
                _save_error_log(Modules.Log.Action.RemoveNode_PermissionFailed, nodeIds, null, null);
                return;
            }
            else if (!hasRight && nodeIds.Count == 1)
            {
                Node node = CNController.get_node(paramsContainer.Tenant.Id, nodeIds.First());

                bool isServiceAdmin = false, isAreaAdmin = false, isCreator = false, isContributor = false,
                isExpert = false, isMember = false, isAdminMember = false, editable = false;

                CNController.get_user2node_status(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    node.NodeID.Value, ref isCreator, ref isContributor, ref isExpert, ref isMember,
                    ref isAdminMember, ref isServiceAdmin, ref isAreaAdmin, ref editable);

                if (!(PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                    isServiceAdmin || isAreaAdmin ||
                    (isCreator && (CNController.is_knowledge(paramsContainer.Tenant.Id, nodeIds[0]) ?
                        (node.Status != Status.Accepted) : (node.Status == Status.Personal ||
                        node.Status == Status.Rejected || node.Status == Status.NotSet))) ||
                    PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        nodeIds[0], PrivacyObjectType.Node, PermissionType.Delete)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.YouHaveNotPermission + "\"}";
                    _save_error_log(Modules.Log.Action.RemoveNode_PermissionFailed, nodeIds, null, null);
                    return;
                }
            }

            bool result = CNController.remove_nodes(paramsContainer.Tenant.Id, nodeIds,
                removeHierarchy, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Remove Notifications
            if (result) NotificationController.remove_notifications(paramsContainer.Tenant.Id,
                new Notification() { SubjectIDs = nodeIds, RefItemIDs = nodeIds });
            //end of Remove Notifications

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveNode,
                    Info = "{\"RemoveHierarchy\":" + (removeHierarchy.HasValue && removeHierarchy.Value).ToString().ToLower() + "}",
                    SubjectIDs = nodeIds,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void recycle_nodes(List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.YouHaveNotPermission + "\"}";
                _save_error_log(Modules.Log.Action.RecycleNode_PermissionFailed, nodeIds, null, null);
                return;
            }

            bool result = CNController.recycle_nodes(paramsContainer.Tenant.Id, nodeIds,
                paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RecycleNode,
                    SubjectIDs = nodeIds,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_node_icon(Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            responseText = !nodeId.HasValue || !DocumentUtilities.icon_exists(paramsContainer.Tenant.Id, nodeId.Value) ?
                "{\"IconURL\":\"\"}" :
                "{\"IconURL\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id, nodeId.Value) + "\",\"Icon\":" +
                DocumentUtilities.get_icon_json(paramsContainer.Tenant.Id, nodeId.Value) + "}";
        }

        protected void set_direct_parent(List<Guid> nodeIds, Guid? parentNodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access. note: only admin can move multiple nodes
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeIds.Count > 0 ? (Guid?)nodeIds.First() : null);

            bool hasAccess = nodeIds.Count > 1 ? accessChecker.checkAccessRole(AccessRoleName.ManageOntology) :
                accessChecker.checkNodeEditAccess(AdminLevel.Node, roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetNodeDirectParent, nodeIds);
                return;
            }
            //end of check access

            string errorMessage = string.Empty;

            bool result = CNController.set_direct_parent(paramsContainer.Tenant.Id,
                nodeIds, parentNodeId, paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetNodeDirectParent,
                    SubjectID = parentNodeId,
                    Info = "{\"NodeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeIds.Select(u => "\"" + u.ToString() + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_node_types_order(List<Guid> nodeTypeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SortNodeTypes_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = CNController.set_node_types_order(paramsContainer.Tenant.Id, nodeTypeIds);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_nodes_order(List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SortNodes_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = CNController.set_nodes_order(paramsContainer.Tenant.Id, nodeIds);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void add_relation(Guid? sourceNodeId, List<Guid> destinationNodeIds, Guid? relationTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: sourceNodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, roleName: AccessRoleName.ManageOntology,
                permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (sourceNodeId.HasValue) _save_error_log(Modules.Log.Action.AddNodeRelation_PermissionFailed, sourceNodeId);
                return;
            }
            //end of check access

            List<Relation> relations = new List<Relation>();

            foreach (Guid dnId in destinationNodeIds)
            {
                Relation newRelation = new Relation();
                newRelation.Source.NodeID = sourceNodeId;
                newRelation.Destination.NodeID = dnId;
                newRelation.RelationType.RelationTypeID = relationTypeId;

                relations.Add(newRelation);
            }

            bool result = CNController.add_relations(paramsContainer.Tenant.Id, relations, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNodeRelation,
                    SubjectID = sourceNodeId,
                    ThirdSubjectID = relationTypeId,
                    Info = "{\"DestinationNodeIDs\":[" +
                        ProviderUtil.list_to_string<string>(destinationNodeIds.Select(u => "\"" + u.ToString() + "\"").ToList()) +
                        "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void save_relations(Guid? nodeId, List<Guid> relatedNodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SaveRelations_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool result = nodeId.HasValue && CNController.save_relations(paramsContainer.Tenant.Id,
                nodeId.Value, relatedNodeIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SaveRelations,
                    SubjectID = nodeId,
                    Info = "{\"DestinationNodeIDs\":[" +
                        ProviderUtil.list_to_string<string>(relatedNodeIds.Select(u => "\"" + u.ToString() + "\"").ToList()) +
                        "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_relation(Guid? sourceNodeId, Guid? destinationNodeId, Guid? relationTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveNodeRelation_PermissionFailed, sourceNodeId);
                return;
            }

            Relation relation = new Relation();
            relation.Source.NodeID = sourceNodeId;
            relation.Destination.NodeID = destinationNodeId;

            bool result = sourceNodeId.HasValue && destinationNodeId.HasValue &&
                CNController.remove_relation(paramsContainer.Tenant.Id,
                relation, relationTypeId, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveNodeRelation,
                    SubjectID = sourceNodeId,
                    SecondSubjectID = destinationNodeId,
                    ThirdSubjectID = relationTypeId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void connect_disconnect_nodes(Guid? nodeId, List<Guid> lstNodeIds, bool connect, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (!nodeId.HasValue || lstNodeIds.Count == 0) return;

            List<Relation> lstRelations = new List<Relation>();
            foreach (Guid destinationId in lstNodeIds)
            {
                Relation newRelation = new Relation();

                newRelation.Source.NodeID = nodeId;
                newRelation.Destination.NodeID = destinationId;

                lstRelations.Add(newRelation);
            }

            bool result = false;

            if (connect) result = CNController.make_correlations(paramsContainer.Tenant.Id,
                ref lstRelations, paramsContainer.CurrentUserID.Value);
            else result = CNController.remove_relations(paramsContainer.Tenant.Id,
                lstRelations, null, paramsContainer.CurrentUserID.Value, true);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected Node _get_node(Guid nodeId)
        {
            return CNController.get_node(paramsContainer.Tenant.Id, nodeId, true);
        }

        protected void get_node(Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            Node node = null;

            if (!nodeId.HasValue || (node = CNController.get_node(paramsContainer.Tenant.Id,
                nodeId.Value, true, paramsContainer.CurrentUserID)) == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool hasChild = CNController.has_childs(paramsContainer.Tenant.Id, nodeId.Value);

            List<PermissionType> permissions = PrivacyController.check_access(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, nodeId.Value, PrivacyObjectType.Node);

            bool hasModifyPermission = permissions.Any(u => u == PermissionType.Modify);

            List<NodeCreator> contributors = CNController.get_node_creators(paramsContainer.Tenant.Id, nodeId.Value, true);

            Service service = CNController.get_service(paramsContainer.Tenant.Id, node.NodeTypeID.Value);
            if (service == null) service = new Service();

            if (!service.EditableForAdmin.HasValue) service.EditableForAdmin = false;
            if (!service.EditableForCreator.HasValue) service.EditableForCreator = false;
            if (!service.EditableForContributors.HasValue) service.EditableForContributors = false;

            bool isCreator = false, isContributor = false, isExpert = false, isMember = false, isAdminMember = false,
                isServiceAdmin = false, isAreaAdmin = false, perCreatorLevel = false;

            bool isSystemAdmin = paramsContainer.CurrentUserID.HasValue &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            bool isPersonal = node.Status == Status.NotSet || node.Status == Status.Personal ||
                    node.Status == Status.SentBackForRevision || node.Status == Status.Rejected;

            if (paramsContainer.CurrentUserID.HasValue)
            {
                CNController.get_user2node_status(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    nodeId.Value, ref isCreator, ref isContributor, ref isExpert, ref isMember, ref isAdminMember,
                    ref isServiceAdmin, ref isAreaAdmin, ref perCreatorLevel);

                if ((isCreator || isContributor) && isPersonal) perCreatorLevel = true;
            }

            //Check Access to Node
            bool hasWorkFlowPermission = false, hasKnowledgePermission = false,
                hasWFEditPermission = false, hideContributors = false;

            check_node_workflow_permissions(node, service == null ? false : service.IsKnowledge, isSystemAdmin,
                isServiceAdmin, isAreaAdmin, isCreator, ref hasKnowledgePermission,
                ref hasWorkFlowPermission, ref hasWFEditPermission, ref hideContributors);

            bool hasAccess = isSystemAdmin || isServiceAdmin || isAreaAdmin || isCreator ||
                isContributor || isExpert || isMember || hasWorkFlowPermission || hasKnowledgePermission;

            PermissionType viewPermission = hasAccess ? PermissionType.View : PermissionType.None;

            if (!hasAccess)
            {
                List<PermissionType> granted = PrivacyController.check_access(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID, nodeId.Value, PrivacyObjectType.Node,
                    new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract });

                hasAccess = granted.Count > 0;

                if (granted.Any(u => u == PermissionType.View)) viewPermission = PermissionType.View;
                else if (granted.Any(u => u == PermissionType.ViewAbstract)) viewPermission = PermissionType.ViewAbstract;
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";

                //Save Log
                try
                {
                    LogController.save_log(paramsContainer.Tenant.Id, new Log()
                    {
                        UserID = paramsContainer.CurrentUserID,
                        HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                        HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                        Action = Modules.Log.Action.Node_AccessDenied,
                        SubjectID = nodeId,
                        Info = "{\"Error\":\"" + Base64.encode(Messages.AccessDenied.ToString()) + "\"}",
                        ModuleIdentifier = ModuleIdentifier.CN
                    });
                }
                catch { }
                //end of Save Log

                return;
            }
            //end of Check Access to Node
            
            bool perSystemAdminLevel = isSystemAdmin;
            bool perServiceAdminLevel = perSystemAdminLevel || isServiceAdmin;
            bool perAreaAdminLevel = perServiceAdminLevel || isAreaAdmin;

            List<Hierarchy> nameHierarchy = !service.IsTree.HasValue || !service.IsTree.Value ? new List<Hierarchy>() :
                CNController.get_node_hierarchy(paramsContainer.Tenant.Id,
                node.NodeID.Value).OrderByDescending(u => u.Level).ToList();

            List<Hierarchy> typeHierarchy = !node.NodeTypeID.HasValue ? new List<Hierarchy>() :
                CNController.get_node_type_hierarchy(paramsContainer.Tenant.Id,
                node.NodeTypeID.Value).OrderByDescending(u => u.Level).ToList();

            List<DocFileInfo> attachedFiles = viewPermission != PermissionType.View ? new List<DocFileInfo>() :
                DocumentsController.get_owner_files(paramsContainer.Tenant.Id, nodeId.Value, FileOwnerTypes.Node);

            string strNodeType = "[" + string.Join(",", typeHierarchy.Select(
                h => "{\"ID\":\"" + h.ID.ToString() + "\"" +
                ",\"Name\":\"" + Base64.encode(h.Name) + "\"" + "}")) + "]";

            string strNameHierarchy = "[" + string.Join(",", nameHierarchy.Select(
                h => "{\"ID\":\"" + h.ID.ToString() + "\"" +
                ",\"Name\":\"" + Base64.encode(h.Name) + "\"" + "}")) + "]";

            if (!service.EnableContribution.HasValue) service.EnableContribution = false;

            string strOwner = ",\"Owner\":{\"NodeID\":\"" + (node.OwnerID.HasValue ?
                node.OwnerID.Value.ToString() : string.Empty) + "\",\"Name\":\"" + Base64.encode(node.OwnerName) + "\"}";
            string strCreator = ",\"Creator\":" + node.Creator.toJson(paramsContainer.Tenant.Id, true);
            string strIconUrl = ",\"IconURL\":{\"Editable\":" + perCreatorLevel.ToString().ToLower() +
                ",\"Value\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                    nodeId.Value, DefaultIconTypes.Node, node.NodeTypeID) + "\"" +
                ",\"HighQuality\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id, nodeId.Value, null, true) + "\"" +
                "}";
            string strCoverUrl = ",\"CoverPhotoURL\":{\"Editable\":" + perCreatorLevel.ToString().ToLower() +
                ",\"Value\":\"" + DocumentUtilities.get_cover_photo_url(paramsContainer.Tenant.Id, nodeId.Value, false, false) + "\"" +
                ",\"HighQuality\":\"" + DocumentUtilities.get_cover_photo_url(paramsContainer.Tenant.Id, nodeId.Value, false, true) + "\"" +
                "}";
            strNameHierarchy = ",\"NameHierarchy\":{\"Editable\":" + (service.IsTree.HasValue && service.IsTree.Value &&
                perSystemAdminLevel).ToString().ToLower() + ",\"Value\":" + strNameHierarchy + "}";
            string strType = ",\"NodeType\":{\"Editable\":" + perSystemAdminLevel.ToString().ToLower() + ",\"Value\":" + strNodeType + "}";
            string strAdminArea = ",\"AdminArea\":{\"Editable\":" +
                (!node.AdminAreaID.HasValue ? perCreatorLevel : perServiceAdminLevel).ToString().ToLower() +
                ",\"Value\":{\"NodeID\":\"" + (node.AdminAreaID.HasValue ? node.AdminAreaID.ToString() : string.Empty) +
                "\",\"NodeName\":\"" + Base64.encode(node.AdminAreaName) +
                "\",\"NodeType\":\"" + Base64.encode(node.AdminAreaType) + "\"}}";


            bool hasConfPermission = hasModifyPermission || perAreaAdminLevel || (
                paramsContainer.CurrentUserID.HasValue && service.IsKnowledge.HasValue && 
                service.IsKnowledge.Value && node.isPersonal(paramsContainer.CurrentUserID.Value));
            string strConfidentiality = ",\"ConfidentialityLevel\":{\"Editable\":" + hasConfPermission.ToString().ToLower() +
                ",\"Value\":" + node.ConfidentialityLevel.toJson() + "}";


            string strPreviousVersion = ",\"PreviousVersion\":{\"Editable\":" + (hasModifyPermission || perAreaAdminLevel).ToString().ToLower() +
                ",\"Value\":{\"ID\":\"" + (node.PreviousVersionID.HasValue ? node.PreviousVersionID.ToString() : string.Empty) +
                "\",\"Name\":\"" + Base64.encode(node.PreviousVersionName) + "\"}}";
            string strName = ",\"Name\":{\"Editable\":" + (hasModifyPermission || perCreatorLevel).ToString().ToLower() +
                ",\"Value\":\"" + Base64.encode(node.Name) + "\"}";
            string strDescription = ",\"Description\":{\"Editable\":" + (hasModifyPermission || perCreatorLevel).ToString().ToLower() +
                ",\"Value\":\"" + Base64.encode(node.Description) + "\"}";
            string strPublicDescription = viewPermission != PermissionType.View || (!perCreatorLevel && !isAdminMember && !hasModifyPermission) ? string.Empty :
                ",\"PublicDescription\":{\"Editable\":" + (viewPermission == PermissionType.View && (hasModifyPermission || perCreatorLevel || isAdminMember)).ToString().ToLower() +
                ",\"Value\":\"" + Base64.encode(node.PublicDescription) + "\"}";
            string strExpirationDate = !Modules.RaaiVanConfig.Modules.Knowledge(paramsContainer.Tenant.Id) ||
                !service.IsKnowledge.HasValue || !service.IsKnowledge.Value || !perAreaAdminLevel ? string.Empty :
                ",\"ExpirationDate\":{\"Editable\":" + (hasModifyPermission || perAreaAdminLevel).ToString().ToLower() +
                ",\"Value\":\"" + (!node.ExpirationDate.HasValue ? string.Empty :
                GenericDate.get_local_date(node.ExpirationDate.Value)) + "\"" +
                ",\"Expired\":" + (node.ExpirationDate.HasValue && node.ExpirationDate < DateTime.Now).ToString().ToLower() +
                "}";
            string strSearchable = ",\"Searchable\":{\"Editable\":" + (hasModifyPermission || perAreaAdminLevel).ToString().ToLower() +
                ",\"Value\":" + (node.Searchable.HasValue && node.Searchable.Value).ToString().ToLower() + "}";
            string strAttachedFiles = ",\"AttachedFiles\":{\"Editable\":" +
                (viewPermission == PermissionType.View && (hasModifyPermission || (node.Status == Status.Accepted ? perAreaAdminLevel : perCreatorLevel))).ToString().ToLower() +
                ",\"Value\":" + DocumentUtilities.get_files_json(attachedFiles, true) + "}";

            string strNewVersions = ",\"NewVersions\":[";
            if (service.EnablePreviousVersionSelect.HasValue && 
                service.EnablePreviousVersionSelect.Value && viewPermission == PermissionType.View)
            {
                List<Node> newVersions = CNController.get_new_versions(paramsContainer.Tenant.Id, nodeId.Value);

                strNewVersions += string.Join(",", newVersions.Select(
                    v => "{\"NodeID\":\"" + v.NodeID.ToString() + "\"" + ",\"Name\":\"" + Base64.encode(v.Name) + "\"" + "}"));
            }
            strNewVersions += "]";

            string strKeywords = string.Empty;
            foreach (string tg in node.Tags)
                strKeywords += (string.IsNullOrEmpty(strKeywords) ? string.Empty : ",") + "\"" + Base64.encode(tg) + "\"";
            strKeywords = ",\"Keywords\":{\"Editable\":" + (hasModifyPermission || perCreatorLevel).ToString().ToLower() + ",\"Value\":[" + strKeywords + "]}";

            string strDocumentTree = string.Empty;
            if (Modules.RaaiVanConfig.Modules.Documents(paramsContainer.Tenant.Id))
            {
                List<Hierarchy> treeNodeHierarchy = !service.IsDocument.HasValue || !service.IsDocument.Value ||
                    !node.DocumentTreeNodeID.HasValue ? new List<Hierarchy>() :
                    DocumentsController.get_tree_node_hierarchy(paramsContainer.Tenant.Id,
                    node.DocumentTreeNodeID.Value).OrderByDescending(u => u.Level).ToList();

                foreach (Hierarchy hr in treeNodeHierarchy) strDocumentTree += (string.IsNullOrEmpty(strDocumentTree) ? string.Empty : ",") +
                    "{\"TreeNodeID\":\"" + hr.ID.ToString() + "\",\"Name\":\"" + Base64.encode(hr.Name) + "\"}";

                bool docTreeEditable = !node.DocumentTreeID.HasValue || isPersonal || node.Status == Status.SentToAdmin ?
                    perCreatorLevel : perAreaAdminLevel;

                strDocumentTree = ",\"DocumentTree\":{\"Editable\":" + (hasModifyPermission || docTreeEditable).ToString().ToLower() +
                    ",\"Value\":{\"ID\":\"" + (!node.DocumentTreeID.HasValue ? string.Empty :
                    node.DocumentTreeID.ToString()) + "\"" +
                    ",\"Name\":\"" + Base64.encode(node.DocumentTreeName) + "\",\"Childs\":[" + strDocumentTree + "]}}";
            }

            string strKnowledgeSettings = string.Empty;

            bool isFreeUser = node.IsFreeUser.HasValue && node.IsFreeUser.Value;

            if (service.IsKnowledge.HasValue && service.IsKnowledge.Value && paramsContainer.IsAuthenticated)
            {
                KnowledgeType knowledgeType =
                    KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, node.NodeTypeID.Value);
                if (knowledgeType == null) knowledgeType = new KnowledgeType();

                bool evaluationNeeded = knowledgeType.Evaluators != KnowledgeEvaluators.NotSet;

                Dashboard evaluatorDashboard = !paramsContainer.CurrentUserID.HasValue || node.Status != Status.SentToEvaluators ? null :
                    NotificationController.get_dashboards(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null, nodeId,
                    null, DashboardType.Knowledge, DashboardSubType.Evaluator, null, null, null, null, null, null, null).FirstOrDefault();

                bool isEvaluator = evaluatorDashboard != null && evaluatorDashboard.NodeID.HasValue;
                bool hasEvaluated = isEvaluator && evaluatorDashboard.Done.HasValue && evaluatorDashboard.Done.Value;

                if (knowledgeType.UnhideNodeCreators.HasValue && knowledgeType.UnhideNodeCreators.Value)
                    hideContributors = false;
                else if (isEvaluator && !perAreaAdminLevel) hideContributors = true;

                if (hideContributors) strCreator = string.Empty;

                bool isDirector = isSystemAdmin || isServiceAdmin || isAreaAdmin;
                if (!isDirector && node.Status != Status.Accepted && node.Status != Status.Rejected && node.Status != Status.NotSet)
                {
                    List<DashboardSubType> st = new List<DashboardSubType>();
                    st.Add(DashboardSubType.Admin);
                    st.Add(DashboardSubType.EvaluationDone);
                    st.Add(DashboardSubType.EvaluationRefused);

                    isDirector = NotificationController.get_dashboards(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, null, nodeId, null, DashboardType.Knowledge, DashboardSubType.NotSet,
                        null, false, null, null, null, null, null).Exists(u => st.Exists(v => v == u.SubType));
                }

                bool kwfPermission = isDirector && (node.Status == Status.SentToAdmin || node.Status == Status.SentToEvaluators);
                if (kwfPermission == false)
                {
                    bool beforeWorkflow = (node.Status == Status.NotSet || node.Status == Status.Personal ||
                        node.Status == Status.Rejected || node.Status == Status.SentBackForRevision) && (isDirector || isCreator);

                    bool afterWorkflow = (node.Status == Status.Accepted || node.Status == Status.Rejected) && (isDirector || isCreator);

                    bool inWorkFlow = !beforeWorkflow && !afterWorkflow && (isDirector || isCreator) &&
                        NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        null, nodeId, DashboardType.Knowledge, null, null, false);

                    kwfPermission = beforeWorkflow || afterWorkflow || inWorkFlow ||
                        NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId, DashboardType.Knowledge, null, null, false) ||
                        NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId, DashboardType.Knowledge, DashboardSubType.Evaluator, null, null);
                }

                bool kwfActionExists = isPersonal && node.Status != Status.Accepted &&
                    (isDirector || (isCreator && isFreeUser));
                kwfActionExists = kwfActionExists || (node.Status == Status.SentToAdmin && isDirector);
                kwfActionExists = kwfActionExists ||
                    (node.Status == Status.SentToEvaluators && (isDirector || isCreator || isContributor));
                kwfActionExists = kwfActionExists || (node.Status != Status.Accepted && isEvaluator);

                bool preEvaluated = knowledgeType.PreEvaluateByOwner.HasValue &&
                    knowledgeType.PreEvaluateByOwner.Value && node.Creator.UserID.HasValue &&
                    KnowledgeController.has_evaluated(paramsContainer.Tenant.Id, nodeId.Value, node.Creator.UserID.Value);

                strKnowledgeSettings = ",\"EvaluationNeeded\":" + evaluationNeeded.ToString().ToLower() +
                    ",\"Evaluators\":\"" + (knowledgeType.Evaluators == KnowledgeEvaluators.NotSet ? string.Empty :
                        knowledgeType.Evaluators.ToString()) + "\"" +
                    ",\"PreEvaluateByOwner\":" + (knowledgeType.PreEvaluateByOwner.HasValue &&
                        knowledgeType.PreEvaluateByOwner.Value).ToString().ToLower() +
                    ",\"ForceEvaluatorsDescribe\":" + (knowledgeType.ForceEvaluatorsDescribe.HasValue &&
                        knowledgeType.ForceEvaluatorsDescribe.Value).ToString().ToLower() +
                    ",\"PreEvaluated\":" + preEvaluated.ToString().ToLower() +
                    ",\"NodeSelectType\":\"" + (knowledgeType.NodeSelectType == KnowledgeNodeSelectType.NotSet ? string.Empty :
                        knowledgeType.NodeSelectType.ToString()) + "\"" +
                    ",\"IsEvaluator\":" + isEvaluator.ToString().ToLower() +
                    ",\"HasEvaluated\":" + hasEvaluated.ToString().ToLower() +
                    ",\"EvaluationsEditable\":" + (knowledgeType.EvaluationsEditable.HasValue &&
                        knowledgeType.EvaluationsEditable.Value).ToString().ToLower() +
                    ",\"EvaluationsEditableForAdmin\":" + (knowledgeType.EvaluationsEditableForAdmin.HasValue &&
                        knowledgeType.EvaluationsEditableForAdmin.Value).ToString().ToLower() +
                    ",\"IsDirector\":" + isDirector.ToString().ToLower() +
                    ",\"KWFPermission\":" + kwfPermission.ToString().ToLower() +
                    ",\"KWFActionExists\":" + kwfActionExists.ToString().ToLower() +
                    ",\"ScoreScale\":" + (knowledgeType.ScoreScale.HasValue ? knowledgeType.ScoreScale : 0).ToString() +
                    ",\"UnhideEvaluators\":" + (knowledgeType.UnhideEvaluators.HasValue &&
                        knowledgeType.UnhideEvaluators.Value).ToString().ToLower() +
                    ",\"UnhideEvaluations\":" + (knowledgeType.UnhideEvaluations.HasValue &&
                        knowledgeType.UnhideEvaluations.Value).ToString().ToLower() +
                    ",\"UnhideNodeCreators\":" + (knowledgeType.UnhideNodeCreators.HasValue &&
                        knowledgeType.UnhideNodeCreators.Value).ToString().ToLower() +
                    ",\"TextOptions\":\"" + Base64.encode(knowledgeType.TextOptions) + "\"";
            }

            //Set contributors
            string strContributors = string.Empty;
            foreach (NodeCreator nc in contributors)
            {
                strContributors += (string.IsNullOrEmpty(strContributors) ? string.Empty : ",") +
                    "{\"UserID\":\"" + nc.User.UserID.ToString() + "\"" +
                    ",\"UserName\":\"" + (hideContributors ? string.Empty : Base64.encode(nc.User.UserName)) + "\"" +
                    ",\"FirstName\":\"" + (hideContributors ? string.Empty : Base64.encode(nc.User.FirstName)) + "\"" +
                    ",\"LastName\":\"" + (hideContributors ? string.Empty : Base64.encode(nc.User.LastName)) + "\"" +
                    ",\"ProfileImageURL\":\"" + (DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        hideContributors ? Guid.NewGuid() : nc.User.UserID.Value)) + "\"" +
                    ",\"Share\":\"" + (nc.CollaborationShare.HasValue ? nc.CollaborationShare.Value : 0).ToString() + "\"" +
                    "}";
            }

            strContributors = viewPermission != PermissionType.View ? string.Empty :
                ",\"Contributors\":{\"Editable\":" + (!hideContributors &&
                    (hasModifyPermission || perAreaAdminLevel || (isPersonal && isCreator))).ToString().ToLower() +
                    ",\"Value\":[" + strContributors + "]}";
            //end of Set contributors

            if (service.IsKnowledge.HasValue && service.IsKnowledge.Value)
            {
                double totalFinancialFeedbacks = 0, totalTemporalFeedbacks = 0, financialFeedbackStatus = 0, temporalFeedbackStatus = 0;
                KnowledgeController.get_feedback_status(paramsContainer.Tenant.Id,
                    nodeId.Value, paramsContainer.CurrentUserID, ref totalFinancialFeedbacks,
                    ref totalTemporalFeedbacks, ref financialFeedbackStatus, ref temporalFeedbackStatus);

                strKnowledgeSettings += ",\"TotalFinancialFeedbacks\":" + totalFinancialFeedbacks.ToString() +
                    ",\"TotalTemporalFeedbacks\":" + totalTemporalFeedbacks.ToString() +
                    ",\"FinancialFeedbackStatus\":" + financialFeedbackStatus.ToString() +
                    ",\"TemporalFeedbackStatus\":" + temporalFeedbackStatus.ToString();
            }

            bool forcedMembership = isMember && node.TypeAdditionalID ==
                CNUtilities.get_node_type_additional_id(NodeTypes.Department).ToString();

            bool removable = perAreaAdminLevel ||
                (isCreator && (service.IsKnowledge.HasValue && service.IsKnowledge.Value ?
                (node.Status != Status.Accepted) :
                (node.Status == Status.Rejected || node.Status == Status.Personal || node.Status == Status.NotSet))) ||
                permissions.Any(u => u == PermissionType.Delete);

            int membershipRequestsCount = 0;

            if (isAdminMember || perAreaAdminLevel)
            {
                membershipRequestsCount =
                    CNController.get_members_count(paramsContainer.Tenant.Id, nodeId.Value, NodeMemberStatuses.Pending);
            }

            FormType formInstance = null;
            if (Modules.RaaiVanConfig.Modules.FormGenerator(paramsContainer.Tenant.Id) &&
                CNController.has_extension(paramsContainer.Tenant.Id, node.NodeID.Value, ExtensionType.Form))
            {
                FormType frm = FGController.get_owner_form(paramsContainer.Tenant.Id, node.NodeTypeID.Value);
                if (frm != null && frm.FormID.HasValue) formInstance = FGController.get_owner_form_instances(
                     paramsContainer.Tenant.Id, node.NodeID.Value, frm.FormID).FirstOrDefault();
            }

            bool nodeEditable = viewPermission == PermissionType.View && (perCreatorLevel || hasWFEditPermission ||
                    permissions.Any(u => u == PermissionType.Modify));

            //Related Nodes
            bool hideRelations = viewPermission != PermissionType.View;

            if (!(nodeEditable && !(service.DisableRelatedNodesSelect.HasValue && service.DisableRelatedNodesSelect.Value)) &&
                !permissions.Any(p => p == PermissionType.ViewRelatedItems)) hideRelations = true;

            string strRelations = hideRelations ? ",\"Relations\":{\"Hide\":true}" : string.Empty;
            //end of Related Nodes
            
            responseText = "{\"NodeID\":\"" + node.NodeID.Value.ToString() + "\"" +
                ",\"NodeTypeID\":\"" + node.NodeTypeID.Value.ToString() + "\"" +
                ",\"AdditionalID\":\"" + node.AdditionalID + "\"" +
                ",\"IsRegisterer\":" + isCreator.ToString().ToLower() +
                ",\"IsContributor\":" + isContributor.ToString().ToLower() +
                ",\"HideCreators\":" + (node.HideCreators.HasValue && node.HideCreators.Value).ToString().ToLower() +
                ",\"AdminType\":\"" + service.AdminType.ToString() + "\"" +
                ",\"Editable\":" + nodeEditable.ToString().ToLower() +
                ",\"HasWorkFlowEditPermission\":" + hasWFEditPermission.ToString().ToLower() +
                ",\"Removable\":" + removable.ToString().ToLower() +
                ",\"ViewPermission\":\"" + (viewPermission == PermissionType.None ?
                    string.Empty : viewPermission.ToString()) + "\"" +
                ",\"Permissions\":{" + string.Join(",", permissions.Select(u => "\"" + u.ToString() + "\":true")) + "}" +
                ",\"IsAreaAdmin\":" + isAreaAdmin.ToString().ToLower() +
                ",\"IsServiceAdmin\":" + isServiceAdmin.ToString().ToLower() +
                ",\"IsSystemAdmin\":" + isSystemAdmin.ToString().ToLower() +
                ",\"IsFreeUser\":" + isFreeUser.ToString().ToLower() +
                ",\"IsExpert\":" + isExpert.ToString().ToLower() +
                ",\"IsAdmin\":" + isAdminMember.ToString().ToLower() +
                ",\"IsMember\":" + isMember.ToString().ToLower() +
                ",\"MembershipRequestsCount\":" + membershipRequestsCount.ToString() +
                ",\"ForcedMembership\":" + forcedMembership.ToString().ToLower() +
                ",\"NoContentService\":" + (service.NoContent.HasValue && service.NoContent.Value).ToString().ToLower() +
                ",\"IsKnowledge\":" + (service.IsKnowledge.HasValue && service.IsKnowledge.Value).ToString().ToLower() +
                ",\"IsDocument\":" + (service.IsDocument.HasValue && service.IsDocument.Value).ToString().ToLower() +
                ",\"EnablePreviousVersionSelect\":" + (service.EnablePreviousVersionSelect.HasValue && 
                    service.EnablePreviousVersionSelect.Value).ToString().ToLower() +
                ",\"EditSuggestion\":" + (service.EditSuggestion.HasValue && service.EditSuggestion.Value).ToString().ToLower() +
                ",\"IsTree\":" + (service.IsTree.HasValue && service.IsTree.Value).ToString().ToLower() +
                ",\"DisableAbstractAndKeywords\":" + (service.DisableAbstractAndKeywords.HasValue && service.DisableAbstractAndKeywords.Value).ToString().ToLower() +
                ",\"DisableFileUpload\":" + (service.DisableFileUpload.HasValue && service.DisableFileUpload.Value).ToString().ToLower() +
                ",\"DisableRelatedNodesSelect\":" + (service.DisableRelatedNodesSelect.HasValue && service.DisableRelatedNodesSelect.Value).ToString().ToLower() +
                ",\"HasChild\":" + hasChild.ToString().ToLower() +
                ",\"Contribution\":" + service.EnableContribution.ToString().ToLower() +
                ",\"HasWikiContent\":" + (node.HasWikiContent.HasValue && node.HasWikiContent.Value).ToString().ToLower() +
                ",\"HasFormContent\":" + (node.HasFormContent.HasValue && node.HasFormContent.Value).ToString().ToLower() +
                ",\"CreationDate\":\"" + (node.CreationDate.HasValue ?
                    GenericDate.get_local_date(node.CreationDate.Value, true) : string.Empty) + "\"" +
                ",\"PublicationDate\":\"" + (node.PublicationDate.HasValue ?
                    GenericDate.get_local_date(node.PublicationDate.Value) : string.Empty) + "\"" +
                ",\"Score\":" + (node.Score.HasValue ? node.Score.Value.ToString() : "0") +
                ",\"Status\":\"" + (node.Status != Status.NotSet ? node.Status.ToString() : string.Empty) + "\"" +
                ",\"LikeStatus\":" + (node.LikeStatus.HasValue ? node.LikeStatus : false).ToString().ToLower() +
                ",\"LikesCount\":" + (node.LikesCount.HasValue ? node.LikesCount.ToString() : "0") +
                ",\"MembershipStatus\":\"" + (string.IsNullOrEmpty(node.MembershipStatus) ?
                    string.Empty : node.MembershipStatus) + "\"" +
                ",\"VisitsCount\":" + (node.VisitsCount.HasValue ? node.VisitsCount.ToString() : "0") +
                ",\"PDFCovers\":[" + string.Join(",", DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                    node.NodeTypeID.Value , FileOwnerTypes.PDFCover).Select(u => u.toJson())) + "]" +
                (formInstance == null ? string.Empty : 
                    ",\"FormInstance\":" + formInstance.toJson(paramsContainer.Tenant.Id)) +
                strOwner + strCreator + strIconUrl + strCoverUrl + strNameHierarchy + strType + strAdminArea +
                strConfidentiality + strName + strDescription + strPublicDescription + strExpirationDate +
                strKeywords + strDocumentTree + strContributors + strAttachedFiles + strPreviousVersion +
                strNewVersions + strSearchable + strKnowledgeSettings + strRelations + "}";
        }

        protected void get_nodes(List<Guid> nodeTypeIds, bool? useNodeTypeHierarchy, List<Guid> relatedToIds,
            string searchText, bool? isDocument, bool? isKnowledge, bool? isMine, List<Guid> creatorUserIds, bool? archive,
            DateTime? lowerCreationDateLimit, DateTime? upperCreationDateLimit, int? count, long? lowerBoundary, 
            bool? searchable, bool? hasChild, bool? isFavorite, bool? isGroup, bool? isExpertiseDomain,
            List<FormFilter> filters, bool? matchAllFilters, bool? fetchCounts, Guid? groupByElementId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;
            
            if (creatorUserIds != null && creatorUserIds.Count > 0) isMine = null;
            else if (isMine.HasValue && isMine.Value && paramsContainer.CurrentUserID.HasValue)
                creatorUserIds = new List<Guid>() { paramsContainer.CurrentUserID.Value };

            bool hasViewAccess = paramsContainer.CurrentUserID.HasValue &&
                AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID);
            bool isAdmin = paramsContainer.CurrentUserID.HasValue &&
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                (nodeTypeIds.Count == 1 &&
                CNController.is_service_admin(paramsContainer.Tenant.Id, nodeTypeIds[0], paramsContainer.CurrentUserID.Value)));

            hasViewAccess = hasViewAccess || isAdmin;

            archive = archive.HasValue && archive.Value && paramsContainer.CurrentUserID.HasValue && hasViewAccess;

            bool? grabNoContentServices = null;
            if (paramsContainer.CurrentUserID.HasValue && nodeTypeIds != null && nodeTypeIds.Count == 1 && isAdmin)
                grabNoContentServices = true;

            if (!count.HasValue || count <= 0) count = 20;

            if (groupByElementId.HasValue && groupByElementId != Guid.Empty && nodeTypeIds.Count == 1)
            {
                responseText = PublicMethods.toJSON(CNController.get_nodes_grouped(applicationId: paramsContainer.Tenant.Id, 
                    nodeTypeId: nodeTypeIds[0], groupByFormElementId: groupByElementId.Value, relatedToIds: relatedToIds, 
                    searchText: searchText, lowerCreationDateLimit: lowerCreationDateLimit, 
                    upperCreationDateLimit: upperCreationDateLimit, searchable: searchable,
                    isFavorite: isFavorite, isGroup: isGroup, isExpertiseDomain: isExpertiseDomain, filters: filters, 
                    matchAllFilters: matchAllFilters, currentUserId: paramsContainer.CurrentUserID, 
                    creatorUserIds: creatorUserIds, checkAccess: !hasViewAccess));
            }
            else {
                long totalCount = 0;
                List<NodesCount> nodesCount = new List<NodesCount>();

                List<Node> nodes = CNController.get_nodes(applicationId: paramsContainer.Tenant.Id,
                    totalCount: ref totalCount, nodesCount: ref nodesCount,
                    nodeTypeIds: nodeTypeIds, useNodeTypeHierarchy: useNodeTypeHierarchy, relatedToIds: relatedToIds, 
                    searchText: searchText, isDocument: isDocument, isKnowledge: isKnowledge,
                    lowerCreationDateLimit: lowerCreationDateLimit, upperCreationDateLimit: upperCreationDateLimit,
                    count: count.Value, lowerBoundary: lowerBoundary, archive: archive.HasValue && archive.Value,
                    searchable: searchable, grabNoContentServices: grabNoContentServices, 
                    isFavorite: isFavorite, isGroup: isGroup, isExpertiseDomain: isExpertiseDomain, filters: filters,
                    matchAllFilters: matchAllFilters, fetchCounts: fetchCounts, currentUserId: paramsContainer.CurrentUserID,
                    creatorUserIds: creatorUserIds, checkAccess: !hasViewAccess);

                List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                    CNController.have_childs(paramsContainer.Tenant.Id, nodes.Select(u => u.NodeID.Value).ToList());

                nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

                responseText = "{\"TotalCount\":" + totalCount.ToString() +
                    ",\"Count\":[" + string.Join(",", nodesCount.Select(u => u.toJson())) + "]" +
                    ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]" +
                    "}";
            }
        }

        protected void get_nodes(List<Guid> nodeIds, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!paramsContainer.CurrentUserID.HasValue ||
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                List<PermissionType> lstPT = new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract };

                nodeIds = PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    nodeIds, PrivacyObjectType.Node, lstPT).Keys.ToList();
            }

            List<Node> nodes = CNController.get_nodes(paramsContainer.Tenant.Id, nodeIds, full: null, currentUserId: null);

            responseText = "{\"Nodes\":[" + string.Join(",", nodes.Select(
                u => u.toJson(paramsContainer.Tenant.Id, iconUrl: true)).ToList()) + "]" + "}";
        }

        protected void get_most_popular_nodes(List<Guid> nodeTypeIds, Guid? parentNodeId,
            int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!count.HasValue || count <= 0) count = 20;

            long totalCount = 0;

            List<Node> nodes = CNController.get_most_popular_nodes(paramsContainer.Tenant.Id,
                nodeTypeIds, parentNodeId, count.Value, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson()).ToList()) + "]" + "}";
        }

        protected void get_parent_nodes(Guid? nodeId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<Hierarchy> nodes = !nodeId.HasValue ? new List<Hierarchy>() :
                CNController.get_node_hierarchy(paramsContainer.Tenant.Id, nodeId.Value, true);

            responseText = "{\"Nodes\":[" +
                ProviderUtil.list_to_string<string>(nodes.Select(u => u.toJSON()).ToList()) + "]}";
        }

        protected void get_child_nodes(Guid? nodeId, Guid? nodeTypeId, string nodeTypeAdditionalId, double? lowerBoundary,
            int? count, string orderBy, bool? orderByDesc, string searchText, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            bool _tidIsNull = !nodeTypeId.HasValue;

            bool checkAccess = !paramsContainer.CurrentUserID.HasValue ||
                (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID.Value) &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) && !(
                    (nodeId.HasValue || nodeTypeId.HasValue) &&
                    CNController.is_service_admin(paramsContainer.Tenant.Id, nodeId.HasValue ? nodeId.Value : nodeTypeId.Value, paramsContainer.CurrentUserID.Value)
                ));

            bool? searchable = null;
            if (checkAccess) searchable = true;

            long totalCount = 0;

            List<Node> nodes = CNController.get_direct_childs(paramsContainer.Tenant.Id,
                nodeId, nodeTypeId, nodeTypeAdditionalId, searchable, lowerBoundary, count, orderBy, orderByDesc,
                searchText, checkAccess, paramsContainer.CurrentUserID, ref totalCount);

            //nodes = nodes.OrderBy(u => u.CreationDate).ToList();
            //nodes = nodes.OrderBy(u => u.Name).ToList();

            List<Guid> nodeIds = CNController.have_childs(paramsContainer.Tenant.Id, 
                nodeIds: nodes.Select(u => u.NodeID.Value).ToList());

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Nodes\":[" + string.Join(",", nodes.Select(
                u => "{\"NodeID\":\"" + u.NodeID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(u.Name) + "\"" +
                    ",\"ParentNodeID\":\"" + (u.ParentNodeID.HasValue ?
                    u.ParentNodeID.ToString() : string.Empty) + "\"" +
                    ",\"HasChild\":" + nodeIds.Exists(x => x == u.NodeID).ToString().ToLower() +
                    "}")) + "]}";
        }

        protected void get_tree_depth(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            int? depth = null;
            if (nodeTypeId.HasValue) depth = CNController.get_tree_depth(paramsContainer.Tenant.Id, nodeTypeId.Value);

            responseText = "{\"Depth\":" + (!depth.HasValue ? "null" : depth.Value.ToString()) + "}";
        }

        protected void like_unlike(Guid? nodeId, bool unlike, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = nodeId.HasValue && (unlike ?
                CNController.unlike_node(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value) :
                CNController.like_node(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value));

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"LikeStatus\":" + (unlike ? "false" : "true") + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Send Notification
            if (result && !unlike)
            {
                Notification not = new Notification()
                {
                    SubjectID = nodeId,
                    RefItemID = nodeId,
                    SubjectType = SubjectType.Node,
                    Action = Modules.NotificationCenter.ActionType.Like
                };
                not.Sender.UserID = paramsContainer.CurrentUserID.Value;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            else if (result && unlike)
            {
                Notification not = new Notification()
                {
                    SubjectID = nodeId,
                    Action = Modules.NotificationCenter.ActionType.Like
                };
                not.Sender.UserID = paramsContainer.CurrentUserID.Value;
                NotificationController.remove_notifications(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = unlike ? Modules.Log.Action.UnlikeNode : Modules.Log.Action.LikeNode,
                    SubjectID = nodeId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        public void get_fans(Guid? nodeId, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            long totalCount = 0;

            List<User> users = !nodeId.HasValue ? new List<User>() : CNController.get_node_fans(paramsContainer.Tenant.Id,
                nodeId.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Users\":[" + ProviderUtil.list_to_string<string>(users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                            u.UserID.Value) + "\"" +
                        "}").ToList()) + "]" +
                "}";
        }

        protected void add_member(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAdminAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node, 
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            bool hasUserAccess = nodeId.HasValue && userId.HasValue && !hasAdminAccess &&
                CNController.has_extension(paramsContainer.Tenant.Id, nodeId.Value, ExtensionType.Group) &&
                !CNController.is_node_member(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);

            if ((!hasAdminAccess && !hasUserAccess) || (hasUserAccess && userId != paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AddNodeMember_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            NodeMember newMember = new NodeMember()
            {
                Node = new Node() { NodeID = nodeId },
                Member = new User() { UserID = userId },
                MembershipDate = DateTime.Now,
                IsAdmin = false,
                Status = (hasAdminAccess ? NodeMemberStatuses.Accepted : NodeMemberStatuses.Pending).ToString(),
                AcceptionDate = hasAdminAccess ? (DateTime?)DateTime.Now : null
            };

            List<Dashboard> retDashboards = new List<Dashboard>();

            bool result = CNController.add_member(paramsContainer.Tenant.Id, newMember, ref retDashboards);

            if (result) NotificationController.transfer_dashboards(paramsContainer.Tenant.Id, retDashboards);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                ",\"Status\":\"" + newMember.Status + "\"" +
                ",\"ImageURL\":\"" +
                    DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, userId.Value) + "\"" +
                "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNodeMember,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_member(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId == paramsContainer.CurrentUserID || (
                userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node, 
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None));

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveNodeMember_PermissionFailed, nodeId, userId);
                return;
            }
            //check access

            bool result = nodeId.HasValue && userId.HasValue &&
                CNController.remove_member(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                ",\"Status\":\"" + NodeMemberStatuses.NotSet.ToString() + "\"" + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveNodeMember,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void accept_member(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AcceptNodeMember_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            bool result = nodeId.HasValue && userId.HasValue &&
                CNController.accept_member(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                ",\"Status\":\"" + NodeMemberStatuses.Accepted.ToString() + "\"" + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AcceptNodeMember,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_members(List<Guid> nodeIds, bool? admin, bool isBatch,
            string searchText, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!string.IsNullOrEmpty(searchText) && searchText.Length > 100) searchText = searchText.Substring(0, 100);

            long totalCount = 0;

            List<NodeMember> members = nodeIds.Count == 0 ? new List<NodeMember>() :
                CNController.get_members(paramsContainer.Tenant.Id, nodeIds, false, admin, searchText, count, lowerBoundary, ref totalCount);

            responseText = string.Join(",", nodeIds.Select(nid =>
                "{\"NodeID\":\"" + nid.ToString() + "\"" +
                    ",\"TotalCount\":" + totalCount.ToString() +
                    ",\"Members\":[" + string.Join(",", members.OrderByDescending(x => x.IsAdmin.HasValue && x.IsAdmin.Value)
                        .Where(u => u.Node.NodeID == nid).Select(v => v.toJson(paramsContainer.Tenant.Id))) + "]" +
                "}"
            ));

            if (nodeIds.Count > 1 || (nodeIds.Count == 1 && isBatch)) responseText = "[" + responseText + "]";
        }

        protected void get_pending_members(Guid? nodeId, int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.PendingMembers_AccessDenied,
                     nodeId, paramsContainer.CurrentUserID.Value);
                return;
            }
            //check access

            long totalCount = 0;

            List<NodeMember> members = CNController.get_members(paramsContainer.Tenant.Id, nodeId.Value,
                pending: true, admin: null, searchText: null, count: count, lowerBoundary: lowerBoundary, totalCount: ref totalCount);

            responseText = "{\"NodeID\":\"" + nodeId.ToString() + "\"" +
                ",\"TotalCount\":" + totalCount.ToString() +
                ",\"Members\":[" + string.Join(",", members.Select(v => v.toJson(paramsContainer.Tenant.Id))) +
                "]}";
        }

        protected void get_child_hierarchy_members(Guid? nodeId, string searchText, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!string.IsNullOrEmpty(searchText) && searchText.Length > 100) searchText = searchText.Substring(0, 100);

            long totalCount = 0;

            List<User> users = !nodeId.HasValue ? new List<User>() : UsersController.get_users(paramsContainer.Tenant.Id,
                CNController.get_child_hierarchy_member_ids(paramsContainer.Tenant.Id, nodeId.Value,
                    searchText, count, lowerBoundary, ref totalCount));

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Users\":[" +
                string.Join(",", users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                            paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                        "}")) + "]}";
        }

        protected void get_child_hierarchy_experts(Guid? nodeId, string searchText, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!string.IsNullOrEmpty(searchText) && searchText.Length > 100) searchText = searchText.Substring(0, 100);

            long totalCount = 0;

            List<User> users = !nodeId.HasValue ? new List<User>() : UsersController.get_users(paramsContainer.Tenant.Id,
                CNController.get_child_hierarchy_expert_ids(paramsContainer.Tenant.Id, nodeId.Value,
                    searchText, count, lowerBoundary, ref totalCount));

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Users\":[" +
                string.Join(",", users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                            paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                        "}")) + "]}";
        }

        protected void make_admin(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.SetUserAsNodeAdmin_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            Node _node = CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);

            bool unique = _node.NodeTypeAdditionalID == CNUtilities.get_node_type_additional_id(NodeTypes.Project) ||
                _node.NodeTypeAdditionalID == CNUtilities.get_node_type_additional_id(NodeTypes.Process) ||
                _node.NodeTypeAdditionalID == CNUtilities.get_node_type_additional_id(NodeTypes.Department);

            if (unique && CNController.has_admin(paramsContainer.Tenant.Id, nodeId.Value))
            {
                responseText = "{\"ErrorText\":\"" + "امکان انتخاب هم زمان بیش از یک مدیر وجود ندارد" + "\"}";
                return;
            }

            bool result = CNController.set_unset_node_admin(paramsContainer.Tenant.Id,
                nodeId.Value, userId.Value, true, unique);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetUserAsNodeAdmin,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void unadmin(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveNodeAdmin_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            bool result = CNController.set_unset_node_admin(paramsContainer.Tenant.Id, nodeId.Value, userId.Value, false);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveNodeAdmin,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void add_expert(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AddExpert_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            Expert expert = new Expert();
            expert.Node.NodeID = nodeId;
            expert.User.UserID = userId;

            bool result = CNController.add_expert(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                "\",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                    userId.Value) + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddExpert,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_expert(Guid? nodeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Node,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveExpert_PermissionFailed, nodeId, userId);
                return;
            }
            //end of check access

            bool result = CNController.remove_expert(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveExpert,
                    SubjectID = nodeId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_experts(List<Guid> nodeIds, bool isBatch,
            string searchText, bool? hierarchy, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (nodeIds.Count != 1) hierarchy = false;

            if (!string.IsNullOrEmpty(searchText) && searchText.Length > 100) searchText = searchText.Substring(0, 100);

            long totalCount = 0;

            List<Expert> experts = nodeIds.Count == 0 ? new List<Expert>() :
                CNController.get_experts(paramsContainer.Tenant.Id, nodeIds, searchText, count, lowerBoundary, 
                ref totalCount, hierarchy: hierarchy.HasValue && hierarchy.Value);

            if (hierarchy.HasValue && hierarchy.Value)
            {
                experts.ForEach(ex => ex.Node.NodeID = nodeIds[0]);

                experts = experts.Select(x => x.User.UserID).Distinct()
                    .Select(id => experts.Where(e => e.User.UserID == id).FirstOrDefault()).ToList();
            }
            
            responseText = string.Join(",", nodeIds.Select(nid =>
                "{\"NodeID\":\"" + nid.ToString() + "\"" +
                    ",\"TotalCount\":" + totalCount.ToString() +
                    ",\"Experts\":[" + string.Join(",", experts.Where(u => u.Node.NodeID == nid).Select(
                        v => v.toJson(paramsContainer.Tenant.Id))) + "]" +
                "}"));

            if (nodeIds.Count > 1 || (nodeIds.Count == 1 && isBatch)) responseText = "[" + responseText + "]";
        }

        protected void relation_exists(Guid? sourceNodeId, Guid? destinationNodeId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            bool result = sourceNodeId.HasValue && destinationNodeId.HasValue &&
                CNController.relation_exists(paramsContainer.Tenant.Id, sourceNodeId.Value, destinationNodeId.Value);

            responseText = "{\"Exists\":" + result.ToString().ToLower() + "}";
        }

        protected void get_expertise_domains_count(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<NodesCount> nodesCount = !userId.HasValue ? new List<NodesCount>() :
                CNController.get_expertise_domains_count(paramsContainer.Tenant.Id,
                userId.Value, nodeTypeId, null, null, null, null);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodesCount.Select(
                u => _get_nodes_count_json(u))) + "]}";
        }

        protected void get_expertise_domains(List<Guid> nodeTypeIds, Guid? userId, Guid? nodeId, string additionalId,
            string searchText, bool? hasChild, DateTime? lowerDateLimit, DateTime? upperDateLimit,
            int? lowerBoundary, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            long totalCount = 0;

            List<Node> nodes = !userId.HasValue ? new List<Node>() :
                CNController.get_expertise_domains(paramsContainer.Tenant.Id, userId.Value, nodeTypeIds, nodeId,
                additionalId, searchText, lowerDateLimit, upperDateLimit, lowerBoundary, count, ref totalCount);

            List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                CNController.have_childs(paramsContainer.Tenant.Id, nodes.Select(u => u.NodeID.Value).ToList());

            nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void get_membership_domains_count(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<NodesCount> nodesCount = !userId.HasValue ? new List<NodesCount>() :
                CNController.get_membership_domains_count(paramsContainer.Tenant.Id,
                userId.Value, nodeTypeId, null, null, null, null);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodesCount.Select(
                u => _get_nodes_count_json(u))) + "]}";
        }

        protected void get_membership_domains(List<Guid> nodeTypeIds, Guid? userId, Guid? nodeId, string additionalId,
            string searchText, bool? hasChild, DateTime? lowerDateLimit, DateTime? upperDateLimit,
            int? lowerBoundary, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            long totalCount = 0;

            List<Node> nodes = !userId.HasValue ? new List<Node>() :
                CNController.get_membership_domains(paramsContainer.Tenant.Id, userId.Value, nodeTypeIds, nodeId,
                additionalId, searchText, lowerDateLimit, upperDateLimit, lowerBoundary, count, ref totalCount);

            List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                CNController.have_childs(paramsContainer.Tenant.Id, nodes.Select(u => u.NodeID.Value).ToList());

            nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void get_member_nodes(Guid? userId, List<Guid> nodeTypeIds, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<NodeMember> nodes = !userId.HasValue ? new List<NodeMember>() : (
                nodeTypeIds.Count > 0 ?
                CNController.get_member_nodes(paramsContainer.Tenant.Id, userId.Value, ref nodeTypeIds) :
                CNController.get_member_nodes(paramsContainer.Tenant.Id, userId.Value));

            responseText = "{\"Nodes\":[";

            bool isFirst = true;
            foreach (NodeMember _node in nodes)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"NodeID\":\"" + _node.Node.NodeID.Value.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(_node.Node.Name) + "\"" +
                        ",\"NodeType\":\"" + Base64.encode(_node.Node.NodeType) + "\"" +
                        ",\"IsAdmin\":" + (!_node.IsAdmin.HasValue ? false : _node.IsAdmin.Value).ToString().ToLower() +
                        "}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_favorite_nodes_count(Guid? nodeTypeId, Guid? userId, bool? isDocument, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<NodesCount> nodesCount = !userId.HasValue ? new List<NodesCount>() :
                CNController.get_favorite_nodes_count(paramsContainer.Tenant.Id,
                userId.Value, nodeTypeId, null, null, isDocument, null, null);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodesCount.Select(
                u => _get_nodes_count_json(u))) + "]}";
        }

        protected void get_favorite_nodes(List<Guid> nodeTypeIds, Guid? userId, Guid? nodeId, 
            string additionalId, string searchText, bool? hasChild, bool? isDocument, 
            DateTime? lowerDateLimit, DateTime? upperDateLimit, int? lowerBoundary, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            long totalCount = 0;

            List<Node> nodes = !userId.HasValue ? new List<Node>() :
                CNController.get_favorite_nodes(paramsContainer.Tenant.Id, userId.Value, nodeTypeIds, nodeId,
                additionalId, searchText, isDocument, lowerDateLimit, upperDateLimit, lowerBoundary, count, ref totalCount);

            List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                CNController.have_childs(paramsContainer.Tenant.Id, nodes.Select(u => u.NodeID.Value).ToList());

            nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void is_fan(Guid? userId, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<Guid> favorites = !userId.HasValue ? new List<Guid>() :
                CNController.is_fan(paramsContainer.Tenant.Id, nodeIds, userId.Value);

            responseText = "[" + ProviderUtil.list_to_string<string>(favorites
                .Select(u => "\"" + u.ToString() + "\"").ToList()) + "]";
        }

        protected void get_related_nodes(Guid? nodeId, Guid? relatedNodeTypeId, string searchText,
            bool? inRelations, bool? outRelations, bool? inTagRelations, bool? outTagRelations,
            int? count, int? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<Node> nodeRelations = !nodeId.HasValue ? new List<Node>() :
                CNController.get_related_nodes(paramsContainer.Tenant.Id,
                nodeId.Value, relatedNodeTypeId, searchText, inRelations, outRelations,
                inTagRelations, outTagRelations, count, lowerBoundary);

            responseText = "{\"Nodes\":[" + string.Join(",", nodeRelations.Select(u => u.toJson())) + "]}";
        }

        protected void get_related_nodes_abstract(Guid? nodeId, bool? inRelations, bool? outRelations,
            bool? inTagRelations, bool? outTagRelations, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            int defaultCount = 3;
            int? nodeTypesCount = count, nodesCount = count;

            List<NodesCount> relationsCount = CNController.get_related_nodes_count(paramsContainer.Tenant.Id,
                nodeId.Value, null, null, true, true, true, true, 1000000)
                .OrderByDescending(u => u.Count.HasValue ? u.Count.Value : 0).ToList();
            
            int totalRelatedNodesCount = relationsCount.Count == 0 ? 0 :
                relationsCount.Sum(u => !u.Count.HasValue ? 0 : u.Count.Value);

            int ntCount = Math.Min(relationsCount.Count,
                !nodeTypesCount.HasValue || nodeTypesCount.Value <= 0 ? defaultCount : nodeTypesCount.Value);
            if (relationsCount.Count == ntCount + 1) ++ntCount;

            int nCount = !nodesCount.HasValue || nodesCount.Value <= 0 ? defaultCount : nodesCount.Value;

            List<Node> relations = CNController.get_related_nodes_partitioned(paramsContainer.Tenant.Id,
                nodeId.Value, relationsCount.Take(ntCount).Select(u => u.NodeTypeID.Value).ToList(),
                inRelations, outRelations, inTagRelations, outTagRelations, nCount + 1);

            responseText = "{\"RelatedNodeTypesCount\":" + relationsCount.Count.ToString() +
                ",\"TotalRelationsCount\":" + totalRelatedNodesCount.ToString() +
                ",\"NodeTypes\":[" + string.Join(",", relationsCount.Select(
                    u => "{\"NodeTypeID\":\"" + u.NodeTypeID.ToString() + "\"" +
                        ",\"NodeType\":\"" + Base64.encode(u.TypeName) + "\"" +
                        ",\"Count\":" + (u.Count.HasValue ? u.Count.Value : 0).ToString() +
                        ",\"Nodes\":[" + string.Join(",", relations.Where(y => y.NodeTypeID == u.NodeTypeID)
                            .Take(nCount == u.Count.Value - 1 ? nCount + 1 : nCount).Select(
                            x => "{\"NodeID\":\"" + x.NodeID.Value.ToString() + "\"" +
                                ",\"Name\":\"" + Base64.encode(x.Name) + "\"" +
                                ",\"CreationDate\":\"" + (!x.CreationDate.HasValue ? string.Empty :
                                    GenericDate.get_local_date(x.CreationDate.Value)) + "\"" +
                                "}")) + "]" +
                    "}").ToList()) +
                "]" +
                "}";
        }

        protected void add_complex(Guid? nodeTypeId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeTypeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.AddComplex_PermissionFailed, nodeTypeId);
                return;
            }
            //check access

            NodeList newList = new NodeList()
            {
                ListID = Guid.NewGuid(),
                NodeTypeID = nodeTypeId,
                Name = name,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now
            };

            bool result = CNController.add_complex(paramsContainer.Tenant.Id, newList);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + ",\"List\":" + _get_list_json(newList) + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddComplex,
                    SubjectID = newList.ListID,
                    Info = "{\"NodeTypeID\":\"" + nodeTypeId.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(name) + "\"" + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void modify_complex(Guid? listId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            Guid? listTypeId = !listId.HasValue ? null : CNController.get_complex_type_id(paramsContainer.Tenant.Id, listId.Value);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: listTypeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.ModifyComplex_PermissionFailed, listId);
                return;
            }
            //end of check access

            NodeList list = new NodeList()
            {
                ListID = listId,
                Name = name,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = list.ListID.HasValue && !string.IsNullOrEmpty(list.Name) &&
                CNController.modify_complex(paramsContainer.Tenant.Id, list);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyComplex,
                    SubjectID = listId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_complex(Guid? listId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid? listTypeId = !listId.HasValue ? null : CNController.get_complex_type_id(paramsContainer.Tenant.Id, listId.Value);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: listTypeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveComplex_PermissionFailed, listId);
                return;
            }
            //end of check access

            bool result = listId.HasValue && CNController.remove_complex(paramsContainer.Tenant.Id,
                listId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyComplex,
                    SubjectID = listId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void add_node_to_complex(Guid? listId, Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = listId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (listId.HasValue && nodeId.HasValue)
                    _save_error_log(Modules.Log.Action.AddNodeToComplex_PermissionFailed, listId, nodeId);
                return;
            }
            //end of check access

            bool result = listId.HasValue && CNController.add_node_to_complex(paramsContainer.Tenant.Id,
                listId.Value, nodeId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNodeToComplex,
                    SubjectID = listId,
                    SecondSubjectID = nodeId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_complex_node(Guid? listId, Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId);

            bool hasAccess = listId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (listId.HasValue && nodeId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveComplexNode_PermissionFailed, listId, nodeId);
                return;
            }
            //end of check access

            bool result = listId.HasValue && CNController.remove_complex_node(paramsContainer.Tenant.Id,
                listId.Value, nodeId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveComplexNode,
                    SubjectID = listId,
                    SecondSubjectID = nodeId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void add_complex_admin(Guid? listId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            Guid? listTypeId = !listId.HasValue ? null : CNController.get_complex_type_id(paramsContainer.Tenant.Id, listId.Value);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: listTypeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (listId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AddComplexAdmin_PermissionFailed, listId, userId);
                return;
            }
            //end of check access

            bool result = listId.HasValue && CNController.add_complex_admin(paramsContainer.Tenant.Id,
                listId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            if (!result)
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else
            {
                User usr = UsersController.get_user(paramsContainer.Tenant.Id, userId.Value);

                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                    "\",\"User\":{\"UserID\":\"" + usr.UserID.ToString() +
                    "\",\"FirstName\":\"" + Base64.encode(usr.FirstName) + "\",\"LastName\":\"" + Base64.encode(usr.LastName) +
                    "\",\"UserName\":\"" + Base64.encode(usr.UserName) +
                    "\",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        usr.UserID.Value) + "\"}}";
            }

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddComplexAdmin,
                    SubjectID = listId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_complex_admin(Guid? listId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            Guid? listTypeId = !listId.HasValue ? null : CNController.get_complex_type_id(paramsContainer.Tenant.Id, listId.Value);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: listTypeId);

            bool hasAccess = userId.HasValue && accessChecker.checkNodeEditAccess(AdminLevel.Service,
                roleName: AccessRoleName.ManageOntology, permission: PermissionType.None);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (listId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveComplexAdmin_PermissionFailed, listId, userId);
                return;
            }
            //end of check access

            bool result = listId.HasValue && CNController.remove_complex_admin(paramsContainer.Tenant.Id,
                listId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveComplexAdmin,
                    SubjectID = listId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_complex_admins(Guid? listId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<User> admins = !listId.HasValue ? new List<User>() :
                UsersController.get_users(paramsContainer.Tenant.Id,
                CNController.get_complex_admins(paramsContainer.Tenant.Id, listId.Value));

            responseText = "{\"Users\":[" + ProviderUtil.list_to_string<string>(admins.Select(
                u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        u.UserID.Value) + "\"" +
                    "}").ToList()) + "]" +
                "}";
        }

        protected string _get_list_json(NodeList list)
        {
            return "{\"ListID\":\"" + list.ListID.Value.ToString() + "\",\"Name\":\"" + Base64.encode(list.Name) + "\"}";
        }

        protected void get_lists(Guid? nodeTypeId, string searchText, Guid? minId, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<NodeList> lst = !nodeTypeId.HasValue ? new List<NodeList>() :
                CNController.get_lists(paramsContainer.Tenant.Id, nodeTypeId.Value, searchText, minId, count);

            responseText = "{\"Lists\":[" + ProviderUtil.list_to_string<string>(lst.Select(u => _get_list_json(u)).ToList()) + "]" + "}";

        }

        protected void get_list_nodes(Guid? listId, int? count, long? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<Node> nodes = !listId.HasValue ? new List<Node>() :
                CNController.get_list_nodes(paramsContainer.Tenant.Id, listId.Value);

            responseText = "{\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]" + "}";
        }

        protected void search_tags(string searchText, int? count, int? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<Tag> tags = CNController.search_tags(paramsContainer.Tenant.Id, searchText, count, lowerBoundary);

            responseText = "{\"Tags\":[";

            bool isFirst = true;
            foreach (Tag _tag in tags)
            {
                string tag = _tag.Text;
                int callsCount = _tag.CallsCount.HasValue ? _tag.CallsCount.Value : 0;
                string approved = _tag.Approved.HasValue ? _tag.Approved.Value.ToString().ToLower() : "null";

                Base64.encode(tag, ref tag);

                if (!isFirst) responseText += ",";
                isFirst = false;

                responseText += "{\"TagID\":\"" + _tag.TagID.Value.ToString() + "\",\"Tag\":\"" + tag +
                    "\",\"CallsCount\":\"" + callsCount.ToString() + "\",\"Approved\":" + approved + "}";
            }

            responseText += "]}";
        }

        protected string _get_map_user_json(User user, List<NodeMember> memberNodes, List<Expert> expertiseDomains,
            List<Node> favoriteNodes, List<Node> createdNodes, List<Friend> friends,
            bool? isAdmin = null, bool? isPending = null, double? collaborationShare = null)
        {
            if (!isAdmin.HasValue) isAdmin = false;
            if (!isPending.HasValue) isPending = false;
            if (!collaborationShare.HasValue) collaborationShare = 0;

            string username = user.UserName;
            string firstName = user.FirstName;
            string lastName = user.LastName;

            Base64.encode(username, ref username);
            Base64.encode(firstName, ref firstName);
            Base64.encode(lastName, ref lastName);

            string retStr = "{\"UserID\":\"" + user.UserID.Value.ToString() + "\",\"UserName\":\"" + username +
                "\",\"FirstName\":\"" + firstName + "\",\"LastName\":\"" + lastName +
                "\",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                    user.UserID.Value) +
                "\",\"IsAdmin\":" + isAdmin.ToString().ToLower() + ",\"IsPending\":" + isPending.ToString().ToLower() +
                ",\"CollaborationShare\":\"" + collaborationShare.ToString() + "\"";

            bool isFirst = true;

            if (memberNodes != null && memberNodes.Count > 0)
            {
                retStr += ",\"MemberNodes\":[";
                isFirst = true;
                foreach (NodeMember nm in memberNodes)
                {
                    retStr += (isFirst ? string.Empty : ",") +
                        _get_map_node_json(nm.Node, nm.IsAdmin, nm.Status == NodeMemberStatuses.Pending.ToString());
                    isFirst = false;
                }
                retStr += "]";
            }

            if (expertiseDomains != null && expertiseDomains.Count > 0)
            {
                retStr += ",\"ExpertiseDomains\":[";
                isFirst = true;
                foreach (Expert exp in expertiseDomains)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_node_json(exp.Node);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (favoriteNodes != null && favoriteNodes.Count > 0)
            {
                retStr += ",\"Favorites\":[";
                isFirst = true;
                foreach (Node fav in favoriteNodes)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_node_json(fav);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (createdNodes != null && createdNodes.Count > 0)
            {
                retStr += ",\"CreatedNodes\":[";
                isFirst = true;
                foreach (Node own in createdNodes)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_node_json(own);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (friends != null && friends.Count > 0)
            {
                retStr += ",\"Friends\":[";
                isFirst = true;
                foreach (Friend frnd in friends)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_user_json(frnd.User, null, !frnd.AreFriends.HasValue || !frnd.AreFriends.Value, null);
                    isFirst = false;
                }
                retStr += "]";
            }

            return retStr + "}";
        }

        protected string _get_map_user_json(User user, bool? isAdmin, bool? isPending, double? collaborationShare)
        {
            return _get_map_user_json(user, new List<NodeMember>(), new List<Expert>(), new List<Node>(),
                new List<Node>(), new List<Friend>(), isAdmin, isPending, collaborationShare);
        }

        protected string _get_map_node_json(Node node, Node parent, List<Node> childs, List<Node> relatedNodes,
            List<NodeMember> members, List<Expert> experts, List<User> fans, List<NodeCreator> creators,
            bool? isAdmin = null, bool? isPending = null)
        {
            if (string.IsNullOrEmpty(node.AdditionalID)) node.AdditionalID = string.Empty;
            if (!isAdmin.HasValue) isAdmin = false;
            if (!isPending.HasValue) isPending = false;

            string name = node.Name;
            string nodeType = node.NodeType;

            Base64.encode(name, ref name);
            Base64.encode(nodeType, ref nodeType);

            string retStr = "{\"NodeID\":\"" + node.NodeID.Value.ToString() + "\",\"Name\":\"" + name +
                "\",\"AdditionalID\":\"" + node.AdditionalID + "\",\"IconURL\":\"" +
                    DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id, node.NodeID.Value) +
                "\",\"NodeTypeID\":\"" + node.NodeTypeID.Value.ToString() + "\",\"NodeType\":\"" + nodeType +
                "\",\"NodeTypeIconURL\":\"" +
                    DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id, node.NodeTypeID.Value) +
                "\",\"IsAdmin\":" + isAdmin.ToString().ToLower() + ",\"IsPending\":" + isPending.ToString().ToLower();

            bool isFirst = true;

            if (parent != null && parent.NodeID.HasValue) retStr += ",\"Parent\":" + _get_map_node_json(parent);

            if (childs != null && childs.Count > 0)
            {
                retStr += ",\"Childs\":[";
                isFirst = true;
                foreach (Node ch in childs)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_node_json(ch);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (relatedNodes != null && relatedNodes.Count > 0)
            {
                retStr += ",\"RelatedNodes\":[";
                isFirst = true;
                foreach (Node rn in relatedNodes)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_node_json(rn);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (members != null && members.Count > 0)
            {
                retStr += ",\"Members\":[";
                isFirst = true;
                foreach (NodeMember mm in members)
                {
                    retStr += (isFirst ? string.Empty : ",") +
                        _get_map_user_json(mm.Member, mm.IsAdmin, mm.Status == NodeMemberStatuses.Pending.ToString(), null);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (experts != null && experts.Count > 0)
            {
                retStr += ",\"Experts\":[";
                isFirst = true;
                foreach (Expert exp in experts)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_user_json(exp.User, null, null, null);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (fans != null && fans.Count > 0)
            {
                retStr += ",\"Fans\":[";
                isFirst = true;
                foreach (User fn in fans)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_user_json(fn, null, null, null);
                    isFirst = false;
                }
                retStr += "]";
            }

            if (creators != null && creators.Count > 0)
            {
                retStr += ",\"Creators\":[";
                isFirst = true;
                foreach (NodeCreator nc in creators)
                {
                    retStr += (isFirst ? string.Empty : ",") + _get_map_user_json(nc.User, null, null, nc.CollaborationShare);
                    isFirst = false;
                }
                retStr += "]";
            }

            return retStr + "}";
        }

        protected string _get_map_node_json(Node node, bool? isAdmin = null, bool? isPending = null)
        {
            return _get_map_node_json(node, null, new List<Node>(), new List<Node>(),
                new List<NodeMember>(), new List<Expert>(), new List<User>(), new List<NodeCreator>(), isAdmin, isPending);
        }

        protected void get_map_items(Guid? itemId, bool? isUser, bool? members, bool? adminMembers,
            bool? experts, bool? fans, bool? creators, bool? friends, bool? hierarchy,
            bool? relatedNodes, Guid? relatedNodesTypeId, Guid? membersNodeTypeId, Guid? creatorsNodeTypeId,
            Guid? expertsNodeTypeId, Guid? fansNodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            bool? adminStatus = null;
            if (adminMembers.HasValue && adminMembers.Value && !(members.HasValue && members.Value)) adminStatus = true;
            members = (members.HasValue && members.Value) || (adminMembers.HasValue && adminMembers.Value);

            long totalCount = 0;

            if (isUser.HasValue && isUser.Value)
            {
                User user = UsersController.get_user(paramsContainer.Tenant.Id, itemId.Value);
                List<NodeMember> nodeMembers = !members.HasValue || !members.Value ? new List<NodeMember>() :
                    (membersNodeTypeId.HasValue ? CNController.get_member_nodes(paramsContainer.Tenant.Id,
                        itemId.Value, membersNodeTypeId.Value, adminStatus) :
                    CNController.get_member_nodes(paramsContainer.Tenant.Id, itemId.Value, adminStatus));
                List<Expert> expertiseDomains =
                    experts.HasValue && experts.Value ? CNController.get_expertise_domains(paramsContainer.Tenant.Id,
                    itemId.Value, true, true, null, expertsNodeTypeId) : new List<Expert>();
                List<Node> favoriteNodes =
                    fans.HasValue && fans.Value ? CNController.get_favorite_nodes(paramsContainer.Tenant.Id, userId: itemId.Value, 
                    nodeTypeIds: !fansNodeTypeId.HasValue ? new List<Guid>() : new List<Guid>() { fansNodeTypeId.Value },
                    null, null, null, null, null, null, null,  10000, ref totalCount) :
                    new List<Node>();
                List<Node> createdNodes = !creators.HasValue || !creators.Value ? new List<Node>() :
                    (creatorsNodeTypeId.HasValue ?
                    CNController.get_creator_nodes(paramsContainer.Tenant.Id, itemId.Value, creatorsNodeTypeId.Value) :
                    CNController.get_creator_nodes(paramsContainer.Tenant.Id, itemId.Value));
                List<Friend> friendUsers =
                    friends.HasValue && friends.Value ? UsersController.get_friends(paramsContainer.Tenant.Id,
                    itemId.Value, false, null, null, ref totalCount, areFriends: true) : new List<Friend>();

                responseText = _get_map_user_json(user, nodeMembers, expertiseDomains,
                    favoriteNodes, createdNodes, friendUsers);
            }
            else
            {
                Node node = CNController.get_node(paramsContainer.Tenant.Id, itemId.Value);
                Node parentNode = hierarchy.HasValue && hierarchy.Value ?
                    CNController.get_direct_parent(paramsContainer.Tenant.Id, itemId.Value) : null;
                List<Node> childNodes = hierarchy.HasValue && hierarchy.Value ?
                    CNController.get_direct_childs(paramsContainer.Tenant.Id, itemId.Value) : new List<Node>();
                List<Node> _relatedNodes = relatedNodes.HasValue && relatedNodes.Value ?
                    CNController.get_related_nodes(paramsContainer.Tenant.Id,
                    itemId.Value, relatedNodesTypeId, null, true, true, true, true) : new List<Node>();
                List<NodeMember> memberNodes = !members.HasValue || !members.Value ? new List<NodeMember>() :
                    CNController.get_members(paramsContainer.Tenant.Id, itemId.Value, pending: false, admin: adminStatus);
                List<Expert> nodeExperts = experts.HasValue && experts.Value ?
                    CNController.get_experts(paramsContainer.Tenant.Id, itemId.Value) : new List<Expert>();
                List<User> nodeFans = fans.HasValue && fans.Value ? UsersController.get_users(paramsContainer.Tenant.Id,
                    CNController.get_node_fans_user_ids(paramsContainer.Tenant.Id, itemId.Value)) : new List<User>();
                List<NodeCreator> nodeCreators = creators.HasValue && creators.Value ?
                    CNController.get_node_creators(paramsContainer.Tenant.Id, itemId.Value, true) : new List<NodeCreator>();

                responseText = _get_map_node_json(node, parentNode, childNodes, _relatedNodes,
                    memberNodes, nodeExperts, nodeFans, nodeCreators);
            }
        }

        protected string _get_nodes_count_json(NodesCount cnt)
        {
            return "{\"NodeTypeID\":\"" + cnt.NodeTypeID.ToString() + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(cnt.TypeName) + "\"" +
                    ",\"Count\":" + (cnt.Count.HasValue ? cnt.Count : 0).ToString() +
                    "}";
        }

        protected void get_nodes_count(List<Guid> nodeTypeIds, DateTime? lowerCreationDateLimit,
                DateTime? upperCreationDateLimit, bool? root, bool? archive, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<NodesCount> nodesCount = CNController.get_nodes_count(paramsContainer.Tenant.Id,
                ref nodeTypeIds, lowerCreationDateLimit, upperCreationDateLimit, archive, root);

            responseText = "{\"NodeTypes\":[";

            bool isFirst = true;
            foreach (NodesCount cnt in nodesCount)
            {
                responseText += (isFirst ? string.Empty : ",") + _get_nodes_count_json(cnt);
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_most_populated_node_types(int? count, int? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<NodesCount> cnts = CNController.get_most_populated_node_types(paramsContainer.Tenant.Id,
                count, lowerBoundary);

            //CNController.have_childs

            int? maxOrder = cnts.Select(u => u.Order).Max();
            int? maxReverseOrder = cnts.Select(u => u.ReverseOrder).Max();

            responseText = "{\"MaxOrder\":" + (maxOrder.HasValue ? maxOrder.Value : 0).ToString() +
                ",\"MaxReverseOrder\":" + (maxReverseOrder.HasValue ? maxReverseOrder.Value : 0).ToString() +
                ",\"NodeTypes\":[" + ProviderUtil.list_to_string<string>(cnts.Select(v => _get_nodes_count_json(v)).ToList()) + "]" +
                "}";
        }

        protected void suggest_node_relations(Guid? relatedNodeTypeId, int? count, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<Node> nodes = CNController.suggest_node_relations(
                    paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, relatedNodeTypeId, count);

            List<Guid> haveChild = CNController.have_childs(paramsContainer.Tenant.Id,
                nodes.Select(u => u.NodeID.Value).ToList());

            nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

            responseText = "{\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void suggest_node_types_for_relations(int? count, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<NodeType> nodeTypes = CNController.suggest_node_types_for_relations(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodeTypes.Select(u => u.toJson())) + "]}";
        }

        protected void suggest_similar_nodes(Guid? nodeId, int? count, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<SimilarNode> nodes = !nodeId.HasValue ? new List<SimilarNode>() :
                CNController.suggest_similar_nodes(paramsContainer.Tenant.Id, nodeId.Value, count)
                .Where(u => u.Suggested != null && !string.IsNullOrEmpty(u.Suggested.Name)).ToList();

            responseText = "{\"Nodes\":[" + string.Join(",", nodes.Select(
                u => "{\"Tags\":" + (u.Tags.HasValue && u.Tags.Value).ToString().ToLower() +
                ",\"Favorites\":" + (u.Favorites.HasValue && u.Favorites.Value).ToString().ToLower() +
                ",\"Relations\":" + (u.Relations.HasValue && u.Relations.Value).ToString().ToLower() +
                ",\"Experts\":" + (u.Experts.HasValue && u.Experts.Value).ToString().ToLower() +
                ",\"Node\":" + u.Suggested.toJson() +
                "}")) + "]}";
        }

        protected void suggest_knowledgable_uesrs(Guid? nodeId, int? count, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<KnowledgableUser> users = !nodeId.HasValue ? new List<KnowledgableUser>() :
                CNController.suggest_knowledgable_users(paramsContainer.Tenant.Id, nodeId.Value, count)
                .Where(u => u.User != null && !string.IsNullOrEmpty(u.User.UserName)).ToList();

            responseText = "{\"Users\":[" + string.Join(",", users.Select(
                u => "{\"Expert\":" + (u.Expert.HasValue && u.Expert.Value).ToString().ToLower() +
                ",\"Contributor\":" + (u.Contributor.HasValue && u.Contributor.Value).ToString().ToLower() +
                ",\"WikiEditor\":" + (u.WikiEditor.HasValue && u.WikiEditor.Value).ToString().ToLower() +
                ",\"Member\":" + (u.Member.HasValue && u.Member.Value).ToString().ToLower() +
                ",\"ExpertOfRelatedNode\":" + (u.ExpertOfRelatedNode.HasValue && u.ExpertOfRelatedNode.Value).ToString().ToLower() +
                ",\"ContributorOfRelatedNode\":" + (u.ContributorOfRelatedNode.HasValue && u.ContributorOfRelatedNode.Value).ToString().ToLower() +
                ",\"MemberOfRelatedNode\":" + (u.MemberOfRelatedNode.HasValue && u.MemberOfRelatedNode.Value).ToString().ToLower() +
                ",\"User\":{\"UserID\":\"" + u.User.UserID.ToString() + "\"" +
                    ",\"UserName\":\"" + Base64.encode(u.User.UserName) + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(u.User.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(u.User.LastName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        u.User.UserID.Value) + "\"" + "}" +
                "}")) + "]}";
        }

        protected void get_node_info(List<Guid> nodeIds, bool isArray, bool? keywords, bool? description, bool? attachments,
            bool? creator, bool? contributorsCount, bool? likesCount, bool? visitsCount, bool? expertsCount,
            bool? membersCount, bool? childsCount, bool? relatedNodesCount, bool? likeStatus, bool? coverPhotoUrl,
            bool? extensions, bool? service, bool? userStatus, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<NodeInfo> lst = CNController.get_node_info(paramsContainer.Tenant.Id, nodeIds,
                paramsContainer.CurrentUserID, keywords, description, creator, contributorsCount, likesCount,
                visitsCount, expertsCount, membersCount, childsCount, relatedNodesCount, likeStatus);

            List<DocFileInfo> lstAttachments = !attachments.HasValue || !attachments.Value ? new List<DocFileInfo>() :
                DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                lst.Select(u => u.NodeID.Value).ToList(), FileOwnerTypes.Node);

            List<Extension> lstExtensions = !extensions.HasValue || !extensions.Value || nodeIds.Count != 1 ?
                new List<Extension>() : CNController.get_extensions(paramsContainer.Tenant.Id, nodeIds[0]);

            List<Service> lstServices = !(service.HasValue && service.Value) && !(userStatus.HasValue && userStatus.Value) ?
                new List<Service>() :
                CNController.get_services(paramsContainer.Tenant.Id, lst.Select(u => u.NodeTypeID.Value).Distinct().ToList());

            responseText = isArray || lst.Count > 1 ? "[" : string.Empty;

            bool isFirst = true;
            foreach (NodeInfo ni in lst)
            {
                responseText += isFirst ? string.Empty : ",";
                isFirst = false;

                List<Extension> exts = lstExtensions.Where(u => u.OwnerID == ni.NodeTypeID).ToList();

                Service srvc = lstServices.Where(u => u.NodeType.NodeTypeID == ni.NodeTypeID).FirstOrDefault();

                bool isCreator = false, isContributor = false, isExpert = false, isMember = false,
                    isAdminMember = false, isServiceAdmin = false, isAreaAdmin = false, editable = false;

                if (userStatus.HasValue && userStatus.Value && paramsContainer.CurrentUserID.HasValue)
                {
                    Guid nodeId = ni.NodeID.Value;

                    CNController.get_user2node_status(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId, ref isCreator, ref isContributor, ref isExpert,
                        ref isMember, ref isAdminMember, ref isServiceAdmin, ref isAreaAdmin, ref editable);
                }

                responseText += "{\"NodeID\":\"" + ni.NodeID.ToString() + "\"" +
                    (ni.Tags == null || ni.Tags.Count == 0 ? string.Empty :
                        ",\"Keywords\":[" + string.Join(",", ni.Tags.Select(u => "\"" + Base64.encode(u) + "\"")) + "]"
                    ) +
                    ",\"Description\":\"" + Base64.encode(ni.Description) + "\"" +
                    (!attachments.HasValue || !attachments.Value ? string.Empty :
                        ",\"Attachments\":[" + string.Join(",", lstAttachments.Where(u => u.OwnerID == ni.NodeID)
                            .Select(x => x.toJson())) + "]"
                    ) +
                    (!creator.HasValue || !creator.Value || !ni.Creator.UserID.HasValue ? string.Empty :
                        ",\"Creator\":{\"UserID\":\"" + ni.Creator.UserID.ToString() + "\"" +
                            ",\"UserName\":\"" + Base64.encode(ni.Creator.UserName) + "\"" +
                            ",\"FirstName\":\"" + Base64.encode(ni.Creator.FirstName) + "\"" +
                            ",\"LastName\":\"" + Base64.encode(ni.Creator.LastName) + "\"" +
                            ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                                paramsContainer.ApplicationID, ni.Creator.UserID.Value) + "\"" +
                        "}"
                    ) +
                    ",\"LikesCount\":" + (ni.LikesCount.HasValue ? ni.LikesCount.Value : 0).ToString() +
                    ",\"VisitsCount\":" + (ni.VisitsCount.HasValue ? ni.VisitsCount.Value : 0).ToString() +
                    ",\"ExpertsCount\":" + (ni.ExpertsCount.HasValue ? ni.ExpertsCount.Value : 0).ToString() +
                    ",\"MembersCount\":" + (ni.MembersCount.HasValue ? ni.MembersCount.Value : 0).ToString() +
                    ",\"ChildsCount\":" + (ni.ChildsCount.HasValue ? ni.ChildsCount.Value : 0).ToString() +
                    ",\"HasChild\":" + (ni.ChildsCount.HasValue && ni.ChildsCount.Value > 0).ToString().ToLower() +
                    ",\"RelatedNodesCount\":" + (ni.RelatedNodesCount.HasValue ? ni.RelatedNodesCount.Value : 0).ToString() +
                    ",\"LikeStatus\":" + (ni.LikeStatus.HasValue && ni.LikeStatus.Value).ToString().ToLower() +
                    ",\"IconURL\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                        ni.NodeID.Value, DefaultIconTypes.Node, ni.NodeTypeID) + "\"" +
                    (!coverPhotoUrl.HasValue || !coverPhotoUrl.Value ? string.Empty :
                        ",\"CoverPhotoURL\":\"" + DocumentUtilities.get_cover_photo_url(paramsContainer.Tenant.Id,
                            ni.NodeID.Value, false, false) + "\""
                    ) +
                    ",\"Extensions\":[" + string.Join(",", exts.Select(u => u.toJson())) + "]" +
                    ",\"Service\":" + (srvc == null ? "{}" : srvc.toJson(paramsContainer.Tenant.Id)) +
                    ",\"UserStatus\":{" + "\"IsServiceAdmin\":" + isServiceAdmin.ToString().ToLower() +
                        ",\"IsAreaAdmin\":" + isAreaAdmin.ToString().ToLower() +
                        ",\"IsCreator\":" + isCreator.ToString().ToLower() +
                        ",\"IsContributor\":" + isContributor.ToString().ToLower() +
                        ",\"IsExpert\":" + isExpert.ToString().ToLower() +
                        ",\"IsMember\":" + isMember.ToString().ToLower() +
                        ",\"IsAdminMember\":" + isAdminMember.ToString().ToLower() +
                        ",\"Editable\":" + editable.ToString().ToLower() +
                        "}" +
                    "}";
            }

            responseText += isArray || lst.Count > 1 ? "]" : string.Empty;

            if (string.IsNullOrEmpty(responseText)) responseText = "{}";
        }

        protected void enable_extension(Guid? ownerId, ExtensionType extensionType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!ownerId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (ownerId.HasValue) _save_error_log(Modules.Log.Action.EnableExtension_PermissionFailed, ownerId, null,
                     "{\"Extension\":\"" + Base64.encode(extensionType.ToString()) + "\"}");
                return;
            }

            bool result = CNController.enable_extension(paramsContainer.Tenant.Id,
                ownerId.Value, extensionType, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.EnableExtension,
                    SubjectID = ownerId,
                    Info = "{\"Extension\":\"" + extensionType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void disable_extension(Guid? ownerId, ExtensionType extensionType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!ownerId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (ownerId.HasValue) _save_error_log(Modules.Log.Action.DisableExtension_PermissionFailed, ownerId, null,
                     "{\"Extension\":\"" + Base64.encode(extensionType.ToString()) + "\"}");
                return;
            }

            bool result = CNController.disable_extension(paramsContainer.Tenant.Id,
                ownerId.Value, extensionType, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.DisableExtension,
                    SubjectID = ownerId,
                    Info = "{\"Extension\":\"" + extensionType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_extension_title(Guid? ownerId, ExtensionType extensionType, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 90)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!ownerId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (ownerId.HasValue) _save_error_log(Modules.Log.Action.SetExtensionTitle_PermissionFailed, ownerId, null,
                     "{\"Extension\":\"" + Base64.encode(extensionType.ToString()) + "\"}");
                return;
            }

            bool result = CNController.set_extension_title(paramsContainer.Tenant.Id,
                ownerId.Value, extensionType, title, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetExtensionTitle,
                    SubjectID = ownerId,
                    Info = "{\"Extension\":\"" + extensionType.ToString() + "\",\"Title\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void move_extension(Guid? ownerId, ExtensionType extensionType, bool? moveDown, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!ownerId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (ownerId.HasValue) _save_error_log(Modules.Log.Action.MoveExtension_PermissionFailed, ownerId, null,
                     "{\"Extension\":\"" + Base64.encode(extensionType.ToString()) + "\"}");
                return;
            }

            bool result = CNController.move_extension(paramsContainer.Tenant.Id, ownerId.Value,
                extensionType, moveDown.HasValue && moveDown.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.MoveExtension,
                    SubjectID = ownerId,
                    Info = "{\"Extension\":\"" + extensionType.ToString() + "\"" +
                        ",\"MoveDown\":" + (moveDown.HasValue && moveDown.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_extensions(Guid? ownerId, bool? initialize, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!ownerId.HasValue || (
                initialize.HasValue && initialize.Value && !CNController.initialize_extensions(paramsContainer.Tenant.Id,
                ownerId.Value, paramsContainer.CurrentUserID.Value)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            List<Extension> extensions = CNUtilities.extend_extensions(paramsContainer.Tenant.Id,
                CNController.get_extensions(paramsContainer.Tenant.Id, ownerId.Value));

            responseText = "{\"Extensions\":[" + string.Join(",", extensions.Select(u => u.toJson())) + "]}";
        }

        protected void has_extension(Guid? ownerId, ExtensionType extensionType, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            responseText = (ownerId.HasValue && CNController.has_extension(paramsContainer.Tenant.Id,
                ownerId.Value, extensionType)).ToString().ToLower();
        }

        protected void get_intellectual_properties_count(Guid? nodeTypeId,
            Guid? userId, bool? isDocument, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            List<NodesCount> nodesCount = !userId.HasValue ? new List<NodesCount>() :
                CNController.get_intellectual_properties_count(paramsContainer.Tenant.Id,
                userId.Value, nodeTypeId, null, null, paramsContainer.CurrentUserID, isDocument, null, null);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodesCount.Select(
                u => _get_nodes_count_json(u))) + "]}";
        }

        protected void get_intellectual_properties(List<Guid> nodeTypeIds, Guid? userId, Guid? nodeId, string additionalId,
            string searchText, bool? isDocument, bool? hasChild, DateTime? lowerDateLimit, DateTime? upperDateLimit,
            int? lowerBoundary, int? count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            long totalCount = 0;

            List<Node> nodes = !userId.HasValue ? new List<Node>() :
                CNController.get_intellectual_properties(paramsContainer.Tenant.Id, userId.Value, nodeTypeIds,
                nodeId, additionalId, paramsContainer.CurrentUserID, searchText, isDocument,
                lowerDateLimit, upperDateLimit, lowerBoundary, count, ref totalCount);

            List<Guid> haveChild = !hasChild.HasValue || !hasChild.Value ? new List<Guid>() :
                CNController.have_childs(paramsContainer.Tenant.Id, nodes.Select(u => u.NodeID.Value).ToList());

            nodes.ForEach(u => u.HasChild = haveChild.Exists(v => v == u.NodeID));

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void _get_intellectual_properties_of_friends(
            ref List<Node> retNodes, Guid? nodeTypeId, ref int? lowerBoundary, int? count)
        {
            if (lowerBoundary <= 0) lowerBoundary = 1;

            if (!count.HasValue) count = 20;

            List<Node> nodes = CNController.get_intellectual_properties_of_friends(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, nodeTypeId, lowerBoundary, count + (count / 2));

            List<PermissionType> lstPT = new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract };

            List<Guid> availableIds = PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                nodes.Select(u => u.NodeID.Value).ToList(), PrivacyObjectType.Node, lstPT).Keys.ToList();

            int newBoundary = !lowerBoundary.HasValue ? 0 : lowerBoundary.Value;

            foreach (Node nd in nodes)
            {
                if (retNodes.Count >= count) break;
                if (availableIds.Contains(nd.NodeID.Value)) retNodes.Add(nd);
                ++newBoundary;
            }

            if (lowerBoundary != newBoundary)
            {
                lowerBoundary = newBoundary;
                if (retNodes.Count < count) _get_intellectual_properties_of_friends(
                    ref retNodes, nodeTypeId, ref lowerBoundary, count - retNodes.Count);
            }
        }

        protected void get_intellectual_properties_of_friends(Guid? nodeTypeId,
            int? lowerBoundary, int? count, bool? creatorInfo, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<Node> nodes = new List<Node>();

            _get_intellectual_properties_of_friends(ref nodes, nodeTypeId, ref lowerBoundary, count);

            List<User> creators = !creatorInfo.HasValue || !creatorInfo.Value ? new List<User>() :
                UsersController.get_users(paramsContainer.Tenant.Id, nodes.Select(u => u.Creator.UserID.Value).ToList());

            nodes.ForEach(u => u.Creator = creators.Where(x => x.UserID == u.Creator.UserID).FirstOrDefault());

            responseText = "{\"LastItem\":" + (lowerBoundary.HasValue && lowerBoundary > 0 ?
                lowerBoundary - 1 : 0).ToString() + ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson(paramsContainer.Tenant.Id, true))) + "]}";
        }

        protected string _get_explorer_node_type_json(Hierarchy hierarchyItem,
            ref List<Hierarchy> hierarchy, ref SortedList<Guid, bool> refList)
        {
            if (refList.ContainsKey(hierarchyItem.ID.Value)) return string.Empty;
            refList[hierarchyItem.ID.Value] = true;

            List<Hierarchy> childs = hierarchy.Where(u => u.ParentID == hierarchyItem.ID).ToList();

            string retStr = "{\"ID\":\"" + hierarchyItem.ID.ToString() + "\"" +
                ",\"Name\":\"" + Base64.encode(hierarchyItem.Name) + "\"";

            if (childs.Count > 0)
            {
                retStr += ",\"Childs\":[";

                bool isFirst = true;
                foreach (Hierarchy c in childs)
                {
                    string cStr = _get_explorer_node_type_json(c, ref hierarchy, ref refList);
                    if (string.IsNullOrEmpty(cStr)) continue;

                    retStr += (isFirst ? string.Empty : ",") + cStr;
                    isFirst = false;
                }

                retStr += "]";
            }

            return retStr + "}";
        }

        protected void explore(Guid? baseId, Guid? relatedId, List<Guid> baseTypeIds, List<Guid> relatedTypeIds,
            Guid? secondLevelNodeId, bool? registrationArea, bool? tags, bool? relations, int? lowerBoundary, int? count,
            string orderBy, bool? orderByDesc, string searchText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (baseId == Guid.Empty) baseId = null;

            if (string.IsNullOrEmpty(searchText) && !baseId.HasValue)
            {
                List<ExtensionType> exts = new List<ExtensionType>() { ExtensionType.Browser };
                if (Modules.RaaiVanConfig.Modules.Documents(paramsContainer.Tenant.Id)) exts.Add(ExtensionType.Documents);

                List<NodeType> nodeTypes = CNController.get_node_types_with_extension(paramsContainer.Tenant.Id, exts);

                if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                {
                    List<Guid> serviceIds = CNController.is_service_admin(paramsContainer.Tenant.Id,
                        nodeTypes.Select(u => u.NodeTypeID.Value).ToList(), paramsContainer.CurrentUserID.Value);

                    List<Guid> ids = PrivacyController.check_access(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID, nodeTypes.Select(u => u.NodeTypeID.Value).ToList(),
                        PrivacyObjectType.NodeType, PermissionType.Create);

                    nodeTypes.Where(u => serviceIds.Any(x => x == u.NodeTypeID) || ids.Any(x => x == u.NodeTypeID)).ToList();
                }

                List<Hierarchy> hierarchy = CNController.get_node_types_hierarchy(paramsContainer.Tenant.Id,
                    nodeTypes.Select(u => u.NodeTypeID.Value).ToList()).OrderByDescending(u => u.Level).ToList();

                SortedList<Guid, bool> sl = new SortedList<Guid, bool>();

                //list of document trees
                string strTrees = string.Empty;

                if (Modules.RaaiVanConfig.Modules.Documents(paramsContainer.Tenant.Id))
                {
                    List<Tree> trees = DocumentsController.get_trees(paramsContainer.Tenant.Id);

                    strTrees = ",\"DocumentTrees\":[" + string.Join(",", trees.Select(
                        u => "{\"ID\":\"" + u.TreeID.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(u.Name) + "\"" + "}")) + "]";
                }
                //end of list of document trees

                responseText = "{\"NodeTypes\":[";

                bool isFirst = true;
                foreach (Hierarchy h in hierarchy)
                {
                    string ntJson = _get_explorer_node_type_json(h, ref hierarchy, ref sl);
                    if (string.IsNullOrEmpty(ntJson)) continue;

                    responseText += (isFirst ? string.Empty : ",") + ntJson;
                    isFirst = false;
                }

                responseText += "]" + strTrees + "}";

                return;
            }

            bool checkAccess = !paramsContainer.CurrentUserID.HasValue ||
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            long totalCount = 0;

            List<ExploreItem> items = CNController.explore(paramsContainer.Tenant.Id, baseId, relatedId,
                baseTypeIds, relatedTypeIds, secondLevelNodeId, registrationArea, tags, relations, lowerBoundary, count,
                orderBy, orderByDesc, searchText, checkAccess, paramsContainer.CurrentUserID, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Items\":[" + ProviderUtil.list_to_string<string>(items.Select(
                u => "{\"Base\":{\"NodeID\":\"" + u.BaseID.ToString() + "\"" +
                    ",\"NodeTypeID\":\"" + u.BaseTypeID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(u.BaseName) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(u.BaseType) + "\"" + "}" +
                    ",\"Related\":{\"NodeID\":\"" + u.RelatedID.ToString() + "\"" +
                    ",\"NodeTypeID\":\"" + u.RelatedTypeID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(u.RelatedName) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(u.RelatedType) + "\"" +
                    ",\"CreationDate\":\"" + (!u.RelatedCreationDate.HasValue ? string.Empty :
                        GenericDate.get_local_date(u.RelatedCreationDate.Value)) + "\"" +
                    ",\"ImageURL\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                        u.RelatedID.Value, DefaultIconTypes.Node, u.RelatedTypeID) + "\"" +
                    "}" +
                    ",\"IsTag\":" + (u.IsTag.HasValue && u.IsTag.Value).ToString().ToLower() +
                    ",\"IsRelation\":" + (u.IsRelation.HasValue && u.IsRelation.Value).ToString().ToLower() +
                    ",\"IsRegistrationArea\":" + (u.IsRegistrationArea.HasValue && u.IsRegistrationArea.Value).ToString().ToLower() +
                    "}").ToList()) + "]}";
        }

        protected void get_excel_headers(Guid? nodeTypeId, Dictionary<string, object> dic, ref string responseText) {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            NodeType nodeType = !nodeTypeId.HasValue ? null : CNController.get_node_type(paramsContainer.Tenant.Id, nodeTypeId.Value);

            if (nodeType == null) {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            FormType form = !CNController.has_extension(paramsContainer.Tenant.Id, nodeTypeId.Value, ExtensionType.Form) ? null :
                FGController.get_owner_form(paramsContainer.Tenant.Id, nodeTypeId.Value);

            List<FormElementTypes> notAllowed = new List<FormElementTypes>() {
                FormElementTypes.File,
                FormElementTypes.Form,
                FormElementTypes.Separator
            };

            List<FormElement> elements = form == null || !form.FormID.HasValue ? new List<FormElement>() :
                FGController.get_form_elements(paramsContainer.Tenant.Id, form.FormID.Value)
                    .Where(e => e.Type.HasValue && !notAllowed.Any(a => a == e.Type.Value)).ToList();

            List<FormElement> limits = FGController.get_element_limits(paramsContainer.Tenant.Id, nodeTypeId.Value);

            if (limits.Count > 0) elements = elements.Where(e => limits.Any(l => l.ElementID == e.ElementID)).ToList();

            DataTable tbl = new DataTable("template");

            List<object> row = new List<object>();

            Dictionary<string, string> nodeItems = new Dictionary<string, string>();

            new List<string>() { "node_name", "node_id", "node_parent_id", "node_tags", "node_abstract" }.ForEach(key => {
                tbl.Columns.Add(key);
                row.Add(dic.ContainsKey(key) ? Base64.decode(dic[key].ToString()) : key);
            });

            elements.OrderBy(e => !e.SequenceNumber.HasValue ? 1000 : e.SequenceNumber.Value)
                .Where(e => !string.IsNullOrEmpty(e.Name)).ToList().ForEach(e => {
                    tbl.Columns.Add(e.Name);
                    row.Add(e.Title);
                });

            tbl.Rows.Add(row.ToArray());

            ExcelUtilities.ExportToExcel(nodeType.Name + "_" + PublicMethods.get_random_number().ToString(), tbl, true, null, null, null);
        }

        protected void import_nodes_from_excel(Guid? nodeTypeId, DocFileInfo uploaded,
            int? sheetNo, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue || (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID) &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id, nodeTypeId.Value, paramsContainer.CurrentUserID.Value) &&
                !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, 
                    nodeTypeId.Value, PrivacyObjectType.NodeType, PermissionType.CreateBulk)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (nodeTypeId == Guid.Empty || uploaded == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            try
            {
                if (!sheetNo.HasValue || sheetNo.Value <= 0) sheetNo = 1;

                if (uploaded != null) uploaded.FolderName = FolderNames.TemporaryFiles;

                if (!uploaded.exists())
                {
                    responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    return;
                }

                Dictionary<string, object> map = new Dictionary<string, object>();
                System.Xml.XmlDocument doc = ExcelUtilities.Excel2XML(uploaded, sheetNo.Value, ref map);

                if (doc == null)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    return;
                }

                List<FormType> formInstances = new List<FormType>();
                List<FormElement> formElements = new List<FormElement>();
                Dictionary<Guid, object> nodesDic = new Dictionary<Guid, object>();

                FormType form = !CNController.has_extension(paramsContainer.Tenant.Id, nodeTypeId.Value, ExtensionType.Form) ?
                    null : FGController.get_owner_form(paramsContainer.Tenant.Id, nodeTypeId.Value);

                bool result = FGImport.extract_forms_from_xml(paramsContainer.Tenant.Id, form == null ? null : form.FormID,
                    doc, map, paramsContainer.CurrentUserID.Value, ref formInstances, ref formElements, ref nodesDic);

                if (!result)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    return;
                }

                //Extract nodes
                List<ExchangeNode> nodes = new List<ExchangeNode>();

                foreach (Guid nodeId in nodesDic.Keys)
                {
                    Dictionary<string, object> theNd = (Dictionary<string, object>)nodesDic[nodeId];

                    string name = !theNd.ContainsKey("Name") ? null : ((string)theNd["Name"]).Trim();
                    string additionalId = !theNd.ContainsKey("AdditionalID") ? null : ((string)theNd["AdditionalID"]).Trim();
                    string parentAdditionalId = !theNd.ContainsKey("ParentAdditionalID") ? null :
                        ((string)theNd["ParentAdditionalID"]).Trim();
                    string tags = !theNd.ContainsKey("Tags") ? null : ((string)theNd["Tags"]).Trim();
                    string desc = !theNd.ContainsKey("Abstract") ? null : ((string)theNd["Abstract"]).Trim();

                    nodes.Add(new ExchangeNode()
                    {
                        NodeID = nodeId,
                        Name = name,
                        AdditionalID = additionalId,
                        ParentAdditionalID = parentAdditionalId,
                        Tags = tags,
                        Abstract = desc
                    });
                }

                nodes = ExchangeNode.sort_nodes_based_on_parents(nodes);
                //end of Extract nodes

                List<Guid> newNodeIds = new List<Guid>();
                Dictionary<string, Guid> existingNodeIds = new Dictionary<string, Guid>();
                List<FormType> existingInstances = new List<FormType>();

                int SIZE = 200;

                //Save nodes
                PublicMethods.split_list<ExchangeNode>(nodes, SIZE, nds =>
                {
                    List<Guid> ids = new List<Guid>();

                    //Update existing node ids
                    Dictionary<string, Guid> tmp = CNController.get_node_ids(paramsContainer.Tenant.Id, nodeTypeId.Value,
                        nds.Where(u => !string.IsNullOrEmpty(u.AdditionalID)).Select(x => x.AdditionalID).ToList());

                    List<Guid> ndIds = tmp.Values.Where(u => !existingNodeIds.Values.Any(x => x == u)).Distinct().ToList();
                    tmp.ToList().ForEach(a => existingNodeIds[a.Key] = a.Value);

                    existingInstances.AddRange(FGController.get_owner_form_instances(paramsContainer.Tenant.Id, ndIds));
                    //end of Update existing node ids

                    DEController.update_nodes(paramsContainer.Tenant.Id,
                        nds, nodeTypeId, null, paramsContainer.CurrentUserID.Value, ref ids);

                    if (ids.Count > 0) newNodeIds.AddRange(ids);
                });
                //end of Save nodes

                //Refine existing items
                existingNodeIds.ToList().ForEach(x =>
                {
                    string additionalId = x.Key;
                    Guid realNodeId = x.Value;
                    Guid fakeNodeId = nodes.Where(u => u.AdditionalID == additionalId).First().NodeID.Value;

                    Guid? existingInstanceId = null;
                    if (existingInstances.Any(u => u.OwnerID == realNodeId))
                        existingInstanceId = existingInstances.Where(u => u.OwnerID == realNodeId).First().InstanceID;

                    formInstances.Where(u => u.OwnerID == fakeNodeId).ToList().ForEach(f =>
                    {
                        f.OwnerID = realNodeId;

                        if (existingInstanceId.HasValue)
                        {
                            formElements.Where(e => e.FormInstanceID == f.InstanceID)
                                .ToList().ForEach(itm => itm.FormInstanceID = existingInstanceId);

                            f.InstanceID = existingInstanceId;
                        }
                    });
                });
                //end of Refine existing items

                //Save form instances
                List<FormType> newInstances = formInstances.Where(
                    u => !existingInstances.Any(x => x.InstanceID == u.InstanceID)).ToList();

                PublicMethods.split_list<FormType>(newInstances, SIZE, insts =>
                {
                    FGController.create_form_instances(paramsContainer.Tenant.Id,
                        insts, paramsContainer.CurrentUserID.Value);
                });
                //end of Save form instances


                //Extract GuidItems for Node Elements => 'NodeTypeID:NodeID,NodeTypeID:NodeID,...'
                Dictionary<string, Dictionary<string, List<Guid>>> selectedIDsDic = new Dictionary<string, Dictionary<string, List<Guid>>>();

                formElements.Where(e => e.ElementID.HasValue && e.Type == FormElementTypes.Node && 
                    !string.IsNullOrEmpty(e.TextValue)).ToList().ForEach(e => {
                    e.TextValue.Split(',').Where(str => !string.IsNullOrEmpty(str))
                        .Select(str => str.Split(':').Where(s => !string.IsNullOrEmpty(s)))
                        .Where(lst => lst != null && lst.Count() == 2).ToList().ForEach(lst =>
                        {
                            string ntId = lst.First(), nId = lst.Last();

                            if (!selectedIDsDic.ContainsKey(ntId)) selectedIDsDic[ntId] = new Dictionary<string, List<Guid>>();

                            if (!selectedIDsDic[ntId].ContainsKey(nId)) selectedIDsDic[ntId][nId] = new List<Guid>();

                            selectedIDsDic[ntId][nId].Add(e.ElementID.Value);
                        });
                });

                selectedIDsDic.Keys.ToList().ForEach((Action<string>)(ntId =>
                {
                    Guid? selectedNTId = CNController.get_node_type_id(paramsContainer.Tenant.Id, ntId);
                    if (!selectedNTId.HasValue) return;

                    Dictionary<string, Guid> nIdsDic = CNController.get_node_ids(paramsContainer.Tenant.Id, 
                        selectedNTId.Value, selectedIDsDic[ntId].Keys.ToList());

                    List<Node> selectedNodes =
                        CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, nodeIds: nIdsDic.Values.ToList());
                    
                    nIdsDic.Keys.ToList().ForEach(id => {
                        selectedIDsDic[ntId][id]
                            .Select(eId => formElements.Where(e => e.ElementID == eId).FirstOrDefault())
                            .Where(e => e != null).ToList()
                            .ForEach(e =>
                            {
                                Guid sNId = nIdsDic[id];
                                Node node = selectedNodes.Where(n => n.NodeID == sNId).FirstOrDefault();

                                e.GuidItems.Add(new SelectedGuidItem(sNId, name: node == null ? id : node.Name,
                                    code: string.Empty, type: SelectedGuidItemType.None));
                            });
                    });
                }));

                formElements.Where(e => e.Type == FormElementTypes.Node && e.GuidItems.Count > 0).ToList().ForEach(e =>
                {
                    e.TextValue = string.Join(" ~ ", e.GuidItems.Select(g => Expressions.get_markup(g.ID.ToString(), "Node", g.Name)));
                });
                //end of Extract GuidItems for Node Elements


                //Extract GuidItems for User Elements => 'UserName,UserName,...'
                Dictionary<string, List<Guid>> selectedUserNamesDic = new Dictionary<string, List<Guid>>();

                formElements.Where(e => e.ElementID.HasValue && e.Type == FormElementTypes.User &&
                    !string.IsNullOrEmpty(e.TextValue)).ToList().ForEach(e =>
                    {
                        e.TextValue.Split(',').Select(str => str.Trim().ToLower())
                            .Where(str => !string.IsNullOrEmpty(str)).ToList().ForEach(username =>
                            {
                                if (!selectedUserNamesDic.ContainsKey(username)) selectedUserNamesDic[username] = new List<Guid>();

                                selectedUserNamesDic[username].Add(e.ElementID.Value);
                            });
                    });

                List<User> selectedUsers = UsersController.get_users(paramsContainer.Tenant.Id, selectedUserNamesDic.Keys.ToList());
                
                selectedUserNamesDic.Keys.ToList().ForEach(username =>
                {
                    User theUser = selectedUsers.Where(u => username == u.UserName.ToLower()).FirstOrDefault();

                    if (theUser == null || !theUser.UserID.HasValue) return;

                    selectedUserNamesDic[username]
                        .Select(eId => formElements.Where(e => e.ElementID == eId).FirstOrDefault())
                        .Where(e => e != null).ToList()
                        .ForEach(e => e.GuidItems.Add(new SelectedGuidItem(theUser.UserID.Value, name: theUser.FullName, 
                            code: string.Empty, type: SelectedGuidItemType.None)));
                });
                
                formElements.Where(e => e.Type == FormElementTypes.User && e.GuidItems.Count > 0).ToList().ForEach(e =>
                {
                    e.TextValue = string.Join(" ~ ", e.GuidItems.Select(g => Expressions.get_markup(g.ID.ToString(), "User", g.Name)));
                });
                //end of Extract GuidItems for User Elements


                //Save instance elements
                PublicMethods.split_list<FormElement>(formElements, SIZE, elems => {
                    FGController.save_form_instance_elements(paramsContainer.Tenant.Id,
                        elems, new List<Guid>(), paramsContainer.CurrentUserID.Value);
                });
                //end of Save instance elements

                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
            }
            catch (Exception ex)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

                LogController.save_error_log(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    "ImportNodesFromExcel", ex, ModuleIdentifier.CN);
            }
        }

        protected void xml2node(Guid? nodeTypeId, DocFileInfo xmlFile, string map,
            bool? attachUploadedFile, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //initial checks
            FormType form = !nodeTypeId.HasValue || nodeTypeId == Guid.Empty ? null :
                FGController.get_owner_form(paramsContainer.Tenant.Id, nodeTypeId.Value);

            if (form == null || !form.FormID.HasValue || !paramsContainer.CurrentUserID.HasValue ||
                !CNController.has_extension(paramsContainer.Tenant.Id, nodeTypeId.Value, ExtensionType.Form))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            else if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id,
                    nodeTypeId.Value, paramsContainer.CurrentUserID.Value) &&
                !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    nodeTypeId.Value, PrivacyObjectType.NodeType, PermissionType.Create))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            //end of initial checks

            //create form instance
            bool isTemporary = true;

            FormType formInstance = FGController.get_owner_form_instances(paramsContainer.Tenant.Id, nodeTypeId.Value,
                form.FormID.Value, isTemporary, paramsContainer.CurrentUserID.Value).FirstOrDefault();

            if (formInstance == null)
            {
                formInstance = new FormType()
                {
                    InstanceID = Guid.NewGuid(),
                    FormID = form.FormID,
                    OwnerID = nodeTypeId.Value,
                    IsTemporary = isTemporary,
                    CreationDate = DateTime.Now
                };

                formInstance.Creator.UserID = paramsContainer.CurrentUserID.Value;

                if (!FGController.create_form_instance(paramsContainer.Tenant.Id, formInstance)) formInstance = null;
            }

            if (formInstance == null || !formInstance.InstanceID.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            //end of create form instance

            //import form from xml
            List<FormElement> savedElements = new List<FormElement>();
            DocFileInfo logo = null;
            string nodeName = string.Empty, errorMessage = string.Empty;

            if (!FGImport.import_form(paramsContainer.Tenant.Id, formInstance.InstanceID, xmlFile, map,
                paramsContainer.CurrentUserID.Value, ref savedElements, ref nodeName, ref logo, ref errorMessage))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            //end of import form from xml

            //register new node
            List<NodeCreator> contributors = new List<NodeCreator>();
            NodeCreator _nc = new NodeCreator();
            _nc.User.UserID = paramsContainer.CurrentUserID.Value;
            _nc.CollaborationShare = 100;
            contributors.Add(_nc);

            Guid nodeId = Guid.NewGuid();

            if (!register_new_node(nodeId, nodeTypeId, null, null, null, nodeName, null, null, contributors,
                null, null, null, formInstance.InstanceID, DateTime.Now, logo, false, false, ref responseText)) return;

            if (attachUploadedFile.HasValue && attachUploadedFile.Value)
            {
                xmlFile.move(FolderNames.TemporaryFiles, FolderNames.Attachments);

                if (!DocumentsController.add_file(paramsContainer.Tenant.Id, nodeId,
                    FileOwnerTypes.Node, xmlFile, paramsContainer.CurrentUserID.Value))
                    xmlFile.move(FolderNames.Attachments, FolderNames.TemporaryFiles);
            }
            //end of register new node
        }

        private DataTable _add_nodes_to_datatable(List<Node> nodes,
            List<FormType> formInstances, List<FormElement> instanceElements)
        {
            List<FormElement> limits = new List<FormElement>();

            if (formInstances.Count > 0 && instanceElements.Count > 0 && nodes.Count > 0)
                limits = FGController.get_element_limits(paramsContainer.Tenant.Id, nodes[0].NodeTypeID.Value);

            List<FormElement> tableColumns = instanceElements
                .Where(x => limits.Count == 0 || (x.RefElementID.HasValue && x.RefElementID != x.ElementID) ||
                    limits.Any(a => a.ElementID == x.RefElementID || a.ElementID == x.ElementID))
                .Select(u => u.RefElementID.HasValue ? u.RefElementID.Value : u.ElementID.Value)
                .Distinct()
                .Select(x => instanceElements.Where(e => e.RefElementID == x || e.ElementID == x).FirstOrDefault())
                .Where(u => u.Type != FormElementTypes.Separator && u.Type != FormElementTypes.Form && u.Type != FormElementTypes.File)
                .OrderBy(x => !x.SequenceNumber.HasValue ? 0 : x.SequenceNumber.Value)
                .ToList();

            DataTable tbl = new DataTable();

            tbl.Columns.Add("Name");
            tbl.Columns.Add("AdditionalID");
            tbl.Columns.Add("NodeType");

            tableColumns.ForEach(col => tbl.Columns.Add(col.Title));

            nodes.ForEach(n =>
            {
                Guid? instanceId = formInstances.Where(u => u.OwnerID == n.NodeID)
                    .Select(x => x.InstanceID.Value).FirstOrDefault();

                List<object> rowData = new List<object>();

                rowData.Add(n.Name);
                rowData.Add(n.AdditionalID);
                rowData.Add(n.NodeType);

                tableColumns.ForEach(col =>
                {
                    FormElement elem = !instanceId.HasValue ? null :
                        instanceElements.Where(u => u.FormInstanceID == instanceId && u.RefElementID.HasValue &&
                        (u.RefElementID == col.RefElementID || u.RefElementID == col.ElementID)).FirstOrDefault();

                    rowData.Add(elem == null ? string.Empty : elem.toString(paramsContainer.Tenant.Id));
                });

                tbl.Rows.Add(rowData.ToArray());
            });

            return tbl;
        }

        public void export_nodes_to_excel(List<Node> nodes, Dictionary<string, string> columnNames, bool formDetails = false)
        {
            formDetails = formDetails && Modules.RaaiVanConfig.Modules.FormGenerator(paramsContainer.Tenant.Id);

            List<FormType> formInstances = new List<FormType>();
            List<FormElement> instanceElements = new List<FormElement>();

            if (formDetails)
            {
                formInstances = FGController.get_owner_form_instances(paramsContainer.Tenant.Id,
                    nodes.Select(u => u.NodeID.Value).ToList());

                instanceElements = FGController.get_form_instance_elements(paramsContainer.Tenant.Id,
                        formInstances.Select(u => u.InstanceID.Value).ToList());
            }

            try
            {
                List<KeyValuePair<string, DataTable>> tables = new List<KeyValuePair<string, DataTable>>();

                if (!formDetails)
                {
                    tables.Add(new KeyValuePair<string, DataTable>("Sheet One",
                        _add_nodes_to_datatable(nodes, new List<FormType>(), new List<FormElement>())));
                }
                else
                {
                    nodes.Select(x => x.NodeTypeID.Value).Distinct().ToList().ForEach(typeId =>
                    {
                        List<Node> sheetNodes = nodes.Where(u => u.NodeTypeID == typeId).ToList();

                        List<FormType> curInstances =
                            formInstances.Where(u => sheetNodes.Any(x => x.NodeID == u.OwnerID)).ToList();

                        List<FormElement> curElements =
                            instanceElements.Where(u => curInstances.Any(x => x.InstanceID == u.FormInstanceID)).ToList();

                        tables.Add(new KeyValuePair<string, DataTable>(sheetNodes[0].NodeType,
                            _add_nodes_to_datatable(sheetNodes, curInstances, curElements)));
                    });
                }

                string searchFileName = "Search_" + PublicMethods.get_random_number().ToString();

                bool rtl = columnNames.Keys.Count > 0 &&
                    PublicMethods.text_direction(columnNames[columnNames.Keys.First()]) == TextDirection.RTL;

                ExcelUtilities.ExportToExcel(searchFileName, tables, rtl, columnNames, string.Empty, null);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    "ExportNodesToExcel", ex, ModuleIdentifier.CN, LogLevel.Fatal);
            }
        }

        /* Service */

        protected void get_service(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasRight = nodeTypeId.HasValue && (
                AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID) ||
                CNController.is_service_admin(paramsContainer.Tenant.Id, nodeTypeId.Value, paramsContainer.CurrentUserID.Value));

            Service service = null;

            if (!(hasRight && CNController.initialize_service(paramsContainer.Tenant.Id, nodeTypeId.Value)) ||
                (service = CNController.get_service(paramsContainer.Tenant.Id, nodeTypeId.Value)) == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (!service.NodeType.NodeTypeID.HasValue) service.NodeType.NodeTypeID = nodeTypeId;

            responseText = service.toJson(paramsContainer.Tenant.Id, isAdmin: hasRight);
        }

        protected void get_services(bool? isDocument, bool? isKnowledge, bool? all, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!all.HasValue || !all.Value)
                all = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            List<Service> services = CNController.get_services(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, isDocument, isKnowledge, !(all.HasValue && all.Value));

            responseText = "{\"Services\":[" + string.Join(",", services.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void get_service_registeration_info(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<Extension> extensions = CNUtilities.extend_extensions(paramsContainer.Tenant.Id,
                CNController.get_extensions(paramsContainer.Tenant.Id, nodeTypeId.Value));

            bool isServiceAdmin = CNController.is_service_admin(paramsContainer.Tenant.Id,
                nodeTypeId.Value, paramsContainer.CurrentUserID.Value);

            KnowledgeType kt = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, nodeTypeId.Value);

            responseText = "{\"IsServiceAdmin\":" + isServiceAdmin.ToString().ToLower() +
                ",\"Extensions\":{" + string.Join(",", extensions.Where(x => !x.Disabled.HasValue || !x.Disabled.Value)
                    .Select(u => "\"" + u.ExtensionType.ToString() + "\":" + u.toJson())) + "}" +
                (kt == null ? string.Empty : ",\"KnowledgeType\":" + kt.toJson()) +
                "}";
        }

        protected void set_service_title(Guid? nodeTypeId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 500)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.SetServiceTitle_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.set_service_title(paramsContainer.Tenant.Id, nodeTypeId.Value, title);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceTitle,
                    SubjectID = nodeTypeId,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_service_description(Guid? nodeTypeId, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(description) && description.Length > 3900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceDescription_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.set_service_description(paramsContainer.Tenant.Id, nodeTypeId.Value, description);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceDescription,
                    SubjectID = nodeTypeId,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_service_success_message(Guid? nodeTypeId, string message, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(message) && message.Length > 3900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceSuccessMessage_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.set_service_success_message(paramsContainer.Tenant.Id, nodeTypeId.Value, message);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceSuccessMessage,
                    SubjectID = nodeTypeId,
                    Info = "{\"Message\":\"" + Base64.encode(message) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_service_success_message(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            string message = CNController.get_service_success_message(paramsContainer.Tenant.Id, nodeTypeId.Value);
            if (message == null) message = string.Empty;

            responseText = "\"" + message + "\"";
        }

        protected void set_service_admin_type(Guid? nodeTypeId, ServiceAdminType adminType, Guid? adminNodeId,
            List<Guid> limitNodeTypeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.SetServiceAdminType_PermissionFailed, nodeTypeId);
                return;
            }

            if (adminNodeId == Guid.Empty) adminNodeId = null;

            bool result = CNController.set_service_admin_type(paramsContainer.Tenant.Id,
                nodeTypeId.Value, adminType, adminNodeId, limitNodeTypeIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceAdminType,
                    SubjectID = nodeTypeId,
                    Info = "{\"AdminType\":\"" + adminType.ToString() +
                        "\",\"AdminNodeID\":\"" + (adminNodeId.HasValue ? adminNodeId.ToString() : string.Empty) +
                        "\",\"LimitNodeTypeIDs\":[" + ProviderUtil.list_to_string<string>(limitNodeTypeIds.Select(u =>
                            "\"" + u.ToString() + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_max_acceptable_admin_level(Guid? nodeTypeId, int? maxAcceptableAdminLevel, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceMaxAcceptableAdminLevel_PermissionFailed, nodeTypeId);
                return;
            }

            if (maxAcceptableAdminLevel < 0) maxAcceptableAdminLevel = null;

            bool result = CNController.set_max_acceptable_admin_level(paramsContainer.Tenant.Id,
                nodeTypeId.Value, maxAcceptableAdminLevel);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"MaxAcceptableAdminLevel\":" +
                (maxAcceptableAdminLevel.HasValue ? maxAcceptableAdminLevel.ToString() : "null") + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceMaxAcceptableAdminLevel,
                    SubjectID = nodeTypeId,
                    Info = "{\"MaxAcceptableAdminLevel\":" +
                        (maxAcceptableAdminLevel.HasValue ? maxAcceptableAdminLevel.ToString() : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_contribution_limits(Guid? nodeTypeId, List<Guid> limitNodeTypeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetContributionLimits_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.set_contribution_limits(paramsContainer.Tenant.Id,
                nodeTypeId.Value, limitNodeTypeIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetContributionLimits,
                    SubjectID = nodeTypeId,
                    Info = "{\"LimitNodeTypeIDs\":[" + ProviderUtil.list_to_string<string>(limitNodeTypeIds.Select(u =>
                            "\"" + u.ToString() + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_contribution_limits(Guid? nodeTypeId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<NodeType> nodeTypes = !nodeTypeId.HasValue ? new List<NodeType>() :
                CNController.get_contribution_limits(paramsContainer.Tenant.Id, nodeTypeId.Value);

            responseText = "[" + string.Join(",", nodeTypes.Select(
                u => "{\"NodeTypeID\":\"" + u.NodeTypeID.ToString() + "\",\"Name\":\"" + Base64.encode(u.Name) + "\"}")) + "]";
        }

        protected void enable_contribution(Guid? nodeTypeId, bool? enable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEnableContribution_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.enable_contribution(paramsContainer.Tenant.Id,
                nodeTypeId.Value, enable.HasValue && enable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEnableContribution,
                    SubjectID = nodeTypeId,
                    Info = "{\"EnableContribution\":" + (enable.HasValue && enable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void no_content_service(Guid? nodeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;
                responseText = (nodeTypeId.HasValue &&
                    CNController.no_content_service(paramsContainer.Tenant.Id, nodeTypeId.Value)).ToString().ToLower();
                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.SetServiceNoContent_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.no_content_service(paramsContainer.Tenant.Id,
                nodeTypeId.Value, value.HasValue && value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceNoContent,
                    SubjectID = nodeTypeId,
                    Info = "{\"NoContent\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void is_knowledge(Guid? nodeTypeId, bool? isKnowledge, ref string responseText)
        {
            //Privacy Check: OK
            if (!isKnowledge.HasValue)
            {
                if (!paramsContainer.GBView) return;

                responseText = (nodeTypeId.HasValue &&
                    CNController.is_knowledge(paramsContainer.Tenant.Id, nodeTypeId.Value)).ToString().ToLower();
                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceIsKnowledge_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.is_knowledge(paramsContainer.Tenant.Id,
                nodeTypeId.Value, isKnowledge.HasValue && isKnowledge.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceIsKnowledge,
                    SubjectID = nodeTypeId,
                    Info = "{\"IsKnowledge\":" + (isKnowledge.HasValue && isKnowledge.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void is_document(Guid? nodeTypeId, bool? isDocument, ref string responseText)
        {
            //Privacy Check: OK
            if (!isDocument.HasValue)
            {
                if (!paramsContainer.GBView) return;
                responseText = (nodeTypeId.HasValue &&
                    CNController.is_document(paramsContainer.Tenant.Id, nodeTypeId.Value)).ToString().ToLower();
                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.SetServiceIsDocument_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.is_document(paramsContainer.Tenant.Id,
                nodeTypeId.Value, isDocument.HasValue && isDocument.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceIsDocument,
                    SubjectID = nodeTypeId,
                    Info = "{\"IsDocument\":" + (isDocument.HasValue && isDocument.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void enable_previous_version_select(Guid? nodeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;
                responseText = (nodeTypeId.HasValue &&
                    CNController.enable_previous_version_select(paramsContainer.Tenant.Id, nodeTypeId.Value)).ToString().ToLower();
                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.SetServiceEnablePreviousVersionSelect_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.enable_previous_version_select(paramsContainer.Tenant.Id,
                nodeTypeId.Value, value.HasValue && value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEnablePreviousVersionSelect,
                    SubjectID = nodeTypeId,
                    Info = "{\"EnablePreviousVersionSelect\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void is_tree(List<Guid> nodeTypeOrNodeIds, bool? isTree, ref string responseText)
        {
            //Privacy Check: OK
            if (!isTree.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> treeIds = CNController.is_tree(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", treeIds.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetServiceIsTree_PermissionFailed, nodeTypeOrNodeIds);
                return;
            }

            bool result = CNController.is_tree(paramsContainer.Tenant.Id, nodeTypeOrNodeIds, isTree.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceIsTree,
                    SubjectIDs = nodeTypeOrNodeIds,
                    Info = "{\"IsTree\":" + isTree.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void has_unique_membership(List<Guid> nodeTypeOrNodeIds, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> groupIds = CNController.has_unique_membership(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", groupIds.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetServiceUniqueMembership_PermissionFailed, nodeTypeOrNodeIds);
                return;
            }

            bool result = CNController.has_unique_membership(paramsContainer.Tenant.Id, nodeTypeOrNodeIds, value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceUniqueMembership,
                    SubjectIDs = nodeTypeOrNodeIds,
                    Info = "{\"Value\":" + value.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void has_unique_admin_member(List<Guid> nodeTypeOrNodeIds, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> groupIds = CNController.has_unique_admin_member(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", groupIds.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetServiceUniqueAdminMember_PermissionFailed, nodeTypeOrNodeIds);
                return;
            }

            bool result = CNController.has_unique_admin_member(paramsContainer.Tenant.Id, nodeTypeOrNodeIds, value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceUniqueAdminMember,
                    SubjectIDs = nodeTypeOrNodeIds,
                    Info = "{\"Value\":" + value.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void abstract_and_keywords_disabled(List<Guid> nodeTypeOrNodeIds, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> ids = CNController.abstract_and_keywords_disabled(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", ids.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = CNController.abstract_and_keywords_disabled(paramsContainer.Tenant.Id,
                nodeTypeOrNodeIds, value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void file_upload_disabled(List<Guid> nodeTypeOrNodeIds, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> ids = CNController.file_upload_disabled(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", ids.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = CNController.file_upload_disabled(paramsContainer.Tenant.Id,
                nodeTypeOrNodeIds, value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void related_nodes_select_disabled(List<Guid> nodeTypeOrNodeIds, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!value.HasValue)
            {
                if (!paramsContainer.GBView) return;

                List<Guid> ids = CNController.related_nodes_select_disabled(paramsContainer.Tenant.Id, nodeTypeOrNodeIds);

                responseText = "{" + string.Join(",", ids.Select(u => "\"" + u.ToString() + "\":true").ToList()) + "}";

                return;
            }

            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = CNController.related_nodes_select_disabled(paramsContainer.Tenant.Id,
                nodeTypeOrNodeIds, value.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void editable_for_admin(Guid? nodeTypeId, bool? editable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditableForAdmin_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.editable_for_admin(paramsContainer.Tenant.Id,
                nodeTypeId.Value, editable.HasValue && editable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditableForAdmin,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditableForAdmin\":" + (editable.HasValue && editable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void editable_for_creator(Guid? nodeTypeId, bool? editable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditableForCreator_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.editable_for_creator(paramsContainer.Tenant.Id,
                nodeTypeId.Value, editable.HasValue && editable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditableForCreator,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditableForCreator\":" + (editable.HasValue && editable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void editable_for_owners(Guid? nodeTypeId, bool? editable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditableForOwners_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.editable_for_owners(paramsContainer.Tenant.Id,
                nodeTypeId.Value, editable.HasValue && editable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditableForOwners,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditableForOwners\":" + (editable.HasValue && editable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void editable_for_experts(Guid? nodeTypeId, bool? editable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditableForExperts_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.editable_for_experts(paramsContainer.Tenant.Id,
                nodeTypeId.Value, editable.HasValue && editable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditableForExperts,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditableForExperts\":" + (editable.HasValue && editable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void editable_for_members(Guid? nodeTypeId, bool? editable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditableForMembers_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.editable_for_members(paramsContainer.Tenant.Id,
                nodeTypeId.Value, editable.HasValue && editable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditableForMembers,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditableForMembers\":" + (editable.HasValue && editable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void edit_suggestion(Guid? nodeTypeId, bool? enable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetServiceEditSuggestion_PermissionFailed, nodeTypeId);
                return;
            }

            bool result = CNController.edit_suggestion(paramsContainer.Tenant.Id,
                nodeTypeId.Value, enable.HasValue && enable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetServiceEditSuggestion,
                    SubjectID = nodeTypeId,
                    Info = "{\"EditSuggestion\":" + (enable.HasValue && enable.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void add_free_user(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (!nodeTypeId.HasValue || !userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AddServiceFreeUser_PermissionFailed, nodeTypeId, userId);
                return;
            }

            bool result = CNController.add_free_user(paramsContainer.Tenant.Id,
                nodeTypeId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            if (!result)
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else
            {
                User usr = UsersController.get_user(paramsContainer.Tenant.Id, userId.Value);

                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                    "\",\"User\":{\"UserID\":\"" + usr.UserID.ToString() +
                    "\",\"FirstName\":\"" + Base64.encode(usr.FirstName) + "\",\"LastName\":\"" + Base64.encode(usr.LastName) +
                    "\",\"UserName\":\"" + Base64.encode(usr.UserName) +
                    "\",\"ProfileImageURL\":\"" +
                        DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, usr.UserID.Value) + "\"}}";
            }

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddServiceFreeUser,
                    SubjectID = nodeTypeId,
                    Info = "{\"UserID\":\"" + userId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_free_user(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (!nodeTypeId.HasValue || !userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveServiceFreeUser_PermissionFailed, nodeTypeId, userId);
                return;
            }

            bool result = CNController.remove_free_user(paramsContainer.Tenant.Id,
                nodeTypeId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveServiceFreeUser,
                    SubjectID = nodeTypeId,
                    Info = "{\"UserID\":\"" + userId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void get_service_admins(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            List<User> admins = CNController.get_service_admins(paramsContainer.Tenant.Id, nodeTypeId.Value);

            responseText = "{\"ServiceAdmins\":[" + string.Join(",", admins.Select(
                u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                        paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                    "}"
            )) + "]}";
        }

        protected void add_service_admin(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (!nodeTypeId.HasValue || !userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.AddServiceAdmin_PermissionFailed, nodeTypeId, userId);
                return;
            }

            bool result = CNController.add_service_admin(paramsContainer.Tenant.Id,
                nodeTypeId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            if (!result)
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else
            {
                User usr = UsersController.get_user(paramsContainer.Tenant.Id, userId.Value);

                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                    "\",\"User\":{\"UserID\":\"" + usr.UserID.ToString() +
                    "\",\"FirstName\":\"" + Base64.encode(usr.FirstName) + "\",\"LastName\":\"" + Base64.encode(usr.LastName) +
                    "\",\"UserName\":\"" + Base64.encode(usr.UserName) +
                    "\",\"ProfileImageURL\":\"" +
                        DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, usr.UserID.Value) + "\"}}";
            }

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddServiceAdmin,
                    SubjectID = nodeTypeId,
                    Info = "{\"UserID\":\"" + userId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void remove_service_admin(Guid? nodeTypeId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (!nodeTypeId.HasValue || !userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue && userId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveServiceAdmin_PermissionFailed, nodeTypeId, userId);
                return;
            }

            bool result = CNController.remove_service_admin(paramsContainer.Tenant.Id,
                nodeTypeId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveServiceAdmin,
                    SubjectID = nodeTypeId,
                    Info = "{\"UserID\":\"" + userId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void is_service_admin(Guid? nodeTypeIdOrNodeId, ref string responseText)
        {
            responseText = (paramsContainer.GBEdit && nodeTypeIdOrNodeId.HasValue &&
                CNController.is_service_admin(paramsContainer.Tenant.Id,
                nodeTypeIdOrNodeId.Value, paramsContainer.CurrentUserID.Value)).ToString().ToLower();
        }

        public static Dictionary<string, string> get_replacement_dictionary(Guid applicationId, 
            Guid currentUserId, Guid nodeId, ref Node node, bool full = false)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            node = CNController.get_node(applicationId, nodeId, true);
            if (node == null) return dic;
            
            User currentUser = UsersController.get_user(applicationId, currentUserId);

            dic.Add("NodeName", node.Name);
            dic.Add("Class", node.NodeType);
            dic.Add("AdditionalID", node.AdditionalID);
            dic.Add("Creator", node.Creator.FullName);
            dic.Add("CreatorUserName", node.Creator.UserName);
            dic.Add("CreationDate", (node.CreationDate.HasValue ?
                GenericDate.get_local_date(node.CreationDate.Value, true) : string.Empty));
            dic.Add("CurrentUser", currentUser == null ? string.Empty : currentUser.FullName);
            dic.Add("CurrentUserName", currentUser == null ? string.Empty : currentUser.UserName);
            dic.Add("Now", GenericDate.get_local_date(DateTime.Now, true));

            if (full)
            {
                dic.Add("PublicationDate", !node.PublicationDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(node.PublicationDate, false));

                //Contirbutors
                List<NodeCreator> contributors = CNController.get_node_creators(applicationId, node.NodeID.Value, full: true);

                if (contributors.Count > 0)
                    contributors = contributors.Where(c => c.CollaborationShare.HasValue && c.CollaborationShare.Value > 0)
                        .OrderBy(c => c.CollaborationShare.Value).ToList();

                for (int i = 0; i < contributors.Count; i++)
                {
                    dic.Add("Contributor_" + (i + 1).ToString(), contributors[i].User.FullName);
                    dic.Add("ContributorUserName_" + (i + 1).ToString(), contributors[i].User.UserName);
                    dic.Add("Contribution_" + (i + 1).ToString(), contributors[i].CollaborationShare.Value.ToString());
                }
                //end of Contirbutors

                //Evaluators
                int? wfVersionId = KnowledgeController.get_last_history_version_id(applicationId, node.NodeID.Value);

                List<KnowledgeEvaluation> evaluations = 
                    KnowledgeController.get_evaluations_done(applicationId, node.NodeID.Value, wfVersionId);

                if (evaluations.Count > 0) evaluations = evaluations.Where(e => e.Score.HasValue && e.Score.Value > 0)
                         .OrderByDescending(e => e.EvaluationDate.Value).ToList();

                for (int i = 0; i < evaluations.Count; i++)
                {
                    dic.Add("Evaluator_" + (i + 1).ToString(), evaluations[i].Evaluator.FullName);
                    dic.Add("EvaluatorUserName_" + (i + 1).ToString(), evaluations[i].Evaluator.UserName);
                    dic.Add("EvaluationScore_" + (i + 1).ToString(), evaluations[i].Score.Value.ToString());
                }
                //end of Evaluators

                //ConfirmedBy
                if (node.Status == Status.Accepted)
                {
                    Dashboard dashboard = NotificationController.get_dashboards(applicationId,
                        null, null, node.NodeID.Value, null,
                        DashboardType.Knowledge, DashboardSubType.Admin, null, done: true, dateFrom: null,
                        dateTo: null, searchText: null, lowerBoundary: null, count: 10)
                        .OrderByDescending(d => d.ActionDate.Value).FirstOrDefault();

                    User dashUser = dashboard == null || !dashboard.UserID.HasValue ? null :
                        UsersController.get_user(applicationId, dashboard.UserID.Value);

                    if (dashUser != null)
                    {
                        dic.Add("ConfirmedBy", dashUser.FullName);
                        dic.Add("ConfirmedByUserName", dashUser.UserName);
                        dic.Add("ConfirmDate", GenericDate.get_local_date(dashboard.ActionDate.Value));
                    }
                }
                //end of ConfirmedBy

                Privacy privacy = PrivacyController.get_settings(applicationId, nodeId);
                if (privacy != null && privacy.Confidentiality != null && !string.IsNullOrEmpty(privacy.Confidentiality.Title))
                    dic.Add("Confidentiality", privacy.Confidentiality.Title);
            }

            return dic;
        }

        public static Dictionary<string, string> get_replacement_dictionary(Guid applicationId,
            Guid currentUserId, Guid nodeId, bool full = false)
        {
            Node node = new Node();
            return get_replacement_dictionary(applicationId, currentUserId, nodeId, ref node, full);
        }

        public void check_node_creation_access(Guid? nodeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Service service = !nodeTypeId.HasValue ? null : CNController.get_service(paramsContainer.Tenant.Id, nodeTypeId.Value);

            bool granted = service != null && !string.IsNullOrEmpty(service.Title);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeTypeId);

            bool serviceLevelAccess = granted && accessChecker.checkNodeEditAccess(AdminLevel.Service,
                privacyObjectType: PrivacyObjectType.NodeType, permission: PermissionType.Create);

            bool hasAccess = granted && (serviceLevelAccess ||
                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    nodeTypeId.Value, PrivacyObjectType.NodeType, PermissionType.Create));
            //end of check access

            responseText = "{\"Result\":" + hasAccess.ToString().ToLower() + "}";
        }

        public bool register_new_node(Guid? nodeId, Guid? nodeTypeId, Guid? parentNodeId, Guid? documentTreeNodeId,
            Guid? previousVersionId, string name, string description, List<string> tags, List<NodeCreator> contributors,
            Guid? ownerId, Guid? workflowId, Guid? adminAreaId, Guid? formInstanceId, DateTime? creationDate,
            DocFileInfo logo, bool? getExtendInfo, bool? getWorkFlowInfo, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return false;

            if (!string.IsNullOrEmpty(name)) name = name.Trim();

            if (tags == null) tags = new List<string>();
            if (contributors == null) contributors = new List<NodeCreator>();

            if (string.IsNullOrEmpty(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TitleIsNotDetermined + "\"}";
                return false;
            }
            else if ((!string.IsNullOrEmpty(name) && name.Length > 250) ||
                (tags != null && tags.Sum(u => string.IsNullOrEmpty(u) ? 0 : u.Length) > 1900))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return false;
            }
            else if (!PublicMethods.is_secure_title(name) || tags.Any(u => !PublicMethods.is_secure_title(u)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return false;
            }

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeTypeId);

            bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Service,
                privacyObjectType: PrivacyObjectType.NodeType, permission: PermissionType.Create);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeTypeId.HasValue) _save_error_log(Modules.Log.Action.AddNode_PermissionFailed, nodeTypeId);
                return false;
            }
            //end of check access

            if (ownerId == Guid.Empty) ownerId = null;
            if (!nodeId.HasValue || nodeId == Guid.Empty) nodeId = Guid.NewGuid();

            bool isSystemAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            Node nodeObject = new Node()
            {
                NodeID = nodeId,
                NodeTypeID = nodeTypeId,
                ParentNodeID = parentNodeId,
                DocumentTreeNodeID = documentTreeNodeId,
                PreviousVersionID = previousVersionId,
                Name = name,
                Description = description,
                Tags = tags,
                Contributors = contributors,
                //CreationDate = (creationDate.HasValue && (isSystemAdmin || isAdminServiceLevel) ? creationDate : DateTime.Now),
                CreationDate = (creationDate.HasValue ? creationDate : DateTime.Now),
                OwnerID = ownerId,
                AdminAreaID = adminAreaId
            };

            creationDate = nodeObject.CreationDate;

            nodeObject.Creator.UserID = paramsContainer.CurrentUserID.Value;

            NodeType nt = CNController.get_node_type(paramsContainer.Tenant.Id, nodeTypeId.Value);
            Service service = CNController.get_service(paramsContainer.Tenant.Id, nodeTypeId.Value);

            if (service != null && service.NoContent.HasValue && service.NoContent.Value &&
                nt != null && !string.IsNullOrEmpty(nt.Name) && string.IsNullOrEmpty(nodeObject.Name))
            {
                User creator = UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

                string fullName = ((string.IsNullOrEmpty(creator.FirstName) ? string.Empty : creator.FirstName) + " " +
                    (string.IsNullOrEmpty(creator.LastName) ? string.Empty : creator.LastName)).Trim();

                List<string> nameParts = new List<string>() { nt.Name };
                if (!string.IsNullOrEmpty(fullName)) nameParts.Add(fullName);
                nameParts.Add(GenericDate.get_local_date(DateTime.Now, true));

                nodeObject.Name = name = string.Join(" - ", nameParts);
            }

            if (nt == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.NodeTypeNotFound + "\"}";
                return false;
            }

            if (!string.IsNullOrEmpty(nt.AdditionalIDPattern) &&
                !Expressions.is_match(nt.AdditionalIDPattern, Expressions.Patterns.AdditionalID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AdditionalIDPatternIsNotValid + "\"}";
                return false;
            }
            else
            {
                nodeObject.AdditionalID_Main = CNUtilities.generate_new_additional_id(paramsContainer.Tenant.Id,
                    nodeTypeId.Value, nt.AdditionalIDPattern, creationDate);

                if (!string.IsNullOrEmpty(nodeObject.AdditionalID_Main))
                {
                    nodeObject.AdditionalID =
                        Expressions.replace(nodeObject.AdditionalID_Main, Expressions.Patterns.AutoTag, "");
                }
            }

            List<Dashboard> sentDashboards = new List<Dashboard>();

            string errorMessage = string.Empty;

            Guid? directorNodeId = null, directorUserId = null;

            bool result = CNController.register_new_node(paramsContainer.Tenant.Id, nodeObject,
                workflowId, formInstanceId, directorNodeId, directorUserId, ref sentDashboards, ref errorMessage);

            if (result && logo != null)
            {
                PublicMethods.set_timeout(() => {
                    IconMeta meta = null, hqMeta = null;

                    string res = string.Empty;
                    logo.FolderName = FolderNames.TemporaryFiles;
                    RVGraphics.create_icon(paramsContainer.Tenant.Id, 
                        nodeObject.NodeID.Value, IconType.Icon, logo.toByteArray(), ref res, ref hqMeta, ref meta);
                }, 0);
            }

            if (result) {
                string updatedAddId = update_additional_id(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value);

                if (!string.IsNullOrEmpty(updatedAddId)) nodeObject.AdditionalID = updatedAddId;
            }

            Node node = new Node();
            Dictionary<string, string> dic = !result ? new Dictionary<string, string>() :
                get_replacement_dictionary(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, nodeObject.NodeID.Value, ref node);

            string successMessge = !result ? string.Empty : Expressions.replace(
                    CNController.get_service_success_message(paramsContainer.Tenant.Id, nodeTypeId.Value),
                    ref dic, Expressions.Patterns.AutoTag, "___").Trim();

            string extendInfo = string.Empty;
            bool? isKnowledge = null;
            if (result && getExtendInfo.HasValue && getExtendInfo.Value)
            {
                List<Extension> extensions = CNController.get_extensions(paramsContainer.Tenant.Id, nodeTypeId.Value);
                bool formCompletionNeeded = extensions.Any(u => u.ExtensionType == ExtensionType.Form && u.Disabled == false) &&
                    FGController.get_element_limits(paramsContainer.Tenant.Id, nodeTypeId.Value).Where(
                        u => u.Necessary.HasValue && u.Necessary.Value).ToList().Count > 0;
                bool wikiCompletionNeeded = extensions.Any(u => u.ExtensionType == ExtensionType.Wiki && u.Disabled == false) &&
                    Modules.Wiki.WikiController.has_title(paramsContainer.Tenant.Id, node.NodeID.Value, paramsContainer.CurrentUserID);
                isKnowledge = CNController.is_knowledge(paramsContainer.Tenant.Id, nodeTypeId.Value);
                KnowledgeType kType = null;
                if (isKnowledge.Value)
                    kType = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, nodeTypeId.Value);

                string strExtensions = ",\"Extensions\":{" + ProviderUtil.list_to_string<string>(
                    extensions.Where(v => v.Disabled == false).Select(
                    u => "\"" + u.ExtensionType.ToString() + "\":" + u.toJson()).ToList()) + "}";

                extendInfo = strExtensions +
                    ",\"FormCompletionNeeded\":" + formCompletionNeeded.ToString().ToLower() +
                    ",\"WikiCompletionNeeded\":" + wikiCompletionNeeded.ToString().ToLower() +
                    ",\"IsKnowledge\":" + isKnowledge.ToString().ToLower() +
                    ",\"NodeSelectType\":\"" + (kType == null || kType.NodeSelectType == KnowledgeNodeSelectType.NotSet ?
                        string.Empty : kType.NodeSelectType.ToString()) + "\"";
            }

            string workfowInfo = string.Empty;
            if (result && getWorkFlowInfo.HasValue && getWorkFlowInfo.Value)
            {
                bool hasKnowledgePermission = false, hasWorkFlowPermission = false,
                    hasWFEditPermission = false, hideContributors = false;

                bool isServiceAdmin = CNController.is_service_admin(paramsContainer.Tenant.Id,
                    node.NodeID.Value, paramsContainer.CurrentUserID.Value);
                bool isAreaAdmin = CNController.is_node_admin(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, node.NodeID.Value,
                    node.NodeTypeID.Value, node.AdminAreaID, true, service, contributors);

                check_node_workflow_permissions(node, isKnowledge, isSystemAdmin, isServiceAdmin,
                    isAreaAdmin, true, ref hasKnowledgePermission, ref hasWorkFlowPermission,
                    ref hasWFEditPermission, ref hideContributors);

                workfowInfo = ",\"ShowWorkFlow\":" + hasWorkFlowPermission.ToString().ToLower() +
                    ",\"ShowKnowledgeOptions\":" + hasKnowledgePermission.ToString().ToLower();
            }

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"NodeID\":\"" + nodeObject.NodeID.ToString() + "\"" +
                ",\"AdditionalID\":\"" + Base64.encode(nodeObject.AdditionalID) + "\"" +
                ",\"Name\":\"" + Base64.encode(nodeObject.Name) + "\"" +
                ",\"SuccessMessage\":\"" + Base64.encode(successMessge) + "\"" +
                extendInfo + workfowInfo + "}";

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                List<InlineTag> inlineTags = Expressions.get_tagged_items(description);

                foreach (InlineTag tg in inlineTags)
                {
                    TaggedType tgTp = TaggedType.None;
                    if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                    tagged.Add(new TaggedItem(nodeObject.NodeID.Value, tg.ID.Value, TagContextType.Node, tgTp));
                }

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, false, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            //Send Notification
            if (result && sentDashboards.Count > 0)
                NotificationController.transfer_dashboards(paramsContainer.Tenant.Id, sentDashboards);
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeObject.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddNode,
                    SubjectID = nodeObject.NodeID,
                    Info = "{\"Name\":\"" + Base64.encode(nodeObject.Name) +
                        "\",\"Description\":\"" + Base64.encode(nodeObject.Description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log

            return result;
        }

        protected void get_admin_area_limits(Guid? nodeId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<NodeType> nodeTypes = !nodeId.HasValue ? new List<NodeType>() :
                CNController.get_admin_area_limits(paramsContainer.Tenant.Id, nodeId.Value);

            responseText = "{\"NodeTypes\":[" + string.Join(",", nodeTypes.Select(
                u => "{\"NodeTypeID\":\"" + u.NodeTypeID.ToString() + "\",\"Name\":\"" + Base64.encode(u.Name) + "\"}")) + "]}";
        }

        protected void set_admin_area(Guid? nodeId, Guid? areaId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Node nd = !nodeId.HasValue ? null : CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);
            
            bool creatorLevel = nd != null && nd.Creator.UserID.HasValue && nd.isPersonal(nd.Creator.UserID.Value);

            //check access
            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId, node: nd);

            bool hasAccess = accessChecker.checkNodeEditAccess(
                creatorLevel ? AdminLevel.Creator : AdminLevel.Service, permission: PermissionType.None);

            hasAccess = hasAccess || (nodeId.HasValue && NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, nodeId.Value, DashboardType.Knowledge, DashboardSubType.Admin, done: false));

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetAdminArea_PermissionFailed, nodeId);
                return;
            }
            //end of check access

            bool succeed = CNController.set_admin_area(paramsContainer.Tenant.Id, nodeId.Value, areaId);

            //Update Node AdditionalID
            if (succeed) update_additional_id(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value);
            //end of Update Node AdditionalID

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetAdminArea,
                    SubjectID = nodeId,
                    SecondSubjectID = areaId,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void set_contributors(Guid? nodeId, List<NodeCreator> contributors, Guid? ownerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Node nd = !nodeId.HasValue ? null :
                CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);

            bool creatorLevel = nd != null && nd.Creator.UserID.HasValue && nd.isPersonal(nd.Creator.UserID.Value);

            AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                nodeIdOrNodeTypeId: nodeId, node: nd);

            bool hasAccess = accessChecker.checkNodeEditAccess(creatorLevel ? AdminLevel.Creator : AdminLevel.Node, 
                privacyObjectType: PrivacyObjectType.Node, permission: PermissionType.Modify);

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (nodeId.HasValue) _save_error_log(Modules.Log.Action.SetDocumentTreeNodeID_PermissionFailed, nodeId);
                return;
            }

            if (ownerId == Guid.Empty) ownerId = null;

            Node nodeObject = new Node()
            {
                NodeID = nodeId,
                Contributors = contributors,
                OwnerID = ownerId,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            string errorMessage = string.Empty;

            bool result = CNController.set_contributors(paramsContainer.Tenant.Id, nodeObject, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? errorMessage : Messages.OperationFailed.ToString()) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = nodeObject.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetContributors,
                    SubjectID = nodeObject.NodeID,
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        /* end of Service */

        protected void get_template_tags(ref string responseText)
        {
            //Constants
            string TAG_CLASS_CODE = "tag";
            //end of Constants

            Guid? refAppId = RaaiVanSettings.ReferenceTenantID;

            Guid? tagClassId = !refAppId.HasValue ? null : CNController.get_node_type_id(refAppId.Value, TAG_CLASS_CODE);

            List<Node> tags = !tagClassId.HasValue ? new List<Node>() :
                CNController.get_nodes(applicationId: refAppId.Value, nodeTypeIds: new List<Guid>(){ tagClassId.Value}, count: 1000);

            responseText = "{\"Tags\":[" + string.Join(",", tags.Select(t => t.toJson())) + "]}";
        }

        protected void get_templates(Guid? tagId, ref string responseText) {
            //Constants
            string TAG_CLASS_CODE = "tag";
            string TAG_ELEMENT_CODE = "template_tags";
            string DESCRIPTION_ELEMENT_CODE = "about_template";
            //end of Constants

            Guid? refAppId = RaaiVanSettings.ReferenceTenantID;
            Guid? templateFormId = RaaiVanSettings.NodeTypeIdentityFormID;

            if (!refAppId.HasValue || paramsContainer.ApplicationID == refAppId) {
                responseText = "{\"Templates\":[]}";
                return;
            }

            long totalCount = 0;

            List<NodeType> nodeTypes = CNController.get_node_types(refAppId.Value, searchText: null, extensions: null, 
                isKnowledge: null, isDocument: null, archive: false, count: 1000, lowerBoundary: null, totalCount: ref totalCount)
                .Where(nt => !string.IsNullOrEmpty(nt.NodeTypeAdditionalID) &&
                (nt.NodeTypeAdditionalID.ToLower().StartsWith("final_") || nt.NodeTypeAdditionalID.ToLower().EndsWith("_final"))).ToList();

            nodeTypes.Where(nt => nt.ParentID.HasValue && !nodeTypes.Any(x => x.NodeTypeID == nt.ParentID))
                .ToList().ForEach(nt => nt.ParentID = null);

            Guid? tagClassId = CNController.get_node_type_id(refAppId.Value, TAG_CLASS_CODE);

            List<Node> tags = !tagClassId.HasValue ? new List<Node>() :
                CNController.get_nodes(applicationId: refAppId.Value, nodeTypeIds: new List<Guid>() { tagClassId.Value }, count: 1000);

            List<FormType> forms = !templateFormId.HasValue ? new List<FormType>() :
                FGController.get_owner_form_instances(refAppId.Value,
                    nodeTypes.Select(nt => nt.NodeTypeID.Value).ToList(), templateFormId.Value);

            List<FormElement> elements =
                FGController.get_form_instance_elements(refAppId.Value, forms.Select(f => f.InstanceID.Value).ToList());

            Dictionary<Guid, List<Guid>> templateTags = new Dictionary<Guid, List<Guid>>();
            
            elements
                .Where(e => e.Type == FormElementTypes.Node && !string.IsNullOrEmpty(e.Name) &&
                    e.Name.ToLower() == TAG_ELEMENT_CODE.ToLower())
                .ToList()
                .ForEach(e =>
                {
                    Expressions.get_tagged_items(e.TextValue, "Node")
                        .Where(i => i.ID.HasValue && tags.Any(t => t.NodeID == i.ID))
                        .ToList()
                        .ForEach(i =>
                        {
                            NodeType template = forms.Where(f => f.InstanceID == e.FormInstanceID && f.OwnerID.HasValue)
                                .Select(f => nodeTypes.Where(nt => nt.NodeTypeID == f.OwnerID).FirstOrDefault()).FirstOrDefault();

                            Node tag = tags.Where(t => t.NodeID == i.ID.Value).FirstOrDefault();

                            if (template == null || tag == null) return;

                            if (!templateTags.ContainsKey(template.NodeTypeID.Value))
                                templateTags[template.NodeTypeID.Value] = new List<Guid>();

                            if (!templateTags[template.NodeTypeID.Value].Any(k => k == tag.NodeID.Value))
                                templateTags[template.NodeTypeID.Value].Add(tag.NodeID.Value);
                        });
                });

            //Set Description
            nodeTypes.ForEach(nt => {
                FormType inst = forms.Where(f => f.OwnerID.HasValue && f.OwnerID == nt.NodeTypeID).FirstOrDefault();

                FormElement desc = inst == null || !inst.InstanceID.HasValue ? null :
                    elements.Where(e => e.FormInstanceID == inst.InstanceID && e.Type == FormElementTypes.Text &&
                    !string.IsNullOrEmpty(e.Name) && e.Name.ToLower() == DESCRIPTION_ELEMENT_CODE.ToLower()).FirstOrDefault();

                if (desc != null && !string.IsNullOrEmpty(desc.TextValue))
                    nt.Description = desc.TextValue;
            });
            //end of Set Description

            if (tagId.HasValue)
            {
                nodeTypes = nodeTypes.Where(nt => {
                    return templateTags.ContainsKey(nt.NodeTypeID.Value) &&
                        templateTags[nt.NodeTypeID.Value].Any(t => t == tagId.Value);
                }).ToList();

                responseText = "{\"Templates\":[" +
                        string.Join(",", NodeType.toTree(nodeTypes).Select(nt => nt.toJson(refAppId, iconUrl: true))) + "]}";
            }
            else
            {
                responseText = "{\"Templates\":[" +
                        string.Join(",", NodeType.toTree(nodeTypes).Select(nt => nt.toJson(refAppId, iconUrl: true))) + "]" +
                    ",\"Tags\":[" + string.Join(",", tags.Select(t => t.toJson())) + "]" +
                    ",\"TemplateTags\":{" + string.Join(",", templateTags.Keys.Select(
                        k => "\"" + k.ToString() + "\":" +
                            "[" + string.Join(",", templateTags[k].Select(t => "\"" + t.ToString() + "\"")) + "]")) +
                        "}" +
                    "}";
            }
        }

        protected void get_template_json(Guid? nodeTypeId, ref string responseText)
        {
            responseText = "{\"Template\":" + PublicMethods.toJSON_typed<Template>(new Template(nodeTypeId)) + "}";
        }

        protected void get_template_status(string templateContent, ref string responseText)
        {
            if (!PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Template template = PublicMethods.fromJSON_typed<Template>(templateContent);

            responseText = template.get_template_status(paramsContainer.ApplicationID.Value);
        }

        protected void activate_template(string templateContent, ref string responseText) {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value)) {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Template template = PublicMethods.fromJSON_typed<Template>(templateContent);

            bool result = template != null && template.activate(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" : 
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}