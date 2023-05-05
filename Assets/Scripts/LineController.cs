using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private Transform headSprite;
    [SerializeField] private Transform tailSprite;
    [SerializeField] private LineRenderer lineRender;

    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] [Range(0f, 1f)] private float moveDurationFactor = 0.5f;
    private float MoveDuration { get { return RhythmManager.BeatDuration * moveDurationFactor; } }

    [SerializeField] private int lineMaxLength = 5;

    [HideInInspector] public List<Vector2> stepPositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> prevPositions = new List<Vector2>();

    public Vector2 Head { get { return (stepPositions.Count == 0) ? Vector2.zero : stepPositions[stepPositions.Count - 1]; } }
    public Vector2 Tail { get { return (stepPositions.Count == 0) ? Vector2.zero : stepPositions[0]; } }

    public Vector2 PrevHead { get { return (prevPositions.Count == 0) ? Vector2.zero : prevPositions[prevPositions.Count - 1]; } }
    public Vector2 PrevTail { get { return (prevPositions.Count == 0) ? Vector2.zero : prevPositions[0]; } }

    public void Move(Vector2 newHead)
    {
        prevPositions = new List<Vector2>(stepPositions);

        stepPositions.Add(newHead);

        if (stepPositions.Count > lineMaxLength)
            stepPositions.RemoveAt(0);

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        float timer = 0;

        while (timer < MoveDuration)
        {
            float ratio = timer / MoveDuration;

            UpdateLine(ratio);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        UpdateLine(1);
    }

    private void UpdateLine(float ratio)
    {
        UpdateLinePoints(ratio);
        UpdateHead(ratio);
        UpdateTail(ratio);
    }

    private void UpdateLinePoints(float ratio)
    {
        float curvedRatio = moveCurve.Evaluate(ratio);

        List<Vector3> linePoints = new List<Vector3>();

        Vector2 smoothHead = Vector2.Lerp(PrevHead, Head, curvedRatio);
        Vector2 smoothTail = Vector2.Lerp(PrevTail, Tail, curvedRatio);

        if (smoothTail != Tail && curvedRatio < 0.8)
            linePoints.Add(smoothTail);

        for (int i = 0; i < stepPositions.Count - 1; i++)
            linePoints.Add(stepPositions[i]);

        bool straightHead = true;

        if (prevPositions.Count >= 2)
        {
            Vector2 lastSegment = Head - PrevHead;
            Vector2 prevLastSegment = stepPositions[stepPositions.Count - 2] - prevPositions[prevPositions.Count - 2];
            straightHead = lastSegment == prevLastSegment;
        }

        if (straightHead || curvedRatio > 0.4)
            linePoints.Add(smoothHead);

        lineRender.positionCount = linePoints.Count;
        lineRender.SetPositions(linePoints.ToArray());
    }

    private void UpdateHead(float ratio)
    {
        headSprite.position = Vector2.Lerp(PrevHead, Head, moveCurve.Evaluate(ratio));
        headSprite.up = Head - PrevHead;
    }

    private void UpdateTail(float ratio)
    {
        tailSprite.position = Vector2.Lerp(PrevTail, Tail, moveCurve.Evaluate(ratio));
    }
}
