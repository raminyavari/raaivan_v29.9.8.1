using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.NotificationCenter;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for UsersAPI
    /// </summary>
    public class UsersAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: false);

            bool wasAsync = false;

            if (ProcessTenantIndependentRequest(context, isAsync: ref wasAsync) || wasAsync) return;

            if (!paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return;
            }

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            Guid? tempUserId = PublicMethods.parse_guid(context.Request.Params["UserID"]);
            Guid mixedUserId = !tempUserId.HasValue ? (paramsContainer.IsAuthenticated ?
                paramsContainer.CurrentUserID.Value : Guid.Empty) : tempUserId.Value;

            string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"]);
            if (string.IsNullOrEmpty(searchText)) searchText = PublicMethods.parse_string(context.Request.Params["text"]);

            switch (command)
            {
                case "GetUsers":
                    {
                        get_users(searchText,
                            PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            PublicMethods.parse_bool(context.Request.Params["IsOnline"]),
                            PublicMethods.parse_bool(context.Request.Params["IsApproved"]),
                            ListMaker.get_guid_items(context.Request.Params["UserIDs"], '|'),
                            PublicMethods.parse_bool(context.Request.Params["Department"]),
                            ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                            PublicMethods.parse_bool(context.Request.Params["LockedStatus"]),
                            PublicMethods.parse_bool(context.Request.Params["ApprovedStatus"]),
                            ref responseText);
                        _return_response(ref context, ref responseText);
                        return;
                    }
                case "AdvancedUserSearch":
                    advanced_user_search(searchText,
                        ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Members"]),
                        PublicMethods.parse_bool(context.Request.Params["Experts"]),
                        PublicMethods.parse_bool(context.Request.Params["Contributors"]),
                        PublicMethods.parse_bool(context.Request.Params["PropertyOwners"]),
                        PublicMethods.parse_bool(context.Request.Params["Resume"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AdvancedUserSearchMeta":
                    advanced_user_search_meta(PublicMethods.parse_guid(context.Request.Params["UserID"]), searchText,
                        ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["Members"]),
                        PublicMethods.parse_bool(context.Request.Params["Experts"]),
                        PublicMethods.parse_bool(context.Request.Params["Contributors"]),
                        PublicMethods.parse_bool(context.Request.Params["PropertyOwners"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "CreateUser":
                    create_user(PublicMethods.parse_string(context.Request.Params["UserName"]),
                        PublicMethods.parse_string(context.Request.Params["FirstName"]),
                        PublicMethods.parse_string(context.Request.Params["LastName"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetRandomPassword":
                    set_random_password(PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "IsApproved":
                    is_approved(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_bool(context.Request.Params["IsApproved"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "UnlockUser":
                    unlock_user(PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetCurrentUser":
                case "GetUser":
                    string strGfn = PublicMethods.parse_string(context.Request.Params["GetPhoneNumbers"], false);
                    string strGe = PublicMethods.parse_string(context.Request.Params["GetEmails"], false);

                    bool hidePhoneIfNotFriends = !string.IsNullOrEmpty(strGfn) && strGfn.ToLower() == "iffriends";
                    bool hideMailIfNotFriends = !string.IsNullOrEmpty(strGe) && strGe.ToLower() == "iffriends";

                    bool? getPhoneNumbers = PublicMethods.parse_bool(context.Request.Params["GetPhoneNumbers"]);
                    bool? getEmails = PublicMethods.parse_bool(context.Request.Params["GetEmails"]);

                    //Guid? userId = command == "GetUser" ?
                    //    PublicMethods.parse_guid(context.Request.Params["UserID"]) : paramsContainer.CurrentUserID;

                    Guid? userId =
                        PublicMethods.parse_guid(context.Request.Params["UserID"], alternatvieValue: paramsContainer.CurrentUserID);

                    get_user(userId,
                        hidePhoneIfNotFriends || (getPhoneNumbers.HasValue && getPhoneNumbers.Value),
                        hideMailIfNotFriends || (getEmails.HasValue && getEmails.Value),
                        hidePhoneIfNotFriends, hideMailIfNotFriends,
                        ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'), ref responseText);

                    _return_response(ref context, ref responseText);
                    return;
                case "UpdateFriendSuggestions":
                    update_friend_suggestions(mixedUserId,
                        PublicMethods.parse_bool(context.Request.Params["Full"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFriendSuggestions":
                    get_friend_suggestions(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        ListMaker.get_guid_items(context.Request.Params["MembershipNodeTypeIDs"], '|'),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFriends":
                    get_friends(mixedUserId,
                        ListMaker.get_guid_items(context.Request.Params["FriendIDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["MutualsCount"]), searchText,
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_bool(context.Request.Params["Online"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetEmailContactsStatus":
                    get_email_contacts_status(mixedUserId,
                        ListMaker.get_string_items(context.Request.Params["Emails"], '|').Select(u => Base64.decode(u)).ToList(),
                        PublicMethods.parse_bool(context.Request.Params["SaveEmails"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFriendshipRequests":
                    get_friendship_requests(mixedUserId,
                        PublicMethods.parse_bool(context.Request.Params["Sent"]),
                        PublicMethods.parse_bool(context.Request.Params["MutualsCount"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SendFriendRequest":
                    send_friendship_request(PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFriendshipStatus":
                    get_friendship_status(mixedUserId,
                        ListMaker.get_guid_items(context.Request.Params["OtherUserIDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["MutualsCount"]),
                        ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AcceptFriendship":
                    accept_friendship(PublicMethods.parse_guid(context.Request.Params["OtherUserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RejectFriendship":
                    reject_friendship(PublicMethods.parse_guid(context.Request.Params["OtherUserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetFriendRequestsCount":
                    get_friend_requests_count(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetLastContentCreators":
                    get_last_content_creators(PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddRemoteServer":
                case "ModifyRemoteServer":
                    add_modify_remote_server(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]),
                        PublicMethods.parse_string(context.Request.Params["RemoteURL"]),
                        PublicMethods.parse_string(context.Request.Params["UserName"]),
                        PublicMethods.parse_string(context.Request.Params["Password"]),
                        command == "AddRemoteServer", ref responseText);

                    paramsContainer.return_response(ref responseText);
                    return;
                case "RemoveRemoteServer":
                    remove_remote_server(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    paramsContainer.return_response(ref responseText);
                    return;
                case "GetRemoteServers":
                    get_remote_servers(ref responseText);
                    paramsContainer.return_response(ref responseText);
                    return;
                case "SetCity":
                    set_city(mixedUserId, PublicMethods.parse_string(context.Request.Params["City"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetOrganization":
                    set_organization(mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["Organization"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetDepartment":
                    set_department(mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["Department"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetJobTitle":
                    set_job_title(mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["JobTitle"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetEmploymentTypes":
                    get_employment_types(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetEmploymentType":
                    EmploymentType employmentType = PublicMethods.parse_enum<EmploymentType>(
                        context.Request.Params["EmploymentType"], defaultValue: EmploymentType.NotSet);

                    set_employment_type(mixedUserId, employmentType, ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetBirthday":
                    set_birthday(mixedUserId,
                        PublicMethods.parse_date(context.Request.Params["Birthday"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetPhoneNumber":
                    {
                        PhoneType phoneType = PublicMethods.parse_enum<PhoneType>(
                            context.Request.Params["PhoneNumberType"], defaultValue: PhoneType.NotSet);

                        set_phone_number(mixedUserId,
                            PublicMethods.parse_string(context.Request.Params["PhoneNumber"]), phoneType, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "EditPhoneNumber":
                    {
                        PhoneType phoneType = PublicMethods.parse_enum<PhoneType>(
                            context.Request.Params["PhoneNumberType"], defaultValue: PhoneType.NotSet);

                        edit_phone_number(PublicMethods.parse_guid(context.Request.Params["NumberID"]),
                            PublicMethods.parse_string(context.Request.Params["PhoneNumber"]), phoneType, ref responseText);
                        _return_response(ref context, ref responseText);
                    }
                    return;
                case "RemovePhoneNumber":
                    remove_phone_number(PublicMethods.parse_guid(context.Request.Params["NumberID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetMainPhone":
                    set_main_phone(PublicMethods.parse_guid(context.Request.Params["NumberID"]),
                        PublicMethods.parse_string(context.Request.Params["VerificationToken"], false),
                        PublicMethods.parse_long(context.Request.Params["Code"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetEmailAddress":
                    set_email_address(mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["Address"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetEmailAddresses":
                    get_email_addresses(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_string(context.Request.Params["UserName"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "EditEmailAddress":
                    edit_email_address(PublicMethods.parse_guid(context.Request.Params["EmailID"]),
                        PublicMethods.parse_string(context.Request.Params["Address"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveEmailAddress":
                    remove_email_address(PublicMethods.parse_guid(context.Request.Params["EmailID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetMainEmail":
                    set_main_email(PublicMethods.parse_guid(context.Request.Params["EmailID"]),
                        PublicMethods.parse_string(context.Request.Params["VerificationToken"], false),
                        PublicMethods.parse_long(context.Request.Params["Code"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetResumeConstantInfo":
                    get_constant_info(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetResumeInfo":
                    get_resume_info(mixedUserId, ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetLanguages":
                    get_languages(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetJobExperience":
                    set_job_experience(PublicMethods.parse_guid(context.Request.Params["JobID"]), mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Employer"]),
                        PublicMethods.parse_date(context.Request.Params["StartDate"]),
                        PublicMethods.parse_date(context.Request.Params["EndDate"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetEducationalExperience":
                    EducationalLevel degree = PublicMethods.parse_enum<EducationalLevel>(
                        context.Request.Params["Level"], defaultValue: EducationalLevel.None);

                    GraduateDegree educationLevel = PublicMethods.parse_enum<GraduateDegree>(
                        context.Request.Params["GraduateDegree"], defaultValue: GraduateDegree.None);

                    set_educational_experience(PublicMethods.parse_guid(context.Request.Params["EducationID"]), mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["School"]),
                        PublicMethods.parse_string(context.Request.Params["StudyField"]), degree, educationLevel,
                        PublicMethods.parse_date(context.Request.Params["StartDate"]),
                        PublicMethods.parse_date(context.Request.Params["EndDate"]), true, ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetInstituteEducationalExperience":
                    set_educational_experience(PublicMethods.parse_guid(context.Request.Params["EducationID"]), mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["School"]),
                        PublicMethods.parse_string(context.Request.Params["StudyField"]), null, null,
                        PublicMethods.parse_date(context.Request.Params["StartDate"]),
                        PublicMethods.parse_date(context.Request.Params["EndDate"]), false, ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetHonorAndAward":
                    set_honor_and_award(PublicMethods.parse_guid(context.Request.Params["HonorID"]), mixedUserId,
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Occupation"]),
                        PublicMethods.parse_string(context.Request.Params["Issuer"]),
                        PublicMethods.parse_date(context.Request.Params["IssueDate"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetLanguage":
                    LanguageLevel langLevel = PublicMethods.parse_enum<LanguageLevel>(
                        context.Request.Params["LanguageLevel"], defaultValue: LanguageLevel.None);

                    set_language(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["LanguageName"]),
                        mixedUserId, langLevel, ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveJobExperience":
                    remove_job_experience(PublicMethods.parse_guid(context.Request.Params["JobID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveEducationalExperience":
                    remove_education_experience(PublicMethods.parse_guid(context.Request.Params["EducationID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveHonorAndAward":
                    remove_honor_and_award(PublicMethods.parse_guid(context.Request.Params["HonorID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveLanguage":
                    remove_language(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "CreateUserGroup":
                    create_user_group(PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RenameUserGroup":
                    rename_user_group(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveUserGroup":
                    remove_user_group(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetUserGroups":
                    get_user_groups(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "AddUserGroupMember":
                    add_user_group_member(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "RemoveUserGroupMember":
                    remove_user_group_member(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetUserGroupMembers":
                    get_user_group_members(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetAccessRoles":
                    get_access_roles(ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "GetUserGroupPermissions":
                    get_user_group_permissions(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "SetUserGroupPermission":
                    set_user_group_permission(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_guid(context.Request.Params["GroupID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
                case "UnsetUserGroupPermission":
                    unset_user_group_permission(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_guid(context.Request.Params["GroupID"]), ref responseText);
                    _return_response(ref context, ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        public bool ProcessTenantIndependentRequest(HttpContext context, ref bool isAsync)
        {
            isAsync = false;

            if (!RaaiVanSettings.SAASBasedMultiTenancy && !paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return true;
            }

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            Guid? tempUserId = PublicMethods.parse_guid(context.Request.Params["UserID"]);
            Guid mixedUserId = !tempUserId.HasValue ? (paramsContainer.IsAuthenticated ?
                paramsContainer.CurrentUserID.Value : Guid.Empty) : tempUserId.Value;

            string searchText = PublicMethods.parse_string(context.Request.Params["SearchText"]);
            if (string.IsNullOrEmpty(searchText)) searchText = PublicMethods.parse_string(context.Request.Params["text"]);

            switch (command)
            {
                case "CheckUserName":
                    {
                        responseText = (UsersController.get_user_id(paramsContainer.ApplicationID,
                            PublicMethods.parse_string(context.Request.Params["UserName"])).HasValue)
                            .ToString().ToLower();
                    }
                    break;
                case "GetPasswordPolicy":
                    get_password_policy(ref responseText);
                    break;
                case "CreateUserToken":
                    if (!Captcha.check(context, PublicMethods.parse_string(context.Request.Params["Captcha"], decode: false)))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.CaptchaIsNotValid + "\"}";
                        break;
                    }

                    create_user_token(PublicMethods.parse_string(context.Request.Params["UserName"]),
                        PublicMethods.parse_string(context.Request.Params["FirstName"]),
                        PublicMethods.parse_string(context.Request.Params["LastName"]),
                        PublicMethods.parse_string(context.Request.Params["Contact"]),
                        PublicMethods.parse_string(context.Request.Params["Password"]),
                        PublicMethods.parse_guid(context.Request.Params["InvitationID"]),
                        ref responseText);
                    break;
                case "ValidateUserCreation":
                    validate_user_creation(PublicMethods.parse_string(context.Request.Params["VerificationToken"], false),
                        PublicMethods.parse_long(context.Request.Params["Code"]),
                        PublicMethods.parse_bool(context.Request.Params["Login"]),
                        ref responseText);
                    break;
                case "SignInWithGoogle":
                    {
                        if (!Captcha.check(context, PublicMethods.parse_string(context.Request.Params["Captcha"], decode: false)))
                        {
                            responseText = "{\"ErrorText\":\"" + Messages.CaptchaIsNotValid + "\"}";
                            break;
                        }

                        LoginUtil loginUtil = new LoginUtil(paramsContainer: paramsContainer,
                            invitationId: PublicMethods.parse_guid(context.Request.Params["InvitationID"]));

                        loginUtil.login_with_google(
                            token: PublicMethods.parse_string(context.Request.Params["GoogleToken"], false),
                            email: PublicMethods.parse_string(context.Request.Params["Email"], decode: false),
                            firstName: PublicMethods.parse_string(context.Request.Params["FirstName"]),
                            lastName: PublicMethods.parse_string(context.Request.Params["LastName"]),
                            googleId: PublicMethods.parse_string(context.Request.Params["GoogleID"], decode: false),
                            imageUrl: PublicMethods.parse_string(context.Request.Params["ImageURL"], decode: false),
                            callback: (res) => paramsContainer.return_response(res));

                        isAsync = true;
                    }
                    break;
                case "CreateTemporaryUser":
                    if (!Captcha.check(context, PublicMethods.parse_string(context.Request.Params["Captcha"])))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.CaptchaIsNotValid + "\"}";
                        break;
                    }

                    create_temporary_user(PublicMethods.parse_string(context.Request.Params["UserName"]),
                        PublicMethods.parse_string(context.Request.Params["FirstName"]),
                        PublicMethods.parse_string(context.Request.Params["LastName"]),
                        PublicMethods.parse_string(context.Request.Params["Email"]),
                        PublicMethods.parse_string(context.Request.Params["Password"]),
                        PublicMethods.parse_guid(context.Request.Params["InvitationID"]),
                        ref responseText);
                    break;
                case "ActivateTemporaryUser":
                    activate_temporary_user(context.Request.Params["ActivationCode"], ref responseText);
                    break;
                case "IsInvited":
                    is_invited(PublicMethods.parse_guid(context.Request.Params["ApplicationID"]),
                        Base64.decode(context.Request.Params["Email"]), ref responseText);
                    break;
                case "InviteUser":
                    invite_user(PublicMethods.parse_guid(context.Request.Params["ApplicationID"]),
                        Base64.decode(context.Request.Params["Email"]),
                        Base64.decode(context.Request.Params["FullName"]),
                        Base64.decode(context.Request.Params["MessageText"]),
                        ref responseText);
                    break;
                case "BatchInviteUsers":
                    batch_invite_users(
                        PublicMethods.parse_guid(context.Request.Params["ApplicationID"]),
                        ListMaker.get_string_items(context.Request.Params["Emails"], '|').Select(u => Base64.decode(u)).ToList(),
                        PublicMethods.parse_string(context.Request.Params["MessageText"]),
                        ref responseText);
                    break;
                case "GetInvitedUsersCount":
                    get_invited_users_count(PublicMethods.parse_guid(context.Request.Params["ApplicationID"]), ref responseText);
                    break;
                case "GetUserInvitations":
                    get_user_invitations(PublicMethods.parse_guid(context.Request.Params["ApplicationID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        ref responseText);
                    break;
                case "GetCurrentInvitations":
                    get_current_invitations(ref responseText);
                    break;
                case "SetPasswordResetTicket":
                    if (!Captcha.check(context, PublicMethods.parse_string(context.Request.Params["Captcha"], decode: false)))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.CaptchaIsNotValid + "\"}";
                        break;
                    }

                    set_password_reset_ticket(PublicMethods.parse_string(context.Request.Params["UserName"]),
                        PublicMethods.parse_string(context.Request.Params["Password"]),
                        PublicMethods.parse_guid(context.Request.Params["InvitationID"]),
                        ref responseText);
                    break;
                case "SetPassword":
                    set_password(PublicMethods.parse_string(context.Request.Params["VerificationToken"], false),
                        PublicMethods.parse_long(context.Request.Params["Code"]),
                        PublicMethods.parse_bool(context.Request.Params["Login"]),
                        ref responseText);
                    break;
                case "ChangePassword":
                    change_password(PublicMethods.parse_string(context.Request.Params["CurrentPassword"]),
                        PublicMethods.parse_string(context.Request.Params["NewPassword"]),
                        ref responseText);
                    break;
                case "ValidatePassword":
                    validate_password(PublicMethods.parse_string(context.Request.Params["Password"]), ref responseText);
                    break;
                case "ModifyEmailTicket":
                    if (!Captcha.check(context, PublicMethods.parse_string(context.Request.Params["Captcha"], decode: false)))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.CaptchaIsNotValid + "\"}";
                        break;
                    }
                    
                    modify_email_ticket(PublicMethods.parse_guid(context.Request.Params["EmailID"]), 
                        PublicMethods.parse_string(context.Request.Params["Address"]),
                        ref responseText);
                    break;
                case "ModifyEmail":
                    modify_email(PublicMethods.parse_string(context.Request.Params["VerificationToken"], false),
                        PublicMethods.parse_long(context.Request.Params["Code"]),
                        ref responseText);
                    break;
                case "SetTheme":
                    set_theme(PublicMethods.parse_string(context.Request.Params["Theme"], false), ref responseText);
                    break;
                case "GetTheme":
                    get_theme(ref responseText);
                    break;
                case "SetVerificationCodeMedia":
                    {
                        TwoStepAuthentication media = PublicMethods.parse_enum<TwoStepAuthentication>(
                            PublicMethods.parse_string(context.Request.Params["Media"], decode: false), TwoStepAuthentication.None);

                        set_verification_code_media(media, ref responseText);
                    }
                    break;
                case "SaveUserSettingsItem":
                    {
                        save_user_settings_item(PublicMethods.parse_string(context.Request.Params["Name"], decode: false),
                            PublicMethods.parse_string(context.Request.Params["Value"], decode: false),
                            PublicMethods.parse_string(context.Request.Params["Type"], decode: false), 
                            ref responseText);
                    }
                    break;
                case "GetApplicationUsers":
                    {
                        get_application_users(PublicMethods.parse_guid(context.Request.Params["ApplicationID"]),
                            searchText,
                            PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"]),
                            ref responseText);
                        break;
                    }
                case "SetUserName":
                    set_user_name(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_string(context.Request.Params["UserName"]), ref responseText);
                    break;
                case "SetFirstAndLastName":
                    set_first_and_last_name(mixedUserId, PublicMethods.parse_string(context.Request.Params["FirstName"]),
                        PublicMethods.parse_string(context.Request.Params["LastName"]), ref responseText);
                    break;
                case "SetFirstName":
                    set_first_name(mixedUserId, PublicMethods.parse_string(context.Request.Params["FirstName"]), ref responseText);
                    break;
                case "SetLastName":
                    set_last_name(mixedUserId, PublicMethods.parse_string(context.Request.Params["LastName"]), ref responseText);
                    break;
                case "SetAboutMe":
                    set_about_me(mixedUserId, PublicMethods.parse_string(context.Request.Params["Text"]), ref responseText);
                    break;
            }

            if (!string.IsNullOrEmpty(responseText))
                paramsContainer.return_response(ref responseText);

            return !string.IsNullOrEmpty(responseText);
        }

        protected void _return_response(ref HttpContext httpContext, ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected string _get_user_json(User usr)
        {
            if (usr == null) return "null";

            return "{\"UserID\":\"" + usr.UserID.Value.ToString() + "\"" +
                ",\"FirstName\":\"" + Base64.encode(usr.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(usr.LastName) + "\"" +
                ",\"UserName\":\"" + Base64.encode(usr.UserName) + "\"" +
                ",\"JobTitle\":\"" + Base64.encode(usr.JobTitle) + "\"" +
                ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                    usr.UserID.Value) + "\"" +
                "}";
        }

        protected void get_password_policy(ref string responseText)
        {
            responseText = "{\"R\":true" +
                (RaaiVanSettings.Users.PasswordPolicy.MinLength(paramsContainer.ApplicationID) <= 0 ? string.Empty :
                    ",\"MinLength\":" + RaaiVanSettings.Users.PasswordPolicy.MinLength(paramsContainer.ApplicationID).ToString()) +
                    (RaaiVanSettings.Users.PasswordPolicy.NewCharacters(paramsContainer.ApplicationID) <= 0 ? string.Empty :
                    ",\"NewCharacters\":" + RaaiVanSettings.Users.PasswordPolicy.NewCharacters(paramsContainer.ApplicationID).ToString()) +
                (!RaaiVanSettings.Users.PasswordPolicy.UpperLower(paramsContainer.ApplicationID) ? string.Empty : ",\"UpperLower\":true") +
                (!RaaiVanSettings.Users.PasswordPolicy.NonAlphabetic(paramsContainer.ApplicationID) ? string.Empty : ",\"NonAlphabetic\":true") +
                (!RaaiVanSettings.Users.PasswordPolicy.Number(paramsContainer.ApplicationID) ? string.Empty : ",\"Number\":true") +
                (!RaaiVanSettings.Users.PasswordPolicy.NonAlphaNumeric(paramsContainer.ApplicationID) ? string.Empty : ",\"NonAlphaNumeric\":true") +
                "}";
        }

        protected void get_users(string searchText, long? lowerBoundary, int? count, bool? isOnline, bool? isApproved, List<Guid> userIds,
            bool? department, List<Guid> nodeTypeIds, bool? lockedStatus, bool? approvedStatus, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!paramsContainer.GBEdit ||
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID))
                lockedStatus = approvedStatus = false;

            if (!count.HasValue || count.Value <= 0) count = 20;

            long totalCount = (long)userIds.Count;
            if(!lockedStatus.HasValue || !lockedStatus.Value) isApproved = true;

            List<User> Users = userIds.Count > 0 ?
                UsersController.get_users(paramsContainer.Tenant.Id, userIds) :
                UsersController.get_users(paramsContainer.Tenant.Id,
                    searchText, lowerBoundary, count, isOnline, ref totalCount, isApproved);

            List<NodeMember> nodeMembers = nodeTypeIds.Count == 0 ?
                (department.HasValue && department.Value ?
                CNController.get_users_departments(paramsContainer.Tenant.Id, Users.Select(u => u.UserID.Value).ToList()) : new List<NodeMember>()) :
                CNController.get_member_nodes(paramsContainer.Tenant.Id, Users.Select(u => u.UserID.Value).ToList(), nodeTypeIds);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Users\":[";

            bool isFirst = true;
            foreach (User User in Users)
            {
                responseText += (isFirst ? string.Empty : ",") + "{\"UserID\":\"" + User.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(User.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(User.LastName) + "\"" +
                    ",\"FullName\":\"" + Base64.encode(User.FullName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(User.UserName) + "\"" +
                    ",\"JobTitle\":\"" + Base64.encode(User.JobTitle) + "\"" +
                    ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        User.UserID.Value) + "\"" +
                    (!lockedStatus.HasValue || !lockedStatus.Value ? string.Empty :
                        ",\"IsLockedOut\":" + (User.IsLockedOut.HasValue && User.IsLockedOut.Value).ToString().ToLower()) +
                    (!approvedStatus.HasValue || !approvedStatus.Value ? string.Empty :
                        ",\"IsApproved\":" + (User.IsApproved.HasValue && User.IsApproved.Value).ToString().ToLower());
                isFirst = false;

                if (nodeMembers.Count > 0)
                {
                    List<NodeMember> gs = nodeMembers.Where(u => u.Member.UserID == User.UserID).ToList();

                    responseText += ",\"Nodes\":[" + ProviderUtil.list_to_string<string>(gs.Select(
                        u => "{\"NodeID\":\"" + u.Node.NodeID.ToString() + "\"" +
                            ",\"Name\":\"" + Base64.encode(u.Node.Name) + "\"" +
                            ",\"NodeTypeID\":\"" + u.Node.NodeTypeID.ToString() + "\"" +
                            "}").ToList()) + "]";
                }

                responseText += "}";
            }

            responseText += "]}";
        }

        protected void get_application_users(Guid? applicationId, 
            string searchText, long? lowerBoundary, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!applicationId.HasValue ||
                GlobalController.get_user_applications(paramsContainer.CurrentUserID.Value, archive: null)
                .Where(a => a.ApplicationID == applicationId).FirstOrDefault() == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }
            
            if (!count.HasValue || count.Value <= 0) count = 20;

            long totalCount = 0;
            
            List<User> users = UsersController.get_users(applicationId.Value,
                searchText, lowerBoundary, count, null, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Users\":[" +
                string.Join(",", users.Select(u => u.toJson(applicationId.Value, true))) + "]}";
        }

        protected void advanced_user_search(string searchText, List<Guid> nodeTypeIds, List<Guid> nodeIds,
            bool? members, bool? experts, bool? contributors, bool? propertyOwners, bool? resume,
            long? lowerBoundary, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            searchText = PublicMethods.convert_numbers_from_local(searchText);

            long totalCount = 0;

            List<AdvancedUserSearch> meta = UsersController.advanced_user_search(paramsContainer.Tenant.Id,
                searchText, nodeTypeIds, nodeIds, members, experts, contributors, propertyOwners, resume,
                lowerBoundary, count, ref totalCount);

            List<User> users =
                UsersController.get_users(paramsContainer.Tenant.Id, meta.Select(u => u.UserID.Value).ToList());

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Users\":[";

            bool isFirst = true;

            for (int i = 0, lnt = meta.Count; i < lnt; ++i)
            {
                AdvancedUserSearch m = meta[i];
                User user = users.Where(u => u.UserID == m.UserID).FirstOrDefault();

                if (user == null) continue;

                responseText += (isFirst ? string.Empty : ",") +
                    "{\"IsMember\":" + (!m.IsMemberCount.HasValue ? 0 : m.IsMemberCount.Value).ToString() +
                    ",\"IsExpert\":" + (!m.IsExpertCount.HasValue ? 0 : m.IsExpertCount.Value).ToString() +
                    ",\"IsContributor\":" + (!m.IsContributorCount.HasValue ? 0 : m.IsContributorCount.Value).ToString() +
                    ",\"HasProperty\":" + (!m.HasPropertyCount.HasValue ? 0 : m.HasPropertyCount.Value).ToString() +
                    ",\"Resume\":" + (!m.Resume.HasValue ? 0 : m.Resume.Value).ToString() +
                    ",\"User\":" + _get_user_json(user) +
                    "}";

                isFirst = false;
            }

            responseText += "]}";
        }

        protected void advanced_user_search_meta(Guid? userId, string searchText, List<Guid> nodeTypeIds,
            List<Guid> nodeIds, bool? members, bool? experts, bool? contributors, bool? propertyOwners,
            ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            searchText = PublicMethods.convert_numbers_from_local(searchText);

            List<AdvancedUserSearchMeta> meta = !userId.HasValue ? new List<AdvancedUserSearchMeta>() :
                UsersController.advanced_user_search_meta(paramsContainer.Tenant.Id, userId.Value, searchText,
                nodeTypeIds, nodeIds, members, experts, contributors, propertyOwners);

            List<Modules.CoreNetwork.Node> nodes = CNController.get_nodes(paramsContainer.Tenant.Id,
                meta.Select(u => u.NodeID.Value).ToList(), full: null, currentUserId: null);

            responseText = "{\"Nodes\":[";

            bool isFirst = true;

            for (int i = 0, lnt = meta.Count; i < lnt; ++i)
            {
                AdvancedUserSearchMeta m = meta[i];
                Modules.CoreNetwork.Node node = nodes.Where(u => u.NodeID == m.NodeID).FirstOrDefault();

                if (node == null) continue;

                responseText += (isFirst ? string.Empty : ",") + "{\"NodeID\":\"" + m.NodeID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(node.Name) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(node.NodeType) + "\"" +
                    ",\"AdditionalID\":\"" + Base64.encode(node.AdditionalID) + "\"" +
                    ",\"IsMember\":" + (m.IsMember.HasValue && m.IsMember.Value).ToString().ToLower() +
                    ",\"IsExpert\":" + (m.IsExpert.HasValue && m.IsExpert.Value).ToString().ToLower() +
                    ",\"IsContributor\":" + (m.IsContributor.HasValue && m.IsContributor.Value).ToString().ToLower() +
                    ",\"HasProperty\":" + (m.HasProperty.HasValue && m.HasProperty.Value).ToString().ToLower() +
                    "}";

                isFirst = false;
            }

            responseText += "]}";
        }

        public void create_user_token(string username, string firstName, string lastName, string contact,
            string password, Guid? invitationId, ref string responseText)
        {
            //Privacy Check: OK
            if (!RaaiVanSettings.UserSignUp(paramsContainer.ApplicationID) &&
                !RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID)) return;

            if (invitationId.HasValue &&
                !UsersController.get_invitation_application_id(invitationId.Value, checkIfNotUsed: true).HasValue)
                invitationId = null;


            //Check Password Policy
            if (!UserUtilities.check_password_policy(paramsContainer.ApplicationID, password, string.Empty))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PasswordPolicyDidntMeet.ToString() + "\"}";
                return;
            }
            //end of Check Password Policy


            //Check UserName
            if (!string.IsNullOrEmpty(username))
            {
                if (username.Length > 250 || string.IsNullOrEmpty(password))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                    return;
                }
                else if (!PublicMethods.is_valid_username(paramsContainer.ApplicationID, username))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.UserNamePatternIsNotValid + "\"}";
                    return;
                }
                else if (UsersController.get_user(paramsContainer.ApplicationID, username) != null)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.UserNameAlreadyExists + "\"}";
                    return;
                }
            }
            else {
                while (string.IsNullOrEmpty(username) || UsersController.get_user(paramsContainer.ApplicationID, username) != null)
                    username = PublicMethods.random_string(8).ToLower();
            }
            //end of Check UserName


            bool isEmail = PublicMethods.is_email(contact);
            bool isNumber = PublicMethods.parse_long(contact).HasValue;

            string email = isEmail ? contact : string.Empty;
            string phone = !isEmail && isNumber ? contact : string.Empty;

            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone)) {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidInput + "\"}";
                return;
            }
            else if (!string.IsNullOrEmpty(email) && 
                UsersController.get_email_owners(paramsContainer.ApplicationID, new List<string>() { email })
                    .Any(e => e.IsMain.HasValue && e.IsMain.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.EmailAlreadyExists + "\"}";
                return;
            }
            else if (!string.IsNullOrEmpty(phone) && 
                UsersController.get_phone_owners(paramsContainer.ApplicationID, new List<string>() { phone })
                    .Any(e => e.IsMain.HasValue && e.IsMain.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PhoneNumberAlreadyExists + "\"}";
                return;
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["UserName"] = username;
            customData["FirstName"] = firstName;
            customData["LastName"] = lastName;
            customData["Email"] = email;
            customData["Phone"] = phone;
            customData["Password"] = password;
            if (invitationId.HasValue) customData["InvitationID"] = invitationId.Value;

            VerificationCode.process_request(paramsContainer.ApplicationID, emailAddress: email, 
                phoneNumber: phone, responseText: ref responseText, customData: PublicMethods.toJSON(customData));
        }

        public void validate_user_creation(string token, long? code, bool? login, ref string responseText)
        {
            //Privacy Check: OK
            if (!RaaiVanSettings.UserSignUp(paramsContainer.ApplicationID) &&
                !RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID)) return;

            if (string.IsNullOrEmpty(token) || !code.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidInput + "\"}";
                return;
            }

            string customData = string.Empty;

            bool result = VerificationCode.process_request(paramsContainer.ApplicationID, token: token, code: code, ref customData);

            if (!result)
            {
                responseText = customData;
                return;
            }

            Dictionary<string, object> dic = PublicMethods.fromJSON(customData);

            LoginUtil loginUtil = new LoginUtil(paramsContainer: paramsContainer,
                invitationId: PublicMethods.parse_guid(PublicMethods.get_dic_value(dic, "InvitationID")));

            responseText = loginUtil.login_via_signup(
                username: PublicMethods.get_dic_value(dic, "UserName"),
                password: PublicMethods.get_dic_value(dic, "Password"),
                email: PublicMethods.get_dic_value(dic, "Email"),
                phone: PublicMethods.get_dic_value(dic, "Phone"),
                firstName: PublicMethods.get_dic_value(dic, "FirstName"),
                lastName: PublicMethods.get_dic_value(dic, "Password"),
                login: login.HasValue && login.Value);
        }

        public void create_temporary_user(string username, string firstName, string lastName, string email,
            string password, Guid? invitationId, ref string responseText)
        {
            //Privacy Check: OK
            if (!RaaiVanSettings.UserSignUp(paramsContainer.ApplicationID) &&
                !RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID)) return;
            
            if (invitationId.HasValue && 
                !UsersController.get_invitation_application_id(invitationId.Value, checkIfNotUsed: true).HasValue) {
                responseText = "{\"ErrorText\":\"" + Messages.TheInvitationHasBeenUsed + "\"}";
                return;
            }

            if (!PublicMethods.is_valid_username(paramsContainer.ApplicationID, username))
            {
                responseText = "{\"ErrorText\":\"" + Messages.UserNamePatternIsNotValid + "\"}";
                return;
            }

            //Check Password Policy
            if (!UserUtilities.check_password_policy(paramsContainer.ApplicationID, password, string.Empty))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PasswordPolicyDidntMeet.ToString() + "\"}";
                return;
            }
            //end of Check Password Policy

            User newUser = new User()
            {
                UserID = Guid.NewGuid(),
                UserName = username,
                FirstName = firstName,
                LastName = lastName,
                PasswordSalt = UserUtilities.generate_password_salt(),
                Password = password
            };

            newUser.SaltedPassword = UserUtilities.encode_password(newUser.Password, newUser.PasswordSalt);

            if (string.IsNullOrEmpty(email) || !PublicMethods.is_email(email))
            {
                responseText = Messages.EmailIsNotValid.ToString();
                return;
            }

            newUser.Emails.Add(new EmailAddress() { Address = email });

            string activationCode = Guid.NewGuid().ToString();

            Dictionary<string, string> dic = new Dictionary<string, string>();

            DateTime now = DateTime.Now;
            string gDate = now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
            string pDate = GenericDate.get_local_date(now);

            dic.Add("Password", newUser.Password);
            dic.Add("UserName", newUser.UserName);
            dic.Add("FirstName", newUser.FirstName);
            dic.Add("LastName", newUser.LastName);
            dic.Add("FullName", newUser.FirstName + " " + newUser.LastName);
            dic.Add("ActivationCode", activationCode);
            dic.Add("SystemURL", RaaiVanSettings.RaaiVanURL(paramsContainer.ApplicationID));
            dic.Add("LoginURL", PublicConsts.LoginPage.Replace("~", dic["SystemURL"]));
            dic.Add("Now", gDate);
            dic.Add("GNow", gDate);
            dic.Add("PNow", pDate);

            string confirmationEmailBody = EmailTemplates.get_email_template(paramsContainer.ApplicationID,
                EmailTemplateType.CreateAccount, dic);
            string emailSubject = EmailTemplates.get_email_subject_template(paramsContainer.ApplicationID,
                EmailTemplateType.CreateAccount, dic);

            string errorMessage = string.Empty;

            bool result = UsersController.create_temporary_user(paramsContainer.ApplicationID, newUser, false, 
                emailSubject, confirmationEmailBody, null, activationCode, invitationId, ref errorMessage);
            
            responseText = result ? "{\"Succeed\":\"" + Messages.YourUserAccountCreatedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";
        }

        public void activate_temporary_user(string activationCode, ref string responseText)
        {
            //Privacy Check: OK

            if (!RaaiVanSettings.UserSignUp(paramsContainer.ApplicationID) &&
                !RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID)) return;

            string errorMessage = string.Empty;

            bool result = UsersController.activate_temporary_user(paramsContainer.ApplicationID,
                activationCode, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.YourUserAccountActivatedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";
        }

        public void is_invited(Guid? applicationId, string email, ref string responseText)
        {
            //Privacy Check: OK

            if (!RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID) && !RaaiVanSettings.SAASBasedMultiTenancy) return;

            bool result = applicationId.HasValue && PublicMethods.is_email(email) &&
                UsersController.is_invited(applicationId.Value, email);

            responseText = "{\"Invited\":" + result.ToString().ToLower() + "}";
        }

        public void invite_user(Guid? applicationId, string email, string fullname, string messageText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit || 
                (!RaaiVanSettings.SignUpViaInvitation(paramsContainer.ApplicationID) && !RaaiVanSettings.SAASBasedMultiTenancy)) return;

            if (!applicationId.HasValue ||
                GlobalController.get_user_applications(paramsContainer.CurrentUserID.Value, isCreator: true, archive: false)
                .Where(app => app.ApplicationID == applicationId).FirstOrDefault() == null) {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Application ap = GlobalController.get_user_application(paramsContainer.CurrentUserID.Value, applicationId.Value);

            if (UsersController.email_exists(applicationId.Value, email))
            {
                responseText = "{\"ErrorText\":\"" + Messages.ThisEmailBelongsToAnExistingUser + "\"}";
                return;
            }

            Guid? invitationId = UsersController.get_invitation_id(applicationId.Value, email, checkIfNotUsed: true);
            bool isExistingInvitation = invitationId.HasValue;

            if (!isExistingInvitation) invitationId = Guid.NewGuid();

            User currentUser = UsersController.get_user(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            DateTime now = DateTime.Now;
            string gDate = now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
            string pDate = GenericDate.get_local_date(now);
            
            dic.Add("SenderUserName", currentUser.UserName);
            dic.Add("SenderFirstName", currentUser.FirstName);
            dic.Add("SenderLastName", currentUser.LastName);
            dic.Add("SenderFullName", currentUser.FirstName + " " + currentUser.LastName);
            dic.Add("SystemURL", RaaiVanSettings.RaaiVanURL(applicationId));
            dic.Add("LoginURL", PublicConsts.LoginPage.Replace("~", dic["SystemURL"]));
            dic.Add("Now", gDate);
            dic.Add("GNow", gDate);
            dic.Add("PNow", pDate);
            dic.Add("InvitedFullName", fullname);
            dic.Add("InvitedEmail", email);
            dic.Add("InvitedEmailBase64", Base64.encode(email));
            dic.Add("InvitationID", invitationId.ToString());
            dic.Add("MessageText", messageText);
            dic.Add("TeamName", ap == null || string.IsNullOrEmpty(ap.Title) ? string.Empty : ap.Title);

            string emailBody = EmailTemplates.get_email_template(applicationId, EmailTemplateType.InviteUser, dic);
            string emailSubject = EmailTemplates.get_email_subject_template(applicationId, EmailTemplateType.InviteUser, dic);

            bool result = UsersController.invite_user(applicationId.Value, invitationId.Value, email, 
                isExistingInvitation, paramsContainer.CurrentUserID.Value, emailSubject, emailBody);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        public void batch_invite_users(Guid? applicationId, List<string> emails, string messageText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit || 
                (!RaaiVanSettings.SignUpViaInvitation(applicationId) && !RaaiVanSettings.SAASBasedMultiTenancy)) return;

            if (!applicationId.HasValue ||
                GlobalController.get_user_applications(paramsContainer.CurrentUserID.Value, isCreator: true, archive: false)
                .Where(app => app.ApplicationID == applicationId).FirstOrDefault() == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<EmailQueueItem> toBeQueued = new List<EmailQueueItem>();

            User currentUser = UsersController.get_user(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

            DateTime now = DateTime.Now;
            string gDate = now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
            string pDate = GenericDate.get_local_date(now);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("SenderUserName", currentUser.UserName);
            dic.Add("SenderFirstName", currentUser.FirstName);
            dic.Add("SenderLastName", currentUser.LastName);
            dic.Add("SenderFullName", currentUser.FirstName + " " + currentUser.LastName);
            dic.Add("SystemURL", RaaiVanSettings.RaaiVanURL(applicationId));
            dic.Add("LoginURL", PublicConsts.LoginPage.Replace("~", dic["SystemURL"]));
            dic.Add("Now", gDate);
            dic.Add("GNow", gDate);
            dic.Add("PNow", pDate);
            dic.Add("MessageText", messageText);

            foreach (string email in emails)
            {
                if (UsersController.email_exists(applicationId.Value, email)) continue;

                Guid invitationId = Guid.NewGuid();

                dic["InvitedEmail"] = email;
                dic["InvitedEmailBase64"] = Base64.encode(email);
                dic["InvitationID"] = invitationId.ToString();

                string emailBody = EmailTemplates.get_email_template(applicationId, EmailTemplateType.InviteUserBatch, dic);
                string emailSubject = EmailTemplates.get_email_subject_template(applicationId, EmailTemplateType.InviteUserBatch, dic);

                toBeQueued.Add(new EmailQueueItem()
                {
                    Email = email,
                    SenderUserID = paramsContainer.CurrentUserID,
                    Action = EmailAction.InviteUser,
                    Title = emailSubject,
                    EmailBody = emailBody
                });
            }

            bool succeed = GlobalController.add_emails_to_queue(applicationId.Value, toBeQueued);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        public void get_invited_users_count(Guid? applicationId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit ||
                (!RaaiVanSettings.SignUpViaInvitation(applicationId) && !RaaiVanSettings.SAASBasedMultiTenancy)) return;

            if (!applicationId.HasValue ||
                GlobalController.get_user_applications(paramsContainer.CurrentUserID.Value, isCreator: true, archive: false)
                .Where(app => app.ApplicationID == applicationId).FirstOrDefault() == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            responseText = "{\"Count\":" +
                UsersController.get_invited_users_count(applicationId.Value, paramsContainer.CurrentUserID.Value).ToString() + "}";
        }

        public void get_user_invitations(Guid? applicationId, int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit ||
                (!RaaiVanSettings.SignUpViaInvitation(applicationId) && !RaaiVanSettings.SAASBasedMultiTenancy)) return;

            if (!applicationId.HasValue ||
                GlobalController.get_user_applications(paramsContainer.CurrentUserID.Value, isCreator: true, archive: false)
                .Where(app => app.ApplicationID == applicationId).FirstOrDefault() == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            long totalCount = 0;

            List<Invitation> invitations = UsersController.get_user_invitations(applicationId.Value,
                paramsContainer.CurrentUserID.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Invitations\":[";

            bool isFirst = true;

            foreach (Invitation i in invitations)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"InvitationID\":\"" + (i.InvitationID.HasValue ? i.InvitationID.Value.ToString() : "") + "\"" +
                    ",\"ReceiverUserID\":\"" + (i.ReceiverUser.UserID.HasValue ? i.ReceiverUser.UserID.Value.ToString() : "") + "\"" +
                    ",\"SenderUserID\":\"" + (i.SenderUser.UserID.HasValue ? i.SenderUser.UserID.Value.ToString() : "") + "\"" +
                    ",\"ReceiverFirstName\":\"" + Base64.encode(i.ReceiverUser.FirstName) + "\"" +
                    ",\"ReceiverLastName\":\"" + Base64.encode(i.ReceiverUser.LastName) + "\"" +
                    ",\"Email\":\"" + Base64.encode(i.Email) + "\"" +
                    ",\"ReceiverImageURL\":\"" + (!i.ReceiverUser.UserID.HasValue ? string.Empty :
                        DocumentUtilities.get_personal_image_address(applicationId, i.ReceiverUser.UserID.Value)) + "\"" +
                    ",\"SendDate\":\"" + GenericDate.get_local_date(i.SendDate.Value, true) + "\"" +
                    ",\"Activated\":" + (i.Activated.HasValue && i.Activated.Value).ToString().ToLower() +
                    "}";
                isFirst = false;
            }

            responseText += "]}";
        }

        public void get_current_invitations(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit || !RaaiVanSettings.SAASBasedMultiTenancy) return;

            EmailAddress email = UsersController.get_users_main_email(paramsContainer.ApplicationID, 
                new List<Guid>() { paramsContainer.CurrentUserID.Value }).FirstOrDefault();

            List<Application> applications = email == null || string.IsNullOrEmpty(email.Address) ? new List<Application>() :
                UsersController.get_current_invitations(paramsContainer.CurrentUserID.Value, email.Address);

            responseText = "{\"Applications\":[" + string.Join(",", applications.Select(a => a.toJson(icon: true))) + "]}";
        }

        public void set_password_reset_ticket(string username, string password, Guid? invitationId, ref string responseText)
        {
            //Privacy Check: OK

            Guid? userId = UsersController.get_user_id(paramsContainer.ApplicationID, username);
            User user = !userId.HasValue ? null : UsersController.get_user(paramsContainer.ApplicationID, userId.Value);

            if (user == null) {
                responseText = "{\"ErrorText\":\"" + Messages.UserNotFound + "\"}";
                return;
            }

            //Check Password Policy
            if (!UserUtilities.check_password_policy(paramsContainer.ApplicationID, password, string.Empty))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PasswordPolicyDidntMeet.ToString() + "\"}";
                return;
            }
            //end of Check Password Policy

            password = PublicMethods.verify_string(password);

            EmailAddress mainEmail = UsersController.get_users_main_email(paramsContainer.ApplicationID, new List<Guid>() { userId.Value })
                .Where(e => !string.IsNullOrEmpty(e.Address)).FirstOrDefault();

            PhoneNumber mainPhone = UsersController.get_users_main_phone(paramsContainer.ApplicationID, new List<Guid>() { userId.Value })
                .Where(n => !string.IsNullOrEmpty(n.Number)).FirstOrDefault();

            bool isEmail = (mainEmail != null && username.ToLower() == mainEmail.Address.ToLower()) || 
                (mainEmail != null && mainPhone == null);
            bool isNumber = (mainPhone != null && username.ToLower() == mainPhone.Number.ToLower()) ||
                (mainEmail == null && mainPhone != null);

            string email = isEmail ? mainEmail.Address : string.Empty;
            string phone = isNumber ? mainPhone.Number : string.Empty;

            if (!isEmail && !isNumber)
            {
                responseText = "{\"Email\":" + (mainEmail != null ? mainEmail.toJson() : "null") +
                    ",\"Phone\":" + (mainPhone != null ? mainPhone.toJson() : "null") +
                    "}";
                return;
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["UserID"] = userId.Value;
            customData["UserName"] = user.UserName;
            customData["Password"] = password;
            customData["FirstName"] = user.FirstName;
            customData["LastName"] = user.LastName;
            customData["Email"] = email;
            customData["Phone"] = phone;
            customData["IsPasswordReset"] = true;
            if (invitationId.HasValue) customData["InvitationID"] = invitationId.Value;

            VerificationCode.process_request(paramsContainer.ApplicationID, emailAddress: email,
                phoneNumber: phone, responseText: ref responseText, customData: PublicMethods.toJSON(customData));
        }

        public void change_password(string oldPassword, string newPassword, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            //Check Password Policy
            if (!UserUtilities.check_password_policy(paramsContainer.ApplicationID, newPassword, oldPassword))
            {
                responseText = "{\"ErrorText\":\"" + Messages.PasswordPolicyDidntMeet.ToString() + "\"}";
                return;
            }
            //end of Check Password Policy

            newPassword = PublicMethods.verify_string(newPassword);

            string errorMessage = string.Empty;

            if (RaaiVanSettings.Users.NotAvailablePreviousPasswordsCount(paramsContainer.ApplicationID) > 0)
            {
                List<Password> last = UsersController.get_last_passwords(paramsContainer.ApplicationID, 
                    paramsContainer.CurrentUserID.Value, autoGenerated: false,
                    count: RaaiVanSettings.Users.NotAvailablePreviousPasswordsCount(paramsContainer.ApplicationID));

                string sha1Pass = PublicMethods.sha1(newPassword);

                if (last.Any(u => u.Value == sha1Pass))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.YouCannotUsePreviousPasswords + "\"}";
                    return;
                }
            }

            if (!_validate_password(paramsContainer.CurrentUserID.Value, oldPassword))
            {
                responseText = "{\"ErrorText\":\"" + Messages.CurrentPasswordIsWrong.ToString() + "\"}";
                return;
            }

            bool result = UsersController.set_password(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value, newPassword,
                false, false, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            if (result) RaaiVanUtil.password_change_not_needed(HttpContext.Current);
        }

        public bool _validate_password(Guid userId, string password)

        {
            password = PublicMethods.verify_string(password);

            string curPass = string.Empty, curPassSalt = string.Empty;

            UsersController.get_current_password(paramsContainer.ApplicationID, userId, ref curPass, ref curPassSalt);

            return UserUtilities.encode_password(password, curPassSalt) == curPass;
        }

        public void validate_password(string password, ref string responseText)
        {
            //Privay Check: OK
            if (!paramsContainer.GBEdit) return;

            responseText = "{\"Result\":" +
                _validate_password(paramsContainer.CurrentUserID.Value, password).ToString().ToLower() + "}";
        }

        public void set_password(string token, long? code, bool? login, ref string responseText)
        {
            //Privacy Check: OK

            if (string.IsNullOrEmpty(token) || !code.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidInput + "\"}";
                return;
            }

            string customData = string.Empty;

            bool result = VerificationCode.process_request(paramsContainer.ApplicationID, token: token, code: code, ref customData);

            if (!result)
            {
                responseText = customData;
                return;
            }

            Dictionary<string, object> dic = PublicMethods.fromJSON(customData);

            if (!PublicMethods.get_dic_value<bool>(dic, "IsPasswordReset", defaultValue: false)) {
                responseText = "";
                return;
            }

            LoginUtil loginUtil = new LoginUtil(paramsContainer: paramsContainer,
                invitationId: PublicMethods.parse_guid(PublicMethods.get_dic_value(dic, "InvitationID")));

            responseText = loginUtil.login_via_change_password(
                userId: PublicMethods.parse_guid(PublicMethods.get_dic_value(dic, "UserID")),
                password: PublicMethods.get_dic_value(dic, "Password"),
                login: login.HasValue && login.Value);
        }

        public void set_random_password(Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            string password = UserUtilities.generate_password();

            string errorMessage = string.Empty;

            bool result = UsersController.set_password(paramsContainer.Tenant.Id, userId.Value, password,
                true, true, ref errorMessage);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                    ",\"Password\":\"" + Base64.encode(password) + "\"}";

            if (result && userId == paramsContainer.CurrentUserID.Value)
                RaaiVanUtil.password_change_not_needed(HttpContext.Current);
        }

        public void modify_email_ticket(Guid? emailId, string emailAddress, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!PublicMethods.is_email(emailAddress))
            {
                responseText = "{\"ErrorText\":\"" + Messages.EmailIsNotValid + "\"}";
                return;
            }
            else if (UsersController.email_exists(paramsContainer.ApplicationID, emailAddress))
            {
                responseText = "{\"ErrorText\":\"" + Messages.ThisEmailBelongsToAnExistingUser + "\"}";
                return;
            }

            Guid? mainEmailId = !paramsContainer.CurrentUserID.HasValue ? null :
                UsersController.get_main_email(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

            EmailAddress address = !emailId.HasValue ? null :
                UsersController.get_email_address(paramsContainer.ApplicationID, emailId.Value);

            User user = !paramsContainer.CurrentUserID.HasValue ? null :
                UsersController.get_user(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

            if (user == null || (emailId.HasValue && address == null) || (mainEmailId.HasValue && mainEmailId != emailId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["UserID"] = user.UserID;
            customData["UserName"] = user.UserName;
            customData["FirstName"] = user.FirstName;
            customData["LastName"] = user.LastName;
            customData["EmailID"] = emailId;
            customData["Email"] = emailAddress;

            VerificationCode.process_request(paramsContainer.ApplicationID, emailAddress: emailAddress,
                phoneNumber: null, responseText: ref responseText, customData: PublicMethods.toJSON(customData));
        }

        public void modify_email(string token, long? code, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (string.IsNullOrEmpty(token) || !code.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidInput + "\"}";
                return;
            }

            string customData = string.Empty;

            bool result = VerificationCode.process_request(paramsContainer.ApplicationID, token: token, code: code, ref customData);

            if (!result)
            {
                responseText = customData;
                return;
            }

            Dictionary<string, object> dic = PublicMethods.fromJSON(customData);

            Guid? emailId = PublicMethods.parse_guid(PublicMethods.get_dic_value(dic, "EmailID"));
            string address = PublicMethods.get_dic_value(dic, "Email");

            if (UsersController.email_exists(paramsContainer.ApplicationID.Value, address))
            {
                responseText = "{\"ErrorText\":\"" + Messages.EmailAlreadyExists + "\"}";
                return;
            }

            bool succeed = false;

            if (emailId.HasValue)
            {
                succeed = UsersController.edit_email_address(paramsContainer.ApplicationID,
                    emailId.Value, address, paramsContainer.CurrentUserID.Value);
            }
            else {
                emailId = Guid.NewGuid();

                succeed = UsersController.set_email_address(paramsContainer.ApplicationID, emailId.Value,
                    paramsContainer.CurrentUserID.Value, address, isMainEmail: true, paramsContainer.CurrentUserID.Value);
            }

            EmailAddress emailObj = !succeed ? new EmailAddress() : new EmailAddress() {
                EmailID = emailId,
                Address = address,
                IsMain = true,
                UserID = paramsContainer.CurrentUserID
            };

            responseText = !succeed ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + ",\"Email\":" + emailObj.toJson() + "}";
        }

        public void create_user(string username, string firstName, string lastName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(username) && username.Length > 250) ||
                (!string.IsNullOrEmpty(firstName) && firstName.Length > 250) ||
                (!string.IsNullOrEmpty(lastName) && lastName.Length > 250))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(username) ||
                !PublicMethods.is_secure_title(firstName) || !PublicMethods.is_secure_title(lastName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            User newUser = new User()
            {
                UserName = username,
                FirstName = firstName,
                LastName = lastName
            };

            if (!PublicMethods.is_valid_username(paramsContainer.Tenant.Id, username))
            {
                responseText = "{\"ErrorText\":\"" + Messages.UserNamePatternIsNotValid + "\"}";
                return;
            }

            responseText = !UsersController.create_user(paramsContainer.Tenant.Id, newUser, passAutoGenerated: true) ?
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
        }

        protected void is_approved(Guid? userId, bool? isApproved, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (!isApproved.HasValue)
            {
                if (!paramsContainer.GBView) return;

                responseText = UsersController.is_approved(paramsContainer.Tenant.Id, userId.Value).ToString().ToLower();
                return;
            }

            if (!paramsContainer.GBEdit) return;

            bool result = UsersController.is_approved(paramsContainer.Tenant.Id, userId.Value, isApproved.Value);

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
                    Action = Modules.Log.Action.SetUserIsApproved,
                    SubjectID = userId,
                    Info = "{\"IsApproved\":" + isApproved.ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.CN
                });
            }
            //end of Save Log
        }

        protected void unlock_user(Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;
            
            if (!userId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool succeed = userId.HasValue && UsersController.unlock_user(paramsContainer.ApplicationID, userId.Value);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_user(Guid? userId, bool getPhoneNumbers, bool getEmails,
            bool hidePhoneIfNotFriends, bool hideMailIfNotFriends, List<Guid> membNodetypeIdLst, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            User user = !userId.HasValue ? null : UsersController.get_user(paramsContainer.Tenant.Id, userId.Value);

            if (user == null)
            {
                responseText = "{}";
                return;
            }

            if (paramsContainer.IsAuthenticated &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                hideMailIfNotFriends = hidePhoneIfNotFriends = false;

            Friend fs = !paramsContainer.IsAuthenticated ? null :
                UsersController.get_friendship_status(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, userId.Value);

            bool showPhones = paramsContainer.IsAuthenticated && getPhoneNumbers && (
                hidePhoneIfNotFriends != true || (
                    paramsContainer.IsAuthenticated && (
                        paramsContainer.CurrentUserID == userId || (fs != null && fs.AreFriends.HasValue && fs.AreFriends.Value)
                    )
                )
            );

            bool showMails = paramsContainer.IsAuthenticated && getEmails && (
                hideMailIfNotFriends != true || (
                    paramsContainer.IsAuthenticated && (
                        paramsContainer.CurrentUserID == userId || (
                            paramsContainer.CurrentUserID == userId || (fs != null && fs.AreFriends.HasValue && fs.AreFriends.Value)
                        )
                    )
                )
            );

            user.PhoneNumbers = showPhones ? 
                UsersController.get_phone_numbers(paramsContainer.ApplicationID, userId.Value) : new List<PhoneNumber>();

            user.Emails = showMails ?
                UsersController.get_email_addresses(paramsContainer.ApplicationID, userId.Value) : new List<EmailAddress>();

            user.MembershipAreas =
                CNController.get_member_nodes(paramsContainer.Tenant.Id, userId.Value, ref membNodetypeIdLst);

            responseText = user.toJson(paramsContainer.ApplicationID, profileImageUrl: true, highQualityProfileImageUrl: true);
        }

        protected void set_theme(string theme, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(theme)) theme = theme.Split(',')[0];
            
            bool result = UsersController.set_theme(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value, theme);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_theme(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            string theme = UsersController.get_theme(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            if (string.IsNullOrEmpty(theme)) theme = "Default";

            responseText = "{\"Theme\":\"" + theme + "\"}";
        }

        protected void set_verification_code_media(TwoStepAuthentication media, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;
            
            bool result = UsersController.set_verification_code_media(paramsContainer.Tenant.Id, 
                paramsContainer.CurrentUserID.Value, media);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected bool save_user_settings_item(User user, string name, object value, string type)
        {            
            if (string.IsNullOrEmpty(type)) type = string.Empty;

            if (user == null) return false;

            Dictionary<string, object> currentSettings = user.Settings == null ? new Dictionary<string, object>() : user.Settings;

            string strValue = value == null ? null : value.ToString();

            switch (type.ToLower())
            {
                case "number":
                    {
                        double? val = PublicMethods.parse_double(strValue);

                        if (!val.HasValue && currentSettings.ContainsKey(name)) currentSettings.Remove(name);
                        else currentSettings[name] = val;
                    }
                    break;
                case "boolean":
                    {
                        bool? val = PublicMethods.parse_bool(strValue);

                        if (!val.HasValue && currentSettings.ContainsKey(name)) currentSettings.Remove(name);
                        else currentSettings[name] = val;
                    }
                    break;
                case "object":
                    {
                        if (value == null && currentSettings.ContainsKey(name)) currentSettings.Remove(name);
                        else currentSettings[name] = value;
                    }
                    break;
                default:
                    {
                        if (string.IsNullOrEmpty(strValue) && currentSettings.ContainsKey(name)) currentSettings.Remove(name);
                        else currentSettings[name] = Base64.decode(strValue);
                    }
                    break;
            }

            return UsersController.save_user_settings(paramsContainer.ApplicationID,
                paramsContainer.CurrentUserID.Value, PublicMethods.toJSON(currentSettings));
        }

        protected void save_user_settings_item(string name, string value, string type, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            User user = !paramsContainer.CurrentUserID.HasValue ? null :
                UsersController.get_user(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value);

            responseText = save_user_settings_item(user, name, value, type) ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        private void update_friend_suggestions(Guid userId, bool? full, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool isSysAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            if (userId != paramsContainer.CurrentUserID && !isSysAdmin) userId = paramsContainer.CurrentUserID.Value;
            if (full.HasValue && full.Value && isSysAdmin) userId = Guid.Empty;

            bool result = UsersController.update_friend_suggestions(userId);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Now\":\"" +
                GenericDate.get_local_date(DateTime.Now, true) + "\"}";
        }

        private void get_friend_suggestions(int? count, long? lowerBoundary, List<Guid> nodeTypeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            long totalCount = 0;

            List<FriendSuggestion> frndSuggestion = UsersController.get_friend_suggestions(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Suggestions\":[";

            bool isFirst = true;
            foreach (FriendSuggestion sug in frndSuggestion)
            {
                if (!isFirst) responseText += ",";
                isFirst = false;

                responseText += "{\"MutualFriendsCount\":" + (sug.MutualFriends.HasValue ? sug.MutualFriends.Value : 0).ToString() +
                    ",\"UserID\":\"" + (sug.User.UserID.HasValue && sug.User.UserID != Guid.Empty ? sug.User.UserID.Value.ToString() : "") + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(sug.User.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(sug.User.LastName) + "\"" +
                    ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        sug.User.UserID.Value) + "\"" +
                    "}";
            }

            responseText += "]}";
        }

        protected void get_friends(Guid userId, List<Guid> friendIds, bool? mutualsCount, string searchText,
            int? count, long? lowerBoundary, bool? online, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit)
            {
                responseText = "{}";
                return;
            }

            long totalCount = 0;
            if (!count.HasValue || count <= 0) count = 20;

            List<Friend> friends = UsersController.get_friends(paramsContainer.Tenant.Id, userId, friendIds, mutualsCount,
                count, lowerBoundary, ref totalCount, true, isSender: null, searchText: searchText);

            if (online.HasValue && online.Value)
                friends = friends.Where(u => RaaiVanHub.is_online(u.User.UserID.Value)).ToList();

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Friends\":[";

            bool isFirst = true;
            foreach (Friend frnd in friends)
            {
                if (!isFirst) responseText += ",";
                isFirst = false;

                responseText += "{\"UserID\":\"" + frnd.User.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(frnd.User.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(frnd.User.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(frnd.User.UserName) + "\"" +
                    ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        frnd.User.UserID.Value) + "\"" +
                    ",\"IsSender\":" + (frnd.IsSender.HasValue && frnd.IsSender.Value).ToString().ToLower() +
                    ",\"MutualFriendsCount\":" + (frnd.MutualFriendsCount.HasValue ? frnd.MutualFriendsCount : 0).ToString() +
                    "}";
            }

            responseText += "]}";
        }

        protected void get_email_contacts_status(Guid userId, List<string> emails, bool? saveEmails, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            List<EmailContactStatus> contacts =
                UsersController.get_email_contacts_status(paramsContainer.Tenant.Id, userId, emails, saveEmails);

            List<User> users = UsersController.get_users(paramsContainer.Tenant.Id, contacts.Where(
                u => u.UserID.HasValue).Select(v => v.UserID.Value).ToList());

            responseText =
                "{\"Emails\":[" + ProviderUtil.list_to_string<string>(contacts.Where(
                    u => !u.UserID.HasValue).Select(v => "\"" + Base64.encode(v.Email) + "\"").ToList()) + "]" +
                ",\"ReceivedFriendRequests\":[" + ProviderUtil.list_to_string<string>(contacts.Where(
                    u => u.UserID.HasValue && u.FriendRequestReceived.HasValue && u.FriendRequestReceived.Value)
                    .Select(v => _get_user_json(users.Where(x => x.UserID == v.UserID).FirstOrDefault())).ToList()) + "]" +
                ",\"OtherUsers\":[" + ProviderUtil.list_to_string<string>(contacts.Where(
                    u => u.UserID.HasValue && (!u.FriendRequestReceived.HasValue || !u.FriendRequestReceived.Value))
                    .Select(v => _get_user_json(users.Where(x => x.UserID == v.UserID).FirstOrDefault())).ToList()) + "]" +
                "}";
        }

        protected void get_friendship_requests(Guid userId, bool? sent, bool? mutualsCount,
            int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            long totalCount = 0;

            List<Friend> _friendshipRequests = UsersController.get_friends(paramsContainer.Tenant.Id, userId, mutualsCount,
                count, lowerBoundary, ref totalCount, false, sent);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Friends\":[";

            bool isFirst = true;
            foreach (Friend _friend in _friendshipRequests)
            {
                if (!isFirst) responseText += ",";
                isFirst = false;

                responseText += "{\"UserID\":\"" + _friend.User.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(_friend.User.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(_friend.User.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(_friend.User.UserName) + "\"" +
                    ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        _friend.User.UserID.Value) + "\"" +
                    ",\"MutualFriendsCount\":" + (_friend.MutualFriendsCount.HasValue ? _friend.MutualFriendsCount : 0).ToString() +
                    ",\"RequestDate\":\"" + GenericDate.get_local_date(_friend.RequestDate) + "\"" +
                    "}";
            }

            responseText += "]}";
        }

        protected void send_friendship_request(Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = userId.HasValue && UsersController.send_friendship_request(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, userId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Send Notification
            if (result)
            {
                Notification not = new Notification()
                {
                    SubjectID = paramsContainer.CurrentUserID,
                    RefItemID = paramsContainer.CurrentUserID,
                    SubjectType = SubjectType.User,
                    Action = Modules.NotificationCenter.ActionType.FriendRequest,
                    UserID = userId
                };
                not.Sender.UserID = paramsContainer.CurrentUserID;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification
        }

        protected void get_friendship_status(Guid userId, List<Guid> otherUserIds, bool? mutualsCount, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            List<Friend> otherUsers =
                UsersController.get_friendship_status(paramsContainer.Tenant.Id, userId, otherUserIds, mutualsCount);

            responseText = "[" + ProviderUtil.list_to_string<string>(otherUsers.Select(
                u => "{\"UserID\":\"" + u.User.UserID.ToString() + "\"" +
                    ",\"IsFriend\":" + (u.AreFriends.HasValue && u.AreFriends.Value).ToString().ToLower() +
                    ",\"IsPending\":" + (u.AreFriends.HasValue && !u.AreFriends.Value).ToString().ToLower() +
                    ",\"IsSender\":" + (u.IsSender.HasValue && u.IsSender.Value).ToString().ToLower() +
                    ",\"MutualFriendsCount\":" + (u.MutualFriendsCount.HasValue ? u.MutualFriendsCount.Value : 0).ToString() +
                    "}").ToList()) +
                "]";
        }

        protected void accept_friendship(Guid? otherUserId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = otherUserId.HasValue && UsersController.accept_friendship(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, otherUserId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Send Notification
            if (result)
            {
                Notification not = new Notification()
                {
                    SubjectID = paramsContainer.CurrentUserID,
                    RefItemID = paramsContainer.CurrentUserID,
                    SubjectType = SubjectType.User,
                    Action = Modules.NotificationCenter.ActionType.AcceptFriendRequest,
                    UserID = otherUserId
                };
                not.Sender.UserID = paramsContainer.CurrentUserID;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
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
                    Action = Modules.Log.Action.AcceptFriendRequest,
                    SubjectID = otherUserId,
                    SecondSubjectID = paramsContainer.CurrentUserID.Value,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void reject_friendship(Guid? otherUserId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = otherUserId.HasValue && UsersController.reject_friendship(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, otherUserId.Value);

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
                    Action = Modules.Log.Action.RejectFriendRequest,
                    SubjectID = otherUserId,
                    SecondSubjectID = paramsContainer.CurrentUserID.Value,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_friend_requests_count(ref string responsText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            responsText = "{\"Count\":" + UsersController.get_friends_count(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, false, false, true).ToString() + "}";
        }

        protected void get_last_content_creators(int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!count.HasValue || count.Value <= 0) count = 20;

            ArrayList lst = GlobalController.get_last_content_creators(paramsContainer.Tenant.Id, count.Value);

            for (int i = 0, lnt = lst.Count; i < lnt; ++i)
            {
                Dictionary<string, object> item = (Dictionary<string, object>)lst[i];

                if (item.ContainsKey("UserID")) item["ProfileImageURL"] =
                        DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, (Guid)item["UserID"]);
                if (item.ContainsKey("Date"))
                {
                    DateTime dt = (DateTime)item["Date"];
                    item["Date"] = GenericDate.get_local_date(dt);
                    item["Date_Gregorian"] = dt.ToString();
                }

                lst[i] = item;
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["Users"] = lst;

            responseText = PublicMethods.toJSON(dic);
        }

        protected void add_modify_remote_server(Guid? id, string name, string url,
            string username, string password, bool add, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paramsContainer.CurrentUserID.HasValue ||
                !PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if ((!string.IsNullOrEmpty(name) && name.Length > 200) || (!string.IsNullOrEmpty(url) && url.Length > 90) ||
                (!string.IsNullOrEmpty(username) && username.Length > 90) || (!string.IsNullOrEmpty(password) && password.Length > 20))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            RemoteServer rs = add ? new RemoteServer() { ID = Guid.NewGuid() } :
                (!id.HasValue ? null : UsersController.get_remote_server(paramsContainer.Tenant.Id, id.Value));

            if (rs == null || !paramsContainer.CurrentUserID.HasValue ||
                (!add && rs.UserID != paramsContainer.CurrentUserID) || string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            rs.Name = name;
            rs.URL = url;
            rs.UserName = username;
            rs.Passwrod = password;
            byte[] binaryPassword = rs.get_password_encrypted();

            bool result = UsersController.add_or_modify_remote_server(paramsContainer.Tenant.Id,
                rs.ID.Value, paramsContainer.CurrentUserID.Value, name, url, username, binaryPassword);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" + ",\"Server\":" + rs.toJson() + "}";

            //Save Log
            if (result)
            {
                rs.Passwrod = "";

                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = add ? Modules.Log.Action.AddRemoteServer : Modules.Log.Action.ModifyRemoteServer,
                    SubjectID = rs.ID,
                    Info = rs.toJson(),
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void remove_remote_server(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paramsContainer.CurrentUserID.HasValue ||
                !PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            RemoteServer rs = !id.HasValue ? null : UsersController.get_remote_server(paramsContainer.Tenant.Id, id.Value);

            if (rs == null || !paramsContainer.CurrentUserID.HasValue || rs.UserID != paramsContainer.CurrentUserID)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = UsersController.remove_remote_server(paramsContainer.Tenant.Id, rs.ID.Value);

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
                    Action = Modules.Log.Action.RemoveRemoteServer,
                    SubjectID = rs.ID,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_remote_servers(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!paramsContainer.CurrentUserID.HasValue || (
                !PublicMethods.is_system_admin(paramsContainer.ApplicationID, paramsContainer.CurrentUserID.Value) &&
                !AuthorizationManager.has_right(AccessRoleName.RemoteServers, paramsContainer.CurrentUserID.Value)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<RemoteServer> servers = UsersController.get_remote_servers(paramsContainer.Tenant.Id);

            responseText = "{\"Servers\":[" + string.Join(",", servers.Select(u => u.toJson())) + "]}";
        }

        /* Profile */

        protected void set_user_name(Guid? userId, string userName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(userName) && userName.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(userName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            User theUser = !userId.HasValue ? null : UsersController.get_user(paramsContainer.ApplicationID, userId.Value);

            bool hasRight = false;

            if (theUser == null || string.IsNullOrEmpty(theUser.UserName) || theUser.UserName.ToLower() == "admin")
                hasRight = false;
            else if (!RaaiVanSettings.SAASBasedMultiTenancy)
                hasRight = AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID);
            else
                hasRight = userId.HasValue && userId == paramsContainer.CurrentUserID;

            if (!hasRight)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid? theUid = UsersController.get_user_id(paramsContainer.ApplicationID, userName);

            if (theUid.HasValue && theUid != userId)
            {
                responseText = "{\"ErrorText\":\"" + Messages.UserNameAlreadyExists + "\"}";
                return;
            }
            else if (!PublicMethods.is_valid_username(paramsContainer.ApplicationID, userName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.UserNamePatternIsNotValid + "\"}";
                return;
            }

            bool result = UsersController.set_username(paramsContainer.ApplicationID, userId.Value, userName);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.ApplicationID, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetUserName,
                    SubjectID = userId,
                    Info = "{\"UserName\":\"" + Base64.encode(userName) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_first_and_last_name(Guid userId, string firstName, string lastName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(firstName) && firstName.Length > 250) ||
                (!string.IsNullOrEmpty(lastName) && lastName.Length > 250))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(firstName) || !PublicMethods.is_secure_title(lastName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = UsersController.set_first_and_last_name(paramsContainer.ApplicationID, userId, firstName, lastName);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.ApplicationID, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetFirstAndLastName,
                    SubjectID = userId,
                    Info = "{\"FirstName\":\"" + Base64.encode(firstName) + "\",\"LastName\":\"" + Base64.encode(lastName) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_first_name(Guid userId, string firstName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            User user = UsersController.get_user(paramsContainer.ApplicationID, userId);

            if (user == null) responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else set_first_and_last_name(userId, firstName, user.LastName, ref responseText);
        }

        protected void set_last_name(Guid userId, string lastName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            User user = UsersController.get_user(paramsContainer.ApplicationID, userId);

            if (user == null) responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            else set_first_and_last_name(userId, user.FirstName, lastName, ref responseText);
        }

        protected void set_about_me(Guid userId, string text, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(text) && text.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = UsersController.set_about_me(paramsContainer.ApplicationID, userId, text);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_city(Guid userId, string city, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(city) && city.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = UsersController.set_city(paramsContainer.ApplicationID, userId, city);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_organization(Guid userId, string organization, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(organization) && organization.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = paramsContainer.ApplicationID.HasValue &&
                UsersController.set_organization(paramsContainer.ApplicationID.Value, userId, organization);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_department(Guid userId, string department, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(department) && department.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !AuthorizationManager.has_right(AccessRoleName.UsersManagement, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            bool result = paramsContainer.ApplicationID.HasValue &&
                UsersController.set_department(paramsContainer.ApplicationID.Value, userId, department);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_job_title(Guid userId, string jobTitle, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(jobTitle) && jobTitle.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(jobTitle))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            bool result = UsersController.set_job_title(paramsContainer.Tenant.Id, userId, jobTitle);

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
                    Action = Modules.Log.Action.SetJobTitle,
                    SubjectID = userId,
                    Info = "{\"JobTitle\":\"" + Base64.encode(jobTitle) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_employment_types(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            responseText = "[" + ProviderUtil.list_to_string<string>(
                UserUtilities.get_employment_types().Select(u => "\"" + u.ToString() + "\"").ToList()) + "]";
        }

        protected void set_employment_type(Guid userId, EmploymentType employmentType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            bool result = employmentType == EmploymentType.NotSet ? false :
                UsersController.set_employment_type(paramsContainer.Tenant.Id, userId, employmentType);

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
                    Action = Modules.Log.Action.SetEmploymentType,
                    SubjectID = userId,
                    Info = "{\"EmploymentType\":\"" + employmentType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_birthday(Guid userId, DateTime? birthday, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            bool result = !birthday.HasValue ? false :
                UsersController.set_birthday(paramsContainer.Tenant.Id, userId, birthday.Value);

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
                    Action = Modules.Log.Action.SetBirthday,
                    SubjectID = userId,
                    Info = "{\"Birthday\":\"" + birthday.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_phone_number(Guid userId, string phoneNumber, PhoneType phoneNumberType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length > 20)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            Guid newPhoneId = Guid.NewGuid();

            bool result = phoneNumberType == PhoneType.NotSet ? false :
                UsersController.set_phone_number(paramsContainer.ApplicationID, 
                newPhoneId, userId, phoneNumber, phoneNumberType, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                "\",\"NumberID\":\"" + newPhoneId.ToString() + "\"}" : "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPhoneNumber,
                    SubjectID = userId,
                    Info = "{\"PhoneNumber\":\"" + Base64.encode(phoneNumber) + "\",\"PhoneType\":\"" + phoneNumberType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void edit_phone_number(Guid? numberId, string phoneNumber, PhoneType phoneType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length > 20)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            PhoneNumber obj = !numberId.HasValue ? null : UsersController.get_phone_number(paramsContainer.ApplicationID, numberId.Value);
            Guid? mainId = obj == null ? null : UsersController.get_main_phone(paramsContainer.ApplicationID, obj.UserID.Value);

            if (mainId.HasValue && mainId == numberId)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (numberId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = !numberId.HasValue || phoneType == PhoneType.NotSet ? false :
                UsersController.edit_phone_number(paramsContainer.ApplicationID, 
                numberId.Value, phoneNumber, phoneType, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.ModifyPhoneNumber,
                    SubjectID = numberId,
                    Info = "{\"PhoneNumber\":\"" + Base64.encode(phoneNumber) + "\",\"PhoneType\":\"" + phoneType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void remove_phone_number(Guid? numberID, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (numberID.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                PhoneNumber obj = UsersController.get_phone_number(paramsContainer.ApplicationID, numberID.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = numberID.HasValue && UsersController.remove_phone_number(paramsContainer.ApplicationID, 
                numberID.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemovePhoneNumber,
                    SubjectID = numberID,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_main_phone(Guid? numberId, string verificationToken, long? code, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            PhoneNumber obj = !numberId.HasValue ? null : UsersController.get_phone_number(paramsContainer.ApplicationID, numberId.Value);
            Guid? curMainId = obj == null ? null : UsersController.get_main_phone(paramsContainer.ApplicationID, obj.UserID.Value);

            if (curMainId.HasValue && curMainId == numberId)
            {
                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
                return;
            }
            else if (obj == null || (obj.UserID != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            else if (UsersController.get_phone_owners(paramsContainer.ApplicationID, new List<string>() { obj.Number })
                    .Any(e => e.UserID != obj.UserID && e.IsMain.HasValue && e.IsMain.Value)) {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (!VerificationCode.process_request(paramsContainer.Tenant.Id,
                null, obj.Number, verificationToken, code, ref responseText)) return;

            bool result = numberId.HasValue && 
                UsersController.set_main_phone(paramsContainer.ApplicationID, numberId.Value, obj.UserID.Value);

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
                    Action = Modules.Log.Action.SetMainPhoneNumber,
                    SubjectID = obj.UserID,
                    SecondSubjectID = numberId,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_email_address(Guid userId, string address, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(address) && address.Length > 90)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.ThisEmailBelongsToAnExistingUser + "\"}";
                return;
            }

            Guid newEmailId = Guid.NewGuid();

            if (UsersController.email_exists(paramsContainer.Tenant.Id, address))
            {
                responseText = "{\"ErrorText\":\"" + Messages.ThisEmailBelongsToAnExistingUser + "\"}";
                return;
            }

            bool result = UsersController.set_email_address(paramsContainer.ApplicationID, 
                newEmailId, userId, address, isMainEmail: false, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully +
                "\",\"EmailID\":\"" + newEmailId.ToString() + "\"}" : "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetEmailAddress,
                    SubjectID = userId,
                    Info = "{\"Email\":\"" + Base64.encode(address) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_email_addresses(Guid? userId, string userName, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!userId.HasValue) userId = UsersController.get_user_id(paramsContainer.Tenant.Id, userName);

            List<EmailAddress> emails = !userId.HasValue ? new List<EmailAddress>() :
                UsersController.get_email_addresses(paramsContainer.ApplicationID, userId.Value);

            responseText = "{\"Emails\":[";

            var isFirst = true;
            foreach (EmailAddress e in emails)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"EmailID\":\"" + e.EmailID.ToString() + "\"" + ",\"Email\":\"" + Base64.encode(e.Address) + "\"}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void edit_email_address(Guid? emailId, string address, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(address) && address.Length > 90)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            EmailAddress obj = !emailId.HasValue ? null : UsersController.get_email_address(paramsContainer.ApplicationID, emailId.Value);
            Guid? mainId = obj == null ? null : UsersController.get_main_email(paramsContainer.ApplicationID, obj.UserID.Value);

            if (mainId.HasValue && mainId == emailId)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (emailId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            if (UsersController.email_exists(paramsContainer.Tenant.Id, address))
            {
                responseText = "{\"ErrorText\":\"" + Messages.ThisEmailBelongsToAnExistingUser + "\"}";
                return;
            }

            bool result = emailId.HasValue && UsersController.edit_email_address(paramsContainer.ApplicationID,
                emailId.Value, address, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.ModifyEmailAddress,
                    SubjectID = emailId,
                    Info = "{\"Email\":\"" + Base64.encode(address) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void remove_email_address(Guid? emailId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (emailId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                EmailAddress obj = UsersController.get_email_address(paramsContainer.ApplicationID, emailId.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = emailId.HasValue && UsersController.remove_email_address(paramsContainer.ApplicationID, 
                emailId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveEmailAddress,
                    SubjectID = emailId,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void set_main_email(Guid? emailId, string verificationToken, long? code, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            EmailAddress obj = !emailId.HasValue ? null : UsersController.get_email_address(paramsContainer.ApplicationID, emailId.Value);
            Guid? curMainId = obj == null ? null : UsersController.get_main_email(paramsContainer.ApplicationID, obj.UserID.Value);

            if (curMainId.HasValue && curMainId == emailId)
            {
                responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
                return;
            }
            else if (obj == null || (obj.UserID != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            else if (UsersController.get_email_owners(paramsContainer.ApplicationID, new List<string>() { obj.Address })
                    .Any(e => e.UserID != obj.UserID && e.IsMain.HasValue && e.IsMain.Value))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            
            if (!VerificationCode.process_request(paramsContainer.Tenant.Id,
                obj.Address, null, verificationToken, code, ref responseText)) return;

            bool result = emailId.HasValue && 
                UsersController.set_main_email(paramsContainer.ApplicationID, emailId.Value, obj.UserID.Value);

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
                    Action = Modules.Log.Action.SetMainEmailAddress,
                    SubjectID = obj.UserID,
                    SecondSubjectID = emailId,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_resume_info(Guid userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<JobExperience> jobExperiences = UsersController.get_job_experiences(paramsContainer.Tenant.Id, userId);
            responseText = "{\"JobExperiences\":[";
            bool isFirst = true;

            foreach (JobExperience je in jobExperiences)
            {
                responseText += (isFirst ? "" : ",");
                isFirst = false;
                responseText += "{ " +
                    "\"JobID\":" + "\"" + je.JobID.ToString() + "\"," +
                    "\"UserID\":" + "\"" + je.UserID + "\"," +
                    "\"Title\":" + "\"" + Base64.encode(je.Title) + "\"," +
                    "\"Employer\":" + "\"" + Base64.encode(je.Employer) + "\"," +
                    "\"JStartDate\":" + "\"" + (je.StartDate.HasValue ? GenericDate.get_local_date(je.StartDate.Value, false) : string.Empty) + "\"," +
                    "\"GStartDate\":" + "\"" + (je.StartDate.HasValue ? je.StartDate.Value.ToString() : string.Empty) + "\"," +
                    "\"JEndDate\":" + "\"" + (je.EndDate.HasValue ? GenericDate.get_local_date(je.EndDate.Value, false) : string.Empty) + "\"," +
                    "\"GEndDate\":" + "\"" + (je.EndDate.HasValue ? je.EndDate.Value.ToString() : string.Empty) + "\"" +
                "}";
            }

            responseText += "]";
            isFirst = true;

            List<EducationalExperience> educationalExperiences =
                UsersController.get_educational_experiences(paramsContainer.Tenant.Id, userId);

            int schoolEducationCount = educationalExperiences.Where(e => e.IsSchool == true).Select(e => e).Count();
            int instituteEducationCount = educationalExperiences.Where(e => e.IsSchool == false).Select(e => e).Count();

            responseText += ", \"SchoolEducationCount\":\"" + schoolEducationCount + "\"," +
                "\"InstituteEducationCount\":\"" + instituteEducationCount + "\"," +
                "\"EducationalExperiences\":[";

            foreach (EducationalExperience ee in educationalExperiences)
            {
                responseText += (isFirst ? "" : ",");
                isFirst = false;

                responseText += "{ " +
                    "\"EducationID\":" + "\"" + ee.EducationID + "\"," +
                    "\"UserID\":" + "\"" + ee.UserID + "\"," +
                    "\"School\":" + "\"" + Base64.encode(ee.School) + "\"," +
                    "\"StudyField\":" + "\"" + Base64.encode(ee.StudyField) + "\"," +
                    "\"Level\": \"" + ee.Level + "\"," +
                    "\"JStartDate\": \"" + (ee.StartDate.HasValue ? GenericDate.get_local_date(ee.StartDate.Value, false) : "") + "\"," +
                    "\"GStartDate\":" + "\"" + (ee.StartDate.HasValue ? ee.StartDate.Value.ToString() : "") + "\"," +
                    "\"JEndDate\": \"" + (ee.EndDate.HasValue ? GenericDate.get_local_date(ee.EndDate.Value, false) : string.Empty) + "\"," +
                    "\"GEndDate\":" + "\"" + (ee.EndDate.HasValue ? ee.EndDate.Value.ToString() : string.Empty) + "\"," +
                    "\"GraduateDegree\":" + "\"" + ee.GraduateDegree + "\"," +
                    "\"IsSchool\":" + ee.IsSchool.ToString().ToLower() +
                "}";
            }

            responseText += "]";
            isFirst = true;
            List<HonorsAndAwards> Honors = UsersController.get_honors_and_awards(paramsContainer.Tenant.Id, userId);
            responseText += ",\"HonorsAndAwards\":[";

            foreach (HonorsAndAwards hnr in Honors)
            {
                responseText += (isFirst ? "" : ",");
                isFirst = false;

                responseText += "{ " +
                    "\"HonorID\":" + "\"" + hnr.ID + "\"," +
                    "\"Title\":" + "\"" + Base64.encode(hnr.Title) + "\"," +
                    "\"Issuer\":" + "\"" + Base64.encode(hnr.Issuer) + "\"," +
                    "\"Occupation\":" + "\"" + Base64.encode(hnr.Occupation) + "\"," +
                    "\"JIssueDate\":" + "\"" + (hnr.IssueDate.HasValue ? GenericDate.get_local_date(hnr.IssueDate.Value, false) : "") + "\"," +
                    "\"GIssueDate\":" + "\"" + (hnr.IssueDate.HasValue ? hnr.IssueDate.Value.ToString() : "") + "\"," +
                    "\"Description\":" + "\"" + Base64.encode(hnr.Description) + "\"" +
                "}";
            }

            responseText += "]";
            isFirst = true;
            List<Modules.Users.Language> langs = UsersController.get_user_languages(paramsContainer.Tenant.Id, userId);
            responseText += ",\"Languages\":[";

            foreach (Modules.Users.Language lang in langs)
            {
                responseText += (isFirst ? "" : ",");
                isFirst = false;

                responseText += "{ " +
                    "\"ID\":" + "\"" + lang.ID + "\"," +
                    "\"LanguageID\":" + "\"" + lang.LanguageID + "\"," +
                    "\"LanguageName\":" + "\"" + Base64.encode(lang.LanguageName) + "\"," +
                    "\"Level\":" + "\"" + lang.Level + "\"" +
                "}";
            }

            responseText += "]}";
        }

        protected void get_constant_info(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            string[] _educationLevelArr = Enum.GetNames(typeof(GraduateDegree));
            string[] _educationDegreeArr = Enum.GetNames(typeof(EducationalLevel));
            string[] _languageLevelArr = Enum.GetNames(typeof(LanguageLevel));

            //EducationalLevels
            bool isFirstLevel = true;
            responseText = "{\"GraduateDegrees\":[";

            foreach (string s in _educationLevelArr)
            {
                if (s != GraduateDegree.None.ToString())
                {
                    responseText += (isFirstLevel ? "" : ",");
                    isFirstLevel = false;
                    responseText += "\"" + s + "\"";
                }
            }

            responseText += "]";

            //LanguageLevels
            bool isFirstlangLevel = true;
            responseText += ", \"LanguageLevels\":[";

            foreach (string s in _languageLevelArr)
            {
                if (s != LanguageLevel.None.ToString())
                {
                    responseText += (isFirstlangLevel ? "" : ",");
                    isFirstlangLevel = false;
                    responseText += "\"" + s + "\"";
                }
            }

            responseText += "]";

            //EducationalDegrees
            bool isFirstDegree = true;
            responseText += ", \"EducationalLevels\":[";

            foreach (string s in _educationDegreeArr)
            {
                if (s != EducationalLevel.None.ToString())
                {
                    responseText += (isFirstDegree ? "" : ",");
                    isFirstDegree = false;
                    responseText += "\"" + s + "\"";
                }
            }

            responseText += "]}";
        }

        protected void get_languages(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<Modules.Users.Language> langs = UsersController.get_languages(paramsContainer.Tenant.Id);

            responseText = "{\"Languages\":[";

            bool isFirst = true;
            foreach (Modules.Users.Language lng in langs)
            {
                responseText += (isFirst ? string.Empty : ",") + "{\"LanguageID\":\"" + lng.LanguageID.Value.ToString() +
                    "\",\"LanguageName\":\"" + Base64.encode(lng.LanguageName) + "\"}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void set_job_experience(Guid? jobId, Guid userId, string title, string employer,
            DateTime? startDate, DateTime? endDate, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(title) && title.Length > 250) ||
                (!string.IsNullOrEmpty(employer) && employer.Length > 250))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title) || !PublicMethods.is_secure_title(employer))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (jobId.HasValue && jobId != Guid.Empty)
            {
                JobExperience obj = UsersController.get_job_experience(paramsContainer.Tenant.Id, jobId.Value);

                if (obj == null || (obj.UserID != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }

                userId = obj.UserID.Value;
            }
            else
            {
                jobId = Guid.NewGuid();

                if (userId != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                    userId = paramsContainer.CurrentUserID.Value;
            }

            bool result = jobId.HasValue && UsersController.set_job_experience(paramsContainer.Tenant.Id, jobId.Value,
                userId, paramsContainer.CurrentUserID.Value, title, employer, startDate, endDate);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"JobId\":\"" + jobId.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_educational_experience(Guid? educationId, Guid userId, string school, string studyField,
            EducationalLevel? degree, GraduateDegree? level, DateTime? startDate, DateTime? endDate, bool isSchool, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(school) && school.Length > 250) ||
                (!string.IsNullOrEmpty(studyField) && studyField.Length > 250))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(school) || !PublicMethods.is_secure_title(studyField))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (educationId.HasValue && educationId != Guid.Empty)
            {
                EducationalExperience obj =
                    UsersController.get_educational_experience(paramsContainer.Tenant.Id, educationId.Value);

                if (obj == null || (obj.UserID != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }

                userId = obj.UserID.Value;
            }
            else
            {
                educationId = Guid.NewGuid();

                if (userId != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                    userId = paramsContainer.CurrentUserID.Value;
            }

            bool result = educationId.HasValue &&
                UsersController.set_educational_experience(paramsContainer.Tenant.Id, educationId.Value, userId,
                paramsContainer.CurrentUserID.Value, school, studyField, degree, level, startDate, endDate, isSchool);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"EducationID\":\"" + educationId.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_honor_and_award(Guid? honorId, Guid userId, string title, string occupation, string issuer,
            DateTime? issueDate, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if ((!string.IsNullOrEmpty(title) && title.Length > 500) ||
                (!string.IsNullOrEmpty(occupation) && occupation.Length > 500) ||
                (!string.IsNullOrEmpty(issuer) && issuer.Length > 2500))
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title) || !PublicMethods.is_secure_title(occupation) ||
                !PublicMethods.is_secure_title(issuer) || !PublicMethods.is_secure_title(description))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (honorId.HasValue && honorId != Guid.Empty)
            {
                HonorsAndAwards obj = UsersController.get_honor_or_award(paramsContainer.Tenant.Id, honorId.Value);

                if (obj == null || (obj.UserID.Value != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }

                userId = obj.UserID.Value;
            }
            else
            {
                honorId = Guid.NewGuid();

                if (userId != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                    userId = paramsContainer.CurrentUserID.Value;
            }

            bool result = honorId.HasValue && UsersController.set_honor_and_award(paramsContainer.Tenant.Id, honorId.Value,
                userId, paramsContainer.CurrentUserID.Value, title, occupation, issuer, issueDate, description);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"HonorID\":\"" + honorId.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_language(Guid? Id, string languageName, Guid userId, LanguageLevel langLevel, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!PublicMethods.is_secure_title(languageName))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (Id.HasValue && Id != Guid.Empty)
            {
                Modules.Users.Language obj = UsersController.get_user_language(paramsContainer.Tenant.Id, Id.Value);

                if (obj == null || (obj.UserID.Value != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }

                userId = obj.UserID.Value;
            }
            else
            {
                Id = Guid.NewGuid();

                if (userId != paramsContainer.CurrentUserID &&
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                    userId = paramsContainer.CurrentUserID.Value;
            }

            bool result = Id.HasValue && UsersController.set_language(paramsContainer.Tenant.Id,
                Id.Value, languageName, userId, paramsContainer.CurrentUserID.Value, langLevel);

            responseText = result ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"ID\":\"" + Id.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_job_experience(Guid? jobId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (jobId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                JobExperience obj = UsersController.get_job_experience(paramsContainer.Tenant.Id, jobId.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = jobId.HasValue && UsersController.remove_job_experience(paramsContainer.Tenant.Id, jobId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_education_experience(Guid? educationId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (educationId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                EducationalExperience obj =
                    UsersController.get_educational_experience(paramsContainer.Tenant.Id, educationId.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = educationId.HasValue &&
                UsersController.remove_education_experience(paramsContainer.Tenant.Id, educationId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_honor_and_award(Guid? honorId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (honorId.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                HonorsAndAwards obj = UsersController.get_honor_or_award(paramsContainer.Tenant.Id, honorId.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = honorId.HasValue &&
                UsersController.remove_honor_and_award(paramsContainer.Tenant.Id, honorId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_language(Guid? Id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (Id.HasValue &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
            {
                Modules.Users.Language obj = UsersController.get_user_language(paramsContainer.Tenant.Id, Id.Value);

                if (obj == null || obj.UserID != paramsContainer.CurrentUserID)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    return;
                }
            }

            bool result = Id.HasValue && UsersController.remove_language(paramsContainer.Tenant.Id, Id.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        /* end of Profile */

        /* User Groups */

        protected void create_user_group(string name, ref string responseText)
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

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid groupId = Guid.NewGuid();

            bool result = UsersController.create_user_group(paramsContainer.Tenant.Id, groupId,
                name, string.Empty, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"ID\":\"" + groupId.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateUserGroup,
                    SubjectID = groupId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void rename_user_group(Guid? id, string name, ref string responseText)
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

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue ? false : UsersController.modify_user_group(paramsContainer.Tenant.Id,
                id.Value, name, string.Empty, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyUserGroup,
                    SubjectID = id.Value,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void remove_user_group(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue ? false :
                UsersController.remove_user_group(paramsContainer.Tenant.Id, id.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveUserGroup,
                    SubjectID = id.Value,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_user_groups(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<UserGroup> groups = UsersController.get_user_groups(paramsContainer.Tenant.Id);

            responseText = "{\"Groups\":[" + ProviderUtil.list_to_string<string>(groups.Select(
                u => "{\"ID\":\"" + u.GroupID.ToString() + "\",\"Name\":\"" + Base64.encode(u.Title) + "\"}")
                .ToList()) + "]}";
        }

        protected void add_user_group_member(Guid? id, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue || !userId.HasValue ? false :
                UsersController.add_user_group_member(paramsContainer.Tenant.Id,
                id.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"ImageURL\":\"" +
                DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, userId.Value) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddUserGroupMember,
                    SubjectID = userId,
                    SecondSubjectID = id.Value,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void remove_user_group_member(Guid? id, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue || !userId.HasValue ? false :
                UsersController.remove_user_group_member(paramsContainer.Tenant.Id,
                id.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveUserGroupMember,
                    SubjectID = userId,
                    SecondSubjectID = id.Value,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void get_user_group_members(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<User> members = !id.HasValue ? new List<User>() :
                UsersController.get_user_group_members(paramsContainer.Tenant.Id, id.Value);

            responseText = "{\"Members\":[" + ProviderUtil.list_to_string<string>(members.Select(
                u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                ",\"ImageURL\":\"" +
                    DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                "}").ToList()) + "]}";
        }

        protected void get_access_roles(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<AccessRole> roles = AccessRole.remove_ref_tenant_specific_roles(paramsContainer.ApplicationID,
                UsersController.get_access_roles(paramsContainer.Tenant.Id));

            responseText = "{\"Roles\":[" + string.Join(",", roles.Select(u => u.toJson())) + "]}";
        }

        protected void get_user_group_permissions(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<AccessRole> roles = !id.HasValue ? new List<AccessRole>() :
                UsersController.get_user_group_access_roles(paramsContainer.Tenant.Id, id.Value);

            responseText = "{\"Permissions\":[" + ProviderUtil.list_to_string<string>(roles.Select(
                u => "{\"ID\":\"" + u.RoleID.ToString() + "\"" + ",\"Title\":\"" + Base64.encode(u.Title) + "\"" + "}")
                .ToList()) + "]}";
        }

        protected void set_user_group_permission(Guid? id, Guid? groupId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue || !groupId.HasValue ? false :
                UsersController.set_user_group_permission(paramsContainer.Tenant.Id,
                groupId.Value, id.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetUserGroupPermission,
                    SubjectID = id.Value,
                    SecondSubjectID = groupId,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        protected void unset_user_group_permission(Guid? id, Guid? groupId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.UserGroupsManagement, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = !id.HasValue || !groupId.HasValue ? false :
                UsersController.unset_user_group_permission(paramsContainer.Tenant.Id,
                groupId.Value, id.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveUserGroupPermission,
                    SubjectID = id.Value,
                    SecondSubjectID = groupId,
                    ModuleIdentifier = ModuleIdentifier.USR
                });
            }
            //end of Save Log
        }

        /* end of User Groups */

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}