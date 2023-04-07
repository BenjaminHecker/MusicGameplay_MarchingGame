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

    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;

    [SerializeField] private float outerStartScale = 3f;

    [SerializeField] private int countdownMax;
    private int counter;

    private float timerTarget;
    private float timer = 0f;

    private bool active = true;

    [Space]
    [SerializeField] private AK.Wwise.Event hitSound;

    private void Awake()
    {
        innerCircle.color = startColor;
        innerOutline.color = startColor;
        outerCircle.color = startColor;

        innerCircle.transform.localScale = Vector3.zero;
        outerCircle.transform.localScale = Vector3.one * outerStartScale;

        counter = countdownMax;

        timerTarget = countdownMax * RhythmManager.BeatDuration;
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
        counter--;
        timerTarget = timer + counter * RhythmManager.BeatDuration;

        if (counter == 0 && (PlayerMovement.instance.targetPos - (Vector2)transform.position).magnitude < 0.1f)
        {
            hitSound.Post(gameObject);
            GameManager.IncrementScore();
        }
    }

    private void Update()
    {
        if (!active) return;

        float ratio = animCurve.Evaluate(Mathf.Min(1, timer / timerTarget));

        Color outlineColor = Color.white;
        outlineColor.a = ratio;
        innerOutline.color = outlineColor;

        Color circleColor = targetColor;
        circleColor.a = Mathf.Lerp(startColor.a, targetColor.a, ratio);
        innerCircle.color = circleColor;
        outerCircle.color = circleColor;

        innerCircle.transform.localScale = Vector3.one * ratio;
        outerCircle.transform.localScale = Vector3.one * Mathf.Lerp(outerStartScale, 1, ratio);

        timer += Time.deltaTime;

        if (timer > timerTarget)
        {
            StartCoroutine(Disappear());
            active = false;
        }
    }

    private IEnumerator Disappear()
    {
        float disappearTimer = 0f;
        float disappearTimerTarget = RhythmManager.BeatDuration * 0.5f;

        float ratio;

        do
        {
            ratio = 1 - animCurve.Evaluate(1 - Mathf.Min(1, disappearTimer / disappearTimerTarget));

            Color outlineColor = Color.white;
            outlineColor.a = 1 - ratio;
            innerOutline.color = outlineColor;

            Color innerCircleColor = targetColor;
            innerCircleColor.a = Mathf.Lerp(targetColor.a, 0, ratio);
            innerCircle.color = innerCircleColor;

            Color outerCircleColor = Color.Lerp(targetColor, Color.white, ratio);
            outerCircleColor.a = 1 - ratio;
            outerCircle.color = outerCircleColor;

            innerCircle.transform.localScale = Vector3.one * (1 - ratio);
            outerCircle.transform.localScale = Vector3.one * Mathf.Lerp(1, 1.5f, ratio);

            disappearTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (ratio < 1);

        Destroy(gameObject);
    }
}
