using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Xml;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for DocsAPI
    /// </summary>
    public class DocsAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: false);

            if (ProcessTenantIndependentRequest(context)) return;

            if (!paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return;
            }
            
            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "CreateTree":
                    create_tree(PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_guid(context.Request.Params["TemplateTreeID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RenameTree":
                    rename_tree(PublicMethods.parse_guid(context.Request.Params["TreeID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveTree":
                    List<Guid> treeIds = ListMaker.get_guid_items(context.Request.Params["TreeIDs"], '|');
                    Guid? treeId = PublicMethods.parse_guid(context.Request.Params["TreeID"]);

                    if (treeIds.Count == 0 && treeId.HasValue) treeIds.Add(treeId.Value);

                    remove_trees(treeIds, PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RecycleTree":
                    recycle_tree(PublicMethods.parse_guid(context.Request.Params["TreeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetTrees":
                    get_trees(PublicMethods.parse_bool(context.Request.Params["Archive"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "CreateTreeNode":
                    create_new_tree_node(PublicMethods.parse_guid(context.Request.Params["TreeID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_guid(context.Request.Params["ParentID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RenameTreeNode":
                    change_tree_node(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "CopyTreeNodes":
                    copy_tree_nodes(PublicMethods.parse_guid(context.Request.Params["DestinationID"]),
                        ListMaker.get_guid_items(context.Request.Params["CopiedIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "MoveTreeNodes":
                    move_tree_nodes(PublicMethods.parse_guid(context.Request.Params["DestinationID"]),
                        ListMaker.get_guid_items(context.Request.Params["MovedIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ChangeParent":
                    change_parent(ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["ParentID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveTreeNode":
                    remove_tree_nodes(ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["TreeOwnerID"]),
                        PublicMethods.parse_bool(context.Request.Params["RemoveHierarchy"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetTreeNodes":
                    get_tree_nodes(PublicMethods.parse_guid(context.Request.Params["TreeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetTreeNode":
                    get_tree_node(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetChildNodes":
                    get_child_nodes(PublicMethods.parse_guid(context.Request.Params["TreeID"]),
                        PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetParentNodes":
                    get_parent_nodes(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetTreeNodeDocs":
                    get_tree_node_docs(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RenameDoc":
                    rename_doc(PublicMethods.parse_guid(context.Request.Params["DocID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "MoveDocs":
                    move_docs(ListMaker.get_guid_items(context.Request.Params["DocIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveDocs":
                    remove_docs(ListMaker.get_guid_items(context.Request.Params["DocIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "Like":
                case "Unlike":
                    like_unlike(ListMaker.get_guid_items(context.Request.Params["DocIDs"], '|'), (command == "Like"), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPreviousVersions":
                    get_previous_versions(PublicMethods.parse_guid(context.Request.Params["DocID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetTreeNodesOrder":
                    set_tree_nodes_order(ListMaker.get_guid_items(context.Request.Params["TreeNodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddOwnerTree":
                    {
                        add_owner_tree(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                            PublicMethods.parse_guid(context.Request.Params["TreeID"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "RemoveOwnerTree":
                    {
                        remove_owner_tree(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                            PublicMethods.parse_guid(context.Request.Params["TreeID"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "GetOwnerTrees":
                    {
                        get_owner_trees(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "AddTreeNodeContents":
                    {
                        add_tree_node_contents(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]),
                            ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                            PublicMethods.parse_guid(context.Request.Params["RemoveFrom"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "RemoveTreeNodeContents":
                    {
                        remove_tree_node_contents(PublicMethods.parse_guid(context.Request.Params["TreeNodeID"]),
                            ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "XML2JSON":
                    {
                        DocFileInfo file = DocumentUtilities.get_files_info(paramsContainer.ApplicationID, 
                            context.Request.Params["Uploaded"]).FirstOrDefault();
                        xml2json(file, ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "GetPDFCovers":
                    {
                        get_pdf_covers(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "RenameFile":
                    {
                        rename_file(PublicMethods.parse_guid(context.Request.Params["FileID"]),
                            PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        public bool ProcessTenantIndependentRequest(HttpContext context)
        {
            if (!RaaiVanSettings.SAASBasedMultiTenancy && !paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return true;
            }

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "Icon":
                    {
                        Guid? ownerId = PublicMethods.parse_guid(context.Request.Params["OwnerID"]);
                        Guid? iconId = PublicMethods.parse_guid(context.Request.Params["IconID"]);
                        Guid? alternateIconId = PublicMethods.parse_guid(context.Request.Params["AlternateIconID"]);
                        if (!iconId.HasValue || iconId == Guid.Empty) iconId = ownerId;

                        bool? highQuality = PublicMethods.parse_bool(context.Request.Params["HighQuality"]);

                        string type = PublicMethods.parse_string(context.Request.Params["Type"], false);
                        if (!string.IsNullOrEmpty(type)) type = type.ToLower();

                        string iconUrl = string.Empty;
                        string highQualityUrl = string.Empty;

                        switch (type)
                        {
                            case "user":
                            case "profileimage":
                            case "highqualityprofileimage":
                                iconUrl = DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                                    ownerId.Value, false, type == FolderNames.HighQualityProfileImage.ToString().ToLower());

                                if (highQuality.HasValue && highQuality.Value &&
                                    type != FolderNames.HighQualityProfileImage.ToString().ToLower())
                                    highQualityUrl = DocumentUtilities.get_personal_image_address(
                                        paramsContainer.Tenant.Id, ownerId.Value, false, true);
                                break;
                            case "coverphoto":
                            case "highqualitycoverphoto":
                                iconUrl = DocumentUtilities.get_cover_photo_url(paramsContainer.Tenant.Id, ownerId.Value,
                                    false, type == FolderNames.HighQualityCoverPhoto.ToString().ToLower());

                                if (highQuality.HasValue && highQuality.Value &&
                                    type != FolderNames.HighQualityCoverPhoto.ToString().ToLower())
                                    highQualityUrl = DocumentUtilities.get_cover_photo_url(paramsContainer.Tenant.Id,
                                        ownerId.Value, false, true);
                                break;
                            case "icon":
                            case "highqualityicon":
                                iconUrl = iconId.HasValue ? DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                                    iconId.Value, null, type == FolderNames.HighQualityIcon.ToString().ToLower()) : string.Empty;

                                if (highQuality.HasValue && highQuality.Value && type != FolderNames.HighQualityIcon.ToString().ToLower())
                                    highQualityUrl = DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                                        iconId.Value, null, true);
                                break;
                            case "applicationicon":
                            case "highqualityapplicationicon":
                                iconUrl = iconId.HasValue ? DocumentUtilities.get_application_icon_url(iconId.Value,
                                    highQuality: type == FolderNames.HighQualityApplicationIcon.ToString().ToLower()) : string.Empty;

                                if (highQuality.HasValue && highQuality.Value && type != FolderNames.HighQualityApplicationIcon.ToString().ToLower())
                                    highQualityUrl = DocumentUtilities.get_application_icon_url(iconId.Value, highQuality: true);
                                break;
                            default:
                                break;
                        }

                        responseText = "{\"IconURL\":" + (string.IsNullOrEmpty(iconUrl) ? "null" : "\"" + iconUrl + "\"") +
                            ",\"HighQualityIconURL\":" + (string.IsNullOrEmpty(highQualityUrl) ? "null" : "\"" + highQualityUrl + "\"") +
                            "}";

                        break;
                    }

            }

            if (!string.IsNullOrEmpty(responseText))
                paramsContainer.return_response(ref responseText);

            return !string.IsNullOrEmpty(responseText);
        }

        protected void _return_response(ref string responseText)
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
                    ModuleIdentifier = ModuleIdentifier.DCT
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

        protected void create_tree(string title, Guid? templateTreeId, Guid? ownerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            bool hasAccess = false;

            if (!ownerId.HasValue)
                hasAccess = AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID);
            else if (CNController.is_node(paramsContainer.Tenant.Id, ownerId.Value))
            {
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, 
                    paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: ownerId);

                hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
            }
            else if (UsersController.get_user(paramsContainer.Tenant.Id, ownerId.Value) != null)
            {
                hasAccess = paramsContainer.CurrentUserID.Value == ownerId.Value ||
                    PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.CreateDocumentTree_PermissionFailed, Guid.Empty);
                return;
            }

            Tree tree = null;
            bool result = false;

            if (!templateTreeId.HasValue)
            {
                tree = new Tree()
                {
                    TreeID = Guid.NewGuid(),
                    OwnerID = ownerId,
                    IsPrivate = ownerId.HasValue,
                    Name = title,
                    CreatorUserID = paramsContainer.CurrentUserID.Value,
                    CreationDate = DateTime.Now
                };

                result = DocumentsController.create_tree(paramsContainer.Tenant.Id, tree);
            }
            else
            {
                tree = DocumentsController.clone_trees(paramsContainer.Tenant.Id,
                    new List<Guid>() { templateTreeId.Value }, ownerId, true,
                    paramsContainer.CurrentUserID.Value).FirstOrDefault();

                if (tree != null) result = true;
            }

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"TreeID\":\"" + tree.TreeID.ToString() + "\"" + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = tree.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateDocumentTree,
                    SubjectID = tree.TreeID,
                    Info = "{\"Name\":\"" + Base64.encode(tree.Name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void rename_tree(Guid? treeId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                bool hasAccess = false;

                Tree tr = !treeId.HasValue ? null : DocumentsController.get_tree(paramsContainer.Tenant.Id, treeId.Value);

                if (tr != null && tr.IsPrivate.HasValue && tr.IsPrivate.Value && tr.OwnerID.HasValue)
                {
                    AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, 
                        paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: tr.OwnerID);

                    hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
                }

                if (!hasAccess)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    _save_error_log(Modules.Log.Action.ModifyDocumentTree_PermissionFailed, treeId);
                    return;
                }
            }

            Tree tree = new Tree()
            {
                TreeID = treeId,
                Name = title,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = treeId.HasValue && DocumentsController.change_tree(paramsContainer.Tenant.Id, tree);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = tree.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyDocumentTree,
                    SubjectID = tree.TreeID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void remove_trees(List<Guid> treeIds, Guid? ownerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasAccess = false;

            if (!ownerId.HasValue)
                hasAccess = AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID);
            else
            {
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID,
                        paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: ownerId);

                hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveDocumentTree_PermissionFailed, treeIds);
                return;
            }

            bool result = DocumentsController.remove_trees(paramsContainer.Tenant.Id, treeIds, ownerId,
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
                    Action = Modules.Log.Action.RemoveDocumentTree,
                    SubjectIDs = treeIds,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void recycle_tree(Guid? treeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RecycleDocumentTree_PermissionFailed, treeId);
                return;
            }

            bool result = treeId.HasValue && DocumentsController.recycle_tree(paramsContainer.Tenant.Id, treeId.Value,
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
                    Action = Modules.Log.Action.RecycleDocumentTree,
                    SubjectID = treeId,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void get_trees(bool? archive, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (archive.HasValue && archive.Value &&
                !AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<Tree> trees = DocumentsController.get_trees(paramsContainer.Tenant.Id, null, archive);
            if (trees == null) trees = new List<Tree>();

            responseText = "{\"Trees\":[" + string.Join(",", trees.Select(u => u.toJson())) + "]}";
        }

        protected void create_new_tree_node(Guid? treeId, string title, Guid? parentId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.CreateDocumentTreeNode_PermissionFailed, treeId);
                return;
            }

            TreeNode treeNode = new TreeNode()
            {
                TreeNodeID = Guid.NewGuid(),
                TreeID = treeId,
                ParentNodeID = parentId,
                Name = title,
                Description = string.Empty,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now
            };

            bool result = treeId.HasValue && DocumentsController.add_tree_node(paramsContainer.Tenant.Id, treeNode);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"TreeNodeID\":\"" +
                treeNode.TreeNodeID.ToString() + "\"}" : "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = treeNode.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateDocumentTreeNode,
                    SubjectID = treeNode.TreeNodeID,
                    Info = "{\"Name\":\"" + Base64.encode(title) +
                        "\",\"ParentID\":\"" + (!parentId.HasValue || parentId == Guid.Empty ?
                            string.Empty : parentId.ToString()) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void change_tree_node(Guid? treeNodeId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (treeNodeId.HasValue)
                    _save_error_log(Modules.Log.Action.ModifyDocumentTreeNodeName_PermissionFailed, treeNodeId);
                return;
            }

            TreeNode treeNode = new TreeNode()
            {
                TreeNodeID = treeNodeId,
                Name = title,
                Description = string.Empty,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = treeNodeId.HasValue &&
                DocumentsController.change_tree_node(paramsContainer.Tenant.Id, treeNode);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = treeNode.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyDocumentTreeNodeName,
                    SubjectID = treeNode.TreeNodeID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void copy_tree_nodes(Guid? destinationId, List<Guid> copiedIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!destinationId.HasValue || !_has_tree_permission(destinationId.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<Guid> createdIds = new List<Guid>();

            bool result = destinationId.HasValue && copiedIds.Count > 0 &&
                DocumentsController.copy_trees_or_tree_nodes(paramsContainer.Tenant.Id, destinationId.Value,
                    copiedIds, paramsContainer.CurrentUserID.Value).Count > 0;

            List<TreeNode> treeNodes = !result ? new List<TreeNode>() :
                DocumentsController.get_tree_nodes(paramsContainer.Tenant.Id, createdIds);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"TreeNodes\":[" + string.Join(",", treeNodes.Select(u => u.toJson())) + "]" +
                "}";
        }

        protected void move_tree_nodes(Guid? destinationId, List<Guid> movedIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!destinationId.HasValue || movedIds.Count == 0 ||
                !_has_tree_permission(destinationId.Value) || !_has_tree_permission(movedIds[0]))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            string errorMessage = string.Empty;

            List<Guid> rootIds = !destinationId.HasValue || movedIds.Count == 0 ? new List<Guid>() :
                DocumentsController.move_trees_or_tree_nodes(paramsContainer.Tenant.Id, destinationId.Value,
                    movedIds, paramsContainer.CurrentUserID.Value, ref errorMessage);

            bool result = rootIds.Count > 0;

            List<TreeNode> treeNodes = !result ? new List<TreeNode>() :
                DocumentsController.get_tree_nodes(paramsContainer.Tenant.Id, rootIds);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"TreeNodes\":[" + string.Join(",", treeNodes.Select(u => u.toJson())) + "]" + "}" :
                "{\"ErrorText\":\"" +
                (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";
        }

        protected void change_parent(List<Guid> treeNodeIds, Guid? newParentId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.MoveDocumentTreeNode_PermissionFailed, treeNodeIds, newParentId);
                return;
            }

            string errorMessage = string.Empty;

            bool result = DocumentsController.move_tree_node(paramsContainer.Tenant.Id, treeNodeIds, newParentId,
                paramsContainer.CurrentUserID.Value, ref errorMessage);

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
                    Action = Modules.Log.Action.MoveDocumentTreeNode,
                    SubjectIDs = treeNodeIds,
                    Info = "{\"ParentNodeID\":\"" + (!newParentId.HasValue || newParentId == Guid.Empty ?
                        string.Empty : newParentId.ToString()) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void remove_tree_nodes(List<Guid> treeNodeIds,
            Guid? treeOwnerId, bool? removeHierarchy, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasAccess = false;

            if (!treeOwnerId.HasValue)
                hasAccess = AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID);
            else if (CNController.is_node(paramsContainer.Tenant.Id, treeOwnerId.Value))
            {
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID,
                        paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: treeOwnerId);

                hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
            }
            else if (UsersController.get_user(paramsContainer.Tenant.Id, treeOwnerId.Value) != null)
            {
                hasAccess = treeOwnerId.Value == paramsContainer.CurrentUserID.Value ||
                    PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveDocumentTreeNode_PermissionFailed, treeNodeIds);
                return;
            }

            bool result = DocumentsController.remove_tree_node(paramsContainer.Tenant.Id,
                treeNodeIds, treeOwnerId, removeHierarchy, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveDocumentTreeNode,
                    Info = "{\"RemoveHierarchy\":" + (removeHierarchy.HasValue && removeHierarchy.Value).ToString().ToLower() + "}",
                    SubjectIDs = treeNodeIds,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void get_tree_nodes(Guid? treeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<TreeNode> treeNodes = !treeId.HasValue ? new List<TreeNode>() :
                DocumentsController.get_tree_nodes(paramsContainer.Tenant.Id, treeId.Value);

            responseText = "{\"TreeNodes\":[" + string.Join(",", treeNodes.Select(u => u.toJson())) + "]}";
        }

        protected void get_tree_node(Guid? treeNodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            TreeNode treeNode = !treeNodeId.HasValue ? new TreeNode() :
                DocumentsController.get_tree_node(paramsContainer.Tenant.Id, treeNodeId.Value);

            responseText = treeNode.toJson();
        }

        protected void get_child_nodes(Guid? treeId, Guid? treeNodeId, string searchText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<TreeNode> treeNodes = DocumentsController.get_child_nodes(paramsContainer.Tenant.Id,
                treeNodeId, treeId, searchText);

            responseText = "{\"TreeNodes\":[" + string.Join(",", treeNodes.Select(u => u.toJson())) + "]}";
        }

        protected void get_parent_nodes(Guid? treeNodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<Hierarchy> list = !treeNodeId.HasValue ? new List<Hierarchy>() :
                DocumentsController.get_tree_node_hierarchy(paramsContainer.Tenant.Id, treeNodeId.Value);

            responseText = "{\"TreeNodes\":[" +
                ProviderUtil.list_to_string<string>(list.Select(u => u.toJSON()).ToList()) + "]}";
        }

        protected string _get_doc_json(Node _dc, string fileExtension, User creator = null,
            bool? likeStatus = null, bool? editable = null, bool? removable = null)
        {
            string strCreator = creator == null ? "{}" :
                "{\"UserID\":\"" + creator.UserID.Value.ToString() + "\",\"UserName\":\"" + Base64.encode(creator.UserName) +
                "\",\"FirstName\":\"" + Base64.encode(creator.FirstName) + "\",\"LastName\":\"" + Base64.encode(creator.LastName) +
                "\",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                creator.UserID.Value) + "\"}";

            return "{\"ID\":\"" + _dc.NodeID.Value.ToString() + "\"" +
                ",\"NodeTypeID\":\"" + _dc.NodeTypeID.ToString() + "\"" +
                ",\"Title\":\"" + Base64.encode(_dc.Name) + "\"" +
                ",\"Type\":\"" + Base64.encode(_dc.NodeType) + "\"" +
                ",\"IconURL\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id,
                    _dc.NodeID.Value, fileExtension) + "\"" +
                ",\"Link\":\"" + PublicConsts.get_client_url(PublicConsts.NodePage) + "/" + _dc.NodeID.Value.ToString() + "\"" +
                ",\"Removable\":" + (removable.HasValue ? removable.Value : false).ToString().ToLower() +
                ",\"Editable\":" + (editable.HasValue ? editable.Value : false).ToString().ToLower() +
                ",\"LikeStatus\":" + (likeStatus.HasValue ? likeStatus.Value : false).ToString().ToLower() +
                ",\"CreationDate\":\"" + GenericDate.get_local_date(_dc.CreationDate.Value) + "\"" +
                ",\"Creator\":" + strCreator + "}";
        }

        protected void get_tree_node_docs(Guid? treeNodeId, int? count, int? lowerBoundary,
            string searchText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            //bool isPrivate = DocumentsController.is_private_tree(paramsContainer.Tenant.Id, treeNodeId);

            bool isSystemAdmin = paramsContainer.IsAuthenticated &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            long totalCount = 0;

            List<Modules.CoreNetwork.Node> docs = !treeNodeId.HasValue ? new List<Modules.CoreNetwork.Node>() :
                CNController.get_document_tree_node_contents(paramsContainer.Tenant.Id, treeNodeId.Value,
                paramsContainer.CurrentUserID, !isSystemAdmin, count, lowerBoundary, searchText, ref totalCount);

            /*
            List<Modules.CoreNetwork.Node> docs = isPrivate ?
                CNController.get_document_tree_node_contents(paramsContainer.Tenant.Id, 
                    treeNodeId, paramsContainer.CurrentUserID, !isSystemAdmin, count, lowerBoundary, searchText) :
                CNController.get_document_tree_node_items(paramsContainer.Tenant.Id, 
                    treeNodeId, paramsContainer.CurrentUserID, !isSystemAdmin, count, lowerBoundary);
            */

            List<Guid> docIds = docs.Select(u => u.NodeID.Value).ToList();

            List<User> users = UsersController.get_users(paramsContainer.Tenant.Id,
                docs.Where(v => v.Creator.UserID.HasValue).Select(u => u.Creator.UserID.Value).ToList());
            List<Guid> favoriteDocIds = !paramsContainer.IsAuthenticated ? new List<Guid>() :
                CNController.is_fan(paramsContainer.Tenant.Id, docIds, paramsContainer.CurrentUserID.Value);
            List<DocFileInfo> attachments = DocumentsController.get_owner_files(paramsContainer.Tenant.Id, docIds);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Docs\":[";

            bool isFirst = true;
            foreach (Modules.CoreNetwork.Node _dc in docs)
            {
                if (docs.Any(u => u.PreviousVersionID == _dc.NodeID)) continue;

                responseText += (isFirst ? string.Empty : ",") + _get_doc_json(_dc,
                    (attachments.Where(u => u.OwnerID == _dc.NodeID &&
                        !string.IsNullOrEmpty(u.Extension)).Select(v => v.Extension).FirstOrDefault()),
                    (_dc.Creator.UserID.HasValue ? users.Where(u => u.UserID == _dc.Creator.UserID).FirstOrDefault() : null),
                    favoriteDocIds.Exists(u => u == _dc.NodeID), isSystemAdmin, isSystemAdmin);
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void rename_doc(Guid? docId, string title, ref string responseText)
        {
            //Privacy Check: OK
            (new CNAPI() { paramsContainer = this.paramsContainer })
                .modify_node_name(docId, title, false, ref responseText);
        }

        protected void move_docs(List<Guid> docIds, Guid? treeNodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<Guid> newDocIds = new List<Guid>();

            bool hasPermission = PublicMethods.is_system_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);

            if (hasPermission)
                newDocIds = docIds;
            else
            {
                foreach (Guid id in docIds)
                {
                    if (CNController.has_edit_permission(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, id)) newDocIds.Add(id);
                }
            }

            if (newDocIds.Count == 0)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                if (treeNodeId.HasValue)
                    _save_error_log(Modules.Log.Action.MoveDocuments_PermissionFailed, docIds, treeNodeId);
                return;
            }

            bool result = treeNodeId.HasValue && CNController.set_document_tree_node_id(paramsContainer.Tenant.Id,
                newDocIds, treeNodeId, paramsContainer.CurrentUserID.Value);

            string strItems = string.Empty;
            for (int i = 0, lnt = newDocIds.Count; i < lnt; ++i)
                strItems += (i == 0 ? string.Empty : ",") + "\"" + newDocIds[i].ToString() + "\"";

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"MovedItems\":[" + strItems + "]}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.MoveDocuments,
                    SubjectIDs = newDocIds,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void remove_docs(List<Guid> docIds, ref string responseText)
        {
            //Privacy Check: OK
            (new CNAPI() { paramsContainer = this.paramsContainer }).remove_nodes(docIds, false, ref responseText);
        }

        protected void like_unlike(List<Guid> docIds, bool like, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = like ? 
                CNController.like_nodes(paramsContainer.Tenant.Id, docIds, paramsContainer.CurrentUserID.Value) :
                CNController.unlike_nodes(paramsContainer.Tenant.Id, docIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_previous_versions(Guid? docId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<Node> docs = !docId.HasValue ? new List<Node>() :
                CNController.get_previous_versions(paramsContainer.Tenant.Id, docId.Value, paramsContainer.CurrentUserID, true);
            List<User> users = UsersController.get_users(paramsContainer.Tenant.Id,
                docs.Where(v => v.Creator.UserID.HasValue).Select(u => u.Creator.UserID.Value).ToList());

            responseText = "{\"Docs\":[";

            bool isFirst = true;
            foreach (Node _dc in docs)
            {
                responseText += (isFirst ? string.Empty : ",") + _get_doc_json(_dc, string.Empty,
                    (_dc.Creator.UserID.HasValue ? users.Where(u => u.UserID == _dc.Creator.UserID).FirstOrDefault() : null));
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void set_tree_nodes_order(List<Guid> treeNodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SortDocumentTreeNodes_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = DocumentsController.set_tree_nodes_order(paramsContainer.Tenant.Id, treeNodeIds);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void add_owner_tree(Guid? ownerId, Guid? treeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!ownerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (treeId.HasValue) _save_error_log(Modules.Log.Action.AddOwnerTree_PermissionFailed, ownerId, treeId);
                return;
            }

            bool result = treeId.HasValue && DocumentsController.add_owner_tree(paramsContainer.Tenant.Id,
                ownerId.Value, treeId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddOwnerTree,
                    SubjectID = ownerId,
                    SecondSubjectID = treeId,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void remove_owner_tree(Guid? ownerId, Guid? treeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!ownerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (treeId.HasValue) _save_error_log(Modules.Log.Action.RemoveOwnerTree_PermissionFailed, ownerId, treeId);
                return;
            }

            bool result = treeId.HasValue && DocumentsController.remove_owner_tree(paramsContainer.Tenant.Id,
                ownerId.Value, treeId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveOwnerTree,
                    SubjectID = ownerId,
                    SecondSubjectID = treeId,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void get_owner_trees(Guid? ownerId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<Tree> trees = new List<Tree>();

            if (ownerId.HasValue)
            {
                if (CNController.is_node_type(paramsContainer.Tenant.Id, ownerId.Value))
                    trees = DocumentsController.get_owner_trees(paramsContainer.Tenant.Id, ownerId.Value);
                else
                {
                    Modules.CoreNetwork.Node node = CNController.get_node(paramsContainer.Tenant.Id, ownerId.Value);

                    if (node != null)
                    {
                        List<Guid> ids = DocumentsController.get_owner_trees(paramsContainer.Tenant.Id,
                            node.NodeTypeID.Value).Select(u => u.TreeID.Value).ToList();

                        trees = DocumentsController.clone_trees(paramsContainer.Tenant.Id,
                            ids, node.NodeID.Value, false, paramsContainer.CurrentUserID.Value);

                        List<Tree> trs = DocumentsController.get_trees(paramsContainer.Tenant.Id, ownerId.Value);
                        foreach (Tree t in trs)
                            if (!trees.Any(u => u.TreeID == t.TreeID)) trees.Add(t);
                    }
                }
            }

            responseText = "{\"Trees\":[" + string.Join(",", trees.Select(u => u.toJson())) + "]}";
        }

        protected bool _has_tree_permission(Guid treeIdOrTreeNodeId,
            bool nodeAdminLevel = true, bool creatorLevel = false, bool expertOrMemberLevel = false)
        {
            if (!paramsContainer.CurrentUserID.HasValue) return false;

            Guid? ownerId = DocumentsController.get_tree_owner_id(paramsContainer.Tenant.Id, treeIdOrTreeNodeId);

            if (!ownerId.HasValue)
                return AuthorizationManager.has_right(AccessRoleName.ContentsManagement, paramsContainer.CurrentUserID);
            else if (CNController.is_node(paramsContainer.Tenant.Id, ownerId.Value))
            {
                if (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                    return true;

                bool isCreator = false, isContributor = false, isExpert = false, isMember = false,
                    isAdminMember = false, isServiceAdmin = false, isAreaAdmin = false, editable = false;

                CNController.get_user2node_status(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    ownerId.Value, ref isCreator, ref isContributor, ref isExpert, ref isMember, ref isAdminMember,
                    ref isServiceAdmin, ref isAreaAdmin, ref editable);

                bool hasNodeAdminLevel = isServiceAdmin || isAreaAdmin || isAdminMember;
                bool hasCreatorLevel = nodeAdminLevel || isCreator || isContributor;
                bool hasExpertOrMemberLevel = nodeAdminLevel || isExpert || isMember;

                return (nodeAdminLevel && hasNodeAdminLevel) || (creatorLevel && hasCreatorLevel) ||
                    (expertOrMemberLevel && hasExpertOrMemberLevel);
            }
            else if (UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) != null)
            {
                return ownerId == paramsContainer.CurrentUserID ||
                    PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            }

            return false;
        }

        protected void add_tree_node_contents(Guid? treeNodeId,
            List<Guid> nodeIds, Guid? removeFrom, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!treeNodeId.HasValue ||
                !_has_tree_permission(treeNodeId.Value, true, !removeFrom.HasValue, !removeFrom.HasValue))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = DocumentsController.add_tree_node_contents(paramsContainer.Tenant.Id,
                treeNodeId.Value, nodeIds, removeFrom, paramsContainer.CurrentUserID.Value);

            string strDocs = string.Empty;

            if (result)
            {
                List<DocFileInfo> attachments = DocumentsController.get_owner_files(paramsContainer.Tenant.Id, nodeIds);

                strDocs = ",\"Docs\":[" + string.Join(",", nodeIds.Select(u => "{\"ID\":\"" + u.ToString() + "\"" +
                    ",\"IconURL\":\"" + DocumentUtilities.get_icon_url(paramsContainer.Tenant.Id, u,
                        attachments.Where(x => x.OwnerID == u &&
                            !string.IsNullOrEmpty(x.Extension)).Select(v => v.Extension).FirstOrDefault()) + "\"" +
                "}")) + "]";
            }

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + strDocs + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddTreeNodeContents,
                    SubjectIDs = nodeIds,
                    SecondSubjectID = treeNodeId,
                    ThirdSubjectID = removeFrom,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void remove_tree_node_contents(Guid? treeNodeId, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasAccess = false;

            Guid? treeOwnerId = !treeNodeId.HasValue ? null :
                DocumentsController.get_tree_owner_id(paramsContainer.Tenant.Id, treeNodeId.Value);

            if (!treeOwnerId.HasValue)
            {
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID,
                        paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: nodeIds[0]);

                hasAccess = treeNodeId.HasValue && nodeIds.Count == 1 &&
                    accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
            }
            else if (CNController.is_node(paramsContainer.Tenant.Id, treeOwnerId.Value))
            {
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID,
                        paramsContainer.CurrentUserID, nodeIdOrNodeTypeId: treeOwnerId);

                hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Node, permission: PermissionType.None);
            }
            else if (UsersController.get_user(paramsContainer.Tenant.Id, treeOwnerId.Value) != null)
            {
                hasAccess = treeOwnerId.Value == paramsContainer.CurrentUserID.Value ||
                    PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (treeNodeId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveTreeNodeContents_PermissionFailed, nodeIds, treeNodeId);
                return;
            }

            bool result = DocumentsController.remove_tree_node_contents(paramsContainer.Tenant.Id,
                treeNodeId.Value, nodeIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveTreeNodeContents,
                    SubjectIDs = nodeIds,
                    SecondSubjectID = treeNodeId,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log
        }

        protected void xml2json(DocFileInfo file, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            if (file != null) file.FolderName = FolderNames.TemporaryFiles;

            if (file == null || !file.FileID.HasValue || string.IsNullOrEmpty(file.Extension) ||
                file.Extension.ToLower() != "xml" || !file.exists())
            {
                responseText = "{}";
                return;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                using (MemoryStream stream = new MemoryStream(file.toByteArray()))
                    doc.Load(stream);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    "DCT_XML2JSON_LoadFile", ex, ModuleIdentifier.DCT);
                responseText = "{}";
                return;
            }

            string errorMessage = string.Empty;

            PublicMethods.shorten_xml_texts(doc.DocumentElement, 200);
            PublicMethods.remove_xml_attributes(doc.DocumentElement);

            responseText = PublicMethods.xml2json(doc, true, ref errorMessage);
            responseText = "{\"Converted\":" + (string.IsNullOrEmpty(responseText) ? "{}" : responseText) + "}";

            if (!string.IsNullOrEmpty(errorMessage)) responseText = "{\"ErrorMessage\":\"" + errorMessage + "\"}";
        }

        protected void get_pdf_covers(Guid? ownerId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<DocFileInfo> files = !ownerId.HasValue ? new List<DocFileInfo>() :
                DocumentsController.get_owner_files(paramsContainer.Tenant.Id, ownerId.Value, FileOwnerTypes.PDFCover);

            responseText = "{\"Files\":[" + string.Join(",", files.Select(f => f.toJson())) + "]}";
        }

        protected void rename_file(Guid? fileId, string name, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            //Only PDFCover files of NodeTypes can be edited
            DocFileInfo file = !fileId.HasValue ? null : DocumentsController.get_file(paramsContainer.Tenant.Id, fileId.Value);

            if (file == null || !file.OwnerID.HasValue || file.OwnerType != FileOwnerTypes.PDFCover ||
                !CNController.is_node_type(paramsContainer.Tenant.Id, file.OwnerID.Value) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

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

            bool result = DocumentsController.rename_file(paramsContainer.Tenant.Id, fileId.Value, name);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        //Jobs
        public void remove_temporary_files(Guid applicationId)
        {
            List<string> files = new List<string>();
            string tempDir = string.Empty;

            try
            {
                tempDir = DocFileInfo.temporary_folder_address(applicationId);

                if (Directory.Exists(tempDir) && !RaaiVanSettings.CephStorage.Enabled)
                    files = Directory.GetFiles(tempDir).Take(100).ToList();

                tempDir = PublicMethods.map_path(PublicConsts.TempDirectory);
                if (Directory.Exists(tempDir)) files.AddRange(Directory.GetFiles(tempDir).Take(100).ToList());
            }
            catch { files = new List<string>(); }

            foreach (string f in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(f);
                    if ((fi.CreationTimeUtc.Ticks - DateTime.UtcNow.AddHours(-6).Ticks) < 0) fi.Delete();
                }
                catch { }
            }

            if (RaaiVanSettings.CephStorage.Enabled) {
                CephStorage.files(DocFileInfo.temporary_folder_address(applicationId))
                    .Where(f => f.Value.Ticks - DateTime.UtcNow.AddHours(-6).Ticks < 0).Take(100).Select(f => f.Key).ToList()
                    .ForEach(f => CephStorage.delete_file(f));
            }
        }

        public void start_remove_temporary_files(object rvThread)
        {
            RVJob trd = (RVJob)rvThread;

            if (!trd.TenantID.HasValue) return;

            if (!trd.StartTime.HasValue) trd.StartTime = new DateTime(2000, 1, 1, 0, 0, 0);
            if (!trd.EndTime.HasValue) trd.EndTime = new DateTime(2000, 1, 1, 23, 59, 59);

            while (true)
            {
                if (!trd.Interval.HasValue) trd.Interval = 21600000; //21600000 Miliseconds Equals to 6 Hours
                else System.Threading.Thread.Sleep(trd.Interval.Value);

                if (!trd.check_time()) continue;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();

                remove_temporary_files(trd.TenantID.Value);

                trd.LastActivityDate = DateTime.Now;

                sw.Stop();
                trd.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }
        //end of Jobs

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}