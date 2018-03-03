using System;

namespace ElectroMaster.Core.Services.Application
{
    public interface IBackgroundService
    {
        /// <summary>
        /// Can check whether Service is already running or not
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Create the Background Sevice
        /// </summary>
        /// <param name="methodToExecute">Action need to perform</param>
        /// <param name="timePeriod">Interval in miliseconds</param>
        void Create(Action methodToExecute, int timePeriod);

        /// <summary>
        /// Stop a Background Service
        /// </summary>
        void Stop();
    }
}
