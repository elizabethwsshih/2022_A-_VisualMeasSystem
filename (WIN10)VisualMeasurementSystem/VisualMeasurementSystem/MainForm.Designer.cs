namespace VisualMeasurementSystem
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.CCDView1 = new System.Windows.Forms.PictureBox();
            this.MeasView = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AreaComboBox = new System.Windows.Forms.ComboBox();
            this.DefectInfoDataGrid = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutoMeasBtn = new System.Windows.Forms.Button();
            this.CCDNameLbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.XCurPosTxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.YCurPosTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.D2460Btn = new System.Windows.Forms.Button();
            this.M1027Btn = new System.Windows.Forms.Button();
            this.M1026Btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TtlTimeLbl = new System.Windows.Forms.Label();
            this.ManualNoStopRadioBtn = new System.Windows.Forms.RadioButton();
            this.AutoGoHomeBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ManualStepDistComboBox = new System.Windows.Forms.ComboBox();
            this.label71 = new System.Windows.Forms.Label();
            this.ScoreChkBox1 = new System.Windows.Forms.CheckBox();
            this.OnlyMeaBtn = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.BmpSaveBtn = new System.Windows.Forms.Button();
            this.ManualXDecBtn = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.ManualStepRadioBox = new System.Windows.Forms.RadioButton();
            this.ManualYIncBtn = new System.Windows.Forms.Button();
            this.ManualXIncBtn = new System.Windows.Forms.Button();
            this.ManualYDecBtn = new System.Windows.Forms.Button();
            this.ManualAsnPosGrpBox = new System.Windows.Forms.GroupBox();
            this.ManualInputBtn = new System.Windows.Forms.Button();
            this.ManualAsnPosMoveBtn = new System.Windows.Forms.Button();
            this.label98 = new System.Windows.Forms.Label();
            this.label97 = new System.Windows.Forms.Label();
            this.ManualYPosTxtBox = new System.Windows.Forms.TextBox();
            this.ManualXPosTxtBox = new System.Windows.Forms.TextBox();
            this.ManualAsnPosRadioBtn = new System.Windows.Forms.RadioButton();
            this.MeasLbl = new System.Windows.Forms.Label();
            this.CCDLbl = new System.Windows.Forms.Label();
            this.PerformanceBtn = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CCDView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeasView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DefectInfoDataGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.ManualAsnPosGrpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // CCDView1
            // 
            this.CCDView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CCDView1.Location = new System.Drawing.Point(5, 30);
            this.CCDView1.Name = "CCDView1";
            this.CCDView1.Size = new System.Drawing.Size(371, 378);
            this.CCDView1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CCDView1.TabIndex = 0;
            this.CCDView1.TabStop = false;
            // 
            // MeasView
            // 
            this.MeasView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MeasView.Location = new System.Drawing.Point(382, 30);
            this.MeasView.Name = "MeasView";
            this.MeasView.Size = new System.Drawing.Size(354, 378);
            this.MeasView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MeasView.TabIndex = 1;
            this.MeasView.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.AreaComboBox);
            this.groupBox1.Controls.Add(this.DefectInfoDataGrid);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(11, 439);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(790, 226);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " 缺陷銲道資訊";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(21, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "選擇區域編號 : ";
            // 
            // AreaComboBox
            // 
            this.AreaComboBox.FormattingEnabled = true;
            this.AreaComboBox.Location = new System.Drawing.Point(131, 24);
            this.AreaComboBox.Name = "AreaComboBox";
            this.AreaComboBox.Size = new System.Drawing.Size(112, 25);
            this.AreaComboBox.TabIndex = 6;
            this.AreaComboBox.Text = "ALL";
            this.AreaComboBox.SelectedIndexChanged += new System.EventHandler(this.AreaComboBox_SelectedIndexChanged);
            // 
            // DefectInfoDataGrid
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.DefectInfoDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DefectInfoDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DefectInfoDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.DefectInfoDataGrid.Location = new System.Drawing.Point(10, 55);
            this.DefectInfoDataGrid.Name = "DefectInfoDataGrid";
            this.DefectInfoDataGrid.RowTemplate.Height = 24;
            this.DefectInfoDataGrid.Size = new System.Drawing.Size(778, 165);
            this.DefectInfoDataGrid.TabIndex = 5;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "模組編號";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "正反面";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "區域編號";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "銲道編號";
            this.Column4.Name = "Column4";
            // 
            // AutoMeasBtn
            // 
            this.AutoMeasBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.AutoMeasBtn.Location = new System.Drawing.Point(278, 10);
            this.AutoMeasBtn.Name = "AutoMeasBtn";
            this.AutoMeasBtn.Size = new System.Drawing.Size(75, 43);
            this.AutoMeasBtn.TabIndex = 167;
            this.AutoMeasBtn.Text = "自動檢測";
            this.AutoMeasBtn.UseVisualStyleBackColor = false;
            this.AutoMeasBtn.Visible = false;
            this.AutoMeasBtn.Click += new System.EventHandler(this.AutoMeasBtn_Click);
            // 
            // CCDNameLbl
            // 
            this.CCDNameLbl.AutoSize = true;
            this.CCDNameLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CCDNameLbl.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CCDNameLbl.ForeColor = System.Drawing.Color.Aqua;
            this.CCDNameLbl.Location = new System.Drawing.Point(7, 388);
            this.CCDNameLbl.Name = "CCDNameLbl";
            this.CCDNameLbl.Size = new System.Drawing.Size(95, 20);
            this.CCDNameLbl.TabIndex = 4;
            this.CCDNameLbl.Text = "CCD 名稱:   ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(1, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "模組編號/正反面 : ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.ManualAsnPosGrpBox);
            this.groupBox2.Location = new System.Drawing.Point(801, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 633);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.XCurPosTxtBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.YCurPosTxtBox);
            this.groupBox4.Location = new System.Drawing.Point(4, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(356, 59);
            this.groupBox4.TabIndex = 171;
            this.groupBox4.TabStop = false;
            // 
            // XCurPosTxtBox
            // 
            this.XCurPosTxtBox.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.XCurPosTxtBox.Location = new System.Drawing.Point(30, 13);
            this.XCurPosTxtBox.Name = "XCurPosTxtBox";
            this.XCurPosTxtBox.Size = new System.Drawing.Size(77, 33);
            this.XCurPosTxtBox.TabIndex = 6;
            this.XCurPosTxtBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(2, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 24);
            this.label5.TabIndex = 3;
            this.label5.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(137, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 24);
            this.label6.TabIndex = 4;
            this.label6.Text = "Y";
            // 
            // YCurPosTxtBox
            // 
            this.YCurPosTxtBox.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.YCurPosTxtBox.Location = new System.Drawing.Point(164, 13);
            this.YCurPosTxtBox.Name = "YCurPosTxtBox";
            this.YCurPosTxtBox.Size = new System.Drawing.Size(80, 33);
            this.YCurPosTxtBox.TabIndex = 7;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.D2460Btn);
            this.groupBox3.Controls.Add(this.M1027Btn);
            this.groupBox3.Controls.Add(this.M1026Btn);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.TtlTimeLbl);
            this.groupBox3.Controls.Add(this.ManualNoStopRadioBtn);
            this.groupBox3.Controls.Add(this.AutoGoHomeBtn);
            this.groupBox3.Controls.Add(this.AutoMeasBtn);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.ManualStepDistComboBox);
            this.groupBox3.Controls.Add(this.label71);
            this.groupBox3.Controls.Add(this.ScoreChkBox1);
            this.groupBox3.Controls.Add(this.OnlyMeaBtn);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.BmpSaveBtn);
            this.groupBox3.Controls.Add(this.ManualXDecBtn);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.ManualStepRadioBox);
            this.groupBox3.Controls.Add(this.ManualYIncBtn);
            this.groupBox3.Controls.Add(this.ManualXIncBtn);
            this.groupBox3.Controls.Add(this.ManualYDecBtn);
            this.groupBox3.Location = new System.Drawing.Point(4, 82);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(356, 387);
            this.groupBox3.TabIndex = 164;
            this.groupBox3.TabStop = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button3.Location = new System.Drawing.Point(275, 154);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 175;
            this.button3.Text = "D16 write";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // D2460Btn
            // 
            this.D2460Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.D2460Btn.Location = new System.Drawing.Point(278, 120);
            this.D2460Btn.Name = "D2460Btn";
            this.D2460Btn.Size = new System.Drawing.Size(75, 30);
            this.D2460Btn.TabIndex = 174;
            this.D2460Btn.Text = "D2460";
            this.D2460Btn.UseVisualStyleBackColor = false;
            this.D2460Btn.Click += new System.EventHandler(this.D2460Btn_Click);
            // 
            // M1027Btn
            // 
            this.M1027Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.M1027Btn.Location = new System.Drawing.Point(278, 86);
            this.M1027Btn.Name = "M1027Btn";
            this.M1027Btn.Size = new System.Drawing.Size(75, 30);
            this.M1027Btn.TabIndex = 173;
            this.M1027Btn.Text = "M1027";
            this.M1027Btn.UseVisualStyleBackColor = false;
            this.M1027Btn.Click += new System.EventHandler(this.M1027Btn_Click);
            // 
            // M1026Btn
            // 
            this.M1026Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.M1026Btn.Location = new System.Drawing.Point(278, 54);
            this.M1026Btn.Name = "M1026Btn";
            this.M1026Btn.Size = new System.Drawing.Size(75, 30);
            this.M1026Btn.TabIndex = 172;
            this.M1026Btn.Text = "M1026";
            this.M1026Btn.UseVisualStyleBackColor = false;
            this.M1026Btn.Click += new System.EventHandler(this.M1026Btn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(215, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 166;
            this.label2.Text = "Y向負";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // TtlTimeLbl
            // 
            this.TtlTimeLbl.AutoSize = true;
            this.TtlTimeLbl.BackColor = System.Drawing.Color.DimGray;
            this.TtlTimeLbl.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TtlTimeLbl.ForeColor = System.Drawing.Color.Aqua;
            this.TtlTimeLbl.Location = new System.Drawing.Point(332, 283);
            this.TtlTimeLbl.Name = "TtlTimeLbl";
            this.TtlTimeLbl.Size = new System.Drawing.Size(13, 20);
            this.TtlTimeLbl.TabIndex = 170;
            this.TtlTimeLbl.Text = " ";
            this.TtlTimeLbl.Visible = false;
            // 
            // ManualNoStopRadioBtn
            // 
            this.ManualNoStopRadioBtn.AutoSize = true;
            this.ManualNoStopRadioBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualNoStopRadioBtn.Location = new System.Drawing.Point(16, 17);
            this.ManualNoStopRadioBtn.Name = "ManualNoStopRadioBtn";
            this.ManualNoStopRadioBtn.Size = new System.Drawing.Size(91, 24);
            this.ManualNoStopRadioBtn.TabIndex = 171;
            this.ManualNoStopRadioBtn.Text = "連續移動";
            this.ManualNoStopRadioBtn.UseVisualStyleBackColor = true;
            this.ManualNoStopRadioBtn.Click += new System.EventHandler(this.ManualNoStopRadioBtn_Click);
            // 
            // AutoGoHomeBtn
            // 
            this.AutoGoHomeBtn.BackColor = System.Drawing.Color.Green;
            this.AutoGoHomeBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.AutoGoHomeBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.AutoGoHomeBtn.Location = new System.Drawing.Point(121, 192);
            this.AutoGoHomeBtn.Name = "AutoGoHomeBtn";
            this.AutoGoHomeBtn.Size = new System.Drawing.Size(77, 78);
            this.AutoGoHomeBtn.TabIndex = 162;
            this.AutoGoHomeBtn.Text = "HOME";
            this.AutoGoHomeBtn.UseVisualStyleBackColor = false;
            this.AutoGoHomeBtn.Click += new System.EventHandler(this.AutoGoHomeBtn_Click);
            this.AutoGoHomeBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AutoGoHomeBtn_MouseDown);
            this.AutoGoHomeBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AutoGoHomeBtn_MouseUp);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(301, 340);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 169;
            this.button2.Text = "拍攝";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // ManualStepDistComboBox
            // 
            this.ManualStepDistComboBox.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualStepDistComboBox.FormattingEnabled = true;
            this.ManualStepDistComboBox.Items.AddRange(new object[] {
            "0.001",
            "0.002",
            "0.005",
            "0.01",
            "0.02",
            "0.05",
            "0.1",
            "0.2",
            "0.5",
            "1",
            "2",
            "5",
            "10"});
            this.ManualStepDistComboBox.Location = new System.Drawing.Point(145, 46);
            this.ManualStepDistComboBox.Name = "ManualStepDistComboBox";
            this.ManualStepDistComboBox.Size = new System.Drawing.Size(80, 28);
            this.ManualStepDistComboBox.TabIndex = 170;
            this.ManualStepDistComboBox.Text = "0.005";
            this.ManualStepDistComboBox.SelectedIndexChanged += new System.EventHandler(this.ManualStepDistComboBox_SelectedIndexChanged);
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.BackColor = System.Drawing.Color.Transparent;
            this.label71.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label71.Location = new System.Drawing.Point(56, 169);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(51, 20);
            this.label71.TabIndex = 154;
            this.label71.Text = "Y向正";
            // 
            // ScoreChkBox1
            // 
            this.ScoreChkBox1.AutoSize = true;
            this.ScoreChkBox1.BackColor = System.Drawing.Color.DimGray;
            this.ScoreChkBox1.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ScoreChkBox1.ForeColor = System.Drawing.Color.Aqua;
            this.ScoreChkBox1.Location = new System.Drawing.Point(270, 284);
            this.ScoreChkBox1.Name = "ScoreChkBox1";
            this.ScoreChkBox1.Size = new System.Drawing.Size(60, 24);
            this.ScoreChkBox1.TabIndex = 166;
            this.ScoreChkBox1.Text = "原圖";
            this.ScoreChkBox1.UseVisualStyleBackColor = false;
            this.ScoreChkBox1.Visible = false;
            this.ScoreChkBox1.CheckedChanged += new System.EventHandler(this.ScoreChkBox1_CheckedChanged);
            // 
            // OnlyMeaBtn
            // 
            this.OnlyMeaBtn.Location = new System.Drawing.Point(227, 340);
            this.OnlyMeaBtn.Name = "OnlyMeaBtn";
            this.OnlyMeaBtn.Size = new System.Drawing.Size(80, 23);
            this.OnlyMeaBtn.TabIndex = 5;
            this.OnlyMeaBtn.Text = "單區檢測";
            this.OnlyMeaBtn.UseVisualStyleBackColor = true;
            this.OnlyMeaBtn.Visible = false;
            this.OnlyMeaBtn.Click += new System.EventHandler(this.OnlyMeaBtn_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label19.Location = new System.Drawing.Point(133, 359);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(51, 20);
            this.label19.TabIndex = 149;
            this.label19.Text = "X向負";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(231, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "開啟舊檔";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label18.Location = new System.Drawing.Point(227, 54);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 20);
            this.label18.TabIndex = 169;
            this.label18.Text = "mm";
            // 
            // BmpSaveBtn
            // 
            this.BmpSaveBtn.Location = new System.Drawing.Point(301, 313);
            this.BmpSaveBtn.Name = "BmpSaveBtn";
            this.BmpSaveBtn.Size = new System.Drawing.Size(48, 23);
            this.BmpSaveBtn.TabIndex = 168;
            this.BmpSaveBtn.Text = "儲存";
            this.BmpSaveBtn.UseVisualStyleBackColor = true;
            this.BmpSaveBtn.Visible = false;
            this.BmpSaveBtn.Click += new System.EventHandler(this.button3_Click);
            // 
            // ManualXDecBtn
            // 
            this.ManualXDecBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ManualXDecBtn.BackgroundImage")));
            this.ManualXDecBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ManualXDecBtn.Location = new System.Drawing.Point(38, 190);
            this.ManualXDecBtn.Name = "ManualXDecBtn";
            this.ManualXDecBtn.Size = new System.Drawing.Size(77, 80);
            this.ManualXDecBtn.TabIndex = 17;
            this.ManualXDecBtn.UseVisualStyleBackColor = true;
            this.ManualXDecBtn.Click += new System.EventHandler(this.ManualXDecBtn_Click);
            this.ManualXDecBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManualXDecBtn_MouseDown);
            this.ManualXDecBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManualXDecBtn_MouseUp);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label23.Location = new System.Drawing.Point(134, 89);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(51, 20);
            this.label23.TabIndex = 150;
            this.label23.Text = "X向正";
            // 
            // ManualStepRadioBox
            // 
            this.ManualStepRadioBox.AutoSize = true;
            this.ManualStepRadioBox.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualStepRadioBox.Location = new System.Drawing.Point(16, 50);
            this.ManualStepRadioBox.Name = "ManualStepRadioBox";
            this.ManualStepRadioBox.Size = new System.Drawing.Size(123, 24);
            this.ManualStepRadioBox.TabIndex = 168;
            this.ManualStepRadioBox.Text = "步進吋動距離";
            this.ManualStepRadioBox.UseVisualStyleBackColor = true;
            this.ManualStepRadioBox.Click += new System.EventHandler(this.ManualStepRadioBox_Click);
            // 
            // ManualYIncBtn
            // 
            this.ManualYIncBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ManualYIncBtn.BackgroundImage")));
            this.ManualYIncBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ManualYIncBtn.Location = new System.Drawing.Point(121, 109);
            this.ManualYIncBtn.Name = "ManualYIncBtn";
            this.ManualYIncBtn.Size = new System.Drawing.Size(77, 80);
            this.ManualYIncBtn.TabIndex = 148;
            this.ManualYIncBtn.UseVisualStyleBackColor = true;
            this.ManualYIncBtn.Click += new System.EventHandler(this.ManualZIncBtn_Click);
            this.ManualYIncBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManualYIncBtn_MouseDown);
            this.ManualYIncBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManualYIncBtn_MouseUp);
            // 
            // ManualXIncBtn
            // 
            this.ManualXIncBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ManualXIncBtn.BackgroundImage")));
            this.ManualXIncBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ManualXIncBtn.Location = new System.Drawing.Point(204, 190);
            this.ManualXIncBtn.Name = "ManualXIncBtn";
            this.ManualXIncBtn.Size = new System.Drawing.Size(77, 80);
            this.ManualXIncBtn.TabIndex = 16;
            this.ManualXIncBtn.UseVisualStyleBackColor = true;
            this.ManualXIncBtn.Click += new System.EventHandler(this.ManualXIncBtn_Click);
            this.ManualXIncBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ManualXIncBtn_MouseClick);
            this.ManualXIncBtn.MouseCaptureChanged += new System.EventHandler(this.ManualXIncBtn_MouseCaptureChanged);
            this.ManualXIncBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManualXIncBtn_MouseDown);
            this.ManualXIncBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManualXIncBtn_MouseUp);
            // 
            // ManualYDecBtn
            // 
            this.ManualYDecBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ManualYDecBtn.BackgroundImage")));
            this.ManualYDecBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ManualYDecBtn.Location = new System.Drawing.Point(121, 276);
            this.ManualYDecBtn.Name = "ManualYDecBtn";
            this.ManualYDecBtn.Size = new System.Drawing.Size(77, 80);
            this.ManualYDecBtn.TabIndex = 147;
            this.ManualYDecBtn.UseVisualStyleBackColor = true;
            this.ManualYDecBtn.Click += new System.EventHandler(this.ManualZDecBtn_Click);
            this.ManualYDecBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManualYDecBtn_MouseDown);
            this.ManualYDecBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManualYDecBtn_MouseUp);
            // 
            // ManualAsnPosGrpBox
            // 
            this.ManualAsnPosGrpBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ManualAsnPosGrpBox.Controls.Add(this.ManualInputBtn);
            this.ManualAsnPosGrpBox.Controls.Add(this.ManualAsnPosMoveBtn);
            this.ManualAsnPosGrpBox.Controls.Add(this.label98);
            this.ManualAsnPosGrpBox.Controls.Add(this.label97);
            this.ManualAsnPosGrpBox.Controls.Add(this.ManualYPosTxtBox);
            this.ManualAsnPosGrpBox.Controls.Add(this.ManualXPosTxtBox);
            this.ManualAsnPosGrpBox.Controls.Add(this.ManualAsnPosRadioBtn);
            this.ManualAsnPosGrpBox.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualAsnPosGrpBox.Location = new System.Drawing.Point(6, 475);
            this.ManualAsnPosGrpBox.Name = "ManualAsnPosGrpBox";
            this.ManualAsnPosGrpBox.Size = new System.Drawing.Size(351, 158);
            this.ManualAsnPosGrpBox.TabIndex = 165;
            this.ManualAsnPosGrpBox.TabStop = false;
            // 
            // ManualInputBtn
            // 
            this.ManualInputBtn.BackColor = System.Drawing.Color.LightSkyBlue;
            this.ManualInputBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualInputBtn.Location = new System.Drawing.Point(190, 34);
            this.ManualInputBtn.Name = "ManualInputBtn";
            this.ManualInputBtn.Size = new System.Drawing.Size(89, 39);
            this.ManualInputBtn.TabIndex = 140;
            this.ManualInputBtn.Text = "入料位置";
            this.ManualInputBtn.UseVisualStyleBackColor = false;
            this.ManualInputBtn.Click += new System.EventHandler(this.ManualInputBtn_Click);
            // 
            // ManualAsnPosMoveBtn
            // 
            this.ManualAsnPosMoveBtn.BackColor = System.Drawing.Color.SteelBlue;
            this.ManualAsnPosMoveBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualAsnPosMoveBtn.Location = new System.Drawing.Point(190, 75);
            this.ManualAsnPosMoveBtn.Name = "ManualAsnPosMoveBtn";
            this.ManualAsnPosMoveBtn.Size = new System.Drawing.Size(89, 39);
            this.ManualAsnPosMoveBtn.TabIndex = 139;
            this.ManualAsnPosMoveBtn.Text = "移動";
            this.ManualAsnPosMoveBtn.UseVisualStyleBackColor = false;
            this.ManualAsnPosMoveBtn.Click += new System.EventHandler(this.ManualAsnPosMoveBtn_Click);
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label98.Location = new System.Drawing.Point(13, 78);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(43, 20);
            this.label98.TabIndex = 25;
            this.label98.Text = "Y軸 :";
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label97.Location = new System.Drawing.Point(13, 43);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(43, 20);
            this.label97.TabIndex = 24;
            this.label97.Text = "X軸 :";
            // 
            // ManualYPosTxtBox
            // 
            this.ManualYPosTxtBox.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualYPosTxtBox.Location = new System.Drawing.Point(62, 75);
            this.ManualYPosTxtBox.Name = "ManualYPosTxtBox";
            this.ManualYPosTxtBox.Size = new System.Drawing.Size(121, 29);
            this.ManualYPosTxtBox.TabIndex = 23;
            this.ManualYPosTxtBox.Text = "271";
            // 
            // ManualXPosTxtBox
            // 
            this.ManualXPosTxtBox.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualXPosTxtBox.Location = new System.Drawing.Point(61, 40);
            this.ManualXPosTxtBox.Name = "ManualXPosTxtBox";
            this.ManualXPosTxtBox.Size = new System.Drawing.Size(121, 29);
            this.ManualXPosTxtBox.TabIndex = 22;
            this.ManualXPosTxtBox.Text = "90";
            // 
            // ManualAsnPosRadioBtn
            // 
            this.ManualAsnPosRadioBtn.AutoSize = true;
            this.ManualAsnPosRadioBtn.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ManualAsnPosRadioBtn.Location = new System.Drawing.Point(10, 16);
            this.ManualAsnPosRadioBtn.Name = "ManualAsnPosRadioBtn";
            this.ManualAsnPosRadioBtn.Size = new System.Drawing.Size(91, 24);
            this.ManualAsnPosRadioBtn.TabIndex = 21;
            this.ManualAsnPosRadioBtn.Text = "指定座標";
            this.ManualAsnPosRadioBtn.UseVisualStyleBackColor = true;
            this.ManualAsnPosRadioBtn.Click += new System.EventHandler(this.ManualAsnPosRadioBtn_Click);
            // 
            // MeasLbl
            // 
            this.MeasLbl.AutoSize = true;
            this.MeasLbl.BackColor = System.Drawing.Color.DimGray;
            this.MeasLbl.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MeasLbl.ForeColor = System.Drawing.Color.Aqua;
            this.MeasLbl.Location = new System.Drawing.Point(386, 32);
            this.MeasLbl.Name = "MeasLbl";
            this.MeasLbl.Size = new System.Drawing.Size(49, 20);
            this.MeasLbl.TabIndex = 5;
            this.MeasLbl.Text = "檢測: ";
            // 
            // CCDLbl
            // 
            this.CCDLbl.AutoSize = true;
            this.CCDLbl.BackColor = System.Drawing.Color.DimGray;
            this.CCDLbl.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CCDLbl.ForeColor = System.Drawing.Color.Aqua;
            this.CCDLbl.Location = new System.Drawing.Point(9, 32);
            this.CCDLbl.Name = "CCDLbl";
            this.CCDLbl.Size = new System.Drawing.Size(45, 20);
            this.CCDLbl.TabIndex = 6;
            this.CCDLbl.Text = "取像:";
            // 
            // PerformanceBtn
            // 
            this.PerformanceBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.PerformanceBtn.Location = new System.Drawing.Point(382, 410);
            this.PerformanceBtn.Name = "PerformanceBtn";
            this.PerformanceBtn.Size = new System.Drawing.Size(96, 33);
            this.PerformanceBtn.TabIndex = 171;
            this.PerformanceBtn.Text = "Run";
            this.PerformanceBtn.UseVisualStyleBackColor = false;
            this.PerformanceBtn.Click += new System.EventHandler(this.PerformanceBtn_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button4.Location = new System.Drawing.Point(16, 284);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 176;
            this.button4.Text = "D16 READ";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 671);
            this.Controls.Add(this.MeasLbl);
            this.Controls.Add(this.PerformanceBtn);
            this.Controls.Add(this.CCDLbl);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CCDNameLbl);
            this.Controls.Add(this.CCDView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MeasView);
            this.Name = "MainForm";
            this.Text = "20220127_A+MainForm";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CCDView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeasView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DefectInfoDataGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ManualAsnPosGrpBox.ResumeLayout(false);
            this.ManualAsnPosGrpBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox CCDView1;
        private System.Windows.Forms.PictureBox MeasView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox YCurPosTxtBox;
        private System.Windows.Forms.TextBox XCurPosTxtBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label CCDNameLbl;
        private System.Windows.Forms.Label MeasLbl;
        private System.Windows.Forms.Label CCDLbl;
        private System.Windows.Forms.GroupBox ManualAsnPosGrpBox;
        private System.Windows.Forms.Button ManualAsnPosMoveBtn;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.TextBox ManualYPosTxtBox;
        private System.Windows.Forms.TextBox ManualXPosTxtBox;
        private System.Windows.Forms.RadioButton ManualAsnPosRadioBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton ManualNoStopRadioBtn;
        private System.Windows.Forms.Button AutoGoHomeBtn;
        private System.Windows.Forms.ComboBox ManualStepDistComboBox;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button ManualXDecBtn;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button ManualXIncBtn;
        private System.Windows.Forms.Button OnlyMeaBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox ScoreChkBox1;
        private System.Windows.Forms.Button AutoMeasBtn;
        private System.Windows.Forms.DataGridView DefectInfoDataGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AreaComboBox;
        private System.Windows.Forms.Button ManualYIncBtn;
        private System.Windows.Forms.Button ManualYDecBtn;
        private System.Windows.Forms.Button BmpSaveBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label TtlTimeLbl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton ManualStepRadioBox;
        private System.Windows.Forms.Button PerformanceBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Button ManualInputBtn;
        private System.Windows.Forms.Button D2460Btn;
        private System.Windows.Forms.Button M1027Btn;
        private System.Windows.Forms.Button M1026Btn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

