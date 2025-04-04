using DemoProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using static Quartz.Logging.OperationName;
using DemoProject.Helper;

namespace DemoProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;
            StartQuartzScheduler();
        }

        private async void StartQuartzScheduler()
        {
            // Create a scheduler factory
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();

            // Start the scheduler
            await scheduler.Start();

            // Create and schedule the job
            IJobDetail job = JobBuilder.Create<MyJob>()
                                       .WithIdentity("myJob", "group1")
                                       .Build();

            // Create a trigger to run every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("myTrigger", "group1")
                                             .StartNow()
                                             .WithSimpleSchedule(x => x
                                                 .WithIntervalInSeconds(240)
                                                 .RepeatForever())
                                             .Build();

            // Schedule the job with the trigger
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
