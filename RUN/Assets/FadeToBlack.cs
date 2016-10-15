using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour {

    public Texture m_Image;
    public bool m_RemoveAfterFadeIn;
    public float m_MaxTime;
    float m_time;

    bool m_input;
    int m_maxAlpha = 0;
    int m_minAlpha = 1;

    void Start()
    {
        Debug.Log("STARTEN");

        m_time = 0;
        m_maxAlpha = 0;
        m_minAlpha = 1;
    }

    void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, Mathf.Lerp(m_minAlpha, m_maxAlpha, m_time / m_MaxTime));
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Image);
        GUI.BringWindowToFront(0);

        if (m_RemoveAfterFadeIn && m_time > 1)
        {
            Destroy(this);
        }

        m_time += Time.unscaledDeltaTime;
    }
    
    public void FadeOut(float time)
    {
        m_MaxTime = time;
        m_time = 0;
        m_maxAlpha = 1;
        m_minAlpha = 0;
    }

    public void FadeIn(float time)
    {
        m_MaxTime = time;
        m_time = 0;
        m_maxAlpha = 0;
        m_minAlpha = 1;
    }
}