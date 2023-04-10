using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OSCv2_WS.Objects;
using Serilog;

using Shared;
using Shared.Constants;

namespace OSCv2_WS.Logic.Websocket;

//ToDo: add rate-limit via redis
//ToDo: Microservice the heartbeat on redis and keep the bool value cached

public class WebSocketServer : IDisposable
{
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Create();

    private static readonly CancellationTokenSource Cts = new();

    private static readonly Socket Listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static readonly IPEndPoint EndPoint = new(IPAddress.Loopback, 8787);

    private SocketState _state = SocketState.Undefined;

    private bool CanRun() => !Cts.Token.IsCancellationRequested && _state is SocketState.Connected;

    private static DateTime GetCurrentTime => DateTime.Now;

    public WebSocketServer()
    {
        Listener.Bind(EndPoint);
        Listener.Listen(32);

        _state = SocketState.Connected;
    }

    public Socket? TryConnect(IPEndPoint endPoint)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(endPoint);

            var socketUser = new SocketUser(socket);
            _ = VirtualUserHandler(socketUser);
            return socket;
        }
        catch
        {
            socket.Close();
            socket.Dispose();
            return null;
        }
    }

    private async Task VirtualUserHandler(SocketUser socketUser)
    {
        bool receivedAck = false;
        
        async Task HeartBeat()
        {
            while (!socketUser.UserCancellation.IsCancellationRequested)
            {
                await socketUser.Send(WebSocketOpCodes.HeartBeat);

                DateTimeOffset nextAck = DateTimeOffset.Now + TimeSpan.FromSeconds(10);

                while (DateTimeOffset.Now < nextAck && !receivedAck)
                    await Task.Delay(2000);

                if (!receivedAck)
                {
                    //if(socketUser.IsIdentified)
                    //  await userDbService.LogOut();

                    await socketUser.Send(WebSocketOpCodes.ConnectionClosed);
                    socketUser.Dispose();
                    
                    return;
                }

                receivedAck = false;
                await Task.Delay(5000);
            }
        }
        
        //_ = Task.Run(HeartBeat, Cts.Token);

        while (socketUser.UserCancellation.IsCancellationRequested)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(512);

            do
            {
                int received = await socketUser.UnderSocket.ReceiveAsync(new Memory<byte>(buffer), SocketFlags.None);
                if (received < 512 )
                    break;
                
                Array.Resize(ref buffer, buffer.Length + 512);
            } 
            while (socketUser.UnderSocket.Available > 0);

            List<Packet?> packets = await GZip.Decompress(buffer);
            
            ArrayPool.Return(buffer);

            foreach (var packet in packets)
            {
                if (packet is null)
                    continue;
                
                if (!JsonHelper.TryDeserialize<WebSocketMessage>(packet.Message, out var socketMessage))
                {
                    await socketUser.Send(WebSocketOpCodes.InvalidRequest, ErrorMessages.MalformedJson);
                    continue;
                }
                
                switch (socketMessage.WebSocketOpCode)
                {
                    case WebSocketOpCodes.HeartBeatAck:
                    {
                        receivedAck = true;
                        break;
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        Listener.Dispose();
        Cts.Dispose();

        Log.Information("Server stopped");
        Log.CloseAndFlush();
    }

    ~WebSocketServer()
    {
        Dispose(false);
    }
}