using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei.GameSetup;

public static class SetupView
{
    internal static void DisplayTeamFileSelection(View view, string teamsFolder)
    {
        string[] teamFiles = Directory.GetFiles(teamsFolder, "*.txt");
        view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < teamFiles.Length; i++)
            view.WriteLine($"{i}: {Path.GetFileName(teamFiles[i])}");
    }
}