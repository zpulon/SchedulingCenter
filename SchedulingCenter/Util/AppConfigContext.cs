using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulingCenter.Util
{
    public class AppConfigContext
    {
        /// <summary>
        /// 服务管道
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 静态锁对象
        /// </summary>
        public static object LockObject = new object();
    }
}
