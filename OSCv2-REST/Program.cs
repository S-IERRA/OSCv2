using OSCv2.Logic.Database;
using Serilog;

/*
EntityFrameworkFactory factory = new();
var ctx = factory.CreateDbContext();
await ctx.Database.EnsureCreatedAsync();
*/

//ToDo: move this else-where
var logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.Enrich.FromLogContext()
	.WriteTo.Seq("http://localhost:5341") 
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(l 
	=> l.ClearProviders()
		.AddSerilog());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();