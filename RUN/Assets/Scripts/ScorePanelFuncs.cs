using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScorePanelFuncs : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 3)
        {
            transform.FindChild("ButtonNextLevel").gameObject.SetActive(true);
        }
        else
        {
            transform.FindChild("ButtonNextLevel").gameObject.SetActive(false);
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("Restart", 1);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
