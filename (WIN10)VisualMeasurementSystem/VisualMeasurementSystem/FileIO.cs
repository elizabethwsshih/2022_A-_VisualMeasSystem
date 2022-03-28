using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace VisualMeasurementSystem
{
    public class PerformanceDataInfo
    {
        public String _pModelID;
        public String _pAB;
        public String _pAreaID;
        public String _pWeldID;


    }
    class FileIO
    {


        public void DeleteFile(String DelFilePath, String FileType, double Mins)
        {
            string[] files = Directory.GetFiles(DelFilePath, FileType);
            foreach (string file in files)
            {
                FileInfo f = new FileInfo(file);
                TimeSpan ts = DateTime.Now - f.CreationTime;
                double TimeDiff = Convert.ToDouble(ts.TotalMinutes.ToString());
                if (TimeDiff > Mins)
                    File.Delete(file);
            }
        }
        public List<PerformanceDataInfo> PerformanceData(String SettingFilePath)
        {
            int linecnt = 0;
            string line, subline;
            System.IO.StreamReader sr = new System.IO.StreamReader(SettingFilePath, Encoding.GetEncoding(950));
            sr = System.IO.File.OpenText(SettingFilePath);

            List<PerformanceDataInfo> _PerformanceData = new List<PerformanceDataInfo>();
            PerformanceDataInfo newPerm = new PerformanceDataInfo();
            

            string _ModelID = "", _AB = "", _AreaID = "", _WeldID = "";
            linecnt = 0;
            //try
            //{
                sr = new System.IO.StreamReader(SettingFilePath, Encoding.GetEncoding(950));
                sr = System.IO.File.OpenText(SettingFilePath);


                while ((line = sr.ReadLine()) != null)
                {
                    newPerm = new PerformanceDataInfo();

                    _ModelID = line.Substring(0, line.IndexOf(","));
                    newPerm._pModelID = _ModelID;

                    subline = line.Substring(line.IndexOf(",") + 1, line.Length - (line.IndexOf(",") + 1));
                    _AB = subline.Substring(0, subline.IndexOf(","));
                    newPerm._pAB = _AB;

                    subline = subline.Substring(subline.IndexOf(",") + 1, subline.Length - (subline.IndexOf(",") + 1));
                    _AreaID = subline.Substring(0, subline.IndexOf(","));
                    newPerm._pAreaID = _AreaID;

                    subline = subline.Substring(subline.IndexOf(",") + 1, subline.Length - (subline.IndexOf(",") + 1));
                    _WeldID = subline.Substring(0, subline.Length);
                    newPerm._pWeldID = _WeldID;

                    _PerformanceData.Add(newPerm);

                }

            //}
            //catch
            //{

            //}
            return _PerformanceData;
        }




    }
}
