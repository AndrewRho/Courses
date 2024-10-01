using Courses.Abstractions;
using Courses.Configs;
using Microsoft.Extensions.Options;

namespace Courses.Data;

public class CoursesBotContextFactory : ICoursesBotContextFactory
{
    private readonly BotConfig _config;

    public CoursesBotContextFactory(IOptions<BotConfig> config)
    {
        _config = config.Value;
    }

    public CoursesBotContext GetContext()
    {
        return new CoursesBotContext(_config.ConnString);
    }
}