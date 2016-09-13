using UnityEngine;
using System.Collections;

public class FadeToBlack : MonoBehaviour {

    public Texture m_Image;
    public bool m_RemoveAfterFadeIn;
    float m_time;


    void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, m_time) * 2);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Image);

        if (m_RemoveAfterFadeIn && m_time > 1)
        {
            Destroy(this);
        }

        m_time += Time.unscaledDeltaTime;
    }

}