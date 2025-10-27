using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.IO;
using System.Net;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for FileDownLoad
    /// </summary>
    public class DownLoad : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer;

        public void ProcessRequest(HttpContext context)
        {
            //Privacy Check: OK
            paramsContainer = new ParamsContainer(context, nullTenantResponse: false);
            
            Guid fileId = string.IsNullOrEmpty(context.Request.Params["FileID"]) ?
                Guid.Empty : Guid.Parse(context.Request.Params["FileID"]);
            if (fileId == Guid.Empty && !Guid.TryParse(context.Request.Params["ATTFID"], out fileId)) fileId = Guid.Empty;

            string category = PublicMethods.parse_string(context.Request.Params["Category"], false);
            
            bool isTemporary = category.ToLower() == FolderNames.TemporaryFiles.ToString().ToLower();
            bool? addFooter = PublicMethods.parse_bool(context.Request.Params["Meta"]);
            Guid? coverId = PublicMethods.parse_guid(context.Request.Params["CoverID"]);
            string pdfPassword = PublicMethods.parse_string(context.Request.Params["PS"]);

            List<FolderNames> freeFolders = new[] {
                FolderNames.ProfileImages,
                FolderNames.HighQualityProfileImage,
                FolderNames.CoverPhoto,
                FolderNames.HighQualityCoverPhoto,
                FolderNames.Icons,
                FolderNames.HighQualityIcon,
                FolderNames.ApplicationIcons,
                FolderNames.HighQualityApplicationIcon,
                FolderNames.Pictures
            }.ToList();

            bool isFreeFolder = !string.IsNullOrEmpty(category) && freeFolders.Any(f => f.ToString().ToLower() == category.ToLower());

            if (isFreeFolder) {
                FolderNames fn = freeFolders.Where(u => u.ToString().ToLower() == category.ToLower()).FirstOrDefault();

                DocFileInfo pic = new DocFileInfo(paramsContainer.ApplicationID)
                {
                    FileID = fileId,
                    Extension = "jpg",
                    FileName = fileId.ToString(),
                    FolderName = fn
                };

                send_file(pic, false);
            }

            if (!paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return;
            }

            if (isTemporary)
            {
                string ext = PublicMethods.parse_string(context.Request.Params["Extension"]);

                DocFileInfo temp = new DocFileInfo(paramsContainer.ApplicationID)
                {
                    FileID = fileId,
                    Extension = ext,
                    FileName = fileId.ToString(),
                    FolderName = FolderNames.TemporaryFiles
                };

                send_file(temp, false);
            }
            else
            {
                DocFileInfo AttachFile = DocumentsController.get_file(paramsContainer.Tenant.Id, fileId);

                if (AttachFile == null) {
                    paramsContainer.return_response("{\"ErrorText\":\"" + Messages.AccessDenied + "\"}");
                    return;
                }

                PrivacyObjectType pot = AttachFile.OwnerType == FileOwnerTypes.Node ?
                    PrivacyObjectType.Node : PrivacyObjectType.None;

                DocFileInfo ownerNode = !AttachFile.FileID.HasValue ? null :
                    DocumentsController.get_file_owner_node(paramsContainer.Tenant.Id, AttachFile.FileID.Value);
                if (ownerNode != null)
                {
                    AttachFile.OwnerNodeID = ownerNode.OwnerNodeID;
                    AttachFile.OwnerNodeName = ownerNode.OwnerNodeName;
                    AttachFile.OwnerNodeType = ownerNode.OwnerNodeType;
                }

                //check access
                AccessChecker accessChecker = new AccessChecker(paramsContainer.ApplicationID, paramsContainer.CurrentUserID,
                    nodeIdOrNodeTypeId: AttachFile.OwnerID);

                bool hasAccess = accessChecker.checkNodeEditAccess(AdminLevel.Creator, 
                    privacyObjectType: pot, permission: PermissionType.View);

                if (!hasAccess)
                {
                    //Save Log
                    try
                    {
                        LogController.save_log(paramsContainer.Tenant.Id, new Log()
                        {
                            UserID = paramsContainer.CurrentUserID,
                            HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                            HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                            Action = Modules.Log.Action.Download_AccessDenied,
                            SubjectID = fileId,
                            Info = "{\"Error\":\"" + Base64.encode(Messages.AccessDenied.ToString()) + "\"}",
                            ModuleIdentifier = ModuleIdentifier.DCT
                        });
                    }
                    catch { }
                    //end of Save Log

                    paramsContainer.return_response("{\"ErrorText\":\"" + Messages.AccessDenied + "\"}");

                    return;
                }
                //end of check access

                AttachFile.refresh_folder_name();

                string ext = AttachFile == null || string.IsNullOrEmpty(AttachFile.Extension) ? string.Empty :
                    AttachFile.Extension.ToLower();
                bool isImage = ext == "jpg" || ext == "jpeg" || ext == "png" || ext == "gif" || ext == "bmp";

                if (string.IsNullOrEmpty(AttachFile.Extension) || AttachFile.Extension.ToLower() != "pdf")
                    coverId = null;

                bool dl = !isImage || PublicMethods.parse_bool(context.Request.Params["dl"], defaultValue: true) == true;
                string contentType = !dl && isImage ? PublicMethods.get_mime_type_by_extension(ext) : null;

                send_file(AttachFile, !isImage, addPDFCover: true,
                    addPDFFooter: addFooter.HasValue && addFooter.Value,
                    coverId: coverId,
                    pdfPassword: pdfPassword,
                    contentType: contentType,
                    isAttachment: dl);
            }
        }

        protected void send_file(DocFileInfo file, bool logNeeded, bool addPDFCover = false, bool addPDFFooter = false, 
            Guid? coverId = null, string pdfPassword = null, string contentType = null, bool isAttachment = true)
        {
            byte[] fileContent = file.toByteArray();

            if (fileContent.Length == 0) {
                send_empty_response();
                return;
            }
            
            //Save Log
            if (logNeeded && paramsContainer.CurrentUserID.HasValue)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.Download,
                    SubjectID = file.FileID,
                    Info = file.toJson(),
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            //end of Save Log

            if (file.Extension.ToLower() == "pdf")
            {
                addPDFCover = addPDFCover && file.OwnerNodeID.HasValue && coverId.HasValue &&
                    paramsContainer.ApplicationID.HasValue && paramsContainer.CurrentUserID.HasValue;

                if (addPDFFooter || addPDFCover)
                {
                    bool invalidPassword = false;

                    fileContent = PDFUtil.get_pdf_content(fileContent, pdfPassword, ref invalidPassword);

                    if (invalidPassword)
                    {
                        string responseText = "{\"InvalidPassword\":" + true.ToString().ToLower() + "}";
                        paramsContainer.return_response(ref responseText);
                        return;
                    }
                }

                if (addPDFFooter) {
                    User currentUser = !paramsContainer.CurrentUserID.HasValue ? null :
                        UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

                    if (currentUser == null)
                    {
                        currentUser = new User()
                        {
                            UserID = Guid.NewGuid(),
                            UserName = "[anonymous]",
                            FirstName = "[anonymous]",
                            LastName = "[anonymous]"
                        };
                    }

                    DownloadedFileMeta meta = new DownloadedFileMeta(PublicMethods.get_client_ip(HttpContext.Current),
                        currentUser.UserName, currentUser.FirstName, currentUser.LastName, null);

                    fileContent = PDFTemplates.append_footer(fileContent, meta.toString());
                }

                if (addPDFCover) fileContent = Wiki2PDF.add_custom_cover(paramsContainer.Tenant.Id, 
                    paramsContainer.CurrentUserID.Value, fileContent, coverId.Value, file.OwnerNodeID.Value);

                if (!string.IsNullOrEmpty(pdfPassword)) fileContent = PDFUtil.set_password(fileContent, pdfPassword);
            }

            string retFileName = file.FileName + (string.IsNullOrEmpty(file.Extension) ? string.Empty : "." + file.Extension);

            paramsContainer.file_response(fileContent, retFileName, contentType: contentType, isAttachment: isAttachment);
        }

        protected void send_empty_response()
        {
            paramsContainer.return_response("FileNotFound");
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