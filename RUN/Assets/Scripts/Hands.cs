using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;
    AnimationHandler m_AnimHandler;

    public bool m_CanClimb;

    float m_LedgeTimer;
    bool m_HasSentMsg = false;

    void Start ()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_CPlayer = GetComponentInParent<ControllerPlayer>();
        m_AnimHandler = GetComponentInParent<AnimationHandler>();
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
                }

                if (!m_HasSentMsg)
                {
                    m_CPlayer.SendMessage("FastClimb");
                    m_HasSentMsg = true;
                    //m_AnimHandler.SendMessage("IsGrabbed", true);
                }

                m_CPlayer.SendMessage("IsGrabbed", false);
            }

            m_LedgeTimer += Time.deltaTime;
        }
        else
        {
            m_LedgeTimer = 0.0f;
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

                m_CPlayer.SendMessage("IsGrabbed", true);
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
                m_CPlayer.SendMessage("IsGrabbed", false);
                //m_AnimHandler.SendMessage("IsGrabbed", false);
            }
        }
    }
}
