
using Quartz;
using Quartz.Impl;
using System.Threading;
using System.Threading.Tasks;

namespace OntrackDb.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            Task<IScheduler> scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<UserNotificationJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .WithIntervalInSeconds(5)
                  )
                .Build();

            scheduler.Result.ScheduleJob(job, trigger);
        }
    }
}
