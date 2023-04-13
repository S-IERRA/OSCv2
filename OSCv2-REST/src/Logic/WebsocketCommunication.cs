using System.Reflection.Emit;
using System.Text.Json;
using Shared;
using Shared.Constants.RedisCommunication;
using StackExchange.Redis;

namespace OSCv2.Logic;

//This is a very light transaction module, we use it mainly for data transactions from REST to WS, although it can support WS <-> REST fully
//ToDo: might be possible to convert the message-queue to a  hangfire task and have it as a long running task there

public interface IWebsocketCommunicationService
{
    Task SendAsync(Guid sessionId, string message);
    Task SendAsync(RedisOpCodes opCode, Guid sessionId);
    Task SendAsync(RedisOpCodes opCode, Guid sessionId, string message);
}

public class WebsocketCommunicationService : IWebsocketCommunicationService
{
    private readonly ISubscriber _redisSubscriber;
    private const string GeneralChannel = "general";

    public WebsocketCommunicationService(ISubscriber redisSubscriber)
    {
        _redisSubscriber = redisSubscriber;
        ChannelMessageQueue messageQueue = _redisSubscriber.Subscribe(GeneralChannel);
        messageQueue.OnMessage(async message =>
        {
            //Currently no messages to handle
            if (!JsonHelper.TryDeserialize<TransferMessage>(message.Message, out var transferMessage))
            {
                await _redisSubscriber.PublishAsync(GeneralChannel, message.ToString());
                return;
            }
        });
    }

    public async Task SendAsync(RedisOpCodes opCode, Guid sessionId, string message)
    {
        var transferMessage = new TransferMessage(opCode, sessionId, message);
        string serializedData = JsonSerializer.Serialize(transferMessage);

        await _redisSubscriber.PublishAsync(GeneralChannel, serializedData);
    }

    public async Task SendAsync(Guid sessionId, string message) =>
        await SendAsync(RedisOpCodes.Event, sessionId, message);

    public async Task SendAsync(RedisOpCodes opCode, Guid sessionId) =>
        await SendAsync(opCode, sessionId, "");
}