namespace Shared.Constants;

public enum WebSocketOpCodes
{
    //Server
    Event = 0,
    Hello = 1,
    HeartBeat = 3,
    SessionUpdate = 9,

    //Client
    Identify = 2,
    Register = 5,
    HeartBeatAck = 4,
    SendMessage = 6,

    CreateServer = 7,
    DeleteServer = 14,
    JoinServer = 8,
    LeaveServer = 13,
    RequestServers = 12,
    RequestChannelMessages = 15,
    CreateServerInvite = 16,
    GetServerMembers = 9,
    BanUser = 10,
    KickUser = 11,

    SubscribeServerMessages = 17,
    UnsubscribeServerMessages = 18,

    //Errors
    InvalidRequest = 400,
    Unauthorized = 401,
    TooManyRequests = 429,
    ConnectionClosed = 444,
    InternalServerError = 500
}
