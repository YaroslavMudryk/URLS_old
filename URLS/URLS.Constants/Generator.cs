using System.Text;

namespace URLS.Constants;

public static class Generator
{
    private static string _upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string _lowerChars = "abcdefghijklmnopqrstuvwxyz";
    private static string _numbersChars = "0123456789";
    private static string _chars = $"{_upperChars}{_numbersChars}{_lowerChars}";
    public static string CreateGroupInviteCode()
    {
        int lengthOfSequence = 4;
        StringBuilder sb = new StringBuilder();
        
        var random = new Random();

        for (int i = 0; i < lengthOfSequence; i++)
        {
            sb.Append(_chars[random.Next(_chars.Length)]);
        }

        sb.Append("#");

        for (int i = 0; i < lengthOfSequence; i++)
        {
            sb.Append(_numbersChars[random.Next(_numbersChars.Length)]);
        }

        return sb.ToString();
    }

    public static string CreateAppId()
    {
        return GetUniqCode(4);
    }

    public static string CreateAppSecret()
    {
        return GetString(70);
    }

    public static string GetUsername()
    {
        string username;
        username = GetString(10);
        return username;
    }

    public static string GetStringId()
    {
        long ticks = DateTime.Now.Ticks;
        byte[] bytes = BitConverter.GetBytes(ticks);
        string id = Convert.ToBase64String(bytes)
                                .Replace('+', '_')
                                .Replace('/', '-')
                                .TrimEnd('=');
        return id;
    }

    public static string GetUniqCode(int sections)
    {
        var commonWords = sections * 4;
        var commonCountOfSymbols = commonWords + (sections - 1);
        var stringChars = new char[commonCountOfSymbols];
        var positons = GetHyphenPositions(sections);
        var random = new Random();
        for (int i = 0; i < stringChars.Length; i++)
        {
            if (positons.Contains(i))
            {
                stringChars[i] = '-';
                continue;
            }
            stringChars[i] = _chars[random.Next(_chars.Length)];
        }
        return new String(stringChars);
    }

    private static int[] GetHyphenPositions(int sections)
    {
        var pos = new int[sections - 1];
        var baseIndex = 4;
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = baseIndex;
            baseIndex += 4 + 1;
        }
        return pos;
    }

    public static string GetString(int length, bool IsUpper = false, bool IsLowwer = false)
    {
        var stringChars = new char[length];
        var random = new Random();
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = _chars[random.Next(_chars.Length)];
        }
        var result = new string(stringChars);
        return IsUpper ? result.ToUpper() : IsLowwer ? result.ToLower() : result;
    }

    public static string CreateTempPassword()
    {
        var sb = new StringBuilder();
        var rnd = new Random();
        char ch;

        for (int i = 0; i < 10; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
            sb.Append(ch);
        }

        return sb.ToString().ToLower();
    }
}
