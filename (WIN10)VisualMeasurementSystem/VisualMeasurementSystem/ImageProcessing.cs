using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.UI;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;



namespace VisualMeasurementSystem
{

    class ImageProcessing
    {
        //CCD 連續拍攝記憶體保護寫法相關宣告
        private Bitmap TemplateBitmap1 = null;
        public object imageLock = new object();



        BitmapDataFast BMPFast = new BitmapDataFast();

        public Bitmap TemplateBMP
        {
            get
            {
                lock (imageLock)
                {
                    if (TemplateBitmap1 == null)
                    {
                        return new Bitmap(10, 10);
                    }
                    else
                    {
                        return (Bitmap)TemplateBitmap1.Clone();
                    }

                }
            }

            private set
            {
                lock (imageLock)
                {
                    Bitmap tmp = TemplateBitmap1;
                    TemplateBitmap1 = null;
                    TemplateBitmap1 = value;
                    if (tmp != null)
                    {
                        //shotBitmap1.Dispose();
                        tmp.Dispose();
                    }
                }
            }
        }


        public Bitmap FormatNormalize(Bitmap bmp)
        {
            Bitmap Formatmap;
            Bitmap FileBitmap = new Bitmap(bmp);

            int PF = -1;
            PixelFormat[] pixelFormatArray = {
                                            PixelFormat.Format1bppIndexed
                                            ,PixelFormat.Format4bppIndexed
                                            ,PixelFormat.Format8bppIndexed
                                            ,PixelFormat.Undefined
                                            ,PixelFormat.DontCare
                                            ,PixelFormat.Format16bppArgb1555
                                            ,PixelFormat.Format16bppGrayScale
                                        };
            foreach (PixelFormat pf in pixelFormatArray)
            {
                if (FileBitmap.PixelFormat == pf)
                {
                    PF = 1;
                    break;
                }

                else PF = 0;
            }

            if (PF == 1)
            {
                Formatmap = new Bitmap(FileBitmap.Width, FileBitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(Formatmap))
                {
                    g.DrawImage(FileBitmap, 0, 0);
                }
            }

            else Formatmap = new Bitmap(FileBitmap);
            return Formatmap;
        }
        public Bitmap GrayLevel(Bitmap bmp)
        {
            Grayscale grayscaleFilter = new Grayscale(0.299, 0.587, 0.114);
            Bitmap GrayBitmap = grayscaleFilter.Apply(bmp);
            return GrayBitmap;
        }
        public Bitmap Erosion3x3(Bitmap bmp, int cnt)
        {

            Erosion3x3 EROfilter33 = new Erosion3x3();
            Bitmap EroBMP = bmp;
            for (int i = 0; i < cnt; i++)
                EroBMP = EROfilter33.Apply(EroBMP);
            return EroBMP;
        }
        public Bitmap LocalThreshold(Bitmap bmp)
        {
            BradleyLocalThresholding BLfilter = new BradleyLocalThresholding();
            Bitmap LocalThresholdBitmap = BLfilter.Apply(bmp);
            return LocalThresholdBitmap;
        }
        public List<PointF> BlobCircle(Bitmap bmp)
        {
            List<PointF> CircleCenterP = new List<PointF>();
            BlobCounter blobCounter = new BlobCounter();
            BlobsFiltering Blobfilter = new BlobsFiltering();
            // configure filter
            Blobfilter.CoupledSizeFiltering = true;
            Blobfilter.MinWidth = 30;
            Blobfilter.MinHeight = 30;
            Blobfilter.MaxHeight = 70;
            Blobfilter.MaxWidth = 70;

            // apply the filter
            Blobfilter.ApplyInPlace(bmp);

            BitmapData bitmapData = bmp.LockBits(
             new Rectangle(0, 0, bmp.Width, bmp.Height),
             ImageLockMode.ReadWrite, bmp.PixelFormat);


            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            bmp.UnlockBits(bitmapData);

            CircleCenterP = new List<PointF>();

            PointF CircleCenter;
            foreach (Blob b in blobs)
            {

                //if (Math.Abs(b.Rectangle.Width - b.Rectangle.Height) <= 20
                //      && b.Area > 1000 && b.Area < 6800
                //      && b.Rectangle.Width >= 40
                //    )//1F
                if (Math.Abs(b.Rectangle.Width - b.Rectangle.Height) <= 20
                     && b.Area > 900 && b.Area < 6800
                   && b.Rectangle.Width >= 30
                    )//2F
                {
                    CircleCenter = new PointF();

                    CircleCenter.X = b.CenterOfGravity.X;
                    CircleCenter.Y = b.CenterOfGravity.Y;
                    CircleCenterP.Add(CircleCenter);
                }
            }

            return CircleCenterP;
        }
        public List<PointF> RemoveBlackNoise(List<PointF> BlobCenter, int[, ,] bmpdata, int w, int h)
        {
            //去除黑點模式
            List<PointF> NewBlobCenterList = new List<PointF>();
            int CenterAvg = 0, AvgCnt = 0;
            List<PointF> BlackCenterP = new List<PointF>();

            foreach (PointF cp in BlobCenter)
            {
                CenterAvg = 0;
                AvgCnt = 0;
                for (int j = -30; j < 30; j++)
                    for (int i = -30; i < 30; i++)
                    {
                        if ((cp.X + i) > 0 && (cp.Y + j) > 0 && (cp.X + i) < w && (cp.Y + j) < h)
                        {
                            CenterAvg += (bmpdata[(int)cp.X + i, (int)cp.Y + j, 0] + bmpdata[(int)cp.X + i, (int)cp.Y + j, 1] + bmpdata[(int)cp.X + i, (int)cp.Y + j, 2]) / 3;
                            AvgCnt++;
                        }
                    }
                CenterAvg = CenterAvg / AvgCnt;
                if (CenterAvg < 70 && CenterAvg != 0)
                {
                    BlackCenterP.Add(cp);
                }
            }

            foreach (PointF p in BlobCenter)
            {
                if (!(BlackCenterP.Exists(Np => Np == p)))
                {
                    NewBlobCenterList.Add(p);

                }
            }


            return NewBlobCenterList;
        }
        public List<PointF> RemoveDistNoise(List<PointF> BlobCenter, int GridLenth, int rownum)
        {
            List<PointF> NewBlobCenterList = new List<PointF>();
            List<PointF> DistNoiseList = new List<PointF>();
            foreach (PointF p in BlobCenter)
            {
                int NoiseFlag = 0;
                PointF pF = new PointF();
                pF.X = p.X;
                pF.Y = p.Y;

                //往左右尋找
                for (int x = -rownum + 1; x < rownum; x++) //-5~6
                {
                    foreach (PointF pp in BlobCenter)
                    {
                        if (!(pF.X == pp.X && pF.Y == pp.Y))
                        {
                            if (Math.Abs((pF.X + (GridLenth * x) - pp.X)) <= 80
                                && Math.Abs((pF.Y - pp.Y)) <= 80)

                                NoiseFlag++;

                        }


                    }
                }

                //往上下尋找

                for (int y = -rownum + 1; y < rownum; y++)
                {
                    foreach (PointF pp in BlobCenter)
                    {
                        if (!(pF.X == pp.X && pF.Y == pp.Y))
                        {
                            if (Math.Abs((pF.X - pp.X)) <= 80
                                && Math.Abs((pF.Y + (GridLenth * y) - pp.Y)) <= 80)

                                NoiseFlag++;

                        }
                    }
                }



                if (NoiseFlag == 0)
                {
                    DistNoiseList.Add(pF);
                    Console.WriteLine("是雜訊");
                }
            }

            foreach (PointF p in BlobCenter)
            {
                if (!(DistNoiseList.Exists(Np => Np == p)))
                {
                    NewBlobCenterList.Add(p);

                }
            }


            return NewBlobCenterList;
        }
        public int GridLength(List<PointF> GoodMapList)
        {
            int GridLength = 0;
            double ShortDist = 100000000;
            double ShortDistAvg = 0;
            int AvgCnt = 0;
            double dist;
            foreach (PointF p1 in GoodMapList)
            {
                ShortDist = 100000;

                foreach (PointF p2 in GoodMapList)
                {
                    if (p1 != p2)
                    {
                        dist = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                        if (dist < ShortDist && dist > 200 * 200 && dist < 300 * 300) ShortDist = dist;  //橫向比較短直向比較長所以放寬range
                    }
                }
                ShortDistAvg += ShortDist;
                AvgCnt++;
            }

            ShortDistAvg = Math.Sqrt(ShortDistAvg / AvgCnt);
            GridLength = (int)ShortDistAvg;//+ 5;
            Console.WriteLine("GridLength =" + GridLength);

            return GridLength;
        }
        public List<List<PointF>> LineClassify(List<PointF> GoodMapList, int GridLength)
        {
            List<List<PointF>> LineCollect = new List<List<PointF>>();
            List<PointF> GridLine = new List<PointF>();
            for (int i = 0; i < 6; i++)
                LineCollect.Add(GridLine);

            for (int i = 0; i < 6; i++)
                LineCollect[i] = new List<PointF>();

            double MinY = 10000000, MinX = 10000000;

            foreach (PointF p in GoodMapList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面

                // if (MinX > p.X) MinX = p.X;
                if (MinY > p.Y) MinY = p.Y;

            }

            //歸線&排序

            foreach (PointF p in GoodMapList)
            {
                //1F
                //if (Math.Abs((p.Y - MinY)) <= 20)//第一條橫線
                //    LineCollect[0].Add(p);
                //else if (Math.Abs((p.Y - (MinY + GridLength))) <= 50)//第2條橫線
                //    LineCollect[1].Add(p);
                //else if (Math.Abs((p.Y - (MinY + GridLength * 2))) <= 50)//第2條橫線
                //    LineCollect[2].Add(p);

                //else if (Math.Abs((p.Y - (MinY + GridLength * 3))) <= 50)//第2條橫線
                //    LineCollect[3].Add(p);

                //else if (Math.Abs((p.Y - (MinY + GridLength * 4))) <= 50)//第2條橫線
                //    LineCollect[4].Add(p);

                //else if (Math.Abs((p.Y - (MinY + GridLength * 5))) <= 50)//第2條橫線
                //    LineCollect[5].Add(p);


                //2F
                if (Math.Abs((p.Y - MinY)) <= 20)//第一條橫線
                    LineCollect[0].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength))) <= 50)//第2條橫線
                    LineCollect[1].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength * 2))) <= 50)//第2條橫線
                    LineCollect[2].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 3))) <= 50)//第2條橫線
                    LineCollect[3].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 4))) <= 80)//第2條橫線
                    LineCollect[4].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 5))) <= 100)//第2條橫線
                    LineCollect[5].Add(p);

            }
            for (int k = 0; k < 6; k++)
            {
                LineCollect[k].Sort((p1, p2) => p1.X.CompareTo(p2.X));
                //Console.WriteLine("LineCollect" + k + "=" + LineCollect[k].Count());
            }
            return LineCollect;

        }


        public List<List<PointF>> ColumnClassify(List<PointF> GoodMapList, int GridLength)
        {
            List<List<PointF>> ColumnCollect = new List<List<PointF>>();
            List<PointF> GridColumn = new List<PointF>();
            for (int i = 0; i < 6; i++)
                ColumnCollect.Add(GridColumn);

            for (int i = 0; i < 6; i++)
                ColumnCollect[i] = new List<PointF>();

            double MinY = 10000000, MinX = 10000000;

            foreach (PointF p in GoodMapList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面

                if (MinX > p.X) MinX = p.X;
                //if (MinY > p.Y) MinY = p.Y;

            }


            foreach (PointF p in GoodMapList)
            {
                //1F
                //if (Math.Abs((p.X - MinX)) <= 20)//第一條直線
                //    ColumnCollect[0].Add(p);
                //else if (Math.Abs((p.X - (MinX + GridLength))) <= 50)//第2條直線
                //    ColumnCollect[1].Add(p);
                //else if (Math.Abs((p.X - (MinX + GridLength * 2))) <= 50)//第2條直線
                //    ColumnCollect[2].Add(p);

                //else if (Math.Abs((p.X - (MinX + GridLength * 3))) <= 50)//第2條直線
                //    ColumnCollect[3].Add(p);

                //else if (Math.Abs((p.X - (MinX + GridLength * 4))) <= 50)//第2條直線
                //    ColumnCollect[4].Add(p);

                //else if (Math.Abs((p.X - (MinX + GridLength * 5))) <= 50)//第2條直線
                //    ColumnCollect[5].Add(p);
                //2F
                //2F
                if (Math.Abs((p.X - MinX)) <= 20)//第一條橫線
                    ColumnCollect[0].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength))) <= 50)//第2條橫線
                    ColumnCollect[1].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength * 2))) <= 50)//第2條橫線
                    ColumnCollect[2].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 3))) <= 50)//第2條橫線
                    ColumnCollect[3].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 4))) <= 80)//第2條橫線
                    ColumnCollect[4].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 5))) <= 100)//第2條橫線
                    ColumnCollect[5].Add(p);
            }
            for (int k = 0; k < 6; k++)
            {
                ColumnCollect[k].Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
                //  Console.WriteLine("ColumnCollect" + k + "=" + ColumnCollect[k].Count());
            }
            return ColumnCollect;
        }
        public Bitmap TemplateSet(int TempSize)
        {

            //------------------------------------------------------
            //1. TEMPLATE訂定
            //------------------------------------------------------
            Bitmap TemplateBMP = new Bitmap(TempSize + 8, TempSize + 8);
            Graphics g2 = Graphics.FromImage(TemplateBMP);
            Pen BlackPen = new Pen(Color.Black, 1);
            Pen RedPen = new Pen(Color.Red, 5);
            g2.Clear(Color.White);
            g2.DrawEllipse(BlackPen, 5, 5, TempSize, TempSize);



            return TemplateBMP;

        }
        public WeldCenterInfo[,] TemplateMapping(int[, ,] ProcessData, List<System.Drawing.Point> TemplateEdgeList, PointF[,] MapGrid, List<PointF> GoodWeldP, int GridNum,
            out List<System.Drawing.Point> _TempEdgeDraw)
        {
            WeldCenterInfo[,] WeldCenterList = new WeldCenterInfo[GridNum, GridNum];
            _TempEdgeDraw = new List<System.Drawing.Point>();


            for (int j = 0; j < GridNum; j++)
                for (int i = 0; i < GridNum; i++)
                    WeldCenterList[i, j] = new WeldCenterInfo();
            //    template 掃描, 取黑點符合 template 最高分的點位
            int EdgeCnt = 0;
            //    System.Drawing.Point MaxCandidte = new System.Drawing.Point();
            //    List<System.Drawing.Point> MaxCandidteList = new List<System.Drawing.Point>();  //最後找出來的最佳解銲道圓心點

            WeldCenterInfo MaxCandidte;
            int MaxScore = 0;
            //    //---------------------------------------
            for (int mmj = 0; mmj < GridNum; mmj++)
                for (int mmi = 0; mmi < GridNum; mmi++)
                {
                    MaxCandidte = new WeldCenterInfo();
                    MaxCandidte.WeldScore = 0;
                    MaxScore = 0;
                    if (!GoodWeldP.Exists(p => p.X == MapGrid[mmi, mmj].X) && !(GoodWeldP.Exists(p => p.Y == MapGrid[mmi, mmj].Y)))
                    //沒有在理想list的網格點
                    {


                        for (int j = (int)(MapGrid[mmi, mmj].Y - 15); j < (int)(MapGrid[mmi, mmj].Y + 15); j++)
                        {
                            for (int i = (int)(MapGrid[mmi, mmj].X - 15); i < (int)(MapGrid[mmi, mmj].X + 15); i++)
                            {


                                EdgeCnt = 0;

                                //算出每個點專屬的EGDE範圍 
                                foreach (System.Drawing.Point p in TemplateEdgeList)//每一個 template 的 edge 點
                                {

                                    if (ProcessData[p.X + (i - 37), p.Y + (j - 37), 0] == 0 && //(i,j) 各網格中心點
                                        ProcessData[p.X + (i - 37), p.Y + (j - 37), 1] == 0 &&
                                        ProcessData[p.X + (i - 37), p.Y + (j - 37), 2] == 0) //TEMPLATE edge 點位移至當前位置, 判斷是否也為邊緣點(黑色)
                                    {
                                        EdgeCnt++;

                                        //debug用
                                        //System.Drawing.Point ppp = new System.Drawing.Point();
                                        //ppp.X = p.X + (i - 37);
                                        //ppp.Y = p.Y + (j - 37);
                                        //_TempEdgeDraw.Add(ppp);
                                    }


                                }

                                if (EdgeCnt > MaxScore && EdgeCnt > TemplateEdgeList.Count() * 0.9)//一半以上銲道點符合
                                {
                                    //Console.WriteLine("mmi=" + mmi + ", mmj=" + mmj);
                                    //Console.WriteLine("EdgeCnt=" + EdgeCnt);
                                    //Console.WriteLine("TemplateEdgeList.Count()=" + TemplateEdgeList.Count());


                                    MaxScore = EdgeCnt;
                                    MaxCandidte.WeldScore = EdgeCnt;
                                    MaxCandidte.WeldCandidate.X = i;
                                    MaxCandidte.WeldCandidate.Y = j;

                                }




                            }//FOR I
                        }//FOR J


                        if (!(MaxCandidte.WeldCandidate.X == MapGrid[mmi, mmj].X && MaxCandidte.WeldCandidate.Y == MapGrid[mmi, mmj].Y))
                        {

                            //MaxCandidte.MapIdx.X = (int)Math.Floor((double)mm / 6);
                            //MaxCandidte.MapIdx.Y = mm % 6;
                            WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                            WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;
                            WeldCenterList[mmi, mmj].WeldScore = MaxCandidte.WeldScore;
                            //      Console.WriteLine("Bad:" + WeldCenterList[mmi, mmj].WeldCandidate.X + "," + WeldCenterList[mmi, mmj].WeldCandidate.Y + ":" + WeldCenterList[mmi, mmj].WeldScore);

                        }

                    }//end if not goodlist

                    else //理想銲道則不用尋找,直接用原本的
                    {
                        MaxCandidte.WeldScore = 100000;
                        MaxCandidte.WeldCandidate.X = (int)MapGrid[mmi, mmj].X;
                        MaxCandidte.WeldCandidate.Y = (int)MapGrid[mmi, mmj].Y;
                        WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                        WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;
                        WeldCenterList[mmi, mmj].WeldScore = MaxCandidte.WeldScore;
                        //  Console.WriteLine("Good:" + WeldCenterList[mmi, mmj].WeldCandidate.X + "," + WeldCenterList[mmi, mmj].WeldCandidate.Y + ":" + WeldCenterList[mmi, mmj].WeldScore);

                    }


                    if (MaxCandidte.WeldScore == 0)
                    {
                        MaxCandidte.WeldCandidate.X = (int)MapGrid[mmi, mmj].X;
                        MaxCandidte.WeldCandidate.Y = (int)MapGrid[mmi, mmj].Y;
                        WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                        WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;

                    }
                }//for map


            return WeldCenterList;
        }

        public Bitmap OstuThresh(Bitmap OldBMP, int[, ,] OtsuBmpArr)
        {
            Bitmap ProcessBMP = new Bitmap(OldBMP);

            int GrayThresh = 20;

            int w = OldBMP.Width;
            int h = OldBMP.Height;

            //int[, ,] OtsuBmpArr = new int[w, h, 3];
            //OtsuBmpArr = BMPFast.GetRGBData(OldBMP);

            int[] pixelCount = new int[GrayThresh];
            double[] pixelPro = new double[GrayThresh];

            for (int i = 0; i < GrayThresh; i++)
            {
                pixelCount[i] = 0;
                pixelPro[i] = 0;
            }
            //統計灰度級中每個像素在整幅圖像中的個數
            int cnt = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if ((OtsuBmpArr[i, j, 0] >= GrayThresh))
                        cnt++;
                    else
                        pixelCount[(int)OtsuBmpArr[i, j, 0]]++;
                }
            }

            //計算每個像素在整幅圖像中的比例
            for (int i = 0; i < GrayThresh; i++)
            {

                pixelPro[i] = ((double)pixelCount[i] / (double)(w * h - cnt));
            }



            //遍歷灰度級[0,255 ]
            double w0, w1, u0tmp, u1tmp, u0, u1, u,
          deltaTmp, deltaMax = 0;

            int thresholdval = 0;

            for (int i = 0; i < GrayThresh; i++)
            {
                w0 = w1 = u0tmp = u1tmp = u0 = u1 = u = deltaTmp = 0;
                for (int j = 0; j < GrayThresh; j++)
                {
                    if (j <= i) //背景部分
                    {
                        w0 += pixelPro[j];
                        u0tmp += j * pixelPro[j];
                    }
                    else //前景部分
                    {
                        w1 += pixelPro[j];
                        u1tmp += j * pixelPro[j];
                    }
                }
                u0 = u0tmp / w0;
                u1 = u1tmp / w1;
                u = u0tmp + u1tmp;
                deltaTmp =
                   w0 * Math.Pow((u0 - u), 2) + w1 * Math.Pow((u1 - u), 2);
                if (deltaTmp > deltaMax)
                {
                    deltaMax = deltaTmp;
                    thresholdval = i;
                }
            }
            Image<Gray, Byte> src = new Image<Gray, byte>(OldBMP); //Image Class from Emgu.CV
            Image<Gray, Byte> dst = new Image<Gray, byte>(OldBMP);
            //Console.WriteLine("thresholdval=" + thresholdval);
            //int thresholdval = 13;
            CvInvoke.Threshold(src, dst, thresholdval, 255, ThresholdType.Binary);
            ProcessBMP = dst.ToBitmap();




            return ProcessBMP;
        }
        public List<PointF> BlackTrayHolePosforPerm(Bitmap OldBMP)
        {
            List<PointF> BlackTrayHoleP = new List<PointF>();

            int[, ,] ProcessArr = new int[OldBMP.Width, OldBMP.Height, 3];
            ProcessArr = BMPFast.GetRGBData(OldBMP);

            List<WeldCenterInfo> AreaList = new List<WeldCenterInfo>();
            WeldCenterInfo PP = new WeldCenterInfo();

            //----------------------(一)---------------------------
            // 影像正中間開始找粗位置(左上角的洞太多雜訊) 
            // 1024,1024 左上角 250,250 內的區域
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < 5; i++)
                {
                    PP = new WeldCenterInfo();
                    //249,1717: 矩陣的左下角第一點 //B面
                    PP.WeldCandidate.X = 249 - 50 * i;
                    PP.WeldCandidate.Y = 1717 - 50 * j;

                    //300,1725: 矩陣的左下角第一點 //A面
                    PP.WeldCandidate.X = 310 - 50 * i;
                    PP.WeldCandidate.Y = 1725 - 50 * j;


                    //PP.WeldCandidate.X = 250 - 50 * i;
                    //PP.WeldCandidate.Y = 250 - 50 * j;
                    AreaList.Add(PP);
                }

            int count = 0;
            int BlackCnt = 0;

            foreach (WeldCenterInfo pp in AreaList)
            {
                BlackCnt = 0;
                for (int y = (int)pp.WeldCandidate.Y; y < pp.WeldCandidate.Y + 50; y++)
                {
                    for (int x = (int)pp.WeldCandidate.X; x < pp.WeldCandidate.X + 50; x++)
                    {
                        if (ProcessArr[x, y, 0] == 0)
                            BlackCnt++;
                    }

                }

                AreaList[count].WeldScore = BlackCnt;
                count++;
            }



            PointF CoarseCenter = new PointF();
            count = 0;
            foreach (WeldCenterInfo pp in AreaList)
            {

                //  g2.FillEllipse(RedBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
                if (pp.WeldScore > 40 * 40 * 0.2) //分數前幾名的區域取平均
                {
                    count++;
                    //g2.FillEllipse(BlueBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
                    CoarseCenter.X += (pp.WeldCandidate.X + 25);
                    CoarseCenter.Y += (pp.WeldCandidate.Y + 25);
                }

            }
            CoarseCenter.X = CoarseCenter.X / count;
            CoarseCenter.Y = CoarseCenter.Y / count;


            //----------------------(二)---------------------------
            //找框選區域中的黑洞準確圓心位置
            BlackCnt = 0;
            int MaxBlackCnt = 0;
            PointF MaxP = new PointF();
            int LOWboundX = (int)CoarseCenter.X - 40;
            int UPboundX = (int)CoarseCenter.X + 40;
            int LOWboundY = (int)CoarseCenter.Y - 40;
            int UPboundY = (int)CoarseCenter.Y + 40;

            for (int n = LOWboundY; n < UPboundY; n++)
                for (int m = LOWboundX; m < UPboundX; m++)
                {
                    BlackCnt = 0;
                    for (int j = n - 45; j < n + 45; j++)
                        for (int i = m - 45; i < m + 45; i++)
                        {
                            if (ProcessArr[i, j, 0] == 0)
                                BlackCnt++;
                        }
                    if (BlackCnt > 45 * 45 * 0.3 && MaxBlackCnt < BlackCnt)
                    {
                        MaxBlackCnt = BlackCnt;
                        MaxP.X = m;
                        MaxP.Y = n;

                    }
                }


            //-------------------------------------------------------------------------------------------------------
            PointF pf = new PointF();

            //回推左上角第一個黑洞在哪邊
            PointF FirstBlackHole = new PointF();
            FirstBlackHole.X = MaxP.X;
            FirstBlackHole.Y = MaxP.Y - 277 * 5;

            for (int j = -1; j < 6; j++)
                for (int i = -1; i < 6; i++)
                {
                    pf = new PointF();

                    //g1.FillEllipse(BlueBrush, FirstBlackHole.X + 277 * i - 15, FirstBlackHole.Y + 277 * j - 15, 30, 30);
                    pf.X = FirstBlackHole.X + 277 * i + 142;
                    pf.Y = FirstBlackHole.Y + 277 * j + 137;
                    BlackTrayHoleP.Add(pf);

                }



            BlackTrayHoleP.Add(FirstBlackHole);
            return BlackTrayHoleP;


        }
        public List<PointF> BlackTrayHolePos(Bitmap OldBMP)
        {
            List<PointF> BlackTrayHoleP = new List<PointF>();

            int[, ,] ProcessArr = new int[OldBMP.Width, OldBMP.Height, 3];
            ProcessArr = BMPFast.GetRGBData(OldBMP);

            List<WeldCenterInfo> AreaList = new List<WeldCenterInfo>();
            WeldCenterInfo PP = new WeldCenterInfo();

            //----------------------(一)---------------------------
            // 影像正中間開始找粗位置(左上角的洞太多雜訊) 
            // 1024,1024 左上角 250,250 內的區域
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < 5; i++)
                {
                    PP = new WeldCenterInfo();
                    PP.WeldCandidate.X = 1024 - 50 * i;
                    PP.WeldCandidate.Y = 1024 - 50 * j;
                    AreaList.Add(PP);
                }

            int count = 0;
            int BlackCnt = 0;

            foreach (WeldCenterInfo pp in AreaList)
            {
                BlackCnt = 0;
                for (int y = (int)pp.WeldCandidate.Y; y < pp.WeldCandidate.Y + 50; y++)
                {
                    for (int x = (int)pp.WeldCandidate.X; x < pp.WeldCandidate.X + 50; x++)
                    {
                        if (ProcessArr[x, y, 0] == 0)
                            BlackCnt++;
                    }

                }

                AreaList[count].WeldScore = BlackCnt;
                count++;
            }



            PointF CoarseCenter = new PointF();
            count = 0;
            foreach (WeldCenterInfo pp in AreaList)
            {

                //  g2.FillEllipse(RedBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
                if (pp.WeldScore > 40 * 40 * 0.2) //分數前幾名的區域取平均
                {
                    count++;
                    //g2.FillEllipse(BlueBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
                    CoarseCenter.X += (pp.WeldCandidate.X + 25);
                    CoarseCenter.Y += (pp.WeldCandidate.Y + 25);
                }

            }
            CoarseCenter.X = CoarseCenter.X / count;
            CoarseCenter.Y = CoarseCenter.Y / count;


            //----------------------(二)---------------------------
            //找框選區域中的黑洞準確圓心位置
            BlackCnt = 0;
            int MaxBlackCnt = 0;
            PointF MaxP = new PointF();
            int LOWboundX = (int)CoarseCenter.X - 40;
            int UPboundX = (int)CoarseCenter.X + 40;
            int LOWboundY = (int)CoarseCenter.Y - 40;
            int UPboundY = (int)CoarseCenter.Y + 40;

            for (int n = LOWboundY; n < UPboundY; n++)
                for (int m = LOWboundX; m < UPboundX; m++)
                {
                    BlackCnt = 0;
                    for (int j = n - 45; j < n + 45; j++)
                        for (int i = m - 45; i < m + 45; i++)
                        {
                            if (ProcessArr[i, j, 0] == 0)
                                BlackCnt++;
                        }
                    if (BlackCnt > 45 * 45 * 0.3 && MaxBlackCnt < BlackCnt)
                    {
                        MaxBlackCnt = BlackCnt;
                        MaxP.X = m;
                        MaxP.Y = n;

                    }
                }
            //-------------------------------------------------------------------------------------------------------
            PointF pf = new PointF();

            for (int j = -3; j < 4; j++)
                for (int i = -3; i < 4; i++)
                {
                    pf = new PointF();

                    //g2.FillEllipse(BlueBrush, MaxP.X + 277 * i - 15, MaxP.Y + 277 * j - 15, 30, 30);
                    //g2.FillEllipse(RedBrush, MaxP.X + 277 * i + 142 - 15, MaxP.Y + 277 * j + 137 - 15, 30, 30);
                    pf.X = MaxP.X + 277 * i + 142;
                    pf.Y = MaxP.Y + 277 * j + 137;
                    BlackTrayHoleP.Add(pf);

                }



            return BlackTrayHoleP;
            //List<PointF> BlackTrayHoleP = new List<PointF>();

            //int[, ,] ProcessArr = new int[OldBMP.Width, OldBMP.Height, 3];
            //ProcessArr = BMPFast.GetRGBData(OldBMP);

            //List<WeldCenterInfo> AreaList = new List<WeldCenterInfo>();
            //WeldCenterInfo PP = new WeldCenterInfo();

            ////----------------------(一)---------------------------
            //// 影像正中間開始找粗位置(左上角的洞太多雜訊) 
            //// 1024,1024 左上角 250,250 內的區域
            //for (int j = 0; j < 5; j++)
            //    for (int i = 0; i < 5; i++)
            //    {
            //        PP = new WeldCenterInfo();
            //        //249,1717: 矩陣的左下角第一點
            //        PP.WeldCandidate.X = 249 - 50 * i;
            //        PP.WeldCandidate.Y = 1717 - 50 * j;
            //        //PP.WeldCandidate.X = 250 - 50 * i;
            //        //PP.WeldCandidate.Y = 250 - 50 * j;
            //        AreaList.Add(PP);
            //    }

            //int count = 0;
            //int BlackCnt = 0;

            //foreach (WeldCenterInfo pp in AreaList)
            //{
            //    BlackCnt = 0;
            //    for (int y = (int)pp.WeldCandidate.Y; y < pp.WeldCandidate.Y + 50; y++)
            //    {
            //        for (int x = (int)pp.WeldCandidate.X; x < pp.WeldCandidate.X + 50; x++)
            //        {
            //            if (ProcessArr[x, y, 0] == 0)
            //                BlackCnt++;
            //        }

            //    }

            //    AreaList[count].WeldScore = BlackCnt;
            //    count++;
            //}



            //PointF CoarseCenter = new PointF();
            //count = 0;
            //foreach (WeldCenterInfo pp in AreaList)
            //{

            //    //  g2.FillEllipse(RedBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
            //    if (pp.WeldScore > 40 * 40 * 0.2) //分數前幾名的區域取平均
            //    {
            //        count++;
            //        //g2.FillEllipse(BlueBrush, pp.WeldCandidate.X - 5, pp.WeldCandidate.Y - 5, 10, 10);
            //        CoarseCenter.X += (pp.WeldCandidate.X + 25);
            //        CoarseCenter.Y += (pp.WeldCandidate.Y + 25);
            //    }

            //}
            //CoarseCenter.X = CoarseCenter.X / count;
            //CoarseCenter.Y = CoarseCenter.Y / count;


            ////----------------------(二)---------------------------
            ////找框選區域中的黑洞準確圓心位置
            //BlackCnt = 0;
            //int MaxBlackCnt = 0;
            //PointF MaxP = new PointF();
            //int LOWboundX = (int)CoarseCenter.X - 40;
            //int UPboundX = (int)CoarseCenter.X + 40;
            //int LOWboundY = (int)CoarseCenter.Y - 40;
            //int UPboundY = (int)CoarseCenter.Y + 40;

            //for (int n = LOWboundY; n < UPboundY; n++)
            //    for (int m = LOWboundX; m < UPboundX; m++)
            //    {
            //        BlackCnt = 0;
            //        for (int j = n - 45; j < n + 45; j++)
            //            for (int i = m - 45; i < m + 45; i++)
            //            {
            //                if (ProcessArr[i, j, 0] == 0)
            //                    BlackCnt++;
            //            }
            //        if (BlackCnt > 45 * 45 * 0.3 && MaxBlackCnt < BlackCnt)
            //        {
            //            MaxBlackCnt = BlackCnt;
            //            MaxP.X = m;
            //            MaxP.Y = n;

            //        }
            //    }


            ////-------------------------------------------------------------------------------------------------------
            //PointF pf = new PointF();

            ////回推左上角第一個黑洞在哪邊
            //PointF FirstBlackHole = new PointF();
            //FirstBlackHole.X = MaxP.X;
            //FirstBlackHole.Y = MaxP.Y - 277 * 5;

            //for (int j = -1; j < 6; j++)
            //    for (int i = -1; i < 6; i++)
            //    {
            //        pf = new PointF();

            //        g1.FillEllipse(BlueBrush, FirstBlackHole.X + 277 * i - 15, FirstBlackHole.Y + 277 * j - 15, 30, 30);
            //        pf.X = MaxP.X + 277 * i + 142;
            //        pf.Y = MaxP.Y + 277 * j + 137;
            //        BlackTrayHoleP.Add(pf);

            //    }


            //BlackTrayHoleP.Add(FirstBlackHole);
            //return BlackTrayHoleP;
        }

        public Bitmap EroLocalThresh(Bitmap OldBMP)
        {
            //Bitmap ProcessBMP = new Bitmap(OldBMP);

            //Image<Bgr, byte> src = new Image<Bgr, byte>(OldBMP); //Image Class from Emgu.CV
            //Image<Bgr, byte> dst = new Image<Bgr, byte>(OldBMP);


            //Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross,
            //    new Size(3, 3), new System.Drawing.Point(-1, -1));

            //CvInvoke.Erode(src, dst, element, new System.Drawing.Point(-1, -1), 3,
            //    Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

            //src = dst;

            //CvInvoke.Erode(src, dst, element, new System.Drawing.Point(-1, -1), 3,
            //    Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));


            //ProcessBMP = dst.ToBitmap();

            //取得灰階影像

            //Image<Gray, byte> grayImage = new Image<Gray, byte>(ProcessBMP);
            //ProcessBMP = grayImage.Bitmap;
            //BradleyLocalThresholding BLfilter = new BradleyLocalThresholding();
            //ProcessBMP = BLfilter.Apply(ProcessBMP);


            //src = new Image<Bgr, byte>(ProcessBMP);
            //CvInvoke.Erode(src, dst, element, new System.Drawing.Point(-1, -1), 3,
            // Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

            //ProcessBMP = dst.Bitmap;
            //ProcessBitmap = dst.Bitmap;
            Bitmap ProcessBMP = new Bitmap(OldBMP);

            Image<Gray, byte> grayImage = new Image<Gray, byte>(ProcessBMP);
            ProcessBMP = grayImage.Bitmap;

            // create filter
            SobelEdgeDetector filter = new SobelEdgeDetector();
            // apply the filter
            filter.ApplyInPlace(ProcessBMP);


            // create filter
            OtsuThreshold OtsuFilter = new OtsuThreshold();
            // apply the filter
            OtsuFilter.ApplyInPlace(ProcessBMP);
            // check threshold value
            int t = OtsuFilter.ThresholdValue;


            Image<Gray, Byte> src = new Image<Gray, byte>(ProcessBMP); //Image Class from Emgu.CV
            Image<Gray, Byte> dst = new Image<Gray, byte>(ProcessBMP);
            CvInvoke.Threshold(src, dst, t, 255, ThresholdType.Binary);
            ProcessBMP = dst.ToBitmap();



            Invert InvFilter = new Invert();
            // apply the filter
            InvFilter.ApplyInPlace(ProcessBMP);

            src = new Image<Gray, byte>(ProcessBMP); //Image Class from Emgu.CV
            dst = new Image<Gray, byte>(ProcessBMP);


            Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross,
                new Size(3, 3), new System.Drawing.Point(-1, -1));

            CvInvoke.Erode(src, dst, element, new System.Drawing.Point(-1, -1), 3,
                Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));


            ProcessBMP = dst.ToBitmap();
            src = dst;
            CvInvoke.Erode(src, dst, element, new System.Drawing.Point(-1, -1), 3,
               Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));

            ProcessBMP = dst.ToBitmap();

            return ProcessBMP;
        }

        public List<PointF> GoodWeldPos(Bitmap OldBMP, int[, ,] BlobBmpArr)
        {
            Bitmap ProcessBMP = new Bitmap(OldBMP);
            List<PointF> GoodWeldP = new List<PointF>();

            BlobCounter blobCounter = new BlobCounter();
            BlobsFiltering Blobfilter = new BlobsFiltering();
            // configure filter
            Blobfilter.CoupledSizeFiltering = true;
            Blobfilter.MinWidth = 20;
            Blobfilter.MinHeight = 20;
            Blobfilter.MaxHeight = 90;
            Blobfilter.MaxWidth = 90;
            Blobfilter.ApplyInPlace(ProcessBMP);

            BitmapData bitmapData = ProcessBMP.LockBits(
             new Rectangle(0, 0, ProcessBMP.Width, ProcessBMP.Height),
             ImageLockMode.ReadWrite, ProcessBMP.PixelFormat);


            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            ProcessBMP.UnlockBits(bitmapData);

            PointF CircleCenter;
            foreach (Blob b in blobs)
            {

                if (Math.Abs(b.Rectangle.Width - b.Rectangle.Height) <= 20
                     && b.Area > 60 && b.Area < 6800
                    && b.Rectangle.Width >= 30
                    )//2F
                {
                    CircleCenter = new PointF();

                    CircleCenter.X = b.CenterOfGravity.X;
                    CircleCenter.Y = b.CenterOfGravity.Y;

                    double GrayVal = (BlobBmpArr[(int)CircleCenter.X, (int)CircleCenter.Y, 0]
                                    + BlobBmpArr[(int)CircleCenter.X, (int)CircleCenter.Y, 1]
                                    + BlobBmpArr[(int)CircleCenter.X, (int)CircleCenter.Y, 2]) / 3;
                    if (GrayVal > 60)
                    {
                        GoodWeldP.Add(CircleCenter);
                        //    g.FillEllipse(BlueBrush, CircleCenter.X - 15, CircleCenter.Y - 15, 30, 30);
                    }
                }
            }

            return GoodWeldP;
        }



        public PointF[,] LineColGrid(List<PointF> TrayHoleList, List<PointF> GoodWeldList)
        {
            
            //------------------------------------------------1.先算間格長
            int GridLength = 0;
            double ShortDist = 100000000;
            double ShortDistAvg = 0;
            int AvgCnt = 0;
            double dist;
            foreach (PointF p1 in TrayHoleList)
            {
                ShortDist = 100000;

                foreach (PointF p2 in TrayHoleList)
                {
                    if (p1 != p2)
                    {
                        dist = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                        if (dist < ShortDist && dist > 200 * 200 && dist < 300 * 300) ShortDist = dist;  //橫向比較短直向比較長所以放寬range
                    }
                }
                ShortDistAvg += ShortDist;
                AvgCnt++;
            }

            ShortDistAvg = Math.Sqrt(ShortDistAvg / AvgCnt);
            GridLength = (int)ShortDistAvg;

            //------------------------------------------------2.橫排歸類
            List<List<PointF>> LineCollect = new List<List<PointF>>();
            List<PointF> GridLine = new List<PointF>();
            for (int i = 0; i < 10; i++)
                LineCollect.Add(GridLine);

            for (int i = 0; i < 10; i++)
                LineCollect[i] = new List<PointF>();

            double MinY = 10000000, MinX = 10000000;

            foreach (PointF p in TrayHoleList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面

                // if (MinX > p.X) MinX = p.X;
                if (MinY > p.Y) MinY = p.Y;

            }

            //歸線&排序

            foreach (PointF p in TrayHoleList)
            {

                //2F
                if (Math.Abs((p.Y - MinY)) <= 20)//第一條橫線
                    LineCollect[0].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength))) <= 25)//第2條橫線
                    LineCollect[1].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength * 2))) <= 30)//第2條橫線
                    LineCollect[2].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 3))) <= 35)//第2條橫線
                    LineCollect[3].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 4))) <= 40)//第2條橫線
                    LineCollect[4].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 5))) <= 45)//第2條橫線
                    LineCollect[5].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 6))) <= 50)//第2條橫線
                    LineCollect[6].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 7))) <= 50)//第2條橫線
                    LineCollect[7].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 8))) <= 50)//第2條橫線
                    LineCollect[8].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 9))) <= 50)//第2條橫線
                    LineCollect[9].Add(p);

            }
            for (int k = 0; k < 10; k++)
            {
                LineCollect[k].Sort((p1, p2) => p1.X.CompareTo(p2.X));
                //Console.WriteLine("LineCollect" + k + "=" + LineCollect[k].Count());
            }


            ////-------------------------------------------------------------------------------------


            List<List<PointF>> ColumnCollect = new List<List<PointF>>();
            List<PointF> GridColumn = new List<PointF>();
            for (int i = 0; i < 10; i++)
                ColumnCollect.Add(GridColumn);

            for (int i = 0; i < 10; i++)
                ColumnCollect[i] = new List<PointF>();



            foreach (PointF p in TrayHoleList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面
                if (MinX > p.X) MinX = p.X;
            }


            foreach (PointF p in TrayHoleList)//取十條
            {

                //2F
                if (Math.Abs((p.X - MinX)) <= 20)//第一條橫線
                    ColumnCollect[0].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength))) <= 20)//第2條橫線
                    ColumnCollect[1].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength * 2))) <= 25)//第2條橫線
                    ColumnCollect[2].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 3))) <= 35)//第2條橫線
                    ColumnCollect[3].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 4))) <= 40)//第2條橫線
                    ColumnCollect[4].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 5))) <= 45)//第2條橫線
                    ColumnCollect[5].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 6))) <= 50)//第2條橫線
                    ColumnCollect[6].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 7))) <= 50)//第2條橫線
                    ColumnCollect[7].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 8))) <= 50)//第2條橫線
                    ColumnCollect[8].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 9))) <= 50)//第2條橫線
                    ColumnCollect[9].Add(p);

            }
            for (int k = 0; k < 10; k++)
            {
                ColumnCollect[k].Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
                //  Console.WriteLine("ColumnCollect" + k + "=" + ColumnCollect[k].Count());
            }

            //-------------------------------------------------------------------------------------
            int GridNum = 10;
            PointF[,] MapGrid = new PointF[GridNum, GridNum];


            //1.6.5 by 線組合計算x座標平均做為爛銲道 x 座標y 座標

            int YAvg = 0;
            ////求每條線的 Y 平均
            for (int k = 0; k < LineCollect.Count(); k++)
            {
                YAvg = 0;
                if (LineCollect[k].Count() > 0)
                {

                    foreach (PointF p in LineCollect[k])
                        YAvg += (int)p.Y;

                    YAvg = YAvg / LineCollect[k].Count();

                    for (int i = 0; i < GridNum; i++)
                    {
                        if (YAvg != 0)
                            MapGrid[i, k].Y = YAvg;
                    }
                }
            }

            int XAvg = 0;
            //求每條線的 Y 平均
            for (int k = 0; k < ColumnCollect.Count(); k++)
            {
                XAvg = 0;
                if (ColumnCollect[k].Count() > 0)
                {

                    foreach (PointF p in ColumnCollect[k])
                        XAvg += (int)p.X;

                    XAvg = XAvg / ColumnCollect[k].Count();

                    for (int j = 0; j < GridNum; j++)
                    {
                        if (XAvg != 0)
                            MapGrid[k, j].X = XAvg;
                    }
                }
            }
            ////---------------------------填入好的銲道-----------------------------------------
            ShortDist = 100000000;
            dist = 0;
            PointF ShortP = new PointF();

            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 10; i++)
                {
                    ShortP = new PointF();
                    ShortDist = 100000000;
                    foreach (PointF pp in GoodWeldList)
                    {
                        dist = (MapGrid[i, j].X - pp.X) * (MapGrid[i, j].X - pp.X) + (MapGrid[i, j].Y - pp.Y) * (MapGrid[i, j].Y - pp.Y);
                        if (dist < 50 * 50 && dist < ShortDist)
                        {
                            ShortDist = dist;
                            ShortP.X = pp.X;
                            ShortP.Y = pp.Y;
                        }
                    }

                    if (ShortP.X == 0 && ShortP.Y == 0)
                    { }
                    else
                    {
                        MapGrid[i, j].X = ShortP.X;
                        MapGrid[i, j].Y = ShortP.Y;

                    }

                }


            return MapGrid;
        }
        public PointF[,] LineColGridforPerm(List<PointF> TrayHoleList, List<PointF> GoodWeldList)
        {

            //------------------------------------------------1.先算間格長
            int GridLength = 0;
            double ShortDist = 100000000;
            double ShortDistAvg = 0;
            int AvgCnt = 0;
            double dist;
            foreach (PointF p1 in TrayHoleList)
            {
                ShortDist = 100000;

                foreach (PointF p2 in TrayHoleList)
                {
                    if (p1 != p2)
                    {
                        dist = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                        if (dist < ShortDist && dist > 200 * 200 && dist < 300 * 300) ShortDist = dist;  //橫向比較短直向比較長所以放寬range
                    }
                }
                ShortDistAvg += ShortDist;
                AvgCnt++;
            }

            ShortDistAvg = Math.Sqrt(ShortDistAvg / AvgCnt);
            GridLength = (int)ShortDistAvg;

            //------------------------------------------------2.橫排歸類
            List<List<PointF>> LineCollect = new List<List<PointF>>();
            List<PointF> GridLine = new List<PointF>();
            for (int i = 0; i < 10; i++)
                LineCollect.Add(GridLine);

            for (int i = 0; i < 10; i++)
                LineCollect[i] = new List<PointF>();

            double MinY = 10000000, MinX = 10000000;

            foreach (PointF p in TrayHoleList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面

                // if (MinX > p.X) MinX = p.X;
                if (MinY > p.Y) MinY = p.Y;

            }

            //歸線&排序

            foreach (PointF p in TrayHoleList)
            {

                //2F
                if (Math.Abs((p.Y - MinY)) <= 20)//第一條橫線
                    LineCollect[0].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength))) <= 25)//第2條橫線
                    LineCollect[1].Add(p);
                else if (Math.Abs((p.Y - (MinY + GridLength * 2))) <= 30)//第2條橫線
                    LineCollect[2].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 3))) <= 35)//第2條橫線
                    LineCollect[3].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 4))) <= 40)//第2條橫線
                    LineCollect[4].Add(p);

                else if (Math.Abs((p.Y - (MinY + GridLength * 5))) <= 45)//第2條橫線
                    LineCollect[5].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 6))) <= 100)//第2條橫線
                    LineCollect[6].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 7))) <= 100)//第2條橫線
                    LineCollect[7].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 8))) <= 100)//第2條橫線
                    LineCollect[8].Add(p);

                else if (Math.Abs((p.X - (MinY + GridLength * 9))) <= 100)//第2條橫線
                    LineCollect[9].Add(p);

            }
            for (int k = 0; k < 10; k++)
            {
                LineCollect[k].Sort((p1, p2) => p1.X.CompareTo(p2.X));
                //Console.WriteLine("LineCollect" + k + "=" + LineCollect[k].Count());
            }


            ////-------------------------------------------------------------------------------------


            List<List<PointF>> ColumnCollect = new List<List<PointF>>();
            List<PointF> GridColumn = new List<PointF>();
            for (int i = 0; i < 10; i++)
                ColumnCollect.Add(GridColumn);

            for (int i = 0; i < 10; i++)
                ColumnCollect[i] = new List<PointF>();



            foreach (PointF p in TrayHoleList)
            {
                //1. 找X最小的點, 在最左邊
                //2. 找Y最小的點,在最上面
                if (MinX > p.X) MinX = p.X;
            }


            foreach (PointF p in TrayHoleList)//取十條
            {

                //2F
                if (Math.Abs((p.X - MinX)) <= 20)//第一條橫線
                    ColumnCollect[0].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength))) <= 20)//第2條橫線
                    ColumnCollect[1].Add(p);
                else if (Math.Abs((p.X - (MinX + GridLength * 2))) <= 25)//第2條橫線
                    ColumnCollect[2].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 3))) <= 35)//第2條橫線
                    ColumnCollect[3].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 4))) <= 40)//第2條橫線
                    ColumnCollect[4].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 5))) <= 45)//第2條橫線
                    ColumnCollect[5].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 6))) <= 100)//第2條橫線
                    ColumnCollect[6].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 7))) <= 100)//第2條橫線
                    ColumnCollect[7].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 8))) <= 100)//第2條橫線
                    ColumnCollect[8].Add(p);

                else if (Math.Abs((p.X - (MinX + GridLength * 9))) <= 100)//第2條橫線
                    ColumnCollect[9].Add(p);

            }
            for (int k = 0; k < 10; k++)
            {
                ColumnCollect[k].Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
                //  Console.WriteLine("ColumnCollect" + k + "=" + ColumnCollect[k].Count());
            }

            //-------------------------------------------------------------------------------------
            int GridNum = 10;
            PointF[,] MapGrid = new PointF[GridNum, GridNum];


            //1.6.5 by 線組合計算x座標平均做為爛銲道 x 座標y 座標

            int YAvg = 0;
            ////求每條線的 Y 平均
            for (int k = 0; k < LineCollect.Count(); k++)
            {
                YAvg = 0;
                if (LineCollect[k].Count() > 0)
                {

                    foreach (PointF p in LineCollect[k])
                        YAvg += (int)p.Y;

                    YAvg = YAvg / LineCollect[k].Count();

                    for (int i = 0; i < GridNum; i++)
                    {
                        if (YAvg != 0)
                            MapGrid[i, k].Y = YAvg;
                    }
                }
            }

            int XAvg = 0;
            //求每條線的 Y 平均
            for (int k = 0; k < ColumnCollect.Count(); k++)
            {
                XAvg = 0;
                if (ColumnCollect[k].Count() > 0)
                {

                    foreach (PointF p in ColumnCollect[k])
                        XAvg += (int)p.X;

                    XAvg = XAvg / ColumnCollect[k].Count();

                    for (int j = 0; j < GridNum; j++)
                    {
                        if (XAvg != 0)
                            MapGrid[k, j].X = XAvg;
                    }
                }
            }
            ////---------------------------填入好的銲道-----------------------------------------
            //ShortDist = 100000000;
            //dist = 0;
            //PointF ShortP = new PointF();

            //for (int j = 0; j < 10; j++)
            //    for (int i = 0; i < 10; i++)
            //    {
            //        ShortP = new PointF();
            //        ShortDist = 100000000;
            //        foreach (PointF pp in GoodWeldList)
            //        {
            //            dist = (MapGrid[i, j].X - pp.X) * (MapGrid[i, j].X - pp.X) + (MapGrid[i, j].Y - pp.Y) * (MapGrid[i, j].Y - pp.Y);
            //            if (dist < 50 * 50 && dist < ShortDist)
            //            {
            //                ShortDist = dist;
            //                ShortP.X = pp.X;
            //                ShortP.Y = pp.Y;
            //            }
            //        }

            //        if (ShortP.X == 0 && ShortP.Y == 0)
            //        { }
            //        else
            //        {
            //            MapGrid[i, j].X = ShortP.X;
            //            MapGrid[i, j].Y = ShortP.Y;

            //        }

            //    }


            return MapGrid;
        }

        public WeldCenterInfo[,] TemplateMatch(Bitmap ProcessBMP, PointF[,] MapGrid, List<PointF> GoodWeldP)
        {

            //------------------------------------------------------
            //1. TEMPLATE訂定
            //------------------------------------------------------
            int TempSize = 64;
            TemplateBMP = new Bitmap(TempSize + 8, TempSize + 8);
            Graphics gg = Graphics.FromImage(TemplateBMP);
            Pen BlackPen = new Pen(Color.Black, 1);

            gg.Clear(Color.White);
            gg.DrawEllipse(BlackPen, 5, 5, TempSize, TempSize);


            //讀取 template 黑框圓周點
            int[, ,] TemplateData = BMPFast.GetRGBData(TemplateBMP);

            List<System.Drawing.Point> TemplateEdgeList = new List<System.Drawing.Point>();
            System.Drawing.Point EdgeP;

            for (int j = 0; j < TemplateBMP.Height; j++)
            {
                for (int i = 0; i < TemplateBMP.Width; i++)
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



            //------------------------------------------------------------------------------
            int[, ,] ProcessBmpArr = BMPFast.GetRGBData(ProcessBMP);


            WeldCenterInfo[,] WeldCenterList = new WeldCenterInfo[10, 10];

            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 10; i++)
                    WeldCenterList[i, j] = new WeldCenterInfo();

            int EdgeCnt = 0;

            WeldCenterInfo MaxCandidte;
            int MaxScore = 0;

            //    //---------------------------------------
            for (int mmj = 0; mmj < 7; mmj++)
                for (int mmi = 0; mmi < 7; mmi++)
                {
                    MaxCandidte = new WeldCenterInfo();
                    MaxCandidte.WeldScore = 0;
                    MaxScore = 0;
                    if (!GoodWeldP.Exists(p => p.X == MapGrid[mmi, mmj].X) && !(GoodWeldP.Exists(p => p.Y == MapGrid[mmi, mmj].Y))
                        && MapGrid[mmi, mmj].Y != -1 && MapGrid[mmi, mmj].X != -1)

                    //沒有在理想list的網格點
                    {


                        for (int j = (int)(MapGrid[mmi, mmj].Y - 15); j < (int)(MapGrid[mmi, mmj].Y + 15); j++)
                        {
                            for (int i = (int)(MapGrid[mmi, mmj].X - 15); i < (int)(MapGrid[mmi, mmj].X + 15); i++)
                            {


                                EdgeCnt = 0;

                                //算出每個點專屬的EGDE範圍 
                                foreach (System.Drawing.Point p in TemplateEdgeList)//每一個 template 的 edge 點
                                {
                                    PointF TT = new PointF();
                                    TT.X = p.X + (i - 37);
                                    TT.Y = p.Y + (j - 37);

                                    if (TT.X > 0 && TT.Y > 0 && TT.X < 2048 && TT.Y < 2048)
                                        if (ProcessBmpArr[(int)TT.X, (int)TT.Y, 0] == 0) //TEMPLATE edge 點位移至當前位置, 判斷是否也為邊緣點(黑色)
                                        {
                                            EdgeCnt++;

                                        }


                                }

                                if (EdgeCnt > MaxScore && EdgeCnt > TemplateEdgeList.Count() * 0.85)//一半以上銲道點符合
                                {

                                    MaxScore = EdgeCnt;
                                    MaxCandidte.WeldScore = EdgeCnt;
                                    MaxCandidte.WeldCandidate.X = i;
                                    MaxCandidte.WeldCandidate.Y = j;

                                }




                            }//FOR I
                        }//FOR J


                        if (!(MaxCandidte.WeldCandidate.X == MapGrid[mmi, mmj].X && MaxCandidte.WeldCandidate.Y == MapGrid[mmi, mmj].Y))
                        {


                            WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                            WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;
                            WeldCenterList[mmi, mmj].WeldScore = MaxCandidte.WeldScore;

                        }

                    }//end if not goodlist

                    else //理想銲道則不用尋找,直接用原本的
                    {
                        MaxCandidte.WeldScore = 100000;
                        MaxCandidte.WeldCandidate.X = (int)MapGrid[mmi, mmj].X;
                        MaxCandidte.WeldCandidate.Y = (int)MapGrid[mmi, mmj].Y;
                        WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                        WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;
                        WeldCenterList[mmi, mmj].WeldScore = MaxCandidte.WeldScore;

                    }


                    if (MaxCandidte.WeldScore == 0)
                    {
                        MaxCandidte.WeldCandidate.X = (int)MapGrid[mmi, mmj].X;
                        MaxCandidte.WeldCandidate.Y = (int)MapGrid[mmi, mmj].Y;
                        WeldCenterList[mmi, mmj].WeldCandidate.X = MaxCandidte.WeldCandidate.X;
                        WeldCenterList[mmi, mmj].WeldCandidate.Y = MaxCandidte.WeldCandidate.Y;

                    }
                }//for map


            return WeldCenterList;
        }

        public PointF[,] MapGridFilter(PointF[,] MapGrid, int[, ,] OriBmpArr)
        {
            //  PointF[,] NewMapGrir = new PointF[10, 10];

            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 10; i++)
                {
                    if (MapGrid[i, j].X == 0 || MapGrid[i, j].Y == 0)
                    {
                        MapGrid[i, j].X = -1;
                        MapGrid[i, j].Y = -1;

                    }
                    if (MapGrid[i, j].X > 0 && MapGrid[i, j].Y > 0)
                        if (OriBmpArr[(int)MapGrid[i, j].X, (int)MapGrid[i, j].Y, 0] < 60)
                        {
                            MapGrid[i, j].X = -1;
                            MapGrid[i, j].Y = -1;

                        }

                }


            return MapGrid;

        }









    }
    public class WeldCenterInfo
    {
        public PointF WeldCandidate;
        public int WeldScore;
        //  public System.Drawing.Point MapIdx;

    }
}
