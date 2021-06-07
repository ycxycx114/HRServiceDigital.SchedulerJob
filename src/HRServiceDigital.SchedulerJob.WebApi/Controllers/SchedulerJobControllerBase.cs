using HRServiceDigital.SchedulerJob.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    public class SchedulerJobControllerBase : ControllerBase
    {
        protected string SchedulerName
        {
            get
            {
                var quartzConfig = (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
                return quartzConfig.GetValue<string>("Quartz:quartz.scheduler.instanceName");
            }
        }
        
    }
}
