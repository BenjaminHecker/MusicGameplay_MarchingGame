using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement")]
    [SerializeField] private Transform sprite;
    [SerializeField] private float stepSize = 1f;

    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] [Range(0f, 1f)] private float moveDurationFactor = 0.5f;
    private float MoveDuration { get { return RhythmManager.BeatDuration * moveDurationFactor; } }

    private float moveTimer = 0f;

    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] [Range(0f, 1f)] private float stepCooldownFactor = 0.5f;

    private float DiagonalValueThreshold { get { return Mathf.Sin(Mathf.Deg2Rad * 45f); } }

    private float lastStepTime = 0f;

    [HideInInspector] public Vector2 dir;
    [HideInInspector] public Vector2 prevPos;
    [HideInInspector] public Vector2 currPos;

    private Vector2 prevDir = Vector2.zero;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (moveTimer < MoveDuration)
        {
            float ratio = moveTimer / MoveDuration;
            transform.position = Vector3.Lerp(prevPos, currPos, moveCurve.Evaluate(ratio));

            moveTimer += Time.deltaTime;
        }
        else
        {
            transform.position = currPos;
        }

        sprite.up = Vector3.Lerp(sprite.up, dir, rotateSpeed * Time.deltaTime);
    }

    public void InputUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dir = Vector2.up;
            Step();
        }
    }

    public void InputDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dir = Vector2.down;
            Step();
        }
    }

    public void InputLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dir = Vector2.left;
            Step();
        }
    }

    public void InputRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dir = Vector2.right;
            Step();
        }
    }

    public void InputJoystick(InputAction.CallbackContext context)
    {
        Vector2 rawDir = context.ReadValue<Vector2>();
        Vector2 snapDir = SnapDir(rawDir);

        if (context.performed && rawDir != Vector2.zero)
        {
            if (snapDir != prevDir)
            {
                dir = snapDir;
                prevDir = dir;

                Step();
            }
        }
        else
        {
            prevDir = Vector2.zero;
        }
    }

    private void Step()
    {
        if (Time.time - lastStepTime >= stepCooldownFactor * RhythmManager.BeatDuration)
        {
            lastStepTime = Time.time;

            prevPos = currPos;

            if (GridManager.InBounds(prevPos + dir))
                currPos = prevPos + dir;

            moveTimer = 0f;
        }
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
}
