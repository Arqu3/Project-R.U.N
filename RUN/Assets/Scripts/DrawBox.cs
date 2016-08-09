using UnityEngine;
using UnityEditor;
using System.Collections;

public class DrawBox : MonoBehaviour {

    public Color m_BoundsColor;
    public string m_tagToFind;

    public bool init;
    bool m_objectsFound = false;

    GameObject[] m_objectsToDraw;

    void OnValidate()
    {
        if (init) { 

            if (GameObject.FindGameObjectWithTag(m_tagToFind))
            {
                m_objectsToDraw = GameObject.FindGameObjectsWithTag(m_tagToFind);
                m_objectsFound = true;
            }
            else
            {
                m_objectsFound = false;
                Debug.LogError("Tag : " + m_tagToFind + " not found!");
            }

            init = false;
        }
    }

	void OnDrawGizmos()
    {
        if (m_objectsFound) { 

            for (int i = 0; i < m_objectsToDraw.Length; i++)
            {
                DrawObject(i);
            }
        }
    }

    void DrawObject(int index)
    {
        Collider c = m_objectsToDraw[index].GetComponent<Collider>();

        //Vector3 max = (c.transform.localRotation * (c.bounds.max - c.transform.position)) + c.bounds.center;
        // Vector3 min = (c.transform.localRotation * (c.bounds.min + c.transform.position)) + c.bounds.center;

        Vector3 max = (Quaternion.FromToRotation(Vector3.forward, c.transform.forward) * c.bounds.extents) + c.bounds.center;
        Vector3 min = (Quaternion.FromToRotation(Vector3.forward, c.transform.forward) * -c.bounds.extents) + c.bounds.center;

        Gizmos.color = m_BoundsColor;

        Gizmos.DrawLine(max, new Vector3(min.x, max.y, max.z));

        Gizmos.DrawLine(max, new Vector3(max.x, min.y, max.z));

        Gizmos.DrawLine(max, new Vector3(max.x, max.y, min.z));

        Gizmos.DrawLine(min, new Vector3(max.x, min.y, min.z));

        Gizmos.DrawLine(min, new Vector3(min.x, max.y, min.z));

        Gizmos.DrawLine(min, new Vector3(min.x, min.y, max.z));


        Gizmos.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));

        Gizmos.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z));

        Gizmos.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z));

        Gizmos.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(min.x, min.y, max.z));

    }
}
