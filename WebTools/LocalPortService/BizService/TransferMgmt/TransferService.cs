using ESC_POS_USB_NET.Printer;
using LocalPortService.Core.Helper;
using LocalPortService.Model.API;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LocalPortService.BizService.TransferMgmt
{
    public class TransferServices
    {
        private readonly IConfiguration _config;
        public TransferServices(IConfiguration config) 
        {
            _config = config;
        }
        public async Task<bool> ExecThermalPrinter((TransferHeader, List<TransferDetail>) data)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var transHeader = data.Item1;
            var transDetail = data.Item2?.Count > 0 ? data.Item2 : null;
            if (transDetail is null) { return false; }
            string transferID = @$"調撥單號 : {transHeader?.TransferID}";
            string transDate = @$"調撥日期 : {transHeader?.TranDate}";
            string tranOutStoreText = @$"調出庫點 : {transHeader?.TranOutStore}";
            string tranInStoreText = @$"調入庫點 : {transHeader?.TranInStore}";
            string desc = @$"說明 : {transHeader?.Remark}";
            string headrText = @$"型號{string.Empty.PadLeft(9)}尺寸{string.Empty.PadLeft(4)}量{string.Empty.PadLeft(4)}備註";
            string total = FormatLineTotal("合計", transDetail.Sum(x => x.Num));
            string signOutCheck = "調出簽收欄: __________________";
            string signInCheck = "調入簽收欄: __________________";
            string separator = new string('─', 16);
            byte[] lineFeed = new byte[] { 0x0A };

            Printer printer = new Printer("SLK-TL120");
            printer.InitializePrint();

            printer.Append(new byte[]
            {
                0x1D, 0x57, // GS W 设置打印区域
                0x90, 0x01 // 宽度为 400 点
            });
            printer.Append(SetBarcodeWidth(2));
            printer.Append(SetBarcodeHeight(60));
            printer.Append(PrintBarcode128(transHeader.TransferID));
            printer.NewLine();
            printer.Append(GetByteArr(transferID));
            printer.NewLine();
            printer.Append(GetByteArr(transDate));
            printer.NewLine();
            printer.Append(GetByteArr(tranOutStoreText));
            printer.NewLine();
            printer.Append(GetByteArr(tranInStoreText));
            printer.NewLine();
            printer.Append(GetByteArr(desc));
            printer.NewLine();
            printer.Append(GetByteArr(headrText));
            printer.NewLine();
            printer.Append(GetByteArr(separator));
            printer.NewLine();
            foreach (var item in transDetail)
            {
                var output = FormatLine(item.GoodID, item.SizeNo, item.Num, "");
                printer.Append(GetByteArr(output));
                printer.NewLine();
            }
            printer.Append(GetByteArr(separator));
            printer.NewLine();
            printer.Append(GetByteArr(total));
            printer.NewLine();
            printer.Append(lineFeed);
            printer.Append(lineFeed);
            printer.Append(GetByteArr(signOutCheck));
            printer.NewLine();
            printer.Append(lineFeed);
            printer.Append(lineFeed);
            printer.Append(GetByteArr(signInCheck));
            printer.Append(lineFeed);
            printer.NewLine(); //這行一定要有

            printer.FullPaperCut();
            printer.PrintDocument();
            return true;
        }

        private byte[] GetByteArr(string str)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoder = System.Text.Encoding.GetEncoding(950); //Sewoo SLK-TL120 印表機設定裡面有寫950

            return encoder.GetBytes(str);
        }

        private string FormatLine(string GoodID, string size, int quantity, string remark)
        {
            // 设定每个字段的起始位置
            int GoodIDStart = 0;
            int sizeStart = 15;
            int quantityStart = 22;
            int remarkStart = 30;

            // 构建格式化的字符串
            StringBuilder line = new StringBuilder();
            line.Append(GoodID.PadRight(sizeStart - GoodIDStart));
            line.Append(size.PadRight(quantityStart - sizeStart));
            line.Append(quantity.ToString().PadRight(remarkStart - quantityStart));
            line.Append(remark);

            return line.ToString();
        }
        private string FormatLineTotal(string totalStr, int quantity)
        {
            // 设定每个字段的起始位置
            int start = 0;
            int totalStart = 13;
            int quantityStart = 20;

            // 构建格式化的字符串
            StringBuilder line = new StringBuilder();
            line.Append(string.Empty.PadRight(totalStart));
            line.Append(totalStr.PadRight(quantityStart - totalStart));
            line.Append(quantity.ToString());

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
    }
}
