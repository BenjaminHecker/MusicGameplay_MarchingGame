using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public static PlayerMovement2 instance;

    public enum TimingWindow
    {
        Perfect = 20,
        Great = 10,
        Good = 5,
        Miss = 0
    }

    [Header("Movement")]
    [SerializeField] private Transform player;
    [SerializeField] private float stepSize = 1f;
    [SerializeField] private float stepSmoothTime = 0.005f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float inputDeadzone = 0.2f;

    [Tooltip("threshold angle for diagonal inputs (in degrees)")]
    [SerializeField] [Range(0f, 45f)] private float diagonalAngleThreshold = 15;

    private float DiagonalValueThreshold { get { return Mathf.Sin(Mathf.Deg2Rad * diagonalAngleThreshold); } }

    [Header("Timing Window")]
    [SerializeField] private TimingIndicator indicatorPrefab;
    [SerializeField] private float perfectWindow = 0.050f;
    [SerializeField] private float greatWindow = 0.075f;
    [SerializeField] private float goodWindow = 0.100f;

    private Vector2 dir;
    private Vector2 prevDir;
    private Vector2 currentPos;
    [HideInInspector] public Vector2 targetPos;

    private bool directionHeld = false;
    private bool canMove = true;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        TimingWindow timing = GetTimingWindow();

        if (timing == TimingWindow.Miss)
            canMove = true;

        UpdateDir();

        if (dir == Vector2.zero)
            directionHeld = false;
        else
        {
            if (!directionHeld)
            {
                directionHeld = true;
                prevDir = dir;
                Step(timing);
            }
        }

        player.up = Vector3.Lerp(player.up, prevDir, rotateSpeed * Time.deltaTime);

        Vector3 currentVelocity = Vector3.zero;
        player.position = Vector3.SmoothDamp(player.position, targetPos, ref currentVelocity, stepSmoothTime);
    }

    private void Step(TimingWindow timing)
    {
        if (canMove)
            canMove = false;
        else
            return;

        // player can make inputs as many times as they want during a Miss window,
        // but during a non-Miss window, they can only make one input

        TimingIndicator indicator = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
        indicator.Init(timing, -dir);

        if (timing != TimingWindow.Miss)
        {
            currentPos = targetPos;

            if (GridManager.InBounds(currentPos + dir))
                targetPos = currentPos += dir;

            GameManager.IncrementScore((int)timing);
        }
    }

    private void UpdateDir()
    {
        Vector2 temp = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (temp.magnitude <= inputDeadzone)
        {
            dir = Vector2.zero;
            return;
        }

        dir = temp.normalized;

        if (dir.x > DiagonalValueThreshold)
            dir.x = 1;
        else if (dir.x < -DiagonalValueThreshold)
            dir.x = -1;
        else
            dir.x = 0;

        if (dir.y > DiagonalValueThreshold)
            dir.y = 1;
        else if (dir.y < -DiagonalValueThreshold)
            dir.y = -1;
        else
            dir.y = 0;

        dir *= stepSize;
    }

    private TimingWindow GetTimingWindow()
    {
        float nearestBeat = Mathf.Round(RhythmManager.CurrentPositionSec / RhythmManager.BeatDuration) * RhythmManager.BeatDuration;
        float beatOffset = Mathf.Abs(RhythmManager.CurrentPositionSec - nearestBeat);

        if (beatOffset <= perfectWindow / 2f) return TimingWindow.Perfect;
        if (beatOffset <= greatWindow / 2f) return TimingWindow.Great;
        if (beatOffset <= goodWindow / 2f) return TimingWindow.Good;
        return TimingWindow.Miss;
    }
}
