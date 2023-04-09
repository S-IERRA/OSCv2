using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Constants;

namespace Shared;

public static class GZip
{
    //[ID][REPLYID=0][LENGTH][DATA]
    public static byte[] Compress(string data, uint id, uint replyId = 0)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] idBytes = BitConverter.GetBytes(id);
        byte[] replyIdBytes = BitConverter.GetBytes(replyId);
        byte[] lengthBytes = BitConverter.GetBytes(dataBytes.Length);

        byte[] prepended = new byte[idBytes.Length + replyIdBytes.Length + lengthBytes.Length + dataBytes.Length];
        Buffer.BlockCopy(idBytes, 0, prepended, 0, idBytes.Length);
        Buffer.BlockCopy(replyIdBytes, 0, prepended, idBytes.Length, replyIdBytes.Length);
        Buffer.BlockCopy(lengthBytes, 0, prepended, idBytes.Length + replyIdBytes.Length, lengthBytes.Length);
        Buffer.BlockCopy(dataBytes, 0, prepended, idBytes.Length + replyIdBytes.Length + lengthBytes.Length, dataBytes.Length);

        using var ms = new MemoryStream();
        using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            zip.Write(prepended, 0, prepended.Length);

        return ms.ToArray();
    }

    public static async Task<List<Packet?>> Decompress(byte[] inBytes)
    {
        using var  inStream = new MemoryStream(inBytes);
        await using var zip = new GZipStream(inStream, CompressionMode.Decompress);
        using var outStream = new MemoryStream();

        var buffer = new Memory<byte>(new byte[4096]);
        int read;

        while ((read = await zip.ReadAsync(buffer)) > 0)
            await outStream.WriteAsync(buffer[..read]);

        byte[] decompressedBytes = outStream.ToArray();
        var packets = new List<Packet?>();

        int packetSize = Marshal.SizeOf<Packet>();

        for (int totalRead = 0; decompressedBytes.Length - totalRead >= packetSize; totalRead += packetSize)
        {
            Packet? packet;
            unsafe
            {
                fixed (byte* pBuffer = &decompressedBytes[totalRead])
                {
                    packet = Marshal.PtrToStructure<Packet>((IntPtr)pBuffer);
                }
            }
            packets.Add(packet);
        }

        return packets;
    }
}