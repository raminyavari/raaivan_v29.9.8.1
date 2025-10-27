using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Management;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using ClosedXML.Excel;
using System.Xml;
using System.Drawing;
using Newtonsoft.Json;
using Microsoft.Owin;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using RaaiVan.Modules.RaaiVanConfig;
using System.Web.Hosting;
using System.Collections;

namespace RaaiVan.Modules.GlobalUtilities
{
    //^:b*[^:b#/]+.*$ --> use this expression to find out number of code lines within the project

    public enum TextDirection
    {
        None,
        RTL,
        LTR
    }

    public static class ConfigUtilities
    {
        public static string get_modules_json(Guid? applicationId)
        {
            return "{" + 
                "\"WorkFlow\":" + RaaiVanConfig.Modules.WorkFlow(applicationId).ToString().ToLower() +
                ",\"WF\":" + RaaiVanConfig.Modules.WorkFlow(applicationId).ToString().ToLower() +
                ",\"FormGenerator\":" + RaaiVanConfig.Modules.FormGenerator(applicationId).ToString().ToLower() +
                ",\"FG\":" + RaaiVanConfig.Modules.FormGenerator(applicationId).ToString().ToLower() + 
                ",\"Notifications\":" + RaaiVanConfig.Modules.Notifications(applicationId).ToString().ToLower() +
                ",\"NTFN\":" + RaaiVanConfig.Modules.Notifications(applicationId).ToString().ToLower() +
                ",\"SocialNetwork\":" + RaaiVanConfig.Modules.SocialNetwork(applicationId).ToString().ToLower() +
                ",\"SH\":" + RaaiVanConfig.Modules.SocialNetwork(applicationId).ToString().ToLower() +
                ",\"KnowledgeAdmin\":" + RaaiVanConfig.Modules.KnowledgeAdmin(applicationId).ToString().ToLower() +
                ",\"Knowledge\":" + RaaiVanConfig.Modules.Knowledge(applicationId).ToString().ToLower() +
                ",\"KW\":" + RaaiVanConfig.Modules.Knowledge(applicationId).ToString().ToLower() +
                ",\"Documents\":" + RaaiVanConfig.Modules.Documents(applicationId).ToString().ToLower() +
                ",\"DCT\":" + RaaiVanConfig.Modules.Documents(applicationId).ToString().ToLower() +
                ",\"PDFViewer\":" + RaaiVanConfig.Modules.PDFViewer(applicationId).ToString().ToLower() +
                ",\"FileContents\":" + RaaiVanSettings.FileContentExtraction.FileContents(applicationId).ToString().ToLower() +
                ",\"QA\":" + RaaiVanConfig.Modules.QA(applicationId).ToString().ToLower() +
                ",\"QAAdmin\":" + RaaiVanConfig.Modules.QAAdmin(applicationId).ToString().ToLower() +
                ",\"Events\":" + RaaiVanConfig.Modules.Events(applicationId).ToString().ToLower() +
                ",\"EVT\":" + RaaiVanConfig.Modules.Events(applicationId).ToString().ToLower() +
                ",\"Messaging\":" + RaaiVanConfig.Modules.Messaging(applicationId).ToString().ToLower() +
                ",\"MSG\":" + RaaiVanConfig.Modules.Messaging(applicationId).ToString().ToLower() +
                ",\"Chat\":" + (RaaiVanSettings.RealTime(applicationId) && RaaiVanConfig.Modules.Chat(applicationId)).ToString().ToLower() +
                ",\"Resume\":" + RaaiVanConfig.Modules.Resume(applicationId).ToString().ToLower() +
                ",\"VisualMap\":" + RaaiVanConfig.Modules.VisualMap(applicationId).ToString().ToLower() +
                ",\"SMSEMailNotifier\":" + RaaiVanConfig.Modules.SMSEMailNotifier(applicationId).ToString().ToLower() +
                ",\"UserSignUp\":" + RaaiVanSettings.UserSignUp(applicationId).ToString().ToLower() +
                ",\"SignUpViaInvitation\":" + RaaiVanSettings.SignUpViaInvitation(applicationId).ToString().ToLower() +
                ",\"RealTime\":" + RaaiVanSettings.RealTime(applicationId).ToString().ToLower() +
                ",\"Explorer\":" + RaaiVanSettings.Explorer(applicationId).ToString().ToLower() +
                ",\"RestAPI\":" + RaaiVanConfig.Modules.RestAPI(applicationId).ToString().ToLower() +
                "}";
        }
    }
    
    public class ListMaker
    {
        protected static void _get_string_items(ref string input, ref List<string> lst, char delimiter)
        {
            //if (!string.IsNullOrEmpty(input)) input = input.Trim(); -> In Base64 strings if the end character is '+' it has been replaced with space character and trimming will damage its data
            if (string.IsNullOrEmpty(input)) return;
            lst = input.Split(delimiter).ToList();
        }

        public static void get_string_items(string input, ref List<string> lst, char delimiter)
        {
            _get_string_items(ref input, ref lst, delimiter);
        }

        public static List<string> get_string_items(string input, char delimiter)
        {
            List<string> lst = new List<string>();
            _get_string_items(ref input, ref lst, delimiter);
            return lst;
        }

        public static void get_long_items(string input, ref List<long> lstLong, char delimiter)
        {
            List<string> lstString = new List<string>();
            _get_string_items(ref input, ref lstString, delimiter);
            lstLong = lstString.ConvertAll(new Converter<string, long>(Convert.ToInt64));
        }

        public static List<long> get_long_items(string input, char delimiter)
        {
            if (!string.IsNullOrEmpty(input)) input = input.Trim();
            List<long> lst = new List<long>();
            if (string.IsNullOrEmpty(input)) return lst;
            get_long_items(input, ref lst, delimiter);
            return lst;
        }
            
        public static void get_guid_items(string input, ref List<Guid> lstGuid, char delimiter)
        {
            List<string> lstString = new List<string>();

            _get_string_items(ref input, ref lstString, delimiter);

            lstGuid = lstString.ConvertAll(new Converter<string, Guid>(Guid.Parse));
        }

        public static List<Guid> get_guid_items(string input, char delimiter)
        {
            if (!string.IsNullOrEmpty(input)) input = input.Trim();
            List<Guid> lst = new List<Guid>();
            if (string.IsNullOrEmpty(input)) return lst;
            get_guid_items(input, ref lst, delimiter);
            return lst;
        }

        public static List<T> get_enum_items<T>(string input, char delimiter, bool unique = true) where T : struct {
            List<T> ret = new List<T>();

            get_string_items(input, delimiter).ForEach(value => {
                try
                {
                    T itm;
                    if (Enum.TryParse<T>(value, out itm)) ret.Add(itm);
                }
                catch { }
            });

            return unique ? ret.Distinct().ToList() : ret;
        }
    }

    public static class Base64
    {
        public static bool encode(string sourceString, ref string returnString)
        {
            if(string.IsNullOrEmpty(sourceString)){
                returnString = string.Empty;
                return true;
            }

            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                byte[] bytes = encoder.GetBytes(sourceString);
                returnString = Convert.ToBase64String(bytes, 0, bytes.Length);

                return true;
            }
            catch
            {
                returnString = sourceString;
                return false;
            }
        }

        public static bool encode(ref string returnString)
        {
            return encode(returnString, ref returnString);
        }

        public static string encode(string returnString)
        {
            encode(returnString, ref returnString);
            return returnString;
        }

        public static bool decode(string sourceString, ref string returnString)
        {
            try
            {
                sourceString = sourceString.Replace(' ', '+');

                UTF8Encoding encoder = new UTF8Encoding();
                Decoder utf8Decode = encoder.GetDecoder();

                byte[] toDecodeByte = Convert.FromBase64String(sourceString);
                int charCount = utf8Decode.GetCharCount(toDecodeByte, 0, toDecodeByte.Length);
                char[] decodeChar = new char[charCount];
                utf8Decode.GetChars(toDecodeByte, 0, toDecodeByte.Length, decodeChar, 0);

                returnString = new String(decodeChar);

                return true;
            }
            catch
            {
                returnString = sourceString;
                return false;
            }
        }

        public static bool decode(ref string returnString)
        {
            return decode(returnString, ref returnString);
        }

        public static string decode(string returnString)
        {
            if (string.IsNullOrEmpty(returnString)) return string.Empty;
            decode(returnString, ref returnString);
            return returnString;
        }
    }

    public static class Expressions
    {
        public enum Patterns
        {
            Tag,
            AutoTag,
            AdditionalID,
            HTMLTag,
            CSSVariable
            //Free = @"(~)\[\[([\w\|\\\/\^\$\u0621-\u064A\u0660-\u0669\u0671-\u06D3\u06F0-\u06F9\s]+)\]\]"
        }

        private static string _get_pattern(Patterns pattern)
        {
            switch (pattern)
            {
                case Patterns.Tag:
                    return @"(@)\[\[([a-zA-Z\d\-_]+):([\w\s\.\-]+):([0-9a-zA-Z\+\/\=]+)(:([0-9a-zA-Z\+\/\=]*))?\]\]";
                case Patterns.AutoTag:
                    return @"(~)\[\[([:\-\w]+)\]\]";
                case Patterns.AdditionalID:
                    return @"^([A-Za-z0-9_\-\/]|(~\[\[(((RND|(NCountS?(PY|GY)?))\d?)|[PG](Year|YearS|Month|Day)|(FormField:[A-Za-z0-9\-]{36})|(AreaID)|(FVersionID)|(PVersionID)|(VersionCounter))\]\]))+$";
                case Patterns.HTMLTag:
                    return @"<.*?>";
                case Patterns.CSSVariable:
                    return @"--[a-zA-Z0-9\-]*:.*;";
                default:
                    return string.Empty;
            }
        }

        public static bool is_match(string input, Patterns pattern)
        {
            string patt = _get_pattern(pattern);
            return (new Regex(_get_pattern(pattern))).IsMatch(input);
        }

        public static MatchCollection get_matches(string input, string pattern)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern) ? 
                (MatchCollection)null : (new Regex(pattern)).Matches(input);
        }

        public static MatchCollection get_matches(string input, Patterns pattern)
        {
            return get_matches(input, _get_pattern(pattern));
        }

        private static List<string> _match_collection_to_string_list(MatchCollection col)
        {
            List<string> ret = new List<string>();
            if (col != null)
                for (int i = 0; i < col.Count; i++) ret.Add(col[i].Value);
            return ret;
        }

        public static List<string> get_matches_string(string input, string pattern)
        {
            MatchCollection col = get_matches(input, pattern);
            return _match_collection_to_string_list(col);
        }

        public static List<string> get_matches_string(string input, Patterns pattern)
        {
            MatchCollection col = get_matches(input, pattern);
            return _match_collection_to_string_list(col);
        }

        public static List<string> get_existing_tags(string input, Patterns pattern)
        {
            MatchCollection matches = get_matches(input, pattern);
            List<string> retList = new List<string>();

            if (matches == null) return retList;

            foreach (Match mth in matches)
            {
                string val = get_value(mth, pattern);
                if (!retList.Any(u => u == val)) retList.Add(val);
            }

            return retList;
        }

        public static string get_value(Match match, Patterns pattern)
        {
            if (match == null || string.IsNullOrEmpty(match.Value) || get_matches(match.Value, pattern) == null) return string.Empty;

            switch (pattern.ToString())
            {
                case "Tag":
                case "AutoTag":
                    return match.Value.Substring(3, match.Value.Length - 5);
                default:
                    return string.Empty;
            }
        }

        public static string replace(string input, string pattern, string replacement = " ")
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return input;
                if (pattern == Patterns.HTMLTag.ToString()) input = HttpUtility.HtmlDecode(input);
                return Regex.Replace(input, pattern, replacement);
            }
            catch { return input; }
        }

        public static string replace(string input, Patterns pattern, string replacement = " ")
        {
            return replace(input, _get_pattern(pattern), replacement);
        }

        public static string replace(string input, ref Dictionary<string, string> dic, Patterns pattern,
            string defaultReplacement = " ")
        {
            MatchCollection matches = get_matches(input, pattern);
            if (matches == null) return string.Empty;

            List<Match> lst = new List<Match>();
            foreach (Match mth in matches) lst.Add(mth);
            lst = lst.OrderByDescending(u => u.Index).ToList();

            string retStr = input;

            foreach (Match mth in lst)
            {
                string val = get_value(mth, pattern);
                val = dic.Any(u => u.Key == val) ? dic[val] : 
                    (defaultReplacement == null ? mth.Value : defaultReplacement);
                retStr = retStr.Substring(0, mth.Index) + val + retStr.Substring(mth.Index + mth.Length);
            }

            return retStr;
        }

        public static List<InlineTag> get_tagged_items(string input, string tagType = null)
        {
            List<InlineTag> tags = new List<InlineTag>();

            if (string.IsNullOrEmpty(input)) return tags;
            
            List<string> items = get_existing_tags(input, Patterns.Tag);

            foreach (string itm in items)
            {
                string[] t = itm.Split(':');
                InlineTag tg = new InlineTag() { Type = t[1], Value = Base64.decode(t[2]), Info = t.Length > 3 ? t[3] : null };
                tg.set_id(t[0]);
                tags.Add(tg);
            }

            return string.IsNullOrEmpty(tagType) ? tags : tags.Where(u => u.Type.ToLower() == tagType.ToLower()).ToList();
        }

        public static string get_markup(string id, string type, string value, string info = null)
        {
            return "@[[" + id + ":" + type + ":" + (Base64.encode(value)) + 
                (!string.IsNullOrEmpty(info) ? ":" + info : "") + "]]";
        }
    }

    public class ObjectsComparer<T> : IEqualityComparer<T>
    {
        public ObjectsComparer(Func<T, T, bool> cmp)
        {
            this.cmp = cmp;
        }
        public bool Equals(T x, T y)
        {
            return cmp(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public Func<T, T, bool> cmp { get; set; }
    }

    public static class PublicConsts
    {
        public static string FavIcon = "~/Images/" + RaaiVanSettings.FavIconName + ".ico";

        public static string LoginPage = "~/login";
        public static string ApplicationsPage = "~/teams";
        public static string HomePage = "~/home";
        public static string ProfilePage = "~/user";
        public static string ChangePasswordPage = "~/changepassword";
        public static string NoAccessPage = "~/accessdenied";
        public static string NodePage = "~/node";
        public static string PostPage = "~/posts";
        public static string RSSPage = "~/rss";
        public static string ServiceUnavailablePage = "~/Default.aspx";
        public static string SearchPage = "~/dosearch";
        public static string ReportsPage = "~/reportspanel";
        public static string FileDownload = "~/download";
        public static string VisualGraphPage = "~/graph";
        public static string ExpolorerPage = "~/explorer";
        public static string QuestionsPage = "~/questions";
        public static string QuestionViewPage = "~/question";
        public static string UserSearchPage = "~/usersearch";
        public static string SystemSettingsPage = "~/systemsettings";
        public static string NetworkPage = "~/network";
        public static string MessagesPage = "~/messages";
        public static string DocumentsBrowserPage = "~/documentsbrowser";
        public static string NoPDFPage = "~/images/no_pdf_page.png";
        public static string TempDirectory = "~/app_data/temp";
        public static string MagickCacheDirectory = "~/app_data/magick_cache";
        public static string EncryptedFileNamePrefix = "e_";
        public static string LicenseFilePath = "~/raaivan.license";
        public static string ThemeCSS = "~/CSS/theme.css";
        public static string GlobalCSS = "~/CSS/global.css";
        public static string FontFaceIranSans = "~/CSS/font-face/iran-sans.css";
        public static string FontFaceRoboto = "~/CSS/font-face/roboto.css";
        public static string FontIranSansWindows = "~/Fonts/IRANSans/windows/IRANSans.ttf";
        public static string CSSLTR = "~/CSS/LTR.css";
        public static string CSSRTL = "~/CSS/RTL.css";
        public static string LanguageFile = "~/Script/Lang/[lang].js";
        public static string LanguageHelpFile = "~/Script/Lang/Help/[lang].js";
        public static string JQuerySignalR = "~/Script/jQuery/jquery.signalr.js";
        public static string RaaiVanHub = "~/Script/RealTime/RaaiVanHub.js";

        public static string NotAuthenticatedResponse = "{\"NotAuthenticated\":" + true.ToString().ToLower() + "}";
        public static string NullTenantResponse = "{\"NoApplicationFound\":" + true.ToString().ToLower() + "}";
        public static string BadRequestResponse = "{\"Message\":\"" + "BadRequest" + "\"}";
        public static string USBTokenNotFoundResponse = "{\"USBTokenNotFound\":" + true.ToString().ToLower() + "}";
        public static string InvalidTicketResponse = "{\"Result\":\"nok\"" +
            ",\"Message\":\"TicketIsNotValid\"" +
            ",\"InvalidTicket\":" + true.ToString().ToLower() + 
            "}";

        public static string DefaultProfileImageURL = "../../Images/unknown.jpg";
        public static string DefaultCoverPhotoURL = "../../Images/DefaultCoverPhoto.jpg";

        public static string get_complete_url(Guid? applicationId, string page)
        {
            return page.Replace("~", RaaiVanSettings.RaaiVanURL(applicationId));
        }

        public static string get_client_url(string page)
        {
            return page.Replace("~", "../..");
        }

        public static int LicenseLengthInBytes = 32;
        public static int EncryptionKeyLengthInBytes = 32;

        public static string LanguageSessionVariableName = "rv_lang";

        public static string ApplicationIDSessionVariableName = "rv_active_app";
    }

    public static class PublicMethods
    {
        private static string _SYSID;

        public static string get_sys_id()
        {
            if (RaaiVanSettings.USBToken) return "USB Token Is Not Connected To Device";

            if (!string.IsNullOrEmpty(_SYSID)) return _SYSID;

            //Get CpuID
            string temp = String.Empty;

            string cpuIds = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            List<string> _cIds = new List<string>();
            foreach (ManagementObject mo in moc) _cIds.Add(mo.Properties["ProcessorId"].Value.ToString());
            _cIds = _cIds.OrderBy(u => u).ToList();
            foreach (string str in _cIds) cpuIds += str;
            //end of Get CpuID


            //Get MacAddress
            string macAddresses = "";
            ////List<string> _nics = new List<string>();
            ////foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            ////    if (nic.OperationalStatus == OperationalStatus.Up /* && nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 */) 
            ////        _nics.Add(nic.GetPhysicalAddress().ToString());
            ////_nics = _nics.OrderBy(u => u).ToList();
            ////foreach (string str in _nics) macAddresses += str;
            //end of Get MacAddress


            //Get SysID (MD5 Hash Code)
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(cpuIds + macAddresses);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
            //end of Get SysID

            _SYSID = sb.ToString();

            return _SYSID;
        }

        public static bool check_sys_id()
        {
            return RaaiVanSettings.USBToken ? USBToken.connect() && validate_license_key(USBToken.read_license()) :
                string.IsNullOrEmpty(GlobalSettings.RefSysID) || (get_sys_id() == GlobalSettings.RefSysID);
        }

        private static string _SYSTEMVERSION;

        public static string SystemVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_SYSTEMVERSION)) _SYSTEMVERSION = GlobalController.get_system_version();
                return _SYSTEMVERSION;
            }
        }

        private static string KeyCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static int _key_calculate_third(int first, int second) {
            return (first + (second * 2) + 2) % KeyCharacters.Length;
        }
        private static int _key_calculate_fourth(int first, int second, int third)
        {
            return ((first * 3) + (second * 2) + third + 3) % KeyCharacters.Length;
        }

        public static string generate_license_key() {
            if (PublicConsts.LicenseLengthInBytes % 4 != 0) return string.Empty;
            
            string key = "";

            for (int i = 0, chunks = PublicConsts.LicenseLengthInBytes / 4; i < chunks; ++i) {
                int first = PublicMethods.get_random_number(0, KeyCharacters.Length - 1);
                int second = PublicMethods.get_random_number(0, KeyCharacters.Length - 1);
                int third = _key_calculate_third(first, second);
                int fourth = _key_calculate_fourth(first, second, third);

                key += KeyCharacters[first].ToString() + KeyCharacters[second].ToString() + 
                    KeyCharacters[third].ToString() + KeyCharacters[fourth].ToString();
            }

            return key;
        }

        public static bool validate_license_key(string key) {
            if (string.IsNullOrEmpty(key) || 
                key.Length != PublicConsts.LicenseLengthInBytes || key.Length % 4 != 0) return false;

            for (int i = 0, chunks = PublicConsts.LicenseLengthInBytes / 4; i < chunks; ++i)
            {
                int first = KeyCharacters.IndexOf(key[i * 4].ToString());
                int second = KeyCharacters.IndexOf(key[(i * 4) + 1].ToString());
                int third = KeyCharacters.IndexOf(key[(i * 4) + 2].ToString());
                int fourth = KeyCharacters.IndexOf(key[(i * 4) + 3].ToString());

                if (first < 0 || second < 0 || third < 0 || fourth < 0 ||
                    _key_calculate_third(first, second) != third ||
                    _key_calculate_fourth(first, second, third) != fourth) return false;
            }

            return true;
        }

        public static bool create_license_file(string username, string password)
        {
            try
            {
                byte[] data = DocumentUtilities.encrypt_bytes_aes_native(Encoding.ASCII.GetBytes(
                    Base64.encode("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}")));
                string path = map_path(PublicConsts.LicenseFilePath);
                if (File.Exists(path)) File.Delete(path);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                    fs.Dispose();
                }
                return true;
            }
            catch { return false; }
        }

        public static bool prepare_usb_token(string username, string password, byte[] encryptionKey) {
            return create_license_file(username, password) &&
                USBToken.write_license(generate_license_key()) &&
                USBToken.write_encryption_key(encryptionKey);
        }

        public static string get_client_ip(ref HttpRequest req)
        {
            try
            {
                return (req.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? req.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
            }
            catch { return string.Empty; }
        }

        public static string get_client_ip(HttpContext con)
        {
            HttpRequest req = con.Request;
            return get_client_ip(ref req);
        }

        public static string get_client_host_name(ref HttpRequest req)
        {
            try { return req.UserHostName.Trim(); }
            catch { return string.Empty; }
        }

        public static string get_client_host_name(HttpContext con)
        {
            HttpRequest req = con.Request;
            return get_client_host_name(ref req);
        }

        private static bool _BackingUp = false;

        private static void _backup_database(object appId)
        {
            try
            {
                Guid applicationId = (Guid)appId;

                string dbName = MSSQLConnector.ConnectionString.Split(';').ToList().Select(u => u.Trim().ToLower()).Where(
                    v => v.IndexOf("database") == 0).FirstOrDefault().Split('=')[1].Trim();

                string persianNow = GenericDate.get_local_date(DateTime.Now);

                string backupFileName = dbName + "-Full Database Backup-" + persianNow.Replace('/', '-') + ".bak";

                string backupFolder = PublicMethods.map_path("~/App_Data/DB_Backup");
                if (!Directory.Exists(backupFolder)) Directory.CreateDirectory(backupFolder);
                string backupFilePath = backupFolder + "/" + backupFileName;

                if (!File.Exists(backupFilePath.Replace("/", "\\")))
                {
                    string cmd = "BACKUP DATABASE [" + dbName + "] TO DISK = N'" + backupFilePath + "' " +
                        "WITH COMPRESSION, NOFORMAT, INIT, NAME = N'" + backupFileName + "', SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                    System.Data.SqlClient.SqlConnection Sqlcon = new System.Data.SqlClient.SqlConnection(MSSQLConnector.ConnectionString);
                    System.Data.SqlClient.SqlCommand Sqlcom = new System.Data.SqlClient.SqlCommand(cmd, Sqlcon);
                    try
                    {
                        Sqlcon.Open();
                        Sqlcom.ExecuteNonQuery();
                    }
                    catch { }
                    finally { Sqlcon.Close(); }

                    if (RaaiVanSettings.RemoveOldDatabaseBackups && File.Exists(backupFilePath))
                    {
                        backupFilePath = backupFilePath.Replace("/", "\\").ToLower();

                        foreach (string bcFile in Directory.GetFiles(backupFolder))
                            if (bcFile.ToLower() != backupFilePath) File.Delete(bcFile);
                    }
                }
            }
            catch (Exception ex) { }

            _BackingUp = false;
        }

        public static void backup_database(Guid applicationId)
        {
            if (_BackingUp) return;
            _BackingUp = true;

            new Thread(_backup_database).Start(applicationId);
        }

        public static void start_db_backup(object rvThread)
        {
            RVJob trd = (RVJob)rvThread;

            if (!RaaiVanSettings.DailyDatabaseBackup) return;
            
            if (!trd.StartTime.HasValue) trd.StartTime = new DateTime(2000, 1, 1, 0, 0, 0);
            if (!trd.EndTime.HasValue) trd.EndTime = new DateTime(2000, 1, 1, 23, 59, 59);

            while (true)
            {
                if (!trd.Interval.HasValue) trd.Interval = 3600000; //3600000 Miliseconds Equals to 1 Hour
                else Thread.Sleep(trd.Interval.Value);

                if (!trd.check_time()) continue;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();

                backup_database(trd.TenantID.Value);

                trd.LastActivityDate = DateTime.Now;

                sw.Stop();
                trd.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }

        public static Application get_current_application()
        {
            object l = null;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                l = HttpContext.Current.Session[PublicConsts.ApplicationIDSessionVariableName];
            return l != null && l.GetType() == typeof(Application) ? (Application)l : null;
        }

        public static void set_current_application(Application application)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[PublicConsts.ApplicationIDSessionVariableName] = application;
        }

        public static RVLang get_current_language(Guid? applicationId = null)
        {
            object l = null;

            //get lang from cookie
            if (HttpContext.Current != null && HttpContext.Current.Request != null &&
                HttpContext.Current.Request.Cookies != null)
            {
                RVLang lng = RVLang.none;
                HttpCookie ck = HttpContext.Current.Request.Cookies.Get(PublicConsts.LanguageSessionVariableName);

                if (ck != null && Enum.TryParse<RVLang>(ck.Value, out lng) && lng != RVLang.none) l = lng;
                else l = null;
            }

            //get lang from session
            if (l == null && HttpContext.Current != null && HttpContext.Current.Session != null)
                l = HttpContext.Current.Session[PublicConsts.LanguageSessionVariableName];

            return l != null && l.GetType() == typeof(RVLang) ? (RVLang)l : RaaiVanSettings.DefaultLang(applicationId);
        }

        public static void set_current_language(RVLang lang)
        {
            if (lang == RVLang.none) return;

            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[PublicConsts.LanguageSessionVariableName] = lang;
        }

        private static SortedList<Guid, Dictionary<Guid, bool>> _SystemAdmins = 
            new SortedList<Guid, Dictionary<Guid, bool>>();

        public static bool is_dev() {
            return HostingEnvironment.IsDevelopmentEnvironment || RaaiVanSettings.IsDevEnvironment;
        }

        public static string get_environment_variable(string variable, string defaultValue = "")
        {
            string ret = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine) ??
                (Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User) ?? defaultValue);

            return string.IsNullOrEmpty(ret) ? ret : 
                (ret.ToLower().EndsWith("_base64") ? Base64.decode(ret.Substring(0, ret.Length - 7)) : ret);
        }

        public static bool is_system_admin(Guid? applicationId, Guid userId, bool ignoreAuthentication = false)
        {
            if (!applicationId.HasValue) applicationId = Guid.Empty;

            if (userId == Guid.Empty || (!ignoreAuthentication && (
                HttpContext.Current == null || !HttpContext.Current.User.Identity.IsAuthenticated))) return false;

            if (_SystemAdmins == null) _SystemAdmins = new SortedList<Guid, Dictionary<Guid, bool>>();
            if (!_SystemAdmins.ContainsKey(applicationId.Value)) _SystemAdmins.Add(applicationId.Value, new Dictionary<Guid, bool>());

            if (!_SystemAdmins[applicationId.Value].ContainsKey(userId)) _SystemAdmins[applicationId.Value][userId] = 
                    GlobalController.is_system_admin(applicationId == Guid.Empty ? null : applicationId, userId);
            return _SystemAdmins[applicationId.Value][userId];
        }

        public static Guid get_current_user_id(int reverseCounter = 12)
        {
            try
            {
                return Guid.Parse(HttpContext.Current.User.Identity.Name);
            }
            catch
            { return Guid.Empty; }
        }
        
        public static long miliseconds()
        {
            return (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds + 0.5);
        }

        public static DateTime? persian_to_gregorian_date(int year, int month, int day, int? hour, int? minute, int? second)
        {
            try
            {
                if (year < 100) year += year > 20 ? 1300 : 1400;

                DateTime? val = (new PersianCalendar()).ToDateTime(year, month, day, (hour.HasValue ? hour.Value : 0),
                    (minute.HasValue ? minute.Value : 0), (second.HasValue ? second.Value : 0), 0);

                return val.HasValue && (val.Value < DateTime.MinValue || val.Value > DateTime.MaxValue) ? null : val;
            }
            catch { return null; }
        }

        public static int n_days_ago(DateTime date) {
            DateTime now = DateTime.Now;
            DateTime nowOrigin = new DateTime(now.Year, now.Month, now.Day);
            DateTime dateOrigin = new DateTime(date.Year, date.Month, date.Day);

            return (int)(nowOrigin - dateOrigin).TotalDays;
        }

        public static int? parse_int(string input, int? defaultValue = null)
        {
            if (string.IsNullOrEmpty(input)) return defaultValue;
            int retVal = 0;
            if (!int.TryParse(input, out retVal)) return defaultValue;
            return retVal;
        }

        public static long? parse_long(string input, long? defaultValue = null)
        {
            if (string.IsNullOrEmpty(input)) return defaultValue;
            long retVal = 0;
            if (!long.TryParse(input, out retVal)) return defaultValue;
            return retVal;
        }

        public static double? parse_double(string input, double? defaultValue = null)
        {
            if (string.IsNullOrEmpty(input)) return defaultValue;
            double retVal = 0;
            if (!double.TryParse(input, out retVal)) return defaultValue;
            return retVal;
        }

        public static bool? parse_bool(string input, bool? defaultValue = null)
        {
            int intValue = 0;

            if (string.IsNullOrEmpty(input)) return defaultValue;
            else if (input.ToLower() == "true" || (int.TryParse(input, out intValue) && intValue > 0)) return true;
            else if (input.ToLower() == "false" || input == "0") return false;
            else return defaultValue;
        }

        public static DateTime? parse_date(string input, int days2Add = 0)
        {
            if(!string.IsNullOrEmpty(input)) input = input.Trim();
            if (string.IsNullOrEmpty(input)) return null;

            string[] strDate = (string.IsNullOrEmpty(input) ? string.Empty : input).Split('-');
            if (!string.IsNullOrEmpty(input) && strDate.Length <= 2) strDate = input.Split('/');

            if (strDate.Length > 2)
            {
                try
                {
                    int year = int.Parse(strDate[0]), month = int.Parse(strDate[1]), day = int.Parse(strDate[2]);

                    return (year < 1600 ? PublicMethods.persian_to_gregorian_date(year, month, day, 0, 0, 0).Value :
                        new DateTime(year, month, day)).AddDays(days2Add);
                }
                catch { }
            }

            try { return DateTime.Parse(input).AddDays(days2Add); }
            catch { return null; }
        }

        public static string parse_string(object input, bool decode = true, string defaultValue = "")
        {
            if (input == null || string.IsNullOrEmpty(input.ToString())) return defaultValue;
            return decode ? Base64.decode(input.ToString()) : input.ToString();
        }

        public static Guid? parse_guid(string input, Guid? alternatvieValue = null)
        {
            if (string.IsNullOrEmpty(input)) return alternatvieValue;
            Guid retVal = Guid.Empty;
            if (!Guid.TryParse(input, out retVal)) return alternatvieValue;
            return retVal;
        }

        public static Guid guid_xor(Guid first, Guid second) {
            int BYTECOUNT = 16;

            byte[] destByte = new byte[BYTECOUNT];
            byte[] firstByte = first.ToByteArray();
            byte[] secondByte = second.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
                destByte[i] = (byte)(firstByte[i] ^ secondByte[i]);

            return new Guid(destByte);
        }

        private static T parse_enum<T>(string input, T defaultValue, ref bool error) where T : struct
        {
            try
            {
                T itm;
                if (Enum.TryParse<T>(value: input, ignoreCase: true, result: out itm))
                {
                    error = false;
                    return itm;
                }
                else
                {
                    error = true;
                    return defaultValue;
                }
            }
            catch
            {
                error = true;
                return defaultValue;
            }
        }

        public static T parse_enum<T>(string input, T defaultValue) where T : struct
        {
            bool error = false;
            return parse_enum<T>(input, defaultValue, ref error);
        }

        public static T? parse_enum<T>(string input, T? defaultValue = null) where T : struct
        {
            bool error = false;
            T result = parse_enum<T>(input, new T(), ref error);
            return error ? defaultValue : (T?)result;
        }

        public static string get_dic_value(Dictionary<string, object> dic, string key, string defaultValue = null) {
            if (dic == null || string.IsNullOrEmpty(key) || !dic.ContainsKey(key) || dic[key] == null) return defaultValue;
            else return dic[key].ToString();
        }

        public static T get_dic_value<T>(Dictionary<string, object> dic, string key)
        {
            if (dic == null || string.IsNullOrEmpty(key) || !dic.ContainsKey(key) ||
                dic[key] == null || !(dic[key] is T)) return default(T);
            else return (T)dic[key];
        }

        public static T get_dic_value<T>(Dictionary<string, object> dic, string key, T defaultValue) {
            T ret = get_dic_value<T>(dic, key);
            return ret == null ? defaultValue : ret;
        }

        private static Random _RND = new Random((int)DateTime.Now.Ticks);

        public static int get_random_number(int min, int max)
        {
            return _RND.Next(min, max + 1);
        }

        public static int get_random_number(int length = 5)
        {
            return get_random_number((int)Math.Pow(10, (double)length - 1), (int)Math.Pow(10, (double)length) - 1);
        }

        public static string random_string(int length = 20)
        {
            string refStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            string value = string.Empty;
            for (int i = 0; i < length; ++i)
                value += refStr[_RND.Next(0, refStr.Length - 1)];

            return value;
        }

        public static string sha1(string input)
        {
            return Convert.ToBase64String(System.Security.Cryptography.HashAlgorithm.Create("SHA1")
                .ComputeHash(Encoding.Unicode.GetBytes(input)));
        }

        public static string sha256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));

                return builder.ToString();
            }
        }

        public static string fit_number_to_size(int number, int size)
        {
            string retStr = number.ToString();
            while (retStr.Length < size) retStr = "0" + retStr;
            return retStr;
        }

        public static string verify_string(string str, bool removeHtmlTags = true)
        {
            if (removeHtmlTags && !string.IsNullOrEmpty(str)) str = Expressions.replace(str, Expressions.Patterns.HTMLTag, " ");
            return convert_numbers_from_local(string.IsNullOrEmpty(str) ? str : str.Replace('ي', 'ی').Replace('ك', 'ک'));
        }

        public static bool is_all_upper(string input)
        {
            return string.IsNullOrEmpty(input) || 
                !input.ToCharArray().Any(c => Char.IsLetter(c) && !Char.IsUpper(c));
        }

        //Arabic - Range: 0600–06FF, Arabic Supplement - Range: 0750–077F, Arabic Presentation Forms-A - Range: FB50–FDFF, Arabic Presentation Forms-B - Range: FE70–FEFF
        private static string RTLCharacters = "\u0600-\u06FF\u0750-\u077F\uFB50-\uFDFF\uFE70-\uFEFF";

        public static TextDirection text_direction(string text)
        {
            //ASCII Punctuation - Range: 0000-0020, General Punctuation - Range: 2000-200D
            string controlChars = "\u0000-\u0020\u2000-\u200D*\"'.0-9()$%^&@!#,=?/\\+-:<>|;";

            Regex reRTL = new Regex("^[" + controlChars + "]*[" + RTLCharacters + "]");
            Regex reControl = new Regex("^[" + controlChars + "]*$");

            return reRTL.IsMatch(text) ? TextDirection.RTL : 
                (reControl.IsMatch(text) ? TextDirection.None : TextDirection.LTR);
        }

        public static bool has_rtl_characters(string text)
        {
            return new Regex("[" + RTLCharacters + "]").IsMatch(text);
        }

        public static int rtl_characters_count(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : new Regex("[" + RTLCharacters + "]").Matches(text).Count;
        }

        public static int dangerous_characters_count(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : new Regex("[<>?/\\~!@#$%^&*+=\\-]").Matches(text).Count;
        }

        public static bool is_secure_title(string text)
        {
            return string.IsNullOrEmpty(text) || ((float)dangerous_characters_count(text) / (float)text.Length <= 0.3);
        }

        public static string get_text_beginning(Guid applicationId, string text, int? length = null) {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            string ret = PublicMethods.markup2plaintext(applicationId, text, false);
            return !length.HasValue || length <= 0 || ret.Length <= length ? ret : ret.Substring(0, length.Value) + "...";
        }

        public static bool is_rtl_language(Guid? applicationId, RVLang lang)
        {
            return lang == RVLang.none ? is_rtl_language(applicationId, RaaiVanSettings.DefaultLang(applicationId)) :
                lang == RVLang.fa || lang == RVLang.ar;
        }

        public static bool has_persian_numbers(RVLang lang) {
            return lang == RVLang.fa || lang == RVLang.ar;
        }

        private static Dictionary<string, string> NUMBERS_DIC = new Dictionary<string, string>() {
            {"0", "۰" },{"1", "۱" },{"2", "۲" },{"3", "۳" },{"4", "۴" },
            {"5", "۵" },{"6", "۶" },{"7", "۷" },{"8", "۸" },{"9", "۹" }
        };

        public static string convert_numbers_to_local(string input)
        {
            if (string.IsNullOrEmpty(input) || !has_persian_numbers(get_current_language())) return input;
            foreach (string key in NUMBERS_DIC.Keys)
                input = Regex.Replace(input, key, NUMBERS_DIC[key]);
            return input;
        }

        public static string convert_numbers_from_local(string input)
        {
            if (string.IsNullOrEmpty(input) || !has_persian_numbers(get_current_language())) return input;
            foreach (string key in NUMBERS_DIC.Keys)
                input = Regex.Replace(input, NUMBERS_DIC[key], key);
            return input;
        }

        public static string get_theme_color(string theme)
        {
            int _ind = theme.LastIndexOf('_');
            return _ind < 0 ? string.Empty : theme.Substring(_ind + 1);
        }

        private static string _Themes;
        public static string get_themes()
        {
            if (!string.IsNullOrEmpty(_Themes)) return _Themes;

            List<FileInfo> themes = (new DirectoryInfo(PublicMethods.map_path("~/CSS/Themes"))).GetFiles().ToList();
            string thms = "[{\"Name\":\"Default\",\"Color\":\"\"}";
            foreach (FileInfo thm in themes)
            {
                string name = thm.Name.Substring(0, thm.Name.LastIndexOf('.'));
                if (name.ToLower() == "default") continue;
                string color = get_theme_color(name);

                thms += "," + "{\"Name\":\"" + name + "\",\"Color\":\"" + color + "\"}";
            }
            thms += "]";

            _Themes = thms;

            return _Themes;
        }

        public static void generate_themes()
        {
            string template = File.ReadAllText(PublicMethods.map_path("~/CSS/theme-template.css"));

            Directory.GetFiles(PublicMethods.map_path("~/CSS/Themes")).ToList().ForEach(path =>
            {
                string newTheme = template;

                string theme = File.ReadAllText(path);
                theme = theme.Substring(theme.IndexOf("{"));
                theme = theme.Substring(0, theme.IndexOf("}") + 1);

                Dictionary<string, object> colors = PublicMethods.fromJSON(theme);

                colors.Keys.ToList().ForEach(clr => {
                    newTheme = newTheme.Replace("[" + clr + "]", (string)colors[clr]);
                });

                using (StreamWriter file = new StreamWriter(path, false))
                    file.Write(newTheme);
            });
        }

        public static string shuffle_text(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty :
                string.Join(". ", text.Split('.').Select(u => new KeyValuePair<string, int>(u, PublicMethods.get_random_number()))
                .OrderBy(x => x.Value).Select(z => z.Key).ToArray());
        }

        public static string diff(string newStr, string oldStr)
        {
            int newLen = string.IsNullOrEmpty(newStr) ? 0 : newStr.Length;
            int oldLen = string.IsNullOrEmpty(oldStr) ? 0 : oldStr.Length;
            string dif = string.Empty;
            
            for (int i = 0, pos = 0; i < newLen; ++i) {
                if (pos < oldLen && newStr[i] == oldStr[pos]) ++pos;
                else dif += newStr[i];
            }

            return dif;
        }

        public static string markup2plaintext(Guid applicationId, string markupText, bool htmlTags = false)
        {
            //if 'htmlTags' is set to false, it means that all tags in the text will be replace with their observable value
            //for example, a node tag will replace with the node name that exists in the tag data

            if (string.IsNullOrEmpty(markupText)) return string.Empty;

            Dictionary<string, string> dic = new Dictionary<string, string>();

            MatchCollection matches = Expressions.get_matches(markupText, Expressions.Patterns.Tag);

            List<string> keys = new List<string>();
            foreach (Match mch in matches)
            {
                if (dic.ContainsKey(mch.Value)) continue;
                keys.Add(mch.Value);

                string[] tagVals = Expressions.get_value(mch, Expressions.Patterns.Tag).Split(':');
                string decodedValue = Base64.decode(tagVals[2]);

                if (!htmlTags) dic[mch.Value] = decodedValue;
                else
                {
                    switch (tagVals[1].ToLower())
                    {
                        case "knowledge":
                        case "node":
                            dic[mch.Value] = "<a href='" + 
                                PublicConsts.get_complete_url(applicationId, PublicConsts.NodePage) + 
                                "/" + tagVals[0] + "'>" + decodedValue + "</a>";
                            break;
                        case "user":
                            dic[mch.Value] = "<a href='" + 
                                PublicConsts.get_complete_url(applicationId, PublicConsts.ProfilePage) + 
                                "/" + tagVals[0] + "'>" + decodedValue + "</a>";
                            break;
                        case "file":
                            Dictionary<string, string> info = tagVals.Length < 4 ?
                                new Dictionary<string, string>() : PublicMethods.json2dictionary(Base64.decode(tagVals[3]));
                            Guid fileId = Guid.Empty;
                            if (!Guid.TryParse(tagVals[0], out fileId)) break;

                            string downloadUrl = DocumentUtilities.get_download_url(applicationId, fileId);

                            string ext = (info.ContainsKey("Extension") ? info["Extension"] : string.Empty).ToLower();

                            if (ext == "jpg" || ext == "gif" || ext == "png") //is image
                            {
                                int width = 0, heigth = 0;

                                if (!info.ContainsKey("W") || !int.TryParse(info["W"], out width)) width = 0;
                                if (!info.ContainsKey("H") || !int.TryParse(info["H"], out heigth)) heigth = 0;

                                dic[mch.Value] = "<img src='" + downloadUrl +
                                    "' style='" + (width > 0 ? "width:" + width.ToString() + "px;" : string.Empty) +
                                    (heigth > 0 ? "height:" + heigth.ToString() + "px;" : string.Empty) + "' />";
                            }
                            else
                                dic[mch.Value] = "<a href='" + downloadUrl + "'>" + decodedValue + "</a>";

                            break;
                        default:
                            dic[mch.Value] = decodedValue;
                            break;
                    }
                }
            }

            foreach (string key in keys)
                markupText = markupText.Replace(key, dic[key]);

            return markupText;
        }

        public static string image_to_base64(Image image, System.Drawing.Imaging.ImageFormat format) {
            if (image == null) return string.Empty;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch { return null; }
        }

        public static Image image_from_byte_array(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            try
            {
                return Image.FromStream(new MemoryStream(bytes));
            }
            catch { return null; }
        }

        public static Image image_from_base64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            return image_from_byte_array(Convert.FromBase64String(base64String));
        }

        public static byte[] image_to_byte_array(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            if (image == null) return null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    return ms.ToArray();
                }
            }
            catch { return null; }
        }

        public static string toJSON(dynamic json, string defaultValue = "")
        {
            if (json == null) return defaultValue;
            try { return new JavaScriptSerializer().Serialize(json); }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                return defaultValue;
            }
        }

        public static string toJSON_typed<T>(T data)
        {
            if (data == null || !data.GetType().IsSerializable) return null;

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new RVJsonContractResolver() };
                return JsonConvert.SerializeObject(data, settings);
            }
            catch { return null; }
        }

        public static Dictionary<string, object> fromJSON(string json)
        {
            if (string.IsNullOrEmpty(json)) return new Dictionary<string, object>();
            try { return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json); }
            catch { return new Dictionary<string, object>(); }
        }

        public static T fromJSON_typed<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return default(T);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new RVJsonContractResolver() };
                return JsonConvert.DeserializeObject<T>(data, settings);
            }
            catch { return default(T); }
        }

        public static Dictionary<string, string> json2dictionary(string json)
        {
            if (string.IsNullOrEmpty(json)) return new Dictionary<string, string>();
            try { return new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json); }
            catch { return new Dictionary<string, string>(); }
        }

        public static XmlDocument json2xml(string json, string root = null, string errorJson = null)
        {
            try
            {
                return string.IsNullOrEmpty(root) ? (XmlDocument)JsonConvert.DeserializeXmlNode(json) :
                    (XmlDocument)JsonConvert.DeserializeXmlNode(json, root);
            }
            catch(Exception ex) {
                if (!string.IsNullOrEmpty(errorJson))
                {
                    string strEx = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
                    return json2xml(errorJson.Replace("[Exception]", Base64.encode(strEx)));
                }
                else return null;
            }
        }

        public static void shorten_xml_texts(XmlNode node, int maxLength, string postFix = "...")
        {
            if (node == null) return;

            int computedMaxLength = maxLength;

            if (!string.IsNullOrEmpty(postFix)) computedMaxLength -= postFix.Length;
            if (computedMaxLength <= 0) computedMaxLength = 1;
            
            if (!node.HasChildNodes)
            {
                string innerText = node.InnerText;

                if (!string.IsNullOrEmpty(innerText))
                {
                    innerText = innerText.Trim();

                    int lnt = innerText.Length;

                    node.InnerText = innerText.Substring(0, Math.Min(lnt, computedMaxLength)) +
                        (string.IsNullOrEmpty(postFix) || lnt <= computedMaxLength ? string.Empty : postFix);
                }
            }
            else {
                foreach (XmlNode n in node.ChildNodes)
                    shorten_xml_texts(n, maxLength, postFix);
            }
        }

        public static void remove_xml_attributes(XmlNode node)
        {
            if (node == null) return;

            if (node.Attributes != null) node.Attributes.RemoveAll();

            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                    remove_xml_attributes(n);
            }
        }

        public static string xml2json(XmlDocument xml, bool omitRoot, ref string errorMessage)
        {
            try
            {
                return JsonConvert.SerializeXmlNode(xml.DocumentElement, Newtonsoft.Json.Formatting.None, omitRoot);
            }
            catch (Exception ex)
            {
                errorMessage = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
                return null;
            }
        }

        public static void set_rv_global(Page pg, Dictionary<string, object> data)
        {
            if (data == null) data = new Dictionary<string, object>();

            data.Add("LogoURL", RaaiVanSettings.LogoURL);
            data.Add("LogoMiniURL", RaaiVanSettings.LogoMiniURL);
            data.Add("SAASBasedMultiTenancy", RaaiVanSettings.SAASBasedMultiTenancy);

            pg.Header.Controls.Add(new LiteralControl("<script type='text/javascript'>" + 
                "window.RVGlobal = " + PublicMethods.toJSON(data) + ";</script>"));
        }

        public static bool is_email(string emailAddress)
        {
            try
            {
                new System.Net.Mail.MailAddress(emailAddress);
                return true;
            }
            catch { return false; }
        }

        public static bool send_email(Guid? applicationId, string email, string emailSubject, string body,
            string senderEmail, string senderPassword, string senderName, string smtpAddress, int? smtpPort, bool? useSSL)
        {
            try
            {
                if (string.IsNullOrEmpty(emailSubject)) emailSubject = RaaiVanSettings.SystemEmail.EmailSubject(applicationId);
                if (string.IsNullOrEmpty(senderEmail)) senderEmail = RaaiVanSettings.SystemEmail.Address(applicationId);
                if (string.IsNullOrEmpty(senderPassword)) senderPassword = RaaiVanSettings.SystemEmail.Password(applicationId);
                if (string.IsNullOrEmpty(senderName)) senderName = RaaiVanSettings.SystemEmail.DisplayName(applicationId);
                if (string.IsNullOrEmpty(smtpAddress)) smtpAddress = RaaiVanSettings.SystemEmail.SMTPAddress(applicationId);
                if (!smtpPort.HasValue || smtpPort.Value <= 0) smtpPort = RaaiVanSettings.SystemEmail.SMTPPort(applicationId);
                if (!useSSL.HasValue) useSSL = RaaiVanSettings.SystemEmail.UseSSL(applicationId);

                if (!is_email(email) || !is_email(senderEmail)) return false;

                using (System.Net.Mail.MailMessage mailObj = new System.Net.Mail.MailMessage())
                {
                    mailObj.From = new System.Net.Mail.MailAddress(senderEmail, senderName);
                    mailObj.To.Add(email);
                    mailObj.Subject = emailSubject;
                    mailObj.Body = body;
                    mailObj.IsBodyHtml = true;
                    mailObj.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;

                    using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpAddress, smtpPort.Value))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtp.EnableSsl = useSSL.Value;
                        if (RaaiVanSettings.SystemEmail.Timeout(applicationId) > 0)
                            smtp.Timeout = RaaiVanSettings.SystemEmail.Timeout(applicationId);
                        smtp.Send(mailObj);
                    }
                }
            }
            catch (Exception ex) { return false; }

            return true;
        }

        public static bool send_email(Guid? applicationId, string email, string emailSubject, string body)
        {
            return send_email(applicationId, email, emailSubject, body, null, null, null, null, null, null);
        }

        public static bool send_email(Guid? applicationId, string email, string body)
        {
            return send_email(applicationId, email, null, body);
        }

        public static bool send_sms(string phoneNumber, string message) {
            return SMSSender.send(phoneNumber, message);
        }

        public static bool is_valid_username(Guid? applicationId, string username)
        {
            try
            {
                if (string.IsNullOrEmpty(RaaiVanSettings.Users.UserNamePattern(applicationId))) return true;
                return new Regex(RaaiVanSettings.Users.UserNamePattern(applicationId)).IsMatch(username);
            }
            catch { return false; }
        }

        public static string map_path(string path, bool localPath = false)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            if (localPath || !(new string[] { "app_data", "global_documents" }).Any(x => path.ToLower().Contains(x)))
                return System.Web.Hosting.HostingEnvironment.MapPath(path);

            path = path.Replace('/', '\\');

            if (path[0] == '~') path = path.Substring(1);
            if (path[0] != '\\') path = "\\" + path;

            return RaaiVanSettings.StoragePath + path;
        }

        public static string get_mime_type_by_extension(string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension)) return "";
                if (extension.LastIndexOf('.') > 0) return "";

                if (extension[0] != '.') extension = "." + extension;

                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

                if (key != null)
                {
                    string contentType = key.GetValue("Content Type").ToString();
                    if (!string.IsNullOrEmpty(contentType)) return contentType;
                }

                return "";
            }
            catch { return ""; }
        }

        public static string http_post_request(string url, NameValueCollection values)
        {
            values["time_stamp"] = DateTime.Now.Millisecond.ToString();
            return Encoding.Default.GetString((new WebClient()).UploadValues(url, values));
        }

        public static T get_value<T>(object obj, Type valueType)
        {
            try
            {
                if (valueType == typeof(Guid))
                    return (T)((object)Guid.Parse(obj.ToString()));
                else if (valueType == typeof(int))
                    return (T)((object)int.Parse(obj.ToString()));
                else if (valueType == typeof(float))
                    return (T)((object)float.Parse(obj.ToString()));
                else if (valueType == typeof(double))
                    return (T)((object)double.Parse(obj.ToString()));
                else if (valueType == typeof(DateTime))
                    return (T)((object)DateTime.Parse(obj.ToString()));
                else if (valueType == typeof(bool))
                    return (T)((object)bool.Parse(obj.ToString()));
                else
                    return (T)obj;
            }
            catch { return default(T); }
        }

        public static T get_value<T>(object obj)
        {
            return get_value<T>(obj, typeof(object));
        }

        public static T get_value<T>(Dictionary<string, object> dic, string name, Type valueType)
        {
            return !dic.ContainsKey(name) ? default(T) : get_value<T>(dic[name], valueType);
        }

        public static T get_value<T>(Dictionary<string, object> dic, string name)
        {
            return get_value<T>(dic, name, typeof(object));
        }
        
        public static ITenant get_current_tenant(IOwinRequest request, List<ITenant> tenants)
        {
            if (tenants.Count == 1) return tenants.First();
            
            string host = request.Uri.Host;
            int port = request.Uri.Port;
            string hostPort = host + (port > 0 ? ":" + port.ToString() : string.Empty);

            ITenant tnt = tenants.FirstOrDefault(x => x.Domain == hostPort);

            return tnt != null ? tnt : tenants.FirstOrDefault(x => x.Domain == host);
        }

        public static string get_exception(Exception ex)
        {
            return ex == null ? string.Empty :
                (ex.InnerException == null || ex.InnerException.Message == null ?
                (string.IsNullOrEmpty(ex.Message) ? ex.ToString() : ex.Message) : ex.InnerException.Message);
        }

        public static string web_exception(WebException ex) {
            try
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                    using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                        return reader.ReadToEnd();
                }
                else return get_exception(ex);
            }
            catch
            {
                return get_exception(ex);
            }
        }

        public static string web_request(string url, NameValueCollection values = null, HTTPMethod method = HTTPMethod.POST,
            NameValueCollection headers = null)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            if (values == null) values = new NameValueCollection();
            if (headers == null) headers = new NameValueCollection();
            
            try
            {
                if (method == HTTPMethod.POST)
                {
                    WebClient client = new WebClient();
                    if (headers != null) client.Headers.Add(headers);
                    return Encoding.Default.GetString(client.UploadValues(url, "POST", values));
                }
                else
                {
                    HttpWebRequest request = WebRequest.CreateHttp(url);

                    if (values.Count == 0) values.Add("dsfalfha4", "1");

                    string data = "{" + string.Join(",", values.AllKeys.Select(
                        key => "\"" + HttpUtility.UrlEncode(key) + "\":\"" + HttpUtility.UrlEncode(values[key]) + "\"")) + "}";

                    request.Method = method.ToString();
                    request.ContentType = "application/json";
                    request.Accept = "application/json";
                    if (method != HTTPMethod.GET) request.ContentLength = data.Length;

                    foreach (string key in headers.Keys)
                        request.Headers.Add(key, headers[key]);

                    if (method != HTTPMethod.GET)
                        using (StreamWriter writer = new StreamWriter(request.GetRequestStream())) writer.Write(data);

                    using (Stream objStream = request.GetResponse().GetResponseStream())
                    using (StreamReader objReader = new StreamReader(objStream)) return objReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                return web_exception(ex);
            }
        }

        public static string upload_file(string url, string fileAddress)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return Encoding.Default.GetString(client.UploadFile(url, fileAddress));
                }
                catch (WebException ex)
                {
                    return web_exception(ex);
                }
            }
        }

        public static bool toZip(string folderPath, string outputFilePath)
        {
            try
            {
                if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

                string[] filenames = Directory.GetFiles(folderPath);

                using (ZipOutputStream s = new ZipOutputStream(File.Create(outputFilePath)))
                {
                    s.SetLevel(9); // 0-9, 9 being the highest compression

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        entry.DateTime = DateTime.Now;

                        s.PutNextEntry(entry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;

                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            }
                            while (sourceBytes > 0);
                        }
                    }

                    s.Finish();
                    s.Close();
                }

                return true;
            }
            catch { return false; }
        }

        public static void set_timeout(Action action, int delay = 0) {
            Thread th = new Thread(() =>
            {
                if (delay > 0) Thread.Sleep(delay);
                action.Invoke();
            });

            th.Priority = ThreadPriority.BelowNormal;

            th.Start();
        }
        
        public static void split_list<T>(List<T> lst, int partLength, Action<List<T>> action, int gap = 0)
        {
            int i = 0;
            int count = lst == null ? 0 : lst.Count;

            while (i * partLength < count)
            {
                List<T> newList = new List<T>();

                for (int x = 0, bs = partLength * i; x < partLength && x + bs < count; ++x)
                    newList.Add(lst[x + bs]);

                action(newList);
                
                ++i;

                if (gap > 0) Thread.Sleep(gap);
            }
        }

        /*
        //to be removed later
        private static bool __FoldersMoved = false;
        
        public static void __MoveFolders(object ObjApplicationId)
        {
            if (__FoldersMoved) return;
            __FoldersMoved = true;
            if (RaaiVanSettings.Tenants.Count > 1) return;

            Guid applicationId = (Guid)ObjApplicationId;

            string[] folders = Enum.GetNames(typeof(FolderNames));
            
            foreach (string str in folders)
            {
                FolderNames folder = (FolderNames)Enum.Parse(typeof(FolderNames), str);

                if (folder == FolderNames.Index) continue;

                string path = DocumentUtilities.map_path(applicationId, folder);

                MatchCollection matches = Regex.Matches(path,
                    "\\\\[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}");
                
                string oldPath = matches.Count == 0 ? path : path.Replace(matches[matches.Count - 1].Value, string.Empty);

                if (oldPath == path) continue;

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                
                if (Directory.Exists(oldPath))
                {
                    string[] subDirs = Directory.GetDirectories(oldPath);
                    string[] files = Directory.GetFiles(oldPath);

                    foreach (string sd in subDirs)
                    {
                        string subfolderName = sd.Replace(oldPath, string.Empty).Substring(1);
                        string newDir = string.Empty;

                        if(folder == FolderNames.PDFImages)
                        {
                            newDir = path + "\\" + DocumentUtilities.get_sub_folder(subfolderName);
                            if (!Directory.Exists(newDir)) Directory.CreateDirectory(newDir);
                            newDir = newDir + "\\" + subfolderName;
                            if (!Directory.Exists(newDir)) Directory.Move(sd, newDir);
                        }
                        else if(oldPath.ToLower().IndexOf("app_data") > 0) {
                            newDir = path + "\\" + subfolderName;
                            if (!Directory.Exists(newDir)) Directory.Move(sd, newDir);
                        }
                        else {
                            newDir = path + "\\" + subfolderName;
                            if (!Directory.Exists(newDir)) Directory.CreateDirectory(newDir);
                        }
                        
                        if (sd.ToLower().IndexOf("highquality") > 0)
                        {
                            string[] newFiles = Directory.GetFiles(sd);

                            foreach (string fl in newFiles)
                            {
                                string fileName = fl.Replace(sd, string.Empty).Substring(1);
                                string newFolderPath = newDir + "\\" + DocumentUtilities.get_sub_folder(fileName);
                                if (!Directory.Exists(newFolderPath)) Directory.CreateDirectory(newFolderPath);
                                string newFilePath = newFolderPath + "\\" + fileName;
                                if (!File.Exists(newFilePath)) File.Move(fl, newFilePath);
                            }
                        }

                        if (Directory.Exists(sd) && !Directory.Exists(sd + "_Moved")) Directory.Move(sd, sd + "_Moved");
                    }

                    foreach (string fl in files)
                    {
                        string fileName = fl.Replace(oldPath, string.Empty).Substring(1);
                        string newFolderPath = path +
                            (path.ToLower().IndexOf("global_documents") > 0 || folder == FolderNames.Attachments ?
                            "\\" + DocumentUtilities.get_sub_folder(fileName) : string.Empty);
                        if (!Directory.Exists(newFolderPath)) Directory.CreateDirectory(newFolderPath);
                        string newFilePath = newFolderPath + "\\" + fileName;
                        if (!File.Exists(newFilePath)) File.Move(fl, newFilePath);
                    }

                    if(Directory.Exists(oldPath) && !Directory.Exists(oldPath + "_Moved"))
                        Directory.Move(oldPath, oldPath + "_Moved");
                }
            }
        }
        //to be removed later
        */
    }

    public static class ExcelUtilities
    {
        private static string _clean_invalid_excel_characters(string input) {
            return Regex.Replace(input, "[\\:\\/\\[\\]\\\\\\?\\*]", " ").Trim(); //:\/?*[]
        }

        private static string _clean_invalid_xml_chars(string input)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            return Regex.Replace(input, @"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]", string.Empty);
        }

        public static XLWorkbook ExportToExcel(string fileName, List<KeyValuePair<string, DataTable>> tables, bool rtl,
            Dictionary<string, string> dic, string password, DownloadedFileMeta meta, bool dontStream = false)
        {
            if (dic == null) dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(fileName)) fileName = fileName.Replace(" ", "_");

            dic.Keys.ToList().ForEach(key => dic[key] = _clean_invalid_excel_characters(dic[key]));

            XLWorkbook wk = new XLWorkbook();
            wk.RightToLeft = rtl;

            int sheetNameCounter = 0;

            tables.ForEach(tableItem => {
                ++sheetNameCounter;

                string sheetName = _clean_invalid_excel_characters(
                    string.IsNullOrEmpty(tableItem.Key) ? "Sheet " + sheetNameCounter.ToString() : tableItem.Key);

                DataTable tbl = tableItem.Value;

                if (string.IsNullOrEmpty(tbl.TableName))
                    tbl.TableName = "tbl_" + PublicMethods.get_random_number().ToString();

                //Remove Hidden Columns & Determine DataTime Columns
                List<string> columNames = new List<string>();
                bool needsDateTimeConversion = false;

                for (int i = 0; i < tbl.Columns.Count; ++i)
                {
                    tbl.Columns[i].ColumnName = _clean_invalid_excel_characters(tbl.Columns[i].ColumnName);

                    if (tbl.Columns[i].ColumnName.IndexOf("_HideC") >= 0)
                        tbl.Columns[i].ColumnName = tbl.Columns[i].ColumnName.Substring(0, tbl.Columns[i].ColumnName.IndexOf("_HideC"));

                    if (tbl.Columns[i].ColumnName.IndexOf("_Hide") >= 0) columNames.Add(tbl.Columns[i].ColumnName);
                    else
                    {
                        bool hasNumber = (new Regex("^[A-Za-z][A-Za-z0-9_]+_[0-9]+$")).IsMatch(tbl.Columns[i].ColumnName);
                        string theName = !hasNumber ? tbl.Columns[i].ColumnName :
                            tbl.Columns[i].ColumnName.Substring(0, tbl.Columns[i].ColumnName.LastIndexOf("_"));
                        string theNumber = !hasNumber ? "" :
                            tbl.Columns[i].ColumnName.Substring(tbl.Columns[i].ColumnName.LastIndexOf("_"));

                        if (dic.ContainsKey(theName) && !string.IsNullOrEmpty(dic[theName]))
                            tbl.Columns[i].ColumnName = dic[theName] + theNumber;

                        if (rtl && tbl.Columns[i].DataType == typeof(DateTime)) needsDateTimeConversion = true;
                    }
                }

                foreach (string cn in columNames) tbl.Columns.Remove(cn);

                if (needsDateTimeConversion)
                {
                    DataTable newDataTable = tbl.Clone();
                    newDataTable.TableName = "tbl_" + PublicMethods.get_random_number().ToString();

                    for (int i = 0, lnt = newDataTable.Columns.Count; i < lnt; ++i)
                        if (tbl.Columns[i].DataType == typeof(DateTime)) newDataTable.Columns[i].DataType = typeof(string);

                    foreach (DataRow row in tbl.Rows) newDataTable.ImportRow(row);

                    for (int c = 0; c < tbl.Columns.Count; ++c)
                    {
                        if (tbl.Columns[c].DataType != typeof(DateTime) && tbl.Columns[c].DataType != typeof(string)) continue;

                        bool isString = tbl.Columns[c].DataType == typeof(string);

                        for (int r = 0; r < tbl.Rows.Count; ++r)
                        {
                            if (!string.IsNullOrEmpty(newDataTable.Rows[r][c].ToString()))
                                newDataTable.Rows[r][c] = isString ? _clean_invalid_xml_chars((string)tbl.Rows[r][c]) :
                                    GenericDate.get_local_date((DateTime)tbl.Rows[r][c]);
                        }
                    }

                    tbl = newDataTable;
                }
                //end of Remove Hidden Columns & Determine DataTime Columns

                if (meta != null && sheetNameCounter == 1)
                {
                    tbl.Columns.Add("Meta").SetOrdinal(0);
                    meta.write2table(tbl, 0);
                }

                IXLWorksheet ws = wk.Worksheets.Add(tbl, sheetName);
                ws.RightToLeft = rtl;

                if (!string.IsNullOrEmpty(password)) ws.Protect(password);
            });

            if (!dontStream)
            {
                if (!string.IsNullOrEmpty(password)) wk.Protect(password);

                HttpResponse response = HttpContext.Current.Response;

                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;

                if (string.IsNullOrEmpty(password))
                {
                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xlsx");

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wk.SaveAs(memoryStream);
                        memoryStream.WriteTo(response.OutputStream);
                        memoryStream.Close();
                    }
                }
                else
                {
                    response.ContentType = "application/octet-stream"; //"application/zip";
                    response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".zip");

                    //response.BinaryWrite(toZip(wk, fileName, password));
                    using (MemoryStream stream = toZip(wk, fileName + ".xlsx", password))
                    {
                        stream.WriteTo(response.OutputStream);
                        stream.Close();
                    }
                }

                try
                {
                    response.Flush();
                    response.End();
                }
                catch { }
            }

            return wk;
        }

        public static XLWorkbook ExportToExcel(string fileName, DataTable tbl, bool rtl,
            Dictionary<string, string> dic, string password, DownloadedFileMeta meta, bool dontStream = false)
        {
            List<KeyValuePair<string, DataTable>> tables = new List<KeyValuePair<string, DataTable>>() {
                new KeyValuePair<string, DataTable>(string.Empty, tbl)
            };

            return ExportToExcel(fileName, tables, rtl, dic, password, meta, dontStream);
        }

        private static MemoryStream toZip(XLWorkbook workbook, string fileName, string password)
        {
            MemoryStream outputStream = new MemoryStream();

            using (MemoryStream inputStream = new MemoryStream())
            {
                workbook.SaveAs(inputStream);

                ZipOutputStream zipStream = new ZipOutputStream(outputStream);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;

                ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName));
                entry.Size = inputStream.Length;

                zipStream.PutNextEntry(entry);

                inputStream.Position = 0;
                StreamUtils.Copy(inputStream, zipStream, new byte[4096]);

                zipStream.CloseEntry();
                zipStream.IsStreamOwner = false;
                zipStream.Close();

                inputStream.Close();
            }

            outputStream.Position = 0;

            return outputStream;
        }

        public static XmlDocument Excel2XML(DocFileInfo excelFile, int sheetNo, ref Dictionary<string, object> map)
        {
            try
            {
                DataTable dt = new DataTable("dt");
                List<string> colNames = new List<string>();

                XLWorkbook wb = null;

                try
                {
                    using (MemoryStream stream = new MemoryStream(excelFile.toByteArray()))
                        wb = new XLWorkbook(stream);
                }
                catch (Exception ex)
                {
                    return null;
                }

                IXLWorksheet ws = wb.Worksheet(sheetNo);

                bool firstRow = true;

                foreach (IXLRow row in ws.Rows())
                {
                    if (row.FirstCellUsed() == null || row.LastCellUsed() == null) break;

                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                        {
                            string colName = cell.Value.ToString().Trim().ToLower();
                            dt.Columns.Add(colName);
                            colNames.Add(colName);
                        }
                        firstRow = false;
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;

                        foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                        {
                            if (i >= dt.Columns.Count) break;
                            dt.Rows[dt.Rows.Count - 1][i++] = cell.Value.ToString().Trim();
                        }
                    }
                }

                wb.Dispose();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ds.GetXml());

                string strMap = "{\"sub\":{" +
                    string.Join(",", colNames.Select(u => "\"" + u + "\":{\"target\":\"" + u + "\"}")) + "}}";

                map = PublicMethods.fromJSON(strMap);

                return doc;
            }
            catch (Exception ex) { string strEx = ex.ToString(); return null; }
        }
    }
}
