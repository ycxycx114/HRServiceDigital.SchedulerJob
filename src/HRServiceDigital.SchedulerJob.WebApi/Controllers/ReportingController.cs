using Dapper;
using HRServiceDigital.SchedulerJob.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    [Route("api/services/quartz/[controller]")]
    [ApiController]
    public class ReportingController : SchedulerJobControllerBase
    {

        private readonly IDbConnection _DbConnection;
        private readonly IConfiguration _Configuration;

        public ReportingController(IDbConnection dbConnection, IConfiguration configuration)
        {
            _DbConnection = dbConnection;
            _Configuration = configuration;
        }

        [HttpGet]
        [Route("allJobs")]
        public async Task<IEnumerable<JobViewModel>> GetJobs(string schedulerName = "QuartzScheduler")
        {
            string sql = @"SELECT
                             SCHED_NAME
                               ,JOB_NAME
                               ,JOB_GROUP
                               ,DESCRIPTION
                               ,JOB_CLASS_NAME
                               ,IS_DURABLE
                               ,IS_NONCONCURRENT
                               ,IS_UPDATE_DATA
                               ,REQUESTS_RECOVERY
                               ,JOB_DATA
                            FROM QRTZ_JOB_DETAILS
                            WHERE SCHED_NAME = @Scheduler";

            return await _DbConnection.QueryAsync<JobViewModel>(sql, new { Scheduler = new DbString { Value = schedulerName } });
        }

        [HttpGet]
        [Route("allTriggers")]
        public async Task<IEnumerable<TriggerViewModel>> GetTrigger(string schedulerName = "QuartzScheduler")
        {
            string sql = @"SELECT 
                            t0.SCHED_NAME,
                            t0.TRIGGER_NAME,
                            t0.TRIGGER_GROUP,
                            t0.JOB_NAME, 
                            t0.JOB_GROUP,
                            t0.DESCRIPTION,
                            t0.TRIGGER_STATE,
                            t0.TRIGGER_TYPE,
                            t1.CRON_EXPRESSION,
                            t1.TIME_ZONE_ID
                            FROM QRTZ_TRIGGERS t0
                            JOIN QRTZ_CRON_TRIGGERS t1 on t0.SCHED_NAME = t1.SCHED_NAME
                            AND t0.TRIGGER_NAME = t1.TRIGGER_NAME
                            AND t0.TRIGGER_GROUP = t1.TRIGGER_GROUP
                            WHERE t0.SCHED_NAME = @Scheduler";

            return await _DbConnection.QueryAsync<TriggerViewModel>(sql,
                new { Scheduler = new DbString { Value = schedulerName } });
        }
    }
}
