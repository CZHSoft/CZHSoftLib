using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CZHSoft.RegularExpressions
{
    public class RegexHelper
    {

        /// <summary>
        /// 数字 Bug
        /// </summary>
        public bool IsMatchNumber(string data)
        {
            return Regex.IsMatch(data, "^[0-9]*$");
        }

        /// <summary>
        /// 非负
        /// </summary>>
        public bool IsMatchEnNegative(string data)
        {
            return Regex.IsMatch(data, @"^\d+$");
        }

        /// <summary>
        /// 非零正整数
        /// </summary>
        public bool IsMatchPosInteger(string data)
        {
            return Regex.IsMatch(data, @"^\+?[1-9][0-9]*$");
        }

        /// <summary>
        /// 是否合法的IP
        /// </summary>
        public bool IsValidIP(string ip)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ips = ip.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            else
                return false;
        } 

    }
}
