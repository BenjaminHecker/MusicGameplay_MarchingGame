using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static int selectedSong;

    [SerializeField] private AK.Wwise.Event[] songs;

    [SerializeField] private TextMeshProUGUI txt_Score;
    [SerializeField] private Animator playerAnim;

    private int score = 0;

    private void Awake()
    {
        instance = this;
        
        score = 0;

        RhythmManager.PlaySong(songs[selectedSong]);
        playerAnim.speed = RhythmManager.BPM / 60f;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            StopSong();
            SceneManager.LoadScene(1);
        }
    }

    public static void StopSong()
    {
        RhythmManager.StopSong(instance.songs[selectedSong]);
    }

    public static void IncrementScore(int value)
    {
        instance.score += value;

        string scoreText = instance.score.ToString("00000");
        instance.txt_Score.text = scoreText;
    }
}
