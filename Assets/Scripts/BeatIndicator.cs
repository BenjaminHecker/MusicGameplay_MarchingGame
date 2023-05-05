using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circle;

    [Space]
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float startScale;
    [SerializeField] private float targetScale = 1f;
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;

    private float duration;
    private float timer = 0f;

    private void Start()
    {
        duration = RhythmManager.BarDuration;

        circle.transform.localScale = Vector3.one * startScale;
        circle.color = startColor;
    }

    private void Update()
    {
        if (timer > duration)
            Destroy(gameObject);

        float curvedRatio = curve.Evaluate(timer / duration);

        circle.transform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, curvedRatio);
        circle.color = Color.Lerp(startColor, targetColor, curvedRatio);

        timer += Time.deltaTime;
    }
}
