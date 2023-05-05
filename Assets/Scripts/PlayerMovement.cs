using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public enum TimingWindow
    {
        Perfect = 20,
        Great = 10,
        Good = 5,
        Miss = -10
    }

    [Header("Movement")]
    [SerializeField] private Transform player;
    [SerializeField] private float stepSize = 1f;

    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] [Range(0f, 1f)] private float moveDurationFactor = 0.5f;
    private float MoveDuration { get { return RhythmManager.BeatDuration * moveDurationFactor; } }

    private float moveTimer = 0f;

    [SerializeField] private float rotateSpeed = 10f;

    [Tooltip("threshold angle for diagonal inputs (in degrees)")]
    [SerializeField] [Range(0f, 45f)] private float diagonalAngleThreshold = 15;

    private float DiagonalValueThreshold { get { return Mathf.Sin(Mathf.Deg2Rad * diagonalAngleThreshold); } }

    [Header("Timing Window")]
    [SerializeField] [Range(0f, 1f)] private float stepCooldownFactor = 0.5f;
    [SerializeField] private TimingIndicator indicatorPrefab;
    [SerializeField] private float perfectWindow = 0.050f;
    [SerializeField] private float greatWindow = 0.075f;
    [SerializeField] private float goodWindow = 0.100f;

    private float lastStepTime = 0f;

    private Vector2 dir;
    private Vector2 prevPos;
    [HideInInspector] public Vector2 currPos;

    private bool tapProcessed = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (moveTimer < MoveDuration)
        {
            float ratio = moveTimer / MoveDuration;
            player.position = Vector3.Lerp(prevPos, currPos, moveCurve.Evaluate(ratio));

            moveTimer += Time.deltaTime;
        }
        else
        {
            player.position = currPos;
        }

        player.up = Vector3.Lerp(player.up, dir, rotateSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 rawDir = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (!tapProcessed && rawDir != Vector2.zero)
            {
                tapProcessed = true;

                dir = SnapDir(rawDir);

                if (RhythmManager.IsPlaying)
                    Step();
            }
        }
        if (context.canceled)
        {
            tapProcessed = false;
        }
    }

    private void Step()
    {
        TimingWindow timing = GetTimingWindow();

        if (Time.time - lastStepTime >= stepCooldownFactor * RhythmManager.BeatDuration)
        {
            lastStepTime = Time.time;

            prevPos = currPos;

            if (GridManager.InBounds(prevPos + dir))
                currPos = prevPos + dir;

            moveTimer = 0f;
        }
        else
        {
            timing = TimingWindow.Miss;
        }

        TimingIndicator indicator = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
        indicator.Init(timing, -dir);

        GameManager.IncrementScore((int)timing);
    }

    private Vector2 SnapDir(Vector2 dir)
    {
        dir.Normalize();

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

        return dir;
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
