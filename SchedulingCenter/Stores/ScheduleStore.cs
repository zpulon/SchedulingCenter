using Microsoft.EntityFrameworkCore;
using SchedulingCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingCenter.Stores
{
    /// <summary>
    /// 调度任务实现
    /// </summary>
    public class ScheduleStore : IScheduleStore
    {
        protected ScheduleDbContext Context { get; }

        /// <summary>
        /// 任务调度构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public ScheduleStore(ScheduleDbContext context) {
            Context = context;
        }

        /// <summary>
        /// 添加一组任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(List<ScheduleEntity> schedules, CancellationToken cancellationToken = default(CancellationToken)) {
            if (schedules == null || !schedules.Any()) throw new ArgumentNullException(nameof(schedules));

            Context.Schedules.AddRange(schedules);

            return await Context.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// 添加调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default(CancellationToken)) {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));

            Context.Schedules.Add(schedule);

            return await Context.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// 更新调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default(CancellationToken)) {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            if (schedule != null) {
                Context.Entry(schedule).State = EntityState.Deleted;
            }
            Context.Schedules.Attach(schedule);
            Context.Schedules.Update(schedule);

            return await Context.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// 更新调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(List<ScheduleEntity> schedules, CancellationToken cancellationToken = default(CancellationToken)) {
            if (schedules == null || !schedules.Any()) throw new ArgumentNullException(nameof(schedules));

            Context.Schedules.UpdateRange(schedules);

            return await Context.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// 获取调度任务
        /// </summary>
        /// <returns></returns>
        public IQueryable<ScheduleEntity> GetSchedules() {
            var q = from s in Context.Schedules.AsNoTracking()
                    select s;
            return q;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));

            Context.Schedules.Remove(schedule);

            return await Context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
