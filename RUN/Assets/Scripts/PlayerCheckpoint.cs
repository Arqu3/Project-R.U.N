using UnityEngine;
using System.Collections;

public class PlayerCheckpoint : MonoBehaviour {

    public static int m_Current = 0;
    public Transform[] m_CheckPoints;
    Transform m_Temp;
    bool m_HasReachedLast = false;

	void Start ()
    {
        //Find checkpoints
        var checkPoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        if (checkPoints.Length > 0)
        {
            m_CheckPoints = new Transform[checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                m_CheckPoints[i] = checkPoints[i].transform;
            }

            //Sort checkpoint depending on ID
            for (int write = 0; write < checkPoints.Length; write++)
            {
                for (int sort = 0; sort < checkPoints.Length - 1; sort++)
                {
                    if (m_CheckPoints[sort].GetComponent<Checkpoint>().m_ID > m_CheckPoints[sort + 1].GetComponent<Checkpoint>().m_ID)
                    {
                        m_Temp = m_CheckPoints[sort + 1];
                        m_CheckPoints[sort + 1] = m_CheckPoints[sort];
                        m_CheckPoints[sort] = m_Temp;
                    }
                }
            }
        }
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetToLastCheckpoint();
        }
	}

    void SetToLastCheckpoint()
    {
        //Reset player velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //If current is first
        if (m_Current - 1 < 0)
        {
            transform.position = m_CheckPoints[0].position;
        }
        else
        {
            //If current is last
            if (m_HasReachedLast)
            {
                transform.position = m_CheckPoints[m_CheckPoints.Length - 1].position;
            }
            else
            {
                transform.position = m_CheckPoints[m_Current - 1].position;
            }
        }
    }

    void SetReachedLast(bool state)
    {
        m_HasReachedLast = state;
    }
}
