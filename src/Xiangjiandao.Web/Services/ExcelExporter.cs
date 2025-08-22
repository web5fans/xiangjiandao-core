using System.Data;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Xiangjiandao.Web.Services;

/// <summary>
/// Excel 导出服务
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// 简单导出 Excel 文件
    /// </summary>
    IActionResult Export(DataTable table, string fileName);

    /// <summary>
    /// 带有合并单元格的导出 Excel
    /// </summary>
    IActionResult ExportWithMerge(DataTable table, string fileName, string flag);
}

public class ExcelExporter : IExcelExporter
{
    public IActionResult Export(DataTable table, string fileName)
    {
        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Sheet1");
        var headerRow = sheet.CreateRow(0);
        foreach (DataColumn column in table.Columns)
        {
            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
        }

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            for (var j = 0; j < table.Columns.Count; j++)
            {
                row.CreateCell(j).SetCellValue(table.Rows[i][j].ToString());
            }
        }

        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            workbook.Write(memoryStream);
            bytes = memoryStream.ToArray();
        }

        return new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
    }

    public IActionResult ExportWithMerge(DataTable table, string fileName, string flag)
    {
        for (var i = 0; i < table.Columns.Count; i++)
        {
            if (flag.Equals(table.Rows[0][i].ToString()))
            {
                throw new ArgumentException("First row can not contains merge flag");
            }
        }

        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Sheet1");
        var headerRow = sheet.CreateRow(0);
        foreach (DataColumn column in table.Columns)
        {
            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
        }

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            for (var j = 0; j < table.Columns.Count; j++)
            {
                if (flag.Equals(table.Rows[i][j].ToString()))
                {
                    row.CreateCell(j).SetCellValue(string.Empty);
                    continue;
                }

                row.CreateCell(j).SetCellValue(table.Rows[i][j].ToString());
            }
        }

        var toMergeCellRangeAddressesArray = new List<CellRangeAddress>[table.Columns.Count];
        for (var i = 0; i < toMergeCellRangeAddressesArray.Length; i++)
        {
            toMergeCellRangeAddressesArray[i] = [];
        }

        for (var i = 0; i < table.Columns.Count; i++)
        {
            var rowCursor = 1;
            for (var j = 0; j < table.Rows.Count; j++)
            {
                if ((flag.Equals(table.Rows[j][i].ToString()) && j != table.Rows.Count - 1) || j == 0)
                {
                    continue;
                }

                if (flag.Equals(table.Rows[j][i].ToString()) && j == table.Rows.Count - 1)
                {
                    toMergeCellRangeAddressesArray[i].Add(new CellRangeAddress(
                            firstRow: rowCursor,
                            lastRow: j + 1,
                            firstCol: i,
                            lastCol: i
                        )
                    );
                    break;
                }

                toMergeCellRangeAddressesArray[i].Add(new CellRangeAddress(
                        firstRow: rowCursor,
                        lastRow: j,
                        firstCol: i,
                        lastCol: i
                    )
                );
                rowCursor = j + 1;
            }
        }

        foreach (var cellRangeAddress in toMergeCellRangeAddressesArray.SelectMany(item => item))
        {
            if (cellRangeAddress.FirstColumn == cellRangeAddress.LastColumn &&
                cellRangeAddress.FirstRow == cellRangeAddress.LastRow
               )
            {
                continue;
            }

            sheet.AddMergedRegion(cellRangeAddress);
        }

        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            workbook.Write(memoryStream);
            bytes = memoryStream.ToArray();
        }

        return new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
    }
}