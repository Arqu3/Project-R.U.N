﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerUI : MonoBehaviour {

    Text m_BlinkText;
    ControllerPlayer m_Player;
    PausePanelFuncs m_PausePanel;
    GameObject m_ScorePanel;
    TutorialPanelFuncs m_TutorialPanel;

    public AudioClip StartMenuMusic;

    bool m_Init = false;
    bool m_Paused = false;
    bool m_MusicStarted = false;
    bool m_IsScoreScreen = false;

	void Start () {
        m_Init = false;
        m_Paused = false;
        m_MusicStarted = false;

        if (transform.FindChild("BlinkText"))
            m_BlinkText = transform.FindChild("BlinkText").GetComponent<Text>();
        if (GameObject.FindGameObjectWithTag("Player"))
            m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<ControllerPlayer>();
        if (transform.FindChild("PausePanel"))
            m_PausePanel = transform.FindChild("PausePanel").GetComponent<PausePanelFuncs>();
        if (transform.FindChild("EndPanel"))
            m_ScorePanel = transform.FindChild("EndPanel").gameObject;
        if (transform.FindChild("TutorialPanel"))
            m_TutorialPanel = transform.FindChild("TutorialPanel").GetComponent<TutorialPanelFuncs>();

        if (PlayerPrefs.GetInt("Restart", 0) != 1)
        {
            m_ScorePanel.SetActive(false);
            m_TutorialPanel.gameObject.SetActive(false);

            PauseUpdate();

            Cursor.lockState = CursorLockMode.None;

            float volume = 1;

            if (!m_PausePanel.ToggleSound)
            {
                volume = 0;
            }

            Camera.main.GetComponent<MusicSystem>().SetVolume(volume);

            Camera.main.GetComponent<MusicSystem>().PlayClip(0);
            m_Init = true;
        }
        else
        {
            m_Init = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            m_PausePanel.gameObject.SetActive(false);
            m_TutorialPanel.gameObject.SetActive(false);
            m_ScorePanel.SetActive(false);
            m_Player.ToggleControls(true);
            Time.timeScale = 1;
            PlayerPrefs.SetInt("Restart", 0);
        }
    }

    void Update () {
        m_TutorialPanel.gameObject.SetActive(m_TutorialPanel.m_Tutorial);
        if (!m_IsScoreScreen)
        {
            PauseUpdate();
            BlinkUpdate();
        }
        else
        {
            ScoreScreenUpdate();
        }

        float volume = 1;

        if (!m_PausePanel.ToggleSound)
        {
            volume = 0;
        }

        Camera.main.GetComponent<MusicSystem>().SetVolume(volume);
    }

    void PauseUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || !m_Init || m_PausePanel.UnPause){
            int timeInt = 0;
            m_Paused = !m_Paused;

            if (!m_Paused)
            {
                timeInt = 1;
            }

            m_Player.ToggleControls(!m_Paused);
            m_PausePanel.UnPause = false;
            m_PausePanel.gameObject.SetActive(m_Paused);
            Camera.main.GetComponent<SimpleSmoothMouseLook>().lockCursor = !m_Paused;
			Cursor.lockState = CursorLockMode.None;
			Camera.main.GetComponent<SimpleSmoothMouseLook>().enabled = !m_Paused;
            Cursor.visible = m_Paused;

			Time.timeScale = timeInt;

            if (!m_MusicStarted && m_Init)
            {
                m_MusicStarted = true;
                Camera.main.GetComponent<MusicSystem>().PlayClip(1);
            }
        }
    }

    void ScoreScreenUpdate()
    {
        m_Player.ToggleControls(false);
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<SimpleSmoothMouseLook>().enabled = false;
        Cursor.visible = true;

        if (m_PausePanel.gameObject.activeSelf)
        {
            m_PausePanel.gameObject.SetActive(false);
        }
        if (!m_ScorePanel.activeSelf)
        {
            m_ScorePanel.SetActive(true);
        }
    }

    void BlinkUpdate()
    {
        float [] cdValues = m_Player.GetUIValues();

        if (cdValues[0] == cdValues[1])
        {
            //Set text to display blink is ready
            m_BlinkText.text = "Blink Ready";
            m_BlinkText.color = Color.green;
        }
        else
        {
            //Set text to current CD
            m_BlinkText.text = cdValues[0].ToString("F1");
            m_BlinkText.color = Color.red;
        }
    }

    void ToggleScoreScreen()
    {
        m_IsScoreScreen = !m_IsScoreScreen;
    }
}
