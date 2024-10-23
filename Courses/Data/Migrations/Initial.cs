using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Courses.Data.Migrations;

[Migration("20241001_0_Initial")]
[DbContext(typeof(CoursesBotContext))]
public class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("TimeSlots", cb => new
            {
                Id = cb.Column<int>(),
                TimeFrom = cb.Column<string>(),
                TimeTo = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_TimeSlots", t => t.Id);
            });
        
        migrationBuilder.CreateTable("Users", cb => new
            {
                Id = cb.Column<long>(),
                UserName = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_Users", x => x.Id);
            });
        
        migrationBuilder.CreateTable( "Disciplines", cb => new
            {
                Id = cb.Column<Guid>(),
                Name = cb.Column<string>()
            }, 
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_Disciplines", x => x.Id);
            });

        migrationBuilder.CreateTable("Topics", cb => new
            {
                Id = cb.Column<Guid>(),
                DisciplineId = cb.Column<Guid>(),
                Number = cb.Column<int>(),
                Name = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_Topics", x => x.Id);
                ctb.ForeignKey("FK_Topics_Disciplines", x => x.DisciplineId, "Disciplines", "Id");
            });

        migrationBuilder.CreateTable("WorkPlans", cb => new
            {
                Id = cb.Column<Guid>(),
                UserId = cb.Column<long>(),
                TopicId = cb.Column<Guid>(),
                Lectures = cb.Column<int>(),
                Practices = cb.Column<int>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_workPlans", x => x.Id);
                ctb.ForeignKey("FK_workPlans_users", x => x.UserId, "Users", "Id");
                ctb.ForeignKey("FK_workPlans_topics", x => x.TopicId, "Topics", "Id");
            });

        migrationBuilder.CreateTable("Schedules", cb => new
            {
                Id = cb.Column<Guid>(),
                WorkPlanId = cb.Column<Guid>(),
                TimeSlotId = cb.Column<int>(),
                Lectures = cb.Column<int>(),
                Practices = cb.Column<int>(),
                Date = cb.Column<DateTime>(),
                Progress = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_schedules", x => x.Id);
                ctb.ForeignKey("FK_schedules_workPlans", x => x.WorkPlanId, "WorkPlans", "Id");
                ctb.ForeignKey("FK_schedules_timeSlot", x => x.TimeSlotId, "TimeSlots", "Id");
            });
    }
}