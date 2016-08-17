using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour
{
    public float bounce = 1.1f;

    Rigidbody m_Rigidbody;

	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
	}

    void OnCollisionEnter(Collision col)
    {
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, col.relativeVelocity.y / bounce, m_Rigidbody.velocity.z);
    }
}
