using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Views;

public interface IView
{
    void WriteLine(string message);
    string ReadLine();
    void DisplayHpMessage(Unit target);
}
