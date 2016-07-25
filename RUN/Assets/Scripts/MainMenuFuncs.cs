using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuFuncs : MonoBehaviour
{
    //Component vars
    MainMenuHandler m_Handler;
    Animator m_Animator;

	void Start ()
    {
        m_Handler = GetComponent<MainMenuHandler>();
        m_Animator = GetComponentInChildren<Animator>();
	}
	
	void Update ()
    {
	
	}

    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnLevelSelectButton()
    {
        m_Handler.SetState(MainMenuHandler.State.LevelSelect);
        m_Animator.Play("MoveButtonsLevelSelect");
    }

    public void OnBackToMenuButton()
    {
        m_Handler.SetState(MainMenuHandler.State.Main);
        m_Animator.Play("MoveButtonsMain");
    }

    public void OnOptionsButton()
    {
        m_Handler.SetState(MainMenuHandler.State.Options);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
