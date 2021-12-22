using ApiCore.Basic;
using ApiCore.JsonFilter;
using Microsoft.EntityFrameworkCore;
using SchedulingCenter.DTO.Request;
using SchedulingCenter.Managers.Quartz.Net;
using SchedulingCenter.Models;
using SchedulingCenter.Stores;
using SchedulingCenter.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingCenter.Managers
{
    /// <summary>
    /// 任务调度管理
    /// </summary>
    public class ScheduleManager
    {
        private IScheduleStore _store;
        private readonly IJsonHelper ijsonHelper;

        public ScheduleManager(IScheduleStore store, IJsonHelper jsonHelper)
        {
            _store = store;
            ijsonHelper = jsonHelper;
        }


        /// <summary>
        /// 从任务列表删除指定的任务
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static void RemoveSchedule(ScheduleEntity schedule) {
            lock (AppConfigContext.LockObject) {
                // 删除缓存及数据库中的任务
                SchedulerCenter.ScheduleList.Remove($"{schedule.JobGroup}{schedule.JobName}");
            }
        }

        /// <summary>
        /// 添加任务并运行
        /// </summary>
        /// <param name="request">添加任务请求</param>
        /// <param name="cancellationToken">请求标识</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage> AddSchedule(QuartzEntityRequest request, string remoteAddress ,CancellationToken cancellationToken = default(CancellationToken))
        {
            // 请求参数为空时不创建任务
            if (request == null) throw new ArgumentNullException(nameof(request));
            var schedule = new ScheduleEntity {
                Id = Guid.NewGuid().ToString(),
                Args = ijsonHelper.ToJson(request.Args),
                CronStr = request.CronStr,
                EndRunTime = request.EndRunTime,
                JobGroup = request.JobGroup,
                JobName = request.JobName,
                RunStatus = EnumType.JobRunStatus.Waiting,
                StarRunTime = request.StarRunTime,
                RunStep = EnumType.JobStep.Planned,
                Status = EnumType.JobStatus.Stopped,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Callback = request.Callback,
                ActuatorAddress = remoteAddress,
                IsDelete = false,
            };
            // 管理列表对象时添加锁
            lock (AppConfigContext.LockObject) {
                // 添加任务
                var key = $"{request.JobGroup}{request.JobName}";
                if (SchedulerCenter.ScheduleList.ContainsKey(key)) {
                    SchedulerCenter.ScheduleList[key] = schedule;
                } else {
                    // 缓存中不存在任务调度时添加至缓存
                    SchedulerCenter.ScheduleList.Add(key, schedule);
                }
            }
            // 数据库中是否存在调度任务
            var scheduleId = await _store.GetSchedules().Where(it => it.JobName == request.JobName && it.JobGroup == request.JobGroup).Select(it => it.Id).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(scheduleId))
            {
                // 添加任务到数据库中
                await _store.CreateAsync(schedule, cancellationToken);
            }
            else
            {
                // 添加任务到数据库中
                schedule.Id = scheduleId;
                await _store.UpdateAsync(schedule, cancellationToken);
            }
            // 运行任务
            var result = await SchedulerCenter.Instance.RunScheduleJob<JobActuator>(schedule.JobGroup, schedule.JobName, cancellationToken);
            var r = new ResponseMessage { Message = result.Msg, Code = result.Status.ToString() };
            if (result.Status != 0) return r; // 执行任务失败
            // 启动任务
            schedule.Status = EnumType.JobStatus.Opened;
            // 任务启动成功后更新任务状态到数据库中
            await _store.UpdateAsync(schedule, cancellationToken);
            return r;
        }

        /// <summary>
        /// 停止任务并允许永久删除
        /// </summary>
        /// <param name="request">停止任务请求</param>
        /// <param name="cancellationToken">请求标识</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage> StopSchedule(StopScheduleRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var key = $"{request.JobGroup}{request.JobName}";
            var r = new ResponseMessage();
            if (!SchedulerCenter.ScheduleList.ContainsKey(key))
            {
                r.Code = ResponseCodeDefines.ArgumentNullError;
                r.Message = "找不到需要停止的任务";
                return r;
            };
            var schedule = SchedulerCenter.ScheduleList[key];
            // 停止任务
            var result = await SchedulerCenter.Instance.StopScheduleJob(request.JobGroup, request.JobName, trigger: null, cancellationToken);
            r.Code = result.Status.ToString();
            r.Message = result.Msg;
            lock (AppConfigContext.LockObject)
            {
                // 删除缓存及数据库中的任务
                SchedulerCenter.ScheduleList.Remove(key);
            }
            schedule.IsDelete = true;
            // 更新删除数据库中的任务状态
            await _store.UpdateAsync(schedule, cancellationToken);
            return r;
        }

        /// <summary>
        /// 根据大于等于当前时间的所有未删除的任务
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<ScheduleEntity>> GetAllSchedules()
        {
            var q = _store.GetSchedules().Where(it => !it.IsDelete  && it.RunStep == EnumType.JobStep.Planned && it.RunStatus != EnumType.JobRunStatus.Finish && it.CreateTime < DateTime.Now);

            var schedules = await q.ToListAsync();
            return schedules;
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> UpdateCreateAsync(List<ScheduleEntity> schedules, CancellationToken  cancellationToken = default(CancellationToken))
        {
            // 更新任务
            return  await _store.UpdateAsync(schedules, cancellationToken);
        }
    }
}
