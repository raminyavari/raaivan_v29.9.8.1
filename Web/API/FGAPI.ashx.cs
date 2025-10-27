using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for FGAPI
    /// </summary>
    public class FGAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            int? sequenceNumber = PublicMethods.parse_int(context.Request.Params["SequenceNumber"]);
            if (!sequenceNumber.HasValue) sequenceNumber = 1;
            
            string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"]);
            if (string.IsNullOrEmpty(searchText)) searchText = PublicMethods.parse_string(context.Request.Params["text"]);
            
            switch (command)
            {
                case "CreateForm":
                    create_form(PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormTitle":
                    set_form_title(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormName":
                    set_form_name(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormDescription":
                    set_form_description(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveForm":
                    remove_form(PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RecycleForm":
                    recycle_form(PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetForms":
                    get_forms(searchText,
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_bool(context.Request.Params["Archive"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddFormElement":
                    FormElementTypes type = new FormElementTypes();
                    if (!Enum.TryParse<FormElementTypes>(context.Request.Params["Type"], out type)) type = FormElementTypes.Text;
                    
                    add_form_element(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Help"]),
                        sequenceNumber, type,
                        PublicMethods.parse_string(context.Request.Params["Info"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyFormElement":
                    modify_form_element(PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Help"]),
                        PublicMethods.parse_string(context.Request.Params["Info"]),
                        PublicMethods.parse_double(context.Request.Params["Weight"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetElementsOrder":
                    set_elements_order(ListMaker.get_guid_items(context.Request.Params["ElementIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormElementNecessity":
                    set_form_element_necessity(PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_bool(context.Request.Params["Necessary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormElementUniqueness":
                    set_form_element_uniqueness(PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveFormElement":
                    remove_form_element(PublicMethods.parse_guid(context.Request.Params["ElementID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SaveFormElements":
                    save_form_elements(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        FGUtilities.get_form_elements(paramsContainer.ApplicationID, context.Request.Params["Elements"],
                        formDesignMode: true), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetFormElements":
                    {
                        get_form_elements(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                            PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                            PublicMethods.parse_enum<FormElementTypes>(context.Request.Params["Type"]),
                            PublicMethods.parse_bool(context.Request.Params["ConsiderElementLimits"]), ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "CreateFormInstance":
                    create_form_instance(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["DirectorID"]),
                        PublicMethods.parse_bool(context.Request.Params["Admin"]),
                        PublicMethods.parse_bool(context.Request.Params["IsTemporary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveFormInstance":
                    remove_form_instance(PublicMethods.parse_guid(context.Request.Params["InstanceID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveOwnerFormInstances":
                    remove_owner_form_instances(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetFormInstance":
                    get_form_instance(PublicMethods.parse_guid(context.Request.Params["InstanceID"]),
                        PublicMethods.parse_guid(context.Request.Params["LimitOwnerID"]),
                        PublicMethods.parse_bool(context.Request.Params["ShowAllIfNoLimit"]),
                        PublicMethods.parse_bool(context.Request.Params["PollAbstract"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetOwnerFormInstances":
                    get_owner_form_instances(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ValidateNewName":
                    validate_new_name(PublicMethods.parse_guid(context.Request.Params["ObjectID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "MeetsUniqueConstraint":
                    meets_unique_constraint(PublicMethods.parse_guid(context.Request.Params["InstanceID"]),
                        PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_string(context.Request.Params["TextValue"]),
                        PublicMethods.parse_double(context.Request.Params["FloatValue"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SaveFormInstanceElements":
                    save_form_instance_elements(
                        FGUtilities.get_form_elements(paramsContainer.ApplicationID, context.Request.Params["Elements"]),
                        ListMaker.get_guid_items(context.Request.Params["ElementsToClear"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_enum(context.Request.Params["PollOwnerType"], defaultValue: PollOwnerType.None), 
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetElementChanges":
                    get_element_changes(PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ImportForm":
                    List<DocFileInfo> files =
                        DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["Uploaded"]);

                    if (files == null || files.Count != 1)
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    else
                    {
                        import_form(PublicMethods.parse_guid(context.Request.Params["InstanceID"]), files[0],
                            PublicMethods.parse_string(context.Request.Params["Map"]), ref responseText);
                    }

                    _return_response(ref responseText);
                    return;
                case "ExportAsPDF":
                    if (!export_form(PublicMethods.parse_guid(context.Request.Params["InstanceID"]),
                        PublicMethods.parse_guid(context.Request.Params["LimitOwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["CoverID"]),
                        PublicMethods.parse_string(context.Request.Params["PS"]), ref responseText))
                        _return_response(ref responseText);
                    return;
                case "SetFormInstanceAsFilled":
                    set_form_instance_as_filled(PublicMethods.parse_guid(context.Request.Params["InstanceID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormInstanceAsNotFilled":
                    set_form_instance_as_not_filled(PublicMethods.parse_guid(context.Request.Params["InstanceID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetFormOwner":
                    set_form_owner(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveFormOwner":
                    remove_form_owner(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetOwnerForm":
                    get_owner_form(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetFormRecords":
                    get_form_records(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        ListMaker.get_guid_items(context.Request.Params["ElementIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["InstanceIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["OwnerIDs"], '|'),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_guid(context.Request.Params["SortByElementID"]),
                        PublicMethods.parse_bool(context.Request.Params["DESC"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "InitializeOwnerFormInstance":
                    initialize_owner_form_instance(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetElementLimits":
                    set_element_limits(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        ListMaker.get_guid_items(context.Request.Params["ElementIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetElementLimits":
                    get_element_limits(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetElementLimitNecessity":
                    set_element_limit_necessity(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_bool(context.Request.Params["Necessary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveElementLimit":
                    remove_element_limit(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["ElementID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetFormStatistics":
                    get_form_statistics(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["InstanceID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPolls":
                    {
                        get_polls(PublicMethods.parse_guid(context.Request.Params["IsCopyOfPollID"]),
                            PublicMethods.parse_bool(context.Request.Params["Archive"]), searchText,
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_long(context.Request.Params["LowerBoundary"]), ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "AddPoll":
                    add_poll(PublicMethods.parse_guid(context.Request.Params["CopyFromPollID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPollInstance":
                    get_poll_instance(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_guid(context.Request.Params["CopyFromPollID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_bool(context.Request.Params["UseExistingPoll"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RenamePoll":
                    rename_poll(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPollDescription":
                    set_poll_description(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPollBeginDate":
                    set_poll_begin_date(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_date(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPollFinishDate":
                    set_poll_finish_date(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_date(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPollShowSummary":
                    set_poll_show_summary(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPollHideContributors":
                    set_poll_hide_contributors(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemovePoll":
                    remove_poll(PublicMethods.parse_guid(context.Request.Params["PollID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RecyclePoll":
                    recycle_poll(PublicMethods.parse_guid(context.Request.Params["PollID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPollStatus":
                    get_poll_status(PublicMethods.parse_guid(context.Request.Params["IsCopyOfPollID"]),
                        PublicMethods.parse_guid(context.Request.Params["PollID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPollAbstract":
                    get_poll_abstract(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        new List<FormElement>(), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetPollElementInstances":
                    get_poll_element_instances(PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_guid(context.Request.Params["ElementID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetCurrentPollsCount":
                    get_current_polls_count(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetCurrentPolls":
                    get_current_polls(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetAPOMaturityAssessmentStatus":
                    responseText = APOMaturityAssessment.get_statistics(paramsContainer.ApplicationID, paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
                case "NewAPOMaturityAssessment":
                    responseText = APOMaturityAssessment.add(paramsContainer.ApplicationID, 
                        PublicMethods.parse_int(context.Request.Params["Period"], defaultValue: 0).Value,
                        paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
                case "EditAPOMaturityAssessmentPeriod":
                    responseText = APOMaturityAssessment.edit(paramsContainer.ApplicationID,
                        PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        PublicMethods.parse_int(context.Request.Params["Period"], defaultValue: 0).Value,
                        paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
                case "RemoveAPOMaturityAssessment":
                    responseText = APOMaturityAssessment.remove(paramsContainer.ApplicationID,
                        PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
                case "GetAPOMaturityAssessmentForm":
                    responseText = APOMaturityAssessment.get_form(paramsContainer.ApplicationID,
                        PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
                case "SaveAPOMaturityAssessmentForm":
                    responseText = APOMaturityAssessment.save_form(paramsContainer.ApplicationID,
                        PublicMethods.parse_guid(context.Request.Params["PollID"]),
                        FGUtilities.get_form_elements(paramsContainer.ApplicationID, context.Request.Params["Elements"]),
                        paramsContainer.CurrentUserID);
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
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
                    ModuleIdentifier = ModuleIdentifier.FG
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

        protected string _get_form_json(FormType form)
        {
            string title = form.Title;
            string description = form.Description;

            Base64.encode(title, ref title);
            Base64.encode(description, ref description);

            return "{\"FormID\":\"" + form.FormID.Value.ToString() + "\",\"Title\":\"" + title +
                "\",\"Description\":\"" + description + "\"}";
        }

        protected void create_form(string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.CreateForm_PermissionFailed, Guid.Empty);
                return;
            }

            FormType formType = new FormType()
            {
                FormID = Guid.NewGuid(),
                Title = title,
                CreationDate = DateTime.Now
            };

            formType.Creator.UserID = paramsContainer.CurrentUserID.Value;

            bool result = FGController.create_form(paramsContainer.Tenant.Id, formType);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Form\":" + _get_form_json(formType) + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = formType.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateForm,
                    SubjectID = formType.FormID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_title(Guid? formId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.ModifyFormTitle_PermissionFailed, formId);
                return;
            }

            FormType formType = new FormType()
            {
                FormID = formId,
                Title = title,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = FGController.set_form_title(paramsContainer.Tenant.Id, formType);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = formType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyFormTitle,
                    SubjectID = formType.FormID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_name(Guid? formId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 90)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.ModifyFormName_PermissionFailed, formId);
                return;
            }

            bool isValidName = FGUtilities.is_valid_name(name);

            string errorMessage = !isValidName ? Messages.FormOrElementNameIsNotValid.ToString() : string.Empty;

            bool result = isValidName && formId.HasValue && FGController.set_form_name(paramsContainer.Tenant.Id,
                formId.Value, name, paramsContainer.CurrentUserID.Value, ref errorMessage);

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
                    Action = Modules.Log.Action.ModifyFormName,
                    SubjectID = formId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_description(Guid? formId, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(description) && description.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.ModifyFormDescription_PermissionFailed, formId);
                return;
            }

            FormType formType = new FormType()
            {
                FormID = formId,
                Description = description,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool result = formId.HasValue && FGController.set_form_description(paramsContainer.Tenant.Id, formType);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = formType.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyFormDescription,
                    SubjectID = formType.FormID,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_form(Guid? formId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.RemoveForm_PermissionFailed, formId);
                return;
            }

            bool result = formId.HasValue &&
                FGController.remove_form(paramsContainer.Tenant.Id, formId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveForm,
                    SubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void recycle_form(Guid? formId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.RecycleForm_PermissionFailed, formId);
                return;
            }

            bool result = formId.HasValue &&
                FGController.recycle_form(paramsContainer.Tenant.Id, formId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RecycleForm,
                    SubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_forms(string searchText, int? count, int? lowerBoundary, bool? archive, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<FormType> forms = FGController.get_forms(paramsContainer.Tenant.Id, searchText: searchText,
                count: count, lowerBoundary: lowerBoundary, archive: archive);

            responseText = "{\"Forms\":[";

            bool isFirst = true;
            foreach (FormType form in forms)
            {
                responseText += (isFirst ? string.Empty : ",") + _get_form_json(form);
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void add_form_element(Guid? formId, string title, string name, string help,
            int? sequenceNumber, FormElementTypes type, string info, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(title) && title.Length > 1900) ||
                (!string.IsNullOrEmpty(name) && name.Length > 90) ||
                (!string.IsNullOrEmpty(info) && info.Length > 3900) ||
                (!string.IsNullOrEmpty(help) && help.Length > 1900))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.AddFormElement_PermissionFailed, formId);
                return;
            }

            FormElement elem = new FormElement()
            {
                ElementID = Guid.NewGuid(),
                FormID = formId,
                Title = title,
                Name = name,
                Help = help,
                SequenceNumber = sequenceNumber,
                Type = type,
                Info = info,
                Creator = new Modules.Users.User() { UserID = paramsContainer.CurrentUserID.Value },
                CreationDate = DateTime.Now
            };

            bool isValidName = FGUtilities.is_valid_name(name);

            string errorMessage = !isValidName ? Messages.FormOrElementNameIsNotValid.ToString() : string.Empty;

            bool result = isValidName && formId.HasValue &&
                FGController.add_form_element(paramsContainer.Tenant.Id, elem, ref errorMessage);

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Element\":" + elem.toJson(paramsContainer.Tenant.Id) + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = elem.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddFormElement,
                    SubjectID = elem.ElementID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"" +
                        ",\"NM\":\"" + Base64.encode(name) + "\",\"Info\":\"" + Base64.encode(info) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void modify_form_element(Guid? elementId, string title, string name, string help,
            string info, double? weight, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(title) && title.Length > 1900) ||
                (!string.IsNullOrEmpty(name) && name.Length > 90) ||
                (!string.IsNullOrEmpty(info) && info.Length > 3900) ||
                (!string.IsNullOrEmpty(help) && help.Length > 1900))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!elementId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (elementId.HasValue) _save_error_log(Modules.Log.Action.ModifyFormElement_PermissionFailed, elementId);
                return;
            }

            FormElement elem = new FormElement()
            {
                ElementID = elementId,
                Title = title,
                Name = name,
                Help = help,
                Info = info,
                Weight = weight,
                LastModifierUserID = paramsContainer.CurrentUserID.Value,
                LastModificationDate = DateTime.Now
            };

            bool isValidName = FGUtilities.is_valid_name(name);

            string errorMessage = !isValidName ? Messages.FormOrElementNameIsNotValid.ToString() : string.Empty;

            bool result = isValidName && FGController.modify_form_element(paramsContainer.Tenant.Id, elem, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = elem.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyFormElement,
                    SubjectID = elem.ElementID,
                    Info = "{\"Name\":\"" + Base64.encode(title) + "\"" +
                        ",\"NM\":\"" + Base64.encode(name) + "\",\"Info\":\"" + Base64.encode(info) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_elements_order(List<Guid> elementIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (elementIds.Count == 0 ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = FGController.set_elements_order(paramsContainer.Tenant.Id, elementIds);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_form_element_necessity(Guid? elementId, bool? necessary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            necessary = necessary.HasValue && necessary.Value;

            if (!elementId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (elementId.HasValue)
                    _save_error_log(Modules.Log.Action.SetFormElementNecessity_PermissionFailed, elementId);
                return;
            }

            bool result = FGController.set_form_element_necessity(paramsContainer.Tenant.Id, elementId.Value, necessary.Value);

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
                    Action = Modules.Log.Action.SetFormElementNecessity,
                    SubjectID = elementId,
                    Info = "{\"Necessary\":" + necessary.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_element_uniqueness(Guid? elementId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            value = value.HasValue && value.Value;

            if (!elementId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (elementId.HasValue)
                    _save_error_log(Modules.Log.Action.SetFormElementUniqueness_PermissionFailed, elementId);
                return;
            }

            bool result = FGController.set_form_element_uniqueness(paramsContainer.Tenant.Id, elementId.Value, value.Value);

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
                    Action = Modules.Log.Action.SetFormElementUniqueness,
                    SubjectID = elementId,
                    Info = "{\"Value\":" + value.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_form_element(Guid? elementId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!elementId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (elementId.HasValue) _save_error_log(Modules.Log.Action.RemoveFormElement_PermissionFailed, elementId);
                return;
            }

            bool result = FGController.remove_form_element(paramsContainer.Tenant.Id,
                elementId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveFormElement,
                    SubjectID = elementId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void save_form_elements(Guid? formId, string title, string name, 
            string description, List<FormElement> elements, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!formId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                if (formId.HasValue) _save_error_log(Modules.Log.Action.SaveFormElements_PermissionFailed, formId);
                return;
            }

            //Rectify inputs
            if (!string.IsNullOrEmpty(name)) name = name.Trim().ToLower();
            if (string.IsNullOrEmpty(name)) name = null;

            if (elements == null) elements = new List<FormElement>();

            elements.Where(e => !e.ElementID.HasValue).ToList().ForEach(e => e.ElementID = Guid.NewGuid());

            elements.ToList()
                .ForEach(e =>
                {
                    if(!string.IsNullOrEmpty(e.Name)) e.Name = e.Name.Trim().ToLower();
                    if (string.IsNullOrEmpty(e.Name)) e.Name = null;
                });
            //end of Rectify inputs

            if ((!string.IsNullOrEmpty(title) && title.Length > 250) || (!string.IsNullOrEmpty(name) && name.Length > 95) ||
                (!string.IsNullOrEmpty(description) && description.Length > 1900) ||
                elements.Any(itm => (!string.IsNullOrEmpty(itm.Title) && itm.Title.Length > 250) ||
                    (!string.IsNullOrEmpty(itm.Name) && itm.Name.Length > 95)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title) || !PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }
            else if (!FGUtilities.is_valid_name(name) ||
                elements.Any(itm =>
                {
                    return !FGUtilities.is_valid_name(itm.Name) || 
                        elements.Any(x => x.ElementID != itm.ElementID && !string.IsNullOrEmpty(itm.Name) && x.Name == itm.Name);
                }) || !FGController.validate_new_name(paramsContainer.Tenant.Id, objectId: formId.Value, formId: null, name: name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidInput + "\"}";
                return;
            }

            elements = elements.Where(e => e.Type.HasValue && !string.IsNullOrEmpty(e.Title)).ToList();
            
            bool result = FGController.save_form_elements(paramsContainer.Tenant.Id,
                formId.Value, title, name, description, elements, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + 
                ",\"Elements\":[" + string.Join(",", elements.Select(e => e.toJson(paramsContainer.Tenant.Id))) + "]" +
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
                    Action = Modules.Log.Action.SaveFormElements,
                    SubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_form_elements(Guid? formId, Guid? ownerId, FormElementTypes? type,
            bool? considerElementLimits, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            string desc = string.Empty;
            string name = string.Empty;

            FormType form = formId.HasValue ? FGController.get_form(paramsContainer.Tenant.Id, formId.Value) :
                (ownerId.HasValue ? FGController.get_owner_form(paramsContainer.Tenant.Id, ownerId.Value) : null);

            if (form != null && !formId.HasValue) formId = form.FormID;

            if (form != null)
            {
                desc = form.Description;
                name = form.Name;
            }

            List<FormElement> elements = !formId.HasValue ? new List<FormElement>() :
                FGController.get_form_elements(paramsContainer.Tenant.Id, formId.Value, ownerId, type)
                .OrderBy(v => v.SequenceNumber).ToList();

            if (formId.HasValue && ownerId.HasValue && considerElementLimits.HasValue && considerElementLimits.Value)
            {
                List<FormElement> limits = FGController.get_element_limits(paramsContainer.Tenant.Id, ownerId.Value);
                if (limits.Count > 0) elements = elements.Where(u => limits.Any(x => x.ElementID == u.ElementID)).ToList();
            }

            responseText = "{\"Elements\":[" +
                string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]" +
                (string.IsNullOrEmpty(desc) ? string.Empty : ",\"FormDescription\":\"" + Base64.encode(desc) + "\"") +
                (string.IsNullOrEmpty(name) ? string.Empty : ",\"FormName\":\"" + Base64.encode(name) + "\"") +
                "}";
        }

        public bool create_form_instance(Guid? formId, Guid? ownerId, Guid? directorId,
            bool? admin, bool? isTemporary, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return false;

            admin = admin.HasValue && admin.Value;

            if (isTemporary.HasValue && isTemporary.Value && formId.HasValue &&
                ownerId.HasValue && paramsContainer.CurrentUserID.HasValue)
            {
                FormType frm = FGController.get_owner_form_instances(paramsContainer.Tenant.Id, ownerId.Value,
                    formId, true, paramsContainer.CurrentUserID.Value).FirstOrDefault();

                if (frm != null)
                {
                    responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                        ",\"Instance\":" + frm.toJson(paramsContainer.Tenant.Id) + "}";
                    return true;
                }
            }

            FormType newForm = new FormType()
            {
                InstanceID = Guid.NewGuid(),
                FormID = formId,
                OwnerID = ownerId,
                DirectorID = !directorId.HasValue || directorId == Guid.Empty ? null : directorId,
                Admin = admin,
                IsTemporary = isTemporary,
                CreationDate = DateTime.Now
            };

            newForm.Creator.UserID = paramsContainer.CurrentUserID.Value;

            bool result = formId.HasValue && ownerId.HasValue &&
                FGController.create_form_instance(paramsContainer.Tenant.Id, newForm);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"Instance\":" + newForm.toJson(paramsContainer.Tenant.Id) + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = newForm.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateFormInstance,
                    SubjectID = newForm.InstanceID,
                    Info = "{\"OwnerID\":\"" + ownerId.ToString() + "\"" +
                        ",\"DirectorID\":\"" + directorId.ToString() + "\"" +
                        ",\"Admin\":" + admin.ToString().ToLower() +
                        ",\"IsTemporary\":" + (isTemporary.HasValue && isTemporary.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log

            return result;
        }

        protected void remove_form_instance(Guid? instanceId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = instanceId.HasValue && FGController.remove_form_instance(paramsContainer.Tenant.Id,
                instanceId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveFormInstance,
                    SubjectID = instanceId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_owner_form_instances(Guid? ownerId, Guid? formId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = ownerId.HasValue && ownerId != Guid.Empty && formId.HasValue && formId != Guid.Empty &&
                FGController.remove_owner_form_instances(paramsContainer.Tenant.Id,
                ownerId.Value, formId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveOwnerFormInstances,
                    SubjectID = ownerId,
                    SecondSubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected List<FormElement> _get_form_instance_elements(FormType instance,
            Guid? limitOwnerId, bool isSystemAdmin, ref bool isDirector)
        {
            List<FormElement> elements = FGController.get_form_instance_elements(paramsContainer.Tenant.Id,
                instance.InstanceID.Value).OrderBy(u => u.SequenceNumber).ToList();

            if (limitOwnerId == Guid.Empty) limitOwnerId = null;
            List<FormElement> limits = limitOwnerId.HasValue ?
                FGController.get_element_limits(paramsContainer.Tenant.Id, limitOwnerId.Value) : new List<FormElement>();

            bool noLimit = limits.Count == 0;

            if (!noLimit && limitOwnerId.HasValue)
            {
                elements = elements.Where(u => (u.RefElementID.HasValue && u.ElementID != u.RefElementID) ||
                    limits.Any(x => x.ElementID == u.ElementID || x.ElementID == u.RefElementID)).ToList();
            }

            //Resolve Tables
            List<Guid> subInstanceIds = new List<Guid>();

            elements.Where(u => u.Type == FormElementTypes.Form && u.RefElementID.HasValue && u.ElementID.HasValue)
                .ToList().ForEach(e =>
                {
                    Dictionary<string, object> dic = PublicMethods.fromJSON(e.Info);
                    Guid? formId = PublicMethods.parse_guid(PublicMethods.get_dic_value(dic, "FormID"));

                    e.TableColumns = !formId.HasValue ? new List<FormElement>() :
                        FGController.get_form_elements(paramsContainer.Tenant.Id, formId).OrderBy(u => u.SequenceNumber).ToList();

                    e.TableContent = !formId.HasValue ? new List<FormType>() :
                        FGController.get_owner_form_instances(paramsContainer.Tenant.Id, e.ElementID.Value, formId.Value);

                    if (e.TableContent.Count > 0) subInstanceIds
                        .AddRange(e.TableContent.Where(t => t.InstanceID.HasValue).Select(t => t.InstanceID.Value).ToList());
                });

            List<FormElement> subInstElements = FGController.get_form_instance_elements(paramsContainer.Tenant.Id, subInstanceIds)
                .OrderBy(u => u.SequenceNumber).ToList();

            elements.Where(u => u.Type == FormElementTypes.Form && u.RefElementID.HasValue && u.TableContent != null).ToList()
                .ForEach(e => e.TableContent.ForEach(t => t.Elements = subInstElements.Where(s => s.FormInstanceID == t.InstanceID).ToList()));
            //end of Resolve Tables

            isDirector = paramsContainer.IsAuthenticated &&
                FGController.is_director(paramsContainer.Tenant.Id, instance.InstanceID.Value, paramsContainer.CurrentUserID.Value);

            if (!isDirector) isDirector = isSystemAdmin;

            if (paramsContainer.IsAuthenticated && !isDirector && Modules.RaaiVanConfig.Modules.WorkFlow(paramsContainer.Tenant.Id))
            {
                Guid? ownerId = Modules.WorkFlow.WFController.get_form_instance_workflow_owner_id(
                    paramsContainer.Tenant.Id, instance.InstanceID.Value);
                if (ownerId.HasValue) isDirector = CNController.is_service_admin(paramsContainer.Tenant.Id,
                    ownerId.Value, paramsContainer.CurrentUserID.Value);
            }

            if (paramsContainer.IsAuthenticated && !isDirector && instance.OwnerID.HasValue)
                isDirector = CNController.is_service_admin(paramsContainer.Tenant.Id,
                    instance.OwnerID.Value, paramsContainer.CurrentUserID.Value);

            //Remove elements which current user is not permited to view
            if (!isDirector)
            {
                Guid? hierarchyOwnerId = FGController.get_form_instance_hierarchy_owner_id(
                    paramsContainer.Tenant.Id, instance.InstanceID.Value);

                bool isNode = CNController.is_node(paramsContainer.Tenant.Id, hierarchyOwnerId.Value);
                bool isNodeType = !isNode && CNController.is_node_type(paramsContainer.Tenant.Id, hierarchyOwnerId.Value);

                bool isNodeAdmin = paramsContainer.CurrentUserID.HasValue && isNode && 
                    (isSystemAdmin || CNController.is_node_admin(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, hierarchyOwnerId.Value, null, null, null));
                bool isNodeTypeAdmin = paramsContainer.CurrentUserID.HasValue && isNodeType &&
                    (isSystemAdmin || CNController.is_service_admin(paramsContainer.Tenant.Id,
                    hierarchyOwnerId.Value, paramsContainer.CurrentUserID.Value));

                if (hierarchyOwnerId.HasValue && (isNode || isNodeType) && !isNodeAdmin && !isNodeTypeAdmin)
                {
                    List<Guid> granted = PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        elements.Where(x => x.ElementID.HasValue || x.RefElementID.HasValue)
                        .Select(u => u.RefElementID.HasValue ? u.RefElementID.Value : u.ElementID.Value).ToList(),
                        PrivacyObjectType.FormElement, PermissionType.View);

                    elements = elements.Where(u => granted.Any(x => x == u.RefElementID || x == u.ElementID)).ToList();
                }
            }
            //end of Remove elements which current user is not permited to view

            List<FormElement> allElements = new List<FormElement>(elements);
            allElements.AddRange(subInstElements);

            //get guid items
            Dictionary<Guid, List<SelectedGuidItem>> guidItems =
                FGController.get_selected_guids(paramsContainer.Tenant.Id, allElements.Where(
                    u => u.Type == FormElementTypes.Node || u.Type == FormElementTypes.User).Select(x => x.ElementID.Value).ToList());
            //end of get guid items

            //get attached files
            List<Guid> elementIds = allElements.Where(u => u.Type == FormElementTypes.File)
                .Select(x => x.ElementID.Value).ToList();

            List<DocFileInfo> attachedFiles =
                DocumentsController.get_owner_files(paramsContainer.Tenant.Id, elementIds);
            //end of get attached files

            foreach (FormElement elem in elements)
            {
                FormElement limit = limits.Where(
                    u => u.ElementID == elem.ElementID || u.ElementID == elem.RefElementID).FirstOrDefault();

                if (limit != null) elem.Necessary = limit.Necessary;
            }

            foreach (FormElement elem in allElements)
            {
                if (elem.Type == FormElementTypes.Date && elem.DateValue.HasValue)
                    elem.TextValue = GenericDate.get_local_date(elem.DateValue.Value);

                if (elem.Type == FormElementTypes.Node && elem.ElementID.HasValue && guidItems.ContainsKey(elem.ElementID.Value))
                {
                    elem.GuidItems = guidItems[elem.ElementID.Value];
                    elem.GuidItems.ForEach(gi => gi.Type = SelectedGuidItemType.Node);
                }

                if (elem.Type == FormElementTypes.User && elem.ElementID.HasValue && guidItems.ContainsKey(elem.ElementID.Value))
                {
                    elem.GuidItems = guidItems[elem.ElementID.Value];
                    elem.GuidItems.ForEach(gi => gi.Type = SelectedGuidItemType.User);
                }

                if (elem.Type == FormElementTypes.File)
                    elem.AttachedFiles = attachedFiles.Where(u => u.OwnerID == elem.ElementID).ToList();
            }

            return elements;
        }

        protected void get_form_instance(Guid? instanceId, Guid? limitOwnerId, bool? showAllIfNoLimit,
            bool? pollAbstract, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            showAllIfNoLimit = showAllIfNoLimit.HasValue && showAllIfNoLimit.Value;

            if (!instanceId.HasValue)
            {
                responseText = "{\"Elements\":[]}";
                return;
            }

            FormType instance = FGController.get_form_instance(paramsContainer.Tenant.Id, instanceId.Value);

            //poll abstract
            string strPollAbstract = string.Empty;

            if (pollAbstract.HasValue && pollAbstract.Value)
            {
                Poll poll = instance == null || !instance.OwnerID.HasValue ? null :
                    FGController.get_poll(paramsContainer.Tenant.Id, instance.OwnerID.Value);

                if (poll != null && poll.PollID.HasValue)
                    get_poll_abstract(poll.PollID, new List<FormElement>(), ref strPollAbstract);
            }
            //end of poll abstract

            bool isSystemAdmin = paramsContainer.IsAuthenticated &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            bool isDirector = false;

            List<FormElement> elements = _get_form_instance_elements(instance, limitOwnerId, isSystemAdmin, ref isDirector);

            responseText = "{\"IsDirector\":" + isDirector.ToString().ToLower() +
                ",\"IsSystemAdmin\":" + isSystemAdmin.ToString().ToLower() +
                ",\"FormID\":\"" + (!instance.FormID.HasValue ? string.Empty : instance.FormID.Value.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(instance.Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(instance.Description) + "\"" +
                ",\"Filled\":" + (instance.Filled.HasValue ? instance.Filled.Value : false).ToString().ToLower() +
                (string.IsNullOrEmpty(strPollAbstract) ? string.Empty :
                    ",\"PollID\":\"" + instance.OwnerID.ToString() + "\"") +
                (string.IsNullOrEmpty(strPollAbstract) ? string.Empty : ",\"PollAbstract\":" + strPollAbstract) +
                ",\"Elements\":[" +
                    string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id, includeRefElementId: true, icon: true))) +
                "]}";
        }

        protected FormElement _get_owner_form_instance_elements(Guid formId, Guid ownerId)
        {
            FormElement element = new FormElement() {
                ElementID = ownerId,
                TableColumns = FGController.get_form_elements(paramsContainer.Tenant.Id, formId)
                    .OrderBy(u => u.SequenceNumber).ToList(),
                TableContent = FGController.get_owner_form_instances(paramsContainer.Tenant.Id, ownerId, formId)
            };

            List<Guid> subInstanceIds = element.TableContent.Count == 0 ? new List<Guid>() :
                element.TableContent.Where(t => t.InstanceID.HasValue).Select(t => t.InstanceID.Value).ToList();

            List<FormElement> subInstElements = FGController.get_form_instance_elements(paramsContainer.Tenant.Id, subInstanceIds)
                .OrderBy(u => u.SequenceNumber).ToList();

            element.TableContent.ForEach(t => t.Elements = subInstElements.Where(s => s.FormInstanceID == t.InstanceID).ToList());

            List<FormElement> allElements = new List<FormElement>(subInstElements);

            //get guid items
            Dictionary<Guid, List<SelectedGuidItem>> guidItems =
                FGController.get_selected_guids(paramsContainer.Tenant.Id, allElements.Where(
                    u => u.Type == FormElementTypes.Node || u.Type == FormElementTypes.User).Select(x => x.ElementID.Value).ToList());
            //end of get guid items

            //get attached files
            List<Guid> elementIds = allElements.Where(u => u.Type == FormElementTypes.File)
                .Select(x => x.ElementID.Value).ToList();

            List<DocFileInfo> attachedFiles =
                DocumentsController.get_owner_files(paramsContainer.Tenant.Id, elementIds);
            //end of get attached files

            foreach (FormElement elem in allElements)
            {
                if (elem.Type == FormElementTypes.Date && elem.DateValue.HasValue)
                    elem.TextValue = GenericDate.get_local_date(elem.DateValue.Value);

                if (elem.Type == FormElementTypes.Node && elem.ElementID.HasValue && guidItems.ContainsKey(elem.ElementID.Value))
                {
                    elem.GuidItems = guidItems[elem.ElementID.Value];
                    elem.GuidItems.ForEach(gi => gi.Type = SelectedGuidItemType.Node);
                }

                if (elem.Type == FormElementTypes.User && elem.ElementID.HasValue && guidItems.ContainsKey(elem.ElementID.Value))
                {
                    elem.GuidItems = guidItems[elem.ElementID.Value];
                    elem.GuidItems.ForEach(gi => gi.Type = SelectedGuidItemType.User);
                }

                if (elem.Type == FormElementTypes.File)
                    elem.AttachedFiles = attachedFiles.Where(u => u.OwnerID == elem.ElementID).ToList();
            }

            return element;
        }

        protected void get_owner_form_instances(Guid? formId, Guid? ownerId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            responseText = !formId.HasValue || !ownerId.HasValue ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                _get_owner_form_instance_elements(formId.Value, ownerId.Value).toJson(paramsContainer.Tenant.Id, icon: true);
        }

        protected void validate_new_name(Guid? objectId, Guid? formId, string name, ref string responseText) {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = objectId.HasValue &&
                FGController.validate_new_name(paramsContainer.Tenant.Id, objectId.Value, formId, name);

            responseText = "{\"Valid\":" + result.ToString().ToLower() + "}";
        }

        protected void meets_unique_constraint(Guid? instanceId,
            Guid? elementId, string textValue, double? floatValue, ref string responseText)
        {
            bool result = instanceId.HasValue && elementId.HasValue &&
                FGController.meets_unique_constraint(paramsContainer.Tenant.Id, instanceId.Value, elementId.Value, textValue, floatValue);

            responseText = "{\"Value\":" + result.ToString().ToLower() + "}";
        }

        public bool save_form_instance_elements(List<FormElement> elements,
            List<Guid> elementsToClear, Guid? pollId, PollOwnerType pollOwnerType, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return false;

            /*
            Guid elemId = elements.Select(u => u.ElementID.Value).FirstOrDefault();
            if (!FGController.is_director(elemId, currentUserId))
            {
                responseText = "{\"ErrorText\":\"" + "این فرم متعلق به شما نیست" + "\"}";
                return;
            }
            */

            if (elements != null && (elementsToClear == null || elementsToClear.Count == 0)) {
                elementsToClear = elements
                    .Where(e => e.is_empty() && e.ElementID.HasValue && e.RefElementID.HasValue && e.ElementID != e.RefElementID)
                    .Select(e => e.ElementID.Value).ToList();
            }

            if (elementsToClear != null && elements != null)
            {
                elementsToClear = elementsToClear.Where(
                    u => !elements.Any(x => x.ElementID == u || x.RefElementID == u)).ToList();
            }

            List<DocFileInfo> attachedFiles = new List<DocFileInfo>();

            elements.Where(u => u != null && u.AttachedFiles != null && u.AttachedFiles.Count > 0).ToList().ForEach(e => {
                attachedFiles.AddRange(e.AttachedFiles);

                e.AttachedFiles.ForEach(f =>
                {
                    f.OwnerID = e.ElementID;
                    f.OwnerType = FileOwnerTypes.FormElement;
                });
            });

            if (attachedFiles != null &&
                attachedFiles.Any(f => !f.move(FolderNames.TemporaryFiles, FolderNames.Attachments))) {
                responseText = "{\"ErrorText\":\"" + Messages.SavingFilesFailed + "\"}";
                return false;
            }

            string errorMessage = string.Empty;

            bool result = FGController.save_form_instance_elements(paramsContainer.Tenant.Id,
                elements, elementsToClear, paramsContainer.CurrentUserID.Value, ref errorMessage);

            if (!result && attachedFiles != null)
                attachedFiles.ForEach(f => f.move(FolderNames.Attachments, FolderNames.TemporaryFiles));

            //update AdditionalID for the form instance owner
            if (result && elements.Count > 0)
            {
                Guid? ownerId = FGController.get_form_instance_owner_id(paramsContainer.Tenant.Id, elements.First().ElementID.Value);

                if(ownerId.HasValue)
                    CNAPI.update_additional_id(paramsContainer.Tenant.Id, ownerId.Value, paramsContainer.CurrentUserID.Value);
            }
            //end of update AdditionalID for the form instance owner

            //Save Tagged Items
            if (result)
            {
                List<TaggedItem> tagged = new List<TaggedItem>();

                foreach (FormElement elem in elements)
                {
                    List<InlineTag> inlineTags = string.IsNullOrEmpty(elem.TextValue) ? new List<InlineTag>() :
                        Expressions.get_tagged_items(elem.TextValue);

                    foreach (InlineTag tg in inlineTags)
                    {
                        TaggedType tgTp = TaggedType.None;
                        if (!Enum.TryParse(tg.Type, out tgTp) || tgTp == TaggedType.None || !tg.ID.HasValue) continue;

                        tagged.Add(new TaggedItem(elem.ElementID.Value, tg.ID.Value, TagContextType.FormElement, tgTp));
                    }

                    if (inlineTags.Count == 0)
                        tagged.Add(new TaggedItem() { ContextID = elem.ElementID.Value });
                }

                foreach (Guid id in elementsToClear)
                    if (!tagged.Any(u => u.ContextID == id)) tagged.Add(new TaggedItem() { ContextID = id });

                GlobalController.save_tagged_items_offline(paramsContainer.Tenant.Id,
                    tagged, true, paramsContainer.CurrentUserID.Value);
            }
            //Save Tagged Items

            string pollAbstract = string.Empty;

            if (result)
            {
                responseText = ",\"FilledElements\":[" +
                    string.Join(",", elements.Where(u => !u.Filled.HasValue || !u.Filled.Value)
                    .Select(x => "{\"ElementID\":\"" + x.RefElementID.Value.ToString() + "\"" +
                        ",\"NewElementID\":\"" + x.ElementID.Value.ToString() + "\"" + "}")) + "]";

                if (pollId.HasValue)
                {
                    get_poll_abstract(pollId.Value, elements, ref pollAbstract);
                    if (!string.IsNullOrEmpty(pollAbstract)) pollAbstract = ",\"PollAbstract\":" + pollAbstract;
                }
            }

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + responseText + pollAbstract + "}";

            //Send Notifications
            if (result && pollId.HasValue && pollId != Guid.Empty && pollOwnerType == PollOwnerType.WorkFlow)
            {
                /*
                long totalCount = 0;

                NotificationController.get_dashboards(paramsContainer.Tenant.Id,
                    null, null, null, null, DashboardType.WorkFlow, DashboardSubType.Admin, false, null, null, null, null, ref totalCount);
                */
            }
            //end of Send Notifications

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyFormInstanceElements,
                    SubjectIDs = elements.Select(u => u.ElementID.Value).ToList(),
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log

            return result;
        }

        protected void get_element_changes(Guid? elementId, int? count, int? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<FormElement> changes = !elementId.HasValue ? new List<FormElement>() :
                FGController.get_element_changes(paramsContainer.Tenant.Id, elementId.Value, count, lowerBoundary);

            responseText = "{\"Changes\":[" + string.Join(",", changes.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void set_form_instance_as_filled(Guid? instanceId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = instanceId.HasValue && FGController.set_form_instance_as_filled(paramsContainer.Tenant.Id,
                instanceId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetFormInstanceAsFilled,
                    SubjectID = instanceId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_instance_as_not_filled(Guid? instanceId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = instanceId.HasValue && FGController.set_form_instance_as_not_filled(paramsContainer.Tenant.Id,
                instanceId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetFormInstanceAsNotFilled,
                    SubjectID = instanceId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_form_owner(Guid? ownerId, Guid? formId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = ownerId.HasValue && formId.HasValue && FGController.set_form_owner(paramsContainer.Tenant.Id,
                ownerId.Value, formId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetFormOwner,
                    SubjectID = ownerId,
                    SecondSubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_form_owner(Guid? ownerId, Guid? formId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = ownerId.HasValue && formId.HasValue && FGController.remove_form_owner(paramsContainer.Tenant.Id,
                ownerId.Value, formId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveFormOwner,
                    SubjectID = ownerId,
                    SecondSubjectID = formId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_owner_form(Guid? ownerId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            FormType form = !ownerId.HasValue ? null :
                FGController.get_owner_form(paramsContainer.Tenant.Id, ownerId.Value);

            responseText = form == null ? "{}" : _get_form_json(form);
        }

        protected void get_form_records(Guid? formId, List<Guid> elementIds, List<Guid> instanceIds, List<Guid> ownerIds,
             int? lowerBoundary, int? count, Guid? sortByElementId, bool? desc, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<FormRecord> records = !formId.HasValue ? new List<FormRecord>() :
                FGController.get_form_records(paramsContainer.Tenant.Id, formId.Value, elementIds,
                instanceIds, ownerIds, new List<FormFilter>(), lowerBoundary, count, sortByElementId, desc);

            Guid? ownerId = null;
            if (ownerIds != null && ownerIds.Count == 1) ownerId = ownerIds[0];

            List<FormElement> elements = !formId.HasValue ? new List<FormElement>() :
                FGController.get_form_elements(paramsContainer.Tenant.Id, formId.Value, ownerId);

            responseText = "{\"Records\":[" + string.Join(",", records.Select(u => u.toJSON())) + "]" +
                ",\"Elements\":[" + string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void initialize_owner_form_instance(Guid? ownerId, Guid? formId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            Guid? instanceId = !ownerId.HasValue ? null :
                FGController.initialize_owner_form_instance(paramsContainer.Tenant.Id,
                ownerId.Value, formId, paramsContainer.CurrentUserID.Value);

            responseText = instanceId.HasValue ? "{\"InstanceID\":\"" + instanceId.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.FormInstanceInitializationFailed + "\"}";
        }

        protected void set_element_limits(Guid? ownerId, List<Guid> elementIds, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = ownerId.HasValue && FGController.set_element_limits(paramsContainer.Tenant.Id,
                ownerId.Value, elementIds, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetElementLimits,
                    SubjectID = ownerId,
                    Info = "{\"ElementIDs\":[" + ProviderUtil.list_to_string<string>(elementIds.Select(u =>
                        "\"" + u.ToString() + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_element_limits(Guid? ownerId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<FormElement> elements = !ownerId.HasValue ? new List<FormElement>() :
                FGController.get_element_limits(paramsContainer.Tenant.Id, ownerId.Value);

            responseText = "{\"Elements\":[" + string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void set_element_limit_necessity(Guid? ownerId, Guid? elementId,
            bool? necessary, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            necessary = necessary.HasValue && necessary.Value;

            bool result = ownerId.HasValue && elementId.HasValue &&
                FGController.set_element_limit_necessity(paramsContainer.Tenant.Id,
                ownerId.Value, elementId.Value, necessary.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetElementLimitNecessity,
                    SubjectID = ownerId,
                    SecondSubjectID = elementId,
                    Info = "{\"Necessary\":" + necessary.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_element_limit(Guid? ownerId, Guid? elementId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = ownerId.HasValue && elementId.HasValue &&
                FGController.remove_element_limit(paramsContainer.Tenant.Id,
                ownerId.Value, elementId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveElementLimit,
                    SubjectID = ownerId,
                    SecondSubjectID = elementId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_form_statistics(Guid? ownerId, Guid? instanceId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            FormStatistics stats = !ownerId.HasValue && !instanceId.HasValue ? null :
                FGController.get_form_statistics(paramsContainer.Tenant.Id, ownerId, instanceId);

            responseText = stats == null ? "{}" : stats.toJson();
        }

        protected void get_polls(Guid? isCopyOfPollId,
            bool? archive, string searchText, int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!archive.HasValue) archive = false;

            long totalCount = 0;

            List<Poll> polls = FGController.get_polls(paramsContainer.Tenant.Id, isCopyOfPollId, null,
                archive, searchText, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Polls\":[" + string.Join(",", polls.Select(u => u.toJson())) + "]}";
        }

        protected void add_poll(Guid? copyFromPollId, Guid? ownerId, string name, ref string responseText)
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

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.AddPoll_PermissionFailed, Guid.Empty);
                return;
            }

            Guid pollId = Guid.NewGuid();

            bool result = FGController.add_poll(paramsContainer.Tenant.Id,
                pollId, copyFromPollId, ownerId, name, paramsContainer.CurrentUserID.Value);

            if (!result) responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else
            {
                Poll pl = FGController.get_poll(paramsContainer.Tenant.Id, pollId);

                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                    (pl == null ? string.Empty : ",\"Poll\":" + pl.toJson()) + "}";
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
                    Action = Modules.Log.Action.AddPoll,
                    SubjectID = pollId,
                    Info = "{\"CopyFromPollID\":\"" + (copyFromPollId.HasValue ?
                        copyFromPollId.ToString() : string.Empty) + "\",\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_poll_instance(Guid? pollId, Guid? copyFromPollId, Guid? ownerId,
            bool? useExistingPoll, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Guid? instanceId = null;

            if (!pollId.HasValue && useExistingPoll.HasValue && useExistingPoll.Value &&
                copyFromPollId.HasValue && ownerId.HasValue)
            {
                pollId = FGController.get_owner_poll_ids(paramsContainer.Tenant.Id,
                    copyFromPollId.Value, ownerId.Value).FirstOrDefault();
                if (pollId == Guid.Empty) pollId = null;
            }

            if (!pollId.HasValue) pollId = Guid.NewGuid();

            if (copyFromPollId.HasValue) instanceId = FGController.get_poll_instance(paramsContainer.Tenant.Id,
                 pollId, copyFromPollId.Value, ownerId, paramsContainer.CurrentUserID.Value);

            Poll poll = !instanceId.HasValue ? null : FGController.get_poll(paramsContainer.Tenant.Id, pollId.Value);

            responseText = !instanceId.HasValue ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"PollID\":\"" + pollId.ToString() + "\"" +
                ",\"InstanceID\":\"" + instanceId.ToString() + "\"" +
                (poll == null ? string.Empty : ",\"Poll\":" + poll.toJson()) +
                "}";
        }

        protected void rename_poll(Guid? pollId, string name, ref string responseText)
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

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.RenamePoll_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.rename_poll(paramsContainer.Tenant.Id,
                pollId.Value, name, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RenamePoll,
                    SubjectID = pollId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_poll_description(Guid? pollId, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(description) && description.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.SetPollDescription_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.set_poll_description(paramsContainer.Tenant.Id,
                pollId.Value, description, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPollDescription,
                    SubjectID = pollId,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_poll_begin_date(Guid? pollId, DateTime? beginDate, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.SetPollBeginDate_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.set_poll_begin_date(paramsContainer.Tenant.Id,
                pollId.Value, beginDate, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPollBeginDate,
                    SubjectID = pollId,
                    Info = "{\"BeginDate\":\"" + (!beginDate.HasValue ? string.Empty : beginDate.Value.ToShortDateString()) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_poll_finish_date(Guid? pollId, DateTime? finishDate, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.SetPollFinishDate_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.set_poll_finish_date(paramsContainer.Tenant.Id,
                pollId.Value, finishDate, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPollFinishDate,
                    SubjectID = pollId,
                    Info = "{\"FinishDate\":\"" + (!finishDate.HasValue ? string.Empty : finishDate.Value.ToShortDateString()) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_poll_show_summary(Guid? pollId, bool? showSummary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.SetPollShowSummary_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.set_poll_show_summary(paramsContainer.Tenant.Id,
                pollId.Value, showSummary.HasValue && showSummary.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPollShowSummary,
                    SubjectID = pollId,
                    Info = "{\"ShowSummary\":" + (showSummary.HasValue && showSummary.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void set_poll_hide_contributors(Guid? pollId, bool? hideContributors, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.SetPollHideContributors_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.set_poll_hide_contributors(paramsContainer.Tenant.Id,
                pollId.Value, hideContributors.HasValue && hideContributors.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPollHideContributors,
                    SubjectID = pollId,
                    Info = "{\"HideContributors\":" + (hideContributors.HasValue && hideContributors.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void remove_poll(Guid? pollId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.RemovePoll_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.remove_poll(paramsContainer.Tenant.Id,
                pollId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemovePoll,
                    SubjectID = pollId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void recycle_poll(Guid? pollId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                _save_error_log(Modules.Log.Action.RecyclePoll_PermissionFailed, pollId);
                return;
            }

            bool result = pollId.HasValue && FGController.recycle_poll(paramsContainer.Tenant.Id,
                pollId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RecyclePoll,
                    SubjectID = pollId,
                    ModuleIdentifier = ModuleIdentifier.FG
                });
            }
            //end of Save Log
        }

        protected void get_poll_status(Guid? isCopyOfPollId, Guid? pollId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            string description = null;
            DateTime? beginDate = null;
            DateTime? finishDate = null;
            Guid? instanceId = null;
            int? elementsCount = null;
            int? filledElementsCount = null;
            int? allFilledFormsCount = null;

            if (isCopyOfPollId.HasValue || pollId.HasValue)
            {
                FGController.get_poll_status(paramsContainer.Tenant.Id, pollId, isCopyOfPollId,
                    paramsContainer.CurrentUserID.Value, ref description, ref beginDate, ref finishDate,
                    ref instanceId, ref elementsCount, ref filledElementsCount, ref allFilledFormsCount);
            }

            Poll pl = null;

            if (instanceId.HasValue && pollId.HasValue)
                pl = FGController.get_poll(paramsContainer.Tenant.Id, pollId.Value);

            responseText = "{\"Description\":\"" + Base64.encode(description) + "\"" +
                (pl == null ? string.Empty : ",\"Poll\":" + pl.toJson()) +
                ",\"BeginDate\":\"" + (!beginDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(beginDate.Value)) + "\"" +
                ",\"FinishDate\":\"" + (!finishDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(finishDate.Value)) + "\"" +
                ",\"NotStarted\":" + (beginDate.HasValue && DateTime.Now < beginDate.Value).ToString().ToLower() +
                ",\"Finished\":" + (finishDate.HasValue && DateTime.Now > finishDate.Value).ToString().ToLower() +
                ",\"InstanceID\":\"" + (!instanceId.HasValue ? string.Empty : instanceId.ToString()) + "\"" +
                ",\"ElementsCount\":\"" + (!elementsCount.HasValue ? 0 : elementsCount.Value).ToString() + "\"" +
                ",\"FilledElementsCount\":\"" +
                    (!filledElementsCount.HasValue ? 0 : filledElementsCount.Value).ToString() + "\"" +
                ",\"AllFilledFormsCount\":\"" +
                    (!allFilledFormsCount.HasValue ? 0 : allFilledFormsCount.Value).ToString() + "\"" +
                "}";
        }

        protected void get_poll_abstract(Guid? pollId, List<FormElement> elements, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            Poll poll = !pollId.HasValue ? null : FGController.get_poll(paramsContainer.Tenant.Id, pollId.Value);

            if (poll == null || !poll.IsCopyOfPollID.HasValue)
            {
                responseText = "{\"PollIDIsNotValid\":true}";
                return;
            }

            if (elements == null || elements.Count == 0)
            {
                elements = FGController.get_element_limits(paramsContainer.Tenant.Id, poll.IsCopyOfPollID.Value);

                if (elements == null || elements.Count == 0)
                {
                    FormType ft = FGController.get_owner_form(paramsContainer.Tenant.Id, poll.IsCopyOfPollID.Value);
                    if (ft != null) elements = FGController.get_form_elements(paramsContainer.Tenant.Id, ft.FormID);
                }

                if (elements == null || elements.Count == 0)
                {
                    responseText = "{\"NoElementsFound\":true}";
                    return;
                }
            }

            Dictionary<Guid, int> counts =
                FGController.get_poll_elements_instance_count(paramsContainer.Tenant.Id, pollId.Value);

            List<PollAbstract> elementsAbstract = new List<PollAbstract>();

            elementsAbstract.AddRange(FGController.get_poll_abstract_text(paramsContainer.Tenant.Id, pollId.Value,
                counts.Where(u => elements.Any(x => u.Key == x.GetRefElementID() &&
                (x.Type == FormElementTypes.Select || x.Type == FormElementTypes.Checkbox))).Select(v => v.Key).ToList(), 100, null));
            
            elementsAbstract.AddRange(FGController.get_poll_abstract_guid(paramsContainer.Tenant.Id, pollId.Value,
                counts.Where(u => elements.Any(x => u.Key == x.GetRefElementID() &&
                (x.Type == FormElementTypes.Node || x.Type == FormElementTypes.User))).Select(v => v.Key).ToList(), 5, null));

            elementsAbstract.AddRange(FGController.get_poll_abstract_bool(paramsContainer.Tenant.Id, pollId.Value,
                counts.Where(u => elements.Any(x => u.Key == x.GetRefElementID() &&
                x.Type == FormElementTypes.Binary)).Select(v => v.Key).ToList()));

            elementsAbstract.AddRange(FGController.get_poll_abstract_number(paramsContainer.Tenant.Id, pollId.Value,
                counts.Where(u => elements.Any(x => u.Key == x.GetRefElementID() &&
                x.Type == FormElementTypes.Numeric)).Select(v => v.Key).ToList(), 5, null));

            string pollStatistics = string.Empty;
            get_form_statistics(pollId, instanceId: null, ref pollStatistics);
            if (!string.IsNullOrEmpty(pollStatistics)) pollStatistics = ",\"Statistics\":" + pollStatistics;

            responseText = "{\"Poll\":" + poll.toJson() + ",\"Items\":[" + string.Join(",", elements.Select(
                u => "{\"Count\":" + (counts.ContainsKey(u.GetRefElementID().Value) ? counts[u.GetRefElementID().Value] : 0).ToString() +
                ",\"Element\":" + u.toJson(paramsContainer.Tenant.Id) +
                (!elementsAbstract.Any(x => x.ElementID == u.GetRefElementID()) ? string.Empty : ",\"Abstract\":" +
                    elementsAbstract.Where(x => x.ElementID == u.GetRefElementID()).First().toJson()) +
                "}")) + "]" + pollStatistics + "}";
        }

        protected void get_poll_element_instances(Guid? pollId, Guid? elementId,
            int? count, int? lowerBoundary, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            Poll poll = !pollId.HasValue ? null : FGController.get_poll(paramsContainer.Tenant.Id, pollId.Value);

            /*
            if (poll == null || !poll.HideContributors.HasValue || poll.HideContributors.Value) {
                responseText = "{\"Items\":[]}";
                return;
            }
            */

            List<FormElement> items = !pollId.HasValue || !elementId.HasValue ? new List<FormElement>() :
                FGController.get_poll_element_instances(paramsContainer.Tenant.Id,
                pollId.Value, elementId.Value, count, lowerBoundary);

            if (items != null && items.Count > 0 && items[0].Type == FormElementTypes.File)
            {
                List<DocFileInfo> files = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                    items.Select(u => u.ElementID.Value).ToList());

                foreach (FormElement e in items)
                    e.AttachedFiles = files.Where(u => u.OwnerID == e.ElementID).ToList();
            }

            responseText = "{\"Items\":[" + string.Join(",", items.Select(u => u.toJson(paramsContainer.Tenant.Id, true))) + "]}";
        }

        protected void get_current_polls_count(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            int count = 0, doneCount = 0;

            FGController.get_current_polls_count(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, ref count, ref doneCount);

            responseText = "{\"Count\":" + count.ToString() + ",\"DoneCount\":" + doneCount.ToString() + "}";
        }

        protected void get_current_polls(int? count, int? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            long totalCount = 0;

            Dictionary<Guid, bool> pollIds = FGController.get_current_polls(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, count, lowerBoundary, ref totalCount);

            List<Poll> polls = FGController.get_polls(paramsContainer.Tenant.Id, pollIds.Select(u => u.Key).ToList());

            List<Poll> notDone = polls.Where(
                u => pollIds.ContainsKey(u.PollID.Value) && !pollIds[u.PollID.Value]).ToList();

            polls = polls.OrderByDescending(u => (pollIds[u.PollID.Value] ? "b" : "a") +
                (u.BeginDate.HasValue ? u.BeginDate : u.FinishDate).ToString()).ToList();

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Polls\":[" + string.Join(",", polls.Select(u => u.toJson(pollIds[u.PollID.Value]))) + "]}";
        }

        protected void import_form(Guid? instanceId, DocFileInfo uploadedFile, string map, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<FormElement> savedElements = new List<FormElement>();
            DocFileInfo logo = null;
            string nodeName = string.Empty, errorMessage = string.Empty;

            bool result = FGImport.import_form(paramsContainer.Tenant.Id, instanceId, uploadedFile, map,
                paramsContainer.CurrentUserID.Value, ref savedElements, ref nodeName, ref logo, ref errorMessage);

            if (!result)
            {
                responseText = "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                    Messages.OperationFailed.ToString() : errorMessage) + "\"}";
            }
            else
            {
                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                    ",\"NodeName\":\"" + Base64.encode(nodeName) + "\"" +
                    ",\"NodeLogo\":" + (logo == null ? "null" : logo.toJson()) +
                    ",\"Elements\":[" + string.Join(",", savedElements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]" +
                    "}";
            }
        }

        protected bool export_form(Guid? instanceId, Guid? limitOwnerId, Guid? coverId, string password, ref string responseText)
        {
            if (!paramsContainer.GBView) return false;

            FormType instance = !instanceId.HasValue ? null :
                FGController.get_form_instance(paramsContainer.Tenant.Id, instanceId.Value);

            if (instance == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return false;
            }

            bool isSystemAdmin = paramsContainer.CurrentUserID.HasValue &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            bool isDirector = false;

            List<FormElement> elements = _get_form_instance_elements(instance, limitOwnerId, isSystemAdmin, ref isDirector);

            Node ownerNode = !instance.OwnerID.HasValue ? null :
                CNController.get_node(paramsContainer.Tenant.Id, instance.OwnerID.Value);
            List<NodeCreator> contributors = new List<NodeCreator>();

            string title = "frm_" + PublicMethods.get_random_number().ToString();
            string description = "";
            List<string> tags = new List<string>();

            if (ownerNode != null && ownerNode.NodeID.HasValue)
            {
                contributors = CNController.get_node_creators(paramsContainer.Tenant.Id, ownerNode.NodeID.Value, true);

                if (contributors.Count == 0 && ownerNode.Creator.UserID.HasValue)
                {
                    User nodeCreator = UsersController.get_user(paramsContainer.Tenant.Id, ownerNode.Creator.UserID.Value);

                    if (nodeCreator != null) contributors.Add(new NodeCreator()
                    {
                        NodeID = ownerNode.NodeID,
                        User = nodeCreator
                    });
                }

                if (!string.IsNullOrEmpty(ownerNode.Name)) title = ownerNode.Name;
                if (!string.IsNullOrEmpty(ownerNode.Description)) description = ownerNode.Description;
                if (ownerNode.Tags != null && ownerNode.Tags.Count > 0) tags = ownerNode.Tags;
            }

            User currentUser = !paramsContainer.CurrentUserID.HasValue ? null :
                UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            if (currentUser == null) currentUser = new User();
            DownloadedFileMeta meta = new DownloadedFileMeta(PublicMethods.get_client_ip(HttpContext.Current),
                currentUser.UserName, currentUser.FirstName, currentUser.LastName, null);
            
            Dictionary<Guid, List<KeyValuePair<string, string>>> wikiParagraphs =
                new Dictionary<Guid, List<KeyValuePair<string, string>>>();

            elements.ForEach((val) =>
            {
                string bodyText = string.IsNullOrEmpty(val.TextValue) ? string.Empty : val.TextValue;

                switch (val.Type)
                {
                    case FormElementTypes.Text:
                        bodyText = "<p style='text-align:justify;'>" +
                            bodyText.Replace("\n", "</p><p style='text-align:justify;'>") + "</p>";
                        break;
                    case FormElementTypes.Binary:
                        //text value is valid for 'Binary' field type
                        //but other field types need some sort of transformation
                        break;
                    case FormElementTypes.Checkbox:
                    case FormElementTypes.Select:
                        bodyText = string.IsNullOrEmpty(bodyText) ? string.Empty :
                            "<ul>" + string.Join(",", bodyText.Split('~').ToList()
                            .Where(u => !string.IsNullOrEmpty(u)).Select(x => "<li>" + x + "</li>")) + "</ul>";
                        break;
                    case FormElementTypes.Date:
                        bodyText = !val.DateValue.HasValue ? bodyText :
                            PublicMethods.convert_numbers_to_local(GenericDate.get_local_date(val.DateValue.Value));
                        break;
                    case FormElementTypes.Numeric:
                        bodyText = !val.FloatValue.HasValue ? bodyText :
                            PublicMethods.convert_numbers_to_local(val.FloatValue.Value.ToString());
                        break;
                    case FormElementTypes.File:
                        List<DocFileInfo> images = val.AttachedFiles.Where(u => u.Extension.ToLower() == "jpg" ||
                            u.Extension.ToLower() == "png" || u.Extension.ToLower() == "gif" ||
                            u.Extension.ToLower() == "jpeg" || u.Extension.ToLower() == "bmp").ToList();
                        List<DocFileInfo> otherFiles = val.AttachedFiles.Where(u => !images.Any(x => x == u)).ToList();

                        bodyText = string.Empty;

                        if (otherFiles.Count > 0) bodyText += string.Join("",
                             otherFiles.Select(x => "<p><a href='" + DocumentUtilities.get_download_url(
                                 paramsContainer.Tenant.Id, x.FileID.Value) + "'>" + x.FileName + "</a></p>"));

                        if (images.Count > 0) bodyText += string.Join("",
                             images.Select(x => "<p><img src='fileid=" + x.FileID.ToString() + "' alt='" + x.FileName + "' /></p>"));

                        break;
                    case FormElementTypes.Node:
                    case FormElementTypes.User:
                        List<string> taggedItems = Expressions.get_tagged_items(bodyText).Select(u => u.toString()).ToList();

                        if (taggedItems.Count == 0 && val.GuidItems != null && val.GuidItems.Count > 0)
                            taggedItems = val.GuidItems.Select(u => u.ToString()).ToList();

                        bodyText = "<ul>" + string.Join(",", taggedItems.Select(x => "<li>" + x + "</li>")) + "</ul>";
                        break;
                    case FormElementTypes.MultiLevel:
                        {
                            Dictionary<string, object> info = PublicMethods.fromJSON(val.Info);

                            ArrayList levels = info.ContainsKey("Levels") && info["Levels"].GetType() == typeof(ArrayList) ?
                                (ArrayList)info["Levels"] : new ArrayList();

                            int counter = 0;

                            List<string> values = bodyText.Split('~').Select(u => u.Trim())
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            bodyText = string.Empty;

                            levels.ToArray().Where(u => u != null).ToList().ForEach((lvl) =>
                            {
                                if (values.Count <= counter) return;

                                string str = Base64.decode(lvl.ToString());
                                bodyText += "<p><strong>" + str + ": </strong> " + values[counter++] + "</p>";
                            });
                            break;
                        }
                    case FormElementTypes.Form:
                        {
                            Dictionary<string, object> info = PublicMethods.fromJSON(val.Info);

                            Guid? formId = !info.ContainsKey("FormID") ? null :
                                PublicMethods.parse_guid(info["FormID"].ToString());

                            if (!formId.HasValue) break;

                            List<FormElement> formElements =
                                FGController.get_form_elements(paramsContainer.Tenant.Id, formId);

                            List<FormRecord> records = FGController.get_form_records(paramsContainer.Tenant.Id,
                                formId.Value, formElements.Select(u => u.ElementID.Value).ToList(), null,
                                new List<Guid>() { val.ElementID.Value }, null, null, 1000000, null, null);

                            string recordsText = string.Join("", records.Select(u =>
                                "<tr>" + string.Join("", u.Cells.Select(
                                x => "<td style='direction:" + PublicMethods.text_direction(x.Value) + ";'>" +
                                PublicMethods.get_text_beginning(paramsContainer.Tenant.Id, x.Value, 30) + "</td>")) + "</tr>"));

                            bodyText = "<br /><table style='width:100%;' cellspacing='1' cellpadding='1' border='1'><tbody><tr>" +
                                string.Join("", formElements.Select(u => "<td style='direction:" +
                                PublicMethods.text_direction(u.Title) + "'><strong>" +
                                PublicMethods.get_text_beginning(paramsContainer.Tenant.Id, u.Title, 30).Replace("~", "-") +
                                "</strong></td>")) + "</tr>" + recordsText + "</tbody></table>";

                            break;
                        }
                    case FormElementTypes.Separator:
                        bodyText = "<br /><p style='background-color:#00517b; color:white;" +
                            "text-align:center;'>« " + val.Title + " »</p>";
                        elements.Where(u => u.ElementID == val.ElementID || u.ElementID == val.RefElementID)
                            .ToList().ForEach(x => x.Title = "");
                        break;
                }

                wikiParagraphs[val.ElementID.Value] = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("" , bodyText)
                };
            });

            //meta data
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            
            if (ownerNode != null)
            {
                metaData["Confidentiality"] = string.IsNullOrEmpty(ownerNode.ConfidentialityLevel.Title) ? "__" :
                        ownerNode.ConfidentialityLevel.Title;
                metaData["PublicationDate"] = !ownerNode.PublicationDate.HasValue ? string.Empty :
                    PublicMethods.convert_numbers_to_local(
                        GenericDate.get_local_date(ownerNode.PublicationDate.Value, reverse: true));
                metaData["RegistrationArea"] = ownerNode.AdminAreaName;
                metaData["RegistrationAreaType"] = ownerNode.AdminAreaType;
            }
            //end of meta data

            byte[] buffer = Wiki2PDF.export_as_pdf(
                applicationId: paramsContainer.Tenant.Id,
                currentUserId: paramsContainer.CurrentUserID,
                isUser: false,
                meta: meta,
                title: title,
                description: description,
                keywords: tags,
                wikiTitles: elements.Select(t => new KeyValuePair<Guid, string>(t.ElementID.Value, t.Title)).ToList(),
                wikiParagraphs: wikiParagraphs,
                metaData: metaData,
                authors: contributors.Select(u => u.User.FullName).ToList(),
                coverId: coverId.HasValue && ownerNode != null && ownerNode.NodeID.HasValue ? coverId : null,
                coverOwnerNodeId: coverId.HasValue && ownerNode != null && ownerNode.NodeID.HasValue ? ownerNode.NodeID : null,
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