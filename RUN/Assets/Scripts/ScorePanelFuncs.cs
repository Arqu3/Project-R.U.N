using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScorePanelFuncs : MonoBehaviour
{
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
