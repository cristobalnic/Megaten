namespace Shin_Megami_Tensei.ErrorHandling;

public class InvalidTeamException : MegatenException
{
    public override string GetErrorMessage()
    {
        return "Archivo de equipos inválido";
    }
}