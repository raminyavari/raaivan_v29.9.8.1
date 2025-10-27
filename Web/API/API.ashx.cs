using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.Jobs;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Search;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for API
    /// </summary>
    public class API : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer = null;

        //to be removed
        private string getFileContent(string fileName)
        {
            return System.IO.File.ReadAllText(PublicMethods.map_path("~/example/" + fileName));
        }

        private byte[] topdf(string size, bool hasCover, bool hasHeader, bool hasFooter, 
            string margin, string pageNumberPosition, string pdfPassword)
        {
            string url = "http://localhost:8080/html2pdf",
                username = "user",
                password = "pass",
                html = getFileContent("main.html"),
                css = getFileContent("style.css"),
                header = !hasHeader ? null : getFileContent("header.html"),
                footer = !hasFooter ? null : getFileContent("footer.html"),
                cover = !hasCover? null : getFileContent("cover.html");

            Dictionary<string, string> values = new Dictionary<string, string>();

            values.Add("bodyUrl", null);
            values.Add("bodyHtml", html);
            values.Add("bodyStyle", css);
            values.Add("header", header);
            values.Add("footer", footer);
            values.Add("cover", cover);
            values.Add("paperSize", string.IsNullOrEmpty(size) ? "A4" : size);
            values.Add("margin", string.IsNullOrEmpty(margin) ? "5% 5% 5% 5%" : margin);
            values.Add("fontFamilyRTL", null);
            values.Add("fontFamilyLTR", null);
            values.Add("fontSizeRTL", null);
            values.Add("fontSizeLTR", null);
            values.Add("defaultDirection", null);
            values.Add("pageNumberPosition", pageNumberPosition);
            values.Add("password", pdfPassword);

            /*
            System.Net.WebClient client = new System.Net.WebClient();

            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
            client.Headers[System.Net.HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

            return client.UploadValues(url, "POST", values);
            */

            System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
            httpWebRequest.Headers[System.Net.HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
                streamWriter.Write(PublicMethods.toJSON(values));

            System.Net.HttpWebResponse httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();

            byte[] buffer = new byte[16 * 1024];

            using (System.IO.Stream stream = httpResponse.GetResponseStream())
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        private void processPDF2HTML(ParamsContainer paramsContainer, string mode)
        {
            switch (mode)
            {
                case "1":
                    paramsContainer.file_response(topdf(
                            size: "A5",
                            hasCover: true,
                            hasHeader: false,
                            hasFooter: true,
                            margin: "4% 3% 4% 3%",
                            pageNumberPosition: "top-center",
                            pdfPassword: null),
                            "output1.pdf", "application/pdf", isAttachment: true);
                    return;
                case "2":
                    paramsContainer.file_response(topdf(
                            size: "Letter",
                            hasCover: false,
                            hasHeader: true,
                            hasFooter: false,
                            margin: "4% 3.5% 7% 6%",
                            pageNumberPosition: "bottom-center",
                            pdfPassword: null),
                            "output2.pdf", "application/pdf", isAttachment: true);
                    return;
                case "3":
                    paramsContainer.file_response(topdf(
                            size: "A3",
                            hasCover: false,
                            hasHeader: false,
                            hasFooter: false,
                            margin: "1% 2% 3% 4%",
                            pageNumberPosition: null,
                            pdfPassword: null),
                            "output3.pdf", "application/pdf", isAttachment: true);
                    return;
                case "4":
                    paramsContainer.file_response(topdf(
                            size: "A5",
                            hasCover: true,
                            hasHeader: true,
                            hasFooter: true,
                            margin: "15% 1% 5% 2%",
                            pageNumberPosition: null,
                            pdfPassword: "1234"),
                            "output4.pdf", "application/pdf", isAttachment: true);
                    return;
                default:
                    paramsContainer.file_response(topdf(
                        size: "A4",
                        hasCover: true,
                        hasHeader: true,
                        hasFooter: true,
                        margin: "5% 5% 5% 5%",
                        pageNumberPosition: null,
                        pdfPassword: null),
                        "output.pdf", "application/pdf", isAttachment: true);
                    return;
            }
        }
        //end of to be removed

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: false);

            /*
            if (PublicMethods.parse_string(context.Request.Params["command"], false) == "abc123")
            {
                processPDF2HTML(paramsContainer, PublicMethods.parse_string(context.Request.Params["mode"], false));
                return;
            }
            */

            if (DevelopmentSpecificRequest(context)) return;
            else if (ProcessTenantIndependentRequest(context)) return;
            
            if (!paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return;
            }
            
            string command = PublicMethods.parse_string(context.Request.Params["command"], false);
            if (!string.IsNullOrEmpty(command)) command = command.ToLower();
            
            string responseText = string.Empty;
            
            Guid userId = Guid.Empty;
            
            if (command != "authenticate")
                check_ticket(ref userId, PublicMethods.parse_string(paramsContainer.request_param("ticket"), false));
            
            switch (command)
            {
                case "getdeletedstates":
                    get_deleted_states(PublicMethods.parse_int(context.Request.Params["count"]),
                        PublicMethods.parse_long(context.Request.Params["startfrom"]), ref responseText);
                    return_response(responseText);
                    break;
                case "getform":
                case "get_form":
                    get_form(PublicMethods.parse_guid(context.Request.Params["FormID"]), ref responseText);
                    return_response(responseText);
                    break;
                case "searchforminstances":
                case "search_form_instances":
                    search_form_instances(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        FGUtilities.get_filters_from_json(context.Request.Params["Filters"]), 
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    return_response(responseText);
                    break;
                case "saveform":
                case "save_form":
                    save_form(PublicMethods.parse_guid(context.Request.Params["FormID"]),
                        context.Request.Params["InputData"], userId, ref responseText);
                    return_response(responseText);
                    break;
                case "getforminstance":
                case "get_form_instance":
                    get_form_instance(PublicMethods.parse_guid(context.Request.Params["InstanceID"]), ref responseText);
                    return_response(responseText);
                    break;
                case "new_node":
                    new_node(PublicMethods.parse_string(context.Request.Params["TypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Abstract"]),
                        ListMaker.get_string_items(context.Request.Params["Keywords"], ',').Select(u => Base64.decode(u)).ToList(),
                        FGUtilities.get_form_elements(paramsContainer.ApplicationID, context.Request.Params["FormDetails"]),
                        ref responseText);
                    return_response(responseText);
                    break;
                case "get_nodes":
                    get_nodes(ListMaker.get_string_items(context.Request.Params["ID"], ','), 
                        ListMaker.get_string_items(context.Request.Params["TypeID"], ','),
                        PublicMethods.parse_string(context.Request.Params["RelatedToTypeID"], false),
                        ListMaker.get_string_items(context.Request.Params["RelatedToID"], '|'),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_date(context.Request.Params["CreationDateFrom"]),
                        PublicMethods.parse_date(context.Request.Params["CreationDateTo"], 1),
                        PublicMethods.parse_string(context.Request.Params["FormFilters"]),
                        PublicMethods.parse_bool(context.Request.Params["MatchAllFilters"]),
                        PublicMethods.parse_bool(context.Request.Params["FormDetails"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]), ref responseText);
                    return_response(responseText);
                    break;
                default:
                    return_response(CustomAPI.handle_request(paramsContainer.Tenant.Id, userId, command, context.Request));
                    break;
            }
        }

        public bool DevelopmentSpecificRequest(HttpContext context)
        {
            if (!PublicMethods.is_dev()) return false;

            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);
            if (!string.IsNullOrEmpty(command)) command = command.ToLower();

            switch (command) {
                case "abc222":
                    {
                        RabbitMQAPI.initialize();

                        paramsContainer.return_response("result: ok");
                        return true;
                    }
                case "abc223":
                    {
                        RabbitMQAPI.create_consumer();

                        paramsContainer.return_response("result: ok");
                        return true;
                    }
                case "abc123":
                    {
                        DBCompositeType<DocFileInfoTableType> nodeIds = new DBCompositeType<DocFileInfoTableType>()
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null))
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null))
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null))
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null))
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null))
                            .add(new DocFileInfoTableType(Guid.NewGuid(), PublicMethods.random_string(10), "jpg", "", PublicMethods.get_random_number(), null, null));

                        DBResultSet results = DBConnector.read_postgre(paramsContainer.ApplicationID,
                            "func_test_3", 22, nodeIds);

                        /*
                        DBCompositeType<GuidTableType> values = new DBCompositeType<GuidTableType>()
                            .add(new GuidTableType() { Value = Guid.NewGuid() })
                            .add(new GuidTableType() { Value = Guid.NewGuid() });

                        DBResultSet results = DBConnector.read("func_test_3", 1, values);
                        */

                        paramsContainer.return_response("result: ok " + results.ToString());
                        return true;
                    }
                case "abc444":
                    {
                        int? val = null;
                        DBResultSet results = DBConnector.read_postgre(paramsContainer.ApplicationID, "test", val);
                        paramsContainer.return_response("result: ok " + results.ToString());
                        return true;
                    }
                case "abc1234":
                    {
                        DBResultSet results = DBConnector.read(paramsContainer.ApplicationID, "func_test_2",
                            (int)1, //int
                            (long)1, //bigint
                            (double)1.1, //float
                            true, //boolean
                            'a', //char
                            DateTime.Now, //timestamp
                            "ramin", //varchar
                            "ramin", //text
                            Guid.NewGuid(), //uuid
                            new byte[10]); //bytea

                        paramsContainer.return_response("result: ok " + results.ToString());
                        return true;
                    }
                case "pg1":
                case "pg2":
                case "pg3":
                    {
                        string funcName = command == "pg1" ? "func_test_4" :
                            (command == "pg2" ? "func_test_5" : "func_test_6");

                        RVDataTable tbl = DBConnector.read_postgre(paramsContainer.ApplicationID, funcName).get_table();

                        ArrayList retList = new ArrayList();
                        Dictionary<string, object> results = new Dictionary<string, object>() { { "result", retList } };

                        for (int i = 0; i < tbl.Rows.Count; i++)
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            tbl.ColumnNames.ForEach(col => row[col] = tbl.GetValue(i, col));
                            retList.Add(row);
                        }

                        paramsContainer.return_response(PublicMethods.toJSON(results));
                        return true;
                    }
                case "sql_scripts":
                    {
                        string resFileName = "Script (v28.5.2.1+ to " + PublicMethods.SystemVersion + ") - " +
                            GenericDate.get_local_date(DateTime.Now).Replace("/", ".") + ".sql";

                        string fileName = PublicMethods.parse_string(paramsContainer.request_param("FileName"), 
                            decode: false, defaultValue: "sql_map.js");

                        string fileContent = "USE [EKM_App]" + "\r\n" + "GO" + "\r\n\r\n" +
                            new GenerateScriptFile(fileName).get();

                        paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(fileContent),
                            fileName: resFileName, contentType: "application/sql", isAttachment: true);
                        return true;
                    }
                case "mssql2postgresql":
                    {
                        string fileName = PublicMethods.parse_string(paramsContainer.request_param("FileName"),
                            decode: false, defaultValue: "sql_map_convert.js");
                        string scriptContent = MSSQL2PostgreSQL.convert_mssql_to_postgresql(new GenerateScriptFile(fileName).get());
                        paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(scriptContent),
                            fileName: "mssql2postgresql_scripts.sql", contentType: "application/sql", isAttachment: true);
                        return true;
                    }
                case "postgresql_scripts":
                    {
                        string fileName = PublicMethods.parse_string(paramsContainer.request_param("FileName"),
                            decode: false, defaultValue: "sql_map_postgre.js");
                        paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(new GenerateScriptFile(fileName).get()),
                            fileName: "postgresql_scripts.sql", contentType: "application/sql", isAttachment: true);
                        return true;
                    }
                case "postgresql_schema":
                    {
                        bool? withPartitioning = PublicMethods.parse_bool(context.Request.Params["WithPartitioning"]);
                        
                        paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(
                            SchemaInfo.toScript(GlobalController.get_schema_info(), withPartitioning)),
                            fileName: "postgre_schema.sql", contentType: "application/sql", isAttachment: true);

                        return true;
                    }
                case "postgresql_transfer_data":
                    PostgreSQLConnector.config(
                        host: PublicMethods.parse_string(paramsContainer.request_param("host"), decode: false),
                        dbName: PublicMethods.parse_string(paramsContainer.request_param("dbname"), decode: false),
                        username: PublicMethods.parse_string(paramsContainer.request_param("username"), decode: false),
                        password: PublicMethods.parse_string(paramsContainer.request_param("password"), decode: false));

                    Dictionary<string, object> result = MSSQL2PostgreSQL.transfer_data(GlobalController.get_schema_info());
                    paramsContainer.return_response(PublicMethods.toJSON(result));
                    return true;
                case "postgresql_foreign_keys":
                    paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(ForeignKey.toScript(GlobalController.get_foreign_keys())),
                    fileName: "postgre_foreign_keys.sql", contentType: "application/sql", isAttachment: true);
                    return true;
                case "postgresql_indexes":
                    paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(DBIndex.toScript(GlobalController.get_indexes())),
                    fileName: "postgre_indexes.sql", contentType: "application/sql", isAttachment: true);
                    return true;
                case "postgresql_user_defined_types":
                    paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(
                        SchemaInfo.toScript_UserDefinedTableTypes(GlobalController.get_user_defined_table_types())),
                    fileName: "postgre_user_defined_types.sql", contentType: "application/sql", isAttachment: true);
                    return true;
                case "postgresql_full_text_indexes":
                    paramsContainer.file_response(System.Text.Encoding.UTF8.GetBytes(
                        SchemaInfo.toScript_FullTextIndexes(GlobalController.get_full_text_indexes())),
                    fileName: "postgre_full_text_indexes.sql", contentType: "application/sql", isAttachment: true);
                    return true;
            }

            return false;
        }

        public bool ProcessTenantIndependentRequest(HttpContext context)
        {
            if (!RaaiVanSettings.SAASBasedMultiTenancy && !paramsContainer.ApplicationID.HasValue)
            {
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
                return true;
            }

            string responseText = string.Empty;

            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);
            if (!string.IsNullOrEmpty(command)) command = command.ToLower();

            Guid userId = Guid.Empty;

            if (command != "authenticate")
                check_ticket(ref userId, PublicMethods.parse_string(paramsContainer.request_param("ticket"), false));

            switch (command)
            {
                case "authenticate":
                    authenticate(PublicMethods.parse_string(paramsContainer.request_param("username"), false),
                        PublicMethods.parse_string(paramsContainer.request_param("password"), false), ref responseText);
                    break;
                case "get_all_applications":
                    get_all_applications(PublicMethods.parse_int(paramsContainer.request_param("Count")),
                        PublicMethods.parse_int(paramsContainer.request_param("LowerBoundary")),
                        ref responseText);
                    break;
                case "run_job":
                    run_job(PublicMethods.parse_guid(paramsContainer.request_param("ApplicationID")),
                        PublicMethods.fromJSON(PublicMethods.parse_string(paramsContainer.request_param("Options"), decode: false)),
                        ref responseText);
                    break;
            }

            if (!string.IsNullOrEmpty(responseText))
                paramsContainer.return_response(ref responseText);

            return !string.IsNullOrEmpty(responseText);
        }

        protected void return_response(string responseText)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.BufferOutput = true;
            HttpContext.Current.Response.Write(responseText);
            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Close();
        }

        protected void check_ticket(ref Guid userId, string ticket)
        {
            Guid? uId = RestAPI.get_user_id(ticket);

            if (!uId.HasValue) return_response(PublicConsts.InvalidTicketResponse);
            else userId = uId.Value;
        }

        protected void authenticate(string username, string password, ref string responseText)
        {
            string failureText = string.Empty;
            int remainingLockoutTime = 0;

            bool loggedInWithActiveDirectory = false;

            bool success = new LoginUtil(paramsContainer: paramsContainer, invitationId: null)
                .authenticate_user(username, password, activeDirDomain: null, hasValidCaptcha: false,
                ref loggedInWithActiveDirectory, ref failureText, ref remainingLockoutTime);
            
            if (success)
            {
                Guid? userId = UsersController.get_user_id(paramsContainer.ApplicationID, username);
                string ticket = !userId.HasValue ? null : RestAPI.get_ticket(userId.Value);

                if (string.IsNullOrEmpty(ticket) && userId.HasValue)
                {
                    ticket = PublicMethods.random_string();
                    RestAPI.new_ticket(ticket, userId.Value);
                }
                
                responseText = "{\"Result\":\"ok\"" + 
                    ",\"Ticket\":\"" + ticket + "\"" + 
                    ",\"AccessToken\":\"" + RestAPI.new_token(ticket) + "\"" +
                    "}";
            }
            else
                responseText = "{\"Result\":\"nok\"}";
        }

        protected string _get_deleted_state_json(string id, string type, DateTime? time, bool? deleted, bool? isRelation, 
            Guid? relSourceId, string relSourceType, Guid? relDestinationId, string relDestinationType, 
            bool bidirectional, bool hasReverse)
        {
            if (!isRelation.HasValue || !relSourceId.HasValue || !relDestinationId.HasValue) isRelation = false;
            if (!time.HasValue) time = new DateTime(2011, 6, 15);

            string json = "{\"ID\":\"" + id + "\"" +
                ",\"Type\":\"" + type + "\"" +
                ",\"Time\":\"" + (!time.HasValue ? string.Empty : time.ToString())  + "\"" +
                ",\"Deleted\":" + (deleted.HasValue && deleted.Value).ToString().ToLower() +
                ",\"IsRelation\":" + isRelation.Value.ToString().ToLower() +
                ",\"RelSourceID\":\"" + (isRelation.Value ? relSourceId.ToString() : string.Empty) + "\"" +
                ",\"RelDestinationID\":\"" + (isRelation.Value ? relDestinationId.ToString() : string.Empty) + "\"" +
                ",\"RelSourceType\":\"" + (isRelation.Value ? relSourceType : string.Empty) + "\"" +
                ",\"RelDestinationType\":\"" + (isRelation.Value ? relDestinationType : string.Empty) + "\"" +
                ",\"Bidirectional\":" + (isRelation.Value ? bidirectional : false).ToString().ToLower() +
                ",\"HasReverse\":" + (isRelation.Value ? hasReverse : false).ToString().ToLower() +
                "}";
            
            return json;
        }

        protected void get_deleted_states(int? count, long? lowerBoundary, ref string responseText)
        {
            List<DeletedState> deletedStates = 
                GlobalController.get_deleted_states(paramsContainer.Tenant.Id, count, lowerBoundary);
            
            responseText = "{\"FirstID\":" + (deletedStates.Count > 0 ? deletedStates.First().ID.Value : 0).ToString() +
                ",\"LastID\":" + (deletedStates.Count > 0 ? deletedStates.Last().ID.Value : 0).ToString() +
                ",\"Items\":[";

            List<Guid> nodeIds = new List<Guid>();

            bool isFirst = true;
            foreach(DeletedState ds in deletedStates)
            {
                if (ds.ObjectType == "EmailAddress") continue;

                if (ds.ObjectType == "Node") nodeIds.Add(ds.ObjectID.Value);

                bool isRelation = ds.RelSourceID.HasValue && ds.RelDestinationID.HasValue;

                responseText += (isFirst ? string.Empty : ",") +
                    _get_deleted_state_json(ds.ObjectID.Value.ToString(), ds.ObjectType, ds.Date, ds.Deleted, isRelation,
                    ds.RelSourceID, ds.RelSourceType, ds.RelDestinationID, ds.RelDestinationType, 
                    ds.Bidirectional.HasValue && ds.Bidirectional.Value, ds.HasReverse.HasValue && ds.HasReverse.Value);

                if(ds.ObjectType == "TaggedItem" && isRelation && ds.RelCreatorID.HasValue)
                {
                    _get_deleted_state_json(Guid.NewGuid().ToString(), "UsedAsTag", ds.Date, false, true,
                    ds.RelCreatorID, "User", ds.RelDestinationID, ds.RelDestinationType, false, true);
                }
                
                isFirst = false;
            }

            if(nodeIds.Count > 0)
            {
                List<Modules.CoreNetwork.Node> nodes = 
                    CNController.get_nodes(paramsContainer.Tenant.Id, nodeIds, full: null, currentUserId: null);

                foreach(Modules.CoreNetwork.Node nd in nodes)
                {
                    responseText += (isFirst ? string.Empty : ",") +
                        _get_deleted_state_json(Guid.NewGuid().ToString(), "IsA", nd.CreationDate, false, true,
                        nd.NodeID.Value, "Node", nd.NodeTypeID.Value, "NodeType", false, true);

                    isFirst = false;
                }
            } //end of 'if(nodeIds.Count > 0)'
            
            responseText += "]}";
        }

        protected void get_form(Guid? formId, ref string responseText)
        {
            if (!formId.HasValue)
            {
                responseText = "{\"Result\":\"nok\"}";
                return;
            }
            
            List<FormElement> elements = FGController.get_form_elements(paramsContainer.Tenant.Id, formId.Value)
                .OrderBy(v => v.SequenceNumber).ToList();

            responseText = "{\"Elements\":[" + string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void get_form_instance(Guid? instanceId, ref string responseText)
        {
            if (!instanceId.HasValue)
            {
                responseText = "{\"Result\":\"nok\"}";
                return;
            }
            
            List<FormElement> elements = FGController.get_form_instance_elements(
                paramsContainer.Tenant.Id, instanceId.Value).OrderBy(v => v.SequenceNumber).ToList();

            responseText = "{\"Elements\":[" + string.Join(",", elements.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]}";
        }

        protected void search_form_instances(Guid? formId, List<FormFilter> filters, 
            int? lowerBoundary, int? count, ref string responseText)
        {
            if (!formId.HasValue) return;
            
            List<FormRecord> records =
                FGController.get_form_records(paramsContainer.Tenant.Id, formId.Value, filters, lowerBoundary, count);
            
            responseText = "{\"Records\":[" +
                ProviderUtil.list_to_string<string>(records.Select(
                    u => "{\"InstanceID\":\"" + u.InstanceID.ToString() + "\"" +
                        ",\"Cells\":[" + ProviderUtil.list_to_string<string>(u.Cells.Select(
                            c => "{\"ElementID\":\"" + c.ElementID.ToString() + "\"" +
                                ",\"Value\":\"" + Base64.encode(c.Value) + "\"}"
                            ).ToList()) + 
                        "]}"
                    ).ToList()) + 
                "]}";
        }
        
        protected void save_form(Guid? formId, string formData, Guid currentUserId, ref string responseText)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            List<FormElement> formElements = new List<FormElement>();

            if(!formId.HasValue || 
                (formElements = FGController.get_form_elements(paramsContainer.Tenant.Id, formId.Value)).Count == 0)
            {
                res["Result"] = "nok";
                res["Message"] = "no form element found";

                responseText = PublicMethods.toJSON(res);
                return;
            }

            Dictionary<string, object> dataDic = PublicMethods.fromJSON(formData);

            ArrayList elements = PublicMethods.get_value<ArrayList>(dataDic, "Elements");
            Guid? instanceId = PublicMethods.get_value<Guid?>(dataDic, "InstanceID", typeof(Guid));

            if (instanceId == Guid.Empty) instanceId = null;

            if (instanceId.HasValue)
            {
                FormType ft = FGController.get_form_instance(paramsContainer.Tenant.Id, instanceId.Value);

                if(ft != null && ft.Filled.HasValue && ft.Filled.Value)
                {
                    res["Result"] = "nok";
                    res["Message"] = "this form has already been filled";

                    responseText = PublicMethods.toJSON(res);
                    return;
                }
            }

            List<FormElement> toBeSaved = new List<FormElement>();

            for (int i = 0; elements != null && i < elements.Count; ++i)
            {
                Dictionary<string, object> elem = (Dictionary<string, object>)elements[i];

                Guid? elementId = PublicMethods.get_value<Guid?>(elem, "ElementID", typeof(Guid));
                string value = PublicMethods.get_value<string>(elem, "Value", typeof(string));
                
                FormElement refElement = formElements.Where(u => u.ElementID == elementId).FirstOrDefault();

                if (refElement == null) continue;
                
                refElement.RefElementID = refElement.ElementID;

                if (refElement.CreationDate.HasValue) refElement.CreationDate = DateTime.Now;
                if (refElement.Creator.UserID.HasValue) refElement.Creator.UserID = currentUserId;

                if (instanceId.HasValue)
                {
                    refElement.FormInstanceID = instanceId;
                    
                    refElement.ElementID = Guid.NewGuid();

                    refElement.LastModificationDate = DateTime.Now;
                    refElement.LastModifierUserID = currentUserId;
                }
                else { refElement.ElementID = Guid.NewGuid(); }

                if (!FGUtilities.set_element_value(value, ref refElement)) continue;
                
                toBeSaved.Add(refElement);
            }

            if(toBeSaved.Count == 0)
            {
                res["Result"] = "nok";
                res["Message"] = "no element found";

                responseText = PublicMethods.toJSON(res);
                return;
            }

            if (!instanceId.HasValue)
            {
                instanceId = Guid.NewGuid();

                if (!FGController.create_form_instance(paramsContainer.Tenant.Id, new FormType()
                {
                    FormID = formId,
                    InstanceID = instanceId,
                    OwnerID = currentUserId,
                    Creator = new User() { UserID = currentUserId },
                    CreationDate = DateTime.Now
                }))
                {
                    res["Result"] = "nok";
                    res["Message"] = "form instance creation failed";

                    responseText = PublicMethods.toJSON(res);
                    return;
                }

                for (int i = 0, lnt = toBeSaved.Count; i < lnt; ++i)
                    toBeSaved[i].FormInstanceID = instanceId;
            }

            bool result = FGController.save_form_instance_elements(paramsContainer.Tenant.Id, 
                toBeSaved, new List<Guid>(), currentUserId);

            res["Result"] = result ? "ok" : "nok";
            res["Message"] = result ? "form saved successfully" : "saving form failed";

            if (result) res["InstanceID"] = instanceId.ToString();

            responseText = PublicMethods.toJSON(res);
        }

        protected void new_node(string typeId, string name, string description, List<string> tags,
            List<FormElement> elements, ref string responseText)
        {
            Guid? nodeTypeId = PublicMethods.parse_guid(typeId);
            if (!nodeTypeId.HasValue) nodeTypeId = CNController.get_node_type_id(paramsContainer.Tenant.Id, typeId);

            if (!nodeTypeId.HasValue || string.IsNullOrEmpty(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            else if (!PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                nodeTypeId.Value, PrivacyObjectType.NodeType, PermissionType.Create)) {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            Guid nodeId = Guid.NewGuid();
            Guid? formInstanceId = null;

            try
            {
                FormType form = CNController.has_extension(paramsContainer.Tenant.Id, nodeTypeId.Value, ExtensionType.Form) ?
                    FGController.get_owner_form(paramsContainer.Tenant.Id, nodeTypeId.Value) : null;

                if (form != null && form.FormID.HasValue)
                {
                    FGAPI fgHandler = new FGAPI() { paramsContainer = this.paramsContainer };

                    if (!fgHandler.create_form_instance(form.FormID.Value, nodeId, null, null, null, ref responseText))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        return;
                    }

                    Dictionary<string, object> formResponse = PublicMethods.fromJSON(responseText);

                    Dictionary<string, object> instanceDic = formResponse.ContainsKey("Instance") ?
                        (Dictionary<string, object>)formResponse["Instance"] : new Dictionary<string, object>();

                    if (instanceDic != null && instanceDic.ContainsKey("InstanceID"))
                        formInstanceId = PublicMethods.parse_guid((string)instanceDic["InstanceID"]);

                    if (!formInstanceId.HasValue)
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        return;
                    }

                    List<FormElement> formElements = 
                        FGController.get_form_elements(paramsContainer.Tenant.Id, form.FormID.Value);

                    List<FormElement> elemsToBeSaved = new List<FormElement>();

                    elements.ForEach(elem =>
                    {
                        FormElement refElem = formElements.Where(u => u.ElementID == elem.ElementID ||
                            u.ElementID == elem.RefElementID || (!string.IsNullOrEmpty(u.Name) &&
                            !string.IsNullOrEmpty(elem.Name) && u.Name.ToLower() == elem.Name.ToLower())).FirstOrDefault();

                        if (refElem == null) return;
                        
                        elem.FormInstanceID = formInstanceId;
                        elem.Type = refElem.Type;

                        if (!elem.ElementID.HasValue) elem.ElementID = Guid.NewGuid();
                        elem.RefElementID = refElem.ElementID;

                        elemsToBeSaved.Add(elem);
                    });
                    
                    if (!fgHandler.save_form_instance_elements(elemsToBeSaved,
                        new List<Guid>(), null, PollOwnerType.None, ref responseText))
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        return;
                    }
                }

                responseText = string.Empty;
                
                (new CNAPI() { paramsContainer = this.paramsContainer })
                    .register_new_node(nodeId, nodeTypeId, null, null, null, name, description, tags,
                        new List<NodeCreator>(), null, null, null, formInstanceId, null, null, null, null, ref responseText);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(paramsContainer.Tenant.Id, null, "API_NewNode", ex, ModuleIdentifier.RV);
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
            }
        }

        protected void get_nodes(List<string> strNodeIds, List<string> typeIds, string relatedToTypeId, List<string> relatedToNodeIds, 
            string searchText, DateTime? creationDateFrom,  DateTime? creationDateTo, string formFilters, bool? matchAllFilters, 
            bool? formDetails, int? count, int? lowerBoundary, ref string responseText)
        {
            if (!count.HasValue) count = 50;
            if (count > 200) count = 200;

            strNodeIds = strNodeIds.Where(u => !string.IsNullOrEmpty(u)).ToList();
            typeIds = typeIds.Where(u => !string.IsNullOrEmpty(u)).ToList();

            List<string> typeAdditionalIds = new List<string>();
            List<Guid> nodeTypeIds = new List<Guid>();

            List<string> nodeAdditionalIds = new List<string>();
            List<Guid> nodeIds = new List<Guid>();


            //Extract NodeTypeIDs
            typeIds.Where(u => !string.IsNullOrEmpty(u)).ToList().ForEach(id => {
                Guid? ntId = PublicMethods.parse_guid(id);

                if (ntId.HasValue) nodeTypeIds.Add(ntId.Value);
                else if (!string.IsNullOrEmpty(id.Trim())) typeAdditionalIds.Add(id);
            });
            
            if (typeAdditionalIds.Count > 0)
                nodeTypeIds.AddRange(CNController.get_node_type_ids(paramsContainer.Tenant.Id, typeAdditionalIds));
            //end of Extract NodeTypeIDs
            

            //Extract NodeIDs
            if (nodeTypeIds.Count <= 1)
            {
                strNodeIds.Where(u => !string.IsNullOrEmpty(u)).ToList().ForEach(id => {
                    Guid? nId = PublicMethods.parse_guid(id);

                    if (nId.HasValue) nodeIds.Add(nId.Value);
                    else if (!string.IsNullOrEmpty(id.Trim()) && nodeTypeIds.Count == 1) nodeAdditionalIds.Add(id);
                });

                if (nodeAdditionalIds.Count > 0) nodeIds.AddRange(CNController.get_node_ids(paramsContainer.Tenant.Id,
                    nodeTypeIds[0], nodeAdditionalIds).Values.ToList());
            }
            //end of Extract NodeIDs


            if (nodeTypeIds.Count == 0 && nodeIds.Count == 0)
            {
                responseText = "{\"Result\":\"nok\",\"Message\":\"InvalidTypeID\"}";
                return;
            }
            else if (strNodeIds.Count > 0 && nodeIds.Count == 0)
            {
                responseText = "{\"Result\":\"nok\",\"Message\":\"InvalidID\"}";
                return;
            }

            //Related to IDs
            List<Guid> relatedToIds = relatedToNodeIds == null ? new List<Guid>() :
                relatedToNodeIds.Select(v => PublicMethods.parse_guid(v)).Where(v => v.HasValue).Select(v => v.Value).ToList();

            if (relatedToIds.Count == 0 && !string.IsNullOrEmpty(relatedToTypeId) && relatedToNodeIds != null && relatedToNodeIds.Count > 0)
                relatedToIds = CNController.get_node_ids(paramsContainer.Tenant.Id, relatedToNodeIds, relatedToTypeId);
            //end of Related to IDs

            long totalCount = 0;
            List<FormFilter> filters = new List<FormFilter>();

            if (!string.IsNullOrEmpty(formFilters) && nodeTypeIds.Count == 1) {
                FormType frm = FGController.get_owner_form(paramsContainer.Tenant.Id, nodeTypeIds[0]);
                if (frm != null && frm.FormID.HasValue) filters = FGUtilities.get_filters_from_json(formFilters,
                    formId: frm.FormID.Value, applicationId: paramsContainer.Tenant.Id);
            }

            List<Node> nodes = nodeIds.Count > 0 ? CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, nodeIds: nodeIds) :
                CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, totalCount: ref totalCount, 
                    nodeTypeIds: nodeTypeIds.Distinct().ToList(), relatedToIds: relatedToIds, searchText: searchText, 
                    lowerCreationDateLimit: creationDateFrom, upperCreationDateLimit: creationDateTo, filters: filters, 
                    matchAllFilters: matchAllFilters, count: count.Value, lowerBoundary: lowerBoundary, checkAccess: true);
            
            List<Guid> downloadAccess = PrivacyController.check_access(paramsContainer.Tenant.Id, null, 
                nodes.Select(u => u.NodeID.Value).ToList(), PrivacyObjectType.Node, PermissionType.Download);

            List<DocFileInfo> files = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                nodes.Select(u => u.NodeID.Value).ToList());

            List<FormType> formInstances = FGController.get_owner_form_instances(paramsContainer.Tenant.Id, 
                nodes.Select(u => u.NodeID.Value).ToList());

            List<FormElement> instanceElements = new List<FormElement>();

            if (formDetails.HasValue && formDetails.Value) {
                instanceElements = FGController.get_form_instance_elements(paramsContainer.Tenant.Id,
                    formInstances.Select(u => u.InstanceID.Value).ToList());

                Dictionary<Guid, List<SelectedGuidItem>> guidItemsDic =
                    FGController.get_selected_guids(paramsContainer.Tenant.Id, instanceElements.Select(e => e.ElementID.Value).ToList());

                instanceElements
                    .Where(e => (e.Type == FormElementTypes.Node || e.Type == FormElementTypes.User) &&
                        guidItemsDic.ContainsKey(e.ElementID.Value)).ToList()
                    .ForEach(e => e.GuidItems = guidItemsDic[e.ElementID.Value]);

                List<Guid> selectedNodeIds = new List<Guid>();
                List<Guid> selectedUserIds = new List<Guid>();

                instanceElements
                    .Where(e => (e.Type == FormElementTypes.Node || e.Type == FormElementTypes.User) && e.GuidItems.Count > 0)
                    .ToList().ForEach(e => {
                        if (e.Type == FormElementTypes.Node) selectedNodeIds.AddRange(e.GuidItems.Select(i => i.ID.Value));
                        else if (e.Type == FormElementTypes.User) selectedUserIds.AddRange(e.GuidItems.Select(i => i.ID.Value));
                    });

                List<Node> selectedNodes = selectedNodeIds.Count == 0 ? new List<Node>() :
                    CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, nodeIds: selectedNodeIds.Distinct().ToList());
                List<User> selectedUsers = selectedUserIds.Count == 0 ? new List<User>() :
                    UsersController.get_users(paramsContainer.Tenant.Id, selectedUserIds.Distinct().ToList());

                instanceElements.Where(e => e.Type == FormElementTypes.Node && e.GuidItems.Count > 0).ToList()
                    .ForEach(e =>
                    {
                        e.GuidItems.ForEach(g => {
                            Node n = selectedNodes.Where(x => x.NodeID == g.ID).FirstOrDefault();

                            g.Type = SelectedGuidItemType.Node;

                            if (n != null) {
                                g.Name = n.Name;
                                g.Code = n.AdditionalID;
                            }
                        });
                    });

                instanceElements.Where(e => e.Type == FormElementTypes.User && e.GuidItems.Count > 0).ToList()
                    .ForEach(e =>
                    {
                        e.GuidItems.ForEach(g => {
                            User u = selectedUsers.Where(x => x.UserID == g.ID).FirstOrDefault();

                            g.Type = SelectedGuidItemType.User;

                            if (u != null)
                            {
                                g.Name = u.FullName;
                                g.Code = u.UserName;
                            }
                        });
                    });
            }

            List<NodeInfo> info = CNController.get_node_info(paramsContainer.Tenant.Id, nodes.Select(n => n.NodeID.Value).ToList(),
                currentUserId: null, description: true, tags: true, creator: false, contributorsCount: false, likesCount: false,
                visitsCount: false, expertsCount: false, membersCount: false, childsCount: false, relatedNodesCount: false, likeStatus: false);

            nodes.ForEach(nd => {
                NodeInfo i = info.Where(x => x.NodeID == nd.NodeID).FirstOrDefault();
                if (i != null)
                {
                    nd.Description = i.Description;
                    nd.Tags = i.Tags;
                }
            });
            
            responseText = "{" + 
                (nodeIds.Count > 0 ? string.Empty : "\"TotalCount\":" + totalCount.ToString() + ",") + 
                "\"Nodes\":[" + string.Join(",", nodes.Select(
                    n => "{\"NodeID\":\"" + n.NodeID.ToString() + "\"" +
                        ",\"AdditionalID\":\"" + Base64.encode(n.AdditionalID) + "\"" +
                        ",\"NodeTypeID\":\"" + n.NodeTypeID.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(n.Name) + "\"" +
                        ",\"NodeType\":\"" + Base64.encode(n.NodeType) + "\"" +
                        ",\"Abstract\":\"" + Base64.encode(n.Description) + "\"" +
                        ",\"Keywords\":[" + string.Join(",", n.Tags.Select(t => "\"" + Base64.encode(t) + "\"")) + "]" +
                        ",\"CreationDate\":\"" + (!n.CreationDate.HasValue ? string.Empty :
                            GenericDate.get_local_date(n.CreationDate.Value)) + "\"" +
                        ",\"CreationDate_Gregorian\":\"" + (!n.CreationDate.HasValue ? string.Empty :
                            n.CreationDate.Value.ToString()) + "\"" +
                        ",\"Status\":\"" + (n.Status == Status.NotSet ? string.Empty : n.Status.ToString()) + "\"" +
                        ",\"URL\":\"" + n.url(paramsContainer.Tenant.Id) + "\"" +
                        ",\"FormInstanceID\":\"" + (!formInstances.Any(u => u.OwnerID == n.NodeID) ? string.Empty :
                            formInstances.Where(u => u.OwnerID == n.NodeID).Select(i => i.InstanceID).First().ToString()) + "\"" +
                        ",\"Files\":[" + string.Join(",", files.Where(x => x.OwnerID == n.NodeID).Select(
                            f => "{\"Name\":\"" + Base64.encode(f.FileName) + "\"" +
                                ",\"Extension\":\"" + Base64.encode(f.Extension) + "\"" +
                                ",\"Size\":" + (f.Size.HasValue ? f.Size.Value : 0).ToString() +
                                ",\"Downloadable\":" + downloadAccess.Any(d => d == f.OwnerID).ToString().ToLower() +
                                ",\"URL\":\"" + PublicConsts.get_complete_url(paramsContainer.Tenant.Id, 
                                    PublicConsts.FileDownload) + "/" + f.FileID.ToString() + "\"" +
                                "}")) + 
                            "]" +
                        ",\"FormDetails\":{" + string.Join(",", instanceElements
                            .Where(u => formInstances.Any(x => x.OwnerID == n.NodeID) && 
                                u.FormInstanceID == formInstances.Where(y => y.OwnerID == n.NodeID)
                                .FirstOrDefault().InstanceID).Select(
                                a => "\"" + (string.IsNullOrEmpty(a.Name) ? a.ElementID.ToString().ToLower() : 
                                a.Name.ToLower()) + "\":" + a.toJson(paramsContainer.Tenant.Id))) + "}" +
                        "}"
                    )) + "]" + 
                "}";
        }

        private bool is_scheduler_user() {
            string schedulerUser = RaaiVanSettings.SchedulerUsername;

            if (!paramsContainer.CurrentUserID.HasValue || string.IsNullOrEmpty(schedulerUser)) return false;
            
            User scheduler = UsersController.get_user(null, paramsContainer.CurrentUserID.Value);

            return scheduler != null && !string.IsNullOrEmpty(scheduler.UserName) &&
                scheduler.UserName.ToLower() == schedulerUser.ToLower();
        }

        protected void get_all_applications(int? count, int? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.CurrentUserID.HasValue || !is_scheduler_user()) return;

            int totalCount = 0;

            List<Application> apps = GlobalController.get_applications(count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + 
                ",\"Applications\":[" + string.Join(",", apps.Select(a => a.toJson())) + "]" +
                "}";
        }

        protected void run_job(Guid? appId, Dictionary<string, object> options, ref string responseText) {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit || !is_scheduler_user()) return;

            string name = PublicMethods.get_dic_value(options, "name");
            
            if (!appId.HasValue || appId == Guid.Empty || options == null || string.IsNullOrEmpty(name)) return;

            switch (name.ToLower())
            {
                case "extract_file_contents":
                    {
                        int count = 0;
                        int.TryParse(PublicMethods.get_dic_value(options, "count"), out count);

                        for (int i = 0; i < Math.Min(50, Math.Max(count, 1)); i++)
                        {
                            bool notExists = false;
                            ExtractFileContents.ExtractOneDocument(appId.Value, ref notExists);
                            if (notExists) break;
                        }
                    }
                    break;
                case "index_update":
                    {
                        SearchDocType type = PublicMethods.parse_enum<SearchDocType>(
                            PublicMethods.get_dic_value(options, "type"), defaultValue: SearchDocType.All);

                        int batchSize = 
                            PublicMethods.parse_int(PublicMethods.get_dic_value(options, "batch_size"), defaultValue: 0).Value;

                        SearchUtilities.update_index(appId.Value, type, batchSize);
                    }
                    break;
                case "send_emails":
                    {
                        int batchSize = 0;
                        int.TryParse(PublicMethods.get_dic_value(options, "batch_size"), out batchSize);

                        EmailQueue.send_emails(appId.Value, batchSize);
                    }
                    break;
                case "remove_temporary_files":
                    (new DocsAPI()).remove_temporary_files(appId.Value);
                    break;
                default:
                    name = null;
                    break;
            }

            ArrayList x = new ArrayList();

            x.Add(new ArrayList());

            if (string.IsNullOrEmpty(name))
                responseText = "{\"result\":\"nok\",\"message\":\"job not found\"}";
            else {
                responseText = "{\"result\":\"ok\"}";

                if (appId.HasValue)
                {
                    LogController.save_log(appId, new Log()
                    {
                        UserID = paramsContainer.CurrentUserID.Value,
                        Date = DateTime.Now,
                        HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                        HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                        Action = Modules.Log.Action.JobDone,
                        SubjectID = appId.Value,
                        Info = "{\"Name\":\"" + name + "\"}",
                        ModuleIdentifier = ModuleIdentifier.RV
                    });
                }
            }
        }
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}