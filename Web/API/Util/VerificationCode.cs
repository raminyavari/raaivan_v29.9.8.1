using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web.API
{
    [Serializable]
    public class VerificationCode
    {
        protected int TimeOut;
        protected static int TotalTimeOutCoefficient = 4;
        
        protected Guid? _ApplicationID;
        protected string _Token;
        protected long _Code;
        protected DateTime? _ExpirationDate;
        protected string _EmailAddress;
        protected string _PhoneNumber;
        protected string _Media;
        protected int _TTL;
        protected EmailTemplateType _EmailTemplate;
        protected EmailTemplateType _SMSTemplate;

        protected string _CustomData;

        protected static Dictionary<string, VerificationCode> Tokens = new Dictionary<string, VerificationCode>();

        public VerificationCode(Guid? applicationId, string emailAddress, string phoneNumber, string customData = "",
            EmailTemplateType emailTemplate = EmailTemplateType.None, EmailTemplateType smsTemplate = EmailTemplateType.None)
        {
            TimeOut = RaaiVanSettings.Users.TwoStepAuthenticationTimeout(applicationId);

            _ApplicationID = applicationId;
            _Token = PublicMethods.random_string(20);
            reset_code();
            _EmailAddress = emailAddress;
            _PhoneNumber = phoneNumber;
            _TTL = 3;

            _EmailTemplate = emailTemplate == EmailTemplateType.None ? EmailTemplateType.ConfirmationCode : emailTemplate;
            _SMSTemplate = smsTemplate == EmailTemplateType.None ? EmailTemplateType.ConfirmationCodeSMS : smsTemplate;

            _CustomData = customData;
        }

        protected bool update_ttl(string token)
        {
            if (_TTL <= 0) return false;
            else
            {
                --_TTL;

                if (RedisAPI.Enabled && !string.IsNullOrEmpty(token)) RedisAPI.set_value<VerificationCode>(token, this);

                return true;
            }
        }

        protected void reset_code() { _Code = PublicMethods.get_random_number(5); }

        protected void reset_expiration_date()
        {
            _ExpirationDate = DateTime.Now.AddSeconds(TimeOut + 10);
        }

        [JsonIgnore]
        public string Token { get { return _Token; } }

        [JsonIgnore]
        public long Code { get { return _Code; } }

        [JsonIgnore]
        public string CustomData { get { return _CustomData; } }

        protected bool use()
        {
            if (_ExpirationDate.HasValue) return false;

            reset_expiration_date();

            if (RedisAPI.Enabled) RedisAPI.set_value<VerificationCode>(_Token, this);
            else Tokens[_Token] = this;

            Task task = Task.Delay((TimeOut + 10) * 1000 * TotalTimeOutCoefficient)
                .ContinueWith(t =>
                {
                    if (RedisAPI.Enabled) RedisAPI.remove_key(_Token);
                    else lock (Tokens) { if (Tokens.ContainsKey(_Token)) Tokens.Remove(_Token); }
                });

            return true;
        }

        public bool send_code()
        {
            bool result = send_email() || send_sms();
            if (result) use();
            return result;
        }

        protected bool resend()
        {
            reset_expiration_date();
            reset_code();

            return _Media == "Email" ? send_email() : (_Media == "SMS" ? send_sms() : false);
        }

        protected virtual void email_body_subject(ref string body, ref string subject)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Code", _Code.ToString());

            body = EmailTemplates.get_email_template(_ApplicationID, _EmailTemplate, dic);
            subject = EmailTemplates.get_email_subject_template(_ApplicationID, _EmailTemplate, dic);
        }

        protected virtual string sms_body()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Code", _Code.ToString());

            return EmailTemplates.get_email_template(_ApplicationID, _SMSTemplate, dic);
        }

        protected bool send_email()
        {
            if (string.IsNullOrEmpty(_EmailAddress)) return false;

            string emailBody = string.Empty, emailSubject = string.Empty;

            email_body_subject(ref emailBody, ref emailSubject);

            bool result = PublicMethods.send_email(_ApplicationID, _EmailAddress, emailSubject, emailBody);
            if (result) _Media = "Email";

            return result;
        }

        protected bool send_sms()
        {
            if (string.IsNullOrEmpty(_PhoneNumber)) return false;

            bool result = PublicMethods.send_sms(_PhoneNumber, sms_body());
            if (result) _Media = "SMS";

            return result;
        }

        public string toJson()
        {
            return "{\"Token\":\"" + _Token + "\"" +
                ",\"Media\":\"" + _Media + "\"" +
                ",\"EmailAddress\":\"" + Base64.encode(_EmailAddress) + "\"" +
                ",\"PhoneNumber\":\"" + Base64.encode(_PhoneNumber) + "\"" +
                ",\"Timeout\":" + TimeOut.ToString() +
                ",\"TotalTimeout\":" + (TimeOut * (TotalTimeOutCoefficient - 1)).ToString() +
                ",\"Length\":" + Code.ToString().Length.ToString() +
                "}";
        }

        public static VerificationCode get_object(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            return RedisAPI.Enabled ? RedisAPI.get_value<VerificationCode>(token) :
                (Tokens.ContainsKey(token) ? Tokens[token] : null);
        }

        public static bool resend_code(string token)
        {
            VerificationCode vc = get_object(token);
            return vc != null && vc.resend();
        }

        public static VerificationCode validate(string token, long code, ref bool disposed)
        {
            disposed = false;
            
            VerificationCode t = get_object(token);

            if (t == null) return null;

            if (!t.update_ttl(token))
            {
                disposed = true;
                return null;
            }

            if (t._Code == code && t._ExpirationDate.HasValue && t._ExpirationDate.Value > DateTime.Now) return t;
            else return null;
        }

        public static bool process_request(Guid? applicationId, string emailAddress, string phoneNumber,
            string token, long? code, ref string responseText, string customData = "")
        {
            bool disposed = false;

            if (string.IsNullOrEmpty(token) || !code.HasValue)
            {
                VerificationCode vc = new VerificationCode(applicationId, emailAddress, phoneNumber, customData: customData);

                if (vc.send_code()) responseText = "{\"VerificationCode\":" + vc.toJson() + "}";
                else responseText = "{\"ErrorText\":\"" + Messages.SendingVerificationCodeFailed.ToString() + "\"}";

                return false;
            }
            else {
                VerificationCode vc = VerificationCode.validate(token, code.Value, ref disposed);

                if (vc != null)
                    responseText = vc.CustomData;
                else if (vc == null)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.VerificationCodeDidNotMatch.ToString() + "\"" +
                        ",\"CodeDisposed\":" + disposed.ToString().ToLower() + "}";
                    return false;
                }
            }

            return true;
        }

        public static bool process_request(Guid? applicationId, string emailAddress, string phoneNumber,
            ref string responseText, string customData = "")
        {
            return process_request(applicationId, emailAddress: emailAddress, phoneNumber: phoneNumber, 
                token: null, code: null, responseText: ref responseText, customData: customData);
        }

        public static bool process_request(Guid? applicationId, string token, long? code, ref string responseText)
        {
            return process_request(applicationId, emailAddress: null, phoneNumber: null,
                token: token, code: code, responseText: ref responseText);
        }
    }

    [Serializable]
    public class TwoStepAuthenticationToken : VerificationCode
    {
        private Guid? _UserID;
        private bool? _WasNormalUserPassLogin;

        [JsonIgnore]
        public Guid? UserID { get { return _UserID; } }

        public TwoStepAuthenticationToken(Guid? applicationId, Guid userId, bool wasNormalUserPassLogin, 
            string emailAddress, string phoneNumber) : 
            base(applicationId, emailAddress, phoneNumber, 
                emailTemplate: EmailTemplateType.TwoStepAuthenticationCode,
                smsTemplate: EmailTemplateType.TwoStepAuthenticationCodeSMS)
        {
            _UserID = userId;
            _WasNormalUserPassLogin = wasNormalUserPassLogin;
        }

        [JsonIgnore]
        public bool WasNormalUserPassLogin { get { return _WasNormalUserPassLogin.HasValue && _WasNormalUserPassLogin.Value; } }
    }
}