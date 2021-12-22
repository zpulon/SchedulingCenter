using ApiCore.JsonFilter;
using LogCore.Filters;
using LogCore.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SchedulingCenter.Managers;
using SchedulingCenter.Models;
using SchedulingCenter.Util;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SchedulingCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            LogLevels logLevel = LogLevels.Info;
            int maxDays = 7;
            IConfigurationSection logConfig = Configuration.GetSection("Log");
            string maxFileSize = "10MB";
            if (logConfig != null)
            {
                Enum.TryParse(logConfig["Level"] ?? "", out logLevel);
                int.TryParse(logConfig["SaveDays"], out maxDays);
                maxFileSize = logConfig["MaxFileSize"];
                if (string.IsNullOrEmpty(maxFileSize))
                {
                    maxFileSize = "10MB";
                }
            }
            string logFolder = Path.Combine(AppContext.BaseDirectory, "Logs");
            LoggerManager.InitLogger(new LogConfig
            {
                LogBaseDir = logFolder,
                MaxFileSize = maxFileSize,
                LogLevels = logLevel,
                IsAsync = true,
                LogFileTemplate = LogFileTemplates.PerDayDirAndLogger,
                LogContentTemplate = LogLayoutTemplates.SimpleLayout
            });
            LoggerManager.SetLoggerAboveLevels(logLevel);
            LoggerManager.StartClear(maxDays, logFolder, LoggerManager.GetLogger("clear"));
            services.AddSingleton(Configuration as IConfigurationRoot);
            //services.AddControllersWithViews().AddJsonOptions(options => {
            //    //����ʱ���ʽ
            //    options.JsonSerializerOptions.Converters.Add(new DateJsonConverter("yyyy-MM-dd HH:mm:ss", DateTimeZoneHandling.Local));
            //    //��ʹ���շ���ʽ��key
            //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    //��ʹ���շ���ʽ��key
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            //    //��ȡ������Ҫ��ת���ַ���ʱʹ�õı�����
            //    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            //}).AddNewtonsoftJson(options => {
            //    //options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            //    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            //    //options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //    //options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //    //options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //});
            services.AddControllers().AddJsonOptions(options =>
            {
                //����ʱ���ʽ
                options.JsonSerializerOptions.Converters.Add(new DateJsonConverter("yyyy-MM-dd HH:mm:ss", DateTimeZoneHandling.Local));
                //��ʹ���շ���ʽ��key
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //��ʹ���շ���ʽ��key
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                //��ȡ������Ҫ��ת���ַ���ʱʹ�õı�����
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            }).AddNewtonsoftJson(options => {
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            });
            
            services.AddSwaggerGen(it => {
                it.SwaggerDoc("v1", new OpenApiInfo { Title = "�����������v1", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "SchedulingCenter.xml");
                it.IncludeXmlComments(filePath);
            })
            .AddMvcCore()
            .AddApiExplorer();
  
            services.AddDbContextPool<ScheduleDbContext>(options => {
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
            });
            services.AddDefined();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new LoggerProvider());
            // ��ӹܵ���־
            //app.UseMiddleware(typeof(ApplicationLogMiddleware));
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "�����������v1");
                options.RoutePrefix = string.Empty;
            });
           // app.ApplicationLog();
            app.UseRouting();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Check}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
           

            try
            {
                Console.WriteLine($"��ʼ����ʷ����...");
                // ��ʼ����ʷ����
                var initManager = serviceProvider.GetService<InitManager>();
                initManager.LoadDataAsync().GetAwaiter().GetResult();
                Console.WriteLine("��ʼ����ʷ�������");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"��ʼ����ʷ����ʧ�ܣ�����{ex.ToString()}");
            }
        }
    }
}
