namespace ReactOnRhythm
{
    public interface IOnCue
    {
        void OnCue(object sender, System.EventArgs e);
    }

    public interface IOnBeat
    {
        void OnBeat(object sender, System.EventArgs e);
    }

    public interface IOnBar
    {
        void OnBar(object sender, System.EventArgs e);
    }

    public interface IOnMIDI
    {
        void OnMIDI(object sender, System.EventArgs e);
    }
}
