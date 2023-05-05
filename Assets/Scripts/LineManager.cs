using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class LineManager : MonoBehaviour, IOnCue, IOnBeat
{
    [SerializeField] private LineController linePrefab;
    private LineController lineController;

    private bool running = false;

    private void Start()
    {
        RhythmManager.onCue += OnCue;
        RhythmManager.onBeat += OnBeat;

        lineController = Instantiate(linePrefab);
        lineController.stepPositions.Add(Vector2.zero);
    }

    private void OnDestroy()
    {
        RhythmManager.onCue -= OnCue;
        RhythmManager.onBeat -= OnBeat;
    }

    public void OnCue(RhythmManager.RhythmEventInfo e)
    {
        if (e.MusicInfo.userCueName == "Start Line")
        {
            running = true;
        }
    }

    public void OnBeat(RhythmManager.RhythmEventInfo e)
    {
        if (running)
            MoveLine();
    }

    private void MoveLine()
    {
        Vector2 newHead = lineController.Head + GetNextOffset();

        // reduces chance of doubling back or crossing over, but still allows it to happen occasionally
        if (lineController.stepPositions.Contains(newHead))
            newHead = lineController.Head + GetNextOffset();

        lineController.Move(newHead);
    }

    private Vector2 GetNextOffset()
    {
        Vector2 head = lineController.Head;
        List<Vector2> potentialTargets = new List<Vector2>();

        Vector2 up = Vector2.up;
        Vector2 down = Vector2.down;
        Vector2 left = Vector2.left;
        Vector2 right = Vector2.right;

        if (GridManager.InBounds(head + up)) potentialTargets.Add(up);
        if (GridManager.InBounds(head + down)) potentialTargets.Add(down);
        if (GridManager.InBounds(head + left)) potentialTargets.Add(left);
        if (GridManager.InBounds(head + right)) potentialTargets.Add(right);

        return potentialTargets[Random.Range(0, potentialTargets.Count)];
    }
}
