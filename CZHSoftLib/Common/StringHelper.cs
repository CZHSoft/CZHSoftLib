using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CZHSoft.Common
{
    public class StringHelper
    {
        public static List<string> GetNumsInString(string str)
        {
            List<string> slist = new List<string>();
            string temp = string.Empty;
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str, i) == true)
                {
                    if (!flag)
                    {
                        temp = "";

                        flag = true;
                    }

                    temp += str.Substring(i, 1);

                }
                else
                {
                    if (flag)
                    {
                        slist.Add(temp);
                        temp = "";

                        flag = false;
                    }
                }

            }

            if (!string.IsNullOrEmpty(temp) && flag)
            {
                slist.Add(temp);
            }

            return slist;
        }

        public static void GetUrlValues(string url, ref Dictionary<string, string> data)
        {
            foreach (string key in data.Keys)
            {
                string regType = string.Format(@"(?:^|/?|&){0}=([^&]*)(?:&|$)", key);

                Regex regex = new Regex(regType);

                Match match = regex.Match(url.ToLower());

                if (match.Success)
                {
                    data[key]=match.Groups[1].Value;
                }

            }

        }

    }
}
