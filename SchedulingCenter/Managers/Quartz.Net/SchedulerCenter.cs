using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SchedulingCenter.Models;
using Quartz;
using Quartz.Impl;

namespace SchedulingCenter.Managers.Quartz.Net
{

    /// <summary>
    /// 任务调度中心
    /// </summary>
    public class SchedulerCenter
    {
        /// <summary>
        /// 任务计划列表
        /// </summary>
        public static Dictionary<string, ScheduleEntity> ScheduleList = new Dictionary<string, ScheduleEntity>();

        /// <summary>
        /// 任务调度对象
        /// </summary>
        public static readonly SchedulerCenter Instance;

        static SchedulerCenter()
        {
            Instance = new SchedulerCenter();
        }

        private IScheduler _scheduler;

        /// <summary>
        /// 返回任务计划（调度机）
        /// </summary>
        /// <returns></returns>
        private IScheduler Scheduler {
            get {
                if (_scheduler != null)
                    return _scheduler;
                else
                {
                    ISchedulerFactory schedf = new StdSchedulerFactory();
                    _scheduler = schedf.GetScheduler().GetAwaiter().GetResult();
                    return _scheduler;
                }
            }
        }

        /// <summary>
        /// 添加一个工作调度
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private async Task<bool> AddScheduleJob<T>(ScheduleEntity m, CancellationToken cancellationToken = default(CancellationToken)) where T : IJob
        {
            try
            {
                if (m == null) return false;
                if (m.StarRunTime == null)
                {
                    m.StarRunTime = DateTime.Now;
                }
                if(!Scheduler.IsStarted) {
                    await Scheduler.Start(cancellationToken);
                }
                var starRunTime = DateBuilder.NextGivenSecondDate(m.StarRunTime, 1);
                DateTimeOffset endRunTime;
                if (m.EndRunTime == null)
                {
                    endRunTime = DateTime.MaxValue;
                }
                else
                {
                    endRunTime = DateBuilder.NextGivenSecondDate(m.EndRunTime, 1);
                }
                var dataMap = new JobDataMap();
                IJobDetail job = job = JobBuilder.Create<T>()
                    .SetJobData(dataMap)
                    .WithIdentity(m.JobName, m.JobGroup).Build();
                ITrigger trigger = null;
                if (!string.IsNullOrEmpty(m.CronStr))
                {
                    trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity(m.JobName, m.JobGroup)
                        .StartAt(starRunTime)
                        .EndAt(endRunTime)
                        .WithIdentity(m.JobName, m.JobGroup)
                        .WithCronSchedule(m.CronStr)
                        .Build();
                }
                else
                {
                    trigger = (ISimpleTrigger)TriggerBuilder.Create()
                              .WithIdentity(m.JobName, m.JobGroup)
                              .StartAt(starRunTime)
                              .WithSimpleSchedule(x => x.WithIntervalInSeconds(1)
                              .WithRepeatCount(0))
                              .Build();
                }
                if(await Scheduler.CheckExists(new JobKey(m.JobName, m.JobGroup), cancellationToken)) {
                    // 重置调度时间
                    await Scheduler.RescheduleJob(new TriggerKey(m.JobName, m.JobGroup), trigger, cancellationToken);
                } else {
                    await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 停止指定的计划
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <param name="isDelete">停止并删除任务</param>
        /// <returns></returns>
        public async Task<StatusResult> StopScheduleJob(string jobGroup, string jobName,  ITrigger trigger = null , CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // 停止计划执行中的作业
                if(trigger == null) {
                    await Scheduler.DeleteJob(new JobKey(jobName, jobGroup) ,cancellationToken);
                }
                else { 
                    await Scheduler.PauseTrigger(trigger.Key, cancellationToken);
                }
                return new StatusResult() { Status = 0, Msg = "停止任务计划成功！" };
            }
            catch (Exception ex)
            {
                return new StatusResult() { Status = 500, Msg = "停止任务计划失败" };
            }
        }

        public async Task<ITrigger> GetTriggerAsync(string jobGroup, string jobName, string suffix)
        {
            try
            {
               return await Scheduler.GetTrigger(new TriggerKey(jobName + suffix, jobGroup));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 运行指定的计划
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public async Task<StatusResult> RunScheduleJob<T>(string jobGroup, string jobName, CancellationToken cancellationToken = default(CancellationToken))
            where T : IJob
        {
            try
            {
                var sm = ScheduleList[$"{jobGroup}{jobName}"];
                var result = await AddScheduleJob<T>(sm);
                if (result)
                {
                    // 更改任务状态
                    // sm.Status = EnumType.JobStatus.Opened;
                    // 重新启动任务
                    await Scheduler.ResumeJob(new JobKey(jobName, jobGroup), cancellationToken);
                    return new StatusResult { Status = 0, Msg = "启动成功" };
                }
                else
                {
                    return new StatusResult { Status = 500, Msg = "启动失败" };
                }
            }
            catch (Exception ex)
            {
                return new StatusResult { Status = 500, Msg = "启动失败" };
            }
        }
    }
}
