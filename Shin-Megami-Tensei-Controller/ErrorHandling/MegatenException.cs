namespace Shin_Megami_Tensei.ErrorHandling;

public abstract class MegatenException : ApplicationException
{
    public abstract string GetErrorMessage();
}