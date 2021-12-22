using ApiCore.Basic;
using ApiCore.Filters;
using ApiCore.JsonFilter;
using LogCore.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchedulingCenter.DTO.Request;
using SchedulingCenter.Managers;
using SchedulingCenter.Util;
using System;
using System.Threading.Tasks;

namespace SchedulingCenter.Controllers
{
    [Produces("application/json")]
    [Route("api/scheduler")]
    public class SchedulerController : BaseController
    {
        private readonly ScheduleManager _scheduleManager;
        private readonly IJsonHelper ijsonHelper;
        private readonly LogCore.Log.ILogger _logger = LoggerManager.GetLogger("SchedulerController");
        public SchedulerController(ScheduleManager scheduleManager,  IJsonHelper jsonHelper)
        {
            ijsonHelper = jsonHelper;
            _scheduleManager = scheduleManager ?? throw new ArgumentNullException(nameof(scheduleManager));
        }

        /// <summary>
        /// 添加并运行任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("start")]
        [AllowAnonymous]
        public async Task<ResponseMessage> Submit([FromBody] QuartzEntityRequest request)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                _logger.Warn($"收到调度任务请求(Submit)模型验证失败：{response.Message ?? ""}，请求参数为：" + (request != null ? ijsonHelper.ToJson(request) : ""));
                return response;
            }
            try
            {
                _logger.Trace("收到调度任务请求(Submit)，请求体为：" + (request != null ? ijsonHelper.ToJson(request) : ""));
                var remote = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}";
                await _scheduleManager.AddSchedule(request, remote, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                _logger.Error($"调度任务（任务名称：{request.JobName}，任务分组：{request.JobGroup}）向调度中心添加(Submit)报错：{e}");
            }
            return response;
        }

        /// <summary>
        /// 停止并选择删除运行任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("stop")]
        [AllowAnonymous]
        public async Task<ResponseMessage> StopSchedule([FromBody] StopScheduleRequest request)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                _logger.Warn($"收调度任务停止请求(StopSchedule)模型验证失败：{response.Message ?? ""}，请求参数为：" + (request != null ? ijsonHelper.ToJson(request) : ""));
                return response;
            }
            try
            {
                _logger.Trace("收到调度任务停止请求(StopSchedule)，请求体为：" + (request != null ? ijsonHelper.ToJson(request) : ""));
                await _scheduleManager.StopSchedule(request, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                _logger.Error($"调度任务停止失败（任务名称：{request.JobName}，任务分组：{request.JobGroup}）错误：{e}");
            }
            return response;
        }

    }
}
