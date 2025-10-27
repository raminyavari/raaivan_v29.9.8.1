using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum Command_Error
    {
        ERR_ERASE_RAM_RTC = 1, ERR_PACKET_NUMBER, ERR_CHECKSUM, ERR_WRITE_FLASH, ERR_LENGTH, ERR_CRYPT,
        ERR_sFLASH, ERR_WR_MEM, ERR_ERASE, ERR_WRITE_RTC, ERR_TYPE_USER, ERR_CMD_LACK, ERR_SEQUENCE, ERR_CS_NOT_VALID,
        ERR_SID, NO_ALG, ERR_EXPIRED_ADMIN, ERR_EXPIRED_FACTORY, ERR_Set_Time, OK, ERR_No_Device, NO_Define_Error, Send_Error, Timeout_Error,
        Incorrect_Password, No_Firmware_Error, Invalid_DataLength, Check_Sum_Error, Overflow_Software_Number_Error, Invalid_Username_pass_length
    };
    
    public class USBToken
    {
        [DllImport("Hid_Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Command_Error Connect_To_Device(string User_name, string pass, byte len_user, byte len_pass);

        [DllImport("Hid_Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Command_Error Set_Sec_Mem_Msg(byte[] Data, int In_Len);

        [DllImport("Hid_Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Command_Error Write_Secure_Memory(byte[] Start_Addr, byte[] Memory_Data, int In_Len);

        [DllImport("Hid_Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Command_Error Read_Secure_Memory(byte[] Start_Addr, byte[] Data, int Out_Len);

        private static string Username = null, Password = null;

        private static void init_login() {
            if (Username != null || Password != null) return;

            Username = Password = "";

            string path = PublicMethods.map_path(PublicConsts.LicenseFilePath);
            if (!File.Exists(path)) return;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                fs.Close();
                fs.Dispose();

                Dictionary<string, object> dic = PublicMethods.fromJSON(Base64.decode(
                    Encoding.ASCII.GetString(DocumentUtilities.decrypt_bytes_aes_native(data))));

                if (dic.ContainsKey("username")) Username = (string)dic["username"];
                if (dic.ContainsKey("password")) Password = (string)dic["password"];
            }
        }
        
        private static bool Connecting = false;
        private static DateTime? LastTimeConnected = null;
        private static int ThreadSleepDuration = 300;

        private static bool _connect(bool checkLastTime)
        {
            while (Connecting) Thread.Sleep(ThreadSleepDuration);

            try
            {
                Connecting = true;

                init_login();

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) ||
                    Username.ToLower() == "factory" || Username.ToLower() == "admin") return (Connecting = false);
                else if (checkLastTime && LastTimeConnected.HasValue && LastTimeConnected.Value > DateTime.Now.AddMinutes(-5))
                    return !(Connecting = false);

                Command_Error result = Connect_To_Device(Username, Password, (byte)Username.Length, (byte)Password.Length);

                Connecting = false;

                if (result == Command_Error.OK)
                {
                    LastTimeConnected = DateTime.Now;
                    return true;
                }
                else return false;
            }
            catch
            {
                Connecting = false;
                return false;
            }
        }

        public static bool connect()
        {
            return _connect(true);
        }

        //Periodical Check
        private static bool _LastConnectedStateInited = false;
        private static bool _StillConnected;
        public static bool StillConnected
        {
            get
            {
                if (!_LastConnectedStateInited) check_connectivity();
                return _StillConnected;
            }
        }
        
        private static void check_connectivity()
        {
            if (_LastConnectedStateInited) return;
            _LastConnectedStateInited = true;

            new Thread(() =>
            {
                while (true)
                {
                    _StillConnected = _connect(false);
                    Thread.Sleep((_StillConnected ? 60 : 10) * 1000);
                }
            }).Start(); ;
        }
        //end of Periodical check
        
        private static byte[] LicenseAddress = new byte[] { 0, 0 };
        private static byte[] EncryptionKeyAddress = new byte[] { 0, 20 };

        private static bool set_data_key() {
            byte[] secMemKey = new byte[] { 12, 171, 48, 83, 5, 213, 124, 37, 96, 142, 65, 29, 118, 239, 152, 7 };
            return connect() && Set_Sec_Mem_Msg(secMemKey, secMemKey.Length) == Command_Error.OK;
        }

        private static bool write(byte[] address, byte[] data)
        {
            try
            {
                return (data.Length % 16 == 0) && set_data_key() &&
                    Write_Secure_Memory(address, data, data.Length) == Command_Error.OK;
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                return false;
            }
        }

        public static bool write_license(string license) {
            byte[] data = Encoding.ASCII.GetBytes(license);
            return data.Length == PublicConsts.LicenseLengthInBytes && write(LicenseAddress, data);
        }

        public static bool write_encryption_key(byte[] data) {
            return data.Length == PublicConsts.EncryptionKeyLengthInBytes && write(EncryptionKeyAddress, data);
        }

        private static bool Reading = false;

        private static byte[] read(byte[] address, int length) {
            try
            {
                if (length % 16 != 0 || !set_data_key()) return new byte[0];

                while (Reading) Thread.Sleep(ThreadSleepDuration);

                Reading = true;

                byte[] ret = new byte[length];
                Command_Error result = Read_Secure_Memory(address, ret, length);

                Reading = false;

                return result == Command_Error.OK ? ret : new byte[0];
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                Reading = false;
                return new byte[0];
            }
        }

        private static string _License = null;

        public static string read_license()
        {
            if (_License == null)
                _License = Encoding.ASCII.GetString(read(LicenseAddress, PublicConsts.LicenseLengthInBytes));
            return _License;
        }

        private static byte[] _EncryptionKey = null;

        public static byte[] read_encryption_key()
        {
            if (_EncryptionKey == null)
                _EncryptionKey = read(EncryptionKeyAddress, PublicConsts.EncryptionKeyLengthInBytes);
            return _EncryptionKey;
        }
    }
}
