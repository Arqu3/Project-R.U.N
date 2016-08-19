using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    Rigidbody m_Rigidbody;
    ControllerPlayer m_CPlayer;
    Vector3 m_PlayerVel;
    Vector3 m_LedgeForward;
    bool m_HasSetDir = false;
    public bool m_CanClimb;

    void Start()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_CPlayer = GetComponentInParent<ControllerPlayer>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.GetComponent<ParkourObject>().m_Climbable)
            {
                if (!m_HasSetDir)
                {
                    if (col.gameObject.name == "LedgeX" || col.gameObject.name == "LedgeZ")
                    {
                        m_LedgeForward = col.transform.parent.position - col.bounds.center;

                        Debug.Log("mult");
                    }
                    else
                    {
                        m_LedgeForward = col.transform.parent.InverseTransformDirection(col.transform.forward);
                        if (Dot(transform.forward, m_LedgeForward) < 0)
                        {
                            m_LedgeForward = -m_LedgeForward;
                            Debug.Log("Behind");
                        }
                        else
                        {
                            Debug.Log("Front");
                        }

                        Debug.Log("one");
                    }
                }
                m_LedgeForward.y = 0;
                m_PlayerVel = m_Rigidbody.velocity;
                m_Rigidbody.useGravity = false;
                m_Rigidbody.velocity = Vector3.zero;
                m_CanClimb = true;
                m_HasSetDir = true;
                m_CPlayer.IsGrabbed(true);
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
                m_HasSetDir = false;
                m_Rigidbody.useGravity = true;
                m_CPlayer.IsGrabbed(false);
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
    }

    public Vector3 GetLedgeForward()
    {
        return m_LedgeForward;
    }

    float Dot(Vector3 lhs, Vector3 rhs)
    {
        return Vector3.Dot(lhs, rhs);
    }
}
