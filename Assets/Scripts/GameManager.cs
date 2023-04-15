using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static int selectedSong;

    [System.Serializable]
    public struct RhythmSong
    {
        public int BPM;
        public AK.Wwise.Event song;
    }
    [SerializeField] private RhythmSong[] songs;

    [SerializeField] private TextMeshProUGUI txt_Score;
    [SerializeField] private Animator playerAnim;

    private int score = 0;

    private void Awake()
    {
        instance = this;
        
        score = 0;

        RhythmManager.PlaySong(songs[selectedSong].song);
        playerAnim.speed = songs[selectedSong].BPM / 60f;
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
        RhythmManager.StopSong(instance.songs[selectedSong].song);
    }

    public static void IncrementScore()
    {
        instance.score++;

        string scoreText = instance.score.ToString();
        if (instance.score < 10)
            scoreText = "0" + scoreText;

        instance.txt_Score.text = scoreText;
    }
}
