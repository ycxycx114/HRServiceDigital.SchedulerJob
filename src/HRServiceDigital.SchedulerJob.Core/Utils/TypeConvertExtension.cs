using System;
using System.Collections.Generic;
using System.Text;

namespace HRServiceDigital.SchedulerJob.Core.Utils
{
    public static class TypeConvertExtension
    {
        public static string FormatDateTime(this DateTimeOffset fireDateTime)
        {
            return string.Format(JobConsts.DATA_TIME_FORMAT, fireDateTime.LocalDateTime);
        }

        public static string FormatDateTime(this DateTimeOffset? fireDateTime)
        {
            if (fireDateTime.HasValue)
            {
                return string.Format(JobConsts.DATA_TIME_FORMAT, fireDateTime.Value.LocalDateTime);
            }
            return default;
        }
    }
}
