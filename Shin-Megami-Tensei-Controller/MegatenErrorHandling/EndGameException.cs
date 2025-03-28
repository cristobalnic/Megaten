namespace Shin_Megami_Tensei.MegatenErrorHandling;

public class EndGameException : MegatenException
{
    public override string GetErrorMessage()
    {
        return "ENDGAME";
    }
}