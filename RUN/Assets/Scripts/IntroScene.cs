using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class IntroScene : MonoBehaviour {

    FadeToBlack m_Fade;
    Image m_Image;
    SoundEmitter m_SoundEmitter;

	// Use this for initialization
	void Start () {
        m_Fade = GetComponent<FadeToBlack>();
        m_Image = GetComponent<Image>();
        m_SoundEmitter = GetComponent<SoundEmitter>();

        StartCoroutine(IntroSequence());
    }

    IEnumerator ScaleLerp()
    {
        float timeElapsed = 0;
        float time = 0.4f;

        while (timeElapsed < time) {

            Vector3 newVector = Vector3.Slerp(Vector3.zero, Vector3.one * 1.2f, timeElapsed / time);

            m_Image.transform.localScale = newVector;

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }

        timeElapsed = 0;
        time = 0.1f;

        while (timeElapsed < time)
        {
            m_Image.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, timeElapsed / time);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(0.6f);
        m_SoundEmitter.PlayClip(0);

        float time = 0.5f;
        float timeElapsed = 0f;

        //StartCoroutine(ScaleLerp());

        while (timeElapsed < time)
        {
            m_Image.color = Color.Lerp(new Color(255, 255, 255, 0), Color.white, timeElapsed / time);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        m_Fade.FadeOut(0.8f);

        timeElapsed = 0f;

        while (timeElapsed < time)
        {
            m_Image.color = Color.Lerp(Color.white, new Color(255, 255, 255, 0), timeElapsed / time);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("Main Menu");
    }
}
