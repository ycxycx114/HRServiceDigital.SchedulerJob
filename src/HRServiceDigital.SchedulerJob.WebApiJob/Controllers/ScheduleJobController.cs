using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApiJob.Controllers
{
    [Route("api/services/schedulerjob")]
    [ApiController]
    public class ScheduleJobController : ControllerBase
    {
        private static int NoParamCount = 0;
        private static int OneParamCount = 0;

        [HttpGet]
        public string Get()
        {
            Thread.Sleep(18000);
            return $"Scheduler job Sucess with {NoParamCount++} times.";
        }

        [Route("a2")]
        [HttpGet]
        public string Get2([FromQuery] string a)
        {
            Thread.Sleep(12000);
            return $"No. 2 with param [{a}] - Scheduler Job sucess with {OneParamCount++} times.";
        }

    }
}
