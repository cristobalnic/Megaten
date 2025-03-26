namespace Shin_Megami_Tensei.MegatenErrorHandling;

public abstract class MegatenException : ApplicationException
{
    public abstract string GetErrorMessage();
}