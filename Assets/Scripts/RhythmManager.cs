using System;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public class RhythmEventInfo
    {
        public AkCallbackType type;
        public AkCallbackInfo info;
        public AkMusicSyncCallbackInfo MusicInfo { get { return (info is AkMusicSyncCallbackInfo) ? (AkMusicSyncCallbackInfo)info : null; } }
        public AkMIDIEventCallbackInfo MidiInfo { get { return (info is AkMIDIEventCallbackInfo) ? (AkMIDIEventCallbackInfo)info : null; } }

        public RhythmEventInfo(AkCallbackType type, AkCallbackInfo info)
        {
            this.type = type;
            this.info = info;
        }
    }

    public static RhythmManager instance;

    public delegate void RhythmEvent(RhythmEventInfo e);

    public static RhythmEvent onCue;
    public static RhythmEvent onBeat;
    public static RhythmEvent onBar;
    public static RhythmEvent onMIDI;

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
        RhythmEventInfo e = new RhythmEventInfo(in_type, in_info);

        if (e.type == AkCallbackType.AK_EndOfEvent)
            isPlaying = false;

        if (e.info is AkMusicSyncCallbackInfo)
        {
            switch (e.type)
            {
                case AkCallbackType.AK_MusicSyncUserCue:
                    OnCue(e);
                    break;
                case AkCallbackType.AK_MusicSyncBeat:
                    OnBeat(e);
                    break;
                case AkCallbackType.AK_MusicSyncBar:
                    OnBar(e);
                    break;
            }

            if (e.type is AkCallbackType.AK_MusicSyncBar)
            {
                if (!durationSet)
                {
                    beatDuration = e.MusicInfo.segmentInfo_fBeatDuration;
                    barDuration = e.MusicInfo.segmentInfo_fBarDuration;
                    durationSet = true;
                }
            }
        }

        if (in_info is AkMIDIEventCallbackInfo)
        {
            switch (e.MidiInfo.byType)
            {
                case AkMIDIEventTypes.NOTE_ON:
                    OnMIDI(e);
                    break;

                case AkMIDIEventTypes.NOTE_OFF:
                    break;

                case AkMIDIEventTypes.PITCH_BEND:
                    break;
            }
        }
    }

    private void OnCue(RhythmEventInfo e)
    {
        if (onCue != null)
            onCue(e);
    }

    private void OnBeat(RhythmEventInfo e)
    {
        if (onBeat != null)
            onBeat(e);
    }

    private void OnBar(RhythmEventInfo e)
    {
        if (onBar != null)
            onBar(e);
    }

    private void OnMIDI(RhythmEventInfo e)
    {
        if (onMIDI != null)
            onMIDI(e);
    }
}
