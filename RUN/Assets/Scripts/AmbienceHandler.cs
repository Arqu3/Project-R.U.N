using UnityEngine;
using System.Collections;

public class AmbienceHandler : MonoBehaviour {

    //Public vars
    public float m_Volume;

    //Component vars
    AudioSource m_AudioSource;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        m_Volume = PlayerPrefs.GetFloat("Sound Volume", 0.5f);

        m_AudioSource.volume = m_Volume;
    }

    void Start ()
    {
	}
	
	void Update ()
    {
	
	}

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        m_Volume = volume;
        m_AudioSource.volume = m_Volume;
    }
}
