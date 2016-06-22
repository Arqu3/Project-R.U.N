using UnityEngine;
using System.Collections;

public class Sides : MonoBehaviour {

    ParkourObject m_WallrunObject;
    public bool m_CanWallrun;

    void OnTriggerStay(Collider col)
    {
        m_CanWallrun = false;
        m_WallrunObject = null;

        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                m_WallrunObject = col.GetComponent<ParkourObject>();
                m_CanWallrun = true;
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                m_CanWallrun = false;
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
