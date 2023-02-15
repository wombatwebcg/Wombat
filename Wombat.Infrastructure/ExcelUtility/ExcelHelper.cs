using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Wombat.Infrastructure
{
    public static partial class ExcelHelper
    {

        public static string ExportExcel(string path, string fileName,DataSet sourceDs)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + path))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + path);
            }
            fileName = fileName.Replace("*", "_");
            fileName = fileName.Replace("<", "_");
            fileName = fileName.Replace(">", "_");
            fileName = fileName.Replace(":", "_");
            fileName = fileName.Replace("?", "_");
            fileName = fileName.Replace("/", "_");
            fileName = fileName.Replace(@"\", "_");
            string newPath = Environment.CurrentDirectory + path + $"\\{fileName}.xls";
            var fs = File.OpenWrite(newPath);//以write方式打开文件，wb工作表写回
            //创建EXCEL
            HSSFWorkbook wk = new HSSFWorkbook();
            //创建一个Sheet
            //ISheet sheet = wk.CreateSheet(fileName);
            wk.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            wk.Write(fs);
            fs.Close();
          return  ToExcel(sourceDs, newPath);
        }


        public static void WriteResult(string fileName,string replaceSign,string serialNumber,string model,double range,double[]distance,double[]output,double[] linear,out string newFileName)
        {
            FileStream fs = File.OpenRead(Environment.CurrentDirectory+ "//" + "template.xls");
            HSSFWorkbook wb = new HSSFWorkbook(fs);
            fs.Close();
            //将工作表读入wk，就可以关闭文件流了，下面将在内存中修改数据
            HSSFSheet st = (HSSFSheet)wb.GetSheetAt(0);
            st.ForceFormulaRecalculation = true;
            HSSFRow row_model = (HSSFRow)st.GetRow(2);
            row_model.Cells[1].SetCellValue(model);//修改后写回
            HSSFRow row_serial = (HSSFRow)st.GetRow(2);
            row_serial.Cells[3].SetCellValue(serialNumber);//修改后写回

            HSSFRow row_rangAdate = (HSSFRow)st.GetRow(3);
            row_rangAdate.Cells[1].SetCellValue(range.ToString());//修改后写回

            row_rangAdate.Cells[3].SetCellValue(DateTime.Now.ToString("d").Replace("/","_"));//修改后写回

            insertRow(wb, st, 7, distance.Length);
            double[] wc = new double[distance.Length];
            for (int i = 0; i < distance.Length; i++)
            {
                HSSFRow row = (HSSFRow)st.GetRow(7+i);
                row.Cells[0].SetCellValue(distance[i]); //修改后写回
                row.Cells[1].SetCellValue(output[i]); //修改后写回
                row.Cells[2].SetCellValue(linear[i]); //修改后写回
                row.Cells[3].SetCellValue(linear[i] - output[i]); //修改后写回
                wc[i] = linear[i] - output[i];
            }

            double max = 0;
            for (int i = 0; i < wc.Length; i++)//从第二个元素开始遍历数组
            {
                if (Math.Abs(wc[i]) >Math.Abs(max))//把第一个元素与剩下的元素相比较，看谁大               
                    max = wc[i];//谁大就把谁赋值给max
            }
            HSSFRow row1 = (HSSFRow)st.GetRow(distance.Length+8);
           double linearity = Math.Abs(Math.Abs(max) / (output[output.Length - 1] - output[0]));

            row1.Cells[1].SetCellValue(linearity);//修改后写回


            HSSFRow row2 = (HSSFRow)st.GetRow(distance.Length+9);

            row2.Cells[1].SetCellValue(max);//修改后写回

            st.GetRow(distance.Length + 20).Cells[3].SetCellValue(DateTime.Now.ToString("d").Replace("/", "_"));
            st.GetRow(distance.Length + 21).Cells[3].SetCellValue(DateTime.Now.ToString("d").Replace("/", "_"));



            //设置style
            ICellStyle cellstyle = wb.CreateCellStyle();
            cellstyle.VerticalAlignment = VerticalAlignment.Center;
            cellstyle.Alignment = HorizontalAlignment.Center;

            //合并操作

            st.AddMergedRegion(new CellRangeAddress(distance.Length+22, distance.Length + 22, 0, 3));//起始行，结束行，起始列，结束列

            //设置合并后style
            var cell = st.GetRow(distance.Length + 22).GetCell(0);
            cell.CellStyle = cellstyle;


            if (!Directory.Exists(Environment.CurrentDirectory + "\\记录文件"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\记录文件");
            }
            fileName= fileName.Replace("*", replaceSign);
            fileName = fileName.Replace("<", replaceSign);
            fileName = fileName.Replace(">", replaceSign);
            fileName = fileName.Replace(":", replaceSign);
            fileName = fileName.Replace("?", replaceSign);
            fileName = fileName.Replace("/", replaceSign);
            fileName = fileName.Replace(@"\", replaceSign);
            newFileName = "记录文件//" + $"{fileName}.xls";
            fs = File.OpenWrite(newFileName);//以write方式打开文件，wb工作表写回
            wb.Write(fs);
            wb.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            fs.Close();
        }




        /// <summary>
        /// Excel复制行
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="sheet"></param>
        /// <param name="starRow"></param>
        /// <param name="rows"></param>
        private static void insertRow(HSSFWorkbook wb, HSSFSheet sheet, int starRow, int rows)
        {
            /*
             * ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight);
             *
             * startRow 开始行
             * endRow 结束行
             * n 移动行数
             * copyRowHeight 复制的行是否高度在移
             * resetOriginalRowHeight 是否设置为默认的原始行的高度
             *
             */

            sheet.ShiftRows(starRow + 1, sheet.LastRowNum, rows, true, true);

            starRow = starRow - 1;

            for (int i = 0; i < rows; i++)
            {

                HSSFRow sourceRow = null;
                HSSFRow targetRow = null;
                HSSFCell sourceCell = null;
                HSSFCell targetCell = null;

                short m;

                starRow = starRow + 1;
                sourceRow = (HSSFRow)sheet.GetRow(starRow);
                targetRow = (HSSFRow)sheet.CreateRow(starRow + 1);
                targetRow.HeightInPoints = sourceRow.HeightInPoints;

                for (m = (short)sourceRow.FirstCellNum; m < sourceRow.LastCellNum; m++)
                {

                    sourceCell = (HSSFCell)sourceRow.GetCell(m);
                    targetCell = (HSSFCell)targetRow.CreateCell(m);

                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);

                }
            }

        }

    }
}
