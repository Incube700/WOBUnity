using RicochetTanks.Gameplay.Combat;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct MatchFinishedEvent
    {
        public MatchFinishedEvent(MatchResult result, string label)
        {
            Result = result;
            Label = label;
        }

        public MatchResult Result { get; }
        public string Label { get; }
    }
}
