using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PausePanelFuncs : MonoBehaviour {

    bool m_Unpause;
    bool m_ToggleSound = false;

    public bool ToggleSound
    {
        get
        {
            return m_ToggleSound;
        }
    }
    public bool UnPause
    {
        get
        {
            return m_Unpause;
        }
        set
        {
            m_Unpause = value;
        }
    }

    public void ButtonUnpause()
    {
        m_Unpause = true;
    }

    public void ButtonExit()
    {
        SceneManager.LoadScene(0);
    }

    public void ButtonSound()
    {
        m_ToggleSound = !m_ToggleSound;
    }
}
