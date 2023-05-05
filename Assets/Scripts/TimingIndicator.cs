using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static BeatManager;

public class TimingIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textbox;
    [SerializeField] private AnimationCurve colorCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float duration = 1f;

    [System.Serializable]
    private struct TimingEffect
    {
        public string text;
        public Color color;
    }

    [SerializeField] private TimingEffect perfect;
    [SerializeField] private TimingEffect great;
    [SerializeField] private TimingEffect good;
    [SerializeField] private TimingEffect miss;

    private float timer = 0f;
    private Vector3 dir;

    public void Init(TimingWindow timing, Vector3 dir)
    {
        switch (timing)
        {
            case TimingWindow.Perfect:
                textbox.text = perfect.text;
                textbox.color = perfect.color;
                break;
            case TimingWindow.Great:
                textbox.text = great.text;
                textbox.color = great.color;
                break;
            case TimingWindow.Good:
                textbox.text = good.text;
                textbox.color = good.color;
                break;
            case TimingWindow.Miss:
                textbox.text = miss.text;
                textbox.color = miss.color;
                break;
        }

        this.dir = dir;
    }

    private void Update()
    {
        if (timer > duration)
            Destroy(gameObject);
        else
        {
            float ratio = timer / duration;
            float colorRatio = colorCurve.Evaluate(ratio);
            float scaleRatio = scaleCurve.Evaluate(ratio);
            float moveRatio = moveCurve.Evaluate(ratio);

            Color newColor = textbox.color;
            newColor.a = 1 - colorRatio;
            textbox.color = newColor;

            textbox.transform.localScale = Vector3.one * scaleRatio;
            textbox.transform.position = transform.position + dir.normalized * moveDistance * moveRatio;
        }

        timer += Time.deltaTime;
    }
}
