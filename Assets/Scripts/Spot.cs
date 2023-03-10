using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReactOnRhythm;

public class Spot : MonoBehaviour, IOnBeat
{
    private SpriteRenderer sRender;
    private Color targetColor;

    private int counter = 5;

    private void Awake()
    {
        sRender = GetComponent<SpriteRenderer>();
        targetColor = sRender.color;
        sRender.color = Color.clear;
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
        newColor.a = 1 - counter / 4f;
        sRender.color = newColor;

        if (counter < 0)
            Destroy(gameObject);
    }
}
