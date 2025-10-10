using ESC_POS_USB_NET.Printer;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LocalPortService.Model.API;
using LocalPortService.Model.CardMachine;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Diagnostics;
using LocalPortService.Core.Helper;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using System.Net;
using ESC_POS_USB_NET.EpsonCommands;
using System.Reflection.Emit;
using System.Diagnostics.Eventing.Reader;


namespace LocalPortService.BizServices.SaleMgmt
{
    public class SaleServices
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public SaleServices(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }



        public string LoadTemplate(string templateFile)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory + $"BizService/PrintTemplate/{templateFile}");
            if (!File.Exists(filePath)) throw new FileNotFoundException("找不到模板");

            return File.ReadAllText(filePath);
        }

        public string RenderWithHelpers(string templateText, object data)
        {
            var handlebars = Handlebars.Create(); // 建立實例

            handlebars.RegisterHelper("padRight", (writer, context, parameters) =>
            {
                string text = parameters[0]?.ToString() ?? "";
                int width = int.Parse(parameters[1]?.ToString() ?? "10");
                writer.WriteSafeString(text.PadRight(width));
            });

            handlebars.RegisterHelper("padLeft", (writer, context, parameters) =>
            {
                string text = parameters[0]?.ToString() ?? "";
                int width = int.Parse(parameters[1]?.ToString() ?? "10");
                writer.WriteSafeString(text.PadLeft(width));
            });

            var template = handlebars.Compile(templateText);
            return template(data);
        }

        public int GetVisualLength(string str)
        {
            int length = 0;
            foreach (char c in str)
            {
                length += c > 127 ? 2 : 1;
            }
            return length;
        }

        public string PadRightVisual (string input, int totalVisualLength)
        {
            int currentLength = GetVisualLength(input);
            int spaces = totalVisualLength - currentLength;
            return new string(' ', spaces);
        }

        public string PadLeftPriceVisual(string goods, string num, int totalVisualLength)
        {
            int currentLength = GetVisualLength(num);
            int goodsLength = GetVisualLength(goods);
            int spaces = totalVisualLength - currentLength + goodsLength;
            if (spaces <= 0) return num;
            return new string(' ', spaces) + num;
        }

        public string PadLeftVisual(string num, int totalVisualLength)
        {
            int currentLength = num.Length;
            int spaces = totalVisualLength - currentLength;
            if (spaces <= 0) return num;
            return new string(' ', spaces) + num;
        }

        public async Task<bool> ExecThermalPrinter((SaleHeaderPrinter, List<SaleDetailPrinter>) data)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var saleHeader = data.Item1;
            var saleDetail = data.Item2?.Count > 0 ? data.Item2 : null;
            var templateFile = data.Item1.TemplateFileName;
            var logo = data.Item1.LogoSrc.Split(',')[1].Trim();

            string QrCodedata1 = data.Item1.FBUrl;
            string QrCodedata2 = data.Item1.IGUrl;

            int qrcodeLength = 20;

            if (saleDetail is null) return false;
            string defaultPrinter = new PrinterSettings().PrinterName;

            // 初始化印表機
            Printer printer = new Printer(defaultPrinter);
            printer.InitializePrint();
            printer.Append(new byte[]
            {
            0x1D, 0x57,
            0x40, 0x02
            });

            // 讀取並套用模板
            var templateText = LoadTemplate(templateFile);
            var totalText = "合計:".PadLeft(20) + saleDetail.Sum(x => x.SellCash).ToString().PadLeft(7);

            var splitGoodDetail = new List<string>();

            string CleanBase64String(string base64)
            {
                // 移除所有非Base64合法字符
                string cleaned = System.Text.RegularExpressions.Regex.Replace(
                    base64,
                    @"[^a-zA-Z0-9\+/=]",
                    ""
                );

                // 确保长度是4的倍数
                int mod4 = cleaned.Length % 4;
                if (mod4 > 0)
                {
                    cleaned += new string('=', 4 - mod4);
                }

                return cleaned;
            }

            foreach (var item in saleDetail)
            {
                var goodName = item.GoodName ?? "";
                var nameLines = Enumerable.Range(0, (int)Math.Ceiling(goodName.Length / 11.0))
                               .Select(i => goodName.Substring(i * 11, Math.Min(11, goodName.Length - i * 11)))
                               .ToList();
                var goodNameSpace = PadRightVisual(nameLines[0], 22);
                var goodID = item.GoodID ?? item.GoodID;
                var price = PadLeftPriceVisual(goodNameSpace, item.SellPrice.ToString("N0"), 6);
                var qty = PadLeftVisual(item.Qty.ToString("N0"), 5);
                var subtotal = PadLeftVisual(item.SellCash.ToString("N0"), 9);
                var sbuilder = new StringBuilder();
                for (int i = 0; i < nameLines.Count; i++)
                {
                    if (i == 0)
                    {
                        sbuilder.Append(nameLines[i]);
                        sbuilder.Append(price);
                        sbuilder.Append(qty);
                        sbuilder.Append(subtotal);
                    }
                    else
                    {
                        sbuilder.AppendLine();
                        sbuilder.Append(nameLines[i]);
                    }
                    if (i == nameLines.Count() - 1)
                    {
                        sbuilder.AppendLine();
                        sbuilder.Append(goodID);
                    }
                }

                splitGoodDetail.Add(sbuilder.ToString());
            }

            int total = 0;
            int totalNum = 0;

            foreach (var item in saleDetail)
            {
                if (item.GoodStyle != "00" && item.GoodStyle != "04")
                {
                    total += item.SellCash;
                }
                else
                {
                    total += item.SellCash;
                    totalNum += item.Qty;
                }
            }

            var tempData = new
            {
                SellStore = saleHeader.SellStore,
                SellID = saleHeader.SellID,
                SellDate = saleHeader.SellDate,
                SplitGoodDetail = splitGoodDetail,
                TerminalID = saleHeader.TerminalID,
                Total = total.ToString("N0"),
                TotalNum = totalNum.ToString("N0"),
                TotalLine = totalText,
                Separator = new string('─', 21),
                FBUrl = QrCodedata1,
                IGUrl = QrCodedata2,
            };

            string result = RenderWithHelpers(templateText, tempData);

            foreach (var line in result.Split('\n'))
            {
                var trimmed = line;

                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.StartsWith("[BARCODE:"))
                {
                    printer.Append(new byte[] { 0x1B, 0x24, 0x7F, 0x00 });
                    var code = trimmed.Replace("[BARCODE:", "").Replace("]", "").Trim();
                    printer.Append(SetBarcodeWidth(2));
                    printer.Append(SetBarcodeHeight(50));
                    printer.Append(PrintBarcode128(code));
                }
                else if (line.Trim() == ("[LOGO]"))
                {
                    printer.Append(new byte[] { 0x1B, 0x61, 0x01 });
                    string cleanedBase64 = CleanBase64String(logo);
                    byte[] logoImageBytes = Convert.FromBase64String(logo);
                    var logoImage = new MemoryStream(logoImageBytes);
                    Bitmap logoBitmap = new Bitmap(logoImage);

                    byte[] escposLogo = ConvertBitmapToRaster(logoBitmap);
                    printer.Append(escposLogo);
                }
                else if (trimmed.StartsWith("[QRCODE:"))
                {
                    printer.Append(new byte[] { 0x1B, 0x40 }); // 初始化
                    printer.Append(new byte[] { 0x1B, 0x4C }); // 進入 Page Mode
                    printer.Append(new byte[] { 0x1B, 0x54, 0x00 }); // 列印方向（左至右）
                    printer.Append(new byte[] { 0x1D, 0x50, 0x00, 0xCB }); // 移動單位
                    printer.Append(new byte[] { 0x1B, 0x57, 0x00, 0x00, 0x00, 0x00, 0x40, 0x02, 0xA0, 0x00 }); // 設定列印區域

                    var qrMatches = Regex.Matches(trimmed, @"\[QRCODE:(.*?)\]");
                    int index = 0;
                    byte[] setHorizontalPosition = new byte[] { 0x1B, 0x24, 0x30, 0x00 };

                    foreach (Match match in qrMatches)
                    {
                        var qrData = match.Groups[1].Value.Trim();

                        // 設定 QRCode 型式 (Model 2)
                        printer.Append(new byte[] { 0x1D, 0x28, 0x6B, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 });

                        // 設定不同的水平位置
                        if (index == 0)
                        {
                            printer.Append(new byte[] { 0x1D, 0x24, 0x00, 0x00 });
                            printer.Append(new byte[] { 0x1B, 0x24, 0x7C, 0x00 });
                            printer.Append(PrintMediaQrCode(qrData));
                        }
                        else if (index == 1)
                        {
                            printer.Append(new byte[] { 0x1D, 0x24, 0x00, 0x00 });
                            printer.Append(new byte[] { 0x1B, 0x24, 0x5C, 0x01 });
                            printer.Append(PrintMediaQrCode(qrData));
                        }
                        index++;
                    }
                    printer.Append(new byte[] { 0x0C });
                    printer.Append(new byte[] { 0x1B, 0x40 });
                    printer.Append(new byte[] { 0x1B, 0x61, 0x00 });
                    await Task.Delay(1000);
                }
                else if (trimmed.StartsWith("[SocialMedia:"))
                {
                    var matches = Regex.Matches(trimmed, "\"(.*?)\"");
                    printer.Append(new byte[] { 0x1B, 0x4D, 0x00 }); // 字體 A
                    printer.Append(new byte[] { 0x1B, 0x61, 0x00 }); // 左對齊
                    for (int i = 0; i < matches.Count; i++)
                    {
                        if (i == 0)
                        {
                            string media1 = matches[i].Groups[1].Value;
                            string spaces = new string(' ', 12);
                            printer.Append(GetByteArr(spaces + media1));

                        }
                        else if (i == 1)
                        {
                            string media2 = matches[i].Groups[1].Value;
                            string spaces = new string(' ', 10);
                            printer.Append(GetByteArr(spaces + media2));
                        }
                    }
                }
                else
                {
                    printer.Append(new byte[] { 0x1B, 0x61, 0x00 });
                    var decoded = WebUtility.HtmlDecode(trimmed);
                    printer.Append(GetByteArr(decoded));
                }

                if (!trimmed.StartsWith("[SocialMedia:"))
                {
                    printer.NewLine();
                }
            }

            printer.FullPaperCut();
            printer.PrintDocument();

            return true;
        }

        public async Task<bool> ExecThermalPrinterLittle((SaleHeaderPrinter, List<SaleDetailPrinter>) data)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var saleHeader = data.Item1;
            var saleDetail = data.Item2?.Count > 0 ? data.Item2 : null;
            var templateFile = data.Item1.TemplateFileName;
            var logo = data.Item1.LogoSrc.Split(',')[1].Trim();

            string QrCodedata1 = data.Item1.FBUrl;
            string QrCodedata2 = data.Item1.IGUrl;

            int qrcodeLength = 20;

            if (saleDetail is null) return false;
            string defaultPrinter = new PrinterSettings().PrinterName;

            // 初始化印表機
            Printer printer = new Printer(defaultPrinter);
            printer.InitializePrint();
            printer.Append(new byte[]
            {
                    0x1D, 0x57,
                    0x7C, 0x01
            });

            // 讀取並套用模板
            var templateText = LoadTemplate(templateFile);
            var totalText = "合計:".PadLeft(20) + saleDetail.Sum(x => x.SellCash).ToString("N0").PadLeft(7);

            var splitGoodDetail = new List<string>();
            var notSplitDetail = new List<object>();

            string CleanBase64String(string base64)
            {
                // 移除所有非Base64合法字符
                string cleaned = System.Text.RegularExpressions.Regex.Replace(
                    base64,
                    @"[^a-zA-Z0-9\+/=]",
                    ""
                );

                // 确保长度是4的倍数
                int mod4 = cleaned.Length % 4;
                if (mod4 > 0)
                {
                    cleaned += new string('=', 4 - mod4);
                }

                return cleaned;
            }

            foreach (var item in saleDetail)
            {
                var goodID = item.GoodID ?? "";
                var nameLines = Enumerable.Range(0, (int)Math.Ceiling(goodID.Length / 11.0))
                               .Select(i => goodID.Substring(i * 11, Math.Min(11, goodID.Length - i * 11)))
                               .ToList();
                var goodNameSpace = PadRightVisual(nameLines[0], 17);
                //var goodID = item.GoodID ?? item.GoodID;
                var price = PadLeftPriceVisual(goodNameSpace, item.SellPrice.ToString("N0"), 2);
                var qty = PadLeftVisual(item.Qty.ToString("N0"), 4);
                var Sort = item.Sort;
                var discount = item.Discount;
                var advicePrice = item.AdvicePrice;
                var subtotal = PadLeftVisual(item.SellCash.ToString("N0"), 7);
                var sbuilder = new StringBuilder();
                for (int i = 0; i < nameLines.Count; i++)
                {
                    if (i == 0)
                    {
                        sbuilder.Append(nameLines[i]);
                        sbuilder.Append(price);
                        sbuilder.Append(qty);
                        sbuilder.Append(subtotal);
                    }
                    else
                    {
                        sbuilder.AppendLine();
                        sbuilder.Append(nameLines[i]);
                    }
                }

                splitGoodDetail.Add(sbuilder.ToString());
            }

            foreach (var item in saleDetail)
            {
                var goodID = item.GoodID ?? "";
                var price = PadLeftVisual(item.SellPrice.ToString("N0"), 5);
                var qty = PadLeftVisual(item.Qty.ToString("N0"), 4);
                var sort = item.Sort != null ? item.Sort.ToString() : "";
                var discount = item.Discount != null ? PadLeftVisual($"{Math.Round(item.Discount, 0).ToString()}%", 6) : "0%";
                var advicePrice = item.AdvicePrice != null ? PadLeftVisual(item.AdvicePrice.ToString("N0"), 7) : "0";
                var subtotal = item.SellCash != null ? PadLeftVisual(item.SellCash.ToString("N0"), 7) : "0";

                notSplitDetail.Add(new
                {
                    GoodID = goodID,
                    Sort = sort,
                    AdvicePrice = advicePrice,
                    Qty = qty,
                    Discount = discount,
                    Subtotal = subtotal
                });
            }

            int total = 0;
            int totalNum = 0;

            foreach (var item in saleDetail)
            {
                if (item.GoodStyle != "00" && item.GoodStyle != "04")
                {
                    total += item.SellCash;
                }
                else
                {
                    total += item.SellCash;
                    totalNum += item.Qty;
                }
            }

            var tempData = new
            {
                SellStore = saleHeader.SellStore,
                SellID = saleHeader.SellID,
                SellDate = saleHeader.SellDate,
                SellPerson = saleHeader.SellPerson,
                SplitGoodDetail = splitGoodDetail,
                NotSplitDetail = notSplitDetail,
                TerminalID = saleHeader.TerminalID,
                Total = total.ToString("N0"),
                TotalNum = totalNum.ToString("N0"),
                TotalLine = totalText,
                Separator = new string('─', 15),
                Remark = saleHeader?.Remark
            };

            string result = RenderWithHelpers(templateText, tempData);

            foreach (var line in result.Split('\n'))
            {
                var trimmed = line;

                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.StartsWith("[BARCODE:"))
                {
                    printer.Append(new byte[] { 0x1B, 0x24, 0x00, 0x00 });
                    var code = trimmed.Replace("[BARCODE:", "").Replace("]", "").Trim();
                    printer.Append(SetBarcodeWidth(2));
                    printer.Append(SetBarcodeHeight(50));
                    printer.Append(PrintBarcode128(code));
                }
                else if (line.Trim() == ("[LOGO]"))
                {
                    printer.Append(new byte[] { 0x1B, 0x61, 0x01 });
                    string cleanedBase64 = CleanBase64String(logo);
                    byte[] logoImageBytes = Convert.FromBase64String(logo);
                    var logoImage = new MemoryStream(logoImageBytes);
                    Bitmap logoBitmap = new Bitmap(logoImage);

                    byte[] escposLogo = ConvertBitmapToRaster(logoBitmap);
                    printer.Append(escposLogo);
                    printer.NewLine();
                }
                else
                {
                    printer.Append(new byte[] { 0x1B, 0x61, 0x00 });
                    var decoded = WebUtility.HtmlDecode(trimmed);
                    printer.Append(GetByteArr(decoded));
                    printer.NewLine();
                }
            }

            printer.FullPaperCut();
            printer.PrintDocument();

            return true;
        }


        public Bitmap MergeQrCodes(Bitmap left, Bitmap right)
        {
            int width = left.Width + right.Width;
            int height = Math.Max(left.Height, right.Height);
            Bitmap merged = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(merged))
            {
                g.Clear(Color.White);
                g.DrawImage(left, 0, 0);
                g.DrawImage(right, left.Width, 0);
            }

            return merged;
        }

        public static Bitmap ConvertToMonochrome(Bitmap original)
        {
            Bitmap mono = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color c = original.GetPixel(x, y);
                    double brightness = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    Color newColor = brightness < 200 ? Color.Black : Color.White;

                    mono.SetPixel(x, y, newColor);
                }
            }

            return mono;
        }

        public static byte[] ConvertBitmapToRaster(Bitmap bmp, int maxWidth = 277)
        {
            if (bmp.Width > maxWidth)
            {
                double scale = (double)maxWidth / bmp.Width;
                int newHeight = (int)(bmp.Height * scale);
                bmp = new Bitmap(bmp, new Size(maxWidth, newHeight));
            }

            // 轉成黑白點陣
            bmp = ConvertToMonochrome(bmp);

            int width = bmp.Width;
            int height = bmp.Height;

            int bytesPerLine = (width + 7) / 8;

            List<byte> escPosCommand = new List<byte>();

            escPosCommand.Add(0x1D); // GS
            escPosCommand.Add(0x76); // 'v'
            escPosCommand.Add(0x30); // '0'
            escPosCommand.Add(0x00); // mode: normal

            // xL xH = 圖片寬度（bytes per line）
            escPosCommand.Add((byte)(bytesPerLine % 256));
            escPosCommand.Add((byte)(bytesPerLine / 256));

            // yL yH = 圖片高度（dots）
            escPosCommand.Add((byte)(height % 256));
            escPosCommand.Add((byte)(height / 256));

            // 圖片資料（點陣）
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < bytesPerLine; x++)
                {
                    byte b = 0x00;

                    for (int bit = 0; bit < 8; bit++)
                    {
                        int px = x * 8 + bit;
                        if (px < width)
                        {
                            Color pixel = bmp.GetPixel(px, y);
                            // 如果是黑點，就寫入 bit
                            if (pixel.R == 0)
                            {
                                b |= (byte)(0x80 >> bit);
                            }
                        }
                    }

                    escPosCommand.Add(b);
                }
            }
            return escPosCommand.ToArray();
        }

        private byte[] GetByteArr(string str)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoder = System.Text.Encoding.GetEncoding(950); //Sewoo SLK-TL120 印表機設定裡面有寫950

            return encoder.GetBytes(str);
        }

        private string FormatThousands(string data)
        {
            if (data.Length > 3)
            {
                return Regex.Replace(data, @"\B(?=(\d{3})+(?!\d))", ",");
            }
            return data;
        }

        private string FormatLine(string GoodID, float SellPrice, float TotalSellNum, float SellCash)
        {
            // 设定每个字段的起始位置
            int SellIDStart = 0;
            int sizeStart = 15;
            int totalSellNumStart = 22;
            int sellCashStart = 28;

            // 构建格式化的字符串
            StringBuilder line = new StringBuilder();
            line.Append(GoodID.PadRight(14));
            line.Append(FormatThousands(SellPrice.ToString()).PadLeft(4));
            line.Append(" ");
            line.Append(FormatThousands(TotalSellNum.ToString()).PadLeft(4));
            line.Append(" ");
            line.Append(FormatThousands(SellCash.ToString()).PadLeft(6));

            return line.ToString();
        }

        private string FormatReturnLine(string GoodID, float SellPrice, float TotalSellNum, float SellCash)
        {
            var sbuilder = new StringBuilder();
            var nameLines = Enumerable.Range(0, (int)Math.Ceiling(GoodID.Length / 11.0))
                               .Select(i => GoodID.Substring(i * 11, Math.Min(11, GoodID.Length - i * 11)))
                               .ToList();
            for (int i = 0; i < nameLines.Count; i++)
            {
                if (i == 0)
                {
                    sbuilder.Append(nameLines[i].PadRight(12));
                    sbuilder.Append(FormatThousands(SellPrice.ToString()).PadLeft(6));
                    sbuilder.Append(FormatThousands(TotalSellNum.ToString()).PadLeft(5));
                    sbuilder.Append(FormatThousands(SellCash.ToString()).PadLeft(7));
                }
                else
                {
                    sbuilder.AppendLine();
                    sbuilder.Append(nameLines[i].PadRight(12));
                    sbuilder.Append("".PadRight(18));
                }
            }

            return sbuilder.ToString();
        }

        private string FormatLineTotal(string totalStr, int quantity)
        {
            // 设定每个字段的起始位置
            int start = 0;
            int totalStart = 18;
            int quantityStart = 28;

            // 构建格式化的字符串
            StringBuilder line = new StringBuilder();
            line.Append(string.Empty.PadRight(totalStart));
            line.Append(totalStr);
            line.Append(quantity.ToString().PadLeft(7));

            return line.ToString();
        }

        public byte[] SetBarcodeWidth(int width)
        {
            byte[] command = { 0x1D, 0x77, (byte)width };
            return command;
        }

        public byte[] SetBarcodeHeight(int height)
        {
            byte[] command = { 0x1D, 0x68, (byte)height };
            return command;
        }

        public byte[] PrintBarcode39(string data)
        {
            byte[] command = new byte[data.Length + 4];
            command[0] = 0x1D;
            command[1] = 0x6B;
            command[2] = (byte)69; // 条码类型，69 表示 Code39
            command[3] = (byte)data.Length; // 条码数据长度
            Encoding.ASCII.GetBytes(data).CopyTo(command, 4);
            return command;
        }
        private byte[] PrintBarcode128(string barcode)
        {
            byte type = 0x49; //CODE128
            byte gs = 0x1D;
            byte printBarcode = 0x6B;
            byte code = 0x42; //CODE_B

            barcode = barcode.Replace("{", "{{");
            barcode = $"{(char)0x7B}{(char)code}" + barcode;

            var command = new List<byte> { gs, printBarcode, (byte)type, (byte)barcode.Length };
            command.AddRange(barcode.ToCharArray().Select(x => (byte)x));
            return command.ToArray();
        }

        public void PrintTwoQRCode(string data1, string data2)
        {
            byte[] setHorizontalPosition = new byte[] { 0x1B, 0x24, 0x30, 0x00 };
            byte[] setHorizontalPosition2 = new byte[] { 0x1B, 0x24, 0xF0, 0x00 }; //ESC $ nL nH x軸
            byte[] setPrintPosition = new byte[] { 0x1D, 0x24, 0x00, 0x00 }; // GS $ nL nH  y軸
            var list = new List<byte>();
            list.AddRange(new byte[] { 0x1b, 0x40 }); //init

            list.AddRange(new byte[] { 0x1b, 0x4C }); //start page mode
            list.AddRange(new byte[] { 0x1D, 0x50, 0x00, 0xCB }); //Set horizontal and vertical motion units
            list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0x58, 0x02 }); //Set print area in page mode
            list.AddRange(new byte[] { 0x1b, 0x54, 0x00 }); //Select print direction in page mode

            list.AddRange(setHorizontalPosition);
            list.AddRange(PrintQrCode(data1));

            list.AddRange(setPrintPosition);
            list.AddRange(setHorizontalPosition2);
            list.AddRange(PrintQrCode(data2));

            list.AddRange(new byte[] { 0x0C }); //Print and return to standard mode(in page mode)

            list.AddRange(new byte[] { 0x1B, 0x4C }); //end page mode
            list.AddRange(new byte[] { 0x1D, 0x56, 0x41, 0x00 }); //cut paper

            var rawData = list.ToArray();
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            IntPtr pointer = handle.AddrOfPinnedObject();
            RawPrinterHelper.SendBytesToPrinter("SLK-TL120", pointer, rawData.Length);
        }
        public bool PrintInvoice(SalesInvoice dto)
        {
            try
            {
                string imgLogo = _config["Invoice:InvoiceLogoName"];
                string companyName = _config["Invoice:InvoiceCompanyName"];
                string seller = dto.Seller;
                string randomCode = dto.RandomCode;
                string invoiceCode = $"{dto.InvoiceCode.Substring(0, 2)}-{dto.InvoiceCode.Substring(2)}"; ;
                decimal total = dto.InvoiceAmount;
                string twyear = dto.InvoiceTerm.Substring(0, 3);
                string monthED = dto.InvoiceTerm.Substring(3, 2);
                string monthST = (int.Parse(monthED) - 1).ToString().PadLeft(2, '0');
                string barCode = $"{twyear}{monthED}{invoiceCode.Replace("-", "")}{randomCode}";
                string twDate = $"{dto.InvoiceDate.Year}/{dto.InvoiceDate.Month.ToString().PadLeft(2,'0')}/{dto.InvoiceDate.Day.ToString().PadLeft(2, '0')}";

                string sellStore = dto.SellStore;
                string formatTelPhone = "";
                if (dto.TelPhone != null && dto.TelPhone != "")
                {
                    if (dto.TelPhone.Contains("-"))
                    {
                        string[] phone = dto.TelPhone.Split('-');
                        formatTelPhone = $"{phone[0]}{phone[1]}";
                    }
                    else
                    {
                        formatTelPhone = dto.TelPhone;
                    }
                }
                string telPhone = formatTelPhone != "" ? $"({formatTelPhone.Substring(0, 2)}){formatTelPhone.Substring(2, 4)}-{formatTelPhone.Substring(6)}" : "";

                string QrCodedata1 = dto.QrCodedata1;
                string QrCodedata2 = dto.QrCodedata2 ?? string.Empty;

                int targetLength1 = Math.Max(QrCodedata1.Length, QrCodedata2.Length);
                int targetLength2 = targetLength1 + 7;

                var list = new List<byte>();
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[]
                {
                0x1D, 0x57, // GS W 设置打印区域
                0x7C, 0x01 // 宽度为 300 点
                });

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x30 }); //大文字（默認）
                if (!string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string imagePath = Path.Combine(executableLocation, imgLogo);
                    if (File.Exists(imagePath))
                    {
                        list.AddRange(ConvertImageToESCPosData(imagePath, 500, 60));
                    }
                    else
                    {
                        list.AddRange(GetByteArr($"{companyName}"));
                        list.AddRange(new byte[] { 0x0A }); //NewLine
                    }
                }
                else if (string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    list.AddRange(GetByteArr($"{companyName}"));
                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }
                list.AddRange(GetByteArr("電子發票證明聯"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1b, 0x21, 0x38 }); //大文字(強調+雙倍高度+雙倍寬度)
                list.AddRange(GetByteArr($"{twyear}年{monthST}-{monthED}月"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr(invoiceCode));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x00 }); //font A
                list.AddRange(GetByteArr(dto.InvoiceDate.ToString("yyyy/MM/dd HH:mm:ss")));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"隨機碼:{randomCode}  總計:{total}元"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                if (string.IsNullOrEmpty(dto.Buyer))
                {
                    list.AddRange(GetByteArr($"賣方:{seller}"));
                }
                else
                {
                    list.AddRange(GetByteArr($"賣方:{seller}  買方:{dto.Buyer}"));
                }
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(SetBarcodeWidth(1));
                list.AddRange(SetBarcodeHeight(60));
                list.AddRange(PrintBarcode39($"*{barCode.PadRight(15, ' ')}*"));

                //page mode start
                byte[] setHorizontalPosition = new byte[] { 0x1B, 0x24, 0x20, 0x00 }; //ESC $ nL nH x軸
                byte[] setHorizontalPosition2 = new byte[] { 0x1B, 0x24, 0xE0, 0x00 }; //ESC $ nL nH x軸
                byte[] setPrintPosition = new byte[] { 0x1D, 0x24, 0x00, 0x00 }; // GS $ nL nH  y軸
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[] { 0x1b, 0x4C }); //start page mode
                list.AddRange(new byte[] { 0x1D, 0x50, 0x00, 0xCB }); //Set horizontal and vertical motion units
                                                                      //list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0xD6, 0x01 }); //Set print area in page mode 460dots
                list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0x9F, 0x00 }); //Set print area in page mode 277dots
                list.AddRange(new byte[] { 0x1b, 0x54, 0x00 }); //Select print direction in page mode

                list.AddRange(setHorizontalPosition);
                list.AddRange(PrintQrCode(QrCodedata1.PadRight(targetLength1, ' ')));

                list.AddRange(setPrintPosition);
                list.AddRange(setHorizontalPosition2);
                list.AddRange(PrintQrCode(QrCodedata2.PadRight(targetLength2, ' ')));

                list.AddRange(new byte[] { 0x0C }); //Print and return to standard mode(in page mode)
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"門市: {sellStore} {telPhone}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"換貨憑電子發票證明聯正本辦理"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                //list.AddRange(new byte[] { 0x0C }); //print
                list.AddRange(new byte[] { 0x1D, 0x56, 0x41, 0x00 }); //cut paper

                var rawData = list.ToArray();
                GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
                IntPtr pointer = handle.AddrOfPinnedObject();
                RawPrinterHelper.SendBytesToPrinter("SLK-TL120", pointer, rawData.Length);

                return true;
            }
            catch (Exception e)
            {

                return false;
            }
            
        }

        public bool PrintInvoiceAndDetail(SalesInvoiceAndDetail dto)
        {
            try
            {
                string imgLogo = _config["Invoice:InvoiceLogoName"];
                string companyName = _config["Invoice:InvoiceCompanyName"];
                string seller = dto.Invoice.Seller;
                string randomCode = dto.Invoice.RandomCode;
                string invoiceCode = $"{dto.Invoice.InvoiceCode.Substring(0, 2)}-{dto.Invoice.InvoiceCode.Substring(2)}"; ;
                decimal total = dto.Invoice.InvoiceAmount;
                string twyear = dto.Invoice.InvoiceTerm.Substring(0, 3);
                string monthED = dto.Invoice.InvoiceTerm.Substring(3, 2);
                string monthST = (int.Parse(monthED) - 1).ToString().PadLeft(2, '0');
                string barCode = $"{twyear}{monthED}{invoiceCode.Replace("-", "")}{randomCode}";
                string twDate = $"{dto.Invoice.InvoiceDate.Year}/{dto.Invoice.InvoiceDate.Month.ToString().PadLeft(2, '0')}/{dto.Invoice.InvoiceDate.Day.ToString().PadLeft(2, '0')}";

                string sellID = dto.Header.SellID;
                string sellStore = dto.Header.SellStore;
                string formatTelPhone = "";
                if (dto.Header.TelPhone != null && dto.Header.TelPhone != "")
                {
                    if (dto.Header.TelPhone.Contains("-"))
                    {
                        string[] phone = dto.Header.TelPhone.Split('-');
                        formatTelPhone = $"{phone[0]}{phone[1]}";
                    } else
                    {
                        formatTelPhone = dto.Header.TelPhone;
                    }
                }
                string telPhone = formatTelPhone != "" ? $"({formatTelPhone.Substring(0, 2)}){formatTelPhone.Substring(2, 4)}-{formatTelPhone.Substring(6)}" : "";
                string sellDate = dto.Header.SellDate;

                string dashedLine = new string('-', 30);
                string separator = new string('─', 15);

                string QrCodedata1 = dto.Invoice.QrCodedata1;
                string QrCodedata2 = dto.Invoice.QrCodedata2 ?? string.Empty;

                int targetLength1 = Math.Max(QrCodedata1.Length, QrCodedata2.Length);
                int targetLength2 = targetLength1 + 7;

                float taxAmount = 0;
                float tax = 0;
                float totalAmount = 0;

                bool Cash = false;
                bool Card = false;
                bool gift = false;

                string payWay = string.Empty;

                var list = new List<byte>();
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[]
                {
                0x1D, 0x57, // GS W 设置打印区域
                0x7C, 0x01 // 宽度为 300 点
                });

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x30 }); //大文字（默認）
                if (!string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string imagePath = Path.Combine(executableLocation, imgLogo);
                    if (File.Exists(imagePath))
                    {
                        list.AddRange(ConvertImageToESCPosData(imagePath, 500, 60));
                    }
                    else
                    {
                        list.AddRange(GetByteArr($"{companyName}"));
                        list.AddRange(new byte[] { 0x0A }); //NewLine
                    }
                }
                else if (string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    list.AddRange(GetByteArr($"{companyName}"));
                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }
                list.AddRange(GetByteArr("電子發票證明聯"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr("(補印)"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1b, 0x21, 0x38 }); //大文字(強調+雙倍高度+雙倍寬度)
                list.AddRange(GetByteArr($"{twyear}年{monthST}-{monthED}月"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr(invoiceCode));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x00 }); //font A
                list.AddRange(GetByteArr($"{dto.Invoice.InvoiceDate.ToString("yyyy/MM/dd HH:mm:ss")}  {"格式 25"}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"隨機碼:{randomCode}  總計:{total}元"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                if (string.IsNullOrEmpty(dto.Invoice.Buyer))
                {
                    list.AddRange(GetByteArr($"賣方:{seller}"));
                }
                else
                {
                    list.AddRange(GetByteArr($"賣方:{seller}  買方:{dto.Invoice.Buyer}"));
                }
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(SetBarcodeWidth(1));
                list.AddRange(SetBarcodeHeight(60));
                list.AddRange(PrintBarcode39($"*{barCode.PadRight(15, ' ')}*"));

                //page mode start
                byte[] setHorizontalPosition = new byte[] { 0x1B, 0x24, 0x20, 0x00 }; //ESC $ nL nH x軸
                byte[] setHorizontalPosition2 = new byte[] { 0x1B, 0x24, 0xE0, 0x00 }; //ESC $ nL nH x軸
                byte[] setPrintPosition = new byte[] { 0x1D, 0x24, 0x00, 0x00 }; // GS $ nL nH  y軸
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[] { 0x1b, 0x4C }); //start page mode
                list.AddRange(new byte[] { 0x1D, 0x50, 0x00, 0xCB }); //Set horizontal and vertical motion units
                                                                      //list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0xD6, 0x01 }); //Set print area in page mode 460dots
                list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0x9F, 0x00 }); //Set print area in page mode 277dots
                list.AddRange(new byte[] { 0x1b, 0x54, 0x00 }); //Select print direction in page mode

                list.AddRange(setHorizontalPosition);
                list.AddRange(PrintQrCode(QrCodedata1.PadRight(targetLength1, ' ')));

                list.AddRange(setPrintPosition);
                list.AddRange(setHorizontalPosition2);
                list.AddRange(PrintQrCode(QrCodedata2.PadRight(targetLength2, ' ')));

                list.AddRange(new byte[] { 0x0C }); //Print and return to standard mode(in page mode)
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 });
                list.AddRange(GetByteArr($"門市: {sellStore} {telPhone}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(GetByteArr(dashedLine));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"門市: {sellStore}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"電話: {telPhone}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"單號: {sellID}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"日期: {sellDate}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(GetByteArr("交易明細"));
                
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr(separator));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"{"型號".PadRight(8, ' ')}{"售價".PadLeft(6, ' ')}{"數量".PadLeft(4, ' ')}{"小計".PadLeft(4, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr(separator));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                foreach (var item in dto.Body)
                {
                    var output = FormatReturnLine(item.GoodID, item.SellPrice, item.Qty, item.SellCash);
                    list.AddRange(GetByteArr(output));

                    float taxRate = (Int32.Parse(dto.Header.Tax));

                    // 稅金
                    int itemTax = (int)Math.Round(item.SellCash - ((float)item.SellCash / (float)(1 + (0.01 * taxRate))), 0);
                    // 未稅
                    int itemTaxAmount = (int)item.SellCash - itemTax;

                    if (item.Cash > 0)
                    {
                        if (!payWay.Contains("現金 "))
                        {
                            payWay += "現金 ";
                        }
                    }

                    if (item.Card > 0)
                    {
                        if (!payWay.Contains("信用卡 "))
                        {
                            payWay += "信用卡 ";
                        }
                    }

                    if (item.Gift > 0)
                    {
                        if (!payWay.Contains("禮物金 "))
                        {
                            payWay += "禮物金 ";
                        }
                    }

                    tax += itemTax;
                    taxAmount += itemTaxAmount;
                    totalAmount += item.SellCash;

                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }
                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊

                list.AddRange(GetByteArr($"{"應稅金額".PadRight(19, ' ')}{FormatThousands(taxAmount.ToString()).PadLeft(7, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"{$"稅金{dto.Header.Tax}%".PadRight(19, ' ')}{FormatThousands(tax.ToString()).PadLeft(9, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr(separator));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"{"合計".PadRight(22, ' ')}{FormatThousands(totalAmount.ToString()).PadLeft(6, ' ')}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"{"付款方式".PadRight(8, ' ')}{payWay}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"如需換貨，請於購買七日內攜帶發票證明聯、簽單、載具及交易明細至原店辦理"));

                list.AddRange(new byte[] { 0x0A }); //NewLine

                //list.AddRange(new byte[] { 0x0C }); //print
                list.AddRange(new byte[] { 0x1D, 0x56, 0x41, 0x00 }); //cut paper

                var rawData = list.ToArray();
                GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
                IntPtr pointer = handle.AddrOfPinnedObject();
                RawPrinterHelper.SendBytesToPrinter("SLK-TL120", pointer, rawData.Length);

                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public bool PrintSaleInvoiceAndDetail(SalesInvoiceAndDetail dto)
        {
            try
            {
                string imgLogo = _config["Invoice:InvoiceLogoName"];
                string companyName = _config["Invoice:InvoiceCompanyName"];
                string seller = dto.Invoice.Seller;
                string randomCode = dto.Invoice.RandomCode;
                string invoiceCode = $"{dto.Invoice.InvoiceCode.Substring(0, 2)}-{dto.Invoice.InvoiceCode.Substring(2)}"; ;
                decimal total = dto.Invoice.InvoiceAmount;
                string twyear = dto.Invoice.InvoiceTerm.Substring(0, 3);
                string monthED = dto.Invoice.InvoiceTerm.Substring(3, 2);
                string monthST = (int.Parse(monthED) - 1).ToString().PadLeft(2, '0');
                string barCode = $"{twyear}{monthED}{invoiceCode.Replace("-", "")}{randomCode}";
                string twDate = $"{dto.Invoice.InvoiceDate.Year}/{dto.Invoice.InvoiceDate.Month.ToString().PadLeft(2, '0')}/{dto.Invoice.InvoiceDate.Day.ToString().PadLeft(2, '0')}";

                string sellID = dto.Header.SellID;
                string sellStore = dto.Header.SellStore;
                string formatTelPhone = "";
                if (dto.Header.TelPhone != null && dto.Header.TelPhone != "")
                {
                    if (dto.Header.TelPhone.Contains("-"))
                    {
                        string[] phone = dto.Header.TelPhone.Split('-');
                        formatTelPhone = $"{phone[0]}{phone[1]}";
                    }
                    else
                    {
                        formatTelPhone = dto.Header.TelPhone;
                    }
                }
                string telPhone = formatTelPhone != "" ? $"({formatTelPhone.Substring(0, 2)}){formatTelPhone.Substring(2, 4)}-{formatTelPhone.Substring(6)}" : "";
                string sellDate = dto.Header.SellDate;

                string dashedLine = new string('-', 30);
                string separator = new string('─', 15);

                string QrCodedata1 = dto.Invoice.QrCodedata1;
                string QrCodedata2 = dto.Invoice.QrCodedata2 ?? string.Empty;

                int targetLength1 = Math.Max(QrCodedata1.Length, QrCodedata2.Length);
                int targetLength2 = targetLength1 + 7;

                float taxAmount = 0;
                float tax = 0;
                float totalAmount = 0;

                bool Cash = false;
                bool Card = false;
                bool gift = false;

                string payWay = string.Empty;

                var list = new List<byte>();
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[]
                {
                0x1D, 0x57, // GS W 设置打印区域
                0x7C, 0x01 // 宽度为 300 点
                });

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x30 }); //大文字（默認）
                if (!string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string imagePath = Path.Combine(executableLocation, imgLogo);
                    if (File.Exists(imagePath))
                    {
                        list.AddRange(ConvertImageToESCPosData(imagePath, 500, 60));
                    }
                    else
                    {
                        list.AddRange(GetByteArr($"{companyName}"));
                        list.AddRange(new byte[] { 0x0A }); //NewLine
                    }
                }
                else if (string.IsNullOrEmpty(imgLogo) && !string.IsNullOrEmpty(companyName))
                {
                    list.AddRange(GetByteArr($"{companyName}"));
                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }
                list.AddRange(GetByteArr("電子發票證明聯"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1b, 0x21, 0x38 }); //大文字(強調+雙倍高度+雙倍寬度)
                list.AddRange(GetByteArr($"{twyear}年{monthST}-{monthED}月"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr(invoiceCode));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x00 }); //font A
                list.AddRange(GetByteArr($"{dto.Invoice.InvoiceDate.ToString("yyyy/MM/dd HH:mm:ss")}  {"格式 25"}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"隨機碼:{randomCode}  總計:{total}元"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                if (string.IsNullOrEmpty(dto.Invoice.Buyer))
                {
                    list.AddRange(GetByteArr($"賣方:{seller}"));
                }
                else
                {
                    list.AddRange(GetByteArr($"賣方:{seller}  買方:{dto.Invoice.Buyer}"));
                }
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(SetBarcodeWidth(1));
                list.AddRange(SetBarcodeHeight(60));
                list.AddRange(PrintBarcode39($"*{barCode.PadRight(15, ' ')}*"));

                //page mode start
                byte[] setHorizontalPosition = new byte[] { 0x1B, 0x24, 0x20, 0x00 }; //ESC $ nL nH x軸
                byte[] setHorizontalPosition2 = new byte[] { 0x1B, 0x24, 0xE0, 0x00 }; //ESC $ nL nH x軸
                byte[] setPrintPosition = new byte[] { 0x1D, 0x24, 0x00, 0x00 }; // GS $ nL nH  y軸
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[] { 0x1b, 0x4C }); //start page mode
                list.AddRange(new byte[] { 0x1D, 0x50, 0x00, 0xCB }); //Set horizontal and vertical motion units
                                                                      //list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0xD6, 0x01 }); //Set print area in page mode 460dots
                list.AddRange(new byte[] { 0x1b, 0x57, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x01, 0x9F, 0x00 }); //Set print area in page mode 277dots
                list.AddRange(new byte[] { 0x1b, 0x54, 0x00 }); //Select print direction in page mode

                list.AddRange(setHorizontalPosition);
                list.AddRange(PrintQrCode(QrCodedata1.PadRight(targetLength1, ' ')));

                list.AddRange(setPrintPosition);
                list.AddRange(setHorizontalPosition2);
                list.AddRange(PrintQrCode(QrCodedata2.PadRight(targetLength2, ' ')));

                list.AddRange(new byte[] { 0x0C }); //Print and return to standard mode(in page mode)
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 });
                list.AddRange(GetByteArr($"門市: {sellStore} {telPhone}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(GetByteArr(dashedLine));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"門市: {sellStore}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"電話: {telPhone}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"單號: {sellID}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"日期: {sellDate}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(GetByteArr("交易明細"));

                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr(separator));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"{"型號".PadRight(8, ' ')}{"售價".PadLeft(6, ' ')}{"數量".PadLeft(4, ' ')}{"小計".PadLeft(4, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr(separator));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                foreach (var item in dto.Body)
                {
                    var output = FormatReturnLine(item.GoodID, item.SellPrice, item.Qty, item.SellCash);
                    list.AddRange(GetByteArr(output));

                    float taxRate = (Int32.Parse(dto.Header.Tax));

                    // 稅金
                    int itemTax = (int)Math.Round(item.SellCash - ((float)item.SellCash / (float)(1 + (0.01 * taxRate))), 0);
                    // 未稅
                    int itemTaxAmount = (int)item.SellCash - itemTax;

                    if (item.Cash > 0)
                    {
                        if (!payWay.Contains("現金 "))
                        {
                            payWay += "現金 ";
                        }
                    }

                    if (item.Card > 0)
                    {
                        if (!payWay.Contains("信用卡 "))
                        {
                            payWay += "信用卡 ";
                        }
                    }

                    if (item.Gift > 0)
                    {
                        if (!payWay.Contains("禮物金 "))
                        {
                            payWay += "禮物金 ";
                        }
                    }

                    tax += itemTax;
                    taxAmount += itemTaxAmount;
                    totalAmount += item.SellCash;

                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }
                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊

                list.AddRange(GetByteArr($"{"應稅金額".PadRight(19, ' ')}{FormatThousands(taxAmount.ToString()).PadLeft(7, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"{$"稅金{dto.Header.Tax}%".PadRight(19, ' ')}{FormatThousands(tax.ToString()).PadLeft(9, ' ')}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr(separator));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"{"合計".PadRight(22, ' ')}{FormatThousands(totalAmount.ToString()).PadLeft(6, ' ')}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"{"付款方式".PadRight(8, ' ')}{payWay}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr($"如需換貨，請於購買七日內攜帶發票證明聯、簽單、載具及交易明細至原店辦理"));

                list.AddRange(new byte[] { 0x0A }); //NewLine

                //list.AddRange(new byte[] { 0x0C }); //print
                list.AddRange(new byte[] { 0x1D, 0x56, 0x41, 0x00 }); //cut paper

                var rawData = list.ToArray();
                GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
                IntPtr pointer = handle.AddrOfPinnedObject();
                RawPrinterHelper.SendBytesToPrinter("SLK-TL120", pointer, rawData.Length);

                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public bool PrintDiscountOrder(DiscountOrderPrint dto)
        {
            try
            {
                string imgLogo = _config["Invoice:InvoiceLogoName"];
                string companyName = _config["Invoice:InvoiceCompanyName"];
                string seller = dto.Invoice.Seller;
                string randomCode = dto.Invoice.RandomCode;
                string invoiceCode = $"{dto.Invoice.InvoiceCode.Substring(0, 2)}{dto.Invoice.InvoiceCode.Substring(2)}"; ;
                string invoiceDate = dto.Invoice.InvoiceDate.ToString();
                decimal total = dto.Invoice.InvoiceAmount;
                string twyear = dto.Invoice.InvoiceTerm.Substring(0, 3);
                string monthED = dto.Invoice.InvoiceTerm.Substring(3, 2);
                string monthST = (int.Parse(monthED) - 1).ToString().PadLeft(2, '0');
                string barCode = $"{twyear}{monthED}{invoiceCode.Replace("-", "")}{randomCode}";
                string twDate = $"{dto.Invoice.InvoiceDate.Year}-{dto.Invoice.InvoiceDate.Month.ToString().PadLeft(2, '0')}-{dto.Invoice.InvoiceDate.Day.ToString().PadLeft(2, '0')}";
                string openDate = $"{dto.Invoice.InvoiceDate.Year}{dto.Invoice.InvoiceDate.Month.ToString().PadLeft(2, '0')}{dto.Invoice.InvoiceDate.Day.ToString().PadLeft(2, '0')}";

                string sellID = dto.Header.SellID;
                string sellStore = dto.Header.SellStore;
                string formatTelPhone = "";
                if (dto.Header.TelPhone != null && dto.Header.TelPhone != "")
                {
                    if (dto.Header.TelPhone.Contains("-"))
                    {
                        string[] phone = dto.Header.TelPhone.Split('-');
                        formatTelPhone = $"{phone[0]}{phone[1]}";
                    }
                }
                string telPhone = formatTelPhone != "" ? $"({formatTelPhone.Substring(0, 2)}){formatTelPhone.Substring(2, 4)}-{formatTelPhone.Substring(6)}" : "";
                string sellDate = dto.Header.SellDate;

                string dashedLine = new string('-', 30);
                string separator = new string('─', 15);

                string QrCodedata1 = dto.Invoice.QrCodedata1;
                string QrCodedata2 = dto.Invoice.QrCodedata2 ?? string.Empty;

                int targetLength1 = Math.Max(QrCodedata1.Length, QrCodedata2.Length);
                int targetLength2 = targetLength1 + 7;

                string taxType = string.Empty;
                if (dto.Invoice.TaxType == "1")
                {
                    taxType = "TX";
                } else if (dto.Invoice.TaxType == "3")
                {
                    taxType = "TZ";
                }

                float taxAmount = 0;
                float tax = 0;
                float totalAmount = 0;

                string payWay = string.Empty;

                var list = new List<byte>();
                list.AddRange(new byte[] { 0x1b, 0x40 }); //init

                list.AddRange(new byte[]
                {
                0x1D, 0x57, // GS W 设置打印区域
                0x7C, 0x01 // 宽度为 300 点
                });

                list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // 置中對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x09 });

                list.AddRange(GetByteArr($"{sellStore}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr("電子發票銷貨返回，進貨退出或折讓證明單證明聯"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x1b, 0x21, 0x08 });
                list.AddRange(GetByteArr($"{twDate}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(new byte[] { 0x1B, 0x61, 0x00 }); // 置左對齊
                list.AddRange(new byte[] { 0x1b, 0x21, 0x00 }); //font A

                list.AddRange(GetByteArr($"賣方統編: {seller}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"賣方名稱: {sellStore}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"發票開立日期: {openDate}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"{invoiceCode}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"買方統編: {dto.Invoice.Buyer}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"買方名稱: {dto.Invoice.BuyerName}"));

                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(new byte[] { 0x0A }); //NewLine

                list.AddRange(GetByteArr(dashedLine));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                foreach (var item in dto.Body)
                {
                    float taxRate = (Int32.Parse(dto.Header.Tax));

                    int itemTax = (int)Math.Round(item.SellCash - ((float)item.SellCash / (float)(1 + (0.01 * taxRate))), 0);

                    int itemTaxAmount = (int)item.SellCash - itemTax;

                    if (item.SellMode != "2" && item.SellMode != "8")
                    {
                        tax += itemTax;
                        totalAmount += itemTaxAmount;
                    } else
                    {
                        tax += itemTax;
                        totalAmount += itemTaxAmount;
                    }
                    

                    list.AddRange(GetByteArr($"{item.GoodID}:{item.Qty}:{item.SellCash}:{itemTaxAmount}"));
                    list.AddRange(new byte[] { 0x0A }); //NewLine
                }

                list.AddRange(GetByteArr($"課稅別: {taxType}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"營業稅額: {tax}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"金額: {totalAmount}"));
                list.AddRange(new byte[] { 0x0A }); //NewLine
                list.AddRange(GetByteArr($"簽收人: "));
                list.AddRange(new byte[] { 0x0A }); //NewLine

                //list.AddRange(new byte[] { 0x0C }); //print
                list.AddRange(new byte[] { 0x1D, 0x56, 0x41, 0x00 }); //cut paper

                var rawData = list.ToArray();
                GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
                IntPtr pointer = handle.AddrOfPinnedObject();
                RawPrinterHelper.SendBytesToPrinter("SLK-TL120", pointer, rawData.Length);

                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        private byte[] PrintMediaQrCode(string data)
        {
            const byte fixedModuleSize = 4;
            const byte fixedVersion = 5;

            var byteslist = new List<byte>();
            byte[] qrBytes = System.Text.Encoding.UTF8.GetBytes(data);
            int dataLength = qrBytes.Length + 3;

            int maxCapacity = 200;
            if (dataLength > maxCapacity)
            {
                Array.Resize(ref qrBytes, maxCapacity - 3);
                dataLength = maxCapacity;
            }

            byte dataPL = (byte)(dataLength % 256);
            byte dataPH = (byte)(dataLength / 256);
            var bytes = new List<byte>();
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x43, fixedModuleSize });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x45, fixedVersion });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, dataPL, dataPH, 0x31, 0x50, 0x30 });
            bytes.AddRange(qrBytes);
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x51, 0x30 });
            return bytes.ToArray();
        }

        private byte[] PrintQrCode(string data)
        {
            const byte fixedModuleSize = 3;
            const byte fixedVersion = 5;

            var byteslist = new List<byte>();
            byte[] qrBytes = System.Text.Encoding.UTF8.GetBytes(data);
            int dataLength = qrBytes.Length + 3;

            int maxCapacity = 200;
            if (dataLength > maxCapacity)
            {
                Array.Resize(ref qrBytes, maxCapacity - 3);
                dataLength = maxCapacity;
            }

            byte dataPL = (byte)(dataLength % 256);
            byte dataPH = (byte)(dataLength / 256);
            var bytes = new List<byte>();
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x43, fixedModuleSize });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x45, fixedVersion });
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, dataPL, dataPH, 0x31, 0x50, 0x30 });
            bytes.AddRange(qrBytes);
            bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x51, 0x30 });
            return bytes.ToArray();
        }

        public static byte[] ConvertImageToESCPosData(string imagePath, int maxWidth, int maxHeight)
        {
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                // 計算縮放比例
                float scale = Math.Min((float)maxWidth / bitmap.Width, (float)maxHeight / bitmap.Height);

                // 設置縮放後的圖片尺寸
                int newWidth = (int)(bitmap.Width * scale);
                int newHeight = (int)(bitmap.Height * scale);

                // 創建縮放後的圖片
                using (Bitmap scaledBitmap = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(scaledBitmap))
                    {
                        g.Clear(Color.White); // 設置背景為白色（或其他顏色）
                        g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                    }

                    int width = scaledBitmap.Width;
                    int height = scaledBitmap.Height;
                    int bytesPerLine = (width + 7) / 8;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        // Header for the ESC/POS graphics command
                        stream.WriteByte(0x1D); // GS
                        stream.WriteByte(0x76); // v
                        stream.WriteByte(0x30); // 0
                        stream.WriteByte(0x00); // Normal size

                        // Image width (in bytes, not pixels)
                        stream.WriteByte((byte)(bytesPerLine % 256));
                        stream.WriteByte((byte)(bytesPerLine / 256));

                        // Image height (in points, not pixels)
                        stream.WriteByte((byte)(height % 256));
                        stream.WriteByte((byte)(height / 256));

                        // Convert bitmap to monochrome and write to stream
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < bytesPerLine; x++)
                            {
                                byte pixelData = 0x00;

                                for (int bit = 0; bit < 8; bit++)
                                {
                                    int px = x * 8 + bit;
                                    if (px < width)
                                    {
                                        Color color = scaledBitmap.GetPixel(px, y);
                                        if (color.A < 128) // If alpha is less than 128, it's transparent
                                        {
                                            pixelData |= (byte)(0x80 >> bit); // Use white for transparent areas
                                        }
                                        else if (color.R < 128) // Convert to monochrome (black/white)
                                        {
                                            pixelData |= (byte)(0x80 >> bit);
                                        }
                                    }
                                }

                                stream.WriteByte(pixelData);
                            }
                        }

                        return stream.ToArray();
                    }
                }
            }

        }
    }
}
