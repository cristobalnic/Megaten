namespace Shin_Megami_Tensei.ErrorHandling;

public class EndGameException : MegatenException
{
    public override string GetErrorMessage()
    {
        return "ENDGAME";
    }
}