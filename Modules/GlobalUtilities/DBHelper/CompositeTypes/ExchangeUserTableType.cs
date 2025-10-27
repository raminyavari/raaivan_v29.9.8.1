using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class ExchangeUserTableType : ITableType
    {
        public ExchangeUserTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "ExchangeUserTableType"; } }

        [PgName("user_id")]
        public Guid? UserID;

        [PgName("username")]
        public string UserName;

        [PgName("new_username")]
        public string NewUserName;

        [PgName("first_name")]
        public string FirstName;

        [PgName("last_name")]
        public string LastName;

        [PgName("employment_type")]
        public string EmploymentType;

        [PgName("department_id")]
        public string DepartmentID;

        [PgName("is_manager")]
        public bool? IsManager;

        [PgName("email")]
        public string Email;

        [PgName("phone_number")]
        public string PhoneNumber;

        [PgName("reset_password")]
        public bool? ResetPassword;

        [PgName("password")]
        public string Password;

        [PgName("password_salt")]
        public string PasswordSalt;

        [PgName("encrypted_password")]
        public string EncryptedPassword;

        public ExchangeUserTableType(Guid? userId, string username, string newUsername, string firstName, 
            string lastName, string employmentType, string departmentId, bool? isManager, string email, 
            string phoneNumber, bool? resetPassword, string password, string passwordSalt, string encryptedPassword)
        {
            UserID = userId;
            UserName = username;
            NewUserName = newUsername;
            FirstName = firstName;
            LastName = lastName;
            EmploymentType = employmentType;
            DepartmentID = departmentId;
            IsManager = isManager;
            Email = email;
            PhoneNumber = phoneNumber;
            ResetPassword = resetPassword;
            Password = password;
            PasswordSalt = passwordSalt;
            EncryptedPassword = encryptedPassword;
        }

        public object[] to_array()
        {
            return new List<object>() {
                UserID,
                UserName,
                NewUserName,
                FirstName,
                LastName,
                EmploymentType,
                DepartmentID,
                IsManager,
                Email,
                PhoneNumber,
                ResetPassword,
                Password,
                PasswordSalt,
                EncryptedPassword
            }.ToArray();
        }

        public ExchangeUserTableType[] get_array(List<ExchangeUserTableType> list)
        {
            return list.ToArray();
        }
    }
}
