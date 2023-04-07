using System;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager instance;

    public static event EventHandler onCue;
    public static event EventHandler onBeat;
    public static event EventHandler onBar;

    //[SerializeField] AK.Wwise.Event musicEvent;
    private uint playingID;

    private bool durationSet = false;
    private float beatDuration;
    private float barDuration;

    public static float BeatDuration { get { return instance.beatDuration; } }
    public static float BarDuration { get { return instance.barDuration; } }

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
            ),
            instance.CallbackFunction
        );
    }

    public static void StopSong(AK.Wwise.Event musicEvent)
    {
        musicEvent.Stop(instance.gameObject);
    }

    private void CallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        AkMusicSyncCallbackInfo musicInfo;

        if (in_info is AkMusicSyncCallbackInfo)
        {
            musicInfo = (AkMusicSyncCallbackInfo)in_info;
            RhythmEventArgs args = new RhythmEventArgs();
            args.musicInfo = musicInfo;

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
}

public class RhythmEventArgs : EventArgs
{
    public AkMusicSyncCallbackInfo musicInfo { get; set; }
}
