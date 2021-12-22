using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingCenter.Models
{

    /// <summary>
    /// 枚举类型
    /// </summary>
    public class EnumType {
        public enum JobStatus {
            /// <summary>
            /// 已启用 
            /// </summary>
            Opened = 1,
            /// <summary>
            /// 已停止
            /// </summary>
            Stopped = 0,
        }

        public enum JobStep {
            /// <summary>
            /// 已完成
            /// </summary>
            Completed = 1,
            /// <summary>
            /// 执行任务计划中
            /// </summary>
            Planned = 2,
        }

        public enum JobRunStatus {
            /// <summary>
            /// 执行中
            /// </summary>
            Running = 1,
            /// <summary>
            /// 待运行
            /// </summary>
            Waiting = 0,

            /// <summary>
            /// 执行完成
            /// </summary>
            Finish = 2
        }
    }
}
