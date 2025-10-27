using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class ErrorHandler
    {
        private static Dictionary<string, string> _ErrorMessages;

        private static void _fill()
        {
            if (_ErrorMessages != null) return;
            _ErrorMessages = new Dictionary<string, string>();

            _ErrorMessages["Succeed"] = Messages.OperationCompletedSuccessfully.ToString();
            _ErrorMessages["Failed"] = Messages.OperationFailed.ToString();

            string prefix = string.Empty;
            
            //WF Messages
            prefix = "WF";

            _ErrorMessages[prefix + "1"] = "StateNotFound";
            _ErrorMessages[prefix + "2"] = "WorkFlowNotFound";
            _ErrorMessages[prefix + "3"] = "ServiceNotFound";
            _ErrorMessages[prefix + "4"] = "WorkFlowStateNotFound";
            _ErrorMessages[prefix + "5"] = "DirectorDetectionFailed";
            _ErrorMessages[prefix + "6"] = "HistoryUpdateFailed";
            _ErrorMessages[prefix + "7"] = "HistoryCreationFailed";
            _ErrorMessages[prefix + "8"] = "HistoryDateNeedsCreationFailed";
            _ErrorMessages[prefix + "9"] = "HistoryFormInstancesCopyFailed";
            _ErrorMessages[prefix + "10"] = "FileAttachmentFailed";
            _ErrorMessages[prefix + "11"] = "RejectionIsNotAllowed";
            _ErrorMessages[prefix + "12"] = "MaxAllowedRejectionsExceeded";
            //end of WF Messages
        }

        public static bool succeed(int value, ModuleIdentifier moduleIdentifier, ref string message)
        {
            _fill();

            if (value > 0)
            {
                message = _ErrorMessages["Succeed"];
                return true;
            }
            else if (value == 0)
            {
                message = _ErrorMessages["Failed"];
                return false;
            }
            else
            {
                string str = moduleIdentifier.ToString() + (-value).ToString();
                message = _ErrorMessages[str];
                return false;
            }
        }
    }
}
