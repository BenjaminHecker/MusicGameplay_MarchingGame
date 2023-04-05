using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class PlayerMovement : MonoBehaviour, IOnBeat
{
    [SerializeField] private Transform player;
    [SerializeField] private float stepSize = 0.5f;
    [SerializeField] private float rotateSpeed = 10f;

    private const float INPUT_THRESHOLD = 0.38f;

    private Vector2 dir;
    private Vector2 currentPos;
    private Vector2 targetPos;

    private void Start()
    {
        RhythmManager.onBeat += OnBeat;
    }

    private void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (dir.x > INPUT_THRESHOLD)
            dir.x = 1;
        else if (dir.x < -INPUT_THRESHOLD)
            dir.x = -1;
        else
            dir.x = 0;

        if (dir.y > INPUT_THRESHOLD)
            dir.y = 1;
        else if (dir.y < -INPUT_THRESHOLD)
            dir.y = -1;
        else
            dir.y = 0;

        dir *= stepSize;

        if ((Vector2)player.up == -dir)
            dir += (Vector2) player.right * 0.01f;

        player.up = Vector3.Lerp(player.up, dir, rotateSpeed * Time.deltaTime);
        
        Vector3 currentVelocity = Vector3.zero;
        player.position = Vector3.SmoothDamp(player.position, targetPos, ref currentVelocity, 0.01f);
    }

    public void OnBeat(object sender, System.EventArgs e)
    {
        Step();
    }

    private void Step()
    {
        currentPos = targetPos;

        if (GridManager.InBounds(currentPos + dir))
            targetPos = currentPos += dir;
    }
}
