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
    
    public Bounds GetColliderInfo()
    {
        Bounds b = GetComponent<Collider>().bounds;

        Vector2 v1 = b.min - new Vector3(b.max.x, b.min.y);
        Vector2 v2 = new Vector3(b.max.x, b.min.y) - b.max;
        Vector2 v3 = b.max - new Vector3(b.min.x, b.max.y);
        Vector2 v4 = new Vector3(b.max.y, b.min.x) - b.min;

        return m_WallrunObject.GetComponent<Collider>().bounds;
    }

}
