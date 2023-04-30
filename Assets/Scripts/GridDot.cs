using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDot : MonoBehaviour
{
    [SerializeField] private float revealRange = 4f;
    [SerializeField] private AnimationCurve revealCurve;

    private SpriteRenderer sRender;

    private void Awake()
    {
        sRender = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Color newColor = sRender.color;

        float dist = (PlayerMovement.instance.transform.position - transform.position).magnitude;
        float ratio = Mathf.InverseLerp(0, revealRange, dist);
        newColor.a = revealCurve.Evaluate(ratio);

        sRender.color = newColor;
    }
}
