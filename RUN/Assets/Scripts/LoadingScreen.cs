using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LoadingScreen : MonoBehaviour
{
    static int m_Index;

    private static LoadingScreen _instance;
    private LoadingScreen() { }

    private static object _lock = new object();

    public static LoadingScreen Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(LoadingScreen) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (LoadingScreen)FindObjectOfType(typeof(LoadingScreen));

                    if (FindObjectsOfType(typeof(LoadingScreen)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<LoadingScreen>();
                        _instance.gameObject.AddComponent<FadeToBlack>();
                        singleton.name = "(singleton) " + typeof(LoadingScreen).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(LoadingScreen) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public static void Load(int index)
    {
        _instance.pLoad(index);
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    IEnumerator LoadWithFade()
    {
        FadeToBlack fade = GetComponent<FadeToBlack>();

        fade.FadeOut(0.5f);

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));
        SceneManager.LoadScene("LoadingScene");
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.1f));

        fade.FadeIn(1f);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));

        AsyncOperation op = SceneManager.LoadSceneAsync(m_Index);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }    

        fade.FadeOut(0.5f);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));
        op.allowSceneActivation = true;

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));

       
        fade.FadeIn(1f);
    }

    private void pLoad(int index)
    {
        m_Index = index;

        StartCoroutine(LoadWithFade());
    }

}