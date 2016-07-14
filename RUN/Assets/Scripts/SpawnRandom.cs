#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class SpawnRandom : MonoBehaviour {

    public string m_ParentName;
    public string m_TagToFind;
    public GameObject[] m_ObjectsToSpawn;
    public int m_Amount;
    public bool m_Spawn = false;

    Collider m_Collider;
    Transform m_ParentObject;

    void OnValidate()
    {

        if (m_Spawn)
        {
            

            if (!GameObject.Find(m_ParentName))
            {
                m_ParentObject = new GameObject(m_ParentName).transform;
            }
            else
            {
                m_ParentObject = GameObject.Find(m_ParentName).transform;
            }

            GameObject[] objectsWithTag = null;

            if (GameObject.FindGameObjectWithTag(m_TagToFind)) { 
                    objectsWithTag = GameObject.FindGameObjectsWithTag(m_TagToFind);
            
           
                for (int j = 0; j < objectsWithTag.Length; j++)
                {
                    m_Collider = objectsWithTag[j].GetComponent<Collider>();

                    for (int i = 0; i < m_Amount; i++)
                    {
                        float randomX = Random.Range(m_Collider.bounds.min.x, m_Collider.bounds.max.x);
                        float randomZ = Random.Range(m_Collider.bounds.min.z, m_Collider.bounds.max.z);

                        GameObject temp = (GameObject)PrefabUtility.InstantiatePrefab(m_ObjectsToSpawn[i % m_ObjectsToSpawn.Length]);
                        temp.transform.position = new Vector3(randomX, m_Collider.bounds.max.y, randomZ);
                        temp.transform.LookAt(temp.transform.position + new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y));
                        temp.transform.parent = m_ParentObject;
                    }
                }
            }
            else
            {
                Debug.LogError(m_TagToFind + " tag name not found!");
            }
            m_Spawn = false;
        }
    }
}
#endif
