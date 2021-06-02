using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    [Route("api/services/schedulerjob")]
    [ApiController]
    public class ScheduleJobController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return $"Scheduler job Sucess - {DateTime.Now}";
        }
    }
}
