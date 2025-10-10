
namespace EinvServiceTestClient
{
    partial class MainFM
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btPost = new System.Windows.Forms.Button();
            this.SysLBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_ExecutePRM = new System.Windows.Forms.Button();
            this.bt_GetDataSetByPRM = new System.Windows.Forms.Button();
            this.TB_DB_ConnectionString_Query = new System.Windows.Forms.TextBox();
            this.DGV_DB_DataSHow = new System.Windows.Forms.DataGridView();
            this.TB_AppId = new System.Windows.Forms.TextBox();
            this.TB_ApiKey = new System.Windows.Forms.TextBox();
            this.bt_ExecuteSP = new System.Windows.Forms.Button();
            this.bt_GetDataSetBySP = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.tb_Sec = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.CB_ECwebUrl = new System.Windows.Forms.ComboBox();
            this.tb_ApiDataTest = new System.Windows.Forms.TextBox();
            this.tb_TestApiKey = new System.Windows.Forms.TextBox();
            this.bt_EncodeTest = new System.Windows.Forms.Button();
            this.bt_DecodeTest = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.TB_DB_ConnectionString_Trade = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CB_Batch = new System.Windows.Forms.ComboBox();
            this.TB_Term = new System.Windows.Forms.TextBox();
            this.bt_GetClientBatchInvoice = new System.Windows.Forms.Button();
            this.bt_B0501 = new System.Windows.Forms.Button();
            this.bt_B0401 = new System.Windows.Forms.Button();
            this.bt_TradeA0501 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.bt_TradeA0401 = new System.Windows.Forms.Button();
            this.bt_TradeVoidCancel = new System.Windows.Forms.Button();
            this.bt_TradeD0501 = new System.Windows.Forms.Button();
            this.tb_allowanceNO = new System.Windows.Forms.TextBox();
            this.bt_TradeD0401 = new System.Windows.Forms.Button();
            this.bt_TradeC0501 = new System.Windows.Forms.Button();
            this.tb_InvoiceNO = new System.Windows.Forms.TextBox();
            this.bt_TradeC0401 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.bt_CreateXML = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Post_Timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_DB_DataSHow)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btPost
            // 
            this.btPost.Location = new System.Drawing.Point(15, 12);
            this.btPost.Name = "btPost";
            this.btPost.Size = new System.Drawing.Size(150, 50);
            this.btPost.TabIndex = 0;
            this.btPost.Text = "Post Ehco";
            this.btPost.UseVisualStyleBackColor = true;
            this.btPost.Click += new System.EventHandler(this.btPost_Click);
            // 
            // SysLBox
            // 
            this.SysLBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SysLBox.FormattingEnabled = true;
            this.SysLBox.ItemHeight = 20;
            this.SysLBox.Location = new System.Drawing.Point(0, 675);
            this.SysLBox.Name = "SysLBox";
            this.SysLBox.Size = new System.Drawing.Size(1401, 324);
            this.SysLBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Service";
            // 
            // bt_ExecutePRM
            // 
            this.bt_ExecutePRM.Location = new System.Drawing.Point(8, 87);
            this.bt_ExecutePRM.Name = "bt_ExecutePRM";
            this.bt_ExecutePRM.Size = new System.Drawing.Size(180, 50);
            this.bt_ExecutePRM.TabIndex = 4;
            this.bt_ExecutePRM.Text = "ExecutePRM";
            this.bt_ExecutePRM.UseVisualStyleBackColor = true;
            this.bt_ExecutePRM.Click += new System.EventHandler(this.bt_ExecutePRM_Click);
            // 
            // bt_GetDataSetByPRM
            // 
            this.bt_GetDataSetByPRM.Location = new System.Drawing.Point(8, 143);
            this.bt_GetDataSetByPRM.Name = "bt_GetDataSetByPRM";
            this.bt_GetDataSetByPRM.Size = new System.Drawing.Size(180, 50);
            this.bt_GetDataSetByPRM.TabIndex = 5;
            this.bt_GetDataSetByPRM.Text = "GetDataSetByPRM";
            this.bt_GetDataSetByPRM.UseVisualStyleBackColor = true;
            this.bt_GetDataSetByPRM.Click += new System.EventHandler(this.bt_GetDataSetByPRM_Click);
            // 
            // TB_DB_ConnectionString_Query
            // 
            this.TB_DB_ConnectionString_Query.Location = new System.Drawing.Point(12, 51);
            this.TB_DB_ConnectionString_Query.Name = "TB_DB_ConnectionString_Query";
            this.TB_DB_ConnectionString_Query.Size = new System.Drawing.Size(536, 29);
            this.TB_DB_ConnectionString_Query.TabIndex = 6;
            this.TB_DB_ConnectionString_Query.Text = "server=.; database=eInvQueryDB; uid=GB2021; pwd=1qaz2WSX3edc;";
            // 
            // DGV_DB_DataSHow
            // 
            this.DGV_DB_DataSHow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_DB_DataSHow.Location = new System.Drawing.Point(554, 16);
            this.DGV_DB_DataSHow.Name = "DGV_DB_DataSHow";
            this.DGV_DB_DataSHow.RowTemplate.Height = 24;
            this.DGV_DB_DataSHow.Size = new System.Drawing.Size(437, 228);
            this.DGV_DB_DataSHow.TabIndex = 7;
            // 
            // TB_AppId
            // 
            this.TB_AppId.Location = new System.Drawing.Point(211, 87);
            this.TB_AppId.MaxLength = 16;
            this.TB_AppId.Name = "TB_AppId";
            this.TB_AppId.Size = new System.Drawing.Size(337, 29);
            this.TB_AppId.TabIndex = 8;
            // 
            // TB_ApiKey
            // 
            this.TB_ApiKey.Location = new System.Drawing.Point(211, 143);
            this.TB_ApiKey.MaxLength = 32;
            this.TB_ApiKey.Name = "TB_ApiKey";
            this.TB_ApiKey.Size = new System.Drawing.Size(337, 29);
            this.TB_ApiKey.TabIndex = 9;
            // 
            // bt_ExecuteSP
            // 
            this.bt_ExecuteSP.Location = new System.Drawing.Point(211, 197);
            this.bt_ExecuteSP.Name = "bt_ExecuteSP";
            this.bt_ExecuteSP.Size = new System.Drawing.Size(150, 50);
            this.bt_ExecuteSP.TabIndex = 10;
            this.bt_ExecuteSP.Text = "ExecuteSP";
            this.bt_ExecuteSP.UseVisualStyleBackColor = true;
            this.bt_ExecuteSP.Click += new System.EventHandler(this.bt_ExecuteSP_Click);
            // 
            // bt_GetDataSetBySP
            // 
            this.bt_GetDataSetBySP.Location = new System.Drawing.Point(398, 197);
            this.bt_GetDataSetBySP.Name = "bt_GetDataSetBySP";
            this.bt_GetDataSetBySP.Size = new System.Drawing.Size(150, 50);
            this.bt_GetDataSetBySP.TabIndex = 11;
            this.bt_GetDataSetBySP.Text = "GetDataSetBySP";
            this.bt_GetDataSetBySP.UseVisualStyleBackColor = true;
            this.bt_GetDataSetBySP.Click += new System.EventHandler(this.bt_GetDataSetBySP_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.tb_Sec);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.CB_ECwebUrl);
            this.panel1.Controls.Add(this.tb_ApiDataTest);
            this.panel1.Controls.Add(this.tb_TestApiKey);
            this.panel1.Controls.Add(this.bt_EncodeTest);
            this.panel1.Controls.Add(this.bt_DecodeTest);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Location = new System.Drawing.Point(558, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(831, 353);
            this.panel1.TabIndex = 14;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(115, 184);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(107, 38);
            this.button6.TabIndex = 27;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // tb_Sec
            // 
            this.tb_Sec.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_Sec.Location = new System.Drawing.Point(564, 208);
            this.tb_Sec.MaxLength = 5;
            this.tb_Sec.Name = "tb_Sec";
            this.tb_Sec.Size = new System.Drawing.Size(110, 39);
            this.tb_Sec.TabIndex = 26;
            this.tb_Sec.Text = "8888";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(698, 210);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(119, 37);
            this.button4.TabIndex = 20;
            this.button4.Text = "GO";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // CB_ECwebUrl
            // 
            this.CB_ECwebUrl.FormattingEnabled = true;
            this.CB_ECwebUrl.Items.AddRange(new object[] {
            "========Local用========",
            "http://localhost/EinvGW/EinvGW/GwSelfTest",
            "http://localhost/EinvTradeSrv/EinvTradeSrv/TradeSelfTest",
            "http://localhost/EinvQuerySrv/EinvQuerySrv/QuerySelfTest",
            "http://localhost/EinvGW/EinvGW/V1/GetClientBatchInvoice",
            "http://localhost/EinvGW/EinvGW/V1/PostEcho",
            "http://localhost/EinvGW/EinvGW/V1/TradeC0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeC0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeVoidCancel",
            "http://localhost/EinvGW/EinvGW/V1/TradeA0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeA0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeD0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeD0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeB0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeB0501",
            "http://localhost/EinvGW/EinvGW/V1/EncodeTest",
            "http://localhost/EinvGW/EinvGW/V1/DecodeTest",
            "========雲端Local用========",
            "http://127.0.0.1:34010/EinvGW/GwSelfTest",
            "http://127.0.0.1:34000/EinvTradeSrv/TradeSelfTest",
            "http://127.0.0.1:34001/EinvQuerySrv/QuerySelfTest",
            "========雲端 Query========",
            "http://61.218.22.135:34010/EinvGW/GwSelfTest",
            "http://61.218.22.135:34010/EinvGW/V1/EncodeTest",
            "http://61.218.22.135:34010/EinvGW/V1/DecodeTest",
            "========雲端 Trade=======",
            "http://61.218.22.135:34010/EinvGW/V1/PostEcho",
            "http://61.218.22.135:34010/EinvGW/V1/GetClientBatchInvoice",
            "http://61.218.22.135:34010/EinvGW/V1/TradeC0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeC0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeVoidCancel",
            "http://61.218.22.135:34010/EinvGW/V1/TradeD0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeD0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeA0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeA0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeB0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeB0501"});
            this.CB_ECwebUrl.Location = new System.Drawing.Point(12, 253);
            this.CB_ECwebUrl.Name = "CB_ECwebUrl";
            this.CB_ECwebUrl.Size = new System.Drawing.Size(805, 28);
            this.CB_ECwebUrl.TabIndex = 19;
            this.CB_ECwebUrl.Text = "http://61.218.22.135:31080/api/dashboard/v1/upload-revenue";
            // 
            // tb_ApiDataTest
            // 
            this.tb_ApiDataTest.Location = new System.Drawing.Point(162, 77);
            this.tb_ApiDataTest.MaxLength = 16;
            this.tb_ApiDataTest.Name = "tb_ApiDataTest";
            this.tb_ApiDataTest.Size = new System.Drawing.Size(655, 29);
            this.tb_ApiDataTest.TabIndex = 10;
            this.tb_ApiDataTest.Text = "I AM api dat Test 我是測試壓解密的資料";
            // 
            // tb_TestApiKey
            // 
            this.tb_TestApiKey.Location = new System.Drawing.Point(162, 21);
            this.tb_TestApiKey.MaxLength = 32;
            this.tb_TestApiKey.Name = "tb_TestApiKey";
            this.tb_TestApiKey.Size = new System.Drawing.Size(655, 29);
            this.tb_TestApiKey.TabIndex = 9;
            this.tb_TestApiKey.Text = "12345678123456781234567812345678";
            // 
            // bt_EncodeTest
            // 
            this.bt_EncodeTest.Location = new System.Drawing.Point(6, 9);
            this.bt_EncodeTest.Name = "bt_EncodeTest";
            this.bt_EncodeTest.Size = new System.Drawing.Size(150, 50);
            this.bt_EncodeTest.TabIndex = 1;
            this.bt_EncodeTest.Text = "Encode Test";
            this.bt_EncodeTest.UseVisualStyleBackColor = true;
            this.bt_EncodeTest.Click += new System.EventHandler(this.bt_EncodeTest_Click);
            // 
            // bt_DecodeTest
            // 
            this.bt_DecodeTest.Location = new System.Drawing.Point(6, 65);
            this.bt_DecodeTest.Name = "bt_DecodeTest";
            this.bt_DecodeTest.Size = new System.Drawing.Size(150, 50);
            this.bt_DecodeTest.TabIndex = 2;
            this.bt_DecodeTest.Text = "Decode Test";
            this.bt_DecodeTest.UseVisualStyleBackColor = true;
            this.bt_DecodeTest.Click += new System.EventHandler(this.bt_DecodeTest_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 292);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(805, 44);
            this.button2.TabIndex = 12;
            this.button2.Text = "POST EC WEB RealTime Trade";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TB_DB_ConnectionString_Trade
            // 
            this.TB_DB_ConnectionString_Trade.Location = new System.Drawing.Point(12, 16);
            this.TB_DB_ConnectionString_Trade.Name = "TB_DB_ConnectionString_Trade";
            this.TB_DB_ConnectionString_Trade.Size = new System.Drawing.Size(536, 29);
            this.TB_DB_ConnectionString_Trade.TabIndex = 15;
            this.TB_DB_ConnectionString_Trade.Text = "server=.; database=eInvTradeDB; uid=GB2021; pwd=1qaz2WSX3edc;";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel2.Controls.Add(this.CB_Batch);
            this.panel2.Controls.Add(this.TB_Term);
            this.panel2.Controls.Add(this.bt_GetClientBatchInvoice);
            this.panel2.Controls.Add(this.bt_B0501);
            this.panel2.Controls.Add(this.bt_B0401);
            this.panel2.Controls.Add(this.bt_TradeA0501);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.bt_TradeA0401);
            this.panel2.Controls.Add(this.bt_TradeVoidCancel);
            this.panel2.Controls.Add(this.bt_TradeD0501);
            this.panel2.Controls.Add(this.tb_allowanceNO);
            this.panel2.Controls.Add(this.bt_TradeD0401);
            this.panel2.Controls.Add(this.bt_TradeC0501);
            this.panel2.Controls.Add(this.tb_InvoiceNO);
            this.panel2.Controls.Add(this.bt_TradeC0401);
            this.panel2.Controls.Add(this.btPost);
            this.panel2.Location = new System.Drawing.Point(16, 51);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(536, 353);
            this.panel2.TabIndex = 16;
            // 
            // CB_Batch
            // 
            this.CB_Batch.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CB_Batch.FormattingEnabled = true;
            this.CB_Batch.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09"});
            this.CB_Batch.Location = new System.Drawing.Point(443, 184);
            this.CB_Batch.Name = "CB_Batch";
            this.CB_Batch.Size = new System.Drawing.Size(74, 38);
            this.CB_Batch.TabIndex = 25;
            // 
            // TB_Term
            // 
            this.TB_Term.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TB_Term.Location = new System.Drawing.Point(327, 184);
            this.TB_Term.MaxLength = 5;
            this.TB_Term.Name = "TB_Term";
            this.TB_Term.Size = new System.Drawing.Size(110, 39);
            this.TB_Term.TabIndex = 24;
            this.TB_Term.Text = "11008";
            // 
            // bt_GetClientBatchInvoice
            // 
            this.bt_GetClientBatchInvoice.Location = new System.Drawing.Point(130, 180);
            this.bt_GetClientBatchInvoice.Name = "bt_GetClientBatchInvoice";
            this.bt_GetClientBatchInvoice.Size = new System.Drawing.Size(191, 50);
            this.bt_GetClientBatchInvoice.TabIndex = 23;
            this.bt_GetClientBatchInvoice.Text = "GetClientBatchInvoice";
            this.bt_GetClientBatchInvoice.UseVisualStyleBackColor = true;
            this.bt_GetClientBatchInvoice.Click += new System.EventHandler(this.bt_GetClientBatchInvoice_Click);
            // 
            // bt_B0501
            // 
            this.bt_B0501.Location = new System.Drawing.Point(15, 298);
            this.bt_B0501.Name = "bt_B0501";
            this.bt_B0501.Size = new System.Drawing.Size(150, 50);
            this.bt_B0501.TabIndex = 22;
            this.bt_B0501.Text = "TradeB0501";
            this.bt_B0501.UseVisualStyleBackColor = true;
            this.bt_B0501.Click += new System.EventHandler(this.bt_B0501_Click);
            // 
            // bt_B0401
            // 
            this.bt_B0401.Location = new System.Drawing.Point(171, 295);
            this.bt_B0401.Name = "bt_B0401";
            this.bt_B0401.Size = new System.Drawing.Size(150, 50);
            this.bt_B0401.TabIndex = 21;
            this.bt_B0401.Text = "TradeB0401";
            this.bt_B0401.UseVisualStyleBackColor = true;
            this.bt_B0401.Click += new System.EventHandler(this.bt_B0401_Click);
            // 
            // bt_TradeA0501
            // 
            this.bt_TradeA0501.Location = new System.Drawing.Point(327, 124);
            this.bt_TradeA0501.Name = "bt_TradeA0501";
            this.bt_TradeA0501.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeA0501.TabIndex = 20;
            this.bt_TradeA0501.Text = "TradeA0501";
            this.bt_TradeA0501.UseVisualStyleBackColor = true;
            this.bt_TradeA0501.Click += new System.EventHandler(this.bt_TradeA0501_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 68);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 50);
            this.button3.TabIndex = 19;
            this.button3.Text = "test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // bt_TradeA0401
            // 
            this.bt_TradeA0401.Location = new System.Drawing.Point(327, 68);
            this.bt_TradeA0401.Name = "bt_TradeA0401";
            this.bt_TradeA0401.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeA0401.TabIndex = 18;
            this.bt_TradeA0401.Text = "TradeA0401";
            this.bt_TradeA0401.UseVisualStyleBackColor = true;
            this.bt_TradeA0401.Click += new System.EventHandler(this.bt_TradeA0401_Click);
            // 
            // bt_TradeVoidCancel
            // 
            this.bt_TradeVoidCancel.Location = new System.Drawing.Point(171, 124);
            this.bt_TradeVoidCancel.Name = "bt_TradeVoidCancel";
            this.bt_TradeVoidCancel.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeVoidCancel.TabIndex = 17;
            this.bt_TradeVoidCancel.Text = "TradeVoidCancel";
            this.bt_TradeVoidCancel.UseVisualStyleBackColor = true;
            this.bt_TradeVoidCancel.Click += new System.EventHandler(this.bt_TradeVoidCancel_Click);
            // 
            // bt_TradeD0501
            // 
            this.bt_TradeD0501.Location = new System.Drawing.Point(15, 239);
            this.bt_TradeD0501.Name = "bt_TradeD0501";
            this.bt_TradeD0501.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeD0501.TabIndex = 16;
            this.bt_TradeD0501.Text = "TradeD0501";
            this.bt_TradeD0501.UseVisualStyleBackColor = true;
            this.bt_TradeD0501.Click += new System.EventHandler(this.bt_TradeD0501_Click);
            // 
            // tb_allowanceNO
            // 
            this.tb_allowanceNO.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_allowanceNO.Location = new System.Drawing.Point(327, 248);
            this.tb_allowanceNO.MaxLength = 16;
            this.tb_allowanceNO.Name = "tb_allowanceNO";
            this.tb_allowanceNO.Size = new System.Drawing.Size(190, 33);
            this.tb_allowanceNO.TabIndex = 15;
            this.tb_allowanceNO.Text = "109RX00000001001";
            // 
            // bt_TradeD0401
            // 
            this.bt_TradeD0401.Location = new System.Drawing.Point(171, 239);
            this.bt_TradeD0401.Name = "bt_TradeD0401";
            this.bt_TradeD0401.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeD0401.TabIndex = 14;
            this.bt_TradeD0401.Text = "TradeD0401";
            this.bt_TradeD0401.UseVisualStyleBackColor = true;
            this.bt_TradeD0401.Click += new System.EventHandler(this.bt_TradeD0401_Click);
            // 
            // bt_TradeC0501
            // 
            this.bt_TradeC0501.Location = new System.Drawing.Point(171, 68);
            this.bt_TradeC0501.Name = "bt_TradeC0501";
            this.bt_TradeC0501.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeC0501.TabIndex = 13;
            this.bt_TradeC0501.Text = "TradeC0501";
            this.bt_TradeC0501.UseVisualStyleBackColor = true;
            this.bt_TradeC0501.Click += new System.EventHandler(this.bt_TradeC0501_Click);
            // 
            // tb_InvoiceNO
            // 
            this.tb_InvoiceNO.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_InvoiceNO.Location = new System.Drawing.Point(327, 16);
            this.tb_InvoiceNO.MaxLength = 16;
            this.tb_InvoiceNO.Name = "tb_InvoiceNO";
            this.tb_InvoiceNO.Size = new System.Drawing.Size(190, 39);
            this.tb_InvoiceNO.TabIndex = 11;
            this.tb_InvoiceNO.Text = "LY41721050";
            this.tb_InvoiceNO.TextChanged += new System.EventHandler(this.tb_InvoiceNO_TextChanged);
            // 
            // bt_TradeC0401
            // 
            this.bt_TradeC0401.Location = new System.Drawing.Point(171, 12);
            this.bt_TradeC0401.Name = "bt_TradeC0401";
            this.bt_TradeC0401.Size = new System.Drawing.Size(150, 50);
            this.bt_TradeC0401.TabIndex = 1;
            this.bt_TradeC0401.Text = "TradeC0401";
            this.bt_TradeC0401.UseVisualStyleBackColor = true;
            this.bt_TradeC0401.Click += new System.EventHandler(this.bt_TradeC0401_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Info;
            this.panel3.Controls.Add(this.button5);
            this.panel3.Controls.Add(this.bt_CreateXML);
            this.panel3.Controls.Add(this.textBox3);
            this.panel3.Controls.Add(this.textBox2);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.TB_AppId);
            this.panel3.Controls.Add(this.bt_ExecutePRM);
            this.panel3.Controls.Add(this.TB_DB_ConnectionString_Trade);
            this.panel3.Controls.Add(this.DGV_DB_DataSHow);
            this.panel3.Controls.Add(this.bt_GetDataSetByPRM);
            this.panel3.Controls.Add(this.TB_DB_ConnectionString_Query);
            this.panel3.Controls.Add(this.bt_GetDataSetBySP);
            this.panel3.Controls.Add(this.TB_ApiKey);
            this.panel3.Controls.Add(this.bt_ExecuteSP);
            this.panel3.Location = new System.Drawing.Point(16, 412);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1370, 255);
            this.panel3.TabIndex = 17;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1209, 16);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 50);
            this.button5.TabIndex = 19;
            this.button5.Text = "AES Decode";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // bt_CreateXML
            // 
            this.bt_CreateXML.Location = new System.Drawing.Point(997, 194);
            this.bt_CreateXML.Name = "bt_CreateXML";
            this.bt_CreateXML.Size = new System.Drawing.Size(150, 50);
            this.bt_CreateXML.TabIndex = 1;
            this.bt_CreateXML.Text = "CreateXML";
            this.bt_CreateXML.UseVisualStyleBackColor = true;
            this.bt_CreateXML.Click += new System.EventHandler(this.bt_CreateXML_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(997, 143);
            this.textBox3.MaxLength = 1024;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(362, 29);
            this.textBox3.TabIndex = 18;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(997, 108);
            this.textBox2.MaxLength = 32;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(362, 29);
            this.textBox2.TabIndex = 17;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(997, 72);
            this.textBox1.MaxLength = 1024;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(362, 29);
            this.textBox1.TabIndex = 16;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(997, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 50);
            this.button1.TabIndex = 1;
            this.button1.Text = "AES Encode";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "========Local用========",
            "http://localhost/EinvGW/EinvGW/GwSelfTest",
            "http://localhost/EinvTradeSrv/EinvTradeSrv/TradeSelfTest",
            "http://localhost/EinvQuerySrv/EinvQuerySrv/QuerySelfTest",
            "http://localhost/EinvGW/EinvGW/V1/GetClientBatchInvoice",
            "http://localhost/EinvGW/EinvGW/V1/PostEcho",
            "http://localhost/EinvGW/EinvGW/V1/TradeC0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeC0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeVoidCancel",
            "http://localhost/EinvGW/EinvGW/V1/TradeA0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeA0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeD0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeD0501",
            "http://localhost/EinvGW/EinvGW/V1/TradeB0401",
            "http://localhost/EinvGW/EinvGW/V1/TradeB0501",
            "http://localhost/EinvGW/EinvGW/V1/EncodeTest",
            "http://localhost/EinvGW/EinvGW/V1/DecodeTest",
            "========雲端Local用========",
            "http://127.0.0.1:34010/EinvGW/GwSelfTest",
            "http://127.0.0.1:34000/EinvTradeSrv/TradeSelfTest",
            "http://127.0.0.1:34001/EinvQuerySrv/QuerySelfTest",
            "========雲端 Query========",
            "http://61.218.22.135:34010/EinvGW/GwSelfTest",
            "http://61.218.22.135:34010/EinvGW/V1/EncodeTest",
            "http://61.218.22.135:34010/EinvGW/V1/DecodeTest",
            "========雲端 Trade=======",
            "http://61.218.22.135:34010/EinvGW/V1/PostEcho",
            "http://61.218.22.135:34010/EinvGW/V1/GetClientBatchInvoice",
            "http://61.218.22.135:34010/EinvGW/V1/TradeC0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeC0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeVoidCancel",
            "http://61.218.22.135:34010/EinvGW/V1/TradeD0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeD0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeA0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeA0501",
            "http://61.218.22.135:34010/EinvGW/V1/TradeB0401",
            "http://61.218.22.135:34010/EinvGW/V1/TradeB0501"});
            this.comboBox1.Location = new System.Drawing.Point(80, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(1309, 28);
            this.comboBox1.TabIndex = 18;
            // 
            // Post_Timer
            // 
            this.Post_Timer.Interval = 1000;
            this.Post_Timer.Tick += new System.EventHandler(this.Post_Timer_Tick);
            // 
            // MainFM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1401, 999);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SysLBox);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainFM";
            this.Text = "eInv Test Tool Ver : ";
            this.Load += new System.EventHandler(this.MainFM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_DB_DataSHow)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btPost;
        private System.Windows.Forms.ListBox SysLBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_ExecutePRM;
        private System.Windows.Forms.Button bt_GetDataSetByPRM;
        private System.Windows.Forms.TextBox TB_DB_ConnectionString_Query;
        private System.Windows.Forms.DataGridView DGV_DB_DataSHow;
        private System.Windows.Forms.TextBox TB_AppId;
        private System.Windows.Forms.TextBox TB_ApiKey;
        private System.Windows.Forms.Button bt_ExecuteSP;
        private System.Windows.Forms.Button bt_GetDataSetBySP;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox TB_DB_ConnectionString_Trade;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button bt_DecodeTest;
        private System.Windows.Forms.Button bt_EncodeTest;
        private System.Windows.Forms.TextBox tb_ApiDataTest;
        private System.Windows.Forms.TextBox tb_TestApiKey;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bt_CreateXML;
        private System.Windows.Forms.Button bt_TradeC0401;
        private System.Windows.Forms.TextBox tb_InvoiceNO;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button bt_TradeC0501;
        private System.Windows.Forms.TextBox tb_allowanceNO;
        private System.Windows.Forms.Button bt_TradeD0401;
        private System.Windows.Forms.Button bt_TradeD0501;
        private System.Windows.Forms.Button bt_TradeVoidCancel;
        private System.Windows.Forms.Button bt_TradeA0401;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button bt_TradeA0501;
        private System.Windows.Forms.Button bt_B0501;
        private System.Windows.Forms.Button bt_B0401;
        private System.Windows.Forms.ComboBox CB_Batch;
        private System.Windows.Forms.TextBox TB_Term;
        private System.Windows.Forms.Button bt_GetClientBatchInvoice;
        private System.Windows.Forms.ComboBox CB_ECwebUrl;
        private System.Windows.Forms.TextBox tb_Sec;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer Post_Timer;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

