using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;
    AnimationHandler m_AnimHandler;

    public bool m_CanClimb;

    bool m_HasSentMsg;

    void Start ()
    {
        m_HasSentMsg = false;

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
                if (!m_HasSentMsg)
                {
                    m_CPlayer.SendMessage("HandClimb");
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

                m_CanClimb = true;
                m_CPlayer.IsGrabbed(true);
                Debug.Log("Grabbed");
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
                Debug.Log("Released");
            }
        }
    }
}
