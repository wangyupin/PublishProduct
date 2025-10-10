using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LocalPortService.Model.API
{
    public class PrinterExample
    {
        public string PrinterName { get; set; }
    }
    public class PrinterSupportLanguage
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        // 打開印表機
        private static extern bool OpenPrinter(string printerName, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        // 關閉印表機
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        // 開始打印
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        // 結束打印
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        // 開始打印頁面
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        // 結束打印頁面
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        // 將數據發送到打印機
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        public static bool SendCommandToPrinter(string printerName, string command)
        {
            IntPtr hPrinter;
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                Console.WriteLine("Failed to open printer.");
                return false;
            }

            try
            {
                DOCINFOA docInfo = new DOCINFOA
                {
                    pDocName = "Printer Command Test",
                    pDataType = "RAW"
                };

                if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                    return false;

                if (!StartPagePrinter(hPrinter))
                    return false;

                IntPtr pBytes = Marshal.StringToHGlobalAnsi(command);
                WritePrinter(hPrinter, pBytes, command.Length, out int bytesWritten);
                Marshal.FreeHGlobal(pBytes);

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
                return true;
            }
            finally
            {
                ClosePrinter(hPrinter);
            }
        }
    }
}
