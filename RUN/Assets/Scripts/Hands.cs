using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    RaycastHit m_Hit;
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
    { /*
        Debug.DrawRay(transform.position + transform.right * 0.4f, transform.TransformDirection(Vector3.forward) * 0.5f);

        Debug.DrawRay(transform.position - transform.right * 0.4f, transform.TransformDirection(Vector3.forward) * 0.5f);

        if (Physics.Raycast(transform.position + transform.right * 0.4f, transform.TransformDirection(Vector3.forward), out m_Hit))
        {
            if (m_Hit.distance < 0.5f)
            {
                m_Hit.collider.SendMessage("HitByRaycast", SendMessageOptions.DontRequireReceiver);
            }
        }

        else if (Physics.Raycast(transform.position - transform.right * 0.4f, transform.TransformDirection(Vector3.forward), out m_Hit))
        {
            if (m_Hit.distance < 0.5f)
            {
                m_Hit.collider.SendMessage("HitByRaycast", SendMessageOptions.DontRequireReceiver);
            }
        }
       */

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

                m_CPlayer.SendMessage("IsGrabbed");
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
                m_CPlayer.SendMessage("IsntGrabbed");
            }
        }
    }

    /*bool RaycastDir(Vector3 direction)
    {
        Vector3 v1 = transform.position + transform.right * 0.4f;
        Vector3 v2 = transform.position - transform.right * 0.4f;

        Ray ray1 = new Ray(v1, direction);
        Ray ray2 = new Ray(v2, direction);

        if (Physics.Raycast(ray1, 0.5f) || Physics.Raycast(ray2, 0.5f))
        {
            return true;
        }

        return false;
    }*/
}
