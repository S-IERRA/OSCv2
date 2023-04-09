using System.Text.Json;
using Shared;
using Shared.Constants.RedisCommunication;
using StackExchange.Redis;

namespace OSCv2.Logic;

//This is a very light transaction module, we use it mainly for data transactions from REST to WS, although it can support WS <-> REST fully
//ToDo: might be possible to convert the message-queue to a  hangfire task and have it as a long running task there

public class WebsocketCommunication
{
    private static readonly ConnectionMultiplexer CacheClient = ConnectionMultiplexer.Connect("127.0.0.1:6379");
    private static readonly ISubscriber       RedisSubscriber = CacheClient.GetSubscriber();

    private const string GeneralChannel = "general";

    //General message channel handling, i.e login/logout
    public WebsocketCommunication()
    {
        ChannelMessageQueue messageQueue = RedisSubscriber.Subscribe(GeneralChannel);
        messageQueue.OnMessage(message =>
        {
            //Currently no messages to handle
            if (!JsonHelper.TryDeserialize<TransferMessage>(message.ToString(), out var transferMessage))
            {
                //RedisSubscriber.PublishAsync()
                return;
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