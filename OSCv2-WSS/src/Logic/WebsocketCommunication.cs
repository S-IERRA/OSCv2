using System.Net;
using System.Text.Json;
using OSCv2_WS.Logic.Websocket;
using Shared;
using Shared.Constants.RedisCommunication;
using StackExchange.Redis;

namespace OSCv2_WS.Logic;

public class WebsocketCommunication
{
    public static readonly WebSocketServer SocketServer = new WebSocketServer();
    
    private static readonly ConnectionMultiplexer CacheClient = ConnectionMultiplexer.Connect("127.0.0.1:6379");
    private static readonly ISubscriber       RedisSubscriber = CacheClient.GetSubscriber();

    private const string GeneralChannel = "general";

    //ToDo: add replies
    public WebsocketCommunication()
    {
        ChannelMessageQueue messageQueue = RedisSubscriber.Subscribe(GeneralChannel);
        messageQueue.OnMessage(message =>
        {
            if (!JsonHelper.TryDeserialize<TransferMessage>(message.ToString(), out var transferMessage))
            {
                //RedisSubscriber.PublishAsync()
                return;
            }

            if (!Guid.TryParse(transferMessage.Data, out var sessionId))
                return;

            switch (transferMessage.OpCodes)
            {
                case RedisOpCodes.Login:
                    //ToDo: use a try method
                    IPEndPoint ipEndPoint = IPEndPoint.Parse(transferMessage.Data);
                    _ = SocketServer.TryConnect(ipEndPoint);
                    break;

                
            }
        });
    }

    public static async Task SendAsync(Guid sessionId, string message)
    {
        var transferMessage = new TransferMessage(RedisOpCodes.Event, sessionId, message);
        string serializedData = JsonSerializer.Serialize(transferMessage);

        await RedisSubscriber.PublishAsync(GeneralChannel, serializedData);
    }
}