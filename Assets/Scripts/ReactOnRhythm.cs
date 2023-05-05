using static RhythmManager;

namespace ReactOnRhythm
{
    public interface IOnCue
    {
        void OnCue(RhythmEventInfo e);
    }

    public interface IOnBeat
    {
        void OnBeat(RhythmEventInfo e);
    }

    public interface IOnBar
    {
        void OnBar(RhythmEventInfo e);
    }

    public interface IOnMIDI
    {
        void OnMIDI(RhythmEventInfo e);
    }
}
