namespace Shared.Constants.RedisCommunication;

public record TransferMessage(RedisOpCodes OpCodes, Guid SessionId, string Data);

