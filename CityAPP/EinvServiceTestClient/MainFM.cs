using EINV_MODEL.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace EinvServiceTestClient
{
    public partial class MainFM : Form
    {
        public class UploadTradeRequest
        {
            public string CompanyId { get; set; }

            public string StoreId { get; set; }

            public int Amount { get; set; }
        }
        public MainFM()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
        }

        private void btPost_Click(object sender, EventArgs e)
        {
            string serviceURL = comboBox1.Text;
            var req = new RequestModel();
            var res = new ResponseMode();
            var posEchoCont = new PostEchoModel();
            posEchoCont.echodata = "Hi Service";
            posEchoCont.memo = "我是 CLIENT";

            req.version = "0.1";
            req.appid = "8351647501404101";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(posEchoCont), "F3200F08109EB3BEDF808D04A8E41865");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        posEchoCont = JsonConvert.DeserializeObject<PostEchoModel>(CITYHUB.CryptoHelper.AES_Decrype(res.ciphertext, "427E3F32B540EC26060E3DC8692F95F9"));
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                        SysLBox.Items.Insert(0, "res.ciphertext AES_Decrype : posecho.echodata->" + posEchoCont.echodata + "    posecho.memo->" + posEchoCont.memo);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_ExecutePRM_Click(object sender, EventArgs e)
        {
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-ExecutePRM Go");

            string sql = "INSERT INTO dbo.basic_apikey (AppId, ApiKey, Fag, UpdateDate, UpdateTime )  "
                                   + "VALUES ( @AppId, @ApiKey, @Fag, @UpdateDate, @UpdateTime ) ";
            SqlParameter[] Parameters = new SqlParameter[] {
                                    new SqlParameter("@AppId", TB_AppId.Text),
                                    new SqlParameter("@ApiKey", TB_ApiKey.Text),
                                    new SqlParameter("@Fag", "Y"),
                                    new SqlParameter("@UpdateDate", System.DateTime.Now),
                                    new SqlParameter("@UpdateTime", System.DateTime.Now)
                            };

            int iRET = CITYHUB.DBHelper.ExecuteSqlPARM(TB_DB_ConnectionString_Trade.Text, sql, Parameters);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-ExecutePRM Done iRET : " + iRET.ToString());
        }

        private void bt_GetDataSetByPRM_Click(object sender, EventArgs e)
        {
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-GetDataSetByPRM Go");
            string sql = "SELECT * FROM  dbo.basic_apikey WHERE AppId = @AppId ";
            SqlParameter[] Parameters = new SqlParameter[] {
                                    new SqlParameter("@AppId",TB_AppId.Text)
                                    };
            DataSet ds = CITYHUB.DBHelper.GetDataSetBySqlPARM(TB_DB_ConnectionString_Trade.Text, sql, Parameters);
            DGV_DB_DataSHow.DataSource = ds.Tables[0];
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-GetDataSetByPRM Done");
        }

        private void bt_ExecuteSP_Click(object sender, EventArgs e)
        {
            /*SqlParameter[] Parameters = new SqlParameter[] {
                                     new SqlParameter("@LogDate",System.DateTime.Now.ToString("yyyy-MM-dd")),
                                     new SqlParameter("@LogTime",System.DateTime.Now.ToString("HH:mm:ss"))
                                     };*/
            /*SqlParameter[] Parameters = new SqlParameter[] {
                                     new SqlParameter("@LogDate",System.DateTime.Now),
                                     new SqlParameter("@LogTime",System.DateTime.Now)
            };*/
            // int iRET = CITYHUB.DBHelper.ExecuteSP(TB_DB_ConnectionString.Text, "dbo.usp_TestDateTime", Parameters);
            SqlParameter sqlP = new SqlParameter()
            {
                ParameterName = "@ret_code",
                Value = 0,
                Direction = ParameterDirection.Output
            };
            SqlParameter[] Parameters = new SqlParameter[] {
                new SqlParameter("@log_yyyymm",System.DateTime.Now.ToString("yyyyMM")),
                new SqlParameter("@SerialNumber", "SerialNumber"),
                new SqlParameter("@RelatedNumber","RelatedNumber"),
                new SqlParameter("@LogDate",System.DateTime.Now),
                new SqlParameter("@LogTime",System.DateTime.Now.ToString("HH:mm:ss.fffffff")),
                new SqlParameter("@Flag","Y"),
                new SqlParameter("@Request",System.DateTime.Now.ToString()),
                new SqlParameter("@Response",System.DateTime.Now.ToString()),
                sqlP
            };
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-ExecuteSP Go");
            int iRET = CITYHUB.DBHelper.ExecuteSP(TB_DB_ConnectionString_Trade.Text, "dbo.usp_Check_Insert_Log", Parameters);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-ExecuteSP  sqlP.Value : " + sqlP.Value.ToString());
        }

        private void bt_GetDataSetBySP_Click(object sender, EventArgs e)
        {
            SqlParameter[] Parameters = new SqlParameter[] {
                new SqlParameter("@id", TB_AppId.Text)
            };
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-GetDataSetBySP Go");
            DataSet ds = CITYHUB.DBHelper.GetDataSetBySP(TB_DB_ConnectionString_Trade.Text, "dbo.usp_GetApiKeyByAppid", Parameters);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-GetDataSetBySP OK");
            DGV_DB_DataSHow.DataSource = ds.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void bt_EncodeTest_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var obj = new AesTestModel();
            obj.apikey = tb_TestApiKey.Text;
            obj.apidata = tb_ApiDataTest.Text;


            req.version = "0.1";
            req.appid = "1234567890123456";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");

            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion
            req.ciphertext = JsonConvert.SerializeObject(obj);

            string s = JsonConvert.SerializeObject(req);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text ;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post EncodeTest To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post EncodeTest request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post EncodeTest response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        tb_ApiDataTest.Text = res.ciphertext;
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_QueryEncodeTest_Click(object sender, EventArgs e)
        {

        }

        private void bt_QueryDecodeTest_Click(object sender, EventArgs e)
        {

        }

        private void bt_DecodeTest_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var obj = new AesTestModel();
            obj.apikey = tb_TestApiKey.Text;
            obj.apidata = tb_ApiDataTest.Text;


            req.version = "0.1";
            req.appid = "1234567890123456";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion
            req.ciphertext = JsonConvert.SerializeObject(obj);

            string s = JsonConvert.SerializeObject(req);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));
            SysLBox.Items.Insert(0, "");
            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post DecodeTest To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post DecodeTest request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post DecodeTest response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        tb_ApiDataTest.Text = res.ciphertext;
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void MainFM_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = CITYHUB.CryptoHelper.AES_Encrypt(textBox1.Text, textBox2.Text);
        }

        private void bt_CreateXML_Click(object sender, EventArgs e)
        {
            EINV_MODEL.XML.V32.A0501Model.CancelInvoice a0501 = new EINV_MODEL.XML.V32.A0501Model.CancelInvoice();
            a0501.CancelInvoiceNumber = "RX16881688";
            a0501.InvoiceDate = System.DateTime.Now.ToString("yyyyMMdd");
            a0501.BuyerId = "0000000000";
            a0501.SellerId = "54659473";
            a0501.CancelDate = System.DateTime.Now.ToString("yyyyMMdd");
            a0501.CancelTime = System.DateTime.Now;
            a0501.CancelReason = "我是測試";
            try
            {
                //NG  schemaLocation --> xmlns:schemaLocation="urn:GEINV:eInvoiceMessage:A0501:3.1 A0501.xsd"
                //                          must be--> xsi:schemaLocation="urn:GEINV:eInvoiceMessage:A0501:3.1 A0501.xsd"
                /*string s = "testXmlTextWriter.xml";
                XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
                xmlNameSpace.Add("schemaLocation", "urn:GEINV:eInvoiceMessage:A0501:3.1 A0501.xsd");
                XmlSerializer serializer = new XmlSerializer(a0501.GetType());
                Stream fs = new FileStream(s, FileMode.Create);
                XmlWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, a0501, xmlNameSpace);
                writer.Close();*/



                //財政部範本
                //?xml version="1.0" encoding="utf-8"?>
                //< Invoice xsi: schemaLocation = "urn:GEINV:eInvoiceMessage:A0401:3.1 A0401.xsd" xmlns = "urn:GEINV:eInvoiceMessage:A0401:3.1" xmlns: xsi = "http://www.w3.org/2001/XMLSchema-instance" >

                //之前測試可上傳<Invoice xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="urn:GEINV:eInvoiceMessage:A0401:3.1">
                //testXmlTextWriter.xml  <?xml version="1.0" encoding="utf-8"?>
                //< CancelInvoice xmlns: xsi = "http://www.w3.org/2001/XMLSchema-instance" xmlns: xsd = "http://www.w3.org/2001/XMLSchema" xmlns = "urn:GEINV:eInvoiceMessage:A0501:3.2" >
                //這個有 encoding="utf-8"
                string s = "testXmlTextWriter.xml";
                XmlSerializer serializer = new XmlSerializer(a0501.GetType());
                Stream fs = new FileStream(s, FileMode.Create);
                XmlWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, a0501);
                writer.Close();

                //testFileStream.xml <CancelInvoice xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="urn:GEINV:eInvoiceMessage:A0501:3.2">
                s = "testFileStream.xml";
                XmlSerializer xml = new XmlSerializer(a0501.GetType());
                using (Stream stream = new FileStream(s, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle);
                    xml.Serialize(stream, a0501);
                    stream.Close();
                }

                s = "GenerateXml.xml";
                if (CITYHUB.Common.GenerateXml(a0501, s))
                    MessageBox.Show("XML DONE");
                else
                    MessageBox.Show("ERROR PLS CHECK");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:"+ex.Message);
            }
        }

        private void bt_TradeC0401_Click(object sender, EventArgs e)
        {
            
            var req = new RequestModel();
            var res = new ResponseMode();
            var c0401 = new InvoiceB2C_Model();
            c0401.main_InvoiceNumber = tb_InvoiceNO.Text;
            c0401.main_InvoiceDate = DateTime.Now.ToString("yyyyMMdd");
            c0401.main_InvoiceTime = DateTime.Now.ToString("HHmmss");
            c0401.main_InvoiceNumber = tb_InvoiceNO.Text;
            c0401.main_InvoiceDate = DateTime.Now.ToString("yyyyMMdd");
            c0401.main_InvoiceTime = DateTime.Now.ToString("HHmmss");
            c0401.seller_Identifier = "53222348";
            c0401.seller_Name = "亞億科技有限公司&#x0;&#x0;&#x0;&#x0;&#x0;&#x0;&#x0;&#x0;&#x0;&#x0;";
            c0401.seller_Address = "seller_address";
            c0401.seller_PersonInCharge = "seller_personincharge ";
            c0401.seller_TelephoneNumber = "11111111";
            c0401.seller_FacsimileNumber = "22222222";
            c0401.seller_EmailAddress = "xxx@mail.com";
            c0401.seller_CustomerNumber = "33333333";
            c0401.seller_RoleRemark = "seller_roleremark";
            c0401.buyer_Identifier = "0000000000";
            c0401.buyer_Name = "0000";
            c0401.buyer_Address = "buyer_address";
            c0401.buyer_PersonInCharge = "buyer_personincharge";
            c0401.buyer_TelephoneNumber = "44444444";
            c0401.buyer_FacsimileNumber = "55555555";
            c0401.buyer_EmailAddress = "xxx@mail.com";
            c0401.buyer_CustomerNumber = "66666666";
            c0401.buyer_RoleRemark = "buyer_roleremark";
            //c0401.main_CheckNumber = "";
            c0401.main_BuyerRemark = "1";
            c0401.main_MainRemark = "main_mainremark";
            c0401.main_CustomsClearanceMark = "1";
            c0401.main_Category = "38";
            c0401.main_RelateNumber = "relatenumber";
            c0401.main_InvoiceType = "07";
            c0401.main_GroupMark = "*";
            c0401.main_DonateMark = "0";
            //c0401.main_CarrierType = "carriertype";
            //c0401.main_CarrierId1 = "main_carrierid1";
            //c0401.main_CarrierId2 = "main_carrierid2";
            c0401.main_PrintMark = "Y";
            //c0401.main_NPOBAN = "npoban";
            c0401.main_RandomNumber = "5238";
            ProductItemModel productItem = new ProductItemModel
            {
                productItem_Description = "description1",
                productItem_Quantity = "2",
                productItem_Unit = "unit",
                productItem_UnitPrice = "1688",
                productItem_Amount = "3376",
                productItem_SequenceNumber = "004",
                productItem_Remark = "remark",
                productItem_RelateNumber = "relatenumber"
            };
            c0401.details.Add(productItem);

            ProductItemModel productItem2 = new ProductItemModel
            {
                productItem_Description = "description2",
                productItem_SequenceNumber = "005",
                productItem_Quantity = "3",
                productItem_UnitPrice = "1111",
                productItem_Amount = "3333"
            };
              
                
            c0401.details.Add(productItem2);

            c0401.amount_SalesAmount = "6709";
            c0401.amount_FreeTaxSalesAmount = "0";
            c0401.amount_ZeroTaxSalesAmount = "0";
            c0401.amount_TotalAmount = "6709";
            c0401.amount_TaxType = "1";
            c0401.amount_TaxRate = "0.05";
            c0401.amount_TaxAmount = "0";
            c0401.amount_DiscountAmount = "0";
            c0401.amount_OriginalCurrencyAmount = "0";
            c0401.amount_ExchangeRate = "0";
            c0401.amount_Currency = "NTD"; 


            req.version = "0.1";
            req.appid = "8351647501404101";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion


            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(c0401), "F3200F08109EB3BEDF808D04A8E41865");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeC0401 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeC0401 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeC0401 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SysLBox.Items.Add(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-POST Trade to EC WEB");
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(CB_ECwebUrl.Text);

            var store = new List<string>();
            store.Add("三重店");
            store.Add("板橋店");
            store.Add("中和店");
            store.Add("永和店");
            store.Add("蘆洲店");
            store.Add("五股店");
            var temp = new UploadTradeRequest();
            Random crandom = new Random();
            Random srandom = new Random();
            temp.Amount = crandom.Next(999);//float.Parse(crandom.Next(999).ToString());
            temp.StoreId = store[srandom.Next(5)];
            temp.CompanyId = "cityapp";
            string s = JsonConvert.SerializeObject(temp);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseData = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseData))
                {
                    s = sr.ReadToEnd();
                    SysLBox.Items.Add(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-POST Trade to EC WEB response code:" + response.StatusCode.ToString());
                }
            }
        }

        private void bt_TradeC0501_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var c0501 = new CancelModel();

            c0501.cancelInvoiceNumber = tb_InvoiceNO.Text;
            c0501.invoiceDate = DateTime.Now.ToString("yyyyMMdd");
            c0501.buyerId = "0000000000";
            c0501.sellerId = "11111111";
            c0501.cancelDate = DateTime.Now.ToString("yyyyMMdd");
            c0501.cancelTime = DateTime.Now.ToString("HHmmss");
            c0501.cancelReason = "cancelReason";
            c0501.returnTaxDocumentNumber = "returnTaxDocumentNumber";
            c0501.remark = "remarkremarkremarkremarkremarkremarkremarkremarkremarkremark";

            req.version = "0.1";
            req.appid = "8351647501404101";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(c0501), "F3200F08109EB3BEDF808D04A8E41865");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeC0501 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeC0501 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post C0501 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_TradeD0401_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var d0401 = new AllowanceModel();
            d0401.main_AllowanceNumber = tb_allowanceNO.Text;
            d0401.main_AllowanceDate = DateTime.Now.ToString("yyyyMMdd");
            d0401.main_AllowanceType = "2";
            d0401.seller_Identifier = "53222348";
            d0401.seller_Name = "亞億科技有限公司";
            d0401.seller_Address = "seller_address";
            d0401.seller_PersonInCharge = "seller_personincharge ";
            d0401.seller_TelephoneNumber = "11111111";
            d0401.seller_FacsimileNumber = "22222222";
            d0401.seller_EmailAddress = "xxx@mail.com";
            d0401.seller_CustomerNumber = "33333333";
            d0401.seller_RoleRemark = "seller_roleremark";
            d0401.buyer_Identifier = "0000000000";
            d0401.buyer_Name = "0000";
            d0401.buyer_Address = "buyer_address";
            d0401.buyer_PersonInCharge = "buyer_personincharge";
            d0401.buyer_TelephoneNumber = "44444444";
            d0401.buyer_FacsimileNumber = "55555555";
            d0401.buyer_EmailAddress = "xxx@mail.com";
            d0401.buyer_CustomerNumber = "66666666";
            d0401.buyer_RoleRemark = "buyer_roleremark";


            AllowanceProductItemModel productItem = new AllowanceProductItemModel
            {
                productItem_OriginalInvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
            productItem_OriginalInvoiceNumber = tb_InvoiceNO.Text,
                productItem_OriginalSequenceNumber = "002",
                productItem_OriginalDescription = "description2",
                productItem_Quantity = "1",
                productItem_Unit = "unit",
                productItem_UnitPrice = "1111",
                productItem_Amount = "1058",
                productItem_Tax = "53",
                productItem_AllowanceSequenceNumber = "001",
                productItem_TaxType = "1"
            };
            d0401.details.Add(productItem);
            d0401.amount_TotalAmount = "1058";
            d0401.amount_TaxAmount = "53";



            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion


            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(d0401), "427E3F32B540EC26060E3DC8692F95F9");
            #endregion

            string s = JsonConvert.SerializeObject(req);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeD0401 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeD0401 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeD0401 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_TradeD0501_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var d0501 = new AllowanceCancelModel();

            d0501.cancelAllowanceNumber = tb_allowanceNO.Text;
            d0501.allowanceDate = DateTime.Now.ToString("yyyyMMdd");
            d0501.buyerId = "0000000000";
            d0501.sellerId = "53222348";
            d0501.cancelDate = DateTime.Now.ToString("yyyyMMdd");
            d0501.cancelTime = DateTime.Now.ToString("HHmmss");
            d0501.cancelReason = "cancelReason";
            d0501.remark = "remarkremarkremarkremarkremarkremarkremarkremarkremarkremark";

            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(d0501), "12345678901234567890123456789012");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeD0501 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeD0501 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post D0501 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_TradeVoidCancel_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var voidcancel = new VoidInvoiceModel();

            voidcancel.voidInvoiceNumber = tb_InvoiceNO.Text;
            voidcancel.invoiceDate = DateTime.Now.ToString("yyyyMMdd");
            voidcancel.buyerId = "0000000000";
            voidcancel.sellerId = "53222348";
            voidcancel.voidDate = DateTime.Now.ToString("yyyyMMdd");
            voidcancel.voidTime = DateTime.Now.ToString("HHmmss");
            voidcancel.voidReason = "voidReason";
            voidcancel.remark = "remarkremarkremarkremarkremarkremarkremarkremarkremarkremark";

            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(voidcancel), "12345678901234567890123456789012");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeVoidCancel To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeVoidCancel request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeVoidCancel response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void tb_InvoiceNO_TextChanged(object sender, EventArgs e)
        {

        }

        private void bt_TradeA0401_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var a0401 = new InvoiceB2B_Model();
            a0401.main_InvoiceNumber = tb_InvoiceNO.Text;
            a0401.main_InvoiceDate = DateTime.Now.ToString("yyyyMMdd");
            a0401.main_InvoiceTime = DateTime.Now.ToString("HHmmss");
            a0401.main_InvoiceNumber = tb_InvoiceNO.Text;
            a0401.main_InvoiceDate = DateTime.Now.ToString("yyyyMMdd");
            a0401.main_InvoiceTime = DateTime.Now.ToString("HHmmss");
            a0401.seller_Identifier = "53222348";
            a0401.seller_Name = "亞億科技有限公司";
            a0401.seller_Address = "seller_address";
            a0401.seller_PersonInCharge = "seller_personincharge ";
            a0401.seller_TelephoneNumber = "11111111";
            a0401.seller_FacsimileNumber = "22222222";
            a0401.seller_EmailAddress = "xxx@mail.com";
            a0401.seller_CustomerNumber = "33333333";
            a0401.seller_RoleRemark = "seller_roleremark";
            a0401.buyer_Identifier = "82923777";
            a0401.buyer_Name = "旌泓股份有限公司";
            a0401.buyer_Address = "buyer_address";
            a0401.buyer_PersonInCharge = "buyer_personincharge";
            a0401.buyer_TelephoneNumber = "44444444";
            a0401.buyer_FacsimileNumber = "55555555";
            a0401.buyer_EmailAddress = "xxx@mail.com";
            a0401.buyer_CustomerNumber = "66666666";
            a0401.buyer_RoleRemark = "buyer_roleremark";
            a0401.main_CheckNumber = "b2bCheckNo";
            a0401.main_BuyerRemark = "1";
            a0401.main_MainRemark = "main_mainremark";
            a0401.main_CustomsClearanceMark = "2";
            a0401.main_Category = "38";
            a0401.main_RelateNumber = "relatenumber";
            a0401.main_InvoiceType = "07";
            a0401.main_GroupMark = "*";
            a0401.main_DonateMark = "0";
            ProductItemModel productItem = new ProductItemModel
            {
                productItem_Description = "description1",
                productItem_Quantity = "2",
                productItem_Unit = "unit",
                productItem_UnitPrice = "1688",
                productItem_Amount = "3376",
                productItem_SequenceNumber = "001",
                productItem_Remark = "remark",
                productItem_RelateNumber = "relatenumber"
            };
            a0401.details.Add(productItem);

            ProductItemModel productItem2 = new ProductItemModel
            {
                productItem_Description = "description2",
                productItem_SequenceNumber = "002",
                productItem_Quantity = "3",
                productItem_UnitPrice = "1111",
                productItem_Amount = "3333"
            };


            a0401.details.Add(productItem2);

            a0401.amount_SalesAmount = "6709";
            a0401.amount_TotalAmount = "6709";
            a0401.amount_TaxType = "1";
            a0401.amount_TaxRate = "0.05";
            a0401.amount_TaxAmount = "0";
            a0401.amount_DiscountAmount = "0";
            a0401.amount_OriginalCurrencyAmount = "0";
            a0401.amount_ExchangeRate = "0";
            a0401.amount_Currency = "NTD";


            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion


            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(a0401), "12345678901234567890123456789012");
            #endregion

            string s = JsonConvert.SerializeObject(req);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeA0401 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeA0401 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeA0401 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string serviceURL = comboBox1.Text;

            string s = "i am rex";
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post test To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post test request : " + s);
            Application.DoEvents();

            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post test response : " + result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_TradeA0501_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var c0501 = new CancelModel();

            c0501.cancelInvoiceNumber = tb_InvoiceNO.Text;
            c0501.invoiceDate = DateTime.Now.ToString("yyyyMMdd");
            c0501.buyerId = "82923777";
            c0501.sellerId = "53222348";
            c0501.cancelDate = DateTime.Now.ToString("yyyyMMdd");
            c0501.cancelTime = DateTime.Now.ToString("HHmmss");
            c0501.cancelReason = "cancelReason";
            c0501.returnTaxDocumentNumber = "returnTaxDocumentNumber";
            c0501.remark = "remarkremarkremarkremarkremarkremarkremarkremarkremarkremark";

            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(c0501), "12345678901234567890123456789012");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeA0501 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeA0501 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post A0501 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_B0401_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var d0401 = new AllowanceModel();
            d0401.main_AllowanceNumber = tb_allowanceNO.Text;
            d0401.main_AllowanceDate = DateTime.Now.ToString("yyyyMMdd");
            d0401.main_AllowanceType = "2";
            d0401.seller_Identifier = "53222348";
            d0401.seller_Name = "亞億科技有限公司";
            d0401.seller_Address = "seller_address";
            d0401.seller_PersonInCharge = "seller_personincharge ";
            d0401.seller_TelephoneNumber = "11111111";
            d0401.seller_FacsimileNumber = "22222222";
            d0401.seller_EmailAddress = "xxx@mail.com";
            d0401.seller_CustomerNumber = "33333333";
            d0401.seller_RoleRemark = "seller_roleremark";
            d0401.buyer_Identifier = "82923777";
            d0401.buyer_Name = "82923777";
            d0401.buyer_Address = "buyer_address";
            d0401.buyer_PersonInCharge = "buyer_personincharge";
            d0401.buyer_TelephoneNumber = "44444444";
            d0401.buyer_FacsimileNumber = "55555555";
            d0401.buyer_EmailAddress = "xxx@mail.com";
            d0401.buyer_CustomerNumber = "66666666";
            d0401.buyer_RoleRemark = "buyer_roleremark";


            AllowanceProductItemModel productItem = new AllowanceProductItemModel
            {
                productItem_OriginalInvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                productItem_OriginalInvoiceNumber = tb_InvoiceNO.Text,
                productItem_OriginalSequenceNumber = "002",
                productItem_OriginalDescription = "description2",
                productItem_Quantity = "1",
                productItem_Unit = "unit",
                productItem_UnitPrice = "1111",
                productItem_Amount = "1058",
                productItem_Tax = "53",
                productItem_AllowanceSequenceNumber = "001",
                productItem_TaxType = "1"
            };
            d0401.details.Add(productItem);
            d0401.amount_TotalAmount = "1058";
            d0401.amount_TaxAmount = "53";



            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion


            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(d0401), "427E3F32B540EC26060E3DC8692F95F9");
            #endregion

            string s = JsonConvert.SerializeObject(req);
            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeB0401 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeB0401 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeB0401 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_B0501_Click(object sender, EventArgs e)
        {
            var req = new RequestModel();
            var res = new ResponseMode();
            var d0501 = new AllowanceCancelModel();

            d0501.cancelAllowanceNumber = tb_allowanceNO.Text;
            d0501.allowanceDate = DateTime.Now.ToString("yyyyMMdd");
            d0501.buyerId = "82923777";
            d0501.sellerId = "53222348";
            d0501.cancelDate = DateTime.Now.ToString("yyyyMMdd");
            d0501.cancelTime = DateTime.Now.ToString("HHmmss");
            d0501.cancelReason = "cancelReason";
            d0501.remark = "remarkremarkremarkremarkremarkremarkremarkremarkremarkremark";

            req.version = "0.1";
            req.appid = "5322234801999999";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(d0501), "12345678901234567890123456789012");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));

            string serviceURL = comboBox1.Text;
            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeB0501 To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post TradeB0501 request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post B0501 response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void bt_GetClientBatchInvoice_Click(object sender, EventArgs e)
        {
            string serviceURL = comboBox1.Text;
            var req = new RequestModel();
            var res = new ResponseMode();
            var getBatch = new BatchInvoiceModel();
            var BatchInfo = new BatchInvoiceIntervalModel();
            getBatch.invoice_Term = TB_Term.Text;
            getBatch.invoice_Batch = CB_Batch.Text;
            getBatch.invoice_Type = "07";

            req.version = "0.1";
            req.appid = "8351647501404101";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            #region M.A.C
            //appid+ serial+ clientdate+ clienttime
            req.msgauthcode = CITYHUB.CryptoHelper.MD5(req.appid + req.serial + req.clientdatetime);
            #endregion

            #region AES加密
            req.ciphertext = CITYHUB.CryptoHelper.AES_Encrypt(JsonConvert.SerializeObject(getBatch), "F3200F08109EB3BEDF808D04A8E41865");
            #endregion

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));


            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post GetClientBatchInvoice To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post GetClientBatchInvoice request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post GetClientBatchInvoice response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        BatchInfo = JsonConvert.DeserializeObject<BatchInvoiceIntervalModel>(CITYHUB.CryptoHelper.AES_Decrype(res.ciphertext, "F3200F08109EB3BEDF808D04A8E41865"));
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                        SysLBox.Items.Insert(0, "res.ciphertext AES_Decrype : BatchInfo->" +BatchInfo.invoice_Term +" "+ BatchInfo.invoice_Batch +" "+ BatchInfo.invoice_Track + BatchInfo.invoice_BeginNumber +"-"+ BatchInfo.invoice_EndNumber);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
        }

        private void Post_Timer_Tick(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*Post_Timer.Interval = int.Parse(tb_Sec.Text);
            Post_Timer.Enabled = !Post_Timer.Enabled;
            if (Post_Timer.Enabled)
                SysLBox.Items.Add(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-GO GO GO");
            else
                SysLBox.Items.Add(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-STOP");*/
            if (CITYHUB.Common.ValidateString("-1685F", 20))
                MessageBox.Show("OK");
            else
                MessageBox.Show("NG");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox3.Text = CITYHUB.CryptoHelper.AES_Decrype(textBox1.Text, textBox2.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string serviceURL = "http://127.0.0.1:52388/Hello/RetGo";
            var req = new RequestModel();
            var res = new ResponseMode();
            var posEchoCont = new PostEchoModel();
            posEchoCont.echodata = "Hi Service";
            posEchoCont.memo = "我是 CLIENT";

            req.version = "0.1";
            req.appid = "8351647501404101";
            req.serial = "5238523838";
            req.clientdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
         

            string s = JsonConvert.SerializeObject(req);

            Encoding enc = Encoding.UTF8;
            int iBC = enc.GetByteCount(s.ToCharArray(), 0, s.Length);
            byte[] byteArray = new byte[iBC];
            enc.GetBytes(s, 0, s.Length, byteArray, byteArray.GetLowerBound(0));


            SysLBox.Items.Insert(0, "");
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho To" + serviceURL);
            SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho request : " + s);
            Application.DoEvents();
            string result = "";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(serviceURL);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseData = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(responseData))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result))
                {
                    SysLBox.Items.Insert(0, System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "-Post PostEcho response : " + result);
                    res = JsonConvert.DeserializeObject<ResponseMode>(result);
                    if (res.code.Equals("9000"))
                    {
                        posEchoCont = JsonConvert.DeserializeObject<PostEchoModel>(CITYHUB.CryptoHelper.AES_Decrype(res.ciphertext, "427E3F32B540EC26060E3DC8692F95F9"));
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                        SysLBox.Items.Insert(0, "res.ciphertext AES_Decrype : posecho.echodata->" + posEchoCont.echodata + "    posecho.memo->" + posEchoCont.memo);
                    }
                    else
                    {
                        SysLBox.Items.Insert(0, "res.code : " + res.code + "    res.msg : " + res.msg + "    res.ciphertext : " + res.ciphertext);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex:" + ex.Message);
            }
            //bt_TradeC0401.PerformClick();
            //bt_TradeC0401.PerformClick();
        }
    }
}
