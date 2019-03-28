using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CZHSoft.Pinvo
{
    public class ConfigurationHelper
    {
        private Configuration appConf;

        public ConfigurationHelper()
        {
            appConf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (!appConf.AppSettings.Settings.AllKeys.Contains("loginName"))
            {
                appConf.AppSettings.Settings.Add("loginName", "CG0382");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("password"))
            {
                appConf.AppSettings.Settings.Add("password", "mm123580@");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("psptid"))
            {
                appConf.AppSettings.Settings.Add("psptid", "500104198512270019");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("username"))
            {
                appConf.AppSettings.Settings.Add("username", "荆涛");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("dbPath"))
            {
                appConf.AppSettings.Settings.Add("dbPath", @"c:\\pinvo_ess.db");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("cerPath"))
            {
                appConf.AppSettings.Settings.Add("cerPath", @"c:\\pinvo_ess.cer");
            }

            if (!appConf.AppSettings.Settings.AllKeys.Contains("customDicFlag"))
            {
                appConf.AppSettings.Settings.Add("customDicFlag", "false");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("dicPath"))
            {
                appConf.AppSettings.Settings.Add("dicPath", "c:\\dic.xls");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("get1Path"))
            {
                appConf.AppSettings.Settings.Add("get1Path", "c:\\get1.xls");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("get2Path"))
            {
                appConf.AppSettings.Settings.Add("get2Path", "c:\\get2.xls");
            }
            if (!appConf.AppSettings.Settings.AllKeys.Contains("essKey"))
            {
                appConf.AppSettings.Settings.Add("essKey", "form:j_id434");
            }

            appConf.Save();
        }

        public void Save(
            string loginName, 
            string password, 
            string psptid,
            string username,
            string dbPath,
            string cerPath,
            string customDicFlag,
            string dicPath,
            string get1Path,
            string get2Path,
            string essKey)
        {
            appConf.AppSettings.Settings["loginName"].Value = loginName;
            appConf.AppSettings.Settings["password"].Value = password;
            appConf.AppSettings.Settings["psptid"].Value = psptid;
            appConf.AppSettings.Settings["username"].Value = username;
            appConf.AppSettings.Settings["dbPath"].Value = dbPath;
            appConf.AppSettings.Settings["cerPath"].Value = cerPath;
            appConf.AppSettings.Settings["customDicFlag"].Value = customDicFlag;
            appConf.AppSettings.Settings["dicPath"].Value = dicPath;
            appConf.AppSettings.Settings["get1Path"].Value = get1Path;
            appConf.AppSettings.Settings["get2Path"].Value = get2Path;
            appConf.AppSettings.Settings["essKey"].Value = essKey;
            appConf.Save();
        }

        public string[] GetConData()
        {
            string[] data = new string[] {
                appConf.AppSettings.Settings["loginName"].Value,
                appConf.AppSettings.Settings["password"].Value,
                appConf.AppSettings.Settings["psptid"].Value,
                appConf.AppSettings.Settings["username"].Value,
                appConf.AppSettings.Settings["dbPath"].Value,
                appConf.AppSettings.Settings["cerPath"].Value,
                appConf.AppSettings.Settings["customDicFlag"].Value,
                appConf.AppSettings.Settings["dicPath"].Value,
                appConf.AppSettings.Settings["get1Path"].Value,
                appConf.AppSettings.Settings["get2Path"].Value,
                appConf.AppSettings.Settings["essKey"].Value
            };

            return data;

        }
    }
}
