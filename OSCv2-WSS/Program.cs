using OSCv2_WS.Logic.Websocket;
using OSCv2_WS.Objects;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341/")
    .CreateLogger();
    
Log.Information("Server started");