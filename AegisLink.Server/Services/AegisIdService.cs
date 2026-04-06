using System.Security.Cryptography;
using System.Text;

namespace AegisLink.Server.Services;

public static class AegisIdService
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string CreateId(byte[] publicKeyBytes)
    {
        var hash = SHA256.HashData(publicKeyBytes);

        var bits = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            bits.Append(Convert.ToString(hash[i], 2).PadLeft(8, '0'));
        }

        string bitString = bits.ToString();

        var id = new StringBuilder();
        for (int i = 0; i < 40; i += 5)
        {
            string chunk = bitString.Substring(i, 5);
            int index = Convert.ToInt32(chunk, 2);
            id.Append(Alphabet[index]);
        }

        return id.ToString();
    }

    public static bool Verify(string claimedId, string publicKeyB64)
    {
        try
        {
            var pubKeyBytes = Convert.FromBase64String(publicKeyB64);
            var expectedId = CreateId(pubKeyBytes);
            return expectedId == claimedId;
        }
        catch
        {
            return false;
        }
    }
}
