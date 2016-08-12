using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum KeyState
{
    Keyboard,
    Controller
};

public class KeybindingButton : MonoBehaviour
{
    public KeyState m_State = KeyState.Keyboard;
    public string m_PlayerPref;
    public string m_KeyBinding;
    public bool m_IsActive = false;

    Text m_Text;

    void Start()
    {
        m_KeyBinding = PlayerPrefs.GetString(m_PlayerPref, m_KeyBinding);
        PlayerPrefs.SetString(m_PlayerPref, m_KeyBinding);
        m_Text = GetComponentInChildren<Text>();
    }

    void Update()
    {
        m_Text.text = m_KeyBinding;
    }

    public void ButtonKeyBidning()
    {
        if (!GetComponentInParent<InputManager>().ButtonActive())
        m_IsActive = true;
    }
}
