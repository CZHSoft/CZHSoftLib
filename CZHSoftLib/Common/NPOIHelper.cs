using System;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Drawing;
using NPOI.HPSF;
using NPOI.HSSF.Util;

namespace CZHSoft.Common
{
    public class NPOIHelper
    {
        readonly int EXCEL03_MaxRow = 65535;

        public bool DataTable2Excel(DataTable dt, string sheetName,string fileName)
        {

            IWorkbook book = new HSSFWorkbook();
            if (dt.Rows.Count < EXCEL03_MaxRow)
                DataWrite2Sheet(dt, 0, dt.Rows.Count - 1, book, sheetName);
            else
            {
                int page = dt.Rows.Count / EXCEL03_MaxRow;
                for (int i = 0; i <= page; i++)
                {
                    int start = i * EXCEL03_MaxRow;
                    int end = (i * EXCEL03_MaxRow) + EXCEL03_MaxRow - 1;
                    if (page == i)
                    {
                        end = dt.Rows.Count - 1;
                    }
                    DataWrite2Sheet(dt, start, end, book, sheetName + i.ToString());
                }
                //int lastPageItemCount = dt.Rows.Count % EXCEL03_MaxRow;
                //DataWrite2Sheet(dt, dt.Rows.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString());
            }
            MemoryStream ms = new MemoryStream();
            book.Write(ms);

            byte[] data = ms.ToArray();

            try
            {
                System.IO.FileStream _FileStream =new System.IO.FileStream(fileName, System.IO.FileMode.Create,System.IO.FileAccess.Write);

                _FileStream.Write(data, 0, data.Length);

                _FileStream.Close();

                return true;

            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());

                return false;
            }
        }
        private void DataWrite2Sheet(DataTable dt, int startRow, int endRow, IWorkbook book, string sheetName)
        {
            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string val = dt.Columns[i].Caption ?? dt.Columns[i].ColumnName;
                cell.SetCellValue(val);
            }
            int rowIndex = 1;
            for (int i = startRow; i <= endRow; i++)
            {
                DataRow dtRow = dt.Rows[i];
                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < dtRow.ItemArray.Length; j++)
                {
                    excelRow.CreateCell(j).SetCellValue(dtRow[j].ToString());
                }
            }

        }

        #region 读取Excel文件内容转换为DataSet
        /// <summary> 
        /// 读取Excel文件内容转换为DataSet,列名依次为 "c0"……c[columnlength-1] 
        /// </summary> 
        /// <param name="FileName">文件绝对路径</param> 
        /// <param name="startRow">数据开始行数(1为第一行)</param> 
        /// <param name="ColumnDataType">每列的数据类型</param> 
        /// <returns></returns> 
        public DataSet ReadExcel(string FileName, int startRow, int sheetCount, params NpoiDataType[] ColumnDataType)
        {
            int ertime = 0;
            int intime = 0;

            DataSet ds = new DataSet("ds");

            StringBuilder sb = new StringBuilder();

            using (FileStream stream = new FileStream(@FileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式 

                for (int c = 0; c < sheetCount; c++)
                {
                    ISheet sheet = workbook.GetSheetAt(c);//得到里面第一个sheet 

                    DataTable dt = new DataTable(string.Format("dt{0}", c));
                    DataRow dr;

                    int j;
                    IRow row;
                    #region ColumnDataType赋值
                    if (ColumnDataType.Length <= 0)
                    {
                        row = sheet.GetRow(startRow - 1);//得到第i行 
                        ColumnDataType = new NpoiDataType[row.LastCellNum];
                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            ICell hs = row.GetCell(i);
                            ColumnDataType[i] = GetCellDataType(hs);
                        }
                    }
                    #endregion
                    for (j = 0; j < ColumnDataType.Length; j++)
                    {
                        Type tp = GetDataTableType(ColumnDataType[j]);
                        dt.Columns.Add("c" + j, tp);
                    }
                    for (int i = startRow - 1; i <= sheet.PhysicalNumberOfRows; i++)
                    {
                        row = sheet.GetRow(i);//得到第i行 
                        if (row == null) continue;
                        try
                        {
                            dr = dt.NewRow();

                            for (j = 0; j < ColumnDataType.Length; j++)
                            {
                                dr["c" + j] = GetCellData(ColumnDataType[j], row, j);
                            }
                            dt.Rows.Add(dr);
                            intime++;
                        }
                        catch (Exception er)
                        {
                            ertime++;
                            sb.Append(string.Format("第{0}行出错：{1}\r\n", i + 1, er.Message));
                            continue;
                        }
                    }

                    ds.Tables.Add(dt);

                    if (ds.Tables[c].Rows.Count == 0 && sb.ToString() != "") throw new Exception(sb.ToString());
                }
            }

            return ds;
        }
        #endregion

        #region 读Excel-根据NpoiDataType创建的DataTable列的数据类型
        /// <summary> 
        /// 读Excel-根据NpoiDataType创建的DataTable列的数据类型 
        /// </summary> 
        /// <param name="datatype"></param> 
        /// <returns></returns> 
        private Type GetDataTableType(NpoiDataType datatype)
        {
            Type tp = typeof(string);//Type.GetType("System.String") 
            switch (datatype)
            {
                case NpoiDataType.Bool:
                    tp = typeof(bool);
                    break;
                case NpoiDataType.Datetime:
                    tp = typeof(DateTime);
                    break;
                case NpoiDataType.Numeric:
                    tp = typeof(double);
                    break;
                case NpoiDataType.Error:
                    tp = typeof(string);
                    break;
                case NpoiDataType.Blank:
                    tp = typeof(string);
                    break;
            }
            return tp;
        }
        #endregion

        #region 读Excel-得到不同数据类型单元格的数据
        /// <summary> 
        /// 读Excel-得到不同数据类型单元格的数据 
        /// </summary> 
        /// <param name="datatype">数据类型</param> 
        /// <param name="row">数据中的一行</param> 
        /// <param name="column">哪列</param> 
        /// <returns></returns> 
        private object GetCellData(NpoiDataType datatype, IRow row, int column)
        {

            switch (datatype)
            {
                case NpoiDataType.String:
                    try
                    {
                        return row.GetCell(column).DateCellValue;
                    }
                    catch
                    {
                        try
                        {
                            return row.GetCell(column).StringCellValue;
                        }
                        catch
                        {
                            return row.GetCell(column).NumericCellValue;
                        }
                    }
                case NpoiDataType.Bool:
                    try { return row.GetCell(column).BooleanCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Datetime:
                    try { return row.GetCell(column).DateCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Numeric:
                    try { return row.GetCell(column).NumericCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Richtext:
                    try { return row.GetCell(column).RichStringCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Error:
                    try { return row.GetCell(column).ErrorCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Blank:
                    try { return row.GetCell(column).StringCellValue; }
                    catch { return ""; }
                default: return "";
            }
        }
        #endregion

        #region 获取单元格数据类型
        /// <summary> 
        /// 获取单元格数据类型 
        /// </summary> 
        /// <param name="hs"></param> 
        /// <returns></returns> 
        private NpoiDataType GetCellDataType(ICell hs)
        {
            NpoiDataType dtype;
            DateTime t1;
            string cellvalue = "";

            switch (hs.CellType)
            {
                case CellType.Blank:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Boolean:
                    dtype = NpoiDataType.Bool;
                    break;
                case CellType.Numeric:
                    dtype = NpoiDataType.Numeric;
                    cellvalue = hs.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Error:
                    dtype = NpoiDataType.Error;
                    break;
                case CellType.Formula:
                default:
                    dtype = NpoiDataType.Datetime;
                    break;
            }
            if (cellvalue != "" && DateTime.TryParse(cellvalue, out t1)) dtype = NpoiDataType.Datetime;
            return dtype;
        }
        #endregion

        #region 枚举(Excel单元格数据类型)
        /// <summary> 
        /// 枚举(Excel单元格数据类型) 
        /// </summary> 
        public enum NpoiDataType
        {
            /// <summary> 
            /// 字符串类型-值为1 
            /// </summary> 
            String,
            /// <summary> 
            /// 布尔类型-值为2 
            /// </summary> 
            Bool,
            /// <summary> 
            /// 时间类型-值为3 
            /// </summary> 
            Datetime,
            /// <summary> 
            /// 数字类型-值为4 
            /// </summary> 
            Numeric,
            /// <summary> 
            /// 复杂文本类型-值为5 
            /// </summary> 
            Richtext,
            /// <summary> 
            /// 空白 
            /// </summary> 
            Blank,
            /// <summary> 
            /// 错误 
            /// </summary> 
            Error
        }
        #endregion
    }
}
