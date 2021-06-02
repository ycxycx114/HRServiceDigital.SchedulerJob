
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz.Models
{
    public class HrsScheduleJob
    {
        private IDictionary<string, object> _JobDataMap;
        public string SchedulerName { get; set; }
        public string JobGroup { get; set; } = "DEFAULT";
        public string JobDescription { get; set; }
        public string JobData { get; set; }
        public string TriggerGroup { get; set; }
        public string TriggerDescription { get; set; }
        public string CronExpression { get; set; }
        public string TimeZoneId { get; set; }

        public IDictionary<string, object> GetJobDataMap()
        {
            if (_JobDataMap == null)
            {
                _JobDataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(JobData);
            }
            return _JobDataMap;
        }
    }
}
