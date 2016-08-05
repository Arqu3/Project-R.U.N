using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsPanelFuncs : MonoBehaviour
{
    //Public vars
    public bool m_Options = false;
    public bool m_IsMainMenu;

    //Component vars
    SimpleSmoothMouseLook m_Camera;
    MusicSystem m_Music;
    SoundEmitter m_Sounds;
    AmbienceHandler m_Ambience;

    //Sensitivity
    Text m_ControllerSensText;
    Slider m_ControllerSensSlider;

    //Music
    Text m_MusicText;
    Slider m_MusicSlider;

    //Sounds
    Text m_SoundText;
    Slider m_SoundSlider;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            m_IsMainMenu = true;
        }
        else
        {
            m_IsMainMenu = false;
        }

        if (GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>())
        {
            m_Camera = GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>();
        }
        if (GameObject.Find("Main Camera").GetComponent<MusicSystem>())
        {
            m_Music = GameObject.Find("Main Camera").GetComponent<MusicSystem>();
        }
        if (!m_IsMainMenu)
        {
            if (GameObject.Find("AudioEmitter").GetComponent<SoundEmitter>())
            {
                m_Sounds = GameObject.Find("AudioEmitter").GetComponent<SoundEmitter>();
            }
            if (GameObject.Find("Ambience").GetComponent<AmbienceHandler>())
            {
                m_Ambience = GameObject.Find("Ambience").GetComponent<AmbienceHandler>();
            }
        }

        m_ControllerSensText = transform.FindChild("ControllerSensText").GetComponent<Text>();
        m_ControllerSensSlider = transform.FindChild("ControllerSensSlider").GetComponent<Slider>();
        m_ControllerSensSlider.value = PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f) * 10.0f;

        m_MusicText = transform.FindChild("MusicText").GetComponent<Text>();
        m_MusicSlider = transform.FindChild("MusicSlider").GetComponent<Slider>();
        m_MusicSlider.value = PlayerPrefs.GetFloat("Music Volume") * 100.0f;

        m_SoundText = transform.FindChild("SoundText").GetComponent<Text>();
        m_SoundSlider = transform.FindChild("SoundSlider").GetComponent<Slider>();
        m_SoundSlider.value = PlayerPrefs.GetFloat("Sound Volume") * 100.0f;
    }

    void Update()
    {
        m_ControllerSensText.text = "Look Sensitivity: " + m_ControllerSensSlider.value / 10;
        if (!m_MusicText)
            m_MusicText = transform.FindChild("MusicText").GetComponent<Text>();
        if (!m_MusicSlider)
            m_MusicSlider = transform.FindChild("MusicSlider").GetComponent<Slider>();
        if (!m_SoundText)
            m_SoundText = transform.FindChild("SoundText").GetComponent<Text>();
        if (!m_SoundSlider)
            m_SoundSlider = transform.FindChild("SoundSlider").GetComponent<Slider>();
        m_MusicText.text = "Music Volume: " + m_MusicSlider.value;
        m_SoundText.text = "Sound Volume: " + m_SoundSlider.value;
    }

    public void ButtonOptions()
    {
        m_Options = !m_Options;
    }

    public void OnControllerSensChange()
    {
        if (m_Options || m_IsMainMenu)
        {
            PlayerPrefs.SetFloat("Controller Sensitivity", m_ControllerSensSlider.value / 10);
            if (m_Camera)
                m_Camera.SetSensitivity(PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f), PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f));
        }
    }

    public void OnMusicVolumeChange()
    {
        if (m_Options || m_IsMainMenu)
        {
            PlayerPrefs.SetFloat("Music Volume", m_MusicSlider.value / 100);
            if (m_Music)
                m_Music.SetVolumeConst(PlayerPrefs.GetFloat("Music Volume", 0.5f));
        }
    }

    public void OnSoundVolumeChange()
    {
        if (m_Options || m_IsMainMenu)
        {
            PlayerPrefs.SetFloat("Sound Volume", m_SoundSlider.value / 100);
            if (m_Sounds)
                m_Sounds.SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
            if (m_Ambience)
                m_Ambience.SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
        }
    }
}
