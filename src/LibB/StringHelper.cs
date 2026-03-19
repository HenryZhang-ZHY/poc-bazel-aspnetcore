using System;

namespace LibB;

public class StringHelper
{
    public string Reverse(string input)
    {
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public bool IsPalindrome(string input) =>
        input.Equals(Reverse(input), StringComparison.OrdinalIgnoreCase);
}
