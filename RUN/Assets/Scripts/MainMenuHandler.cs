using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public enum State
    {
        Main,
        LevelSelect,
        Options,
        Instructions
    };

    //Public vars
    public Button[] m_LevelSelectButtons;

    public State m_State;
    public State m_OldState;

    //Buttons
    Button m_Temp;
    GameObject m_NewGameButton;

	void Start ()
    {
        m_State = State.Main;
        m_NewGameButton = GameObject.Find("ButtonStart");

        //Find buttons
        var levelSelectButtons = GameObject.FindGameObjectsWithTag("LevelSelectButton");

        if (levelSelectButtons.Length > 0)
        {
            m_LevelSelectButtons = new Button[levelSelectButtons.Length];

            for (int i = 0; i < levelSelectButtons.Length; i++)
            {
                m_LevelSelectButtons[i] = levelSelectButtons[i].GetComponent<Button>();
            }

            //Sort buttons
            for (int write = 0; write < levelSelectButtons.Length; write++)
            {
                for (int sort = 0; sort < levelSelectButtons.Length - 1; sort++)
                {
                    if (m_LevelSelectButtons[sort].GetComponent<LevelSelectButton>().m_ID > m_LevelSelectButtons[sort + 1].GetComponent<LevelSelectButton>().m_ID)
                    {
                        m_Temp = m_LevelSelectButtons[sort + 1];
                        m_LevelSelectButtons[sort + 1] = m_LevelSelectButtons[sort];
                        m_LevelSelectButtons[sort] = m_Temp;
                    }
                }
            }
        }

    }

    void Update ()
    {
        switch (m_State)
        {
            case State.Main:
                break;

            case State.LevelSelect:
                break;

            case State.Options:
                break;

            case State.Instructions:
                break;
        }

        TextUpdate();
	}

    void LateUpdate()
    {
        m_OldState = m_State;
    }

    void TextUpdate()
    {
        if (m_State.Equals(State.LevelSelect))
        {
            for (int i = 0; i < m_LevelSelectButtons.Length; i++)
            {
                m_LevelSelectButtons[i].GetComponentInChildren<Text>().text = "Level: " + (i + 1);
            }
        }

        if (PlayerPrefs.GetInt("CurrentLevel", 0) == 0)
        {
            m_NewGameButton.GetComponentInChildren<Text>().text = "New Game";
        }
        else
        {
            m_NewGameButton.GetComponentInChildren<Text>().text = "Continue";
        }
    }

    public void SetState(State state)
    {
        m_State = state;
    }

    public State GetState()
    {
        return m_State;
    }

    public State GetOldState()
    {
        return m_OldState;
    }
}
