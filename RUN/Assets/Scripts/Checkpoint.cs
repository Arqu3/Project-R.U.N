using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public int m_ID = 0;
    static Transform m_PlayerTransform;

	void Start ()
    {
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update ()
    {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            //If this is the current checkpoint
            if (transform == m_PlayerTransform.GetComponent<PlayerCheckpoint>().m_CheckPoints[PlayerCheckpoint.m_Current].transform)
            {
                //If the current checkpoint + 1 is not the last one
                if (PlayerCheckpoint.m_Current + 1 < m_PlayerTransform.GetComponent<PlayerCheckpoint>().m_CheckPoints.Length)
                {
                    //Increase lap *NOT NEEDED*
                    /*if (PlayerCheckpoint.m_Current == 0)
                    {
                        PlayerCheckpoint.m_Lap++;
                    }*/

                    PlayerCheckpoint.m_Current++;
                    Debug.Log("Next checkpoint: " + PlayerCheckpoint.m_Current);
                }
                else
                {
                    //We've reached the last checkpoint
                    m_PlayerTransform.SendMessage("SetReachedLast", true);
                    Debug.Log("Reached last checkpoint");
                }
            }
        }
    }
}
