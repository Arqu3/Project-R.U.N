using UnityEngine;
using System.Collections;

public class TutorialPanelFuncs : MonoBehaviour
{
    public bool m_Tutorial = false;

    void Update()
    {
        if (m_Tutorial && Input.GetButtonDown("Pause"))
            ButtonTutorial();
    }

    public void ButtonTutorial()
    {
        m_Tutorial = !m_Tutorial;
    }
}
