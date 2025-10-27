using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web
{
    public class ThemeUtil
    {
        private static Dictionary<string, Dictionary<string, string>> ThemeContent =
            new Dictionary<string, Dictionary<string, string>>();

        private static string TemplateContent;
        private static string GlobalContent;
        private static Dictionary<string, string> TemplateVariables = new Dictionary<string, string>();
        private static Dictionary<RVLang, string> FontFace = new Dictionary<RVLang, string>();

        private static Dictionary<string, string> extract_variables(string content)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(content)) return vars;

            Expressions.get_matches_string(content, Expressions.Patterns.CSSVariable)
                .Where(v => !string.IsNullOrEmpty(v)).ToList()
                .ForEach(v =>
                {
                    List<string> pair = v.Replace(";", "").Split(':').ToList();
                    if (pair.Count == 2 && !string.IsNullOrEmpty(pair[0].Trim()) && !string.IsNullOrEmpty(pair[1].Trim()))
                        vars[pair[0].Trim()] = pair[1].Trim();
                });

            return vars;
        }

        private static Dictionary<string, string> get_theme_content(Guid? applicationId, string name)
        {
            if (!string.IsNullOrEmpty(name)) name = name.Trim().ToLower();

            if (string.IsNullOrEmpty(name)) return null;
            else if (ThemeContent.ContainsKey(name)) return ThemeContent[name];

            string thm = new DocFileInfo(applicationId)
            {
                FileName = name,
                Extension = "css",
                FolderName = FolderNames.Themes
            }.get_text_content();

            if (string.IsNullOrEmpty(thm)) return null;

            Dictionary<string, string> vars = extract_variables(thm);

            if (!PublicMethods.is_dev()) ThemeContent[name] = vars;

            return vars;
        }

        private static string get_font_face(RVLang lang)
        {
            if (FontFace.ContainsKey(lang) && !string.IsNullOrEmpty(FontFace[lang])) return FontFace[lang];

            string content = string.Empty;

            string path = PublicMethods.map_path(lang == RVLang.fa ? PublicConsts.FontFaceIranSans : PublicConsts.FontFaceRoboto);

            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
                if (!PublicMethods.is_dev()) FontFace[lang] = content;
            }

            return content;
        }

        private static string get_template()
        {
            if (!string.IsNullOrEmpty(TemplateContent)) return TemplateContent;

            string content = string.Empty;

            string path = PublicMethods.map_path(PublicConsts.ThemeCSS);

            if (File.Exists(path))
            {
                content = File.ReadAllText(path);

                TemplateVariables = extract_variables(content.Substring(0, content.IndexOf('}')));

                if (TemplateVariables == null) TemplateVariables = new Dictionary<string, string>();

                if (!PublicMethods.is_dev()) TemplateContent = content;
            }

            return content;
        }

        private static string get_global()
        {
            if (!string.IsNullOrEmpty(GlobalContent)) return GlobalContent;

            string content = string.Empty;

            string path = PublicMethods.map_path(PublicConsts.GlobalCSS);

            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
                if (!PublicMethods.is_dev()) GlobalContent = content;
            }

            return content;
        }

        public static string get_theme(Guid? applicationId, string name, RVLang lang)
        {
            string template = get_template(), 
                global = get_global(), 
                fontFace = get_font_face(lang);

            Dictionary<string, string> dic = get_theme_content(applicationId, name);

            if (string.IsNullOrEmpty(template) || dic == null) return string.Empty;
            
            /*
            dic.Keys.ToList().ForEach(k => {
                template = template.Replace("var(" + k + ")", dic[k]);
            });

            TemplateVariables.Keys.ToList().ForEach(k => {
                template = template.Replace("var(" + k + ")", TemplateVariables[k]);
            });
            */

            List<string> keys = dic.Keys.ToList();
            keys.AddRange(TemplateVariables.Keys);

            keys = keys.Distinct().ToList()
                .Select(k => k + ": " + (dic.ContainsKey(k) ? dic[k] : TemplateVariables[k]) + ";").ToList();

            template = Expressions.replace(template,
                @":root\s*\{([^\}]+)\}",
                "html {\r\n    " + string.Join("\r\n    ", keys) + "\r\n}");

            return template + "\r\n\r\n" + fontFace + "\r\n\r\n" + global;
        }

        public static string theme_name(Guid? applicationId, Guid? userId, HttpContext context)
        {
            string theme = userId.HasValue && RaaiVanSettings.EnableThemes(applicationId) ?
                UsersController.get_theme(applicationId, userId.Value) : string.Empty;

            if (!userId.HasValue)
            {
                theme = context.Request.Cookies["ck_theme"] == null ? string.Empty : context.Request.Cookies["ck_theme"].Value;
                if (!string.IsNullOrEmpty(theme)) theme = theme.Split(',')[0];
            }

            if (string.IsNullOrEmpty(theme) || !RaaiVanSettings.Themes.Any(t => t.ToLower().IndexOf(theme.ToLower()) >= 0))
                theme = RaaiVanSettings.DefaultTheme(applicationId);

            return theme;
        }
    }
}