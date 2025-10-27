using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class GenerateScriptFile
    {
        private string MapFileName;

        public GenerateScriptFile(string mapFileName)
        {
            MapFileName = mapFileName;
        }

        private string resolve_path(string basePath, string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            else if (string.IsNullOrEmpty(basePath))
            {
                if (path[0] == '\\')
                {
                    string pt = PublicMethods.map_path("~/");
                    path = pt.Substring(0, pt.ToLower().LastIndexOf("web") - 1) + path.Replace('/', '\\');
                }

                return path;
            }
            else
                return basePath + (basePath[basePath.Length - 1] != '\\' ? "\\" : "") + path.Replace('/', '\\');
        }

        private string process_folder(string path, bool scanSubFolders)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return string.Empty;
            else
            {
                List<string> folders = !scanSubFolders ? new List<string>() :
                    Directory.GetDirectories(path).Select(f => process_folder(f, scanSubFolders))
                        .Where(f => !string.IsNullOrEmpty(f)).ToList();

                List<string> files = Directory.GetFiles(path).Select(f =>
                {
                    string heading = @"[Uu][Ss][Ee][\s\t\n\r]+.{1,20}[\s\t\n\r]+[Gg][Oo][\s\t\n\r]+";
                    return Regex.Replace(File.ReadAllText(f), heading, "");
                }).Where(f => !string.IsNullOrEmpty(f)).ToList();

                return string.Join("\r\n\r\n", folders.Concat(files));
            }
        }

        private string process_array(string path, ArrayList sub)
        {
            if (!string.IsNullOrEmpty(path) && sub != null && sub.Count > 0)
            {
                List<string> contentArr = sub
                    .ToArray()
                    .ToList()
                    .Select(s =>
                    {
                        if (s.GetType() == typeof(string))
                            return process_folder(resolve_path(path, (string)s), scanSubFolders: false);
                        else if (s.GetType() == typeof(Dictionary<string, object>))
                            return process_map(path, (Dictionary<string, object>)s);
                        else
                            return string.Empty;
                    })
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                return string.Join("\r\n\r\n", contentArr);
            }
            else
                return process_folder(path, scanSubFolders: false);
        }

        private string process_map(string path, Dictionary<string, object> dic)
        {
            path = resolve_path(path, PublicMethods.get_dic_value(dic, "path"));

            object sub = PublicMethods.get_dic_value<object>(dic, "sub");

            if (sub == null)
                return process_folder(path, scanSubFolders: false);
            else if (sub.GetType() == typeof(string) && (string)sub == "*")
                return process_folder(path, scanSubFolders: true);
            else if (sub.GetType() == typeof(ArrayList))
                return process_array(path, (ArrayList)sub);
            else
                return string.Empty;
        }

        public string get()
        {
            if (string.IsNullOrEmpty(MapFileName)) return string.Empty;

            string address = PublicMethods.map_path("~/" + MapFileName);

            string content = File.Exists(address) ? File.ReadAllText(address) : string.Empty;

            content = content.Substring(content.IndexOf("{"));
            content = content.Substring(0, content.LastIndexOf("}") + 1);

            return process_map(path: null, PublicMethods.fromJSON(content));
        }
    }
}
