using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchedulingCenter.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quartz_schedule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CronStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RunStatus = table.Column<int>(type: "int", nullable: false),
                    RunStep = table.Column<int>(type: "int", nullable: false),
                    NextTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StarRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Args = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Callback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActuatorAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quartz_schedule", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quartz_schedule");
        }
    }
}
