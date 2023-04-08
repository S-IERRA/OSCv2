using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Shared;
using Shared.Constants;

namespace OSCv2_Client.Logic;

//ToDo: Convert the client and the server to aa shared file to inherit from
//ToDo: Convert reply tasks to a cached task, kafka seems good in this situation
public class WebSocketHandler
{
    private static readonly Socket Client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Create();
    private readonly Dictionary<uint, TaskCompletionSource<WebSocketMessage>> _replyTasks = new();

    //Move this to a user config file
    private static Guid _userSession { get; set; }
    private static uint PacketIndex = 1;

    private async void ReceiveMessages()
    {
        byte[] localBuffer = new byte[512];

        for (;;)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(512);

            do
            {
                int received = await Client.ReceiveAsync(new Memory<byte>(buffer), SocketFlags.None);
                if (received < 512 )
                    break;
                
                Array.Resize(ref buffer, buffer.Length + 512);
            } 
            while (Client.Available > 0);

            List<Packet?> packets = await GZip.Decompress(buffer);
            
            ArrayPool.Return(buffer);

            foreach (var packet in packets)
            {
                if (packet is null)
                    continue;
                
                PacketIndex++;

                if (!JsonHelper.TryDeserialize<WebSocketMessage>(packet.Message, out var socketMessage))
                {
                    Console.WriteLine("Invalid message");
                    continue;
                }

                if (_replyTasks.TryGetValue(packet.ReplyId, out var replyTask))
                {
                    _replyTasks.Remove(packet.ReplyId);
                    replyTask.SetResult(socketMessage);
                }

                if (socketMessage.WebSocketOpCode == WebSocketOpCodes.HeartBeat)
                    await Send(WebSocketOpCodes.HeartBeatAck);

                if (socketMessage.EventType is not null)
                    Console.WriteLine(socketMessage.Message);
            }
        }
    }

    public WebSocketHandler()
    {
        Client.DontFragment = true;

        //Resolve dns
        //IPAddress addresses = Dns.GetHostAddresses("0.tcp.eu.ngrok.io")[0];
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8787);

        Client.Connect(endPoint);
        ReceiveMessages();
    }

    private async Task SendData(WebSocketOpCodes webSocketOpCode, Events? eventType = null, string? dataSerialized = default)
    {
        if (!Client.Connected)
            return;

        WebSocketMessage message = new(webSocketOpCode, dataSerialized, eventType, _userSession);

        string messageSerialized = JsonSerializer.Serialize(message);
        byte[] dataCompressed = GZip.Compress(messageSerialized, PacketIndex);

        await Client.SendAsync(dataCompressed, SocketFlags.None);
    }

    private async Task<WebSocketMessage?> SendWithReply(WebSocketOpCodes webSocketOpCode, Events? eventType = null,
        string? dataSerialized = default)
    {
        if (!Client.Connected)
            return null;

        WebSocketMessage message = new(webSocketOpCode, dataSerialized, eventType, _userSession);
        string socketMessageSerialized = JsonSerializer.Serialize(message);

        byte[] dataCompressed = GZip.Compress(socketMessageSerialized, PacketIndex);

        var replyTask = new TaskCompletionSource<WebSocketMessage>();
        _replyTasks[PacketIndex] = replyTask;

        await Client.SendAsync(dataCompressed, SocketFlags.None);

        return await replyTask.Task;
    }

    public async Task Send(WebSocketOpCodes webSocketOpCode)
        => await SendData(webSocketOpCode);

    public async Task Send(Events eventType)
        => await SendData(WebSocketOpCodes.Event, eventType);

    public async Task Send(WebSocketOpCodes webSocketOpCode, Events eventType)
        => await SendData(webSocketOpCode, eventType);

    public async Task Send(WebSocketOpCodes webSocketOpCode, string? message)
        => await SendData(webSocketOpCode, null, message);

    public async Task<WebSocketMessage?> SendReply(WebSocketOpCodes webSocketOpCode)
        => await SendWithReply(webSocketOpCode);

    public async Task<WebSocketMessage?> SendReply(Events eventType)
        => await SendWithReply(WebSocketOpCodes.Event, eventType);

    public async Task<WebSocketMessage?> SendReply(WebSocketOpCodes webSocketOpCode, Events eventType)
        => await SendWithReply(webSocketOpCode, eventType);

    public async Task<WebSocketMessage?> SendReply(WebSocketOpCodes webSocketOpCode, string? message)
        => await SendWithReply(webSocketOpCode, null, message);

    public async Task<WebSocketMessage?> SendReply(WebSocketOpCodes webSocketOpCode, object message)
    {
        string jsonMessage = JsonSerializer.Serialize(message);

        return await SendWithReply(webSocketOpCode, null, jsonMessage);
    }

    public async Task<WebSocketMessage?> SendReply(Events eventType, object message)
    {
        string jsonMessage = JsonSerializer.Serialize(message);

        return await SendWithReply(WebSocketOpCodes.Event, eventType, jsonMessage);
    }

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