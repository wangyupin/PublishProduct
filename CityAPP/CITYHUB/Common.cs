using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LineNotifySDK;

namespace CITYHUB
{
    public class Common
    {

        public static string FilterChar(string inputValue)
        {
            // return Regex.Replace(inputValue, "[`~!@#$^&*()=|{}‘:;‘,\\[\\].<>/?~！@#￥……&*（）&mdash;|{}【】；‘’，。/*-+]+", "", RegexOptions.IgnoreCase);
            if (Regex.IsMatch(inputValue, "[A-Za-z0-9\u4e00-\u9fa5-]+"))
            {
                return Regex.Match(inputValue, "[A-Za-z0-9\u4e00-\u9fa5-]+").Value;
            }
            return "";
        }


        public static bool LineSendNotify(string token, string msg)
        {
            bool bRET = true;
            try
            {
                //token = "awRUdeFuc8PpHwESgqbVOjh1ud6VBShNdhOJQYsAHRx";
                //msg = "MSG";
                Utility.SendNotification(token, msg);
            }
            catch (Exception ex)
            {
                bRET = false;
            }
            return bRET;
        }
        public static void Delay(Double dTimeSpan)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(dTimeSpan));
            });
            t.Wait();
        }

        public static void WriteLog(String msg, string LogPath, string LogFileName)
        {
            StreamWriter writer = null;
            try
            {
                if (!System.IO.Directory.Exists(LogPath))
                    System.IO.Directory.CreateDirectory(LogPath);

                writer = File.AppendText(LogPath + "\\" + LogFileName);
                writer.WriteLine("{0} {1}", DateTime.Now.ToString("yyyyMMdd HHmmss"), msg);
                writer.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
            Delay(0.1688);
        }

        /// <summary>
        /// 数字转字符(26進制)
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static string WorkNtoW(int iNumber) 
        {
            if (iNumber < 1 || iNumber > 702)
            {
                return "";
            }
            string sLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iUnits = 26;
            int iDivisor = (int)(iNumber / iUnits);
            int iResidue = iNumber % iUnits;
            if (iDivisor == 1 && iResidue == 0)
            {
                iDivisor = 0;
                iResidue += iUnits;
            }
            else
            {
                if (iResidue == 0)
                {
                    iDivisor -= 1;
                    iResidue += iUnits;
                }
            }
            if (iDivisor == 0)
            {
                return sLetters.Substring(iResidue - 1, 1);
            }
            else
            {
                return sLetters.Substring(iDivisor - 1, 1) + sLetters.Substring(iResidue - 1, 1);
            }
        }
        /// <summary>
        /// 字符转数字
        /// </summary>
        /// <param name="sString"></param>
        /// <returns></returns>
        public static int WorkWtoN(string sString) 
        {
            if (string.Compare(sString, "A") == -1 || string.Compare(sString, "ZZ") == 1)
            {
                return 0;
            }
            string sLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iUnits = 26;
            int sFirst = -1;
            int sSecond;
            if (sString.Length == 1)
            {
                sSecond = sLetters.IndexOf(sString);
            }
            else
            {
                sFirst = sLetters.IndexOf(sString.Substring(0, 1));
                sSecond = sLetters.IndexOf(sString.Substring(1, 1));
            }
            return (sFirst + 1) * iUnits + (sSecond + 1);
        }


        public static bool GenerateJsonFile<T>(T data, string filename)
        {
            bool bRET = true;
            try
            {
                string sFileData = JsonConvert.SerializeObject(data);
                using (TextWriter txtW = new StreamWriter(filename, false, System.Text.Encoding.UTF8))
                {
                    txtW.Write(sFileData);
                    txtW.Close();
                    txtW.Dispose();
                }
            }
            catch { bRET = false; }
                
            return bRET;
        }

        /// <summary>
        /// 產生XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool GenerateXml<T>(T data, string filename)
        {
            bool bRET = true;
            try
            {
                XmlSerializer xml = new XmlSerializer(data.GetType());
                using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle);
                    xml.Serialize(stream, data);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch
            {
                bRET = false;
            }
            return bRET;
        }

        /// <summary>
        /// Call Api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="uri"></param>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static R CallAPI<T, R>(string uri, string cmd, T data) where T : class
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            //return;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uri + cmd);

            myRequest.Method = "POST";
            myRequest.ContentType = "application/json;charset=utf-8";
            myRequest.ContentLength = byteArray.Length;
            Stream dataStream = myRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string result = "";
            using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
            {
                Stream responseData = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseData))
                {
                    result = sr.ReadToEnd();
                }
            }
            if (!string.IsNullOrWhiteSpace(result))
            {
                var resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<R>(result);
                return resultObj;
            }
            else
            {
                return default;
            }

        }

        /// <summary>
        /// 驗證日期格式
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /*EX:if (!CITYHUB.Common.ValidateDateTime(InputData.SysDate, "yyyyMMdd"))*/
        public static bool ValidateDateTime(string datetime, string format)
        {
            if (datetime == null || datetime.Length == 0)
            {
                return false;
            }
            try
            {
                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo
                {
                    FullDateTimePattern = format
                };
                DateTime dt = DateTime.ParseExact(datetime, "F", dtfi);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 判斷輸入的字串類型。　
        ///
        ///  1: 由26個英文字母組成的字串 
        ///  2: 正整數 
        ///  3: 非負整數（正整數 + 0)
        ///  4: 非正整數（負整數 + 0）
        ///  5: 負整數 
        ///  6: 整數
        ///  7: 非負浮點數（正浮點數 + 0）
        ///  8: 正浮點數
        ///  9: 非正浮點數（負浮點數 + 0)
        /// 10: 負浮點數 
        /// 11: 浮點數
        /// 12: 由26個英文字母的大寫組成的字串
        /// 13: 由26個英文字母的小寫組成的字串
        /// 14: 由數位和26個英文字母組成的字串
        /// 15: 由數位、26個英文字母或者下劃線組成的字串
        /// 16: Email
        /// 17: URL
        /// 18: 只能輸入入中文
        /// 19: 只能輸入0和非0打頭的數字
        /// 20: 只能輸入數字
        /// 21: 只能輸入數字加2位小數
        /// 22: 只能輸入0和非0打頭的數字加2位小數
        /// 23: 只能輸入0和非0打頭的數字加2位小數，但不匹配0.00
        /// 24: "^[0-9]+(.[0-9]{1,3})?$"   該實數只能帶3位小數
        /// 26: 驗證日期:YYYYMM
        /// 27: 驗證日期: YYYYMMDD
        /// 28: 驗證日期:YYYY/MM/DD
        /// 29:驗證特殊字元
        /// 30:驗證時間:HHmmss
        /// 驗証通過則傳回 True，反之則為 False。
        /// </summary>
        /// <param name="_value">欲驗證的資字串</param>
        /// <param name="_kind">驗證種類</param>
        /// <returns></returns>
        /*EX:if (!CITYHUB.Common.ValidateString(InputData.SellerId, 20))*/
        public static bool ValidateString(String _value, int _kind)
        {
            string RegularExpressions;
            switch (_kind)
            {
                case 1:
                    //由26個英文字母組成的字串
                    RegularExpressions = "^[A-Za-z]+$";
                    break;
                case 2:
                    //正整數 
                    RegularExpressions = "^[0-9]*[1-9][0-9]*$";
                    break;
                case 3:
                    //非負整數（正整數 + 0)
                    RegularExpressions = "^\\d+$";
                    break;
                case 4:
                    //非正整數（負整數 + 0）
                    RegularExpressions = "^((-\\d+)|(0+))$";
                    break;
                case 5:
                    //負整數 
                    RegularExpressions = "^-[0-9]*[1-9][0-9]*$";
                    break;
                case 6:
                    //整數
                    RegularExpressions = "^-?\\d+$";
                    break;
                case 7:
                    //非負浮點數（正浮點數 + 0）
                    RegularExpressions = "^\\d+(\\.\\d+)?$";
                    break;
                case 8:
                    //正浮點數
                    RegularExpressions = "^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$";
                    break;
                case 9:
                    //非正浮點數（負浮點數 + 0）
                    RegularExpressions = "^((-\\d+(\\.\\d+)?)|(0+(\\.0+)?))$";
                    break;
                case 10:
                    //負浮點數
                    RegularExpressions = "^(-(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*)))$";
                    break;
                case 11:
                    //浮點數
                    RegularExpressions = "^(-?\\d+)(\\.\\d+)?$";
                    break;
                case 12:
                    //由26個英文字母的大寫組成的字串
                    RegularExpressions = "^[A-Z]+$";
                    break;
                case 13:
                    //由26個英文字母的小寫組成的字串
                    RegularExpressions = "^[a-z]+$";
                    break;
                case 14:
                    //由數位和26個英文字母組成的字串
                    RegularExpressions = "^[A-Za-z0-9]+$";
                    break;
                case 15:
                    //由數位、26個英文字母或者下劃線組成的字串 
                    RegularExpressions = "^[0-9a-zA-Z_]+$";
                    break;
                case 16:
                    //email地址
                    RegularExpressions = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                    break;
                case 17:
                    //url
                    RegularExpressions = "^[a-zA-z]+://(\\w+(-\\w+)*)(\\.(\\w+(-\\w+)*))*(\\?\\S*)?$";
                    break;
                case 18:
                    //只能輸入中文
                    RegularExpressions = "^[^\u4E00-\u9FA5]";
                    break;
                case 19:
                    //只能輸入0和非0打頭的數字
                    RegularExpressions = "^(0|[1-9][0-9]*)$";
                    break;
                case 20:
                    //只能輸入數字
                    RegularExpressions = "^[-0-9]*$";
                    break;
                case 21:
                    //只能輸入數字加2位小數
                    RegularExpressions = "^[0-9]+(.[0-9]{1,2})?$";
                    break;
                case 22:
                    //只能輸入0和非0打頭的數字加2位小數
                    RegularExpressions = "^(0|[1-9]+)(.[0-9]{1,2})?$";
                    break;
                case 23:
                    //只能輸入0和非0打頭的數字加2位小數，但不匹配0.00
                    RegularExpressions = "^(0(.(0[1-9]|[1-9][0-9]))?|[1-9]+(.[0-9]{1,2})?)$";
                    break;
                case 24:
                    //驗證日期格式 YYYYMMDD, 範圍19000101~20991231
                    RegularExpressions = "(19|20)\\d\\d+(0[1-9]|1[012])+(0[1-9]|[12][0-9]|3[01])$";
                    break;
                case 25:
                    //驗證日期格式 MMDDYYYY
                    RegularExpressions = "(0[1-9]|1[012])+(0[1-9]|[12][0-9]|3[01])+(19|20)\\d\\d$";
                    break;
                case 26:
                    //驗證日期格式 YYYYMM
                    RegularExpressions = "(19|20)\\d\\d+(0[1-9]|1[012])$";
                    break;
                case 27:
                    //驗證日期格式 YYYYMMDD, 範圍00010101~99991231
                    RegularExpressions = "(^0000|0001|9999|[0-9]{4})+(0[1-9]|1[0-2])+(0[1-9]|[12][0-9]|3[01])$";
                    break;
                case 28: //驗證日期格式YYYY/MM/DD
                    RegularExpressions = "^([2][0]\\d{2}/([0]\\d|[1][0-2])/([0-2]\\d|[3][0-1]))$|^([2][0]\\d{2}/([0]\\d|[1][0-2])/([0-2]\\d|[3][0-1]))$";
                    break;
                case 29:  //驗證特殊字元
                    RegularExpressions = "(?=.*[@#$%^&+=])";
                    break;
                case 30:  //驗證時間 yyyyMMddHHmmss
                    {
                        try
                        {
                            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo
                            {
                                FullDateTimePattern = "yyyyMMddHHmmss"
                            };
                            DateTime dt = DateTime.ParseExact(_value, "F", dtfi);
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                case 31:  //驗證時間HHmmss
                    {
                        try
                        {
                            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo
                            {
                                FullDateTimePattern = "HHmmss"
                            };
                            DateTime dt = DateTime.ParseExact(_value, "F", dtfi);
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }

                //break;
                default:
                    return false;
                    //break;
            }

            Match m = Regex.Match(_value, RegularExpressions);

            if (m.Success)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 確認統一編號是否正確
        /// </summary>
        /// <param name="bid"></param>
        /// <returns></returns>
        public static bool CheckBuyerID(string bid)
        {
            string[] tmps = new string[] { "1", "2", "1", "2", "1", "2", "4", "1" };
            int sum = 0, s1, s2;
            string re = @"(^\d{8}$)";   //正則運算式驗證，也可以在進函式前先行驗證
            bool isMatch = Regex.IsMatch(bid, re);  //正則運算式驗證

            if (!isMatch)
            {
                return false;
            }

            for (int i = 0; i < 8; i++)
            {
                s1 = Int32.Parse(bid.Substring(i, 1));
                s2 = Int32.Parse(tmps[i]);
                sum += Cal(s1 * s2);
            }

            if (!Valid(sum))
            {
                if (bid.Substring(6, 1) == "7") return (Valid(sum + 1));
            }

            return (Valid(sum));
        }

        //CheckBuyerID結果驗證
        private static bool Valid(int n)
        {
            return (n % 10 == 0);
        }

        //CheckBuyerID計算公式
        private static int Cal(int n)
        {
            int sum = 0;
            while (n != 0)
            {
                sum += (n % 10);
                n = (n - n % 10) / 10;  // 取整數
            }
            return sum;
        }
    }
}
