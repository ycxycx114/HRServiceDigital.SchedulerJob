using Dapper.FluentMap;
using HRServiceDigital.SchedulerJob.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi
{
    public class Bootstrap
    {
        public static void ConfigMap()
        {
            FluentMapper.Initialize(config =>
            {
                
            });
        }
    }
}
