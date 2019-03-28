using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

namespace CZHSoft.Http
{
    /// <summary>
    /// Cookie操作帮助类
    /// </summary>
    public static class HttpCookieHelper
    {
        /// <summary>
        /// 根据字符生成Cookie列表
        /// </summary>
        /// <param name="cookie">Cookie字符串</param>
        /// <returns></returns>
        public static List<CookieItem> GetCookieList(string cookie)
        {
            List<CookieItem> cookielist = new List<CookieItem>();
            foreach (string item in cookie.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Regex.IsMatch(item, @"([\s\S]*?)=([\s\S]*?)$"))
                {
                    Match m = Regex.Match(item, @"([\s\S]*?)=([\s\S]*?)$");
                    cookielist.Add(new CookieItem() { Key = m.Groups[1].Value, Value = m.Groups[2].Value });
                }
            }
            return cookielist;
        }

        /// <summary>
        /// 根据Key值得到Cookie值,Key不区分大小写
        /// </summary>
        /// <param name="Key">key</param>
        /// <param name="cookie">字符串Cookie</param>
        /// <returns></returns>
        public static string GetCookieValue(string Key, string cookie)
        {
            foreach (CookieItem item in GetCookieList(cookie))
            {
                if (item.Key == Key)
                    return item.Value;
            }
            return "";
        }
        /// <summary>
        /// 格式化Cookie为标准格式
        /// </summary>
        /// <param name="key">Key值</param>
        /// <param name="value">Value值</param>
        /// <returns></returns>
        public static string CookieFormat(string key, string value)
        {
            return string.Format("{0}={1};", key, value);
        }

        public static string GetCookieValueByCookieCollection(CookieCollection cc,string key)
        {
            foreach (Cookie c in cc)
            {
                if (c.Name == key)
                {
                    //Console.WriteLine(c.Name + ":" + c.Value);

                    return c.Value;
                }
            }
            return "";
        }

        public static string GetCookieStringByCookieCollection(CookieCollection cc)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Cookie c in cc)
            {
                sb.AppendFormat("{0}={1};", c.Name, c.Value);
            }
            return sb.ToString();

        }

        public static bool ResetDomainByCookieCollection(CookieCollection cc,string key,string domain)
        {
            foreach (Cookie c in cc)
            {
                if (c.Name == key)
                {
                    c.Domain = domain;
                    return true;
                }
            }
            return false;

        }

        public static string GetHtmlSubString(string html, string begin, string end)
        {
            int startPosition = html.IndexOf(begin);
            if (startPosition == -1)
            {
                return "";
            }
            int msgPosition = startPosition + begin.Length;
            int endPosition = html.IndexOf(end, msgPosition);
            if (endPosition == -1)
            {
                return "";
            }
            string state = html.Substring(msgPosition, endPosition - msgPosition);
            return state;
        }

        public static string GetPostParam(Dictionary<string, string> dic, bool isUrlEncode)
        {
            string param = string.Empty;

            List<string> keys = new List<string>(dic.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                if (i == 0)
                {
                    if (isUrlEncode)
                    {
                        param += HttpUtility.UrlEncode(keys[i]) + "=" + HttpUtility.UrlEncode(dic[keys[i]]);
                    }
                    else
                    {
                        param += keys[i] + "=" + dic[keys[i]];
                    }
                }
                else
                {
                    if (isUrlEncode)
                    {
                        param += "&" + HttpUtility.UrlEncode(keys[i]) + "=" + HttpUtility.UrlEncode(dic[keys[i]]);
                    }
                    else
                    {
                        param += "&" + keys[i] + "=" + dic[keys[i]];
                    }
                }
            }

            return param;
        }



        

    }

    /// <summary>
    /// Cookie对象
    /// </summary>
    public class CookieItem
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
