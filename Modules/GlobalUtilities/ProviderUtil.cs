using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class ProviderUtil
    {
        public static string list_to_string<T>(List<T> lst, char? delimiter = ',')
        {
            string del = !delimiter.HasValue ? string.Empty : delimiter.Value.ToString();
            return lst == null ? string.Empty : string.Join(del, lst.Select(t => t.ToString()));
        }

        public static string pair_list_to_string<F, S>(ref List<KeyValuePair<F, S>> lst, char innerDelimiter, char outerDelimiter)
        {
            string strGuids = string.Empty;
            bool isFirst = true;
            foreach (KeyValuePair<F, S> _item in lst)
            {
                strGuids += (isFirst ? string.Empty : outerDelimiter.ToString()) +
                    (_item.Key.ToString() + innerDelimiter.ToString() + _item.Value.ToString());
                isFirst = false;
            }

            return strGuids;
        }

        public static string triple_list_to_string<F, S, T>(ref List<KeyValuePair<KeyValuePair<F, S>, T>> lst,
            char innerDelimiter, char outerDelimiter)
        {
            string strGuids = string.Empty;
            bool isFirst = true;
            foreach (KeyValuePair<KeyValuePair<F, S>, T> _item in lst)
            {
                strGuids += (isFirst ? string.Empty : outerDelimiter.ToString()) +
                    (_item.Key.Key.ToString() + innerDelimiter.ToString() + _item.Key.Value.ToString() + innerDelimiter.ToString() +
                    _item.Value.ToString());
                isFirst = false;
            }

            return strGuids;
        }

        public static string get_search_text(string searchText, bool startWith = true)
        {
            if (string.IsNullOrEmpty(searchText)) return searchText;

            if (RaaiVanSettings.UsePostgreSQL)
            {
                return string.Join(" OR ",
                    PublicMethods.convert_numbers_from_local(searchText.Replace("\"", " ").Replace("'", " "))
                    .Split(' ').Select(u => u.Trim()).Where(x => !string.IsNullOrEmpty(x)));
            }
            else
            {
                return "ISABOUT(" + string.Join(",",
                    PublicMethods.convert_numbers_from_local(searchText.Replace("\"", " ").Replace("'", " "))
                    .Split(' ').Select(u => u.Trim()).Where(x => !string.IsNullOrEmpty(x))
                    .Select(v => "\"" + v + (startWith ? "*" : "") + "\"")) + ")";
            }

            /*
            searchText = PublicMethods.convert_numbers_from_persian(searchText.Replace("\"", " ").Replace("'", " ").Replace("(", " ").Replace(")", " "));

            string[] words = searchText.Split(' ');
            List<string> lstWords = new List<string>();

            for (int i = 0; i < words.Count(); ++i)
                if (!string.IsNullOrEmpty(words[i].Trim())) lstWords.Add(words[i].Trim());

            return "ISABOUT(" + string.Join(",", lstWords.Select(u => "\"" + u + (startWith ? "*" : "") + "\"")) + ")";

            searchText = "ISABOUT(";
            for (int i = 0, _count = lstWords.Count; i < _count; ++i)
            {
                searchText += (i == 0 ? string.Empty : ",") + "\"" + lstWords[i] + (startWith ? "*" : "") +
                    "\" WEIGHT(" + (i > 4 ? 0.1 : (i == 0 ? 0.99 : 1.0) - (i * 0.2)).ToString() + ")";
            }
            searchText += ")";
            
            return searchText;
            */
        }

        public static string get_tags_text(List<string> tags)
        {
            if (tags == null) return string.Empty;

            string strTags = string.Empty;

            bool isFirst = true;
            foreach (string _t in tags)
            {
                strTags += (isFirst ? string.Empty : " ~ ") + _t;
                isFirst = false;
            }

            return strTags;
        }

        public static List<string> get_tags_list(string strTags, char delimiter = '~')
        {
            if (string.IsNullOrEmpty(strTags)) return new List<string>();
            List<string> tags = strTags.Split(delimiter).ToList();

            List<string> retList = new List<string>();

            foreach (string _t in tags)
            {
                if (string.IsNullOrEmpty(_t.Trim())) continue;
                retList.Add(_t.Trim());
            }

            return retList;
        }
    }
}
