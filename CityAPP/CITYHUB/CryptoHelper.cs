using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CITYHUB
{
    public class CryptoHelper
    {
       // com.tradevan.qrutil.QREncrypter eInvAesEncrypter = new com.tradevan.qrutil.QREncrypter();
        private  static byte[] ConvertHexToByte(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            int index = 0;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                int num3 = Convert.ToInt32(hexString.Substring(i, 2), 0x10);
                buffer[index] = BitConverter.GetBytes(num3)[0];
                index++;
            }
            return buffer;
        }
        public static string AES_Encrypt(string plainText, string key)
        {
            string sRET;
            try
            {
                var iv_base64 = "Dt8lyToo17X/XkXaQvihuA==";
                byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(plainText);
                RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    KeySize = 128,
                    Key = ConvertHexToByte(key),
                    BlockSize = 128,
                    //IV要找地方放.rex.add.20210622
                    IV = Convert.FromBase64String(iv_base64)
                };
                ICryptoTransform transform = rijndaelManaged.CreateEncryptor();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                byte[] inArray = memoryStream.ToArray();
                sRET = Convert.ToBase64String(inArray);
            }
            catch (Exception)
            {
                sRET = "ERROR";
            }
            return sRET;
        }

        public static string AES_Decrype(string cipherText, string key)
        {
            /*byte[] array = Convert.FromBase64String(cipherText);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Key = convertHexToByte(AESKey);
		rijndaelManaged.BlockSize = 128;
		rijndaelManaged.IV = Convert.FromBase64String("Dt8lyToo17X/XkXaQvihuA==");
		ICryptoTransform transform = rijndaelManaged.CreateDecryptor();
		MemoryStream stream = new MemoryStream(array);
		CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		byte[] array2 = new byte[array.Length];
		cryptoStream.Read(array2, 0, array2.Length);
		cryptoStream.Close();
		return Encoding.UTF8.GetString(array2);
             */


            string sRET;
            try
            {
                var eInvAesEncrypter = new com.tradevan.qrutil.QREncrypter();
                sRET = eInvAesEncrypter.AESDecrype(cipherText, key);
            }
            catch
            {
                sRET = "ERROR";
            }

            return sRET;
        }

        public static string MD5(string input)
        {
            using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
            {
                //將字串編碼成 UTF8 位元組陣列
                var bytes = Encoding.UTF8.GetBytes(input);

                //取得雜湊值位元組陣列
                var hash = cryptoMD5.ComputeHash(bytes);

                //取得 MD5
                var md5 = BitConverter.ToString(hash)
                  .Replace("-", String.Empty)
                  .ToUpper();

                return md5;
            }
        }
    }
}
