using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    [Route("api/services/quartz/[controller]")]
    [ApiController]
    public class SchedulerController : SchedulerJobControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly ISchedulerFactory _SchedulerFactory;

        public SchedulerController(IConfiguration configuration, ISchedulerFactory schedulerFactory)
        {
            _Configuration = configuration;
            _SchedulerFactory = schedulerFactory;
        }

        [HttpGet]
        public string GetSchedulerName()
        {
            //return GetSchedulerName(_Configuration.GetSection("Quartz"));
            return SchedulerName;
        }

        [Route("cluster")]
        [HttpGet]
        public async Task<List<string>> GetAllSchedulers()
        {
            var schedulers = await _SchedulerFactory.GetAllSchedulers();
            return schedulers.Select(p => p.SchedulerName).ToList();
        }
    }
}
