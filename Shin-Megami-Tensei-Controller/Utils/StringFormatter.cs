namespace Shin_Megami_Tensei.Utils;

public static class StringFormatter
{
    public static string[] SplitSamuraiInfo(string input)
    {
        return input.Contains('(') ? SplitSamuraiInfoWithSkills(input) : [input, ""];
    }

    private static string[] SplitSamuraiInfoWithSkills(string input)
    {
        var split = input.Split(" (");
        var name = split[0];
        var skills = split[1].Trim('(', ')');
        return [name, skills];
    }
}