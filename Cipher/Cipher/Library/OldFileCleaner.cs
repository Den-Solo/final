using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.IO;

namespace Cipher.Library
{
    public class OldFileCleaner
    {
        private Thread _th;
        private TimeSpan _cleanInterval;
        private TimeSpan _outdatingInterval;
        private string _dirPath;
        private string _logPath;

        public OldFileCleaner(TimeSpan cleanInterval, TimeSpan outdatingInterval, string filePath, string logPath)
        {
            _cleanInterval = cleanInterval;
            _outdatingInterval = outdatingInterval;
            _dirPath = filePath;
            _logPath = logPath;
            _th = new Thread(Clean);
            _th.Start();
        }
        private void Clean()
        {
            while (true)
            {
                string[] fileNames = Directory.EnumerateFiles(_dirPath).ToArray();
                DateTime curTime = DateTime.Now;
                foreach (var fName in fileNames)
                {

                    if (curTime - File.GetCreationTime(fName) > _outdatingInterval)
                    {
                        File.Delete(fName);
                        File.AppendAllLines(_logPath, new string[] { fName + " Deleted" });
                    }
                }
                Thread.Sleep(_cleanInterval);
            }
        }
        public void ClearAll()
        {
            _outdatingInterval = new TimeSpan(-1);
            Clean();
        }
        public void Abort()
        {
            _th.Abort();
        }

    }
}