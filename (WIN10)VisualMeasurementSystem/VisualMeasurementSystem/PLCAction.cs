using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ActUtlTypeLib;

namespace VisualMeasurementSystem
{
    public partial class PLCAction : Form
    {
        //FileIO FileIO = new FileIO();
        public PLCAction()
        {
            InitializeComponent();
        }
        public int PLC_Connect(int iLogicalStationNumber)//LogicalStationNumber for ActUtlType
        {
            int iReturnCode = 0;				//Return code

            try
            {
                //Set the value of 'LogicalStationNumber' to the property.
                axActUtlType1.ActLogicalStationNumber = iLogicalStationNumber;

                //The Open method is executed.
                iReturnCode = axActUtlType1.Open();

                //When the Open method is succeeded, make the EventHandler of ActProgType Controle.
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message,
                            Name, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return iReturnCode;

        }
        //public double[] ReadPLCDataRandom(string DeviceName, int DataSize)//一次讀多個位置
        //{

        //    short[] DataRead = new short[DataSize];
        //    int ReadValINT32;
        //    double ReadValSingle = 0.0;
        //    try
        //    {
        //        axActUtlType1.ReadDeviceRandom2(DeviceName, DataSize, out DataRead[0]);

        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message, Name,
        //                         MessageBoxButtons.OK, MessageBoxIcon.Error);


        //    }
        //    //解析 DeviceName 有幾個D,幾個R, 幾個M
        //    Regex rD = new Regex("D");
        //    MatchCollection MCD;
        //    MCD = rD.Matches(DeviceName);

        //    Regex rR = new Regex("R");
        //    MatchCollection MCR;
        //    MCR = rR.Matches(DeviceName);

        //    double[] ReadVal = new double[((MCD.Count + MCR.Count) / 2) + (DataRead.Length - (MCD.Count + MCR.Count))];

        //    //前幾組D/R要一次讀兩個
        //    int k = 0;
        //    for (int i = 0; i < (MCD.Count + MCR.Count); i = i + 2)
        //    {
        //        ReadValINT32 = Short2Int32(DataRead[i], DataRead[i + 1]);
        //        ReadValSingle = Convert.ToDouble(ReadValINT32) / Convert.ToDouble(10000);
        //        ReadVal[k] = ReadValSingle;
        //        k++;
        //    }
        //    //M一次只要讀一個
        //    for (int i = (MCD.Count + MCR.Count); i < DataRead.Length; i++)
        //    {
        //        ReadValSingle = DataRead[i];
        //        ReadVal[k] = ReadValSingle;
        //        k++;
        //    }

        //    return ReadVal;

        //}
        public double[] ReadPLCDataRandom(string DeviceName, int DataSize)//一次讀多個位置
        {

            short[] DataRead = new short[DataSize];
            int ReadValINT32;
            double ReadValSingle = 0.0;
            try
            {
                axActUtlType1.ReadDeviceRandom2(DeviceName, DataSize, out DataRead[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name,
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //解析 DeviceName 有幾個D,幾個R, 幾個M
            Regex rD = new Regex("D");
            MatchCollection MCD;
            MCD = rD.Matches(DeviceName);

            Regex rR = new Regex("R");
            MatchCollection MCR;
            MCR = rR.Matches(DeviceName);

            double[] ReadVal = new double[((MCD.Count + MCR.Count) / 2) + (DataRead.Length - (MCD.Count + MCR.Count))];

            //前幾組D/R要一次讀兩個

            //for (int i=0; i< ;i++)
            //{
            //    if()

            //}
            string[] SubDevices;
            SubDevices = ParsingDevice(DeviceName);
            int j = 0, k = 0;
            for (int i = 0; i < SubDevices.Length; i++)
            {
                // D or R 要一次讀兩個,兩個一組
                if (SubDevices[i].Substring(0, 1) == "D" || SubDevices[i].Substring(0, 1) == "R")
                {
                    ReadValINT32 = Short2Int32(DataRead[j], DataRead[j + 1]);
                    ReadValSingle = Convert.ToDouble(ReadValINT32) / Convert.ToDouble(10000);
                    ReadVal[k] = ReadValSingle;
                    i++;  // 因為一次讀兩個,i也要跳過增加1
                    k++;
                    j += 2;
                }
                // M 一次讀一個
                else if (SubDevices[i].Substring(0, 1) == "M")
                {
                    ReadValSingle = DataRead[j];
                    ReadVal[k] = ReadValSingle;
                    k++;
                    j++;
                }
            }
            ////前幾組D/R要一次讀兩個
            //for (int i = 0; i < (MCD.Count + MCR.Count); i = i + 2)
            //{
            //    ReadValINT32 = Short2Int32(DataRead[i], DataRead[i + 1]);
            //    ReadValSingle = Convert.ToDouble(ReadValINT32) / Convert.ToDouble(10000);
            //    ReadVal[k] = ReadValSingle;
            //    k++;
            //}
            ////M一次只要讀一個
            //for (int i = (MCD.Count + MCR.Count); i < DataRead.Length; i++)
            //{
            //    ReadValSingle = DataRead[i];
            //    ReadVal[k] = ReadValSingle;
            //    k++;
            //}

            return ReadVal;

        }

        public double[] ReadPLCDataRandom16(string DeviceName, int DataSize)//一次讀多個位置
        {

            int[] DataRead16 = new int[DataSize];
            int ReadValINT16;
            double ReadValSingle = 0.0;
            try
            {
                axActUtlType1.ReadDeviceRandom(DeviceName, DataSize, out DataRead16[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name,
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //解析 DeviceName 有幾個D,幾個R, 幾個M
            Regex rD = new Regex("D");
            MatchCollection MCD;
            MCD = rD.Matches(DeviceName);


            double[] ReadVal = new double[MCD.Count];


            string[] SubDevices;
            SubDevices = ParsingDevice(DeviceName);

            int j = 0, k = 0;
            for (int i = 0; i < SubDevices.Length; i++)
            {

                if (SubDevices[0].Substring(0, 1) == "D")
                {
                    ReadValINT16 = DataRead16[j];// Short2Int32(DataRead[j], DataRead[j + 1]);
                    ReadValSingle = Convert.ToDouble(ReadValINT16);/// Convert.ToDouble(10000);
                    ReadVal[k] = ReadValSingle;
                    k++;
                    j++;

                }

            }

            return ReadVal;

        }


        public string[] ParsingDevice(string _DeviceName)
        {
            Char delimiter = '\n';
            string[] _SubDevices = _DeviceName.Split(delimiter);
            //foreach (var SubDevice in SubDevices)
            //    Console.WriteLine(SubDevice);
            return _SubDevices;
        }
        public void WriteD16Val(double InsertVal)
        {
            int[] Int16Val = new int[12];
            Int16Val[0] = 122;
            Int16Val[1] = 112;
            Int16Val[2] = 135;
            Int16Val[3] = 27;
            Int16Val[4] = 223;
            Int16Val[5] = 33;


            Int16Val[6] = 54;
            Int16Val[7] = 541;
            Int16Val[8] = 51;
            Int16Val[9] = 531;
            Int16Val[10] = 62;
            Int16Val[11] = 642;


            axActUtlType1.WriteDeviceBlock("D2460", 1, ref Int16Val[0]);
            axActUtlType1.WriteDeviceBlock("D2461", 1, ref Int16Val[1]);
            axActUtlType1.WriteDeviceBlock("D2462", 1, ref Int16Val[2]);
            axActUtlType1.WriteDeviceBlock("D2463", 1, ref Int16Val[3]);
            axActUtlType1.WriteDeviceBlock("D2464", 1, ref Int16Val[4]);
            axActUtlType1.WriteDeviceBlock("D2465", 1, ref Int16Val[5]);


            axActUtlType1.WriteDeviceBlock("D2466", 1, ref Int16Val[6]);
            axActUtlType1.WriteDeviceBlock("D2467", 1, ref Int16Val[7]);
            axActUtlType1.WriteDeviceBlock("D2468", 1, ref Int16Val[8]);
            axActUtlType1.WriteDeviceBlock("D2469", 1, ref Int16Val[9]);
            axActUtlType1.WriteDeviceBlock("D2470", 1, ref Int16Val[10]);
            axActUtlType1.WriteDeviceBlock("D2471", 1, ref Int16Val[11]);

            double[] test1 = ReadPLCDataRandom16("D2460\nD2461\nD2462\nD2463\nD2464\nD2465", 6);
            double[] test2 = ReadPLCDataRandom16("D2466\nD2467\nD2468\nD2469\nD2470\nD2471", 6);

            for (int i = 0; i < test1.Count(); i++)
                Console.WriteLine("test1[" + i + "]=" + test1[i]);
            for (int i = 0; i < test2.Count(); i++)
                Console.WriteLine("test2[" + i + "]=" + test2[i]);
        }

        public short[] Int16Short(double argument1, int ratio)
        {

            int arg1;
            arg1 = Convert.ToInt16(argument1 * ratio);
            short[] ValueInt16 = new short[1];
            byte[] byteArray1 = BitConverter.GetBytes(arg1);
            ValueInt16[0] = BitConverter.ToInt16(byteArray1, 0);


            return ValueInt16;
        }
        public int Short2Int32(short argument1, short argument2)
        {
            int ValueInt32;
            byte[] byteArray1 = BitConverter.GetBytes(argument1);
            byte[] byteArray2 = BitConverter.GetBytes(argument2);
            byte[] byteArray3 = new byte[byteArray1.Length + byteArray2.Length];
            Array.Copy(byteArray1, 0, byteArray3, 0, byteArray1.Length);//合併 2 bytes+ 2 bytes
            Array.Copy(byteArray2, 0, byteArray3, byteArray1.Length, byteArray2.Length);

            ValueInt32 = BitConverter.ToInt32(byteArray3, 0);
            return ValueInt32;
        }
        public short[] Int32Short(double argument1, int ratio)
        {
            // input double=>32bits=>4 bytes=>2 short
            int arg1;
            arg1 = Convert.ToInt32(argument1 * ratio);
            short[] ValueInt16 = new short[2];
            byte[] byteArray1 = BitConverter.GetBytes(arg1);
            byte[] byteArray2 = new byte[2];
            byte[] byteArray3 = new byte[2];
            for (int i = 0; i <= 1; i++)
            {
                byteArray2[i] = byteArray1[i];
            }
            for (int i = 0; i <= 1; i++)
            {
                byteArray3[i] = byteArray1[i + 2];
            }
            ValueInt16[0] = BitConverter.ToInt16(byteArray2, 0);
            ValueInt16[1] = BitConverter.ToInt16(byteArray3, 0);
            return ValueInt16;
        }
        public short[] Int32ShortSpeed(double argument1)
        {
            // input double=>32bits=>4 bytes=>2 short
            int arg1;
            arg1 = Convert.ToInt32(argument1 * 100);
            short[] ValueInt16 = new short[2];
            byte[] byteArray1 = BitConverter.GetBytes(arg1);
            byte[] byteArray2 = new byte[2];
            byte[] byteArray3 = new byte[2];
            for (int i = 0; i <= 1; i++)
            {
                byteArray2[i] = byteArray1[i];
            }
            for (int i = 0; i <= 1; i++)
            {
                byteArray3[i] = byteArray1[i + 2];
            }
            ValueInt16[0] = BitConverter.ToInt16(byteArray2, 0);
            ValueInt16[1] = BitConverter.ToInt16(byteArray3, 0);
            return ValueInt16;
        }
        public double ReadPLCData(string DeviceName)
        {
            short[] DataRead = new short[2];
            int ReadValINT32;
            double ReadVal = 0.0;
            try
            {
                axActUtlType1.ReadDeviceBlock2(DeviceName, 2, out DataRead[0]);
                ReadValINT32 = Short2Int32(DataRead[0], DataRead[1]);
                ReadVal = Convert.ToDouble(ReadValINT32) / Convert.ToDouble(10000);
                // ManualMsgTxtBox.Text += Convert.ToString(StepDistVal) + "\r\n";

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name,
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
            return ReadVal;
        }
        public void GoHome()
        {

            //x軸復歸
            axActUtlType1.SetDevice("M1003", 0);
            axActUtlType1.SetDevice("M1013", 0);
            axActUtlType1.SetDevice("M1003", 1);

            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1000\nD1001\nM1108", 3);


                if (_ReadVal[0] == 300 &&//OriX && 
                    _ReadVal[1] == 0)
                    break;
            }
            //y軸復歸
            axActUtlType1.SetDevice("M1013", 0);
            axActUtlType1.SetDevice("M1013", 1);
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1010\nD1011\nM1118", 3);


                if (_ReadVal[0] == 300 && // OriY && 
                    _ReadVal[1] == 0)
                    break;
            }
            //201903 復歸MARK
            //Z軸復歸
            axActUtlType1.SetDevice("M1023", 0);
            axActUtlType1.SetDevice("M1023", 1);
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1020\nD1021\nM1128", 3);


                if (_ReadVal[0] == 300 && //OriZ && 
                    _ReadVal[1] == 0)
                    break;
            }



            axActUtlType1.SetDevice("M1003", 0);
            axActUtlType1.SetDevice("M1013", 0);
            axActUtlType1.SetDevice("M1023", 0);

            axActUtlType1.SetDevice("M1202", 1);
            axActUtlType1.SetDevice("M1202", 0);
            axActUtlType1.SetDevice("M1200", 1);
            axActUtlType1.SetDevice("M1200", 0);

            MessageBox.Show(this, "復歸完成!");

        }
        public void ManualContinous(string ActionPos)
        {
            //axActUtlType1.SetDevice("M1201", 0);//連續移動(滑鼠一值按著)

            //if (ActionPos == "XPos")
            //{
            //    axActUtlType1.SetDevice("M1001", 1);//X正向
            //    //FileIO.LogMotion("ManualContinous M1001=1");
            //}
            //else if (ActionPos == "XNag")
            //{
            //    axActUtlType1.SetDevice("M1000", 1);//X反向
            //    //FileIO.LogMotion("ManualContinous M1000=1");
            //}
            //else if (ActionPos == "YPos")
            //{
            //    axActUtlType1.SetDevice("M1010", 1);//Y正向
            //    // FileIO.LogMotion("ManualContinous M1010=1");
            //}
            //else if (ActionPos == "YNag")
            //{
            //    axActUtlType1.SetDevice("M1011", 1);//Y反向
            //    // FileIO.LogMotion("ManualContinous M1011=1");
            //}
            //else if (ActionPos == "ZPos")
            //{
            //    axActUtlType1.SetDevice("M1020", 1);//Z反向
            //    //FileIO.LogMotion("ManualContinous M1020=1");
            //}
            //else if (ActionPos == "ZNeg")
            //{
            //    axActUtlType1.SetDevice("M1021", 1);//Z反向
            //    // FileIO.LogMotion("ManualContinous M1021=1");
            //}
            //A+
            axActUtlType1.SetDevice("M1004", 0);//連續移動

            if (ActionPos == "XPos")
            {
                axActUtlType1.SetDevice("M1001", 1);//X正向
                //     FileIO.LogMotion("ManualContinous M1001=1");
            }
            else if (ActionPos == "XNag")
            {
                axActUtlType1.SetDevice("M1000", 1);//X反向
                //       FileIO.LogMotion("ManualContinous M1000=1");
            }
            else if (ActionPos == "YPos")
            {
                axActUtlType1.SetDevice("M1010", 1);//Y正向
                //      FileIO.LogMotion("ManualContinous M1010=1");
            }
            else if (ActionPos == "YNag")
            {
                axActUtlType1.SetDevice("M1011", 1);//Y反向
                //     FileIO.LogMotion("ManualContinous M1011=1");
            }
            else if (ActionPos == "ZPos")
            {
                axActUtlType1.SetDevice("M1020", 1);//Z反向
                //     FileIO.LogMotion("ManualContinous M1020=1");
            }
            else if (ActionPos == "ZNeg")
            {
                axActUtlType1.SetDevice("M1021", 1);//Z反向
                //      FileIO.LogMotion("ManualContinous M1021=1");
            }







        }

        public void ManualContinousPause()
        {
            //停止
            axActUtlType1.SetDevice("M1001", 0);//X正向
            //  FileIO.LogMotion("ManualContinousPause M1001=1");
            axActUtlType1.SetDevice("M1000", 0);//X反向
            //  FileIO.LogMotion("ManualContinousPause M1000=1");
            axActUtlType1.SetDevice("M1010", 0);//Y正向
            // FileIO.LogMotion("ManualContinousPause M1010=1");
            axActUtlType1.SetDevice("M1011", 0);//Y反向
            // FileIO.LogMotion("ManualContinousPause M1011=1");
            axActUtlType1.SetDevice("M1020", 0);//Z正向
            //  FileIO.LogMotion("ManualContinousPause M1020=1");
            axActUtlType1.SetDevice("M1021", 0);//Z反向
            //  FileIO.LogMotion("ManualContinousPause M1021=1");

        }
        public void ManualStep(string ActionPos)
        {
            axActUtlType1.SetDevice("M1201", 1);//連續移動(滑鼠按一次只移動一定距離)
            //   FileIO.LogMotion("ManualStep M1201=1");
            if (ActionPos == "XInc")
            {
                axActUtlType1.SetDevice("M1001", 1);//X正向
                //    FileIO.LogMotion("ManualStep M1001=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1101\nM1105", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }

                axActUtlType1.SetDevice("M1001", 0);
                //   FileIO.LogMotion("ManualStep M1001=0");
            }
            else if (ActionPos == "XDec")
            {
                axActUtlType1.SetDevice("M1000", 1);//X反向
                // FileIO.LogMotion("ManualStep M1000=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1100\nM1105", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }
                axActUtlType1.SetDevice("M1000", 0);
                //    FileIO.LogMotion("ManualStep M1000=0");
            }
            else if (ActionPos == "YInc")
            {
                axActUtlType1.SetDevice("M1010", 1);//Y正向
                //    FileIO.LogMotion("ManualStep M1010=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1110\nM1115", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }
                axActUtlType1.SetDevice("M1010", 0);
                // FileIO.LogMotion("ManualStep M1010=0");
            }
            else if (ActionPos == "YDec")
            {
                axActUtlType1.SetDevice("M1011", 1);//Y反向
                //   FileIO.LogMotion("ManualStep M1011=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1111\nM1115", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }
                axActUtlType1.SetDevice("M1011", 0);
                // FileIO.LogMotion("ManualStep M1011=1");
            }
            else if (ActionPos == "ZInc")
            {
                axActUtlType1.SetDevice("M1020", 1);//Z正向
                //  FileIO.LogMotion("ManualStep M1020=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1120\nM1125", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }
                axActUtlType1.SetDevice("M1020", 0);
                // FileIO.LogMotion("ManualStep M1020=0");
            }
            else if (ActionPos == "ZDec")
            {
                axActUtlType1.SetDevice("M1021", 1);//Z正向
                // FileIO.LogMotion("ManualStep M1021=1");
                while (true)
                {
                    double[] _ReadVal = ReadPLCDataRandom("M1121\nM1125", 2);
                    if (_ReadVal[0] == 1 && _ReadVal[1] == 0)
                        break;
                }
                axActUtlType1.SetDevice("M1021", 0);
                // FileIO.LogMotion("ManualStep M1021=0");
            }
        }
        public int FindMaxNum(List<int> AreaNumList)
        {
            int MaxNum = 0;
            for (int i = 0; i < AreaNumList.Count(); i++)
            {
                if (AreaNumList[i] > MaxNum) MaxNum = AreaNumList[i];
            }
            return (MaxNum);
        }
        public void ManualSet(double StepDistVal, double XSpeedVal, double YSpeedVal, double ZSpeedVal, double SpeedRateVal)
        {

            short[] StepDist = new short[2];
            short[] XSpeed = new short[2];
            short[] YSpeed = new short[2];
            short[] ZSpeed = new short[2];
            short[] SpeedRate = new short[2];
            short[] XTarget = new short[2];
            short[] YTarget = new short[2];
            short[] ZTarget = new short[2];


            //if (ManualStepDistComboBox.Text.Length > 0)
            StepDist = Int32Short(StepDistVal, 10000);

            //if (ManualXSpeedTxtBox.Text.Length > 0)
            XSpeed = Int32ShortSpeed(XSpeedVal);

            //if (ManualYSpeedTxtBox.Text.Length > 0)
            YSpeed = Int32ShortSpeed(YSpeedVal);

            ZSpeed = Int32ShortSpeed(ZSpeedVal);

            SpeedRate = Int32Short(SpeedRateVal, 10);

            //寫入吋動距離
            try
            { axActUtlType1.WriteDeviceBlock2("R1202", 2, ref StepDist[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }


            //寫入X軸速度
            try
            { axActUtlType1.WriteDeviceBlock2("R1002", 2, ref XSpeed[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }



            //寫入Y軸速度
            try
            { axActUtlType1.WriteDeviceBlock2("R1012", 2, ref YSpeed[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //寫入Z軸速度
            try
            { axActUtlType1.WriteDeviceBlock2("R1022", 2, ref ZSpeed[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //寫入速度比例
            try
            { axActUtlType1.WriteDeviceBlock2("R1200", 2, ref SpeedRate[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }


        }
        public void AutoZAxisMove(int i, int j, double IniZPt, string[][] AutoSpeedArray)
        {
            double ZTgtPos1, ZTgtPos2;
            short[] ZTarget = new short[2];
            short[] ZSpeed = new short[2];
            ZTgtPos1 = IniZPt;

            //寫入Z 軸目標位置
            ZTarget = Int32Short(ZTgtPos1, 10000);
            try
            { axActUtlType1.WriteDeviceBlock2("R1020", 2, ref ZTarget[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            ZTgtPos2 = ReadPLCData("R1020");

            //寫入Z軸速度
            for (int k = 0; k < AutoSpeedArray.Length; k++)
            {
                if (AutoSpeedArray[k][0] == "(" + (i + 1) + ',' + (j + 1) + ")") //user介面比實際陣列編號多1
                {
                    ZSpeed = Int32Short(Convert.ToDouble(AutoSpeedArray[k][3]), 100);
                    break;
                }
            }
            //--------------Z移動動作--------------
            axActUtlType1.SetDevice("M1122", 0);//自動模式要用一速移,先設為0避免卡住
            axActUtlType1.SetDevice("M1122", 1);//自動模式要用一速移動


            //Console.WriteLine("Z一速移動開始 M1122=1");
            //  FileIO.LogMotion("Z一速移動開始 M1122=1");
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1020\nD1021\nM1122\nM1125", 4);
                if (_ReadVal[1] == 1 && _ReadVal[2] == 0 && _ReadVal[0] == ZTgtPos2)
                    break;

            }

            axActUtlType1.SetDevice("M1122", 0);//自動模式要用一速移動
            //Console.WriteLine("X結束一速接收命令狀態 M1122=0");
            //  FileIO.LogMotion("X結束一速接收命令狀態 M1122=0");


        }
        public void AutoStageMove(double IniXPt, double IniYPt, double IniZPt, double SpeedRate)
        {
            double XTgtPos1, YTgtPos1 = 0;
            short[] XTarget = new short[2];
            short[] YTarget = new short[2];
            short[] XSpeed = new short[2];
            short[] YSpeed = new short[2];

            double ZTgtPos1;
            short[] ZTarget = new short[2];
            short[] ZSpeed = new short[2];

            //================ X ======================
            XTgtPos1 = IniXPt;
            //寫入X 軸目標位置
            XTarget = Int32Short(XTgtPos1, 10000);
            try
            { axActUtlType1.WriteDeviceBlock2("R1000", 2, ref XTarget[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            // XTgtPos2 = ReadPLCData("R1000");
            //寫入X軸速度-----------------------------------
            //XSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //try
            //{ axActUtlType1.WriteDeviceBlock2("R1002", 2, ref XSpeed[0]); }
            //catch (Exception exception)
            //{ MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //--------------X移動動作--------------
            axActUtlType1.SetDevice("M1002", 0);//自動模式要用一速移,先設為0避免卡住
            axActUtlType1.SetDevice("M1002", 1);//自動模式要用一速移動
            //FileIO.LogMotion("X一速移動開始 M1002=1");
            //----確認是否接受指令
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("M1102", 1);
                if (_ReadVal[0] == 1)
                {
                    axActUtlType1.SetDevice("M1002", 0);
                    break;
                }

            }

            ////-----------------判斷是否移動至目標位置完成--------------------

            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1064\nD1065\nM1102\nM1105", 4);
                if (//_ReadVal[1] == 1 && _ReadVal[2] == 0 && 
                    Math.Abs(_ReadVal[0] - XTgtPos1) < 0.001)
                    break;

            }
            //axActUtlType1.SetDevice("M1002", 0);//自動模式要用一速移動
            // FileIO.LogMotion("X結束一速接收命令狀態 M1002=0");
            //-----------------------------------------------------

            //================ Y ======================
            YTgtPos1 = IniYPt;
            //寫入Y 軸目標位置
            YTarget = Int32Short(YTgtPos1, 10000);
            try
            { axActUtlType1.WriteDeviceBlock2("R1010", 2, ref YTarget[0]); }
            catch (Exception exception)
            { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            //Console.WriteLine("寫入Y 軸目標位置 M1010" + "YTgtPos1=" + YTgtPos1);
            //寫入Y軸速度
            //YSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //try
            //{ axActUtlType1.WriteDeviceBlock2("R1012", 2, ref YSpeed[0]); }
            //catch (Exception exception)
            //{ MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //--------------Y移動動作--------------
            axActUtlType1.SetDevice("M1012", 0);//自動模式要用一速移,先設為0避免卡住
            axActUtlType1.SetDevice("M1012", 1);//自動模式要用一速移動
            //FileIO.LogMotion("Y一速移動開始 M1012=1");
            //----確認是否接受指令
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("M1112", 1);
                if (_ReadVal[0] == 1)
                {
                    axActUtlType1.SetDevice("M1012", 0);
                    break;
                }

            }
            //-----------------判斷是否移動至目標位置完成--------------------
            while (true)
            {
                double[] _ReadVal = ReadPLCDataRandom("D1068\nD1069\nM1112\nM1115", 4);
                if (//_ReadVal[1] == 1 && _ReadVal[2] == 0 && 
                    Math.Abs(_ReadVal[0] - YTgtPos1) < 0.001)
                    break;
                //   Console.WriteLine("Y一速移動,YTgtPos1=" + YTgtPos1 + ", _ReadVal[0]=" + _ReadVal[0]);
            }
            //axActUtlType1.SetDevice("M1012", 0);//自動模式要用一速移動
            //  FileIO.LogMotion("Y結束一速接收命令狀態 M1012=0");

            //-----------------------------------------------------
            //201903 復歸MARK
            //================ Z ======================
            //ZTgtPos1 = IniZPt;
            ////寫入Z 軸目標位置
            //ZTarget = Int32Short(ZTgtPos1, 10000);
            //ZSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //try
            //{ axActUtlType1.WriteDeviceBlock2("R1020", 2, ref ZTarget[0]); }
            //catch (Exception exception)
            //{ MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }


            //////寫入z軸速度
            ////try
            ////{ axActUtlType1.WriteDeviceBlock2("R1022", 2, ref ZSpeed[0]); }
            ////catch (Exception exception)
            ////{ MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            ////--------------Z移動動作--------------
            //axActUtlType1.SetDevice("M1022", 0);//自動模式要用一速移,先設為0避免卡住
            //axActUtlType1.SetDevice("M1022", 1);//自動模式要用一速移動
            ////----確認是否接受指令
            //while (true)
            //{

            //    double[] _ReadVal = ReadPLCDataRandom("M1122", 1);
            //    if (_ReadVal[0] == 1)
            //    {
            //        axActUtlType1.SetDevice("M1022", 0);
            //        break;
            //    }

            //}
            ////Console.WriteLine("Z一速移動開始 M1022=1");
            ////FileIO.LogMotion("Z一速移動開始 M1022=1");
            //while (true)
            //{
            //    double[] _ReadVal = ReadPLCDataRandom("D1060\nD1061\nM1122\nM1125", 4);
            //    if (//_ReadVal[1] == 1 && _ReadVal[2] == 0 && 
            //        Math.Abs(_ReadVal[0] - ZTgtPos1) < 0.001)
            //        break;

            //}

            //axActUtlType1.SetDevice("M1022", 0);//自動模式要用一速移動
            //FileIO.LogMotion("Z 結束一速接收命令狀態 M1122=0");





            //         double XTgtPos1, YTgtPos1 = 0;
            //         short[] XTarget = new short[2];
            //         short[] YTarget = new short[2];
            //         short[] XSpeed = new short[2];
            //         short[] YSpeed = new short[2];

            //         double ZTgtPos1, ZTgtPos2;
            //         short[] ZTarget = new short[2];
            //         short[] ZSpeed = new short[2];

            //         //================ X ======================
            //         XTgtPos1 = IniXPt;
            //         //寫入X 軸目標位置
            //         XTarget = Int32Short(XTgtPos1, 10000);
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1000", 2, ref XTarget[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //         // XTgtPos2 = ReadPLCData("R1000");
            //         //寫入X軸速度-----------------------------------
            //         XSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1002", 2, ref XSpeed[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //         //--------------X移動動作--------------
            //         axActUtlType1.SetDevice("M1002", 0);//自動模式要用一速移,先設為0避免卡住
            //         axActUtlType1.SetDevice("M1002", 1);//自動模式要用一速移動
            //         //  FileIO.LogMotion("X一速移動開始 M1002=1");
            //         ////-----------------判斷是否移動至目標位置完成--------------------
            //         while (true)
            //         {
            //             double[] _ReadVal = ReadPLCDataRandom("D1064\nD1065\nM1102\nM1105", 4);
            //             if (_ReadVal[1] == 1 && _ReadVal[2] == 0 && Math.Abs(_ReadVal[0] - XTgtPos1) < 0.0001)
            //                 break;

            //         }
            //         axActUtlType1.SetDevice("M1002", 0);//自動模式要用一速移動

            //         //-----------------------------------------------------

            //         //================ Y ======================
            //         YTgtPos1 = IniYPt;
            //         //寫入Y 軸目標位置
            //         YTarget = Int32Short(YTgtPos1, 10000);
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1010", 2, ref YTarget[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            // //      Console.WriteLine("寫入Y 軸目標位置 M1010" + "YTgtPos1=" + YTgtPos1);
            //         //寫入Y軸速度
            //         YSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1012", 2, ref YSpeed[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //         //--------------Y移動動作--------------
            //         axActUtlType1.SetDevice("M1012", 0);//自動模式要用一速移,先設為0避免卡住
            //         axActUtlType1.SetDevice("M1012", 1);//自動模式要用一速移動
            //         //FileIO.LogMotion("Y一速移動開始 M1012=1");

            //         //-----------------判斷是否移動至目標位置完成--------------------
            //         while (true)
            //         {
            //             double[] _ReadVal = ReadPLCDataRandom("D1010\nD1011\nM1112\nM1115", 4);
            //             if (_ReadVal[1] == 1 && _ReadVal[2] == 0 && Math.Abs(_ReadVal[0] - YTgtPos1) < 0.0001)
            //                 break;
            ////             Console.WriteLine("Y一速移動,YTgtPos1=" + YTgtPos1 + ", _ReadVal[0]=" + _ReadVal[0]);
            //         }
            //         axActUtlType1.SetDevice("M1012", 0);//自動模式要用一速移動
            //         //FileIO.LogMotion("Y結束一速接收命令狀態 M1012=0");

            //         //-----------------------------------------------------

            //         //================ Z ======================
            //         ZTgtPos1 = IniZPt;
            //         //寫入Z 軸目標位置
            //         ZTarget = Int32Short(ZTgtPos1, 10000);
            //         ZSpeed = Int32Short(3000 * SpeedRate / 100, 100);
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1020", 2, ref ZTarget[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }


            //         //寫入z軸速度
            //         try
            //         { axActUtlType1.WriteDeviceBlock2("R1022", 2, ref ZSpeed[0]); }
            //         catch (Exception exception)
            //         { MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //         //--------------Z移動動作--------------
            //         axActUtlType1.SetDevice("M1022", 0);//自動模式要用一速移,先設為0避免卡住
            //         axActUtlType1.SetDevice("M1022", 1);//自動模式要用一速移動

            //         //Console.WriteLine("Z一速移動開始 M1022=1");
            //         //FileIO.LogMotion("Z一速移動開始 M1022=1");
            //         while (true)
            //         {
            //             double[] _ReadVal = ReadPLCDataRandom("D1020\nD1021\nM1122\nM1125", 4);
            //             if (_ReadVal[1] == 1 && _ReadVal[2] == 0 && Math.Abs(_ReadVal[0] - ZTgtPos1) < 0.0001)
            //                 break;

            //         }

            //         axActUtlType1.SetDevice("M1022", 0);//自動模式要用一速移動
            //         //FileIO.LogMotion("Z 結束一速接收命令狀態 M1122=0");




        }
        public void MainControl(int PC_ControlVal)
        {
            //axActUtlType1.SetDevice("M1203", PC_ControlVal);//plc主控權給PC
            axActUtlType1.SetDevice("M1005", PC_ControlVal);//plc主控權給PC

        }

        public void PLC_StageMotor(int OnOff)
        {
            if (OnOff == 1)
            {
                //axActUtlType1.SetDevice("M1208", 0); //防呆先關
                //axActUtlType1.SetDevice("M1207", 1); //馬達on要等三秒.   

                //A+      
                axActUtlType1.SetDevice("M1043", 0); //防呆先關
                axActUtlType1.SetDevice("M1042", 1); //馬達on要等三秒. 
                while (true)
                {
                    //double[] _ReadVal = ReadPLCDataRandom("M1211", 1); //伺服馬達要等ready才能繼續進行下步驟
                    //A+
                    double[] _ReadVal = ReadPLCDataRandom("M1142", 1); //伺服馬達要等ready才能繼續進行下步驟

                    if (_ReadVal[0] == 1)
                    {
                        break;
                    }

                }
            }
            else
            {

                //axActUtlType1.SetDevice("M1202", 1);//20211019 自己加的伺服馬達關閉
                //axActUtlType1.SetDevice("M1207", 0);//20211019 自己加的伺服馬達關閉
                //axActUtlType1.SetDevice("M1208", 1);//伺服馬達關閉
                //axActUtlType1.SetDevice("M1208", 0);
                //A+  
                axActUtlType1.SetDevice("M1041", 1);//20211019 自己加的伺服馬達關閉
                axActUtlType1.SetDevice("M1042", 0);//20211019 自己加的伺服馬達關閉
                axActUtlType1.SetDevice("M1043", 1);//伺服馬達關閉
                axActUtlType1.SetDevice("M1043", 0);
                axActUtlType1.SetDevice("M1041", 0);

            }

        }


        public void LaserReq()
        {
            axActUtlType1.SetDevice("X36", 0);
            axActUtlType1.SetDevice("Y70", 0);

        }
        public int PLC_HandShakingRead()
        {
            //系統交握訊號讀回
            double[] _ReadVal1 = ReadPLCDataRandom("M1209", 1);
            int M1209_ = Convert.ToInt32(_ReadVal1[0]);
            return M1209_;
        }
        public void PLC_AllClear()
        {
            //初始化所有平台動作指令

            axActUtlType1.SetDevice("M1000", 0);
            axActUtlType1.SetDevice("M1001", 0);
            axActUtlType1.SetDevice("M1002", 0);
            axActUtlType1.SetDevice("M1003", 0);
            axActUtlType1.SetDevice("M1003", 0);
            axActUtlType1.SetDevice("M1005", 0);
            axActUtlType1.SetDevice("M1006", 0);
            axActUtlType1.SetDevice("M1007", 0);
            axActUtlType1.SetDevice("M1008", 0);
            axActUtlType1.SetDevice("M1010", 0);
            axActUtlType1.SetDevice("M1011", 0);
            axActUtlType1.SetDevice("M1012", 0);
            axActUtlType1.SetDevice("M1013", 0);
            axActUtlType1.SetDevice("M1014", 0);
            axActUtlType1.SetDevice("M1015", 0);
            axActUtlType1.SetDevice("M1016", 0);
            axActUtlType1.SetDevice("M1017", 0);
            axActUtlType1.SetDevice("M1018", 0);
            axActUtlType1.SetDevice("M1019", 0);
            axActUtlType1.SetDevice("M1020", 0);
            axActUtlType1.SetDevice("M1021", 0);
            axActUtlType1.SetDevice("M1022", 0);
            axActUtlType1.SetDevice("M1023", 0);
            axActUtlType1.SetDevice("M1024", 0);
            axActUtlType1.SetDevice("M1025", 0);
            axActUtlType1.SetDevice("M1026", 0);
            axActUtlType1.SetDevice("M1027", 0);
            axActUtlType1.SetDevice("M1028", 0);
            axActUtlType1.SetDevice("M1029", 0);
            axActUtlType1.SetDevice("M1030", 0);
            axActUtlType1.SetDevice("M1031", 0);
            axActUtlType1.SetDevice("M1032", 0);
            axActUtlType1.SetDevice("M1033", 0);
            axActUtlType1.SetDevice("M1034", 0);
            axActUtlType1.SetDevice("M1035", 0);
            axActUtlType1.SetDevice("M1036", 0);
            axActUtlType1.SetDevice("M1037", 0);
            axActUtlType1.SetDevice("M1038", 0);
            axActUtlType1.SetDevice("M1039", 0);
            axActUtlType1.SetDevice("M1040", 0);
            axActUtlType1.SetDevice("M1041", 0);

            //1222
            axActUtlType1.SetDevice("M1044", 0);


            axActUtlType1.SetDevice("M1100", 0);
            axActUtlType1.SetDevice("M1101", 0);
            axActUtlType1.SetDevice("M1102", 0);
            axActUtlType1.SetDevice("M1103", 0);
            axActUtlType1.SetDevice("M1104", 0);
            axActUtlType1.SetDevice("M1105", 0);
            axActUtlType1.SetDevice("M1106", 0);
            axActUtlType1.SetDevice("M1107", 0);
            axActUtlType1.SetDevice("M1108", 0);
            axActUtlType1.SetDevice("M1109", 0);
            axActUtlType1.SetDevice("M1110", 0);
            axActUtlType1.SetDevice("M1111", 0);
            axActUtlType1.SetDevice("M1112", 0);
            axActUtlType1.SetDevice("M1113", 0);
            axActUtlType1.SetDevice("M1114", 0);
            axActUtlType1.SetDevice("M1115", 0);
            axActUtlType1.SetDevice("M1116", 0);
            axActUtlType1.SetDevice("M1117", 0);
            axActUtlType1.SetDevice("M1118", 0);
            axActUtlType1.SetDevice("M1119", 0);
            axActUtlType1.SetDevice("M1120", 0);
            axActUtlType1.SetDevice("M1121", 0);
            axActUtlType1.SetDevice("M1122", 0);
            axActUtlType1.SetDevice("M1123", 0);
            axActUtlType1.SetDevice("M1124", 0);
            axActUtlType1.SetDevice("M1125", 0);
            axActUtlType1.SetDevice("M1126", 0);
            axActUtlType1.SetDevice("M1127", 0);
            axActUtlType1.SetDevice("M1128", 0);
            axActUtlType1.SetDevice("M1129", 0);
            axActUtlType1.SetDevice("M1130", 0);
            axActUtlType1.SetDevice("M1131", 0);
            axActUtlType1.SetDevice("M1132", 0);
            axActUtlType1.SetDevice("M1133", 0);
            axActUtlType1.SetDevice("M1134", 0);
            axActUtlType1.SetDevice("M1135", 0);
            axActUtlType1.SetDevice("M1136", 0);
            axActUtlType1.SetDevice("M1137", 0);
            axActUtlType1.SetDevice("M1138", 0);
            axActUtlType1.SetDevice("M1139", 0);
            axActUtlType1.SetDevice("M1140", 0);

            axActUtlType1.SetDevice("M1200", 0);
            axActUtlType1.SetDevice("M1201", 0);
            axActUtlType1.SetDevice("M1202", 0);
            axActUtlType1.SetDevice("M1203", 0);
            axActUtlType1.SetDevice("M1204", 0);
            axActUtlType1.SetDevice("M1205", 0);
            axActUtlType1.SetDevice("M1206", 0);
            axActUtlType1.SetDevice("M1207", 0);
            axActUtlType1.SetDevice("M1208", 0);
            axActUtlType1.SetDevice("M1209", 0);
            axActUtlType1.SetDevice("M1210", 0);
            axActUtlType1.SetDevice("M1211", 0);
            axActUtlType1.SetDevice("M1212", 0);
            axActUtlType1.SetDevice("M1213", 0);
            axActUtlType1.SetDevice("M1214", 0);
            axActUtlType1.SetDevice("M1215", 0);
            axActUtlType1.SetDevice("M1216", 0);
            axActUtlType1.SetDevice("M1217", 0);
            axActUtlType1.SetDevice("M1218", 0);
            axActUtlType1.SetDevice("M1219", 0);
            axActUtlType1.SetDevice("M1220", 0);
            axActUtlType1.SetDevice("M1221", 0);
            axActUtlType1.SetDevice("M1222", 0);
            axActUtlType1.SetDevice("M1223", 0);
            axActUtlType1.SetDevice("M1224", 0);
            axActUtlType1.SetDevice("M1225", 0);
            axActUtlType1.SetDevice("M1226", 0);
            axActUtlType1.SetDevice("M1227", 0);
            axActUtlType1.SetDevice("M1228", 0);
            axActUtlType1.SetDevice("M1229", 0);
            axActUtlType1.SetDevice("M1230", 0);

            //for A+改掉
            axActUtlType1.SetDevice("M1040", 0);
            axActUtlType1.SetDevice("M1004", 0);
            axActUtlType1.SetDevice("M1041", 0);
            axActUtlType1.SetDevice("M1005", 0);
            axActUtlType1.SetDevice("M1006", 0);
            axActUtlType1.SetDevice("M1017", 0);
            axActUtlType1.SetDevice("M1018", 0);
            axActUtlType1.SetDevice("M1042", 0);
            axActUtlType1.SetDevice("M1043", 0);
            axActUtlType1.SetDevice("M1007", 0);
            axActUtlType1.SetDevice("M1140", 0);
            axActUtlType1.SetDevice("M1142", 0);
            axActUtlType1.SetDevice("M1141", 0);

            axActUtlType1.SetDevice("M1043", 0);

            axActUtlType1.SetDevice("M1044", 0);
            axActUtlType1.SetDevice("M1045", 0);
            //axActUtlType1.SetDevice("M1215", 0);
            axActUtlType1.SetDevice("M1156", 0);
            //axActUtlType1.SetDevice("M1217", 0);
            //axActUtlType1.SetDevice("M1218", 0);
            //axActUtlType1.SetDevice("M1219", 0);
            axActUtlType1.SetDevice("M1046", 0);
            //axActUtlType1.SetDevice("M1221", 0);
            //axActUtlType1.SetDevice("M1222", 0);
            //axActUtlType1.SetDevice("M1223", 0);
            axActUtlType1.SetDevice("M1049", 0);
            //axActUtlType1.SetDevice("M1225", 0);
            axActUtlType1.SetDevice("M1048", 0);
            axActUtlType1.SetDevice("M1016", 0);
            //axActUtlType1.SetDevice("M1228", 0);
            //axActUtlType1.SetDevice("M1229", 0);
            //axActUtlType1.SetDevice("M1230", 0);

        }
        public void PLC_AllInitial()
        {
            PLC_AllClear();

            ////初始化所有平台動作指令
            ////X軸相關
            //axActUtlType1.SetDevice("M1000", 0);
            //axActUtlType1.SetDevice("M1001", 0);
            //axActUtlType1.SetDevice("M1002", 0);
            //axActUtlType1.SetDevice("M1003", 0);
            ////axActUtlType1.SetDevice("M1100", 0);
            ////axActUtlType1.SetDevice("M1101", 0);
            ////axActUtlType1.SetDevice("M1102", 0);
            //axActUtlType1.SetDevice("M1103", 0);
            ////axActUtlType1.SetDevice("M1105", 0);
            ////axActUtlType1.SetDevice("M1106", 0);
            ////axActUtlType1.SetDevice("M1107", 0);
            ////axActUtlType1.SetDevice("M1108", 0);

            ////Y軸相關
            //axActUtlType1.SetDevice("M1010", 0);
            //axActUtlType1.SetDevice("M1011", 0);
            //axActUtlType1.SetDevice("M1012", 0);
            //axActUtlType1.SetDevice("M1013", 0);
            ////axActUtlType1.SetDevice("M1110", 0);
            ////axActUtlType1.SetDevice("M1111", 0);
            ////axActUtlType1.SetDevice("M1112", 0);
            //axActUtlType1.SetDevice("M1113", 0);
            ////axActUtlType1.SetDevice("M1115", 0);
            ////axActUtlType1.SetDevice("M1116", 0);
            ////axActUtlType1.SetDevice("M1117", 0);
            ////axActUtlType1.SetDevice("M1118", 0);

            ////Z軸相關
            //axActUtlType1.SetDevice("M1020", 0);
            //axActUtlType1.SetDevice("M1021", 0);
            //axActUtlType1.SetDevice("M1022", 0);
            //axActUtlType1.SetDevice("M1023", 0);
            ////axActUtlType1.SetDevice("M1120", 0);
            ////axActUtlType1.SetDevice("M1121", 0);
            ////axActUtlType1.SetDevice("M1122", 0);
            //axActUtlType1.SetDevice("M1123", 0);
            ////axActUtlType1.SetDevice("M1125", 0);
            ////axActUtlType1.SetDevice("M1126", 0);
            ////axActUtlType1.SetDevice("M1127", 0);
            ////axActUtlType1.SetDevice("M1128", 0);
            ////reset & stop

            //axActUtlType1.SetDevice("M1200", 1);
            //axActUtlType1.SetDevice("M1202", 1);

            //while (true)
            //{

            //    double[] _ReadVal = ReadPLCDataRandom("M1210", 1);
            //    if (_ReadVal[0] == 1)
            //    {
            //        axActUtlType1.SetDevice("M1200", 0);
            //        axActUtlType1.SetDevice("M1202", 0);
            //        break;
            //    }
            //}


        }
        public void FlowMeasModuleIn()
        {
            while (true)
            {
                double[] _ReadVal1 = ReadPLCDataRandom("M1161", 1);
                if (_ReadVal1[0] == 1)
                {
                    Console.WriteLine("分料機2:可以準備入料了!M1161=1");
                    axActUtlType1.SetDevice("M1026", 1);
                    Console.WriteLine("已經通知分料機2可以入料了!");
                }
                break;

            }
            while (true)
            {
                double[] _ReadVal2 = ReadPLCDataRandom("M1162", 1);
                if (_ReadVal2[0] == 1)
                {
                    Console.WriteLine("分料機2:完成入料了!M1162=1");
                    break;
                }
            }
        }
        public void FlowMeasModuleOut()
        {
            while (true)
            {
                double[] _ReadVal1 = ReadPLCDataRandom("M1163", 1);
                if (_ReadVal1[0] == 1)
                {
                    Console.WriteLine("分料機3:可以準備出料了!M1163=1");
                    axActUtlType1.SetDevice("M1027", 1);
                    Console.WriteLine("已經通知分料機3可以出料了!");
                }
                break;

            }
            while (true)
            {
                double[] _ReadVal2 = ReadPLCDataRandom("M1164", 1);
                if (_ReadVal2[0] == 1)
                {
                    Console.WriteLine("分料機3:完成出料了!M1164=1");
                    break;
                }
            }
        }
        public void FlowMeasInsertRepairData()
        {
            short[] XTarget = new short[2];


            //寫入X 軸目標位置
            XTarget = Int32Short(35, 10000);
            axActUtlType1.WriteDeviceBlock2("D2460", 2, ref XTarget[0]);

            double[] _ReadVal = ReadPLCDataRandom("D2460\nD2461", 2);
            //// hardcode 上面順序比較節省尋找時間
            double testVal = Math.Round(_ReadVal[0], 3);
            Console.WriteLine("捕銲資訊=" + testVal);



        }
        private void PLCAction_Load(object sender, EventArgs e)
        {

        }

        private void axActUtlType1_OnDeviceStatus(object sender, AxActUtlTypeLib._IActUtlTypeEvents_OnDeviceStatusEvent e)
        {

        }
    }
}
