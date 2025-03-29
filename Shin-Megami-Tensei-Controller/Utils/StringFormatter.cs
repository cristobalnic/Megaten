using System.Text.RegularExpressions;

namespace Shin_Megami_Tensei.Utils;

public static class StringFormatter
{
    public static string GetSamuraiName(string samuraiRawData)
    {
        return samuraiRawData.Split(" ")[1];
    }

    public static string[] GetSamuraiSkills(string samuraiRawData)
    {
        return samuraiRawData.Split(" (")[1].Trim('(', ')').Split(',');
    }

    public static string NormalizeAffinityValues(string json)
    {
        var replacements = new Dictionary<string, string>
        {
            { "\"-\"", "\"Neutral\"" },
            { "\"Wk\"", "\"Weak\"" },
            { "\"Rs\"", "\"Resist\"" },
            { "\"Nu\"", "\"Null\"" },
            { "\"Rp\"", "\"Repel\"" },
            { "\"Dr\"", "\"Drain\"" }
        };
        
        foreach (var pair in replacements)
        {
            json = Regex.Replace(json, pair.Key, pair.Value);
        }

        return json;
    }
}