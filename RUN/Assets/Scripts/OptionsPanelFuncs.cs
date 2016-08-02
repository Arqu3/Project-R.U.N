using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsPanelFuncs : MonoBehaviour
{
    //Public vars
    public bool m_Options = false;

    //Component vars
    SimpleSmoothMouseLook m_Camera;
    Text m_ControllerSensText;
    Slider m_ControllerSensSlider;

    //Other vars

    void Start()
    {
        m_Camera = GameObject.Find("Main Camera").GetComponent<SimpleSmoothMouseLook>();
        m_ControllerSensText = transform.FindChild("ControllerSensText").GetComponent<Text>();
        m_ControllerSensSlider = transform.FindChild("ControllerSensSlider").GetComponent<Slider>();

        m_ControllerSensSlider.value = PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f) * 10.0f;
    }

    void Update()
    {
        m_ControllerSensText.text = "Look Sensitivity: " + m_ControllerSensSlider.value / 10;
    }

    public void ButtonOptions()
    {
        m_Options = !m_Options;
    }

    public void OnControllerSensChange()
    {
        PlayerPrefs.SetFloat("Controller Sensitivity", m_ControllerSensSlider.value / 10);
        m_Camera.SetSensitivity(PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f), PlayerPrefs.GetFloat("Controller Sensitivity", 1.0f));
    }
}
