using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Shared;

public static class JsonHelper
{
    public static bool TryDeserialize<TClass>(string? message, [NotNullWhen(true)] out TClass? result)
    {
        result = default;
        try
        {
            if (message is null)
                return false;

            using var document = JsonDocument.Parse(message);

            if (document.RootElement.TryGetProperty("Error", out _))
                return false;

            result = JsonSerializer.Deserialize<TClass>(message)!;
            return true;
        }
        catch
        {
            return false;
        }
    }
}