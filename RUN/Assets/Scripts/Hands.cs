using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    RaycastHit hit;
    Collider m_Collider;

    public bool m_CanClimb;

    void Start ()
    {
        m_Collider = GetComponent<BoxCollider>();
	}
	
	void Update ()
    { /*
        Debug.DrawRay(transform.position + transform.right * 0.4f, transform.TransformDirection(Vector3.forward) * 0.5f);

        Debug.DrawRay(transform.position - transform.right * 0.4f, transform.TransformDirection(Vector3.forward) * 0.5f);

        if (Physics.Raycast(transform.position + transform.right * 0.4f, transform.TransformDirection(Vector3.forward), out hit))
        {
            if (hit.distance < 0.5f)
            {
                hit.collider.SendMessage("HitByRaycast", SendMessageOptions.DontRequireReceiver);
            }
        }

        else if (Physics.Raycast(transform.position - transform.right * 0.4f, transform.TransformDirection(Vector3.forward), out hit))
        {
            if (hit.distance < 0.5f)
            {
                hit.collider.SendMessage("HitByRaycast", SendMessageOptions.DontRequireReceiver);
            }
        }
       */
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<ParkourObject>())
        {
            if (col.bounds.max.y < m_Collider.bounds.max.y && col.GetComponent<ParkourObject>().m_Climbable)
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
