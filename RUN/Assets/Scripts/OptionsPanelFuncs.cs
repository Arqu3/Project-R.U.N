using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum State
{
    MainMenu,
    InGame
};

public class OptionsPanelFuncs : MonoBehaviour
{
    //Public vars
    public bool m_Options = false;
    public State m_State;

    //Component vars
    SimpleSmoothMouseLook m_Camera;
    MusicSystem m_Music;
    ControllerUI m_UI;
    public SoundEmitter[] m_Sounds;
    AmbienceHandler m_Ambience;
    PausePanelFuncs m_Pause;
    ControllerPlayer m_Player;
    InputManager m_InputManager;

    //Sensitivity
    Text m_ControllerSensText;
    Slider m_ControllerSensSlider;

    //Music
    Text m_MusicText;
    Slider m_MusicSlider;

    //Sounds
    Text m_SoundText;
    Slider m_SoundSlider;

    public bool m_ToggleSound;
    Text m_ToggleButton;

    void Start()
    {
        //PlayerPrefs.SetInt("Toggle Sound", 0);
        //Debug.Log("Set default toggle value");

        //Check if current scene is main menu or not
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetState(State.MainMenu);
        }
        else
        {
            SetState(State.InGame);
        }

        //Get camera components
        if (GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>())
        {
            m_Camera = GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>();
        }
        if (GameObject.Find("Main Camera").GetComponent<MusicSystem>())
        {
            m_Music = GameObject.Find("Main Camera").GetComponent<MusicSystem>();
        }

        //Get inputmanager
        m_InputManager = GetComponent<InputManager>();

        //Find child components
        //Sensitivity-slider
        m_ControllerSensText = transform.FindChild("ControllerSensText").GetComponent<Text>();
        m_ControllerSensSlider = transform.FindChild("ControllerSensSlider").GetComponent<Slider>();
        m_ControllerSensSlider.value = PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f) * 10.0f;

        //Music slider
        m_MusicText = transform.FindChild("MusicText").GetComponent<Text>();
        m_MusicSlider = transform.FindChild("MusicSlider").GetComponent<Slider>();
        m_MusicSlider.value = PlayerPrefs.GetFloat("Music Volume") * 100.0f;

        //Sound slider
        m_SoundText = transform.FindChild("SoundText").GetComponent<Text>();
        m_SoundSlider = transform.FindChild("SoundSlider").GetComponent<Slider>();
        m_SoundSlider.value = PlayerPrefs.GetFloat("Sound Volume") * 100.0f;

        //Toggle button
        //m_Toggle = transform.FindChild("Toggle").GetComponent<Toggle>();
        m_ToggleButton = transform.FindChild("Toggle").GetComponentInChildren<Text>();

        //Get components if in-game
        if (m_State.Equals(State.InGame))
        {
            //Player
            m_Player = GameObject.Find("PlayerBody").GetComponent<ControllerPlayer>();
            //m_Player.SetKeybinds(m_InputManager.GetPrefs(), m_InputManager.GetKeyCodes(), m_InputManager.GetAxisPrefs());

            //Sounds
            var sounds = GameObject.FindGameObjectsWithTag("Sound Emitter");
            if (sounds.Length > 0)
            {
                m_Sounds = new SoundEmitter[sounds.Length];
                for (int i = 0; i < sounds.Length; i++)
                {
                    m_Sounds[i] = sounds[i].GetComponent<SoundEmitter>();
                }
                for (int i = 0; i < m_Sounds.Length; i++)
                {
                    if (m_ToggleSound)
                        m_Sounds[i].SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
                    else
                        m_Sounds[i].SetVolume(0);
                }
            }

            //Ambience
            if (GameObject.Find("Ambience").GetComponent<AmbienceHandler>())
            {
                m_Ambience = GameObject.Find("Ambience").GetComponent<AmbienceHandler>();
                if (m_ToggleSound)
                    m_Ambience.SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
                else
                    m_Ambience.SetVolume(0);
            }
            //UI
            if (GameObject.Find("Canvas").GetComponent<ControllerUI>())
            {
                m_UI = GameObject.Find("Canvas").GetComponent<ControllerUI>();
            }
            //Pause panel
            if (GameObject.Find("PausePanel").GetComponent<PausePanelFuncs>())
            {
                m_Pause = GameObject.Find("PausePanel").GetComponent<PausePanelFuncs>();
            }
        }

        //Set toggle value
        if (PlayerPrefs.GetInt("Toggle Sound", 0) == 0)
        {
            m_ToggleSound = false;
        }
        else
        {
            m_ToggleSound = true;
        }
    }

    void Update()
    {
        //Update text values
        m_ControllerSensText.text = "Look Sensitivity: " + m_ControllerSensSlider.value / 10;
        m_MusicText.text = "Music Volume: " + m_MusicSlider.value;
        m_SoundText.text = "Sound Volume: " + m_SoundSlider.value;

        if (m_ToggleSound)
            m_ToggleButton.text = "Sound Enabled";
        else
            m_ToggleButton.text = "Sound Disabled";

        if (m_Options && Input.GetButtonDown("Pause"))
            ButtonOptions();
    }

    void LateUpdate()
    {
        //Find child-objects if they're missing
        if (!m_MusicText)
            m_MusicText = transform.FindChild("MusicText").GetComponent<Text>();
        if (!m_MusicSlider)
            m_MusicSlider = transform.FindChild("MusicSlider").GetComponent<Slider>();
        if (!m_SoundText)
            m_SoundText = transform.FindChild("SoundText").GetComponent<Text>();
        if (!m_SoundSlider)
            m_SoundSlider = transform.FindChild("SoundSlider").GetComponent<Slider>();
        if (!m_ToggleButton)
            m_ToggleButton = transform.FindChild("Toggle").GetComponentInChildren<Text>();
    }

    public void ButtonOptions()
    {
        if (m_Options)
            m_Player.SetKeybinds(m_InputManager.GetPrefs(), m_InputManager.GetKeyCodes(), m_InputManager.GetAxisPrefs());
        m_Options = !m_Options;
    }

    public void OnControllerSensChange()
    {
        if (m_Options || m_State.Equals(State.MainMenu))
        {
            PlayerPrefs.SetFloat("Controller Sensitivity", m_ControllerSensSlider.value / 10);
            if (m_Camera)
                m_Camera.SetSensitivity(PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f), PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f));
        }
    }

    public void OnMusicVolumeChange()
    {
        if (m_Options || m_State.Equals(State.MainMenu))
        {
            PlayerPrefs.SetFloat("Music Volume", m_MusicSlider.value / 100);
            if (m_UI && m_Music && m_ToggleSound)
            {
                m_UI.SetVolume(PlayerPrefs.GetFloat("Music Volume", 0.5f));
                m_Music.SetVolume(PlayerPrefs.GetFloat("Music Volume", 0.5f));
            }
        }
    }

    public void OnSoundVolumeChange()
    {
        if (m_Options || m_State.Equals(State.MainMenu))
        {
            PlayerPrefs.SetFloat("Sound Volume", m_SoundSlider.value / 100);
            if (m_Sounds.Length > 0 && m_ToggleSound)
            {
                for (int i = 0; i < m_Sounds.Length; i++)
                {
                    m_Sounds[i].SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
                }
            }
            if (m_Ambience && m_ToggleSound)
                m_Ambience.SetVolume(PlayerPrefs.GetFloat("Sound Volume", 0.5f));
        }
    }

    public void ToggleSound()
    {
        m_ToggleSound = !m_ToggleSound;
        if (m_ToggleSound)
        {
            PlayerPrefs.SetInt("Toggle Sound", 1);

            if (m_State.Equals(State.InGame))
            {
                m_Music.SetVolumeMuted(Mathf.Clamp01(PlayerPrefs.GetFloat("Music Volume", 0.5f)));
                m_Ambience.SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Sound Volume", 0.5f)));
                m_UI.SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Music Volume", 0.5f)));
                for (int i = 0; i < m_Sounds.Length; i++)
                {
                    m_Sounds[i].SetVolume(Mathf.Clamp01(PlayerPrefs.GetFloat("Sound Volume", 0.5f)));
                }
                Debug.Log("Sound enabled");
            }
        }
        else
        {
            PlayerPrefs.SetInt("Toggle Sound", 0);

            if (m_State.Equals(State.InGame))
            {
                m_Music.SetVolumeMuted(0);
                m_Ambience.SetVolume(0);
                m_UI.SetVolume(0);
                for (int i = 0; i < m_Sounds.Length; i++)
                {
                    m_Sounds[i].SetVolume(0);
                }
                Debug.Log("Sound disabled");
            }
        }
    }

    public bool GetIsSound()
    {
        return m_ToggleSound;
    }

    void SetState(State newState)
    {
        m_State = newState;
    }

    public State GetState()
    {
        return m_State;
    }
}
