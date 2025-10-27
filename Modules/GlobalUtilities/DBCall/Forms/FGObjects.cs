using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using System.Text.RegularExpressions;

namespace RaaiVan.Modules.FormGenerator
{
    public enum FormElementTypes
    {
        Binary,
        Checkbox,
        Date,
        File,
        Form,
        MultiLevel,
        Node,
        Numeric,
        Select,
        Separator,
        Text,
        User
    }

    public enum PollOwnerType {
        None,
        WorkFlow
    }

    public static class FGUtilities
    {
        public static List<FormElement> get_form_elements(Guid? applicationId, string strElements, bool formDesignMode = false)
        {
            try
            {
                List<FormElement> retList = new List<FormElement>();

                if (string.IsNullOrEmpty(strElements)) return retList;

                Dictionary<string, object> dic = PublicMethods.fromJSON(Base64.decode(strElements));

                if (!dic.ContainsKey("Elements") || dic["Elements"].GetType() != typeof(ArrayList)) return retList;

                foreach (Dictionary<string, object> elem in (ArrayList)dic["Elements"])
                {
                    FormElement newElement = new FormElement()
                    {
                        ElementID = PublicMethods.parse_guid(PublicMethods.get_dic_value(elem, "ElementID")),
                        RefElementID = PublicMethods.parse_guid(PublicMethods.get_dic_value(elem, "RefElementID")),
                        FormInstanceID = PublicMethods.parse_guid(PublicMethods.get_dic_value(elem, "InstanceID")),
                        Title = PublicMethods.parse_string(PublicMethods.get_dic_value(elem, "Title"), defaultValue: null),
                        Name = formDesignMode ? Base64.decode(PublicMethods.get_dic_value(elem, "Name", defaultValue: null)) :
                            PublicMethods.get_dic_value(elem, "Name", defaultValue: null),
                        Filled = PublicMethods.parse_bool(PublicMethods.get_dic_value(elem, "Filled"), defaultValue: false),
                        SequenceNumber = PublicMethods.parse_int(PublicMethods.get_dic_value(elem, "SequenceNumber"), defaultValue: -1),
                        Necessary = PublicMethods.parse_bool(PublicMethods.get_dic_value(elem, "Necessary")),
                        UniqueValue = PublicMethods.parse_bool(PublicMethods.get_dic_value(elem, "UniqueValue")),
                        Help = PublicMethods.parse_string(PublicMethods.get_dic_value(elem, "Help"), defaultValue: null),
                        Info = PublicMethods.parse_string(PublicMethods.get_dic_value(elem, "Info"), defaultValue: null),
                        Weight = PublicMethods.parse_double(PublicMethods.get_dic_value(elem, "Weight")),
                        TextValue = PublicMethods.parse_string(PublicMethods.get_dic_value(elem, "TextValue"), defaultValue: null),
                        FloatValue = PublicMethods.parse_double(PublicMethods.get_dic_value(elem, "FloatValue")),
                        BitValue = PublicMethods.parse_bool(PublicMethods.get_dic_value(elem, "BitValue")),
                        DateValue = PublicMethods.parse_date(PublicMethods.get_dic_value(elem, "DateValue")),
                        AttachedFiles = DocumentUtilities.get_files_info(applicationId, PublicMethods.get_dic_value(elem, "Files"))
                    };

                    newElement.Type = PublicMethods.parse_enum<FormElementTypes>(PublicMethods.get_dic_value(elem, "Type"));

                    if (newElement.Type == FormElementTypes.Separator) newElement.Necessary = null;
                    if (newElement.Type != FormElementTypes.Text && newElement.Type != FormElementTypes.Numeric)
                        newElement.UniqueValue = null;

                    if (elem.ContainsKey("GuidItems") && elem["GuidItems"] != null && elem["GuidItems"].GetType() == typeof(ArrayList))
                    {
                        SelectedGuidItemType tp = SelectedGuidItemType.None;
                        if (newElement.Type == FormElementTypes.Node || newElement.Type == FormElementTypes.MultiLevel)
                            tp = SelectedGuidItemType.Node;
                        else if (newElement.Type == FormElementTypes.User) tp = SelectedGuidItemType.User;

                        foreach (Dictionary<string, object> d in (ArrayList)elem["GuidItems"])
                            newElement.GuidItems.Add(new SelectedGuidItem(Guid.Parse((string)d["ID"]), code: string.Empty,
                                name: Base64.decode((string)d["Name"]), type: tp));
                    }

                    if (newElement.ElementID.HasValue && !formDesignMode && (!newElement.Filled.HasValue || !newElement.Filled.Value))
                    {
                        newElement.RefElementID = newElement.ElementID;
                        newElement.ElementID = Guid.NewGuid();
                    }

                    if (newElement.AttachedFiles != null) newElement.AttachedFiles.ForEach(f =>
                    {
                        f.OwnerID = newElement.ElementID;
                        f.OwnerType = FileOwnerTypes.FormElement;
                    });

                    //Modify TextValue
                    if (string.IsNullOrEmpty(newElement.TextValue) && newElement.Type.HasValue)
                    {
                        switch (newElement.Type.Value)
                        {
                            case FormElementTypes.Numeric:
                                if (newElement.FloatValue.HasValue) newElement.TextValue = newElement.FloatValue.ToString();
                                break;
                            case FormElementTypes.Date:
                                if (newElement.DateValue.HasValue)
                                    newElement.TextValue = GenericDate.get_local_date(newElement.DateValue.Value);
                                break;
                            case FormElementTypes.MultiLevel:
                            case FormElementTypes.User:
                            case FormElementTypes.Node:
                                if (newElement.GuidItems != null)
                                    newElement.TextValue = string.Join(" ~ ", 
                                        newElement.GuidItems.Select(gi => gi.toMarkup()).Where(gi => !string.IsNullOrEmpty(gi)));
                                break;
                        }
                    }
                    //end of Modify TextValue

                    retList.Add(newElement);
                }

                return retList;
            }
            catch { return new List<FormElement>(); }
        }

        public static bool set_element_value(string value, ref FormElement element)
        {
            if (string.IsNullOrEmpty(value)) value = string.Empty;
            FormElementTypes type = element.Type.HasValue ? element.Type.Value : FormElementTypes.Text;

            switch (type)
            {
                case FormElementTypes.Text:
                case FormElementTypes.Select:
                case FormElementTypes.Checkbox:
                    element.TextValue = value;
                    return !string.IsNullOrEmpty(element.TextValue);
                case FormElementTypes.Binary:
                    element.BitValue = value.ToLower() == "true";
                    return true;
                case FormElementTypes.Date:
                    if (value.ToLower() == "now")
                        element.DateValue = DateTime.Now;
                    else
                    {
                        DateTime dt = new DateTime();
                        if (DateTime.TryParse(value, out dt)) element.DateValue = dt;
                    }
                    return element.DateValue.HasValue;
                case FormElementTypes.Node:
                case FormElementTypes.User:
                    SelectedGuidItemType tp = type == FormElementTypes.Node ? SelectedGuidItemType.Node : SelectedGuidItemType.User;
                    Guid id = Guid.Empty;
                    if (Guid.TryParse(value, out id) && id != Guid.Empty)
                        element.GuidItems.Add(new SelectedGuidItem(id, string.Empty, string.Empty, tp));
                    return id != Guid.Empty;
                case FormElementTypes.Numeric:
                    float flt = 0;
                    if (float.TryParse(value, out flt)) element.FloatValue = flt;
                    return element.FloatValue.HasValue;
            }

            return false;
        }

        public static List<FormFilter> get_filters_from_json(Dictionary<string, object> dic,
            Guid? ownerId = null, Guid? formId = null, Guid? applicationId = null)
        {
            List<FormFilter> retFilters = new List<FormFilter>();

            Dictionary<string, Guid> namesDic = new Dictionary<string, Guid>();

            if (formId.HasValue && applicationId.HasValue)
            {
                Guid tempId = Guid.Empty;
                List<String> fieldNames = dic.Keys.Where(i => !Guid.TryParse(i, out tempId)).ToList();
                namesDic = FGController.get_form_element_ids(applicationId.Value, formId.Value, fieldNames);
            }
            
            foreach (string id in dic.Keys)
            {
                try
                {
                    Guid elementId = namesDic.ContainsKey(id.ToLower()) ? namesDic[id.ToLower()] : Guid.Empty;

                    if ((elementId == Guid.Empty && !Guid.TryParse(id, out elementId)) ||
                        (dic[id] != null && dic[id].GetType() != typeof(Dictionary<string, object>))) continue;

                    Dictionary<string, object> elemDic = (Dictionary<string, object>)dic[id];

                    FormElementTypes type = FormElementTypes.Text;
                    //if (!Enum.TryParse<FormElementTypes>(elemDic.ContainsKey("Type") ?
                    //    elemDic["Type"].ToString() : string.Empty, out type)) continue;
                    if (!Enum.TryParse<FormElementTypes>(PublicMethods.get_dic_value(elemDic, "Type", string.Empty), out type))
                        type = FormElementTypes.Text;
                    
                    if (type == FormElementTypes.Form)
                    {
                        List<FormFilter> subFilters = get_filters_from_json(elemDic, elementId);

                        if (subFilters != null && subFilters.Count > 0)
                        {
                            retFilters.Add(new FormFilter() { ElementID = elementId, OwnerID = ownerId }); //just to say that this field has filters
                            retFilters.AddRange(subFilters);
                        }

                        continue;
                    }

                    FormFilter filter = new FormFilter() { ElementID = elementId, OwnerID = ownerId };

                    filter.Exact = PublicMethods.parse_bool(PublicMethods.get_dic_value(elemDic, "Exact"));
                    filter.Or = PublicMethods.parse_bool(PublicMethods.get_dic_value(elemDic, "Or"));
                    filter.Bit = PublicMethods.parse_bool(PublicMethods.get_dic_value(elemDic, "Bit"));
                    filter.FloatFrom = PublicMethods.parse_double(PublicMethods.get_dic_value(elemDic, "FloatFrom"));
                    filter.FloatTo = PublicMethods.parse_double(PublicMethods.get_dic_value(elemDic, "FloatTo"));
                    filter.DateFrom = PublicMethods.parse_date(PublicMethods.get_dic_value(elemDic, "DateFrom"));
                    filter.DateTo = PublicMethods.parse_date(PublicMethods.get_dic_value(elemDic, "DateTo"), 1);
                    filter.Compulsory = PublicMethods.parse_bool(PublicMethods.get_dic_value(elemDic, "Compulsory"));

                    if (elemDic.ContainsKey("TextItems") && elemDic["TextItems"] != null && 
                        elemDic["TextItems"].GetType() == typeof(ArrayList))
                    {
                        ArrayList lst = (ArrayList)elemDic["TextItems"];
                        foreach (object o in lst)
                            if (o != null) filter.TextItems.Add(Base64.decode(o.ToString()));
                    }

                    if (elemDic.ContainsKey("GuidItems") && elemDic["GuidItems"] != null && 
                        elemDic["GuidItems"].GetType() == typeof(ArrayList))
                    {
                        ArrayList lst = (ArrayList)elemDic["GuidItems"];
                        foreach (object o in lst)
                        {
                            Guid val = Guid.Empty;
                            if (Guid.TryParse(o.ToString(), out val)) filter.GuidItems.Add(val);
                        }
                    }

                    retFilters.Add(filter);
                }
                catch { }
            }

            return retFilters;
        }

        public static List<FormFilter> get_filters_from_json(string jsonInput,
            Guid? ownerId = null, Guid? formId = null, Guid? applicationId = null) {
            return get_filters_from_json(PublicMethods.fromJSON(jsonInput), ownerId, formId, applicationId);
        }

        public static bool is_valid_name(string formOrElementName) {
            try
            {
                return string.IsNullOrEmpty(formOrElementName) || 
                    new Regex("^[a-zA-Z0-9_]{1,100}$").IsMatch(formOrElementName);
            }
            catch { return false; }
        }
    }

    public enum SelectedGuidItemType
    {
        None,
        Node,
        User
    }

    public class SelectedGuidItem
    {
        public Guid? ID;
        public string Code;
        public string Name;
        public SelectedGuidItemType Type;

        public SelectedGuidItem()
        {
            Type = SelectedGuidItemType.None;
        }

        public SelectedGuidItem(Guid id, string code, string name, SelectedGuidItemType type)
        {
            ID = id;
            Code = code;
            Name = name;
            Type = type;
        }

        public string toMarkup() {
            if (!ID.HasValue || string.IsNullOrEmpty(Name)) return string.Empty;

            switch (Type) {
                case SelectedGuidItemType.Node:
                    return Expressions.get_markup(ID.ToString(), "Node", Name);
                case SelectedGuidItemType.User:
                    return Expressions.get_markup(ID.ToString(), "User", Name);
            }

            return string.Empty;
        }

        public string toJson(Guid? applicationId = null, bool icon = false)
        {
            string iconUrl = string.Empty;

            if (icon && ID.HasValue && applicationId.HasValue) {
                switch (Type) {
                    case SelectedGuidItemType.User:
                        iconUrl = DocumentUtilities.get_personal_image_address(applicationId, ID);
                        break;
                    case SelectedGuidItemType.Node:
                        iconUrl = DocumentUtilities.get_icon_url(applicationId.Value, ownerId: ID.Value, defaultIcon: DefaultIconTypes.Node);
                        break;
                }
            }

            return "{\"ID\":\"" + (ID.HasValue ? ID.ToString() : string.Empty) + "\"" +
                ",\"Code\":\"" + Base64.encode(Code) + "\"" +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                (string.IsNullOrEmpty(iconUrl) ? string.Empty : ",\"IconURL\":\"" + iconUrl + "\"") +
                (Type == SelectedGuidItemType.Node ? ",\"NodeID\":\"" + (ID.HasValue ? ID.ToString() : string.Empty) + "\"" : string.Empty) +
                (Type == SelectedGuidItemType.Node ? ",\"AdditionalID\":\"" + Base64.encode(Code) + "\"" : string.Empty) +
                (Type == SelectedGuidItemType.User ? ",\"UserID\":\"" + (ID.HasValue ? ID.ToString() : string.Empty) + "\"" : string.Empty) +
                (Type == SelectedGuidItemType.User ? ",\"UserName\":\"" + Base64.encode(Code) + "\"" : string.Empty) +
                (Type == SelectedGuidItemType.User ? ",\"FullName\":\"" + Base64.encode(Name) + "\"" : string.Empty) +
                "}";
        }
    }

    public class FormElement : ICloneable
    {
        public Guid? ElementID;
        public Guid? FormID;
        public Guid? FormInstanceID;
        public Guid? RefElementID;
        public Guid? TemplateElementID;
        public long? ChangeID;
        public string Title;
        public string Name;
        public string Help;
        public bool? Necessary;
        public bool? UniqueValue;
        public int? SequenceNumber;
        public FormElementTypes? Type;
        public string Info;
        public double? Weight;
        public string InitialValue;
        public string TextValue;
        public double? FloatValue;
        public bool? BitValue;
        public DateTime? DateValue;
        public List<SelectedGuidItem> GuidItems;
        public bool? Filled;
        public User Creator;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public List<DocFileInfo> AttachedFiles;
        public List<FormElement> TableColumns;
        public List<FormType> TableContent;
        public int? EditionsCount;
        
        public FormElement()
        {
            GuidItems = new List<SelectedGuidItem>();
            Creator = new User();
            AttachedFiles = new List<DocFileInfo>();
            TableColumns = new List<FormElement>();
            TableContent = new List<FormType>();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public bool is_empty()
        {
            return string.IsNullOrEmpty(TextValue) && !BitValue.HasValue && !FloatValue.HasValue &&
                !DateValue.HasValue && (GuidItems == null || GuidItems.Count == 0) &&
                (AttachedFiles == null || AttachedFiles.Count == 0);
        }

        public Guid? GetRefElementID() {
            return RefElementID.HasValue ? RefElementID : ElementID;
        }

        public string toString(Guid applicationId)
        {
            string ret = string.Empty;

            switch (Type) {
                case FormElementTypes.Text:
                case FormElementTypes.Select:
                case FormElementTypes.Checkbox:
                case FormElementTypes.MultiLevel:
                case FormElementTypes.Node:
                case FormElementTypes.User:
                case FormElementTypes.Binary:
                    ret = TextValue;
                    break;
                case FormElementTypes.Date:
                    ret = GenericDate.get_local_date(DateValue);
                    break;
                case FormElementTypes.Numeric:
                    ret = FloatValue.ToString();
                    break;
                case FormElementTypes.File:
                case FormElementTypes.Form:
                case FormElementTypes.Separator:
                    break;
            }

            return PublicMethods.markup2plaintext(applicationId, ret);
        }

        public string toJson(Guid applicationId, bool persianDate = false, bool includeRefElementId = false, bool icon = false)
        {
            //Modify TextValue
            if (!string.IsNullOrEmpty(TextValue) && Type.HasValue)
            {
                switch (Type.Value)
                {
                    case FormElementTypes.Numeric:
                        if (!FloatValue.HasValue) FloatValue = PublicMethods.parse_double(TextValue);
                        break;
                    case FormElementTypes.MultiLevel:
                    case FormElementTypes.User:
                    case FormElementTypes.Node:
                        if (GuidItems == null) GuidItems = new List<SelectedGuidItem>();

                        if (GuidItems.Count == 0) {
                            GuidItems = Expressions.get_tagged_items(TextValue).Select(tg =>
                            {
                                SelectedGuidItemType tp = PublicMethods.parse_enum(tg.Type.ToString(), 
                                    defaultValue: SelectedGuidItemType.None);

                                return !tg.ID.HasValue || string.IsNullOrEmpty(tg.Value) || tp == SelectedGuidItemType.None ? null : 
                                    new SelectedGuidItem(tg.ID.Value, code: null, name: tg.Value, tp);
                            }).Where(tg => tg != null).ToList();
                        }

                        break;
                }
            }
            //end of Modify TextValue

            string strElementId = string.Empty;

            if (includeRefElementId)
                strElementId = ElementID.HasValue ? ElementID.Value.ToString() : string.Empty;
            else
                strElementId = (RefElementID.HasValue ? RefElementID : ElementID).ToString();

            string strGuidItems = string.Join(",", GuidItems.Select(u => u.toJson(applicationId, icon: icon)));

            return "{\"ElementID\":\"" + strElementId + "\"" +
                ",\"InstanceID\":\"" + (FormInstanceID.HasValue ? FormInstanceID.ToString() : string.Empty) + "\"" +
                (!includeRefElementId ? string.Empty :
                    ",\"RefElementID\":\"" + (RefElementID.HasValue ? RefElementID.ToString() : string.Empty) + "\""
                ) +
                (!TemplateElementID.HasValue ? string.Empty : ",\"TemplateElementID\":\"" + TemplateElementID.Value.ToString() + "\"") +
                ",\"FormID\":\"" + (FormID.HasValue ? FormID.ToString() : string.Empty) + "\"" +
                (!ChangeID.HasValue ? string.Empty : ",\"ChangeID\":\"" + ChangeID.Value.ToString() + "\"") +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                ",\"Help\":\"" + Base64.encode(Help) + "\"" +
                ",\"Type\":\"" + (Type.HasValue ? Type.ToString() : FormElementTypes.Text.ToString()) + "\"" +
                ",\"Necessary\":" + (Necessary.HasValue && Necessary.Value).ToString().ToLower() +
                ",\"UniqueValue\":" + (UniqueValue.HasValue && UniqueValue.Value).ToString().ToLower() +
                ",\"SequenceNumber\":" + (SequenceNumber.HasValue ? SequenceNumber.Value : 0).ToString() +
                ",\"Info\":\"" + Base64.encode(Info) + "\"" +
                (!Weight.HasValue ? string.Empty : ",\"Weight\":" + Weight.ToString()) +
                (string.IsNullOrEmpty(InitialValue) ? string.Empty : ",\"InitialValue\":\"" + Base64.encode(InitialValue) + "\"") +
                ",\"TextValue\":\"" + Base64.encode(TextValue) + "\"" +
                ",\"DateValue\":" + (!DateValue.HasValue ? "null" : "\"" + (persianDate ?
                    GenericDate.get_local_date(DateValue.Value) : DateValue.ToString()) + "\"") +
                ",\"DateValue_Jalali\":" + (!DateValue.HasValue ? "null" : "\"" +
                    GenericDate.get_local_date(DateValue.Value) + "\"") +
                ",\"FloatValue\":" + (!FloatValue.HasValue ? "null" : FloatValue.Value.ToString()) +
                ",\"BitValue\":" + (!BitValue.HasValue ? "null" : BitValue.Value.ToString().ToLower()) +
                ",\"GuidItems\":[" + strGuidItems + "]" +
                ",\"SelectedItems\":[" + strGuidItems + "]" +
                (TableColumns == null || TableColumns.Count == 0 ? string.Empty :
                    ",\"TableColumns\":[" + string.Join(",", TableColumns.Select(c => c.toJson(applicationId))) + "]"
                ) +
                (TableContent == null || TableContent.Count == 0 ? string.Empty : 
                    ",\"TableContent\":[" + string.Join(",", TableContent.Select(t => 
                        t.toJson(applicationId, persianDate: persianDate, includeRefElementId: includeRefElementId, icon: icon))) + "]"
                ) +
                ",\"Filled\":" + (Filled.HasValue && Filled.Value).ToString().ToLower() +
                (!Creator.UserID.HasValue || string.IsNullOrEmpty(Creator.UserName) ? string.Empty :
                    ",\"Creator\":{\"UserID\":\"" + Creator.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(Creator.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(Creator.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(Creator.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(applicationId,
                            Creator.UserID.Value) + "\"" +
                    "}"
                ) +
                ",\"CreationDate\":\"" + (CreationDate.HasValue ?
                    GenericDate.get_local_date(CreationDate.Value, true) : string.Empty) + "\"" +
                ",\"LastModificationDate\":\"" + (LastModificationDate.HasValue ?
                    GenericDate.get_local_date(LastModificationDate.Value, true) : string.Empty) + "\"" +
                (AttachedFiles == null || AttachedFiles.Count == 0 ? string.Empty :
                    ",\"Files\":" + DocumentUtilities.get_files_json(AttachedFiles, true)
                ) +
                ",\"EditionsCount\":" + (!EditionsCount.HasValue ? 0 : EditionsCount.Value).ToString() +
                "}";
        }
    }
    
    public class FormType : ICloneable
    {
        public Guid? FormID;
        public Guid? TemplateFormID;
        public Guid? InstanceID;
        public Guid? OwnerID;
        public Guid? DirectorID;
        public bool? Admin;
        public string Title;
        public string Name;
        public string Description;
        public User Creator;
        public bool? Filled;
        public bool?IsTemporary;
        public DateTime? FillingDate;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public List<FormElement> Elements;

        public FormType()
        {
            Elements = new List<FormElement>();
            Creator = new User();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string toJson(Guid applicationId, bool persianDate = false,
            bool includeRefElementId = false, bool icon = false, bool creator = false)
        {
            return "{\"InstanceID\":\"" + (!InstanceID.HasValue ? string.Empty : InstanceID.Value.ToString()) + "\"" +
                ",\"FormID\":\"" + (!FormID.HasValue ? string.Empty : FormID.Value.ToString()) + "\"" +
                (!TemplateFormID.HasValue ? string.Empty :
                    ",\"TemplateFormID\":\"" + TemplateFormID.Value.ToString() + "\"") +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"OwnerID\":\"" + (OwnerID.HasValue ? OwnerID.Value.ToString() : string.Empty) + "\"" +
                ",\"DirectorID\":\"" + (DirectorID.HasValue ? DirectorID.Value.ToString() : string.Empty) + "\"" +
                ",\"IsTemporary\":" + (IsTemporary.HasValue && IsTemporary.Value).ToString().ToLower() +
                ",\"CreationDate\":\"" + (CreationDate.HasValue ? CreationDate.Value.ToString() : string.Empty) + "\"" +
                ",\"CreationDate_Jalali\":\"" + (!CreationDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(CreationDate.Value)) + "\"" +
                (!Filled.HasValue || !Filled.Value ? string.Empty : 
                    ",\"Filled\":" + true.ToString().ToLower()) +
                (!FillingDate.HasValue ? string.Empty : 
                    ",\"FillingDate\":\"" + GenericDate.get_local_date(FillingDate.Value, true) + "\"") +
                (!creator || !Creator.UserID.HasValue ? string.Empty :
                    ",\"Creator\":" + Creator.toJson(applicationId, profileImageUrl: true)) + 
                (Elements == null || Elements.Count == 0 ? string.Empty :
                    ",\"Elements\":[" + string.Join(",", Elements.Select(e =>
                        e.toJson(applicationId, persianDate: persianDate, includeRefElementId: includeRefElementId, icon: icon))) + "]"
                ) +
                "}";
        }
    }

    public class FormFilter
    {
        public Guid? OwnerID;
        public Guid? ElementID;
        public string Text;
        public List<string> TextItems;
        public bool? Or;
        public bool? Exact;
        public DateTime? DateFrom;
        public DateTime? DateTo;
        public double? FloatFrom;
        public double? FloatTo;
        public bool? Bit;
        public Guid? Guid;
        public List<Guid> GuidItems;
        public bool? Compulsory;

        public FormFilter()
        {
            TextItems = new List<string>();
            GuidItems = new List<System.Guid>();
        }
    }

    public class RecordCell
    {
        public Guid? ElementID;
        public string Value;

        public RecordCell(Guid elementId, string value)
        {
            ElementID = elementId;
            Value = value;
        }
    }

    public class FormRecord
    {
        public Guid? InstanceID;
        public Guid? OwnerID;
        public DateTime? CreationDate;
        public List<RecordCell> Cells;

        public FormRecord()
        {
            Cells = new List<RecordCell>();
        }

        public string toJSON() {
            return "{\"InstanceID\":\"" + (InstanceID.HasValue ? InstanceID.Value.ToString() : string.Empty) + "\"" +
                ",\"OwnerID\":\"" + (OwnerID.HasValue ? OwnerID.Value.ToString() : string.Empty) + "\"" +
                ",\"CreationDate\":\"" + (CreationDate.HasValue ? CreationDate.Value.ToString() : string.Empty) + "\"" +
                ",\"CreationDate_Jalali\":\"" + (CreationDate.HasValue ?
                    GenericDate.get_local_date(CreationDate.Value) : string.Empty) + "\"" +
                "," + string.Join(",", Cells.Where(x => x.ElementID.HasValue)
                    .Select(u => "\"" + u.ElementID.ToString() + "\":\"" + Base64.encode(u.Value) + "\"")) + "}";
        }
    }

    public class FormStatistics {
        public double? WeightSum;
        public double? Sum;
        public double? WeightedSum;
        public double? Average;
        public double? WeightedAverage;
        public double? Minimum;
        public double? Maximum;
        public double? Variance;
        public double? StandardDeviation;

        public string toJson()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (WeightSum.HasValue) dic["WeightSum"] = WeightSum;
            if (Sum.HasValue) dic["Sum"] = Sum;
            if (WeightedSum.HasValue) dic["WeightedSum"] = WeightedSum;
            if (Average.HasValue) dic["Avg"] = Average;
            if (WeightedAverage.HasValue) dic["WeightedAvg"] = WeightedAverage;
            if (Minimum.HasValue) dic["Min"] = Minimum;
            if (Maximum.HasValue) dic["Max"] = Maximum;
            if (Variance.HasValue) dic["Var"] = Variance;
            if (StandardDeviation.HasValue) dic["StDev"] = StandardDeviation;

            return PublicMethods.toJSON(dic);
        }
    }

    public class Poll
    {
        public Guid? PollID;
        public Guid? IsCopyOfPollID;
        public Guid? OwnerID;
        public string Name;
        public string RefName;
        public string Description;
        public string RefDescription;
        public DateTime? BeginDate;
        public DateTime? FinishDate;
        public bool? ShowSummary;
        public bool? HideContributors;
        public bool? Archived;

        public string toJson(bool? done = null)
        {
            return "{\"PollID\":\"" + (!PollID.HasValue ? string.Empty : PollID.ToString()) + "\"" +
                (!IsCopyOfPollID.HasValue ? string.Empty :
                    ",\"IsCopyOfPollID\":\"" + IsCopyOfPollID.ToString() + "\"") +
                (!OwnerID.HasValue ? string.Empty : ",\"OwnerID\":\"" + OwnerID.ToString() + "\"") +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                ",\"RefName\":\"" + Base64.encode(RefName) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"RefDescription\":\"" + Base64.encode(RefDescription) + "\"" +
                ",\"BeginDate\":\"" + (!BeginDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(BeginDate.Value)) + "\"" +
                ",\"FinishDate\":\"" + (!FinishDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(FinishDate.Value)) + "\"" +
                ",\"ShowSummary\":" + (ShowSummary.HasValue && ShowSummary.Value).ToString().ToLower() +
                (!done.HasValue ? string.Empty : ",\"Done\":" + done.Value.ToString().ToLower()) +
                ",\"HideContributors\":" + (HideContributors.HasValue && HideContributors.Value).ToString().ToLower() +
                "}";
        }
    }

    public class PollAbstractValue
    {
        public string TextValue;
        public double? NumberValue;
        public bool? BitValue;
        public int? Count;

        public bool hasValue()
        {
            return !string.IsNullOrEmpty(TextValue) || NumberValue.HasValue || BitValue.HasValue;
        }

        public string toString()
        {
            if (!string.IsNullOrEmpty(TextValue)) return TextValue;
            else if (NumberValue.HasValue) return NumberValue.Value.ToString();
            else if (BitValue.HasValue) return BitValue.Value.ToString().ToLower();
            else return string.Empty;
        }

        public string toJSONString()
        {
            if (!string.IsNullOrEmpty(TextValue)) return "\"" + Base64.encode(TextValue) + "\"";
            else if (NumberValue.HasValue) return NumberValue.Value.ToString();
            else if (BitValue.HasValue) return BitValue.Value.ToString().ToLower();
            else return "\"\"";
        }
    }

    public class PollAbstract
    {
        public int? TotalCount;
        public Guid? ElementID;
        public List<PollAbstractValue> Values;
        public FormStatistics Stats;

        public PollAbstract() {
            Values = new List<PollAbstractValue>();
            Stats = new FormStatistics();
        }

        public string toJson()
        {
            return "{\"DistinctValuesCount\":" + (!TotalCount.HasValue ? 0 : TotalCount.Value).ToString() +
                ",\"ElementID\":\"" + (!ElementID.HasValue ? string.Empty : ElementID.ToString()) + "\"" +
                (!Stats.Minimum.HasValue ? string.Empty : ",\"Min\":" + Stats.Minimum.Value.ToString()) +
                (!Stats.Maximum.HasValue ? string.Empty : ",\"Max\":" + Stats.Maximum.Value.ToString()) +
                (!Stats.Average.HasValue ? string.Empty : ",\"Avg\":" + Stats.Average.Value.ToString()) +
                (!Stats.Variance.HasValue ? string.Empty : ",\"Var\":" + Stats.Variance.Value.ToString()) +
                (!Stats.StandardDeviation.HasValue ? string.Empty : ",\"StDev\":" + Stats.StandardDeviation.Value.ToString()) +
                ",\"Values\":[" + string.Join(",", Values.Where(u => u.hasValue()).Select(
                    x => "{\"Value\":" + x.toJSONString() +
                    ",\"Count\":" + (!x.Count.HasValue ? 0 : x.Count).ToString() + "}")) +
                "]}";
        }
    }

    public class TemplateStatus
    {
        public Guid? TemplateID;
        public string TemplateName;
        public Guid? ActivatedID;
        public string ActivatedName;
        public DateTime? ActivationDate;
        public User Activator;
        public int? TemplateElementsCount;
        public int? ElementsCount;
        public int? NewTemplateElementsCount;
        public int? RemovedTemplateElementsCount;
        public int? NewCustomElementsCount;

        public TemplateStatus()
        {
            Activator = new User();
        }

        public string toJson(Guid applicationId) {
            return "{\"TemplateID\":\"" + (!TemplateID.HasValue ? string.Empty : TemplateID.ToString()) + "\"" +
                ",\"TemplateName\":\"" + Base64.encode(TemplateName) + "\"" +
                ",\"ActivatedID\":\"" + (!ActivatedID.HasValue ? string.Empty : ActivatedID.ToString()) + "\"" +
                ",\"ActivatedName\":\"" + Base64.encode(ActivatedName) + "\"" +
                ",\"ActivationDate\":\"" + (!ActivationDate.HasValue ? string.Empty : ActivationDate.Value.ToString("yyyy-MM-dd")) + "\"" +
                ",\"ActivationDate_Jalali\":\"" + GenericDate.get_local_date(ActivationDate) + "\"" +
                ",\"Activator\":" + Activator.toJson(applicationId, profileImageUrl: true) +
                ",\"TemplateElementsCount\":" + (!TemplateElementsCount.HasValue ? 0 : TemplateElementsCount.Value).ToString() +
                ",\"ElementsCount\":" + (!ElementsCount.HasValue ? 0 : ElementsCount.Value).ToString() +
                ",\"NewTemplateElementsCount\":" + (!NewTemplateElementsCount.HasValue ? 0 : NewTemplateElementsCount.Value).ToString() +
                ",\"RemovedTemplateElementsCount\":" + (!RemovedTemplateElementsCount.HasValue ? 0 : RemovedTemplateElementsCount.Value).ToString() +
                ",\"NewCustomElementsCount\":" + (!NewCustomElementsCount.HasValue ? 0 : NewCustomElementsCount.Value).ToString() +
                "}";
        }
    }
}
