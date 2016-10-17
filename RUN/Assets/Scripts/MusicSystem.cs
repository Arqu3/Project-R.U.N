using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(AudioSource))]
public class MusicSystem : MonoBehaviour {

    public bool m_FindCamera;
    public bool m_PlayOnAwake;
    public AudioClip[] m_audioClips;
    public float m_Volume = 0.1f;
    AudioSource m_AudioSource;
    bool m_AudioBusy;

	void Start () {

        if (PlayerPrefs.GetInt("Toggle Sound", 0) == 1)
        {
            m_Volume = PlayerPrefs.GetFloat("Music Volume", 0.5f);
            m_Volume = Mathf.Clamp01(m_Volume);
            Debug.Log("Music volume set");
        }
        else
            m_Volume = 0;

        if (m_FindCamera)
            m_AudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        else
        { 
            m_AudioSource = GetComponent<AudioSource>();
        }

        if (m_PlayOnAwake)
        {
            PlayClip(0);
        }
    }
	
    public void PlayClip(int index)
    {
        StartCoroutine(Crossfade(2f, index));
    }

    public void SetVolume(float volume)
    {
        if (volume != m_Volume && !m_AudioBusy) {
            //PlayerPrefs.SetFloat("Music Volume", volume);
            m_Volume = volume;
            StartCoroutine(LerpVolume(volume));
        }
    }

    IEnumerator LerpVolume(float toVolume)
    {
        m_AudioBusy = true;
        float timeAtStart = Time.realtimeSinceStartup;
        float t = 0;

        while (t < 1)
        {
            t = Time.realtimeSinceStartup - timeAtStart;

            if (m_FindCamera)
            {
                Camera.main.GetComponent<AudioSource>().volume = Mathf.Lerp(m_Volume, toVolume, t);
            }
            else
            {
                GetComponent<AudioSource>().volume = Mathf.Lerp(m_Volume, toVolume, t);
            }
            yield return new WaitForEndOfFrame();
        }

        m_Volume = toVolume;
        m_AudioBusy = false;

    }

    IEnumerator Crossfade(float time, int index)
    {
        while (m_AudioBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        time = time / 2;

        float f = 0;
        float volume = m_Volume;
        float timeAtStart = Time.realtimeSinceStartup;

        while (f < time)
        {
            f = Time.realtimeSinceStartup - timeAtStart;

            if (m_FindCamera)
            {
                Camera.main.GetComponent<AudioSource>().volume = volume * (1 - f / time);
            }
            else
            {
                GetComponent<AudioSource>().volume = volume * (1 - f / time);
            }
            
            yield return new WaitForEndOfFrame();
        }

        m_AudioSource.clip = m_audioClips[index];
        m_AudioSource.Play();

        timeAtStart = Time.realtimeSinceStartup;
        f = 0;

        while (f < time)
        {
            f = Time.realtimeSinceStartup - timeAtStart;
            m_AudioSource.volume = volume * (f / time);
            yield return new WaitForEndOfFrame();
        }

        m_AudioSource.volume = volume;
    }
}
