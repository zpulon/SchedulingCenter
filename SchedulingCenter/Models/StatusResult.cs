using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingCenter.Models
{
    public class StatusResult {

        /// <summary>
        /// 0-成功，500-失败
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Msg { get; set; }
    }
}
