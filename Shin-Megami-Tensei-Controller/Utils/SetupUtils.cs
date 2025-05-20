namespace Shin_Megami_Tensei.Utils;

public static class SetupUtils
{
    public static string[] GetTeamFiles(string teamsFolder)
    {
        return Directory
            .EnumerateFiles(teamsFolder, "*.txt")
            .OrderBy(path => Path.GetFileName(path))
            .ToArray();
    }
}