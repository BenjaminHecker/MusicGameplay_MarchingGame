using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Menu")
            Application.Quit();
    }

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
