var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(e => e.MapControllers());
app.Run();