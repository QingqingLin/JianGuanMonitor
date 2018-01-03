using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace MonitorPorts
{
    internal class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime, stopTime;
        private long freq;
        private long _frequent;

        public long StartTime
        {
            get
            {
                if (QueryPerformanceCounter(out startTime) == false)
                {
                    // high-performance counter not supported 
                    throw new Win32Exception();
                }
                return startTime;
            }
        }
        public long StopTime
        {
            get
            {
                if (QueryPerformanceCounter(out stopTime) == false)
                {
                    // high-performance counter not supported 
                    throw new Win32Exception();
                }
                return stopTime;
            }
        }
        #region 时钟频率
        public long frequent
        {
            get
            {
                if (QueryPerformanceFrequency(out _frequent) == false)
                {
                    // high-performance counter not supported 
                    throw new Win32Exception();
                }
                return _frequent;
            }

        }
        #endregion

        // Constructor
        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported 
                throw new Win32Exception();
            }
            if (QueryPerformanceFrequency(out _frequent) == false)
            {
                // high-performance counter not supported 
                throw new Win32Exception();
            }
        }

        // Start the timer
        public void Start()
        {
            // lets do the waiting threads there work
            Thread.Sleep(0);

            QueryPerformanceCounter(out startTime);
        }

        // Stop the timer
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="timeInterval"></param>毫秒
        public void Interval(double timeInterval)
        {
            timeInterval /= 1000;

            long intervalStartTime, intervalStopTime;
            QueryPerformanceCounter(out intervalStartTime);
            QueryPerformanceCounter(out intervalStopTime);
            while (((double)(intervalStopTime - intervalStartTime) / (double)freq) < timeInterval)
            {
                QueryPerformanceCounter(out intervalStopTime);
            }
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {

            get
            {
                QueryPerformanceCounter(out stopTime);
                return (double)(stopTime - startTime) / (double)freq;
            }
        }
    }
}
