using UnityEngine;
using System.Collections;

public class Front : MonoBehaviour
{
    ControllerPlayer m_Player;
    bool m_HasSentMsg = false;

	void Start ()
    {
        m_Player = GetComponentInParent<ControllerPlayer>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>() && m_Player.GetState() != MovementState.Falling)
        {
            if (!m_HasSentMsg)
            {
                m_Player.SetVerticalClimb(true);
                Debug.Log("Climb");
                m_HasSentMsg = true;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>())
        {
            m_Player.SetVerticalClimb(false);
            m_HasSentMsg = false;
        }
    }
}
