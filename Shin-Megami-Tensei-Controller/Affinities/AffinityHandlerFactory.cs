using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Affinities;

public static class AffinityHandlerFactory
{
    public static AffinityHandler CreateAffinityHandler(AffinityType affinityType)
    {
        return affinityType switch
        {
            AffinityType.Neutral => new NeutralAffinityHandler(),
            AffinityType.Weak => new WeakAffinityHandler(),
            AffinityType.Resist => new ResistAffinityHandler(),
            AffinityType.Null => new NullAffinityHandler(),
            AffinityType.Repel => new RepelAffinityHandler(),
            AffinityType.Drain => new DrainAffinityHandler(),
            _ => throw new ArgumentOutOfRangeException(nameof(affinityType), affinityType, null)
        };
    }
}