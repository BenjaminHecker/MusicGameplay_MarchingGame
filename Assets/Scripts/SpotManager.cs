using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class SpotManager : MonoBehaviour, IOnCue, IOnBeat
{
    [SerializeField] private Spot prefab;

    private Vector2 prevPos = Vector2.zero;
    private bool firstSpot = true;
    private int dist = 2;

    public enum Difficulty { Easy, Medium, Hard }
    private Difficulty difficulty = Difficulty.Easy;

    private void Start()
    {
        RhythmManager.onCue += OnCue;
        RhythmManager.onBeat += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.onCue -= OnCue;
        RhythmManager.onBeat -= OnBeat;
    }

    public void OnCue(object sender, System.EventArgs e)
    {
        SpawnSpot();
    }

    public void OnBeat(object sender, System.EventArgs e)
    {
        if (!firstSpot)
            dist++;
    }

    private void SpawnSpot()
    {
        Vector2 newPos = prevPos + GetNextSpotOffset();

        firstSpot = false;
        dist = 0;

        prevPos = newPos;
        Instantiate(prefab, newPos, Quaternion.identity);
    }

    private Vector2 GetNextSpotOffset()
    {
        List<Vector2> outline = new List<Vector2>();

        if (difficulty == Difficulty.Hard)
        {
            for (int x = -dist; x <= dist; x++)
            {
                Vector2 topOffset = new Vector2(x, dist);
                Vector2 bottomOffset = new Vector2(x, -dist);

                if (GridManager.InBounds(prevPos + topOffset)) outline.Add(topOffset);
                if (GridManager.InBounds(prevPos + bottomOffset)) outline.Add(bottomOffset);
            }

            for (int y = -dist + 1; y <= dist - 1; y++)
            {
                Vector2 leftOffset = new Vector2(-dist, y);
                Vector2 rightOffset = new Vector2(dist, y);

                if (GridManager.InBounds(prevPos + leftOffset)) outline.Add(leftOffset);
                if (GridManager.InBounds(prevPos + rightOffset)) outline.Add(rightOffset);
            }
        }
        else
        {
            Vector2 topOffset = new Vector2(0, dist);
            Vector2 bottomOffset = new Vector2(0, -dist);
            Vector2 leftOffset = new Vector2(-dist, 0);
            Vector2 rightOffset = new Vector2(dist, 0);

            if (GridManager.InBounds(prevPos + topOffset)) outline.Add(topOffset);
            if (GridManager.InBounds(prevPos + bottomOffset)) outline.Add(bottomOffset);
            if (GridManager.InBounds(prevPos + leftOffset)) outline.Add(leftOffset);
            if (GridManager.InBounds(prevPos + rightOffset)) outline.Add(rightOffset);

            if (difficulty == Difficulty.Medium)
            {
                Vector2 topRightOffset = new Vector2(dist, dist);
                Vector2 topLeftOffset = new Vector2(-dist, dist);
                Vector2 bottomRightOffset = new Vector2(dist, -dist);
                Vector2 bottomLeftOffset = new Vector2(-dist, -dist);

                if (GridManager.InBounds(prevPos + topRightOffset)) outline.Add(topRightOffset);
                if (GridManager.InBounds(prevPos + topLeftOffset)) outline.Add(topLeftOffset);
                if (GridManager.InBounds(prevPos + bottomRightOffset)) outline.Add(bottomRightOffset);
                if (GridManager.InBounds(prevPos + bottomLeftOffset)) outline.Add(bottomLeftOffset);
            }
        }

        return outline[Random.Range(0, outline.Count)];
    }
}
