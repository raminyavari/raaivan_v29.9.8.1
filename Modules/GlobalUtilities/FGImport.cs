using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Drawing;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;

namespace RaaiVan.Modules.FormGenerator
{
    public class FGImport
    {
        private static List<String> StrNationalID = ("national_id|nationalid|national_code|nationalcode" +
            "|mellicode|melli_code|code_melli|codemelli").Split('|').ToList();
        private static List<String> StrFirstName = ("first_name|firstname").Split('|').ToList();
        private static List<String> StrLastName = ("last_name|lastname").Split('|').ToList();
        private static List<String> StrFullName = ("full_name|fullname").Split('|').ToList();

        private static string DEFAULTPREFIX = "RVDef";

        public static bool extract_forms_from_xml(Guid applicationId, Guid? formId, XmlDocument xmlDoc, 
            Dictionary<string, object> map, Guid currentUserId, ref List<FormType> formsToBeSaved, 
            ref List<FormElement> elementsToBeSaved, ref Dictionary<Guid, object> nodes)
        {
            try
            {
                if (map.ContainsKey("sub")) map = (Dictionary<string, object>)map["sub"];

                List<FormElement> refElements = !formId.HasValue ? null :
                    FGController.get_form_elements(applicationId, formId).OrderBy(u => u.SequenceNumber).ToList();

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);

                foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
                {
                    XmlNodeList lstNodeName = get_child_nodes(xmlNode, "node_name", nsmgr);
                    XmlNodeList lstNodeId = get_child_nodes(xmlNode, "node_id", nsmgr);
                    XmlNodeList lstParentId = get_child_nodes(xmlNode, "node_parent_id", nsmgr);
                    XmlNodeList lstTags = get_child_nodes(xmlNode, "node_tags", nsmgr);
                    XmlNodeList lstAbstract = get_child_nodes(xmlNode, "node_abstract", nsmgr);

                    Guid nodeId = Guid.NewGuid();
                    string nodeName = lstNodeName == null || lstNodeName.Count <= 0 ? null : lstNodeName.Item(0).InnerText;
                    string nodeAddId = lstNodeId == null || lstNodeId.Count <= 0 ? null : lstNodeId.Item(0).InnerText;
                    string nodeParentId = lstParentId == null || lstParentId.Count <= 0 ? null : lstParentId.Item(0).InnerText;
                    string nodeTags = lstTags == null || lstTags.Count <= 0 ? null : lstTags.Item(0).InnerText
                        .Replace('-', '~').Replace(';', '~').Replace(',', '~');
                    string nodeAbstract = lstAbstract == null || lstAbstract.Count <= 0 ? null : lstAbstract.Item(0).InnerText;

                    if (string.IsNullOrEmpty(nodeName)) continue;

                    //Store the node
                    Dictionary<string, object> nd = new Dictionary<string, object>();

                    nd["Name"] = nodeName;
                    if (!string.IsNullOrEmpty(nodeAddId)) nd["AdditionalID"] = nodeAddId;
                    if (!string.IsNullOrEmpty(nodeParentId)) nd["ParentAdditionalID"] = nodeParentId;
                    if (!string.IsNullOrEmpty(nodeTags)) nd["Tags"] = nodeTags;
                    if (!string.IsNullOrEmpty(nodeAbstract)) nd["Abstract"] = nodeAbstract;

                    nodes[nodeId] = nd;
                    //end of Store the node

                    if (!formId.HasValue || refElements == null || refElements.Count == 0) continue;

                    FormType formInstance = new FormType()
                    {
                        FormID = formId,
                        InstanceID = Guid.NewGuid(),
                        OwnerID = nodeId,
                        Creator = new Users.User() { UserID = currentUserId },
                        CreationDate = DateTime.Now
                    };

                    List<FormElement> formElements = new List<FormElement>();
                    foreach (FormElement e in refElements) formElements.Add((FormElement)e.Clone());

                    List<FormElement> theElements = new List<FormElement>();

                    foreach (string name in map.Keys)
                    {
                        List<FormElement> newElems = _parse_imported_form(applicationId, xmlNode,
                            get_child_nodes(xmlNode, name, nsmgr), nsmgr, map[name], formElements, map);
                        if (newElems != null && newElems.Count > 0) theElements.AddRange(newElems);
                    }

                    List<FormType> newFormInstances = new List<FormType>();
                    List<FormElement> newElements = new List<FormElement>();
                    List<DocFileInfo> newFiles = new List<DocFileInfo>();

                    get_save_items(formInstance, theElements, ref newFormInstances, ref newElements, ref newFiles);

                    //remove empty text elements
                    newElements = newElements
                        .Where(u => u.Type != FormElementTypes.Text || !string.IsNullOrEmpty(u.TextValue)).ToList();
                    //end of remove empty text elements

                    if (theElements != null && newElements.Count > 0)
                    {
                        formsToBeSaved.Add(formInstance);
                        elementsToBeSaved.AddRange(newElements);
                    }
                }

                return true;
            }
            catch (Exception ex) {
                LogController.save_error_log(applicationId, currentUserId, 
                    "ExtractFormsFromXML", ex, ModuleIdentifier.FG, LogLevel.Fatal);
                return false;
            }
        }

        public static bool import_form(Guid applicationId, Guid? instanceId, DocFileInfo uploadedFile,
            Dictionary<string, object> map, Guid currentUserId, ref List<FormElement> savedElements, 
            List<FormElement> nodeElements, ref string errorMessage)
        {
            if (!instanceId.HasValue || uploadedFile == null || !uploadedFile.FileID.HasValue) return false;

            if (!uploadedFile.exists()) return false;

            if (map.ContainsKey("sub")) map = (Dictionary<string, object>)map["sub"];

            FormType formInstance = FGController.get_form_instance(applicationId, instanceId.Value);

            if (formInstance == null || !formInstance.InstanceID.HasValue || !formInstance.FormID.HasValue) return false;

            List<FormElement> formElements = FGController.get_form_instance_elements(applicationId,
                instanceId.Value).OrderBy(u => u.SequenceNumber).ToList();
            
            if (formElements == null || formElements.Count == 0) return false;
            
            XmlDocument doc = new XmlDocument();
            
            try
            {
                using (MemoryStream stream = new MemoryStream(uploadedFile.toByteArray()))
                    doc.Load(stream);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, currentUserId,
                    "FG_ImportForm_LoadFile", ex, ModuleIdentifier.FG);
                errorMessage = Messages.OperationFailed.ToString();
                return false;
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            if (!string.IsNullOrEmpty(doc.DocumentElement.GetNamespaceOfPrefix("")))
            {
                nsmgr.AddNamespace(DEFAULTPREFIX, doc.DocumentElement.GetNamespaceOfPrefix(""));
                nsmgr.AddNamespace("", doc.DocumentElement.GetNamespaceOfPrefix(""));
            }
            foreach (XmlAttribute attr in doc.SelectSingleNode("/*").Attributes)
                if (attr.Prefix == "xmlns") nsmgr.AddNamespace(attr.LocalName, attr.Value);

            List<FormElement> theElements = new List<FormElement>();

            //add node elements
            if (nodeElements == null) nodeElements = new List<FormElement>();

            foreach (FormElement e in nodeElements)
            {
                e.Type = FormElementTypes.Text;
                formElements.Add(e);
            }
            //end of add node elements

            foreach (string name in map.Keys)
            {
                List<FormElement> newElems = _parse_imported_form(applicationId, doc.DocumentElement, 
                    get_child_nodes(doc.DocumentElement, name, nsmgr), nsmgr, map[name], formElements, map);
                if (newElems != null && newElems.Count > 0) theElements.AddRange(newElems);
            }

            //remove node elements
            foreach (FormElement e in nodeElements) {
                FormElement elem = theElements.Where(
                    u => (u.ElementID == e.ElementID || u.RefElementID == e.ElementID) && u.Name == e.Name).FirstOrDefault();

                if (elem != null)
                {
                    e.TextValue = elem.TextValue;
                    theElements.Remove(elem);
                }
            }
            //end of remove node elements

            List<FormType> newFormInstances = new List<FormType>();
            List<FormElement> newElements = new List<FormElement>();
            List<DocFileInfo> newFiles = new List<DocFileInfo>();
            
            get_save_items(formInstance, theElements, ref newFormInstances, ref newElements, ref newFiles);

            //set_national_ids(ref newElements);
            set_identity_values(ref newElements);

            //remove empty text elements
            newElements = newElements
                .Where(u => u.Type != FormElementTypes.Text || !string.IsNullOrEmpty(u.TextValue)).ToList();
            //end of remove empty text elements

            if (newFiles != null) newFiles.ForEach(f => f.move(FolderNames.TemporaryFiles, FolderNames.Attachments));

            bool result = newFormInstances == null || newFormInstances.Count == 0 || 
                FGController.create_form_instances(applicationId, newFormInstances, currentUserId);
            
            result = result && FGController.save_form_instance_elements(applicationId, 
                newElements, new List<Guid>(), currentUserId, ref errorMessage);

            if(!result && newFiles != null)
                newFiles.ForEach(f => f.move(FolderNames.Attachments, FolderNames.TemporaryFiles));

            if (result) savedElements = theElements;

            return result;
        }

        public static bool import_form(Guid applicationId, Guid? instanceId, DocFileInfo uploadedFile,
            string map, Guid currentUserId, ref List<FormElement> savedElements, ref string nodeName,
            ref DocFileInfo logo, ref string errorMessage)
        {
            if (uploadedFile != null) uploadedFile.FolderName = FolderNames.TemporaryFiles;

            List<FormElement> nodeElements = new List<FormElement>();
            nodeElements.Add(new FormElement() { ElementID = Guid.NewGuid(), Name = "node_name", Type = FormElementTypes.Text });
            nodeElements.Add(new FormElement() { ElementID = Guid.NewGuid(), Name = "node_logo", Type = FormElementTypes.Text });

            if (!FGImport.import_form(applicationId, instanceId, uploadedFile,
                PublicMethods.fromJSON(map), currentUserId, ref savedElements, nodeElements, ref errorMessage)) return false;

            nodeName = nodeElements.Where(u => u.Name == "node_name").Select(x => x.TextValue).FirstOrDefault();
            string nodeLogo = nodeElements.Where(u => u.Name == "node_logo").Select(x => x.TextValue).FirstOrDefault();

            logo = DocumentUtilities.decode_base64_file_content(applicationId, null, nodeLogo, FileOwnerTypes.None);

            return true;
        }

        private static void set_national_ids(ref List<FormElement> elements)
        {
            foreach (FormElement elem in elements.Where(u => !string.IsNullOrEmpty(u.Name) &&
                u.FormInstanceID.HasValue && u.Type == FormElementTypes.Text &&
                field_name_match(u.Name, StrNationalID) && string.IsNullOrEmpty(u.TextValue)))
            {
                string firstName = _find_text_value(elem, ref elements, StrFirstName);
                string lastName = _find_text_value(elem, ref elements, StrLastName);
                string fullName = _find_text_value(elem, ref elements, StrFullName);

                if (string.IsNullOrEmpty(fullName) &&
                    (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))) continue;

                bool useFullName = !string.IsNullOrEmpty(fullName);

                List<FormElement> match = elements.Where(x => x.FormInstanceID.HasValue &&
                    x.FormInstanceID != elem.FormInstanceID && x.Type == FormElementTypes.Text &&
                    field_name_match(x.Name, StrFullName) && x.TextValue == (useFullName ? fullName : firstName)).ToList();

                foreach (FormElement m in match)
                {
                    string lName = useFullName ? string.Empty : _find_text_value(m, ref elements, StrLastName);
                    if (!useFullName && lName != lastName) continue;

                    string nationalId = _find_text_value(m, ref elements, StrNationalID);

                    if (!string.IsNullOrEmpty(nationalId))
                    {
                        elem.TextValue = nationalId;
                        break;
                    }
                }
            }
        }

        private static bool is_match(FormElement elem, FormElement elemToCheck, ref List<FormElement> allElements,
            string nationalId, string firstName, string lastName, string fullName) {
            List<string> strAll = new List<string>();
            strAll.AddRange(StrNationalID);
            strAll.AddRange(StrFirstName);
            strAll.AddRange(StrLastName);
            strAll.AddRange(StrFullName);

            List<string> firstAndLast = new List<string>();
            firstAndLast.AddRange(StrFirstName);
            firstAndLast.AddRange(StrLastName);
            
            if (!elemToCheck.FormInstanceID.HasValue || elemToCheck.FormInstanceID == elem.FormInstanceID ||
                elemToCheck.Type != FormElementTypes.Text || !field_name_match(elemToCheck.Name, strAll)) return false;

            if (field_name_match(elemToCheck.Name, StrNationalID))
                return !string.IsNullOrEmpty(nationalId) && elemToCheck.TextValue == nationalId;
            else if (field_name_match(elemToCheck.Name, StrFullName))
            {
                return (!string.IsNullOrEmpty(fullName) && elemToCheck.TextValue == fullName) ||
                        (!string.IsNullOrEmpty(firstName) && elemToCheck.TextValue == firstName + " " + lastName);
            }
            else if (field_name_match(elemToCheck.Name, firstAndLast))
            {
                string f = _find_text_value(elemToCheck, ref allElements, StrFirstName);
                string l = _find_text_value(elemToCheck, ref allElements, StrLastName);

                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) &&
                    f == firstName && l == lastName) return true;
                else if (!string.IsNullOrEmpty(fullName) && f + " " + l == fullName) return true;
                else return false;
            }

            return false;
        }
        
        private static void set_identity_values(ref List<FormElement> elements)
        {
            List<string> strAll = new List<string>();
            strAll.AddRange(StrNationalID);
            strAll.AddRange(StrFirstName);
            strAll.AddRange(StrLastName);
            strAll.AddRange(StrFullName);
            
            foreach (FormElement elem in elements.Where(u => !string.IsNullOrEmpty(u.Name) &&
                u.FormInstanceID.HasValue && u.Type == FormElementTypes.Text &&
                field_name_match(u.Name, strAll) && string.IsNullOrEmpty(u.TextValue)))
            {
                string nationalId = _find_text_value(elem, ref elements, StrNationalID);
                string fullName = _find_text_value(elem, ref elements, StrFullName);
                string firstName = _find_text_value(elem, ref elements, StrFirstName);
                string lastName = _find_text_value(elem, ref elements, StrLastName);

                if (string.IsNullOrEmpty(nationalId) && string.IsNullOrEmpty(fullName) &&
                    (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))) continue;
                
                List<FormElement> match = new List<FormElement>();
                
                foreach (FormElement x in elements)
                    if (is_match(elem, x, ref elements, nationalId, firstName, lastName, fullName)) match.Add(x);

                foreach (FormElement m in match)
                {
                    if (field_name_match(elem.Name, StrNationalID))
                        elem.TextValue = _find_text_value(m, ref elements, StrNationalID);
                    else if (field_name_match(elem.Name, StrFirstName))
                        elem.TextValue = _find_text_value(m, ref elements, StrFirstName);
                    else if (field_name_match(elem.Name, StrLastName))
                        elem.TextValue = _find_text_value(m, ref elements, StrLastName);
                    else if (field_name_match(elem.Name, StrFullName))
                    {
                        string refFullName = _find_text_value(m, ref elements, StrFullName);

                        elem.TextValue = !string.IsNullOrEmpty(refFullName) ? refFullName :
                            _find_text_value(m, ref elements, StrFirstName) + " " + _find_text_value(m, ref elements, StrLastName);
                    }

                    if (!string.IsNullOrEmpty(elem.TextValue)) break;
                }
            }
        }

        private static bool field_name_match(string name, List<string> lst) {
            if(!string.IsNullOrEmpty(name)) name = name.ToLower();
            return !string.IsNullOrEmpty(name) && (lst.Any(u => u.StartsWith(name) || u.EndsWith(name)));
        }

        private static string _find_text_value(FormElement refElement, 
            ref List<FormElement> elements, List<string> strValidNames)
        {
            return elements.Where(u => u.FormInstanceID.HasValue && u.Type == FormElementTypes.Text &&
                (refElement == null || u.FormInstanceID == refElement.FormInstanceID) && 
                field_name_match(u.Name, strValidNames) &&
                !string.IsNullOrEmpty(u.TextValue)).Select(x => x.TextValue).FirstOrDefault();
        }

        private static XmlNodeList get_child_nodes(XmlNode node, string name, XmlNamespaceManager nsmgr) {
            if (!string.IsNullOrEmpty(node.Prefix)) nsmgr.AddNamespace(node.Prefix, node.NamespaceURI);
            XmlNodeList ret = node.SelectNodes(name, nsmgr);

            if (ret.Count == 0 && !string.IsNullOrEmpty(nsmgr.DefaultNamespace))
                ret = node.SelectNodes(DEFAULTPREFIX + ":" + name, nsmgr);

            if (ret.Count == 0 && !string.IsNullOrEmpty(node.Prefix) && 
                !name.ToLower().StartsWith(node.Prefix.ToLower() + ":"))
                return node.SelectNodes(node.Prefix + ":" + name, nsmgr);
            else return ret;
        }

        private static void get_save_items(FormType currentFormInstance, List<FormElement> currentElements,
            ref List<FormType> retFormInstances, ref List<FormElement> retElements, ref List<DocFileInfo> retFiles)
        {
            Dictionary<Guid, bool> processed = new Dictionary<Guid, bool>();

            foreach (FormElement elem in currentElements)
            {
                if (!elem.RefElementID.HasValue || processed.ContainsKey(elem.RefElementID.Value)) continue;
                processed[elem.RefElementID.Value] = true;

                elem.FormInstanceID = currentFormInstance.InstanceID;
                elem.FormID = currentFormInstance.FormID;

                retElements.Add(elem);

                if (elem.AttachedFiles != null && elem.AttachedFiles.Count > 0) retFiles.AddRange(elem.AttachedFiles);

                if (elem.TableContent != null && elem.TableContent.Count > 0)
                {
                    foreach (FormType frm in elem.TableContent)
                    {
                        if (!frm.InstanceID.HasValue || !frm.FormID.HasValue || frm.Elements == null ||
                            frm.Elements.Count == 0 || processed.ContainsKey(frm.InstanceID.Value)) continue;
                        processed[frm.InstanceID.Value] = true;

                        retFormInstances.Add(frm);

                        get_save_items(frm, frm.Elements, ref retFormInstances, ref retElements, ref retFiles);
                    }
                }
            }
        }

        private static List<FormElement> _parse_imported_form(Guid applicationId, 
            XmlNode parentNode, XmlNodeList nodeList, XmlNamespaceManager nsmgr, object mapEntry, 
            List<FormElement> formElements, Dictionary<string, object> parentMap)
        {
            if (nodeList == null || nodeList.Count == 0) return new List<FormElement>();

            List<FormElement> retList = new List<FormElement>();

            Type tp = mapEntry.GetType();

            if (tp == typeof(Dictionary<string, object>))
            {
                Dictionary<string, object> map = (Dictionary<string, object>)mapEntry;

                string target = map.ContainsKey("target") &&
                    formElements.Any(u => !string.IsNullOrEmpty(u.Name) && u.Name.ToLower() == map["target"].ToString().ToLower()) ?
                    map["target"].ToString().ToLower() : string.Empty;

                if (!string.IsNullOrEmpty(target))
                {
                    FormElement elem = formElements.Where(
                        u => !string.IsNullOrEmpty(u.Name) && u.Name.ToLower() == target).FirstOrDefault();

                    FormElement newElement = _extract_xml_node_data(applicationId, 
                        parentNode, nodeList, nsmgr, elem, map, parentMap);

                    if (newElement != null) retList.Add(newElement);
                }
                else
                {
                    Dictionary<string, object> subMap = map.ContainsKey("sub") &&
                        map["sub"].GetType() == typeof(Dictionary<string, object>) ?
                        (Dictionary<string, object>)map["sub"] : null;

                    if (subMap != null)
                    {
                        foreach (string name in subMap.Keys)
                        {
                            foreach (XmlNode nd in nodeList)
                            {
                                if (!string.IsNullOrEmpty(nd.Prefix)) nsmgr.AddNamespace(nd.Prefix, nd.NamespaceURI);

                                List<FormElement> lst = _parse_imported_form(applicationId, nd, 
                                    get_child_nodes(nd, name, nsmgr), nsmgr, subMap[name], formElements, subMap);
                                if (lst != null && lst.Count > 0) retList.AddRange(lst);
                            }
                        }
                    }
                }
            }
            else if (tp == typeof(string))
            {
                FormElement elem = formElements
                    .Where(u => !string.IsNullOrEmpty(u.Name) && u.Name.ToLower() == ((string)mapEntry).ToString().ToLower()).FirstOrDefault();

                FormElement el = 
                    _extract_xml_node_data(applicationId, parentNode, nodeList, nsmgr, elem, null, parentMap);

                if (el != null) retList.Add(el);
            }

            return retList;
        }

        private static FormElement _extract_xml_node_data(Guid applicationId, 
            XmlNode parentNode, XmlNodeList nodeList, XmlNamespaceManager nsmgr, FormElement formElement, 
            Dictionary<string, object> map, Dictionary<string, object> parentMap)
        {
            if (formElement == null || !formElement.ElementID.HasValue || 
                nodeList == null || nodeList.Count == 0) return null;
            
            FormElement retElement = new FormElement()
            {
                ElementID = formElement.ElementID,
                RefElementID = formElement.RefElementID,
                FormInstanceID = formElement.FormInstanceID,
                Type = formElement.Type,
                SequenceNumber = formElement.SequenceNumber,
                Name = formElement.Name
            };
            
            if (!retElement.RefElementID.HasValue || retElement.RefElementID == retElement.ElementID)
            {
                retElement.RefElementID = retElement.ElementID;
                retElement.ElementID = Guid.NewGuid();
            }
            
            switch (formElement.Type)
            {
                case FormElementTypes.Text:
                case FormElementTypes.Select:
                case FormElementTypes.Node:
                case FormElementTypes.User:
                    {
                        string strValue = nodeList.Item(0).InnerText.Trim();
                        int intValue = 0;

                        List<string> options = null;

                        if (formElement.Type == FormElementTypes.Select && 
                            int.TryParse(strValue, out intValue) && intValue > 0) {
                            //if strValue is an integer, probably it is the index of selected option
                            Dictionary<string, object> dic = PublicMethods.fromJSON(formElement.Info);
                            if (dic.ContainsKey("Options") && dic["Options"].GetType() == typeof(ArrayList)) {
                                ArrayList arr = (ArrayList)dic["Options"];
                                options = new List<string>();
                                foreach (object obj in arr) options.Add(Base64.decode((string)obj));

                                if (options.Count >= intValue && !options.Any(u => u == strValue))
                                    strValue = options[intValue - 1];
                            }
                        }

                        if (!string.IsNullOrEmpty(strValue))
                        {
                            retElement.TextValue = strValue;
                            return retElement;
                        }
                        else return null;
                    }
                case FormElementTypes.Checkbox:
                case FormElementTypes.MultiLevel:
                    {
                        List<string> strItems = new List<string>();
                        foreach (XmlNode nd in nodeList)
                            strItems.Add(nd.InnerText.Trim());
                        string strValue = string.Join(" ~ ", strItems.Where(u => !string.IsNullOrEmpty(u)));
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            retElement.TextValue = strValue;
                            return retElement;
                        }
                        else return null;
                    }
                case FormElementTypes.Binary:
                    {
                        Dictionary<string, object> dic = PublicMethods.fromJSON(formElement.Info);

                        string yes = dic.ContainsKey("Yes") ? Base64.decode((string)dic["Yes"]) : string.Empty;
                        string no = dic.ContainsKey("No") ? Base64.decode((string)dic["No"]) : string.Empty;

                        string txt = nodeList.Item(0).InnerText.Trim().ToLower();
                        bool? bitValue = null;
                        if (!string.IsNullOrEmpty(txt) && "truefalse".IndexOf(txt) >= 0) bitValue = txt == "true";
                        if (bitValue.HasValue)
                        {
                            retElement.BitValue = bitValue;
                            retElement.TextValue = bitValue.Value ? (string.IsNullOrEmpty(yes) ? null : yes) :
                                (string.IsNullOrEmpty(no) ? null : no);
                            return retElement;
                        }
                        else return null;
                    }
                case FormElementTypes.Numeric:
                    {
                        double dblValue = 0;
                        if (double.TryParse(nodeList.Item(0).InnerText, out dblValue))
                        {
                            retElement.FloatValue = dblValue;
                            return retElement;
                        }
                        else return null;
                    }
                case FormElementTypes.Date:
                    {
                        string calendarType = map != null && map.ContainsKey("calendar_type") ?
                            map["calendar_type"].ToString() : string.Empty;
                        DateTime? dateValue = null;

                        if (string.IsNullOrEmpty(calendarType) || calendarType.ToLower() == "jalali")
                        {
                            string[] parts = nodeList.Item(0).InnerText.Trim().Split('/');
                            int first = 0, second = 0, third = 0;
                            if (parts.Length == 3 && int.TryParse(parts[0], out first) &&
                                int.TryParse(parts[1], out second) && int.TryParse(parts[2], out third))
                                dateValue = PublicMethods.persian_to_gregorian_date(first, second, third, null, null, null);
                        }

                        if (!dateValue.HasValue && parentMap != null)
                        {
                            int year = 0;
                            if (int.TryParse(nodeList.Item(0).InnerText, out year) && year > 0)
                                dateValue = extract_date_from_parts(year, retElement.Name, parentNode, nsmgr, parentMap);
                        }

                        if (dateValue.HasValue)
                        {
                            retElement.DateValue = dateValue;
                            retElement.TextValue = GenericDate.get_local_date(dateValue);
                            return retElement;
                        }
                        else
                        {
                            string strValue = nodeList.Item(0).InnerText.Trim();
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                retElement.TextValue = strValue;
                                return retElement;
                            }
                            else return null;
                        }
                    }
                case FormElementTypes.File:
                    {
                        string strContent = nodeList.Item(0).InnerText.Trim();
                        DocFileInfo doc = DocumentUtilities.decode_base64_file_content(applicationId,
                            retElement.ElementID, strContent, FileOwnerTypes.FormElement);
                        if (doc != null && !string.IsNullOrEmpty(doc.FileName) && doc.Size.HasValue && doc.Size > 0)
                            retElement.AttachedFiles.Add(doc);
                        return retElement.AttachedFiles.Count > 0 ? retElement : null;
                    }
                case FormElementTypes.Form:
                    {
                        Dictionary<string, object> dic = PublicMethods.fromJSON(formElement.Info);

                        Guid formId = Guid.Empty;
                        if (!dic.ContainsKey("FormID") || map == null || !map.ContainsKey("sub") ||
                            map["sub"].GetType() != typeof(Dictionary<string, object>) ||
                            !Guid.TryParse(dic["FormID"].ToString(), out formId)) return null;

                        List<FormElement> elements = FGController.get_form_elements(applicationId, formId);

                        if (elements == null || elements.Count == 0) return null;

                        Dictionary<string, object> subMap = (Dictionary<string, object>)map["sub"];

                        foreach (XmlNode node in nodeList)
                        {
                            Guid formInstanceId = Guid.NewGuid();

                            List<FormElement> subElements = new List<FormElement>();

                            foreach (string name in subMap.Keys)
                            {
                                List<FormElement> newElements = _parse_imported_form(applicationId, node,
                                    get_child_nodes(node, name, nsmgr), nsmgr, subMap[name], elements, subMap);

                                if (newElements != null && newElements.Count > 0) subElements.AddRange(newElements);
                            }

                            if (subElements.Count == 0) continue;

                            //add [national_id], [first_name], [last_name] & [full_name] elements that are empty
                            List<string> strAll = new List<string>();
                            strAll.AddRange(StrNationalID);
                            strAll.AddRange(StrFirstName);
                            strAll.AddRange(StrLastName);
                            strAll.AddRange(StrFullName);

                            foreach (FormElement e in elements.Where(u => u.Type == FormElementTypes.Text &&
                                field_name_match(u.Name, strAll) && !subElements.Any(x => x.RefElementID == u.ElementID)))
                            {
                                subElements.Add(new FormElement()
                                {
                                    ElementID = Guid.NewGuid(),
                                    RefElementID = e.ElementID,
                                    FormInstanceID = formInstanceId,
                                    Type = e.Type,
                                    SequenceNumber = e.SequenceNumber,
                                    Name = e.Name
                                });
                            }
                            //end of add [national_id], [first_name], [last_name] & [full_name] elements that are empty

                            retElement.TableContent.Add(new FormType()
                            {
                                FormID = formId,
                                InstanceID = formInstanceId,
                                OwnerID = retElement.ElementID,
                                Elements = subElements
                            });
                        }

                        return retElement.TableContent.Count > 0 ? retElement : null;
                    }
                default:
                    return null;
            }
        }

        private static DateTime? extract_date_from_parts(int year, string dateFieldName, XmlNode parentNode,
            XmlNamespaceManager nsmgr, Dictionary<string, object> map) {
            if (string.IsNullOrEmpty(dateFieldName)) return null;
            dateFieldName = dateFieldName.ToLower();

            string monthFieldName = dateFieldName + "_month";
            string dayFieldName = dateFieldName + "_day";

            int? month = null;
            int? day = null;

            foreach (string name in map.Keys)
            {
                string target = map[name].GetType() == typeof(string) ? (string)map[name] : (
                    map[name].GetType() != typeof(Dictionary<string, object>) ? null : (
                        !((Dictionary<string, object>)map[name]).ContainsKey("target") ? null :
                            ((Dictionary<string, object>)map[name])["target"].ToString()
                    )
                );

                if (string.IsNullOrEmpty(target) ||
                    (target.ToLower() != monthFieldName && target.ToLower() != dayFieldName)) continue;

                XmlNodeList lst = get_child_nodes(parentNode, name, nsmgr);
                int val = 0;

                if (lst == null || lst.Count == 0 || !int.TryParse(lst.Item(0).InnerText, out val)) continue;

                if (target.ToLower() == monthFieldName) month = val;
                else day = val;
            }

            return !month.HasValue || !day.HasValue ? null :
                PublicMethods.persian_to_gregorian_date(year, month.Value, day.Value, null, null, null);
        }
    }
}
