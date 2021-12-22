using ApiCore.JsonFilter;
using LogCore.Log;
using SchedulingCenter.Managers.Quartz.Net;
using SchedulingCenter.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulingCenter.Managers
{
    /// <summary>
    /// 数据初始化
    /// </summary>
    public class InitManager {
        protected ScheduleManager ScheduleManager { get; }
        private readonly IJsonHelper ijsonHelper;
        private readonly ILogger _logger = LoggerManager.GetLogger("InitManager");
        public InitManager (ScheduleManager scheduleManager, IJsonHelper jsonHelper) {
            ScheduleManager = scheduleManager;
            ijsonHelper = jsonHelper;
        }

        /// <summary>
        /// 加载历史数据
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync () {
            try {
                if (SchedulerCenter.ScheduleList.Count > 0) {
                    Console.WriteLine($"现有任务列表：{ijsonHelper.ToJson(SchedulerCenter.ScheduleList)} ");
                }
                var schedules =await ScheduleManager.GetAllSchedules();
                if (schedules == null || !schedules.Any()) return;
                for (int i = 0; i < schedules.Count; i++) {
                    var item = schedules[i];
                    var key = $"{item.JobGroup}{item.JobName}";
                    Console.WriteLine($"正在添加的Key：{key}");
                    if (SchedulerCenter.ScheduleList.ContainsKey(key)) continue;
                    // 添加任务运行
                    SchedulerCenter.ScheduleList.Add (key, item);
                    var result =await SchedulerCenter.Instance.RunScheduleJob<JobActuator> (item.JobGroup, item.JobName);
                    if (result.Status == 0) {
                        item.Status = EnumType.JobStatus.Opened;
                    }
                }
                ScheduleManager.UpdateCreateAsync(schedules).GetAwaiter(); // 更新状态
                Console.WriteLine($"任务初始化完成,所有任务信息：{ijsonHelper.ToJson(SchedulerCenter.ScheduleList)} ");
            } catch (Exception ex) {
                _logger.Error ($"任务初始化失败，错误：{ex.ToString()}");
            }
        }
    }
}