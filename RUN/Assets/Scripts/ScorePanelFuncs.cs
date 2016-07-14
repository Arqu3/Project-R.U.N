using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScorePanelFuncs : MonoBehaviour
{
    void Start()
    {
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("Restart", 1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
