using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelFuncs : MonoBehaviour {

    bool m_Unpause;
    public bool m_ToggleSound = false;

    ControllerUI m_UI;
    Toggle m_MusicToggle;

    void Awake()
    {
        m_MusicToggle = GameObject.Find("Toggle").GetComponent<Toggle>();
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("Toggle Sound", 0) == 0)
        {
            m_ToggleSound = true;
        }
        else
        {
            m_ToggleSound = false;
        }
        m_MusicToggle.isOn = !m_ToggleSound;

        m_UI = GetComponentInParent<ControllerUI>();
    }

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
        if (ToggleSound)
        {
            PlayerPrefs.SetInt("Toggle Sound", 1);
            Camera.main.GetComponent<MusicSystem>().SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Music Volume", 0.5f)));
            Camera.main.GetComponentInChildren<AmbienceHandler>().SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Sound Volume", 0.5f)));
            GameObject.Find("AudioEmitter").GetComponent<SoundEmitter>().SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Sound Volume", 0.5f)));
        }
        else
        {
            PlayerPrefs.SetInt("Toggle Sound", 0);
            Camera.main.GetComponent<MusicSystem>().SetVolume(0);
            Camera.main.GetComponentInChildren<AmbienceHandler>().SetVolume(0);
            GameObject.Find("AudioEmitter").GetComponent<SoundEmitter>().SetVolume(0);
        }
    }
}
