using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.FormGenerator
{
    public static class FGParsers
    {
        public static List<FormType> form_types(DBResultSet results)
        {
            List<FormType> retList = new List<FormType>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new FormType()
                {
                    FormID = table.GetGuid(i, "FormID"),
                    Title = table.GetString(i, "Title"),
                    Name = table.GetString(i, "Name"),
                    Description = table.GetString(i, "Description")
                });
            }

            return retList;
        }

        public static List<FormElement> form_elements(DBResultSet results)
        {
            List<FormElement> retList = new List<FormElement>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                FormElement formElement = new FormElement() {
                    ElementID = table.GetGuid(i, "ElementID"),
                    FormID = table.GetGuid(i, "FormID"),
                    Title = table.GetString(i, "Title"),
                    Name = table.GetString(i, "Name"),
                    Help = table.GetString(i, "Help"),
                    Necessary = table.GetBool(i, "Necessary"),
                    UniqueValue = table.GetBool(i, "UniqueValue"),
                    SequenceNumber = table.GetInt(i, "SequenceNumber"),
                    Type = null,
                    Info = table.GetString(i, "Info"),
                    Weight = table.GetDouble(i, "Weight")
                };

                FormElementTypes tp = FormElementTypes.Text;
                if (Enum.TryParse<FormElementTypes>(table.GetString(i, "Type"), out tp)) formElement.Type = tp;

                if (!string.IsNullOrEmpty(formElement.Info) && formElement.Info[0] != '{')
                {
                    formElement.Info = "{\"Options\":[" + ProviderUtil.list_to_string<string>(
                        ProviderUtil.get_tags_list(formElement.Info).Select(u => "\"" + Base64.encode(u) + "\"").ToList()) + "]}";
                }

                retList.Add(formElement);
            }

            return retList;
        }

        public static Dictionary<string, Guid> element_ids(DBResultSet results)
        {
            Dictionary<string, Guid> dic = new Dictionary<string, Guid>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string name = table.GetString(i, "Name");
                Guid? elementId = table.GetGuid(i, "ElementID");

                if (!string.IsNullOrEmpty(name) && elementId.HasValue) dic[name.ToLower()] = elementId.Value;
            }

            return dic;
        }

        public static List<FormElement> element_limits(DBResultSet results)
        {
            List<FormElement> retList = new List<FormElement>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                FormElement formElement = new FormElement()
                {
                    ElementID = table.GetGuid(i, "ElementID"),
                    Title = table.GetString(i, "Title"),
                    Necessary = table.GetBool(i, "Necessary"),
                    Type = null,
                    Info = table.GetString(i, "Info")
                };

                FormElementTypes tp = FormElementTypes.Text;
                if (Enum.TryParse<FormElementTypes>(table.GetString(i, "Type"), out tp)) formElement.Type = tp;

                if (!string.IsNullOrEmpty(formElement.Info) && formElement.Info[0] != '{')
                {
                    formElement.Info = "{\"Options\":[" + ProviderUtil.list_to_string<string>(
                        ProviderUtil.get_tags_list(formElement.Info).Select(u => "\"" + Base64.encode(u) + "\"").ToList()) + "]}";
                }

                retList.Add(formElement);
            }

            return retList;
        }

        public static List<FormType> form_instances(DBResultSet results)
        {
            List<FormType> retList = new List<FormType>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new FormType()
                {
                    InstanceID = table.GetGuid(i, "InstanceID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    FormID = table.GetGuid(i, "FormID"),
                    Title = table.GetString(i, "FormTitle"),
                    Description = table.GetString(i, "Description"),
                    Filled = table.GetBool(i, "Filled"),
                    FillingDate = table.GetDate(i, "FillingDate"),
                    Creator = new User()
                    {
                        UserID = table.GetGuid(i, "CreatorUserID"),
                        UserName = table.GetString(i, "CreatorUserName"),
                        FirstName = table.GetString(i, "CreatorFirstName"),
                        LastName = table.GetString(i, "CreatorLastName")
                    }
                });
            }

            return retList;
        }

        public static List<FormElement> form_instance_elements(DBResultSet results)
        {
            List<FormElement> retList = new List<FormElement>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                FormElement formElement = new FormElement()
                {
                    ElementID = table.GetGuid(i, "ElementID"),
                    FormInstanceID = table.GetGuid(i, "InstanceID"),
                    RefElementID = table.GetGuid(i, "RefElementID"),
                    Title = table.GetString(i, "Title"),
                    Name = table.GetString(i, "Name"),
                    Help = table.GetString(i, "Help"),
                    SequenceNumber = table.GetInt(i, "SequenceNumber"),
                    TextValue = table.GetString(i, "TextValue"),
                    FloatValue = table.GetDouble(i, "FloatValue"),
                    BitValue = table.GetBool(i, "BitValue"),
                    DateValue = table.GetDate(i, "DateValue"),
                    Necessary = table.GetBool(i, "Necessary"),
                    UniqueValue = table.GetBool(i, "UniqueValue"),
                    Type = null,
                    Info = table.GetString(i, "Info"),
                    Weight = table.GetDouble(i, "Weight"),
                    Filled = table.GetBool(i, "Filled"),
                    EditionsCount = table.GetInt(i, "EditionsCount"),
                    Creator = new User()
                    {
                        UserID = table.GetGuid(i, "CreatorUserID"),
                        UserName = table.GetString(i, "CreatorUserName"),
                        FirstName = table.GetString(i, "CreatorFirstName"),
                        LastName = table.GetString(i, "CreatorLastName")
                    }
                };

                FormElementTypes tp = FormElementTypes.Text;
                if (Enum.TryParse<FormElementTypes>(table.GetString(i, "Type"), out tp)) formElement.Type = tp;

                if (!string.IsNullOrEmpty(formElement.Info) && formElement.Info[0] != '{')
                {
                    formElement.Info = "{\"Options\":[" + ProviderUtil.list_to_string<string>(
                        ProviderUtil.get_tags_list(formElement.Info).Select(u => "\"" + u + "\"").ToList()) + "]}";
                }

                retList.Add(formElement);
            }

            return retList;
        }

        public static Dictionary<Guid, List<SelectedGuidItem>> selected_guids(DBResultSet results)
        {
            Dictionary<Guid, List<SelectedGuidItem>> ret = new Dictionary<Guid, List<SelectedGuidItem>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? elementId = table.GetGuid(i, "ElementID");
                Guid? id = table.GetGuid(i, "ID");
                string name = table.GetString(i, "Name");

                if (!elementId.HasValue || !id.HasValue) continue;

                if (!ret.ContainsKey(elementId.Value)) ret[elementId.Value] = new List<SelectedGuidItem>();

                ret[elementId.Value].Add(new SelectedGuidItem(id.Value, name: name, code: string.Empty, type: SelectedGuidItemType.None));
            }

            return ret;
        }

        public static List<FormElement> element_changes(DBResultSet results)
        {
            List<FormElement> retList = new List<FormElement>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new FormElement()
                {
                    ChangeID = table.GetLong(i, "ID"),
                    ElementID = table.GetGuid(i, "ElementID"),
                    Info = table.GetString(i, "Info"),
                    TextValue = table.GetString(i, "TextValue"),
                    FloatValue = table.GetDouble(i, "FloatValue"),
                    BitValue = table.GetBool(i, "BitValue"),
                    DateValue = table.GetDate(i, "DateValue"),
                    CreationDate = table.GetDate(i, "CreationDate"),
                    Creator = new User()
                    {
                        UserID = table.GetGuid(i, "CreatorUserID"),
                        UserName = table.GetString(i, "CreatorUserName"),
                        FirstName = table.GetString(i, "CreatorFirstName"),
                        LastName = table.GetString(i, "CreatorLastName")
                    }
                });
            }

            return retList;
        }

        public static List<FormRecord> form_records(DBResultSet results, List<FormElement> elements)
        {
            if (elements == null) elements = new List<FormElement>();

            List<FormRecord> retList = new List<FormRecord>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                FormRecord rec = new FormRecord();

                rec.InstanceID = table.GetGuid(i, "InstanceID");

                if (!rec.InstanceID.HasValue) continue;

                rec.OwnerID = table.GetGuid(i, "OwnerID");
                rec.CreationDate = table.GetDate(i, "CreationDate");

                rec.Cells.AddRange(elements.Where(e => e.ElementID.HasValue)
                    .Select(e => new RecordCell(e.ElementID.Value, table.GetString(i, e.ElementID.ToString()))));

                retList.Add(rec);
            }

            return retList;
        }

        public static FormStatistics form_statistics(DBResultSet results)
        {
            RVDataTable table = results.get_table();

            return new FormStatistics()
            {
                WeightSum = table.GetDouble(0, "WeightSum", defaultValue: 0).Value,
                Sum = table.GetDouble(0, "Sum", defaultValue: 0).Value,
                WeightedSum = table.GetDouble(0, "WeightedSum", defaultValue: 0).Value,
                Average = table.GetDouble(0, "Avg", defaultValue: 0).Value,
                WeightedAverage = table.GetDouble(0, "WeightedAvg", defaultValue: 0).Value,
                Minimum = table.GetDouble(0, "Min", defaultValue: 0).Value,
                Maximum = table.GetDouble(0, "Max", defaultValue: 0).Value,
                Variance = table.GetDouble(0, "Var", defaultValue: 0).Value,
                StandardDeviation = table.GetDouble(0, "StDev", defaultValue: 0).Value
            };
        }

        public static List<TemplateStatus> template_status(DBResultSet results)
        {
            List<TemplateStatus> retList = new List<TemplateStatus>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new TemplateStatus()
                {
                    TemplateID = table.GetGuid(i, "TemplateID"),
                    TemplateName = table.GetString(i, "TemplateName"),
                    ActivatedID = table.GetGuid(i, "ActivatedID"),
                    ActivatedName = table.GetString(i, "ActivatedName"),
                    ActivationDate = table.GetDate(i, "ActivationDate"),
                    Activator = new User()
                    {
                        UserID = table.GetGuid(i, "ActivatorUserID"),
                        UserName = table.GetString(i, "ActivatorUserName"),
                        FirstName = table.GetString(i, "ActivatorFirstName"),
                        LastName = table.GetString(i, "ActivatorLastName")
                    },
                    TemplateElementsCount = table.GetInt(i, "TemplateElementsCount"),
                    ElementsCount = table.GetInt(i, "ElementsCount"),
                    NewTemplateElementsCount = table.GetInt(i, "NewTemplateElementsCount"),
                    RemovedTemplateElementsCount = table.GetInt(i, "RemovedTemplateElementsCount"),
                    NewCustomElementsCount = table.GetInt(i, "NewCustomElementsCount")
                });
            }

            return retList;
        }

        public static List<Poll> polls(DBResultSet results, ref long totalCount)
        {
            List<Poll> retList = new List<Poll>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Poll()
                {
                    PollID = table.GetGuid(i, "PollID"),
                    IsCopyOfPollID = table.GetGuid(i, "IsCopyOfPollID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    Name = table.GetString(i, "Name"),
                    RefName = table.GetString(i, "RefName"),
                    Description = table.GetString(i, "Description"),
                    RefDescription = table.GetString(i, "RefDescription"),
                    BeginDate = table.GetDate(i, "BeginDate"),
                    FinishDate = table.GetDate(i, "FinishDate"),
                    ShowSummary = table.GetBool(i, "ShowSummary"),
                    HideContributors = table.GetBool(i, "HideContributors"),
                    Archived = table.GetBool(i, "Archived")
                });
            }

            totalCount = results.get_table(1).GetLong(row: 0, column: 0, defaultValue: 0).Value;

            return retList;
        }

        public static List<Poll> polls(DBResultSet results) {
            long totalCount = 0;
            return polls(results, ref totalCount);
        }

        public static void poll_status(DBResultSet results, ref string description, ref DateTime? beginDate, ref DateTime? finishDate, 
            ref Guid? instanceId, ref int? elementsCount, ref int? filledElementsCount, ref int? allFilledFormsCount)
        {
            RVDataTable table = results.get_table();

            description = table.GetString(0, "Description");
            beginDate = table.GetDate(0, "BeginDate");
            finishDate = table.GetDate(0, "FinishDate");
            instanceId = table.GetGuid(0, "InstanceID");
            elementsCount = table.GetInt(0, "ElementsCount");
            filledElementsCount = table.GetInt(0, "FilledElementsCount");
            allFilledFormsCount = table.GetInt(0, "AllFilledFormsCount");
        }

        public static List<PollAbstract> poll_abstract(DBResultSet results)
        {
            Dictionary<Guid, PollAbstract> dic = new Dictionary<Guid, PollAbstract>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? elementId = table.GetGuid(i, "ElementID");

                if (!elementId.HasValue) continue;

                if (!dic.ContainsKey(elementId.Value)) dic[elementId.Value] =
                        new PollAbstract() { ElementID = elementId };

                dic[elementId.Value].TotalCount = table.GetInt(i, "TotalValuesCount");

                PollAbstractValue val = new PollAbstractValue() {
                    Count = table.GetInt(i, "Count")
                };

                if (val.Count.HasValue) continue;

                object obj = table.GetValue(i, "Value");

                if (obj == null) continue;

                Type tp = obj.GetType();

                if (tp == typeof(String)) val.TextValue = (string)obj;
                else if (tp == typeof(bool)) val.BitValue = (bool)obj;
                else if (tp == typeof(double)) val.NumberValue = (double)obj;
                else continue;

                dic[elementId.Value].Values.Add(val);
            }

            if (results.TablesCount > 1)
            {
                table = results.get_table(1);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Guid? elementId = table.GetGuid(i, "ElementID");

                    if (!elementId.HasValue || !dic.ContainsKey(elementId.Value)) continue;

                    dic[elementId.Value].Stats.Minimum = table.GetDouble(i, "Min");
                    dic[elementId.Value].Stats.Maximum = table.GetDouble(i, "Max");
                    dic[elementId.Value].Stats.Average = table.GetDouble(i, "Avg");
                    dic[elementId.Value].Stats.Variance = table.GetDouble(i, "Var");
                    dic[elementId.Value].Stats.StandardDeviation = table.GetDouble(i, "StDev");
                }
            }

            return dic.Values.ToList();
        }

        public static List<FormElement> poll_element_instances(DBResultSet results)
        {
            List<FormElement> retList = new List<FormElement>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                FormElement formElement = new FormElement()
                {
                    Creator = new User() {
                        UserID = table.GetGuid(i, "UserID"),
                        UserName = table.GetString(i, "UserName"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    },
                    ElementID = table.GetGuid(i, "ElementID"),
                    RefElementID = table.GetGuid(i, "RefElementID"),
                    TextValue = table.GetString(i, "TextValue"),
                    FloatValue = table.GetDouble(i, "FloatValue"),
                    BitValue = table.GetBool(i, "BitValue"),
                    DateValue = table.GetDate(i, "DateValue"),
                    CreationDate = table.GetDate(i, "CreationDate"),
                    LastModificationDate = table.GetDate(i, "LastModificationDate"),
                    Type = null
                };

                FormElementTypes tp = FormElementTypes.Text;
                if (Enum.TryParse<FormElementTypes>(table.GetString(i, "Type"), out tp)) formElement.Type = tp;

                retList.Add(formElement);
            }

            return retList;
        }

        public static void current_polls_count(DBResultSet results, ref int count, ref int doneCount)
        {
            RVDataTable table = results.get_table();

            count = table.GetInt(0, "Count", defaultValue: 0).Value;
            doneCount = table.GetInt(0, "DoneCount", defaultValue: 0).Value;
        }
    }
}
