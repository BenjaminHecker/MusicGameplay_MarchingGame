using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class Countdown : MonoBehaviour, IOnCue
{
    [SerializeField] private CountdownIndicator countdownPrefab;

    [System.Serializable]
    public struct CountdownItem
    {
        public string cue;
        public Color color;
    }
    [SerializeField] private CountdownItem[] items;

    private void Start()
    {
        RhythmManager.onCue += OnCue;
    }

    private void OnDestroy()
    {
        RhythmManager.onCue -= OnCue;
    }

    public void OnCue(RhythmManager.RhythmEventInfo e)
    {
        switch (e.MusicInfo.userCueName)
        {
            case "4":
            case "3":
            case "2":
            case "1":

                CountdownIndicator countdownIndicator = Instantiate(countdownPrefab);
                countdownIndicator.number.text = e.MusicInfo.userCueName;

                foreach (CountdownItem item in items)
                    if (e.MusicInfo.userCueName == item.cue)
                        countdownIndicator.number.color = item.color;

                break;
        }
    }
}
