using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.CoreNetwork;

namespace RaaiVan.Modules.Users
{
    public enum VisitItemTypes
    {
        Knowledge,
        Node,
        User,
        Question
    }

    public enum EmploymentType
    {
        NotSet,
        Permanent, //Rasmi
        Temporary, //Gharardadi
        Contract, //Peymani
        Other
    }

    public enum PhoneType
    {
        NotSet,
        Mobile,
        Home,
        Office,
        Fax,
        Other
    }

    public enum GraduateDegree
    {
        First,
        Second,
        Third,
        Other,
        None
    }

    public enum EducationalLevel
    {
        Diploma,
        PostDiploma,
        Bachelor,
        Master,
        Doctor,
        None
    }

    public enum LanguageLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        None
    }

    public enum TwoStepAuthentication
    {
        None,
        On,
        Email,
        SMS
    }

    public static class UserUtilities
    {
        private static Dictionary<Guid, User> _SystemUsers = new Dictionary<Guid, User>();
        public static User SystemUser(Guid? applicationId)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            try
            {
                if (_SystemUsers.ContainsKey(!applicationId.HasValue ? Guid.Empty : applicationId.Value))
                    return _SystemUsers[!applicationId.HasValue ? Guid.Empty : applicationId.Value];
                else
                {
                    User su = UsersController.get_system_user(applicationId);
                    if (su != null) _SystemUsers[!applicationId.HasValue ? Guid.Empty : applicationId.Value] = su;
                    return su;
                }
            }
            catch { return null; }
        }

        public static bool check_user_existence_in_active_directory(Guid? applicationId,
            string domainName, string username, ref string errorMessage)
        {
            try
            {
                if (RaaiVanSettings.Users.IgnoreActiveDirectoryUserCheck(applicationId)) return true;

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, /* "LDAP://" +  */ domainName);
                return UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username) != null;
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "ActiveDir_UserExistanceCheck", ex, ModuleIdentifier.RV);
                errorMessage = "CheckingUserExistanceInActiveDirectoryFailed";
                return false;
            }
        }

        public static bool is_active_directory_authenticated(Guid? applicationId,
            string domainName, string username, string password, ref string errorMessage)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, /* "LDAP://" +  */ domainName);
                return ctx.ValidateCredentials(username, password);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "ActiveDir_UserExistanceCheck", ex, ModuleIdentifier.RV);
                errorMessage = "ValidatingUserInActiveDirectoryFailed";
                return false;
            }

            /*
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainName, domainName + @"\" + username, password);
                DirectorySearcher searcher = new DirectorySearcher(entry);
                searcher.Filter = "(SamAccountName=" + username + ")";
                searcher.PropertiesToLoad.Add("cn");

                return searcher.FindOne() != null;
            }
            catch (Exception ex) { return false; }
            */
        }

        public static List<EmploymentType> get_employment_types()
        {
            List<EmploymentType> types = new List<EmploymentType>();

            Array arr = Enum.GetValues(typeof(EmploymentType));
            for (int i = 0, lnt = arr.Length; i < lnt; ++i)
                if ((EmploymentType)arr.GetValue(i) != EmploymentType.NotSet) types.Add((EmploymentType)arr.GetValue(i));

            return types;
        }

        public static string generate_password_salt()
        {
            var buf = new byte[16];
            (new System.Security.Cryptography.RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public static string encode_password(string password, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] src = Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            System.Security.Cryptography.HashAlgorithm algorithm = System.Security.Cryptography.HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        private static Random _RND = new Random((int)DateTime.Now.Ticks);

        public static string generate_password(int length = 12)
        {
            string refStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_@^*$!";

            string password = string.Empty;
            for (int i = 0; i < length; ++i)
                password += refStr[_RND.Next(0, refStr.Length - 1)];

            return password;
        }

        public static bool check_password_policy(Guid? applicationId, string password, string oldPassword, ref string errorMessage)
        {
            if (string.IsNullOrEmpty(password)) password = string.Empty;

            bool meetsLength = password.Length >= Math.Max(1, RaaiVanSettings.Users.PasswordPolicy.MinLength(applicationId));
            bool meetsNewCharacters = oldPassword == null || PublicMethods.diff(password, oldPassword).Length >=
                RaaiVanSettings.Users.PasswordPolicy.NewCharacters(applicationId);
            bool meetsUpperLower = !RaaiVanSettings.Users.PasswordPolicy.UpperLower(applicationId) || (
                (new Regex("[A-Z]").IsMatch(password)) && (new Regex("[a-z]").IsMatch(password)));
            bool meetsNonAlphabetic = !RaaiVanSettings.Users.PasswordPolicy.NonAlphabetic(applicationId) ||
                !(new Regex("^[a-zA-Z]+$").IsMatch(password));
            bool meetsNumber = !RaaiVanSettings.Users.PasswordPolicy.Number(applicationId) ||
                (new Regex("[0-9]").IsMatch(password));
            bool meetsNonAlphaNumeric = !RaaiVanSettings.Users.PasswordPolicy.NonAlphaNumeric(applicationId) ||
                !(new Regex("^[a-zA-Z0-9]+$").IsMatch(password));

            List<string> notMeet = new List<string>();

            if (!meetsLength) notMeet.Add("MinLength_" + RaaiVanSettings.Users.PasswordPolicy.MinLength(applicationId).ToString());
            if (!meetsNewCharacters) notMeet.Add("NewCharacters_" + RaaiVanSettings.Users.PasswordPolicy.NewCharacters(applicationId).ToString());
            if (!meetsUpperLower) notMeet.Add("UpperLower");
            if (!meetsNonAlphabetic) notMeet.Add("NonAlphabetic");
            if (!meetsNumber) notMeet.Add("Number");
            if (!meetsNonAlphaNumeric) notMeet.Add("NonAlphaNumeric");

            if (notMeet.Count > 0)
            {
                errorMessage = "{\"ErrorText\":\"" + Messages.PasswordPolicyDidntMeet.ToString() + "\"" +
                    ",\"NotMeetItems\":[" + string.Join(",", notMeet.Select(u => "\"" + u + "\"").ToList()) + "]}";
                return false;
            }

            return true;
        }

        public static bool check_password_policy(Guid? applicationId, string password, string oldPassword)
        {
            string str = string.Empty;
            return check_password_policy(applicationId, password, oldPassword, ref str);
        }

        public static bool validate_user(Guid? applicationId, string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            string savedPass = string.Empty, savedSalt = string.Empty;
            UsersController.get_current_password(applicationId, username, ref savedPass, ref savedSalt);

            bool loggedIn = false;
            int failedLoginAttemptsCount = 0;

            if (string.IsNullOrEmpty(savedPass) || string.IsNullOrEmpty(savedSalt)) return false;
            password = PublicMethods.verify_string(password);

            loggedIn = UserUtilities.encode_password(password, savedSalt) == savedPass;
            failedLoginAttemptsCount = UsersController.login_attempt(applicationId, username, loggedIn);

            if (!loggedIn &&
                failedLoginAttemptsCount >= RaaiVanSettings.AllowedConsecutiveFailedLoginAttempts(applicationId))
                UsersController.lock_user(applicationId, username);

            return loggedIn;
        }

        public static void create_admin_user(Guid applicationId)
        {
            UsersController.create_admin_user(applicationId);
        }
    }

    public class ItemVisitsCount
    {
        private Guid? _ItemID;
        private int? _Count;

        public Guid? ItemID
        {
            get { return _ItemID; }
            set { _ItemID = value; }
        }

        public int? Count
        {
            get { return _Count; }
            set { _Count = value; }
        }
    }

    public class Password
    {
        private string _Value;
        private string _Salt;
        private string _Salted;
        private string _Encrypted;
        public bool? AutoGenerated;

        public Password()
        {
            _Salt = UserUtilities.generate_password_salt();
        }

        public Password(string value)
        {
            _Value = value;
            _Salt = UserUtilities.generate_password_salt();
            _set();
        }

        private void _set()
        {
            _Salted = UserUtilities.encode_password(_Value, _Salt);
            _Encrypted = PublicMethods.sha1(_Value);
        }

        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                _set();
            }
        }

        public string Salt
        {
            get { return _Salt; }
        }

        public string Salted
        {
            get { return _Salted; }
        }

        public string Encrypted
        {
            get { return _Encrypted; }
        }
    }

    public class User
    {
        public Guid? UserID;
        public string UserName;
        public string Password;
        public string PasswordSalt;
        public string SaltedPassword;
        public string FirstName;
        public string LastName;
        public string AvatarName;
        public TwoStepAuthentication TwoStepAuthentication;
        public bool? EnableNewsLetter;
        public DateTime? Birthday;
        public string AboutMe;
        public string City;
        public string Organization;
        public string Department;
        public string JobTitle;
        public EmploymentType EmploymentType;
        public Guid? MainPhoneID;
        public Guid? MainEmailID;
        public List<PhoneNumber> PhoneNumbers;
        public List<EmailAddress> Emails;
        public List<NodeMember> MembershipAreas;
        public bool? IsApproved;
        public bool? IsLockedOut;
        public DateTime? LastLockoutDate;
        public Dictionary<string, object> Settings;

        public User()
        {
            TwoStepAuthentication = TwoStepAuthentication.None;
            EmploymentType = EmploymentType.NotSet;
            PhoneNumbers = new List<PhoneNumber>();
            Emails = new List<EmailAddress>();
            MembershipAreas = new List<NodeMember>();
            Settings = new Dictionary<string, object>();
        }

        public string FullName
        {
            get
            {
                return ((string.IsNullOrEmpty(FirstName) ? string.Empty : FirstName) + " " +
                    (string.IsNullOrEmpty(LastName) ? string.Empty : LastName)).Trim();
            }
        }

        public bool profileCompleted()
        {
            return UserID.HasValue && UserID != Guid.Empty && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);
        }

        public string toJson(Guid? applicationId = null, bool profileImageUrl = false,
            bool highQualityProfileImageUrl = false, bool coverPhotoUrl = false, bool highQualityCoverPhotoUrl = false)
        {
            string imageUrl = !profileImageUrl ? string.Empty :
                DocumentUtilities.get_personal_image_address(applicationId, UserID.HasValue ? UserID.Value : Guid.Empty);

            string hqImageUrl = !highQualityProfileImageUrl || !UserID.HasValue ? string.Empty :
                DocumentUtilities.get_personal_image_address(applicationId, UserID.Value, highQuality: true);

            return "{\"UserID\":\"" + (UserID.HasValue ? UserID.ToString() : string.Empty) + "\"" +
                ",\"UserName\":\"" + Base64.encode(UserName) + "\"" +
                ",\"FirstName\":\"" + Base64.encode(FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(LastName) + "\"" +
                ",\"FullName\":\"" + Base64.encode(FullName) + "\"" +
                (Birthday.HasValue ? ",\"Birthday\":\"" + GenericDate.get_local_date(Birthday.Value) + "\"" : string.Empty) +
                (EmploymentType == EmploymentType.NotSet ? string.Empty :
                    ",\"EmploymentType\":\"" + EmploymentType.ToString() + "\"") +
                (!EnableNewsLetter.HasValue || !EnableNewsLetter.Value ? string.Empty :
                    ",\"EnableNewsLetter\":" + EnableNewsLetter.Value.ToString().ToLower()
                ) +
                (TwoStepAuthentication == TwoStepAuthentication.None ? string.Empty :
                    ",\"TwoStepAuthentication\":\"" + TwoStepAuthentication.ToString() + "\"" +
                    ",\"VerificationCodeMedia\":\"" + TwoStepAuthentication.ToString() + "\""
                ) +
                (MainPhoneID.HasValue ? ",\"MainPhoneID\":\"" + MainPhoneID.ToString() + "\"" : string.Empty) +
                (MainEmailID.HasValue ? ",\"MainEmailID\":\"" + MainEmailID.ToString() + "\"" : string.Empty) +
                ",\"IncompleteProfile\":" + (!profileCompleted()).ToString().ToLower() +
                (string.IsNullOrEmpty(AboutMe) ? string.Empty :
                    ",\"AboutMe\":\"" + Base64.encode(AboutMe) + "\"") +
                (string.IsNullOrEmpty(City) ? string.Empty :
                    ",\"City\":\"" + Base64.encode(City) + "\"") +
                (string.IsNullOrEmpty(Organization) ? string.Empty :
                    ",\"Organization\":\"" + Base64.encode(Organization) + "\"") +
                (string.IsNullOrEmpty(Department) ? string.Empty :
                    ",\"Department\":\"" + Base64.encode(Department) + "\"") +
                (string.IsNullOrEmpty(JobTitle) ? string.Empty :
                    ",\"JobTitle\":\"" + Base64.encode(JobTitle) + "\"") +
                (string.IsNullOrEmpty(AvatarName) ? string.Empty :
                    ",\"AvatarName\":\"" + AvatarName + "\"") +
                (string.IsNullOrEmpty(imageUrl) ? string.Empty :
                    ",\"ProfileImageURL\":\"" + imageUrl + "\"" +
                    ",\"ImageURL\":\"" + imageUrl + "\""
                ) +
                (string.IsNullOrEmpty(hqImageUrl) ? string.Empty : ",\"HighQualityImageURL\":\"" + hqImageUrl + "\"") +
                (!coverPhotoUrl || !UserID.HasValue ? string.Empty : ",\"CoverPhotoURL\":\"" +
                    DocumentUtilities.get_cover_photo_url(applicationId, UserID.Value, networkAddress: false, highQuality: false) + "\"") +
                (!highQualityCoverPhotoUrl || !UserID.HasValue ? string.Empty : ",\"HighQualityCoverPhotoURL\":\"" +
                    DocumentUtilities.get_cover_photo_url(applicationId, UserID.Value, networkAddress: false, highQuality: true) + "\"") +
                (PhoneNumbers == null || PhoneNumbers.Count == 0 ? string.Empty :
                    ",\"PhoneNumbers\":[" + string.Join(",", PhoneNumbers.Select(n => n.toJson())) + "]") +
                (Emails == null || Emails.Count == 0 ? string.Empty :
                    ",\"Emails\":[" + string.Join(",", Emails.Select(e => e.toJson())) + "]") +
                (MembershipAreas == null || MembershipAreas.Count == 0 ? string.Empty :
                    ",\"Nodes\":[" + string.Join(",", MembershipAreas.Select(nm =>
                    {
                        return "{\"NodeID\":\"" + nm.Node.NodeID.ToString() + "\"" +
                            ",\"Name\":\"" + Base64.encode(nm.Node.Name) + "\"" +
                            ",\"NodeTypeID\":\"" + nm.Node.NodeTypeID.ToString() + "\"" +
                            "}";
                    })) + "]") +
                (Settings == null || Settings.Keys.Count == 0 ? string.Empty :
                    ",\"Settings\":" + PublicMethods.toJSON(Settings)) +
                "}";
        }
    }

    public class ApplicationUsers
    {
        public Guid? ApplicationID;
        public int? Count;
        public List<User> Users;

        public ApplicationUsers()
        {
            Users = new List<User>();
        }

        public string toJson()
        {
            return "{\"ApplicationID\":\"" + (!ApplicationID.HasValue ? string.Empty : ApplicationID.ToString()) + "\"" +
                ",\"TotalCount\":" + (!Count.HasValue ? 0 : Count.Value).ToString() +
                ",\"Users\":[" + string.Join(",", Users.Where(u => u != null)
                    .Select(u => u.toJson(ApplicationID, profileImageUrl: true))) + "]" +
                "}";
        }
    }

    public class Friend
    {
        public User User;
        public DateTime? RequestDate;
        public DateTime? AcceptionDate;
        public bool? AreFriends;
        public bool? IsSender;
        public int? MutualFriendsCount;

        public Friend()
        {
            User = new User();
        }
    }

    public class FriendSuggestion
    {
        private User _User;
        private int? _MutualFriends;

        public FriendSuggestion()
        {
            _User = new User();
        }

        public User User
        {
            get { return _User; }
            set { _User = value; }
        }

        public int? MutualFriends
        {
            get { return _MutualFriends; }
            set { _MutualFriends = value; }
        }
    }

    public class Invitation
    {
        private Guid? _InvitationID;
        private User _SenderUser;
        private User _ReceiverUser;
        private DateTime? _SendDate;
        private string _Email;
        private bool? _Activated;

        public Invitation()
        {
            _SenderUser = new User();
            _ReceiverUser = new User();
        }

        public Guid? InvitationID
        {
            get { return _InvitationID; }
            set { _InvitationID = value; }
        }

        public User SenderUser
        {
            get { return _SenderUser; }
            set { _SenderUser = value; }
        }

        public User ReceiverUser
        {
            get { return _ReceiverUser; }
            set { _ReceiverUser = value; }
        }

        public DateTime? SendDate
        {
            get { return _SendDate; }
            set { _SendDate = value; }
        }

        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        public bool? Activated
        {
            get { return _Activated; }
            set { _Activated = value; }
        }
    }

    public class PhoneNumber
    {
        public Guid? NumberID;
        public Guid? UserID;
        public string Number;
        public PhoneType PhoneType;
        public bool? IsMain;

        public string toJson()
        {
            return "{\"NumberID\":\"" + (!NumberID.HasValue ? string.Empty : NumberID.ToString()) + "\"" +
                ",\"Number\":\"" + Base64.encode(Number) + "\"" +
                ",\"Type\":\"" + (PhoneType == PhoneType.NotSet ? string.Empty : PhoneType.ToString()) + "\"" +
                (!IsMain.HasValue || !IsMain.Value ? string.Empty : ",\"IsMain\":" + IsMain.ToString().ToLower()) +
                "}";
        }
    }

    public class EmailAddress
    {
        public Guid? EmailID;
        public Guid? UserID;
        public string Address;
        public bool? IsMain;

        public string toJson()
        {
            return "{\"EmailID\":\"" + (!EmailID.HasValue ? string.Empty : EmailID.ToString()) + "\"" +
                ",\"Email\":\"" + Base64.encode(Address) + "\"" +
                (!IsMain.HasValue || !IsMain.Value ? string.Empty : ",\"IsMain\":" + IsMain.ToString().ToLower()) +
                "}";
        }
    }

    public class EmailContactStatus
    {
        public Guid? UserID;
        public string Email;
        public bool? FriendRequestReceived;
    }

    public class JobExperience
    {
        private Guid? _JobId;
        private Guid? _UserId;
        private string _Title;
        private string _Employer;
        private DateTime? _StartDate;
        private DateTime? _EndDate;

        public Guid? JobID
        {
            get { return _JobId; }
            set { _JobId = value; }
        }

        public Guid? UserID
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Employer
        {
            get { return _Employer; }
            set { _Employer = value; }
        }

        public DateTime? StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        public DateTime? EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
    }

    public class EducationalExperience
    {
        private Guid? _EducationId;
        private Guid? _UserId;
        private string _School;
        private string _StudyField;
        private EducationalLevel? _Level;
        private DateTime? _StartDate;
        private DateTime? _EndDate;
        private GraduateDegree? _GraduateDegree;
        public bool? IsSchool;

        public Guid? EducationID
        {
            get { return _EducationId; }
            set { _EducationId = value; }
        }

        public Guid? UserID
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public string School
        {
            get { return _School; }
            set { _School = value; }
        }

        public string StudyField
        {
            get { return _StudyField; }
            set { _StudyField = value; }
        }

        public EducationalLevel? Level
        {
            get { return _Level; }
            set { _Level = value; }
        }

        public DateTime? StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        public DateTime? EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        public GraduateDegree? GraduateDegree
        {
            get { return _GraduateDegree; }
            set { _GraduateDegree = value; }
        }
    }

    public class HonorsAndAwards
    {
        private Guid? _ID;
        private Guid? _UserID;
        private string _Title;
        private string _Issuer;
        private string _Occupation;
        private DateTime? _IssueDate;
        private string _Description;

        public Guid? ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public Guid? UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Issuer
        {
            get { return _Issuer; }
            set { _Issuer = value; }
        }

        public string Occupation
        {
            get { return _Occupation; }
            set { _Occupation = value; }
        }

        public DateTime? IssueDate
        {
            get { return _IssueDate; }
            set { _IssueDate = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }

    public class Language
    {
        public Guid? ID;
        public Guid? UserID;
        private Guid? _LanguageID;
        private string _LanguageName;
        private LanguageLevel? _Level;

        public Guid? LanguageID
        {
            get { return _LanguageID; }
            set { _LanguageID = value; }
        }

        public string LanguageName
        {
            get { return _LanguageName; }
            set { _LanguageName = value; }
        }

        public LanguageLevel? Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
    }

    public class UserGroup
    {
        public Guid? GroupID;
        public string Title;
        public string Description;
    }

    public enum AccessRoleName
    {
        None,
        ManagementSystem,
        UsersManagement,
        ManageConfidentialityLevels,
        UserGroupsManagement,
        ContentsManagement,
        Reports,
        DataImport,
        ManageOntology,
        ManageWorkflow,
        ManageForms,
        ManagePolls,
        KnowledgeAdmin,
        SMSEMailNotifier,
        ManageQA,
        RemoteServers
    }

    public class AccessRole
    {
        public Guid? RoleID;
        public AccessRoleName Name;
        public string Title;

        public string toJson()
        {
            return "{\"ID\":\"" + (!RoleID.HasValue ? string.Empty : RoleID.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                "}";
        }

        public static List<AccessRoleName> remove_ref_tenant_specific_roles(Guid? applicationId, List<AccessRoleName> names)
        {
            List<AccessRoleName> refTenantSpecificItems =
                !RaaiVanSettings.ReferenceTenantID.HasValue || RaaiVanSettings.ReferenceTenantID == applicationId ?
                new List<AccessRoleName>() :
                new List<AccessRoleName>()
                {
                    AccessRoleName.SMSEMailNotifier
                };

            return names.Where(n => !refTenantSpecificItems.Any(x => x == n)).ToList();
        }

        public static List<AccessRole> remove_ref_tenant_specific_roles(Guid? applicationId, List<AccessRole> roles)
        {
            List<AccessRoleName> names = remove_ref_tenant_specific_roles(applicationId, roles.Select(r => r.Name).Distinct().ToList());
            return roles.Where(r => names.Any(x => x == r.Name)).ToList();
        }
    }

    public class AdvancedUserSearch
    {
        public Guid? UserID;
        public double? Rank;
        public int? IsMemberCount;
        public int? IsExpertCount;
        public int? IsContributorCount;
        public int? HasPropertyCount;
        public int? Resume;
    }

    public class AdvancedUserSearchMeta
    {
        public Guid? NodeID;
        public double? Rank;
        public bool? IsMember;
        public bool? IsExpert;
        public bool? IsContributor;
        public bool? HasProperty;
    }

    public class RemoteServer
    {
        public Guid? ID;
        public Guid? UserID;
        public string Name;
        public string URL;
        public string UserName;
        public string Passwrod;

        public byte[] get_password_encrypted()
        {
            return DocumentUtilities.encrypt_bytes_aes_native(
                Encoding.UTF8.GetBytes(string.IsNullOrEmpty(Passwrod) ? string.Empty : Passwrod));
        }

        public void set_password_encrypted(byte[] value)
        {
            Passwrod = value == null ? string.Empty :
                Encoding.UTF8.GetString(DocumentUtilities.decrypt_bytes_aes_native(value));
        }

        public string toJson()
        {
            return "{\"ID\":" + (!ID.HasValue ? "null" : "\"" + ID.Value.ToString() + "\"") +
                //",\"UserID\":" + (!UserID.HasValue ? "null" : "\"" + UserID.Value.ToString() + "\"") +
                ",\"Name\":\"" + (string.IsNullOrEmpty(Name) ? string.Empty : Base64.encode(Name)) + "\"" +
                ",\"URL\":\"" + (string.IsNullOrEmpty(URL) ? string.Empty : Base64.encode(URL)) + "\"" +
                ",\"UserName\":\"" + (string.IsNullOrEmpty(UserName) ? string.Empty : Base64.encode(UserName)) + "\"" +
                ",\"Password\":\"" + (string.IsNullOrEmpty(Passwrod) ? string.Empty : Base64.encode(Passwrod)) + "\"" +
                "}";
        }
    }
}