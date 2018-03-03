using ElectroMaster.Core.Services.Application;
using System;
using System.Threading;

namespace ElectroMaster.Services.Application
{
    public class BackgroundService : IBackgroundService
    {
        private object _createThreadSafe = new object();
        private object _stopThreadSafe = new object();

        /// <summary>
        /// Can check whether Service is already running or not
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Initialize the Background Sevice but not created
        /// </summary>
        public BackgroundService()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Initialize and create the Background Sevice
        /// </summary>
        /// <param name="methodToExecute">Action need to perform</param>
        /// <param name="timePeriod">Interval in miliseconds</param>
        public BackgroundService(Action methodToExecute, int timePeriod) : this()
        {
            Create(methodToExecute, timePeriod);
        }

        /// <summary>
        /// Create the Background Sevice
        /// </summary>
        /// <param name="methodToExecute">Action need to perform</param>
        /// <param name="timePeriod">Interval in miliseconds</param>
        public void Create(Action methodToExecute, int timePeriod)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                lock (_createThreadSafe)
                {
                    Thread thread = new Thread(() =>
                    {
                        System.Timers.Timer timeCounterTimer = new System.Timers.Timer();
                        timeCounterTimer.Interval = timePeriod;
                        timeCounterTimer.Elapsed += (s, e) =>
                        {
                            if (IsRunning)
                                methodToExecute();
                            else
                                timeCounterTimer.Stop();
                        };
                        timeCounterTimer.Enabled = true;
                    });

                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Stop a Background Service
        /// </summary>
        public void Stop()
        {
            lock (_stopThreadSafe)
            {
                if (IsRunning)
                {
                    IsRunning = false;
                }
            }
        }
    }
}
