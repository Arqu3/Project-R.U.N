using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EssentialsSpawner : MonoBehaviour {

    public GameObject m_Camera;
    public GameObject m_Canvas;
    public GameObject m_Player;
    bool m_Init = false;
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!GameObject.FindGameObjectWithTag("Canvas"))
        {
            PrefabUtility.InstantiatePrefab(m_Canvas);
        }
        if (!GameObject.FindGameObjectWithTag("MainCamera"))
        {
            PrefabUtility.InstantiatePrefab(m_Camera);
        }
        if (!GameObject.FindGameObjectWithTag("Player"))
        {
            PrefabUtility.InstantiatePrefab(m_Player);
        }
    }

    void OnDrawGizmosSelected()
    {
        DestroyImmediate(this.gameObject, true);
    }
#endif
}
