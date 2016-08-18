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
    public int m_ID = 0;
    public KeyState m_State = KeyState.Keyboard;
    public string m_PlayerPref;
    public string m_KeyBinding;
    public bool m_IsActive = false;
    public bool m_IsAxis = false;

    Text m_Text;

    void Start()
    {
        m_KeyBinding = PlayerPrefs.GetString(m_PlayerPref, m_KeyBinding);
        PlayerPrefs.SetString(m_PlayerPref, m_KeyBinding);
        m_Text = GetComponentInChildren<Text>();

        if (m_KeyBinding == "Right Trigger" || m_KeyBinding == "Left Trigger" || m_KeyBinding == "DPadX" || m_KeyBinding == "DPadY")
        {
            PlayerPrefs.SetInt("IsAxis" + m_ID + "1", 1);
            //Debug.Log("Button with ID: " + m_ID + " is axis");
        }

        if (PlayerPrefs.GetInt("IsAxis" + m_ID + "1", 0) == 1)
            m_IsAxis = true;
        else if (PlayerPrefs.GetInt("IsAxis" + m_ID + "1", 0) == 0)
            m_IsAxis = false;
    }

    void Update()
    {
        m_Text.text = m_KeyBinding;
    }

    public void ButtonKeyBidning()
    {
        if (!GetComponentInParent<InputManager>().ButtonActive(m_ID))
            m_IsActive = true;
    }

    public void SetIsAxis(bool state)
    {
        m_IsAxis = state;
        if (m_IsAxis)
            PlayerPrefs.SetInt("IsAxis" + m_ID + "1", 1);
        else
            PlayerPrefs.SetInt("IsAxis" + m_ID + "1", 0);
        PlayerPrefs.Save();
    }
}
