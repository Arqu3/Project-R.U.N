using UnityEngine;
using System.Collections;

public class Hands : MonoBehaviour {

    RaycastHit hit;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
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
