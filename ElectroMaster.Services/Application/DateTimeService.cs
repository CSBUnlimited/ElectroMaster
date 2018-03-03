using ElectroMaster.Models.Application;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ElectroMaster.Services.Application
{
    public static class DateTimeService
    {
        private static readonly DateTime s_initialStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentDateTime { get; private set; }
        
        public static DateTime GetCurrentTimeInDateTime()
        {
            return s_initialStartDate.AddSeconds(CurrentDateTime);
        }

        public static long GetCurrentDateInLong()
        {
            DateTime NowDate = GetCurrentTimeInDateTime();
            return GetCustomDateTimeInInt(NowDate);
        }

        public static long GetCustomDateTimeInInt(DateTime dateTime)
        {
            return (long)new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond)
                                    .Subtract(s_initialStartDate).TotalSeconds;
        }

        public static async Task<bool> SetApplicationDateTime()
        {
            return await Task.Run(() =>
            {
                bool returns = false;

                for (byte tries = 0; tries < 3 && !returns; tries++)
                {
                    try
                    {
                        DateTime dateTime;

                        using (WebResponse response = WebRequest.Create(@"http://www.microsoft.com").GetResponse())
                        {
                            dateTime = DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
                        }

                        CurrentDateTime = GetCustomDateTimeInInt(dateTime) + 1;
                        //Fix time to Sri Lanka
                        CurrentDateTime += ((5 * 60) + 30) * 60;

                        new BackgroundService(() => 
                        {
                            CurrentDateTime++;
                        }, 1000);

                        returns = true;
                    }
                    catch
                    {
                        returns = false;
                        Thread.Sleep(250);
                    }
                }

                return returns;
            });
        }
    }
}
