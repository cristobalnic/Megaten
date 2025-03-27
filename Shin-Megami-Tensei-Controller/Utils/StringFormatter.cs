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
}