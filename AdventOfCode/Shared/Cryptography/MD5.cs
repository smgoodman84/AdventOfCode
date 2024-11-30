using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode.Shared.Cryptography;

public static class MD5
{
    public static string GetMD5String(string input)
    {
        var bytes = GetMD5(input);
        return bytes.ToHexString();
    }

    public static byte[] GetMD5(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            return md5.ComputeHash(inputBytes);
        }
    }
}