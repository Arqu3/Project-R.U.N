using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour
{
    public string m_SceneName = "";

	void Start()
    {
        if (!Application.CanStreamedLevelBeLoaded(m_SceneName))
        {
            Debug.Log("ERROR, scene with name: " + m_SceneName + " doesn't exist in current build settings");
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(m_SceneName);
        }
    }
}
