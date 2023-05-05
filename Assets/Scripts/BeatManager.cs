using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ReactOnRhythm;

public class BeatManager : MonoBehaviour, IOnMIDI
{
    public enum TimingWindow
    {
        Perfect = 20,
        Great = 10,
        Good = 5,
        Miss = -10
    }

    [SerializeField] private Transform player;
    [SerializeField] private BeatIndicator beatPrefab;

    [Header("Markers")]
    [SerializeField] private Transform playerDiamond;
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float targetScale = 1f;
    [SerializeField] private float pulseDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.1f;

    private IEnumerator diamondPulseRoutine;

    [Header("Timing")]
    [SerializeField] private TimingIndicator timingIndicatorPrefab;
    [SerializeField] private float perfectWindow = 0.050f;
    [SerializeField] private float greatWindow = 0.075f;
    [SerializeField] private float goodWindow = 0.100f;

    [Space]
    [SerializeField] private AK.Wwise.Event drumSound;

    private List<float> beatTimes = new List<float>();

    private void Start()
    {
        RhythmManager.onMIDI += OnMIDI;

        playerDiamond.localScale = Vector3.one * startScale;
    }

    private void OnDestroy()
    {
        RhythmManager.onMIDI -= OnMIDI;
    }

    public void OnMIDI(RhythmManager.RhythmEventInfo e)
    {
        beatTimes.Add(RhythmManager.CurrentPositionSec + RhythmManager.BarDuration);
        Instantiate(beatPrefab, player);
    }

    public void Drum(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            drumSound.Post(gameObject);

            if (diamondPulseRoutine != null)
                StopCoroutine(diamondPulseRoutine);

            diamondPulseRoutine = DiamondPulse();
            StartCoroutine(diamondPulseRoutine);

            SpawnTimingIndicator();
        }
    }

    private IEnumerator DiamondPulse()
    {
        float timer = 0f;

        while (timer < pulseDuration)
        {
            float ratio = timer / pulseDuration;
            playerDiamond.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, ratio);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        timer = 0f;

        while (timer < shrinkDuration)
        {
            float ratio = timer / shrinkDuration;
            playerDiamond.localScale = Vector3.one * Mathf.Lerp(targetScale, startScale, ratio);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        playerDiamond.localScale = Vector3.one * startScale;
    }

    private void SpawnTimingIndicator()
    {
        TimingWindow timing = GetTimingWindow();

        TimingIndicator indicator = Instantiate(timingIndicatorPrefab, player.position, Quaternion.identity);
        indicator.Init(timing, -PlayerMovement.instance.dir);

        GameManager.IncrementScore((int)timing);
    }

    private TimingWindow GetTimingWindow()
    {
        float closestBeatOffset = float.PositiveInfinity;

        foreach (float beat in beatTimes)
        {
            float offset = Mathf.Abs(RhythmManager.CurrentPositionSec - beat);

            closestBeatOffset = Mathf.Min(offset, closestBeatOffset);
        }

        if (closestBeatOffset <= perfectWindow / 2f) return TimingWindow.Perfect;
        if (closestBeatOffset <= greatWindow / 2f) return TimingWindow.Great;
        if (closestBeatOffset <= goodWindow / 2f) return TimingWindow.Good;
        return TimingWindow.Miss;
    }
}
