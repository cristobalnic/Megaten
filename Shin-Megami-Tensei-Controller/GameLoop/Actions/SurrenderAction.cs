using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class SurrenderAction
{
    private readonly View _view;
    
    public SurrenderAction(View view)
    {
        _view = view;
    }

    internal void ExecuteSurrender(Player turnPlayer, Player waitPlayer)
    {
        _view.WriteLine($"{turnPlayer.Samurai?.Name} (J{turnPlayer.Id}) se rinde");
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {waitPlayer.Samurai?.Name} (J{waitPlayer.Id})");
        throw new EndGameException();
    }
}