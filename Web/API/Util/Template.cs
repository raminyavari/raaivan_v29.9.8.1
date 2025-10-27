using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Newtonsoft.Json;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.API
{
    [Serializable]
    public class TemplateNodeType {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("children_ids")]
        public List<string> ChildrenIDs;

        [JsonProperty("extensions")]
        public List<Extension> Extensions;

        [JsonProperty("service_title")]
        public string ServiceTitle;

        [JsonProperty("service_description")]
        public string ServiceDescription;

        [JsonProperty("form_id")]
        public string FormID;

        [JsonProperty("no_content")]
        public bool? NoContent;

        [JsonProperty("is_tree")]
        public bool? IsTree;

        [JsonProperty("enable_contribution")]
        public bool? EnableContribution;

        [JsonProperty("enable_previous_version_select")]
        public bool? EnablePreviousVersionSelect;

        [JsonProperty("disable_abstract_and_keywords")]
        public bool? DisableAbstractAndKeywords;

        [JsonProperty("disable_file_upload")]
        public bool? DisableFileUpload;

        [JsonProperty("disable_related_nodes_select")]
        public bool? DisableRelatedNodesSelect;

        public TemplateNodeType() {
            ChildrenIDs = new List<string>();
            Extensions = new List<Extension>();
        }
    }

    [Serializable]
    public class TemplateFormElement
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("code")]
        public string Code;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("weight")]
        public double? Weight;

        [JsonProperty("info")]
        public Dictionary<string, object> Info;

        [JsonProperty("help")]
        public string Help;
    }

    [Serializable]
    public class TemplateForm {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("elements")]
        public List<TemplateFormElement> Elements;

        public TemplateForm() {
            Elements = new List<TemplateFormElement>();
        }
    }

    [Serializable]
    public class TemplateIDs {
        [JsonProperty("dic")]
        private Dictionary<string, string> Dic; //Key: Guid, Value: string

        [JsonProperty("use_existing_objects")]
        private Dictionary<string, Guid> UseExistingObjects;

        [JsonIgnore]
        private Dictionary<string, Guid> NewIDs;

        public TemplateIDs() {
            Dic = new Dictionary<string, string>();
            UseExistingObjects = new Dictionary<string, Guid>();
            NewIDs = new Dictionary<string, Guid>();
        }

        public string resolve(Guid? id)
        {
            if (!id.HasValue) return string.Empty;
            else if (Dic.ContainsKey(id.Value.ToString())) return Dic[id.Value.ToString()];
            else
            {
                string key = string.Empty;

                while (string.IsNullOrEmpty(key) || Dic.Keys.Any(u => Dic[u] == key))
                    key = PublicMethods.random_string(10).ToLower();

                Dic[id.Value.ToString()] = key;

                return key;
            }
        }

        public Guid? get_id_from_template(string id)
        {
            return PublicMethods.parse_guid(Dic.Keys.Where(k => Dic[k] == id).FirstOrDefault());
        }

        public Guid? get_existing_id(string id) {
            return string.IsNullOrEmpty(id) || UseExistingObjects == null || !UseExistingObjects.ContainsKey(id) ?
                null : (Guid?)UseExistingObjects[id];
        }

        public bool is_replaced(string id) {
            return get_existing_id(id).HasValue;
        }

        public Guid? new_id(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (!NewIDs.ContainsKey(id)) NewIDs[id] = Guid.NewGuid();

            return NewIDs[id];
        }
    }

    [Serializable]
    public class Template
    {
        [JsonProperty("ids")]
        private TemplateIDs IDs;

        [JsonProperty("root")]
        private List<string> RootIDs;

        [JsonProperty("node_types")]
        private Dictionary<string, TemplateNodeType> NodeTypes;

        [JsonProperty("forms")]
        private Dictionary<string, TemplateForm> Forms;

        [JsonProperty("dependencies")]
        private Dictionary<string, ArrayList> Dependencies;

        public Template(Guid? nodeTypeId) {
            //Initilization
            IDs = new TemplateIDs();
            RootIDs = new List<string>() { IDs.resolve(nodeTypeId) };
            NodeTypes = new Dictionary<string, TemplateNodeType>();
            Forms = new Dictionary<string, TemplateForm>();
            Dependencies = new Dictionary<string, ArrayList>();
            //end of Initialization
            
            add_node_type(nodeTypeId);
        }

        [JsonIgnore]
        private Guid? RefAppID
        {
            get
            {
                return RaaiVanSettings.ReferenceTenantID;
            }
        }

        private void add_dependency(Guid? id, Guid? dependsOnId) {
            if (!id.HasValue || !dependsOnId.HasValue) return;

            string strId = IDs.resolve(id);
            string strDependsOnId = IDs.resolve(dependsOnId);

            if (!Dependencies.ContainsKey(strId)) Dependencies[strId] = new ArrayList();

            if (!Dependencies[strId].ToArray().Select(i => (string)i).Any(i => i == strDependsOnId))
                Dependencies[strId].Add(strDependsOnId);
        }

        private void add_node_type(Guid? nodeTypeId)
        {
            if (!RefAppID.HasValue || !nodeTypeId.HasValue || !RaaiVanSettings.SAASBasedMultiTenancy) return;

            //get new id
            string newNtId = IDs.resolve(nodeTypeId);
            if (NodeTypes.ContainsKey(newNtId)) return;
            //end of get new id

            Guid appId = RefAppID.Value;
            Guid ntId = nodeTypeId.Value;

            NodeType nodeType = CNController.get_node_type(appId, ntId);

            if (nodeType == null || string.IsNullOrEmpty(nodeType.Name)) return;

            Service service = CNController.get_service(appId, ntId);
            if (service == null) service = new Service();

            List<Extension> extensions = CNController.get_extensions(appId, ntId)
                .Where(ex => !ex.Disabled.HasValue || !ex.Disabled.Value).ToList();
            FormType form = FGController.get_owner_form(appId, ntId);
            List<FormElement> formElements = form == null || !form.FormID.HasValue ? new List<FormElement>() :
                FGController.get_form_elements(appId, form.FormID.Value);

            TemplateNodeType newNodeType = new TemplateNodeType()
            {
                ID = newNtId,
                Name = Base64.encode(nodeType.Name),
                ServiceTitle = Base64.encode(service.Title),
                ServiceDescription = Base64.encode(service.Description),
                Extensions = extensions,
                FormID = form == null ? null : IDs.resolve(form.FormID),
                NoContent = service.NoContent,
                IsTree = service.IsTree,
                EnableContribution = service.EnableContribution,
                EnablePreviousVersionSelect = service.EnablePreviousVersionSelect,
                DisableAbstractAndKeywords = service.DisableAbstractAndKeywords,
                DisableFileUpload = service.DisableFileUpload,
                DisableRelatedNodesSelect = service.DisableRelatedNodesSelect
            };

            NodeTypes[newNtId] = newNodeType;

            if (form != null && form.FormID.HasValue)
            {
                add_dependency(ntId, form.FormID);
                add_form(form, formElements);
            }

            List<NodeType> children = CNController.get_child_node_types(appId, nodeTypeId.Value);
            
            if (children != null && children.Count > 0)
            {
                newNodeType.ChildrenIDs = children.Select(c => IDs.resolve(c.NodeTypeID)).ToList();
                children.ForEach(c => add_node_type(c.NodeTypeID));
            }
        }

        private void add_form(FormType form, List<FormElement> elements)
        {
            //get new id
            string newFormId = IDs.resolve(form.FormID.Value);
            if (Forms.ContainsKey(newFormId)) return;
            //end of get new id

            TemplateForm newForm = new TemplateForm()
            {
                ID = newFormId,
                Title = Base64.encode(form.Title)
            };

            Forms[newFormId] = newForm;

            elements.Where(e => e.Type.HasValue).ToList().ForEach(elem => {
                TemplateFormElement newFormElement = new TemplateFormElement()
                {
                    ID = IDs.resolve(elem.ElementID),
                    Code = Base64.encode(elem.Name),
                    Title = Base64.encode(elem.Title),
                    Type = elem.Type.Value.ToString(),
                    Weight = !elem.Weight.HasValue ? 0 : elem.Weight.Value,
                    Info = PublicMethods.fromJSON(elem.Info),
                    Help = elem.Help
                };

                newForm.Elements.Add(newFormElement);

                if (elem.Type == FormElementTypes.MultiLevel && newFormElement.Info.ContainsKey("NodeType")) {
                    Dictionary<string, object> infoNt = 
                        PublicMethods.get_dic_value<Dictionary<string, object>>(newFormElement.Info, "NodeType");

                    Guid? _id = infoNt == null || !infoNt.ContainsKey("ID") ? null : PublicMethods.parse_guid(infoNt["ID"].ToString());

                    if (_id.HasValue) {
                        string strId = IDs.resolve(_id);
                        ((Dictionary<string, object>)newFormElement.Info["NodeType"])["ID"] = strId.ToString();

                        add_dependency(elem.ElementID, _id);
                        add_node_type(_id);
                    }
                }
                else if (elem.Type == FormElementTypes.Form && newFormElement.Info.ContainsKey("FormID"))
                {
                    Guid? _id = PublicMethods.parse_guid(newFormElement.Info["FormID"].ToString());

                    if (_id.HasValue)
                    {
                        string strId = IDs.resolve(_id);
                        newFormElement.Info["FormID"] = strId;

                        add_dependency(elem.ElementID, _id);
                        add_form(_id.Value);
                    }
                }
                else if (elem.Type == FormElementTypes.Node && newFormElement.Info.ContainsKey("NodeTypes"))
                {
                    ArrayList infoNts = PublicMethods.get_dic_value<ArrayList>(newFormElement.Info, "NodeTypes");

                    List<Dictionary<string, object>> newNts = (infoNts == null ? new ArrayList() : infoNts).ToArray().ToList()
                        .Where(itm => itm.GetType() == typeof(Dictionary<string, object>))
                        .Select(itm => (Dictionary<string, object>)itm)
                        .Where(itm => itm.ContainsKey("NodeTypeID")).ToList()
                        .Select(itm =>
                        {
                            Guid? ntId = PublicMethods.parse_guid(itm["NodeTypeID"].ToString());

                            if (!ntId.HasValue) return null;

                            string strId = IDs.resolve(ntId);

                            itm["NodeTypeID"] = strId;

                            add_dependency(elem.ElementID, ntId);
                            add_node_type(ntId);

                            return itm;
                        }).Where(itm => itm != null).ToList();

                    newFormElement.Info["NodeTypes"] = newNts;
                }
            });
        }

        private void add_form(Guid formId) {
            FormType form = FGController.get_form(RefAppID.Value, formId);
            List<FormElement> elements = FGController.get_form_elements(RefAppID.Value, formId);
            add_form(form, elements);
        }

        public bool activate(Guid applicationId, Guid currentUserId)
        {
            return RootIDs != null && RootIDs.Where(id => !IDs.is_replaced(id)).ToList()
                .Select(id => activate_node_type(applicationId, currentUserId, id) != null).Count() > 0;
        }

        private NodeType activate_node_type(Guid applicationId, Guid currentUserId, string id, Guid? parentNodeTypeId = null)
        {
            TemplateNodeType tempNodeType = !NodeTypes.ContainsKey(id) ? null : NodeTypes[id];
            if (tempNodeType == null) return null;

            NodeType nodeType = new NodeType()
            {
                NodeTypeID = IDs.new_id(tempNodeType.ID),
                TemplateTypeID = IDs.get_id_from_template(tempNodeType.ID),
                Name = Base64.decode(tempNodeType.Name),
                ParentID = parentNodeTypeId,
                CreatorUserID = currentUserId
            };
            
            if(!CNController.add_node_type(applicationId, nodeType)) return null;

            if (tempNodeType.Extensions != null && tempNodeType.Extensions.Count > 0)
            {
                CNController.initialize_extensions(applicationId, nodeType.NodeTypeID.Value, currentUserId, ignoreDefault: true);
                CNController.save_extensions(applicationId, nodeType.NodeTypeID.Value, tempNodeType.Extensions, currentUserId);
            }

            CNController.initialize_service(applicationId, nodeType.NodeTypeID.Value);

            if (!string.IsNullOrEmpty(tempNodeType.ServiceTitle))
                CNController.set_service_title(applicationId, nodeType.NodeTypeID.Value, Base64.decode(tempNodeType.ServiceTitle));

            if (!string.IsNullOrEmpty(tempNodeType.ServiceDescription))
                CNController.set_service_description(applicationId, nodeType.NodeTypeID.Value, Base64.decode(tempNodeType.ServiceDescription));

            if (tempNodeType.NoContent.HasValue && tempNodeType.NoContent.Value)
                CNController.no_content_service(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.IsTree.HasValue && tempNodeType.IsTree.Value)
                CNController.is_tree(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.EnableContribution.HasValue && tempNodeType.EnableContribution.Value)
                CNController.enable_contribution(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.EnablePreviousVersionSelect.HasValue && tempNodeType.EnablePreviousVersionSelect.Value)
                CNController.enable_previous_version_select(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.DisableAbstractAndKeywords.HasValue && tempNodeType.DisableAbstractAndKeywords.Value)
                CNController.abstract_and_keywords_disabled(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.DisableFileUpload.HasValue && tempNodeType.DisableFileUpload.Value)
                CNController.file_upload_disabled(applicationId, nodeType.NodeTypeID.Value, true);

            if (tempNodeType.DisableRelatedNodesSelect.HasValue && tempNodeType.DisableRelatedNodesSelect.Value)
                CNController.related_nodes_select_disabled(applicationId, nodeType.NodeTypeID.Value, true);

            if (!string.IsNullOrEmpty(tempNodeType.FormID) && tempNodeType.Extensions.Any(e => e.ExtensionType == ExtensionType.Form))
            {
                FormType form = activate_form(applicationId, currentUserId, tempNodeType.FormID);
                if (form != null) FGController.set_form_owner(applicationId, nodeType.NodeTypeID.Value, form.FormID.Value, currentUserId);
            }

            if (tempNodeType.ChildrenIDs != null && tempNodeType.ChildrenIDs.Count > 0)
            {
                tempNodeType.ChildrenIDs
                    .ForEach(cId => activate_node_type(applicationId, currentUserId, cId, parentNodeTypeId: nodeType.NodeTypeID));
            }

            activate_node_type_icon(applicationId, id);

            return nodeType;
        }

        private FormType activate_form(Guid applicationId, Guid currentUserId, string id)
        {
            TemplateForm tempForm = !Forms.ContainsKey(id) ? null : Forms[id];
            if (tempForm == null) return null;

            FormType form = new FormType()
            {
                FormID = IDs.new_id(id),
                TemplateFormID = IDs.get_id_from_template(id),
                Title = Base64.decode(tempForm.Title) + " " + PublicMethods.get_random_number().ToString(),
                Creator = new User() { UserID = currentUserId }
            };

            if (!FGController.create_form(applicationId, form)) return null;

            List<FormElement> elements = tempForm.Elements == null ? new List<FormElement>() : tempForm.Elements.Select(e =>
            {
                FormElement newElem = new FormElement()
                {
                    ElementID = IDs.new_id(e.ID),
                    TemplateElementID = IDs.get_id_from_template(e.ID),
                    Name = Base64.decode(e.Code),
                    Title = Base64.decode(e.Title),
                    Weight = e.Weight,
                    Help = e.Help
                };

                e.Info = PublicMethods.fromJSON(PublicMethods.toJSON_typed(e.Info));

                FormElementTypes tp = FormElementTypes.Text;
                if (!Enum.TryParse<FormElementTypes>(e.Type, out tp)) return null;
                else newElem.Type = tp;

                if (tp == FormElementTypes.MultiLevel && e.Info.ContainsKey("NodeType"))
                {
                    Dictionary<string, object> infoNt = PublicMethods.get_dic_value<Dictionary<string, object>>(e.Info, "NodeType");

                    string _id = infoNt == null || !infoNt.ContainsKey("ID") ? null : infoNt["ID"].ToString();

                    NodeType nodeType = string.IsNullOrEmpty(_id) ? null : (IDs.is_replaced(_id) ? 
                        CNController.get_node_type(applicationId, IDs.get_existing_id(_id).Value) :
                        activate_node_type(applicationId, currentUserId, _id));

                    if (nodeType == null) return null;

                    ((Dictionary<string, object>)e.Info["NodeType"])["ID"] = nodeType.NodeTypeID.Value.ToString();
                    ((Dictionary<string, object>)e.Info["NodeType"])["Name"] = Base64.encode(nodeType.Name);
                }
                else if (tp == FormElementTypes.Form && e.Info.ContainsKey("FormID"))
                {
                    string _id = e.Info["FormID"].ToString();

                    FormType elmForm = string.IsNullOrEmpty(_id) ? null : (IDs.is_replaced(_id) ? 
                        FGController.get_form(applicationId, IDs.get_existing_id(_id).Value) :
                        activate_form(applicationId, currentUserId, _id));

                    if (elmForm == null) return null;

                    e.Info["FormID"] = elmForm.FormID.Value.ToString();
                    e.Info["FormName"] = Base64.encode(elmForm.Title);
                }
                else if (tp == FormElementTypes.Node && e.Info.ContainsKey("NodeTypes"))
                {
                    ArrayList infoNts = PublicMethods.get_dic_value<ArrayList>(e.Info, "NodeTypes");

                    List<Dictionary<string, object>> newNts = (infoNts == null ? new ArrayList() : infoNts).ToArray().ToList()
                        .Where(itm => itm.GetType() == typeof(Dictionary<string, object>))
                        .Select(itm => (Dictionary<string, object>)itm)
                        .Where(itm => itm.ContainsKey("NodeTypeID")).ToList()
                        .Select(itm =>
                        {
                            string _id = itm["NodeTypeID"].ToString();

                            NodeType nodeType = string.IsNullOrEmpty(_id) ? null : (IDs.is_replaced(_id) ?
                                CNController.get_node_type(applicationId, IDs.get_existing_id(_id).Value) :
                                activate_node_type(applicationId, currentUserId, _id));

                            if (nodeType == null) return null;

                            itm["NodeTypeID"] = nodeType.NodeTypeID.Value;
                            itm["NodeType"] = Base64.encode(nodeType.Name);

                            return itm;
                        }).Where(itm => itm != null).ToList();

                    e.Info["NodeTypes"] = newNts;
                }

                newElem.Info = PublicMethods.toJSON(e.Info);

                return newElem;
            }).Where(e => e != null).ToList();

            FGController.save_form_elements(applicationId, form.FormID.Value, null, null, null, elements, currentUserId);

            return form;
        }

        private void activate_node_type_icon(Guid applicationId, string id)
        {
            Guid? templateNodeTypeId = IDs.get_id_from_template(id);

            if (!templateNodeTypeId.HasValue) return;

            DocFileInfo pic = new DocFileInfo(RaaiVanSettings.ReferenceTenantID) {
                FileID = templateNodeTypeId,
                Extension = "jpg",
                FileName = templateNodeTypeId.ToString(),
                FolderName = FolderNames.Icons
            };

            byte[] fileContent = pic.toByteArray();

            if (fileContent.Length == 0) return;

            Guid? newFileId = IDs.new_id(id);

            DocFileInfo newPic = new DocFileInfo(applicationId) {
                FileID = newFileId,
                Extension = "jpg",
                FileName = newFileId.ToString(),
                FolderName = FolderNames.Icons
            };

            newPic.store(fileContent);
        }

        public string get_template_status(Guid applicationId)
        {
            List<Guid> formIds = Forms.Keys
                .Where(k => Dependencies.Any(d => d.Value.Contains(k)))
                .Select(k => IDs.get_id_from_template(k))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            List<TemplateStatus> status = FGController.get_template_status(applicationId, formIds);

            List<string> lst = status
                .Select(s => s.TemplateID.Value)
                .Distinct()
                .Select(id => "\"" + id.ToString() + "\":" +
                    "[" + string.Join(",", status.Where(s => s.TemplateID == id).Select(s => s.toJson(applicationId))) + "]")
                .ToList();

            return "{" + string.Join(",", lst) + "}";
        }
    }
}