namespace Shin_Megami_Tensei.MegatenErrorHandling;

public class CancelObjectiveSelectionException : MegatenException
{
    public override string GetErrorMessage()
    {
        return "Selección de objetivo cancelada";
    }
}