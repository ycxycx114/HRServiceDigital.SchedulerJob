using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz.Controllers
{
    public class SchedulerJobControllerBase : ControllerBase
    {
        protected string GetSchedulerName(IConfigurationSection configurationSection)
        {
            return configurationSection.GetSection("quartz.scheduler.instanceName").Value;
        }
    }
}
