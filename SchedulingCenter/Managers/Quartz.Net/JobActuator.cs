using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCore.Utils;
using SchedulingCenter.Stores;
using Quartz;
using LogCore.Log;
using SchedulingCenter.Util;
using ApiCore.JsonFilter;
using SchedulingCenter.DTO.Response;

namespace SchedulingCenter.Managers.Quartz.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class JobActuator : IJob {
        private readonly ILogger _logger = LoggerManager.GetLogger("JobActuator");

        private HttpClientActuator _client;

        private IScheduleStore _store;
        private  IJsonHelper ijsonHelper;
        /// <summary>
        /// 
        /// </summary>
        public JobActuator() {
            ijsonHelper  = AppConfigContext.ServiceProvider.GetService(typeof(IJsonHelper)) as IJsonHelper;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context){
            using (var scope = AppConfigContext.ServiceProvider.CreateScope())
            {
                _store = scope.ServiceProvider.GetService<IScheduleStore>();
                _client = scope.ServiceProvider.GetService<HttpClientActuator>();
                await ExecuteJob(context);
            }

        }
        private async Task ExecuteJob(IJobExecutionContext context) {
            try {
                _logger.Info($"任务执行器开始工作Execute(IJobExecutionContext context)，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}，下次执行时间：{(context.NextFireTimeUtc == null ? "无" : context.NextFireTimeUtc.Value.ToString())}");
                var key = $"{context.JobDetail.Key.Group}{context.JobDetail.Key.Name}";
                // 该任务是否有下一次执行
                var executeStatus = context.NextFireTimeUtc != null && context.NextFireTimeUtc.Value.ToLocalTime() > DateTime.Now;
                if (SchedulerCenter.ScheduleList == null || !SchedulerCenter.ScheduleList.ContainsKey(key)) {
                    _logger.Info($"任务执行器未找到任务主键，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}");
                    return;
                }

                var schedule = SchedulerCenter.ScheduleList[key];
                schedule.RunStatus = Models.EnumType.JobRunStatus.Running;
                schedule.UpdateTime = DateTime.Now;
                await _store.UpdateAsync(schedule, context.CancellationToken);

                var callback = schedule.Callback;
                if (string.IsNullOrEmpty(callback)) {
                    _logger.Error($"任务执行器未找到任务回调地址，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}");
                };
                var args = schedule.Args;
                // 回调参数 重新提交
                var response = new ScheduleExecuteResponse {
                    Callback = callback,
                    JobGroup = context.JobDetail.Key.Group,
                    JobName = context.JobDetail.Key.Name,
                    Args = ijsonHelper.ToObject<Dictionary<string, object>>(args)
                };
                
                // 任务失败后回调3次，3次均失败时放弃处理
                var callCount = 0;
                // 执行完成后删除列表中的任务
                if (!executeStatus) {
                    // 管理列表对象时添加锁
                    lock (AppConfigContext.LockObject) {
                        // 删除缓存及数据库中的任务
                        SchedulerCenter.ScheduleList.Remove(key);
                    }
                }
                while (callCount < 3) {
                    try {
                        var result = await _client.Post(callback, ijsonHelper.ToJson(response))  ?? "";
                        _logger.Info($"任务执行器接收的回调消息为{result}，回调次数：{callCount + 1}，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}");
                        if (result.ToLower().Trim('"') == "success") {
                            schedule.RunStep = executeStatus ? Models.EnumType.JobStep.Planned : Models.EnumType.JobStep.Completed;
                            schedule.RunStatus = Models.EnumType.JobRunStatus.Finish;
                            schedule.UpdateTime = DateTime.Now;
                            await _store.UpdateAsync(schedule, context.CancellationToken);
                            _logger.Info($"任务执行器回调成功，任务状态更改，任务信息：{ijsonHelper.ToJson(schedule)}");
                            return; // 终止重试
                        }
                        _logger.Error($"任务执行器回调失败，客户端无响应，回调地址：{callback}，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}");
                    } catch (Exception ex) {
                        _logger.Error($"任务执行器回调失败，回调地址：{callback}，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group}错误：{ex.Message}跟踪：{ex.StackTrace}");
                    }
                    callCount++;
                }
            } catch (Exception ex) {
                _logger.Error($"任务执行器失败Execute(IJobExecutionContext context)，任务名称：{context.JobDetail.Key.Name}，任务分组：{context.JobDetail.Key.Group},错误信息:{ex.ToString()}");
            }
        }
    }
}
