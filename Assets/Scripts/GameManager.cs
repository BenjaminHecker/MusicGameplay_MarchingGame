using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event[] songs;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            RhythmManager.StopSong(songs[0]);
            RhythmManager.PlaySong(songs[0]);
        }
    }
}
