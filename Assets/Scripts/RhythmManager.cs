using System;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager instance;

    public static event EventHandler onCue;
    public static event EventHandler onBeat;
    public static event EventHandler onBar;
    public static event EventHandler onMIDI;

    //[SerializeField] AK.Wwise.Event musicEvent;
    private uint playingID;

    private bool durationSet = false;
    private bool isPlaying = false;
    private float beatDuration;
    private float barDuration;

    public static bool IsPlaying { get { return instance.isPlaying; } }
    public static float BPM { get { return 60f / instance.beatDuration; } }
    public static float BeatDuration { get { return instance.beatDuration; } }
    public static float BarDuration { get { return instance.barDuration; } }
    public static float CurrentPositionMS
    {
        get
        {
            AkSegmentInfo out_info = new AkSegmentInfo();
            AkSoundEngine.GetPlayingSegmentInfo(instance.playingID, out_info);
            return out_info.iCurrentPosition;
        }
    }
    public static float CurrentPositionSec { get { return CurrentPositionMS / 1000f; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static void PlaySong(AK.Wwise.Event musicEvent)
    {
        instance.playingID = musicEvent.Post(
            instance.gameObject,
            (uint)(
                AkCallbackType.AK_MusicSyncAll
                | AkCallbackType.AK_EnableGetMusicPlayPosition
                | AkCallbackType.AK_MIDIEvent
                | AkCallbackType.AK_EndOfEvent
            ),
            instance.CallbackFunction
        );

        instance.isPlaying = true;
    }

    public static void StopSong(AK.Wwise.Event musicEvent)
    {
        musicEvent.Stop(instance.gameObject);
    }

    private void CallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfEvent)
            isPlaying = false;

        if (in_info is AkMusicSyncCallbackInfo)
        {
            AkMusicSyncCallbackInfo musicInfo = (AkMusicSyncCallbackInfo)in_info;
            RhythmEventArgs args = new RhythmEventArgs(musicInfo);

            switch (in_type)
            {
                case AkCallbackType.AK_MusicSyncUserCue:
                    OnCue(args);
                    break;
                case AkCallbackType.AK_MusicSyncBeat:
                    OnBeat(args);
                    break;
                case AkCallbackType.AK_MusicSyncBar:
                    OnBar(args);
                    break;
            }

            if (in_type is AkCallbackType.AK_MusicSyncBar)
            {
                if (!durationSet)
                {
                    beatDuration = musicInfo.segmentInfo_fBeatDuration;
                    barDuration = musicInfo.segmentInfo_fBarDuration;
                    durationSet = true;
                }
            }
        }

        if (in_info is AkMIDIEventCallbackInfo)
        {
            AkMIDIEventCallbackInfo midiInfo = (AkMIDIEventCallbackInfo)in_info;
            RhythmEventArgs args = new RhythmEventArgs(midiInfo);

            switch (midiInfo.byType)
            {
                case AkMIDIEventTypes.NOTE_ON:
                    OnMIDI(args);
                    break;

                case AkMIDIEventTypes.NOTE_OFF:
                    break;

                case AkMIDIEventTypes.PITCH_BEND:
                    break;
            }
        }
    }

    private void OnCue(RhythmEventArgs e)
    {
        onCue?.Invoke(this, e);
    }

    private void OnBeat(RhythmEventArgs e)
    {
        onBeat?.Invoke(this, e);
    }

    private void OnBar(RhythmEventArgs e)
    {
        onBar?.Invoke(this, e);
    }

    private void OnMIDI(RhythmEventArgs e)
    {
        onMIDI?.Invoke(this, e);
    }
}

public class RhythmEventArgs : EventArgs
{
    public AkMusicSyncCallbackInfo musicInfo { get; set; }
    public AkMIDIEventCallbackInfo midiInfo { get; set; }

    public RhythmEventArgs(AkMusicSyncCallbackInfo info)
    {
        musicInfo = info;
    }

    public RhythmEventArgs(AkMIDIEventCallbackInfo info)
    {
        midiInfo = info;
    }
}
