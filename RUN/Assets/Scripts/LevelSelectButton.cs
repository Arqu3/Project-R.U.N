using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public int m_ID = 0;

	void Start()
    {
	
	}
	
	void Update()
    {
        if (m_ID == 4)
        {
            GetComponentInChildren<Text>().text = "Tutorial";
        }
        else
        {
            GetComponentInChildren<Text>().text = "Level: " + m_ID;
        }
	}

    public void SelectLevel()
    {
        if (m_ID >= 0)
        {
            if (m_ID <= SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("Loaded level:" + m_ID);
                Camera.main.GetComponent<LoadingScreen>().Load(m_ID);
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
