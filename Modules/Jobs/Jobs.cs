using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Modules.Jobs
{
    public class Jobs
    {
        public static void run(Guid applicationId, string jobName)
        {
            jobName = jobName.ToLower();

            switch (jobName)
            {
                case "updatefriendsuggestions":
                    {
                        if (RaaiVanConfig.Modules.SocialNetwork(applicationId)) UsersController.update_friend_suggestions(applicationId);
                        break;
                    }
                case "updateanalyzerdata":
                    {
                        Recommender.send_data_to_analyzer(applicationId);
                        break;
                    }
                case "convertformstotables":
                    List<FormType> forms = FGController.get_forms(applicationId, hasName: true);

                    foreach (FormType f in forms)
                        if (f.FormID.HasValue) FGController.convert_form_to_table(applicationId, f.FormID.Value);
                    break;
                default:
                    CustomJobs.do_job(applicationId, jobName);
                    break;
            }
        }

        /*
        public static void copy_files(Guid applicationId) {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ProviderUtil.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "select nd.TypeName as FolderName, nd.NodeName as Title, " + 
                "f.FileNameGuid as GuidName, f.[FileName], f.Extension " +
                "from dbo.DCT_Files as f " +
                "inner join dbo.CN_View_Nodes_Normal as nd " +
                "on nd.NodeID = f.OwnerID and nd.Deleted = 0 " +
                "where f.Deleted = 0";

            con.Open();

            List<TheFile> files = new List<Modules.Jobs.Jobs.TheFile>();

            try
            {
                System.Data.IDataReader reader = (System.Data.IDataReader)cmd.ExecuteReader();
                files = parse_results(ref reader);
            }
            catch (Exception ex)
            {
            }
            finally { con.Close(); }

            string rootPath = PublicMethods.map_path("~") + "TheFiles";
            string attachmentsFolder = DocumentUtilities.map_path(applicationId, FolderNames.Attachments);

            if (!System.IO.Directory.Exists(rootPath)) System.IO.Directory.CreateDirectory(rootPath);

            string sourceFile = string.Empty, destFile = string.Empty, fileName = string.Empty;
            
            try
            {
                foreach (TheFile f in files)
                {
                    try
                    {
                        if (!f.GuidName.HasValue) continue;

                        string destFolder = rootPath + "\\" + f.FolderName;

                        if (!System.IO.Directory.Exists(destFolder)) System.IO.Directory.CreateDirectory(destFolder);

                        sourceFile = attachmentsFolder + "\\" +
                            DocumentUtilities.get_sub_folder(f.GuidName.Value) + "\\" + f.GuidName.ToString();

                        if (System.IO.File.Exists(sourceFile) || (!string.IsNullOrEmpty(f.Extension) &&
                            System.IO.File.Exists(sourceFile = sourceFile + "." + f.Extension)))
                        {
                            while (true)
                            {
                                fileName = (f.Title.Substring(0, Math.Min(f.Title.Length, 50)) + " - " +
                                    f.FileName.Substring(0, Math.Min(f.FileName.Length, 30)) + " - " +
                                    PublicMethods.get_random_number(4));

                                string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) +
                                    new string(System.IO.Path.GetInvalidPathChars());
                                System.Text.RegularExpressions.Regex r =
                                    new System.Text.RegularExpressions.Regex(string.Format("[{0}]",
                                    System.Text.RegularExpressions.Regex.Escape(regexSearch)));
                                fileName = r.Replace(fileName, "");

                                destFile = destFolder + "\\" + fileName +
                                    (string.IsNullOrEmpty(f.Extension) ? string.Empty : "." + f.Extension);

                                if (!System.IO.File.Exists(destFile))
                                {
                                    System.IO.File.Copy(sourceFile, destFile);
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "copy files for shahrdari", ex, ModuleIdentifier.Jobs);
            }
        }

        private static List<TheFile> parse_results(ref System.Data.IDataReader reader)
        {
            List<TheFile> ret = new List<TheFile>();

            while (reader.Read())
            {
                try
                {
                    TheFile itm = new TheFile();
                    
                    if (!string.IsNullOrEmpty(reader["FolderName"].ToString())) itm.FolderName = (string)reader["FolderName"];
                    if (!string.IsNullOrEmpty(reader["Title"].ToString())) itm.Title = (string)reader["Title"];
                    if (!string.IsNullOrEmpty(reader["GuidName"].ToString())) itm.GuidName = (Guid)reader["GuidName"];
                    if (!string.IsNullOrEmpty(reader["FileName"].ToString())) itm.FileName = (string)reader["FileName"];
                    if (!string.IsNullOrEmpty(reader["Extension"].ToString())) itm.Extension = (string)reader["Extension"];

                    ret.Add(itm);
                }
                catch {  }
            }

            if (!reader.IsClosed) reader.Close();

            return ret;
        }

        private class TheFile
        {
            public string FolderName;
            public string Title;
            public Guid? GuidName;
            public string FileName;
            public string Extension;
        }
        */
    }
}
