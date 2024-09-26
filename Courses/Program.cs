using Courses.Abstractions;
using Courses.BotHandlers;
using Courses.Configs;
using Courses.Implementations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMvc();

builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("BotConfig"));
builder.Services.AddSingleton<IBotHost, BotHost>();

builder.Services.AddTransient<IBotHandlerFactory, BotHandlerFactory>();
builder.Services.AddTransient<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddTransient<ITableRenderService, TableRenderService>();

var app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints(e => e.MapControllers());

var botHost = app.Services.GetRequiredService<IBotHost>();
botHost.Start();
app.Run();