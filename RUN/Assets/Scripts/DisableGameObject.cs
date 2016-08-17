using UnityEngine;
using System.Collections;

public class DisableGameObject : MonoBehaviour
{
    public GameObject m_ToggleGameObject;

    void Start()
    {
        if (!m_ToggleGameObject)
        {
            Debug.Log("DisableGameObject needs a gameobject");
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (m_ToggleGameObject.activeSelf)
                m_ToggleGameObject.SetActive(false);
            else
                m_ToggleGameObject.SetActive(true);
        }
    }
}
