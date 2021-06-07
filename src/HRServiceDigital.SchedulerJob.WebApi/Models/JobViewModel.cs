using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Models
{
    public class JobViewModel
    {
        public string SchedulerName { get; set; }
        public string JobName { get; set; }
        public string JobClassName { get; set; }
        public string JobGroup { get; set; } = "DEFAULT";
        public string Description { get; set; }
        public string FireInstanceId { get; set; }
        public string ScheduledFireTime { get; set; }
        public string FireTime { get; set; }
        public string PreviousFireTimeUtc { get; set; }
        public string NextFireTimeUtc { get; set; }
        public string JobData { get; set; }
    }
}
