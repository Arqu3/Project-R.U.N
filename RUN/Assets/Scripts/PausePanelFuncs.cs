using UnityEngine;
using System.Collections;

public class PausePanelFuncs : MonoBehaviour {

    public bool m_Unpause;

    public void ButtonUnpause()
    {
        m_Unpause = true;
    }

    public void ButtonExit()
    {
        Application.Quit();
    }
}
