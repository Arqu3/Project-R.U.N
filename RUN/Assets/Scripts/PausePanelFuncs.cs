using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelFuncs : MonoBehaviour {

    bool m_Unpause;

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

    public void ButtonRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
