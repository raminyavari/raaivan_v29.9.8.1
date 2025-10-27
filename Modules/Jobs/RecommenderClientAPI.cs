using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Threading;

namespace RaaiVan.Modules.Jobs
{
    public class AnalyzeEntity
    {
        public string ID;
        public int TypeID;
        public DateTime? Time;

        public AnalyzeEntity()
        {
            ID = string.Empty;
            TypeID = 0;
            Time = null;
        }
    }

    public class AnalyzeRelation
    {
        public string ID;
        public int TypeID;
        public string SourceID;
        public int SourceTypeID;
        public string DestinationID;
        public int DestinationTypeID;
        public DateTime? Time;
        public bool Bidirectional;

        public AnalyzeRelation()
        {
            ID = SourceID = DestinationID = string.Empty;
            TypeID = SourceTypeID = DestinationTypeID = 0;
            Time = null;
            Bidirectional = false;
        }
    }

    public class AnalyzeConfigRelation
    {
        private int _TypeID;
        private double _Value;

        public AnalyzeConfigRelation()
        {
            _TypeID = 0;
            _Value = 0;
        }

        public int TypeID
        {
            get { return _TypeID; }
            set { _TypeID = value < 0 ? 0 : value; }
        }

        public double Value
        {
            get { return _Value; }
            set { _Value = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }
    }

    public class AnalyzeConfigSelectionCount
    {
        private int _EntityTypeID;
        private int _SelectionCount;

        public AnalyzeConfigSelectionCount()
        {
            _EntityTypeID = 0;
            _SelectionCount = 100;
        }

        public int EntityTypeID
        {
            get { return _EntityTypeID; }
            set { _EntityTypeID = value < -1 ? -1 : value; }
        }

        public int SelectionCount
        {
            get { return _SelectionCount; }
            set { _SelectionCount = value < 10 ? 10 : value; }
        }
    }

    public class AnalyzeConfigLimitFilterItem
    {
        private int _EntityTypeID;
        private int _RecommendedEntityTypeID;

        public AnalyzeConfigLimitFilterItem()
        {
            _EntityTypeID = _RecommendedEntityTypeID = 0;
        }

        public int EntityTypeID
        {
            get { return _EntityTypeID; }
            set { _EntityTypeID = value < -1 ? -1 : value; }
        }

        public int RecommendedEntityTypeID
        {
            get { return _RecommendedEntityTypeID; }
            set { _RecommendedEntityTypeID = value < -1 ? -1 : value; }
        }
    }

    public enum AnalyzeConfigTimeDependence : int
    {
        Independent = 0,
        LowlyDependent = 1,
        HighlyDependent = 2,
        AbsolutelyDependent = 3
    }

    public enum AnalyzeConfigPathMergeMode : int
    {
        Average = 1,
        Maximum = 2
    }

    public class AnalyzeContext
    {
        private int _ID;
        private int _TargetRelationTypeID;
        private List<int> _TargetRelationDependentTypeIDs;
        private double _AnalyzeRelationTimeFactor;
        private double _AnalyzeRelationTypeFactor;
        private double _AnalyzeRelationCountFactor;
        private double _FinalScoreNormalizationMergeCountFactor;
        private double _FinalScoreNormalizationMergeValueFactor;
        private bool _FinalScoreDevideByPathLength;
        private List<AnalyzeConfigRelation> _Relations;
        private List<AnalyzeConfigSelectionCount> _SelectionCounts;
        private List<AnalyzeConfigLimitFilterItem> _Recommends;

        public bool ConsiderRecommendsAsFilter;
        public AnalyzeConfigTimeDependence TimeDependence;
        public AnalyzeConfigPathMergeMode MergeMode;

        public int ID
        {
            get { return _ID; }
            set { _ID = value < 0 ? 0 : value; }
        }

        public int TargetRelationTypeID
        {
            get { return _TargetRelationTypeID; }
            set { _TargetRelationTypeID = value < 0 ? 0 : value; }
        }

        public List<int> TargetRelationDependentTypeIDs
        {
            get { return _TargetRelationDependentTypeIDs; }
            set { _TargetRelationDependentTypeIDs = value == null ? new List<int>() : value; }
        }

        public double AnalyzeRelationTimeFactor
        {
            get { return _AnalyzeRelationTimeFactor; }
            set { _AnalyzeRelationTimeFactor = value > 1000 ? 1000 : (value < 0 ? 0 : value); }
        }

        public double AnalyzeRelationTypeFactor
        {
            get { return _AnalyzeRelationTypeFactor; }
            set { _AnalyzeRelationTypeFactor = value > 1000 ? 1000 : (value < 0 ? 0 : value); }
        }

        public double AnalyzeRelationCountFactor
        {
            get { return _AnalyzeRelationCountFactor; }
            set { _AnalyzeRelationCountFactor = value > 1000 ? 1000 : (value < 0 ? 0 : value); }
        }

        public double FinalScoreNormalizationMergeCountFactor
        {
            get { return _FinalScoreNormalizationMergeCountFactor; }
            set { _FinalScoreNormalizationMergeCountFactor = value > 1000 ? 1000 : (value < 0 ? 0 : value); }
        }

        public double FinalScoreNormalizationMergeValueFactor
        {
            get { return _FinalScoreNormalizationMergeValueFactor; }
            set { _FinalScoreNormalizationMergeValueFactor = value > 1000 ? 1000 : (value < 0 ? 0 : value); }
        }

        public bool FinalScoreDivideByPathLenght
        {
            get { return _FinalScoreDevideByPathLength; }
            set { _FinalScoreDevideByPathLength = value; }
        }

        public List<AnalyzeConfigRelation> Relations
        {
            get { return _Relations; }
            set { _Relations = value == null ? new List<AnalyzeConfigRelation>() : value; }
        }

        public List<AnalyzeConfigSelectionCount> SelectionCounts
        {
            get { return _SelectionCounts; }
            set { _SelectionCounts = value == null ? new List<AnalyzeConfigSelectionCount>() : value; }
        }

        public List<AnalyzeConfigLimitFilterItem> Recommends
        {
            get { return _Recommends; }
            set { _Recommends = value == null ? new List<AnalyzeConfigLimitFilterItem>() : value; }
        }

        public AnalyzeContext()
        {
            _ID = 0;
            _TargetRelationTypeID = 0;
            _TargetRelationDependentTypeIDs = new List<int>();
            _AnalyzeRelationTimeFactor = 0;
            _AnalyzeRelationTypeFactor = 1;
            _AnalyzeRelationCountFactor = 0.1;
            _FinalScoreNormalizationMergeCountFactor = 0.3;
            _FinalScoreNormalizationMergeValueFactor = 0.8;
            _FinalScoreDevideByPathLength = true;

            _Relations = new List<AnalyzeConfigRelation>();
            _SelectionCounts = new List<AnalyzeConfigSelectionCount>();
            _Recommends = new List<AnalyzeConfigLimitFilterItem>();
            ConsiderRecommendsAsFilter = false;
            TimeDependence = AnalyzeConfigTimeDependence.Independent;
            MergeMode = AnalyzeConfigPathMergeMode.Maximum;
        }
    }

    public enum AnalyzeLogMode
    {
        info,
        trace,
        debug,
        error
    }

    public class AnalyzeMasterConfiguration
    {
        private float _DefaultRelationValue;
        private int _DefaultSelectionCount;
        private int _MaxSelectionCount;
        private bool _FinalScoreNormalization;
        private double _FinalScoreNormalizationFactor;
        private double _ForgetFactor_TimeAbsolutelyDependent;
        private double _ForgetFactor_TimeHighlyDependent;
        private double _ForgetFactor_TimeLowlyDependent;
        private double _FreshFactor_TimeAbsolutelyDependent;
        private double _FreshFactor_TimeHighlyDependent;
        private double _FreshFactor_TimeLowlyDependent;
        private AnalyzeLogMode _LogMode;
        private bool _LogWrite;
        private int _LogWriteMaxSizeInMB;
        private int _MappersCount;
        private int _ReducersCount;
        private int _AnalyzationDepth;

        public float DefaultRelationValue
        {
            get { return _DefaultRelationValue; }
            set { _DefaultRelationValue = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public int DefaultSelectionCount
        {
            get { return _DefaultSelectionCount; }
            set { _DefaultSelectionCount = value > 300 ? 300 : (value < 10 ? 10 : value); }
        }

        public int MaxSelectionCount
        {
            get { return _MaxSelectionCount; }
            set { _MaxSelectionCount = value > 300 ? 300 : (value < 100 ? 100 : value); }
        }

        public bool FinalScoreNormalization
        {
            get { return _FinalScoreNormalization; }
            set { _FinalScoreNormalization = value; }
        }

        public double FinalScoreNormalizationFactor
        {
            get { return _FinalScoreNormalizationFactor; }
            set { _FinalScoreNormalizationFactor = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double ForgetFactor_TimeAbsolutelyDependent
        {
            get { return _ForgetFactor_TimeAbsolutelyDependent; }
            set { _ForgetFactor_TimeAbsolutelyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double ForgetFactor_TimeHighlyDependent
        {
            get { return _ForgetFactor_TimeHighlyDependent; }
            set { _ForgetFactor_TimeHighlyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double ForgetFactor_TimeLowlyDependent
        {
            get { return _ForgetFactor_TimeLowlyDependent; }
            set { _ForgetFactor_TimeLowlyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double FreshFactor_TimeAbsolutelyDependent
        {
            get { return _FreshFactor_TimeAbsolutelyDependent; }
            set { _FreshFactor_TimeAbsolutelyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double FreshFactor_TimeHighlyDependent
        {
            get { return _FreshFactor_TimeHighlyDependent; }
            set { _FreshFactor_TimeHighlyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public double FreshFactor_TimeLowlyDependent
        {
            get { return _FreshFactor_TimeLowlyDependent; }
            set { _FreshFactor_TimeLowlyDependent = value > 1 ? 1 : (value < 0 ? 0 : value); }
        }

        public AnalyzeLogMode LogMode
        {
            get { return _LogMode; }
            set { _LogMode = value; }
        }

        public bool LogWrite
        {
            get { return _LogWrite; }
            set { _LogWrite = value; }
        }

        public int LogWriteMaxSizeInMB
        {
            get { return _LogWriteMaxSizeInMB; }
            set { _LogWriteMaxSizeInMB = value > 10000 ? 10000 : (value < 10 ? 10 : value); }
        }

        public int MappersCount
        {
            get { return _MappersCount; }
            set { _ReducersCount = value > 1000 ? 1000 : (value < 2 ? 2 : value); }
        }

        public int ReducersCount
        {
            get { return _ReducersCount; }
            set { _ReducersCount = value > 1000 ? 1000 : (value < 2 ? 2 : value); }
        }

        public int AnalyzationDepth
        {
            get { return _AnalyzationDepth; }
            set { _AnalyzationDepth = value > 4 ? 4 : (value < 2 ? 2 : value); }
        }

        public AnalyzeMasterConfiguration()
        {
            _DefaultRelationValue = 1;
            _DefaultSelectionCount = 100;
            _MaxSelectionCount = 100;
            _FinalScoreNormalization = true;
            _FinalScoreNormalizationFactor = 0.2;
            _ForgetFactor_TimeAbsolutelyDependent = 0.5;
            _ForgetFactor_TimeHighlyDependent = 0.93;
            _ForgetFactor_TimeLowlyDependent = 0.991;
            _FreshFactor_TimeAbsolutelyDependent = 0.001;
            _FreshFactor_TimeHighlyDependent = 0.0001;
            _FreshFactor_TimeLowlyDependent = 0.00001;
            _LogMode = AnalyzeLogMode.info;
            _LogWrite = false;
            _LogWriteMaxSizeInMB = 10;
            _MappersCount = 4;
            _ReducersCount = 4;
            _AnalyzationDepth = 2;
        }
    }

    public class RaaiVanAnalyzerClientAPI
    {
        protected string Username;
        protected string Password;
        protected string BaseURL;

        protected string _TimeFormat = "yyyy-MM-dd HH:mm:ss";

        protected string Ticket;
        protected bool TicketError;
        protected string AuthResponse;
        protected bool _AuthenticationInProgress;

        protected string http_post_request(string url, NameValueCollection values)
        {
            values["format"] = "json";
            values["input_format"] = "json";
            values["id_string_or_int"] = "string";
            values["time_stamp"] = DateTime.Now.Millisecond.ToString();
            if (!string.IsNullOrEmpty(Ticket)) values["ticket"] = Ticket;

            try { return Encoding.Default.GetString((new WebClient()).UploadValues(url, values)); }
            catch { return string.Empty; }
        }

        public RaaiVanAnalyzerClientAPI()
        {
            TicketError = _AuthenticationInProgress = false;
            Username = Password = BaseURL = Ticket = AuthResponse = string.Empty;
        }

        public RaaiVanAnalyzerClientAPI(string username, string password, string baseUrl)
        {
            TicketError = _AuthenticationInProgress = false;
            initialize(username, password, baseUrl);
        }

        public void initialize(string username, string password, string baseUrl)
        {
            Username = username;
            Password = password;
            BaseURL = baseUrl;
        }

        public static Dictionary<string, string> json2dictionary(string json)
        {
            if (string.IsNullOrEmpty(json)) return new Dictionary<string, string>();
            try { return new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json); }
            catch { return new Dictionary<string, string>(); }
        }

        protected void _authenticate()
        {
            if (_AuthenticationInProgress)
            {
                while (true)
                {
                    Thread.Sleep(50);
                    if (!_AuthenticationInProgress) break;
                }
            }
            else if (TicketError) return;

            NameValueCollection values = new NameValueCollection();

            values["username"] = Username;
            values["password"] = Password;

            Dictionary<string, string> data =
                json2dictionary((AuthResponse = http_post_request(BaseURL + "/auth", values)));

            if (data.ContainsKey("title") && data["title"] == "ticket" && data.ContainsKey("message"))
                Ticket = data["message"];
            else TicketError = true;

            _AuthenticationInProgress = false;
        }

        public string InsertEntities(IList<AnalyzeEntity> entities, int age = 0)
        {
            _authenticate();

            if (age < 0) age = 0;

            NameValueCollection values = new NameValueCollection();

            string input_data = "{\"age\":" + age.ToString() + ",\"entities\":[";

            bool isFirst = true;
            foreach (AnalyzeEntity itm in entities)
            {
                input_data += (isFirst ? string.Empty : ",") +
                    "{\"id\":\"" + itm.ID + "\",\"type\":\"" + itm.TypeID.ToString() + "\"" +
                    (!itm.Time.HasValue ? string.Empty : ",\"timestamp\":\"" + itm.Time.Value.ToString(_TimeFormat) + "\"") +
                    "}";
                isFirst = false;
            }
            input_data += "]}";

            values["command_type"] = "insert_entities";
            values["input_data"] = input_data;

            return http_post_request(BaseURL + "/command", values);
        }

        public string DeleteEntities(IList<AnalyzeEntity> entities, int age = 0)
        {
            _authenticate();

            if (age < 0) age = 0;

            NameValueCollection values = new NameValueCollection();

            string input_data = "{\"age\":" + age.ToString() + ",\"entities\":[";

            bool isFirst = true;
            foreach (AnalyzeEntity itm in entities)
            {
                input_data += (isFirst ? string.Empty : ",") +
                    "{\"id\":\"" + itm.ID + "\",\"type\":\"" + itm.TypeID.ToString() + "\"" +
                    (!itm.Time.HasValue ? string.Empty : ",\"timestamp\":\"" + itm.Time.Value.ToString(_TimeFormat) + "\"") +
                    "}";
                isFirst = false;
            }
            input_data += "]}";

            values["command_type"] = "delete_entities";
            values["input_data"] = input_data;

            return http_post_request(BaseURL + "/command", values);
        }

        public string InsertRelations(IList<AnalyzeRelation> relations, int age = 0)
        {
            _authenticate();

            if (age < 0) age = 0;

            NameValueCollection values = new NameValueCollection();

            string input_data = "{\"age\":" + age.ToString() + ",\"relations\":[";

            bool isFirst = true;
            foreach (AnalyzeRelation itm in relations)
            {
                input_data += (isFirst ? string.Empty : ",") +
                    "{\"id_rel\":\"" + itm.ID + "\",\"type_rel\":\"" + itm.TypeID.ToString() + "\"" +
                    ",\"id_1\":\"" + itm.SourceID + "\",\"type_1\":\"" + itm.SourceTypeID.ToString() + "\"" +
                    ",\"id_2\":\"" + itm.DestinationID + "\",\"type_2\":\"" + itm.DestinationTypeID.ToString() + "\"" +
                    (!itm.Time.HasValue ? string.Empty : ",\"timestamp\":\"" + itm.Time.Value.ToString(_TimeFormat) + "\"") +
                    ",\"two_way\":" + itm.Bidirectional.ToString().ToLower() +
                    "}";
                isFirst = false;
            }
            input_data += "]}";

            values["command_type"] = "insert_relations";
            values["input_data"] = input_data;

            return http_post_request(BaseURL + "/command", values);
        }

        public string DeleteRelations(IList<AnalyzeRelation> relations, int age = 0)
        {
            _authenticate();

            if (age < 0) age = 0;

            NameValueCollection values = new NameValueCollection();

            string input_data = "{\"age\":" + age.ToString() + ",\"relations\":[";

            bool isFirst = true;
            foreach (AnalyzeRelation itm in relations)
            {
                input_data += (isFirst ? string.Empty : ",") +
                    "{\"id_rel\":\"" + itm.ID + "\",\"type_rel\":\"" + itm.TypeID.ToString() + "\"" +
                    ",\"id_1\":\"" + itm.SourceID + "\",\"type_1\":\"" + itm.SourceTypeID.ToString() + "\"" +
                    ",\"id_2\":\"" + itm.DestinationID + "\",\"type_2\":\"" + itm.DestinationTypeID.ToString() + "\"" +
                    (!itm.Time.HasValue ? string.Empty : ",\"timestamp\":\"" + itm.Time.Value.ToString(_TimeFormat) + "\"") +
                    ",\"two_way\":" + itm.Bidirectional.ToString().ToLower() +
                    "}";
                isFirst = false;
            }
            input_data += "]}";

            values["command_type"] = "delete_relations";
            values["input_data"] = input_data;

            return http_post_request(BaseURL + "/command", values);
        }

        public string DeleteAll(int age = 1)
        {
            _authenticate();

            if (age <= 0) age = 1;

            NameValueCollection values = new NameValueCollection();

            values["command_type"] = "delete_all";
            values["before_age"] = age.ToString();

            return http_post_request(BaseURL + "/command", values);
        }

        public string MasterConfiguration(AnalyzeMasterConfiguration config, string username, string password)
        {
            NameValueCollection values = new NameValueCollection();

            string input_data = "{\"items\":{" +
                "\"default_link_value\":\"" + config.DefaultRelationValue.ToString() + "\"" +
                ",\"default_selection_count\":\"" + config.DefaultSelectionCount.ToString() + "\"" +
                ",\"final_score_do_normalization\":\"" + config.FinalScoreNormalization.ToString().ToLower() + "\"" +
                ",\"final_score_normalization_factor\":\"" + config.FinalScoreNormalizationFactor.ToString() + "\"" +
                ",\"forgetfulness_factor_time_full_dependent\":\"" + config.ForgetFactor_TimeAbsolutelyDependent.ToString() + "\"" +
                ",\"forgetfulness_factor_time_high_dependent\":\"" + config.ForgetFactor_TimeHighlyDependent.ToString() + "\"" +
                ",\"forgetfulness_factor_time_low_dependent\":\"" + config.ForgetFactor_TimeLowlyDependent.ToString() + "\"" +
                ",\"fresh_factor_time_full_dependent\":\"" + config.FreshFactor_TimeAbsolutelyDependent.ToString() + "\"" +
                ",\"fresh_factor_time_high_dependent\":\"" + config.FreshFactor_TimeHighlyDependent.ToString() + "\"" +
                ",\"fresh_factor_time_low_dependent\":\"" + config.FreshFactor_TimeLowlyDependent.ToString() + "\"" +
                ",\"log_mode\":\"" + config.LogMode.ToString() + "\"" +
                ",\"log_write\":\"" + config.LogWrite.ToString().ToLower() + "\"" +
                ",\"log_write_max_size\":\"" + config.LogWriteMaxSizeInMB.ToString() + "MB\"" +
                ",\"max_selection_count\":\"" + config.MaxSelectionCount.ToString() + "\"" +
                ",\"num_mappers\":\"" + config.MappersCount.ToString() + "\"" +
                ",\"num_reducers\":\"" + config.ReducersCount.ToString() + "\"" +
                ",\"tree_creation_depth\":\"" + config.AnalyzationDepth.ToString() + "\"" +
                "}}";

            values["command_type"] = "main_configuration";
            values["username"] = username;
            values["password"] = password;
            values["input_data"] = input_data;

            return http_post_request(BaseURL + "/command", values);
        }

        public string AnalyzeConfiguration(List<AnalyzeContext> contexts)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            string inputData = "{\"contexts\":[";

            bool isFirst = true;
            foreach (AnalyzeContext ctx in contexts)
            {
                string relationValues = string.Empty;
                string selectionCounts = string.Empty;
                string recommends = string.Empty;
                string targetRelationsOptions = string.Empty;

                foreach (AnalyzeConfigRelation rel in ctx.Relations)
                {
                    relationValues += (string.IsNullOrEmpty(relationValues) ? string.Empty : ",") +
                        "{\"relation_type\":" + rel.TypeID.ToString() + ",\"value\":" + rel.Value.ToString() + "}";
                }

                foreach (AnalyzeConfigSelectionCount slcnt in ctx.SelectionCounts)
                {
                    selectionCounts += (string.IsNullOrEmpty(selectionCounts) ? string.Empty : ",") +
                        "{\"node_type\":" + slcnt.EntityTypeID.ToString() +
                        ",\"selection_count\":" + slcnt.SelectionCount.ToString() + "}";
                }

                foreach (AnalyzeConfigLimitFilterItem itm in ctx.Recommends)
                {
                    recommends += (string.IsNullOrEmpty(recommends) ? string.Empty : ",") +
                        "{\"node_type\":" + itm.EntityTypeID.ToString() +
                        ",\"recommended_type\":" + itm.RecommendedEntityTypeID.ToString() + "}";
                }

                if (ctx.TargetRelationTypeID > 0 && ctx.TargetRelationDependentTypeIDs.Count > 0)
                {
                    foreach (int id in ctx.TargetRelationDependentTypeIDs)
                    {
                        targetRelationsOptions += (string.IsNullOrEmpty(targetRelationsOptions) ? string.Empty : ",") +
                            "{\"implicant_type\":" + id.ToString() +
                            ",\"implied_type\":" + ctx.TargetRelationTypeID.ToString() + "}";
                    }
                }

                inputData += (isFirst ? string.Empty : ",") + "{\"context_value\":" + ctx.ID.ToString() +
                    ",\"relation_values\":[" + relationValues + "]" +
                    ",\"selection_counts\":[" + selectionCounts + "]" +
                    ",\"filter_recom\":{\"pass_by_default\":" + ctx.ConsiderRecommendsAsFilter.ToString().ToLower() +
                        ",\"allow_list\":[" + (ctx.ConsiderRecommendsAsFilter ? string.Empty : recommends) + "]" +
                        ",\"disallow_list\":[" + (ctx.ConsiderRecommendsAsFilter ? recommends : string.Empty) + "]" +
                    "}" +
                    ",\"vv_implications\":[" + targetRelationsOptions + "]" +
                    ",\"time_importance_type\":" + ((int)ctx.TimeDependence).ToString() +
                    ",\"measuring_params\":{" +
                        "\"link_value_time_value_factor\":" + ctx.AnalyzeRelationTimeFactor.ToString() +
                        ",\"link_value_link_type_value_factor\":" + ctx.AnalyzeRelationTypeFactor.ToString() +
                        ",\"link_value_count_value_factor\":" + ctx.AnalyzeRelationCountFactor.ToString() +
                        ",\"compute_path_value_type\":" + ((int)ctx.MergeMode).ToString() +
                        ",\"final_score_normalization_mergecount_factor\":" +
                            ctx.FinalScoreNormalizationMergeCountFactor.ToString() +
                        ",\"final_score_normalization_mergevalue_factor\":" +
                            ctx.FinalScoreNormalizationMergeValueFactor.ToString() +
                        ",\"finel_score_devide_by_path_length\":" +
                            ctx.FinalScoreDivideByPathLenght.ToString().ToLower() +
                    "}" +
                    "}";
                isFirst = false;
            }

            inputData += "]}";

            values["command_type"] = "analyze_configuration";
            values["input_data"] = inputData;

            return http_post_request(BaseURL + "/command", values);
        }

        public string AnalyzeConfiguration(AnalyzeContext context)
        {
            return AnalyzeConfiguration(new List<AnalyzeContext>() { context });
        }

        public string GetTopIndirectRelations(string nodeId, int nodeTypeId, int recommendedTypeId = 0,
            int contextId = 0, int count = 0, int lowerBoundary = 0)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            values["query_type"] = "recom";
            values["node_id"] = nodeId;
            values["node_type"] = nodeTypeId.ToString();
            if (recommendedTypeId > 0) values["recommended_type"] = recommendedTypeId.ToString();
            if (contextId > 0) values["context"] = contextId.ToString();
            if (lowerBoundary > 0) values["start"] = lowerBoundary.ToString();
            if (count > 0) values["num"] = count.ToString();

            return http_post_request(BaseURL + "/query", values);
        }

        public string GetTopDirectRelations(string nodeId, int nodeTypeId, int recommendedTypeId = 0,
            int contextId = 0, int count = 0, int lowerBoundary = 0)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            values["query_type"] = "direct";
            values["node_id"] = nodeId;
            values["node_type"] = nodeTypeId.ToString();
            if (recommendedTypeId > 0) values["recommended_type"] = recommendedTypeId.ToString();
            if (contextId > 0) values["context"] = contextId.ToString();
            if (lowerBoundary > 0) values["start"] = lowerBoundary.ToString();
            if (count > 0) values["num"] = count.ToString();

            return http_post_request(BaseURL + "/query", values);
        }

        public string GetTopRelations(string nodeId, int nodeTypeId, int recommendedTypeId = 0,
            int contextId = 0, int count = 0, int lowerBoundary = 0)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            values["query_type"] = "both";
            values["node_id"] = nodeId;
            values["node_type"] = nodeTypeId.ToString();
            if (recommendedTypeId > 0) values["recommended_type"] = recommendedTypeId.ToString();
            if (contextId > 0) values["context"] = contextId.ToString();
            if (lowerBoundary > 0) values["start"] = lowerBoundary.ToString();
            if (count > 0) values["num"] = count.ToString();

            return http_post_request(BaseURL + "/query", values);
        }

        public string GetNodesWithMostRecommendation(int nodeTypeId = 0,
            int contextId = 0, int count = 0, int lowerBoundary = 0)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            values["query_type"] = "has_recom";
            if (nodeTypeId > 0) values["node_type"] = nodeTypeId.ToString();
            if (contextId > 0) values["context"] = contextId.ToString();
            if (lowerBoundary > 0) values["start"] = lowerBoundary.ToString();
            if (count > 0) values["num"] = count.ToString();

            return http_post_request(BaseURL + "/query", values);
        }

        public string GetMostRecommendedNodes(int recommendedTypeId = 0,
            int contextId = 0, int count = 0, int lowerBoundary = 0)
        {
            _authenticate();

            NameValueCollection values = new NameValueCollection();

            values["query_type"] = "most_recom";
            if (recommendedTypeId > 0) values["recommended_type"] = recommendedTypeId.ToString();
            if (contextId > 0) values["context"] = contextId.ToString();
            if (lowerBoundary > 0) values["start"] = lowerBoundary.ToString();
            if (count > 0) values["num"] = count.ToString();

            return http_post_request(BaseURL + "/query", values);
        }
    }
}
