using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameSetup;

public static class ViewSetup
{
    internal static void DisplayTeamFileSelection(IView view, string teamsFolder)
    {
        string[] teamFiles = Directory.GetFiles(teamsFolder, "*.txt");
        view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < teamFiles.Length; i++)
            view.WriteLine($"{i}: {Path.GetFileName(teamFiles[i])}");
    }
}