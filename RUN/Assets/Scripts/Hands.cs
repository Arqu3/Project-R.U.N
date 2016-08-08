using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;
    AnimationHandler m_AnimHandler;
    Vector3 m_PlayerVel;

    public bool m_CanClimb;

    bool m_HasSentMsg;

    void Start()
    {
        m_HasSentMsg = false;

        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_CPlayer = GetComponentInParent<ControllerPlayer>();
        m_AnimHandler = GetComponentInParent<AnimationHandler>();
	}
	
	void Update()
    { 
        if (m_CanClimb)
        {
            //Climb if pressing space
            if (Input.GetButton("Jump"))
            {
                m_CPlayer.Climb();
                if (!m_HasSentMsg)
                {
                    m_AnimHandler.PlayAnimation("Climb");
                    m_HasSentMsg = true;
                }
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
                m_PlayerVel = m_Rigidbody.velocity;
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

    public void SetVelocity()
    {
        m_PlayerVel.y = 0;
        m_Rigidbody.velocity = transform.forward * m_PlayerVel.magnitude;
        //Add a constant force if previous velocity was too low
        if (m_PlayerVel.magnitude < 3.0f)
        {
            m_Rigidbody.AddForce(new Vector3(transform.forward.x, 0.0f, transform.forward.z) * 100.0f, ForceMode.Impulse);
        }
        Debug.Log("Velocity set");
    }
}
