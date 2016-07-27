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
        m_Animator.SetFloat("Direction", 1.0f);
        m_Animator.Play("MoveButtonsLevelSelect", 0, 0.0f);
    }

    public void OnBackToMenuButton()
    {
        //Set direction to -1 and normalized time to 1 to reverse animation
        if (m_Handler.GetState().Equals(MainMenuHandler.State.LevelSelect))
        {
            m_Animator.SetFloat("Direction", -1.0f);
            m_Animator.Play("MoveButtonsLevelSelect", 0, 1.0f);
        }
        else if (m_Handler.GetState().Equals(MainMenuHandler.State.Options))
        {
            m_Animator.SetFloat("Direction", -1.0f);
            m_Animator.Play("MoveButtonsOptions", 0, 1.0f);
        }
        m_Handler.SetState(MainMenuHandler.State.Main);
    }

    public void OnOptionsButton()
    {
        m_Handler.SetState(MainMenuHandler.State.Options);
        m_Animator.SetFloat("Direction", 1.0f);
        m_Animator.Play("MoveButtonsOptions", 0, 0.0f);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
