using Dapper.FluentMap;
using HRServiceDigital.SchedulerJob.Quartz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz
{
    public class Bootstrap
    {
        public static void ConfigMap()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new TriggerViewModel.TriggerViewModelMap());
                config.AddMap(new JobViewModel.JobVieModelwMap());
            });
        }
    }
}
