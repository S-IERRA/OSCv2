namespace Shared.Constants;


public enum Events
{
    Null = 0,

    Identified = 1,
    LoggedOut = 2,
    Registered = 3,

    ServerCreated = 5,
    ServerUpdated = 7,
    ServerJoined = 11,
    ServerLeft = 8,
    ServerInviteCreated = 12,

    JoinedServer = 6,
    UserIsBanned = 10,

    MessageReceived = 4,
    MessageUpdated = 8,
    MessageDeleted = 9,

    MessagesRequested,
    ChannelsRequested,
}