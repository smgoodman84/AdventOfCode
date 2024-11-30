using System.Text;

namespace AdventOfCode.Shared.PrimitiveExtensions;

public static class ByteArrayExtensions
{
    public static string ToHexString(this byte[] bytes)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }
}