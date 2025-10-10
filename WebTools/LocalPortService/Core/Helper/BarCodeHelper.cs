using System.Text.RegularExpressions;

namespace LocalPortService.Core.Helper
{
    public static class BarCodeHelper
    {
        public static string BarCode128(string code)
        {
            // 開始碼
            var startSymbol = "Ë";
            // 這是我們要進行編碼的資料
            var encodedData = code;
            // 檢查碼
            var checkSymbol = "";
            // 結束碼
            var stopSymbol = "Î";

            // 預設使用 Code Set A ( 128A ) 字元集
            Code128Type type = Code128Type.CodeSetA;

            // 如果有小寫字元就改用 Code Set B ( 128B ) 字元集
            if (Regex.IsMatch(encodedData, "[a-z]"))
            {
                type = Code128Type.CodeSetB;
            }

            // 不同的 Code 128 字元集會使用不同的開始碼，其 checksum (驗證碼) 的起算點也會不一樣
            var checksum = 0;
            switch (type)
            {
                case Code128Type.CodeSetA:
                    startSymbol = "Ë";
                    checksum = 103;
                    break;
                case Code128Type.CodeSetB:
                    startSymbol = "Ì";
                    checksum = 104;
                    break;
                case Code128Type.CodeSetC:
                    startSymbol = "Í";
                    checksum = 105;
                    break;
                default:
                    throw new ArgumentException("錯誤的 Code128Type 類型", "type");
            }

            // 你必須一個字元、一個字元拆開計算驗證碼(checksum)
            var chars = encodedData.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                // 取得字元的位置，從 1 開始
                var position = i + 1;
                // 這裡必須計算該字元在 Code 128 中的 Value 是多少
                var value = 0;
                // 其實 Value 可以不用查表，你可以透過 ASCII 的字碼轉換成 Value 值
                var asciiasc = (int)chars[i];
                if (asciiasc >= 32)
                {
                    value = asciiasc - 32;
                    checksum += value * position;
                }
                else
                {
                    value = asciiasc + 64;
                    checksum += value * position;
                }
            }

            // 驗證碼最終要取 103 的餘數
            checksum = checksum % 103;

            // 驗證碼轉檢查也不用查表，透過以下公式即可轉換出字元
            if (checksum < 95)
            {
                checkSymbol = char.ConvertFromUtf32(checksum + 32);
            }
            else
            {
                checkSymbol = char.ConvertFromUtf32(checksum + 100);
            }

            // 取得最終結果
            var result = startSymbol + encodedData + checkSymbol + stopSymbol;

            return result;
        }
    }

    public enum Code128Type
    {
        CodeSetA,
        CodeSetB,
        CodeSetC
    }
}
