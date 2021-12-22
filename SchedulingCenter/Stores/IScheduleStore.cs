
using SchedulingCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingCenter.Stores
{
    /// <summary>
    /// 调度任务数据层接口
    /// </summary>
    public interface IScheduleStore
    {
        /// <summary>
        /// 添加一组任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        Task<bool> CreateAsync(List<ScheduleEntity> schedules, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 添加调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        Task<bool> CreateAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 删除调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 更新调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(ScheduleEntity schedule, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新调度任务
        /// </summary>
        /// <param name="schedules">调度任务</param>
        /// <param name="cancellationToken">操作标识</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(List<ScheduleEntity> schedules, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 获取调度任务
        /// </summary>
        /// <returns></returns>
        IQueryable<ScheduleEntity> GetSchedules();
    }
}
