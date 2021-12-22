using ApiCore.JsonFilter;
using ApiCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using SchedulingCenter.Managers;
using SchedulingCenter.Stores;

namespace SchedulingCenter.Util
{
    /// <summary>
    /// 服务扩展集合
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddDefined(this IServiceCollection services) {
            // 初始化任务调度信息
            services.AddScoped<IScheduleStore, ScheduleStore>();

            services.AddScoped<ScheduleManager>();
            services.AddScoped<InitManager>();
            //services.AddHttpContextAccessor();
            //services.AddHttpClient();
            //services.AddSingleton<IJsonHelper, JsonHelper>();
            //services.AddSingleton<HttpClientActuator>();
            services.AddUserDefined();
            // 服务注入完成后添加服务管道
            AppConfigContext.ServiceProvider = services.BuildServiceProvider();
        }
    }
}
