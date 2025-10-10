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
using static LocalPortService.Model.API.PrinterSupportLanguage;
using System.Runtime.InteropServices;

namespace LocalPortService.BizService.PrinterInformation
{
    public class PrinterService
    {
        private readonly IConfiguration _config;
        public PrinterService(IConfiguration config)
        {
            _config = config;
        }
        public string GetPrinter(PrinterExample example)
        {
           // 測試 ESC/POS 支援
           if (SendCommandToPrinter(example.PrinterName, "^XA^XZ")) // 測試 ZPL 支援
           {
             return "Support ZPL";
           } else if (SendCommandToPrinter(example.PrinterName, "\x1B\x40")) // 測試 ESC/POS 支援
           {
             return "Support ESC/POS";
           } else if (SendCommandToPrinter(example.PrinterName, "! 0 200 200 210 1")) // 測試 SPL 支援
           {
             return "Support SPL";
           } else
           {
             return null;
           }
        }
    }
}
