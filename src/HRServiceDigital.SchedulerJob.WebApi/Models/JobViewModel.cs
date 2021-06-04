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
        public bool IsDurable { get; set; }
        public bool IsNonConcurrent { get; set; }
        public bool IsUpdateData { get; set; }
        public bool RequestsRecovery { get; set; }
        public string JobData
        {
            get => System.Text.Encoding.UTF8.GetString(this.JobDataByte);
        }

        [JsonIgnore]
        public byte[] JobDataByte { get; set; }
        public class JobVieModelwMap : EntityMap<JobViewModel>
        {
            public JobVieModelwMap()
            {
                Map(p => p.SchedulerName).ToColumn("SCHED_NAME");
                Map(p => p.JobName).ToColumn("JOB_NAME");
                Map(p => p.JobClassName).ToColumn("JOB_CLASS_NAME");
                Map(p => p.JobGroup).ToColumn("JOB_GROUP");
                Map(p => p.Description).ToColumn("DESCRIPTION");
                Map(p => p.IsDurable).ToColumn("IS_DURABLE");
                Map(p => p.IsNonConcurrent).ToColumn("IS_NONCONCURRENT");
                Map(p => p.IsUpdateData).ToColumn("IS_UPDATE_DATA");
                Map(p => p.JobDataByte).ToColumn("JOB_DATA");
            }
        }
    }
}
