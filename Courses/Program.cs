using Courses;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints(e => e.MapControllers());

var host = new BotHost("***");
host.Start();


app.Run();