using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Texture2D m_Image;
    static bool loading = false;
    static int m_Index;

    GUITexture m_guiTexture;

    void Start()
    {

    }

    void OnGUI()
    {
        if (loading)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Image);
            GUI.BringWindowToBack(0);
        }
    }

    IEnumerator LoadWithFade()
    {
        FadeToBlack fade = GetComponent<FadeToBlack>();

        fade.FadeOut(0.5f);

        Time.timeScale = 1;

        yield return new WaitForSeconds(0.5f);
        loading = true;

        fade.FadeIn(0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(m_Index);

        if (!SceneManager.LoadSceneAsync(m_Index).isDone)
        {

            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 0;

        loading = false;
    }

    public void Load(int index)
    {
        m_Index = index;

        StartCoroutine(LoadWithFade());
    }

}