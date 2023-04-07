using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void SelectSong(int index)
    {
        GameManager.selectedSong = index;
        SceneManager.LoadScene(1);
    }

    public void Back()
    {
        GameManager.StopSong();
        SceneManager.LoadScene(0);
    }
}
