using UnityEngine;
using System.Collections;

public class TutorialPanelFuncs : MonoBehaviour
{
    public bool m_Tutorial = false;

    public void ButtonTutorial()
    {
        m_Tutorial = !m_Tutorial;
    }
}
