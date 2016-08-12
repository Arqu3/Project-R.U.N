using UnityEngine;
using System.Collections;

public class Front : MonoBehaviour
{
    public float m_GraceTime = 0.5f;
    float m_GraceTimer = 0.0f;

    ControllerPlayer m_Player;
    bool m_HasSentMsg = false;

	void Start ()
    {
        m_Player = GetComponentInParent<ControllerPlayer>();
	}

    void Update()
    {
        if (m_Player.GetState() == MovementState.Falling)
            m_GraceTimer += Time.deltaTime;
        else
            m_GraceTimer = 0.0f;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ParkourObject>() && m_GraceTimer < m_GraceTime)
        {
            if (col.gameObject.GetComponent<ParkourObject>().m_VerticalClimbable)
            {
                if (!m_HasSentMsg)
                {
                    m_Player.SetVerticalClimb(true);
                    Debug.Log("Climb");
                    m_HasSentMsg = true;
                }
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
