using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public int m_ID = 0;

	void Start()
    {
	
	}
	
	void Update()
    {
	
	}

    public void SelectLevel()
    {
        if (m_ID >= 0)
        {
            if (m_ID <= SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("Loaded level:" + m_ID);
                SceneManager.LoadScene(m_ID);
            }
            else
            {
                Debug.Log(m_ID);
                Debug.Log(SceneManager.sceneCountInBuildSettings);
                Debug.Log("Level with ID: " + m_ID + " does not exist");
            }
        }
        else
        {
            Debug.Log("Level cannot be loaded, ID is negative");
        }
    }
}
