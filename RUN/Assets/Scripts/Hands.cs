using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Collider m_Collider;
    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;

    public bool m_CanClimb;

    float m_LedgeTimer;

    void Start ()
    {
        m_Collider = GetComponent<BoxCollider>();

        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_CPlayer = GetComponentInParent<ControllerPlayer>();
        m_LedgeTimer = 0.0f;
	}
	
	void Update ()
    { 
        if (m_CanClimb)
        {
            //Climb if pressing space
            if (Input.GetKey(KeyCode.Space))
            {
                m_Rigidbody.useGravity = true;
                if (m_LedgeTimer < 0.5)
                {
                    m_CPlayer.SendMessage("FastClimb");
                }
                else
                {
                    m_CPlayer.SendMessage("SlowClimb");
                }

                m_CPlayer.SendMessage("IsGrabbed", false);
            }

            m_LedgeTimer += Time.deltaTime;
        }
        else
        {
            m_LedgeTimer = 0.0f;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
            {
                m_Rigidbody.useGravity = false;
                m_Rigidbody.velocity = Vector3.zero;

                m_CPlayer.SendMessage("IsGrabbed", true);
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (/*col.bounds.max.y < m_Collider.bounds.max.y &&*/ col.GetComponent<ParkourObject>().m_Climbable)
            {
                m_CanClimb = true;
            }
            else
            {
                m_CanClimb = false;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
            {
                m_CanClimb = false;

                m_Rigidbody.useGravity = true;
                m_CPlayer.SendMessage("IsGrabbed", false);
            }
        }
    }
}
