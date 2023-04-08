using System.Runtime.InteropServices;

namespace Shared.Constants;

public record WebSocketMessage(WebSocketOpCodes WebSocketOpCode, string? Message, Events? EventType, Guid? Session);

[StructLayout(LayoutKind.Sequential)]
public record Packet(uint Id, uint ReplyId, int Length, string Message);