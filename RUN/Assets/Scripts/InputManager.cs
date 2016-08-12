using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public KeybindingButton[] m_Buttons;

    System.Array values;
    string m_LastAxis = "";

    void Start()
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
                        if (m_Buttons[i].m_State.Equals(KeyState.Keyboard))
                        {
                            if (!IsControllerInput())
                            {
                                //Debug.Log(System.Enum.GetName(typeof(KeyCode), code));
                                m_Buttons[i].m_KeyBinding = code.ToString();
                                SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
                            }
                        }
                        else if (m_Buttons[i].m_State.Equals(KeyState.Controller))
                        {
                            if (IsControllerInput())
                            {
                                //Debug.Log(System.Enum.GetName(typeof(KeyCode), code));
                                m_Buttons[i].m_KeyBinding = code.ToString();
                                SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
                            }
                        }
                    }
                }

                if (m_Buttons[i].m_State.Equals(KeyState.Controller))
                {
                    if (IsControllerAxis())
                    {
                        m_Buttons[i].m_KeyBinding = m_LastAxis;
                        SetKeyBinding(m_Buttons[i].m_PlayerPref, m_Buttons[i].m_KeyBinding);
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
        Debug.Log(playerpref + " set to " + keybind);
    }

    public bool ButtonActive()
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (m_Buttons[i].m_IsActive)
                return true;
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

    bool IsControllerAxisAll()
    {
        if (Input.GetAxisRaw("Left Trigger") != 0 ||
            Input.GetAxisRaw("Right Trigger") != 0 ||
            Input.GetAxisRaw("DPadX") != 0 ||
            Input.GetAxisRaw("DPadY") != 0 ||
            Input.GetAxisRaw("Joystick X") != 0 ||
            Input.GetAxisRaw("Joystick Y") != 0 ||
            Input.GetAxisRaw("Vertical") != 0 ||
            Input.GetAxisRaw("Horizontal") != 0)
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
        return false;
    }
}
