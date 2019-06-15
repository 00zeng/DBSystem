using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing;
using NPOI.HSSF.Util;
using System.Data;

namespace ProjectShare.Process
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class ExcelApi
    {
        /// <summary>
        /// NPOI导出EXCEl
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <param name="arrHead">表头</param>
        /// <param name="arrColWidth">列宽</param>
        /// <param name="headHeight">表头高度</param>
        /// <param name="colHeight">列高度</param>
        /// <param name="dt">数据</param>
        /// <returns>工作簿</returns>
        public static void ExcelExport(HSSFWorkbook book, string sheetName, string[] arrHead, int[] arrColWidth, DataTable dt, int start, int end)
        {
            //建立空白工作簿
            if(book == null)
                book = new HSSFWorkbook();
            //在工作簿中：建立空白工作表
            ISheet sheet = book.CreateSheet("Sheet1");

            IFont font = book.CreateFont();
            font.Boldweight = short.MaxValue; 
            font.FontHeightInPoints = 11;

            //设置单元格的样式：水平垂直对齐居中
            ICellStyle cellStyle = book.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;
            cellStyle.WrapText = true;//自动换行

            //设置表头的样式：水平垂直对齐居中，加粗
            ICellStyle titleCellStyle = book.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index; //图案颜色
            titleCellStyle.FillPattern = FillPattern.SparseDots; //图案样式
            titleCellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index; //背景颜色
                                                                                //设置边框
            titleCellStyle.BorderBottom = BorderStyle.Thin;
            titleCellStyle.BorderLeft = BorderStyle.Thin;
            titleCellStyle.BorderRight = BorderStyle.Thin;
            titleCellStyle.BorderTop = BorderStyle.Thin;
            titleCellStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleCellStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleCellStyle.RightBorderColor = HSSFColor.Black.Index;
            titleCellStyle.TopBorderColor = HSSFColor.Black.Index;
            //设置字体
            titleCellStyle.SetFont(font);

            //表头
            IRow headRow = sheet.CreateRow(0);
            headRow.HeightInPoints = 18;
            for (int i = 0; i < arrHead.Length; i++)
            {
                headRow.CreateCell(i).SetCellValue(arrHead[i]);
                headRow.GetCell(i).CellStyle = titleCellStyle;
            }

            //列宽
            for (int j = 0; j < arrColWidth.Length; j++)
            {
                sheet.SetColumnWidth(j, arrColWidth[j] * 256);
            }

            int rowIndex = 1;
            //循环添加行
            for (int r = start; r < end; r++)
            {
                IRow row = sheet.CreateRow(rowIndex++);
               // row.HeightInPoints = colHeight;
                //循环列赋值
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    row.CreateCell(c).SetCellValue(dt.Rows[r][c].ToString());
                    row.GetCell(c).CellStyle = cellStyle;
                }
            };
        }

    }
}