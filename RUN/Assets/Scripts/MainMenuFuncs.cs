using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuFuncs : MonoBehaviour
{
	//Public vars
	public enum State
	{
		None,
		IsOut,
		IsIn
	};

	public State m_State;

    //Component vars
    LoadingScreen m_loadScreen;
	MainMenuHandler m_Handler;
	Animator m_Animator;
	GameObject m_LevelSelect;
	GameObject m_Options;
	GameObject m_Instructions;

	void Start ()
	{
		m_State = State.IsOut;
		m_LevelSelect = GameObject.Find("LevelSelectButtons");
		m_Options = GameObject.Find("OptionButtons");
		m_Instructions = GameObject.Find("Instructions");

		m_Handler = GetComponent<MainMenuHandler>();
		m_Animator = GetComponent<Animator>();
        m_loadScreen = Camera.main.GetComponent<LoadingScreen>();


        Time.timeScale = 1.0f;
		m_Animator.SetFloat("Direction", 1.0f);
        PlayerPrefs.SetInt("Continue", 0);
	}

	public void OnPlayButton()
	{
        PlayerPrefs.SetInt("Continue", 1);
        if (PlayerPrefs.GetInt("CurrentLevel", 0) == 0)
		{
			PlayerPrefs.SetInt("CurrentLevel", 1);

            LoadingScreen.Load(PlayerPrefs.GetInt("CurrentLevel", 0));

			//SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel", 0));
		}
		else
		{
            LoadingScreen.Load(PlayerPrefs.GetInt("CurrentLevel", 0));
            //SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel", 0));
		}
	}

	public void OnLevelSelectButton()
	{
		if (!m_Handler.GetState().Equals(MainMenuHandler.State.LevelSelect))
		{
			m_Handler.SetState(MainMenuHandler.State.LevelSelect);
		}
	}

	public void OnOptionsButton()
	{
		if (!m_Handler.GetState().Equals(MainMenuHandler.State.Options))
		{
			m_Handler.SetState(MainMenuHandler.State.Options);
		}
	}

	public void OnInstructionsButton()
	{
		if (!m_Handler.GetState().Equals(MainMenuHandler.State.Instructions))
		{
			m_Handler.SetState(MainMenuHandler.State.Instructions);
		}
	}

	//Checks what current animation state that should be set
	public void OnMenuButton()
	{
		//Only performs check if menu changes state
		if (!m_Handler.GetOldState().Equals(m_Handler.GetState()))
		{
			if (m_State.Equals(State.IsOut))
			{
				SetState(State.IsIn);
				m_Animator.SetFloat("Direction", 1.0f);
				m_Animator.Play("MoveButtons", 0, 0.0f);
			}
			else if (m_State.Equals(State.None))
			{
				SetState(State.IsOut);
				m_Animator.SetFloat("Direction", 1.0f);
				m_Animator.Play("MoveButtons", 0, 0.0f);
			}
			else if (m_State.Equals(State.IsIn))
			{
				SetState(State.IsOut);
				m_Animator.SetFloat("Direction", -1.0f);
				m_Animator.Play("MoveButtons", 0, 1.0f);
			}
		}
	}

	public void OnExitButton()
	{
		Application.Quit();
	}

	//Reverts animation to create a "bounce"
	public void AnimationStarted()
	{
		Debug.Log("Animation started");
		CheckActive();
		if (!m_State.Equals(State.IsIn))
		{
			SetState(State.IsIn);
			m_Animator.SetFloat("Direction", 1.0f);
			m_Animator.Play("MoveButtons", 0, 0.0f);
		}
	}

	public void AnimationFinished()
	{
		Debug.Log("Animation finished");
		SetState(State.IsIn);
	}

	void SetState(State newState)
	{
		m_State = newState;
	}

	//Sets what current UI element that should be active
	void CheckActive()
	{
		m_LevelSelect.SetActive(false);
		m_Options.SetActive(false);
		m_Instructions.SetActive(false);

		if (m_Handler.GetState().Equals(MainMenuHandler.State.Instructions))
		{
			m_Instructions.SetActive(true);
		}
		else if (m_Handler.GetState().Equals(MainMenuHandler.State.Options))
		{
			m_Options.SetActive(true);
		}
		else if (m_Handler.GetState().Equals(MainMenuHandler.State.LevelSelect))
		{
			m_LevelSelect.SetActive(true);
		}
	}
}
