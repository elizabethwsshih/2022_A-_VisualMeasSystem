using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

using Basler.Pylon;



namespace VisualMeasurementSystem
{
    public partial class MainForm : Form
    {
        //----------------------Pylon CCD -------------------------------------------

        CCDForm BaslerCCD = new CCDForm();
        public System.Windows.Forms.Timer tmrUiUpdate = null;//連續拍攝 的 timer
        int iTickNum = 0;//拍攝控制

        //CCD 連續拍攝記憶體保護寫法相關宣告
        private Bitmap shotBitmap1 = null;
        private Bitmap ProcessMeasBmp1 = null;
        public object imageLock = new object();
        public EventHandler imageHandler;


        Bitmap ProcessBmp, DrawBmp, DrawScoreBmp; //顯示於 pictuebox的共同 bimtap:  viewbmp:顯示連續攝影 /processbmp: 影像處理後之影像


        //---------------------- PLC Control -------------------------------------------
        PLCAction PLCAction = new PLCAction();
        double XCurPos, YCurPos, ZCurPos;
        int XGoHomeStatus = -1, YGoHomeStatus = -1, ZGoHomeStatus = -1;
        int GoHomeFlag = 0; //程式開啟時gohonme只做一次的旗標
        System.Threading.Timer PLCTimer;

        //(2) 手動控制 PLC
        int ManualMode = 0;
        Thread ManualMarkThread;

        //Battery Info
        string ModelID = "";
        string AB = "";

        //----------------------image process----------------------
        ImageProcessing IP = new ImageProcessing();
        BitmapDataFast BMPFast = new BitmapDataFast();
        int[, ,] OriBmpData;
        Bitmap FileBMP;
        List<System.Drawing.Point> TemplateEdgeList;
        // Bitmap ShotBMP; // 取像
        //--------------------Grid------------
        WeldCenterInfo[,] FinalWeldGrid;
        List<PointF> GoodWeldP;
        PointF[,] MapGrid;
        int GridNum = 6;
        //int IniGridLength = 260; //1F

        int IniGridLength = 240; //2F

        //-------------執行緒相關宣告------------
        Thread AutoShotThread; //取像 thread
        Thread FileProcessThread; //檔案處理 thread
        Thread ImageProcessThread; //影像處理 thread

        //-----------影像處理前讀檔宣告-------------
        string SaveFilePath = "D:\\A60496\\取像\\";
        FileIO FileIO = new FileIO();
        List<string> ShotFileName = new List<string>(); //本run取像存檔之list

        //----------Defect Info------------------
        List<List<DefectP>> DefectInfoList = new List<List<DefectP>>();
        int SelectIdx = 1;
        //-----------生產與時間資訊-------------
        double ShotTime, MeasTime, TotalCycleTime;
        System.Diagnostics.Stopwatch TotalClock;


        //WeldCenterInfo[,] FinalWeldGrid;

        public MainForm()
        {
            InitializeComponent();
            //Pylon.Initialize();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Get the first selected item.
            CCDView1.Image = new Bitmap(10, 10);

            //BaslerCCD.UpdateDeviceList();
            //ListViewItem item = deviceListView.Items[0];
            ////// Get the attached device data.

            //selectedCamera = item.Tag as ICameraInfo;

            BaslerCCD.OpenCamera();

            //連續攝影事件
            BaslerCCD.ContinuousShot();

            tmrUiUpdate = new System.Windows.Forms.Timer();
            tmrUiUpdate.Enabled = false;
            tmrUiUpdate.Interval = 500;
            this.tmrUiUpdate.Tick += new System.EventHandler(this.tmrUiUpdate_Tick);
            tmrUiUpdate.Enabled = true;



            //---------------------------------PLC ON ----------------------
            //1. 通訊
            PLCAction.PLC_Connect(0);

            //2.主控權PLC FOR DEBUG
            PLCAction.MainControl(0);//1:pc,0:plc


            //20180920 蕭兄建議開啟後通通初始化
            PLCAction.PLC_AllInitial();

            //2.主控權PLC FOR DEBUG
            PLCAction.MainControl(1);//1:pc,0:plc

            PLCAction.PLC_StageMotor(1);


            TimerCallback callback = new TimerCallback(_do);
            PLCTimer = new System.Threading.Timer(callback, null, 0, 500);//500豪秒起來一次

            ////ManualAsnPosMoveBtn.Enabled = false;

            ////purge file
            //FileIO.DeleteFile(SaveFilePath, "*.bmp", 5);

            //--------------------------------UI-------------
            AreaComboBox.Items.Add("ALL");
            for (int i = 0; i < 6; i++)
                AreaComboBox.Items.Add((i + 1).ToString());

        }
        private void _do(object state)
        {

            this.BeginInvoke(new setlabel2(setlabel3));//委派

        }
        delegate void setlabel2();
        private void setlabel3()
        {

            //每秒更新一次
            //CurTimeLbl.Text = DateTime.Now.ToString("HH:mm:ss");


            // ReadPLCDataRandom 一次呼叫完畢,節省時間
            //double[] _ReadVal = PLCAction.ReadPLCDataRandom("D1000\nD1001\nD1010\nD1011\nR1000\nR1001\nR1010\nR1011\nD1006\nD1007" //1-2-3-4-5
            //                                           + "\nD1016\nD1017\nR1002\nR1003\nR1012\nR1013\nM1107\nM1117\nM1102\nM1105" //6-7-8--9-10-11-12
            //                                           + "\nM1112\nM1115\nM1209\nM1600\nM1601\nM1602\nM1603\nM1604\nM1605\nM1606" //13-14-15-16-17-18-19-20-21-22
            //                                           + "\nM1607\nM1608\nM1609\nM1610\nM1611\nM1602\nM1002\nM1012\nD1020\nD1021"// 23-24-25-26-27-28-29-30--31
            //                                           + "\nR1020\nR1021\nD1026\nD1027\nR1022\nR1023\nM1122\nM1125"//32-33-34--35-36
            //                                           + "\nM1613\nM1614\nM1615\nM1107\nM1117\nM1127\nM1616", 55);//37-38-39-40-41-42-43


            //A+ 2022 D碼
            double[] _ReadVal = PLCAction.ReadPLCDataRandom("D1064\nD1065\nD1068\nD1069\nD1060\nD1061\n"//1-2-3  (6)--MainForm & StageForm--
                                                       + "M1107\nM1117\nM1127\n"//4-5-6 --MainForm & StageForm--(9)
                                                       + "M1017\nM1215\nM1018\nM1156\n" //7-8-9-10 --Laser & Welding--(13)
                                                       + "M1138\nM1139\nM1241\nM1006", 17); //11-12-13-14-- Welding--(16)

            //// hardcode 上面順序比較節省尋找時間
            XCurPos = Math.Round(_ReadVal[0], 3); //D1000
            YCurPos = Math.Round(_ReadVal[1], 3); //D1010

            XCurPosTxtBox.Text = XCurPos.ToString();
            YCurPosTxtBox.Text = YCurPos.ToString();

            // GOHOME STATUS
            XGoHomeStatus = Convert.ToInt32(_ReadVal[3]); //M1107
            YGoHomeStatus = Convert.ToInt32(_ReadVal[4]); //M1117




        }
        private void DisableOtherBtn()
        {
            AutoGoHomeBtn.Enabled = false;
            AutoGoHomeBtn.BackColor = Color.Gray;

            ManualAsnPosMoveBtn.Enabled = false;
            ManualAsnPosMoveBtn.BackColor = Color.Gray;


            ManualYDecBtn.Enabled = false;
            ManualYDecBtn.BackColor = Color.Gray;
            ManualXDecBtn.Enabled = false;
            ManualXDecBtn.BackColor = Color.Gray;


            ManualYIncBtn.Enabled = false;
            ManualYIncBtn.BackColor = Color.Gray;
            ManualXIncBtn.Enabled = false;
            ManualXIncBtn.BackColor = Color.Gray;

        }

        public Bitmap ShotBMP
        {
            get
            {
                lock (imageLock)
                {
                    if (shotBitmap1 == null)
                    {
                        return new Bitmap(10, 10);
                    }
                    else
                    {
                        return (Bitmap)shotBitmap1.Clone();
                    }

                }
            }

            private set
            {
                lock (imageLock)
                {
                    Bitmap tmp = shotBitmap1;
                    shotBitmap1 = null;
                    shotBitmap1 = value;
                    if (tmp != null)
                    {
                        //shotBitmap1.Dispose();
                        tmp.Dispose();
                    }
                }
            }
        }
        public Bitmap ProcessMeasBmp
        {
            get
            {
                lock (imageLock)
                {
                    if (ProcessMeasBmp1 == null)
                    {
                        return new Bitmap(10, 10);
                    }
                    else
                    {
                        return (Bitmap)ProcessMeasBmp1.Clone();
                    }

                }
            }

            private set
            {
                lock (imageLock)
                {
                    Bitmap tmp = ProcessMeasBmp1;
                    ProcessMeasBmp1 = null;
                    ProcessMeasBmp1 = value;
                    if (tmp != null)
                    {
                        //shotBitmap1.Dispose();
                        tmp.Dispose();
                    }
                }
            }
        }
        private void tmrUiUpdate_Tick(object sender, EventArgs e)
        {
            switch (iTickNum)
            {
                case 0:////連續攝影
                    ShotBMP = BaslerCCD.ReturnBMP();


                    // Assign a temporary variable to dispose the bitmap after assigning the new bitmap to the display control.
                    if (CCDView1.Image != null)
                        CCDView1.Image.Dispose();
                    // Provide the display control with the new bitmap. This action automatically updates the display.
                    CCDView1.Image = (Bitmap)ShotBMP.Clone();
                    GC.Collect();
                    break;

                case 1:

                    break;


                default:
                    break;
            }
        }




        private void MainForm_Activated(object sender, EventArgs e)
        {
            tmrUiUpdate.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        private void ManualSet()
        {
            //(3000/100)mm/s = 30mm/s
            double _StepDistVal = 0;// Convert.ToDouble(ManualStepDistComboBox.Text);
            double _XSpeedVal = 3000;//Convert.ToDouble(ManualXSpeedTxtBox.Text);
            double _YSpeedVal = 3000;//Convert.ToDouble(ManualYSpeedTxtBox.Text);
            double _ZSpeedVal = 3000;//Convert.ToDouble(ManualZSpeedTxtBox.Text);
            double _SpeedRateVal = 100;//Convert.ToDouble(ManualSpeedRateLbl.Text);

            PLCAction.ManualSet(_StepDistVal, _XSpeedVal, _YSpeedVal, _ZSpeedVal, _SpeedRateVal);// for 吋動需要修改步階距離

        }
        private void ManualXIncBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManualXDecBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManualYDecBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManualYIncBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManualZIncBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManualZDecBtn_Click(object sender, EventArgs e)
        {

        }

        private void OnlyMeaBtn_Click(object sender, EventArgs e)
        {
            MeasLbl.Text = "取像:";
            CCDLbl.Text = "檢測:";
            //List<DefectP> DPList = new List<DefectP>();
            //Bitmap ProcessMeasBmp = ImageProcess(ShotBMP, out DPList, 0);
            //MeasView.Image = ProcessMeasBmp;

            //string[] row;
            //DefectInfoDataGrid.Rows.Clear();
            //foreach (DefectP DP in DPList)
            //{
            //    row = new string[] { "", "", "" };


            //    row = new string[] { DP.AreaIdx.ToString(),
            //                              "("+DP.PID.X+","+DP.PID.Y+")",  
            //                                DP.DefectScore.ToString()
            //                            };
            //    DefectInfoDataGrid.Rows.Add(row);
            //}



        }
        private WeldCenterInfo[,] FinePosition(Bitmap _ProcessBmp, out List<System.Drawing.Point> TempEdgeDraw)
        {
            //---------------------------1.設定 TEMPLATE------------------------
            Bitmap TemplateBmp = IP.TemplateSet(64);
            //讀取 template 黑框圓周點
            int[, ,] TemplateData = BMPFast.GetRGBData(TemplateBmp);

            TemplateEdgeList = new List<System.Drawing.Point>();
            System.Drawing.Point EdgeP;

            for (int j = 0; j < TemplateBmp.Height; j++)
            {
                for (int i = 0; i < TemplateBmp.Width; i++)
                {
                    if (TemplateData[i, j, 0] == 0 && TemplateData[i, j, 1] == 0 && TemplateData[i, j, 2] == 0)
                    {
                        EdgeP = new System.Drawing.Point();
                        EdgeP.X = i;
                        EdgeP.Y = j;
                        TemplateEdgeList.Add(EdgeP);
                    }
                }
            }

            // -------------------------2.Template Mapping------------------------
            int[, ,] ProcessData = BMPFast.GetRGBData(_ProcessBmp);

            //debug 點用
            TempEdgeDraw = new List<System.Drawing.Point>();

            FinalWeldGrid = IP.TemplateMapping(ProcessData, TemplateEdgeList, MapGrid, GoodWeldP, GridNum, out TempEdgeDraw);


            return FinalWeldGrid;
        }
        private PointF[,] CoarsePosition(Bitmap LocalThreshBMP)
        {
            //===============================================
            ////==================粗定位==================
            //===============================================
            //1.3 ERO *4
            Bitmap THEroBitmap = IP.Erosion3x3(LocalThreshBMP, 4);//1F:4次

            //1.4  blob circle
            //判斷 blob 中接近圓圈的物件
            List<PointF> BlobCircleP = new List<PointF>();
            BlobCircleP = IP.BlobCircle(THEroBitmap);  //1F


            //1.5去除 blob抓到的雜訊
            List<PointF> FilterBlackBlobCircleP = IP.RemoveBlackNoise(BlobCircleP, OriBmpData, ShotBMP.Width, ShotBMP.Height);
            ////DEBUG 驗證 
            //MapGrid = new PointF[20, 20];
            //int z = 0;
            //foreach (PointF Gp in FilterBlackBlobCircleP)
            //{
            //    for (int j = 0; j < GridNum; j++)
            //        for (int i = 0; i < GridNum; i++)
            //        {
            //            if (z < FilterBlackBlobCircleP.Count())
            //            {
            //                MapGrid[i, j] = FilterBlackBlobCircleP[z];
            //                z++;
            //            }
            //        }
            //}
            //return MapGrid;

            //1.6 Fit 網格
            //1.6.1 移除by距離判定之雜訊,剩下都是好銲道
            GoodWeldP = IP.RemoveDistNoise(FilterBlackBlobCircleP, IniGridLength, GridNum);
            //DEBUG 驗證 
            //MapGrid = new PointF[20, 20];
            //int z = 0;
            //foreach (PointF Gp in GoodWeldP)
            //{
            //    for (int j = 0; j < GridNum; j++)
            //        for (int i = 0; i < GridNum; i++)
            //        {
            //            if (z < GoodWeldP.Count())
            //            {
            //                MapGrid[i, j] = GoodWeldP[z];
            //                z++;
            //            }
            //        }
            //}
            //return MapGrid;

            //1.6.2 by 好銲道修正網格間距
            int GoodGridLength = IP.GridLength(GoodWeldP);
            ////1.6.3 組網格橫線
            List<List<PointF>> LineCollection = IP.LineClassify(GoodWeldP, GoodGridLength);
            ////1.6.4 組網格橫線
            List<List<PointF>> ColumnCollection = IP.ColumnClassify(GoodWeldP, GoodGridLength);
            ////DEBUG 驗證 
            MapGrid = new PointF[100, 100];
            //int z = 0;

            //PointF[] tempPF = new PointF[100];
            //int SUMSUM = 0;
            //foreach (List<PointF> LINE1 in ColumnCollection)
            //{
            //    foreach (PointF PF in LINE1)
            //    {
            //        tempPF[SUMSUM] = PF;
            //        SUMSUM++;
            //    }
            //}
            //for (int j = 0; j < 20; j++)
            //    for (int i = 0; i < 20; i++)
            //    {
            //        if (z < SUMSUM)
            //        {
            //            MapGrid[i, j] = tempPF[z];
            //            z++;
            //        }

            //    }
            //return MapGrid;


            //1.6.5 by 線組合計算x座標平均做為爛銲道 x 座標y 座標
            MapGrid = new PointF[GridNum, GridNum];
            int YAvg = 0;
            ////求每條線的 Y 平均
            for (int k = 0; k < GridNum; k++)
            {
                YAvg = 0;
                if (LineCollection[k].Count() > 0)
                {

                    foreach (PointF p in LineCollection[k])
                        YAvg += (int)p.Y;

                    YAvg = YAvg / LineCollection[k].Count();

                    for (int i = 0; i < GridNum; i++)
                    {

                        MapGrid[i, k].Y = YAvg;
                    }
                }
            }

            int XAvg = 0;
            //求每條線的 Y 平均
            for (int k = 0; k < GridNum; k++)
            {
                XAvg = 0;
                if (ColumnCollection[k].Count() > 0)
                {

                    foreach (PointF p in ColumnCollection[k])
                        XAvg += (int)p.X;

                    XAvg = XAvg / ColumnCollection[k].Count();


                    for (int j = 0; j < GridNum; j++)
                    {

                        MapGrid[k, j].X = XAvg;
                    }
                }
            }

            ////1.6.6 把好銲道蓋上去
            double ShortDist = 100000000;
            double dist;
            System.Drawing.Point GPIdx;

            foreach (PointF Gp in GoodWeldP)
            {
                ShortDist = 100000000;
                GPIdx = new System.Drawing.Point();

                for (int j = 0; j < GridNum; j++)
                    for (int i = 0; i < GridNum; i++)
                    {
                        dist = (MapGrid[i, j].X - Gp.X) * (MapGrid[i, j].X - Gp.X) + (MapGrid[i, j].Y - Gp.Y) * (MapGrid[i, j].Y - Gp.Y);
                        if (dist < ShortDist && dist != 0)
                        {
                            ShortDist = dist;
                            GPIdx.X = i;
                            GPIdx.Y = j;
                        }
                    }
                if (ShortDist < 300 * 300)
                {
                    MapGrid[GPIdx.X, GPIdx.Y] = Gp;
                }
            }


            return MapGrid;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //開啟舊檔先暫停攝影
            iTickNum = 1;

            MeasLbl.Text = "取像:";
            CCDLbl.Text = "檢測:";

            string ImgFileName;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImgFileName = openFileDialog1.FileName;
                FileBMP = new Bitmap(ImgFileName);
            }

            // ViewBmp = FileBMP;
            CCDView1.Image = FileBMP;
            ShotBMP = FileBMP;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //繼續攝影
            //MeasLbl.Text = "取像:";
            //CCDLbl.Text = "檢測:";
            //iTickNum = 0;
        }

        private void ScoreChkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (ScoreChkBox1.Checked == true && DrawScoreBmp != null)
            {
                MeasView.Image = DrawScoreBmp;

            }
            else if (DrawBmp != null)
            {
                MeasView.Image = DrawBmp;
            }
        }

        private void AutoMeasBtn_Click(object sender, EventArgs e)
        {

            AutoMeasBtn.Enabled = false;

            //purge file
            FileIO.DeleteFile(SaveFilePath, "*.bmp", 0.3);

            AutoShotThread = new Thread(new ThreadStart(AutoStageShot));
            Form.CheckForIllegalCrossThreadCalls = false; // 存取 UI 時需要用,較不安全的寫法,改用委派較佳(EX: UPDATE TXTBOX)
            AutoShotThread.Start();


            ImageProcessThread = new Thread(new ThreadStart(MeasImageProcess));
            Form.CheckForIllegalCrossThreadCalls = false; // 存取 UI 時需要用,較不安全的寫法,改用委派較佳(EX: UPDATE TXTBOX)
            ImageProcessThread.Start();
        }

        private void MeasImageProcess()
        {

            string ImgFileName;

            int ImageCnt = 0;
            int StartClock = 0;//計算時間開始,只開始一次

            List<String> ProcessOKFileName = new List<String>();
            string[] files;

            System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();//引用stopwatch物件

            List<DefectP> AreaDefectP;
            DefectInfoList = new List<List<DefectP>>();
            string[] row;
            DefectInfoDataGrid.Rows.Clear();
            Bitmap NewResultSaveBmp = new Bitmap(100, 100);

            while (true)
            {

                files = Directory.GetFiles(SaveFilePath, "*.bmp");
                foreach (string file in files)
                {
                    if (!(ProcessOKFileName.Exists(OKfile => OKfile == file))//處理過的不可再抓
                        && (ShotFileName.Exists(Shotfile => Shotfile == file))) //只抓本次取像
                    {

                        if (StartClock == 0)
                        {
                            clock.Reset();//碼表歸零
                            clock.Start();//碼表開始計時
                            StartClock = 1;//有符合條件者開始計時只開一次
                        }

                        ImgFileName = file;
                        string Substr = file.Substring(file.LastIndexOf("\\") + 1, file.Length - file.LastIndexOf("\\") - 1);
                        int AreaIdx = Convert.ToInt32(Substr.Substring(Substr.IndexOf("_") + 1, 1));

                        Console.WriteLine("1.COPY FILE");
                        //避免檔案被咬住
                        FileStream FS1 = new FileStream(ImgFileName, FileMode.Open, FileAccess.Read);
                        FileBMP = new Bitmap(FS1);
                        FS1.Close();
                        FS1.Dispose();



                        //AreaDefectP = new List<DefectP>();
                        //----------------偵測銲道開始----------------------
                        ProcessMeasBmp = ImageProcess(FileBMP, out AreaDefectP, AreaIdx);

                        Console.WriteLine("13.準備顯示");
                        DefectInfoList.Add(AreaDefectP);
                        //----------------偵測銲道結束----------------------



                        //------------------------------------------------
                        this.MeasLbl.BeginInvoke((MethodInvoker)delegate()
                        { this.MeasLbl.Text = "檢測: 第" + AreaIdx + "區  (" + Substr + ")"; });

                        MeasView.Image = ProcessMeasBmp;


                        Console.WriteLine("11.");


                        //存偵測結果
                        //NewResultSaveBmp = (Bitmap)ProcessMeasBmp.Clone();
                        string FileName = "ResultImage-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + AreaIdx + ".bmp";
                        FileName = SaveFilePath + FileName;
                        ProcessMeasBmp.Save(FileName);


                        ImageCnt++;
                        ProcessOKFileName.Add(file);



                        Console.WriteLine("12.");
                        if (ImageCnt == 6)

                            break;
                    }//end if
                }//end foreach

                if (ImageCnt == 6) break;
            }//end while


            Console.WriteLine("112.");
            if (StartClock == 1)
            {
                clock.Stop();//碼錶停止
                //印出所花費的總豪秒數
                MeasTime = Math.Floor(clock.Elapsed.TotalSeconds);
                StartClock = 0;
            }
            Console.WriteLine("113.");
            TotalClock.Stop();//碼錶停止
            //印出所花費的總豪秒數
            TotalCycleTime = Math.Floor(TotalClock.Elapsed.TotalSeconds);
            // Console.WriteLine("流程共花" + TotalCycleTime.ToString() + "秒");




            //-----------------------------------------------------
            Console.WriteLine("114.");
            MeasLbl.Text = "流程共花" + TotalCycleTime.ToString() + "秒";



            foreach (List<DefectP> DPList in DefectInfoList)
                foreach (DefectP DP in DPList)
                {

                    row = new string[] { "", "", "" };


                    row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                    DefectInfoDataGrid.Rows.Add(row);

                }



            Console.WriteLine("115.");
            //GC.Collect();
            MessageBox.Show("檢測結束!");
            AutoMeasBtn.Enabled = true;
            Console.WriteLine("116.");
            ProcessMeasBmp.Dispose();
            NewResultSaveBmp.Dispose();
            //MeasLbl.Text = "流程共花" + TotalCycleTime.ToString() + "秒";

        }
        private Bitmap PerformanceDemoProcess(Bitmap OriFileBmp, List<PerformanceDataInfo> _PerformanceData, string _ModelID, string _AB, string _AreaID)
        {


            Console.WriteLine("2.讀檔");
            int[, ,] OriBmpArr = new int[OriFileBmp.Width, OriFileBmp.Height, 3];
            OriBmpArr = BMPFast.GetRGBData(OriFileBmp);


            Console.WriteLine("3.複製");
            Bitmap FormatBmp = (Bitmap)OriFileBmp.Clone();
            FormatBmp = IP.FormatNormalize(FormatBmp);
            DrawBmp = new Bitmap(FormatBmp);

            Console.WriteLine("4.");
            Bitmap OtsuBMP = IP.OstuThresh(FormatBmp, OriBmpArr);


            Console.WriteLine("5.");
            List<PointF> _BlackTrayHoleP = new List<PointF>();
            _BlackTrayHoleP = IP.BlackTrayHolePosforPerm(OtsuBMP);

            Console.WriteLine("6.");
            PointF[,] _MapGrid = new PointF[10, 10];
            List<PointF> _GoodWeldP = new List<PointF>();
            _MapGrid = IP.LineColGridforPerm(_BlackTrayHoleP, _GoodWeldP);

            Thread.Sleep(500);

            DrawBmp = new Bitmap(FormatBmp);
            Graphics g1 = Graphics.FromImage(DrawBmp);
            System.Drawing.SolidBrush RedBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);//画刷
            System.Drawing.SolidBrush GreenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);//画刷
            System.Drawing.SolidBrush BlueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);//画刷
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 80);
            string drawString = "x";


            foreach (PerformanceDataInfo di in _PerformanceData)
            {
                if (di._pModelID == _ModelID)//plc告知
                {
                    //---------------------if (di._pAB =="A")//plc告知--------------------
                    if (di._pAreaID == _AreaID)//拍攝時電腦cnt
                    {
                        if (((_AreaID == "1" || _AreaID == "2") && _AB == "B")     //B面
                                || ((_AreaID == "5" || _AreaID == "6") && _AB == "A"))//A面
                        {
                            int DefectWeldID = Convert.ToInt32(di._pWeldID) - 1;
                            int DefectWeldID_X = DefectWeldID / 7;
                            int DefectWeldID_Y = DefectWeldID % 7;
                            //------------------
                            for (int i = 0; i < 7; i++)
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    if (DefectWeldID_X == j)
                                        if (DefectWeldID_Y == i)
                                            //g1.FillEllipse(RedBrush, _MapGrid[i, j].X + 50, _MapGrid[i, j].Y + 50, 60, 60);
                                            g1.DrawString(drawString, drawFont, RedBrush, _MapGrid[i, j].X - 10, _MapGrid[i, j].Y + 15, drawFormat);
                                }

                            }  //------------------
                        }
                        else if (((_AreaID == "5" || _AreaID == "6") && _AB == "B")   //456第一排(x,0)都是黑的不能算  //B面
                                  || ((_AreaID == "1" || _AreaID == "2") && _AB == "A"))//123第一排(x,0)都是黑的不能算  //A面
                        {
                            int DefectWeldID = 0;
                            if (_AB == "B")
                                DefectWeldID = Convert.ToInt32(di._pWeldID) - 1 + 7; // B面
                            else if (_AB == "A")
                                DefectWeldID = Convert.ToInt32(di._pWeldID) - 1;//A面

                            int DefectWeldID_X = DefectWeldID / 7;
                            int DefectWeldID_Y = DefectWeldID % 7;
                            //------------------
                            for (int i = 0; i < 7; i++)
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    if (DefectWeldID_X == j)
                                        if (DefectWeldID_Y == i)
                                            //g1.FillEllipse(RedBrush, _MapGrid[i, j].X + 50, _MapGrid[i, j].Y + 50, 60, 60);
                                            g1.DrawString(drawString, drawFont, RedBrush, _MapGrid[i, j].X - 10, _MapGrid[i, j].Y + 15, drawFormat);
                                }

                            }  //------------------
                        }
                        else if ( (_AreaID == "3" && _AB == "B")//34只有最右邊那一排,且4最上面是空的 B面
                               || (_AreaID == "4" && _AB == "A"))//34只有最右邊那一排,且4最上面是空的 A面
                        {
                            int DefectWeldID = Convert.ToInt32(di._pWeldID) - 1;
                            //g1.FillEllipse(RedBrush, _MapGrid[6, DefectWeldID].X + 50, _MapGrid[6, DefectWeldID].Y + 50, 60, 60);
                            g1.DrawString(drawString, drawFont, RedBrush, _MapGrid[6, DefectWeldID].X - 10, _MapGrid[6, DefectWeldID].Y + 15, drawFormat);

                        }
                        else if (    (_AreaID == "4" && _AB == "B")//34只有最有邊那一排,且4最上面是空的 B面
                                 ||  (_AreaID == "3" && _AB == "A"))//34只有最右邊那一排,且4最上面是空的 A面
                        {
                            int DefectWeldID = Convert.ToInt32(di._pWeldID);
                            //g1.FillEllipse(RedBrush, _MapGrid[6, DefectWeldID].X + 50, _MapGrid[6, DefectWeldID].Y + 50, 60, 60);
                            g1.DrawString(drawString, drawFont, RedBrush, _MapGrid[6, DefectWeldID].X - 10, _MapGrid[6, DefectWeldID].Y + 15, drawFormat);

                        }
                    }//---------if AreaID--------------------
                }//---------if ModelID--------------------

            }//end foreach

            //for (int i = 0; i < 7; i++)
            //{
            //    for (int j = 0; j < 7; j++)
            //    {

            //        g1.FillEllipse(BlueBrush, _MapGrid[i, j].X - 20, _MapGrid[i, j].Y - 20, 40, 40);
            //    }
            //}
            //foreach (PointF PF in _BlackTrayHoleP)
            //{
            //    g1.FillEllipse(BlueBrush, PF.X - 20, PF.Y - 20, 40, 40);
            //}


            return DrawBmp;


        }
        private Bitmap ImageProcess(Bitmap OriFileBmp, out List<DefectP> _DefectInfo, int _AreaIdx)
        {
            _DefectInfo = new List<DefectP>();

            Console.WriteLine("2.讀檔");
            int[, ,] OriBmpArr = new int[OriFileBmp.Width, OriFileBmp.Height, 3];
            OriBmpArr = BMPFast.GetRGBData(OriFileBmp);

            Console.WriteLine("3.複製");
            Bitmap FormatBmp = (Bitmap)OriFileBmp.Clone();
            FormatBmp = IP.FormatNormalize(FormatBmp);

            Console.WriteLine("4.");
            Bitmap OtsuBMP = IP.OstuThresh(FormatBmp, OriBmpArr);


            Console.WriteLine("5.");
            List<PointF> _BlackTrayHoleP = new List<PointF>();
            _BlackTrayHoleP = IP.BlackTrayHolePos(OtsuBMP);

            Console.WriteLine("6.");
            Bitmap EroLocalThreshBMP = IP.EroLocalThresh(FormatBmp);

            Console.WriteLine("7.");
            List<PointF> _GoodWeldP = new List<PointF>();
            _GoodWeldP = IP.GoodWeldPos(EroLocalThreshBMP, OriBmpArr);

            Console.WriteLine("8.");
            PointF[,] _MapGrid = new PointF[10, 10];
            _MapGrid = IP.LineColGrid(_BlackTrayHoleP, _GoodWeldP);

            Console.WriteLine("9.");
            //處理 mapgrid
            _MapGrid = IP.MapGridFilter(_MapGrid, OriBmpArr);

            Console.WriteLine("10.");
            WeldCenterInfo[,] _WeldCenterList = new WeldCenterInfo[10, 10];
            _WeldCenterList = IP.TemplateMatch(EroLocalThreshBMP, _MapGrid, _GoodWeldP);


            //////------------繪圖--------------
            Console.WriteLine("11.畫圖");
            DrawBmp = new Bitmap(FormatBmp);
            DrawScoreBmp = new Bitmap(FormatBmp);
            Graphics g1 = Graphics.FromImage(DrawBmp);
            Graphics g2 = Graphics.FromImage(DrawScoreBmp);

            System.Drawing.SolidBrush RedBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);//画刷
            System.Drawing.SolidBrush GreenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);//画刷
            System.Drawing.SolidBrush BlueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);//画刷

            //for (int j = 0; j < 7; j++)
            //    for (int i = 0; i < 7; i++)
            //    {
            //        //foreach (PointF PF in _BlackTrayHoleP)
            //        //{
            //        g1.FillEllipse(RedBrush, _MapGrid[i, j].X - 15, _MapGrid[i, j].Y - 15, 30, 30);
            //        g2.FillEllipse(RedBrush, _MapGrid[i, j].X - 15, _MapGrid[i, j].Y - 15, 30, 30);
            //    }

            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();

            string drawString = "";
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 40);


            DefectP DP = new DefectP();
            for (int j = 0; j < 7; j++)
                for (int i = 0; i < 7; i++)
                {
                    //處理找不到者 :０分，　座標-1

                    float x = 150.0F;
                    float y = 50.0F;

                    if (_WeldCenterList[i, j].WeldScore < 9999)
                    {
                        if (_WeldCenterList[i, j].WeldScore == 0)
                        {
                            drawString = "x";
                            DP = new DefectP();
                            DP.AreaIdx = _AreaIdx;
                            DP.DefectScore = _WeldCenterList[i, j].WeldScore;
                            DP.PID.X = i + 1;
                            DP.PID.Y = j + 1;
                            _DefectInfo.Add(DP);

                            g1.DrawString(drawString, drawFont, RedBrush, _WeldCenterList[i, j].WeldCandidate.X - 25, _WeldCenterList[i, j].WeldCandidate.Y + 35, drawFormat);
                            g2.DrawString(drawString, drawFont, RedBrush, _WeldCenterList[i, j].WeldCandidate.X - 25, _WeldCenterList[i, j].WeldCandidate.Y + 35, drawFormat);
                        }
                        else
                        {
                            drawString = _WeldCenterList[i, j].WeldScore.ToString();
                            g1.DrawString(drawString, drawFont, BlueBrush, _WeldCenterList[i, j].WeldCandidate.X - 25, _WeldCenterList[i, j].WeldCandidate.Y + 35, drawFormat);
                            g2.DrawString(drawString, drawFont, BlueBrush, _WeldCenterList[i, j].WeldCandidate.X - 25, _WeldCenterList[i, j].WeldCandidate.Y + 35, drawFormat);

                        }






                    }

                    if (_WeldCenterList[i, j].WeldScore != 0)
                    {
                        //    g1.FillEllipse(GreenBrush, _WeldCenterList[i, j].WeldCandidate.X - 15, _WeldCenterList[i, j].WeldCandidate.Y - 15, 30, 30);
                        //     g2.FillEllipse(GreenBrush, _WeldCenterList[i, j].WeldCandidate.X - 15, _WeldCenterList[i, j].WeldCandidate.Y - 15, 30, 30);

                    }

                }

            Console.WriteLine("11.");


            if (ScoreChkBox1.Checked == true)
                return DrawScoreBmp;
            else
                return DrawBmp;



        }
        private void FileProcess()
        {
            // copy old file



            // purge old file
            //  FileIO.DeleteFile(SaveFilePath, "*.bmp", 5);



        }
        private void AutoStageShot()
        {



            //1. 根據位置 list 走每個點
            List<PointF> StagePosList = new List<PointF>();
            PointF p1 = new PointF();
            PointF p2 = new PointF();
            PointF p3 = new PointF();
            PointF p4 = new PointF();
            PointF p5 = new PointF();
            PointF p6 = new PointF();


            //第一版
            //p1.X = (float)40;
            //p1.Y = (float)27;
            //p2.X = (float)40;
            //p2.Y = (float)172;
            //p3.X = (float)40;
            //p3.Y = (float)190;

            //p4.X = (float)190;
            //p4.Y = (float)190;
            //p5.X = (float)190;
            //p5.Y = (float)172;
            //p6.X = (float)190;
            //p6.Y = (float)27;

            //demo版本
            //p1.X = (float)40;
            //p1.Y = (float)5;
            //p2.X = (float)40;
            //p2.Y = (float)150;
            //p3.X = (float)40;
            //p3.Y = (float)165;

            //p4.X = (float)190;
            //p4.Y = (float)165;
            //p5.X = (float)190;
            //p5.Y = (float)150;
            //p6.X = (float)190;
            //p6.Y = (float)5;

            //0322版本
            p1.X = (float)30;
            p1.Y = (float)40;
            p2.X = (float)30;
            p2.Y = (float)182;
            p3.X = (float)30;
            p3.Y = (float)205;

            p4.X = (float)180;
            p4.Y = (float)205;
            p5.X = (float)180;
            p5.Y = (float)182;
            p6.X = (float)180;
            p6.Y = (float)40;




            StagePosList.Add(p1);
            StagePosList.Add(p2);
            StagePosList.Add(p3);
            StagePosList.Add(p4);
            StagePosList.Add(p5);
            StagePosList.Add(p6);


            System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();//引用stopwatch物件
            clock.Reset();//碼表歸零
            clock.Start();//碼表開始計時


            TotalClock = new System.Diagnostics.Stopwatch();//引用stopwatch物件
            TotalClock.Reset();//碼表歸零
            TotalClock.Start();//碼表開始計時



            ShotFileName = new List<string>();
            int ShotCnt = 0;
            foreach (PointF pf in StagePosList)
            {
                ShotCnt++;

                PLCAction.AutoStageMove(pf.X, pf.Y, 0, 100);
                Console.WriteLine(pf.X + "," + pf.Y);

                Thread.Sleep(250);
                while (true)
                {

                    double ReadVal1 = PLCAction.ReadPLCData("D1064");
                    double ReadVal2 = PLCAction.ReadPLCData("D1068");

                    // 到位時
                    if (Math.Abs(ReadVal1 - pf.X) < 0.001
                        && Math.Abs(ReadVal2 - pf.Y) < 0.001)
                    {

                        this.CCDLbl.BeginInvoke((MethodInvoker)delegate()
                        { this.CCDLbl.Text = "取像第" + ShotCnt + "區"; });

                        iTickNum = 0;
                        //Thread.Sleep(400);
                        Thread.Sleep(300);//0609



                        string FileName = "Image-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + ShotCnt + ".bmp";
                        FileName = SaveFilePath + FileName;
                        ShotBMP.Save(FileName);
                        Thread.Sleep(50);
                        ShotFileName.Add(FileName);


                        break;
                    }
                }

            }




            clock.Stop();//碼錶停止
            //印出所花費的總豪秒數
            ShotTime = Math.Floor(clock.Elapsed.TotalSeconds);
            this.CCDLbl.BeginInvoke((MethodInvoker)delegate()
            { this.CCDLbl.Text = "取像共花" + ShotTime.ToString() + "秒"; });

            //回歸第一點
           // PLCAction.AutoStageMove(p1.X, p1.Y, 0, 100);
            PLCAction.AutoStageMove(p1.X, 0, 0, 100);

        }

        private void AreaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AreaComboBox.Text != "ALL")
                SelectIdx = Convert.ToInt32(AreaComboBox.Text);
            else
                SelectIdx = 0;

            string[] row;
            DefectInfoDataGrid.Rows.Clear();

            string[] files = Directory.GetFiles(SaveFilePath, "*.bmp");

            switch (SelectIdx)
            {
                case 0:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            //if (DP.AreaIdx == 9)
                            //{
                            row = new string[] { "", "", "" };


                            row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                            DefectInfoDataGrid.Rows.Add(row);
                            //}
                        }




                    break;

                case 1:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 1)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }


                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_1") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;




                    break;

                case 2:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 2)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_2") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;

                    break;

                case 3:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 3)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }


                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_3") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;

                    break;
                case 4:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 4)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_4") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;

                    break;
                case 5:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 5)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_5") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;

                    break;
                case 6:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 6)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }


                    for (int i = 0; i < files.Length; i++)
                    {
                        if ((files[i].IndexOf("ResultImage-") > 0) && (files[i].IndexOf("_6") > 0))
                        {   //避免檔案被咬住
                            FileStream FS1 = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            FileBMP = new Bitmap(FS1);
                            FS1.Close();
                            FS1.Dispose();
                        }

                    }
                    MeasView.Image = FileBMP;

                    break;
                case 7:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 7)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    break;
                case 8:
                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 8)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    break;
                case 9:

                    foreach (List<DefectP> DPList in DefectInfoList)
                        foreach (DefectP DP in DPList)
                        {
                            if (DP.AreaIdx == 9)
                            {
                                row = new string[] { "", "", "" };


                                row = new string[] { DP.AreaIdx.ToString(),
                                          "("+DP.PID.X+","+DP.PID.Y+")",  
                                            DP.DefectScore.ToString()
                                        };
                                DefectInfoDataGrid.Rows.Add(row);
                            }
                        }

                    break;

                default:
                    break;
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BaslerCCD.DestroyCamera();
        }

        private void ManualXIncBtn_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void ManualXIncBtn_MouseCaptureChanged(object sender, EventArgs e)
        {

        }

        private void ManualXIncBtn_MouseUp(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinousPause();
        }

        private void ManualNoStopRadioBtn_Click(object sender, EventArgs e)
        {
            ManualMode = 1;
            ManualSet();
            PLCAction.axActUtlType1.SetDevice("M1201", 0);//連續移動
            ManualAsnPosRadioBtn.Checked = false;
            ManualAsnPosMoveBtn.Enabled = false;
        }

        private void ManualStepRadioBox_Click(object sender, EventArgs e)
        {
            ManualMode = 2;
            ManualSet();
            PLCAction.axActUtlType1.SetDevice("M1201", 1);//步階
            ManualAsnPosRadioBtn.Checked = false;
            ManualAsnPosMoveBtn.Enabled = false;
        }

        private void ManualXIncBtn_MouseDown(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinous("YNag");
        }

        private void ManualXDecBtn_MouseDown(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinous("YPos");
        }

        private void ManualXDecBtn_MouseUp(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinousPause();
        }

        private void ManualYIncBtn_MouseDown(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinous("XPos");
        }

        private void ManualYIncBtn_MouseUp(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinousPause();
        }

        private void ManualYDecBtn_MouseDown(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinous("XNag");
        }

        private void ManualYDecBtn_MouseUp(object sender, MouseEventArgs e)
        {
            ManualSet();
            PLCAction.ManualContinousPause();
        }

        private void ManualAsnPosMoveBtn_Click(object sender, EventArgs e)
        {
            ManualSet();//移動速度也要設定速度
            ManualMarkThread = new Thread(new ThreadStart(ManualMark));
            Form.CheckForIllegalCrossThreadCalls = false; // 存取 UI 時需要用,較不安全的寫法,改用委派較佳(EX: UPDATE TXTBOX)
            ManualMarkThread.Start();
        }
        private void ManualMark()
        {

            //平台走位
            double ManualXPos = Convert.ToDouble(ManualXPosTxtBox.Text);
            double ManualYPos = Convert.ToDouble(ManualYPosTxtBox.Text);
            PLCAction.AutoStageMove(ManualXPos, ManualYPos, 0, 100);//預設手動走3000,100%
            MessageBox.Show(this, "已移動至指定位置!");

        }

        private void ManualAsnPosRadioBtn_Click(object sender, EventArgs e)
        {
            ManualStepRadioBox.Checked = false;
            ManualNoStopRadioBtn.Checked = false;
            ManualAsnPosMoveBtn.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //string curDir;
            //curDir = Directory.GetCurrentDirectory();
            //saveFileDialog1.InitialDirectory = curDir;

            //saveFileDialog1.Filter = "Bitmap Image|*.bmp";
            //saveFileDialog1.Title = "儲存影像檔";
            //saveFileDialog1.FilterIndex = 3;
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //    ShotBMP.Save(saveFileDialog1.FileName);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            iTickNum = 0;
        }

        private void ManualStepDistComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void AutoGoHomeBtn_Click(object sender, EventArgs e)
        {

        }

        private void AutoGoHomeBtn_MouseDown(object sender, MouseEventArgs e)
        {

            PLCAction.axActUtlType1.SetDevice("M1003", 1); //x
            PLCAction.axActUtlType1.SetDevice("M1013", 1); //y
            Console.Write("hello");

        }

        private void AutoGoHomeBtn_MouseUp(object sender, MouseEventArgs e)
        {

            PLCAction.axActUtlType1.SetDevice("M1003", 0); //x
            PLCAction.axActUtlType1.SetDevice("M1013", 0); //y
        }

        private void PerformanceBtn_Click(object sender, EventArgs e)
        {
            AutoMeasBtn.Enabled = false;

            //purge file
            FileIO.DeleteFile(SaveFilePath, "*.bmp", 0.3);

            AutoShotThread = new Thread(new ThreadStart(AutoStageShot));
            Form.CheckForIllegalCrossThreadCalls = false; // 存取 UI 時需要用,較不安全的寫法,改用委派較佳(EX: UPDATE TXTBOX)
            AutoShotThread.Start();


            ImageProcessThread = new Thread(new ThreadStart(PerformanceProcess));
            Form.CheckForIllegalCrossThreadCalls = false; // 存取 UI 時需要用,較不安全的寫法,改用委派較佳(EX: UPDATE TXTBOX)
            ImageProcessThread.Start();
        }
        private void PerformanceProcess()
        {
            ModelID = "1";
            AB = "A";
            string ImgFileName;

            int ImageCnt = 0;
            int StartClock = 0;//計算時間開始,只開始一次

            List<String> ProcessOKFileName = new List<String>();
            string[] files;

            System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();//引用stopwatch物件


            DefectInfoList = new List<List<DefectP>>();
            string[] row;
            DefectInfoDataGrid.Rows.Clear();

            Bitmap NewResultSaveBmp = new Bitmap(100, 100);

            string SettingFilePath = "D:\\ITRI\\檢測機\\test\\A+PermanceTest1.csv";
            List<PerformanceDataInfo> PerformaceData = new List<PerformanceDataInfo>();
            PerformaceData = FileIO.PerformanceData(SettingFilePath);

            while (true)
            {

                files = Directory.GetFiles(SaveFilePath, "*.bmp");
                foreach (string file in files)
                {
                    if (!(ProcessOKFileName.Exists(OKfile => OKfile == file))//處理過的不可再抓
                        && (ShotFileName.Exists(Shotfile => Shotfile == file))) //只抓本次取像
                    {

                        if (StartClock == 0)
                        {
                            clock.Reset();//碼表歸零
                            clock.Start();//碼表開始計時
                            StartClock = 1;//有符合條件者開始計時只開一次
                        }

                        ImgFileName = file;
                        string Substr = file.Substring(file.LastIndexOf("\\") + 1, file.Length - file.LastIndexOf("\\") - 1);
                        int AreaIdx = Convert.ToInt32(Substr.Substring(Substr.IndexOf("_") + 1, 1));

                        Console.WriteLine("1.COPY FILE");
                        //避免檔案被咬住
                        FileStream FS1 = new FileStream(ImgFileName, FileMode.Open, FileAccess.Read);
                        FileBMP = new Bitmap(FS1);
                        FS1.Close();
                        FS1.Dispose();


                        Console.WriteLine("====================檢測: 第" + AreaIdx + "區  (" + Substr + ")====================");
                        //AreaDefectP = new List<DefectP>();
                        //----------------偵測銲道開始----------------------
                        //ProcessMeasBmp = ImageProcess(FileBMP, out AreaDefectP, AreaIdx);

                        ProcessMeasBmp = PerformanceDemoProcess(FileBMP, PerformaceData, ModelID, AB, AreaIdx.ToString());
                        //ProcessMeasBmp = FileBMP;

                        Console.WriteLine("13.準備顯示");
                        // DefectInfoList.Add(AreaDefectP);
                        //----------------偵測銲道結束----------------------


                        //------------------------------------------------
                        this.MeasLbl.BeginInvoke((MethodInvoker)delegate()
                        { this.MeasLbl.Text = "檢測: 第" + AreaIdx + "區  (" + Substr + ")"; });

                        MeasView.Image = ProcessMeasBmp;


                        Console.WriteLine("11.");


                        //存偵測結果
                        //NewResultSaveBmp = (Bitmap)ProcessMeasBmp.Clone();
                        string FileName = "ResultImage-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + AreaIdx + ".bmp";
                        FileName = SaveFilePath + FileName;
                        ProcessMeasBmp.Save(FileName);


                        ImageCnt++;
                        ProcessOKFileName.Add(file);



                        Console.WriteLine("12.");
                        if (ImageCnt == 6)

                            break;
                    }//end if
                }//end foreach

                if (ImageCnt == 6) break;
            }//end while


            Console.WriteLine("112.");
            if (StartClock == 1)
            {
                clock.Stop();//碼錶停止
                //印出所花費的總豪秒數
                MeasTime = Math.Floor(clock.Elapsed.TotalSeconds);
                StartClock = 0;
            }
            Console.WriteLine("113.");
            TotalClock.Stop();//碼錶停止
            //印出所花費的總豪秒數
            TotalCycleTime = Math.Floor(TotalClock.Elapsed.TotalSeconds);
            // Console.WriteLine("流程共花" + TotalCycleTime.ToString() + "秒");




            //-----------------------------------------------------
            Console.WriteLine("114.");
            MeasLbl.Text = "流程共花" + TotalCycleTime.ToString() + "秒";


            for (int i = 0; i < PerformaceData.Count; i++)
            {

                row = new string[] { "", "", "", "" };
                row = new string[] {            PerformaceData[i]._pModelID.ToString(),
                                                PerformaceData[i]._pAB.ToString(),  
                                                PerformaceData[i]._pAreaID.ToString(),
                                                PerformaceData[i]._pWeldID.ToString()
                                            };
                if (PerformaceData[i]._pModelID.ToString() != "ModelID")
                    DefectInfoDataGrid.Rows.Add(row);


            }






            Console.WriteLine("115.");
            //GC.Collect();
            MessageBox.Show("檢測結束!");
            AutoMeasBtn.Enabled = true;
            Console.WriteLine("116.");
            ProcessMeasBmp.Dispose();
            NewResultSaveBmp.Dispose();
            //MeasLbl.Text = "流程共花" + TotalCycleTime.ToString() + "秒";

           

        }

        private void ManualInputBtn_Click(object sender, EventArgs e)
        {
            //平台走位
            PLCAction.AutoStageMove(90, 271, 0, 100);//預設手動走3000,100%
            MessageBox.Show(this, "已移動至入料位置!");
        }

        private void M1026Btn_Click(object sender, EventArgs e)
        {
            PLCAction.FlowMeasModuleIn();
        }

        private void M1027Btn_Click(object sender, EventArgs e)
        {
            PLCAction.FlowMeasModuleOut();
        }

        private void D2460Btn_Click(object sender, EventArgs e)
        {
            PLCAction.FlowMeasInsertRepairData();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            PLCAction.WriteD16Val(5);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double[] test = PLCAction.ReadPLCDataRandom16("D2457\nD2458\nD2459", 3);

            for (int i = 0; i < test.Count(); i++)
                Console.WriteLine("test[" + i + "]=" + test[i]);
        }
       

    }
    public class DefectP
    {
        public int AreaIdx;
        public System.Drawing.Point PID;
        public int DefectScore;
    }
}
