﻿using System.ComponentModel.Design;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Tests;

public class Tests
{
    [Theory]
    [MemberData(nameof(GetTestsAssociatedWithThisFolder), parameters: "E1-BasicCombat")]
    public void TestE1_BasicCombat(string teamsFolder, string testFile)
        => RunTest(teamsFolder, testFile);

    [Theory]
    [MemberData(nameof(GetTestsAssociatedWithThisFolder), parameters: "E1-InvalidTeams")]
    public void TestE1_InvalidTeams(string teamsFolder, string testFile)
        => RunTest(teamsFolder, testFile);

    [Theory]
    [MemberData(nameof(GetTestsAssociatedWithThisFolder), parameters: "E1-Random")]
    public void TestE1_Random(string teamsFolder, string testFile)
        => RunTest(teamsFolder, testFile);
    
    public static IEnumerable<object[]> GetTestsAssociatedWithThisFolder(string teamsfolder)
    {
        teamsfolder = Path.Combine("data", teamsfolder);
        var testFolder = teamsfolder + "-Tests";
        var testFiles = GetAllTestFilesFrom(testFolder);
        return ConvertDataIntoTheAppropriateFormat(teamsfolder, testFiles);
    }
    
    private static string[] GetAllTestFilesFrom(string folder)
        => Directory.GetFiles(folder, "*.txt", SearchOption.TopDirectoryOnly);
    
    private static IEnumerable<object[]> ConvertDataIntoTheAppropriateFormat(string teamsfolder, string[] testFiles)
    {
        var allData = new List<object[]>();
        foreach (var testFile in testFiles)
            allData.Add(new object[] { teamsfolder, testFile });
        return allData;
    }
    
    private static void RunTest(string teamsFolder, string testFile)
    {
        Game.Player1 = new Player(1);
        Game.Player2 = new Player(2);
        
        var view = View.BuildTestingView(testFile);
        var game = new Game(view, teamsFolder);
        game.TryToPlay();
        
        var actualScript = view.GetScript();
        var expectedScript = File.ReadAllLines(testFile);
        CompareScripts(expectedScript, actualScript);
    }
    
    private static void CompareScripts(IReadOnlyList<string> actualScript, IReadOnlyList<string> expectedScript)
    {
        var numberOfLines = Math.Max(expectedScript.Count, actualScript.Count);
        for (var i = 0; i < numberOfLines; i++)
        {
            var expected = GetTheItemOrEmptyIfOutOfIndex(i, expectedScript);
            var actual = GetTheItemOrEmptyIfOutOfIndex(i, actualScript);
            Assert.Equal($"[L{i+1}] " + expected, $"[L{i+1}] " + actual);
        }
    }
    
    private static string GetTheItemOrEmptyIfOutOfIndex(int index, IReadOnlyList<string> script)
        => index < script.Count ? script[index] : "";
}