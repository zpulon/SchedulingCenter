using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulingCenter.DTO.Response
{
    /// <summary>
    /// 任务执行回调
    /// </summary>
    public class ScheduleExecuteResponse
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }

        /// <summary>
        /// 参数，添加任务时用户自定参数
        /// </summary>
        public Dictionary<string, object> Args { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string Callback { get; set; }
    }
}
