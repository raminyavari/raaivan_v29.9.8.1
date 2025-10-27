using System;
using System.Collections.Generic;
using System.IO;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum EmailTemplateType
    {
        None,
        Master,
        CreateAccount,
        InviteUser,
        InviteUserBatch,
        PasswordReset,
        TwoStepAuthenticationCode,
        TwoStepAuthenticationCodeSMS,
        ConfirmationCode,
        ConfirmationCodeSMS
    }

    public class EmailTemplateDictionary
    {
        private Dictionary<Guid, Dictionary<string, string>> Dic; //ApplicationID, TemplateType, TemplateBody

        public EmailTemplateDictionary() {
            Dic = new Dictionary<Guid, Dictionary<string, string>>();
        }

        public void set_value(Guid? applicationId, string templateType, string templateBody) {
            if (string.IsNullOrEmpty(templateType) || string.IsNullOrEmpty(templateBody)) return;

            if (!applicationId.HasValue) applicationId = Guid.Empty;
            if (!Dic.ContainsKey(applicationId.Value)) Dic[applicationId.Value] = new Dictionary<string, string>();
            Dic[applicationId.Value][templateType] = templateBody;
        }

        public string get_value(Guid? applicationId, string templateType)
        {
            if (!applicationId.HasValue) applicationId = Guid.Empty;

            return !string.IsNullOrEmpty(templateType) && Dic.ContainsKey(applicationId.Value) &&
                Dic[applicationId.Value].ContainsKey(templateType) ? Dic[applicationId.Value][templateType] : string.Empty;
        }
    }

    public class EmailTemplates
    {
        public static Dictionary<Guid, bool> Initialized = new Dictionary<Guid, bool>();

        private static EmailTemplateDictionary Templates = new EmailTemplateDictionary();
        private static EmailTemplateDictionary TemplateSubjects = new EmailTemplateDictionary();
        private static EmailTemplateDictionary OtherTemplates = new EmailTemplateDictionary();

        public static void Initialize(Guid? applicationId)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            if (Initialized.ContainsKey(!applicationId.HasValue ? Guid.Empty : applicationId.Value)) return;
            else Initialized[!applicationId.HasValue ? Guid.Empty : applicationId.Value] = true;

            Templates.set_value(applicationId, EmailTemplateType.Master.ToString(), 
                inject_into_master(applicationId, _get_email_template(applicationId, EmailTemplateType.Master.ToString())));

            foreach (string str in Enum.GetNames(typeof(EmailTemplateType)))
            {
                if (str != EmailTemplateType.Master.ToString() && str != EmailTemplateType.None.ToString())
                {
                    Templates.set_value(applicationId, str, inject_into_master(applicationId, _get_email_template(applicationId, str)));
                    TemplateSubjects.set_value(applicationId, str, _get_email_template(applicationId, str + "Subject"));
                }
            }
        }

        private static string _get_email_template(Guid? applicationId, string templateType)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileName = templateType,
                Extension = "txt",
                FolderName = FolderNames.EmailTemplates
            };

            return fi.exists() ? fi.get_text_content() :
                (applicationId.HasValue ? _get_email_template(null, templateType) : string.Empty);
        }

        public static string inject_into_master(Guid? applicationId, string template)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            string retStr = string.Empty;

            if (string.IsNullOrEmpty(template)) return retStr;
            
            if (!Initialized.ContainsKey(!applicationId.HasValue ? Guid.Empty : applicationId.Value) ||
                string.IsNullOrEmpty(Templates.get_value(applicationId, EmailTemplateType.Master.ToString()))) retStr = template;
            else
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["Content"] = template;
                
                retStr = Expressions.replace(Templates.get_value(applicationId, EmailTemplateType.Master.ToString()), 
                    ref dic, Expressions.Patterns.AutoTag);
            }

            return string.IsNullOrEmpty(retStr) && applicationId.HasValue ? inject_into_master(null, template) : retStr;
        }

        public static string get_email_template(Guid? applicationId,
            EmailTemplateType templateType, Dictionary<string, string> dic)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            Initialize(applicationId);
            if (dic == null) dic = new Dictionary<string, string>();

            string retStr = Expressions.replace(Templates.get_value(applicationId, templateType.ToString()), 
                ref dic, Expressions.Patterns.AutoTag);

            return string.IsNullOrEmpty(retStr) && applicationId.HasValue ?
                get_email_template(null, templateType, dic) : retStr;
        }

        public static string get_email_template(Guid? applicationId,
            string templateName, bool intoMaster, Dictionary<string, string> dic)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            Initialize(applicationId);

            templateName = templateName.ToLower();

            if (string.IsNullOrEmpty(OtherTemplates.get_value(applicationId, templateName)))
                OtherTemplates.set_value(applicationId, templateName, _get_email_template(applicationId, templateName));

            string template = OtherTemplates.get_value(applicationId, templateName);

            if (intoMaster) template = inject_into_master(applicationId, template);

            string retStr = Expressions.replace(template, ref dic, Expressions.Patterns.AutoTag);

            return string.IsNullOrEmpty(retStr) && applicationId.HasValue ?
                get_email_template(null, templateName, intoMaster, dic) : retStr;
        }

        public static string get_email_subject_template(Guid? applicationId,
            EmailTemplateType templateType, Dictionary<string, string> dic)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            Initialize(applicationId);
            if (dic == null) dic = new Dictionary<string, string>();

            string retStr = Expressions.replace(TemplateSubjects.get_value(applicationId, templateType.ToString()), 
                ref dic, Expressions.Patterns.AutoTag);

            return string.IsNullOrEmpty(retStr) && applicationId.HasValue ?
                get_email_subject_template(null, templateType, dic) : retStr;
        }
    }
}
