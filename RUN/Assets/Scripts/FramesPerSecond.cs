using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour {

    //Public vars
    public float m_updateInterval = 0.5f;
    Text m_Text;

    //Frames vars
    float m_accum = 0.0f;
    float m_timeLeft;
    int m_frames = 0;
    int m_TotalFrames = 0;
    float m_AVGFrames = 0.0f;
    float m_Elapsed;

	void Start ()
    {
        m_Text = GameObject.Find("FpsText").GetComponent<Text>();

        //If no text is selected
        if (!m_Text)
        {
            Debug.Log("FramesPerSecond needs a text component");
            enabled = false;
            return;
        }
        m_timeLeft = m_updateInterval;
	}
	
	void Update ()
    {
        m_Elapsed += Time.deltaTime;

        m_timeLeft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        //Make sure the value is given after incrementation
        ++m_frames;
        ++m_TotalFrames;

        if (m_Elapsed >= 1.0f)
            m_AVGFrames = m_TotalFrames / m_Elapsed;
        if (Input.GetKeyDown(KeyCode.F12))
            Debug.Log("Current average FPS: " + m_AVGFrames);

        //End of interval
        if (m_timeLeft <= 0.0f)
        {
            //Display fps in F2 format
            float fps = m_accum / m_frames;
            m_Text.text = "" + fps.ToString("F0");

            //Color change depending on fps
            if (fps < 30)
            {
                m_Text.color = Color.yellow;
            }
            else if (fps < 10)
            {
                m_Text.color = Color.red;
            }
            else
            {
                m_Text.color = Color.white;
            }

            //Reset
            m_timeLeft = m_updateInterval;
            m_accum = 0.0f;
            m_frames = 0;
        }
	}
}
