using UnityEngine;
using System.Collections;

public class MusicSystem : MonoBehaviour {

    public AudioClip[] m_audioClips;
    public float m_Volume = 1;
    AudioSource m_AudioSource;

	void Start () {
        m_Volume = Mathf.Clamp01(m_Volume);
        m_AudioSource = Camera.main.GetComponent<AudioSource>();
        Debug.Log(m_AudioSource.volume);
	}
	
    public void PlayClip(int index)
    {
        StartCoroutine(Crossfade(2f, index));
    }

    IEnumerator Crossfade(float time, int index)
    {
        time = time / 2;

        float f = 0;
        float volume = m_Volume;
        float timeAtStart = Time.realtimeSinceStartup;

        while (f < time)
        {
            f = Time.realtimeSinceStartup - timeAtStart;
            Camera.main.GetComponent<AudioSource>().volume = volume * (1 - f/time);
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
