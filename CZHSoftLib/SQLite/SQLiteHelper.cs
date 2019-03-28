using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;

namespace CZHSoft.SQLite
{
    public class SQLiteHelper
    {
        private SQLiteConnection con = null;
        private IDbCommand cmd = null;
        private bool isServerOrClient=false;
        //private string dbFilePath = "C:\\CZHSoft.SQLite.db";
        private string dbRootPath = string.Empty;
        private string dbId = string.Empty;
        private string dbFileName = string.Empty;
        private string dbFilePath = string.Empty;
        private Dictionary<string, string> tableDetailDic;

        public delegate void GetDbMSG(string msg);
        public event GetDbMSG OnGetDbMSG;

        //public delegate void GetDbUIUpdate(IDataReader reader,string state);
        //public event GetDbUIUpdate OnGetDbUIUpdate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">isServerOrClient</param>
        /// <param name="id"></param>
        /// <param name="root"></param>
        /// <param name="table"></param>
        public SQLiteHelper(
            bool flag,
            string id,
            string root,
            Dictionary<string, string> table)
        {
            isServerOrClient = flag;

            dbId = id;
            dbRootPath = root;
            dbFileName = string.Format("{0}.db",id);

            tableDetailDic = table;
        }

        private bool InitDB()
        {
            try
            {
                if (isServerOrClient)
                {
                    dbFilePath = dbRootPath;

                }
                else
                {
                    dbFilePath = string.Format("{0}/{1}/{2}",
                    dbRootPath,
                    DateTime.Now.ToString("yyyyMMdd"),
                    dbId);

                    if (!Directory.Exists(dbFilePath))
                    {
                        Directory.CreateDirectory(dbFilePath);
                    }
                }

                con = new SQLiteConnection();
                //string connectionString = string.Format("Version=3,uri=file:{0}", dbFilename);
                string connectionString = string.Format(
                    "Data Source={0};Pooling=true;FailIfMissing=false",
                    string.Format("{0}/{1}", dbFilePath,dbFileName));

                con.ConnectionString = connectionString;
                con.Open();
                cmd = con.CreateCommand();

                string sql = string.Empty;

                foreach (string key in tableDetailDic.Keys)
                {
                    cmd.CommandText = string.Format("SELECT COUNT(*) FROM sqlite_master where type='table' and name='{0}'", key);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        cmd.CommandText = tableDetailDic[key];
                        cmd.ExecuteNonQuery();
                    }
                }

                if (OnGetDbMSG != null)
                {
                    OnGetDbMSG("DB init OK... \n");
                }

                return true;
            }
            catch
            {
                if (con != null)
                {
                    con.Close();
                    con = null;
                }
                if (OnGetDbMSG != null)
                {
                    OnGetDbMSG("DB init Fail... \n");
                }

                return false;
            }
        }

        public DataTable GetDateWithExecuteReader(string sql, string state)
        {
            if (!InitDB())
            {
                return null;
            }

            cmd.CommandText = sql;

            IDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Clear();

            try
            {
                int fieldCount = reader.FieldCount;//Access to the current line number of rows
                for (int count = 0; count < fieldCount; count++)
                {
                    if (reader.GetFieldType(count).IsArray)
                    {
                        dt.Columns.Add(reader.GetName(count), typeof(object));
                        continue;
                    }

                    dt.Columns.Add(reader.GetName(count), reader.GetFieldType(count));
                }
                //populate datatable
                while (reader.Read())
                {
                    DataRow datarow = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        datarow[i] = reader[i].ToString();
                    }
                    dt.Rows.Add(datarow);
                }

                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                dt = null;
            }
            finally
            {
                reader.Close();

                CloseDB();
            }

            return dt;
        }

        public int DoWithExecuteNonQuery(string sql)
        {
            try
            {
                if (!InitDB())
                {
                    return 0;
                }

                cmd.CommandText = sql;
                int result = cmd.ExecuteNonQuery();

                CloseDB();

                return result;
            }
            catch
            {
                return -1;
            }
        }

        public int DoWithExecuteNonQuery(string sql,
            Dictionary<string, object> parameters)
        {
            try
            {
                if (!InitDB())
                {
                    return 0;
                }

                cmd.CommandText = sql;

                foreach (string key in parameters.Keys)
                {
                    SQLiteParameter param = new SQLiteParameter("@" + key,
                        System.Data.DbType.Binary);

                    param.Value = parameters[key];
                    cmd.Parameters.Add(param);
                }

                int result = cmd.ExecuteNonQuery();

                CloseDB();

                return result;
            }
            catch
            {
                return -1;
            }
        }

        public byte[] GetBinaryData(string sql, int field)
        {
            if (!InitDB())
            {
                return null;
            }

            cmd.CommandText = sql;

            IDataReader reader = cmd.ExecuteReader();
            byte[] byteArray = new byte[12];
            byteArray[0] = 0xaa;

            while (reader.Read())
            {
                //Console.WriteLine(reader.GetString(0));
                reader.GetBytes(field, 0, byteArray, 0, 12);
                return byteArray;
            }

            return null;
        }

        public object DoWithExecuteScalar(string sql)
        {
            if (!InitDB())
            {
                return null;
            }

            cmd.CommandText = sql;
            object result = cmd.ExecuteScalar();
            CloseDB();

            return result;
        }

        private void CloseDB()
        {
            if (con != null)
            {
                if (con != null)
                {
                    con.Close();
                    con = null;
                }
                if (OnGetDbMSG != null)
                {
                    OnGetDbMSG("DB Closed... \n");
                }
            }
        }

    }
}
