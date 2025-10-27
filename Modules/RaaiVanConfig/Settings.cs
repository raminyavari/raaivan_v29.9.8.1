using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaaiVan.Modules.RaaiVanConfig
{
    public static class Modules
    {
        private static bool _WorkFlow = true;
        private static bool _FormGenerator = true;
        private static bool _Notifications = true;
        private static bool _SMSEMailNotifier = true;
        private static bool _SocialNetwork = true;
        private static bool _KnowledgeAdmin = true;
        private static bool _Knowledge = true;
        private static bool _Documents = true;
        private static bool _PDFViewer = true;
        private static bool _QA = true;
        private static bool _QAAdmin = true;
        private static bool _Events = false;
        private static bool _Messaging = true;
        private static bool _Chat = true;
        private static bool _Resume = true;
        private static bool _VisualMap = true;
        private static bool _RealTime = true;
        private static bool _Explorer = true;
        private static bool _RestAPI = true;
        private static bool _Recommender = false;

        public static bool WorkFlow(Guid? applicationId = null) { return _WorkFlow; }
        public static bool FormGenerator(Guid? applicationId) { return _FormGenerator; }
        public static bool Notifications(Guid? applicationId) { return _Notifications; }
        public static bool SMSEMailNotifier(Guid? applicationId) { return _SMSEMailNotifier; }
        public static bool SocialNetwork(Guid? applicationId) { return _SocialNetwork; }
        public static bool KnowledgeAdmin(Guid? applicationId) { return _KnowledgeAdmin; }
        public static bool Knowledge(Guid? applicationId) { return _Knowledge; }
        public static bool Documents(Guid? applicationId) { return _Documents; }
        public static bool PDFViewer(Guid? applicationId) { return _PDFViewer; }
        public static bool QA(Guid? applicationId) { return _QA; }
        public static bool QAAdmin(Guid? applicationId) { return _QAAdmin; }
        public static bool Events(Guid? applicationId) { return _Events; }
        public static bool Messaging(Guid? applicationId) { return _Messaging; }
        public static bool Chat(Guid? applicationId) { return _Chat; }
        public static bool Resume(Guid? applicationId) { return _Resume; }
        public static bool VisualMap(Guid? applicationId) { return _VisualMap; }
        public static bool RealTime(Guid? applicationId) { return _RealTime; }
        public static bool Explorer(Guid? applicationId) { return _Explorer; }
        public static bool RestAPI(Guid? applicationId) { return _RestAPI; }
        public static bool Recommender(Guid? applicationId) { return _Recommender; }
    }

    public static class GlobalSettings
    {
        public static int? MaxAllowedActiveUsersCount = null;
        public static DateTime? SystemExpirationDate = null;
        public static List<Guid> TenantIDs = new List<Guid>();
        private static int _MaxTenantsCount = 20;
        public static int MaxTenantsCount
        {
            get { return TenantIDs != null && TenantIDs.Count > 0 ? TenantIDs.Count : _MaxTenantsCount; }
        }

        public static string RefSysID = "";
    }

    public enum Edition
    {
        Custom,
        Fandogh
    }
}
