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
    public int m_SetToCheckPoint = 0;
    public float m_ResetDepth = 0.0f;
    public float m_PromptTime = 1.0f;
    public static int m_LastPassed;
    public Transform[] m_CheckPoints;
    public List<float> m_HighScores;
    public MovingPlatform[] m_MovingPlatforms;

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
    ControllerUI m_UI;

    float m_FTemp = 0.0f;
    string m_SaveString = "";
    string m_CompleteTime;

    void Start ()
    {
        m_LastPassed = 0;
        m_CPlayer = GetComponentInChildren<ControllerPlayer>();
        m_Camera = GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>();

        m_ElapsedText = GameObject.Find("TimeText").GetComponent<Text>();
        m_PromptText = GameObject.Find("CheckpointPromptText").GetComponent<Text>();
        m_UI = GameObject.Find("Canvas").GetComponent<ControllerUI>();

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

        //Find moving platforms
        var movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        if (movingPlatforms.Length > 0)
        {
            m_MovingPlatforms = new MovingPlatform[movingPlatforms.Length];
            for (int i = 0; i < movingPlatforms.Length; i++)
            {
                if (movingPlatforms[i].GetComponent<MovingPlatform>())
                m_MovingPlatforms[i] = movingPlatforms[i].GetComponent<MovingPlatform>();
            }
        }

        if (PlayerPrefs.GetInt("Continue", 0) == 1)
        {
            m_ElapsedTime = PlayerPrefs.GetFloat("TimeAtCheckpoint" + SceneManager.GetActiveScene().buildIndex.ToString(), 0.0f);
            SetToCheckpoint(PlayerPrefs.GetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex.ToString(), 0));
            Debug.Log(m_ElapsedTime);
            Debug.Log(PlayerPrefs.GetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex.ToString(), 0));
            PlayerPrefs.SetInt("Continue", 0);
        }

        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + 1000), new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + -1000));

        Gizmos.DrawLine(new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + 1000), new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + 1000));

        Gizmos.DrawLine(new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + -1000), new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + -1000));

        Gizmos.DrawLine(new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + -1000), new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + 1000));

        Gizmos.DrawLine(new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + -1000), new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + 1000));

        Gizmos.DrawLine(new Vector3(transform.position.x + 1000, m_ResetDepth, transform.position.z + -1000), new Vector3(transform.position.x + -1000, m_ResetDepth, transform.position.z + 1000));
    }
#endif
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.Log(PlayerPrefs.GetFloat("HighScore" + SceneManager.GetActiveScene().buildIndex.ToString(), Mathf.Infinity));
        }

        m_IsColliding = false;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (m_SetToCheckPoint < 0)
            {
                Debug.Log("ERROR, SetToCheckPoint is negative");
            }
            else
            {
                if (m_SetToCheckPoint > m_CheckPoints.Length - 1)
                {
                    Debug.Log("ERROR, SetToCheckPoint is greater than amount of checkpoints");
                }
                else
                {
                    SetToCheckpoint(m_SetToCheckPoint);
                }
            }
        }

        if (transform.position.y < m_ResetDepth)
        {
            Debug.Log("Depth reset");
            SetToCheckpoint(m_LastPassed);
        }

        TextUpdate();
	}

    public void SetToCheckpoint(int num)
    {
        //Reset player velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //Set position
        //Note - y position -16 because of parent offset position in playerbody
        transform.position = new Vector3(m_CheckPoints[num].position.x, m_CheckPoints[num].position.y - 16.0f, m_CheckPoints[num].position.z);

        //Reset player
        if (PlayerPrefs.GetInt("Continue", 0) == 0)
        m_CPlayer.ResetValues();

        //Set rotation to corresponding checkpoint
        m_Camera._mouseAbsolute.x = m_CheckPoints[num].localEulerAngles.y;
        m_Camera._mouseAbsolute.y = m_CheckPoints[num].localEulerAngles.x;

        for (int i = 0; i < m_MovingPlatforms.Length; i++)
        {
            m_MovingPlatforms[i].Reset();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetLastPassed()
    {
        return m_LastPassed;
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
                    m_UI.ToggleScoreScreen();
                    SetHighscore(m_ElapsedTime);
                    m_HasSetScore = true;
                }
            }
            else
            {
                PlayerPrefs.SetFloat("TimeAtCheckpoint" + SceneManager.GetActiveScene().buildIndex.ToString(), m_ElapsedTime);
                PlayerPrefs.SetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex.ToString(), m_LastPassed);
            }
        }

        //If collision with killbox
        if (col.gameObject.tag == "KillBox")
        {
            Debug.Log("Killbox reset");
            SetToCheckpoint(m_LastPassed);
        }
    }

    void TextUpdate()
    {
        if (!m_HasReachedLast)
        {
            m_ElapsedTime += Time.deltaTime;
        }

        int minutes = Mathf.FloorToInt(m_ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(m_ElapsedTime - minutes * 60f);
        string decimals = (m_ElapsedTime - (int)m_ElapsedTime).ToString("F2");
        decimals = decimals.Remove(0, 2);

        string time = String.Format("{0:00}:{1:00}", minutes, seconds) + ":" + decimals;

        m_ElapsedText.text = time;
        m_CompleteTime = time;

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

        if (m_UI.GetIsScoreScreen())
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
                minutes = Mathf.FloorToInt(m_HighScores[i] / 60f);
                seconds = Mathf.FloorToInt(m_HighScores[i] - minutes * 60f);
                decimals = (m_HighScores[i] - (int)m_HighScores[i]).ToString("F2");
                decimals = decimals.Remove(0, 2);
                time = String.Format("{0:00}:{1:00}", minutes, seconds) + ":" + decimals;
                s += i + 1 + ": " + time + "\n";
            }
            if (GameObject.Find("EndText"))
                GameObject.Find("EndText").GetComponent<Text>().text = "Level Complete!\nTime: " + m_CompleteTime;
            if (GameObject.Find("HighscoreText"))
                GameObject.Find("HighscoreText").GetComponent<Text>().text = "Your Best Times:\n" + s;
        }
    }

    void SetHighscore(float score)
    {
        float oldHighScore = PlayerPrefs.GetFloat("HighScore" + SceneManager.GetActiveScene().buildIndex.ToString(), Mathf.Infinity);

        if (score < oldHighScore)
        {
            Debug.Log("New best time!");
            PlayerPrefs.SetFloat("HighScore" + SceneManager.GetActiveScene().buildIndex.ToString(), score);
            PlayerPrefs.Save();
        }

        //Save current score to string, add that to the total score string
        m_SaveString = score.ToString("F2", CultureInfo.InvariantCulture.NumberFormat) + " ";
        string temp = PlayerPrefs.GetString("Time" + SceneManager.GetActiveScene().buildIndex.ToString(), "");
        temp += m_SaveString;
        PlayerPrefs.SetString("Time" + SceneManager.GetActiveScene().buildIndex.ToString(), temp);

        //Get total score string and split it at every " "
        char[] splits = { ' ' };
        string[] elements = PlayerPrefs.GetString("Time" + SceneManager.GetActiveScene().buildIndex.ToString(), "").Split(splits);

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
