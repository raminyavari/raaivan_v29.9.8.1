using RaaiVan.Modules.GlobalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.Users
{
    public static class USRParsers
    {
        public static List<User> users(DBResultSet results, ref long totalCount, 
            bool systemAlso = false, bool parseSettings = false)
        {
            List<User> retList = new List<User>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                User user = new User();

                user.UserID = table.GetGuid(i, "UserID");
                user.UserName = table.GetString(i, "UserName");
                user.FirstName = table.GetString(i, "FirstName");
                user.LastName = table.GetString(i, "LastName");

                if (!systemAlso && user.UserName.ToLower() == "system") continue;

                user.AboutMe = table.GetString(i, "AboutMe");
                user.City = table.GetString(i, "City");
                user.Organization = table.GetString(i, "Organization");
                user.Department = table.GetString(i, "Department");
                user.JobTitle = table.GetString(i, "JobTitle");
                user.Birthday = table.GetDate(i, "BirthDay");
                user.MainPhoneID = table.GetGuid(i, "MainPhoneID");
                user.MainEmailID = table.GetGuid(i, "MainEmailID");
                user.IsApproved = table.GetBool(i, "IsApproved");
                user.IsLockedOut = table.GetBool(i, "IsLockedOut");
                user.EmploymentType = table.GetEnum<EmploymentType>(i, "EmploymentType", EmploymentType.NotSet);
                user.TwoStepAuthentication = table.GetEnum<TwoStepAuthentication>(i, "TwoStepAuthentication", TwoStepAuthentication.None);
                if (parseSettings) user.Settings = PublicMethods.fromJSON(table.GetString(i, "Settings"));

                retList.Add(user);
            }

            totalCount = results.get_table(1).GetLong(row: 0, column: 0, defaultValue: 0).Value;

            return retList;
        }

        public static List<User> users(DBResultSet results, bool systemAlso = false, bool parseSettings = false)
        {
            long totalCount = 0;
            return users(results, ref totalCount, systemAlso, parseSettings);
        }

        public static List<ApplicationUsers> application_users(DBResultSet results)
        {
            List<ApplicationUsers> retList = new List<ApplicationUsers>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? userId = table.GetGuid(i, "UserID");
                Guid? applicationId = userId.HasValue ? table.GetGuid(i, "ApplicationID") : null;

                if (!applicationId.HasValue) continue;

                ApplicationUsers item = retList.Where(l => l.ApplicationID == applicationId).FirstOrDefault();

                if (item == null)
                {
                    item = new ApplicationUsers() { ApplicationID = applicationId };
                    retList.Add(item);
                }

                User user = new User()
                {
                    UserID = userId,
                    UserName = table.GetString(i, "UserName"),
                    FirstName = table.GetString(i, "FirstName"),
                    LastName = table.GetString(i, "LastName")
                };

                item.Count = table.GetInt(i, "TotalCount", defaultValue: 0);

                item.Users.Add(user);
            }

            return retList;
        }

        public static List<ItemVisitsCount> item_visits_count(DBResultSet results)
        {
            List<ItemVisitsCount> retList = new List<ItemVisitsCount>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                ItemVisitsCount visit = new ItemVisitsCount();

                visit.ItemID = table.GetGuid(i, "ItemID");
                visit.Count = table.GetInt(i, "VisitsCount");

                retList.Add(visit);
            }

            return retList;
        }

        public static List<Friend> friends(DBResultSet results, ref long totalCount)
        {
            List<Friend> retList = new List<Friend>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                Friend friend = new Friend();

                friend.User.UserID = table.GetGuid(i, "FriendID");
                friend.User.UserName = table.GetString(i, "UserName");
                friend.User.FirstName = table.GetString(i, "FirstName");
                friend.User.LastName = table.GetString(i, "LastName");
                friend.RequestDate = table.GetDate(i, "RequestDate");
                friend.AcceptionDate = table.GetDate(i, "AcceptionDate");
                friend.AreFriends = table.GetBool(i, "AreFriends");
                friend.IsSender = table.GetBool(i, "IsSender");
                friend.MutualFriendsCount = table.GetInt(i, "MutualFriendsCount");

                retList.Add(friend);
            }

            return retList;
        }

        public static List<Friend> friends(DBResultSet results) {
            long totalCount = 0;
            return friends(results, ref totalCount);
        }

        public static List<Friend> friendship_statuses(DBResultSet results)
        {
            List<Friend> retList = new List<Friend>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Friend friend = new Friend();

                friend.User.UserID = table.GetGuid(i, "UserID");
                friend.AreFriends = table.GetBool(i, "IsFriend");
                friend.IsSender = table.GetBool(i, "IsSender");
                friend.MutualFriendsCount = table.GetInt(i, "MutualFriendsCount");

                retList.Add(friend);
            }

            return retList;
        }

        public static List<PhoneNumber> phone_numbers(DBResultSet results)
        {
            List<PhoneNumber> retList = new List<PhoneNumber>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                PhoneNumber phoneNumber = new PhoneNumber();

                phoneNumber.UserID = table.GetGuid(i, "UserID");
                phoneNumber.NumberID = table.GetGuid(i, "NumberID");
                phoneNumber.Number = table.GetString(i, "PhoneNumber");
                phoneNumber.IsMain = table.GetBool(i, "IsMain");
                phoneNumber.PhoneType = table.GetEnum<PhoneType>(i, "PhoneType", PhoneType.NotSet);

                retList.Add(phoneNumber);
            }

            return retList;
        }

        public static List<EmailAddress> email_addresses(DBResultSet results)
        {
            List<EmailAddress> retList = new List<EmailAddress>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmailAddress emailAddress = new EmailAddress();

                emailAddress.UserID = table.GetGuid(i, "UserID");
                emailAddress.EmailID = table.GetGuid(i, "EmailID");
                emailAddress.Address = table.GetString(i, "EmailAddress");
                emailAddress.IsMain = table.GetBool(i, "IsMain");

                retList.Add(emailAddress);
            }

            return retList;
        }

        public static List<EmailContactStatus> email_contacts_status(DBResultSet results)
        {
            List<EmailContactStatus> retList = new List<EmailContactStatus>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmailContactStatus contact = new EmailContactStatus();

                contact.UserID = table.GetGuid(i, "UserID");
                contact.Email = table.GetString(i, "Email");
                contact.FriendRequestReceived = table.GetBool(i, "FriendRequestReceived");

                retList.Add(contact);
            }

            return retList;
        }

        public static List<JobExperience> job_experiences(DBResultSet results)
        {
            List<JobExperience> retList = new List<JobExperience>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                JobExperience jobExp = new JobExperience();

                jobExp.JobID = table.GetGuid(i, "JobID");
                jobExp.UserID = table.GetGuid(i, "UserID");
                jobExp.Title = table.GetString(i, "Title");
                jobExp.Employer = table.GetString(i, "Employer");
                jobExp.StartDate = table.GetDate(i, "StartDate");
                jobExp.EndDate = table.GetDate(i, "EndDate");

                retList.Add(jobExp);
            }

            return retList;
        }

        public static List<EducationalExperience> educational_experiences(DBResultSet results)
        {
            List<EducationalExperience> retList = new List<EducationalExperience>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                EducationalExperience educationExp = new EducationalExperience();

                educationExp.EducationID = table.GetGuid(i, "EducationID");
                educationExp.UserID = table.GetGuid(i, "UserID");
                educationExp.School = table.GetString(i, "School");
                educationExp.StudyField = table.GetString(i, "StudyField");
                educationExp.StartDate = table.GetDate(i, "StartDate");
                educationExp.EndDate = table.GetDate(i, "EndDate");
                educationExp.IsSchool = table.GetBool(i, "IsSchool");
                educationExp.Level = table.GetEnum<EducationalLevel>(i, "Level", EducationalLevel.None);
                educationExp.GraduateDegree = table.GetEnum<GraduateDegree>(i, "GraduateDegree", GraduateDegree.None);

                retList.Add(educationExp);
            }

            return retList;
        }

        public static List<HonorsAndAwards> honors_and_awards(DBResultSet results)
        {
            List<HonorsAndAwards> retList = new List<HonorsAndAwards>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                HonorsAndAwards honors = new HonorsAndAwards();

                honors.ID = table.GetGuid(i, "ID");
                honors.UserID = table.GetGuid(i, "UserID");
                honors.Title = table.GetString(i, "Title");
                honors.Issuer = table.GetString(i, "Issuer");
                honors.Occupation = table.GetString(i, "Occupation");
                honors.IssueDate = table.GetDate(i, "IssueDate");
                honors.Description = table.GetString(i, "Description");

                retList.Add(honors);
            }

            return retList;
        }

        public static List<Language> languages(DBResultSet results, bool isUserLanguage)
        {
            List<Language> retList = new List<Language>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Language lang = new Language();

                lang.LanguageName = table.GetString(i, "LanguageName");

                if (isUserLanguage)
                {
                    lang.ID = table.GetGuid(i, "ID");
                    lang.UserID = table.GetGuid(i, "UserID");
                    lang.Level = table.GetEnum<LanguageLevel>(i, "Level", LanguageLevel.None);

                }
                else
                    lang.LanguageID = table.GetGuid(i, "LanguageID");

                retList.Add(lang);
            }

            return retList;
        }

        public static List<FriendSuggestion> friend_suggestions(DBResultSet results, ref long totalCount)
        {
            List<FriendSuggestion> retList = new List<FriendSuggestion>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                FriendSuggestion fSuggestion = new FriendSuggestion();

                fSuggestion.User.UserID = table.GetGuid(i, "UserID");
                fSuggestion.User.FirstName = table.GetString(i, "FirstName");
                fSuggestion.User.LastName = table.GetString(i, "LastName");
                fSuggestion.MutualFriends = table.GetInt(i, "MutualFriendsCount");

                retList.Add(fSuggestion);
            }

            return retList;
        }

        public static List<FriendSuggestion> friend_suggestions(DBResultSet results) {
            long totalCount = 0;
            return friend_suggestions(results, ref totalCount);
        }

        public static List<Invitation> invitations(DBResultSet results, ref long totalCount)
        {
            List<Invitation> retList = new List<Invitation>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                Invitation inv = new Invitation();

                inv.ReceiverUser.UserID = table.GetGuid(i, "ReceiverUserID");
                inv.ReceiverUser.FirstName = table.GetString(i, "ReceiverFirstName");
                inv.ReceiverUser.LastName = table.GetString(i, "ReceiverLastName");
                inv.Email = table.GetString(i, "Email");
                inv.SendDate = table.GetDate(i, "SendDate");
                inv.Activated = table.GetBool(i, "Activated");

                retList.Add(inv);
            }

            return retList;
        }

        public static List<Invitation> invitations(DBResultSet results) {
            long totalCount = 0;
            return invitations(results, ref totalCount);
        }

        public static void password(DBResultSet results, ref string password, ref string passwordSalt)
        {
            RVDataTable table = results.get_table();

            password = table.GetString(0, "Password");
            passwordSalt = table.GetString(0, "PasswordSalt");
        }

        public static List<Password> last_passwords(DBResultSet results)
        {
            List<Password> retList = new List<Password>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Password pass = new Password();

                pass.Value = table.GetString(i, "Password");
                pass.AutoGenerated = table.GetBool(i, "AutoGenerated");

                retList.Add(pass);
            }

            return retList;
        }

        public static List<UserGroup> user_groups(DBResultSet results)
        {
            List<UserGroup> retList = new List<UserGroup>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                UserGroup grp = new UserGroup();

                grp.GroupID = table.GetGuid(i, "GroupID");
                grp.Title = table.GetString(i, "Title");
                grp.Description = table.GetString(i, "Description");

                retList.Add(grp);
            }

            return retList;
        }

        public static List<AccessRole> access_roles(DBResultSet results)
        {
            List<AccessRole> retList = new List<AccessRole>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                AccessRole rl = new AccessRole();

                rl.RoleID = table.GetGuid(i, "RoleID");
                rl.Title = table.GetString(i, "Title");
                rl.Name = table.GetEnum<AccessRoleName>(i, "Name", AccessRoleName.None);

                if (rl.Name != AccessRoleName.None) retList.Add(rl);
            }

            return retList;
        }

        public static List<AccessRoleName> permissions(DBResultSet results)
        {
            List<AccessRoleName> retList = new List<AccessRoleName>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                AccessRoleName nm = table.GetEnum<AccessRoleName>(row: i, column: 0, defaultValue: AccessRoleName.None);

                if (nm != AccessRoleName.None) retList.Add(nm);
            }

            return retList;
        }

        public static List<AdvancedUserSearch> advanced_user_search(DBResultSet results, ref long totalCount)
        {
            List<AdvancedUserSearch> retList = new List<AdvancedUserSearch>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                AdvancedUserSearch m = new AdvancedUserSearch();

                m.UserID = table.GetGuid(i, "UserID");
                m.Rank = table.GetDouble(i, "Rank");
                m.IsMemberCount = table.GetInt(i, "IsMemberCount");
                m.IsExpertCount = table.GetInt(i, "IsExpertCount");
                m.IsContributorCount = table.GetInt(i, "IsContributorCount");
                m.HasPropertyCount = table.GetInt(i, "HasPropertyCount");
                m.Resume = table.GetInt(i, "Resume");

                retList.Add(m);
            }

            return retList;
        }

        public static List<AdvancedUserSearchMeta> advanced_user_search_meta(DBResultSet results)
        {
            List<AdvancedUserSearchMeta> retList = new List<AdvancedUserSearchMeta>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                AdvancedUserSearchMeta m = new AdvancedUserSearchMeta();

                m.NodeID = table.GetGuid(i, "NodeID");
                m.Rank = table.GetDouble(i, "Rank");
                m.IsMember = table.GetBool(i, "IsMember");
                m.IsExpert = table.GetBool(i, "IsExpert");
                m.IsContributor = table.GetBool(i, "IsContributor");
                m.HasProperty = table.GetBool(i, "HasProperty");

                retList.Add(m);
            }

            return retList;
        }

        public static User lockout_date(DBResultSet results)
        {
            RVDataTable table = results.get_table();

            User usr = new User()
            {
                UserID = table.GetGuid(0, "UserID"),
                IsLockedOut = table.GetBool(0, "IsLockedOut"),
                LastLockoutDate = table.GetDate(0, "LastLockoutDate"),
                IsApproved = table.GetBool(0, "IsApproved")
            };

            return usr;
        }

        public static List<RemoteServer> remote_servers(DBResultSet results)
        {
            List<RemoteServer> retList = new List<RemoteServer>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                RemoteServer item = new RemoteServer();

                item.ID = table.GetGuid(i, "ServerID");
                item.UserID = table.GetGuid(i, "UserID");
                item.Name = table.GetString(i, "Name");
                item.URL = table.GetString(i, "URL");
                item.UserName = table.GetString(i, "UserName");
                item.set_password_encrypted(table.GetByteArray(i, "Password"));

                retList.Add(item);
            }

            return retList;
        }
    }
}
