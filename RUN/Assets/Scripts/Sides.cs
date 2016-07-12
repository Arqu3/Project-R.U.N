using UnityEngine;
using System.Collections;

public class Sides : MonoBehaviour {

    ParkourObject m_ParkourObject;
    private Collision m_Collision;
    private bool m_isInit;
    public bool m_CanWallrun;
    public bool m_WallrunSideRight;
    private bool m_WallrunObjectChanged;
    public bool WallrunObjectChanged {
        get
        {
            return m_WallrunObjectChanged;
        }
    }

    int m_lastWallrunObject = 0;
    Object m_WallrunObject;

    void Update()
    {
        if (m_isInit) { 
            if (m_lastWallrunObject != m_WallrunObject.GetInstanceID())
            {
                m_WallrunObjectChanged = true;
                //Debug.Log(m_WallrunObjectChanged);
            }
            else
            {
                m_WallrunObjectChanged = false;
            }

            m_lastWallrunObject = m_WallrunObject.GetInstanceID();
        }
        


        //Debug.Log(m_WallrunObject.GetInstanceID() - m_lastWallrunObject.GetInstanceID());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                m_WallrunObject = col;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        m_CanWallrun = false;

        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                //CheckWallrunObjectChanged(col);
                m_ParkourObject = col.GetComponent<ParkourObject>();
                m_CanWallrun = true;

                m_isInit = true;
            }
        }

    }

    void OnCollisionStay(Collision col)
    {
        Debug.Log("Colliding");

        if (m_CanWallrun) {

            Vector3 colVector = col.contacts[0].point - transform.position;

            colVector = Vector3.Project(colVector, transform.right);

            if (colVector.normalized == transform.right.normalized)
            {
                m_WallrunSideRight = true;
            }
            else
            {
                m_WallrunSideRight = false;
            }


        }
    }


    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Wallrunnable)
            {
                m_ParkourObject = null;
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

        returnVector[0] = m_ParkourObject.transform.forward;
        returnVector[1] = -m_ParkourObject.transform.forward;
        returnVector[2] = m_ParkourObject.transform.right;
        returnVector[3] = -m_ParkourObject.transform.right;

        for (int i = 0; i < 4; i++)
        {
            returnVector[i].y = 0;
        }

        return returnVector;
    }
}
