using UnityEngine;
using System.Collections;

public class Sides : MonoBehaviour {

    ParkourObject m_WallrunObject;
    public bool m_CanWallrun;
    private bool m_WallrunObjectChanged;
    public bool WallrunObjectChanged {
        get
        {
            return m_WallrunObjectChanged;
        }
    }

    void OnTriggerStay(Collider col)
    {
        m_CanWallrun = false;

        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                CheckWallrunObjectChanged(col);
                m_WallrunObject = col.GetComponent<ParkourObject>();
                m_CanWallrun = true;
            }
            else
            {
                m_WallrunObject = null;
            }
        }
        else
        {
            m_WallrunObject = null;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                m_WallrunObject = null;
                m_CanWallrun = false;
            }
        }
    }

    void CheckWallrunObjectChanged(Collider c)
    {
        if (c != null) {
            if (m_WallrunObject != null) {  

                if (m_WallrunObject.Equals(c.GetComponent<ParkourObject>()))
                {
                    m_WallrunObjectChanged = false;
                }
                else
                {
                    m_WallrunObjectChanged = false;
                }
            }
            else
            {
                m_WallrunObjectChanged = false;
            }
        }
    }

    public Vector3[] GetColliderInfo()
    {
        Bounds b = GetComponent<Collider>().bounds;

        Vector3[] returnVector = new Vector3[4];

        returnVector[0] = b.min - new Vector3(b.max.x, 0, b.min.z);
        returnVector[1] = new Vector3(b.max.x, 0, b.min.z) - b.max;
        returnVector[2] = b.max - new Vector3(b.min.x, 0,b.max.z);
        returnVector[3] = new Vector3(b.max.y, 0,b.min.z) - b.min;

        for (int i = 0; i < 4; i++)
        {
            returnVector[i].y = 0;
        }

        return returnVector;
    }

}
