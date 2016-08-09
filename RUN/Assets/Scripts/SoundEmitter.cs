using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEmitter : MonoBehaviour {

    public float m_Volume = 0.1f;
    public AudioClip[] m_AudioClips;
    AudioSource m_AudioSource;

    float m_crossfadeTime;
    int m_crossfadeClipIndex;

    int m_CurrentClipIndex;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Volume = PlayerPrefs.GetFloat("Sound Volume", 0.5f);
        m_Volume = Mathf.Clamp01(m_Volume);
        m_AudioSource.volume = m_Volume;
    }

    void Update()
    {

    }

    /// <summary>
    /// Plays the clip at the index from AudioClips
    /// </summary>
    /// <param name="index"></param>
    public void PlayClip(int index)
    {
        m_AudioSource.clip = m_AudioClips[index];
        m_CurrentClipIndex = index;
        m_AudioSource.Play();
    }

    /// <summary>
    /// Plays a random clip from AudioClips, in the range: offset to max length
    /// </summary>
    /// <param name="offset"></param>
    public void PlayRandomClip(int offset)
    {
        int random = Mathf.RoundToInt(Random.Range(offset, m_AudioClips.Length));
        m_CurrentClipIndex = random;

        PlayClip(random);
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

        int random = Mathf.RoundToInt(Random.Range(min, max));
        m_CurrentClipIndex = random;

        PlayClip(random);
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        m_Volume = volume;
        m_AudioSource.volume = m_Volume;
    }

    public void CrossfadeToClip(int index, float time)
    {
        m_crossfadeClipIndex = index;
        m_crossfadeTime = time;
        StartCoroutine(CrossfadeClip());
    }

    IEnumerator CrossfadeClip()
    {
        float timer = 0;
        float min = m_Volume, max = 0;

        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                PlayClip(m_crossfadeClipIndex);

                max = min;
                min = 0;
            }

            while (timer < m_crossfadeTime * 0.5f)
            {
                SetVolume(Mathf.Lerp(min, max, timer / (m_crossfadeTime * 0.5f)));
                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            SetVolume(max);
            timer = 0;

            Debug.Log("LOOPING");
        }
    }

    public void ToggleLoop(bool active)
    {
        m_AudioSource.loop = active;
    }

}
