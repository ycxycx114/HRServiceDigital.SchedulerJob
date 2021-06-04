using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApiJob.Controllers
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

        [Route("a2")]
        [HttpGet]
        public string Get2([FromQuery] string a)
        {
            return $"No. 2 with param [{a}] - Scheduler Job sucess - {DateTime.Now}";
        }

    }
}
