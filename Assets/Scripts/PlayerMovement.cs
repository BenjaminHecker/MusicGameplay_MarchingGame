using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class PlayerMovement : MonoBehaviour, IOnBeat
{
    public static PlayerMovement instance;
    
    [SerializeField] private Transform player;
    [SerializeField] private float stepSize = 1f;
    [SerializeField] private float stepSmoothTime = 0.005f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private float inputThreshold = 0.38f;
    [SerializeField] private float inputDeadzone = 0.2f;

    private Vector2 dir;
    private Vector2 currentPos;
    [HideInInspector] public Vector2 targetPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RhythmManager.onBeat += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.onBeat -= OnBeat;
    }

    private void Update()
    {
        UpdateDir();

        player.up = Vector3.Lerp(player.up, dir, rotateSpeed * Time.deltaTime);

        Vector3 currentVelocity = Vector3.zero;
        player.position = Vector3.SmoothDamp(player.position, targetPos, ref currentVelocity, stepSmoothTime);
    }

    public void OnBeat(object sender, System.EventArgs e)
    {
        Step();
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

        if (dir.x > inputThreshold)
            dir.x = 1;
        else if (dir.x < -inputThreshold)
            dir.x = -1;
        else
            dir.x = 0;

        if (dir.y > inputThreshold)
            dir.y = 1;
        else if (dir.y < -inputThreshold)
            dir.y = -1;
        else
            dir.y = 0;

        dir *= stepSize;
    }

    private void Step()
    {
        currentPos = targetPos;

        if (GridManager.InBounds(currentPos + dir))
            targetPos = currentPos += dir;
    }
}
