using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer marker;

    [Header("Fall")]
    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private float startScale;
    [SerializeField] private float targetScale = 1f;
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;

    [Header("Disappear")]
    [SerializeField] private AnimationCurve disappearCurve;
    [SerializeField] private float disappearDurationFactor = 0.2f;
    [SerializeField] private float disappearScale = 1.5f;
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 targetRotation;

    private float DisappearDuration { get { return RhythmManager.BarDuration * disappearDurationFactor; } }

    private float duration;
    private float timer = 0f;

    private bool running = true;

    private void Start()
    {
        duration = RhythmManager.BarDuration;

        marker.transform.localScale = Vector3.one * startScale;
        marker.transform.localEulerAngles = startRotation;
        marker.color = startColor;
    }

    private void Update()
    {
        if (running && timer > duration)
        {
            running = false;
            timer = 0f;
        }

        if (running)
            UpdateFall();
        else
            UpdateDisappear();
    }

    private void UpdateFall()
    {
        float curvedRatio = fallCurve.Evaluate(timer / duration);

        marker.transform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, curvedRatio);
        marker.color = Color.Lerp(startColor, targetColor, curvedRatio);

        timer += Time.deltaTime;
    }

    private void UpdateDisappear()
    {
        float curvedRatio = disappearCurve.Evaluate(timer / DisappearDuration);

        marker.transform.localScale = Vector3.one * Mathf.Lerp(targetScale, disappearScale, curvedRatio);
        marker.transform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, curvedRatio);
        marker.color = Color.Lerp(targetColor, startColor, curvedRatio);

        if (timer > DisappearDuration)
            Destroy(gameObject);

        timer += Time.deltaTime;
    }
}
