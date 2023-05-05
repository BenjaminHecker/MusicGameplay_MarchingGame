using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepIndicator : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailPrefab;
    private TrailRenderer trail;

    [SerializeField] private int trailCount = 4;
    [SerializeField] private Color trailColorMin;
    [SerializeField] private Color trailColorMax;

    [SerializeField] private float moveDuration = 0.2f;
    private float moveTimer = 0f;

    LineManager lineManager;
    private Vector2 targetPos;

    public void Init(LineManager lineManager, Vector2 targetPos)
    {
        this.lineManager = lineManager;
        this.targetPos = targetPos;

        StartCoroutine(MoveRoutine());
    }

    private void Update()
    {
        if (trail != null)
        {
            float ratio = moveTimer / moveDuration;

            trail.transform.position = Vector2.Lerp(transform.position, targetPos, ratio);

            moveTimer += Time.deltaTime;
        }
    }

    private IEnumerator MoveRoutine()
    {
        float timeUntilNextTrail = RhythmManager.BeatDuration;

        for (int i = 0; i < trailCount; i++)
        {
            moveTimer = 0;

            trail = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform);

            Color newColor = Color.Lerp(trailColorMin, trailColorMax, Mathf.InverseLerp(0, trailCount - 1, i));
            trail.startColor = newColor;
            trail.endColor = newColor;

            yield return new WaitForSeconds(timeUntilNextTrail);
            Destroy(trail.gameObject);
        }

        //lineManager.stepArrowPositions.RemoveAt(0);
        Destroy(gameObject);
    }
}
