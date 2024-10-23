using Courses.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Courses.Data;

public class CoursesBotContext : DbContext
{
    private readonly string _connString;
    public DbSet<TimeSlotEntity> TimeSlots { get; protected set; } = default!;
    public DbSet<UserEntity> Users { get; protected set; } = default!;
    public DbSet<ScheduleEntity> Schedules { get; protected set; } = default!;
    public DbSet<DisciplineEntity> Disciplines { get; protected set; } = default!;
    public DbSet<TopicEntity> Topics { get; protected set; } = default!;
    public DbSet<WorkPlanEntity> WorkPlans { get; protected set; } = default!; 

    public CoursesBotContext(string connString)
    {
        _connString = connString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connString);
    }
}