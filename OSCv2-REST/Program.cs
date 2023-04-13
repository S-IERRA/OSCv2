using OSCv2.Logic;
using OSCv2.Logic.Database;
using Serilog;
using StackExchange.Redis;

/*
EntityFrameworkFactory factory = new();
var ctx = factory.CreateDbContext();
await ctx.Database.EnsureDeletedAsync();
await ctx.Database.EnsureCreatedAsync();
*/

//ToDo: move this else-where
var logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.Seq("http://localhost:5341") 
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
builder.Services.AddSingleton<ISubscriber>(x => x.GetRequiredService<ConnectionMultiplexer>().GetSubscriber());
builder.Services.AddScoped<IWebsocketCommunicationService, WebsocketCommunicationService>();

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