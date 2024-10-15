namespace MetaquotesHomework;

public static class Utils
{
    /// <summary>
    /// Convert string representation of IP-4 (e.g. "8.8.8.8") to uint
    /// </summary>
    /// <returns>
    /// Uint32 representation of IP-4 or <c>NULL</c> if <paramref name="value"/> is not a valid IP-4 stiring
    /// </returns>
    public static uint? IpToUint(string? value)
    {
        if (value == null || value.Length < 7)
            return null;

        int result = 0;
        int part = 0;
        int dotsCounter = 0;
        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (c == '.')
            {
                if (part > byte.MaxValue || 
                    dotsCounter == 3 || 
                    i == value.Length - 1 || 
                    value[i + 1] == '.')
                    return null;

                dotsCounter++;
                result = (result << 8) | part;
                part = 0;
            }
            else if(char.IsDigit(c))
            {
                part *= 10;
                part += c - '0';
            }
            else
                return null;
        }

        if (part > byte.MaxValue || dotsCounter != 3)
            return null;
        result = (result << 8) | part;
        return (uint)result;
    }
}
