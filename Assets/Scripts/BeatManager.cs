using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class BeatManager : MonoBehaviour, IOnMIDI
{
    [SerializeField] private Transform player;
    [SerializeField] private BeatIndicator beatPrefab;

    private void Start()
    {
        RhythmManager.onMIDI += OnMIDI;
    }

    private void OnDestroy()
    {
        RhythmManager.onMIDI -= OnMIDI;
    }

    public void OnMIDI(RhythmManager.RhythmEventInfo e)
    {
        Instantiate(beatPrefab, player);
    }
}
