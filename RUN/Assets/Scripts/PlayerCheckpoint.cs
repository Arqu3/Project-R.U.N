using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCheckpoint : MonoBehaviour
{
    //Public vars
    public float m_ResetDepth = 0.0f;
    public float m_PromptTime = 1.0f;
    public static int m_LastPassed = 0;
    public Transform[] m_CheckPoints;
    public List<float> m_HighScores;

    //Other vars
    Transform m_Temp;
    bool m_IsColliding = false;

    float m_ElapsedTime = 0.0f;
    float m_PromptTimer = 0.0f;
    bool m_ReachedCheckpoint = false;
    bool m_HasReachedLast = false;
    bool m_HasSetScore = false;

    ControllerPlayer m_CPlayer;
    SimpleSmoothMouseLook m_Camera;
    Text m_ElapsedText;
    Text m_PromptText;
    Text m_HighscoreText;
    Text m_EndText;
    GameObject m_UI;

    float m_FTemp = 0.0f;
    string m_SaveString = "";

    void Start ()
    {
        m_CPlayer = GetComponent<ControllerPlayer>();
        m_Camera = GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>();

        m_ElapsedText = GameObject.Find("TimeText").GetComponent<Text>();
        m_PromptText = GameObject.Find("CheckpointPromptText").GetComponent<Text>();
        m_HighscoreText = GameObject.Find("HighscoreText").GetComponent<Text>();
        m_EndText = GameObject.Find("EndText").GetComponent<Text>();
        m_UI = GameObject.Find("Canvas");

        //Find checkpoints
        var checkPoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        if (checkPoints.Length > 0)
        {
            m_CheckPoints = new Transform[checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                m_CheckPoints[i] = checkPoints[i].transform;
            }

            //Sort checkpoint depending on ID
            for (int write = 0; write < checkPoints.Length; write++)
            {
                for (int sort = 0; sort < checkPoints.Length - 1; sort++)
                {
                    if (m_CheckPoints[sort].GetComponent<Checkpoint>().m_ID > m_CheckPoints[sort + 1].GetComponent<Checkpoint>().m_ID)
                    {
                        m_Temp = m_CheckPoints[sort + 1];
                        m_CheckPoints[sort + 1] = m_CheckPoints[sort];
                        m_CheckPoints[sort] = m_Temp;
                    }
                }
            }
        }
	}

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted all playerprefs");
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.Log(PlayerPrefs.GetFloat("HighScore", Mathf.Infinity));
        }

        m_IsColliding = false;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetToLastCheckpoint();
        }

        if (transform.localPosition.y < m_ResetDepth)
        {
            Debug.Log("Depth reset");
            SetToLastCheckpoint();
        }

        //Get and reload current scene
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        TextUpdate();
	}

    void SetToLastCheckpoint()
    {
        //Reset player velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        //Set position
        transform.position = m_CheckPoints[m_LastPassed].position;

        //Reset blink CD and fall time
        m_CPlayer.SendMessage("BlinkReset");
        m_CPlayer.SendMessage("FallTimerReset");

        //Set rotation to corresponding checkpoint
        m_Camera._mouseAbsolute.x = m_CheckPoints[m_LastPassed].localEulerAngles.y;
        m_Camera._mouseAbsolute.y = m_CheckPoints[m_LastPassed].localEulerAngles.x;
    }

    void OnTriggerEnter(Collider col)
    {
        if (m_IsColliding)
        {
            return;
        }
        m_IsColliding = true;

        //If collision with checkpoint
        if (col.gameObject.tag == "Checkpoint")
        {
            for (int i = 0; i < m_CheckPoints.Length; i++)
            {
                if (col.gameObject == m_CheckPoints[i].gameObject)
                {
                    m_LastPassed = i;
                    m_ReachedCheckpoint = true;
                }
            }
            if (m_LastPassed + 1 == m_CheckPoints.Length)
            {
                m_HasReachedLast = true;
                Debug.Log("Reached last checkpoint");

                if (!m_HasSetScore)
                {
                    m_UI.SendMessage("ToggleScoreScreen");
                    SetHighscore(m_ElapsedTime);
                    if (m_EndText)
                    {
                        m_EndText.text = "Level Complete!\nTime: " + m_ElapsedTime.ToString("F1");
                    }
                    m_HasSetScore = true;
                }
            }
        }

        //If collision with killbox
        if (col.gameObject.tag == "KillBox")
        {
            Debug.Log("Killbox reset");
            SetToLastCheckpoint();
        }
    }

    void TextUpdate()
    {
        if (!m_HasReachedLast)
        {
            m_ElapsedTime += Time.deltaTime;
        }
        m_ElapsedText.text = "Time: " + m_ElapsedTime.ToString("F1");

        if (m_ReachedCheckpoint)
        {
            m_PromptTimer += Time.deltaTime;
            m_PromptText.text = "CHECKPOINT";
        }
        else
        {
            m_PromptText.text = "";
        }
        if (m_PromptTimer >= m_PromptTime)
        {
            m_ReachedCheckpoint = false;
            m_PromptTimer = 0.0f;
        }

        if (m_HighscoreText)
        {
            int t = 0;
            if (m_HighScores.Count < 10)
            {
                t = m_HighScores.Count;
            }
            else
            {
                t = 10;
            }
            string s = "";
            for (int i = 0; i < t; i++)
            {
                s += i + 1 + ": " + m_HighScores[i] + "\n";
            }
            m_HighscoreText.text = "Your best times:\n" + s;

        }
    }

    void SetHighscore(float score)
    {
        float oldHighScore = PlayerPrefs.GetFloat("HighScore", Mathf.Infinity);

        if (score < oldHighScore)
        {
            Debug.Log("New best time!");
            PlayerPrefs.SetFloat("HighScore", score);
            PlayerPrefs.Save();
        }

        //Save current score to string, add that to the total score string
        m_SaveString = score.ToString("F1", CultureInfo.InvariantCulture.NumberFormat) + " ";
        string temp = PlayerPrefs.GetString("Time", "");
        temp += m_SaveString;
        PlayerPrefs.SetString("Time", temp);

        //Get total score string and split it at every " "
        char[] splits = { ' ' };
        string[] elements = PlayerPrefs.GetString("Time", "").Split(splits);

        //Parse every score string in elements
        for (int i = 0; i < elements.Length - 1; i++)
        {
            float t = (float)double.Parse(elements[i], System.Globalization.NumberStyles.AllowDecimalPoint);
            m_HighScores.Add(t);
        }

        SortHighScore();
    }

    void SortHighScore()
    {
        for (int write = 0; write < m_HighScores.Count; write++)
        {
            for (int sort = 0; sort < m_HighScores.Count - 1; sort++)
            {
                if (m_HighScores[sort] > m_HighScores[sort + 1])
                {
                    m_FTemp = m_HighScores[sort + 1];
                    m_HighScores[sort + 1] = m_HighScores[sort];
                    m_HighScores[sort] = m_FTemp;
                }
            }
        }
    }
}
