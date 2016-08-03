using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEmitter : MonoBehaviour {

    public float m_Volume = 0.1f;
    public AudioClip[] m_AudioClips;
    AudioSource m_AudioSource;
    
	void Start () {
        m_AudioSource = GetComponent<AudioSource>();
        m_Volume = PlayerPrefs.GetFloat("Sound Volume", 0.5f);
        m_Volume = Mathf.Clamp01(m_Volume);
        m_AudioSource.volume = m_Volume;
	}

    /// <summary>
    /// Plays the clip at the index from AudioClips
    /// </summary>
    /// <param name="index"></param>
    public void PlayClip(int index)
    {
        m_AudioSource.clip = m_AudioClips[index];
        m_AudioSource.Play();
    }

    /// <summary>
    /// Plays a random clip from AudioClips, in the range: offset to max length
    /// </summary>
    /// <param name="offset"></param>
    public void PlayRandomClip(int offset)
    {
        PlayClip(Mathf.RoundToInt(Random.Range(offset, m_AudioClips.Length)));
    }

    /// <summary>
    /// Plays a random clip in the range min to max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void PlayRandomClip(int min, int max)
    {
        if (max > m_AudioClips.Length)
            max = m_AudioClips.Length;

        PlayClip(Mathf.RoundToInt(Random.Range(min, max)));
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("Sound Volume", volume);
        m_Volume = volume;
        m_AudioSource.volume = m_Volume;
    }
}
