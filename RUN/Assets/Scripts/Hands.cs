using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;
    AnimationHandler m_AnimHandler;

    public bool m_CanClimb;

    bool m_HasSentMsg = false;

    void Start ()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_CPlayer = GetComponentInParent<ControllerPlayer>();
        m_AnimHandler = GetComponentInParent<AnimationHandler>();
	}
	
	void Update ()
    { 
        if (m_CanClimb)
        {
            //Climb if pressing space
            if (Input.GetKey(KeyCode.Space))
            {
                m_Rigidbody.useGravity = true;

                if (!m_HasSentMsg)
                {
                    m_CPlayer.SendMessage("FastClimb");
                    m_HasSentMsg = true;
                }

                m_CPlayer.SendMessage("IsGrabbed", false);
            }

        }
        else
        {
            m_HasSentMsg = false;
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

                m_CPlayer.IsGrabbed(false);
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
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
                m_CPlayer.IsGrabbed(false);
            }
        }
    }
}
