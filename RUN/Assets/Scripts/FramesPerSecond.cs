using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour {

    //Public vars
    public float m_updateInterval = 0.5f;
    public Text m_Text;

    //Frames vars
    float m_accum = 0.0f;
    float m_timeLeft;
    int m_frames = 0;

	void Start ()
    {
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
        m_timeLeft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        //Make sure the value is given after incrementation
        ++m_frames;

        //End of interval
        if (m_timeLeft <= 0.0f)
        {
            //Display fps in F2 format
            float fps = m_accum / m_frames;
            m_Text.text = "" + fps.ToString("F2");

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
