using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchedulingCenter.DTO.Request
{
    /// <summary>
    /// 停止一个任务
    /// </summary>
    public class StopScheduleRequest
    {
        /// <summary>
        /// 任务分组
        /// </summary>
        [Required(ErrorMessage = "任务分组不能为空")]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Required(ErrorMessage = "任务名称不能为空")]
        public string JobName { get; set; }

    }
}
