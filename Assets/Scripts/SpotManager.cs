using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReactOnRhythm;

public class SpotManager : MonoBehaviour, IOnCue
{
    [SerializeField] private Spot prefab;

    private Vector2 prevPos = Vector2.zero;

    private void Start()
    {
        RhythmManager.onCue += OnCue;
    }

    public void OnCue(object sender, System.EventArgs e)
    {
        SpawnSpot();
    }

    private void SpawnSpot()
    {
        Vector2 newPos = new Vector2(Random.Range(-4, 5), Random.Range(-4, 5));
        newPos += prevPos;

        if (!GridManager.InBounds(newPos)) SpawnSpot();

        Instantiate(prefab, newPos, Quaternion.identity);
    }
}
