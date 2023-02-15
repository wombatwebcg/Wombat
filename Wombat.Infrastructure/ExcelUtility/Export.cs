using ExcelReport;
using Wombat.Infrastructure;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.Extend;

namespace Wombat.Infrastructure
{
    /// <summary>
    /// EXCEL导出功能集合类
    /// 作者：Zuowenjun
    /// 日期：2016/1/15
    /// </summary>
    public static partial class ExcelHelper
    {
        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataSet</param>
        /// <param name="filePath">导出路径，可选</param>
        /// <returns></returns>
        public static string ToExcel(DataSet sourceDs, string filePath = null)
        {


            if (string.IsNullOrEmpty(filePath)) return null;

            bool isCompatible = Common.GetIsCompatible(filePath);

            IWorkbook workbook = Common.CreateWorkbook(isCompatible);
            ICellStyle headerCellStyle = Common.GetCellStyle(workbook, true);
            //ICellStyle cellStyle = Common.GetCellStyle(workbook);

            for (int i = 0; i < sourceDs.Tables.Count; i++)
            {
                DataTable table = sourceDs.Tables[i];
                string sheetName = string.IsNullOrEmpty(table.TableName) ? "result" + i.ToString() : table.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);
                IRow headerRow = sheet.CreateRow(0);
                Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();
                // handling header.
                foreach (DataColumn column in table.Columns)
                {
                    ICell headerCell = headerRow.CreateCell(column.Ordinal);
                    headerCell.SetCellValue(column.ColumnName);
                    headerCell.CellStyle = headerCellStyle;
                    sheet.AutoSizeColumn(headerCell.ColumnIndex);
                    colStyles[headerCell.ColumnIndex] = Common.GetCellStyle(workbook);
                }

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in table.Columns)
                    {
                        ICell cell = dataRow.CreateCell(column.Ordinal);
                        //cell.SetCellValue((row[column] ?? "").ToString());
                        //cell.CellStyle = cellStyle;
                        Common.SetCellValue(cell, (row[column] ?? "").ToString(), column.DataType, colStyles);
                        Common.ReSizeColumnWidth(sheet, cell);
                    }

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
            }

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            workbook.Write(fs);
            fs.Dispose();
            workbook = null;

            return filePath;

        }

        /// <summary>
        /// 由DataTable导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="colAliasNames">导出的列名重命名数组</param>
        /// <param name="sheetName">工作薄名称，可选</param>
        /// <param name="filePath">导出路径，可选</param>
        /// <param name="colDataFormats">列格式化集合，可选</param>
        /// <returns></returns>
        public static string ToExcel(DataTable sourceTable, string[] colAliasNames, string sheetName = "result", string filePath = null, IDictionary<string, string> colDataFormats = null)
        {
            if (sourceTable.Rows.Count <= 0) return null;


            if (string.IsNullOrEmpty(filePath)) return null;

            if (colAliasNames == null || sourceTable.Columns.Count != colAliasNames.Length)
            {
                throw new ArgumentException("列名重命名数组与DataTable列集合不匹配。", "colAliasNames");
            }

            bool isCompatible = Common.GetIsCompatible(filePath);

            IWorkbook workbook = Common.CreateWorkbook(isCompatible);
            ICellStyle headerCellStyle = Common.GetCellStyle(workbook, true);
            //ICellStyle cellStyle = Common.GetCellStyle(workbook);
            Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();

            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow headerRow = sheet.CreateRow(0);
            // handling header.
            foreach (DataColumn column in sourceTable.Columns)
            {
                ICell headerCell = headerRow.CreateCell(column.Ordinal);
                headerCell.SetCellValue(colAliasNames[column.Ordinal]);
                headerCell.CellStyle = headerCellStyle;
                sheet.AutoSizeColumn(headerCell.ColumnIndex);
                if (colDataFormats != null && colDataFormats.ContainsKey(column.ColumnName))
                {
                    colStyles[headerCell.ColumnIndex] = Common.GetCellStyleWithDataFormat(workbook, colDataFormats[column.ColumnName]);
                }
                else
                {
                    colStyles[headerCell.ColumnIndex] = Common.GetCellStyle(workbook);
                }
            }

            // handling value.
            int rowIndex = 1;

            foreach (DataRow row in sourceTable.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in sourceTable.Columns)
                {
                    ICell cell = dataRow.CreateCell(column.Ordinal);
                    //cell.SetCellValue((row[column] ?? "").ToString());
                    //cell.CellStyle = cellStyle;
                    Common.SetCellValue(cell, (row[column] ?? "").ToString(), column.DataType, colStyles);
                    Common.ReSizeColumnWidth(sheet, cell);
                }

                rowIndex++;
            }
            sheet.ForceFormulaRecalculation = true;

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            workbook.Write(fs);
            fs.Dispose();

            sheet = null;
            headerRow = null;
            workbook = null;

            return filePath;
        }


        /// <summary>
        /// 由DataTable导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="sheetName">工作薄名称，可选</param>
        /// <param name="filePath">导出路径，可选</param>
        /// <param name="colNames">需要导出的列名,可选</param>
        /// <param name="colAliasNames">导出的列名重命名，可选</param>
        /// <param name="colDataFormats">列格式化集合，可选</param>
        /// <param name="sheetSize">指定每个工作薄显示的记录数，可选（不指定或指定小于0，则表示只生成一个工作薄）</param>
        /// <returns></returns>
        public static string ToExcel(DataTable sourceTable, string sheetName = "result", string filePath = null, string[] colNames = null, IDictionary<string, string> colAliasNames = null, IDictionary<string, string> colDataFormats = null, int sheetSize = 0)
        {
            if (sourceTable.Rows.Count <= 0) return null;

            if (string.IsNullOrEmpty(filePath)) return null;

            bool isCompatible = Common.GetIsCompatible(filePath);

            IWorkbook workbook = Common.CreateWorkbook(isCompatible);
            ICellStyle headerCellStyle = Common.GetCellStyle(workbook, true);
            //ICellStyle cellStyle = Common.GetCellStyle(workbook);

            if (colNames == null || colNames.Length <= 0)
            {
                colNames = sourceTable.Columns.Cast<DataColumn>().OrderBy(c => c.Ordinal).Select(c => c.ColumnName).ToArray();
            }

            IEnumerable<DataRow> batchDataRows, dataRows = sourceTable.Rows.Cast<DataRow>();
            int sheetCount = 0;
            if (sheetSize <= 0)
            {
                sheetSize = sourceTable.Rows.Count;
            }
            while ((batchDataRows = dataRows.Take(sheetSize)).Count() > 0)
            {

                Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();

                ISheet sheet = workbook.CreateSheet(sheetName + (++sheetCount).ToString());
                IRow headerRow = sheet.CreateRow(0);

                // handling header.
                for (int i = 0; i < colNames.Length; i++)
                {
                    ICell headerCell = headerRow.CreateCell(i);
                    if (colAliasNames != null && colAliasNames.ContainsKey(colNames[i]))
                    {
                        headerCell.SetCellValue(colAliasNames[colNames[i]]);
                    }
                    else
                    {
                        headerCell.SetCellValue(colNames[i]);
                    }
                    headerCell.CellStyle = headerCellStyle;
                    sheet.AutoSizeColumn(headerCell.ColumnIndex);
                    if (colDataFormats != null && colDataFormats.ContainsKey(colNames[i]))
                    {
                        colStyles[headerCell.ColumnIndex] = Common.GetCellStyleWithDataFormat(workbook, colDataFormats[colNames[i]]);
                    }
                    else
                    {
                        colStyles[headerCell.ColumnIndex] = Common.GetCellStyle(workbook);
                    }
                }

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in batchDataRows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    for (int i = 0; i < colNames.Length; i++)
                    {
                        ICell cell = dataRow.CreateCell(i);
                        //cell.SetCellValue((row[colNames[i]] ?? "").ToString());
                        //cell.CellStyle = cellStyle;
                        Common.SetCellValue(cell, (row[colNames[i]] ?? "").ToString(), sourceTable.Columns[colNames[i]].DataType, colStyles);
                        Common.ReSizeColumnWidth(sheet, cell);
                    }

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;

                dataRows = dataRows.Skip(sheetSize);
            }

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            workbook.Write(fs);
            fs.Dispose();
            workbook = null;

            return filePath;
        }









    }
}
