using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchedulingCenter.DTO.Request
{
    /// <summary>
    /// 任务调度添加请求体
    /// </summary>
    public class QuartzEntityRequest
    {
        /// <summary>
        /// 任务分组（JobGroup + JobName 唯一确定一个任务）（*必传）
        /// </summary>
        [Required(ErrorMessage = "任务分组标识不能为空")]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务名称（*必传）
        /// </summary>
        [Required(ErrorMessage = "任务名称不能为空")]
        public string JobName { get; set; }

        /// <summary>
        /// 执行表达式（任务执行的时间及方式，例：0 * * * * ? *表示每分钟执行一次），
        /// CronStr为空时任务只在开始时间内执行一次
        /// </summary>
        public string CronStr { get; set; }

        /// <summary>
        /// 运行时间，如为空则默认当前时间
        /// </summary>
        public DateTime? StarRunTime { get; set; }

        /// <summary>
        /// 任务停止时间，为空则任务不停止
        /// </summary>
        public DateTime? EndRunTime { get; set; }

        /// <summary>
        /// 回调地址（*必传）
        /// </summary>
        [Required(ErrorMessage = "回调地址不能为空")]
        public string Callback { get; set; }

        /// <summary>
        /// 参数（自定义参数，回调时原样返回）
        /// </summary>
        public Dictionary<string, object> Args { get; set; }
    }
}
