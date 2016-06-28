﻿using UnityEngine;
using System.Collections;

public class PlayerCheckpoint : MonoBehaviour {

    public float ResetDepth = 0.0f;
    public static int m_LastPassed = 0;
    public Transform[] m_CheckPoints;
    Transform m_Temp;
    bool m_IsColliding = false;

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
        m_IsColliding = false;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetToLastCheckpoint();
        }

        if (transform.localPosition.y < ResetDepth)
        {
            Debug.Log("Depth reset");
            SetToLastCheckpoint();
        }
	}

    void SetToLastCheckpoint()
    {
        //Reset player velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.position = m_CheckPoints[m_LastPassed].position;
    }

    void OnTriggerEnter(Collider col)
    {
        if (m_IsColliding)
        {
            return;
        }
        m_IsColliding = true;

        //If collision with checkpoint
        if (col.gameObject.tag == "Checkpoint")
        {
            for (int i = 0; i < m_CheckPoints.Length; i++)
            {
                if (col.gameObject == m_CheckPoints[i].gameObject)
                {
                    m_LastPassed = i;
                }
            }
            if (m_LastPassed + 1 == m_CheckPoints.Length)
            {
                Debug.Log("Reached last checkpoint");
            }
        }

        //If collision with killbox
        if (col.gameObject.tag == "KillBox")
        {
            Debug.Log("Killbox reset");
            SetToLastCheckpoint();
        }
    }
}