namespace Shin_Megami_Tensei.ErrorHandling;

public class CancelObjectiveSelectionException : MegatenException
{
    public override string GetErrorMessage()
    {
        return "Selección de objetivo cancelada";
    }
}