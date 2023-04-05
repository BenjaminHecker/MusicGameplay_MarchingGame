using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactOnRhythm;

public class Spot : MonoBehaviour, IOnBeat
{
    [SerializeField] private SpriteRenderer innerCircle;
    [SerializeField] private SpriteRenderer innerOutline;
    [SerializeField] private SpriteRenderer outerCircle;

    [SerializeField] private Color targetColor;

    [SerializeField] private float smoothTime = 0.02f;

    [SerializeField] private int countdownMax;
    private int counter;

    private Vector3 innerTargetScale;
    private Vector3 outerTargetScale;

    private void Awake()
    {
        innerCircle.color = Color.clear;
        innerOutline.color = Color.clear;
        outerCircle.color = Color.clear;

        counter = countdownMax;

        innerTargetScale = Vector3.zero;
        innerCircle.transform.localScale = innerTargetScale;

        outerTargetScale = Vector3.one * (countdownMax * 2 - 1);
        outerCircle.transform.localScale = outerTargetScale;
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
        Vector3 vel = Vector3.zero;
        innerCircle.transform.localScale = Vector3.SmoothDamp(innerCircle.transform.localScale, innerTargetScale, ref vel, smoothTime);
        outerCircle.transform.localScale = Vector3.SmoothDamp(outerCircle.transform.localScale, outerTargetScale, ref vel, smoothTime);
    }

    public void OnBeat(object sender, EventArgs e)
    {
        Countdown();
    }

    private void Countdown()
    {
        counter--;

        UpdateSpot();
    }

    private void UpdateSpot()
    {
        float ratio = 1 - (float)counter / countdownMax;

        Color innerCircleColor = targetColor;
        innerCircleColor.a = ratio;
        innerCircle.color = innerCircleColor;

        Color innerOutlineColor = Color.white;
        innerOutlineColor.a = ratio;
        innerOutline.color = innerOutlineColor;

        Color outerCircleColor = targetColor;
        outerCircleColor.a = Mathf.Lerp(0, targetColor.a, ratio);
        outerCircle.color = outerCircleColor;

        innerTargetScale = Vector3.one * ratio;
        outerTargetScale = Vector3.one * Mathf.Lerp(countdownMax * 2 - 1, 1, ratio);

        if (counter < 0)
            Destroy(gameObject);
    }
}
