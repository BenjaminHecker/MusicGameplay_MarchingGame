using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReactOnRhythm;

public class Spot : MonoBehaviour, IOnBeat
{
    private SpriteRenderer sRender;
    private Color targetColor;

    [SerializeField] private int countdownMax;
    private int counter;

    private void Awake()
    {
        sRender = GetComponent<SpriteRenderer>();
        targetColor = sRender.color;
        sRender.color = Color.clear;

        counter = countdownMax;
    }

    private void Start()
    {
        RhythmManager.onBeat += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.onBeat -= OnBeat;
    }

    public void OnBeat(object sender, EventArgs e)
    {
        Countdown();
    }

    private void Countdown()
    {
        counter--;

        Color newColor = targetColor;
        newColor.a = 1 - (float) counter / countdownMax;
        sRender.color = newColor;

        if (counter < 0)
            Destroy(gameObject);
    }
}
