using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour {

    public int m_ID = 0;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    public void SelectLevel()
    {
        Debug.Log("Loaded level:" + (m_ID + 1));
        SceneManager.LoadScene(m_ID);
    }
}
