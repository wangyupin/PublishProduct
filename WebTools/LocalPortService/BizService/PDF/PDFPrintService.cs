using LocalPortService.Model.API;
using Microsoft.Extensions.Hosting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using System.Text.Json;
using LocalPortService.Core.Helper;
using System;
using PdfSharp.UniversalAccessibility.Drawing;
using PdfSharp.Fonts;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Diagnostics.Metrics;
using ESC_POS_USB_NET.Enums;
using System.Data.SqlTypes;
using PdfSharp.Drawing.Layout;
using System.Drawing.Text;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using PdfSharp.Charting;

namespace LocalPortService.BizService.PDF
{
    public class PDFPrintService
    {
        private readonly IConfiguration _config;
        public PDFPrintService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> Print(PDFExample example)
        {
            try
            {
                //計算表身行數
                int bodyRowNum = example.PDFExampleBody.Rows.Count;
                bool printerEndEvent = false;
                int rowStartIndex = 0;
                int rowRemainNum = bodyRowNum;
                int pageNum = 1;
                // 创建一个新的 PDF 文档
                PdfDocument document = new PdfDocument();
                document.Info.Title = "PDF with Table Example";
                GlobalFontSettings.FontResolver = new FileFontResolver();

                while (!printerEndEvent)
                {
                    // 创建一个页面
                    PdfPage page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(210);
                    page.Height = XUnit.FromMillimeter(139);
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont titleFont = new XFont("標楷體", 16, XFontStyleEx.Bold);
                    XFont headerFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyColumnHeaderFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyFont = new XFont("標楷體", 12, XFontStyleEx.Regular);
                    XFont pageFont = new XFont("標楷體", 9, XFontStyleEx.Regular);

                    // 獲取頁面尺寸
                    double pageWidth = page.Width;
                    double pageHeight = page.Height;

                    // 定义页面大小和初始位置
                    double yPoint = 15;
                    double margin = 40;
                    double columnNum = example.PDFExampleBody.Column.Count > 1 ? example.PDFExampleBody.Column.Count - 1 : 1;
                    double columnWidth = pageWidth / columnNum;

                    // 绘制標題
                    gfx.DrawString(example.CompanyName, titleFont, XBrushes.Black,
                        new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
                    yPoint += 15;

                    if (!string.IsNullOrEmpty(example.DocType))
                    {
                        gfx.DrawString(example.DocType, titleFont, XBrushes.Black,
                            new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
                        yPoint += 15;
                    }

                    //頁次
                    gfx.DrawString($"頁次: {pageNum}", pageFont, XBrushes.Black, new XRect(margin + 485, yPoint, 100, 20), XStringFormats.TopLeft);

                    //繪製表頭
                    // 兩兩分組
                    var pairs = example.PDFExampleHead.headItems.Select((value, index) => new { Index = index, Value = value })
                                                                .GroupBy(x => x.Index / 2)
                                                                .Select(g => g.Select(x => x.Value).ToList())
                                                                .ToList();
                    double marginLeft = 0;
                    foreach (var pair in pairs)
                    {
                        marginLeft = 0; //一行
                        foreach (var item in pair)
                        {
                            var txt = $"{item.ItemName}: {item.ItemValue}";
                            gfx.DrawString(txt, headerFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, 100, 20), XStringFormats.TopLeft);

                            marginLeft += pageWidth / pair.Count;
                        }

                        yPoint += 15; // 向下移动
                    }

                    yPoint += 3; // 向下移动
                    //線
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 3; //間隔
                    marginLeft = 0;
                    // 绘制表格欄位名稱
                    foreach (var column in example.PDFExampleBody.Column)
                    {
                        if (column.Align.ToLower() == "left") gfx.DrawString(column.ColumnName, bodyColumnHeaderFont, XBrushes.Black, new XRect(margin + marginLeft + 10, yPoint, column.Width, 20), XStringFormats.TopLeft);
                        else if (column.Align.ToLower() == "right") gfx.DrawString(column.ColumnName, bodyColumnHeaderFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, column.Width, 20), XStringFormats.TopRight);
                        marginLeft += column.Width;
                    }


                    yPoint += 15; //間隔
                    //線
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 5; // 向下移动


                    // 绘制表格内容
                    foreach (var row in example.PDFExampleBody.Rows.Select((value, index) => new { Index = index, Data = value }).ToList().GetRange(rowStartIndex, rowRemainNum))
                    {
                        marginLeft = 0;
                        int counter = 0;
                        var options = new JsonSerializerOptions
                        {
                            Converters = { new ObjectAsPrimitiveConverter(floatFormat: FloatFormat.Double, unknownNumberFormat: UnknownNumberFormat.Error, objectFormat: ObjectFormat.Expando) },
                            WriteIndented = true,
                        };
                        IDictionary<string, object> rowColumns = JsonSerializer.Deserialize<dynamic>(row.Data, options);
                        foreach (var column in rowColumns)
                        {
                            var valueType = column.Value.GetType();
                            bool numberType = valueType == typeof(int) || valueType == typeof(double) || valueType == typeof(decimal) || valueType == typeof(float);
                            double width = example.PDFExampleBody.Column.ElementAt(counter).Width;
                            // now process the values
                            dynamic kvpValue = column.Value ?? "";
                            string value = kvpValue.ToString();
                            if (numberType && example.PDFExampleBody.DecimalPoint) value = kvpValue.ToString("N2");
                            else if (numberType) value = kvpValue.ToString("N0");
                            //超過長度裁切
                            string trimValue = value.Length > 30 ? value.Substring(0, 30) : value;
                            trimValue = Regex.IsMatch(trimValue, @"[\u4e00-\u9fff]{1,8}") && value.Length >= 15 ? trimValue.Substring(0, 15) : trimValue;
                            if (numberType && column.Key != "index") gfx.DrawString(trimValue, bodyFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, width, 20), XStringFormats.TopRight);
                            else if (column.Key == "index") gfx.DrawString(trimValue, bodyFont, XBrushes.Black, new XRect(margin + marginLeft + 5, yPoint, width, 20), XStringFormats.TopCenter);
                            else if (column.Key.Contains("num", StringComparison.OrdinalIgnoreCase)) gfx.DrawString(trimValue, bodyFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, width, 20), XStringFormats.TopRight);
                            else gfx.DrawString(trimValue, bodyFont, XBrushes.Black, new XRect(margin + marginLeft + 10, yPoint, width, 20), XStringFormats.TopLeft);

                            marginLeft += width;

                            counter++;
                        }
                        yPoint += 17; // 移动到下一行

                        if (row.Index + 1 == bodyRowNum)
                        {
                            printerEndEvent = true;
                        };

                        double moveYPoint = example.PDFExampleBody.TaxOption ? 100 : 60;

                        if (yPoint + moveYPoint > pageHeight)
                        {
                            rowStartIndex = row.Index + 1;
                            rowRemainNum = bodyRowNum - rowStartIndex;
                            pageNum++;
                            break;
                        };

                    }

                    //線
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 5; // 向下移动

                    marginLeft = 0;
                    if (printerEndEvent && example.PDFExampleBody.Totals.Count > 0)
                    {
                        int startIndex = example.PDFExampleBody.Column.FindIndex(x => x.ColumnName == example.PDFExampleBody.Totals[0].ColumnName);
                        double witdth = startIndex > -1 ? example.PDFExampleBody.Column[startIndex].Width : 0;
                        double startMargin = startIndex > -1 ? example.PDFExampleBody.Column.GetRange(0, startIndex).Sum(x => x.Width) - 10 : 0;
                        gfx.DrawString("合計:", bodyFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                        foreach (var item in example.PDFExampleBody.Totals)
                        {
                            var valueType = item.Total.GetType();
                            string value = string.Empty;
                            // now process the values
                            if (item.Total is JsonElement element)
                            {
                                if (element.ValueKind == JsonValueKind.String) value = item.Total.ToString(); // Extract the string value
                                else if (element.ValueKind == JsonValueKind.Number)
                                {
                                    decimal eleValue = element.TryGetDecimal(out decimal result) ? result : 0;
                                    if (example.PDFExampleBody.DecimalPoint) value = eleValue.ToString("N2");
                                    else value = eleValue.ToString("N0");

                                }
                            }
                            int eleIndex = example.PDFExampleBody.Column.FindIndex(x => x.ColumnName == item.ColumnName);
                            witdth = eleIndex > -1 ? example.PDFExampleBody.Column[eleIndex].Width : 0;
                            double startXpoint = example.PDFExampleBody.Column.GetRange(0, eleIndex).Sum(x => x.Width);
                            gfx.DrawString(value, bodyFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                        }
                        if (example.PDFExampleBody.TaxOption)
                        {
                            yPoint += 15; // 向下移动
                            string taxStr = example.PDFExampleBody.DecimalPoint ? example.PDFExampleBody.Tax.ToString("N2") : example.PDFExampleBody.Tax.ToString("N0");
                            string amountStr = example.PDFExampleBody.DecimalPoint ? example.PDFExampleBody.Amount.ToString("N2") : example.PDFExampleBody.Amount.ToString("N0");
                            int eleIndex = example.PDFExampleBody.Column.FindIndex(x => x.ColumnName == example.PDFExampleBody.SummaryColumn);
                            witdth = eleIndex > -1 ? example.PDFExampleBody.Column[eleIndex].Width : 0;
                            double startXpoint = eleIndex > -1 ? example.PDFExampleBody.Column.GetRange(0, eleIndex).Sum(x => x.Width) : example.PDFExampleBody.Column.Sum(x => x.Width);
                            gfx.DrawString("稅額:", bodyFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                            gfx.DrawString(taxStr, bodyFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                            yPoint += 15; // 向下移动

                            gfx.DrawString("總計:", bodyFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                            gfx.DrawString(amountStr, bodyFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                        }
                        yPoint += 20; // 向下移动
                    }
                    string remark = example.PDFExampleBody.Remark;
                    double bottomRemarkYPoint = 350;
                    gfx.DrawString("詳細說明:", bodyFont, XBrushes.Black, new XRect(margin, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                    int counter2 = 0;
                    while (!string.IsNullOrEmpty(remark))
                    {
                        int length = remark.Length;
                        string substr = remark.Substring(0, length);
                        XSize size = gfx.MeasureString(substr, bodyFont);


                        if (counter2 == 0)
                        {
                            while (size.Width > pageWidth - margin * 2 - 60)
                            {
                                length--;
                                substr = remark.Substring(0, length);
                                size = gfx.MeasureString(substr, bodyFont);
                            }
                            gfx.DrawString(remark.Substring(0, length), bodyFont, XBrushes.Black, new XRect(margin + 60, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                        }
                        else
                        {
                            while (size.Width > pageWidth - margin * 2)
                            {
                                length--;
                                substr = remark.Substring(0, length);
                                size = gfx.MeasureString(substr, bodyFont);
                            }
                            gfx.DrawString(remark.Substring(0, length), bodyFont, XBrushes.Black, new XRect(margin, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                        }

                        yPoint += 10;
                        remark = remark.Substring(length).Trim();

                        if (yPoint + 30 > page.Height.Point)
                        {
                            remark = string.Empty;
                        }
                        counter2++;
                    }
                    double checkedYPoint = bottomRemarkYPoint + 20;
                    gfx.DrawString("經理:", pageFont, XBrushes.Black, new XRect(margin, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("主管:", pageFont, XBrushes.Black, new XRect(margin + 80, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("會計:", pageFont, XBrushes.Black, new XRect(margin + 160, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("客戶:", pageFont, XBrushes.Black, new XRect(margin + 240, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("送貨:", pageFont, XBrushes.Black, new XRect(margin + 320, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("倉庫:", pageFont, XBrushes.Black, new XRect(margin + 400, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("業務:", pageFont, XBrushes.Black, new XRect(margin + 480, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                }

                // 保存PDF
                var coverFilePath = Path.GetTempFileName() + Guid.NewGuid().ToString() + ".pdf";
                document.Save(coverFilePath);
                Process.Start(new ProcessStartInfo(coverFilePath) { UseShellExecute = true });
                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public async Task<bool> PrintPivot(PDFExample example)
        {
            try
            {
                //計算表身行數
                int bodyRowNum = example.PDFExampleBody.Rows.Count;
                bool printerEndEvent = false;
                int rowStartIndex = 0;
                int rowRemainNum = bodyRowNum;
                int pageNum = 1;
                // 创建一个新的 PDF 文档
                PdfDocument document = new PdfDocument();
                document.Info.Title = "PDF with Table Example";
                GlobalFontSettings.FontResolver = new FileFontResolver();

                while (!printerEndEvent)
                {
                    // 创建一个页面
                    PdfPage page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(210);
                    page.Height = XUnit.FromMillimeter(139);
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont titleFont = new XFont("標楷體", 16, XFontStyleEx.Bold);
                    XFont headerFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyColumnHeaderFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyFont = new XFont("標楷體", 12, XFontStyleEx.Regular);
                    XFont pageFont = new XFont("標楷體", 9, XFontStyleEx.Regular);

                    // 獲取頁面尺寸
                    double pageWidth = page.Width;
                    double pageHeight = page.Height;

                    // 定义页面大小和初始位置
                    double yPoint = 15;
                    double margin = 40;
                    double columnNum = example.PDFExampleBody.Column.Count > 1 ? example.PDFExampleBody.Column.Count - 1 : 1;
                    double columnWidth = pageWidth / columnNum;

                    // 绘制標題
                    gfx.DrawString(example.CompanyName, titleFont, XBrushes.Black,
                        new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
                    yPoint += 15;

                    if (!string.IsNullOrEmpty(example.DocType))
                    {
                        gfx.DrawString(example.DocType, titleFont, XBrushes.Black,
                            new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
                        yPoint += 15;
                    }

                    //頁次
                    gfx.DrawString($"頁次: {pageNum}", pageFont, XBrushes.Black, new XRect(margin + 485, yPoint, 100, 20), XStringFormats.TopLeft);

                    //繪製表頭
                    // 兩兩分組
                    var pairs = example.PDFExampleHead.headItems.Select((value, index) => new { Index = index, Value = value })
                                                                .GroupBy(x => x.Index / 2)
                                                                .Select(g => g.Select(x => x.Value).ToList())
                                                                .ToList();
                    double marginLeft = 0;
                    foreach (var pair in pairs)
                    {
                        marginLeft = 0; //一行
                        foreach (var item in pair)
                        {
                            var txt = $"{item.ItemName}: {item.ItemValue}";
                            gfx.DrawString(txt, headerFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, 100, 20), XStringFormats.TopLeft);

                            marginLeft += pageWidth / pair.Count;
                        }

                        yPoint += 15; // 向下移动
                    }

                    yPoint += 3; // 向下移动
                    //線
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 3; //間隔
                    marginLeft = 0;
                    // 绘制表格欄位名稱

                    margin = 30;
                    foreach (var columns in example.PDFExampleBody.Columns)
                    {
                        foreach (var column in columns)
                        {
                            if (column.Align.ToLower() == "left") gfx.DrawString(column.ColumnName, pageFont, XBrushes.Black, new XRect(margin + marginLeft + 10, yPoint, column.Width, 20), XStringFormats.TopLeft);
                            else if (column.Align.ToLower() == "right") gfx.DrawString(column.ColumnName, pageFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, column.Width, 20), XStringFormats.TopRight);
                            marginLeft += column.Width;
                        }
                        marginLeft = 0;
                        yPoint += 10; //間隔
                    }



                    //線
                    margin = 40;
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 5; // 向下移动

                    margin = 30;
                    // 绘制表格内容
                    foreach (var row in example.PDFExampleBody.Rows.Select((value, index) => new { Index = index, Data = value }).ToList().GetRange(rowStartIndex, rowRemainNum))
                    {
                        marginLeft = 0;
                        int counter = 0;
                        var options = new JsonSerializerOptions
                        {
                            Converters = { new ObjectAsPrimitiveConverter(floatFormat: FloatFormat.Double, unknownNumberFormat: UnknownNumberFormat.Error, objectFormat: ObjectFormat.Expando) },
                            WriteIndented = true,
                        };
                        IDictionary<string, object> rowColumns = JsonSerializer.Deserialize<dynamic>(row.Data, options);
                        // 根據需求建構你需要的資料
                        var raw = new Dictionary<string, object>(rowColumns);

                        // 假設你想計算 count（假設我們需要扣除某些欄位）
                        int count = raw.Count - 5;  // 減去不需要的欄位數量

                        // 根據你提供的要求，構建排序的欄位順序
                        var orderedKeys = new List<string> { "parentID", "sizeNo" };
                        orderedKeys.AddRange(Enumerable.Range(1, count).Select(i => i.ToString()));
                        orderedKeys.AddRange(new[] { "num", "price", "amount" });

                        var orderedResult = new Dictionary<string, object>();

                        foreach (var key in orderedKeys)
                        {
                            // 檢查 raw[key] 是否為 null
                            if (raw.ContainsKey(key))
                            {
                                orderedResult[key] = raw[key] == null ? null : raw[key];
                            }
                            else
                            {
                                orderedResult[key] = null; // 若原資料中沒有這個 key，也設為 null
                            }
                        }


                        foreach (var column in orderedResult)
                        {
                            var valueType = column.Value == null ? typeof(string) : column.Value.GetType();
                            bool numberType = valueType == typeof(int) || valueType == typeof(double) || valueType == typeof(decimal) || valueType == typeof(float);
                            var largeLengthSizeColumn = example.PDFExampleBody.Columns.OrderByDescending(x => x.Count).First();
                            double width = largeLengthSizeColumn.ElementAt(counter).Width;
                            // now process the values
                            dynamic kvpValue = column.Value ?? "";
                            string value = kvpValue.ToString();
                            if (numberType && example.PDFExampleBody.DecimalPoint) value = kvpValue.ToString("N2");
                            else if (numberType) value = kvpValue.ToString("N0");
                            //超過長度裁切
                            string trimValue = value.Length > 30 ? value.Substring(0, 30) : value;
                            trimValue = Regex.IsMatch(trimValue, @"[\u4e00-\u9fff]{1,8}") && value.Length >= 15 ? trimValue.Substring(0, 15) : trimValue;
                            if (column.Key.Contains("num", StringComparison.OrdinalIgnoreCase)) gfx.DrawString(trimValue, pageFont, XBrushes.Black, new XRect(margin + marginLeft, yPoint, width, 20), XStringFormats.TopRight);
                            else gfx.DrawString(trimValue, pageFont, XBrushes.Black, new XRect(margin + marginLeft + 10, yPoint, width, 20), XStringFormats.TopLeft);

                            marginLeft += width;

                            counter++;
                        }
                        yPoint += 13; // 移动到下一行

                        if (row.Index + 1 == bodyRowNum)
                        {
                            printerEndEvent = true;
                        }
                        ;

                        double moveYPoint = example.PDFExampleBody.TaxOption ? 100 : 60;

                        if (yPoint + moveYPoint > pageHeight)
                        {
                            rowStartIndex = row.Index + 1;
                            rowRemainNum = bodyRowNum - rowStartIndex;
                            pageNum++;
                            break;
                        }
                        ;

                    }
                    margin = 40;
                    //線
                    gfx.DrawLine(XPens.Gray, new XPoint(margin, yPoint), new XPoint(pageWidth - margin, yPoint));
                    yPoint += 5; // 向下移动

                    marginLeft = 0;
                    margin = 30;
                    if (printerEndEvent && example.PDFExampleBody.Totals.Count > 0)
                    {
                        var column = example.PDFExampleBody.Columns[example.PDFExampleBody.Columns.Count - 1];
                        int startIndex = column.FindIndex(x => x.ColumnName == example.PDFExampleBody.Totals[0].ColumnName);
                        double witdth = startIndex > -1 ? column[startIndex].Width : 0;
                        double startMargin = startIndex > -1 ? column.GetRange(0, startIndex).Sum(x => x.Width) - 10 : 0;
                        gfx.DrawString("合計:", pageFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                        foreach (var item in example.PDFExampleBody.Totals)
                        {
                            var valueType = item.Total.GetType();
                            string value = string.Empty;
                            // now process the values
                            if (item.Total is JsonElement element)
                            {
                                if (element.ValueKind == JsonValueKind.String) value = item.Total.ToString(); // Extract the string value
                                else if (element.ValueKind == JsonValueKind.Number)
                                {
                                    decimal eleValue = element.TryGetDecimal(out decimal result) ? result : 0;
                                    if (example.PDFExampleBody.DecimalPoint) value = eleValue.ToString("N2");
                                    else value = eleValue.ToString("N0");

                                }
                            }
                            int eleIndex = column.FindIndex(x => x.ColumnName == item.ColumnName);
                            witdth = eleIndex > -1 ? column[eleIndex].Width : 0;
                            double startXpoint = column.GetRange(0, eleIndex).Sum(x => x.Width);
                            gfx.DrawString(value, pageFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                        }
                        if (example.PDFExampleBody.TaxOption)
                        {
                            yPoint += 15; // 向下移动
                            string taxStr = example.PDFExampleBody.DecimalPoint ? example.PDFExampleBody.Tax.ToString("N2") : example.PDFExampleBody.Tax.ToString("N0");
                            string amountStr = example.PDFExampleBody.DecimalPoint ? example.PDFExampleBody.Amount.ToString("N2") : example.PDFExampleBody.Amount.ToString("N0");
                            int eleIndex = example.PDFExampleBody.Column.FindIndex(x => x.ColumnName == example.PDFExampleBody.SummaryColumn);
                            witdth = eleIndex > -1 ? example.PDFExampleBody.Column[eleIndex].Width : 0;
                            double startXpoint = eleIndex > -1 ? column.GetRange(0, eleIndex).Sum(x => x.Width) : column.Sum(x => x.Width);
                            gfx.DrawString("稅額:", bodyFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                            gfx.DrawString(taxStr, bodyFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                            yPoint += 15; // 向下移动

                            gfx.DrawString("總計:", bodyFont, XBrushes.Black, new XRect(margin + startMargin, yPoint, 5, 20), XStringFormats.TopRight);
                            gfx.DrawString(amountStr, bodyFont, XBrushes.Black, new XRect(margin + startXpoint, yPoint, witdth, 20), XStringFormats.TopRight);
                        }
                        yPoint += 20; // 向下移动
                    }
                    string remark = example.PDFExampleBody.Remark;
                    double bottomRemarkYPoint = 350;
                    margin = 40;
                    gfx.DrawString("詳細說明:", bodyFont, XBrushes.Black, new XRect(margin, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                    int counter2 = 0;
                    while (!string.IsNullOrEmpty(remark))
                    {
                        int length = remark.Length;
                        string substr = remark.Substring(0, length);
                        XSize size = gfx.MeasureString(substr, bodyFont);


                        if (counter2 == 0)
                        {
                            while (size.Width > pageWidth - margin * 2 - 60)
                            {
                                length--;
                                substr = remark.Substring(0, length);
                                size = gfx.MeasureString(substr, bodyFont);
                            }
                            gfx.DrawString(remark.Substring(0, length), bodyFont, XBrushes.Black, new XRect(margin + 60, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                        }
                        else
                        {
                            while (size.Width > pageWidth - margin * 2)
                            {
                                length--;
                                substr = remark.Substring(0, length);
                                size = gfx.MeasureString(substr, bodyFont);
                            }
                            gfx.DrawString(remark.Substring(0, length), bodyFont, XBrushes.Black, new XRect(margin, bottomRemarkYPoint, 100, 20), XStringFormats.TopLeft);
                        }

                        yPoint += 10;
                        remark = remark.Substring(length).Trim();

                        if (yPoint + 30 > page.Height.Point)
                        {
                            remark = string.Empty;
                        }
                        counter2++;
                    }
                    double checkedYPoint = bottomRemarkYPoint + 20;
                    gfx.DrawString("經理:", pageFont, XBrushes.Black, new XRect(margin, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("主管:", pageFont, XBrushes.Black, new XRect(margin + 80, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("會計:", pageFont, XBrushes.Black, new XRect(margin + 160, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("客戶:", pageFont, XBrushes.Black, new XRect(margin + 240, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("送貨:", pageFont, XBrushes.Black, new XRect(margin + 320, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("倉庫:", pageFont, XBrushes.Black, new XRect(margin + 400, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString("業務:", pageFont, XBrushes.Black, new XRect(margin + 480, checkedYPoint, 100, 20), XStringFormats.TopLeft);
                }

                // 保存PDF
                var coverFilePath = Path.GetTempFileName() + Guid.NewGuid().ToString() + ".pdf";
                document.Save(coverFilePath);
                Process.Start(new ProcessStartInfo(coverFilePath) { UseShellExecute = true });
                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public void PrintECPickingList(PdfDocument document, ECPickingListPDFModel example)
        {
            //計算表身行數
            int bodyRowNum = example.ECPickingListBody.Rows.Count;
            bool printerEndEvent = false;
            int rowStartIndex = 0;
            int rowRemainNum = bodyRowNum;
            int pageNum = 1;
            // 创建一个新的 PDF 文档
            document.Info.Title = "PDF with Table Example";
            GlobalFontSettings.FontResolver = new FileFontResolver();

            while (!printerEndEvent)
            {
                // 创建一个页面
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(210);
                page.Height = XUnit.FromMillimeter(297);
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Left;
                XFont BigtitleFont = new XFont("標楷體", 20, XFontStyleEx.Bold);
                XFont titleFont = new XFont("標楷體", 16, XFontStyleEx.Bold);
                XFont headerFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                XFont bodyColumnHeaderFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                XFont bodyFont = new XFont("標楷體", 12, XFontStyleEx.Regular);
                XFont pageFont = new XFont("標楷體", 10, XFontStyleEx.Regular);
                XFont code128 = new XFont("code128", 25, XFontStyleEx.Regular);

                // 獲取頁面尺寸
                double pageWidth = page.Width;
                double pageHeight = page.Height;

                // 定义页面大小和初始位置
                double yPoint = 15;
                double margin = 40;
                double columnNum = example.ECPickingListBody.Column.Count > 1 ? example.ECPickingListBody.Column.Count - 1 : 1;
                double columnWidth = pageWidth / columnNum;

                // 绘制標題
                gfx.DrawString(example.CompanyName, bodyFont, XBrushes.Black,
                    new XRect(15, yPoint, pageWidth, 30), XStringFormats.TopLeft);
                //頁次
                gfx.DrawString($"頁次: {pageNum}", pageFont, XBrushes.Black, new XRect(margin + 485, yPoint, 100, 20), XStringFormats.TopLeft);
                yPoint += 15;


                if (!string.IsNullOrEmpty(example?.CompanyAddress))
                {
                    gfx.DrawString($"{example?.CompanyAddress} / {example?.CompanyTel}", pageFont, XBrushes.Black,
                        new XRect(15, yPoint, pageWidth, 30), XStringFormats.TopLeft);
                    yPoint += 15;
                }

                gfx.DrawString(example.DocType, BigtitleFont, XBrushes.Black,
                    new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
                gfx.DrawRectangle(XPens.Black, new XRect(369, yPoint-15, 100, 30));//(130/25.4)*72
                gfx.DrawString(example.ECPickingListHead.PlatformName, titleFont, XBrushes.Black,
                    new XRect(369, yPoint-15, 100, 30), XStringFormats.Center);
                yPoint += 15;

                yPoint += 5;
                gfx.DrawString(example.ECPickingListHead.BarCode, bodyFont, XBrushes.Black, new XRect(370, yPoint, 200, 20), XStringFormats.TopLeft);

                yPoint += 10;

                //create the barcode from string
                string barCode128Str = BarCodeHelper.BarCode128(example.ECPickingListHead.BarCode);
                gfx.DrawString(barCode128Str, code128, XBrushes.Black, new XRect(360, yPoint, 100, 150), XStringFormats.TopLeft);

                yPoint += 12;

                gfx.DrawString("收件人 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead.Recipient, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);


                yPoint += 12;

                gfx.DrawString("收件人電話 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead?.TelPhone ?? string.Empty, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);

                yPoint += 12;

                gfx.DrawString("送貨方式 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead?.ShippingMethod ?? string.Empty, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);

                gfx.DrawString($"列印日期 {example.ECPickingListHead.PrintDate}", pageFont, XBrushes.Black, new XRect(369, yPoint, 100, 20), XStringFormats.TopLeft);
                yPoint += 12;

                gfx.DrawString("店號 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead?.ClientID ?? string.Empty, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);

                yPoint += 12;

                gfx.DrawString("門市名稱 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead?.ClientName ?? string.Empty, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);

                yPoint += 12;

                gfx.DrawString("地址 ", pageFont, XBrushes.Black, new XRect(15, yPoint, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString(example.ECPickingListHead.RecipientAddress, pageFont, XBrushes.Black, new XRect(80, yPoint, 100, 20), XStringFormats.TopLeft);

                yPoint += 12;

                gfx.DrawLine(XPens.Gray, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));

                yPoint += 5;

                double marginLeft = 0;
                foreach (var columnSetting in example.ECPickingListBody.Column)
                {
                    if(columnSetting.Align == "left")
                    {
                        gfx.DrawString(columnSetting.ColumnName, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, 100, 20), XStringFormats.TopLeft);
                    }
                    else
                    {
                        gfx.DrawString(columnSetting.ColumnName, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, columnSetting.Width - 10, 20), XStringFormats.TopRight);
                    }
                    marginLeft += columnSetting.Width;
                }

                yPoint += 12;
                int index = 0;
                var column = example.ECPickingListBody.Column;
                foreach (var row in example.ECPickingListBody.Rows.GetRange(rowStartIndex, rowRemainNum))
                {
                    (string parentSKUTextWrap, int rowCount1) = AddSpacesByWidth(row.ParentSKU, 20);
                    (string goodIDTextWrap, int rowCount2) = AddSpacesByWidth(row.GoodID, 20);
                    (string goodNameTextWrap, int rowCount3)  = AddSpacesByWidth(row.GoodName, 28);
                    List<int> tempListStr = new List<int>() { rowCount1, rowCount2, rowCount3 };
                    int yPointOffsetBase = tempListStr.Max(x => x);
                    int nextyOffsetNum = 12 * yPointOffsetBase + 5;

                    if (yPoint + nextyOffsetNum + 100 > pageHeight)
                    {
                        rowStartIndex = index;
                        rowRemainNum = bodyRowNum - rowStartIndex;
                        pageNum++;
                        break;
                    } ;

                    marginLeft = 0;
                    gfx.DrawString((rowStartIndex + index + 1).ToString(), pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[0].Width, 20), XStringFormats.TopLeft);
                    marginLeft += column[0].Width;
                    gfx.DrawString(parentSKUTextWrap, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[1].Width, 20), XStringFormats.TopLeft);
                    marginLeft += column[1].Width;
                    tf.DrawString(goodIDTextWrap, pageFont, XBrushes.Black, new XRect(marginLeft, yPoint, column[2].Width, 40), XStringFormats.TopLeft);
                    marginLeft += column[2].Width;

                    tf.DrawString(goodNameTextWrap, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[3].Width, 40), XStringFormats.TopLeft);
                    marginLeft += column[3].Width;
                    string sellPrice = example.ECPickingListBody.DecimalPoint ? row.SellPrice.ToString("N2") : row.SellPrice.ToString("N0");
                    string sellAmount = example.ECPickingListBody.DecimalPoint ? row.SellAmount.ToString("N2") : row.SellAmount.ToString("N0");
                    gfx.DrawString(row.Quantity.ToString(), pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[4].Width - 10, 40), XStringFormats.TopRight);
                    marginLeft += column[4].Width; ;
                    gfx.DrawString(sellPrice, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[5].Width - 10, 20), XStringFormats.TopRight);
                    marginLeft += column[5].Width; ;
                    gfx.DrawString(sellAmount, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[6].Width - 10, 20), XStringFormats.TopRight);
                    marginLeft += column[6].Width;

                    yPoint += nextyOffsetNum;
                    index++;

                   
                }

                gfx.DrawLine(XPens.Gray, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));

                yPoint += 3;
                double totalNumMarginLeft = column[0].Width + column[1].Width + column[2].Width + column[3].Width;
                double totalAmountMarginLeft = column[0].Width + column[1].Width + column[2].Width + column[3].Width + column[4].Width + column[5].Width;
                gfx.DrawString(example.ECPickingListBody.TotalNum.ToString(), pageFont, XBrushes.Black, new XRect(15 + totalNumMarginLeft, yPoint, column[4].Width - 10, 20), XStringFormats.TopRight);
                gfx.DrawString(example.ECPickingListBody.Amount.ToString(), pageFont, XBrushes.Black, new XRect(15 + totalAmountMarginLeft, yPoint, column[6].Width - 10, 20), XStringFormats.TopRight);

                yPoint += 20;
                decimal totalAmountWithDiscountandShoppingFee = example.ECPickingListBody.Amount + example.ECPickingListBody.Discount + example.ECPickingListBody.ShippingFee;
                double sumMarginLeft = column[0].Width + column[1].Width + column[2].Width + column[3].Width + column[4].Width;
                gfx.DrawString("折扣", pageFont, XBrushes.Black, new XRect(15 + sumMarginLeft, yPoint, column[5].Width - 10, 20), XStringFormats.TopRight);
                gfx.DrawString(example.ECPickingListBody.Discount.ToString(), pageFont, XBrushes.Black, new XRect(15 + totalAmountMarginLeft, yPoint, column[6].Width - 10, 20), XStringFormats.TopRight);
                yPoint += 12;
                gfx.DrawString("運費", pageFont, XBrushes.Black, new XRect(15 + sumMarginLeft, yPoint, column[5].Width - 10, 20), XStringFormats.TopRight);
                gfx.DrawString(example.ECPickingListBody.ShippingFee.ToString(), pageFont, XBrushes.Black, new XRect(15 + totalAmountMarginLeft, yPoint, column[6].Width - 10, 20), XStringFormats.TopRight);
                yPoint += 12;
                gfx.DrawString("合計", pageFont, XBrushes.Black, new XRect(15 + sumMarginLeft, yPoint, column[5].Width - 10, 20), XStringFormats.TopRight);
                gfx.DrawString(totalAmountWithDiscountandShoppingFee.ToString(), pageFont, XBrushes.Black, new XRect(15 + totalAmountMarginLeft, yPoint, column[6].Width - 10, 20), XStringFormats.TopRight);

                if(rowStartIndex + index == bodyRowNum)
                {
                    printerEndEvent = true;
                }
            }

        }

        public async Task<bool> PrintECMasterPickingList(ECMasterPickingListPDFModel example)
        {
            PdfDocument document = new PdfDocument();
            //計算表身行數
            int bodyRowNum = example.ECMasterPickingListBody.Rows.Count;
            bool printerEndEvent = false;
            int rowStartIndex = 0;
            int rowRemainNum = bodyRowNum;
            int pageNum = 1;
            // 创建一个新的 PDF 文档
            document.Info.Title = "PDF with Table Example";
            GlobalFontSettings.FontResolver = new FileFontResolver();

            try
            {
                while (!printerEndEvent)
                {
                    // 创建一个页面
                    PdfPage page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(210);
                    page.Height = XUnit.FromMillimeter(297);
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont BigtitleFont = new XFont("標楷體", 20, XFontStyleEx.Bold);
                    XFont titleFont = new XFont("標楷體", 16, XFontStyleEx.Bold);
                    XFont headerFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyColumnHeaderFont = new XFont("標楷體", 14, XFontStyleEx.Bold);
                    XFont bodyFont = new XFont("標楷體", 12, XFontStyleEx.Regular);
                    XFont pageFont = new XFont("標楷體", 10, XFontStyleEx.Regular);

                    // 獲取頁面尺寸
                    double pageWidth = page.Width;
                    double pageHeight = page.Height;

                    // 定义页面大小和初始位置
                    double yPoint = 15;
                    double margin = 40;
                    

                    //頁次
                    gfx.DrawString($"頁次: {pageNum}", pageFont, XBrushes.Black, new XRect(margin + 485, yPoint, 100, 20), XStringFormats.TopLeft);


                    gfx.DrawString(example.DocType, BigtitleFont, XBrushes.Black,
                        new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);

                    yPoint += 25;

                    gfx.DrawLine(XPens.Gray, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));

                    yPoint += 5;

                    double marginLeft = 0;
                    foreach (var columnSetting in example.ECMasterPickingListBody.Column)
                    {
                        if (columnSetting.Align == "left")
                        {
                            gfx.DrawString(columnSetting.ColumnName, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, 100, 20), XStringFormats.TopLeft);
                        }
                        else
                        {
                            gfx.DrawString(columnSetting.ColumnName, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, columnSetting.Width, 20), XStringFormats.TopRight);
                        }
                        marginLeft += columnSetting.Width;
                    }
                    yPoint += 12;
                    gfx.DrawLine(XPens.Gray, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));
                    yPoint += 12;
                    int index = 0;
                    var column = example.ECMasterPickingListBody.Column;
                    foreach (var row in example.ECMasterPickingListBody.Rows.GetRange(rowStartIndex, rowRemainNum))
                    {

                        if (yPoint > pageHeight)
                        {
                            rowStartIndex = index;
                            rowRemainNum = bodyRowNum - rowStartIndex;
                            pageNum++;
                            break;
                        }
                        ;


                        marginLeft = 0;
                        gfx.DrawString((rowStartIndex + index + 1).ToString(), pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[0].Width, 20), XStringFormats.TopLeft);
                        marginLeft += column[0].Width;
                        gfx.DrawString(row.GoodID, pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[1].Width, 20), XStringFormats.TopLeft);
                        marginLeft += column[1].Width;
                        gfx.DrawString(row.Quantity.ToString(), pageFont, XBrushes.Black, new XRect(15 + marginLeft, yPoint, column[2].Width, 40), XStringFormats.TopRight);
                        marginLeft += column[2].Width;

                        yPoint += 15;
                        XPen dashPen = new XPen(XColors.Black, 0.5); // 黑色，粗細為 1
                        dashPen.DashStyle = XDashStyle.Solid; // 使用預設虛線樣式

                        gfx.DrawLine(dashPen, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));

                        yPoint += 6;

                        index++;

                    }

                    gfx.DrawLine(XPens.Gray, new XPoint(15, yPoint), new XPoint(pageWidth - 20, yPoint));


                    if (rowStartIndex + index == bodyRowNum)
                    {
                        printerEndEvent = true;
                    }
                }

                var coverFilePath = Path.GetTempFileName() + Guid.NewGuid().ToString() + ".pdf";
                document.Save(coverFilePath);
                Process.Start(new ProcessStartInfo(coverFilePath) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                var c = e.Message;
                return false;

            }

            return true;

        }

        public async Task<bool> PrintECPickingListMulti(ECPickingListMultiPDFModel data)
        {
            try
            {
                PdfDocument document = new PdfDocument();

                foreach (var item in data.Examples)
                {
                    item.CompanyName = data.CompanyName;
                    item.CompanyAddress = data.CompanyAddress;
                    item.CompanyTel = data.CompanyTel;
                    item.DocType = data.DocType;
                    PrintECPickingList(document, item);
                }
                var coverFilePath = Path.GetTempFileName() + Guid.NewGuid().ToString() + ".pdf";
                document.Save(coverFilePath);
                Process.Start(new ProcessStartInfo(coverFilePath) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                return false;
            }
        

            return true;
        }

        public (string, int) AddSpacesByWidth(string input, int maxWidth)
        {
            StringBuilder result = new StringBuilder();
            int currentWidth = 0;  // 当前宽度
            int rowCount = 1;
            foreach (char c in input)
            {
                int charWidth = GetCharWidth(c);

                // 如果当前宽度加上字符宽度超过最大宽度，插入一个空格
                if (currentWidth + charWidth > maxWidth)
                {
                    result.Append(" ");  // 插入空格
                    currentWidth = 0;    // 重置当前宽度
                    rowCount++;
                }

                result.Append(c);  // 添加字符到结果
                currentWidth += charWidth;  // 更新当前宽度
            }

            return (result.ToString(), rowCount);
        }

        private int GetCharWidth(char c)
        {
            // 如果是中文字符，宽度为2
            if (c >= 0x4e00 && c <= 0x9fff)
            {
                return 2;
            }
            // 如果是英文字符，宽度为1
            else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                return 1;
            }
            // 对其他字符设定默认宽度为1
            else
            {
                return 1;
            }
        }
    }
}
