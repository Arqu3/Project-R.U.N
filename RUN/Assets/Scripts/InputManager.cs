using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public KeybindingButton[] m_Buttons;

    public List<string> m_KeyCodePrefs = new List<string>();
    public List<string> m_ControllerPrefs = new List<string>();

    public List<string> m_AxisPrefs = new List<string>();

    public string[] m_PlayerPrefs;

    System.Array values;
    string m_LastAxis = "";

    KeybindingButton m_Temp;

    void Awake()
    {
        values = System.Enum.GetValues(typeof(KeyCode));

        //Find buttons
        var buttons = GameObject.FindGameObjectsWithTag("KeyBindingButton");
        if (buttons.Length > 0)
        {
            m_Buttons = new KeybindingButton[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                m_Buttons[i] = buttons[i].GetComponent<KeybindingButton>();
            }

            //Sort buttons
            for (int write = 0; write < buttons.Length; write++)
            {
                for (int sort = 0; sort < buttons.Length - 1; sort++)
                {
                    if (m_Buttons[sort].GetComponent<KeybindingButton>().m_ID > m_Buttons[sort + 1].GetComponent<KeybindingButton>().m_ID)
                    {
                        m_Temp = m_Buttons[sort + 1];
                        m_Buttons[sort + 1] = m_Buttons[sort];
                        m_Buttons[sort] = m_Temp;
                    }
                }
            }

            for (int i = 0; i < m_Buttons.Length; i++)
            {
                //Add to different list depending on axis or not
                if (buttons[i].GetComponent<KeybindingButton>().m_State.Equals(KeyState.Keyboard) && !buttons[i].GetComponent<KeybindingButton>().m_IsAxis)
                {
                    m_KeyCodePrefs.Add(buttons[i].GetComponent<KeybindingButton>().m_PlayerPref);
                }
                if (buttons[i].GetComponent<KeybindingButton>().m_IsAxis)
                {
                    m_AxisPrefs.Add(buttons[i].GetComponent<KeybindingButton>().m_PlayerPref);
                }
                if (buttons[i].GetComponent<KeybindingButton>().m_State.Equals(KeyState.Controller) && !buttons[i].GetComponent<KeybindingButton>().m_IsAxis)
                {
                    m_ControllerPrefs.Add(buttons[i].GetComponent<KeybindingButton>().m_PlayerPref);
                }
            }
        }

        //Store playerprefs
        m_PlayerPrefs = new string[m_Buttons.Length];

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_PlayerPrefs[i] = m_Buttons[i].m_PlayerPref;
        }
    }

    void Update()
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (m_Buttons[i].m_IsActive)
            {
                foreach (KeyCode code in values)
                {
                    if (Input.GetKeyDown(code))
                    {
                        if (code != KeyCode.Escape)
                        {
                            if (m_Buttons[i].m_State.Equals(KeyState.Keyboard))
                            {
                                if (!IsControllerInput() && !IsControllerAxis())
                                {
                                    if (!DuplicateCheck(i, code.ToString()))
                                    {
                                        //Debug.Log(System.Enum.GetName(typeof(KeyCode), code));
                                        m_Buttons[i].m_KeyBinding = code.ToString();
                                        SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
                                    }
                                }
                            }
                            else if (m_Buttons[i].m_State.Equals(KeyState.Controller))
                            {
                                if (IsControllerInput())
                                {
                                    if (!DuplicateCheck(i, code.ToString()))
                                    {
                                        m_Buttons[i].SetIsAxis(false);
                                        for (int j = 0; j < m_AxisPrefs.Count; j++)
                                        {
                                            if (!m_Buttons[i].m_IsAxis && m_Buttons[i].m_PlayerPref == m_AxisPrefs[j])
                                            {
                                                m_ControllerPrefs.Add(m_AxisPrefs[j]);
                                                m_AxisPrefs.Remove(m_AxisPrefs[j]);
                                            }
                                        }

                                        //Debug.Log(System.Enum.GetName(typeof(KeyCode), code));
                                        m_Buttons[i].m_KeyBinding = code.ToString();
                                        SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
                                    }
                                }
                            }
                        }
                    }
                }

                if (m_Buttons[i].m_State.Equals(KeyState.Controller))
                {
                    if (IsControllerAxis())
                    {
                        if (!DuplicateCheck(i, m_LastAxis))
                        {
                            m_Buttons[i].SetIsAxis(true);
                            for (int j = 0; j < m_ControllerPrefs.Count; j++)
                            {
                                if (m_Buttons[i].m_IsAxis && m_Buttons[i].m_PlayerPref == m_ControllerPrefs[j])
                                {
                                    m_AxisPrefs.Add(m_ControllerPrefs[j]);
                                    m_ControllerPrefs.Remove(m_ControllerPrefs[j]);
                                }
                            }
                            m_Buttons[i].m_KeyBinding = m_LastAxis;
                            SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
                        }
                    }
                }
            }
        }
    }

    void SetKeyBinding(string playerpref, string keybind)
    {
        PlayerPrefs.SetString(playerpref, keybind);
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (m_Buttons[i].m_IsActive)
            {
                m_Buttons[i].m_IsActive = false;
            }
        }
    }

    bool DuplicateCheck(int index, string keybind)
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (i != index)
            {
                if (m_Buttons[i].m_KeyBinding == keybind)
                {
                    Debug.Log("Button with ID: " + m_Buttons[i].m_ID + " already has the keybind: " + keybind);
                    return true;
                }
            }
        }
        return false;
    }

    public bool ButtonActive(int ID)
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (i != ID)
            {
                if (m_Buttons[i].m_IsActive)
                {
                    Debug.Log("ERROR, Another button is already active");
                    return true;
                }
            }
        }
        return false;
    }

    bool IsControllerInput()
    {
        //joystick buttons
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19))
        {
            return true;
        }

        return false;
    }

    bool IsControllerAxis()
    {
        if (Input.GetAxisRaw("Left Trigger") != 0)
        {
            m_LastAxis = "Left Trigger";
            return true;
        }
        else if (Input.GetAxisRaw("Right Trigger") != 0)
        {
            m_LastAxis = "Right Trigger";
            return true;
        }
        else if (Input.GetAxisRaw("DPadX") != 0)
        {
            m_LastAxis = "DPadX";
            return true;
        }
        else if (Input.GetAxisRaw("DPadY") != 0)
        {
            m_LastAxis = "DPadY";
            return true;
        }
        return false;
    }

    public string[] GetPrefs()
    {
        return m_PlayerPrefs;
    }

    public List<string> GetKeyCodes()
    {
        return m_KeyCodePrefs;
    }

    public List<string> GetControllerPrefs()
    {
        return m_ControllerPrefs;
    }

    public List<string> GetAxisPrefs()
    {
        return m_AxisPrefs;
    }

    void PrintKeybindings()
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            Debug.Log(m_Buttons[i].m_PlayerPref + " has " + m_Buttons[i].m_KeyBinding);
        }
    }
}
