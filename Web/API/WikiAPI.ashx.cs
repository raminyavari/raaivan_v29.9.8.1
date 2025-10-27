using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.Wiki;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for WikiAPI
    /// </summary>
    public class WikiAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            WikiOwnerType ownerType = WikiOwnerType.Node;
            if (!Enum.TryParse<WikiOwnerType>(context.Request["OwnerType"], out ownerType)) ownerType = WikiOwnerType.Node;

            switch (command)
            {
                case "GetWiki":
                    get_wiki(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["Removed"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddTitle":
                    add_title(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_int(context.Request.Params["SequenceNumber"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyTitle":
                    modify_title(PublicMethods.parse_guid(context.Request.Params["TitleID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveTitle":
                    remove_title(PublicMethods.parse_guid(context.Request.Params["TitleID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RecycleTitle":
                    recycle_title(PublicMethods.parse_guid(context.Request.Params["TitleID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetTitlesOrder":
                    set_titles_order(ListMaker.get_guid_items(context.Request.Params["TitleIDs"], '|'), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddParagraph":
                    add_paragraph(PublicMethods.parse_guid(context.Request.Params["TitleID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["BodyText"]),
                        PublicMethods.parse_bool(context.Request.Params["IsRichText"], true),
                        PublicMethods.parse_int(context.Request.Params["SequenceNumber"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyParagraph":
                    modify_paragraph(PublicMethods.parse_guid(context.Request.Params["ParagraphID"]),
                        PublicMethods.parse_guid(context.Request.Params["ChangeID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["BodyText"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveParagraph":
                    remove_paragraph(PublicMethods.parse_guid(context.Request.Params["ParagraphID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RecycleParagraph":
                    recycle_paragraph(PublicMethods.parse_guid(context.Request.Params["ParagraphID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetParagraphsOrder":
                    set_paragraphs_order(ListMaker.get_guid_items(context.Request.Params["ParagraphIDs"], '|'), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetParagraphs":
                    get_paragraphs(PublicMethods.parse_guid(context.Request.Params["TitleID"]),
                        PublicMethods.parse_bool(context.Request.Params["Removed"]), ownerType, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetParagraphRelatedUsers":
                    get_paragraph_related_users(PublicMethods.parse_guid(context.Request.Params["ParagraphID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AcceptChange":
                case "RejectChange":
                    bool accept = command == "AcceptChange" ? true : false;

                    accept_reject_wiki_change(PublicMethods.parse_guid(context.Request.Params["ChangeID"]),
                        accept, ownerType, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveChange":
                    remove_wiki_change(PublicMethods.parse_guid(context.Request.Params["ChangeID"]), ownerType, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetParagraphChanges":
                    get_paragraph_changes(PublicMethods.parse_guid(context.Request.Params["ParagraphID"]), ownerType,
                        PublicMethods.parse_bool(context.Request.Params["CheckWorkFlowEditPermission"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ExportAsPDF":
                    if (!export_as_pdf(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ownerType,
                        PublicMethods.parse_guid(context.Request.Params["CoverID"]),
                        PublicMethods.parse_string(context.Request.Params["PS"]), ref responseText))
                        _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void _save_error_log(Modules.Log.Action action, Guid subjectId)
        {
            try
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = action,
                    SubjectID = subjectId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            catch { }
        }

        protected string _get_change_json(Change _change, bool isAdmin)
        {
            if (_change == null) return "{\"Empty\":true}";

            return "{\"ChangeID\":\"" + _change.ChangeID.Value.ToString() + "\"" +
                ",\"ParagraphID\":\"" + _change.ParagraphID.Value.ToString() + "\"" +
                ",\"Sender\":{\"UserID\":\"" + _change.Sender.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(_change.Sender.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(_change.Sender.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(_change.Sender.UserName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        _change.Sender.UserID.Value) + "\"" +
                "}" +
                ",\"Title\":\"" + Base64.encode(_change.Title) + "\"" +
                ",\"BodyText\":\"" + Base64.encode(_change.BodyText) + "\"" +
                ",\"CreationDate\":\"" + GenericDate.get_local_date(_change.SendDate.Value) + "\"" +
                ",\"IsExpert\":" + isAdmin.ToString().ToLower() +
                ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(_change.AttachedFiles, true) +
                "}";
        }

        protected string _get_paragraph_json(Paragraph paragraph, bool isAdmin, bool editSuggestion, bool removed = false, bool titleRemoved = false)
        {
            string changesJson = string.Empty;
            if (paragraph.Changes != null && !removed)
            {
                foreach (Change _change in paragraph.Changes)
                    changesJson += (string.IsNullOrEmpty(changesJson) ? string.Empty : ",") + _get_change_json(_change, isAdmin);
            }

            bool isRichText = paragraph.IsRichText.HasValue && paragraph.IsRichText.Value;
            bool citationNeeded = paragraph.Status == WikiStatuses.CitationNeeded.ToString();
            bool removable = paramsContainer.IsAuthenticated &&
                (isAdmin || (citationNeeded && paramsContainer.CurrentUserID.Value == paragraph.CreatorUserID));
            bool movable = removable;
            bool editMode = paragraph.Status == WikiStatuses.Pending.ToString() && isAdmin;
            string lastModificationDate = GenericDate.get_local_date(paragraph.LastModificationDate.HasValue ?
                paragraph.LastModificationDate.Value : paragraph.CreationDate.Value);

            return "{\"ParagraphID\":\"" + paragraph.ParagraphID.Value.ToString() + "\"" +
                ",\"Title\":\"" + Base64.encode(paragraph.Title) + "\"" +
                ",\"TitleID\":\"" + paragraph.TitleID.ToString() + "\"" +
                ",\"BodyText\":\"" + Base64.encode(paragraph.BodyText) + "\"" +
                ",\"CitationNeeded\":" + citationNeeded.ToString().ToLower() +
                ",\"Editable\":" + (!titleRemoved && !removed && (isAdmin || editSuggestion)).ToString().ToLower() +
                ",\"Removable\":" + (!titleRemoved && !removed && removable).ToString().ToLower() +
                ",\"Removed\":" + removed.ToString().ToLower() +
                ",\"Movable\":" + (!titleRemoved && movable).ToString().ToLower() +
                ",\"EditMode\":" + (!titleRemoved && editMode).ToString().ToLower() +
                ",\"IsRichText\":" + isRichText.ToString().ToLower() +
                ",\"LastModificationDate\":\"" + lastModificationDate + "\"" +
                ",\"AppliedChangesCount\":" + (!paragraph.AppliedChangesCount.HasValue ? 0 :
                    paragraph.AppliedChangesCount.Value).ToString() +
                ",\"Changes\":[" + changesJson + "]" +
                ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(paragraph.AttachedFiles, true) + "}";
        }

        protected string _get_title_json(WikiTitle wikiTitle, bool isAdmin, bool editSuggestion, bool removed = false)
        {
            string status = wikiTitle.Status;
            bool editable = paramsContainer.IsAuthenticated &&
                (isAdmin || (editSuggestion && wikiTitle.CreatorUserID == paramsContainer.CurrentUserID.Value));
            bool removable = paramsContainer.IsAuthenticated && (
                isAdmin || (wikiTitle.CreatorUserID == paramsContainer.CurrentUserID.Value && (
                    wikiTitle.Paragraphs == null ||
                    wikiTitle.Paragraphs.Where(u => u.Status == "Accepted" || u.Status == "CitationNeeded").Count() == 0
                )));

            string paragraphsJson = string.Empty;
            if (wikiTitle.Paragraphs != null)
            {
                bool isFirst = true;
                foreach (Paragraph _paragraph in wikiTitle.Paragraphs)
                {
                    if (_paragraph.Status == WikiStatuses.Pending.ToString() && (_paragraph.Changes == null || _paragraph.Changes.Count == 0)) continue;

                    paragraphsJson += (isFirst ? string.Empty : ",") +
                        _get_paragraph_json(_paragraph, isAdmin, editSuggestion, false, removed);
                    isFirst = false;
                }
            }

            return "{\"TitleID\":\"" + wikiTitle.TitleID.Value.ToString() + "\"" +
                ",\"Title\":\"" + Base64.encode(wikiTitle.Title) + "\"" +
                ",\"OwnerID\":\"" + wikiTitle.OwnerID.Value.ToString() + "\"" +
                ",\"Editable\":" + (!removed && editable).ToString().ToLower() +
                ",\"Removable\":" + (!removed && removable).ToString().ToLower() +
                ",\"Removed\":" + removed.ToString().ToLower() +
                ",\"RemovedParagraphsCount\":" + (!wikiTitle.RemovedParagraphsCount.HasValue ? 0 :
                    wikiTitle.RemovedParagraphsCount.Value).ToString() +
                ",\"Paragraphs\":[" + paragraphsJson + "]}";
        }

        protected bool _check_workflow_edit_permission(Guid? nodeId)
        {
            bool hasKnowledgePermission = false, hasWorkFlowPermission = false,
                hasWFEditPermission = false, hideContributors = false;

            (new CNAPI() { paramsContainer = this.paramsContainer })
                .check_node_workflow_permissions(new Node() { NodeID = nodeId },
                null, false, false, false, false, ref hasKnowledgePermission, ref hasWorkFlowPermission,
                ref hasWFEditPermission, ref hideContributors);

            return hasWFEditPermission;
        }

        protected void get_wiki(Guid? ownerId, WikiOwnerType ownerType, bool? removed, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!ownerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            //bool isExpert = CNController.is_expert(currentUserId, ownerId, true, true);
            bool editSuggestion = paramsContainer.IsAuthenticated && !(ownerType == WikiOwnerType.User);

            bool isAdmin = paramsContainer.IsAuthenticated &&
                (ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                    ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, ownerId.Value, true, ref editSuggestion));

            //Check Access to Wiki
            bool accessDenied = false;

            if (ownerType == WikiOwnerType.User &&
                UsersController.get_user(paramsContainer.Tenant.Id, ownerId.Value) == null) accessDenied = true;
            else if (ownerType != WikiOwnerType.User && !isAdmin &&
                !PrivacyController.check_access(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID, ownerId.Value, PrivacyObjectType.Node, PermissionType.View)) accessDenied = true;

            if (!removed.HasValue) removed = false;
            if (accessDenied || (removed.Value && !isAdmin))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.Wiki_AccessDenied, ownerId.Value);
                return;
            }
            //end of Check Access to Wiki

            List<WikiTitle> titles = WikiController.get_titles(paramsContainer.Tenant.Id,
                ownerId.Value, isAdmin, paramsContainer.CurrentUserID, removed.Value);

            List<Guid> titleIds = titles.Select(u => u.TitleID.Value).ToList();

            List<Paragraph> paragraphs = WikiController.get_title_paragraphs(paramsContainer.Tenant.Id,
                titleIds, isAdmin, paramsContainer.CurrentUserID, false);

            List<Guid> paragraphIds = paragraphs.Select(v => v.ParagraphID.Value).ToList();
            Guid? creatorUserId = null;
            if (!isAdmin) creatorUserId = paramsContainer.CurrentUserID;
            List<Change> changes = !paramsContainer.IsAuthenticated ? new List<Change>() :
                WikiController.get_changes(paramsContainer.Tenant.Id,
                    paragraphIds, creatorUserId, WikiStatuses.Pending, false);

            paragraphIds.AddRange(changes.Select(v => v.ChangeID.Value).ToList());
            List<DocFileInfo> attachedFiles = DocumentsController.get_owner_files(paramsContainer.Tenant.Id, paragraphIds);

            Dictionary<Guid, int> removedParagraphsCount = WikiController.get_paragraphs_count(paramsContainer.Tenant.Id,
                titleIds, isAdmin, paramsContainer.CurrentUserID, true);
            Dictionary<Guid, int> appliedChangesCount =
                WikiController.get_changes_count(paramsContainer.Tenant.Id, paragraphIds, true);

            foreach (Change _change in changes) _change.AttachedFiles =
                attachedFiles.Where(u => u.OwnerID == _change.ChangeID).OrderBy(v => v.FileName).ToList();

            foreach (Paragraph _paragraph in paragraphs)
            {
                _paragraph.Changes = changes.Where(u => u.ParagraphID == _paragraph.ParagraphID).OrderBy(v => v.SendDate).ToList();
                _paragraph.AttachedFiles = attachedFiles.Where(u => u.OwnerID == _paragraph.ParagraphID).OrderBy(v => v.FileName).ToList();
                _paragraph.AppliedChangesCount = !appliedChangesCount.ContainsKey(_paragraph.ParagraphID.Value) ? 0 :
                    appliedChangesCount[_paragraph.ParagraphID.Value];
            }

            List<DocFileInfo> wikiFiles = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                ownerId.Value, FileOwnerTypes.WikiContent);

            responseText = "{\"IsAdmin\":" + isAdmin.ToString().ToLower() +
                ",\"Editable\":" + (isAdmin || editSuggestion).ToString().ToLower() +
                ",\"RemovedTitlesCount\":" + WikiController.get_titles_count(paramsContainer.Tenant.Id,
                    ownerId.Value, isAdmin, paramsContainer.CurrentUserID, true).ToString() +
                ",\"Titles\":[";

            bool isFirst = true;
            foreach (WikiTitle _wikiTitle in titles)
            {
                _wikiTitle.Paragraphs = paragraphs.Where(u => u.TitleID == _wikiTitle.TitleID).OrderBy(v => v.SequenceNumber).ToList();
                _wikiTitle.RemovedParagraphsCount = !removedParagraphsCount.ContainsKey(_wikiTitle.TitleID.Value) ? 0 :
                    removedParagraphsCount[_wikiTitle.TitleID.Value];

                //if (!isAdmin && _wikiTitle.Status != "Accepted" && (_wikiTitle.Paragraphs == null || _wikiTitle.Paragraphs.Count == 0)) continue;

                responseText += (isFirst ? string.Empty : ",") + _get_title_json(_wikiTitle, isAdmin, editSuggestion, removed.Value);
                isFirst = false;
            }

            responseText += "]" + ",\"Files\":" + DocumentUtilities.get_files_json(wikiFiles) + "}";
        }

        protected void add_title(Guid? ownerId, string title, int? sequenceNumber,
            WikiOwnerType ownerType, bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 490)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!ownerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            //bool isExpert = CNController.is_expert(currentUserId, ownerId, true, true);
            bool editSuggestion = !(ownerType == WikiOwnerType.User);
            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                    ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, ownerId.Value, true, ref editSuggestion);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            //Check Access to Wiki
            if (!isAdmin)
            {
                if ((ownerType == WikiOwnerType.User &&
                        UsersController.get_user(paramsContainer.Tenant.Id, ownerId.Value) == null) ||
                    (ownerType != WikiOwnerType.User &&
                    !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        ownerId.Value, PrivacyObjectType.Node, PermissionType.View)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    _save_error_log(Modules.Log.Action.AddWikiTitle_PermissionFailed, ownerId.Value);
                    return;
                }
            }
            //end of Check Access to Wiki

            WikiTitle newTitle = new WikiTitle()
            {
                TitleID = Guid.NewGuid(),
                OwnerID = ownerId,
                Title = title,
                SequenceNumber = sequenceNumber,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now,
                OwnerType = WikiOwnerType.Node.ToString()
            };

            bool result = WikiController.add_title(paramsContainer.Tenant.Id, newTitle, isAdmin);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                "\",\"Title\":" + _get_title_json(newTitle, isAdmin, editSuggestion) + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = newTitle.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddWikiTitle,
                    SubjectID = newTitle.TitleID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void modify_title(Guid? titleId, string title, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 490)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!titleId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleId.Value);

            //bool isExpert = CNController.is_expert(currentUserId, ownerId, true, true);
            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                    ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            //Check Access to Wiki
            if (!isAdmin)
            {
                if ((ownerType == WikiOwnerType.User &&
                    UsersController.get_user(paramsContainer.Tenant.Id, ownerId) == null) ||
                    (ownerType != WikiOwnerType.User &&
                    !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        ownerId, PrivacyObjectType.Node, PermissionType.View)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    _save_error_log(Modules.Log.Action.ModifyWikiTitle_PermissionFailed, titleId.Value);
                    return;
                }
            }
            //end of Check Access to Wiki


            WikiTitle wikiTitle = new WikiTitle()
            {
                TitleID = titleId,
                Title = title,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = WikiController.modify_title(paramsContainer.Tenant.Id, wikiTitle, isAdmin);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = wikiTitle.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyWikiTitle,
                    SubjectID = wikiTitle.TitleID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void set_titles_order(List<Guid> titleIds, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid ownerId = titleIds.Count == 0 ? Guid.Empty :
                WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleIds[0]);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                    ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetWikiTitlesOrder_PermissionFailed, ownerId);
                return;
            }

            bool result = WikiController.set_titles_order(paramsContainer.Tenant.Id, titleIds);

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
                    Action = Modules.Log.Action.SetWikiTitlesOrder,
                    SubjectIDs = titleIds,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void remove_title(Guid? titleId, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!titleId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleId.Value);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            WikiTitle title = WikiController.get_title(paramsContainer.Tenant.Id,
                titleId.Value, paramsContainer.CurrentUserID.Value);

            bool removable = isAdmin || (
                (title != null && title.CreatorUserID == paramsContainer.CurrentUserID) &&
                (WikiController.get_paragraphs_count(paramsContainer.Tenant.Id, titleId.Value, false, null, false) == 0)
            );

            if (!removable)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveWikiTitle, titleId.Value);
                return;
            }

            bool result = WikiController.remove_title(paramsContainer.Tenant.Id,
                titleId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveWikiTitle,
                    SubjectID = titleId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void recycle_title(Guid? titleId, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!titleId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleId.Value);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RecycleWikiTitle_PermissionFailed, titleId.Value);
                return;
            }

            bool result = !isAdmin ? false : WikiController.recycle_title(paramsContainer.Tenant.Id,
                titleId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RecycleWikiTitle,
                    SubjectID = titleId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void add_paragraph(Guid? titleId, string title, string bodyText, bool? isRichText,
            int? sequenceNumber, WikiOwnerType ownerType, bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 490)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!titleId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleId.Value);

            bool isNode = !(ownerType == WikiOwnerType.User);
            bool editSuggestion = isNode;

            List<Guid> adminIds = ownerType == WikiOwnerType.User ? new List<Guid>() :
                CNController.get_users_with_edit_permission(paramsContainer.Tenant.Id, ownerId, true, ref editSuggestion);
            if (ownerType == WikiOwnerType.User) adminIds.Add(paramsContainer.CurrentUserID.Value);

            bool isAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                CNController.is_service_admin(paramsContainer.Tenant.Id, ownerId, paramsContainer.CurrentUserID.Value) ||
                adminIds.Any(u => u == paramsContainer.CurrentUserID);
            bool hasAdmin = adminIds != null && adminIds.Count > 0;

            if (!isAdmin && isNode && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin && isNode) isAdmin = PrivacyController.check_access(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, ownerId, PrivacyObjectType.Node, PermissionType.Modify);

            //Check Access to Wiki
            if (!isAdmin)
            {
                if ((ownerType == WikiOwnerType.User &&
                    UsersController.get_user(paramsContainer.Tenant.Id, ownerId) == null) ||
                    (ownerType != WikiOwnerType.User &&
                    !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        ownerId, PrivacyObjectType.Node, PermissionType.View)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    _save_error_log(Modules.Log.Action.AddWikiParagraph_PermissionFailed, titleId.Value);
                    return;
                }
            }
            //end of Check Access to Wiki

            Paragraph newParagraph = new Paragraph()
            {
                ParagraphID = Guid.NewGuid(),
                TitleID = titleId,
                Title = title,
                BodyText = bodyText,
                IsRichText = isRichText,
                SequenceNumber = sequenceNumber,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now
            };

            List<Dashboard> sentDashboards = new List<Dashboard>();

            bool result = WikiController.add_paragraph(paramsContainer.Tenant.Id,
                newParagraph, !isAdmin, hasAdmin, adminIds, ref sentDashboards);

            if (result && !isAdmin && hasAdmin)
            {
                Change change = WikiController.get_last_pending_change(paramsContainer.Tenant.Id,
                    newParagraph.ParagraphID.Value, paramsContainer.CurrentUserID.Value);
                newParagraph.Changes.Add(change);
            }

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                "\",\"Paragraph\":" + _get_paragraph_json(newParagraph, isAdmin, editSuggestion) + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                List<InlineTag> tags = Expressions.get_tagged_items(newParagraph.BodyText);

                foreach (InlineTag tg in tags)
                {
                    TaggedType tgTp = TaggedType.None;
                    if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                    tagged.Add(new TaggedItem(newParagraph.ParagraphID.Value,
                        tg.ID.Value, TagContextType.WikiChange, tgTp));
                }

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, false, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            //Send Notification
            if (result && sentDashboards.Count > 0)
                NotificationController.transfer_dashboards(paramsContainer.Tenant.Id, sentDashboards);

            if (result && (!hasAdmin || isAdmin))
            {
                Notification not = new Notification()
                {
                    SubjectID = newParagraph.ParagraphID,
                    RefItemID = ownerId,
                    SubjectType = SubjectType.Wiki,
                    Action = Modules.NotificationCenter.ActionType.Modify,
                    Description = bodyText
                };
                not.Sender.UserID = paramsContainer.CurrentUserID.Value;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = newParagraph.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddWikiParagraph,
                    SubjectID = newParagraph.ParagraphID,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\",\"Description\":\"" + Base64.encode(bodyText) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void modify_paragraph(Guid? paragraphId, Guid? changeId, string title, string bodyText,
            WikiOwnerType ownerType, bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 490)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!paragraphId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (changeId.HasValue && changeId != Guid.Empty)
            {
                Change chng = WikiController.get_change(paramsContainer.Tenant.Id, changeId.Value);
                title = chng.Title;
                bodyText = chng.BodyText;
            }

            if (!string.IsNullOrEmpty(title)) title = title.Trim();
            if (!string.IsNullOrEmpty(bodyText)) bodyText = bodyText.Trim();

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(bodyText))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.ModifyWikiParagraph_PermissionFailed, paragraphId.Value);
                return;
            }

            //Check if no change
            Paragraph oldValue = WikiController.get_paragraph(paramsContainer.Tenant.Id,
                paragraphId.Value, paramsContainer.CurrentUserID.Value);

            if (!string.IsNullOrEmpty(oldValue.Title)) oldValue.Title = oldValue.Title.Trim();
            if (!string.IsNullOrEmpty(oldValue.BodyText)) oldValue.BodyText = oldValue.BodyText.Trim();

            if (((string.IsNullOrEmpty(oldValue.BodyText) && string.IsNullOrEmpty(bodyText)) || (oldValue.BodyText == bodyText)) &&
                ((string.IsNullOrEmpty(title) && string.IsNullOrEmpty(oldValue.Title)) || (title == oldValue.Title)) &&
                oldValue.Status == "Accepted")
            {
                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
                return;
            }
            //end of Check if no change

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, paragraphId.Value);

            bool isNode = !(ownerType == WikiOwnerType.User);

            List<Guid> adminIds = ownerType == WikiOwnerType.User ? new List<Guid>() :
                CNController.get_users_with_edit_permission(paramsContainer.Tenant.Id, ownerId, true);
            if (ownerType == WikiOwnerType.User) adminIds.Add(paramsContainer.CurrentUserID.Value);

            bool isAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                CNController.is_service_admin(paramsContainer.Tenant.Id, ownerId, paramsContainer.CurrentUserID.Value) ||
                adminIds.Any(u => u == paramsContainer.CurrentUserID);
            bool hasAdmin = adminIds != null && adminIds.Count > 0;

            if (!isAdmin && isNode && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin && isNode) isAdmin = PrivacyController.check_access(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, ownerId, PrivacyObjectType.Node, PermissionType.Modify);

            //Check Access to Wiki
            if (!isAdmin)
            {
                if ((ownerType == WikiOwnerType.User &&
                    UsersController.get_user(paramsContainer.Tenant.Id, ownerId) == null) ||
                    (ownerType != WikiOwnerType.User &&
                    !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        ownerId, PrivacyObjectType.Node, PermissionType.View)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }
            //end of Check Access to Wiki

            Paragraph paragraph = new Paragraph()
            {
                ParagraphID = paragraphId,
                Title = title,
                BodyText = bodyText,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool citationNeeded = !isAdmin && !hasAdmin ? true : false;

            List<Dashboard> sentDashboards = new List<Dashboard>();

            bool result = WikiController.modify_paragraph(paramsContainer.Tenant.Id, paragraph, changeId, hasAdmin,
                adminIds, ref sentDashboards, citationNeeded, isAdmin || citationNeeded);

            string changeJson = "\"";
            Change change = new Change();
            if (result && !isAdmin && hasAdmin)
            {
                change = WikiController.get_last_pending_change(paramsContainer.Tenant.Id,
                    paragraphId.Value, paramsContainer.CurrentUserID.Value);
                change.AttachedFiles = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                    change.ChangeID.HasValue ? change.ChangeID.Value : Guid.Empty);
                changeJson += ",\"Change\":" + _get_change_json(change, isAdmin);
            }

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + changeJson +
                ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(paragraph.AttachedFiles, icon: true) + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                List<InlineTag> tags = Expressions.get_tagged_items(paragraph.BodyText);

                foreach (InlineTag tg in tags)
                {
                    TaggedType tgTp = TaggedType.None;
                    if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                    tagged.Add(new TaggedItem(paragraphId.Value, tg.ID.Value, TagContextType.WikiChange, tgTp));
                }

                if (tags.Count == 0)
                    tagged.Add(new TaggedItem() { ContextID = paragraphId });

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, true, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            //Send Notification
            if (result && sentDashboards.Count > 0)
                NotificationController.transfer_dashboards(paramsContainer.Tenant.Id, sentDashboards);

            if (result && (!hasAdmin || isAdmin))
            {
                Notification not = new Notification()
                {
                    SubjectID = paragraphId,
                    RefItemID = ownerId,
                    SubjectType = SubjectType.Wiki,
                    Action = Modules.NotificationCenter.ActionType.Modify,
                    Description = bodyText
                };
                not.Sender.UserID = paramsContainer.CurrentUserID.Value;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = paragraph.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = !isAdmin && hasAdmin ? Modules.Log.Action.SuggestWikiParagraphChange : Modules.Log.Action.AddWikiParagraph,
                    SubjectID = !isAdmin && hasAdmin ? change.ChangeID : paragraphId,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\",\"Description\":\"" + Base64.encode(bodyText) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void remove_paragraph(Guid? paragraphId, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paragraphId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, paragraphId.Value);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveWikiParagraph_PermissionFailed, paragraphId.Value);
                return;
            }

            bool result = WikiController.remove_paragraph(paramsContainer.Tenant.Id,
                paragraphId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveWikiParagraph,
                    SubjectID = paragraphId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void recycle_paragraph(Guid? paragraphId, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paragraphId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, paragraphId.Value);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RecycleWikiParagraph_PermissionFailed, paragraphId.Value);
                return;
            }

            bool result = WikiController.recycle_paragraph(paramsContainer.Tenant.Id,
                paragraphId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RecycleWikiParagraph,
                    SubjectID = paragraphId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void set_paragraphs_order(List<Guid> paragraphIds, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid ownerId = paragraphIds.Count == 0 ? Guid.Empty :
                WikiController.get_wiki_owner(paramsContainer.Tenant.Id, paragraphIds[0]);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            if (!isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RecycleWikiParagraph_PermissionFailed, ownerId);
                return;
            }

            bool result = WikiController.set_paragraphs_order(paramsContainer.Tenant.Id, paragraphIds);

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
                    Action = Modules.Log.Action.SetWikiParagraphsOrder,
                    SubjectIDs = paragraphIds,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void get_paragraphs(Guid? titleId, bool? removed, WikiOwnerType ownerType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!titleId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, titleId.Value);

            bool editSuggestion = paramsContainer.IsAuthenticated && !(ownerType == WikiOwnerType.User);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);

            //Check Access to Wiki
            bool accessDenied = false;

            if (ownerType == WikiOwnerType.User && UsersController.get_user(paramsContainer.Tenant.Id, ownerId) == null)
                accessDenied = true;
            else if (ownerType != WikiOwnerType.User && !isAdmin &&
                !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    ownerId, PrivacyObjectType.Node, PermissionType.View)) accessDenied = true;

            if (!removed.HasValue) removed = false;
            if (accessDenied || (removed.Value && !isAdmin))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.Wiki_AccessDenied, ownerId);
                return;
            }
            //end of Check Access to Wiki

            List<Paragraph> paragraphs = WikiController.get_title_paragraphs(paramsContainer.Tenant.Id, titleId.Value,
                isAdmin, paramsContainer.CurrentUserID, removed.Value);

            responseText = "{\"Paragraphs\":[" + ProviderUtil.list_to_string<string>(paragraphs.Select(
                u => _get_paragraph_json(u, isAdmin, editSuggestion, removed.Value)).ToList()) + "]}";
        }

        protected void get_paragraph_related_users(Guid? paragraphId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            responseText = "{\"Users\":[" + (!paragraphId.HasValue ? string.Empty : ProviderUtil.list_to_string<string>(
                WikiController.get_paragraph_related_users(paramsContainer.Tenant.Id, paragraphId.Value).Select(
                u => "{\"UserID\":\"" + u.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                        paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                    "}").ToList())) + "]}";
        }

        protected void accept_reject_wiki_change(Guid? changeId, bool accept, WikiOwnerType ownerType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid ownerId = !changeId.HasValue ? Guid.Empty :
                WikiController.get_wiki_owner(paramsContainer.Tenant.Id, changeId.Value);

            bool isAdmin = changeId.HasValue && (ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true));

            if (!changeId.HasValue || !isAdmin)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (changeId.HasValue) _save_error_log(accept ? Modules.Log.Action.AcceptWikiParagraphChange_PermissionFailed :
                     Modules.Log.Action.RejectWikiParagraphChange_PermissionFailed, changeId.Value);
                return;
            }

            bool result = (accept ? WikiController.accept_change(paramsContainer.Tenant.Id,
                changeId.Value, paramsContainer.CurrentUserID.Value) :
                WikiController.reject_change(paramsContainer.Tenant.Id, changeId.Value, paramsContainer.CurrentUserID.Value));

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
                    Action = accept ? Modules.Log.Action.AcceptWikiParagraphChange : Modules.Log.Action.RejectWikiParagraphChange,
                    SubjectID = changeId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void remove_wiki_change(Guid? changeId, WikiOwnerType ownerType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid ownerId = !changeId.HasValue ? Guid.Empty :
                WikiController.get_wiki_owner(paramsContainer.Tenant.Id, changeId.Value);

            bool isAdmin = changeId.HasValue && (ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true));

            if (!changeId.HasValue || (!isAdmin && WikiController.get_change(paramsContainer.Tenant.Id, changeId.Value)
                .Sender.UserID != paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (changeId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveWikiParagraphChange_PermissionFailed, changeId.Value);
                return;
            }

            bool result = WikiController.remove_change(paramsContainer.Tenant.Id, changeId.Value);

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
                    Action = Modules.Log.Action.RemoveWikiParagraphChange,
                    SubjectID = changeId,
                    ModuleIdentifier = ModuleIdentifier.WK
                });
            }
            //end of Save Log
        }

        protected void get_paragraph_changes(Guid? paragraphId, WikiOwnerType ownerType,
            bool? checkWorkFlowEditPermission, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paragraphId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid ownerId = WikiController.get_wiki_owner(paramsContainer.Tenant.Id, paragraphId.Value);

            bool editSuggestion = paramsContainer.IsAuthenticated && !(ownerType == WikiOwnerType.User);

            bool isAdmin = ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId, true);
            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
                isAdmin = _check_workflow_edit_permission(ownerId);

            //Check Access to Wiki
            if ((ownerType == WikiOwnerType.User && UsersController.get_user(paramsContainer.Tenant.Id, ownerId) == null) ||
                (ownerType != WikiOwnerType.User &&
                !isAdmin && !PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    ownerId, PrivacyObjectType.Node, PermissionType.View)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            //end of Check Access to Wiki

            List<Change> changes = WikiController.get_changes(paramsContainer.Tenant.Id, paragraphId.Value, null, null, true)
                .OrderByDescending(u => u.SendDate).ToList();

            responseText = "{\"Changes\":[" +
                ProviderUtil.list_to_string<string>(changes.Select(u => _get_change_json(u, false)).ToList()) + "]}";
        }

        protected bool export_as_pdf(Guid? ownerId, WikiOwnerType ownerType, Guid? coverId, string password, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return false;

            bool editSuggestion = paramsContainer.IsAuthenticated && !(ownerType == WikiOwnerType.User);

            bool isAdmin = ownerId.HasValue && paramsContainer.IsAuthenticated &&
                (ownerType == WikiOwnerType.User ?
                (PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                ownerId == paramsContainer.CurrentUserID) :
                CNController.has_edit_permission(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ownerId.Value, true, ref editSuggestion));

            //Check Access to Wiki
            bool accessDenied = false;

            if (ownerId.HasValue && ownerType == WikiOwnerType.User &&
                UsersController.get_user(paramsContainer.Tenant.Id, ownerId.Value) == null) accessDenied = true;
            else if (ownerId.HasValue && ownerType != WikiOwnerType.User && !isAdmin &&
                !PrivacyController.check_access(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, ownerId.Value, PrivacyObjectType.Node, PermissionType.Download)) accessDenied = true;

            if (!ownerId.HasValue || accessDenied)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (ownerId.HasValue) _save_error_log(Modules.Log.Action.Wiki_AccessDenied, ownerId.Value);
                return false;
            }
            //end of Check Access to Wiki

            string title = string.Empty;
            string description = string.Empty;
            List<string> tags = new List<string>();

            Dictionary<string, string> metaData = new Dictionary<string, string>();
            List<string> authorFullnames = new List<string>();
            bool isUser = ownerType == WikiOwnerType.User;

            if (ownerType == WikiOwnerType.User)
            {
                User usr = UsersController.get_user(paramsContainer.Tenant.Id, ownerId.Value);

                if (usr == null)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.UserNotFound + "\"}";
                    return false;
                }

                title = usr.FirstName + " " + usr.LastName;
            }
            else if (ownerType == WikiOwnerType.Node)
            {
                Node node =
                    CNController.get_node(paramsContainer.Tenant.Id, ownerId.Value, true, paramsContainer.CurrentUserID);

                if (node == null)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.NodeNotFound + "\"}";
                    return false;
                }

                title = node.Name;
                description = node.Description;
                tags = node.Tags;

                //Authors
                List<NodeCreator> creators =
                    CNController.get_node_creators(paramsContainer.Tenant.Id, ownerId.Value, full: true)
                    .OrderByDescending(u => u.CollaborationShare).ToList();
                List<KeyValuePair<Guid, int>> authors = WikiController.wiki_authors(paramsContainer.Tenant.Id, ownerId.Value);

                List<Guid> userIds = authors.Select(u => u.Key).ToList();
                if (node.Creator.UserID.HasValue) userIds.Add(node.Creator.UserID.Value);
                userIds.AddRange(creators.Select(u => u.User.UserID.Value));
                userIds = userIds.Distinct().ToList();
                List<User> authorUsers = UsersController.get_users(paramsContainer.Tenant.Id, userIds);

                List<KeyValuePair<Guid, int>> wikiAuthors = new List<KeyValuePair<Guid, int>>();

                for (int i = 0; i < creators.Count; ++i)
                {
                    List<NodeCreator> nc =
                        creators.Where(u => u.CollaborationShare == creators[i].CollaborationShare).ToList();

                    List<KeyValuePair<Guid, int>> tmp = new List<KeyValuePair<Guid, int>>();
                    foreach (NodeCreator n in nc)
                    {
                        int count = authors.Any(u => u.Key == n.User.UserID) ?
                            authors.Where(u => u.Key == n.User.UserID).First().Value : 0;
                        tmp.Add(new KeyValuePair<Guid, int>(n.User.UserID.Value, count));
                    }

                    tmp = tmp.OrderByDescending(u => u.Value).ToList();

                    foreach (KeyValuePair<Guid, int> kv in tmp)
                        wikiAuthors.Add(kv);

                    if (tmp.Count > 1) i += tmp.Count - 1;
                }

                if (wikiAuthors.Count == 0 && node.Creator.UserID.HasValue)
                    wikiAuthors.Add(new KeyValuePair<Guid, int>(node.Creator.UserID.Value, 0));

                foreach (KeyValuePair<Guid, int> kv in authors)
                    if (!wikiAuthors.Any(u => u.Key == kv.Key)) wikiAuthors.Add(kv);

                foreach (KeyValuePair<Guid, int> kv in wikiAuthors)
                {
                    User u = authorUsers.Where(x => x.UserID == kv.Key).FirstOrDefault();
                    if (u == null) continue;
                    authorFullnames.Add(u.FirstName + " " + u.LastName);
                }
                //end of Authors

                DateTime? publicationDate = null;

                if (node.PublicationDate.HasValue || node.CreationDate.HasValue)
                    publicationDate = !node.PublicationDate.HasValue ? node.CreationDate : node.PublicationDate;

                DateTime? lastModificationDate =
                    WikiController.last_modification_date(paramsContainer.Tenant.Id, ownerId.Value);

                if ((lastModificationDate.HasValue && publicationDate.HasValue &&
                    lastModificationDate < publicationDate) || !lastModificationDate.HasValue)
                    lastModificationDate = publicationDate;


                metaData["Confidentiality"] = string.IsNullOrEmpty(node.ConfidentialityLevel.Title) ? "__" :
                    node.ConfidentialityLevel.Title;
                metaData["PublicationDate"] = !publicationDate.HasValue ? string.Empty :
                    PublicMethods.convert_numbers_to_local(
                        GenericDate.get_local_date(publicationDate.Value, reverse: true));
                metaData["LastModificationDate"] = !lastModificationDate.HasValue ? string.Empty :
                    PublicMethods.convert_numbers_to_local(
                        GenericDate.get_local_date(lastModificationDate.Value, reverse: true));
                metaData["RegistrationArea"] = node.AdminAreaName;
                metaData["RegistrationAreaType"] = node.AdminAreaType;
            }
            else
            {
                responseText = "{\"ErrorText\":\"" + Messages.WikiNotFound + "\"}";
                return false;
            }

            List<WikiTitle> titles =
                WikiController.get_titles(paramsContainer.Tenant.Id, ownerId.Value, isAdmin, paramsContainer.CurrentUserID);

            List<Paragraph> paragraphs = WikiController.get_title_paragraphs(paramsContainer.Tenant.Id,
                titles.Select(u => u.TitleID.Value).ToList(), isAdmin, paramsContainer.CurrentUserID, false);

            Dictionary<Guid, List<KeyValuePair<string, string>>> wikiParagraphs =
                new Dictionary<Guid, List<KeyValuePair<string, string>>>();

            foreach (WikiTitle t in titles)
            {
                wikiParagraphs[t.TitleID.Value] = paragraphs.Where(p => p.TitleID == t.TitleID).OrderBy(o => o.SequenceNumber)
                    .Select(p => new KeyValuePair<string, string>(p.Title, p.BodyText)).ToList();
            }


            User currentUser = !paramsContainer.CurrentUserID.HasValue ? null :
                UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            if (currentUser == null) currentUser = new User();
            DownloadedFileMeta meta = new DownloadedFileMeta(PublicMethods.get_client_ip(HttpContext.Current),
                currentUser.UserName, currentUser.FirstName, currentUser.LastName, null);

            byte[] buffer = Wiki2PDF.export_as_pdf(
                applicationId: paramsContainer.Tenant.Id, 
                currentUserId: paramsContainer.CurrentUserID,
                isUser: isUser, 
                meta: meta, 
                title: title, 
                description: description,
                keywords: tags,
                wikiTitles: titles.Select(t => new KeyValuePair<Guid, string>(t.TitleID.Value, t.Title)).ToList(),
                wikiParagraphs: wikiParagraphs, 
                metaData: metaData, 
                authors: authorFullnames, 
                coverId: coverId.HasValue && ownerType == WikiOwnerType.Node && ownerId.HasValue ? coverId : null,
                coverOwnerNodeId: coverId.HasValue && ownerType == WikiOwnerType.Node && ownerId.HasValue ? ownerId : null,
                password: password, 
                context: HttpContext.Current);

            paramsContainer.file_response(buffer, title + ".pdf");

            return true;
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