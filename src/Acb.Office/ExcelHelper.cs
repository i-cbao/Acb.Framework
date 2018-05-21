using Acb.Core;
using Acb.Core.Extensions;
using Acb.Office.Excel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Office
{
    /// <summary> Excel辅助类 </summary>
    public static class ExcelHelper
    {
        /// <summary> 创建workbook样式 </summary>
        /// <param name="wb"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(IWorkbook wb, XlsStyle str)
        {
            var cellStyle = wb.CreateCellStyle();

            //边框  
            cellStyle.BorderBottom =
                cellStyle.BorderLeft = cellStyle.BorderRight = cellStyle.BorderTop = BorderStyle.Thin;
            //边框颜色  
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;

            //背景图形，我没有用到过。感觉很丑 
            cellStyle.FillForegroundColor = HSSFColor.White.Index;
            cellStyle.FillBackgroundColor = HSSFColor.Blue.Index;

            //水平对齐  
            cellStyle.Alignment = HorizontalAlignment.Left;

            //垂直对齐  
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            //自动换行  
            //cellStyle.WrapText = true;

            //缩进;当设置为1时，前面留的空白太大了。希旺官网改进。或者是我设置的不对  
            cellStyle.Indention = 0;

            //上面基本都是设共公的设置  
            //下面列出了常用的字段类型  
            switch (str)
            {
                case XlsStyle.Header:
                    // cellStyle.FillPattern = FillPatternType.LEAST_DOTS; 
                    cellStyle.FillForegroundColor = 41;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                    cellStyle.Alignment = HorizontalAlignment.Center;

                    var heaerFont = wb.CreateFont();
                    heaerFont.FontHeightInPoints = 12;
                    heaerFont.Boldweight = 600;
                    heaerFont.FontName = "微软雅黑";
                    cellStyle.SetFont(heaerFont);
                    break;
                case XlsStyle.DateTime:
                    IDataFormat datastyle = wb.CreateDataFormat();

                    cellStyle.DataFormat = datastyle.GetFormat("yyyy/mm/dd");
                    cellStyle.SetFont(SetCellNormalFont(wb));
                    break;
                case XlsStyle.Number:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.SetFont(SetCellNormalFont(wb));
                    break;
                case XlsStyle.Money:
                    IDataFormat format = wb.CreateDataFormat();
                    cellStyle.DataFormat = format.GetFormat("￥#,##0");
                    cellStyle.SetFont(SetCellNormalFont(wb));
                    break;
                case XlsStyle.Url:
                    var fontcolorblue = wb.CreateFont();
                    fontcolorblue.Color = HSSFColor.Black.Index;
                    fontcolorblue.IsItalic = true; //下划线  
                    fontcolorblue.FontName = "微软雅黑";
                    fontcolorblue.Underline = FontUnderlineType.Single;
                    cellStyle.SetFont(fontcolorblue);
                    break;
                case XlsStyle.Percent:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                    cellStyle.SetFont(SetCellNormalFont(wb));
                    break;
                //case XlsStyle.中文大写:
                //    IDataFormat format1 = wb.CreateDataFormat();
                //    cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");
                //    cellStyle.SetFont(font);
                //    break;
                //case XlsStyle.科学计数法:
                //    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                //    cellStyle.SetFont(font);
                //    break;
                case XlsStyle.Default:
                    cellStyle.SetFont(SetCellNormalFont(wb));
                    break;
            }
            return cellStyle;
        }

        /// <summary>
        /// 设置基本字体格式
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static IFont SetCellNormalFont(IWorkbook wb)
        {
            var font = wb.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "微软雅黑";
            return font;
        }
        #region 创建Excel

        /// <summary>
        /// 是否数值类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsNumber(object value)
        {
            if (value == null) return false;
            return
                new[] { typeof(int), typeof(double), typeof(long), typeof(decimal), typeof(float), typeof(byte) }.Contains
                    (value.GetType());
        }

        /// <summary> 创建原生Workbook </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static IWorkbook Create(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;
            IWorkbook wb = new HSSFWorkbook();

            var headerStyle = GetCellStyle(wb, XlsStyle.Header);
            var style = GetCellStyle(wb, XlsStyle.Default);

            var index = 0;
            foreach (DataTable tb in dataSet.Tables)
            {
                var colLength = tb.Columns.Count;   //列
                if (colLength == 0)
                    continue;

                index++;
                var sheet =
                    wb.CreateSheet((string.IsNullOrWhiteSpace(tb.TableName) || tb.TableName.StartsWith("Table"))
                        ? $"sheet{index}"
                        : tb.TableName);
                //表头
                var row = sheet.CreateRow(0);
                for (var i = 0; i < colLength; i++)
                {
                    sheet.AutoSizeColumn(i);
                    var column = tb.Columns[i];
                    var cell = row.CreateCell(i);
                    cell.CellStyle = headerStyle;
                    cell.SetCellValue(column.ColumnName);
                }

                //数据
                for (var i = 0; i < tb.Rows.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    var cellItems = tb.Rows[i].ItemArray;

                    var eachColLength = Math.Min(colLength, cellItems.Length);
                    for (var j = 0; j < eachColLength; j++)
                    {
                        var value = cellItems[j];
                        if (IsNumber(value))
                        {
                            var cell = row.CreateCell(j, CellType.Numeric);
                            cell.CellStyle = style;
                            cell.SetCellValue(value == null ? 0D : Convert.ToDouble(value));
                        }
                        else
                        {
                            var cell = row.CreateCell(j);
                            cell.CellStyle = style;
                            cell.SetCellValue((value ?? string.Empty).ToString());
                        }
                    }
                }
            }
            return wb;
        }


        public static async Task<IWorkbook> CreateAsync(DataSet dataSet)
        {
            return await Task.FromResult(Create(dataSet));
        }

        /// <summary> 创建Excel </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <param name="streamAction"></param>
        /// <returns></returns>
        public static T Create<T>(DataSet dataSet, Func<MemoryStream, T> streamAction)
        {
            if (streamAction == null)
                return default(T);
            var wb = Create(dataSet);
            if (wb == null) return default(T);
            using (var ms = new MemoryStream())
            {
                wb.Write(ms);
                wb.Close();
                return streamAction(ms);
            }
        }

        public static void Create(DataSet dataSet, Action<MemoryStream> streamAction)
        {
            Create(dataSet, ms =>
            {
                streamAction(ms);
            });
        }

        /// <summary> 创建Excel </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static byte[] CreateBytes(DataSet dataSet)
        {
            return Create(dataSet, ms => ms?.ToArray() ?? new byte[] { });
        }

        /// <summary> 创建Excel文件 </summary>
        /// <param name="dataSet"></param>
        /// <param name="filePath"></param>
        public static void CreateFile(DataSet dataSet, string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                return;
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(ext))
                return;
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var t = CreateBytes(dataSet);
                fs.Write(t, 0, t.Length);
                fs.Flush();
            }
        }

        /// <summary> 导出Excel </summary>
        /// <param name="dataSet"></param>
        /// <param name="filename"></param>
        public static async Task Export(DataSet dataSet, string filename = null)
        {
            var context = AcbHttpContext.Current;
            if (context == null)
                return;
            var resp = context.Response;
            filename = filename ?? dataSet.DataSetName;
            var ext = Path.GetExtension(filename);
            if (string.IsNullOrWhiteSpace(ext) || !new[] { ".xls", ".xlsx" }.Contains(ext))
            {
                ext = ".xls";
            }

            var name = Path.GetFileNameWithoutExtension(filename);
            filename = $"{name.UrlEncode()}{ext}";
            //resp.Buffer = true;
            //resp.Charset = Encoding.UTF8.BodyName;
            resp.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            resp.Headers.Add("Content-Disposition", $"attachment;filename={filename}");
            resp.ContentType = "application/vnd.ms-excel; charset=UTF-8";
            var wb = await CreateAsync(dataSet);
            wb.Write(resp.Body);
            wb.Close();
        }

        #endregion

        #region 读取Excel

        private static DataTable ReadTable(ISheet sheet, bool hasColumnHeader = true)
        {
            var dt = new DataTable(sheet.SheetName);
            int startRow;
            var firstRow = sheet.GetRow(0);
            int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

            if (hasColumnHeader)
            {
                for (int j = firstRow.FirstCellNum; j < cellCount; ++j)
                {
                    var cell = firstRow.GetCell(j);
                    var cellValue = cell?.StringCellValue;
                    if (cellValue == null) continue;
                    var column = new DataColumn(cellValue);
                    dt.Columns.Add(column);
                }
                startRow = sheet.FirstRowNum + 1;
            }
            else
            {
                for (int j = firstRow.FirstCellNum; j < cellCount; ++j)
                {
                    dt.Columns.Add(new DataColumn($"column{j}"));
                }
                startRow = sheet.FirstRowNum;
            }

            //最后一列的标号
            var rowCount = sheet.LastRowNum;
            for (var i = startRow; i <= rowCount; ++i)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue; //没有数据的行默认是null　　　　　　　

                var dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; ++j)
                {
                    if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary> 读取Excel到DataSet </summary>
        /// <param name="excelStream"></param>
        /// <param name="hasColumnHeader"></param>
        /// <returns></returns>
        public static DataSet Read(Stream excelStream, bool hasColumnHeader = true)
        {
            var wb = new HSSFWorkbook(new POIFSFileSystem(excelStream));
            var ds = new DataSet();
            for (var i = 0; i < wb.NumberOfSheets; i++)
            {
                var dt = ReadTable(wb.GetSheetAt(i), hasColumnHeader);
                ds.Tables.Add(dt);
            }
            return ds;
        }

        /// <summary> 读取Excel到DataSet </summary>
        /// <param name="filePath"></param>
        /// <param name="hasColumnHeader"></param>
        /// <returns></returns>
        public static DataSet Read(string filePath, bool hasColumnHeader = true)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return Read(fs, hasColumnHeader);
            }
        }

        /// <summary> 读取第一个表格 </summary>
        /// <param name="excelStream"></param>
        /// <param name="hasColumnHeader"></param>
        /// <returns></returns>
        public static DataTable ReadFirst(Stream excelStream, bool hasColumnHeader = true)
        {
            var wb = new HSSFWorkbook(new POIFSFileSystem(excelStream));
            if (wb.NumberOfSheets == 0)
                return null;
            return ReadTable(wb.GetSheetAt(0), hasColumnHeader);
        }

        /// <summary> 读取Excel到DataSet </summary>
        /// <param name="filePath"></param>
        /// <param name="hasColumnHeader"></param>
        /// <returns></returns>
        public static DataTable ReadFirst(string filePath, bool hasColumnHeader = true)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return ReadFirst(fs, hasColumnHeader);
            }
        }

        #endregion
    }
}
