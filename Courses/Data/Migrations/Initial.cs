using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Courses.Data.Migrations;

[Migration("20241001_0_Initial")]
[DbContext(typeof(CoursesBotContext))]
public class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("timeSlots", cb => new
            {
                id = cb.Column<int>(),
                timeFrom = cb.Column<string>(),
                timeTo = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_timeSlots", t => t.id);
            });
        
        migrationBuilder.CreateTable("users", cb => new
            {
                id = cb.Column<long>(),
                userName = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_users", x => x.id);
            });
        
        migrationBuilder.CreateTable( "disciplines", cb => new
            {
                id = cb.Column<Guid>(),
                name = cb.Column<string>()
            }, 
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_disciplines", x => x.id);
            });

        migrationBuilder.CreateTable("topics", cb => new
            {
                id = cb.Column<Guid>(),
                disciplineId = cb.Column<Guid>(),
                number = cb.Column<int>(),
                name = cb.Column<string>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_topics", x => x.id);
                ctb.ForeignKey("FK_topics_disciplines", x => x.disciplineId, "disciplines", "id");
            });

        migrationBuilder.CreateTable("workPlans", cb => new
            {
                id = cb.Column<Guid>(),
                userId = cb.Column<long>(),
                topicId = cb.Column<Guid>(),
                lectures = cb.Column<int>(),
                practices = cb.Column<int>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_workPlans", x => x.id);
                ctb.ForeignKey("FK_workPlans_users", x => x.userId, "users", "id");
                ctb.ForeignKey("FK_workPlans_topics", x => x.topicId, "topics", "id");
            });

        migrationBuilder.CreateTable("schedules", cb => new
            {
                id = cb.Column<Guid>(),
                workPlanId = cb.Column<Guid>(),
                timeSlotId = cb.Column<int>(),
                lectures = cb.Column<int>(),
                practices = cb.Column<int>(),
                date = cb.Column<DateTime>()
            },
            constraints: ctb =>
            {
                ctb.PrimaryKey("PK_schedules", x => x.id);
                ctb.ForeignKey("FK_schedules_workPlans", x => x.workPlanId, "workPlans", "id");
                ctb.ForeignKey("FK_schedules_timeSlot", x => x.timeSlotId, "timeSlots", "id");
            });
    }
}