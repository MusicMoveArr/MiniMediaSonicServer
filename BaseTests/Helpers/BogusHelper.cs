using System.Globalization;
using BaseTests.Models;
using Bogus;

namespace BaseTests.Helpers;

public class BogusHelper
{
    private const string CharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:',.<>?/";
    
    public static string GetrandomString(int minLength, int maxLength)
    {
        var stringGenerator = new Faker<BogusString>()
            .RuleFor(cs => cs.Value, f => f.Random.String2(minLength, maxLength, CharSet));

        return stringGenerator.Generate().Value;
    }
}