using Newtonsoft.Json;
using System.Collections.Generic;

namespace HRServiceDigital.SchedulerJob.Quartz.Models
{
    public class HrsJob
    {
        private IDictionary<string, object> _JobDataMap;
        public string SchedulerName { get; set; }
        public string JobName { get; set; }
        public string JobClassName { get; set; }
        public string JobGroup { get; set; } = "DEFAULT";
        public string Description { get; set; }
        public bool IsDurable { get; set; }
        public bool IsNonConcurrent { get; set; }
        public bool IsUpdateData { get; set; }
        public bool RequestsRecovery { get; set; }
        public string JobData { get; set; }
        public IDictionary<string, object> GetJobDataMap()
        {
            if(_JobDataMap == null)
            {
                _JobDataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(JobData);
            }
            return _JobDataMap;
        }
    }
}
