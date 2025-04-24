using Shin_Megami_Tensei_View;

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
}