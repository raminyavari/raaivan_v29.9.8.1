using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using System.Collections.Specialized;
using System.Collections;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class SMSSender
    {
        private static string apiKey = "ce18278b1fc4afb7eac3d10";
        private static string secretKey = "391F2B12-4EB2-4160-A5E8-192E421B40CC";

        private static GsmCommMain _GsmComm;

        private static bool _connect2gsm()
        {
            string[] _coms = SerialPort.GetPortNames();

            foreach (string com in _coms)
            {
                _GsmComm = new GsmCommMain(com, 9600, 150);

                try
                {
                    if (!_GsmComm.IsOpen()) _GsmComm.Open();
                    if (_GsmComm.IsOpen()) return true;
                }
                catch (Exception e) { string st = e.Message; }
            }

            return false;
        }

        private static bool _disconnect_from_gsm()
        {
            _GsmComm.Close();
            if (!_GsmComm.IsOpen()) return true;
            return false;
        }

        private static bool send_gsm(string phoneNumber, string message)
        {
            bool unicode = true;
            bool alert = false;

            if (((_GsmComm == null || !_GsmComm.IsOpen()) && !_connect2gsm()) || string.IsNullOrEmpty(message)) return false;

            try
            {
                SmsSubmitPdu pdu;

                if (!alert && !unicode)
                    pdu = new SmsSubmitPdu(message, phoneNumber, "");
                else
                {
                    byte dcs;

                    if (!alert && unicode) dcs = DataCodingScheme.NoClass_16Bit;
                    else if (alert && !unicode) dcs = DataCodingScheme.Class0_7Bit;
                    else if (alert && unicode) dcs = DataCodingScheme.Class0_16Bit;
                    else dcs = DataCodingScheme.NoClass_7Bit;

                    pdu = new SmsSubmitPdu(message, phoneNumber, "", dcs);

                    _GsmComm.SendMessage(pdu);
                }

            }
            catch (Exception ex) { return false; }

            return true;
        }

        private static bool send_api(string phoneNumber, string message)
        {
            return true;

            //Get Token
            NameValueCollection headers = new NameValueCollection();

            NameValueCollection data = new NameValueCollection();
            data.Add("UserApiKey", apiKey);
            data.Add("SecretKey", secretKey);

            Dictionary<string, object> response = PublicMethods.fromJSON(
                PublicMethods.web_request("https://RestfulSms.com/api/Token", data, HTTPMethod.POST, headers: headers));

            bool succeed = PublicMethods.get_dic_value<bool>(response, "IsSuccessful", false);
            string token = !succeed ? string.Empty : PublicMethods.get_dic_value(response, "TokenKey", string.Empty);
            //end of Get Token


            if (string.IsNullOrEmpty(token)) return false;


            //Send SMS
            headers = new NameValueCollection();
            headers.Add("x-sms-ir-secure-token", token);

            data = new NameValueCollection();
            data.Add("Messages", "new message");
            data.Add("MobileNumbers", "09192388661");
            data.Add("LineNumber", "50002015264136");
            data.Add("CanContinueInCaseOfError", "true");
            
            response = PublicMethods.fromJSON(
                PublicMethods.web_request("https://RestfulSms.com/api/MessageSend", data, HTTPMethod.POST, headers: headers));

            string xx = response.ToString();
            //end of Send SMS



            return true;
        }

        public static bool send(string phoneNumber, string message)
        {
            return send_api(phoneNumber, message);

            return send_gsm(phoneNumber, message);
        }
    }
}
