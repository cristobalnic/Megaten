using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Views;

public class ConsoleView : IView
{
    private readonly View _view;

    private ConsoleView(View view)
    {
        _view = view;
    }

    public static ConsoleView SetConsoleView(View view)
    {
        return new ConsoleView(view);
    }
    

    public void WriteLine(string message)
    {
        _view.WriteLine(message);
    }

    public string ReadLine()
    {
        return _view.ReadLine();
    }
    
    public void DisplayHpMessage(Unit monster) 
        => _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
}