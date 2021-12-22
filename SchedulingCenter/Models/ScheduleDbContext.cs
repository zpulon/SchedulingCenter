using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingCenter.Models
{
    public class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)
           : base(options)
        {
        }

        /// <summary>
        /// 调度任务
        /// </summary>
        public DbSet<ScheduleEntity> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ScheduleEntity>(it => {
                it.ToTable("quartz_schedule");
            });
        }

       

        
    }
}
