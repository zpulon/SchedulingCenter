using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingCenter.Models
{

    /// <summary>
    /// 任务调度实体
    /// </summary>
    public class ScheduleEntity {
        /// <summary>
        /// 任务标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }


        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 执行表达式
        /// </summary>
        public string CronStr { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EnumType.JobStatus Status { get; set; }

        /// <summary>
        /// 任务运行状态
        /// </summary>
        public EnumType.JobRunStatus RunStatus { get; set; }

        /// <summary>
        /// 任务运行步骤
        /// </summary>
        public EnumType.JobStep RunStep { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextTime { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public DateTime? StarRunTime { get; set; }

        /// <summary>
        /// 停止时间
        /// </summary>
        public DateTime? EndRunTime { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 执行器地址（IP，端口号）
        /// </summary>
        public string ActuatorAddress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

    }
}
