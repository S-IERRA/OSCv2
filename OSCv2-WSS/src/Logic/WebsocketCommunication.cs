using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using OSCv2_WS.Logic.Websocket;
using OSCv2_WS.Objects;
using Shared;
using Shared.Constants;
using Shared.Constants.RedisCommunication;
using StackExchange.Redis;

namespace OSCv2_WS.Logic;

//ToDo: should be able to interface this with a custom event handler member
public class WebsocketCommunication
{
    public static readonly WebSocketServer SocketServer = new WebSocketServer();
    
    private static readonly ConnectionMultiplexer CacheClient = ConnectionMultiplexer.Connect("127.0.0.1:6379");
    private static readonly ISubscriber       RedisSubscriber = CacheClient.GetSubscriber();

    private static readonly Dictionary<Guid, SocketUser> ClientList = new Dictionary<Guid, SocketUser>();

    private const string GeneralChannel = "general";

    //ToDo: add replies
    public WebsocketCommunication()
    {
        ChannelMessageQueue messageQueue = RedisSubscriber.Subscribe(GeneralChannel);
        messageQueue.OnMessage(message =>
        {
            if (!JsonHelper.TryDeserialize<TransferMessage>(message.ToString(), out var transferMessage))
                return;

            if (!Guid.TryParse(transferMessage.Data, out var sessionId))
                return;

            switch (transferMessage.OpCodes)
            {
                //ToDo: once heartbeat is moved, might have to start it here
                case RedisOpCodes.Login:
                {
                    if (!IPEndPoint.TryParse(transferMessage.Data, out var ipEndPoint))
                        return;

                    Socket? socket = SocketServer.TryConnect(ipEndPoint);
                    if (socket is null)
                        return;

                    var socketUser = new SocketUser(socket);
                    ClientList.Add(sessionId, socketUser);

                    _ =socketUser.Send(WebSocketOpCodes.Hello);

                    break;
                }

                case RedisOpCodes.Logout:
                {
                    SocketUser socket = ClientList.FirstOrDefault(x => x.Key == sessionId).Value;
                    socket.Dispose(); //Closes and disposes the connection

                    break;
                }

                case RedisOpCodes.Event:
                {
                    SocketUser socket = ClientList.FirstOrDefault(x => x.Key == sessionId).Value;
                    _ = socket.Send(WebSocketOpCodes.Event, transferMessage.Data);

                    break;
                }
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