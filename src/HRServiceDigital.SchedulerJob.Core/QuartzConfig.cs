using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

namespace HRServiceDigital.SchedulerJob.Core
{
    public class QuartzConfig
    {
        [JsonPropertyName("quartz.scheduler.instanceName")]
        public string InstanceName { get; set; }
        [JsonPropertyName("quartz.jobStore.clustered")]
        public bool Clustered { get; set; }
    }
}
