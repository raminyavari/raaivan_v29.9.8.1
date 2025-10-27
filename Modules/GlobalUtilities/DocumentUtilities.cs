using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Drawing;
using System.Collections;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum FileOwnerTypes
    {
        None,
        Node,
        FormElement,
        Message,
        Wiki,
        WikiContent,
        WorkFlow,
        PDFCover
    }

    public enum FolderNames
    {
        TemporaryFiles,
        Attachments,
        ProfileImages,
        PDFImages,
        PDFCovers,
        HighQualityProfileImage,
        CoverPhoto,
        HighQualityCoverPhoto,
        Icons,
        HighQualityIcon,
        ApplicationIcons,
        HighQualityApplicationIcon,
        Index,
        Themes,
        WikiContent,
        Pictures,
        EmailTemplates
    }
    
    public enum DefaultIconTypes
    {
        None,
        Node,
        Document,
        Extension
    }

    public enum IconType {
        None,
        ProfileImage,
        CoverPhoto,
        Icon,
        ApplicationIcon
    }

    public class DocumentUtilities
    {
        private static byte[] _FavIcon;

        public static byte[] FavIcon
        {
            get
            {
                if (_FavIcon !=  null) return _FavIcon;

                try
                {
                    byte[] fav = File.ReadAllBytes(PublicMethods.map_path(PublicConsts.FavIcon));
                    if (!PublicMethods.is_dev()) _FavIcon = fav;
                    return fav;
                }
                catch { return new byte[0]; }
            }
        }

        private static Dictionary<string, string> _StaticFiles = new Dictionary<string, string>();

        public static string StaticFile(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            if (_StaticFiles.ContainsKey(name.ToLower())) return _StaticFiles[name.ToLower()];

            try
            {
                string path = PublicMethods.map_path(name);
                string content = !File.Exists(path) ? string.Empty : File.ReadAllText(path);
                if (!PublicMethods.is_dev()) _StaticFiles[name.ToLower()] = content;
                return content;
            }
            catch { return string.Empty; }
        }

        protected static DocFileInfo _get_file_info(Guid? applicationId, Dictionary<string, object> dic)
        {
            if (dic == null) return null;

            Guid? fileId = !dic.ContainsKey("FileID") ? null : PublicMethods.parse_guid(dic["FileID"].ToString());
            string extension = !dic.ContainsKey("Extension") ? null : PublicMethods.parse_string(dic["Extension"].ToString());
            string fileName = !dic.ContainsKey("FileName") ? null : PublicMethods.parse_string(dic["FileName"].ToString());
            long? size = !dic.ContainsKey("Size") ? null : PublicMethods.parse_long(dic["Size"].ToString());
            Guid? ownerId = !dic.ContainsKey("OwnerID") ? null : PublicMethods.parse_guid(dic["OwnerID"].ToString());

            FileOwnerTypes ownerType = FileOwnerTypes.None;
            if (dic.ContainsKey("OwnerType")) Enum.TryParse<FileOwnerTypes>(dic["OwnerType"].ToString(), true, out ownerType);

            DocFileInfo fi = new DocFileInfo(applicationId)
            {
                FileID = fileId,
                FileName = fileName,
                Extension = extension,
                Size = size
            };

            if (ownerId.HasValue && ownerId != Guid.Empty) fi.OwnerID = ownerId;
            if (ownerType != FileOwnerTypes.None) fi.OwnerType = ownerType;

            return !fileId.HasValue ? null : fi;
        }

        public static List<DocFileInfo> get_files_info(Guid? applicationId, string strFiles)
        {
            List<DocFileInfo> retList = new List<DocFileInfo>();

            if (string.IsNullOrEmpty(strFiles)) return retList;

            Dictionary<string, object> dic = PublicMethods.fromJSON("{\"Items\":" + Base64.decode(strFiles) + "}");

            if (!dic.ContainsKey("Items")) return retList;

            if (dic["Items"].GetType() == typeof(Dictionary<string, object>)) {
                DocFileInfo fi = _get_file_info(applicationId, (Dictionary<string, object>)dic["Items"]);
                if (fi != null) retList.Add(fi);
            }
            else if (dic["Items"].GetType() == typeof(ArrayList))
            {
                foreach (object obj in (ArrayList)dic["Items"])
                {
                    Dictionary<string, object> f = obj.GetType() == typeof(string) ? PublicMethods.fromJSON((string)obj) :
                        (obj.GetType() == typeof(Dictionary<string, object>) ? (Dictionary<string, object>)obj : null);

                    DocFileInfo fi = f == null ? null : _get_file_info(applicationId, f);
                    if (fi != null) retList.Add(fi);
                }
            }

            return retList;
        }
        
        public static string get_files_json(List<DocFileInfo> attachedFiles, bool icon = false)
        {
            return attachedFiles == null || attachedFiles.Count == 0 ? "[]" : 
                "[" + string.Join(",", attachedFiles.Select(u => u.toJson(icon))) + "]";
        }

        public static DocFileInfo decode_base64_file_content(Guid applicationId, Guid? ownerId,
            string base64FileContent, FileOwnerTypes ownerType)
        {
            if (string.IsNullOrEmpty(base64FileContent)) return null;

            byte[] theData = null;

            try { theData = Convert.FromBase64String(base64FileContent); }
            catch { return null; }

            int FIXED_HEADER = 16;

            DocFileInfo ret = new DocFileInfo(applicationId)
            {
                FileID = Guid.NewGuid(),
                OwnerID = ownerId,
                OwnerType = ownerType,
                FolderName = FolderNames.TemporaryFiles
            };

            try
            {
                using (MemoryStream ms = new MemoryStream(theData))
                {
                    using (BinaryReader theReader = new BinaryReader(ms))
                    {
                        //Position the reader to get the file size.
                        byte[] headerData = new byte[FIXED_HEADER];
                        headerData = theReader.ReadBytes(headerData.Length);

                        ret.Size = (int)theReader.ReadUInt32();
                        int fileNameLength = (int)theReader.ReadUInt32() * 2;

                        if (fileNameLength <= 0 || fileNameLength > 255) throw new Exception("what the fuzz!!");

                        byte[] fileNameBytes = theReader.ReadBytes(fileNameLength);
                        //InfoPath uses UTF8 encoding.
                        Encoding enc = Encoding.Unicode;
                        string fullFileName = enc.GetString(fileNameBytes, 0, fileNameLength - 2);

                        int dotIndex = fullFileName.LastIndexOf(".");
                        if (dotIndex > 0 && dotIndex < (fullFileName.Length - 1))
                            ret.Extension = fullFileName.Substring(dotIndex + 1);

                        ret.FileName = string.IsNullOrEmpty(ret.Extension) ?
                            fullFileName : fullFileName.Substring(0, dotIndex);

                        byte[] fileBytes = theReader.ReadBytes((int)ret.Size.Value);

                        if (!ret.store(fileBytes)) return null;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                //maybe the file is a base64 image!!
                try
                {
                    Image img = PublicMethods.image_from_byte_array(theData);
                    if (img == null) return null;
                    byte[] imageBytes = PublicMethods.image_to_byte_array(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                    if (imageBytes == null || imageBytes.Length == 0) return null;

                    ret.Size = imageBytes.Length;
                    ret.FileName = "img";
                    ret.Extension = "jpg";

                    if (!ret.store(imageBytes)) return null;

                    return ret;
                }
                catch { return null; }
            }
        }

        public static string get_personal_image_address(Guid? applicationId, 
            Guid? userId, bool networkAddress = false, bool highQuality = false)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;
            
            if (!userId.HasValue || userId == Guid.Empty)
            {
                string addr = PublicConsts.DefaultProfileImageURL;

                return highQuality ? string.Empty :
                    (networkAddress ? addr.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : addr);
            }

            FolderNames folderName = highQuality ? FolderNames.HighQualityProfileImage : FolderNames.ProfileImages;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = userId,
                Extension = "jpg",
                FolderName = folderName
            };
            
            string address = !fi.exists() ? 
                (highQuality ? string.Empty : PublicConsts.DefaultProfileImageURL) : fi.url();
            
            return networkAddress ? address.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : address;
        }
        
        public static bool picture_exists(Guid applicationId, Guid pictureId)
        {
            if (pictureId == Guid.Empty) return false;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = pictureId,
                Extension = "jpg",
                FolderName = FolderNames.Pictures
            };

            return fi.exists();
        }

        private static bool get_icon_parameters(Guid? applicationId, IconType iconType, ref int width, ref int height, 
            ref int highQualityWidth, ref int highQualityHeight, ref FolderNames folder, 
            ref FolderNames highQualityFolder, ref string defaultIconUrl)
        {
            switch (iconType)
            {
                case IconType.ProfileImage:
                    width = height = 100;
                    highQualityWidth = highQualityHeight = 600;
                    folder = FolderNames.ProfileImages;
                    highQualityFolder = FolderNames.HighQualityProfileImage;
                    defaultIconUrl = get_personal_image_address(applicationId, null);
                    break;
                case IconType.CoverPhoto:
                    width = 900;
                    height = 220;
                    highQualityWidth = 1800;
                    highQualityHeight = 600;
                    folder = FolderNames.CoverPhoto;
                    highQualityFolder = FolderNames.HighQualityCoverPhoto;
                    defaultIconUrl = get_cover_photo_url(applicationId, null);
                    break;
                case IconType.Icon:
                    width = height = 100;
                    highQualityWidth = highQualityHeight = 600;
                    folder = FolderNames.Icons;
                    highQualityFolder = FolderNames.HighQualityIcon;
                    defaultIconUrl = get_icon_url(applicationId, DefaultIconTypes.Node);
                    break;
                case IconType.ApplicationIcon:
                    width = height = 100;
                    highQualityWidth = highQualityHeight = 600;
                    folder = FolderNames.ApplicationIcons;
                    highQualityFolder = FolderNames.HighQualityApplicationIcon;
                    defaultIconUrl = get_application_icon_url(null);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public static bool get_icon_parameters(Guid? applicationId, IconType iconType, ref int width, ref int height, 
            ref int highQualityWidth, ref int highQualityHeight, ref FolderNames folder, ref FolderNames highQualityFolder) {
            string defaultIconUrl = string.Empty;

            return get_icon_parameters(applicationId, iconType, ref width, ref height, ref highQualityWidth, 
                ref highQualityHeight, ref folder, ref highQualityFolder, ref defaultIconUrl);
        }

        public static bool get_icon_parameters(Guid? applicationId, IconType iconType, ref int width, ref int height,
            ref FolderNames folder, ref FolderNames highQualityFolder)
        {
            int highQualityWidth = 0, highQualityHeight = 0;
            return get_icon_parameters(applicationId, iconType, ref width, ref height,
                ref highQualityWidth, ref highQualityHeight, ref folder, ref highQualityFolder);
        }

        public static bool get_icon_parameters(Guid? applicationId, IconType iconType, ref FolderNames folder, 
            ref FolderNames highQualityFolder, ref string defaultIconUrl)
        {
            int width = 0, height = 0, highQualityWidth = 0, highQualityHeight = 0;
            return get_icon_parameters(applicationId, iconType, ref width, ref height,
                ref highQualityWidth, ref highQualityHeight, ref folder, ref highQualityFolder, ref defaultIconUrl);
        }

        public static string get_icon_url(Guid? applicationId, DefaultIconTypes defaultIcon, 
            string extension = "", bool networkAddress = false)
        {
            string adr = string.Empty;

            switch (defaultIcon)
            {
                case DefaultIconTypes.Document:
                    adr = "../../images/archive.png";
                    break;
                case DefaultIconTypes.Extension:
                    adr = "../../images/extensions/" + extension + ".png";
                    string path = PublicMethods.map_path("~/images/extensions") + "\\" + extension + ".png";
                    adr = File.Exists(path) ? adr : "../../images/archive.png";
                    break;
                default:
                    adr = "../../images/Preview.png";
                    break;
            }

            return networkAddress ? adr.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : adr;
        }
        
        public static string get_icon_url(Guid applicationId, Guid ownerId, 
            DefaultIconTypes defaultIcon = DefaultIconTypes.Node, Guid? alternateOwnerId = null, bool networkAddress = false)
        {
            if (ownerId == Guid.Empty) return string.Empty;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = ownerId,
                OwnerID = ownerId,
                Extension = "jpg",
                FolderName = FolderNames.Icons
            };

            string retUrl = fi.exists() ? fi.url() : string.Empty;

            if (string.IsNullOrEmpty(retUrl) && alternateOwnerId.HasValue)
            {
                fi.FileID = alternateOwnerId;
                retUrl = fi.exists() ? fi.url() : string.Empty;
            }

            if (string.IsNullOrEmpty(retUrl) && defaultIcon != DefaultIconTypes.None)
                retUrl = get_icon_url(applicationId, defaultIcon);

            return networkAddress ? retUrl.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : retUrl;
        }
        
        public static string get_icon_url(Guid applicationId, Guid ownerId, string extension,
            bool highQuality = false, bool networkAddress = false)
        {
            if (ownerId == Guid.Empty) return string.Empty;

            FolderNames folderName = highQuality ? FolderNames.HighQualityIcon : FolderNames.Icons;

            DocFileInfo fi = new DocFileInfo(applicationId)
            {
                FileID = ownerId,
                OwnerID = ownerId,
                Extension = "jpg",
                FolderName = folderName
            };

            string retUrl = fi.exists() ? fi.url() :
                (highQuality ? string.Empty : get_icon_url(applicationId, DefaultIconTypes.Extension, extension));

            return networkAddress ? retUrl.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : retUrl;
        }
        
        public static string get_icon_url(Guid applicationId, string fileExtention, bool networkAddress = false)
        {
            string url = "~/images/extensions/" + fileExtention + ".png";
            string adr = File.Exists(PublicMethods.map_path(url)) ? url.Replace("~", "../..") : string.Empty;

            return networkAddress ? adr.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : adr;
        }
        
        public static bool icon_exists(Guid applicationId, Guid ownerId)
        {
            if (ownerId == Guid.Empty) return false;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = ownerId,
                OwnerID = ownerId,
                Extension = "jpg",
                FolderName = FolderNames.Icons
            };

            return fi.exists();
        }
        
        public static string get_icon_json(Guid applicationId, Guid ownerId)
        {
            return new DocFileInfo(applicationId)
            {
                FileID = ownerId,
                FileName = "آیکون",
                Extension = "jpg",
                OwnerID = ownerId
            }.toJson();
        }

        public static string get_application_icon_url(Guid? applicationId, bool highQuality = false, bool networkAddress = false)
        {
            FolderNames folderName = highQuality ? FolderNames.HighQualityApplicationIcon : FolderNames.ApplicationIcons;

            if (!applicationId.HasValue || applicationId == Guid.Empty)
            {
                string addr = RaaiVanSettings.LogoMiniURL;
                
                return highQuality ? string.Empty :
                    (networkAddress ? addr.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : addr);
            }

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = applicationId,
                OwnerID = applicationId,
                Extension = "jpg",
                FolderName = folderName
            };

            string retUrl = fi.exists() ? fi.url() : string.Empty;

            if (string.IsNullOrEmpty(retUrl) && !highQuality) retUrl = RaaiVanSettings.LogoMiniURL;

            return networkAddress ? retUrl.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : retUrl;
        }

        public static string get_cover_photo_url(Guid? applicationId, 
            Guid? ownerId, bool networkAddress = false, bool highQuality = false)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) applicationId = null;

            if (!ownerId.HasValue || ownerId == Guid.Empty)
            {
                string addr = PublicConsts.DefaultCoverPhotoURL;

                return highQuality ? string.Empty :
                    (networkAddress ? addr.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : addr);
            }

            FolderNames folderName = highQuality ? FolderNames.HighQualityCoverPhoto : FolderNames.CoverPhoto;

            DocFileInfo fi = new DocFileInfo(applicationId) {
                FileID = ownerId,
                OwnerID = ownerId,
                Extension = "jpg",
                FolderName = folderName
            };

            string retUrl = fi.exists() ? fi.url() : string.Empty;

            return networkAddress ? retUrl.Replace("../..", RaaiVanSettings.RaaiVanURL(applicationId)) : retUrl;
        }
        
        public static string get_download_url(Guid applicationId, Guid fileId)
        {
            return PublicConsts.get_complete_url(applicationId, PublicConsts.FileDownload) +
                "?timeStamp=" + DateTime.Now.Ticks.ToString() + "&FileID=" + fileId.ToString();
        }
        
        private static byte[] _aes_encryption(byte[] input, bool decrypt, bool useTokenKey)
        {
            //static key & salt
            byte[] AES_KEY = useTokenKey ? USBToken.read_encryption_key() : new byte[] {
                111, 14, 160, 236, 16, 107, 182, 80, 12, 58, 227, 77, 4, 127, 67, 27,
                212, 21, 173, 27, 254, 16, 130, 6, 198, 112, 21, 71, 144, 48, 170, 183
            };

            byte[] AES_SALT = new byte[] { 198, 254, 21, 67, 107, 14, 183, 80 };
            //end of static key & salt

            byte[] retBytes = new byte[0];

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(AES_KEY, AES_SALT, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    using (CryptoStream cs = new CryptoStream(memoryStream,
                        decrypt ? aes.CreateDecryptor() : aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            cs.Write(input, 0, input.Length);
                            cs.Dispose();
                        }
                        catch { }
                    }

                    retBytes = memoryStream.ToArray();
                }
            }

            return retBytes;
        }

        private static byte[] _aes_encryption(byte[] input, bool decrypt) {
            return _aes_encryption(input, decrypt, RaaiVanSettings.USBToken);
        }

        public static byte[] encrypt_bytes_aes(byte[] input)
        {
            return _aes_encryption(input, decrypt: false);
        }

        public static byte[] encrypt_bytes_aes_native(byte[] input)
        {
            return _aes_encryption(input, decrypt: false, useTokenKey: false);
        }

        public static byte[] decrypt_bytes_aes(byte[] input)
        {
            return _aes_encryption(input, decrypt: true);
        }

        public static byte[] decrypt_bytes_aes_native(byte[] input)
        {
            return _aes_encryption(input, decrypt: true, useTokenKey: false);
        }
    }
}
