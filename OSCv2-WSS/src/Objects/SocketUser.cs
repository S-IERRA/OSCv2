using System.Net.Sockets;
using System.Text.Json;
using Shared;
using Shared.Constants;

namespace OSCv2_WS.Objects;

public record SocketUser(Socket UnderSocket) : IDisposable 
{
    public readonly CancellationTokenSource UserCancellation = new CancellationTokenSource();

    //public IPAddress UserIp { get; set; 
    public bool IsIdentified = false;
    public Guid? SessionId;
        
    private uint _packetId = 1;
    public uint ReplyId = 1;
        
    public void Dispose()
    {
        UserCancellation.Cancel();
        UnderSocket.Close();
            
        UserCancellation.Dispose();

        GC.SuppressFinalize(this);
    }

    private async Task SendData(WebSocketOpCodes webSocketOpCode, Events? eventType = null, string? dataSerialized = default, uint replyId = 0)
    {
        if (!UnderSocket.Connected)
            Dispose();
            
        WebSocketMessage socketMessage = new(webSocketOpCode, dataSerialized, eventType, default);

        string messageSerialized = JsonSerializer.Serialize(socketMessage);
            
        byte[] dataCompressed = GZip.Compress(messageSerialized, _packetId++, ReplyId);

        await UnderSocket.SendAsync(dataCompressed, SocketFlags.None);
    }

    public async Task Send(WebSocketOpCodes webSocketOpCode) 
        => await SendData(webSocketOpCode);
        
    public async Task Send(Events eventType)
        => await SendData(WebSocketOpCodes.Event, eventType);
        
    public async Task Send(WebSocketOpCodes webSocketOpCode, Events eventType) 
        => await SendData(webSocketOpCode, eventType);
        
    public async Task Send(WebSocketOpCodes webSocketOpCode, string message) 
        => await SendData(webSocketOpCode, null, message);

    public async Task Send(WebSocketOpCodes webSocketOpCode, object message)
    {
        string jsonMessage = JsonSerializer.Serialize(message);
            
        await SendData(webSocketOpCode, null, jsonMessage);
    }
        
    public async Task Send(Events eventType, object message)
    { 
        string jsonMessage = JsonSerializer.Serialize(message);
            
        await SendData(WebSocketOpCodes.Event, eventType, jsonMessage);
    }
}